using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using System.Net.Http;
using Windows.UI.Xaml.Navigation;
using System.Net.WebSockets;
using System.Net.Sockets;
using Windows.Networking.Sockets;
using Windows.Networking;
using System.Text;
using System.Net.NetworkInformation;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Proxy_Tester
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {

               
            
            this.InitializeComponent();
            
        }

        private void ProxyListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ProxyListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Proxy clicked = e.ClickedItem as Proxy;


            string s = clicked.IP + ":" + clicked.Port.ToString();
            DataPackage dp = new DataPackage();
            dp.SetText(s);
            Clipboard.SetContent(dp);
        }

        string Sproxylist;

     


        List<Proxy> ProxyList = new List<Proxy>();
        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (IPTextBox.Text == null || PortTextBox.Text == null)
            {
                return;
            }
            try
            {
                
                ProxyList.Add(new Proxy(IPTextBox.Text, Convert.ToInt32(PortTextBox.Text)));
                ProxyItem pi = new ProxyItem(new Proxy(IPTextBox.Text, Convert.ToInt32(PortTextBox.Text)));
                pi.Margin = new Thickness(pi.Margin.Left + 5, 0, pi.Margin.Right + 5, pi.Margin.Bottom + 5);
                rel.Children.Add(pi);
               
            }
            catch (Exception t)
            {
                PortTextBox.Text = t.Message;
            }
            
        }
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            
          
            foreach (UIElement pru in rel.Children)
            {
                ProxyItem proxy = pru as ProxyItem;
                
                Task.Run(() => { proxy.TestSpeed(); }) ;   
            }
        }

        private void PasteButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            GetProxies(); //gets proxies into proxylist as string

            ProxyList = ParseProxiest();
            if (ProxyList.Count == 0)
                ProxyList = ParseProxiesrn();

            
        }

        List<Proxy> ParseProxiest() //This was all i could do :D
        {
            if (Sproxylist == null)
                return null;
          

            List<Proxy> PastedList = new List<Proxy>();
            int seek = 0;
            string IP;
            string port;
            int last = 0;
            for (int i = Sproxylist.Length - 1; i > 0; i--)
            {
                if (Sproxylist[i] == '\t')
                {
                    last = i;
                    break;
                }
            }
            while (seek < last-2)
            {
                 IP = Sproxylist.Substring(seek, Sproxylist.IndexOf('\t', seek) - seek);
                 seek = Sproxylist.IndexOf('\t', seek);
                

                port = Sproxylist.Substring(seek + 1, Sproxylist.IndexOf('\t', seek + 1) - seek - 1);
                seek = Sproxylist.IndexOf('\n', seek+2) +1;
                if (seek == 0)
                    break;
                PastedList.Add(new Proxy(IP, Convert.ToInt32(port)));
                ProxyItem pi = new ProxyItem(new Proxy(IP, Convert.ToInt32(port)));
                pi.Margin = new Thickness(pi.Margin.Left + 5, 0, pi.Margin.Right + 5, pi.Margin.Bottom + 5);
                rel.Children.Add(pi);


            }
            return PastedList;
        }

        List<Proxy> ParseProxiesrn()
        {
            List<Proxy> proxylist = new List<Proxy>();
            string IP;
            int port;
            int seek = 0;
            int last = 0;
            //for the first one
           IP = Sproxylist.Substring(0, Sproxylist.IndexOf('\r'));
            seek = Sproxylist.IndexOf('\r') + 1;
            port = Convert.ToInt32(Sproxylist.Substring(seek + 1, Sproxylist.IndexOf('\r', seek + 1) - seek - 1));
            seek = Sproxylist.IndexOf('\r', seek + 1);
            proxylist.Add(new Proxy(IP, port));

            for (int i=Sproxylist.Length - 1; i> 0; i--)
            {
                if (Sproxylist[i] == '\r')
                {
                    last = i;
                    break;
                }
            }

            //first one added, now go through proxylist
            while (seek < last - 3)
            {
                seek = Sproxylist.IndexOf('\r', seek + 1);
                if (Sproxylist.IndexOf('.', seek + 1) - seek < 6 && Sproxylist.IndexOf('.', seek +1) != -1) //its an IP FUUUUCKK
                {
                    IP = Sproxylist.Substring(seek + 2 , Sproxylist.IndexOf('\r', seek + 1) - seek -2);
                    seek = Sproxylist.IndexOf('\r', seek + 1);
                    port = Convert.ToInt32(Sproxylist.Substring(seek + 2, Sproxylist.IndexOf('\r', seek + 1) - seek - 2));
                    proxylist.Add(new Proxy(IP, port));
                }
            }

            return proxylist;
        }


        //Get the proxies in clipboard
        async void GetProxies()
        {
            DataPackageView dpv = Clipboard.GetContent();
            Sproxylist = await dpv.GetTextAsync();
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {

           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DownloadProgressRing.IsActive = true;
            await DownloadProxies();
            DownloadProgressRing.IsActive = false;
        }


        async Task DownloadProxies()
        {
            try
            {
                ProxyList = await ProxyFetcher.FetchFrom("");
            }
            catch (Exception e)
            {
                Download_List.Content = "Failed";
            }
            foreach (Proxy proxy in ProxyList)
            {
                ProxyItem pi = new ProxyItem(proxy);
                pi.Margin = new Thickness(pi.Margin.Left + 5, 0, pi.Margin.Right + 5, pi.Margin.Bottom + 5);
                rel.Children.Add(pi);
            }
           
        }
    }



    public class Proxy
    {
        public string IP;
        public int Port;
        public ProxyType ProxyType;
        public bool IsAlive;
        public int Ping;


    
        public Proxy(string ip, int port)
        {
            if (ip != null)
                IP = ip;
            if (port > 0)
                Port = port;

        }


        public void LifeTest() //Test if proxy is alive
        {
            IsAlive = false;
            DateTime bef = DateTime.Now;
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            try
            {
                s.Connect(IP, Port);
            }
            catch (Exception e)
            {
                Ping = -1;
                return;
            }
            if (s.Connected)
            {
                IsAlive = true;
                DateTime af = DateTime.Now;
              Ping = (af - bef).Milliseconds;
             
            }
            s.Shutdown(SocketShutdown.Both);
        }


        public  void DownloadSpeedTest()
        {

            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(IP, Port);
                string req = "GET /connecttest.txt HTTP/1.1\r\nHost: msftconnecttest.com\n\n";
                byte[] ans = new byte[4];
                s.SendTimeout = 2000;
                s.ReceiveTimeout = 2000;
                
                if (!s.Connected)
                {
                    Ping = -1;
                    return;
                }
                s.Send(Encoding.ASCII.GetBytes(req));
                DateTime bef = DateTime.Now;
                DateTime af = DateTime.Now;

                s.Receive(ans);
                af = DateTime.Now;

                s.Shutdown(SocketShutdown.Both);
                Ping = (af - bef).Milliseconds;
            }
            catch (Exception r)
            {
                Ping = -1;
                return;
            }
               


          

            if (Ping < 0)
            {
                Ping = 0;
                IsAlive = false;
            }
        }
    
    }

    public enum ProxyType
    {
        HTTP, HTTPS, SOCKS4, SOCKS5
    }
}
