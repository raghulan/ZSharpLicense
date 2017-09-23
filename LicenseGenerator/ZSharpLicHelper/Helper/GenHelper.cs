//This is an Intelectual Property of Zcodia Technologies and Raghulan Gowthaman.
//www.zcodiatechnologies.com.au
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GV = ZSharpLicHelper.Global.variable;
using ZSharpXMLHelper;
using logit = ZSharpLogger.Log;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace ZSharpLicHelper.Helper
{
    public class GenHelper
    {
        public static string appregPath = @"DFC\Settings";
        public static string licglobalPath = @"VL\Protection";
        public static string defaultCPUID = "E2B13243-1486-44B3-B13D-29C6D6097A29";
        public static string defaultMotherBoardID = "F134FB7B-F4B3-42EF-82D3-20A189E10F53";
        public static string defaultDiskID = "B3819E45-9FDC-4360-BFD4-ADA2F8D61329";

        public static void getSettings()
        {
            try
            {
                //carete log dir
                createDir(Path.GetDirectoryName(GV.logFile));

                //if settings file exists then get the settings
                if (File.Exists(Global.variable.settingsFile))
                {
                    GV.fileStore = xmlParser.getXMLValue(Global.variable.settingsFile, "Settings", "name", "FileStore");
                    GV.logFile = xmlParser.getXMLValue(Global.variable.settingsFile, "Settings", "name", "logFile");
                    GV.logSwitch = bool.Parse(xmlParser.getXMLValue(Global.variable.settingsFile, "Settings", "name", "logSwitch"));
                }
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        public static bool checkNullString(string inputString)
        {

            bool result;
            if (inputString == null || inputString == string.Empty || string.IsNullOrEmpty(inputString) || inputString.Length == 0)
            {
                result = true;
            }
            else
            {
                result = false;
            }
            logit.logger("-----------Null Check: " + inputString + " | " + inputString.Length.ToString() + " | " + result);
            return result;
        }

        public static void createDir(string fodler)
        {
            try
            {
                if (!Directory.Exists(fodler))
                    Directory.CreateDirectory(fodler);
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }

        public static void setLogger()
        {
            try
            {
                //delete existing log file.
                if (File.Exists(GV.logFile))
                    File.Delete(GV.logFile);
                //setup log settings
                ZSharpLogger.logSetting logSettings = new ZSharpLogger.logSetting(GV.appName, null, null, GV.logFile, GV.logSwitch);

                logit.log_header(logSettings);
            }
            catch (SystemException ex)
            {
                //MessageBox.Show(ex.ToString());
            }

        }

        public static string DecodeFrom64(string encodedData)
        {
            string returnValue = null;
            try
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);
                returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (SystemException e)
            {
            }
            return returnValue;
        }

        public static string Split_csv_get_specific(string csv_value, int part)
        {
            string result;
            try
            {
                result = csv_value.Split(',')[part];

            }
            catch (IndexOutOfRangeException ex)
            {
                result = "no";
            }
            return result;

        }


        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }


        public static string GetCpuIdMotherBoardIdDiskId()
        {

            string CpuId = identifier("Win32_Processor", "UniqueId");
            if (CpuId == "") //If no UniqueID, use ProcessorID
            {
                CpuId = identifier("Win32_Processor", "ProcessorId");
                if (CpuId == "") //If no ProcessorId, use Name
                {
                    CpuId = identifier("Win32_Processor", "Name");
                    if (CpuId == "") //If no Name, use Manufacturer
                    {
                        CpuId = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    CpuId += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }

            string MotherBoardId = identifier("Win32_ComputerSystemProduct", "UUID"); 

            string DiskId = identifier("Win32_DiskDrive", "Signature");

            return CpuId + " " + MotherBoardId + " " + DiskId;

        }


        public static string cpuId()
        {
            //Uses first CPU identifier available in order of preference
            //Don't get all identifiers, as it is very time consuming
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "") //If no UniqueID, use ProcessorID
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                if (retVal == "") //If no ProcessorId, use Name
                {
                    retVal = identifier("Win32_Processor", "Name");
                    if (retVal == "") //If no Name, use Manufacturer
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    //Add clock speed for extra security
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            if (String.IsNullOrWhiteSpace(retVal))
            {
                return defaultCPUID;
            }
            else
            return retVal;
        }

        public static string motherBoardId()
        {
            string retVal =identifier("Win32_ComputerSystemProduct", "UUID");
            if (String.IsNullOrWhiteSpace(retVal))
            {
                return defaultMotherBoardID;
            }
            else
                return retVal;
        }

        public static string diskId()
        {
            string retVal = identifier("Win32_DiskDrive", "Signature");

            if (String.IsNullOrWhiteSpace(retVal))
            {
                return defaultDiskID;
            }
            else
                return retVal;
        }

        public static string machineName()
        {
           return Environment.MachineName.Trim();
        }

        public static string Ipaddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return string.Empty;
        }

        public static string GetMACAddress()
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
      
        public static string AppReadRegistry(string keyName)
        {
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\" + appregPath))
                {
                    if (Key != null)
                    {
                        return (string)Key.GetValue(keyName);
                    }
                    else
                    {
                        return string.Empty;
                    }

                }
            }
            catch (Exception ex)
            {
                return string.Empty;
                //MessageBox.Show(ex.Message);
            }
        }

        public static string LicReadRegistry(string keyName)
        {
            try
            {
                using (RegistryKey Key = Registry.CurrentUser.OpenSubKey(@"Software\" + licglobalPath))
                {
                    if (Key != null)
                    {
                        return (string)Key.GetValue(keyName);
                    }
                    else
                    {
                        return string.Empty;
                    }

                }
            }
            catch (Exception ex)
            {
                return string.Empty;
                //MessageBox.Show(ex.Message);
            }
        }
        public static bool LicWriteRegistry(string KeyName, object Value)
        {
            try
            {
                // Setting
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.CreateSubKey(@"Software\" + licglobalPath);
                sk1.SetValue(KeyName, Value);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }
        public static bool AppWriteRegistry(string KeyName, object Value)
        {
            try
            {
                // Setting
                RegistryKey rk = Registry.CurrentUser;
                RegistryKey sk1 = rk.CreateSubKey(@"Software\" + appregPath);
                sk1.SetValue(KeyName, Value);
                return true;
            }
            catch (System.Exception e)
            {
                return false;
            }
        }

    }
}
