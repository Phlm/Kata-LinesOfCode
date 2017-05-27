using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata_LOC
{
    internal class Program
    {
     
        static void Main(string[] args)
        {
            ReadCodeFromStandardFile();
        }

        public static string ReadCodeFromStandardFile()
        {
            string codeAsString = "";
            try
            {
                codeAsString = File.ReadAllText(@"..\..\Kata_LOC_bsp2.cs");
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
            var codeAsLines = RemoveCommentsFromCode(originalCodeAsString);
            return codeAsLines.CountNonWhiteSpaceLines();
        }

        static CodeAsLines RemoveCommentsFromCode(string originalCodeAsString)
        {
            string code = CodeTransforms.EliminateMultiLineComments(originalCodeAsString);
            var codeLines = new CodeAsLines(code);
            Debug.Assert(codeLines != null);
            var codeLinesAfterFilter= CodeTransforms.EliminateSingleLineComments(codeLines);
            return codeLines;
        }
    }
}
