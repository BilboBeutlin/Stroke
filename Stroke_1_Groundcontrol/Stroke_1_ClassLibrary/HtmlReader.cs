using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace Stroke_1_ClassLibrary
{
    public class HtmlReader
    {
        public LiveData _data;
        string Link;
        string htmlCode;

        /// <summary>
        /// Ist der Konstruktor der Klasse HtmlReader. Beim Erstellen wird erstmalig die Seite auf Daten geprüft.
        /// This is the constructor for the class HtmlReader.The constructor check the webpage for data.
        /// </summary>
        /// <param name="Link">
        /// Ist der link zu der zu prüfenden Internetseite.
        /// Is the link to the webpage which will be checked.
        /// </param>
        public HtmlReader(string Link)
        {
            string[] DataText;
            _data = new LiveData();
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Link);
                myRequest.Method = "GET";
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
                string result = sr.ReadToEnd();
                DataText = result.Split('\n');
                sr.Close();
                myResponse.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("internetseite konnte nicht geöffnet werden,\nBitte überprüfen Sie die Internetverbindung\nund die Einstellungen im Programm");
            }
            

            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {

                string pfad = dialog.FileName;
                File.Copy(pfad, "temp.txt", true);
                DataText = File.ReadAllLines("temp.txt");
                Interprete(DataText);
            }
            //Interprete(DataText);
        }

        private void Interprete(String[] data)
        {
            int decode = data[115].IndexOf("selected=\"selected\">Dekodiert");
            if (decode != -1)
            {
                InterpreteDecode(data);
            }
            int hex = data[115].IndexOf("value=\"hex\" selected=\"selected\"");
            if (hex != -1)
            {
                MessageBox.Show("Hex-Code noch nicht implementiert");
            }
            int coded = data[115].IndexOf("value=\"normal\" selected=\"selected\"");
            if (coded != -1)
            {
                MessageBox.Show("Coded noch nicht implementiert");
            }
            if ((coded == -1) & (hex == -1) & (decode == -1))
            {
                MessageBox.Show("Internetseite kann nicht Interpretiert werden");
            }
        }

        private void InterpreteDecode(string[] data)
        {
            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            provider.NumberGroupSizes = new int[] { 3 };

            int detection = 0;
            int year = 0;
            int Month = 0;
            int Day = 0;
            int Hour = 0;
            int Minte = 0;
            int Secounds = 0;
            double Latitude = 0;
            double londitude = 0;
            double atitude = 0;

            for (int i = 138; i < data.Length; i++)
            {
                int datefound = data[i].IndexOf("<span class=\"raw_line\">");
                if (datefound > -1)
                {
                    string S_Year = data[i].Substring(23, 4);
                    string S_Month = data[i].Substring(28, 2);
                    string S_Day = data[i].Substring(31, 2);
                    string S_Houer = data[i].Substring(34, 2);
                    string S_Minute = data[i].Substring(37, 2);
                    string S_Secound = data[i].Substring(40, 2);

                    year = Convert.ToInt32(S_Year, provider);
                    Month = Convert.ToInt32(S_Month, provider);
                    Day = Convert.ToInt32(S_Day, provider);
                    Hour = Convert.ToInt32(S_Houer, provider);
                    Minte = Convert.ToInt32(S_Minute, provider);
                    Secounds = Convert.ToInt32(S_Secound, provider);

                    detection = 1;
                }
                int latitudeFound = data[i].IndexOf("latitude: ");
                if (latitudeFound > -1)
                {
                    int blub = data[i].IndexOf(" °");
                    string S_latitude = data[i].Substring(28,(blub - 28));

                    Latitude = Convert.ToDouble(S_latitude, provider);

                    detection++;
                }
                int longitudeFound = data[i].IndexOf("longitude: ");
                if (longitudeFound > -1)
                {
                    int blub = data[i].IndexOf(" °");
                    string S_longitude = data[i].Substring(29, (blub - 29));

                    londitude = Convert.ToDouble(S_longitude, provider);

                    detection++;
                }
                int altitudeFound = data[i].IndexOf("altitude: ");
                if (altitudeFound > -1)
                {
                    int blub = data[i].IndexOf(" m");
                    string S_altitude = data[i].Substring(28, (blub - 28));

                    atitude = Convert.ToDouble(S_altitude, provider);

                    detection++;
                }
                if (detection == 4)
                {
                    LiveDatum tempdate = new LiveDatum();
                    tempdate.time = new DateTime(year, Month, Day, Hour, Minte, Secounds);
                    tempdate.latitude = Latitude;
                    tempdate.longitude = londitude;
                    tempdate.altitude = atitude;
                    _data.AddData(tempdate);
                    detection = 0;
                }
                     
            }
        }

        /// <summary>
        /// Internetseite (Link) wird erneut geladen und auf neue Daten geprüft. 
        /// Sind neue Daten enthalten, so werden sie der Variable "data" hinzugefügt.
        /// Webpage will be loaded again and checked for new data. If there are new Data, 
        /// they will be added to the variable "data".
        /// </summary>
        public void refresh()
        {

        }

    }
}
