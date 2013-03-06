using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubtitleDownloader.Core;

namespace findsubtitles.eu
{
  interface ISubtitleEntryParser
  {
    string XPath { get; }
    string GetId(string href);
    string GetDownloadUrl(string subtitleId);
    Subtitle CreateSub(string subtitleId, string programName, string fileName, string languageCode);
  }
}
