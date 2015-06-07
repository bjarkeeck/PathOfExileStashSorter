using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Image = System.Drawing.Image;

namespace POEStashSorterModels
{
    public abstract class SortingAlgorithm
    {
        private static bool isSorting = false;
        private static Point startPos;
        private static float cellWidth;
        private static float cellHeight;
        private static bool interrupt;

        public SortOption SortOption { get; set; }

        public SortingAlgorithm()
        {
            SortOption = new SortOption();
            SortOption.ReadMode = true;
            Sort(new Tab() { Items = new List<Item>() }, SortOption);
            SortOption.ReadMode = false;
            SortOption.SelectedOption = SortOption.Options.FirstOrDefault();
        }

        public Tab SortTab(Tab tab)
        {
            Tab sortedTab = new Tab();
            sortedTab.Index = tab.Index;
            sortedTab.League = tab.League;
            sortedTab.Name = tab.Name;
            sortedTab.srcC = tab.srcC;
            sortedTab.srcL = tab.srcL;
            sortedTab.srcR = tab.srcR;
            sortedTab.Items = tab.Items.Select(item => item.CloneItem()).ToList();


            Sort(sortedTab, SortOption);

            foreach (var item in sortedTab.Items)
            {
                double offsetX = 47.4 * 13;
                item.Image.Margin = new Thickness(item.X * 47.4f + 2.2f + offsetX, item.Y * 47.4f + 2.2f, 0, 0);
            }

            return sortedTab;
        }

        public abstract string Name { get; }

        protected abstract void Sort(Tab tab, SortOption options);

        public static SortingAlgorithm CreateFromType(Type type)
        {
            return (SortingAlgorithm)Activator.CreateInstance(type);
        }

        private void GetStashDimentions()
        {
            const int CELL_COUNT_X = 12;
            RECT rect = ApplicationHelper.PathOfExileDimentions;
            //int width = rect.Right;
            //int height = rect.Bottom;
            var stashRectangle = GetStashRectangle(rect);

            cellHeight = (float)stashRectangle.Width / CELL_COUNT_X; // height * 0.0484f;
            cellWidth = cellHeight;

            float startX = stashRectangle.Left + cellHeight / 2.0f;// height * 0.033f;
            float startY = stashRectangle.Top + cellHeight / 2.0f;//height * 0.1783f;

            startPos = new Point(rect.Left + (int)startX, rect.Top + (int)startY);

        }

        private struct PointColor
        {
            public int Hue;
            public int X, Y;

            public PointColor(int x, int y, int hue)
            {
                X = x;
                Y = y;
                Hue = hue;
            }
        }

        private static Rectangle GetStashRectangle(RECT rect)
        {
            using (Bitmap img = new Bitmap(rect.Right, rect.Bottom))
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    g.CopyFromScreen(rect.Left, rect.Top, 0, 0, img.Size, CopyPixelOperation.SourceCopy);
                }
                //img.Save("c:\\444.png", ImageFormat.Png);

                var points = new List<PointColor>();
                for (int y = (int)(img.Height * 0.1); y < img.Height * 0.8; y++)
                {
                    for (int x = 0; x < img.Width / 2; x++)
                    {
                        var color = img.GetPixel(x, y);
                        float hue = color.GetHue();
                        if (hue < 30 && color.GetBrightness() > 0.10)
                        {
                            points.Add(new PointColor(x, y, (int)hue));
                        }
                    }
                }

                Func<PointColor[], Func<int, bool>, bool> IsLineBody = (ordered, isChain) =>
                {
                    const int ACCURACY = 300;
                    int maxChain = 0;
                    int curChain = 0;
                    for (int i = 0; i < ordered.Length - 1; i++)
                    {
                        if (isChain(i) && Math.Abs(ordered[i].Hue - ordered[i + 1].Hue) < 10)
                            curChain++;
                        else
                        {
                            if (curChain > maxChain)
                                maxChain = curChain;
                            curChain = 0;
                        }
                    }
                    return Math.Max(curChain, maxChain) > ACCURACY;
                };


                Func<IGrouping<int, PointColor>, bool> IsLineY = group =>
                {
                    var orderedByY = group.OrderBy(y => y.Y).ToArray();
                    return IsLineBody(orderedByY, i => orderedByY[i].Y == orderedByY[i + 1].Y - 1);

                };

                Func<IGrouping<int, PointColor>, bool> IsLineX = group =>
                {
                    var orderedByX = group.OrderBy(y => y.X).ToArray();
                    return IsLineBody(orderedByX, i => orderedByX[i].X == orderedByX[i + 1].X - 1);
                };

                Func<List<int>, List<List<int>>> Grouping = list =>
                {
                    var collection = new List<List<int>>();
                    var temp = new List<int>();
                    if (list.Count > 0)
                        temp.Add(list[0]);
                    for (int i = 1; i < list.Count; i++)
                    {
                        if (list[i - 1] + 1 == list[i])
                        {
                            temp.Add(list[i]);
                        }
                        else
                        {
                            collection.Add(temp);
                            temp = new List<int>();
                            temp.Add(list[i]);
                        }
                    }
                    collection.Add(temp);
                    return collection;
                };

                var xBorders =
                    Grouping(
                        points.GroupBy(y => y.X)
                            .Where(y => IsLineY(y))
                            .Select(y => y.Key)
                            .Where(y => y != 0)
                            .OrderBy(y => y)
                            .ToList());
                var yBorders =
                    Grouping(points.GroupBy(y => y.Y).Where(y => IsLineX(y)).Select(y => y.Key).OrderBy(y => y).ToList());


                if (xBorders.Count < 2 || yBorders.Count < 2)
                    throw new Exception("Stash hasn't found");

                var xx = xBorders[0].Max();
                var yy = yBorders[0].Max();
                return new Rectangle(xx, yy, xBorders[1].Min() - xx, yBorders[1].Min() - yy);
            }
        }

        public void StartSorting(Tab unsortedTab, Tab sortedTab)
        {
            try
            {
                ApplicationHelper.OpenPathOfExile();
                List<Item> unsortedItems = unsortedTab.Items.Where(x => sortedTab.Items.Any(c => c.Id == x.Id && c.X == x.X && x.Y == c.Y) == false).ToList();
                if (isSorting == false)
                {
                    GetStashDimentions();
                    isSorting = true;

                    Item unsortedItem = unsortedItems.FirstOrDefault();

                    if (unsortedItem != null)
                    {
                        MouseTools.MoveCursor(MouseTools.GetMousePosition(), new Vector2(startPos.X + unsortedItem.X * cellWidth, startPos.Y + unsortedItem.Y * cellHeight), 20);
                        bool selectGem = true;

                        while (unsortedItem != null)
                        {
                            if (interrupt == true)
                            {
                                interrupt = false;
                                break;
                            }
                            Item sortedItem = sortedTab.Items.FirstOrDefault(x => x.Id == unsortedItem.Id);
                            Vector2 unsortedPos = new Vector2(startPos.X + unsortedItem.X * cellWidth, startPos.Y + unsortedItem.Y * cellHeight);

                            if (selectGem)
                            {
                                //Move to item
                                MouseTools.MoveCursor(MouseTools.GetMousePosition(), unsortedPos, 10);
                                //select item
                                MouseTools.MouseClickEvent();
                                //wait a little (internet delay)
                                Thread.Sleep((int)(80f / Settings.Instance.Speed));
                            }

                            Vector2 sortedPos = new Vector2(startPos.X + sortedItem.X * cellWidth, startPos.Y + sortedItem.Y * cellHeight);
                            //Log.Message("Moving " + unsortedItem.Name + " from " + unsortedItem.X + "," + unsortedItem.Y + " to " + sortedItem.X + "," + sortedItem.Y);

                            //move to correct position
                            MouseTools.MoveCursor(MouseTools.GetMousePosition(), sortedPos, 10);
                            //place item
                            MouseTools.MouseClickEvent();
                            //wait a little (internet delay)
                            Thread.Sleep((int)(80f / Settings.Instance.Speed));

                            Item newGem = unsortedItems.FirstOrDefault(x => x.X == sortedItem.X && x.Y == sortedItem.Y);

                            //remove unsorted now that it is sorted
                            unsortedItems.Remove(unsortedItem);

                            //if there wassent a item where the item was placed
                            if (newGem == null)
                            {
                                //selected a new to sort
                                unsortedItem = unsortedItems.FirstOrDefault();
                                selectGem = true;
                            }
                            else
                            {
                                unsortedItem = newGem;
                                selectGem = false;
                            }

                        }
                    }
                    //Log.Message("Sorting Complete");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                isSorting = false;
            }


        }

    }
}
