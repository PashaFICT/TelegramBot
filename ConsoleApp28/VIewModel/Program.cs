using System;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.InputFiles;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Timer = System.Threading.Timer;
using Prometheus;
using System.Collections.Generic;

namespace ConsoleApp28
{
    public class Program
    {
        private static readonly Counter KursTick =
    Metrics.CreateCounter("kurs_ticks_total", "Count kurs");
        private static readonly Counter KryptoTick =
    Metrics.CreateCounter("krypto_ticks_total", "Count krypto");
        private static readonly Counter ReminderTick =
    Metrics.CreateCounter("reminder_ticks_total", "Count reminder");
        private static readonly Counter ChartK =
   Metrics.CreateCounter("krypto_chart_ticks_total", "Count krypto chart");
        static DB db = new DB();
        private static Krypt krypt = new Krypt();
        private static Kurs kurs = new Kurs();
        public static readonly TelegramBotClient Bot =
            new TelegramBotClient(Properties.Resources.tokenId);

        public static void Main(string[] args)
        {
            var server = new MetricServer(hostname: "localhost", port: 1234);

            server.Start();

            int num = 0;
            TimerCallback tm = new TimerCallback(Timerr);
            Timer timer = new Timer(Timerr, num, 0, 58000);
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnMessageEdited += Bot_OnMessage;
            Bot.StartReceiving();
            Console.ReadLine();
            Bot.StopReceiving();
        }


        public static void Timerr(object obj)
        {
            List<long> ids = db.CheckTimeReminder();
            foreach (var id in ids)
            {
                Bot.SendTextMessageAsync(id, krypt.Cryptos());
                db.UpdateDateLastReminder(id);
            }
        }


        public static async void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            db.CheckUserID(e.Message.Chat.Id, e.Message.Chat.Username);
            if (e.Message.Text == "/start")
            {
                var markup = new ReplyKeyboardMarkup();
                markup.Keyboard = new KeyboardButton[][]
                {
                        new KeyboardButton[]
                        {
                            new KeyboardButton("kurs"),
                            new KeyboardButton("krypto"),

                        },

                        new KeyboardButton[]
                        {
                            new KeyboardButton("chart eth week"),
                            new KeyboardButton("chart btc week"),
                            new KeyboardButton("chart bnb week"),
                        },

                        new KeyboardButton[]
                        {
                            new KeyboardButton("individual chart"),
                            new KeyboardButton("all tokens"),

                        },

                        new KeyboardButton[]
                        {
                            new KeyboardButton("reminder"),
                            new KeyboardButton("off"),
                        }
                };
                markup.ResizeKeyboard = true;
                markup.Selective = false;
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Привіт", replyMarkup: markup);
            }
            else if (e.Message.Text == "kurs")
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, kurs.Finance());
                KursTick.Inc();
            }
            else if (e.Message.Text == "krypto")
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, krypt.Cryptos());
                KryptoTick.Inc();
            }
            else if (e.Message.Text == "chart btc week")
            {
                krypt.cryptoChange(DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now, e.Message.Chat.Id.ToString(), "bitcoin");

                string pathToFile = $@"d:\{e.Message.Chat.Id.ToString()}.png";
                using (var stream = File.OpenRead(pathToFile))
                {
                    InputOnlineFile iof = new InputOnlineFile(stream);
                    iof.FileName = $"{e.Message.Chat.Id.ToString()}.png";
                    var send = await Bot.SendDocumentAsync(e.Message.Chat.Id, iof, "Тижневий графік bitcoin");
                }
                KryptoTick.Inc();
            }
            else if (e.Message.Text == "chart eth week")
            {
                krypt.cryptoChange(DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now, e.Message.Chat.Id.ToString(), "ethereum");

                string pathToFile = $@"d:\{e.Message.Chat.Id.ToString()}.png";
                using (var stream = File.OpenRead(pathToFile))
                {
                    InputOnlineFile iof = new InputOnlineFile(stream);
                    iof.FileName = $"{e.Message.Chat.Id.ToString()}.png";
                    var send = await Bot.SendDocumentAsync(e.Message.Chat.Id, iof, "Тижневий графік ethereum");
                }
                KryptoTick.Inc();
            }
            else if (e.Message.Text == "chart bnb week")
            {
                krypt.cryptoChange(DateTimeOffset.Now.AddDays(-7), DateTimeOffset.Now, e.Message.Chat.Id.ToString(), "binance-coin");

                string pathToFile = $@"d:\{e.Message.Chat.Id.ToString()}.png";
                using (var stream = File.OpenRead(pathToFile))
                {
                    InputOnlineFile iof = new InputOnlineFile(stream);
                    iof.FileName = $"{e.Message.Chat.Id.ToString()}.png";
                    var send = await Bot.SendDocumentAsync(e.Message.Chat.Id, iof, "Тижневий графік binance-coin");
                }
                KryptoTick.Inc();
            }
            else if (e.Message.Text == "individual chart")
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Введіть текст в форматі *individual chart 2022-04-12/12:00 2022-04-18/12:00 ethereum*");
            }
            else if (e.Message.Text == "all tokens")
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, krypt.allTokens());
            }
            else if (e.Message.Text.IndexOf("individual chart") == 0)
            {
                string[] words = e.Message.Text.Split(new char[] { ' ' });
                DateTimeOffset fromDate = DateTimeOffset.ParseExact(words[2], "yyyy-MM-dd/HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                DateTimeOffset toDate = DateTimeOffset.ParseExact(words[3], "yyyy-MM-dd/HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                if (krypt.cryptoChange(fromDate, toDate, e.Message.Chat.Id.ToString(), words[4]))
                {
                    string pathToFile = $@"d:\{e.Message.Chat.Id.ToString()}.png";
                    using (var stream = File.OpenRead(pathToFile))
                    {
                        InputOnlineFile iof = new InputOnlineFile(stream);
                        iof.FileName = $"{e.Message.Chat.Id.ToString()}.png";
                        var send = Bot.SendDocumentAsync(e.Message.Chat.Id, iof, $"Графік за обраний період {words[4]}");
                    }
                }
                else
                {
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, "Виникла помилка");
                }
                

                
            }
            else if (e.Message.Text == "reminder")
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id,
                    "Для створення щоденного оповіщення курсу криптовалют використайте формат hh:mm, наприклад  'reminder 19:12'");
            }
            else if (e.Message.Text == "off")
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, db.CheckReminder(e.Message.Chat.Id));
            }
            else if (e.Message.Text.IndexOf("reminder ") == 0)
            {
                ReminderTick.Inc();


                DateTime dateTime;
                string[] words = e.Message.Text.Split(new char[] { ' ' });
                string time = words[1];
                Regex r = new Regex(@"^(\d{2}:\d{2})$");
                Match m = r.Match(time);
                if (m.Success)
                {
                    
                    dateTime = Convert.ToDateTime(time);
                    Bot.SendTextMessageAsync(e.Message.Chat.Id, db.CreateReminder(e.Message.Chat.Id, dateTime));
                }

                }
            else if (krypt.allTokens().Contains(e.Message.Text))
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, krypt.tokenPrice(e.Message.Text.Remove(0,1)));
            }
            else
            {
                Bot.SendTextMessageAsync(e.Message.Chat.Id, "Введіть команду '/start'");
            }
            }

        }
    }
