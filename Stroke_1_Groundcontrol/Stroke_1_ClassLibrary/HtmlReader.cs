using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

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
            // später für livedaten
            
            
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://google.de");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            /*
            string[] temp = {"<span class='raw_line'>"};
            string[] temp2 = result.Split(temp,StringSplitOptions.RemoveEmptyEntries);
            */
            sr.Close();
            myResponse.Close();

            string pfad;
            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "_:-|*.TXT";
            if (dialog.ShowDialog() == true)
            {

                pfad = dialog.FileName;
                File.Copy(pfad, "temp.txt", true);
                string[] blubb = File.ReadAllLines("temp.txt");
                string[] array = { "latitude: ", " °" };
                string[] hihi = blubb[174].Split(array, StringSplitOptions.RemoveEmptyEntries);
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberDecimalSeparator = ".";
                provider.NumberGroupSeparator = ",";
                provider.NumberGroupSizes = new int[] { 3 };

                List<LiveDatum> schoeneListe = new List<LiveDatum>();
                schoeneListe.Add(new LiveDatum());
                schoeneListe.ElementAt(schoeneListe.Count - 1).latitude = Convert.ToDouble(hihi[1], provider);
                
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
