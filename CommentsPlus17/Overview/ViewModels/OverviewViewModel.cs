using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CommentsPlus.Overview.ViewModels
{
    public class OverviewViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        public static OverviewViewModel instance;

        private object _SelectedViewModel;
        public object SelectedViewModel
        {
            get { return _SelectedViewModel; }
            set
            {
                _SelectedViewModel = value;
                OnPropertyChanged();
            }
        }

        public OverviewViewModel()
        {
            instance = this;
            this._SelectedViewModel = new SolutionListViewModel();
        }
    }
}
