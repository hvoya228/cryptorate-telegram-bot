using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptocurrencyRateBot
{
    public class CryptoRateController
    {
        public async Task<string> GetPingInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = "https://api.coingecko.com/api/v3/ping";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        return "Server connected";
                    }
                    else
                    {
                        return "Server is not responding";
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> GetCryptoRate(string cryptoName, string currencyName)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string apiUrl = $"https://api.coingecko.com/api/v3/simple/price?ids={cryptoName}&vs_currencies={currencyName}";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        return json;
                    }
                    else
                    {
                        return response.StatusCode.ToString();
                    }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

        }

    }
}
