using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace StealTraffic
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0].Equals("SAVE_MAC"))
            {
                var listMac = new List<string>();

                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    listMac.Add(nic.GetPhysicalAddress().ToString());
                }

                File.WriteAllLines(Path.Combine(Environment.CurrentDirectory, "mac.txt"), listMac);
            }
            else 
            {
                StartThief("references.txt");
            }
        }

        static void StartThief(string refFile) 
        {
            WebClient web = new WebClient();

            if(File.Exists((Path.Combine(Environment.CurrentDirectory, "mac.txt")))) 
            {
                string[] banMacs = File.ReadAllLines("mac.txt");

                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (banMacs.Contains(nic.GetPhysicalAddress().ToString())) 
                    {
                        Console.WriteLine("This is my mac");
                        Environment.Exit(0);
                    }
                }
            }
            

            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "Content")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "Content"));

            var r = new Random( DateTime.Now.Millisecond );

            foreach (var reference in File.ReadAllLines(refFile))
            {
                web.Proxy.Credentials = CredentialCache.DefaultCredentials;
                web.Credentials = CredentialCache.DefaultCredentials;

                byte[] data = web.DownloadData(reference);
                var ct = web.ResponseHeaders[HttpResponseHeader.ContentType];

                File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, "Content", r.Next(10000).ToString() + "." + ct.Split('/')[1]), data);
            }

            web.Dispose();
        }
    }
}
