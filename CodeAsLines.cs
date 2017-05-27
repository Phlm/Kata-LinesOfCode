using System.Linq;

namespace Kata_LOC
{
    public class CodeAsLines
    {
        public string[] Lines { get; set; }

        public CodeAsLines(string code)
        {
            Lines = code.Replace("\r","").Split('\n');
        }

        public CodeAsLines(string[] codeLines)
        {
            Lines = codeLines;
        }

        public int CountNonWhiteSpaceLines()
        {
            return Lines.Length;
        }
    }
}