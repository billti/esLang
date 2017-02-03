using Microsoft.VisualStudio.Package;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace esLang
{
    // See info in IScanner docs: https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.package.iscanner.aspx
    class EsScanner : IScanner
    {
        private string source;
        private int offset;
        private ParseState parseState = ParseState.InText;

        private enum ParseState
        {
            InText = 0,     // Default
            InQuotes = 1,   // Use template literals across lines ``
            InComment = 2   // Use line comments only after #
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if(tokenInfo == null || this.offset >= this.source.Length)
            {
                return false;
            }

            tokenInfo.StartIndex = this.offset;

            // Jus color each alternate char for now
            switch (this.parseState)
            {
                case ParseState.InQuotes:
                    tokenInfo.Color = TokenColor.String;
                    this.parseState = ParseState.InComment;
                    break;
                case ParseState.InComment:
                    tokenInfo.Color = TokenColor.Comment;
                    this.parseState = ParseState.InText;
                    break;
                default: // Text
                    tokenInfo.Color = TokenColor.Text;
                    this.parseState = ParseState.InQuotes;
                    break;
            }
            tokenInfo.EndIndex = this.offset++;
            state = (int)this.parseState;
            return true;
        }

        public void SetSource(string source, int offset)
        {
            this.source = source;
            this.offset = offset;
        }
    }
}
