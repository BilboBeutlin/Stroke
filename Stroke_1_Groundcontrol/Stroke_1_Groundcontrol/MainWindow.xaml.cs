using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Stroke_1_ClassLibrary;
using System.IO;
using System.Net;

namespace Stroke_1_Groundcontrol
{
    public partial class MainWindow : Window
    {
        //Globale Variablen
        Calculator calculator;
        Browserverlauf browserlog;



//_____________ zum Löschen
        //HtmlReader blubb = new HtmlReader("file:///C:/Users/Dennis/Dropbox/Mechatronik%20Projekt/Team%202%20-%20Dennis/Internetseite%20mit%20beispiel-Rohpacketen/Rohpackete_decodiert/Roh-Pakete%20von%20KD4BFP-8%20%E2%80%93%20Google%20Maps%20APRS.htm");
        

        

        //_____________bis Hier

        /// <summary>
        /// Konstruktor des Hauptfensters
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.calculator = new Calculator("standart");
            this.FillAllBoxes();
            browserlog = new Browserverlauf(30);
            RefteshBrowserloglist();
            this.label_BrowserStatus.Content = "";

        }

        /// <summary>
        /// Button -> Editieren der Konstanten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_editConstants_click(object sender, RoutedEventArgs e)
        {
            Edit_constans_Window editWindow = new Edit_constans_Window();
            editWindow.constants = this.calculator.constants;
            editWindow.ShowDialog();
            if (editWindow.result == true)
            {
                this.calculator.refreshConstants(editWindow.constants);
                this.FillAllBoxes();
            }
        }
        
        /// <summary>
        /// Füllen aller Textboxen mit den entsprechenden Werten
        /// </summary>
        private void FillAllBoxes()
        {
            this.refreshResults();
            this.TextBox_T_akt.Text = (this.calculator.T_akt).ToString();
            this.TextBox_P_akt.Text = this.calculator.p_akt.ToString();
            this.TextBox_m_Ballon.Text = this.calculator.m_ballon.ToString();
            this.TextBox_m_last.Text = this.calculator.m_use.ToString();
            this.ListBox_Hoehenschicht.Items.Clear();
            for (int i = 0; i < this.calculator.constants.contour_level.Length; i++)
            {
                this.ListBox_Hoehenschicht.Items.Add(this.calculator.constants.contour_level[i]);
            }
        }

        /// <summary>
        /// Button -> Neu berechnen mit den Angegebenen Werten
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_refresh_calculate_click(object sender, RoutedEventArgs e)
        {
            if (this.Radio_d0.IsChecked == true)
            {
                double d0;
                try 
	            {	        
		            d0 = Convert.ToDouble(this.TextBox_d0.Text);
                    this.calculator.new_d0(d0);      
	            }
	            catch (Exception)
	            {
		            MessageBox.Show("Fehlerhafte Eingabe");
		            throw;
	            }
                          
            }
            else if (this.Radio_hmax.IsChecked == true)
            {
                double hmax;
                try
                {
                    hmax = Convert.ToDouble(this.TextBox_hmax.Text) * 1000;
                    this.calculator.new_hmax(hmax);
                }
                catch (Exception)
                {
                    MessageBox.Show("Fehlerhafte Eingabe");
                    throw;
                }                
            }
            else if (this.Radio_V0.IsChecked == true)
            {
                double V0;
                try
                {
                    V0 = Convert.ToDouble(this.TextBox_V0.Text);
                    this.calculator.new_V0(V0);
                }
                catch (Exception)
                {
                    MessageBox.Show("Fehlerhafte Eingabe");
                }
            }
            double t_akt,p_akt,m_balloon,m_last,v_0;
            int counter_level;
            if (this.ListBox_Hoehenschicht.SelectedIndex >= 0) counter_level=this.ListBox_Hoehenschicht.SelectedIndex;
            else counter_level=this.calculator.counter_level;
            try
            {
                t_akt = Convert.ToDouble(this.TextBox_T_akt.Text);
                p_akt = Convert.ToDouble(this.TextBox_P_akt.Text);
                m_balloon = Convert.ToDouble(this.TextBox_m_Ballon.Text);
                m_last = Convert.ToDouble(this.TextBox_m_last.Text);
                v_0 = Convert.ToDouble(this.TextBox_v0_calc.Text);
                this.calculator.newcalc(t_akt, p_akt, m_balloon, m_last, v_0, counter_level);

            }
            catch (Exception)
            {
                MessageBox.Show("Fehlerhafte Eingabe");
            }

            this.refreshResults();
        }

        /// <summary>
        /// Füllen der Textboxen mit den Aktuellen ergebnissen
        /// </summary>
        private void refreshResults()
        {
            this.TextBox_hmax.Text = (this.calculator.hmax / 1000).ToString("f");
            this.TextBox_V0.Text = this.calculator.V0.ToString("f");
            this.TextBox_d0.Text = this.calculator.d0.ToString("f");
            this.TextBox_Re.Text = this.calculator.result.Re.ToString("f");
            this.TextBox_cw.Text = this.calculator.result.cw.ToString("f");
            this.TextBox_Fauftrieb_stat.Text = this.calculator.result.f_lift_static.ToString("f");
            this.TextBox_v0_calc.Text = this.calculator.result.v0.ToString("f");
        }

        /// <summary>
        /// Wechsel der Anwahl des RadioButton
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButtonCange(object sender, RoutedEventArgs e)
        {
            try
            {
                this.TextBox_d0.IsEnabled = Convert.ToBoolean(this.Radio_d0.IsChecked);
                this.TextBox_V0.IsEnabled = Convert.ToBoolean(this.Radio_V0.IsChecked);
                this.TextBox_hmax.IsEnabled = Convert.ToBoolean(this.Radio_hmax.IsChecked);
            }
            catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Button -> Zurücksetzen aller werte auf Ausgangszustand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_reset_input_Click(object sender, RoutedEventArgs e)
        {
            this.calculator = new Calculator();
            this.refreshResults();
            this.TextBox_T_akt.Text = (this.calculator.T_akt).ToString();
            this.TextBox_P_akt.Text = this.calculator.p_akt.ToString();
            this.TextBox_m_Ballon.Text = this.calculator.m_ballon.ToString();
            this.TextBox_m_last.Text = this.calculator.m_use.ToString();
            this.TextBox_v0_calc.Text = this.calculator.v.ToString();
            this.ListBox_Hoehenschicht.Items.Clear();
            for (int i = 0; i < this.calculator.constants.contour_level.Length; i++)
            {
                this.ListBox_Hoehenschicht.Items.Add(this.calculator.constants.contour_level[i]);
            }
            this.ListBox_Hoehenschicht.SelectedIndex = this.calculator.counter_level;
        }

        /// <summary>
        /// Neue internetadresse wurde eingegeben und mit "Enter" bestätigt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Textbox_BrowserEntering_enter_new_page(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
               
                if (this.TextBoxBrowserPage.Text.Substring(0, Math.Min(this.TextBoxBrowserPage.Text.Length, 7))=="http://"
                    ||this.TextBoxBrowserPage.Text.Substring(0, Math.Min(this.TextBoxBrowserPage.Text.Length, 7))=="Http://"
                    ||this.TextBoxBrowserPage.Text.Substring(0, Math.Min(this.TextBoxBrowserPage.Text.Length, 7))=="HTTP://")
                {
                    this.StockeBrowser.Source = new Uri(this.TextBoxBrowserPage.Text);
                }
                else
                {
                    this.TextBoxBrowserPage.Text = "http://" + this.TextBoxBrowserPage.Text;
                    this.StockeBrowser.Source = new Uri(this.TextBoxBrowserPage.Text);
                }
                this.label_BrowserStatus.Content = "loading";
                this.browserlog.Add(this.TextBoxBrowserPage.Text);
                RefteshBrowserloglist();
            }            
        }

        /// <summary>
        /// Die logliste des Browsers wird Aktualisiert
        /// </summary>
        private void RefteshBrowserloglist()
        {
            this.TextBoxBrowserPage.Items.Clear();
            for (int i = 0; i < browserlog.log.Length; i++)
            {
                this.TextBoxBrowserPage.Items.Add(browserlog.log[i]);
            }
        }

        /// <summary>
        /// Eine Seite aus dem Verlauf wird angewählt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void select_logged_page(object sender, EventArgs e)
        {
            string temp = this.TextBoxBrowserPage.Text;
            this.StockeBrowser.Source = new Uri(temp);
            this.browserlog.Add(temp);
            RefteshBrowserloglist();
            this.TextBoxBrowserPage.Text = temp;
            this.label_BrowserStatus.Content = "loading";
        }

        private void BrowserLoadingCompleted(object sender, NavigationEventArgs e)
        {
            this.label_BrowserStatus.Content = "";
        }

        private void button_BrowserRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.StockeBrowser.Refresh();
            this.label_BrowserStatus.Content = "loading";
        }
    }
}
