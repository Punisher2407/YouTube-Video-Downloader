using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace YouTube_Video_Downloader
{
    public class Parse : Window, IParse
    {
        public string VideoTitle { get; set; }
        public string PathForDownload { get; set; }

        public Dictionary<string, string> GetStream(string videoInfo, ComboBox comboBox, Dictionary<string, string> Stream)
        {
            try
            {
                bool IsKey = false;
                bool IsValue = false;
                string keyQuality = string.Empty;
                string valueUrl = string.Empty;
                Stream = new Dictionary<string, string>();
                JsonTextReader readerS = new JsonTextReader(new StringReader(videoInfo));
                while (readerS.Read())
                {
                    if (readerS.Value != null)
                    {
                        if (IsKey)
                        {
                            if (valueUrl == "")
                            {
                                throw new Exception();
                            }
                            else
                            {
                                keyQuality = readerS.Value.ToString();
                                Stream.Add(keyQuality, valueUrl);
                                comboBox.Items.Add(keyQuality);
                                IsKey = false;
                            }
                        }
                        if (readerS.Value.ToString() == "qualityLabel")
                        {
                            IsKey = true;
                        }
                        if (IsValue)
                        {
                            valueUrl = readerS.Value.ToString();
                            valueUrl = UnicodeEncode(valueUrl);
                            IsValue = false;
                        }
                        if ((readerS.Value.ToString() == "url"))
                        {
                            IsValue = true;
                        }
                    }
                }
                return Stream;
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("URL don't found.");
                throw; 
            }
        }
        public string UnicodeEncode(string source)
        {
            byte[] bytes = Encoding.Default.GetBytes(source);
            source = Encoding.UTF8.GetString(bytes);
            return source;
        }
        public string GetTitle(string source)
        {
            bool IsTitle = false;
            JsonTextReader readerS = new JsonTextReader(new StringReader(source));
            while (readerS.Read())
            {
                if (readerS.Value != null)
                {
                    if (IsTitle)
                    {
                        VideoTitle = readerS.Value.ToString();
                        IsTitle = false;
                    }
                    if (readerS.Value.ToString() == "title")
                    {
                        IsTitle = true;
                    }
                }
            }
            return VideoTitle;
        }
        public string GetVideoDetails(string videoInfo)
        {
            string subString = "\"videoDetails\"";
            int endIndex = videoInfo.IndexOf(subString);
            videoInfo = videoInfo.Remove(0, endIndex);
            videoInfo = videoInfo.Insert(0, "{");

            subString = ",\"averageRating\"";
            int startIndex = videoInfo.IndexOf(subString);
            videoInfo = videoInfo.Remove(startIndex, (videoInfo.Length - startIndex));
            videoInfo += "}" + "}";
            return videoInfo;
        }
        public string ParseInfo(string videoInfo)
        {
            string subString = "\"formats\":";
            int endIndex = videoInfo.IndexOf(subString);
            videoInfo = videoInfo.Remove(0, endIndex);
            videoInfo = videoInfo.Insert(0, "{");

            subString = ",\"adaptiveFormats\"";
            int startIndex = videoInfo.IndexOf(subString);
            videoInfo = videoInfo.Remove(startIndex, (videoInfo.Length - startIndex));

            subString = "\"itag\":";
            startIndex = videoInfo.IndexOf(subString);
            videoInfo = videoInfo.Remove(0, startIndex);
            videoInfo = videoInfo.Insert(0, "{");
            videoInfo = videoInfo.Insert(0, "[");

            return videoInfo;
        }
    }
}
