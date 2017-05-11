//This is an Intelectual Property of Zcodia Technologies and Raghulan Gowthaman.
//www.zcodiatechnologies.com.au

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Data;
using System.Security.Cryptography;
using System.Windows;

namespace ZSharpLicHelper
{
    public class Lic
    {
        string globalPath = @"VL\Protection";
        string reg_Install = "Install";
        string reg_Use = "Use";
        string reg_TICKCAD = "Lisp";
        string guid = string.Empty;

        private DateTime GetCreationDateOfFolder(string Path)
        {
            string directoryString = Path;
            Directory.CreateDirectory(directoryString);
            DateTime dateTime = Directory.GetCreationTime(directoryString);
            return dateTime;
        }

        private string SetKey()
        {
            if (globalPath != string.Empty)
            {

                return globalPath;
            }
            else
            {
                return "SampleKey";
            }
        }

        private String dayDifPutPresent(int NoOFDAYS) //no of days in aurgument is from the license file
        {
            // get present date from system
            DateTime dt = DateTime.Now;
            string today = dt.ToShortDateString();
            DateTime presentDate = Convert.ToDateTime(today);

            // get instalation date
            RegistryKey regkey = Registry.CurrentUser;

            //string encrp_global = EncriptionDecription.Encrypt(globalPath, SetKey());
            //string encrp_install = EncriptionDecription.Encrypt(reg_Install, SetKey());
            //string encrp_use = EncriptionDecription.Encrypt(reg_Use, SetKey());


            regkey = regkey.CreateSubKey(@"Software\" + globalPath); //path
            string Br = (string)regkey.GetValue(reg_Install);
            //MessageBox.Show("1 " + Br);
            // string encrp_br = EncriptionDecription.Decrypt(Br, SetKey());

            DateTime installationDate = Convert.ToDateTime(Br);
            //MessageBox.Show("2 " + installationDate.ToString());
            TimeSpan diff = presentDate.Subtract(installationDate); //first.Subtract(second);
            int totaldays = (int)diff.TotalDays;
            //MessageBox.Show("3 " + totaldays.ToString() + " Lic No of days: " + NoOFDAYS);
            // special check if user chenge date in system
            string usd = (string)regkey.GetValue(reg_Use);

            //string encrp_usd = EncriptionDecription.Decrypt(usd, SetKey());

            DateTime lastUse = Convert.ToDateTime(usd);
            TimeSpan diff1 = presentDate.Subtract(lastUse); //first.Subtract(second);
            int useBetween = (int)diff1.TotalDays;

            // put next use day in registry
            regkey.SetValue(reg_Use, today); //Value Name,Value Data

            if (useBetween >= 0)
            {
                if (totaldays < 0)
                    return "Error"; // if user change date in system like date set before installation
                else if (totaldays >= 0 && totaldays <= NoOFDAYS)
                    return Convert.ToString(NoOFDAYS - totaldays); //how many days remaining
                else
                    return "Expired"; //Expired
            }
            else
                return "Error"; // if user change date in system
        }

        private void blackList()
        {
            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.CreateSubKey(globalPath); //path
            regkey.SetValue("Black", "True");
        }

        private bool blackListCheck()
        {
            //RegistryKey regkey = Registry.CurrentUser;

            //string encrp_global = EncriptionDecription.Encrypt(globalPath, SetKey());
            //string black = EncriptionDecription.Encrypt("Black", SetKey());
            //regkey = regkey.CreateSubKey(@"Software\" + encrp_global); //path

            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.CreateSubKey(globalPath); //path

            if (!string.IsNullOrEmpty((string)regkey.GetValue("Black")))
                return false; //No
            else
                return true;//Yes
        }

        private String checkfirstDate()
        {


            //RegistryKey regkey = Registry.CurrentUser;
            //regkey = regkey.CreateSubKey(globalPath); //path
            //string Br = (string)regkey.GetValue("Install");
            //if (regkey.GetValue("Install") == null)
            //    return "First";
            //else
            //    return Br;


            RegistryKey regkey = Registry.CurrentUser;

            //string encrp_global = EncriptionDecription.Encrypt(globalPath, SetKey());
            //string encrp_install = EncriptionDecription.Encrypt(reg_Install, SetKey());
            //string encrp_use = EncriptionDecription.Encrypt(reg_Use, SetKey());
            //string encrp_tickcad = EncriptionDecription.Encrypt(reg_TICKCAD, SetKey());
            ////string encrp_version = EncriptionDecription.Encrypt("V.1.10", SetKey());

            regkey = regkey.CreateSubKey(@"Software\" + globalPath); //path
            string Br = (string)regkey.GetValue(reg_Install);
            string Brs = (string)regkey.GetValue(reg_TICKCAD);

            if (Brs == guid)
            {
                Br = "Same";
            }
            else
            {
                try
                {
                    string[] regs = regkey.GetSubKeyNames();

                    if (regs.Count() > 0)
                    {

                        regkey.DeleteValue(reg_Install);
                        regkey.DeleteValue(reg_Use);
                        regkey.DeleteValue(reg_TICKCAD);
                        if (regkey.GetValue(reg_Install) == null)
                            return "First";
                        else
                            Br = "First";

                    }
                    else
                    {
                        Br = "First";

                    }
                }
                catch
                {

                }
            }
            return Br;
        }

        private void firstTime()
        {
            RegistryKey regkey = Registry.CurrentUser;
            //string encrp_global = EncriptionDecription.Encrypt(globalPath, SetKey());
            //string encrp_install = EncriptionDecription.Encrypt(reg_Install, SetKey());
            //string encrp_use = EncriptionDecription.Encrypt(reg_Use, SetKey());
            //string encrp_tickcad = EncriptionDecription.Encrypt(reg_TICKCAD, SetKey());

            //  string encrp_version = EncriptionDecription.Encrypt(guid, SetKey());
            regkey = regkey.CreateSubKey(@"Software\" + globalPath); //path

            DateTime dt = DateTime.Now;
            string onlyDate = dt.ToShortDateString(); // get only date not time
                                                      //  string encrp_onlyDate = EncriptionDecription.Encrypt(dt.ToShortDateString(), SetKey());
            regkey.SetValue(reg_Install, onlyDate); //Value Name,Value Data
            regkey.SetValue(reg_Use, onlyDate); //Value Name,Value Data
            regkey.SetValue(reg_TICKCAD, guid);
        }

        public string GetMACAddress()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAddress = adapter.GetPhysicalAddress().ToString();

                    //MessageBox.Show(sMacAddress);
                }
            }
            return sMacAddress;
        }

        public void Appstatus(string statusValue)
        {
            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.CreateSubKey(@"Software\" + globalPath); //path
            regkey.SetValue("status", statusValue); //Value Name,Value Data

        }

        public void Appmessage(string messgaeValue)
        {
            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.CreateSubKey(@"Software\" + globalPath); //path
            regkey.SetValue("Message", messgaeValue); //Value Name,Value Data
        }

        public string getAppmessage()
        {
            RegistryKey regkey = Registry.CurrentUser;
            regkey = regkey.OpenSubKey(@"Software\" + globalPath); //path
            string val = regkey.GetValue("Message").ToString(); //Value Name,Value Data
            return val;
        }

        private static Dictionary<string, string> licDetailsDict = new Dictionary<string, string>();
        public Dictionary<string,string> getLicDetails(string licFile)
        {
            try
            {
                string path = licFile;// @"License.XML";

                if (File.Exists(path))
                {
                    System.Data.DataSet _ds = new System.Data.DataSet();
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc = Helper.Encryption.getXmlData(path);
                    _ds.ReadXml(new XmlTextReader(new System.IO.StringReader(xmlDoc.InnerXml)));
                    guid = _ds.Tables[0].Rows[0]["GUID"].ToString();

                    if (_ds.Tables[0].Rows.Count > 0)
                    {
                        string macAddress = GetMACAddress();
                        // MessageBox.Show(macAddress);
                        licDetailsDict.Add("MAC" , _ds.Tables[0].Rows[0]["MacAddress"].ToString().Trim());
                        licDetailsDict.Add("MachineName", _ds.Tables[0].Rows[0]["MachineName"].ToString().Trim());
                        licDetailsDict.Add("Version", _ds.Tables[0].Rows[0]["Version"].ToString().Trim());
                        licDetailsDict.Add("NoOfDays", _ds.Tables[0].Rows[0]["NoOfDays"].ToString().Trim());
                        licDetailsDict.Add("DevCode", _ds.Tables[0].Rows[0]["DevCode"].ToString().Trim());
                        licDetailsDict.Add("GUID", _ds.Tables[0].Rows[0]["GUID"].ToString().Trim());
                    }
                }
            }
            catch (SystemException ex)
            { }

            return licDetailsDict;
        }
        public bool ApplicationStatus(string licFile)
        {
            //string path = @"C:\Program Files\Autodesk\License.xml";
            string path = licFile;// @"License.XML";
            //  DataTable dt = getIniFileValues();


            if (File.Exists(path))
            {

                System.Data.DataSet _ds = new System.Data.DataSet();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc = Helper.Encryption.getXmlData(path);
                _ds.ReadXml(new XmlTextReader(new System.IO.StringReader(xmlDoc.InnerXml)));

                guid = _ds.Tables[0].Rows[0]["GUID"].ToString();


                if (_ds.Tables[0].Rows.Count > 0)
                {
                    string macAddress = GetMACAddress();
                    // MessageBox.Show(macAddress);
                    Global.variable.machineDetails = "MAC Address: " + _ds.Tables[0].Rows[0]["MacAddress"].ToString().Trim() + "\n";
                    Global.variable.machineDetails += "Machine Name: " + _ds.Tables[0].Rows[0]["MachineName"].ToString() + "\n";
                    Global.variable.machineDetails += "Version: " + _ds.Tables[0].Rows[0]["Version"].ToString() + "\n";
                    if (_ds.Tables[0].Rows[0]["MacAddress"].ToString().Trim().Equals(macAddress.Trim(), StringComparison.InvariantCultureIgnoreCase) && _ds.Tables[0].Rows[0]["MachineName"].ToString().Equals(Environment.MachineName.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        bool block = blackListCheck();
                        string chinstall = checkfirstDate();
                        if (chinstall == "First")
                        {
                            firstTime();
                            Appstatus("Access");
                            Appmessage("FirsttimeLogin");
                            Global.variable.app_lic_status = true;
                        }
                        else
                        {
                            Global.variable.app_lic_status = false;
                            string status = dayDifPutPresent(Convert.ToInt16(_ds.Tables[0].Rows[0]["NoOfDays"].ToString()));
                            //MessageBox.Show("4 no of days from LIC file" + _ds.Tables[0].Rows[0]["NoOfDays"].ToString() + " | " + status);
                            Global.variable.machineDetails += "Status: " + status + "\n";
                            Global.variable.machineDetails += "No of Days Remaining: " + _ds.Tables[0].Rows[0]["NoOfDays"].ToString() + "\n";
                            Global.variable.app_lic_status = true;
                            Appmessage(Global.variable.machineDetails);
                            if (status == "Error")
                            {
                                Global.variable.app_lic_status = false;
                                Global.variable.app_lic_status_mess = "Application Can't be loaded, Unauthorized Date Interrupt Occurred! Without activation it can't run! Would you like to activate? \nPlease Contact Administrator!" + "\nwww.zcodia.com.au";
                                blackList();
                                Appstatus("NOAccess");
                                Appmessage("Application Can't be loaded, Unauthorized Date Interrupt Occurred! Without activation it can't run! Would you like to activate? \nPlease Contact Administrator!" + "\nwww.zcodia.com.au");
                            }
                            else if (status == "Expired")
                            {
                                Global.variable.app_lic_status = false;
                                Global.variable.app_lic_status_mess = "The trial version is now expired! Would you Like to Activate? \nPlease Contact Administrator!" + "\nwww.zcodia.com.au";
                                Appstatus("NOAccess");
                                Appmessage("The trial version is now expired! Would you Like to Activate? \nPlease Contact Administrator!" + "\nwww.zcodia.com.au");
                            }
                        }


                    }
                    else
                    {
                        Global.variable.app_lic_status = false;
                        Global.variable.app_lic_status_mess = "Machine Name or Mac Address is not valid. \nPlease Contact Administartor!" + "\nwww.zcodia.com.au";
                        Appstatus("NOAccess");
                        Appmessage("Machine Name or Mac Address is not valid. \nPlease Contact Administartor!" + "\nwww.zcodia.com.au");
                    }
                }
            }
            else
            {
                Global.variable.app_lic_status = false;
                Global.variable.app_lic_status_mess = @"License File Not Found!" + "\n" + @"Check if license file exists under " + Global.variable.app_path;
                Appstatus("NOAccess");
                Appmessage("License file is not available. Please Contact Administartor!");
            }
            
            return Global.variable.app_lic_status;
        }
    }
}
