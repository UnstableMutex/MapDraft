using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyOwnMap
{
    class WebClientBuilder
    {
        public static WebClient GetWebClient()
        {
            using (var wc = new WebClient())
            {
                wc.Proxy = WebRequest.DefaultWebProxy;
                wc.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                wc.Credentials = wc.Proxy.Credentials;

                return wc;
            }


        }
    }
}
