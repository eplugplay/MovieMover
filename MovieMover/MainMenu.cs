using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MovieMover;

namespace MovieMover
{
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            rdMove.Checked = true;
        }

        public class data
        {
            public string Source { get; set; }
            public string Destination { get; set; }
            public bool Copy { get; set; }
            public data(string _source, string _destination, bool _copy)
            {
                Source = _source;
                Destination = _destination;
                Copy = _copy;
            }
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {
            if (!File.Exists(Uti.GetFilePath(Uti.FileTypes.ini)))
            {
                File.Copy(Uti.GetLocalFilePath(Uti.FileTypes.ini), Uti.GetFilePath(Uti.FileTypes.ini), true);
            }

            txtSource.Text = Uti.ReadIniFile("Source", "path");
            txtDestination.Text = Uti.ReadIniFile("Destination", "path");
            txtMessage.ReadOnly = true;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            data data = (data)e.Argument;
            MovieMover.MoveData.MoveMovies(data.Source, data.Destination, txtMessage, data.Copy);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnStart.InvokeEx(x => x.Enabled = true);
            gBox.InvokeEx(x => x.Enabled = true);
            MessageBox.Show("Completed!");
        }

        private void btnSourceBrowse_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                return;
            }
            string path = Uti.GetFolderPath(folderBrowserDialog1, txtSource.Text);
            if (path == "")
                return;
            txtSource.Text = path;
            Uti.SaveIniFile("Source", "Path", path);
        }

        private void btnDestinationBrowse_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                return;
            }
            string path = Uti.GetFolderPath(folderBrowserDialog1, txtDestination.Text);
            if (path == "")
                return;
            txtDestination.Text = path;
            Uti.SaveIniFile("Destination", "Path", path);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                return;
            }

            if (MessageBox.Show("Do you want to start?", "Start Moving Movies?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                txtMessage.Clear();
                btnStart.Enabled = false;
                gBox.Enabled = false;
                backgroundWorker1.RunWorkerAsync(new data(txtSource.Text, txtDestination.Text, rdCopy.Checked));
            }
        }
    }
}
