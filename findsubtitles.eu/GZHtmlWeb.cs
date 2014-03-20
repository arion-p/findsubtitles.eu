using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace findsubtitles.eu
{
  internal class GZHtmlWeb : HtmlWeb 
  {
    public GZHtmlWeb()
    {
      PreRequest = delegate(HttpWebRequest request)
      {
        request.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
        return true;
      };
    }
  }
}
