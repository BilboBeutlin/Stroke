using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Stroke_1_ClassLibrary
{
    public class Browserverlauf
    {
        private string _exportPfad;
        private List<string> _log;
        public int maxlength;
        public string[] log
        {
            get
            {
                return this._log.ToArray();
            }
        }
        
        public Browserverlauf(int maxlength)
        {
            this._exportPfad = AppDomain.CurrentDomain.BaseDirectory + "data\\";
            if (!Directory.Exists(this._exportPfad)) Directory.CreateDirectory(this._exportPfad);
            _log = new List<string>();
            this.loadFile();
            this.maxlength = maxlength;
        }

        private void loadFile()
        {
            string filePfad = this._exportPfad + "BrowserLog.st1";
            if (!File.Exists(filePfad)) File.Create(filePfad);
            else
            {
                string[] lines = File.ReadAllLines(filePfad);
                for (int i = 0; i < lines.Length; i++)
                {
                    _log.Add(lines[i]);
                }
            }
        }

        private void saveToFile()
        {
            string filePfad = this._exportPfad + "BrowserLog.st1";
            File.WriteAllLines(filePfad, _log);
        }

        public void Add(string url)
        {
            _log.Add(url);
            while (_log.Count > maxlength & _log.Count > 0)
            {
                _log.RemoveAt(_log.Count - 1);
            }
            saveToFile();
        }
    }
}
