using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kata_LOC.Program;
using static Kata_LOC.Tokenlist;
using static System.Environment;
using System.Collections.Generic;

namespace Kata_LOC.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AcceptanceTest2_Phil1()
        {
            //Arrange
            var codeAsString = ReadCodeFromFile(@"..\..\Kata_LOC_bsp2.cs"); ;
            //Act
            var loc = CountLoc(codeAsString);
            //Assert
            Assert.AreEqual(5, loc);
        }

        [TestMethod]
        public void DetermineOuterActiveToken_VerbatimString_recognize()
        {
            //Arrange
            string codeAsString = "@\"1\""; // length 4
            var tokenCodeparts = new List<string>();
            var tokenTypes = new List<OuterTokenType>();
            //Act
            foreach (var codeToken in EnumerateOuterTokens(codeAsString))
            {
                tokenCodeparts.Add(codeToken.Code);
                tokenTypes.Add(codeToken.Typ);
            }
            //Assert
            Assert.AreEqual(1, tokenCodeparts.Count);
            Assert.AreEqual(OuterTokenType.VerbatimString, tokenTypes[0]);
            Assert.AreEqual(codeAsString, tokenCodeparts[0]);
        }

        [TestMethod]
        public void DetermineOuterActiveToken_verbatim_plus_SingleComment()
        {
            //Arrange
            string codeAsString = $"@\"1\"//ab1 //{EOL}";  //  VerbatimString+SinglelineComment with line
            var tokenCodeparts = new List<string>();
            var tokenTypes = new List<OuterTokenType>();
            //Act
            foreach (Token codeToken in EnumerateOuterTokens(codeAsString))
            {
                tokenCodeparts.Add(codeToken.Code);
                tokenTypes.Add(codeToken.Typ);
            }
            //Assert
            Assert.AreEqual(2, tokenCodeparts.Count);
            Assert.AreEqual(OuterTokenType.SingleLineComment, tokenTypes[1]);
            Assert.AreEqual($"//ab1 //\r", tokenCodeparts[1]);
        }

        [TestMethod]
        public void DetermineOuterActiveToken_SingleLineComment_recognize()
        {
            //Arrange
            string codeAsString = $"//ab{EOL}"; //5
            var tokenCodeparts = new List<string>();
            var tokenTypes = new List<OuterTokenType>();
            //Act
            foreach (Token codeToken in EnumerateOuterTokens(codeAsString))
            {
                tokenCodeparts.Add(codeToken.Code);
                tokenTypes.Add(codeToken.Typ);
            }
            //Assert
            Assert.AreEqual(1, tokenCodeparts.Count);
            Assert.AreEqual(OuterTokenType.SingleLineComment, tokenTypes[0]);
            Assert.AreEqual(codeAsString, tokenCodeparts[0]);
        }

        [TestMethod]
        public void DetermineOuterActiveToken_MultilineComment_recognize()
        {
            //Arrange
            string codeAsString = "/*ab*/";
            var tokenCodeparts = new List<string>();
            var tokenTypes = new List<OuterTokenType>();
            //Act
            foreach (Token codeToken in EnumerateOuterTokens(codeAsString))
            {
                tokenCodeparts.Add(codeToken.Code);
                tokenTypes.Add(codeToken.Typ);
            }
            //Assert
            Assert.AreEqual(1, tokenCodeparts.Count);
            Assert.AreEqual(OuterTokenType.MultilineComment, tokenTypes[0]);
            Assert.AreEqual(codeAsString, tokenCodeparts[0]);
        }

        [TestMethod]
        public void DetermineOuterActiveToken_String_recognize()
        {
            //Arrange
            string codeAsString = "\"ab\""; // len 4
            var tokenCodeparts = new List<string>();
            var tokenTypes = new List<OuterTokenType>();
            //Act
            foreach (Token codeToken in EnumerateOuterTokens(codeAsString))
            {
                tokenCodeparts.Add(codeToken.Code);
                tokenTypes.Add(codeToken.Typ);
            }
            //Assert
            Assert.AreEqual(1, tokenCodeparts.Count);
            Assert.AreEqual(OuterTokenType.String, tokenTypes[0]);
            Assert.AreEqual(codeAsString, tokenCodeparts[0]);
        }

        [TestMethod]
        public void CountLoc_1()
        {
            //Arrange
            string codeAsString = "\"ab\""; // len 4, eine Zeile
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(1, zeilen);
        }

        [TestMethod]
        public void CountLoc_2()
        {
            //Arrange
            var n = EOL;
            string codeAsString = $"a/*\"{n}ab\"*/b"; //eine Zeile
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(1, zeilen);
        }

        [TestMethod]
        public void CountLoc_Leerzeile()
        {
            //Arrange
            var n = EOL;
            string codeAsString = $"{n}";
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(0, zeilen);
        }

        [TestMethod]
        public void CountLoc_blank_Leerzeile_a()
        {
            //Arrange
            var n = EOL;
            string codeAsString = $" {n}a";
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(1, zeilen);
        }


        [TestMethod]
        public void CountLoc_Blankzeile()
        {
            //Arrange
            //var n = EOL;
            string codeAsString = " ";
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(0, zeilen);
        }

        [TestMethod]
        public void CountLoc_LeerzeilenMehrere1()
        {
            //Arrange
            var n = EOL;
            string codeAsString = $" a{n} ";
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(1, zeilen);
        }

        [TestMethod]
        public void CountLoc_LeerString()
        {
            //Arrange
            //var n = EOL;
            string codeAsString = "";
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(0, zeilen);
        }

        [TestMethod]
        public void CountLoc_3()
        {
            //Arrange
            //var n = EOL;
            string codeAsString = @"/* 10 Zeilen, davon 5 Codezeilen: ""String in Kommentar 1""
*/
  
1 AA /*Kommentar /*Kommentar2*/ */ aa""
2 Jetzt /* Kommentar in String 1
3
4 */"" ";

            //5 var a = "\""; /*"String in Kommentar 2" */";
            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(4, zeilen);
        }

        [TestMethod]
        public void CountLoc_Acceptance2b()
        {
            //Arrange
            //var n = EOL;
            string codeAsString = @"/* 10 Zeilen, davon 5 Codezeilen: ""String in Kommentar 1""
*/
  
1 AA /*Kommentar /*Kommentar2*/ */ aa""
2 Jetzt /* Kommentar in String 1
3
4 */""
5 var a = ""\""; /*""String in Kommentar 2"" */";  // Zeile 5 ist unvollständiger String ?

            //Act
            int zeilen = CountLoc(codeAsString);
            Assert.AreEqual(5, zeilen);
        }




    }
}
