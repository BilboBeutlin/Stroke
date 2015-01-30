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
using Microsoft.Win32;
using System.Data;

namespace Stroke_1_Groundcontrol
{
    public partial class MainWindow : Window
    {
        //Globale Variablen
        Calculator calculator;
        Browserverlauf browserlog;
        LiveData liveFromHtml;
        LiveData liveFromLog;
        setup allSetup;

        System.Windows.Threading.DispatcherTimer timer;

        /// <summary>
        /// Konstruktor des Hauptfensters
        /// </summary>
        public MainWindow()
        {
            this.calculator = new Calculator("standart");
            this.allSetup = new setup("standart");
            this.liveFromHtml = new LiveData("Html");
            this.liveFromLog = new LiveData("Log");

            InitializeComponent();
            this.FillAllBoxes();
            browserlog = new Browserverlauf(30);
            RefteshBrowserloglist();
            this.label_BrowserStatus.Content = "";
            RunInitSetup();
            
        }

        /// <summary>
        /// ereigniss, welches zyklisch ezeugt wird
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CycleRead(object sender, EventArgs e)
        {
            if (this.Checkbox_ReadFromInternet.IsChecked == true)
            {
                liveFromHtml.Read();
                ShowToGrid();
            }
            if (this.CheckBox_ReadFromLog.IsChecked == true)
            {
                liveFromLog.Read();
                ShowToGrid();
            }
        }

        #region Internal Fuctions

        /// <summary>
        /// Übernahme von einstellungen in die anzeige
        /// </summary>
        private void RunInitSetup()
        {
            this.TextBox_HtmlToRead.Text = allSetup.Link;

            this.Label_LogfileToRead.Content = allSetup.PfathToLogfile;
            this.CheckBox_ReadFromLog.IsChecked = allSetup.readLog;
            this.Checkbox_ReadFromInternet.IsChecked = allSetup.readHtml;
            this.TextBox_RefreshMinute.Text = allSetup.refreshMinute.ToString();
            this.TextBox_RefreshSecounds.Text = allSetup.refreshSecounds.ToString();

            this.liveFromHtml.PfadOrLink = this.allSetup.Link;
            this.liveFromLog.PfadOrLink = this.allSetup.PfathToLogfile;



            TimeSpan refreshtime = new TimeSpan(0, allSetup.refreshMinute, allSetup.refreshSecounds);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = refreshtime;
            timer.IsEnabled = true;
            timer.Tick += CycleRead;
        }

        /// <summary>
        /// editieren der Einstellungen
        /// </summary>
        private void EditSetup()
        {
            try
            {
                this.allSetup.refreshMinute = Convert.ToInt32(this.TextBox_RefreshMinute.Text);
                this.allSetup.refreshSecounds = Convert.ToInt32(this.TextBox_RefreshSecounds.Text);
                TimeSpan refreshtime = new TimeSpan(0, allSetup.refreshMinute, allSetup.refreshSecounds);
                timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = refreshtime;
                timer.IsEnabled = true;
                timer.Tick += CycleRead;
                this.allSetup.save();
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler bei der Übernahme der Aktualisierungszeit");
                throw;
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
        /// Trifft auswahl, welche daten angezeigt werden und gibt diese Vorauswahl an die anzeigefunktion "showAll" weiter
        /// </summary>
        private void ShowToGrid()
        {
            DataTable cleartable = new DataTable();
            this.DataGrid_LiveData.ItemsSource = cleartable.DefaultView;
            if (this.Radiobutton_ShowInternet.IsChecked == true)
            {
                if (liveFromHtml.length > 0)
                {
                    showAll(true);
                }
            }
            else if (this.Radiobutton_ShowLog.IsChecked == true)
            {
                if (liveFromLog.length > 0)
                {
                    showAll(false);
                }
            }
        }

        /// <summary>
        /// liest die daten aus der LiveData-Klasse aus und zeigt diese auf dem DataGrid
        /// </summary>
        /// <param name="ShowInternet">true -> zeige daten aus dem Internet; 
        /// false -> zeige Daten von der Log-Datei</param>
        private void showAll(bool ShowInternet)
        {
            this.DataGrid_LiveData.Columns.Clear();
            String[,] show;
            if (ShowInternet)
            {
                show = liveFromHtml.DatalistToStringArray();
            }
            else
            {
                show = liveFromLog.DatalistToStringArray();
            }
            DataTable tempTable = new DataTable();
            DataColumn col = new DataColumn("nr", typeof(int));
            col.ReadOnly = true;
            tempTable.Columns.Add(col);
            col = new DataColumn("time", typeof(string));
            col.ReadOnly = true;
            tempTable.Columns.Add(col);
            col = new DataColumn("lat", typeof(string));
            col.ReadOnly = true;
            tempTable.Columns.Add(col);
            col = new DataColumn("lon", typeof(string));
            col.ReadOnly = true;
            tempTable.Columns.Add(col);
            col = new DataColumn("alt", typeof(string));
            col.ReadOnly = true;
            tempTable.Columns.Add(col);

            for (int Zeile = 0; Zeile < show.GetLength(0); Zeile++)
            {
                DataRow row;
                row = tempTable.NewRow();
                row[0] = Zeile;
                row[1] = show[Zeile, 0];
                row[2] = show[Zeile, 1];
                row[3] = show[Zeile, 2];
                row[4] = show[Zeile, 3];
                tempTable.Rows.Add(row);

            }
            //Spalten Zuweisen
            this.DataGrid_LiveData.ItemsSource = tempTable.DefaultView;
        }

        #endregion

        #region input Events

        /// <summary>
        /// Wechsel der Anwahl des RadioButton für den Rechner
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

        /// <summary>
        /// Browserfenster geladen, löschen der statusanzeige
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowserLoadingCompleted(object sender, NavigationEventArgs e)
        {
            this.label_BrowserStatus.Content = "";
        }

        /// <summary>
        /// Speichert, welche date gelesen werden sollen und speichert das für den nächsten Prgrammstart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cahnge_CheckBox_toRead(object sender, RoutedEventArgs e)
        {
            if (this.Checkbox_ReadFromInternet.IsChecked == true)
            {
                this.allSetup.readHtml = true;
            }
            if (this.Checkbox_ReadFromInternet.IsChecked == false)
            {
                this.allSetup.readHtml = false;
            }
            if (this.CheckBox_ReadFromLog.IsChecked == true)
            {
                this.allSetup.readLog = true;
            }
            if (this.CheckBox_ReadFromLog.IsChecked == false)
            {
                this.allSetup.readLog = false;
            }
            this.allSetup.save();

        }

        /// <summary>
        /// Radiobutton wurde geändert, welcher die anzuzeigenden daten angibt.
        /// wirkung: aktualisieren des DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Radiobutton_Schowdata_Cange(object sender, RoutedEventArgs e)
        {
            ShowToGrid();
        }

        #endregion

        #region Button Events

        /// <summary>
        /// Button -> Browser Aktualisieren
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_BrowserRefresh_Click(object sender, RoutedEventArgs e)
        {
            this.StockeBrowser.Refresh();
            this.label_BrowserStatus.Content = "loading";
        }

        /// <summary>
        /// Button -> Einstellungen Übernehmen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CkeckEditSetup(object sender, RoutedEventArgs e)
        {
            this.EditSetup();
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
            double t_akt, p_akt, m_balloon, m_last, v_0;
            int counter_level;
            if (this.ListBox_Hoehenschicht.SelectedIndex >= 0) counter_level = this.ListBox_Hoehenschicht.SelectedIndex;
            else counter_level = this.calculator.counter_level;
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
        /// Button -> Änden des Pfades für die Logdatei
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bottonEvent_ChangeLogpfath(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                allSetup.PfathToLogfile = fileDialog.FileName;
                allSetup.save();
                this.liveFromLog.PfadOrLink = fileDialog.FileName;
                this.Label_LogfileToRead.Content = fileDialog.FileName;
            }
            this.EditSetup();
        }

        /// <summary>
        /// Button-Click-Event zum übernehmen und abspeichern der zu lesenden Internetseite
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_CheckEditSetup_Url(object sender, RoutedEventArgs e)
        {
            this.allSetup.Link = this.TextBox_HtmlToRead.Text;
            this.allSetup.save();
            this.liveFromHtml.PfadOrLink = this.TextBox_HtmlToRead.Text;
        }

        #endregion

    }
}
