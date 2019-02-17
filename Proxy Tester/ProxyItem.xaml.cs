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
using Windows.UI.Xaml.Navigation;
using Proxy_Tester;
using Windows.UI;
using Windows.ApplicationModel.DataTransfer;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Proxy_Tester
{
    public sealed partial class ProxyItem : UserControl
    {
        Proxy containingproxy;
        SolidColorBrush white = new SolidColorBrush(Colors.White);
        SolidColorBrush gray = new SolidColorBrush(Colors.LightGray);

        public ProxyItem(Proxy proxy)
        {
            this.InitializeComponent();
            IPText.Text = proxy.IP;
            PortText.Text = proxy.Port.ToString();
            containingproxy = proxy;
        }

        public async void TestSpeed()
        {
           
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                TestProgressRing.IsActive = true;
                InvalidateArrange();
                PingText.Text = "N/A";
                PingText.InvalidateArrange();
            });


            containingproxy.DownloadSpeedTest();
            //containingproxy.LifeTest();

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                
                
                PingText.Text = containingproxy.Ping == -1 ? "-" : containingproxy.Ping.ToString() + " ms";
                TestProgressRing.IsActive = false;
                TestProgressRing.InvalidateArrange();
                PingText.InvalidateArrange();
            });

          
           

        }

        private void PortText_Copy_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {

            string s = containingproxy.IP + ":" + containingproxy.Port.ToString();
            DataPackage dp = new DataPackage();
            dp.SetText(s);
            Clipboard.SetContent(dp);
            
        }

        private void grd_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            grd.Background = gray;
        }

        private void grd_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            grd.Background = white;
        }

        private void grd_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            grd.Background = white;
        }
    }
}
