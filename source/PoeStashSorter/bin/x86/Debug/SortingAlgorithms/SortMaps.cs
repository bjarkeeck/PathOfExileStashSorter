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
        int numberOfItems = tab.Items.Count();

        if (options["Default"])
        {
            numberOfColumns = 12;
        }
        if (options["1 Margin"])
        {
            if (numberOfColumns <= 11)
                numberOfColumns = 11;
        }
        if (options["2 Margin"])
        {
            if (numberOfColumns <= 10)
                numberOfColumns = 10;
        }
        if (options["Dynamic"])
        {
            numberOfColumns = (int)Math.Ceiling(tab.Items.Count() / 12f);
        }

        int x = 11;
        int y = 11;

        int i = 0;
        int lastLevel = 0;
        bool maySkip = true;
        foreach (var item in tab.Items.OrderByDescending(xx => xx.Level).ThenBy(xx => xx.Icon).ThenBy(xx => xx.Quality))
        {
            item.X = x;
            item.Y = y;
            i++;

            x--;
            bool canSkip = (lastLevel != item.Level && (numberOfItems - i) < numberOfColumns * y);
            if (maySkip && lastLevel != item.Level && ((numberOfItems - i) < numberOfColumns * y) == false)
                maySkip = false;

            if (x == 11 - numberOfColumns || canSkip && maySkip)
            {
                x = 11;
                y--;
            }
            lastLevel = item.Level;
        }
    }
}
