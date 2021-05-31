using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace YouTube_Video_Downloader
{
    public class Utility : IUtility
    {
        public bool CorrectUrl(string url)
        {
            bool IsCorrect = false;
            string pattern = @"youtu(?:\.be|be\.com)/(?:(.*)v(/|=)|(.*/)?)([a-zA-Z0-9-_]+)";
            if (Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase))
            {
                IsCorrect = true;
            }
            else
            {
                IsCorrect = false;
            }
            return IsCorrect;
        }

        public string GetVideoInfoUrl(string url)
        {
            Uri videoUri = new Uri(url);
            string videoID = HttpUtility.ParseQueryString(videoUri.Query).Get("v");
            string videoGetInfoUrl = "https://www.youtube.com/get_video_info?video_id="
                + videoID + "&ps=default&eurl=https%253A%2F%2Fyoutube.com%2Fwatch%253Fv%253D2lAe1cqCOXo&hl=en_US&html5=1";
            return videoGetInfoUrl;
        }

        public string GetStreamInfo(string videoGetInfoUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(videoGetInfoUrl);
            request.Timeout = 5000;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream);
            string StreamReader = reader.ReadToEnd();
            string videoInfo = HttpUtility.UrlDecode(StreamReader);
            return videoInfo;
        }
        public void CopyBuf(string valueUrl)
        {
            var data = new System.Windows.Forms.DataObject();
            data.SetData(System.Windows.DataFormats.UnicodeText, true, valueUrl);
            var thread = new Thread(() => System.Windows.Clipboard.SetDataObject(data, true));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
        }
        public string GetPathForDownload(string PathForDownload)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Chose the folder for succsses download.";
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PathForDownload = fbd.SelectedPath;
                PathForDownload = PathForDownload.Replace(@"\", "/") + "/";
            }
            return PathForDownload;
        }
    }
}
