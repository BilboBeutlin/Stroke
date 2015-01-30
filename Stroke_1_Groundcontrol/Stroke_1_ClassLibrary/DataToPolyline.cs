using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace Stroke_1_ClassLibrary
{
    public class DataToPolyline
    {
        int MarginLeft, MarginRight, MarginTop, MarginButton;
        int With, Hight;
        List<Polyline> linelist = new List<Polyline>();

        public DataToPolyline(int MarginLeft, int MarginRight, int MarginTop, int MarginButton, int CanvisWith, int CanvisHigh)
        {
            this.MarginLeft = MarginLeft;
            this.MarginRight = MarginRight;
            this.MarginTop = MarginTop;
            this.MarginButton = MarginButton;
            this.With = CanvisWith;
            this.Hight = CanvisHigh;
        }

        public List<Polyline> Createlines(LiveData data, int X_Axis)
        {
            LiveDatum[] dataArray = data.DatatArray;
            if (dataArray.Length <= 1) return null;
            
            //erzeugen der X-Werte
            List<double> ListX = new List<double>();
            double maxvalX = -1000000;
            double minvalX = 1000000;
            int Y_Size = this.Hight - this.MarginButton - this.MarginTop;
            int X_Size = this.With - this.MarginLeft - this.MarginRight;
            #region finden von minima und maxima
            //###############################################//
            for (int i = 0; i < dataArray.Length; i++)
            {
                double Value;
                switch (X_Axis)
                {
                    case 0:
                        Value = (double)dataArray[i].time.Minute + (double)(dataArray[i].time.Hour * 60.0) + ((double)dataArray[i].time.Second / 60.0);
                        break;
                    case 1:
                        Value = (double)dataArray[i].latitude;
                        break;
                    case 2:
                        Value = (double)dataArray[i].longitude;
                        break;
                    case 3:
                        Value = (double)dataArray[i].altitude;
                        break;
                    default:
                        Value = 0;
                        break;
                }
                if (Value > maxvalX) maxvalX = Value;
                if (Value < minvalX) minvalX = Value;
            }
            #endregion
            #region Berechnen der X-Werte und zuweise in die liste
            //#############################################################
            for (int i = 0; i < dataArray.Length; i++)
            {
                double Value;
                switch (X_Axis)
                {
                    case 0:
                        Value = (double)dataArray[i].time.Minute + (double)(dataArray[i].time.Hour * 60.0) + ((double)dataArray[i].time.Second / 60.0);
                        break;
                    case 1:
                        Value = (double)dataArray[i].latitude;
                        break;
                    case 2:
                        Value = (double)dataArray[i].longitude;
                        break;
                    case 3:
                        Value = (double)dataArray[i].altitude;
                        break;
                    default:
                        Value = 0;
                        break;
                }
                //Normieren * Skalieren + Offset
                Value = ((Value / (maxvalX - minvalX)) * (double)X_Size) + (double)MarginLeft;
                ListX.Add(Value);
            }
            #endregion
            for (int NrOfLines = 0; NrOfLines < dataArray[0].numberOfValue; NrOfLines++)
			{
                double maxvalY = -1000000;
                double minvalY = 1000000;
                List<double> ListY = new List<double>();
                #region finden von Max und Minwerten
                //#######################################################//
                for (int i = 0; i < dataArray.Length; i++)
                {
                    double Value;
                    switch (NrOfLines)
                    {
                        case 0:
                            Value = (double)dataArray[i].time.Minute + (double)(dataArray[i].time.Hour * 60.0) + ((double)dataArray[i].time.Second / 60.0);
                            break;
                        case 1:
                            Value = (double)dataArray[i].latitude;
                            break;
                        case 2:
                            Value = (double)dataArray[i].longitude;
                            break;
                        case 3:
                            Value = (double)dataArray[i].altitude;
                            break;
                        default:
                            Value = 0;
                            break;
                    }
                    if (Value < minvalY) minvalY = Value;
                    if (Value > maxvalY) maxvalY = Value;
                }
                #endregion
                Polyline templine = new Polyline();
                #region Berechnen und zuweisen Der Y-Werte
                //#######################################################//
                for (int i = 0; i < dataArray.Length; i++)
                {
                    double Value;
                    switch (NrOfLines)
                    {
                        case 0:
                            Value = (double)dataArray[i].time.Minute + (double)(dataArray[i].time.Hour * 60.0) + ((double)dataArray[i].time.Second / 60.0);
                            break;
                        case 1:
                            Value = (double)dataArray[i].latitude;
                            break;
                        case 2:
                            Value = (double)dataArray[i].longitude;
                            break;
                        case 3:
                            Value = (double)dataArray[i].altitude;
                            break;
                        default:
                            Value = 0;
                            break;
                    }
                    Value = (double)Y_Size - ((Value / (maxvalY - minvalX)) * (double)Y_Size) + (double)MarginTop;
                    ListY.Add(Value);
                    templine.Points.Add(new System.Windows.Point(ListX[i],ListY[i]));
                }
                #endregion
                this.linelist.Add(templine);
            }
            return this.linelist;
        }
    }
}
