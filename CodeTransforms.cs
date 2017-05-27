using System;

namespace Kata_LOC
{
    internal class CodeTransforms
    {
        public static string EliminateMultiLineComments(string originalCodeAsString)
        {
            if (String.IsNullOrEmpty(originalCodeAsString)) throw new NotImplementedException();
            return originalCodeAsString;
        }

        public static CodeAsLines EliminateSingleLineComments(CodeAsLines code)
        {
            if (code==null) throw new NotImplementedException();
            string[] newCodeLines = code.Lines;
            var newCode = new CodeAsLines(newCodeLines);
            return newCode;
        }
    }
}