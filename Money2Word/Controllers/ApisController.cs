using Money2Word.Models;
using Money2Word.Services.Interfaces;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Money2Word.Controllers
{
    public class ApisController : ApiController
    {
        private readonly IService service;
        public ApisController(IService service)
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
