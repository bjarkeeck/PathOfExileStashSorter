using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            RECT rect = ApplicationHelper.PathOfExileDimentions;
            int width = rect.Right - rect.Left;
            int height = rect.Bottom - rect.Top;

            float startX = height * 0.033f;
            float startY = height * 0.1783f;
            cellHeight = height * 0.0484f;
            cellWidth = cellHeight;
            startPos = new Point(rect.Left + (int)startX, rect.Top + (int)startY);

        }

        public void StartSorting(Tab unsortedTab, Tab sortedTab)
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
            isSorting = false;

        }
    }
}
