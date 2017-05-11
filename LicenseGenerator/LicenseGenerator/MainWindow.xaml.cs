using System;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro.Controls;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using ZSharpGeneralHelper;

namespace LicenseGenerator
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

        private void clear()
        {
            txtInventorVersion.Clear();
            txtMachineName.Clear();
            txtNoofDays.Clear();
            txtMACAddress.Clear();
            txtFilePath.Clear();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtMachineName.Text == "")
            {
                MessageBox.Show("Please input Machine Name");
            }
            else if (txtMACAddress.Text == "")
            {
                MessageBox.Show("Please input MAC Address");
            }
            else if (txtInventorVersion.Text == "")
            {
                MessageBox.Show("Please input Version Number");
            }
            else if (txtNoofDays.Text == "")
            {
                MessageBox.Show("Please input No Of Days ");
            }
            else if (txtFilePath.Text == "")
            {
                MessageBox.Show("Please input File Path");
            }
            else
            {  // @"F:\Temp.xml"
                try
                {
                    string Path = string.Empty;
                    if (txtFilePath.Text.Trim().EndsWith(@"\"))
                    {
                        Path = txtFilePath.Text + @"License.XML";
                    }
                    else
                    {
                        Path = txtFilePath.Text + @"\License.XML";
                    }

                    //Create a datatable to store XML data
                    DataTable DT = new DataTable();
                    DT.Columns.Add("MachineName");
                    DT.Columns.Add("MACAddress");
                    DT.Columns.Add("Version");
                    DT.Columns.Add("NoOfDays");
                    DT.Columns.Add("DevCode");
                    DT.Columns.Add("GUID");
                    String guid = Guid.NewGuid().ToString();
                    DT.Rows.Add(new object[] { txtMachineName.Text.ToUpper(CultureInfo.CurrentCulture), txtMACAddress.Text.ToUpper(CultureInfo.CurrentCulture).Replace("-", ""), txtInventorVersion.Text.ToUpper(CultureInfo.CurrentCulture), txtNoofDays.Text.ToUpper(CultureInfo.CurrentCulture), tBox_devCode.Text, guid });
                    //Create a dataset
                    DataSet DS = new DataSet();
                    //Add datatable to this dataset
                    DS.Tables.Add(DT);
                    //XmlWriter writer = XmlWriter.Create(@"F:\Temp.xml");
                    //Write dataset to XML file
                    DS.WriteXml(Path);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.PreserveWhitespace = false;
                    xmlDoc.Load(Path);
                    EncryptionHelper.EncryptXML(xmlDoc);
                    xmlDoc.Save(Path);
                    MessageBox.Show("XML data written successfully to " + Path + "");
                    clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception:- Path cannot Found " + ex.Message);
                }
            }
        }
    }
}
