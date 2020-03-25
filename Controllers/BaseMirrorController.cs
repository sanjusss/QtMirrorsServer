﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace QtMirrorsServer.Controllers
{
    public abstract class BaseMirrorController : Controller
    {
        public abstract string TargetUrl { get; }

        [HttpGet("{*.}")]
        public async Task<object> Get()
        {
            if (Request.Path.HasValue == false)
            {
                return NotFound();
            }

            int prefix = Request.RouteValues["controller"].ToString().Length + 1;
            if (Request.Path.Value.Length == prefix)
            {
                return RedirectPermanent(Request.Path.Value + "/");
            }

            prefix += 1;
            string targerUrl = TargetUrl;
            if (targerUrl.EndsWith('/') == false)
            {
                targerUrl += '/';
            }

            string path = targerUrl + Request.Path.Value.Substring(prefix);
            if (path.EndsWith(".xml"))
            {
                string content;
                using (HttpClient client = new HttpClient())
                {
                    content = await client.GetStringAsync(path);
                }

                var mid = new StringBuilder()
                            .Append(Request.Scheme)
                            .Append("://")
                            .Append(Request.Host)
                            .Append(Request.PathBase)
                            .Append(Request.Path.Value.Substring(0, prefix)).ToString();
                content = content.Replace("http://download.qt-project.org/", mid);
                return Content(content, "text/xml");
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    var res = await client.GetAsync(path, HttpCompletionOption.ResponseHeadersRead);
                    if (res.Content.Headers.ContentType.MediaType == "text/html")
                    {
                        string content = await res.Content.ReadAsStringAsync();
                        var mid = new StringBuilder()
                                    .Append(Request.Scheme)
                                    .Append("://")
                                    .Append(Request.Host)
                                    .Append(Request.PathBase)
                                    .Append(Request.Path.Value.Substring(0, prefix)).ToString();
                        content = content.Replace(targerUrl, mid);
                        return Content(content, "text/html");
                    }
                }
#if DEBUG
                return path;
#else
                return Redirect(path);
#endif
            }
        }
    }
}
