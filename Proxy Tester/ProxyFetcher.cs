using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using System.Net.Security;
using Windows.Storage.Streams;
using System.Net.Http;
using Windows.Networking;

namespace Proxy_Tester
{
    public static class ProxyFetcher
    {
        public async static Task<List<Proxy>> FetchFrom(string url)
        {
                HttpClientHandler hcl = new HttpClientHandler();
                hcl.UseProxy = false;
                HttpClient cl = new HttpClient(hcl);

                HttpResponseMessage r = await cl.GetAsync("https://www.proxy-list.download/api/v1/get?type=https");
                string response = await r.Content.ReadAsStringAsync();
                return ParseProxies(response);
        }

        static List<Proxy> ParseProxies(string data)
        {
           List<Proxy> proxylist = new List<Proxy>();
           string[] proxies = data.Split('\n');

           for (int i = 0;  i < 100;  i++)
           {
                string proxy = proxies[i];
                int colindex = proxy.IndexOf(":");
                try
                {
                    proxylist.Add(new Proxy(proxy.Substring(0, proxy.IndexOf(":")), Convert.ToInt32(proxy.Substring(colindex + 1, proxy.Length - colindex - 2))));
                }
                catch (Exception e)
                {
                    string s = e.Message;
                }
           }
            return proxylist;  
        }
        

    }



}
