using System.Net;
using POEStashSorterModels;

namespace PoeStashSorterModels.Servers
{
    public abstract class Server
    {
        protected virtual string Domain
        {
            get { return "www.pathofexile.com"; }
        }

        public virtual string Url
        {
            get { return "http://" + Domain; }
        }

        protected virtual string LoginUrl
        {
            get { return string.Format("https://{0}/login", Domain); }
        }

        protected virtual string SessionIdName
        {
            get { return "PHPSESSID"; }
        }

        public virtual string CharacterUrl
        {
            get { return string.Format("http://{0}/character-window/get-characters", Domain); }
        }

        public virtual string StashUrl
        {
            get
            {
                return string.Format("http://{0}/character-window/get-stash-items?league={{0}}&tabs=1&tabIndex={{1}}",
                    Domain);
            }
        }

        public virtual string EmailLoginName
        {
            get { return "Email"; }
        }

        public abstract string Name { get; }

        public CookieAwareWebClient WebClient { get; private set; }

        public virtual void Connect(string email, string password, bool useSessionId = false)
        {
            WebClient = new CookieAwareWebClient();
            if (useSessionId)
            {
                SetCookie(password);
            }
        }


        protected void SetCookie(string password)
        {
            WebClient.Cookies.Add(new Cookie(SessionIdName, password, "/", Domain));
        }
    }
}