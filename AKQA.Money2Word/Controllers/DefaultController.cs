using Money2Word.Models;
using Money2Word.Services.Interfaces;
using System.Web.Http;

namespace Money2Word.Controllers
{

    public class DefaultController : ApiController
    {
        private readonly IService service;
        public DefaultController(IService service)
        {
            this.service = service;
        }

        [HttpPost]
        [Route("api/show")]
        public IHttpActionResult Show(InputModel model)
        {
            return Ok(service.FillModel(model));
        }
    }
}
