using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeStashSorterModels.Servers;

namespace POEStashSorterModels
{

    public static class PoeConnector
    {
        public static Server server;
       
        public static void Connect(Server server, string email, string password, bool useSessionId = false)
        {
            PoeConnector.server = server;
            server.Connect(email,password,useSessionId);
        }

        public static List<League> FetchLeagues()
        {
            return FetchCharecters()
                .GroupBy(x => x.League)
                .Select(x => new League(x.First().League))
                .ToList();
        }

        public static List<Tab> FetchTabs(League league)
        {
            string jsonData = server.WebClient.DownloadString(string.Format(server.StashUrl, league.Name, 0));
            if (jsonData != "false")
            {
                Stash stash = JsonConvert.DeserializeObject<Stash>(jsonData);
                List<Tab> tabs = stash.Tabs;
                tabs.ForEach(x => x.League = league);
                return tabs;
            }
            return new List<Tab>();
        }

        [Obsolete]
        public static Tab FetchTab(int tabIndex, League league)
        {
            string jsonData = server.WebClient.DownloadString(string.Format(server.StashUrl, league.Name, tabIndex));
            Stash stash = JsonConvert.DeserializeObject<Stash>(jsonData);
            Tab tab = stash.Tabs.FirstOrDefault(x => x.Index == tabIndex);
            tab.Items = stash.Items;
            return tab;
        }

        public static async Task<Tab> FetchTabAsync(int tabIndex, League league)
        {
            while (server.WebClient.IsBusy) { }
            string jsonData = await server.WebClient.DownloadStringTaskAsync(new Uri(string.Format(server.StashUrl, league.Name, tabIndex)));
            Stash stash = JsonConvert.DeserializeObject<Stash>(jsonData);
            Tab tab = stash.Tabs.FirstOrDefault(x => x.Index == tabIndex);
            tab.Items = stash.Items;
            return tab;
        }

        public static List<Character> FetchCharecters()
        {
            List<Character> charecters;
            string jsonData = server.WebClient.DownloadString(server.CharacterUrl);
            charecters = JsonConvert.DeserializeObject<List<Character>>(jsonData);
            return charecters;
        }
    }
}
