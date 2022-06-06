using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommentsPlus.Annotations;
using CommentsPlus.CommentClassifier;
using CommentsPlus.TaskList;
using Mvvm.Core.Command;

namespace CommentsPlus.Overview.ViewModels
{
    //see link:SolutionList.xaml
    public class SolutionListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProjectViewModel> projects
        {
            get => ViewModelLocator.Instance.Scanner.Projects;
        }

        public static SolutionListViewModel Instance;

        public SolutionListViewModel()
        {
            Instance = this;

            _WatchDebug = true;
            _WatchChapter = true;
            _WatchTodo = true;
            _WatchSuper = true;
            HelloWorldClick = new RelayCommand(() => Logger.Log("Clicked"));
            AltClick = new RelayCommand(() => Logger.Log("Alted"));

            ScanSolutionCommand = new RelayCommand(() => ViewModelLocator.Instance.Scanner.ScanSolution());
            UpActiveDocCommand = new RelayCommand(() => ViewModelLocator.Instance.Scanner.ScanActivedocument());

        }

        

        

        private bool _WatchTodo;
        public bool WatchTodo
        {
            get { return _WatchTodo; }
            set
            {
                _WatchTodo = value;
                CommentTagger.BookmarkTags.FirstOrDefault(a=> a.cls == Classification.Task).isWatched = value;
                OnPropertyChanged();
                ViewModelLocator.Instance.Scanner.ScanSolution();
            }
        }

        private bool _WatchDebug;
        public bool WatchDebug
        {
            get { return _WatchDebug; }
            set
            {
                _WatchDebug = value;
                CommentTagger.BookmarkTags.FirstOrDefault(a => a.cls == Classification.Debug).isWatched = value;
                OnPropertyChanged();
                ViewModelLocator.Instance.Scanner.ScanSolution();
            }
        }

        private bool _WatchChapter;
        public bool WatchChapter
        {
            get { return _WatchChapter; }
            set
            {
                _WatchChapter = value;
                CommentTagger.BookmarkTags.FirstOrDefault(a => a.cls == Classification.Chapter).isWatched = value;
                OnPropertyChanged();
                ViewModelLocator.Instance.Scanner.ScanSolution();
            }
        }

        private bool _WatchSuper;
        public bool WatchSuper
        {
            get { return _WatchSuper; }
            set
            {
                _WatchSuper = value;
                CommentTagger.BookmarkTags.FirstOrDefault(a => a.cls == Classification.Super).isWatched = value;
                OnPropertyChanged();
                ViewModelLocator.Instance.Scanner.ScanSolution();

            }
        }


        public ICommand ScanSolutionCommand { get; }
        public ICommand UpActiveDocCommand { get; }

        public ICommand HelloWorldClick { get; }
        public ICommand AltClick { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
