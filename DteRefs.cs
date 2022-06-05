using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace CommentsPlus
{
    public class DteRefs
    {

        public static DTE DTE;
        public static DTE2 DTE2;
        public static Package Package;
        public static IVsSolution Solution;
        public static IVsRunningDocumentTable RDT;
    }
}