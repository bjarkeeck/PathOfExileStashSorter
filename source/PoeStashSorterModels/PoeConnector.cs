using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace POEStashSorterModels
{

    public static class PoeConnector
    {
        public static CookieAwareWebClient WebClinet = null;

        private const string LOGINURL = "https://www.pathofexile.com/login";
        private const string CHARACTERURL = "http://www.pathofexile.com/character-window/get-characters";
        private const string STASHURL = "http://www.pathofexile.com/character-window/get-stash-items?league={0}&tabs=1&tabIndex={1}";

        public static void Connect(string email, string password, bool useSessionId = false)
        {
            WebClinet = new CookieAwareWebClient();
            if (useSessionId)
            {
                WebClinet.Cookies.Add(new System.Net.Cookie("PHPSESSID", password, "/", "www.pathofexile.com"));
            }
            else
            {
                string loginHtml = WebClinet.DownloadString(LOGINURL);
                HtmlDocument h = new HtmlDocument();
                h.LoadHtml(loginHtml);
                string hash = h.DocumentNode.SelectNodes("//input[@name='hash']").First().Attributes["value"].Value;

                WebClinet.BaseAddress = LOGINURL;
                var loginData = new NameValueCollection();
                loginData.Add("login_email", email);
                loginData.Add("login_password", password);
                loginData.Add("login", "Login");
                loginData.Add("remember_me", "0");
                loginData.Add("hash", hash);
                WebClinet.UploadValues("/login", "POST", loginData);
            }
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
            string jsonData = WebClinet.DownloadString(string.Format(STASHURL, league.Name, 0));
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
            string jsonData = WebClinet.DownloadString(string.Format(STASHURL, league.Name, tabIndex));
            Stash stash = JsonConvert.DeserializeObject<Stash>(jsonData);
            Tab tab = stash.Tabs.FirstOrDefault(x => x.Index == tabIndex);
            tab.Items = stash.Items;
            return tab;
        }

        public static async Task<Tab> FetchTabAsync(int tabIndex, League league)
        {
            while (WebClinet.IsBusy) { }
            string jsonData = await WebClinet.DownloadStringTaskAsync(new Uri(string.Format(STASHURL, league.Name, tabIndex)));
            Stash stash = JsonConvert.DeserializeObject<Stash>(jsonData);
            Tab tab = stash.Tabs.FirstOrDefault(x => x.Index == tabIndex);
            tab.Items = stash.Items;
            return tab;
        }

        public static List<Character> FetchCharecters()
        {
            List<Character> charecters;
            string jsonData = WebClinet.DownloadString(CHARACTERURL);
            charecters = JsonConvert.DeserializeObject<List<Character>>(jsonData);
            return charecters;
        }
    }

    public class CookieAwareWebClient : WebClient
    {
        internal CookieContainer Cookies = new CookieContainer();
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = Cookies;
            }
            return request;
        }
    }

}
