using Microsoft.VisualStudio.Package;
using Microsoft.VisualStudio.TextManager.Interop;

namespace esLang
{
    class EsSource : Source
    {
        public EsSource(LanguageService service, IVsTextLines textLines, Colorizer colorizer) : base(service, textLines, colorizer)
        {
        }

        // TODO: To implement formatting
        public override void ReformatSpan(EditArray mgr, TextSpan span)
        {
            base.ReformatSpan(mgr, span);
        }
    }
}
