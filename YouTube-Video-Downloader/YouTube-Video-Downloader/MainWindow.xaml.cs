using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace YouTube_Video_Downloader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            TxtBox_URL.IsEnabled = false;
        }
    }
}
