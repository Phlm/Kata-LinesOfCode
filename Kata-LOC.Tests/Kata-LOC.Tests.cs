using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Kata_LOC.Program;

namespace Kata_LOC.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void AcceptanceTest2_Phil1()
        {
            //Arrange
            var codeAsString = ReadCodeFromStandardFile();
            //Act
            var loc = CountLoc(codeAsString);
            //Assert
            Assert.AreEqual(5, loc);
        }

       
    }
}
