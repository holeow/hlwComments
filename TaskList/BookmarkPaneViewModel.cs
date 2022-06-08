using System;
using System.Collections.ObjectModel;
using CommentsPlus.CommentClassifier;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Mvvm.Core;
using Mvvm.Core.Command;

namespace CommentsPlus.TaskList
{
    public sealed class BookmarksPaneViewModel : ViewModelBase
    {
        //readonly ICommentConfiguration config;
        readonly BookmarkScanner scanner;
        private string selectedClassificationFilter;

        public ObservableCollection<ProjectViewModel> Projects => scanner.Projects;

        public ObservableCollection<string> ClassificationFilters { get; }
            = new ObservableCollection<string>();

        public RelayCommand ScanSolutionCommand { get; set; }

        //debug
        //todo remove the debug
        public RelayCommand DebugSolutionCommand { get; set; }

        public void DebugSolution()
        {
            foreach (var project in Projects)
            {
                Logger.Log("PN : "+project.ProjectName);
                foreach (var fileViewModel in project.Files)
                {
                    Logger.Log(("FN : " + fileViewModel.FilePath));
                    foreach (var bookmarkViewModel in fileViewModel.Bookmarks)
                    {
                        Logger.Log($"({fileViewModel.FilePath == DteRefs.DTE2.ActiveDocument.Path + DteRefs.DTE2.ActiveDocument.Name}){bookmarkViewModel.Classification} : {bookmarkViewModel.Content}");
                    }
                }
            }
            
        }

        //! End of debug

        public string SelectedClassificationFilter
        {
            get => selectedClassificationFilter;
            set
            {
                selectedClassificationFilter = value;

                if (value == "Any")
                    scanner.ClassificationFilter = string.Empty;
                else
                    scanner.ClassificationFilter = value;
            }
        }

        public BookmarksPaneViewModel(/*ICommentConfiguration configs,*/ BookmarkScanner scanner)
        {
            //config = configs;
            //configs.ChangesSaved += Configs_ChangesSaved;

            this.scanner = scanner;

            ScanSolutionCommand = new RelayCommand(scanner.ScanSolution, () => true);
            DebugSolutionCommand = new RelayCommand(DebugSolution, () => true);
            SetClassificationFilters();

            SelectedClassificationFilter = "Any";

        }

        private void SetClassificationFilters()
        {
            var selectedFilter = SelectedClassificationFilter;

            ClassificationFilters.Clear();
            ClassificationFilters.Add("Any");

            foreach (var cls in CommentTagger.BookmarkTags)
                ClassificationFilters.Add(cls.cls.ToString());

            // To Rest the selected filter in the view
            SelectedClassificationFilter
                = ClassificationFilters.Contains(selectedFilter)
                    ? selectedFilter
                    : "Any";
        }

        private void Configs_ChangesSaved(object sender, System.EventArgs e)
        {
            SetClassificationFilters();
        }
    }
}