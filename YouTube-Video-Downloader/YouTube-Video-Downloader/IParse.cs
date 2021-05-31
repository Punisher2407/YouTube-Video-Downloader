using System.Collections.Generic;
using System.Windows.Controls;

namespace YouTube_Video_Downloader
{
    public interface IParse
    {
        Dictionary<string, string> GetStream(string videoInfo, ComboBox comboBox, Dictionary<string, string> Stream);
        string UnicodeEncode(string source);
        string GetTitle(string sourse);
        string GetVideoDetails(string videoInfo);
        string ParseInfo(string videoInfo);
    }
}
