using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Linq;

namespace esLang
{
    // See info in IScanner docs: https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.package.iscanner.aspx
    class EsScanner : IScanner
    {
        private string source;
        private int offset;
        private string[] keywords = {"var", "function", "if", "else", "return", "while", "break"};
        private char[] braces = { '{', '}', '(', ')', '[', ']' };
        private IVsTextLines buffer;

        private bool More => this.offset < this.source.Length;

        private char Curr => this.More ? this.source[offset] : '\0';

        public EsScanner(IVsTextLines buffer)
        {
            this.buffer = buffer;
        }

        private bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private bool EatPunctuation(TokenInfo tokenInfo)
        {
            char curr = this.Curr;
            if(this.braces.Contains(curr))
            {
                tokenInfo.Color = TokenColor.Number;
                tokenInfo.Type = TokenType.Delimiter;
                tokenInfo.Trigger = TokenTriggers.MatchBraces;
                tokenInfo.EndIndex = this.offset++;
                return true;
            }
            return false;
        }

        private bool EatWhitespace(TokenInfo tokenInfo)
        {
            if (!IsWhitespace(this.Curr))
            {
                return false;
            }
            tokenInfo.Type = TokenType.Text;
            tokenInfo.Color = TokenColor.Text;
            while (true)
            {
                this.offset++;
                if (!this.IsWhitespace(this.Curr)) break;
            }
            tokenInfo.EndIndex = this.offset - 1;
            return true;
        }

        private bool EatComment(TokenInfo tokenInfo)
        {
            if(this.Curr != '#')
            {
                return false;
            }
            tokenInfo.Type = TokenType.Comment;
            tokenInfo.Color = TokenColor.Comment;
            while (this.Curr != '\0')
            {
                this.offset++;
            }
            tokenInfo.EndIndex = this.offset - 1;
            return true;
        }

        private bool EatString(TokenInfo tokenInfo)
        {
            if(this.Curr != '"')
            {
                return false;
            }
            tokenInfo.Type = TokenType.String;
            tokenInfo.Color = TokenColor.String;
            this.offset++;
            while(true)
            {
                if(this.Curr == '"')
                {
                    this.offset++;
                    break;
                }
                if(this.Curr == '\0')
                {
                    break;
                }
                this.offset++;
            }
            tokenInfo.EndIndex = this.offset - 1;
            return true;
        }

        private bool EatText(TokenInfo tokenInfo)
        {
            // Always true. Consume up to #, ", punctuation, braces, whitespace, or EOF
            while(true)
            {
                char next = this.Curr;
                if(next == '\0' || next == '#' || next == '"' || this.braces.Contains(next) || this.IsWhitespace(next))
                {
                    break;
                }
                this.offset++;
            }
            tokenInfo.EndIndex = this.offset - 1;
            int tokenLength = tokenInfo.EndIndex - tokenInfo.StartIndex + 1;
            string tokenText = this.source.Substring(tokenInfo.StartIndex, tokenLength);
            if(this.keywords.Contains(tokenText))
            {
                tokenInfo.Type = TokenType.Keyword;
                tokenInfo.Color = TokenColor.Keyword;
            }
            else
            {
                tokenInfo.Type = TokenType.Text;
                tokenInfo.Color = TokenColor.Text;
            }
            return true;
        }

        public bool ScanTokenAndProvideInfoAboutIt(TokenInfo tokenInfo, ref int state)
        {
            if(tokenInfo == null || !this.More)
            {
                return false;
            }

            tokenInfo.StartIndex = this.offset;

            // No tokens span lines, so there is no state between calls. Always return a whole, independant token
            return EatPunctuation(tokenInfo) || EatWhitespace(tokenInfo) || EatComment(tokenInfo) || EatString(tokenInfo) || EatText(tokenInfo);
        }

        public void SetSource(string source, int offset)
        {
            this.source = source;
            this.offset = offset;
        }
    }
}
