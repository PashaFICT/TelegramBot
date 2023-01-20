using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp28.Model
{
    public class CryptoAPI
    {
        public IRestResponse SelectedCrypto(string curr)
        {
            try
            {
                RestClient client = new RestClient($"https://api.coincap.io/v2/assets/{curr}/markets");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            return client.Execute(request);
            }
            catch
            {
                return null;
            }
        }
        public IRestResponse allTokens()
        {
        try
        {
            RestClient client = new RestClient($"https://api.coincap.io/v2/assets");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            var body = @"";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            return client.Execute(request);
        }
            catch
            {
                return null;
            }
        }
        public IRestResponse CreateData(string from, string to, string tokenName)
        {
    try
    {
        var client = new RestClient($"https://api.coincap.io/v2/candles?exchange=poloniex&interval=h8&baseId={tokenName}&quoteId=tether&start={from}&end={to}");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            return client.Execute(request);
        }
            catch
            {
                return null;
            }
        }
        public IRestResponse Kurs(string currency)
        {
            try
            {
                RestClient client = new RestClient($"https://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?valcode={currency}&json");
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                var body = @"";
                request.AddParameter("text/plain", body, ParameterType.RequestBody);
                return client.Execute(request);
            }
            catch
            {
                return null;
            }
            
        }
    }
}
