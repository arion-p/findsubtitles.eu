﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace findsubtitles.eu
{
  internal class GZipWebClient : WebClient
  {

    protected override WebRequest GetWebRequest(Uri address)
    {
      HttpWebRequest request = (HttpWebRequest) base.GetWebRequest(address);
      request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
      return request;
    }

  }
}
