using System.IO;
using System.Security.Cryptography;
using System.Xml;

namespace PluralSight.AWS.IAM.Native.WinUI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public sealed class Program
    {
        public static void Main(String[] args)
        {
            Console.WriteLine("** Pluralsight Course Demo - Native HTTP Calls **");
            Console.WriteLine("** Pluralsight Course Demo - Create IAM User **");

            // Timestamp
            var timeStamp = CalculateTimeStamp();

            // Create string to sign -- must be alpha ordered
            // encode path directly
            var stringToConvert = new StringBuilder();
            stringToConvert.Append("GET");
            stringToConvert.Append(Environment.NewLine);
            stringToConvert.Append("iam.amazonaws.com");
            stringToConvert.Append(Environment.NewLine);
            stringToConvert.Append("/");
            stringToConvert.Append(Environment.NewLine);
            stringToConvert.Append("AWSAccessKeyId=AKIAIBNNDYVTOMCHWX2Q");
            stringToConvert.Append("&Action=CreateUser");
            stringToConvert.Append("&Path=%2FIT%2Farchitecture%2F");
            stringToConvert.Append("&SignatureMethod=HmacSHA1");
            stringToConvert.Append("&SignatureVersion=2");
            stringToConvert.AppendFormat("&Timestamp={0}", timeStamp);
            stringToConvert.Append("&UserName=mundyNative");
            stringToConvert.Append("&Version=2010-05-08");

            const String awsPrivateKey = "VUAK/kk9V29TKVB5SeuMZTKImpAl1UvQMRY5OuEL";
            var ae = new UTF8Encoding();
            var signature = new HMACSHA1 {Key = ae.GetBytes(awsPrivateKey)};
            var bytes = ae.GetBytes(stringToConvert.ToString());
            var moreBytes = signature.ComputeHash(bytes);
            var encodedCanonical = Convert.ToBase64String(moreBytes);
            var urlEncode = HttpUtility.UrlEncode(encodedCanonical);

                var urlEncodedCanonical =
                    urlEncode
                        .Replace("+", "%20")
                        .Replace("%3d", "%3D")
                        .Replace("%2f", "%2F")
                        .Replace("%2b", "%2B");

                var iamUrl = new StringBuilder();
                iamUrl.Append("https://iam.amazonaws.com/?Action=CreateUser");
                iamUrl.Append("&AWSAccessKeyId=AKIAIBNNDYVTOMCHWX2Q");
                iamUrl.Append("&Path=%2FIT%2Farchitecture%2F");
                iamUrl.AppendFormat("&Signature={0}", urlEncodedCanonical);
                iamUrl.Append("&SignatureMethod=HmacSHA1");
                iamUrl.Append("&SignatureVersion=2");
                iamUrl.AppendFormat("&Timestamp={0}", timeStamp);
                iamUrl.Append("&UserName=mundyNative");
                iamUrl.Append("&Version=2010-05-08");

                var httpRequest = WebRequest.Create(iamUrl.ToString()) as HttpWebRequest;
                var doc = new XmlDocument();
                using (var httpResponse = httpRequest.GetResponse() as HttpWebResponse)
                {
                    var reader = new StreamReader(httpResponse.GetResponseStream());
                    var responseXml = reader.ReadToEnd();
                    doc.LoadXml(responseXml);
                }
            


            Console.WriteLine("User created");
            Console.WriteLine(doc.OuterXml);
            Console.ReadLine();
        }

        private static String CalculateTimeStamp()
        {
            var timeStamp = Uri.EscapeUriString(String.Format("{0:s}", DateTime.UtcNow));
            timeStamp = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
            var urlEncode = HttpUtility.UrlEncode(timeStamp);
            if (urlEncode != null)
            {
                timeStamp = urlEncode.Replace("%3a", "%3A");
            }

            return timeStamp;
        }
    }
}
