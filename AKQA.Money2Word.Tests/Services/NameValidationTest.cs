using AKQA.Money2Word.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AKQA.Money2Word.Tests.Services
{
    [TestClass]
    public class NameValidationTest
    {
        INameValidator sut;
        public NameValidationTest()
        {
            sut = new NameValidator();
        }

        [TestMethod]
        public void Returns_Error_If_Input_Is_Not_In_Range()
        {
            //ACT
           var result= sut.Validate("a");
            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void Returns_Error_If_Input_Is_Null()
        {
            //ACT
            var result = sut.Validate(null);
            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void Returns_Error_If_Input_Has_Wrong_Char()
        {
            //ACT
            var result = sut.Validate("this is n3me");
            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void Returns_Error_If_Input_Has_Only_Space()
        {
            //ACT
            var result = sut.Validate("     ");
            //Assert
            Assert.IsTrue(result.HasError);
        }
        [TestMethod]
        public void Accepts_Space_In_Input()
        {
            //Arrange
           const string INPUT ="this is name";
            //ACT
            var result = sut.Validate("this is name");
            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual(INPUT,result.NameResult);
        }
    }
}
