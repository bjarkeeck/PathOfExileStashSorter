using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace POEStashSorter
{
    public class StashTab
    {
        public bool IsSorted = false;

        public int Id { get; set; }
        public int StashId { get; set; }
        public string Name { get; set; }
        public League League { get; set; }
        private string leagueString
        {
            get
            {
                return Enum.GetName(League.GetType(), League);
            }
        }
        public List<StashItem> StashItems { get; set; }

        private List<Item> items = new List<Item>();
        public List<Item> Items
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                AddStashItems();
            }
        }

        public void DownloadStashItems()
        {
            string url = string.Format(PoeConnector.StashUrl, leagueString, 0, StashId);
            PoeConnector.IsBusy = true;
            Log.Message("Downloading " + leagueString + " - " + Name + " stashtab");
            string htmlOrJson = PoeConnector.Client.DownloadString(url);
            Log.Message("Complete");
            Items = JsonConvert.DeserializeObject<RootObject>(htmlOrJson).items;
            PoeConnector.IsBusy = false;

        }

        private void AddStashItems()
        {
            StashItems = new List<StashItem>();
            int i = 0;
            if (Items != null && Items.Count() > 0)
            {
                foreach (Item item in Items)
                {
                    StashItem gem = new StashItem();

                    if (item.icon.ToLower().Contains("gems") || item.icon.ToLower().Contains("maps") || item.icon.ToLower().Contains("amulets") || item.icon.ToLower().Contains("rings"))
                    {
                        i++;

                        gem.Id = i;
                        gem.X = item.x;
                        gem.Y = item.y;
                        gem.Name = item.typeLine;
                        gem.Image = item.icon;

                        if (item.icon.ToLower().Contains("maps"))
                        {
                            var taa = item.properties.FirstOrDefault(x => x.name.ToLower() == "map level");
                            gem.MapLevel = Convert.ToInt32(((JArray)taa.values.First()).ToObject<List<string>>().First());
                        }
                        else if (item.icon.ToLower().Contains("gems"))
                        {
                            Requirement topRequirement = item.requirements
                                .Where(x => x.name.ToLower() == "dex" || x.name.ToLower() == "str" || x.name.ToLower() == "int")
                                .OrderByDescending(x => Convert.ToInt32(x.values[0][0]))
                                .FirstOrDefault();

                            if (topRequirement != null)
                            {
                                if (topRequirement.name.ToLower() == "dex")
                                    gem.GemColor = GemColor.Green;
                                if (topRequirement.name.ToLower() == "int")
                                    gem.GemColor = GemColor.Blue;
                                if (topRequirement.name.ToLower() == "str")
                                    gem.GemColor = GemColor.Red;
                            }
                            else
                            {
                                //Scan image and find color.
                                gem.GemColor = GetImageColor(item.icon);
                            }

                            if (gem.Image.ToLower().Contains("icenova"))
                            {
                                gem.GemColor = GemColor.Green;
                            }
                        }

                        if (item.properties != null && item.properties.Any(x => x.name.ToLower() == "quality"))
                        {
                            var ta = item.properties.FirstOrDefault(x => x.name.ToLower() == "quality");
                            gem.Quiality = Convert.ToInt32(((JArray)ta.values.First()).ToObject<List<string>>().First().Replace("+", "").Replace("-", "").Replace("%", ""));
                        }
                        else
                        {
                            gem.Quiality = 0;
                        }

                        gem.GemType = GemType.Normal;

                        if (item.support)
                            gem.GemType = GemType.Support;

                        if (string.IsNullOrWhiteSpace(item.secDescrText) == false)
                        {
                            if (item.secDescrText.ToLower().Contains("aura"))
                                gem.GemType = GemType.Aura;
                        }

                        StashItems.Add(gem);
                    }
                }
            }

        }





        public Bitmap GetImageFromUrl(string url)
        {
            WebRequest request = System.Net.WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();

            return new Bitmap(responseStream);
        }
        private GemColor GetImageColor(string url)
        {
            Bitmap img = GetImageFromUrl(url);

            int r = 0;
            int g = 0;
            int b = 0;
            int count = 0;
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    var p = img.GetPixel(x, y);
                    if (p.A > 0)
                    {
                        r += p.R;
                        g += p.G;
                        b += p.B;
                        count++;
                    }
                }
            }

            r /= count;
            g /= count;
            b /= count;

            if (r > g && r > b)
                return GemColor.Red;
            if (g > r && g > b)
                return GemColor.Green;
            else
                return GemColor.Blue;
        }
    }
}
