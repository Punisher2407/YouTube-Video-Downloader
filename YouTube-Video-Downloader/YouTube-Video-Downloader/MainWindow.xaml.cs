using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Forms;

namespace YouTube_Video_Downloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary
    public partial class MainWindow : Window
    {
        public string VideoTitle { get; set; }
        public string PathForDownload { get; set; }
        public Dictionary<string, string> Streams { get; set; }
        public bool IsCorrect { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Lbl_Size.Visibility = Visibility.Hidden;
            Btn_Save.IsEnabled = false;
            Btn_Path.IsEnabled = false;
            Btn_Buf.IsEnabled = false;
            ComboBox_Quality.IsEnabled = false;
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IParse parser = new Parse();
                IUtility utility = new Utility();
                Streams = new Dictionary<string, string>();
                TxtBox_URL.IsEnabled = false;

                IsCorrect = utility.CorrectUrl(TxtBox_URL.Text);
                string videoGetInfoUrl = utility.GetVideoInfoUrl(TxtBox_URL.Text);
                string videoInfo = utility.GetStreamInfo(videoGetInfoUrl);
                string bufVideoInfo = videoInfo;
                videoInfo = parser.ParseInfo(videoInfo);
                Streams = parser.GetStream(videoInfo, ComboBox_Quality, Streams);
                bufVideoInfo = parser.GetVideoDetails(bufVideoInfo);
                VideoTitle = parser.GetTitle(bufVideoInfo);
                TxtBox_Title.Text = VideoTitle;
                ComboBox_Quality.IsEnabled = true;
                Btn_Connect.IsEnabled = false;
            }
            catch (System.Net.WebException)
            {
                System.Windows.MessageBox.Show("No internet connection.");
                return;
            }
            catch (Exception ex)
            {
                if (IsCorrect == false)
                {
                    System.Windows.MessageBox.Show("Incorrect URL.");
                    Btn_Connect.IsEnabled = true;
                    TxtBox_URL.IsEnabled = true;
                }
                else
                {
                    if (Streams.Count == 0)
                    {
                        System.Windows.MessageBox.Show("Stream doesn't contain audio support.");
                        Btn_Connect.IsEnabled = true;
                        TxtBox_URL.IsEnabled = true;
                    }
                    else
                    {
                        Btn_Connect.IsEnabled = true;
                        TxtBox_URL.IsEnabled = true;
                        System.Windows.MessageBox.Show(ex.Message);
                        return;
                    }
                }
            }
        }

        private async void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            string valueUrl = string.Empty;
            if ((Streams.TryGetValue(ComboBox_Quality.SelectedValue.ToString(), out valueUrl)) && 
                (ComboBox_Quality.SelectedValue.ToString() != null))
            {   
                WebClient webClient = new WebClient();
                Uri downloadAddress = new Uri(valueUrl);
                int index;
                while ((index = VideoTitle.IndexOfAny(Path.GetInvalidFileNameChars())) != -1)
                {
                    VideoTitle = VideoTitle.Remove(index, 1);
                }
                PathForDownload += VideoTitle + ".mp4";
                webClient.DownloadProgressChanged += (s, a) => { PB_Download.Value = a.ProgressPercentage; Lbl_StartDownload.Content = a.ProgressPercentage.ToString()+"%"; };
                await webClient.DownloadFileTaskAsync(downloadAddress, PathForDownload);
                if (PB_Download.Value == 100)
                {
                    System.Windows.MessageBox.Show("Successfuly downloaded!");
                }
                PathForDownload = string.Empty;
            }
        }

        private void Btn_Path_Click(object sender, RoutedEventArgs e)
        {
            IUtility utility = new Utility();
            PathForDownload = utility.GetPathForDownload(PathForDownload);
            Btn_Save.IsEnabled = true;
        }

        private void Button_Buf_Click(object sender, RoutedEventArgs e)
        {
            IUtility utility = new Utility();
            string valueUrl = string.Empty;
            if (ComboBox_Quality.SelectedItem != null)
            {
                if ((Streams.TryGetValue(ComboBox_Quality.SelectedValue.ToString(), out valueUrl)) &&
                (ComboBox_Quality.SelectedValue.ToString() != null))
                {
                    utility.CopyBuf(valueUrl);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Buffer is empty.");
            }
        }

        private void Main_Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((PB_Download.Value < 100) && (PB_Download.Value != 0))
            {
                if (System.Windows.MessageBox.Show("Are you sure you want to close the YouTube-Downloader, the video will not be uploaded until the end?",
        "Close?", MessageBoxButton.YesNoCancel,
        MessageBoxImage.Question, MessageBoxResult.Cancel) != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void ComboBox_Quality_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) // добавить проверку на пустые значения
        {
            string valueUrl = string.Empty;
            if ((Streams.TryGetValue(ComboBox_Quality.SelectedValue.ToString(), out valueUrl)) &&
                (ComboBox_Quality.SelectedValue.ToString() != null))
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(valueUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                request.Timeout = 5000;
                long bytes = response.ContentLength;
                string[] suffix = { "B", "KB", "MB", "GB", "TB" };
                int i;
                double dblSByte = response.ContentLength;
                for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
                {
                    dblSByte = bytes / 1024.0;
                }
                Lbl_Size.Content = String.Format("{0:0.##} {1}", dblSByte, suffix[i]);
            }
            Lbl_Size.Visibility = Visibility.Visible;
            Btn_Path.IsEnabled = true;
            Btn_Buf.IsEnabled = true;
        }

        private void MenuItem_Help_Click(object sender, RoutedEventArgs e)
        {
            Help helpWindow = new Help();
            helpWindow.ShowDialog();
        }
    }
}
