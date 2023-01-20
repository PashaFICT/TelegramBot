using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ConsoleApp28;
using RestSharp;
using System.Net;
using ConsoleApp28.Model;

namespace ConsoleApp28.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodSelectedCryptoTrue()
        {
            CryptoAPI cryptoAPI = new CryptoAPI();
            foreach (var curr in ConsoleApp28.Krypt.currents)
            {
                IRestResponse response = cryptoAPI.SelectedCrypto(curr);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            }

        }
        [TestMethod]
        public void TestMethodCreateDataTrue()
        {
            CryptoAPI cryptoAPI = new CryptoAPI();
                IRestResponse response = cryptoAPI.CreateData("1653425647000", "1654030447000", "ethereum");
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
        }
        [TestMethod]
        public void TestMethodAllTokensTrue()
        {
            CryptoAPI cryptoAPI = new CryptoAPI();
                IRestResponse response = cryptoAPI.allTokens();
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
        }
        [TestMethod]
        public void TestMethodKursTrue()
        {
            CryptoAPI cryptoAPI = new CryptoAPI();
            IRestResponse response = cryptoAPI.Kurs("USD");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        }
        [TestMethod]
        public void TestMethodKryptCryptos()
        {
            Krypt crypt = new Krypt();
            string response = crypt.Cryptos();
            Assert.AreEqual(true, response.IsNormalized());

        }
        [TestMethod]
        public void TestMethodKryptAllTokens()
        {
            Krypt crypt = new Krypt();
            string response = crypt.allTokens();
            Assert.AreEqual(true, response.IsNormalized());

        }
        [TestMethod]
        public void TestMethodKryptTokenPriceTrue()
        {
            Krypt crypt = new Krypt();
            string response = crypt.tokenPrice("ethereum");
            Assert.AreEqual(true, response.IsNormalized());

        }
        [TestMethod]
        public void TestMethodKryptTokenPriceFalse()
        {
            Krypt crypt = new Krypt();
            string response = crypt.tokenPrice("etherium");
            Assert.AreEqual(true, response.Contains("На данній біржі обрана валюта не торгуєця"));

        }
        [TestMethod]
        public void TestMethodKryptChangeTrue()
        {
            Krypt crypt = new Krypt();
            bool response = crypt.cryptoChange(DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now, "test", "ethereum");
            Assert.AreEqual(true, response);

        }
        [TestMethod]
        public void TestMethodKryptChangeFalse()
        {
            Krypt crypt = new Krypt();
            bool response = crypt.cryptoChange(DateTimeOffset.Now.AddDays(+7), DateTimeOffset.Now, "test", "ethereum");
            Assert.AreEqual(false, response);

        }
        [TestMethod]
        public void TestMethodKursFinance()
        {
            Kurs kurs = new Kurs();
            string response = kurs.Finance();
            Assert.AreEqual(true, response.IsNormalized());

        }

    }

}
