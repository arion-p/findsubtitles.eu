using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
using SubtitleDownloader.Core;
using SubtitleDownloader.Util;
using findsubtitles.eu;


namespace SubtitleDownloader.Implementations.FindSubtitlesEU
{
  public class FindSubtitlesEUDownloader : ISubtitleDownloader
  {
    //private const string DownloadPageUrl = "http://www.findsubtitles.eu/get.php?id=";
    //private const string DownloadPageUrlGreek = "http://www.findsubtitles.eu/getp.php?id=";
    private const string SearchUrlBase = "http://www.findsubtitles.eu/search.php?text=";
    private const string SearchSeriesUrlBase = "http://www.tvsubtitles.eu/episode_subs.php";
    //private readonly string baseUrl = "http://www.findsubtitles.eu";
    //private XmlTextReader xmlReader;
    //private XmlDocument xmlDoc;
    private int searchTimeout;

    private readonly string[] _subtitleExtensions = new[] {".srt", ".sub"};
    private readonly string[] _archiveExtensions = new[] { ".zip", ".rar" };

    private readonly IDictionary<string, ISubtitleEntryParser> MovieParsers =
      new Dictionary<string, ISubtitleEntryParser>()
        {
          {"default", new MovieDefaultSubEntryParser()},
          {"gre", new MovieGreekSubEntryParser()}
        };

    private readonly IDictionary<string, ISubtitleEntryParser> SeriesParsers =
      new Dictionary<string, ISubtitleEntryParser>()
        {
          {"default", new SeriesDefaultSubEntryParser()}
        };

    public string GetName()
    {
      return "FindSubtitles.eu";
    }

    public List<Subtitle> SearchSubtitles(SearchQuery query)
    {
      string url = SearchUrlBase + Uri.EscapeDataString(query.Query);

      //if (query.Year.HasValue)
      //{
      //  url += "&sY=" + query.Year;
      //}
      return Search(url, query, MovieParsers);
    }

    public List<Subtitle> SearchSubtitles(EpisodeSearchQuery query)
    {
      //throw new NotSupportedException();
      string url = SearchSeriesUrlBase
                   + "?t=" + Uri.EscapeDataString(query.SerieTitle)
                   + "&s=" + query.Season
                   + "&e=" + query.Episode;

      return Search(url, query, SeriesParsers);
    }

    public List<Subtitle> SearchSubtitles(ImdbSearchQuery query)
    {
      throw new NotSupportedException();
    }

    public List<FileInfo> SaveSubtitle(Subtitle subtitle)
    {
      //var downloadBaseUrl = subtitle.LanguageCode == "gre" ? DownloadPageUrlGreek : DownloadPageUrl;
      //string url = downloadBaseUrl + subtitle.Id;
      var subEx = subtitle as SubtitleEx;
      if (subEx == null)
        throw new Exception("No download link found for subtitle!");

      var uri = new Uri(subEx.Parser.GetDownloadUrl(subEx.Id));

      string archiveFile = Path.GetTempFileName();

      var client = new GZipWebClient();
      client.DownloadFile(uri, archiveFile);
      // Handle meta-refresh
      if (client.ResponseHeaders["Content-Type"] == "text/html")
      {
        var web = new GZHtmlWeb();
        HtmlDocument redirectPage = web.Load(archiveFile);
        var node = redirectPage.DocumentNode.SelectSingleNode("//meta[@http-equiv='refresh']");
        if (node != null)
        {
          uri = new Uri(uri, node.GetAttributeValue("content", "").Split(new[]{"url="}, 2, StringSplitOptions.None )[1]);
        }
        else
        {
          node = redirectPage.DocumentNode.SelectSingleNode("//body[starts-with(@onload, 'download(')]");
          if(node != null)
          {
            uri = new Uri(uri, node.GetAttributeValue("onload","").Substring(10).TrimEnd('\'',')'));
          }
        }
        if (node != null)
          client.DownloadFile(uri, archiveFile);
      }
      var files = new List<FileInfo>();
      var subs = new List<FileInfo>();
      //if (client.ResponseHeaders["Content-Type"] != "")

      Debug.WriteLine("Content-Type: " + client.ResponseHeaders["Content-Type"]);
      if (client.ResponseHeaders["Content-Type"] == "text/plain")
      {
        var subFilename = Path.Combine(Path.GetDirectoryName(archiveFile), Path.GetFileName(uri.LocalPath));
        File.Move(archiveFile, subFilename);
        subs.Add(new FileInfo(subFilename));
      }
      else if (client.ResponseHeaders["Content-Type"] == "text/html")
        return subs;
      else
        files.Add(new FileInfo(archiveFile));

      while (!subs.Any() && files.Any())
      {
        var innerContents = new List<FileInfo>();
        innerContents = files.Where(fi => fi.Length > 0)
                             .Aggregate(innerContents,(current, fi) =>
                                        current.Concat(FileUtils.ExtractFilesFromZipOrRarFile(fi.FullName)).ToList());
        subs = innerContents.Where(fi => _subtitleExtensions.Contains(fi.Extension)).ToList();
        files = innerContents.Where(fi => _archiveExtensions.Contains(fi.Extension)).ToList();
      }

      return subs;
    }

    public int SearchTimeout
    {
      get { return searchTimeout; }
      set { searchTimeout = value; }
    }

    private List<Subtitle> Search(string url, SubtitleSearchQuery query, IDictionary<string, ISubtitleEntryParser> parsers)
    {
      var results = new List<Subtitle>();

      foreach (string languageCode in query.LanguageCodes)
      {

        string languageName = Languages.GetLanguageName(languageCode);

        string searchUrl = url + "&lang=" + languageName;
        //xmlReader = new XmlTextReader(url);

        var web = new GZHtmlWeb();
        HtmlDocument resultsPage = web.Load(searchUrl);
        ISubtitleEntryParser parser;
        if(!parsers.TryGetValue(languageCode, out parser))
        {
          if (!parsers.TryGetValue("default", out parser))
            continue;
        }

        string nodesXPpath = parser.XPath;
        HtmlNodeCollection subtitleNodes = resultsPage.DocumentNode.SelectNodes(nodesXPpath);

        if (subtitleNodes  != null)
        {
          foreach (HtmlNode subtitleNode in subtitleNodes)
          {
            string href = subtitleNode.GetAttributeValue("href", "");
            string subtitleId = parser.GetId(href);
            string releaseName = subtitleNode.InnerText;
            //string downloadUrl = parser.GetDownloadUrl(subtitleId);

            if (!String.IsNullOrEmpty(releaseName) && !String.IsNullOrEmpty(subtitleId) /*&& !String.IsNullOrEmpty(downloadUrl)*/)
            {
              results.Add(parser.CreateSub(subtitleId, releaseName, releaseName, languageCode));
            }
          }
        }
      }
      return results;
    }

  }
}
