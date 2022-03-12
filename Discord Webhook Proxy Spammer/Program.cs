using System;
using Leaf.xNet;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;


namespace Discord_Webhook_Proxy_Spammer
{
    class Program
    {
       

            public static string url;
            public static string msg;
            public static int type;

            private static Queue<string> proxies = new Queue<string>();

            static void Main(string[] args)
            {


                Console.Write("Your webhook URL: ");
                url = Console.ReadLine();

                Console.Write("Your message: ");
                msg = Console.ReadLine();

            reproxy:

                Console.Write("Drag your proxies file: ");
                string path = Console.ReadLine().Replace("\"", string.Empty);

                try
                {
                    var lines = File.ReadAllLines(path);

                    Parallel.ForEach(lines, line =>
                    {
                        lock (proxies)
                        {
                            proxies.Enqueue(line);
                        }
                    });
                }
                catch
                {
                    Console.WriteLine("Looks like there's something wrong with the input!");
                    Thread.Sleep(2500);
                    Console.Clear();
                    goto reproxy;
                }




            retry:

                Console.WriteLine("Proxy type: (Type a num)");
                Console.WriteLine("1 - HTTP");
                Console.WriteLine("2 - SOCKS4");
                Console.WriteLine("3 - SOCKS5");
                Console.Write("Input: ");

                type = 0;

                try
                {
                    type = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Looks like there's something wrong with the input!");
                    Thread.Sleep(2500);
                    Console.Clear();
                    goto retry;

                }

            rethread:

                Console.Write("Threads to use (blank for 50): ");

                int threads = 0;

                try
                {
                    threads = int.Parse(Console.ReadLine());
                }
                catch
                {
                    Console.WriteLine("Looks like there's something wrong with the input!");
                    Thread.Sleep(2500);
                    Console.Clear();
                    goto rethread;

                }

                for (int i = 0; i < threads; i++)
                {
                    var t = new Thread(() => Worker());
                    t.Start();
                }



                while (true)
                {

                }







            }

            private static void Worker()
            {
                while (true)
                {
                    string proxy;
                    try
                    {
                        lock (proxies)
                        {
                            proxy = proxies.Dequeue();
                        }
                    }
                    catch
                    {
                        break;
                    }

                tryagain:
                    try
                    {
                        while (true)
                        {
                            SendWebhook(proxy);
                            Console.WriteLine(proxy + " sent");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("canary.discord.com"))
                        {
                            Console.WriteLine(proxy + " dead " + ex.Message);
                        }
                        else
                        {
                            goto tryagain;
                        }

                    }



                }




            }


            private static void SendWebhook(string proxy)
            {







                HttpRequest client = new HttpRequest();


                client.Proxy = GP(type, proxy);

                client.Post(url, "{\"content\":\"" + msg + "\"}", "application/json");






            }



            private static ProxyClient GP(int type, string proxy)
            {


                switch (type)
                {

                    case 1:
                        return HttpProxyClient.Parse(proxy);


                    case 2:
                        return Socks4ProxyClient.Parse(proxy);


                    case 3:
                        return Socks5ProxyClient.Parse(proxy);


                    default:
                        throw new Exception("Look's like there is no proxy type given?");


                }



            }



        
    }
}
