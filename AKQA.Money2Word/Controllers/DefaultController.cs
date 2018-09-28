using AKQA.Money2Word.Models;
using AKQA.Money2Word.Services.Interfaces;
using System.Web.Http;

namespace AKQA.Money2Word.Controllers
{

    public class DefaultController : ApiController
    {
        private readonly IService service;
        public DefaultController(IService service)
        {
            this.service = service;
        }

        public IResponseModel Get([FromUri]InputModel model)
        {
            IResponseModel responseModel;

            responseModel = service.FillModel(model);
            return responseModel;
        }
    }
}
