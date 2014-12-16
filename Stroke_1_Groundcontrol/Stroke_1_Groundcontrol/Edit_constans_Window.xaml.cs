using Stroke_1_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Stroke_1_Groundcontrol
{
    /// <summary>
    /// Interaktionslogik für Edit_constans_Window.xaml
    /// </summary>
    public partial class Edit_constans_Window : Window
    {
        public Constants constants;
        private bool _result;
        public bool result { get{ return this._result; } }
        
        /// <summary>
        /// Konstruktor des Fensters
        /// </summary>
        public Edit_constans_Window()
        {
            InitializeComponent();
            this._result = false;
        }

        /// <summary>
        /// füllt die textboxen mit den entsprechenden Werten
        /// </summary>
        private void showConstants()
        {
            this.TextBox_etha_air.Text = this.constants.etha_air.ToString();
            this.TextBox_M_air.Text = this.constants.M_air.ToString();
            this.TextBox_M_gas.Text = this.constants.M_gas.ToString();
            this.TextBox_RE.Text = this.constants.r_earth.ToString();
            this.TextBox_g0.Text = this.constants.g0.ToString();
            this.TextBox_p0.Text = this.constants.p0.ToString();
            this.TextBox_R.Text = this.constants.R.ToString();
            this.TextBox_roh_luft.Text = this.constants.roh_air.ToString();
            this.TextBox_roh_gas.Text = this.constants.roh_helium.ToString();
            this.TextBox_dmax.Text = this.constants.dmax.ToString();
            this.ListBox_contour_level.Items.Clear();
            for (int i = 0; i < this.constants.contour_level.Length; i++)
            {
                this.ListBox_contour_level.Items.Add(this.constants.contour_level[i]);
            }
            this.TextBox_a.Text = "";
            this.TextBox_h.Text = "";
            this.TextBox_p.Text = "";
            this.TextBox_T.Text = "";
            this.TextBox_level_name.Text = "";
        } 

        /// <summary>
        /// Übernahme der Eingetragenen werte und schließen des Fensters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_chec_click(object sender, RoutedEventArgs e)
        {
            this.GetNewConstants();
            this._result = true;
            this.Close();
        }

        /// <summary>
        /// Realisierung des Einlesens der neuen Werte
        /// </summary>
        private void GetNewConstants()
        {
            try
            {
                this.constants.etha_air = Convert.ToDouble(this.TextBox_etha_air.Text);
                this.constants.M_air = Convert.ToDouble(this.TextBox_M_air.Text);
                this.constants.M_gas = Convert.ToDouble(this.TextBox_M_gas.Text);
                this.constants.r_earth = Convert.ToDouble(this.TextBox_RE.Text);
                this.constants.g0 = Convert.ToDouble(this.TextBox_g0.Text);
                this.constants.p0 = Convert.ToDouble(this.TextBox_p0.Text);
                this.constants.R = Convert.ToDouble(this.TextBox_R.Text);
                this.constants.roh_air = Convert.ToDouble(this.TextBox_roh_luft.Text);
                this.constants.roh_helium = Convert.ToDouble(this.TextBox_roh_gas.Text);
                this.constants.dmax = Convert.ToDouble(this.TextBox_dmax.Text);
            }
            catch (Exception)
            {
                
                throw;
            }
            
        }

        /// <summary>
        /// Wenn fenster geladen ist, wird "showConstants()" ausgeführt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ist_loaded(object sender, RoutedEventArgs e)
        {
            this.showConstants();
        }

        /// <summary>
        /// Wechsel der angewählten höhenschicht, anzeige der entsprechenden Werte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cange_level_click(object sender, SelectionChangedEventArgs e)
        {
            if (this.ListBox_contour_level.SelectedIndex >= 0)
            {
                this.TextBox_a.Text = this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].a.ToString();
                this.TextBox_h.Text = this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].h.ToString();
                this.TextBox_p.Text = this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].p.ToString();
                this.TextBox_T.Text = this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].T.ToString();
                this.TextBox_level_name.Text = this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].name;
            }
            
        }

        /// <summary>
        /// Ändern der ausgewählten Höhenschicht(Übernahme der eingetragenen Werte)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_change(object sender, RoutedEventArgs e)
        {
            if (this.ListBox_contour_level.SelectedIndex >= 0)
            {
                this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].a = Convert.ToDouble(this.TextBox_a.Text);
                this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].h = Convert.ToDouble(this.TextBox_h.Text);
                this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].p = Convert.ToDouble(this.TextBox_p.Text);
                this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].T = Convert.ToDouble(this.TextBox_T.Text);
                this.constants.contour_level[this.ListBox_contour_level.SelectedIndex].name = this.TextBox_level_name.Text;
            }
        }

        /// <summary>
        /// Löschen der angewählten Höhenschicht
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_del(object sender, RoutedEventArgs e)
        {
            if (this.ListBox_contour_level.SelectedIndex >= 0)
            {
                ContourLevel[] temp = new ContourLevel[this.constants.contour_level.Length - 1];
                int d = 0;
                for (int i = 0; i < this.constants.contour_level.Length; i++)
                {
                    if(i == this.ListBox_contour_level.SelectedIndex)
                    {
                        d=-1;
                    }
                    else
                    {
                        temp[i+d] = this.constants.contour_level[i];
                    }                    
                }
                this.constants.contour_level = temp;
                this.ListBox_contour_level.Items.Clear();
                for (int i = 0; i < this.constants.contour_level.Length; i++)
                {
                    this.ListBox_contour_level.Items.Add(this.constants.contour_level[i]);
                }
                this.TextBox_a.Text = "";
                this.TextBox_h.Text = "";
                this.TextBox_p.Text = "";
                this.TextBox_T.Text = "";
                this.TextBox_level_name.Text = "";
            }
        }

        /// <summary>
        /// Fügt eine neue Höhenschicht entsprechend den Angaben hinzu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_click_Add_contour_level(object sender, RoutedEventArgs e)
        {
            
            if (this.TextBox_level_name.Text.Length <= 1)
            {
                MessageBox.Show("Fehler bei der eingabe", "Error");
                return;
            }
            try
            {
                ContourLevel temp = new ContourLevel();
                temp.a = Convert.ToDouble(this.TextBox_a.Text);
                temp.h = Convert.ToDouble(this.TextBox_h.Text);
                temp.p = Convert.ToDouble(this.TextBox_p.Text);
                temp.T = Convert.ToDouble(this.TextBox_T.Text);
                temp.name = this.TextBox_level_name.Text;
                List<ContourLevel> templist = new List<ContourLevel>();
                for (int i = 0; i < this.constants.contour_level.Length; i++)
			    {
			        templist.Add(this.constants.contour_level[i]);
			    }
                templist.Add(temp);
                this.constants.contour_level = templist.ToArray();
                this.showConstants();
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler bei der eingabe", "Error");
            }
            
        }       
    }
}
