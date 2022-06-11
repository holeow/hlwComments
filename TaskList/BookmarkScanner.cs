using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using VSIX.Package.Comments.Parsers;

namespace CommentsPlus.TaskList
{
    public class BookmarkScanner
    {
        //readonly ICommentConfiguration config;
        private string classificationFilter;

        readonly Dictionary<string, Document> openDocs
           = new Dictionary<string, Document>();

        //private WarningList warnings = new WarningList();

        public ObservableCollection<ProjectViewModel> Projects { get; }

        //[AlsoNotifyFor(nameof(Projects))]
        public string ClassificationFilter
        {
            get => classificationFilter;
            set
            {
                classificationFilter = value;

                foreach (var proj in Projects)
                    proj.ClassificationFilter = value;
            }
        }

        public BookmarkScanner(/*ICommentConfiguration config*/)
        {
            //this.config = config;
            Projects = new ObservableCollection<ProjectViewModel>();
        }


        public void Clear()
        {
            Projects.Clear();
        }

        public void ScanSolution()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (DteRefs.DTE == null)
            {
                return;
            }

            Projects.Clear();
            openDocs.Clear();

            

            var docs = DteRefs.DTE.Documents;
            if (docs.Count > 0)
            {
                for (var i = 1; i <= docs.Count; i++)
                {
                    var currDoc = docs.Item(i);

                    openDocs.Add(
                       currDoc.FullName,
                       currDoc);
                }
            }
            

            var projects = DteUtils.GetProjects(DteRefs.DTE2);

            foreach (var proj in projects.Where(a =>
                         a.FullName.EndsWith(".shproj") || a.FullName.EndsWith(".projitems")))
            {
                ScanProject(proj);
            }

            foreach (var proj in projects.Where(a =>
            !a.FullName.EndsWith(".shproj") || !a.FullName.EndsWith(".projitems")))
            {
                ScanProject(proj);
            }
            //foreach (var proj in DteUtils.GetProjects(DteRefs.DTE2))
            //{
            //    ScanProject(proj);
            //}

            //warnings.SyncWarnings();
        }

        public void ScanProject(Project project)
        {

            bool AlreadyExists(FileViewModel fvm)
            {
                foreach (var projectViewModel in Projects)
                {
                    if (projectViewModel.Files.SingleOrDefault(a => a.FilePath == fvm.FilePath) != null)
                    {
                        return true;
                    }
                }
                return false;
            }

            ThreadHelper.ThrowIfNotOnUIThread();

            //debug trying to get shproj file
            Logger.Log($"{project.FullName}");
            //>> .csproj for normal projects
            //>> shared projects return one shproj file and one projitems file.


            var files = new List<FileViewModel>();

            if (project.ProjectItems == null)
                return;
           
            foreach (ProjectItem item in project.ProjectItems)
            {
                
                var fvm = GetFileViewModels(item).ToList();
                var itemFiles =  new List<FileViewModel>();
                foreach (var file in fvm)
                {
                    
                    if(!AlreadyExists(file))
                        itemFiles.Add(file);

                   
                   

                }

                foreach (ProjectItem subItem in item.ProjectItems)
                {
                    var subFiles = GetFileViewModels(subItem).ToList();
                    foreach (var file in subFiles)
                    {
                        if (!itemFiles.Exists(a => a.FilePath == file.FilePath))
                        {
                            if(!AlreadyExists(file))
                                itemFiles.Add(file);
                        }
                    }

                    //itemFiles.AddRange(GetFileViewModels(subItem));
                }
                if (itemFiles.Any())
                {
                    files.AddRange(itemFiles);
                    
                }
                    
            }
            

            if (files.Any())
            {
                var proj = new ProjectViewModel(project, new ObservableCollection<FileViewModel>(files))
                {
                    ClassificationFilter = ClassificationFilter
                };

                Projects.Add(proj);
            }

            //warnings.SyncWarnings();
        }

        public void RenameProject(Project project, string oldName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            //Todo For some reason it's not working for C++ projects!

            // oldName contains the full path of the project file
            var oldProjName = Path.GetFileName(oldName).Split('.')[0];

            var target = Projects.SingleOrDefault(p => p.ProjectName == oldProjName);
            if (target != null)
                target.ProjectName = project.Name;

           // warnings.SyncWarnings();
        }

        public void RemoveProject(Project project)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var rm = Projects.Where(p => p.ProjectName == project.Name);

            foreach (var p in rm)
                Projects.Remove(p);

           // warnings.SyncWarnings();
        }

        public void ScanFile(string filePath, Project project)
        {
            
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!File.Exists(filePath) || !IsSupportedFile(filePath))
                return;

            // Remove any duplicate
            RemoveFile(filePath, project.Name);

            var lines = File.ReadAllLines(filePath).ToArray();

            var bookmarks = new ObservableCollection<BookmarkViewModel>(
            ExtractBookmarks(lines, filePath));

            

            var fileVm = new FileViewModel(filePath, bookmarks);

            var projVm = Projects.SingleOrDefault(p => p.ProjectName == project.Name);

            if (projVm == null)
            {
                Projects.Add(
                   new ProjectViewModel(project, new ObservableCollection<FileViewModel> { fileVm })
                   {
                       ClassificationFilter = ClassificationFilter
                   });
            }
            else
            {
                projVm.Files.Add(fileVm);
            }
           
        }

        public void RemoveFile(string filePath, string projectName)
        {
            if (filePath == null) return;
            openDocs.Remove(filePath);

            var project = Projects.SingleOrDefault(p => p.ProjectName == projectName);
            if (project != null)
            {
                var file = project.Files.SingleOrDefault(f => f.FilePath.Equals(filePath, StringComparison.OrdinalIgnoreCase));
                if (file != null)
                {
                    project.Files.Remove(file);
                }
            }

            //warnings.SyncWarnings();
        }

        public void ScanActivedocument()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
           
            if (DteRefs.DTE == null)
            {
                Logger.Log($" dterefs.dte is null!");
                return;
            }
            var doc = DteRefs.DTE.ActiveDocument;
            var path = doc.FullName;
            var project = doc.ProjectItem.ContainingProject;

            RemoveFile(path, project.Name);

            var bookmarks = ExtractBookmarks(doc);

           

            var fileVm = new FileViewModel(
               doc.FullName,
               new ObservableCollection<BookmarkViewModel>(bookmarks))
            {
                ClassificationFilter = ClassificationFilter
            };

            var projVm = Projects.SingleOrDefault(p => p.ProjectName == project.Name);

            if (projVm == null)
            {
                Projects.Insert(0,
                   new ProjectViewModel(project, new ObservableCollection<FileViewModel> { fileVm })
                   {
                       ClassificationFilter = ClassificationFilter
                   });
            }
            else
            {
                projVm.Files.Insert(0,fileVm);
                Projects.Move(Projects.IndexOf(projVm), 0);
            }

            //warnings.SyncWarnings();
        }
        
        public void ScanProjectItem(ProjectItem item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var project = item.ContainingProject;
            
            var projVm = Projects.SingleOrDefault(p => p.ProjectName == project.Name);

            var files = GetFileViewModels(item);

            if (projVm == null)
            {
                Projects.Add(
                   new ProjectViewModel(project, new ObservableCollection<FileViewModel>(files))
                   {
                       ClassificationFilter = ClassificationFilter
                   });
            }
            else
            {
                foreach (var file in files)
                    projVm.Files.Add(file);
            }

            //warnings.SyncWarnings();
        }

        private List<BookmarkViewModel> ExtractBookmarks(string[] lines, string fileName)
        {
            var bookmarks = new List<BookmarkViewModel>();

            if (!IsSupportedFile(fileName))
                return bookmarks;

            var extractor = GetExtractor(fileName);


            foreach (var ce in extractor.Extract(lines))
            {
                bookmarks.Add(
                      new BookmarkViewModel
                      {
                          FilePath = fileName,
                          Line = ce.Line,
                          Column = ce.Column,
                          Classification = ce.Classification,
                          Content = ce.Content
                      });

            }

            return bookmarks;
        }

        private IEnumerable<BookmarkViewModel> ExtractBookmarks(Document doc)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var path = doc.FullName;
            var projName = doc.ProjectItem.ContainingProject.Name;

            var docObj = doc.Object("TextDocument") as TextDocument;
            var editPt = docObj.StartPoint.CreateEditPoint();

            var txt = editPt.GetText(docObj.EndPoint);

            var lines = txt.Split(new[] { Environment.NewLine }, StringSplitOptions.None);


            var result = ExtractBookmarks(lines, path);

            return new List<BookmarkViewModel>(result);
        }

        private IEnumerable<FileViewModel> GetFileViewModels(ProjectItem item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            
            //? solution to bug: IsSupportedFile
            if (!IsSupportedFile(item.Name)) return new List<FileViewModel>();

            

            if (item.Document == null) return new List<FileViewModel>();
            if (openDocs.ContainsKey(item.Document.FullName))
            {
                var doc = openDocs[item.Document.FullName];

                return new List<FileViewModel>()
                {
                    new FileViewModel(
                        item.FileNames[0],
                        new ObservableCollection<BookmarkViewModel>(ExtractBookmarks(doc)))
                    {
                        ClassificationFilter = ClassificationFilter
                    }
                };
            }

            var files = new List<FileViewModel>();

            if (item == null)
                return files;

            if (File.Exists(item.Document.FullName) && IsSupportedFile(item.Name))
            {
                var lines = File.ReadAllLines(item.Document.FullName).ToArray();

                var bookmarks = new ObservableCollection<BookmarkViewModel>(
                   ExtractBookmarks(lines, item.Document.FullName));

                if (bookmarks.Any())
                {
                    files.Add(new FileViewModel(item.Document.FullName, bookmarks)
                    {
                        ClassificationFilter = ClassificationFilter
                    });
                }
            }

            if (item.ProjectItems?.Count > 0)
            {
                foreach (ProjectItem nestedItem in item.ProjectItems)
                    files.AddRange(GetFileViewModels(nestedItem));

            }

            return files;
        }

        private IBookmarkExtractor GetExtractor(string name)
        {
            if (name.EndsWith(".cs"))
                return new CSharpCommentExtractor();
            /*
            if (name.EndsWith(".vb"))
                return new VBCommentExtractor(config);

            if (name.EndsWith(".cpp") || name.EndsWith(".h"))
                return new CppCommentExtractor(config);

            if (name.EndsWith(".js") || name.EndsWith(".ts"))
                return new JSCommentExtractor(config);
            */
            //todo handle other languages
            throw new InvalidOperationException("File type is not supported.");
        }

        private bool IsSupportedFile(string fileName)
        {
            var name = fileName.ToLower();

            return name.EndsWith(".cs");
            //|| name.EndsWith(".xaml")
            //|| name.EndsWith(".html")
            //|| name.EndsWith(".fs")
            //|| name.EndsWith(".py")
            //|| name.EndsWith(".js")
            //|| name.EndsWith(".ts")
            //|| name.EndsWith(".vb")
            //|| name.EndsWith(".cpp")
            //|| name.EndsWith(".h");
        }

    }
}
