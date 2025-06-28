using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics.Metrics;
using System.Transactions;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
namespace Sandbox
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {

            string base64 = "czl1ZURoYjFKeGRaV1JoRFRLZzF1eVhUY0lQc2MyZ1FaM1hXMmJqRA==";
            byte[] data = Convert.FromBase64String(base64);
            string decoded = Encoding.UTF8.GetString(data);
            Console.WriteLine(decoded);

        }
    }
}
