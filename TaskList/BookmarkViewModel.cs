using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using CommentsPlus.CommentClassifier;
using Microsoft.VisualStudio.Shell;
using Mvvm.Core.Command;

namespace CommentsPlus.TaskList
{
    public sealed class BookmarkViewModel
    {
        //[DependsOn(nameof(FilePath))]
        public string FileName => Path.GetFileName(FilePath);
        public string FilePath { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
        public Classification Classification { get; set; }
        public string Content { get; set; } = string.Empty;

        public RelayCommand GoToBookmarkCommand { get; }

        public BookmarkViewModel()
        {
            GoToBookmarkCommand = new RelayCommand(GoToBookmark, () => true);
        }
        private void GoToBookmark()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!File.Exists(FilePath))
                return;

            DteRefs.DTE.ItemOperations.OpenFile(FilePath);
            DteRefs.DTE.ExecuteCommand("Edit.Goto", Line.ToString());
        }
    }

    
}
