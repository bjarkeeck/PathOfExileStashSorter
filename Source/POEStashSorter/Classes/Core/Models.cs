using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEStashSorter
{
    public class Requirement
    {
        public string name { get; set; }
        public List<List<object>> values { get; set; }
        public int displayMode { get; set; }
    }

    public class Tab
    {
        public string n { get; set; }
        public int i { get; set; }
        public Colour colour { get; set; }
        public string src { get; set; }
    }

    public class Colour
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
    }
    public class Property
    {
        public string name { get; set; }
        public List<object> values { get; set; }
        public int displayMode { get; set; }
    }

    public class AdditionalProperty
    {
        public string name { get; set; }
        public List<List<object>> values { get; set; }
        public int displayMode { get; set; }
        public double progress { get; set; }
    }

    public class Item
    {
        public bool verified { get; set; }
        public int w { get; set; }
        public int h { get; set; }
        public string icon { get; set; }
        public bool support { get; set; }
        public string league { get; set; }
        public List<object> sockets { get; set; }
        public string name { get; set; }
        public string typeLine { get; set; }
        public bool identified { get; set; }
        public string secDescrText { get; set; }
        public List<Requirement> requirements { get; set; }
        public List<string> implicitMods { get; set; }
        public List<string> explicitMods { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        public List<object> socketedItems { get; set; }
        public List<Property> properties { get; set; }
        public string descrText { get; set; }
        public List<string> flavourText { get; set; }
        public List<AdditionalProperty> additionalProperties { get; set; }
    }

    public class RootObject
    {
        public int numTabs { get; set; }
        public List<Item> items { get; set; }
        public List<Tab> tabs { get; set; }

    }


    public class StashItem
    {
        public int X;
        public int Y;
        public string Name;
        public GemType GemType;
        public GemColor GemColor;
        public int Quiality;
        public int MapLevel = 0;
        public int Id;
    }


    public enum GemType
    {
        Support,
        Aura,
        Curse,
        Normal
    }

    public enum League
    {
        Domination,
        Nemisis,
        Standard,
        Hardcore
    }

    public enum GemColor
    {
        Red,
        Blue,
        Green
    }

    public enum SortBy
    {
        GemType,
        GemColor,
        GemQuality,
        MapLevel
    }

}
