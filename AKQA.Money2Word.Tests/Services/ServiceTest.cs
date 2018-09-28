using AKQA.Money2Word.Models;
using AKQA.Money2Word.Services;
using AKQA.Money2Word.Services.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKQA.Money2Word.Tests.Services
{
    [TestClass]
    public class ServiceTest
    {
        IService sut { get; set; }
        Mock<IMoney2WordConvertor> money2WordConvertor;
        Mock<INameValidator> nameValidator;

        public ServiceTest()
        {
            money2WordConvertor = new Mock<IMoney2WordConvertor>();
            nameValidator = new Mock<INameValidator>();

            sut = new Service(money2WordConvertor.Object, nameValidator.Object, Mock.Of<IResponseModel>());
        }

        [TestMethod]
        public void Returns_Error_If_Model_Is_Null()
        {
            //Arrange           
            money2WordConvertor.Setup(x => x.Money2Word(It.IsAny<string>())).Returns(
                ((string, bool) y) => { return ("TestErrorMessag", true); });
            //Act
            var result = sut.FillModel(null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ErrorMessage.Length > 0);
            Assert.IsNull(result.Amount);

        }

        [TestMethod]
        public void Returns_Error_If_Number_Convertor_Returns_Error()
        {
            //Arrange           
            money2WordConvertor.Setup(x => x.Money2Word(It.IsAny<string>())).Returns(
                ((string, bool) y) => { return ("TestErrorMessag", true); });
            //Act
            var result = sut.FillModel(null);

            //Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsNull(result.Amount);
        }

        [TestMethod]
        public void Returns_Error_If_Name_Validation_Fails()
        {
            //Arrange           
            money2WordConvertor.Setup(x => x.Money2Word(It.IsAny<string>())).Returns(
                ((string, bool) y) => ("TestText", false));

            nameValidator.Setup(x => x.Validate(It.IsAny<string>())).Returns(
                ((bool, string) y) => (true, "TestErrorMessage"));
            //Act
            var result = sut.FillModel(null);

            //Assert
            Assert.IsTrue(!string.IsNullOrEmpty(result.ErrorMessage));
            Assert.IsNotNull(result.Amount);
            Assert.IsNull(result.Name);
            nameValidator.Verify(x => x.Validate(It.IsAny<string>()), Times.Once);
            money2WordConvertor.Verify(x => x.Money2Word(It.IsAny<string>()), Times.Once);
        }


        [TestMethod]
        public void Returns_Response_Model_If_Validations_Pass()
        {
            //Arrange           
            money2WordConvertor.Setup(x => x.Money2Word(It.IsAny<string>())).Returns(
                ((string, bool) y) => ("TestText", false));

            nameValidator.Setup(x => x.Validate(It.IsAny<string>())).Returns(
                ((bool, string) y) => (false, "TestErrorMessage"));

            //Act
            var result = sut.FillModel(null);

            //Assert
            Assert.IsNotNull(result.Amount);
            Assert.IsNotNull(result.Name);
            nameValidator.Verify(x => x.Validate(It.IsAny<string>()), Times.Once);
            money2WordConvertor.Verify(x => x.Money2Word(It.IsAny<string>()), Times.Once);
        }
    }
}
