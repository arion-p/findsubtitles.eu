using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubtitleDownloader.Core;

namespace findsubtitles.eu
{
  internal class MovieDefaultSubEntryParser : ISubtitleEntryParser
  {
    private const string DownloadPageUrl = "http://www.findsubtitles.eu/get.php?id=";

    public string XPath
    {
      get { return "//div[@class='search_release']/a"; }
    }

    public string GetId(string href)
    {
      int lastSlash = href.LastIndexOf('/');
      return (lastSlash >= 0) ? href.Substring(lastSlash + 1) : null;
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
