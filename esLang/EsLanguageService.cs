using System;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

/*
 * TODO list:
 *  - Brace matching: https://social.msdn.microsoft.com/Forums/en-US/7ff77737-090e-4db0-8776-f32e7182cc4c/clarifications-on-brace-matching-in-the-managed-package-framework?forum=vsx
 *  - Brace matching notes: http://www.jagregory.com/writings/brace-matching-and-your-language-service/
 *  - Formatting: https://msdn.microsoft.com/en-us/library/bb164633.aspx
 */

namespace esLang
{
    // See "Creating a legacy language service": https://msdn.microsoft.com/en-us/library/bb165744.aspx
    // See also LanguageService class docs: https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.package.languageservice.aspx
    class EsLanguageService : LanguageService
    {
        private LanguagePreferences languagePreferences;
        public override string Name => "es";

        public override string GetFormatFilterList()
        {
            return "ES files (*.es)|*.es";
        }

        public override LanguagePreferences GetLanguagePreferences()
        {
            if(this.languagePreferences == null)
            {
                this.languagePreferences = new LanguagePreferences(this.Site, typeof(EsLanguageService).GUID, this.Name);
                if(this.languagePreferences != null)
                {
                    this.languagePreferences.Init();
                    // Can enable language service features here.
                }
            }
            return this.languagePreferences;
        }

        public override Source CreateSource(IVsTextLines buffer)
        {
            var src = new EsSource(this, buffer, this.GetColorizer(buffer));
            src.LastParseTime = 0;
            return src;
        }

        // See "Legacy language service parser and scanner": https://msdn.microsoft.com/en-us/library/bb164730.aspx
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return new EsScanner(buffer);
        }

        // Needs to be implemented to do brace matching, etc... See https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.package.languageservice.parsesource.aspx
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            var authoringScope = new EsAuthoringScope(req.Sink);

            /* Currently hard-coded errors and braces for now for first 3 lines of:
var class = {name: "Bill"};  # Error on class reserved word

function foo(a, b, c) {
            */

            if (req.Reason == ParseReason.Check || req.Reason == ParseReason.None)
            {
                // Do a full parse
                var errSpan = new TextSpan() { iStartLine = 0, iStartIndex = 4, iEndLine = 0, iEndIndex = 9 };
                req.Sink.AddError(req.FileName, "(ES1001): Invalid use of reserved word", errSpan, Severity.Error);
            }

            if(req.Sink.BraceMatching)
            {
                if (req.Line == 0)
                {
                    TextSpan first = new TextSpan { iStartLine = 0, iEndLine = 0, iStartIndex = 12, iEndIndex = 13 };
                    TextSpan second = new TextSpan { iStartLine = 0, iEndLine = 0, iStartIndex = 25, iEndIndex = 26 };
                    req.Sink.MatchPair(first, second, 1);
                    req.Sink.FoundMatchingBrace = true;
                }

                if (req.Line == 2)
                {
                    // 3rd line function parens
                    TextSpan first = new TextSpan { iStartLine = 2, iEndLine = 2, iStartIndex = 12, iEndIndex = 13 };
                    TextSpan second = new TextSpan { iStartLine = 2, iEndLine = 2, iStartIndex = 20, iEndIndex = 21 };
                    req.Sink.MatchPair(first, second, 1);
                    req.Sink.FoundMatchingBrace = true;
                }

            }
            return authoringScope;
        }

        //// Need to do this for the ParseSource calls of 'check' to happen if we weren't
        //// providing our own Source class and setting LastParseTime there.
        //
        //public override void OnIdle(bool periodic)
        //{
        //    Source src = GetSource(this.LastActiveTextView);
        //    if (src != null && src.LastParseTime == Int32.MaxValue)
        //    {
        //        src.LastParseTime = 0;
        //    }
        //    base.OnIdle(periodic);
        //}
    }
}
