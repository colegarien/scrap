using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipefier.Domain.Model;
using Recipefier.Persuement;
using System.Web;

namespace Recipefier.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PersueController : ControllerBase
    {
        RecipePersuer persuer;

        public PersueController()
        {
            persuer = new RecipePersuer(); // TODO use dependency injection?
        }


        [HttpPost]
        [Route("queue/{url:required}")]
        public Recipe Queue(string url)
        {
            return persuer.Persue(HttpUtility.UrlDecode(url));
        }
    }
}
