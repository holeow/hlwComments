﻿using System;
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
        public BookmarksPaneViewModel BookmarksListViewModel { get; }

        public BookmarkScanner Scanner { get; }
        //! end of comment

        private ViewModelLocator()
        {
            //! Commented here
            Scanner = new BookmarkScanner(/*ConfigService.Current.CommentConfiguration*/);
            BookmarksListViewModel = new BookmarksPaneViewModel(/*ConfigService.Current.CommentConfiguration,*/ Scanner);
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
