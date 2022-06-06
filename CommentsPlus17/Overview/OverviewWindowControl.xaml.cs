using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using CommentsPlus.TaskList;

namespace CommentsPlus.Overview
{
    /// <summary>
    /// Interaction logic for OverviewWindowControl.
    /// </summary>
    public partial class OverviewWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewWindowControl"/> class.
        /// </summary>
        public OverviewWindowControl()
        {
            this.InitializeComponent();
            //this.DataContext = ViewModelLocator.Instance.BookmarksListViewModel;
            this.DataContext = new ViewModels.OverviewViewModel();
        }

        
    }
}