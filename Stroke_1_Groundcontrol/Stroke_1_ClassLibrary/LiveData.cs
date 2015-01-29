using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stroke_1_ClassLibrary
{
    public class LiveDatum
    {
        public double longitude;    //Längengrad
        public double latitude;     //Breitengrad
        public DateTime time;       //Zeitangabe
        public double altitude;     //Höhe

        int _numberOfValue = 4;
        public int numberOfValue { get { return this._numberOfValue; } }

        public LiveDatum()
        {
            this._numberOfValue = 4;
        }

        public override string ToString()
        {
            return base.ToString();
        }
        public string[] ToStringArray()
        {
            string[] tempstring = new string[this._numberOfValue];
            tempstring[0] = time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString();
            tempstring[1] = latitude.ToString();
            tempstring[2] = longitude.ToString();
            tempstring[3] = altitude.ToString();
            return tempstring;
        }
    }
    public class LiveData
    {
        List<LiveDatum> Datalist;
        public string PfadOrLink;    //Quellpfad
        private bool InterpreteHtml;
        private bool InterpreteLog;
        public int length { get { return this.Datalist.Count; } }

        /// <summary>
        /// erstellt eine Instanz der Klsaae Live Data.
        /// </summary>
        /// <param name="Input">Html = interpretiert eine aprs.fi, Rohdatenpacket, nternetseite
        /// LOG = Interpretiert eine hinterlegte Logdatei</param>
        public LiveData(string Input)
        {
            this.Datalist = new List<LiveDatum>();
            if ((Input == "Html") | (Input == "html") | (Input == "HTML"))
            {
                this.InterpreteHtml = true;
                this.InterpreteLog = false;
            }
            else if ((Input == "log") | (Input == "Log") | (Input == "LOG"))
            {
                this.InterpreteLog = true;
                this.InterpreteHtml = false;
            }
            else
            {
                this.InterpreteHtml = false;
                this.InterpreteLog = false;
            }
        }

        public void AddData(LiveDatum datum)
        {
            Datalist.Add(datum);
        }

        public void AddData(double longitude,double latitude,double altitude,DateTime time)
        {
            LiveDatum tempdate = new LiveDatum();
            tempdate.longitude = longitude;
            tempdate.latitude = latitude;
            tempdate.altitude = altitude;
            tempdate.time = time;
            Datalist.Add(tempdate);
        }

        public void DelDatum(int index)
        {
            Datalist.RemoveAt(index);
        }

        public void ClearData()
        {
            Datalist.Clear();
        }

        public string[,] DatalistToStringArray()
        {
            if (this.Datalist.Count <= 0) return null;
            string[,] tempstring = new string[this.Datalist.Count, this.Datalist.ElementAt(0).numberOfValue];
            for (int i = 0; i < this.Datalist.Count; i++)
            {
                string[] temp = this.Datalist.ElementAt(i).ToStringArray();
                for (int j = 0; j < this.Datalist.ElementAt(0).numberOfValue; j++)
                {
                    tempstring[i, j] = temp[j];
                }
                
            }
            return tempstring;
        }

        /// <summary>
        /// Liest die angegebene Quelle ein und aktualisiert die hinterlegte Liste
        /// </summary>
        public void Read()
        {
            if (InterpreteHtml)
            {
                HtmlReader reader = new HtmlReader(this.PfadOrLink);
                if (this.Datalist.Count <= 0)
                {
                    this.Datalist = reader._data.Datalist;
                }
                else
                {
                    this.Insert(reader._data.Datalist);
                }
            }
            if (InterpreteLog)
            {
                aprs reader = new aprs(this.PfadOrLink);
                if (this.Datalist.Count <= 0)
                {
                    this.Datalist = reader.ToDatalist();
                }
                else
                {
                    this.Insert(reader.ToDatalist());
                }
            }
        }

        private void Insert(List<LiveDatum> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                bool found = false;
                for (int j = 0; j < this.Datalist.Count ; j++)
                {
                    if (list[i].time == this.Datalist[j].time)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    this.Datalist.Add(list[i]);
                }
            }
        }
    }
}
