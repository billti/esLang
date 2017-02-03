using System;
using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace esLang
{
    // See "Creating a legacy language service": https://msdn.microsoft.com/en-us/library/bb165744.aspx
    // See also LanguageService class docs: https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.package.languageservice.aspx
    class EsLanguageService : LanguageService
    {
        private LanguagePreferences languagePreferences;
        public override string Name => "es language service";

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

        // See "Legacy language service parser and scanner": https://msdn.microsoft.com/en-us/library/bb164730.aspx
        public override IScanner GetScanner(IVsTextLines buffer)
        {
            return new EsScanner();
        }

        // Will need to be implemented to do brace matching, etc... See https://msdn.microsoft.com/en-us/library/microsoft.visualstudio.package.languageservice.parsesource.aspx
        public override AuthoringScope ParseSource(ParseRequest req)
        {
            return null;
        }
    }
}
