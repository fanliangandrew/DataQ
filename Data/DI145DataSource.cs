using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dataq.Simple;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Security.Cryptography;

namespace WaveApplication.Data
{
    class DI145DataSource
    {
        DataqDevice[] DataqDeviceArray;
        //array to retain the channel configuration defined via the GUI
        protected DataqDevice DI_145;

        protected FileStream fsWrite;
        protected FileStream fsOpen;

        protected StreamWriter sw;
        protected StreamReader sr;
        protected string filePath= @"C:\myfiles\DI145Data.csv";
        protected string encryptPath = @"C:\myfiles\EncryptData.csv";
        protected string decryptPath = @"C:\myfiles\DecryptData.csv";
        protected string sKey = "abcdefgh";
        protected bool isFileAvaible = false;
        bool isStop = false;
        protected int columnCount = 8;
        protected int count;
        protected readonly char[] _buffer = new char[30];

        protected void EncryptFile(string sInputFilename, string sOutputFilename, string sKey)
        {
            FileStream fsInput = new FileStream(sInputFilename,
              FileMode.Open,  FileAccess.Read);

            FileStream fsEncrypted = new FileStream(sOutputFilename,
               FileMode.Create, FileAccess.Write);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();
            CryptoStream cryptostream = new CryptoStream(fsEncrypted,
               desencrypt, CryptoStreamMode.Write);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Close();
            fsInput.Close();
            fsEncrypted.Close();
        }

        protected void DecryptFile(string sInputFilename, string sOutputFilename, string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();
            //A 64 bit key and IV is required for this provider.
            //Set secret key For DES algorithm.
            DES.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            //Set initialization vector.
            DES.IV = ASCIIEncoding.ASCII.GetBytes(sKey);

            //Create a file stream to read the encrypted file back.
            FileStream fsread = new FileStream(sInputFilename,
               FileMode.Open, FileAccess.Read);
            //Create a DES decryptor from the DES instance.
            ICryptoTransform desdecrypt = DES.CreateDecryptor();
            //Create crypto stream set to read and do a 
            //DES decryption transform on incoming bytes.
            CryptoStream cryptostreamDecr = new CryptoStream(fsread,
               desdecrypt, CryptoStreamMode.Read);
            //Print the contents of the decrypted file.
            StreamWriter fsDecrypted = new StreamWriter(sOutputFilename);
            fsDecrypted.Write(new StreamReader(cryptostreamDecr).ReadToEnd());
            fsDecrypted.Flush();
            fsDecrypted.Close();
        }



        protected void initDI145()
        {
            count = columnCount;
            detectFileAviable();
            if(isFileAvaible)
            {
                DecryptFile(encryptPath, decryptPath, sKey);
                fsOpen = new FileStream(decryptPath, FileMode.Open);
                sr = new StreamReader(fsOpen, Encoding.Default);
            }
            else
            {
                DataqDeviceArray = Discovery.DiscoverAllDevices();
                if (DataqDeviceArray.Length == 0)
                {
                    MessageBox.Show("No compatible DATAQ Instruments devices found.",
                        "Missing DATAQ Instruments hardware", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                else
                {
                    DI_145 = DataqDeviceArray[0];
                    DI_145.Connect();
                }

                if (DI_145.Model != "DI-145")
                {
                    MessageBox.Show("This program works only with DATAQ Instruments model DI-145",
                        "Incompatible Device (" + DI_145.Model + ")", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
                Dataq.Range<double> Range = new Dataq.Range<double>();
                DI_145.ChannelArray[0].Enabled = true;

                DI_145.NewDataMinimum = 1;
                //set the number of scans to acquire before the NewData event fires
                DI_145.Start();
                //start scanning
               fsWrite = new FileStream(filePath, FileMode.Create);
               sw = new StreamWriter(fsWrite, Encoding.Default);
            }
            
        }

        protected void detectFileAviable()
        {
            long size = 0;
            FileInfo fi = new FileInfo(encryptPath);
            try
            {
                size = fi.Length;
            }
            catch(Exception e)
            {
                size = 0;
            }
            if(size != 0)
            {
                isFileAvaible = true;
            }
        }

        protected float getDI145Data()
        {
            if(isFileAvaible)
            {
                float? returnValue = getDI145DataFromFile();
                return returnValue == null?0f:(float)returnValue;
            }
            else
            {
                return getDI145DataFromDevice();
            }
        }

        protected float? getDI145DataFromFile()
        {   
            int c;
            while (true)
            {
                c = sr.Read();
                if (c != ' ') break;
            }
            if (c <0)
            {
                stopWork();
                isStop = true;
                Console.WriteLine("stop");
                Thread.CurrentThread.Abort();
                return null;
            }           
           else
            {
                _buffer[0] = (char)c;
                int i;
                for (i = 1; ; i++)
                {
                    c = sr.Read();
                    if (c == ',' || c == '\r' || c == '\n' || c==-1)
                        break;
                    _buffer[i] = (char)c;
                
                }
                string data = new string(_buffer, 0, i);
                if(c!=-1)
                {
                    Console.WriteLine("data= " + data);
                   // return decrypt(data);
                   float  f = (float)Double.Parse(data);
                    return float.Parse(f.ToString("#0.00"));
                }
                else
                {
                    return null;
                }
            }

        }
        /*
        protected string encrypt(double data)
        {
            float f = (float)data;
             f = f * 12.3f + 12.3f;
            return String.Format("{0:N2}", f);
        }

        protected float decrypt(string data)
        {
            float f = (float)Double.Parse(data);
            f = (f - 12.3f) / 12.3f;
            f = float.Parse(f.ToString("#0.00"));
            return f;
        }*/

        protected float getDI145DataFromDevice()
        {
            int Scans = 0;
            while(Scans==0)
            {
                Scans = DI_145.NumberOfScansAvailable;
            }

            short Channels = (short)DI_145.NumberOfChannelsEnabled;

            double[] DI_145_Data = new double[1];
            DI_145_Data[0] = 0;
            //Move data into temporary array

            DI_145.GetInterleavedScaledData(DI_145_Data, 0, Scans);
            float data = float.Parse(DI_145_Data[0].ToString("#0.00"));
            if (count>0)
            {
                // sw.Write(encrypt(DI_145_Data[0])+",");
                sw.Write(data + ",");
                count--;
            }
            else
            {
                //sw.Write(encrypt(DI_145_Data[0]) + "\r\n") ;
                sw.Write(data + "\r\n");
                count = columnCount;
            }
            Console.WriteLine(data);
            return data;
        }

        protected void stopWork()
        {
            if(isFileAvaible)
            {
                fsOpen.Close();
                sr.Close();
            //    File.Delete(decryptPath);
            }
            else
            {
                DI_145.Stop();
                fsWrite.Close();
                EncryptFile(filePath, encryptPath, sKey);
             //   File.Delete(filePath);
            }
            
          
        }
    }
}
