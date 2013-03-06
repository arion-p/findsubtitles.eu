using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubtitleDownloader.Core;

namespace findsubtitles.eu
{
  internal class MovieGreekSubEntryParser : ISubtitleEntryParser
  {
    private const string DownloadPageUrl = "http://www.findsubtitles.eu/getp.php?id=";

    public string XPath
    {
      get { return "//td[@class='result_top_k']/a"; }
    }

    public string GetId(string href)
    {
      int idPos = href.LastIndexOf("id=");
      return (idPos >= 0) ? href.Substring(idPos + 3) : null;
    }

    public string GetDownloadUrl(string subtitleId)
    {
      return DownloadPageUrl + subtitleId;
    }

    public Subtitle CreateSub(string subtitleId, string programName, string fileName, string languageCode)
    {
      return new Subtitle(subtitleId, programName, fileName, languageCode);
    }
  }
}
