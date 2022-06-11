using CommentsPlus.CommentClassifier;

namespace VSIX.Package.Comments.Parsers
{
   public class CommentExtract
   {
      public int Line { get; }
      public int Column { get; }
      public Classification Classification { get; }
      public string Content { get; }

      public string ExactString { get; }

      public CommentExtract(int line, int column, Classification classification, string comment, string exactString)
      {
         Line = line;
         Column = column;
         Classification = classification;
         Content = comment;
         ExactString = exactString;
      }
   }
}