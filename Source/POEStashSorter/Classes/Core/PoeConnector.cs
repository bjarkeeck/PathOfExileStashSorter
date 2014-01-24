using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEStashSorter
{
    public class PoeConnector
    {
        public static CookieAwareWebClient Client = new CookieAwareWebClient();
        public static string StashUrl = "http://" + "www.pathofexile.com/character-window/get-stash-items?league={0}&tabs={1}&tabIndex={2}";
        public static string LoginUrl = "https://" + "www.pathofexile.com/login";
        public static bool IsBusy = true;
        public List<StashTab> Tabs = new List<StashTab>();

        public PoeConnector(string email, string password)
        {
            string loginHtml = Client.DownloadString(LoginUrl);

            HtmlDocument h = new HtmlDocument();
            h.LoadHtml(loginHtml);
            string hash = h.DocumentNode.SelectNodes("//input[@name='hash']").First().Attributes["value"].Value;

            Client.BaseAddress = LoginUrl;
            var loginData = new NameValueCollection();
            loginData.Add("login_email", email);
            loginData.Add("login_password", password);
            loginData.Add("login", "Login");
            loginData.Add("remember_me", "0");
            loginData.Add("hash", hash);
            Client.UploadValues("/login", "POST", loginData);
        }


        public void FetchData()
        {
            Tabs.Clear();
            int i = 0;
            foreach (string league in Enum.GetNames(typeof(League)))
            {
                League l = (League)Enum.Parse(typeof(League), league);

                string url = string.Format(StashUrl, league, 1, 0);
                Log.Message("Downloading first " + league + " stashtab");
                string html = Client.DownloadString(url);
                Log.Message("Complete");

                if (html.ToLower() != "false")
                {
                    var obj = JsonConvert.DeserializeObject<RootObject>(html);

                    if (obj.tabs != null && obj.tabs.Count() > 0)
                    {
                        foreach (var item in obj.tabs)
                        {
                            List<Item> items = null;

                            if (item.i == 0)
                                items = obj.items;

                            Tabs.Add(new StashTab()
                            {
                                StashId = item.i,
                                Id = i,
                                League = l,
                                Name = item.n,
                                Items = items
                            });
                            i++;
                        }
                    }
                }
            }
        }


    }
}
