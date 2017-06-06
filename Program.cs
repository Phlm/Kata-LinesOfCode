using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tools.ToolFunctions;
// using static Kata_LOC.CodeTransforms;
// ReSharper disable UnnecessaryWhitespace

namespace Kata_LOC
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var codeAsString = ReadCodeFromFile(@"..\..\Kata-LOC.Tests\Kata_LOC_bsp2.cs");
            var linesOfCode = CountLoc(codeAsString);
        }

        public static string ReadCodeFromFile(string inputFile)
        {
            string codeAsString = "";
            try
            {
                codeAsString = File.ReadAllText(inputFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: "+e.Message);
                throw;
            }
            return codeAsString;
        }

        public static int CountLoc(string originalCodeAsString)
        {
            Tokenlist.ScanCodeForTokens(originalCodeAsString);
            return Tokenlist.CountLinesInTokenList();
         }
    }
}
