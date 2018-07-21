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
using Windows.Networking;
using System.Text;
using System.Net.NetworkInformation;
using Windows.ApplicationModel.DataTransfer;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


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

          
               
            IsTesting = false;
            this.InitializeComponent();

            
            
         
            
        }
        string Sproxylist;

     


        List<Proxy> ProxyList = new List<Proxy>();
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {


         
            if (IPTextBox.Text == null || PortTextBox.Text == null)
            {
                return;
            
            }

            try
            {
              
                ProxyList.Add(new Proxy(IPTextBox.Text, Convert.ToInt32(PortTextBox.Text)));
                ProxyListView.ItemsSource = ProxyList;
                
            }
            catch (Exception t)
            {
                PortTextBox.Text = t.Message;
            }
            
        }
        bool IsTesting;
        private void TestButton_Click(object sender, RoutedEventArgs e)
        {

            if (IsTesting)
            {
                ProxyListView.ItemsSource = null;
                ProxyListView.ItemsSource = ProxyList;
                return;

            }
            foreach (Proxy proxy in ProxyList)
            { 
                Task.Run(() =>
                {

                    IsTesting = true;
                //  proxy.LifeTest();
                proxy.DownloadSpeedTest();




                });
        }
            ProxyListView.ItemsSource = null;
            ProxyListView.ItemsSource = ProxyList;
            ////IsTesting = false;
        }

        private void PasteButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            GetProxies(); //gets proxies into proxylist as string
        ProxyList = ProxyList.Concat<Proxy>(ParseProxies()).ToList<Proxy>();
            
            ProxyListView.ItemsSource = null; //didn't find any other way to refresh the view
            ProxyListView.ItemsSource = ProxyList;
        }

        List<Proxy> ParseProxies() //This was all i could do :D
        {
            if (Sproxylist == null)
                return null;

            List<Proxy> PastedList = new List<Proxy>();
            int seek = 0;
            string IP;
            string port;
            int last = 0;
            for (int i =Sproxylist.Length-1; i>0; i--)
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
                

            }
            return PastedList;
        }


        //Get the proxies in clipboard
        async void GetProxies()
        {
            DataPackageView dpv = Clipboard.GetContent();
            Sproxylist = await dpv.GetTextAsync();
        }
    }



    class Proxy
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

        public override string ToString()
        {

            //if (IsAlive == false)
            //return IP + ":" + Port.ToString() + " ---> DEAD";

            //else
                return IP + ":" + Port.ToString() + " ---> " + Ping;
        }

        public  void LifeTest() //Test if proxy is alive
        {
            IsAlive = false;

            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s.Connect(IP, Port);
            if (s.Connected)
            {
                IsAlive = true;
            }
            s.Shutdown(SocketShutdown.Both);
        }


        public  void DownloadSpeedTest()
        {
            //if (IsAlive == false) //make sure we're talking to alive proxy
            //  return;
            
                Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(IP, Port);
                string req = "GET /connecttest.txt HTTP/1.1\r\nHost: msftconnecttest.com\n\n";
                byte[] ans = new byte[4];
                s.SendTimeout = 3000;
                s.ReceiveTimeout = 5000;
            if (!s.Connected)
                return;
                s.Send(Encoding.ASCII.GetBytes(req));
                DateTime bef = DateTime.Now;
                DateTime af = DateTime.Now;
            try
            {
                s.Receive(ans);
            }
            catch (Exception r)
            {
                return;
            }
                af = DateTime.Now;

                s.Shutdown(SocketShutdown.Both);
            
            
            Ping = af.Subtract(bef).Milliseconds;

            if (Ping < 0)
            {
                Ping = 0;
                IsAlive = false;
            }
        }
    
    }

    enum ProxyType
    {
        HTTP, HTTPS, SOCKS4, SOCKS5
    }
}
