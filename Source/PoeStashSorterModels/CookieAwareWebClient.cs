using System;
using System.Net;

namespace POEStashSorterModels
{
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