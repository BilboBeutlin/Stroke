using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace Stroke_1_ClassLibrary
{
    public class setup
    {
        private int _LanguageNr;
        private string _LanguagePfad;
        private string _SetupPfad;
        public string PfathToLogfile;
        public string Link;
        public int refreshMinute;
        public int refreshSecounds;
        public bool readLog;
        public bool readHtml;

        public setup(string pfad_data, string pfad_languagedata)
        {
            if (pfad_languagedata == "standart") { _LanguagePfad = AppDomain.CurrentDomain.BaseDirectory + "data\\language.st1"; }
            else { _LanguagePfad = pfad_languagedata; }
            if (!File.Exists(_LanguagePfad))
            {
                MessageBox.Show("Sprachdatei konnte nicht gefunden werden");

            }
            if (pfad_data == "standart") { _SetupPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\setupdata.st1"; }
            else { _SetupPfad = pfad_data; }
            if (!File.Exists(_SetupPfad))
            {
                MessageBox.Show("Setupdatei konnte nicht gefunden werden");
                _LanguagePfad = AppDomain.CurrentDomain.BaseDirectory + "data\\language.st1";
                _LanguageNr = 1;
                PfathToLogfile = "none";
                Link = "none";
                refreshMinute = 2;
                refreshSecounds = 0;
                readHtml = false;
                readLog = false;
                save();
            }

        }
        public setup(string pfad_data)
        {
            if (pfad_data == "standart") { _SetupPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\setupdata.st1"; }
            else { _SetupPfad = pfad_data; }
            if (!File.Exists(_SetupPfad))
            {
                MessageBox.Show("Setupdatei konnte nicht gefunden werden");
                _LanguagePfad = AppDomain.CurrentDomain.BaseDirectory + "data\\language.st1";
                _LanguageNr = 1;
                PfathToLogfile = "none";
                Link = "none";
                refreshMinute = 2;
                refreshSecounds = 0;
                readHtml = false;
                readLog = false;
                save();
            }
            else
            {
                read();
            }
        }

        private void read()
        {
            try
            {
                string[] readedLines = File.ReadAllLines(_SetupPfad);
                this._LanguageNr = Convert.ToInt32(readedLines[3]);
                this.PfathToLogfile = readedLines[5];
                this.Link = readedLines[7];
                this.refreshMinute = Convert.ToInt32(readedLines[9]);
                this.refreshSecounds = Convert.ToInt32(readedLines[11]);
                this.readLog = Convert.ToBoolean(readedLines[13]);
                this.readHtml = Convert.ToBoolean(readedLines[15]);
            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim Lesen der Setupdatei");
            } 
        }

        public void save()
        {
            List<string> towrite = new List<string>();
            towrite.Add("###### Setup ######");
            towrite.Add("");
            towrite.Add("# LanguageNr #");
            towrite.Add(_LanguageNr.ToString());
            towrite.Add("# Pfad to Logfile #");
            towrite.Add(PfathToLogfile);
            towrite.Add("# Internetseite mit Daten #");
            towrite.Add(Link);
            towrite.Add("# Aktualisierungszeit, minuten #");
            towrite.Add(refreshMinute.ToString());
            towrite.Add("# Aktualisierungszeit, Sekunden #");
            towrite.Add(refreshSecounds.ToString());
            towrite.Add("# Lesen Logdaten #");
            towrite.Add(readLog.ToString());
            towrite.Add("# Lesen vom Internet #");
            towrite.Add(readHtml.ToString());
            string[] temp = towrite.ToArray();
            File.WriteAllLines(_SetupPfad, temp);
        }
    }
}
