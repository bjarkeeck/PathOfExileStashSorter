using System.Collections.Specialized;
using System.Linq;
using HtmlAgilityPack;

namespace PoeStashSorterModels.Servers
{
    public class GeneralServer : Server
    {
        public override string Name
        {
            get { return "EuServer"; }
        }

        public override void Connect(string email, string password, bool useSessionId = false)
        {
            base.Connect(email, password, useSessionId);
            if (!useSessionId)
            {
                string loginHtml = WebClient.DownloadString(LoginUrl);
                HtmlDocument h = new HtmlDocument();
                h.LoadHtml(loginHtml);
                string hash = h.DocumentNode.SelectNodes("//input[@name='hash']").First().Attributes["value"].Value;

                WebClient.BaseAddress = LoginUrl;
                var loginData = new NameValueCollection();
                loginData.Add("login_email", email);
                loginData.Add("login_password", password);
                loginData.Add("login", "Login");
                loginData.Add("remember_me", "0");
                loginData.Add("hash", hash);
                WebClient.UploadValues("/login", "POST", loginData);
            }
        }
    }
}