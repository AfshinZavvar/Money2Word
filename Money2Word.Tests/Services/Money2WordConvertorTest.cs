using Money2Word.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Money2Word.Tests.Services
{

    [TestClass]
    public class Money2WordConvertorTest
    {
        IMoney2WordConvertor sut;
        public Money2WordConvertorTest()
        {
            sut = new Money2WordConvertor();
        }
        [TestMethod]
        public void Handles_Zero()
        {
            //Arrange

            //ACT
            var result = sut.Money2Word("0");

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual("ZERO DOLLARS AND ZERO CENTS", result.Word);
        }

        [TestMethod]
        public void Returns_Error_If_More_Than_One_Decimal_Point_Found()
        {
            //ACT
            var result = sut.Money2Word("100.05.12");

            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void Returns_Error_If_Dollar_Is_Out_Of_Range()
        {
            //ACT
            var result = sut.Money2Word("1000");

            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void Returns_Error_If_Cent_Is_Out_Of_Range()
        {
            //ACT
            var result = sut.Money2Word("1.123");

            //Assert
            Assert.IsTrue(result.HasError);
        }

        [TestMethod]
        public void Converts_Money_To_result()
        {
            //ACT
            var result = sut.Money2Word("123.45");

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual("ONE HUNDRED AND TWENTY-THREE DOLLARS AND FOURTY-FIVE CENTS", result.Word);
        }

        [TestMethod]
        public void Converts_If_Number_Is_Round()
        {
            //ACT
            var result = sut.Money2Word("100.45");

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual("ONE HUNDRED DOLLARS AND FOURTY-FIVE CENTS", result.Word);
        }

        [TestMethod]
        public void Converts_If_Number_Is_Less_Than_Twenty()
        {
            //ACT
            var result = sut.Money2Word("12.00");

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual("TWELVE DOLLARS AND ZERO CENTS", result.Word);
        }


        [TestMethod]
        public void Handles_One_Digit_Cents()
        {
            //ACT
            var result = sut.Money2Word("0.4");

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual("ZERO DOLLARS AND FOURTY CENTS", result.Word);
        }

        [TestMethod]
        public void Handles_Two_Digit_Cents()
        {
            //ACT
            var result = sut.Money2Word("0.04");

            //Assert
            Assert.IsFalse(result.HasError);
            Assert.AreEqual("ZERO DOLLARS AND FOUR CENTS", result.Word);
        }

    }
}
