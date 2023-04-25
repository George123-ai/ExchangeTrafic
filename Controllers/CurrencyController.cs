//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Newtonsoft.Json.Linq;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore.Storage;
////using WebApplication1.models;
//using System.Data.SqlClient;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace ExchangeTrafic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        [HttpGet("GetRates")]
        public async Task<string> GetRates()
        {
            string output = "";
            
            const string key = "vbjeljWWPNFs5FGuVoXeefSI6jdplMS1";
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey",key);
            string url = @"https://api.apilayer.com/exchangerates_data/latest";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                output = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "error 404";
            }

            

            //Dictionary<dynamic,dynamic> data= new Dictionary<dynamic, dynamic>();
            //data.Add(output.)
            return output;
        }

        //[HttpGet("GetJsonRates")]
        //public async Task<string> GetJsonRatesAsync()
        //{
        //    string SaveInputs = string.Empty;
        //    Dictionary<string, JToken> keyValuePairs = new Dictionary<string, JToken>();
        //    HttpClient httpClient = new HttpClient();
        //    httpClient.DefaultRequestHeaders.Add("apikey", "vbjeljWWPNFs5FGuVoXeefSI6jdplMS1");

        //    string url = @"https://api.apilayer.com/exchangerates_data/latest";
        //    HttpResponseMessage response = await httpClient.GetAsync(url);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        SaveInputs = await response.Content.ReadAsStringAsync();
        //        keyValuePairs = JObject.Parse(SaveInputs)["rates"].ToObject<Dictionary<string, JToken>>();
        //        Console.WriteLine(SaveInputs);
        //    }
        //    else
        //    {
        //        return "NotFound";
        //    }
        //    return SaveInputs;
        //}
        [HttpGet("GetJsonRates")]
        public async Task<IActionResult> GetJsonRatesAsync()
        {
            string connectionString = "Data Source=MSI;Initial Catalog=Rates;Persist Security Info=True;User ID=sa;Password=goshan2034;TrustServerCertificate=true;";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string SaveInputs = string.Empty;
            Dictionary<string, decimal> keyValuePairs = new Dictionary<string, decimal>();
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("apikey", "vbjeljWWPNFs5FGuVoXeefSI6jdplMS1");

            string url = @"https://api.apilayer.com/exchangerates_data/latest";
            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                SaveInputs = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(SaveInputs);
                keyValuePairs = json["rates"].ToObject<Dictionary<string, decimal>>();

                string insertCommand = "INSERT INTO MoneyRate (Currency, Rate) VALUES (@currency, @rate)";
                SqlCommand command = new SqlCommand(insertCommand, connection);

                foreach (KeyValuePair<string, decimal> pair in keyValuePairs)
                {
                    command.Parameters.AddWithValue("@currency", pair.Key);
                    command.Parameters.AddWithValue("@rate", pair.Value);
                    command.ExecuteNonQuery();
                    command.Parameters.Clear();
                }
            }
            else
            {
                return NotFound();
            }

            connection.Close();
            return Ok();
        }
    }
}
