//This is an Intelectual Property of Zcodia Technologies and Raghulan Gowthaman.
//www.zcodiatechnologies.com.au

using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZSharpLicHelper;
using GV = ZSharpLicenseTester.Global.variables;

namespace ZSharpLicenseTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_launch_licGen_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"..\LicenseGenerator.exe");
        }

        private void btn_getMac_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(@"..\SystemNameMac.exe");
        }

        private void btn_read_Click(object sender, RoutedEventArgs e)
        {
            checkLicense();
            Lic lobj = new Lic();
            lobj.getLicDetails(@"R:\Dropbox\GitHub\ZSharpLicense\ReleasePackage\Lic\License.XML");
        }

        public static List<object> checkLicense()
        {
            List<object> li = new List<object>();
            try
            {
                Lic lobj = new Lic();
                string ss = lobj.GetMACAddress().ToString();
                string app_path = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);
                GV.licStatus = lobj.ApplicationStatus(@"R:\Dropbox\GitHub\ZSharpLicense\ReleasePackage\Lic\License.XML");
                GV.licMess = lobj.getAppmessage();

                li.Add(GV.licStatus);
                li.Add(GV.licMess);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return li;
        }
    }
}
