using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CommentsPlus.CommentClassifier;
using Microsoft.VisualStudio.Shell;
using Mvvm.Core.Command;

namespace CommentsPlus.TaskList
{
    //see link:BookmarkView.xaml 
    public sealed class BookmarkViewModel
    {
        //[DependsOn(nameof(FilePath))]
        public string FileName => Path.GetFileName(FilePath);
        public string FilePath { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
        public Classification Classification { get; set; }
        public string Content { get; set; } = string.Empty;

        public string LineText => $"Line: {Line}";
        public SolidColorBrush TextColor
        {
            get
            {
                switch (Classification)
                {
                    case Classification.Chapter: return new SolidColorBrush(Constants.ChapterColor);
                    case Classification.Super: return new SolidColorBrush(Constants.ImportantColor);
                    case Classification.Task: return new SolidColorBrush(Constants.TaskColor);
                    case Classification.Debug: return new SolidColorBrush(Constants.DebugColor);
                    default: return new SolidColorBrush(Colors.White);
                }
            }
        }

        public Thickness TextMargin
        {
            get
            {
                return new Thickness(3, 3, 3, 0);
            }
        }

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
