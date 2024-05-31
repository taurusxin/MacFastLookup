using System;
using System.IO;
using System.Windows.Forms;

namespace MacFastLookup
{
    public partial class Updater : Form
    {
        public Updater()
        {
            InitializeComponent();
        }

        string url = Global.config.DownloadUrl;
        string destinationFilePath = Environment.CurrentDirectory + "\\mac_vendors.sqlite3.temp";
        public bool finished = false;

        FileDownloader downloader;

        private void Updater_Load(object sender, EventArgs e)
        {
            StartUpdate();
        }
        private async void StartUpdate()
        {
            downloader = new FileDownloader(url, destinationFilePath, ShowProgress, DownloadCompleted);

            await downloader.StartDownloadAsync();
        }
        private void ShowProgress(double progress)
        {
            if ((int)progress == 100)
            {
                finished = true;
                button1.Text = "Finish";
            }
            progressBar1.Value = (int)(progress);
            label1.Text = $"{Math.Round(progress, 2)}%";
        }

        private void DownloadCompleted(string filePath)
        {
            if (filePath != null)
            {
                Console.WriteLine($"下载完成，文件保存在: {filePath}");
            }
            else
            {
                Console.WriteLine("下载失败");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!finished)
            {
                downloader.CancelDownload();
                Close();
            }
            else
            {
                Close();
            }
        }
    }
}
