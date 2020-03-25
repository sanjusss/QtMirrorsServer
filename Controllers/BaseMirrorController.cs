using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            string path = TargetUrl + Request.Path.Value.Substring(prefix);
            if (path.EndsWith(".xml"))
            {
                string content;
                using (HttpClient client = new HttpClient())
                {
                    content = await client.GetStringAsync(path);
                }

                content = content.Replace("download.qt-project.org", Request.Host.Value);
                return Content(content, "text/xml");
            }
            else
            {
#if DEBUG
                return path;
#else
                return Redirect(path);
#endif
            }
        }
    }
}
