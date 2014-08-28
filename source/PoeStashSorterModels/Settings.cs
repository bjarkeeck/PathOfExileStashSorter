using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace POEStashSorterModels
{
    [XmlType]
    public class Settings
    {
        internal static readonly string CONFIGFILE = "../config.bin";
        private static Settings instance;
        public static Settings Instance
        {
            get
            {
                if (instance == null)
                {
                    string xmlPath = AppDomain.CurrentDomain.BaseDirectory + CONFIGFILE;
                    if (File.Exists(xmlPath))
                        using (FileStream file = File.OpenRead(xmlPath))
                            try
                            {
                                instance = Serializer.Deserialize<Settings>(file);
                                file.Close();
                            }
                            catch (Exception ex)
                            {
                                file.Close();
                                instance = new Settings();
                            }
                    else
                        instance = new Settings();
                }
                return instance;
            }
        }

        [XmlElement(Order = 6)]
        public double Speed { get; set; }

        [XmlElement(Order = 5)]
        public string LastSelectedLeague { get; set; }

        [XmlElement(Order = 3)]
        public List<SortingAlgorithmInfo> SortingAlgorithmInfos = new List<SortingAlgorithmInfo>();

        public SortingAlgorithmInfo GetSortingAlgorithmForTab(Tab tab)
        {
            var s = SortingAlgorithmInfos.FirstOrDefault(x => x.League == tab.League.Name && x.TabIndex == tab.Index);
            return s ?? new SortingAlgorithmInfo()
            {
                Name = PoeSorter.SortingAlgorithms.FirstOrDefault().Name,
                Option = PoeSorter.SortingAlgorithms.FirstOrDefault().SortOption.Options.FirstOrDefault()
            };
        }

        public void SaveChanges()
        {
            string xmlPath = AppDomain.CurrentDomain.BaseDirectory + CONFIGFILE;

            using (FileStream file = File.Open(xmlPath, FileMode.Create, FileAccess.ReadWrite))
                Serializer.Serialize(file, this);
        }

        internal void SetSortingAlgorithmForTab(string name, string option, Tab SelectedTab)
        {
            SortingAlgorithmInfo s = SortingAlgorithmInfos.FirstOrDefault(x => x.League == SelectedTab.League.Name && x.TabIndex == SelectedTab.Index);
            if (s == null)
            {
                s = new SortingAlgorithmInfo()
                {
                    League = SelectedTab.League.Name,
                    TabIndex = SelectedTab.Index
                };
                SortingAlgorithmInfos.Add(s);
            }
            s.Name = name;
            s.Option = option;
            SaveChanges();
        }

        [XmlElement(Order = 1)]
        public string Username;

        [XmlElement(Order = 2)]
        public string Password;

        [XmlElement(Order = 4)]
        public Dictionary<string, GemRequirement> GemColorInfo = new Dictionary<string, GemRequirement>();

        [XmlType]
        public class SortingAlgorithmInfo
        {
            [XmlElement(Order = 1)]
            public string League { get; set; }
            [XmlElement(Order = 2)]
            public int TabIndex { get; set; }
            [XmlElement(Order = 3)]
            public string Name { get; set; }
            [XmlElement(Order = 4)]
            public string Option { get; set; }
        }


    }
}
