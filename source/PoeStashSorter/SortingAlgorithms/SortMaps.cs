using POEStashSorterModels;
using System;
using System.Collections.Generic;
using System.Linq;

public class SortMaps : SortingAlgorithm
{
    public override string Name
    {
        get { return "Sort Maps"; }
    }
    protected override void Sort(Tab tab, SortOption options)
    {
        int numberOfColumns = (int)Math.Ceiling(tab.Items.Count() / 12f);

        if (options["Default"])
        {
            numberOfColumns = 11;
            sortMaps(tab, numberOfColumns);
        }
        if (options["Full"])
        {
            numberOfColumns = 12;
            sortMaps(tab, numberOfColumns);
        }
        if (options["1 Margin"])
        {
            if (numberOfColumns <= 11)
                numberOfColumns = 11;
            sortMaps(tab, numberOfColumns);
        }
        if (options["2 Margin"])
        {
            if (numberOfColumns <= 10)
                numberOfColumns = 10;
            sortMaps(tab, numberOfColumns);
        }
        if (options["3 Margin"])
        {
            if (numberOfColumns <= 9)
                numberOfColumns = 9;
            sortMaps(tab, numberOfColumns);
        }
        if (options["Dynamic"])
        {
            numberOfColumns = (int)Math.Ceiling(tab.Items.Count() / 12f);
            sortMaps(tab, numberOfColumns);
        }
        if (options["Dynamic + 1"])
        {
            numberOfColumns = (int)Math.Ceiling(tab.Items.Count() / 12f) + 1;
            numberOfColumns = Mathf.Clamp(numberOfColumns, 0, 11);
            sortMaps(tab, numberOfColumns);
        }
        if (options["Dynamic + 2"])
        {
            numberOfColumns = (int)Math.Ceiling(tab.Items.Count() / 12f) + 2;
            numberOfColumns = Mathf.Clamp(numberOfColumns, 0, 11);
            sortMaps(tab, numberOfColumns);
        }

    }

    void sortMaps(Tab tab, int numberOfColumns)
    {
        if (tab.Items.Count() > 0)
        {
            int numberOfItems = tab.Items.Count();
            int x = 11;
            int y = 11;

            var q = tab.Items.OrderByDescending(xx => xx.Level).ThenBy(xx => xx.Icon).ThenBy(xx => xx.Quality);
            int i = 0;
            int lastLevel = q.FirstOrDefault().Level;
            bool maySkip = true;
            foreach (var item in q)
            {
                bool canSkip = (lastLevel != item.Level && (numberOfItems - i) < numberOfColumns * y);
                if (maySkip && lastLevel != item.Level && ((numberOfItems - i) < numberOfColumns * y) == false)
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

                lastLevel = item.Level;
            }

            //Move all maps up!
            int minY = tab.Items.Min(xx => xx.Y);
            tab.Items.ForEach(c => c.Y -= minY);
        }
    }
}
