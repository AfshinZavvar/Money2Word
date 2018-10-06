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
            sut = new DefaultController(service.Object);
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