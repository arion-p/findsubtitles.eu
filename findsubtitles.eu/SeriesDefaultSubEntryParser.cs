using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using SubtitleDownloader.Core;

namespace findsubtitles.eu
{
  internal class SeriesDefaultSubEntryParser : ISubtitleEntryParser
  {
    private static Regex _getIdRegEx = new Regex(@"\?Id=([0-9]+)\&", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    
    public string XPath
    {
      get { return "//div[@class='box_main']/ul/li/a"; }
    }

    public string GetId(string href)
    {
      var res = _getIdRegEx.Match(href);
      return (res != null && res.Groups.Count > 1) ? res.Groups[1].Value : null;
    }

    public string GetDownloadUrl(string subtitleId)
    {
      string downloadPageUrl = "http://www.tvsubtitles.eu/download.php?id=" + subtitleId + "&site=tv";
      var web = new HtmlWeb();
      HtmlDocument resultsPage = web.Load(downloadPageUrl);
      HtmlNode subtitleNode = resultsPage.DocumentNode.SelectNodes("//div[@class='menu_d']/a[span='Download Subtitle']").FirstOrDefault();
      downloadPageUrl = (subtitleNode != null) ? new Uri(new Uri(downloadPageUrl),  subtitleNode.GetAttributeValue("href","")).ToString() : null;
      if (downloadPageUrl == null)
      {
        subtitleNode = resultsPage.DocumentNode.SelectNodes("//div[@id='down_msg']/a").FirstOrDefault();
        downloadPageUrl = (subtitleNode != null)
                            ? new Uri(new Uri(downloadPageUrl), subtitleNode.GetAttributeValue("href", "")).ToString()
                            : null;
      }
      return downloadPageUrl;
    }

    public Subtitle CreateSub(string subtitleId, string programName, string fileName, string languageCode)
    {
      return new Subtitle(subtitleId, programName, fileName, languageCode);
    }
  }
}
