using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POEStashSorterModels
{
    public class SortOption
    {
        public List<string> Options = new List<string>();
        public string SelectedOption = "Default";
        internal bool ReadMode = false;
        public bool this[string key]
        {
            get
            {
                if (ReadMode)
                {
                    Options.Add(key);
                    return false;
                }
                else
                {
                    return SelectedOption == key;
                }
            }
        }
    }
}
