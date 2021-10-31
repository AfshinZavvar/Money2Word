using Microsoft.VisualStudio.TestTools.UnitTesting;
using Money2Word.Controllers;
using Money2Word.Services.Interfaces;
using Moq;
using Money2Word.Models;

namespace Money2Word.Tests.Controllers
{
    [TestClass]
    public class DefaultControllerTest
    {
        ApisController sut;
        Mock<IService> service;

        public DefaultControllerTest()
        {
            service = new Mock<IService>();
            sut = new ApisController(service.Object);
        }

        [TestMethod]
       
        public void Returns_Response()
        {
            //ARRANGE
            var inputModel = new InputModel() { Amount = "1", Name = "test" };
            service.Setup(x => x.FillModel(inputModel))
                .Returns(new ResponseModel() { Name = "TestName", Amount = "123.45" });

            //ACT
           var result= sut.Show(inputModel);

            //ASSERT
            service.Verify(x => x.FillModel(inputModel), Times.Once);
        }
    }
}