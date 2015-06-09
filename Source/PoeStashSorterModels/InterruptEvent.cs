using System;

namespace PoeStashSorterModels
{
   public class InterruptEvent
    {
        public Func<bool> Isinterrupted = () => false;
    }
}