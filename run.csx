#r "Newtonsoft.Json"

using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text; 
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers; 

public static async Task<IActionResult> Run(HttpRequest req, ILogger log)
{
    log.LogInformation("C# HTTP trigger function processed a request.");
   
    long nonce = DateTime.Now.Ticks; //Get ticks for nonce
            string APIKey = "YourAPIKey";
            string Secret = "YourSecret";

            HttpClient client = new HttpClient();
            string url = "https://www.coinspot.com.au/api/ro/my/balances";

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post
            };
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var hash = new StringBuilder();

            //Create data string in JSON format
            string signatureBaseString = "{\"nonce\":\"" + nonce.ToString() + "\"}";

            //Set POST data
            request.Content = new StringContent(signatureBaseString, Encoding.UTF8, "application/json");

            //Calculate HMAC512
            hash.Append(SHA512_ComputeHash(request.Content.ReadAsStringAsync().Result, Secret));
            //Set request headers
            request.Headers.Add("key", APIKey);
            request.Headers.Add("sign", hash.ToString());

            //Send the request
            HttpResponseMessage response = await client.SendAsync(request);

            //Response as String
            var result = response.Content.ReadAsStringAsync().Result;

    return (ActionResult)new OkObjectResult(result);

    
}

static string SHA512_ComputeHash(string text, string secretKey)
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
