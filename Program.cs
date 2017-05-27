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
            var codeAsLines = EliminateCommentsFromCode(originalCodeAsString);
            return codeAsLines.CountNonWhiteSpaceLines();
        }

        static CodeAsLines EliminateCommentsFromCode(string originalCodeAsString)
        {
            var codeLines = new CodeAsLines(originalCodeAsString);
            Debug.Assert(codeLines != null);

            // var activeOuterElement = enum
            foreach (var line in codeLines.Lines)
            {
                 
            }
            //string code = CodeTransforms.EliminateMultiLineComments(originalCodeAsString);
            
            //var codeLinesAfterFilter= CodeTransforms.EliminateSingleLineComments(codeLines);
            return codeLines;
        }
    }
}
