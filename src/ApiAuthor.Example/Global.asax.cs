using System;
using System.Web;
using System.Web.Http;
using ApiAuthor.Example.App_Start;

namespace ApiAuthor.Example
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configure(ApiAuthorConfig.Configure);
        }
    }
}