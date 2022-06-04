using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace CommentsPlus.CommentClassifier
{
    //The quick brown fox jumps over the lazy dog
    //! Important note
    //? What's all this?
    //TODO Work remaining
    //ToDo@NN: Some work remaining for you
    //x object q = dt(42); //What is your question?
    //// double pi = Math.PI;
    //!? Wait, what‽

    //TODO: More work remaining
    // TODO: Even more work remaining

    //# Bold Test
    //¤ Removed ¤
    //x Removed x
    //WTF I don't even?
    //‽ Not in the slightest
    //HACK Hackety hack ack!
    //HACK: Not a crack
    // HACK: Slash!
    /////////////////////////////////////////
    ////string commentedOut = OldMethod(a++); /* an old style comment */

    /*? hallo for en kommentar!? */
    /*! A long comment - will it get bold!? 
     * Should this be bold as well?
     * Another line
   */

    //+? The quick brown fox jumps over the lazy dog! 0123456789 AbBbCcDdEeFf Jackdaws love my big sphinx of quartz.

    enum Classification
    {
        None,
        Important,
        Sub,
        Ressource,
        Removed,
        Task,
        Rainbow,
        Chapter,
        Pattern,
        Version,
        Example,
        Super
    }

    /*!? Normal comment - should be italics '*/
    static class Constants
    {
        //! Important
        public const string ImportantComment = "hlwComment - Important";
        public const string ImportantHtmlComment = "hlwComment HTML - Important";
        public const string ImportantXmlComment = "hlwComment XML - Important";
        //? Question
        public const string SubComment = "hlwComment - Sub";
        public const string SubHtmlComment = "hlwComment HTML - Sub";
        public const string SubXmlComment = "hlwComment XML - Sub";
        //!? WAT
        public const string RessourceComment = "hlwComment - Ressource";

        //x Removed
        public const string RemovedComment = "hlwComment - Removed";
        public const string RemovedHtmlComment = "hlwComment HTML - Removed";
        public const string RemovedXmlComment = "hlwComment XML - Removed";
        //TODO: This does not need work
        public const string TaskComment = "hlwComment - Task";
        public const string TaskHtmlComment = "hlwComment HTML - Task";
        public const string TaskXmlComment = "hlwComment XML - Task";

        public const string RainbowComment = "hlwComment - Rainbow";

        public const string ChapterComment = "hlwComment - Chapter";
        public const string ChapterHtmlComment = "hlwComment HTML - Chapter";
        public const string ChapterXmlComment = "hlwComment XML - Chapter";

        public const string PatternComment = "hlwComment - Pattern";

        public const string VersionComment = "hlwComment - Version";
        public const string VersionHtmlComment = "hlwComment HTML - Version";
        public const string VersionXmlComment = "hlwComment XML - Version";

        public const string ExampleComment = "hlwComment - Example";

        public const string SuperComment = "hlwComment - Super";


        //x public const string LargeComment = "Comment + Large";
        //x public const string LargerComment = "Comment ++ Large";

        public static readonly Color ImportantColor = Colors.Red;
        public static readonly Color SubColor = Color.FromRgb(255, 0, 255);
        public static readonly Color RessourceColor = Color.FromRgb(0, 190, 164);
        public static readonly Color RemovedColor = Colors.Gray;
        public static readonly Color TaskColor = Colors.Orange;
        public static readonly Color ChapterColor = Color.FromRgb(255, 0, 255);
        public static readonly Color PatternColor = Color.FromRgb(64, 176, 255);
        public static readonly Color VersionColor = Color.FromRgb(224, 176, 255);
        public static readonly Color ExampleColor = Color.FromRgb(150, 255, 0);

    }

    public static class ClassificationDefinitions
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.ExampleComment)]
        internal static ClassificationTypeDefinition ExampleCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.SuperComment)]
        internal static ClassificationTypeDefinition SuperCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.VersionComment)]
        internal static ClassificationTypeDefinition VersionCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.PatternComment)]
        internal static ClassificationTypeDefinition PatternCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.ChapterComment)]
        internal static ClassificationTypeDefinition ChapterCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.ImportantComment)]
        internal static ClassificationTypeDefinition ImportantCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.SubComment)]
        internal static ClassificationTypeDefinition SubCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.RessourceComment)]
        internal static ClassificationTypeDefinition RessourceCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.RemovedComment)]
        internal static ClassificationTypeDefinition StrikeoutCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.TaskComment)]
        internal static ClassificationTypeDefinition TaskCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Comment")]
        [Name(Constants.RainbowComment)]
        internal static ClassificationTypeDefinition RainbowCommentClassificationType = null;

        #region HTML

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("HTML Comment")]
        [Name(Constants.VersionHtmlComment)]
        internal static ClassificationTypeDefinition VersionHtmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("HTML Comment")]
        [Name(Constants.ImportantHtmlComment)]
        internal static ClassificationTypeDefinition ImportantHtmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("HTML Comment")]
        [Name(Constants.ChapterHtmlComment)]
        internal static ClassificationTypeDefinition ChapterHtmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("HTML Comment")]
        [Name(Constants.SubHtmlComment)]
        internal static ClassificationTypeDefinition SubHtmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("HTML Comment")]
        [Name(Constants.RemovedHtmlComment)]
        internal static ClassificationTypeDefinition StrikeoutHtmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("HTML Comment")]
        [Name(Constants.TaskHtmlComment)]
        internal static ClassificationTypeDefinition TaskHtmlCommentClassificationType = null;

        #endregion

        #region XML

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("XML Comment")]
        [Name(Constants.VersionXmlComment)]
        internal static ClassificationTypeDefinition VersionXmlCommentClassificationType = null;


        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("XML Comment")]
        [Name(Constants.ImportantXmlComment)]
        internal static ClassificationTypeDefinition ImportantXmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("XML Comment")]
        [Name(Constants.SubXmlComment)]
        internal static ClassificationTypeDefinition SubXmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("XML Comment")]
        [Name(Constants.ChapterXmlComment)]
        internal static ClassificationTypeDefinition ChapterXmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("XML Comment")]
        [Name(Constants.RemovedXmlComment)]
        internal static ClassificationTypeDefinition StrikeoutXmlCommentClassificationType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [BaseDefinition("Xml Comment")]
        [Name(Constants.TaskXmlComment)]
        internal static ClassificationTypeDefinition TaskXmlCommentClassificationType = null;

        #endregion
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.PatternComment)]
    [Name(Constants.PatternComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class PatternCommentFormat : ClassificationFormatDefinition
    {
        public PatternCommentFormat()
        {
            this.DisplayName = Constants.PatternComment + " (//++)";
            this.ForegroundColor = Constants.PatternColor;
            this.IsBold = true;
            this.TextDecorations = new System.Windows.TextDecorationCollection();
            this.TextDecorations.Add(System.Windows.TextDecorations.Underline);
            this.FontRenderingSize = 20;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ChapterComment)]
    [Name(Constants.ChapterComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ChapterCommentFormat : ClassificationFormatDefinition
    {
        public ChapterCommentFormat()
        {
            this.DisplayName = Constants.ChapterComment + " (//??)";
            this.ForegroundColor = Constants.ChapterColor;
            this.IsBold = true;
            this.TextDecorations = new System.Windows.TextDecorationCollection();
            this.TextDecorations.Add(System.Windows.TextDecorations.Underline);
            this.FontRenderingSize = 30;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.SuperComment)]
    [Name(Constants.SuperComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class SuperCommentFormat : ClassificationFormatDefinition
    {
        public SuperCommentFormat()
        {
            this.DisplayName = Constants.SuperComment + " (//!!)";
            this.ForegroundColor = Constants.ImportantColor;
            this.IsBold = true;
            this.TextDecorations = new System.Windows.TextDecorationCollection();
            this.TextDecorations.Add(System.Windows.TextDecorations.Underline);
            this.FontRenderingSize = 30;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ImportantComment)]
    [Name(Constants.ImportantComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ImportantCommentFormat : ClassificationFormatDefinition
    {
        public ImportantCommentFormat()
        {
            this.DisplayName = Constants.ImportantComment + " (//!)";
            this.ForegroundColor = Constants.ImportantColor;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ExampleComment)]
    [Name(Constants.ExampleComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ExampleCommentFormat : ClassificationFormatDefinition
    {
        public ExampleCommentFormat()
        {
            this.DisplayName = Constants.ExampleComment + " (//%)";
            this.ForegroundColor = Constants.ExampleColor;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.VersionComment)]
    [Name(Constants.VersionComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class VersionCommentFormat : ClassificationFormatDefinition
    {
        public VersionCommentFormat()
        {
            this.DisplayName = Constants.VersionComment + " (//=)";
            this.ForegroundColor = Constants.VersionColor;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.SubComment)]
    [Name(Constants.SubComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class SubCommentFormat : ClassificationFormatDefinition
    {
        public SubCommentFormat()
        {
            this.DisplayName = Constants.SubComment + " (//?)";
            this.ForegroundColor = Constants.SubColor;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.RessourceComment)]
    [Name(Constants.RessourceComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class RessourceCommentFormat : ClassificationFormatDefinition
    {
        public RessourceCommentFormat()
        {
            this.DisplayName = Constants.RessourceComment + " (//>>)";
            this.ForegroundColor = Constants.RessourceColor;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.RemovedComment)]
    [Name(Constants.RemovedComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class StrikeoutCommentFormat : ClassificationFormatDefinition
    {
        public StrikeoutCommentFormat()
        {
            this.DisplayName = Constants.RemovedComment + " (//x)";
            this.ForegroundColor = Constants.RemovedColor;
            this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.TaskComment)]
    [Name(Constants.TaskComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class TaskCommentFormat : ClassificationFormatDefinition
    {
        public TaskCommentFormat()
        {
            this.DisplayName = Constants.TaskComment + " (//TODO)";
            this.ForegroundColor = Constants.TaskColor;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.RainbowComment)]
    [Name(Constants.RainbowComment)]
    [UserVisible(false)]
    [Order(After = Priority.High)]
    public sealed class RainbowCommentFormat : ClassificationFormatDefinition
    {
        public RainbowCommentFormat()
        {
            this.DisplayName = Constants.RainbowComment + " (//+?)";
            this.ForegroundBrush = new LinearGradientBrush(new GradientStopCollection() {
                new GradientStop(Colors.Red, 0.0),
                new GradientStop(Colors.Orange, 0.22),
                new GradientStop(Colors.Yellow, 0.45),
                new GradientStop(Colors.Lime, 0.65),
                new GradientStop(Color.FromRgb(30,190,255), 0.89),
                new GradientStop(Colors.Violet, 1.0),
            }, 0.0);
            this.IsBold = true;
        }
    }

    #region HTML

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ImportantHtmlComment)]
    [Name(Constants.ImportantHtmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ImportantHtmlCommentFormat : ClassificationFormatDefinition
    {
        public ImportantHtmlCommentFormat()
        {
            this.DisplayName = Constants.ImportantHtmlComment + " (<!--!)";
            this.ForegroundColor = Constants.ImportantColor;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.VersionHtmlComment)]
    [Name(Constants.VersionHtmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class VersionHtmlCommentFormat : ClassificationFormatDefinition
    {
        public VersionHtmlCommentFormat()
        {
            this.DisplayName = Constants.VersionHtmlComment + " (<!--=)";
            this.ForegroundColor = Constants.VersionColor;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.SubHtmlComment)]
    [Name(Constants.SubHtmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class SubHtmlCommentFormat : ClassificationFormatDefinition
    {
        public SubHtmlCommentFormat()
        {
            this.DisplayName = Constants.SubHtmlComment + " (<!--?)";
            this.ForegroundColor = Constants.SubColor;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ChapterHtmlComment)]
    [Name(Constants.ChapterHtmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ChapterHtmlCommentFormat : ClassificationFormatDefinition
    {
        public ChapterHtmlCommentFormat()
        {
            this.DisplayName = Constants.SubHtmlComment + " (<!--??)";
            this.ForegroundColor = Constants.ChapterColor;
            this.IsBold = true;
            this.TextDecorations = new System.Windows.TextDecorationCollection();
            this.TextDecorations.Add(System.Windows.TextDecorations.Underline);
            this.FontRenderingSize = 30;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.RemovedHtmlComment)]
    [Name(Constants.RemovedHtmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class StrikeoutHtmlCommentFormat : ClassificationFormatDefinition
    {
        public StrikeoutHtmlCommentFormat()
        {
            this.DisplayName = Constants.RemovedHtmlComment + " (<!--x)";
            this.ForegroundColor = Constants.RemovedColor;
            this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.TaskHtmlComment)]
    [Name(Constants.TaskHtmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class TaskHtmlCommentFormat : ClassificationFormatDefinition
    {
        public TaskHtmlCommentFormat()
        {
            this.DisplayName = Constants.TaskHtmlComment + " (<!--TODO)";
            this.ForegroundColor = Constants.TaskColor;
        }
    }

    #endregion

    #region XML

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ImportantXmlComment)]
    [Name(Constants.ImportantXmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ImportantXmlCommentFormat : ClassificationFormatDefinition
    {
        public ImportantXmlCommentFormat()
        {
            this.DisplayName = Constants.ImportantXmlComment + " (<!--!)";
            this.ForegroundColor = Constants.ImportantColor;
            this.IsBold = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.VersionXmlComment)]
    [Name(Constants.VersionXmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class VersionXmlCommentFormat : ClassificationFormatDefinition
    {
        public VersionXmlCommentFormat()
        {
            this.DisplayName = Constants.VersionXmlComment + " (<!--=)";
            this.ForegroundColor = Constants.VersionColor;
            this.IsBold = true;
        }
    }


    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.SubXmlComment)]
    [Name(Constants.SubXmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class SubXmlCommentFormat : ClassificationFormatDefinition
    {
        public SubXmlCommentFormat()
        {
            this.DisplayName = Constants.SubXmlComment + " (<!--?)";
            this.ForegroundColor = Constants.SubColor;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.ChapterXmlComment)]
    [Name(Constants.ChapterXmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class ChapterXmlCommentFormat : ClassificationFormatDefinition
    {
        public ChapterXmlCommentFormat()
        {
            this.DisplayName = Constants.SubXmlComment + " (<!--??)";
            this.ForegroundColor = Constants.ChapterColor;
            this.IsBold = true;
            this.TextDecorations = new System.Windows.TextDecorationCollection();
            this.TextDecorations.Add(System.Windows.TextDecorations.Underline);
            this.FontRenderingSize = 30;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.RemovedXmlComment)]
    [Name(Constants.RemovedXmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class StrikeoutXmlCommentFormat : ClassificationFormatDefinition
    {
        public StrikeoutXmlCommentFormat()
        {
            this.DisplayName = Constants.RemovedXmlComment + " (<!--x)";
            this.ForegroundColor = Constants.RemovedColor;
            this.TextDecorations = System.Windows.TextDecorations.Strikethrough;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Constants.TaskXmlComment)]
    [Name(Constants.TaskXmlComment)]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    public sealed class TaskXmlCommentFormat : ClassificationFormatDefinition
    {
        public TaskXmlCommentFormat()
        {
            this.DisplayName = Constants.TaskXmlComment + " (<!--TODO)";
            this.ForegroundColor = Constants.TaskColor;
        }
    }

    #endregion
}
