using Microsoft.VisualStudio.Shell;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommentsPlus.TaskList;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace CommentsPlus
{


 

    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    ///! Hook events doesn't work if we don't autoload.
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionOpening_string, PackageAutoLoadFlags.BackgroundLoad)]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[Guid(CommentsPlus17Package.PackageGuidString)]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideToolWindow(typeof(CommentsPlus.Overview.OverviewWindow))]
	public sealed class CommentsPlus17Package : AsyncPackage
	{
        //>> We store a solution service
        

        private IVsSolution solService;

        /// <summary>
        /// CommentsPlus17Package GUID string.
        /// </summary>
        public const string PackageGuidString = "6dda59d8-fd13-4d8b-b715-38674320ce66";

		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
            await base.InitializeAsync(cancellationToken, progress);


            
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var dte = (DTE)await GetServiceAsync(typeof(DTE));
            Assumes.Present(dte);

            var dte2 = (DTE2)await GetServiceAsync(typeof(SDTE));
            Assumes.Present(dte2);

            solService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
            Assumes.Present(solService);

            var rdt = await GetServiceAsync(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;
            Assumes.Present(rdt);

            //! Set globals
            DteRefs.Package = this;
            DteRefs.DTE = dte;
            DteRefs.DTE2 = dte2;
            DteRefs.Solution = solService;
            DteRefs.RDT = rdt;

            //! Hook events
            solService.AdviseSolutionEvents(ViewModelLocator.Instance, out var cookie);

            var events = dte2.Events as Events2;
            if (events != null)
                ViewModelLocator.Instance.SubscribeToEvents(events);

            if (await IsSolutionLoadedAsync())
            {
                //todo Remove this if not used

                //! Only handle it if it's solution, and ignore it if it's a folder.
                //solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value);
                //if (await IsSolutionLoadedAsync())
                //HandleOpenSolution();

                // ViewModelLocator.Instance.Scanner.ScanSolution();
            }
		    await CommentsPlus.Overview.OverviewWindowCommand.InitializeAsync(this);


        }

        private async Task<bool> IsSolutionLoadedAsync()
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync();

            ErrorHandler.ThrowOnFailure(solService.GetProperty((int)__VSPROPID.VSPROPID_IsSolutionOpen, out var value));

            return value is bool isSolOpen && isSolOpen;
        }

        #endregion
    }
}
