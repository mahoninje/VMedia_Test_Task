using HabrProxy.Services;
using HabrProxy.Services.Static;
using Microsoft.AspNetCore.Mvc;

namespace HabrProxy.Controllers
{
    [ApiController]
    [Route("/")]
    public class HabrProxyController : ControllerBase
    {
        private const string NotFoundMessage = "Sorry, page is not found :(";

        private readonly IContentModifier _contentModifier;

        public HabrProxyController(IContentModifier contentModifier)
        {
            _contentModifier = contentModifier;
        }


        [HttpGet]
        public IActionResult Get()
        {
            var modifiedContent = _contentModifier.Modify(PathKeeper.OriginalContent);
            if (!string.IsNullOrEmpty(modifiedContent))
                return Content(modifiedContent, Routing.OriginalContentType);
            else
                return NotFound(NotFoundMessage);
        }
    }
}