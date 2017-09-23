using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ZSharpLicHelper.Helper
{
   public class Encryption
    {
        public static XmlDocument getXmlData(string path)
        {
            XmlDocument doc;
            RijndaelManaged key = null;
            try
            {
                key = new RijndaelManaged();
                doc = new XmlDocument();
                doc.Load(path);
                Decrypt(doc);
                // Convert.ToDateTime(path, CultureInfo.CurrentCulture);
                //doc.Save(path);
            }
            catch (Exception ex)
            {
                throw;
            }
            return doc;
        }

        public static XmlDocument EncryptXmlData(string path)
        {
            XmlDocument doc;
            RijndaelManaged key = null;
            try
            {
                key = new RijndaelManaged();
                doc = new XmlDocument();
                doc.Load(path);
                Encrypt(doc);
                doc.Save(path);
            }
            catch (Exception ex)
            {
                throw;
            }
            return doc;
        }

        private static void Decrypt(XmlDocument doc)
        {
            try
            {
                if (doc == null)
                    throw new ArgumentNullException("Doc");
                SymmetricAlgorithm symAlgo = new RijndaelManaged();
                byte[] salt = Encoding.ASCII.GetBytes("This is my salt");
                Rfc2898DeriveBytes theKey = new Rfc2898DeriveBytes("myclass", salt);
                symAlgo.Key = theKey.GetBytes(symAlgo.KeySize / 8);
                symAlgo.IV = theKey.GetBytes(symAlgo.BlockSize / 8);
                XmlElement encryptedElement = doc.DocumentElement;
                if (encryptedElement == null)
                {
                    throw new XmlException("The EncryptedData element was not found.");
                }
                EncryptedData edElement = new EncryptedData();
                edElement.LoadXml(encryptedElement);
                EncryptedXml exml = new EncryptedXml();
                byte[] rgbOutput = exml.DecryptData(edElement, symAlgo);
                exml.ReplaceData(encryptedElement, rgbOutput);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static void Encrypt(XmlDocument doc)
        {
            SymmetricAlgorithm symAlgo = new RijndaelManaged();
            byte[] salt = Encoding.ASCII.GetBytes("This is my salt");
            Rfc2898DeriveBytes theKey = new Rfc2898DeriveBytes("myclass", salt);
            symAlgo.Key = theKey.GetBytes(symAlgo.KeySize / 8);
            symAlgo.IV = theKey.GetBytes(symAlgo.BlockSize / 8);
            if (doc == null)
                throw new ArgumentNullException("Doc");
            if (symAlgo == null)
                throw new ArgumentNullException("Alg");
            XmlElement elementToEncrypt = doc.DocumentElement;
            if (elementToEncrypt == null)
            {
                throw new XmlException("The specified element was not found");

            }
            EncryptedXml eXml = new EncryptedXml();
            byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, symAlgo, false);
            EncryptedData edElement = new EncryptedData();
            edElement.Type = EncryptedXml.XmlEncElementUrl;
            string encryptionMethod = null;
            if (symAlgo is TripleDES)
            {
                encryptionMethod = EncryptedXml.XmlEncTripleDESUrl;
            }
            else if (symAlgo is DES)
            {
                encryptionMethod = EncryptedXml.XmlEncDESUrl;
            }
            if (symAlgo is Rijndael)
            {
                switch (symAlgo.KeySize)
                {
                    case 128:
                        encryptionMethod = EncryptedXml.XmlEncAES128Url;
                        break;
                    case 192:
                        encryptionMethod = EncryptedXml.XmlEncAES192Url;
                        break;
                    case 256:
                        encryptionMethod = EncryptedXml.XmlEncAES256Url;
                        break;
                    default:
                        // do the defalut action
                        break;
                }
            }
            else
            {
                throw new CryptographicException("The specified algorithm is not supported for XML Encryption.");
            }
            edElement.EncryptionMethod = new EncryptionMethod(encryptionMethod);
            edElement.CipherData.CipherValue = encryptedElement;
            EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);
        }
        public static string Encrypt(string toEncrypt, bool useHashing =true)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "This is my salt";
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string Decrypt(string cipherString, bool useHashing=true)
        {
            byte[] keyArray;
            //get the byte code of the string

            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            string key = "This is my salt";

            if (useHashing)
            {
                //if hashing was used get the hash code with regards to your key
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //release any resource held by the MD5CryptoServiceProvider

                hashmd5.Clear();
            }
            else
            {
                //if hashing was not implemented get the byte code of the key
                keyArray = UTF8Encoding.UTF8.GetBytes(key);
            }

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes. 
            //We choose ECB(Electronic code Book)

            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(
                                 toEncryptArray, 0, toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor                
            tdes.Clear();
            //return the Clear decrypted TEXT
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
    }
}
