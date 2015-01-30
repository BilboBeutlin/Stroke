using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stroke_1_ClassLibrary
{
    class aprs
    {
        string text;
        //public string time;
        // unsortierte Daten
        public List<string> uns_data    = new List<string>();
        // die wichtigsten Zeilen aus uns_date werde hier abgespeichert
        public List<string> sort_data   = new List<string>();
        // beinhaltetet die Zeit
        public List<string> Time = new List<string>();
        // beinhaltetet die Posiionswerte
        public List<string> Raw_Position = new List<string>();
        // beinhaltet die Latitude decodierten
        public List<double> Lat = new List<double>();
        // beinhaltet die Longditude decodierten
        public List<double> Long = new List<double>();
        // beinhaltet die Telemetriedaten
        public List<string> Telemetry = new List<string>();
        private string pfad;
        
        public aprs(string pfad)
        {
            this.pfad = pfad;
            this.encode();
        }

        private void encode()
        {

            // Datum + Uhrzeit aus System lesen und in das entsprechende Format bringen
            DateTime system_date = System.DateTime.Now;                               // System Datum+Uhrzeit holen
            system_date = System.DateTime.SpecifyKind(system_date, DateTimeKind.Utc);  // System Datum+Uhrzeit  in UTC    
            string sytem_date_str = system_date.ToUniversalTime().ToString("u");      // System Datum+Uhrzeit Konvertieren in "Universelles, sortierbares Datums-/Zeitmuster."
            sytem_date_str = sytem_date_str.Remove(sytem_date_str.Length - 1);         // Das letzte Zeichen "Z" aus dem Datum löschen, wird immer mit rangehängt
            try
            {
                this.uns_data = System.IO.File.ReadAllLines(this.pfad).ToList();
                // Herausfiltern der Informationen
                for (int i = 0; i < this.uns_data.Count; i++)
                {
                    if (this.uns_data[i].Contains(">:"))
                    {
                        this.sort_data.Add(this.uns_data[i + 1]);
                    }
                }
                // Sortieren Zeit <-> Position
                for (int i = 0; i < this.sort_data.Count; i++)
                {
                    List<string> temp_list = new List<string>(this.sort_data[i].Split(new char[] { '/', 'E' }, StringSplitOptions.RemoveEmptyEntries));
                    // ZeitString Formatieren
                    temp_list[0] = temp_list[0].Insert(2, ":");
                    temp_list[0] = temp_list[0].Insert(5, ":");
                    temp_list[0] = temp_list[0].Remove(8);
                    // ZeitString in informationsliste übertragen
                    this.Time.Add(temp_list[0]);
                    // DatenString in informationsliste übertragen
                    this.Raw_Position.Add(temp_list[1]);
                }
                // decodieren des DatenStrings
                for (int i = 0; i < this.Raw_Position.Count; i++)
                {
                    double lat = (double)(90 - ((this.Raw_Position[i][0] - 33) * Math.Pow(91, 3) + (this.Raw_Position[i][1] - 33) * Math.Pow(91, 2) + (this.Raw_Position[i][2] - 33) * 91 + (this.Raw_Position[i][3] - 33)) / 380926);
                    this.Lat.Add(lat);
                    double longd = (double)(-180 + ((this.Raw_Position[i][4] - 33) * Math.Pow(91, 3) + (this.Raw_Position[i][5] - 33) * Math.Pow(91, 2) + (this.Raw_Position[i][6] - 33) * 91 + this.Raw_Position[i][7] - 33) / 190463);
                    this.Long.Add(longd);
                }
            }
            catch (Exception)
            {
                System.Windows.MessageBox.Show("fehler beim Öffnen der Log-Datei");
            }
            
        }

        internal List<LiveDatum> ToDatalist()
        {
            List<LiveDatum> Datalist = new List<LiveDatum>();
            for (int i = 0; i < this.Lat.Count; i++)
            {
                String[] splittime = Time[i].Split(':');
                int houer = Convert.ToInt32(splittime[0]);
                int minite =  Convert.ToInt32(splittime[1]);
                int secound = Convert.ToInt32(splittime[2]);
                LiveDatum tempdate = new LiveDatum();
                tempdate.time = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, houer, minite, secound);
                tempdate.latitude = this.Lat[i];
                tempdate.longitude = this.Long[i];
                Datalist.Add(tempdate);
            }
            return Datalist;
        }
    }
}
