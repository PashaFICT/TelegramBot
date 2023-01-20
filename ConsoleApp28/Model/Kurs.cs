using System.Xml.Serialization;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using ConsoleApp28.Model;

namespace ConsoleApp28
{
    public class Currency
    {
        public string r030 { get; set; }
        public string txt { get; set; }
        public string rate { get; set; }
        public string cc { get; set; }
        public string exchangedate { get; set; }
    }

    public class Kurs
    {
        List<Currency> price = new List<Currency>();
        CryptoAPI cryptoAPI = new CryptoAPI();
        public static List<string> currencys = new List<string>() { "USD", "EUR", "GBP" };

        public string Finance()
        {
            string res = "Bank.gov:\n";
            foreach (var currency in currencys)
            {
                IRestResponse response = cryptoAPI.Kurs(currency);
                price = JsonConvert.DeserializeObject<List<Currency>>(response.Content);
                res += currency + "\n" + price.First().rate + " грн. \r\n";
            }
            return res;
        }

    }
}
