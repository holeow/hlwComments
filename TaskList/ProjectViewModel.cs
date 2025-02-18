﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Mvvm.Core;
using CommentsPlus.MVVM;

namespace CommentsPlus.TaskList
{
    //see link:ProjectView.xaml#L10
    public class ProjectViewModel : ViewModelBase 
    {
        private string classificationFilter;

        public Project Project { get; private set; }

        public string ProjectName { get; set; }

        //public BookmarkViewModel SelectedComment { get; set; }

        public ObservableCollection<FileViewModel> Files { get; set; }

        public ObservableCollection<FileViewModel> FilteredFiles
        {
            get
            {
                return Files.Filter(a => a.Bookmarks.Count > 0);
            }
        }

        

        public string ClassificationFilter
        {
            get => classificationFilter;
            set
            {
                classificationFilter = value;
                foreach (var file in Files)
                    file.ClassificationFilter = value;
            }
        }

        public ProjectViewModel(Project source, ObservableCollection<FileViewModel> files)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            Project = source;
            ProjectName = source.Name;
            this.Files = files;
            Files.CollectionChanged += Files_CollectionChanged;
        }

        private void Files_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged("FilteredFiles");
        }
    }
    //see link:FileView.xaml#L10 
    public class FileViewModel : ViewModelBase
    {
        readonly ObservableCollection<BookmarkViewModel> bookmarks;

        public string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);

        public string ClassificationFilter { get; set; }

        public ObservableCollection<BookmarkViewModel> Bookmarks
        {
            get
            {
                if (string.IsNullOrEmpty(ClassificationFilter))
                    return bookmarks;

                return bookmarks.Filter(b => b.Classification.ToString() == ClassificationFilter);
            }
        }

        public FileViewModel(string filePath, ObservableCollection<BookmarkViewModel> comments)
        {
            FilePath = filePath;
            bookmarks = comments;
        }
    }
}