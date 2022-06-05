using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;


//todo remove debug lines once figured out.
namespace CommentsPlus.TaskList
{
    public class ProjectHierarchy : IVsHierarchyEvents
    {
        public IVsHierarchy vsHierarchy;
        public Project Project;
        public string Name;

        public ProjectHierarchy(IVsHierarchy h)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            vsHierarchy = h;
            vsHierarchy.AdviseHierarchyEvents(this, out var cookie);

            vsHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out var proj);
            Project = (proj as Project);
            Name = Project?.Name;
        }

        public int OnItemAdded(uint itemidParent, uint itemidSiblingPrev, uint itemidAdded)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //Logger.Log("Item Added");
            //Logger.Log($"Project {Project?.Name}");
            vsHierarchy.GetCanonicalName(itemidAdded, out var fullName);
            Logger.Log($"ProjectHierarchy Item Added Canonical Name : {fullName}");

            vsHierarchy.GetProperty(itemidAdded, (int)__VSHPROPID.VSHPROPID_FirstChild, out object item);
            var itemName = item as string;
            Logger.Log($"ProjectHierarchy Item added Get Property : {itemName}");

            Logger.Log($"ProjectHierarchy Item added Project: {Project?.Name}");
            vsHierarchy.GetCanonicalName(itemidParent, out var pName);
            //Logger.Log($"Parent Canonical Name : {pName}");

            ////  Logger.Log($"itemidParent : {itemidParent}");
            ////  Logger.Log($"itemidSiblingPrev : {itemidSiblingPrev}");
            //Logger.Log($"item id: {itemidAdded}");
            //Logger.Log("-------------------");
            //Logger.Log("");

             ViewModelLocator.Instance.Scanner.ScanFile(fullName, Project);
            return VSConstants.S_OK;
        }

        public int OnItemsAppended(uint itemidParent)
        {
            vsHierarchy.GetCanonicalName(itemidParent, out var fullName);
            Logger.Log($"ProjectHierarchy Items Appended parent: {fullName}");
            return 1;
        }

        public int OnItemDeleted(uint itemid)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //Logger.Log("Item Deleted");
            //Logger.Log($"Project {Project?.Name}");
            vsHierarchy.GetCanonicalName(itemid, out var name);
            Logger.Log($"ProjectHierarchy Item deleted :Canonical Name : {name}");

            vsHierarchy.GetProperty(itemid, (int)__VSHPROPID.VSHPROPID_FirstChild, out object item);
            var itemName = item as string;
            Logger.Log($"ProjectHierarchy Item deleted Get Property : {itemName}");

            //Logger.Log($"id : {itemid}");
            //Logger.Log("-------------------");
            //Logger.Log("");

          
            ViewModelLocator.Instance.Scanner.RemoveFile(name, Project.Name);
            
            return 1;
        }

        public int OnPropertyChanged(uint itemid, int propid, uint flags)
        {
            return VSConstants.S_OK;
        }

        public int OnInvalidateItems(uint itemidParent)
        {
            return VSConstants.S_OK;
        }

        public int OnInvalidateIcon(IntPtr hicon)
        {
            return VSConstants.S_OK;
        }
    }






    //## ViewModelLocator
    //todo add listening in package!
    //url https://qa.wujigu.com/qa/?qa=1008648/c%23-visual-studio-sdk-handling-file-add-remove-and-rename-events
    //see in region the answer contained in that website (in case it goes down)

    //>> file to modify link:CommentsPlus17Package.cs:""
    #region answer
//    First, don't use DTE if you can help it. It's a very incomplete, shaky abstraction papered over an extremely complex interface. Having said that, I admit that sometimes it's super handy because the equivalent either can't be done without it(rare) or the alternate code would be quite long (less rare).

//There are two concepts being conflated here.The first is the Running Document Table(RDT). The RDT represents all the open files(including the open.sln and project files). You can subscribe to RDT events to be notified of files being opened, closed, renamed, etc.But these events are for open files only!

//The second concept is the project system.Each project loaded and displayed in the solution explorer is loaded by the project system for that project's type. C++ projects, C# projects, F# projects, WIX installer projects, etc. all have different project systems. There can even be custom project systems implemented by extensions. It sounds like you want to know about events in the project system, and not events for (just) open files. So your focus is the project system. However, since all project systems have different implementations, this becomes very tricky. VS is moving towards a common project system (CPS), but it's not 100% there yet, and even when it is there remains the problem of all the legacy extensions, etc.

//You can subscribe to general "hierarchy" events which all project systems must furnish.They'll tell you for example when a file is added or removed (really, when a hierarchy item (node) is added or removed, since there's not necessarily a correspondence between files and hierarchy items). There's also an event that says the entire hierarchy has been invalidated -- a sort of refresh where you have to discard everything you know about the project and gather up new info.

//Rename is probably the hardest thing to detect.Every project system implements it differently.In some project systems, a rename will present itself as a node deletion followed by a node addition, with no solid way to identify that it was due to a rename.

//To sum up, nothing is as simple as it seems, particularly when it comes to project systems (one of the least extensible parts of Visual Studio). You'll likely end up with code that is specific to one or a handful of project systems, but won't work universally. (After all, not all projects even represent file hierarchies! And those that do still have folders, special reference nodes, etc.that aren't files.)

//Some concrete pointers in the right direction:

//Implement IVsSolutionEvents3 to be notified of a project being loaded/unloaded(and IVsSolutionEvents4 to be notified of a project itself being renamed). Register that object as a listener in your package initialization code(make sure your package is loaded before a solution is opened) via the SVsSolutionBuildManager service(cast to IVsSolutionBuildManager3 and call AdviseUpdateSolutionEvents3 on it).
//Implement IVsHierarchyEvents to be notified of project changes like node properties changing(use the __VSHPROPID enum to find out which is which), nodes being added, removed, invalidated, etc.Call AdviseHierarchyEvents on the IVsHierarchy object passed to the IVsSolutionEvents3's OnAfterProjectOpen implementation to register the event listener object.
    #endregion
    //todo remove commented code here !
    public class ViewModelLocator : IVsSolutionEvents3
    {
        private static ViewModelLocator instance;
        private static object lockKey = new object();

        List<ProjectHierarchy> Projects = new List<ProjectHierarchy>();

        public static ViewModelLocator Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockKey)
                    {
                        if (instance == null)
                        {
                            instance = new ViewModelLocator();
                        }
                    }
                }

                return instance;
            }
        }

        //! Commented here
        //public BookmarksPaneViewModel BookmarksListViewModel { get; }

        public BookmarkScanner Scanner { get; }
        //! end of comment

        private ViewModelLocator()
        {
            //! Commented here
            Scanner = new BookmarkScanner(/*ConfigService.Current.CommentConfiguration*/);
            //BookmarksListViewModel = new BookmarksPaneViewModel(ConfigService.Current.CommentConfiguration, Scanner);
            //! end of comment
            //Scanner.ScanSolution();
        }

        private ProjectItemsEvents projectItemsEvents;
        private SolutionEvents solutionEvents;
        // private DocumentEvents documentEvents;

        public void SubscribeToEvents(Events2 events)
        {
            // documentEvents = events.DocumentEvents;
            //documentEvents.DocumentSaved += DocumentEvents_DocumentSaved;

            projectItemsEvents = events.ProjectItemsEvents;
            
            projectItemsEvents.ItemAdded += Scanner.ScanProjectItem;
            


            // projectItemsEvents.ItemRemoved += Scanner.RemoveProjectItem;
            // projectItemsEvents.ItemRenamed += Scanner.RenameProjectItem;

            solutionEvents = events.SolutionEvents;
            
             solutionEvents.Opened += Scanner.ScanSolution;
             solutionEvents.ProjectAdded += Scanner.ScanProject;
            solutionEvents.ProjectRemoved += Scanner.RemoveProject;
             solutionEvents.ProjectRenamed += Scanner.RenameProject;
            

            
        }

        

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
        {
            var p = new ProjectHierarchy(pHierarchy);
            Projects.Add(p);
            //Scanner.ScanProject(p.Project);
            return 1;
        }


        #region Not Implemented IVsSolutionEvents3 Methods

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
           => VSConstants.S_OK;

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
           => VSConstants.S_OK;

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
           => VSConstants.S_OK;

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
           => VSConstants.S_OK;

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
           => VSConstants.S_OK;

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
           => VSConstants.S_OK;

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
           => VSConstants.S_OK;

        public int OnBeforeCloseSolution(object pUnkReserved)
           => VSConstants.S_OK;

        public int OnAfterCloseSolution(object pUnkReserved)
           => VSConstants.S_OK;

        public int OnAfterMergeSolution(object pUnkReserved)
           => VSConstants.S_OK;

        public int OnBeforeOpeningChildren(IVsHierarchy pHierarchy)
           => VSConstants.S_OK;

        public int OnAfterOpeningChildren(IVsHierarchy pHierarchy)
           => VSConstants.S_OK;

        public int OnBeforeClosingChildren(IVsHierarchy pHierarchy)
           => VSConstants.S_OK;

        public int OnAfterClosingChildren(IVsHierarchy pHierarchy)
           => VSConstants.S_OK;

        #endregion
    }







}
