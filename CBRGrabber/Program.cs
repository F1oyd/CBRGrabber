using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CBRGrabber
{
    class Program
    {
        static void Main()
        {
            var date = DateTime.Today;
            while (true)
            {
                Console.Write(@"Введите дату (для текущей даты оставте поле пустым): ");
                var inputDate = Console.ReadLine();
                try
                {
                    if (!string.IsNullOrWhiteSpace(inputDate)) date = Convert.ToDateTime(inputDate);
                    break;
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("Извлекаю данные за {0:d}", date);

            if (date.Date >= new DateTime(2015, 10, 30))
                GetRepoListNew(date);
            else GetRepoList(date);
            //GetLombardList(date);
            Console.WriteLine();
            Console.Write("Нажмите Enter чтобы выйти...");
            Console.ReadLine();
        }

        static void GetRepoListNew(DateTime date)
        {
            var url = new Uri(@"http://cbr.ru/hd_base/Default.aspx?Prtid=infodirectreporub");
            var postFieldsList = new[]
            {
                "__EVENTTARGET=",
                "__EVENTARGUMENT=",
                string.Format("ctl00$ContentPlaceHolder1$UC_itm_20867$ToDate={0:d}", date),
                "ctl00$ContentPlaceHolder1$UC_itm_20867$doFilterGibrid=Получить"
            };
            var postBytes = Encoding.UTF8.GetBytes(string.Join("&", postFieldsList));

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            request.Method = WebRequestMethods.Http.Post;
            request.ServicePoint.Expect100Continue = false;
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;

            using (var postStream = request.GetRequestStream())
                postStream.Write(postBytes, 0, postBytes.Length);

            string responseFromServer;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Console.WriteLine("Статус подключения к серверу: {0}", response.StatusDescription);
                using (var responseStream = response.GetResponseStream())
                    responseFromServer = new StreamReader(responseStream).ReadToEnd();
            }

            const string pattern = @"<tr>[^<>]*<td>(?<isin>[^<>]*)<\/td>[^<>]*<td>(?<regnum>[^<>]*)<\/td>[^<>]*<td>(?<issuer>[^<>]*)<\/td>[^<>]*<td>(?<maturity>[^<>]*)<\/td>[^<>]*<td>(?<price>[^<>]*)<\/td>[^<>]*<td>(?<act1>[^<>]*)<\/td>[^<>]*<td>(?<min1>[^<>]*)<\/td>[^<>]*<td>(?<max1>[^<>]*)<\/td>[^<>]*<td>(?<act7>[^<>]*)<\/td>[^<>]*<td>(?<min7>[^<>]*)<\/td>[^<>]*<td>(?<max7>[^<>]*)<\/td>[^<>]*<\/tr>";
            var matches = Regex.Matches(responseFromServer, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                var fileName = string.Format("{0:d} РЕПО.txt", date);
                File.WriteAllText(fileName, string.Empty, Encoding.GetEncoding("cp866"));
                foreach (Match match in matches)
                {
                    var line = string.Empty;
                    for (var i = 1; i <= 20; i++)
                    {
                        line += (i > 4) ? match.Groups[i].Value.Trim().Replace("—", "").Replace(" ", "") : match.Groups[i].Value.Trim();
                        line += (i < 20) ? ";" : "";
                    }
                    File.AppendAllText(fileName, line + Environment.NewLine, Encoding.GetEncoding("cp866"));
                    Console.WriteLine(line);
                }
                Console.WriteLine("За {0:d} найдено {1} записей", date, matches.Count);
            }
            else Console.WriteLine("За {0:d} данных нет!", date);
        }
        static void GetRepoList(DateTime date)
        {
            var url = new Uri(@"http://cbr.ru/archive/Default.aspx?Prtid=infodirectrepo");
            var postFieldsList = new[]
            {
                "__EVENTTARGET=",
                "__EVENTARGUMENT=",
                string.Format("ctl00$ContentPlaceHolder1$UC_itm_42866$ToDate={0:d}", date),
                "ctl00$ContentPlaceHolder1$UC_itm_42866$doFilterGibrid=Получить"
            };
            var postBytes = Encoding.UTF8.GetBytes(string.Join("&", postFieldsList));

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            request.Method = WebRequestMethods.Http.Post;
            request.ServicePoint.Expect100Continue = false;
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.153 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;

            using (var postStream = request.GetRequestStream())
                postStream.Write(postBytes, 0, postBytes.Length);

            string responseFromServer;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Console.WriteLine("Статус подключения к серверу: {0}", response.StatusDescription);
                using (var responseStream = response.GetResponseStream())
                    responseFromServer = new StreamReader(responseStream).ReadToEnd();
            }

            const string pattern = @"<tr>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<td>(.*?)<\/td>\s*?<\/tr>";
            var matches = Regex.Matches(responseFromServer, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                var fileName = string.Format("{0:d} РЕПО.txt", date);
                File.WriteAllText(fileName, string.Empty, Encoding.GetEncoding("cp866"));
                foreach (Match match in matches)
                {
                    var line = string.Empty;
                    for (var i = 1; i <= 20; i++)
                    {
                        line += (i > 4) ? match.Groups[i].Value.Trim().Replace("—", "").Replace(" ", "") : match.Groups[i].Value.Trim();
                        line += (i < 20) ? ";" : "";
                    }
                    File.AppendAllText(fileName, line + Environment.NewLine, Encoding.GetEncoding("cp866"));
                    Console.WriteLine(line);
                }
                Console.WriteLine("За {0:d} найдено {1} записей", date, matches.Count);
            }
            else Console.WriteLine("За {0:d} данных нет!", date);
        }

        static void GetLombardList(DateTime date)
        {
            Uri url = new Uri(@"http://cbr.ru/hd_base/default.aspx?PrtID=bankpapers");
            List<string> postFieldsList = new List<string>();
            postFieldsList.Add("__EVENTTARGET=");
            postFieldsList.Add("__EVENTARGUMENT=");
            postFieldsList.Add(string.Format("ctl00$ContentPlaceHolder1$UC_itm_34120$ToDate={0:d}", date));
            postFieldsList.Add("ctl00$ContentPlaceHolder1$UC_itm_34120$doFilterGibrid=Получить");
            byte[] postBytes = Encoding.UTF8.GetBytes(string.Join("&", postFieldsList));

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Proxy.Credentials = CredentialCache.DefaultCredentials;
            request.Method = WebRequestMethods.Http.Post;
            request.ServicePoint.Expect100Continue = false;
            request.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;

            using (Stream postStream = request.GetRequestStream())
                postStream.Write(postBytes, 0, postBytes.Length);

            string responseFromServer;
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Console.WriteLine("Статус подключения к серверу: {0}", response.StatusDescription);
                using (Stream responseStream = response.GetResponseStream())
                    responseFromServer = new StreamReader(responseStream).ReadToEnd();
            }

            string pattern = @"<tr[^<>]*?>[^<>]*?<td>([^<>]*?)<\/td>[^<>]*?<td>([^<>]*?)<\/td>[^<>]*?<td class=""right"">([^<>]*?)<\/td>[^<>]*?<td class=""right"">([^<>]*?)<\/td>[^<>]*?<td class=""right"">([^<>]*?)<\/td>[^<>]*?<td class=""right"">([^<>]*?)<\/td>[^<>]*?<\/tr>";
            MatchCollection matches = Regex.Matches(responseFromServer, pattern, RegexOptions.IgnoreCase);
            if (matches.Count > 0)
            {
                string fileName = String.Format("{0:d} ЛОМБАРД.txt", date);
                File.WriteAllText(fileName, String.Empty, Encoding.GetEncoding("cp866"));
                foreach (Match match in matches)
                {
                    List<string> itemStr = new List<string>();
                    for (int i = 1; i <= 6; i++)
                        itemStr.Add(match.Groups[i].Value.Trim());

                    string line = string.Join("\t", itemStr);
                    File.AppendAllText(fileName, line + Environment.NewLine, Encoding.GetEncoding("cp866"));
                    Console.WriteLine(line);
                }
                Console.WriteLine("За {0:d} найдено {1} записей", date, matches.Count);
            }
            else Console.WriteLine("За {0:d} данных нет!", date);
        }
    }
}
