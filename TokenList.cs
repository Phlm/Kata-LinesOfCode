using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static System.Environment;
using static Tools.ToolFunctions;

// ReSharper disable All

namespace Kata_LOC
{
    public enum OuterTokenType
    {
        UnknownToken,
        StandardCode,
        EmptyLine,
        NewLine,
        SingleLineComment,
        MultilineComment,
        String,
        VerbatimString,
        IncompleteToken,
        EndOfCode,
    }

    public struct Token
    {
        public string Code;
        public OuterTokenType Typ;
        //public int OwnLineSpan;

        public Token(string code, OuterTokenType tokenType) // constructor with two params
        {
            this.Code = code;
            this.Typ=tokenType;
        }

    }

    internal class Tokenlist
    {
        internal static readonly string EOL = "\r"; // NewLine
        readonly static string emptyLinePattern = $@"[ \f\n\t\v]*{EOL}"; // any white space charactersor + one NewLine
        //readonly static string emptyLinePattern = @"(^\s*$)|(^\s*\r)"; // empty or NewLine
        //readonly static string emptyLinePattern = @"(^\s*$)|(^\s*\r)|(^\r)"; // empty or NewLine
        //string strRegex = @"^\s*?$"; // empty or NewLine


        public static Token[] tokenListe;

        private static int iCursorOfCode;
        private static string code;

        private const string VerbatimStringBeginPattern = "@\"";
        private const string MultilineCommentBeginPattern = "/*";
        private const string SinglelineCommentBeginPattern = "//";


        public static Token[] ScanCodeForTokens(string code)
        {
            //code = "@\"1\""; // length 4
            tokenListe = EnumerateOuterTokens(code).ToArray();
            return tokenListe;
        }

        public static int CountLinesInTokenList()
        {
            int noOfLines = 0;
            var myTokenList = FilterCommentsAndWhitespaceInTokenList(tokenListe);
            var tokenAsLines = GetCodeAsArray(myTokenList);
            var tokenAsString = String.Join("", tokenAsLines);
            //tokenAsLines = tokenAsString.Split( new string[] {  }, StringSplitOptions.None);
            tokenAsLines = tokenAsString.Split('\r');
            noOfLines = String.IsNullOrWhiteSpace(tokenAsLines[tokenAsLines.Length-1])? tokenAsLines.Length-1 : tokenAsLines.Length; // letzte Leerzeile ändert nichts an Codezahl, da Zeile auch ohne Zeilenvorschub zählt
            return noOfLines;
        }
        public static string[] GetCodeAsArray(Token[] myTokenList)
        {
            return myTokenList.Select(t => t.Code).ToArray();
        }

        public static void AggregateNodeWithFollowingNewLine()
        {
            var ta = tokenListe.SelectTwo((Token x, Token y) =>  new Token(x.Code + y.Code, x.Typ));

            var ts = tokenListe.Where(Token x, Token y) => y.Typ == OuterTokenType.NewLine);
            //var tt = tokenListe.SelectTwo((Token x, Token y) => 
            //{
            //    if (y.Typ == OuterTokenType.NewLine)
            //    {
            //        if (x.Typ != OuterTokenType.NewLine && x.Typ != OuterTokenType.SingleLineComment)
            //            return new Token(x.Code + y.Code, x.Typ);
            //    }
            //    return x;
            //});

            //var a = tokenListe.Zip(tokenListe, tokenListe.Skip(1), Func:);
            //tokenListe = tokenListe.Zip((tokenListe, tokenListe.Skip(1), (a, b) => new Token(a.Code + b.Code, a.Typ));
        }

        //public static IEnumerable<TResult> SelectTwo<TSource, TResult>(this IEnumerable<TSource> source,
        //                                                                Func<TSource, TSource, TResult> selector)
        //{
        //    return Enumerable.Zip(source, source.Skip(1), selector);
        //}

        public static Token[] FilterCommentsAndWhitespaceInTokenList(Token[] myTokenList) // unnötig, nur Where Demo bis jetzt
        {
            myTokenList = tokenListe.Where(t => t.Typ != OuterTokenType.EmptyLine && t.Typ != OuterTokenType.SingleLineComment && t.Typ != OuterTokenType.MultilineComment).ToArray();
            return myTokenList;
        }

        //public static int CountLinesInTokenList2()
        //{
        //    int noOfLines = 0;
        //    foreach (var token in tokenListe)
        //    {
        //        switch (token.Typ)
        //        {
        //            case OuterTokenType.EmptyLine:
        //            case OuterTokenType.SingleLineComment:
        //            case OuterTokenType.MultilineComment:
        //                break;

        //            case OuterTokenType.EndOfCode:
        //                Debug.Assert(Not(token.Code.EndsWith(EOL)), $"Error! {token.Typ} shall not appear!");
        //                break;

        //            //case OuterTokenType.NewLine:
        //            //    noOfLines++;
        //            //    break;

        //            case OuterTokenType.VerbatimString:
        //            case OuterTokenType.String:
        //                Debug.Assert(Not(token.Code.EndsWith(NewLine)), "Error! 'Newline' found in 'verbatimString' token at last position!");
        //                var tokenAsLines = token.Code.Replace("\r", "").Split('\n');
        //                Debug.Assert(tokenAsLines.Length > 0);
        //                noOfLines += tokenAsLines.Length-1;
        //                if (noOfLines == 0)
        //                    noOfLines = 1;
        //                break;

        //            case OuterTokenType.StandardCode:
        //            case OuterTokenType.IncompleteToken:
        //            default:
        //                if (noOfLines == 0)
        //                    noOfLines = 1;
        //                //var tokenAsLines = code.Replace("\r", "").Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        //                //noOfLines += tokenAsLines.Length;
        //                Debug.Assert(token.Code.IndexOf(NewLine) < 0, $"Error! 'Newline' found in {token.Typ} token!");
        //                break;
        //        }


        //    }

        //    return noOfLines;
        //}

        public static IEnumerable<Token> EnumerateOuterTokens(string codeParam)
        {
            code = codeParam;
            code = code.Replace(NewLine, EOL);
            //code = code.Replace("\n", "\r") + "\r";
            Debug.Assert(code.IndexOf("\n") < 0);

            const RegexOptions ignoreNewLineAndOtherOpts = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            int tokenBeginMinimum = 0;
            iCursorOfCode = tokenBeginMinimum;
            var activeOuterEl = OuterTokenType.UnknownToken;

            if (code.Length == 0)
                yield break;

            while (activeOuterEl != OuterTokenType.EndOfCode )
            {
                Debug.Assert(iCursorOfCode < code.Length+1, "Error! Unexpected end of code while still tokenizing!");
                string newTokenString = "", beginPattern="", endPattern="";
                int endIndexOfToken;
                Token newToken;
                Match myMatch;
                Regex myRegex;

                switch (activeOuterEl)
                {   
                    case OuterTokenType.UnknownToken:
                        
                        activeOuterEl= SearchForNextTokenBeginPattern(activeOuterEl, ref tokenBeginMinimum);
                        //if (activeOuterEl == OuterTokenType.EndOfCode)
                        //    goto case OuterTokenType.EndOfCode;

                        if (tokenBeginMinimum - iCursorOfCode > 0)
                        {
                            // check, if standard token has to be closed before
                            var finishedLeftToken = new Token(code.Substring(iCursorOfCode, tokenBeginMinimum - iCursorOfCode), OuterTokenType.StandardCode);
                            yield return finishedLeftToken;
                        }
                        iCursorOfCode = tokenBeginMinimum;
                        break;

                    //case OuterTokenType.EndOfCode:
                    //    newToken = new Token("", OuterTokenType.EndOfCode);
                    //    yield return newToken;
                    //    break;

                    case OuterTokenType.VerbatimString:
                        beginPattern = VerbatimStringBeginPattern; endPattern = "\"";
                        Regex regExEndOfVerbatimString = new Regex(".*?\"(?!\"\")", ignoreNewLineAndOtherOpts); // Search for next quote without following other quote
                        myMatch = regExEndOfVerbatimString.Match(code.Substring(iCursorOfCode+ beginPattern.Length));
                        newTokenString = "";
                        if (myMatch.Success)
                        {
                            newTokenString = beginPattern + myMatch.Value;
                            newToken = new Token(newTokenString, OuterTokenType.VerbatimString);
                        }
                        else
                        {
                            newTokenString = code.Substring(iCursorOfCode, iCursorOfCode + beginPattern.Length);
                            newToken = new Token(newTokenString, OuterTokenType.IncompleteToken);
                        }
                        yield return newToken;
                        iCursorOfCode += newToken.Code.Length;
                        activeOuterEl = OuterTokenType.UnknownToken;
                        break;

                    case OuterTokenType.MultilineComment:
                        beginPattern = MultilineCommentBeginPattern; endPattern = "*/";
                        endIndexOfToken = code.IndexOf(endPattern, iCursorOfCode + beginPattern.Length);
                        if (endIndexOfToken >= 0) // if comment is not closed, close that token implicitly now.
                            newToken = createTokenOutOfCode(OuterTokenType.MultilineComment, endIndexOfToken + endPattern.Length);
                        else
                            newToken = createTokenOutOfCode(OuterTokenType.IncompleteToken, iCursorOfCode + beginPattern.Length);
                        yield return newToken;
                        iCursorOfCode += newToken.Code.Length;
                        activeOuterEl = OuterTokenType.UnknownToken;
                        break;

                    case OuterTokenType.String:
                        beginPattern = endPattern = "\"";
                        endIndexOfToken = code.IndexOf(endPattern, iCursorOfCode + beginPattern.Length);
                        if (endIndexOfToken >= 0) // if comment is not closed, close that token implicitly now.
                            newToken = createTokenOutOfCode(OuterTokenType.String, endIndexOfToken + endPattern.Length);
                        else
                            newToken = createTokenOutOfCode(OuterTokenType.IncompleteToken, iCursorOfCode + beginPattern.Length);
                        yield return newToken;
                        iCursorOfCode += newToken.Code.Length;
                        activeOuterEl = OuterTokenType.UnknownToken;
                        break;

                    case OuterTokenType.SingleLineComment:
                        beginPattern = SinglelineCommentBeginPattern; endPattern = EOL;
                        endIndexOfToken = code.IndexOf(endPattern, iCursorOfCode + beginPattern.Length);
                        if (endIndexOfToken >= 0) // if not, SingleComment reaches end of code, comments goes until this end
                            newToken = createTokenOutOfCode(OuterTokenType.SingleLineComment, endIndexOfToken + endPattern.Length);
                        else
                            newToken = createTokenOutOfCode(OuterTokenType.SingleLineComment, code.Length); // until end of line
                        yield return newToken;
                        iCursorOfCode += newToken.Code.Length;
                        activeOuterEl = OuterTokenType.UnknownToken;
                        break;

                    case OuterTokenType.EmptyLine:
                        var a = code.Substring(iCursorOfCode);
                        beginPattern = EOL;
                        //emptyLinePattern = @"(^\s*$)|(^\s*\r)"; // empty or NewLine
                        myRegex = new Regex(emptyLinePattern, RegexOptions.Multiline);
                        myMatch = myRegex.Match(code.Substring(iCursorOfCode));
                        newTokenString = beginPattern;
                        if (myMatch.Success)
                        {
                            newTokenString = myMatch.Value;
                        }
                        newToken = createTokenOutOfCode(OuterTokenType.EmptyLine, iCursorOfCode + newTokenString.Length);
                        yield return newToken;
                        iCursorOfCode += newToken.Code.Length;
                        activeOuterEl = OuterTokenType.UnknownToken;
                        break;

                        //case OuterTokenType.NewLine:
                        //    beginPattern = NewLine;
                        //    newToken = createTokenOutOfCode(OuterTokenType.NewLine, iCursorOfCode + beginPattern.Length);
                        //    yield return newToken;
                        //    iCursorOfCode += newToken.Code.Length;
                        //    activeOuterEl = OuterTokenType.UnknownToken;
                        //    break;
                }
            }

            Token createTokenOutOfCode(OuterTokenType tokenType, int endIndex)
            {
                string myToken = code.Substring(iCursorOfCode, endIndex-iCursorOfCode);
                return new Token(myToken, tokenType);
            }

        }
        static OuterTokenType SearchForNextTokenBeginPattern(OuterTokenType activeToken, ref int tokenBeginMinimum)
        {
            int verbatimQuoteBegin = code.IndexOf(VerbatimStringBeginPattern, iCursorOfCode);
            int multiLineCommentBegin = code.IndexOf(MultilineCommentBeginPattern, iCursorOfCode);
            int quoteBegin = code.IndexOf("\"", iCursorOfCode);
            int singleLineCommentBegin = code.IndexOf(SinglelineCommentBeginPattern, iCursorOfCode);
            int emptyLineBegin=-1;
            var a = code.Substring(iCursorOfCode);
            if (iCursorOfCode < code.Length)
            {
                
                Regex emptyOrNewLine = new Regex(emptyLinePattern, RegexOptions.Multiline);
                Match match = emptyOrNewLine.Match(code.Substring(iCursorOfCode));
                emptyLineBegin = match.Success ? match.Index+iCursorOfCode : -1;
                //if (iCursorOfCode + 2 <= code.Length) emptyLineBegin = code.Substring(iCursorOfCode, NewLine.Length) == NewLine ? iCursorOfCode : -1;  // first character is NewLine; sensitive for iCursorCode < 2
            }
            int nextLineBegin = code.IndexOf($"{NewLine}", iCursorOfCode);
            // Contract requirement: tokenBeginMinimum has to contain one value not -1: code.Length as one more as the max. possible index assures that, for empty code and for last token .EndOfCode
            int[] tokenBegins = { code.Length, verbatimQuoteBegin, multiLineCommentBegin, quoteBegin, singleLineCommentBegin, emptyLineBegin }; //, nextLineBegin };
            tokenBeginMinimum = tokenBegins.Where(t => t > -1).Min();
            //Now consider priority!
            if (tokenBeginMinimum == verbatimQuoteBegin)
                activeToken = OuterTokenType.VerbatimString;
            else if (tokenBeginMinimum == multiLineCommentBegin)
                activeToken = OuterTokenType.MultilineComment;
            else if (tokenBeginMinimum == quoteBegin)
                activeToken = OuterTokenType.String;
            else if (tokenBeginMinimum == singleLineCommentBegin)
                activeToken = OuterTokenType.SingleLineComment;
            else if (tokenBeginMinimum == emptyLineBegin)
                activeToken = OuterTokenType.EmptyLine;
            //else if (tokenBeginMinimum == nextLineBegin)
            //    activeToken = OuterTokenType.NewLine;
            else
            {
                activeToken = OuterTokenType.EndOfCode;
                tokenBeginMinimum = code.Length;
            }
            return activeToken;
        }



    }
    
}