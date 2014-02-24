using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace POEStashSorter
{

    public class PoeSorter
    {
        Point startPos;
        float cellWidth;
        float cellHeight;
        StashTab currentStash;
        List<StashItem> UnSortedItems = new List<StashItem>();
        List<StashItem> SortedItems = new List<StashItem>();
        private float speed;

        public void SortStash(StashTab stash, SortBy sort, float speed)
        {
            SortedItems.Clear();
            UnSortedItems.Clear();
            this.speed = speed;
            if (stash.IsSorted)
                stash.DownloadStashItems();

            this.currentStash = stash;
            UnSortedItems = stash.StashItems.ToList();

            if (ApplicationHelper.OpenPathOfExile())
            {
                Log.Message("Sort started");
                GetStashDimentions();

                switch (sort)
                {
                    case SortBy.GemType:
                        IEnumerable<StashItem> s1 = UnSortedItems.Where(c => c.GemType == GemType.Normal || c.GemType == GemType.Curse);
                        IEnumerable<StashItem> s2 = UnSortedItems.Where(c => c.GemType == GemType.Support);
                        IEnumerable<StashItem> s3 = UnSortedItems.Where(c => c.GemType == GemType.Aura);
                        SortGems(s1, s2, s3);
                        break;
                    case SortBy.GemColor:
                        IEnumerable<StashItem> x1 = UnSortedItems.Where(c => c.GemColor == GemColor.Red);
                        IEnumerable<StashItem> x2 = UnSortedItems.Where(c => c.GemColor == GemColor.Green);
                        IEnumerable<StashItem> x3 = UnSortedItems.Where(c => c.GemColor == GemColor.Blue);
                        SortGems(x1, x2, x3);
                        break;
                    case SortBy.MapLevel:
                        SortMaps();
                        break;
                    case SortBy.Image:
                        SimpleSort(UnSortedItems.OrderBy(x => x.Image));
                        break;
                    case SortBy.GemQuality:
                        SortGemsByQuality();
                        break;
                    default:
                        break;
                }

                StartSorting();
                Log.Message("Sort finnished");
                stash.IsSorted = true;
            }
            else
            {
                Log.Message("Path of exile is not open");
            }
        }


        private void SortGemsByQuality()
        {
            var q = UnSortedItems.Where(c => c.Quiality > 0).OrderByDescending(c => c.Quiality).ThenBy(c => c.Name);
            if (q == null || q.Count() == 0)
            {
                Log.Message("");
                Log.Message("No quality gems found in stash");
            }
            else
            {
                int x = 0;
                int y = 0;
                foreach (var item in q)
                {
                    AddSortedItem(x, y, item);
                    x++;
                    if (x == 12)
                    {
                        y += 1;
                        x = 0;
                    }
                }

                x = 11;
                y = 11;
                foreach (var item in sort(UnSortedItems.Where(c => c.Quiality == 0)).Reverse())
                {
                    AddSortedItem(x, y, item);
                    x--;
                    if (x == -1)
                    {
                        x = 11;
                        y -= 1;
                    }
                }
            }
        }
        private void SimpleSort(IEnumerable<StashItem> q)
        {
            int x = 0;
            int y = 0;

            foreach (var item in q)
            {
                AddSortedItem(x, y, item);
                x += 1;
                if (x == 12)
                {
                    y++;
                    x = 0;
                }
            }
        }

        private void SortMaps()
        {
            int x = 0;
            int y = 0;

            var q = UnSortedItems.OrderBy(c => c.MapLevel).ThenBy(c => c.Image).ThenBy(c => c.Quiality);
            int currentMapLevel = q.First().MapLevel;

            foreach (var item in q)
            {

                if (UnSortedItems.Count() > 110)
                {
                    AddSortedItem(x, y, item);
                }
                else
                {
                    if (item.MapLevel != currentMapLevel)
                    {
                        y += 1;
                        x = 0;
                    }
                    currentMapLevel = item.MapLevel;

                    AddSortedItem(x, y, item);
                }

                x += 1;
                if (x == 12)
                {
                    y++;
                    x = 0;
                }
            }
        }

        private void GetStashDimentions()
        {
            RECT rect = ApplicationHelper.PathOfExileDimentions;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            float startX = height * 0.033f;
            float startY = height * 0.1783f;
            cellHeight = height * 0.0484f;
            cellWidth = cellHeight;
            startPos = new Point(rect.Left + (int)startX, rect.Top + (int)startY);


            MouseTools.MoveCursor(MouseTools.GetMousePosition(), startPos);
            Thread.Sleep(500);
            MouseTools.MoveCursor(startPos, new Point(startPos.X + (int)(cellWidth * 11), startPos.Y + (int)(cellHeight * 11)));
            Thread.Sleep(500);
        }

        private IOrderedEnumerable<StashItem> sort(IEnumerable<StashItem> enumerable)
        {
            return enumerable
                .OrderByDescending(c => c.GemColor)
                .ThenByDescending(c => c.GemType)
                .ThenBy(c => c.Name)
                .ThenBy(c => c.Quiality);
        }
        private void AddSortedItem(int x, int y, StashItem item)
        {
            if (x == 12 || SortedItems.Any(c => c.X == x && c.Y == y))
            {
                Log.Message("Something terrible has happened");
            }
            StashItem stashItem = new StashItem()
            {
                GemColor = item.GemColor,
                GemType = item.GemType,
                Name = item.Name,
                MapLevel = item.MapLevel,
                Quiality = item.Quiality,
                X = x,
                Id = item.Id,
                Y = y
            };
            SortedItems.Add(stashItem);
        }
        private void SortGems(IEnumerable<StashItem> sortedNormal, IEnumerable<StashItem> sortedSupport, IEnumerable<StashItem> sortedAura)
        {
            int fromLeft = 0;
            int fromRight = 11;
            int yMatters = 0;

            int x = 0;
            int y = 0;
            foreach (var item in sort(sortedNormal))
            {
                AddSortedItem(x, y, item);
                y++;
                if (y == 12)
                {
                    x += 1;
                    y = 0;
                }
            }
            yMatters = y;
            fromLeft = x;


            x = 11;
            y = 0;
            foreach (var item in sort(sortedAura))
            {
                AddSortedItem(x, y, item);
                y++;
                if (y == 12)
                {
                    x -= 1;
                    y = 0;
                }
            }

            fromRight = x;

            int freeRows = fromRight - fromLeft;

            int rowsToTake = (int)Math.Ceiling(sortedSupport.Count() / 12f);

            if (rowsToTake >= freeRows)
            {
                x = fromLeft;
                y = yMatters;
            }
            else if (rowsToTake < freeRows)
            {
                x = fromLeft + (int)Math.Ceiling((freeRows - rowsToTake) / 2f);
                y = 0;
            }

            foreach (var item in sort(sortedSupport))
            {
                AddSortedItem(x, y, item);
                do
                {
                    y++;
                    if (y == 12)
                    {
                        x += 1;
                        y = 0;
                    }
                } while ((SortedItems.Any(c => c.X == x && c.Y == y)));
            }

        }

        public StashItem GetFirstUnsortedItem()
        {
            return UnSortedItems.FirstOrDefault(c =>
                SortedItems.First(v => v.Id == c.Id).X != c.X ||
                SortedItems.First(v => v.Id == c.Id).Y != c.Y);
        }

        bool isSorting = false;
        private void StartSorting()
        {
            if (isSorting == false)
            {
                isSorting = true;

                StashItem unsortedItems = GetFirstUnsortedItem();

                if (unsortedItems != null)
                {
                    MouseTools.MoveCursor(MouseTools.GetMousePosition(), new Point(startPos.X + unsortedItems.X * cellWidth, startPos.Y + unsortedItems.Y * cellHeight), 20);

                    bool selectGem = true;

                    int max = UnSortedItems.Count();
                    int i = 0;

                    while (unsortedItems != null)
                    {
                        StashItem sortedItem = SortedItems.FirstOrDefault(x => x.Id == unsortedItems.Id);

                        Point unsortedPos = new Point(startPos.X + unsortedItems.X * cellWidth, startPos.Y + unsortedItems.Y * cellHeight);

                        if (selectGem)
                        {
                            MouseTools.MoveCursor(MouseTools.GetMousePosition(), unsortedPos, (int)(5 * speed));
                            MouseTools.MouseClickEvent();
                            Thread.Sleep((int)(400 * speed));
                        }

                        Point sortedPos = new Point(startPos.X + sortedItem.X * cellWidth, startPos.Y + sortedItem.Y * cellHeight);

                        Log.Message("Moving " + unsortedItems.Name + " from " + unsortedItems.X + "," + unsortedItems.Y + " to " + sortedItem.X + "," + sortedItem.Y);

                        MouseTools.MoveCursor(MouseTools.GetMousePosition(), sortedPos, (int)(5 * speed));
                        MouseTools.MouseClickEvent();
                        Thread.Sleep((int)(400 * speed));

                        StashItem newGem = UnSortedItems.FirstOrDefault(x => x.X == sortedItem.X && x.Y == sortedItem.Y);

                        unsortedItems.X = sortedItem.X;
                        unsortedItems.Y = sortedItem.Y;

                        if (newGem == null)
                        {
                            unsortedItems = GetFirstUnsortedItem();
                            selectGem = true;
                        }
                        else
                        {
                            unsortedItems = newGem;
                            selectGem = false;
                        }

                        i++;
                        if (i > 15)
                        {

                        }

                    }
                }
                Log.Message("Sorting Complete");

            }
            isSorting = false;
        }

    }
}
