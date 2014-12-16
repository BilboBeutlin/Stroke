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
            tempstring[0] = time.ToString("de-DE");
            tempstring[1] = latitude.ToString();
            tempstring[2] = longitude.ToString();
            tempstring[3] = altitude.ToString();
            return tempstring;
        }
    }
    public class LiveData
    {
        List<LiveDatum> Datalist;

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
    }
}
