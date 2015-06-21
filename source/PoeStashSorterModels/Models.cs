using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace POEStashSorterModels
{
    [DataContract]
    public class Property
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<List<object>> Values { get; set; }

        [JsonProperty(PropertyName = "displayMode")]
        public int DisplayMode { get; set; }
    }

    public class League
    {
        public League(string name)
        {
            Name = name;
            Tabs = PoeConnector.FetchTabs(this);
            var t = Tabs.FirstOrDefault();
            if (t != null && t.Name.Trim() == "1")
                Tabs.Remove(t);
        }
        public string Name { get; set; }

        public List<Tab> AllTabs = new List<Tab>();
        private List<Tab> tabs;
        public List<Tab> Tabs
        {
            get { return tabs; }
            set { tabs = value; }
        }

    }

    [DataContract]
    public class AdditionalProperty
    {
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<List<object>> values { get; set; }

        [JsonProperty(PropertyName = "displayMode")]
        public int displayMode { get; set; }

        [JsonProperty(PropertyName = "progress")]
        public double progress { get; set; }
    }

    [DataContract]
    public class Requirement
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "values")]
        public List<List<object>> Value { get; set; }

        [JsonProperty(PropertyName = "displayMode")]
        public int DisplayMode { get; set; }
    }

    public enum ItemType
    {
        Gem,
        Gear,
        Map,
        Ring,
        Currency,
        Amulet
    }

    [DataContract]
    public class Item
    {
        internal bool Sorted = false;
        public static int SimpleIdIncrementer = 0;
        internal int Id { get; private set; }

        public Item()
        {
            Id = SimpleIdIncrementer;
            SimpleIdIncrementer++;
        }

        public string FullItemName
        {
            get
            {
                return (Name + " " + TypeLine).Trim();
            }
        }

        public GemRequirement GemRequirement
        {
            get
            {
                if (Requirements != null)
                {
                    Requirement topRequirement = Requirements
                                    .Where(x => x.Name.ToLower() == "dex" || x.Name.ToLower() == "str" || x.Name.ToLower() == "int")
                                    .OrderByDescending(x => x.Value.Select(c => Convert.ToInt32(c.Max(v => Convert.ToInt32(v)))).Max())
                                    .FirstOrDefault();

                    if (topRequirement != null)
                    {
                        if (topRequirement.Name.ToLower() == "dex")
                            return POEStashSorterModels.GemRequirement.Dex;
                        if (topRequirement.Name.ToLower() == "int")
                            return POEStashSorterModels.GemRequirement.Int;
                        if (topRequirement.Name.ToLower() == "str")
                            return POEStashSorterModels.GemRequirement.Str;
                    }
                    else
                    {
                        if (Settings.Instance.GemColorInfo.ContainsKey(FullItemName))
                            return Settings.Instance.GemColorInfo[FullItemName];
                    }
                }
                else
                {
                    if (Settings.Instance.GemColorInfo.ContainsKey(FullItemName))
                        return Settings.Instance.GemColorInfo[FullItemName];
                }

                return POEStashSorterModels.GemRequirement.None;
            }
        }

        public GemType GemType
        {
            get
            {

                var t = GemType.Normal;

                if (this.Support)
                    t = GemType.Support;
                else if (this.SecDescrText != null && this.SecDescrText.ToLower().Contains("aura"))
                    t = GemType.Aura;

                return t;

            }
        }

        public ItemType ItemType
        {
            get
            {
                if (this.FrameType == 4)
                    return POEStashSorterModels.ItemType.Gem;

                //TODO determen the item type

                return ItemType.Gear;
            }
        }

        [JsonProperty(PropertyName = "verified")]
        public bool Verified { get; set; }

        [JsonProperty(PropertyName = "w")]
        public int W { get; set; }

        [JsonProperty(PropertyName = "h")]
        public int H { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; }

        [JsonProperty(PropertyName = "support")]
        public bool Support { get; set; }

        [JsonProperty(PropertyName = "league")]
        public string League { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "typeLine")]
        public string TypeLine { get; set; }

        [JsonProperty(PropertyName = "identified")]
        public bool Identified { get; set; }

        [JsonProperty(PropertyName = "properties")]
        public List<Property> Properties { get; set; }

        [JsonProperty(PropertyName = "explicitMods")]
        public List<string> ExplicitMods { get; set; }

        [JsonProperty(PropertyName = "descrText")]
        public string DescrText { get; set; }

        [JsonProperty(PropertyName = "frameType")]
        public int FrameType { get; set; }

        [JsonProperty(PropertyName = "x")]
        public int X { get; set; }

        [JsonProperty(PropertyName = "y")]
        public int Y { get; set; }

        [JsonProperty(PropertyName = "inventoryId")]
        public string InventoryId { get; set; }

        [JsonProperty(PropertyName = "socketedItems")]
        public List<Item> SocketedItems { get; set; }

        [JsonProperty(PropertyName = "sockets")]
        public List<Socket> Sockets { get; set; }

        [JsonProperty(PropertyName = "additionalProperties")]
        public List<AdditionalProperty> AdditionalProperties { get; set; }

        [JsonProperty(PropertyName = "secDescrText")]
        public string SecDescrText { get; set; }

        [JsonProperty(PropertyName = "implicitMods")]
        public List<string> ImplicitMods { get; set; }

        [JsonProperty(PropertyName = "flavourText")]
        public List<string> FlavourText { get; set; }

        [JsonProperty(PropertyName = "requirements")]
        public List<Requirement> Requirements { get; set; }

        [JsonProperty(PropertyName = "nextLevelRequirements")]
        public List<Requirement> nextLevelRequirements { get; set; }

        [JsonProperty(PropertyName = "socket")]
        public int Socket { get; set; }

        [JsonProperty(PropertyName = "colour")]
        public string Color { get; set; }

        [JsonProperty(PropertyName = "corrupted")]
        public bool Corrupted { get; set; }

        [JsonProperty(PropertyName = "cosmeticMods")]
        public List<string> CosmeticMods { get; set; }

        private static Dictionary<string, BitmapImage> downloadedImages = new Dictionary<string, BitmapImage>();

        private Image image;

        public Image Image
        {
            get
            {
                if (image == null)
                {
                    image = new Image();
                    image.Width = 43 * this.W;
                    image.Height = 43 * this.H;
                    image.Stretch = Stretch.None;
                    image.Margin = new Thickness(this.X * 47.4f + 2.2f, this.Y * 47.4f + 2.2f, 0, 0);
                    DownloadImageAsync();
                }
                return image;
            }
            set
            {
                image = value;
                DownloadImageAsync();
            }
        }
        private bool isDownloading = false;

        private void DownloadImageAsync()
        {
            if (downloadedImages.ContainsKey(this.Icon))
            {
                this.Image.Source = downloadedImages[this.Icon];
                //if (Tab.IsSelected)
                //    PoeSorter.ItemCanvas.Children.Add(this.Image);
            }
            else
            {
                if (isDownloading == false)
                {
                    isDownloading = true;
                    new Thread(() =>
                    {
                        WebClient client = new WebClient();
                        client.BaseAddress = PoeConnector.server.Url;
                        byte[] imageData = client.DownloadData(this.Icon);
                        PoeSorter.Dispatcher.Invoke(() =>
                        {
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = new MemoryStream(imageData);
                            bitmap.EndInit();
                            if (downloadedImages.ContainsKey(this.Icon) == false)
                                downloadedImages.Add(this.Icon, bitmap);

                            this.image.Source = bitmap;

                            if (this.GemRequirement == POEStashSorterModels.GemRequirement.None && this.ItemType == POEStashSorterModels.ItemType.Gem)
                                ScanGemImage(bitmap);
                            //if (Tab.IsSelected)
                            //    PoeSorter.ItemCanvas.Children.Add(this.image);
                        });
                        client.Dispose();
                    }).Start();
                }
                else
                {
                    //I prevented something
                }

            }
        }

        private bool IsCloseTo(float value, float point, float threshold)
        {
            return value >= point - threshold && value <= point + threshold;
        }
        private void ScanGemImage(BitmapImage bitmap)
        {
            if (Settings.Instance.GemColorInfo.ContainsKey(this.Name) == false)
            {
                int stride = bitmap.PixelWidth * 4;
                int size = bitmap.PixelHeight * stride;
                byte[] pixels = new byte[size];
                bitmap.CopyPixels(pixels, stride, 0);

                int rHue = 356;
                int gHue = 100;
                int bHue = 220;
                int hueThreshold = 10;

                float rCount = 0;
                float gCount = 0;
                float bCount = 0;

                for (int i = 0; i < pixels.Length; i += 4)
                {
                    int alpha = pixels[i + 3];
                    if (alpha > 0)
                    {
                        int r = pixels[i + 2];
                        int g = pixels[i + 1];
                        int b = pixels[i];
                        System.Drawing.Color c = System.Drawing.Color.FromArgb(r, g, b);
                        float colorHue = c.GetHue();

                        if (c.GetSaturation() > 0.4f)
                        {
                            if (IsCloseTo(colorHue, rHue, hueThreshold))
                                rCount++;
                            else if (IsCloseTo(colorHue, gHue, hueThreshold))
                                gCount++;
                            else if (IsCloseTo(colorHue, bHue, hueThreshold))
                                bCount++;
                        }

                    }
                }

                GemRequirement gemReq = POEStashSorterModels.GemRequirement.None;

                if (rCount > bCount && rCount > gCount)
                    gemReq = POEStashSorterModels.GemRequirement.Str;
                else if (bCount > rCount && bCount > gCount)
                    gemReq = POEStashSorterModels.GemRequirement.Int;
                else // (gCount > rCount && gCount > bCount)
                    gemReq = POEStashSorterModels.GemRequirement.Dex;

                Settings.Instance.GemColorInfo.Add(this.FullItemName, gemReq);
                Settings.Instance.SaveChanges();
            }
        }


        public Tab Tab { get; set; }

        internal Item Clone { get; set; }
        internal Item CloneItem()
        {
            var clone = new Item();
            clone.Id = Id;
            clone.Verified = this.Verified;
            clone.W = this.W;
            clone.H = this.H;
            clone.Icon = this.Icon;
            clone.Support = this.Support;
            clone.League = this.League;
            clone.Name = this.Name;
            clone.TypeLine = this.TypeLine;
            clone.Identified = this.Identified;
            clone.Properties = this.Properties;
            clone.ExplicitMods = this.ExplicitMods;
            clone.DescrText = this.DescrText;
            clone.FrameType = this.FrameType;
            clone.X = this.X;
            clone.Y = this.Y;
            clone.InventoryId = this.InventoryId;
            clone.SocketedItems = this.SocketedItems;
            clone.Sockets = this.Sockets;
            clone.AdditionalProperties = this.AdditionalProperties;
            clone.SecDescrText = this.SecDescrText;
            clone.ImplicitMods = this.ImplicitMods;
            clone.FlavourText = this.FlavourText;
            clone.Requirements = this.Requirements;
            clone.nextLevelRequirements = this.nextLevelRequirements;
            clone.Socket = this.Socket;
            clone.Color = this.Color;
            clone.Corrupted = this.Corrupted;
            clone.CosmeticMods = this.CosmeticMods;
            clone.Tab = this.Tab;

            clone.image = new Image();
            clone.image.Width = 43 * this.W;
            clone.image.Stretch = Stretch.None;
            clone.image.Height = 43 * this.H;
            double offsetX = 47.4 * 13;
            clone.image.Margin = new Thickness(this.X * 47.4f + 2.2f + offsetX, this.Y * 47.4f + 2.2f, 0, 0);
            clone.DownloadImageAsync();
            this.Clone = clone;

            return clone;
        }




        private GemRequirement GetImageColor()
        {
            var img = (BitmapImage)this.image.Source;

            PixelColor[,] pixels = GetPixels(img);

            int r = 0;
            int g = 0;
            int b = 0;
            int count = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    var p = pixels[x, y];
                    if (p.Alpha > 0)
                    {
                        r += p.Red;
                        g += p.Green;
                        b += p.Blue;
                        count++;
                    }
                }
            }
            r /= count;
            g /= count;
            b /= count;

            if (r > g && r > b)
                return GemRequirement.Str;
            if (g > r && g > b)
                return GemRequirement.Dex;
            else
                return GemRequirement.Int;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct PixelColor
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        public PixelColor[,] GetPixels(BitmapSource source)
        {
            if (source.Format != PixelFormats.Bgra32)
                source = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);

            int width = source.PixelWidth;
            int height = source.PixelHeight;
            PixelColor[,] result = new PixelColor[width, height];

            source.CopyPixels(result, width * 4, 0);
            return result;
        }

        public int LevelRequirement
        {
            get
            {
                if (this.Requirements != null)
                {
                    var lvlReq = this.Requirements.FirstOrDefault(x => x.Name == "Level");
                    if (lvlReq != null)
                    {
                        return lvlReq.Value.Max(x => Convert.ToInt32(x.Max(c => Convert.ToInt32(c))));
                    }
                }

                return 0;
            }
        }

        public int Quality
        {
            get
            {
                if (this.Properties != null)
                {
                    var lvlReq = this.Properties.FirstOrDefault(x => x.Name == "Quality");
                    if (lvlReq != null)
                    {
                        return lvlReq.Values.Max(x => Convert.ToInt32(x[0].ToString().Replace("+", "").Replace("%", "").Replace("-", "")));
                    }
                }

                return 0;
            }
        }

        public int Level
        {
            get
            {
                int level = 0;
                if (Properties != null)
                {
                    try
                    {
                        var taa = Properties
                            .FirstOrDefault(x => x.Name.ToLower() == "map level" || x.Name.ToLower() == "level");
                        level = taa.Values.Max(x => Convert.ToInt32(x.Max(c => Convert.ToInt32(c))));
                    }
                    catch (Exception)
                    {
                    }
                }
                return level;
            }
        }
    }

    [DataContract]
    public class Socket
    {
        [JsonProperty(PropertyName = "attr")]
        public string Attribute { get; set; }

        [JsonProperty(PropertyName = "group")]
        public int Group { get; set; }
    }

    [DataContract(Name = "RootObject")]
    public class Stash
    {
        [JsonProperty(PropertyName = "numTabs")]
        public int NumTabs { get; set; }

        [JsonProperty(PropertyName = "items")]
        public List<Item> Items { get; set; }

        [JsonProperty(PropertyName = "tabs")]
        public List<Tab> Tabs { get; set; }
    }

    [DataContract(Name = "RootObject")]
    public class Character
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "league")]
        public string League { get; set; }

        [JsonProperty(PropertyName = "class")]
        public string Class { get; set; }

        [JsonProperty(PropertyName = "classId")]
        public int ClassId { get; set; }

        [JsonProperty(PropertyName = "level")]
        public int Level { get; set; }
    }

    [DataContract(Name = "RootObject")]
    public class Inventory
    {
        [JsonProperty(PropertyName = "items")]
        public List<Item> Items { get; set; }
    }

    public class Colour
    {
        [JsonProperty(PropertyName = "r")]
        public int R { get; set; }
        [JsonProperty(PropertyName = "g")]
        public int G { get; set; }
        [JsonProperty(PropertyName = "b")]
        public int B { get; set; }
    }

    public class Tab
    {
        [JsonProperty(PropertyName = "n")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "i")]
        public int Index { get; set; }
        [JsonProperty(PropertyName = "colour")]
        public Colour Colour { get; set; }
        public string srcL { get; set; }
        public string srcC { get; set; }
        public string srcR { get; set; }

        public bool IsSelected { get; set; }

        public List<Item> Items { get; set; }

        public League League { get; set; }

        public SolidColorBrush Background
        {
            get
            {
                var color = Color.FromRgb((byte)Colour.R, (byte)Colour.G, (byte)Colour.B);
                return new SolidColorBrush(color);
            }
        }

        public SolidColorBrush BackgroundSelected
        {
            get
            {
                var color = Color.FromRgb(
                    (byte)((Colour.R + 255) / 2),
                    (byte)((Colour.G + 255) / 2),
                    (byte)((Colour.B + 255) / 2)
                );
                return new SolidColorBrush(color);
            }
        }

        public bool IsVisible { get; set; }

        public Tab()
        {
            IsVisible = true;
        }
    }
}
