/*
 * This is way harder than it needs to be :-( It works something like this:
 * - A content type ("mjs") is declared by an exported "ContentTypeDefinition", which has a base type ("code");
 * - A "FileExtensionToContentTypeDefinition" export maps an extension (".mjs") to the content type ("mjs");
 * - An export of type "IClassifierProvider" has an associated ContentType "mjs" and a GetClassifier method.
 * - Calls to GetClassifier are made to return an IClassifier for that content type.
 * - When "IClassifier.GetClassificationSpans" is called, it returns ClassificationSpans (spans with classification types).
 * - The classifier uses a "IClassificationTypeRegistryService" (saved during construction) to get the classification types
 * - Custom ClassificationTypes can be declared also. These effectively just classify a span of text.
 * - ClassificationFormatDefinition classes can be exported to declare styles to different classification types.
*/

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace esLang
{
    internal class MjsClassifier : IClassifier
    {
        private readonly IClassificationType mjsRandom;
        private readonly IClassificationType keyword;
        private readonly IClassificationType comment;

        internal MjsClassifier(IClassificationTypeRegistryService registry)
        {
            this.mjsRandom = registry.GetClassificationType("MjsRandom");
            this.keyword = registry.GetClassificationType("keyword");
            this.comment = registry.GetClassificationType("comment");
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>()
            {
                new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)), this.mjsRandom)
            };

            return result;
        }

        #endregion
    }
}
