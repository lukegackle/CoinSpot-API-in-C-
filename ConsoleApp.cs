
//Implementation for a local C# Console application using the CoinSpot API


using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace CoinSpot_Test
{
    class Program
    {
        static void Main(string[] args) => RunAsync().GetAwaiter().GetResult();

        private static async Task RunAsync()
        {
            long nonce = DateTime.Now.Ticks; //Get ticks for nonce
            string APIKey = "APIKEY";
            string Secret = "SECRET";



            HttpClient client = new HttpClient();
            string url = "https://www.coinspot.com.au/api/ro/my/deposits";

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var hash = new StringBuilder();

            //Create data string in JSON format
            string signatureBaseString = "{\"nonce\":\"" + nonce + "\"}";

            //Set POST data
            request.Content = new StringContent(signatureBaseString, Encoding.UTF8, "application/json");
            Console.WriteLine("Nonce in Request: " + request.Content.ReadAsStringAsync().Result);
            //Calculate HMAC512
            hash.Append(SHA512_ComputeHash(request.Content.ReadAsStringAsync().Result, Secret));

            //Set request headers
            request.Headers.Add("key", APIKey);
            request.Headers.Add("sign", hash.ToString());

            //Send the request
            HttpResponseMessage response = await client.SendAsync(request);

            //Response as String
            var result = response.Content.ReadAsStringAsync().Result;

            Console.WriteLine("Response: " + result);
            Console.WriteLine("Hash: " + hash);
            Console.WriteLine("Postdata: " + new StringContent(signatureBaseString, Encoding.UTF8, "application/json").ReadAsStringAsync().Result);
            Console.WriteLine("URL: " + url);


            Console.ReadLine();
        }

            public static string SHA512_ComputeHash(string text, string secretKey)
        {
            var hash = new StringBuilder();
            byte[] secretkeyBytes = Encoding.UTF8.GetBytes(secretKey);
            byte[] inputBytes = Encoding.UTF8.GetBytes(text);
            using (var hmac = new HMACSHA512(secretkeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes); //Update HMAC with postdata
                foreach (var theByte in hashValue)
                {
                   hash.Append(theByte.ToString("x2")); //Output to HEX
                }
            }

            return hash.ToString();
        }

    }
}
