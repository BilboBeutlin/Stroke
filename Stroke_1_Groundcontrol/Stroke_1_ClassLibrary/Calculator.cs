using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Stroke_1_ClassLibrary
{
    public struct ContourLevel
    {
        public double a;
        public double T;
        public double h;
        public double p;
        public string name;
        public override string ToString()
        {
            return this.name;
        }
    }
    public struct Constants
    {
        public double M_air;
        public double M_gas;
        public double r_earth;
        public double g0;
        public double p0;
        public double R;
        public double roh_air;
        public double roh_helium;
        public double etha_air;
        public double dmax;
        public ContourLevel[] contour_level; 
        public void refresh()
        {
            this.dmax = 10;
            this.M_air = 0.0289644;
            this.M_gas = 0.004003;
            this.r_earth = 6378000.0;
            this.g0 = 9.81;
            this.p0 = 101325.0;
            this.R = 8.3144621;
            this.roh_air = 1.2409;
            this.roh_helium = 0.1715;
            this.etha_air = 17.1 / 1000000;
            this.contour_level = new ContourLevel[4];
            
            this.contour_level[0].h = 0.0;
            this.contour_level[0].T = 15.0 + 273.15;
            this.contour_level[0].a = 0.0065;
            this.contour_level[0].p = 101325;
            this.contour_level[0].name = "0 km .. 11 km";

            this.contour_level[1].h = 11000.0;
            this.contour_level[1].T = -56.5 + 273.15;
            this.contour_level[1].a = 0.0;
            this.contour_level[1].p = 22632;
            this.contour_level[1].name = "11 km .. 20 km";

            this.contour_level[2].h = 20000.0;
            this.contour_level[2].T = -56.5 + 273.15;
            this.contour_level[2].a = -0.001;
            this.contour_level[2].p = 5475.18;
            this.contour_level[2].name = "20 km .. 32 km";

            this.contour_level[3].h = 32000.0;
            this.contour_level[3].T = -44.5 + 273.15;
            this.contour_level[3].a = -0.0028;
            this.contour_level[3].p = 868.094;
            this.contour_level[3].name = "32 km .. 47 km";
        }

        internal string[] ToStringArray()
        {
            string[] tempStringArray = new string[11+(this.contour_level.Length * 5)];
            tempStringArray[0] = this.M_air.ToString();
            tempStringArray[1] = this.M_gas.ToString();
            tempStringArray[2] = this.r_earth.ToString();
            tempStringArray[3] = this.g0.ToString();
            tempStringArray[4] = this.p0.ToString();
            tempStringArray[5] = this.R.ToString();
            tempStringArray[6] = this.roh_air.ToString();
            tempStringArray[7] = this.roh_helium.ToString();
            tempStringArray[8] = this.etha_air.ToString();
            tempStringArray[9] = this.dmax.ToString();
            tempStringArray[10] = this.contour_level.Length.ToString();
            for (int i = 0; i < this.contour_level.Length; i++)
            {
                tempStringArray[11 + (i * 5) + 0] = this.contour_level[i].name.ToString();
                tempStringArray[11 + (i * 5) + 1] = this.contour_level[i].a.ToString();
                tempStringArray[11 + (i * 5) + 2] = this.contour_level[i].h.ToString();
                tempStringArray[11 + (i * 5) + 3] = this.contour_level[i].p.ToString();
                tempStringArray[11 + (i * 5) + 4] = this.contour_level[i].T.ToString();
            }
            return tempStringArray;
        }
    }    
    public struct Result
    {
        //public double f_result; //resultierende Kraft
        public double Re; //Reynoldszahl
        public double cw; //Cw-Wert
        //public double f_friction; //Reibkraft
        public double f_lift_static; //statische Auftriebskraft
        public double v0;

    }

    public class Calculator
    {
        #region Variablendeklaration

        Constants _constants;
        Result _result;
        double _hmax;
        double _V0;
        double _d0;
        double _T_akt;
        double _p_akt;
        double _m_balloon;
        double _m_use;
        double _v;
        double _pn;
        double _an;
        double _Tn;
        double _hn;
        double _Vmax;
        int _contour_level;
        string _exportPfad;

        #endregion
        #region Get-routienen

        public Result result { get { return this._result; } }
        public Constants constants { get { return this._constants; } }
        public double hmax { get { return this._hmax; } }
        public double V0 { get { return this._V0; } }
        public double d0 { get { return this._d0; } }
        public double T_akt { get { return this._T_akt; } }
        public double p_akt { get { return this._p_akt; } }
        public double m_ballon { get { return this._m_balloon; } }
        public double m_use { get { return this._m_use; } }
        public double v { get { return this._v; } }
        public string exportPfad { get { return this._exportPfad; } }
        public int counter_level { get { return this._contour_level; } }

        #endregion
        #region Konstruktoren

        /// <summary>
        /// erstellt die Calculator klasse mit Initialwerten
        /// </summary>
        public Calculator()
        {
            this._constants.refresh();
            this._d0 = 2;
            this._V0 = this.calc_V0_from_d0();
            this._Vmax = Math.Pow(this._constants.dmax, 3) / 6 * Math.PI;
            this._T_akt = 15.0;
            this._p_akt = this._constants.p0;
            this._m_balloon = 2.3;
            this._m_use = 2.0;
            this._v = 5;
            this._contour_level = 3;
            this._pn = this._constants.contour_level[this._contour_level].p;
            this._an = this._constants.contour_level[this._contour_level].a;
            this._Tn = this._constants.contour_level[this._contour_level].T;
            this._hn = this._constants.contour_level[this._contour_level].h;
            this._exportPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\";
            if (!Directory.Exists(this._exportPfad)) Directory.CreateDirectory(this._exportPfad);
            this.calc_result();
            this._hmax = this.calc_hmax_from_V0();
            this.saveToFile();
        }


        /// <summary>
        /// erstellt die Calculator klasse mithilfe der angegebenen datei;
        /// </summary>
        /// <param name="pfad_constantdata">pfad zur datei mit den hinterlegten Werten. "standart" sucht im Standartpfad</param>
        public Calculator(string pfad_data)
        {
            string pfad;
            if (pfad_data == "standart") { pfad = AppDomain.CurrentDomain.BaseDirectory + "data\\calcdata.st1"; }
            else { pfad = pfad_data; }
            if (!File.Exists(pfad))
            {
                this._constants.refresh();
                this._d0 = 2;
                this._V0 = this.calc_V0_from_d0();
                this._Vmax = Math.Pow(this._constants.dmax, 3) / 6 * Math.PI;
                this._T_akt = 15.0;
                this._p_akt = this._constants.p0;
                this._m_balloon = 2.3;
                this._m_use = 2.0;
                this._v = 5;
                this._contour_level = 3;
                this._pn = this._constants.contour_level[this._contour_level].p;
                this._an = this._constants.contour_level[this._contour_level].a;
                this._Tn = this._constants.contour_level[this._contour_level].T;
                this._hn = this._constants.contour_level[this._contour_level].h;
                this._exportPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\";
                if (!Directory.Exists(this._exportPfad)) Directory.CreateDirectory(this._exportPfad);
                this._hmax = this.calc_hmax_from_V0();
                this.calc_result();
            }
            else
            {
                try
                {
                    string[] Lines = File.ReadAllLines(pfad);
                    this.CreateFromStringArray(Lines);
                    this._exportPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\";
                    this.calc_result();
                }
                catch (Exception)
                {
                    //MessageBox.Show("fehler beim Laden der Datei \n Standartwerte werden generiert","Fehler",MessageBoxButton.OK);
                    this._constants.refresh();
                    this._d0 = 2;
                    this._V0 = this.calc_V0_from_d0();
                    this._Vmax = 1000 / 6 * Math.PI;
                    this._T_akt = 15.0;
                    this._p_akt = this._constants.p0;
                    this._m_balloon = 2.3;
                    this._m_use = 2.0;
                    this._v = 5;
                    this._contour_level = 3;
                    this._pn = this._constants.contour_level[this._contour_level].p;
                    this._an = this._constants.contour_level[this._contour_level].a;
                    this._Tn = this._constants.contour_level[this._contour_level].T;
                    this._hn = this._constants.contour_level[this._contour_level].h;
                    this.calc_result();
                    this._hmax = this.calc_hmax_from_V0();
                    this._exportPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\";
                    if (!Directory.Exists(this._exportPfad)) Directory.CreateDirectory(this._exportPfad);
                    this.saveToFile();
                }                
            }
        }
        #endregion
        #region Werteänderung
        
        public void refreshConstants(Constants conatants)
        {
            this._constants = conatants;
            this._hmax = this.calc_hmax_from_V0();
            this.calc_result();
        }
        public void new_hmax(double new_hmax)
        {
            this._hmax = new_hmax;
            this._V0 = this.calc_V0_from_hmax();
            this._d0 = this.calc_d0_from_V0();
            this.saveToFile();
        }
        public void new_V0(double new_V0)
        {
            this._V0 = new_V0;
            this._d0 = this.calc_d0_from_V0();
            this._hmax = this.calc_hmax_from_V0();
            this.saveToFile();
        }
        public void new_d0(double new_d0)
        {
            this._d0 = new_d0;
            this._V0 = this.calc_V0_from_d0();
            this._hmax = this.calc_hmax_from_V0();
            this.saveToFile();
        }
        public void new_T_akt(double new_T_akt)
        {
            //TODO:
        }
        public void new_p_akt(double new_p_akt)
        {
            //TODO:
        }
        public void new_m_Balloon(double new_m_Balloon)
        {
            //TODO:
        }
        public void new_m_use(double new_m_use)
        {
            //TODO:
        }
        
        #endregion
        #region interne Funktionen

        private void saveToFile()
        {
            string filePfad = this._exportPfad + "calcdata.st1";
            List<string> lines = new List<string>();
            lines.Add(this._hmax.ToString());
            lines.Add(this._V0.ToString());
            lines.Add(this._d0.ToString());
            lines.Add(this._T_akt.ToString());
            lines.Add(this._p_akt.ToString());
            lines.Add(this._m_balloon.ToString());
            lines.Add(this._m_use.ToString());
            lines.Add(this._v.ToString());
            lines.Add(this._pn.ToString());
            lines.Add(this._an.ToString());
            lines.Add(this._Tn.ToString());
            lines.Add(this._hn.ToString());
            lines.Add(this._Vmax.ToString());
            lines.Add(this._contour_level.ToString());
            string[] lines_constants = this._constants.ToStringArray();
            for (int i = 0; i < lines_constants.Length; i++)
            {
                lines.Add(lines_constants[i]);
            }
            File.WriteAllLines(filePfad, lines);
        }
        private void CreateFromStringArray(string[] Lines)
        {
            this._hmax = Convert.ToDouble(Lines[0]);
            this._V0 = Convert.ToDouble(Lines[1]);
            this._d0 = Convert.ToDouble(Lines[2]);
            this._T_akt = Convert.ToDouble(Lines[3]);
            this._p_akt = Convert.ToDouble(Lines[4]);
            this._m_balloon = Convert.ToDouble(Lines[5]);
            this._m_use = Convert.ToDouble(Lines[6]);
            this._v = Convert.ToDouble(Lines[7]);
            this._pn = Convert.ToDouble(Lines[8]);
            this._an = Convert.ToDouble(Lines[9]);
            this._Tn = Convert.ToDouble(Lines[10]);
            this._hn = Convert.ToDouble(Lines[11]);
            this._Vmax = Convert.ToDouble(Lines[12]);
            this._contour_level = Convert.ToInt32(Lines[13]);
            this._constants.M_air = Convert.ToDouble(Lines[14]);
            this._constants.M_gas = Convert.ToDouble(Lines[15]);
            this._constants.r_earth = Convert.ToDouble(Lines[16]);
            this._constants.g0 = Convert.ToDouble(Lines[17]);
            this._constants.p0 = Convert.ToDouble(Lines[18]);
            this._constants.R = Convert.ToDouble(Lines[19]);
            this._constants.roh_air = Convert.ToDouble(Lines[20]);
            this._constants.roh_helium = Convert.ToDouble(Lines[21]);
            this._constants.etha_air = Convert.ToDouble(Lines[22]);
            this._constants.dmax = Convert.ToDouble(Lines[23]);
            this._constants.contour_level = new ContourLevel[Convert.ToInt32(Lines[24])];
            for (int i = 0; i < this._constants.contour_level.Length; i++)
            {
                this._constants.contour_level[i].name = Lines[25 + i * 5 + 0];
                this._constants.contour_level[i].a = Convert.ToDouble(Lines[25 + i * 5 + 1]);
                this._constants.contour_level[i].h = Convert.ToDouble(Lines[25 + i * 5 + 2]);
                this._constants.contour_level[i].p = Convert.ToDouble(Lines[25 + i * 5 + 3]);
                this._constants.contour_level[i].T = Convert.ToDouble(Lines[25 + i * 5 + 4]);
            }
        }
        #endregion
        #region rechnerfunktionen
        private double calc_V0_from_d0()
        {
            return 1.0 / 6.0 * Math.PI * Math.Pow(this._d0,3) ;
        }
        private double calc_V0_from_hmax()
        {
            double hx = (this._constants.r_earth * this._hmax)/(this._constants.r_earth - this._hmax);
            double potenz = 1.0 - ((hx - this._hn )*this._an/this._Tn);
            double exponent = ((this._constants.M_air * this._constants.g0) / (this._constants.R * this._an));
            return _pn * _Vmax / this._constants.p0 * Math.Pow(potenz, exponent);
        }
        private double calc_d0_from_V0()
        {
            return Math.Pow(6.0 * this._V0 / Math.PI, 1.0 / 3);
        }
        private double calc_hmax_from_V0()
        {
            this._Vmax = Math.Pow(this._constants.dmax, 3) / 6 * Math.PI;
            this._pn = this._constants.contour_level[this._contour_level].p;
            this._an = this._constants.contour_level[this._contour_level].a;
            this._Tn = this._constants.contour_level[this._contour_level].T;
            this._hn = this._constants.contour_level[this._contour_level].h;
            double potenz = (this._constants.p0 * this._V0) / (this._pn * this._Vmax);
            double exponent = (this._constants.R * this._an) / (this._constants.M_air * this._constants.g0);
            double hx = ((this._Tn / this._an) * (1 - Math.Pow(potenz, exponent))) + this._hn;
            return (this._constants.r_earth / (this._constants.r_earth + hx)) * hx;
        }
        private double calc_cw()
        {
            return (24.0 / this._result.Re) + (6.0 / (1 + Math.Sqrt(this._result.Re))) + 0.4;
        }
        private double calc_Re()
        {
            return ((this._constants.roh_air * this._v * this._d0) / this._constants.etha_air);
        }
        /*private double calc_f_friction()
        {
            return this.result.cw * (this._constants.roh_air / 2) * (Math.PI / 4.0 * Math.Pow(6.0 * this._V0 / Math.PI, 2 / 3)) * this._result.v0 * this._result.v0;
        }*/
        private double calc_f_lift_static()
        {
            double roh_air_akt = (this._p_akt * this._constants.M_air) / (this._constants.R * (273.15 + this._T_akt));
            double roh_He_akt = (this._p_akt * this._constants.M_gas) / (this._constants.R * (273.15 + this._T_akt));
            return ((roh_air_akt - roh_He_akt) * this._V0 - this._m_balloon - this._m_use) * this._constants.g0;
        }
        /*private double calc_f_result()
        {
            return this._result.f_lift_static - this._result.f_friction;
        }*/
        private double calc_v0_ber()
        {
            return Math.Sqrt(this._result.f_lift_static / (this.result.cw * (this._constants.roh_air / 2) * (Math.PI / 4.0 * Math.Pow(6.0 * this._V0 / Math.PI, 2 / 3))));
        }
        private void calc_result()
        {
            this._result.f_lift_static = this.calc_f_lift_static();
            if (this._result.v0 == 0 | this._v == 0)
            {
                this._result.v0 = 5;
                this._v = this._result.v0;
            }
            for (int i = 0; i < 10; i++)
            {
                this._result.Re = this.calc_Re();
                this._result.cw = this.calc_cw();
                this._result.v0 = this.calc_v0_ber();
                this._v = this._result.v0;
            }
            //this._result.f_friction = this.calc_f_friction();
            //this._result.f_result = this.calc_f_result();
            this.saveToFile();
        }

        #endregion
        


        /// <summary>
        /// Berechnungen werden aktualisiert und gespeichert
        /// </summary>
        /// <param name="t_akt"></param>
        /// <param name="p_akt"></param>
        /// <param name="m_balloon"></param>
        /// <param name="m_last"></param>
        /// <param name="v_0"></param>
        /// <param name="counter_level"></param>
       
        public void newcalc(double t_akt, double p_akt, double m_balloon, double m_last, double v_0, int counter_level)
        {
            this._T_akt = t_akt;
            this._p_akt = p_akt;
            this._m_balloon = m_balloon;
            this._m_use = m_last;
            this._v = v_0;
            this._contour_level = counter_level;
            this._pn = this._constants.contour_level[this._contour_level].p;
            this._an = this._constants.contour_level[this._contour_level].a;
            this._Tn = this._constants.contour_level[this._contour_level].T;
            this._hn = this._constants.contour_level[this._contour_level].h;
            this._hmax = this.calc_hmax_from_V0();
            this.calc_result();
        }
    }
}
