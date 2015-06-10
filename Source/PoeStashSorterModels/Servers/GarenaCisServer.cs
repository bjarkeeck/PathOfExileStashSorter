using System;
using System.Linq;
using OpenQA.Selenium.PhantomJS;

namespace PoeStashSorterModels.Servers
{
    public class GarenaCisServer : Server
    {
        protected override string Domain
        {
            get { return "web.poe.garena.ru"; }
        }

        public override string Name
        {
            get { return "GarenaCIS"; }
        }
        
        public override string EmailLoginName
        {
            get { return "Login"; }
        }

        public override void Connect(string email, string password, bool useSessionId = false)
        {
            base.Connect(email, password, useSessionId);
            if (!useSessionId)
            {
                var driverService = PhantomJSDriverService.CreateDefaultService();
                driverService.HideCommandPromptWindow = true;
                using (var driver = new PhantomJSDriver(driverService))
                {
                    driver.Url = LoginUrl;
                    driver.FindElementById("sso_login_form_account").SendKeys(email);
                    driver.FindElementById("sso_login_form_password").SendKeys(password);
                    var oldUrl = driver.Url;
                    driver.FindElementById("confirm-btn").Click();
                    while (oldUrl == driver.Url)
                    {
                        driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(100));
                    }
                    var sid =driver.Manage().Cookies.AllCookies.Where(y => y.Name == SessionIdName).Select(y => y.Value).FirstOrDefault();
                    SetCookie(sid);
                }
            }
        }
    }
}