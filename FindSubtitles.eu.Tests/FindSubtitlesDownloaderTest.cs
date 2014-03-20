using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SubtitleDownloader.Implementations.FindSubtitlesEU;
using SubtitleDownloader.Core;

namespace FindSubtitlesEUDownloaderTests
{
  [TestClass]
  public class FindSubtitlesDownloaderTest
  {
    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }


    [TestMethod]
    public void SaveSubtitleTestGreek()
    {
      var target = new FindSubtitlesEUDownloader();

      SearchQuery query = new SearchQuery("men in black") {LanguageCodes = new[] {"gre"}};
      List<Subtitle> subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0);
      Debug.WriteLine("Subtitles count = {0}", subtitles.Count);

      List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[0]);

      Assert.IsTrue(fileInfos[0].Exists);
      Debug.WriteLine(fileInfos[0].FullName);

      query = new SearchQuery("nord") { LanguageCodes = new[] { "gre" } };
      subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0);
      Debug.WriteLine("Subtitles count = {0}", subtitles.Count);

      fileInfos = target.SaveSubtitle(subtitles[0]);

      Assert.IsTrue(fileInfos[0].Exists);
      Debug.WriteLine(fileInfos[0].FullName);
    }

    [TestMethod]
    public void SaveSubtitleTestEnglish()
    {
      var target = new FindSubtitlesEUDownloader();

      SearchQuery query = new SearchQuery("men in black") { LanguageCodes = new[] { "eng" } };
      List<Subtitle> subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0);

      List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[0]);

      Assert.IsTrue(fileInfos[0].Exists);
    }

    [TestMethod()]
    public void SearchQuerySearchSubtitlesTest()
    {
      var target = new FindSubtitlesEUDownloader();

      SearchQuery query = new SearchQuery("men in black");

      List<Subtitle> actual = target.SearchSubtitles(query);

      Assert.IsTrue(actual.Count > 0);
    }


    [TestMethod()]
    public void EmptySearchQuerySearchSubtitlesTest()
    {
      var target = new FindSubtitlesEUDownloader();

      SearchQuery query = new SearchQuery("foobarnotfound");

      List<Subtitle> actual = target.SearchSubtitles(query);

      Assert.IsTrue(actual.Count == 0);
    }


    [TestMethod]
    public void SaveNestedSubtitleTestGreek()
    {
      var target = new FindSubtitlesEUDownloader();

      SearchQuery query = new SearchQuery("The Belly of an Architect") { LanguageCodes = new[] { "gre" } };
      List<Subtitle> subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 1);

      List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[1]);

      Assert.IsTrue(fileInfos[0].Exists);
      Assert.AreEqual(fileInfos[0].Extension,".sub","Downloaded file is not a subtitle");
      Debug.WriteLine(fileInfos[0].FullName);

    }

    [TestMethod()]
    public void EpisodeSearchQuerySearchSubtitlesTestGreek()
    {
      var target = new FindSubtitlesEUDownloader();

      var query = new EpisodeSearchQuery("Monk", 8, 16) {LanguageCodes = new[] {"gre"}};

      List<Subtitle> actual = target.SearchSubtitles(query);

      Assert.IsTrue(actual.Count > 0);
      Debug.WriteLine("Subtitles count = {0}", actual.Count);

      query = new EpisodeSearchQuery("Game of thrones", 2, 5) { LanguageCodes = new[] { "gre" } };

      actual = target.SearchSubtitles(query);

      Assert.IsTrue(actual.Count > 0);
      Debug.WriteLine("Subtitles count = {0}", actual.Count);
    }

    [TestMethod()]
    public void EpisodeSearchQuerySearchSubtitlesTestEnglish()
    {
      var target = new FindSubtitlesEUDownloader();

      var query = new EpisodeSearchQuery("Monk", 8, 16) { LanguageCodes = new[] { "eng" } };

      List<Subtitle> actual = target.SearchSubtitles(query);

      Assert.IsTrue(actual.Count > 0);
    }

    [TestMethod()]
    public void SaveEpisodeSubtitlesTestEnglish()
    {
      var target = new FindSubtitlesEUDownloader();

      var query = new EpisodeSearchQuery("Monk", 8, 16) { LanguageCodes = new[] { "eng" } };
      List<Subtitle> subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0);

      List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[0]);

      Assert.IsTrue(fileInfos[0].Exists);
      Debug.WriteLine(fileInfos[0].FullName);
    }

    [TestMethod()]
    public void SaveEpisodeSubtitlesTestGreek()
    {
      var target = new FindSubtitlesEUDownloader();

      var query = new EpisodeSearchQuery("Monk", 8, 16) { LanguageCodes = new[] { "gre" } };
      List<Subtitle> subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0, "No sutitles returned for Monk S8 E16");

      for (int i = 0; i < subtitles.Count; i++)
      {
        List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[i]);

        if (fileInfos.Count > 0)
          Assert.IsTrue(fileInfos[0].Exists,
                        string.Format("No subtitle files downloaded for Monk S8 E16, option {0} ", i));
      }

      query = new EpisodeSearchQuery("NCIS", 2, 5) { LanguageCodes = new[] { "gre" } };
      subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0, "No sutitles returned for NCIS S2 E5");

      for (int i = 0; i < subtitles.Count; i++)
      {
        List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[i]);

        if (fileInfos.Count > 0)
          Assert.IsTrue(fileInfos[0].Exists, string.Format("No subtitle files downloaded for NCIS S2 E5, option {0}", i));
      }

      query = new EpisodeSearchQuery("Game of thrones", 2, 5) { LanguageCodes = new[] { "gre" } };
      subtitles = target.SearchSubtitles(query);

      Assert.IsTrue(subtitles.Count > 0, "No sutitles returned for Game of Thrones S2 E5");

      for (int i = 0; i < subtitles.Count; i++)
      {
        List<FileInfo> fileInfos = target.SaveSubtitle(subtitles[i]);

        if (fileInfos.Count > 0)
          Assert.IsTrue(fileInfos[0].Exists, string.Format("No subtitle files downloaded for Game of Thrones S2 E5, option {0}", i));
      }


    }


  }

}
