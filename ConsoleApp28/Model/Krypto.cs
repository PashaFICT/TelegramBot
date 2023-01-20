using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;
using ConsoleApp28.Model;

namespace ConsoleApp28
{
    public class Crypto
    {
        public string exchangeId { get; set; }
        public string baseId { get; set; }
        public string quoteId { get; set; }
        public string baseSymbol { get; set; }
        public string quoteSymbol { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string volumePercent { get; set; }


    }
    public class Token
    {
        public string id { get; set; }
        public string rank { get; set; }
        public string symbol { get; set; }
        public string name { get; set; }
        public string suply { get; set; }
        public string maxSuply { get; set; }
        public string marketCapUsd { get; set; }
        public string volumeUsd24Hr { get; set; }
        public string priceUsd { get; set; }
        public string changePercent24Hr { get; set; }
        public string vwap24Hr { get; set; }


    }
    public class CryptoChart
    {
        public string open { get; set; }
        public string high { get; set; }
        public string low { get; set; }
        public string close { get; set; }
        public string volume { get; set; }
        public string period { get; set; }
    }
    public class ListCrypto
    {
        public List<Crypto> data { get; set; }
    }
    public class ListChart
    {
        public List<CryptoChart> data { get; set; }
    }
    public class ListToken
    {
        public List<Token> data { get; set; }
    }
    public class Krypt
    {
        public static List<string> rialtos = new List<string>() { "Binance", "WhiteBIT", "Gate" };
        public static List<string> currents = new List<string>() { "bitcoin", "ethereum", "binance-coin" };
        ListCrypto cryptos = new ListCrypto();
        ListToken tokens = new ListToken();
        ListChart cryptoChart = new ListChart();
        private CryptoAPI cryptoAPI = new CryptoAPI();

        public string Cryptos()
        {
            string res = "";
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (var rial in rialtos)
            {
                res += rial + ":\n";
                Dictionary<string, string> selectedCoins = new Dictionary<string, string>();
                foreach (var curr in currents)
                {                    
                    IRestResponse response = cryptoAPI.SelectedCrypto(curr);
                    cryptos = JsonConvert.DeserializeObject<ListCrypto>(response.Content);
               
                    string usdt = cryptos.data.First(d => (d.exchangeId == rial) && (d.baseId == curr) && (d.quoteSymbol == "USDT")).priceUsd;
                    selectedCoins.Add(curr, usdt.Substring(0, usdt.LastIndexOf('.') + 3));
                    res += curr + ' ' + usdt.Substring(0, usdt.LastIndexOf('.') + 3) + " $;\n";


                }
                res += "\n";
                result.Add(selectedCoins);
            }
            
            return res;
        }
        public string allTokens()
        {
            string res = "";
            IRestResponse response = cryptoAPI.allTokens();
            tokens = JsonConvert.DeserializeObject<ListToken>(response.Content);

            foreach (var item in tokens.data)
            {
                res += "/" + item.id + "\n";
            }
            return res;
        }
        public string tokenPrice(string curr)
        {
            string res = "";
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (var rial in rialtos)
            {
                res += rial + ":\n";
                Dictionary<string, string> selectedCoins = new Dictionary<string, string>();
                try
                {
                    IRestResponse response = cryptoAPI.SelectedCrypto(curr);
                    cryptos = JsonConvert.DeserializeObject<ListCrypto>(response.Content);

                    string usdt = cryptos.data.First(d => (d.exchangeId == rial) && (d.baseId == curr) && (d.quoteSymbol == "USDT")).priceUsd;
                    selectedCoins.Add(curr, usdt.Substring(0, usdt.LastIndexOf('.') + 3));
                    res += curr + ' ' + usdt.Substring(0, usdt.LastIndexOf('.') + 3) + " $;\n";
                }
                    catch (Exception ex)
                {
                    res += "На данній біржі обрана валюта не торгуєця";
                }

                res += "\n";
                result.Add(selectedCoins);
            }
            return res;
        }
        public bool cryptoChange(DateTimeOffset from, DateTimeOffset to, string userID, string tokenName)
        {
            try
            {
                string fromMS = ((long)from.ToUnixTimeSeconds()).ToString() + "000";
                string toMS = ((long)to.ToUnixTimeSeconds()).ToString() + "000";
                Draw(CreateData(fromMS, toMS, tokenName), userID);
                return true;
            }
            catch
            {
                return false;
            }
            
        }

        private DataSet CreateData(string from, string to, string tokenName)
        {
            IRestResponse response = cryptoAPI.CreateData(from, to, tokenName);
            cryptoChart = JsonConvert.DeserializeObject<ListChart>(response.Content);


            DataSet dataSet = new DataSet();
            DataTable dt = new DataTable();
            dt.TableName = "Tab";
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Price", typeof(int));
            foreach (var candle in cryptoChart.data)
            {
                DataRow r1 = dt.NewRow();
                r1[0] = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(candle.period.Remove(candle.period.Length - 3))).ToString("MM/dd/yyyy HH:mm");
                r1[1] = Convert.ToDecimal(candle.close.Replace('.', ','));
                dt.Rows.Add(r1);
            }
            dataSet.Tables.Add(dt);
            return dataSet;
        }
        private void Draw(DataSet dataSet, string userID)
        {
            Chart chart = new Chart();
            chart.DataSource = dataSet.Tables[0];
            chart.Width = 600;
            chart.Height = 350;
            //create serie...
            Series serie1 = new Series();
            serie1.Name = "Serie1";
            serie1.Color = Color.Red;
            serie1.BorderColor = Color.FromArgb(164, 164, 164);
            serie1.ChartType = SeriesChartType.Line;
            serie1.BorderDashStyle = ChartDashStyle.Solid;
            serie1.BorderWidth = 1;
            serie1.ShadowColor = Color.FromArgb(128, 128, 128);
            serie1.ShadowOffset = 1;
            serie1.IsValueShownAsLabel = true;
            serie1.XValueMember = "Date";
            serie1.YValueMembers = "Price";
            serie1.Font = new Font("Tahoma", 8.0f);
            serie1.BackSecondaryColor = Color.FromArgb(0, 102, 153);
            serie1.LabelForeColor = Color.FromArgb(100, 100, 100);
            chart.Series.Add(serie1);
            //create chartareas...
            ChartArea ca = new ChartArea();
            ca.Name = "ChartArea1";
            ca.BackColor = Color.White;
            ca.BorderColor = Color.FromArgb(26, 59, 105);
            ca.BorderWidth = 0;
            ca.BorderDashStyle = ChartDashStyle.Solid;
            ca.AxisX = new Axis();
            ca.AxisY = new Axis();
            ca.AxisY.Minimum = dataSet.Tables["Tab"].AsEnumerable()
                  .Min(r => r.Field<int>("Price"))*0.99;
            ca.AxisY.Maximum = dataSet.Tables["Tab"].AsEnumerable()
                  .Max(r => r.Field<int>("Price")) * 1.01;
            chart.ChartAreas.Add(ca);
            //databind...
            chart.DataBind();
            //save result...
            chart.SaveImage($@"d:\{userID}.png", ChartImageFormat.Png);
        }
    }
}
