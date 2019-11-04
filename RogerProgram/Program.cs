/*BIBLIOTEKI*/

using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.IO;
using System.Net;



/*NAZWA PROJKETU */
namespace RogerProgram
{
/*KLASA*/
    class Program
    {
        /*DEKLARACJA ZMIENNYCH GLOBALNYCH ZWIAZANYCH Z OBSŁUGĄ PLIKÓW*/
        int RunNoCounter = 1;
        private static readonly string RogerLogPath = Environment.CurrentDirectory + "\\" + "test.txt";    //Adres ścieżki z której będą czytane logi Rogera
        private static readonly string AttendanceRaportPath = DateTime.Now.ToString("yyyy/MM/dd") + "_Obecnosc.csv";    //Adres ścieżki, gdzie będzie produkowany/tworzony raport obecności

        /*DEKLARACJA ZMIENNYCH GLOBALNYCH ZWIAZANYCH Z OBSŁUGĄ POCZTY*/

        private static readonly string MailAdresat = "rafalsiwek.95@gmail.com";     //Adres mailowy, na który będzie wysyłany log
        private static readonly string MailNadawca = "ball.full@o2.pl";     //Adres mailowy, z którego bedzie wysyłany mail
        private static readonly string SmtpAdres = "poczta.o2.pl";      //Adres domeny SMTP nadawcy, udostępniającej usługę wysyłania wiadmości
        private static readonly int SmtpPort = 587;     //Port domeny SMTP po której będzie wysyłana wiadomość

        /*DEKLARACJA ZMIENNYCH GLOBALNYCH ZWIĄZANYCH Z OBSŁUGĄ API*/
        

        /*DEKLARACJA ZMIENNYCH GLOBALNYCH ZWIĄZANYCH Z USTALENIEM CZASU WYSYŁANIA RAPORTU*/

        private static int TargetHour = 12;     //Godzina terminu
        private static int TargetMinute = 0;   //Minuta terminu

        /*DEKLARACJA ZMIENNYCH GLOBALNYCH PROCESOWYCH */
        private int TaskMailSentDone = 0;   //Zmienna odpowiedzialna za jednorazowe wykonanie zadania raportowania - zapobiega wykonywaniu tysiąc razy zadania w czasie kiedy czas globalny będzie wynosił 12:00
        private int TaskClearLog = 0;       //Zmienna odpowiedzialna za jednorazowe wykonanie czyszczenia logu;

        /*FUNKCJA WYSYŁAJĄCA DANE DO API HRNEST*/
        private static readonly string StartWorkTimeURL = "https://hrnest.io/api/StartWorkTime";
        private static readonly string EndtWorkTimeURL = "https://hrnest.io/api/EndWorkTime";
        private void PostAPIAttendanceUSERStartTime(string UserID, string StartDateTime)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(StartWorkTimeURL);
            DateTime time = DateTime.Now;
            ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{\"login\":\"balluff\"," +
                               "\"apiKey\":\"dloAsVmDbGrGQY@l7iQrBFM09PP7oQAc5HBAV9LWkGI6WjjRRX\"," +
                               "\"userId\":"+"\"" + UserID +"\""+"," +
                               "\"time\":" + "\"" + StartDateTime + "\"" +"}"; 

                
                Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " : Wysyłanie zapytania: " + json);
                streamWriter.Write(json);
                //streamWriter.Close();
            }

            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    Console.WriteLine(result);
            //    streamReader.Close();


            //}
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " : SUKCES!: Odpowiedź serwera: " + result);
                    Console.ResetColor();
                    streamReader.Close();

                }
            }
            catch (WebException WebException)
            {
                using (var stream = WebException.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " : BŁĄD! Odpowiedź serwera: " + reader.ReadToEnd());
                    Console.ResetColor();
                    //reader.Close();
                }
                //Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " :" + WebException);
            }

        }

        private void PostAPIAttendanceUSEREndTime(string UserID, string EndDateTime)
        {

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(EndtWorkTimeURL);
            DateTime time = DateTime.Now;
            // ServicePointManager.SecurityProtocol = ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                 string json = "{\"login\":\"balluff\"," +
                               "\"apiKey\":\"dloAsVmDbGrGQY@l7iQrBFM09PP7oQAc5HBAV9LWkGI6WjjRRX\"," +
                               "\"userId\":\"" + UserID + "\"," +
                               "\"time\":\"" + EndDateTime + "\"," +
                               "\"breakMinutes\":\"0\"}";
                              
                Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " : Wysyłanie zapytania: " + json);

                streamWriter.Write(json);
                streamWriter.Close();
            }
            //var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    Console.WriteLine(result);
            //    streamReader.Close();

            //}
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " : SUKCES!: Odpowiedź serwera: " + result);
                    Console.ResetColor();
                    streamReader.Close();                  

                }
            }
            catch (WebException WebException)
            {
                using (var stream = WebException.Response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " : BŁĄD! Odpowiedź serwera: " + reader.ReadToEnd());
                    Console.ResetColor();
                    reader.Close();
                }
                //Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm") + " :" + WebException);
            }
                
        }

        /*FUNKCJA ODCZYTUJĄCA LOG Z MONITORINGU ROGERA ORAZ TWORZĄCA RAPORT OBECNOŚCI*/

        private void ReadLogSendtoAPIAndCreateCSV()
        {
            /*DEKLARACJA LIST ZAWIERAJĄCYCH WYEKSTRAHOWANE INFORMACJE Z LOGÓW ROGERA*/

            List<string> Godzina = new List<string>();  //Deklaracja listy zawierającej godziny wejścia użytkownika
            List<string> ID = new List<string>(); //Deklaracja listy zawierającej ID użytkownika, który o danej godzinie wszedł

            /*BEZPIECZNE UTWORZENIE STRUMIENIA ODCZYTUJĄCEGO PLIK Z LOGAMI Z DANEGO ADRESU ŚCIEŻKI*/

            using (var reader = new StreamReader(RogerLogPath))     //Wykorzystanie operatora "using" pozwala na bezpieczne zarządzanie nieobsługiwanymi zmiennymi - zapobiega wypływowi danych
            {
               
                while (!reader.EndOfStream) //Pętla zczytujaca kolejne linie logu, dopóki nie napotka końca znaków w pliku
                {
                    
                    var line = reader.ReadLine();   //Zmienna przechowująca sczytną linię
                    if(line != "") //Funkcja warunkowa, sprawdzająca poprawność lini
                    {
                        var values = line.Split(';');   //Tablica zawierająca w każdej komórce ciąg znaków, deklarowany przestrzenią pomiedzy separatorem ";"

                        /*FUNKCJA WARUNKOWA SPRAWDZAJĄCA, CZY POLE ID NIE JEST PUSTYM CIĄGIEM, ORAZ CZY DANE ID JUŻ POPRZEDNIO WYSTĄPIŁO W ODCZYCIE - POZWALA NA OKREŚLENIE NAJWCZEŚNIEJSZEGO WYSTĄPIENIA UŻYCIA CZYTNIKÓW */

                        if (values[1] != "" && ID.FindIndex(x => x.StartsWith(values[1])) == -1)
                        {
                            // JEŻELI JEST WYSZCZEGÓLNIONE ID UŻYTKOWNIKA...
                            Godzina.Add(values[0]); //Zapisz wyekstrahowaną godzinę użycia Rogera
                            ID.Add(values[1]);  //Zapisz wyekstrahowane ID uzytkownika, powiązane z godzina wykorzystania Rogera
                        }
                    }
                    
                }
                reader.Close(); //Zamknięcie strumienia odczytu
            }

            /*BEZPIECZNE UTWORZENIE STRUMIENIA TWORZĄCEGO RAPORT, JEŻELI NIE MA TAKOWEGO POD DANYM ADRESEM ŚCIEŻKI */

            using (var writer = new StreamWriter(AttendanceRaportPath)) //Jak wyżej ^
            {
                /*TWORZENIE ZMIENNYCH TYPU DateTime, ZAWIERAJĄCYCH FORMAT CZASU*/
                DateTime timeStart; 
                DateTime timeEnd;
                /*PĘTLNA ZAPISUJĄCA DANE Z LIST DO PLIKU CSV*/
                for (int i = 0; i < ID.Count; i++)
                {
                    timeStart = DateTime.Parse(Godzina[i]); //Zapisanie czasu wejścia użytkownika w formie "HH:mm:ss" do formatu DateTime

                    /*FUNKCJA WARUNOWA ZAOKRĄGLAJĄCA CZAS PRZYBYCIA DO PEŁNYCH GODZIN*/

                    if (timeStart.Minute < 30) //Jeżeli pracownik przyszedł do 30 min później ma zaokrąglony czas przyjścia w dół
                    {
                        timeStart = timeStart.Date + new TimeSpan(timeStart.Hour, 0,0); //Update zmiennej zawierającej czas przyjścia

                    }
                    else //Jeżeli pracownik przyszedł max 30 min wcześniej ma czas zaokrąglony do godziny w góre
                    {
                        timeStart = timeStart.Date + new TimeSpan(timeStart.Hour+1, 0, 0); //Analogicznie do  ^
                    }
                    timeEnd = timeStart.Date + new TimeSpan(timeStart.Hour + 8, 0, 0); //Ustalenie czasu wyjścia jako + 8h pracy
                    writer.WriteLine(ID[i]+';'+timeStart.ToString("yyyy'-'MM'-'dd' 'HH':'mm") +';'+timeEnd.ToString("yyyy'-'MM'-'dd' 'HH':'mm")); //Zapisanie do pliku wiersza CSV w formacie ID;Czas przyjścia;Czas Wyjścia
                    PostAPIAttendanceUSERStartTime(ID[i], timeStart.ToString("yyyy'-'MM'-'dd' 'HH':'mm"));
                    PostAPIAttendanceUSEREndTime(ID[i], timeEnd.ToString("yyyy'-'MM'-'dd' 'HH':'mm"));
                }

                writer.Close(); //Bezpieczne zamknięcie strumienia zapisu
            }
                TaskMailSentDone = 1;   //Ustawienie zmiennej sygnalizującej wykonanie zadania

            //Potwierdzenie wyslania informacji na ekranie. Wyczyszczenie ekranu.

            
            RunNoCounter += 1;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(DateTime.Now.ToString("yyyy/MM/dd") + ": All data of " + ID.Count + " users have been send!");
            Console.WriteLine("Data stored to file: " + Environment.CurrentDirectory + "\\" + AttendanceRaportPath);
            Console.ResetColor();

            Console.WriteLine("####################################################################################");
            Console.WriteLine("####################################################################################");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Run number counter: " + RunNoCounter);
            Console.WriteLine("Roger Path: " + RogerLogPath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Report target time: " + TargetHour + ":" + TargetMinute);
            Console.WriteLine("Running...");
            Console.ResetColor();
            Console.WriteLine("####################################################################################");
            Console.WriteLine("####################################################################################");
        }
   
        /* FUNKCJA BĘDĄCA DELEGATEM WĄTKU SPRAWDZAJĄCEGO CZAS */

        private void DailyTimer()
        {
            DateTime time; //Ustalenie zmiennej zawierającej czas chwilowy w formacie DateTime
            
            //now = DateTime.Now;
            

            /*WIECZNA PĘTLA SPRAWDZAJĄCA CZAS*/
            while (true) 
            {
                time = DateTime.Now; //Odczytanie czasu w danej chwili
                        //Console.WriteLine(time.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss"));
                int hour = time.Hour;   //Wyekstrahowanie godziny w formie zmiennej całkowitej 32 bit
                int minute = time.Minute;   //Wyekstrachowanie minuty ...^

                if (hour == TargetHour && minute == TargetMinute) //Petla warunkowa wykonująca czynność, jeżeli osiągnięto czas predefinowany - w tym przypadku 12:00
                {
                    if (TaskMailSentDone == 0)
                    {
                        ReadLogSendtoAPIAndCreateCSV(); //Stwórz raport obecności
                        //SendMail(MailAdresat, MailNadawca, SmtpAdres, time.ToString(), SmtpPort); //Wyślij wiadomość
                    }
                }
                else if(hour == 0 && minute == 0) // Jeżeli jest godzina 0:00
                {
                    if(TaskClearLog == 0)
                    {
                        File.WriteAllText(RogerLogPath, String.Empty);  //Wyczyszczenie loga Rogera
                        TaskClearLog = 1; //Ustaw informację, że wykonano zadanie
                    }
                    
                }

                else //Jeżeli jeszcze nie osiągnieto danego momentu nie wykonuj powyższych czynności
                {
                    if(TaskMailSentDone == 1) //Jeżeli wykonano czynność
                    {
                        TaskMailSentDone = 0; //Ustaw czynność jako jeszcze nie wykonaną - czekaj kolejne 12 h
                    }
                    else if (TaskClearLog == 1) //Jeżeli wykonano czynność
                    {
                        TaskClearLog = 0; //Ustaw czynność jako jeszcze nie wykonaną - czekaj kolejne 12 h
                    }
                    


                }
                System.Threading.Thread.Sleep(20);
            }
        }

        /*FUNKCJA WYSYŁAJĄCA EMAIL Z ZAŁĄCZNIKIEM*/
         
        private static void SendMail(string ToAdr, string FromAdr, string SmtpAdr, string Time, int SmtpPortID)
        {

            MailMessage mailMessage = new MailMessage(); //Utworzenie obiektu typu MailMessage
            mailMessage.From = new MailAddress(FromAdr); //Zdefiniowanie adresu wychodzącego
            mailMessage.To.Add(ToAdr);  //Zdefiniwanie adresu odbiorcy
            mailMessage.Subject = "Obecnosc";   //Zdefiniowanie tematu wiadomości
            mailMessage.Body = "Obecnosc";  //Zdefiniowanie treści wiadomości

                /*SPAKOWANIE PLIKU ZAŁĄCZNIKA*/

            Attachment DataAttatchment = new Attachment(AttendanceRaportPath, MediaTypeNames.Application.Octet);
            ContentDisposition disposition = DataAttatchment.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(AttendanceRaportPath);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(AttendanceRaportPath);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(AttendanceRaportPath);
            mailMessage.Attachments.Add(DataAttatchment);  //Dodaj załącznik do wiadomości

            SmtpClient smtpServer = new SmtpClient(SmtpAdr);    //Utworzenie strumienia serwera, którego klientem jest adres usługi SMTP nadawcy
            smtpServer.Port = SmtpPort; //Zdefiniowanie portu wysyłania wiadomości
            smtpServer.Credentials = new System.Net.NetworkCredential(FromAdr, "Ball.Uff"); //Zdefiniowanie danych serwera, z którego zostanie wysłany email obok FromAdr jest hasło skrzynki
            smtpServer.EnableSsl = true; //Zezwolenie szyfrowania SSL

            try
            {
                smtpServer.Send(mailMessage);   //Spróbuj wysłać wiadomość wg w/w danych
                Console.WriteLine(Time + ": Wyslano"); //Wydrukuj na konsoli log z czasem wysłania wiadomości w wypadku powodzenia
            }
            catch (Exception ex)
            {
                Console.WriteLine(Time + ": "+ ex.ToString()); //Wydrukuj na konsoli log z błędem w razie niepowodzenia
            }


        }

        

        static void Main(string[] args)
        {


            Console.Write("Set Hour:");
            string strTargetHour = Console.ReadLine();

            TargetHour = int.Parse(strTargetHour);

            Console.Write("Set Minute:");
            string strTargetMinute = Console.ReadLine();

            TargetMinute = int.Parse(strTargetMinute);

            Console.WriteLine("####################################################################################");
            Console.WriteLine("####################################################################################");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Roger Path: " + RogerLogPath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Report target time: " + TargetHour + ":" + TargetMinute);
            Console.WriteLine("Running...");
            Console.ResetColor();
            Console.WriteLine("####################################################################################");
            Console.WriteLine("####################################################################################");


            Program program = new Program(); //Zadeklarowanie obszaru pracy
            Thread thread = new Thread(new ThreadStart(program.DailyTimer)); //Utworzenie wątku
            thread.Priority = ThreadPriority.Lowest;

            thread.Start(); //Rozpoczęcie pracy wątku


        }
    }
}
