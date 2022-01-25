using HabrProxy.Services;
using HabrProxy.Services.Static;

namespace HabrProxy.Middlewares
{
    /// <summary>
    /// Middleware, that catches requests, makes additional request from httpClient to the Habr.com
    /// according to the requested path, takes it content and then redirecting to root base controller 'HabrProxy'
    /// and showing modified content
    /// </summary>
    public class RedirectRequestMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            // handling media requests to .svg content
            if (httpContext.Request.Path.Value != null && httpContext.Request.Path.Value.Contains(".svg"))
            {
                var sourceUrl = Constants.OriginalHost + httpContext.Request.Path.Value;

                using (HttpClient client = new())
                {
                    using (HttpResponseMessage response = client.GetAsync(sourceUrl).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (HttpContent content = response.Content)
                            {
                                string result = content.ReadAsStringAsync().Result;
                                Supervisor.ImageContent = result;
                            }
                        }
                    }
                }
                httpContext.Response.ContentType = Constants.ImageContentType;
            }
            // requests to load pages
            else if (httpContext.Request.Path.Value != null && !httpContext.Request.Path.Value.Contains("."))
            {
                string path = string.Empty;
                if (string.IsNullOrEmpty(httpContext.Request.Path.Value) || httpContext.Request.Path.Value == Constants.RedirectToPath)
                    path = Constants.DefaultOriginalPath;
                else path = httpContext.Request.Path.Value;
                var sourceUrl = Constants.OriginalHost + path;

                using (HttpClient client = new())
                {
                    using (HttpResponseMessage response = client.GetAsync(sourceUrl).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (HttpContent content = response.Content)
                            {
                                if (!Supervisor.IsRedirected)
                                {
                                    string result = content.ReadAsStringAsync().Result;
                                    Supervisor.OriginalContent = result;
                                }
                            }
                        }
                        else Supervisor.OriginalContent = null;
                    }
                }

                if (path != Constants.DefaultOriginalPath)
                {
                    httpContext.Response.Redirect(Constants.RedirectToPath);
                    Supervisor.CurrentPath = path;
                    Supervisor.IsRedirected = true;
                    return;
                }
                else Supervisor.IsRedirected = false;
            }

            await _next(httpContext);
        }
    }
}
