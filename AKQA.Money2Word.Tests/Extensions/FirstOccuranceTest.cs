using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Money2Word.Extensions.StringExtension;

namespace Money2Word.Tests.Extensions
{
    [TestClass]
    public class FirstOccuranceTest
    {
        const string STRING_TO_FIND = "[AND]";
        const string STRING_TO_REPLACE = "[-]";

        [TestMethod]
        public void Replaces_First_Occurance_Pattern_With_Given_String()
        {
            //Arange
            var text = "This [AND] is Fake [AND] Text";

            //Act
            var result = text.ReplaceFirstOccurrence(STRING_TO_FIND, STRING_TO_REPLACE);

            //Assert
            Assert.AreEqual("This [-] is Fake [AND] Text", result);
        }

        [TestMethod]
        public void Does_Not_Replace_If_Pattern_Not_Found()
        {
            //Arange
            var text = "This [OR] is Fake [OR] Text";

            //Act
            var result = text.ReplaceFirstOccurrence(STRING_TO_FIND, STRING_TO_REPLACE);

            //Assert
            Assert.AreEqual(text, result);
        }

        [TestMethod]
        public void Returns_Empty_String_If_Input_Is_Null()
        {
            //Arrange
            string text = null;

            //Act
            var result = text.ReplaceFirstOccurrence(STRING_TO_FIND, STRING_TO_REPLACE);

            //Assert
            Assert.AreEqual(string.Empty, result);

        }

        [TestMethod]
        public void String_Comparision_Is_Not_Case_Sensetive()
        {
            //Arange
            var text = "This is Fake [aNd] Text";

            //Act
            var result = text.ReplaceFirstOccurrence(STRING_TO_FIND, STRING_TO_REPLACE);

            //Assert
            Assert.AreEqual("This is Fake [-] Text", result);

        }
    }
}
