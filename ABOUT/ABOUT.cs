//about in shared

//x todo BUG Things going in wrong places
// Dedoubled links with shared projects
//! solution might be to check first sharedProjects somehow, then check if files come from the shared things (hopefully there won't be shared projects between shared projects?)
//x now scanning first shared projects
//x todo don't scan files that are already added to the tasklist.


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

//todo fix: Empty projects appear in the list.


//todo make scrolling on items of the tasklist scroll the parent listview
//making the scrollbars not appear didn't fix it.
//actually it might have half fixed it. Scrolling on projects work, but it seems scrolling on child of chilf doesn't work on grand parent.

//done todo add link copy on alt+click

//done TODO somehow stopped to read files that aren't currently open in scanSolution or scanproject!
//ProjectItem.Document is null if the file is not open in visual studio. Was checking if document was null in the code. Replaced it.
