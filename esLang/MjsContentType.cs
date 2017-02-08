using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace esLang
{
    internal static class MjsContentType
    {
        // Attributes to inform VS there is a content type named "mjs" which is used by ".mjs" files.
#pragma warning disable 169
        [Export]
        [Name("mjs")]
        [BaseDefinition("code")]
        private static ContentTypeDefinition mjsContentTypeDefinition;

        [Export]
        [FileExtension(".mjs")]
        [ContentType("mjs")]
        private static FileExtensionToContentTypeDefinition mjsFileExtensionDefinition;
#pragma warning restore 169
    }
}
