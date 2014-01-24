using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace POEStashSorter
{
    public class Log
    {
        public static List<string> Messages = new List<string>();

        public static void Message(string msg)
        {
            Messages.Add(msg);
        }

    }
}
