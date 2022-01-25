using HabrProxy.Services;
using HabrProxy.Services.Static;
using Microsoft.AspNetCore.Mvc;

namespace HabrProxy.Controllers
{
    [ApiController]
    [Route("/")]
    public class HabrProxyController : ControllerBase
    {
        private readonly IContentModifier _contentModifier;

        public HabrProxyController(IContentModifier contentModifier)
        {
            _contentModifier = contentModifier;
        }

        /// <summary>
        /// Load Habr.com pages
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            var modifiedContent = _contentModifier.Modify(Supervisor.OriginalContent);
            if (!string.IsNullOrEmpty(modifiedContent))
                return Content(modifiedContent, Constants.OriginalContentType);
            else
                return NotFound(Constants.NotFoundMessage);
        }

        /// <summary>
        /// Get Media content to load .svg images
        /// </summary>
        /// <returns></returns>
        [HttpGet("img/{name}")]
        public IActionResult GetImage() 
        {
            if (!string.IsNullOrEmpty(Supervisor.ImageContent))
                return Content(Supervisor.ImageContent, Constants.ImageContentType);
            else
                return Ok();
        }
    }
}