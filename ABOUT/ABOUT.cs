//about in shared

//todo BUG Things going in wrong places
// Dedoubled links when having multiple projects
//! is the first project receiving the things from the other ones?



//x todo BUG xaml.cs files not read when scanning solution...
// maybe when scanning ProjectItems, the xaml.cs file is a subItem of the xaml file so it is not found as a child of the project?
//>> see link:BookmarkScanner.cs:public%20void%20ScanProjectItem(ProjectItem%20item)
//SOLUTION : MY EXPECTATIONS WERE CORRECT , had to scan for subitems.

//x todo Set project to top when scanning active document.

//todo Only set to top when clicking the top button
//not everytime we modify a document.
//Could be set in an option ?

//x todo something wrong happening with this very solution
//When scanning solution, CommentsPlus17 seems to be skipped.
//SOLUTION RIGHT BELOW ANSWERED TO THE PROBLEM SOMEHOW

//x todo BUG on certain solutions, thing was crashing all around where project items had no filenames
//SOLUTION only scan files for which extension was known  link:BookmarkScanner.cs://?%20solution%20to%20bug:%20IsSupportedFile