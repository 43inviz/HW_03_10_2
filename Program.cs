using System.Net;

namespace HW_03_10_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> urls = new List<string>()
            {
                "https://www.youtube.com/",
                "https://github.com/"
            };

            int newTask = Environment.ProcessorCount;
            CancellationTokenSource cts = new CancellationTokenSource();

            Task[] tasks = new Task[newTask];

            for (int i = 0; i < newTask; i++)
            {
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    while (urls.Count > 0 && !cts.Token.IsCancellationRequested)
                    {
                        string url = null;

                        lock (urls)
                        {
                            if (urls.Count > 0)
                            {
                                url = urls[0];
                                urls.RemoveAt(0);
                            }
                        }


                        if (url != null)
                        {
                            try
                            {
                                Console.WriteLine("Downloading {0}", url);
                                WebClient client = new WebClient();
                                client.DownloadFile(url, $"{new DomainCutter().CleanURL(url)}.txt");
                                Console.WriteLine("Succesfully uploaded - {0}", url);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error downloading {0} : {1}", url, ex.Message);
                            }
                        }
                    }
                }, cts.Token);

                Console.WriteLine("Press enter to cancel operation ...");
                Console.ReadLine();

                cts.Cancel();

                Task.WaitAll(tasks);
            }
        }



        public class DomainCutter
        {
            public string CleanURL(string url)
            {
                string URL = "";
                string CleanUrl = "";
                if (URL.Contains("https://"))
                {
                    CleanUrl = URL.Substring(8);
                    URL = "https://" + url;
                }
                else if (url.Contains("http://") == false && url.Contains("https://") == false)
                {
                    URL = "http://" + url;
                }
                else
                {
                    URL = url;
                }
                string clearurl = URL.Substring(7);
                if (clearurl.Contains("/"))
                {
                    string trimed = clearurl.Trim();
                    clearurl = "http://" + trimed.Substring(0, trimed.IndexOf('/'));
                }
                else
                {
                    return "http://" + clearurl;
                }


                return clearurl.Substring(7);
            }
        }
    }
}
