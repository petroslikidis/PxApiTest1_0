using Microsoft.AspNetCore.Mvc;

namespace PxApiTest.Controllers
{
    [ApiController]
    [Route("api/v2/navigate")]
    public class Api2NavigateController : Controller
    {
        [HttpGet()]
        public string[] ListLevelsAtRoot()
        {
            return new string[] {"Befolkning", "Näringsliv", "Hälsa"};
        }

        [HttpGet("{levelid}")]
        public string[] ListSubLevels([FromRoute]string levelId)
        {
            return new string[] { "Sub level1", "Sub level2", "Sub level3" };
        }
    }
}
