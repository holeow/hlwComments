﻿//#define DIAG_TIMING
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Microsoft.Win32;

namespace CommentsPlus.CommentClassifier
{
    [Export(typeof(IViewTaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(ClassificationTag))]
    public class CommentTaggerProvider : IViewTaggerProvider
    {
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService Aggregator = null;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag
        {
            return new CommentTagger(ClassificationRegistry,
                 Aggregator.CreateTagAggregator<IClassificationTag>(buffer)) as ITagger<T>;
        }
    }

    class CommentTagger : ITagger<ClassificationTag>
    {
        Dictionary<Classification, ClassificationTag> _classifications;
        Dictionary<Classification, ClassificationTag> _htmlClassifications;
        Dictionary<Classification, ClassificationTag> _xmlClassifications;

        ITagAggregator<IClassificationTag> _aggregator;

        static bool _enabled;

        static readonly string[] Comments = { "///","//", "'", "#", "<!--"};

        static readonly string[] ImportantComments = { "! ","warning ", "important ", "bug ", "issue ", "notice ", "warning:", "important:", "bug:", "issue:", "notice:" };
        static readonly string[] SubComments = { "? " ,"# ", "sub ", "zone " };
        static readonly string[] RessourceComments = { ">> ","source " ,"src ", "ressource ", "url ", "guide ", "see ", "source:", "src:", "ressource:", "url:", "guide:", "see:" }; 
        static readonly string[] RemovedComments = { "x ", "¤ ", "// ", "//", "done ", "done:" };
        static readonly string[] TaskComments = { "TODO ", "TODO:", "TODO@", "HACK ", "HACK:" }; 
        static readonly string[] RainbowComments = { "+? ","♥" }; //シ  
        static readonly string[] ChapterComments = { "?? " ,"## ", "chapter ","ch " };
        static readonly string[] PatternComments = {"++ ", "singleton ", "notify ", "dispose ", "constructor ", "finalizer ", "destructor ", "visionary "};
        static readonly string[] VersionComments = { "= ", "version ", "v ", "version:" };


        static readonly List<ITagSpan<ClassificationTag>> EmptyTags = new List<ITagSpan<ClassificationTag>>();

#pragma warning disable 67
        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

        internal CommentTagger(IClassificationTypeRegistryService registry, ITagAggregator<IClassificationTag> aggregator)
        {
            _classifications = new string[]
                {
                    Constants.ImportantComment, Constants.SubComment, Constants.RessourceComment, Constants.RemovedComment, Constants.TaskComment, Constants.RainbowComment, Constants.ChapterComment , Constants.PatternComment, Constants.VersionComment  }
                    .ToDictionary(GetClassification, s => new ClassificationTag(registry.GetClassificationType(s)));

            _htmlClassifications = new string[] { Constants.ImportantHtmlComment, Constants.SubHtmlComment, Constants.RessourceComment, Constants.RemovedHtmlComment, Constants.TaskHtmlComment, Constants.VersionHtmlComment, Constants.ChapterHtmlComment , Constants.RainbowComment, Constants.PatternComment }
                    .ToDictionary(GetClassification, s => new ClassificationTag(registry.GetClassificationType(s)));

            _xmlClassifications = new string[] { Constants.ImportantXmlComment, Constants.SubXmlComment, Constants.RessourceComment, Constants.RemovedXmlComment, Constants.TaskXmlComment, Constants.VersionXmlComment, Constants.ChapterXmlComment , Constants.RainbowComment, Constants.PatternComment }
                    .ToDictionary(GetClassification, s => new ClassificationTag(registry.GetClassificationType(s)));

            _aggregator = aggregator;
        }

        static CommentTagger()
        {
            _enabled = IsEnabled();
        }

        static bool IsEnabled()
        {
            bool res = true;

            try
            {
                using (var subKey = Registry.CurrentUser.OpenSubKey("Software\\hlwComments", false))
                {
                    int value = Convert.ToInt32(subKey== null ?  1 : subKey.GetValue("EnableTags", 1));
                    res = value != 0;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to read registry: " + ex.Message, "CommentsPlus");
            }

            return res;
        }

        /// <summary>
        /// Gets all the tags that intersect the specified spans.
        /// </summary>
        /// <param name="spans">The spans to visit.</param>
        /// <returns>A TagSpan for each tag.</returns>
        public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
#if DIAG_TIMING
      var sw = Stopwatch.StartNew();

      //ToList seems to perform better than returning the iterator
      var tags = GetTagsInternal(spans).AsList();

      sw.Stop();
      if (sw.Elapsed > TimeSpan.FromMilliseconds(1))
        Trace.WriteLine("GetTags took: " + sw.Elapsed, "CommentsPlus");

      return tags;
#else
            return GetTagsInternal(spans);
#endif
        }

        private IEnumerable<ITagSpan<ClassificationTag>> GetTagsInternal(NormalizedSnapshotSpanCollection spans)
        {
            if (!_enabled || spans.Count == 0)
                return EmptyTags;

            ITextSnapshot snapshot = spans[0].Snapshot;
            var contentType = snapshot.TextBuffer.ContentType;
            if (!contentType.IsOfType("code"))
                return EmptyTags;

            bool isMarkup = IsMarkup(contentType);

            var lookup = _classifications;
            if (IsHtmlMarkup(contentType))
                lookup = _htmlClassifications;
            else if (IsXmlMarkup(contentType))
                lookup = _xmlClassifications;

            List<ITagSpan<ClassificationTag>> resultTags = null;

            string previousCommentType = null;
            foreach (var tagSpan in _aggregator.GetTags(spans))
            {
                // find spans that the language service has already classified as comments ...
                string classificationName = tagSpan.Tag.ClassificationType.Classification;
                if (!classificationName.Contains("comment", StringComparison.OrdinalIgnoreCase))
                   continue;

                var nssc = tagSpan.Span.GetSpans(snapshot);
                if (nssc.Count > 0)
                {
                    // ... and from those, ones that match our comment strings
                    var snapshotSpan = nssc[0];

                    string text = snapshotSpan.GetText();
                    if (String.IsNullOrWhiteSpace(text))
                        continue;

                    string startCommentOnlyType = text.EqualsOneOf(Comments);
                    if (startCommentOnlyType != null)
                    {
                        previousCommentType = startCommentOnlyType;
                        continue;
                    }

                    //NOTE: markup comment span does not include comment start token
                    //NOTE: .js/.ts comment span has changed to not include comment start token
                    string commentType = text.StartsWithOneOf(Comments);
                    if (commentType == null)
                    {
                        if (!isMarkup && previousCommentType == null)
                            continue;

                        commentType = "";
                    }

                    previousCommentType = null;

                    int startIndex = commentType.Length;

                    int endTokenLength = 0;
                    if (isMarkup && commentType.Length > 0)
                    {
                        if (!text.EndsWith("-->"))
                            continue;

                        endTokenLength = 3;
                    }

                    //¤ int? removedSpanLength = null;
                    ClassificationTag ctag = null;
                    string match;
                    if (Match(text, startIndex, ImportantComments, out match))
                    {
                        ctag = lookup[Classification.Important];
                    }
                    else if (Match(text, startIndex, SubComments, out match))
                    {
                        ctag = lookup[Classification.Sub];
                    }
                    else if (Match(text, startIndex, VersionComments, out match))
                    {
                        ctag = lookup[Classification.Version];
                    }
                    else if (Match(text, startIndex, PatternComments, out match))
                    {
                        ctag = lookup[Classification.Pattern];
                    }
                    else if (Match(text, startIndex, ChapterComments, out match))
                    {
                        ctag = lookup[Classification.Chapter];
                    }
                    else if (Match(text, startIndex, RemovedComments, out match))
                    {
                        if (commentType == "//" && match == "//")
                        {
                            int len = startIndex + match.Length;
                            if (text.Length > len && text[len] != '/')
                                ctag = lookup[Classification.Removed];
                        }
                        else
                            ctag = lookup[Classification.Removed];

                        //¤ int index = text.IndexOf(commentType, startIndex + match.Length, StringComparison.Ordinal);
                        //¤ if (index > 0)
                        //¤ {
                        //¤     int len = text.Length - index;
                        //¤     removedSpanLength = text.Length - (startIndex + match.Length + len);
                        //¤ }
                    }
                    else if (Match(text, startIndex, TaskComments, StringComparison.OrdinalIgnoreCase, true, out match))
                    {
                        bool fix = FixTaskComment(text, startIndex, ref match);
                        ctag = lookup[Classification.Task];
                    }
                    else if (Match(text, startIndex, RessourceComments, out match))
                    {
                        ctag = lookup[Classification.Ressource];
                    }
                    else if (Match(text, startIndex, RainbowComments, out match))
                    {
                        ctag = lookup[Classification.Rainbow];
                    }

                    if (ctag != null)
                    {
                        int matchLength = commentType.Length + match.Length;

                        int spanLength = /*removedSpanLength ??*/ (snapshotSpan.Length /*- (matchLength + endTokenLength)*/);

                        var span = new SnapshotSpan(snapshotSpan.Snapshot, snapshotSpan.Start /*+ matchLength*/, spanLength);
                        var outTagSpan = new TagSpan<ClassificationTag>(span, ctag);

                        if (resultTags == null)
                            resultTags = new List<ITagSpan<ClassificationTag>>();

                        resultTags.Add(outTagSpan);
                    }
                }
            }

            return resultTags ?? EmptyTags;
        }

        private static bool FixTaskComment(string text, int startIndex, ref string match)
        {
            bool res = false;
            if (match != null && match.EndsWith("@", StringComparison.Ordinal))
            {
                int index = text.IndexOfAny(new char[] { ' ', '\t', ':' }, startIndex + match.Length);
                if (index >= 0)
                {
                    int len = (index - startIndex) + 1;
                    match = text.Substring(startIndex, len);
                    res = true;
                }
            }
            return res;
        }

        static bool Match(string commentText, int startIndex, string[] templates, out string match)
        {
            return Match(commentText, startIndex, templates, StringComparison.Ordinal, false, out match);
        }

        static bool Match(string commentText, int startIndex, string[] templates, StringComparison comparison, bool allowLeadingWhiteSpace, out string match)
        {
            bool lws = false;
            if (allowLeadingWhiteSpace && commentText.StartsWithWhiteSpace(startIndex))
            {
                lws = true;
                startIndex += 1;
            }
            match = commentText.StartsWithOneOf(startIndex, templates, comparison);
            if (lws && match != null)
                match = commentText.Substring(startIndex - 1, match.Length + 1);
            return match != null;
        }

        static Classification GetClassification(string s)
        {
            if (s.Contains("Important"))
                return Classification.Important;
            if (s.Contains("Sub"))
                return Classification.Sub;
            if (s.Contains("Removed"))
                return Classification.Removed;
            if (s.Contains("Task"))
                return Classification.Task;
            if (s.Contains("Ressource"))
                return Classification.Ressource;
            if (s.Contains("Rainbow"))
                return Classification.Rainbow;
            if (s.Contains("Chapter"))
            {
                return Classification.Chapter;
            }
            if (s.Contains("Pattern"))
            {
                return Classification.Pattern;
            }
            if (s.Contains("Version"))
            {
                return Classification.Version;
            }

            throw new ArgumentException($"Unknown classification type");
        }

        private bool IsMarkup(IContentType contentType)
        {
            bool res = IsHtmlMarkup(contentType) || IsXmlMarkup(contentType);

            return res;
        }

        private static bool IsHtmlMarkup(IContentType contentType)
        {
            bool res = contentType.IsOfType("html") || contentType.IsOfType("htmlx");
            return res;
        }

        private static bool IsXmlMarkup(IContentType contentType)
        {
            bool res = contentType.IsOfType("XAML") || contentType.IsOfType("XML");
            return res;
        }
    }
}
