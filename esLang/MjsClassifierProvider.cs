using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace esLang
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("mjs")] // This classifier applies to files with the "mjs" content type.
    internal class MjsClassifierProvider : IClassifierProvider
    {
#pragma warning disable 649
        // Classification registry to be used for getting a reference to the classification types later.
        [Import]
        private IClassificationTypeRegistryService classificationRegistry;
#pragma warning restore 649

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty<MjsClassifier>(creator: () => new MjsClassifier(this.classificationRegistry));
        }
    }
}
