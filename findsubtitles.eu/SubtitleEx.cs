using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubtitleDownloader.Core;

namespace findsubtitles.eu
{
  class SubtitleEx : Subtitle
  {
    public ISubtitleEntryParser Parser { get; protected set; }
    
    public SubtitleEx(string id, string programName, string fileName, string languageCode, ISubtitleEntryParser parser) : base(id, programName, fileName, languageCode)
    {
      Parser = parser;
    }

  }
}
