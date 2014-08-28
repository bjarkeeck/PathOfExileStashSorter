using POEStashSorterModels;
using System;
using System.Collections.Generic;
using System.Linq;

public class SortJewlery : SortingAlgorithm
{
    public override string Name
    {
        get { return "Sort Jewlery"; }
    }

    protected override void Sort(Tab tab, SortOption options)
    {
        if (options["Default"])
        {
            sortJewls(tab, 11);
        }
        if (options["Dynamic"])
        {
            int numberOfColumns = (int)Math.Ceiling(tab.Items.Count() / 12f);
            sortJewls(tab, numberOfColumns);
        }
    }

    void sortJewls(Tab tab, int numberOfColumns)
    {
        if (tab.Items.Count() > 0)
        {
            int numberOfItems = tab.Items.Count();
            int x = 11;
            int y = 11;

            var q = tab.Items.OrderByDescending(xx => xx.Icon);
            int i = 0;
            string lastIcon = q.FirstOrDefault().Icon;
            bool maySkip = true;
            foreach (var item in q)
            {
                bool canSkip = (lastIcon != item.Icon && (numberOfItems - i) < numberOfColumns * y);
                if (maySkip && lastIcon != item.Icon && ((numberOfItems - i) < numberOfColumns * y) == false)
                    maySkip = false;

                if (x == 11 - numberOfColumns || canSkip && maySkip)
                {
                    x = 11;
                    y--;
                }

                item.X = x;
                item.Y = y;
                i++;
                x--;

                lastIcon = item.Icon;
            }

            //Move all maps up!
            int minY = tab.Items.Min(xx => xx.Y);
            tab.Items.ForEach(c => c.Y -= minY);
        }
    }
}
