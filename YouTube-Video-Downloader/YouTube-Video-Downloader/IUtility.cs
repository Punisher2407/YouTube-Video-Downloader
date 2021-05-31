using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTube_Video_Downloader
{
    public interface IUtility
    {
        bool CorrectUrl(string url);
        string GetVideoInfoUrl(string url);
        string GetStreamInfo(string videoGetInfoUrl);
        void CopyBuf(string valueUrl);
        string GetPathForDownload(string PathForDownload);
    }
}
