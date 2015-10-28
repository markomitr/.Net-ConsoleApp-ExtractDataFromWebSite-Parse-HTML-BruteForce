using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using HtmlAgilityPack;

namespace ZemiHtmlKod
{
   
    class Program
    {
       static  Dictionary<string, string> infoKopmanija = new Dictionary<string, string>();
       static Dictionary<string, string> rezPrebaruvanje = new Dictionary<string, string>();
       static List<String> lstZipKodoj = new List<string>();
       static String url = "http://www.manta.com/mb_41_ALL_11/georgia?";//"http://www.manta.com/mb?";
        static String search = "search=";
        static String strPaginator = "pg=";
        static int brZapisi = 0;
        public static  void Main(string[] args)
        {

            //procitajZipKodoj();
            //ReadTextFile();
            //return;

            brZapisi = 0;

            TextWriter tw = new StreamWriter("ElementaryGeorgia.txt");
            napolniRecnikPodatociKompanija();
            napolniRecnikPodatociStrani();


            int vkupnoStanici=0;
            int tekPaginator=1;
            string novSearch = "";
            string novUrlPaginator = "";
            string urlPrati="";
            string urlZipKod = "";
            int brSkolinaStrana = 0;
            napolniRecnikPodatociKompanija();
            napolniRecnikPodatociStrani();      
           try
            {
                procitajZipKodoj();
                for (int ii = 0; ii < lstZipKodoj.Count; ii++) // (int ii = 0; ii < lstZipKodoj.Count ; ii++)
                {


                    //novSearch = search + "elementary+school+" + lstZipKodoj[ii];
                    novSearch = search + "elementary+school+" + lstZipKodoj[ii];
                    novUrlPaginator = strPaginator + tekPaginator;
                    urlPrati = url + novUrlPaginator +"&" + novSearch;
                   vkupnoStanici = 1;
                   bool imagreska = true;
                   HtmlDocument doc = new HtmlDocument();
                   HtmlNodeCollection pomNodeRezultati;
                    for (int j = 1; j<= 1 ; j++) //vkupnoStanici -- ogranicuvanje na dve
                    {

                        while (imagreska)
                        {
                                doc = new HtmlDocument();
                                spijaj(5);
                                novUrlPaginator = strPaginator + j;
                                urlPrati = url + novUrlPaginator + "&" + novSearch;
                                zacuvajfile(urlPrati,"0_" + j.ToString());
                                Console.WriteLine(urlPrati);
                                doc = new HtmlDocument();
                                doc.Load("TestFile"+"0_" + j.ToString()+".html");
                                pomNodeRezultati = doc.DocumentNode.SelectNodes(rezPrebaruvanje["UrlAdresi"]);
                                if (pomNodeRezultati == null)
                                {
                                    Console.WriteLine("Nema REZULTATI se Null!!!");
                                    imagreska = true;
                                }
                                else
                                {
                                    imagreska = false;
                                }
                        }
                        imagreska = true;
                        if (j == 1)
                        {
                            HtmlNodeCollection pomNodePaginator = doc.DocumentNode.SelectNodes(rezPrebaruvanje["Paginator"]);
                            if (pomNodePaginator != null)
                            {
                                if (pomNodePaginator.Count > 0)
                                {
                                    vkupnoStanici = int.Parse(pomNodePaginator[0].InnerText.ToString());
                                }

                            }
                            else
                            {
                                Console.WriteLine("Paginator e Null!!!");
                            }
                        }

                         pomNodeRezultati = doc.DocumentNode.SelectNodes(rezPrebaruvanje["UrlAdresi"]);
                        if (pomNodeRezultati ==null)
                        {
                            Console.WriteLine("Nema REZULTATI se Null!!!");
                            break;
                        }
                        if (pomNodeRezultati.Count > 0)
                        {
                            brSkolinaStrana = 0;
                            for (int i = 0; i < pomNodeRezultati.Count; i++) //(int i = 0; i < pomNodeRezultati.Count - 1 && brSkolinaStrana  <= 10; i++)
                            {
                                spijaj(3);
                                zacuvajfile(pomNodeRezultati[i].Attributes["href"].Value,i.ToString());
                                HtmlDocument docKomp = new HtmlDocument();
                                docKomp.Load("TestFile"+i.ToString()+".html");

                                // close the stream
                                string imekomp = "",adresa="", adresaLok = "", adresaReg = "",zipkod="", tel = "", ime = "", titula = "",urlKomp="",website="",email="";
                                urlKomp = pomNodeRezultati[i].Attributes["href"].Value;
                                foreach (KeyValuePair<string, string> pom in infoKopmanija)
                                {
                                    try
                                    {
                                        HtmlNodeCollection pomNode = docKomp.DocumentNode.SelectNodes(pom.Value);
                                        if (pomNode != null)
                                        {
                                            Console.WriteLine(pom.Key.ToString() + ": " + pomNode[0].InnerText);
                                            switch (pom.Key)
                                            {
                                                case "Ime":
                                                    imekomp = pomNode[0].InnerText;
                                                    break;
                                                case "Adresa":
                                                    adresa = pomNode[0].InnerText;
                                                    break;
                                                case "AdresaLokalno":
                                                    adresaLok = pomNode[0].InnerText;
                                                    break;
                                                case "AdresaRegion":
                                                    adresaReg = pomNode[0].InnerText;
                                                    break;
                                                case "ZipCode":
                                                    zipkod = pomNode[0].InnerText;
                                                    break;
                                                case "Telefon":
                                                    tel = pomNode[0].InnerText.Trim();
                                                    break;
                                                case "ImePrincipal":
                                                    ime = pomNode[0].InnerText;
                                                    break;
                                                case "ImePrincipal2":
                                                    if (String.IsNullOrEmpty(ime))
                                                    {
                                                        ime = pomNode[0].InnerText;
                                                    }
                                                    break;
                                                case "Titula":
                                                    titula = pomNode[0].InnerText;
                                                    break;
                                                case "WebSite" :
                                                    //website = pomNode[0].InnerText;
                                                    break;
                                                case "WebSite1":
                                                    //website = pomNode[0].InnerText;
                                                    break;
                                                case "Email":
                                                    String pomEm ="";
                                                    pomEm= pomNode[0].InnerText.Replace("wr(\"<span>","");
                                                    pomEm= pomEm.Replace("\");wr(\"@\");wr(\"","@");
                                                    pomEm =pomEm.Replace("</span>\")","");
                                                    email = pomEm;
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Nema vrednost za: " + pom.Value);
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                        Console.WriteLine(ex.Message);
                                        tw.Close();

                                    }

                                }
                                if (!(String.IsNullOrEmpty(imekomp) || 
                                    String.IsNullOrEmpty(adresaLok) ||         
                                    String.IsNullOrEmpty(adresaReg ) ||
                                    String.IsNullOrEmpty(tel) || 
                                    String.IsNullOrEmpty(ime) || 
                                    String.IsNullOrEmpty(titula)))
                                {
                                    brZapisi++;
                                    brSkolinaStrana++;
                                    tw.WriteLine(imekomp + "\t" + adresa + "\t" + adresaLok + "\t" + adresaReg + "\t" + zipkod + "\t" + tel + "\t" + ime + "\t" + titula + "\t" + email + "\t" + website + "\t" + urlKomp);
                                    Console.WriteLine("<----- " + brZapisi + " ----->");
                                    Console.WriteLine("<----- Zip:" + lstZipKodoj[ii].ToString() + " ----->");
                                }
                            
                            }

                        }
                        else
                        {
                            Console.WriteLine("Nema REZULTATI se 0!!!");
                            Console.Beep();
                        }
                    }
                  

                } //FOR GOLEM ZA RAZLICNI ZIPKODES
                tw.Close();
            }
            catch (Exception ex )
            {

                Console.WriteLine(ex.Message);
                tw.Close();
                Console.Beep();
            }
           Console.WriteLine("Zavrsiv");
           Console.Beep();
           Console.ReadLine();
        }
        public static void napolniRecnikPodatociStrani()
        {

            rezPrebaruvanje = new Dictionary<string, string>();
            rezPrebaruvanje.Add("UrlAdresi","//h2[@itemprop='name']/a[@class='url']");
            rezPrebaruvanje.Add("Paginator", "(//div[@class='pagination']/div/ul/li)[last()]");               
        }
        public static  void napolniRecnikPodatociKompanija()
        {
            infoKopmanija = new Dictionary<string, string>();
            infoKopmanija.Add("Ime", "//*[@itemprop='name'][@class='profile-company_name']");
            infoKopmanija.Add("Adresa", "//*[@itemprop='streetAddress']");
            infoKopmanija.Add("AdresaLokalno", "//*[@itemprop='addressLocality']" );
            infoKopmanija.Add("AdresaRegion", "//*[@itemprop='addressRegion']");
            infoKopmanija.Add("ZipCode", " //*[@itemprop='postalCode']");
            infoKopmanija.Add("Telefon", " //*[@itemprop='telephone']");
            infoKopmanija.Add("ImePrincipal", "//*[@itemprop='employees']/h6/span[@itemprop='name']");
            infoKopmanija.Add("Titula", "//*[@itemprop='employees']/em[@itemprop='jobTitle']");
            infoKopmanija.Add("WebSite", "//p[@id='profile-main_website']/a");
            infoKopmanija.Add("Email", "//p[@id='profile-main_email']/script");
            infoKopmanija.Add("ImePrincipal2", "//*[@itemprop='employees']/a/h6[@itemprop='name']");
            infoKopmanija.Add("WebSite1", "//dl[@class='website_info']/dd/span");


        }
        public static void zacuvajfile(string url,string broj)
        {
           bool trebaPak = true;
            while (trebaPak)
            {
                try
                {

                    if (File.Exists("TestFile"+broj+".html"))
                    {
                        File.Delete("TestFile"+broj+".html");
                    }


                    Uri uri = new Uri(url);
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/535.2 (KHTML, like Gecko) Chrome/15.0.874.121 Safari/535.2";
                   //HttpWebrequest = WebRequest.Create(uri);
                    request.Method = WebRequestMethods.Http.Get;

                    WebResponse response = request.GetResponse();

                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    Stream oStream = response.GetResponseStream();

                    StreamReader oStreamReader = new StreamReader(oStream, System.Text.Encoding.UTF8);

                    String tmp = oStreamReader.ReadToEnd();
                    oStream.Close();
                    oStreamReader.Close();

                    using (StreamWriter sw = new StreamWriter("TestFile" + broj + ".html"))
                    {
                        sw.WriteLine(tmp);
                    }
                    FileInfo pomInfo = new FileInfo("TestFile" + broj + ".html");
                    if (pomInfo.Length / 1024 < 50)
                    {
                        trebaPak = true;

                    }
                    else
                        trebaPak = false;
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                    Console.Beep();
                    trebaPak = true;
                    spijaj(5);
                }
            } 
        }
        public static void spijaj(int kolkusek)
        {
            Console.WriteLine();
            Console.WriteLine("Spijam:" + kolkusek + "sek");
            Thread.Sleep(kolkusek * 1000);
            Console.WriteLine();

        }
        public static void procitajZipKodoj()
        {
            String line = "";
            int counter = 0;
            StreamReader file = new StreamReader("zipkod.txt");
            while ((line = file.ReadLine()) != null)
            {
                lstZipKodoj.Add(line.Trim());
                counter++;
            }

            file.Close();

            // Suspend the screen.
            Console.WriteLine("Procitav ZipKodoj br:" +counter);
        }
        public static void ReadTextFile()
        {
            string line;

            // Read the file and display it line by line.
            TextWriter tw = new StreamWriter("PreSchoolsGorgiaSreden_ZipKod.txt");
            Dictionary<int, int> procitaniKodoj = new Dictionary<int, int>();
            using (StreamReader file = new StreamReader("PreSchoolsGorgiaSreden.txt"))
            {
                while ((line = file.ReadLine()) != null)
                {
                    //if (!line.Trim().StartsWith("http"))
                    //{
                    //    tw.WriteLine(line);
                    //}

                    char[] delimiters = new char[] { '\t' };
                    string[] parts = line.Split(delimiters);
                    StringBuilder nov = new StringBuilder();
                    bool dalizapis = false;
                    for (int i = 0; i < parts.Length; i++)
                    {

                        if (i == 4)
                        {

                            parts[i] = parts[i].Split('-')[0];
                            parts[i] = parts[i].Trim();
                            //if (lstZipKodoj.Contains(parts[i]))
                            //{
                            //    dalizapis = true;
                            //}
                            //if (procitaniKodoj.ContainsKey(int.Parse(parts[i])))
                            //{
                            //    procitaniKodoj[int.Parse(parts[i])] = procitaniKodoj[int.Parse(parts[i])] + 1;
                            //}
                            //else
                            //{
                            //    procitaniKodoj.Add(int.Parse(parts[i]), 1);
                            //}
                        }
                        //Console.Write(parts[i] + " ");
                        nov.Append(parts[i] + "\t");
                        //sepList.Add(parts[i]);

                    }
                    //if (dalizapis) { tw.WriteLine(nov.ToString()); }
                    //dalizapis = false;
                    tw.WriteLine(nov.ToString());
                }

                file.Close();
            }
            tw.Close();
            //tw = new StreamWriter("PreSchool - ZipKodoj.txt");
            //// Suspend the screen.
            //Console.WriteLine(procitaniKodoj.Count);
            //Console.ReadLine();
            //var list = procitaniKodoj.Keys.ToList();
            //list.Sort();

            //// Loop through keys.
            //foreach (var key in list)
            //{
            //    Console.WriteLine();
            //    tw.WriteLine(key.ToString() + "\t" + procitaniKodoj[key].ToString());
            //}
            //tw.Close();
            Console.ReadLine();
        }
    }
}
