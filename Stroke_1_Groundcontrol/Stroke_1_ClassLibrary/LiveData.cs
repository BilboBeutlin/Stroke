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
        List<LiveDatum> _Datalist;
        public string PfadOrLink;    //Quellpfad
        private bool InterpreteHtml;
        private bool InterpreteLog;
        public int length { get { return this._Datalist.Count; } }
        public LiveDatum[] DatatArray { get { return this._Datalist.ToArray(); } }

        /// <summary>
        /// erstellt eine Instanz der Klsaae Live Data.
        /// </summary>
        /// <param name="Input">Html = interpretiert eine aprs.fi, Rohdatenpacket, nternetseite
        /// LOG = Interpretiert eine hinterlegte Logdatei</param>
        public LiveData(string Input)
        {
            this._Datalist = new List<LiveDatum>();
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
            _Datalist.Add(datum);
        }

        public void AddData(double longitude,double latitude,double altitude,DateTime time)
        {
            LiveDatum tempdate = new LiveDatum();
            tempdate.longitude = longitude;
            tempdate.latitude = latitude;
            tempdate.altitude = altitude;
            tempdate.time = time;
            _Datalist.Add(tempdate);
        }

        public void DelDatum(int index)
        {
            _Datalist.RemoveAt(index);
        }

        public void ClearData()
        {
            _Datalist.Clear();
        }

        public string[,] DatalistToStringArray()
        {
            if (this._Datalist.Count <= 0) return null;
            string[,] tempstring = new string[this._Datalist.Count, this._Datalist.ElementAt(0).numberOfValue];
            for (int i = 0; i < this._Datalist.Count; i++)
            {
                string[] temp = this._Datalist.ElementAt(i).ToStringArray();
                for (int j = 0; j < this._Datalist.ElementAt(0).numberOfValue; j++)
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
                if (this._Datalist.Count <= 0)
                {
                    this._Datalist = reader._data._Datalist;
                }
                else
                {
                    this.Insert(reader._data._Datalist);
                }
            }
            if (InterpreteLog)
            {
                aprs reader = new aprs(this.PfadOrLink);
                if (this._Datalist.Count <= 0)
                {
                    this._Datalist = reader.ToDatalist();
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
                for (int j = 0; j < this._Datalist.Count ; j++)
                {
                    if (list[i].time == this._Datalist[j].time)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    this._Datalist.Add(list[i]);
                }
            }
        }
    }
}
