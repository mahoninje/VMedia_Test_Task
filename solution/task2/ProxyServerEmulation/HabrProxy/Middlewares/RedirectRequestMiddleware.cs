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
            if (httpContext.Request.Path.Value != null && !httpContext.Request.Path.Value.Contains(".")) 
            {
                string path = string.Empty;
                if (string.IsNullOrEmpty(httpContext.Request.Path.Value) || httpContext.Request.Path.Value == Routing.RedirectToPath)
                    path = Routing.DefaultOriginalPath;
                else path = httpContext.Request.Path.Value;
                var sourceUrl = Routing.OriginalHost + path;

                using (HttpClient client = new())
                {
                    using (HttpResponseMessage response = client.GetAsync(sourceUrl).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (HttpContent content = response.Content)
                            {
                                if (!PathKeeper.IsRedirected)
                                {
                                    string result = content.ReadAsStringAsync().Result;
                                    PathKeeper.OriginalContent = result;
                                }
                            }
                        }
                        else PathKeeper.OriginalContent = null;

                    }
                }

                if (path != Routing.DefaultOriginalPath)
                {
                    httpContext.Response.Redirect(Routing.RedirectToPath);
                    PathKeeper.CurrentPath = path;
                    PathKeeper.IsRedirected = true;
                    return;
                }
                else PathKeeper.IsRedirected = false;
            }

            await _next(httpContext);
        }
    }
}
