using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace esLang
{
    internal static class MjsClassifierTypes
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        // Declare a classification type that is assigned to spans, and can be assigned a format.
        [Export]
        [Name("MjsRandom")]
        private static ClassificationTypeDefinition typeDefinition;
#pragma warning restore 169
    }
}
