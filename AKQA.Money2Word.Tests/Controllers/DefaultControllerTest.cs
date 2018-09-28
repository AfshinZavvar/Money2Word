using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AKQA.Money2Word.Controllers;
using AKQA.Money2Word.Services.Interfaces;
using Moq;
using AKQA.Money2Word.Models;

namespace AKQA.Money2Word.Tests.Controllers
{
    [TestClass]
    public class DefaultControllerTest
    {
        DefaultController sut;
        Mock<IService> service;

        public DefaultControllerTest()
        {

            service = new Mock<IService>();
            service.Setup(x => x.FillModel(It.IsAny<InputModel>())).Returns(
                (IResponseModel y) =>
                {
                    return new ResponseModel() { Name = "TestName", Amount = "123.45" };
                });
        }

        [TestMethod]
        [Ignore]
        public void Returns_Response()
        {
            //ARRANGE
            sut = new DefaultController(service.Object);
            InputModel inputModel = new InputModel() { Amount = "1", Name = "test" };

            //ACT
            sut.Get(inputModel);

            //ASSERT
            service.Verify(x => x.FillModel(It.IsAny<IInputModel>()), Times.Once);
        }
    }
}