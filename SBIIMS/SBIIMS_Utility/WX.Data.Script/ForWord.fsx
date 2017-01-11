// Copyright (c) Microsoft Corporation 2005-2008. 
// This sample code is provided "as is" without warranty of any kind.  
// We disclaim all warranties, either express or implied, including the  
// warranties of merchantability and fitness for a particular purpose.  
 
#I @"C:\WINDOWS\assembly\GAC\"
#r "Microsoft.Office.Interop.Word"  //注意直接引用GAC组件时，不加*.dll后缀
#r "Office" 

open System 
open System.Diagnostics 
open System.Runtime.InteropServices 
open System.Reflection // For Missing.Value  
open System.Collections 
open System.Threading 
// In Office 2003 and XP the Excel.dll defines a namespace 'Excel' 
// In Office 2007 it defines a namespace 'Microsoft.Office.Interop.Excel' 
// This hack means that we don't get an error when opening Microsoft.Office.Interop  
// if it's not present 
open Microsoft.Office.Interop 
 
 
// Automation APIs such as the Word COM object model are semi-typed, i.e.  
// many values are passed using the .NET type "object".  
// So we often use the F# "box" operation to produce a value of type  
// "object" from an arbitrary F# value.  "box" has type "'a -> obj".  
 
// Furthermore, many arguments are passed to the Word Automation API  
// "by reference", e.g. as the C# type &object.  In F# by-reference argument  
// "&T" appears as the F# type "T ref".  Thus many arguments are wrapped,  
// e.g. "ref x" or "(refArg x)".  
 
// Create a Word Appilcation and document.  
let app = new Word.ApplicationClass() 
let doc = new Word.DocumentClass() 
 
// Add some Autocorrect entries.  
let autocorrect = app.AutoCorrect 
let autoEntries = autocorrect.Entries  
 
autoEntries.Add("Inntroduction", "Introduction") |> ignore 
 
// Count the number of open documents the Word application.  
let docs = app.Documents 
Console.WriteLine (string docs.Count + " docs") 
 
// Show the application.  
app.Visible <- true 
 
// The COM Word object model requires empty values many places.  
// These are given the special value System.Reflection.Missing.Value. This  
// value is always passed by reference this file, i.e. where values of  
// type "&object" are expected, so we make a byref location for it here.  
// Note that we assume that the COM API never writes into this byref  
// value.  
let refArg x = ref (box x) 
let none = refArg Missing.Value 
 
// Add a document.  
let doc2=docs.Add(none, none, none, none) 
 
// Add text to the doc.  This contains some deliberate misspellings so  
// that we can correct them below.  
  
let range = doc2.Range(none,none)  
range.Text <- "Microsoft Word Interoperability Sample\n\nInntroduction:\n\nMicrosoft .NET will alow the creation of truly distributed XML Web services. These services will integrate and collaborate with a range of complementary services to work for customers ways that today's internet companies can only dream of. Microsoft .NET will drive the Next Generation Internet and will shift the focus from individual Web sites or devices connected to the Internet, to constellations of computers, devices, and services that work together to deliver broader, richer solutions.\nFor more info go to:\n   " 
 
// Wait so the starting state can be admired; 
Thread.Sleep(2000) 
 
// Format the title: for some reason this raises an exception on my  
// computer, both for the C# sample and the code below.  It might work  
// for you, so I've included the code below.  
 
try  
    Console.WriteLine("Formatting the title"); 
    let range=doc2.Range(refArg 0, refArg 40) 
    let fc= new Word.FontClass(Size = 24.0f, Bold = 1, Color = Word.WdColor.wdColorGray30) 
    range.Font <- fc; 
    let range=doc2.Range(refArg 40, refArg 54)  
    let fc= new Word.FontClass(Size = 14.0f);  
    range.Font <- fc; 
    // Wait so the new formatting can be appreciated  
    Thread.Sleep(3000) 
with e -> Console.WriteLine("font error: {0}", e.ToString()) 
 
// Fix spelling mistakes  
autocorrect.ReplaceTextFromSpellingChecker <- true; 
 
// Fix inntroduction 
let errEntry= autoEntries.Item(refArg "Inntroduction") 
let myWords=doc2.Words  
let errRange= myWords.Item(7)  
errEntry.Apply(errRange) 
 
// Add a caption to the window and get it back  
let myWindow = app.ActiveWindow  
myWindow.Caption <- "Managed Word execution from C# "; 
let  gotCaption = myWindow.Caption  
if (gotCaption.Equals("Managed Word execution from C# ")) then 
    Console.WriteLine("Caption assigned and got back"); 
Thread.Sleep(2000); 
 
// List the spelling errors  
let pr1 = doc2.SpellingErrors 
Console.WriteLine("Spelling errors count: {0}", pr1.Count); 
for i=1 to pr1.Count do 
    let rg = pr1.Item(i)  
    Console.WriteLine(rg.Text); 
    Thread.Sleep(2000); 
done 
 
// Define the selection object, find and replace text  
let mySelection = myWindow.Selection  
let myFind = mySelection.Find  
Console.WriteLine(myFind.Text) 
 
//  Find "alow" and replace with "allow"  
try 
    ignore (myFind.Execute(FindText = refArg "alow", 
                           MatchCase= none, 
                           MatchWholeWord=none, 
                           MatchWildcards=none, 
                           MatchSoundsLike=none, 
                           MatchAllWordForms=none, 
                           Forward=none, 
                           Wrap=none, 
                           Format=none, 
                           ReplaceWith=refArg "allow", 
                           Replace=none, 
                           MatchKashida=none, 
                           MatchDiacritics=none, 
                           MatchAlefHamza=none, 
                           MatchControl=none)); 
    Thread.Sleep(2000); 
with e -> Console.WriteLine("find error: {0}", e.ToString()) 
 
// Change the formatting of ".NET"  
try 
    let range=doc2.Range(refArg 65, refArg 69)  
    Console.WriteLine("The color of .NET is being changed"); 
    let fc= new Word.FontClass() 
    fc.Bold <- 16; 
    fc.Color<- Word.WdColor.wdColorLavender;  
    range.Font<- fc; 
    Thread.Sleep(2000); 
with e -> Console.WriteLine("font error: {0}", e.ToString()) 
 
// Underline the selected text  
let range2=doc2.Range(refArg 65,refArg 69)  
range2.Underline <- Word.WdUnderline.wdUnderlineDouble 
 
// Add hyperlink and follow the hyperlink 
 
let my_Hyperlinks = doc2.Hyperlinks  
// Make the range past the end of all document text  
app.ActiveWindow.Selection.Start <- 9999; 
app.ActiveWindow.Selection.End  <- 9999; 
 
// Add a hyperlink  
let myAddress = "http://research.microsoft.com/fsharp"  
Console.WriteLine("Adding hyperlink to the document"); 
 
let my_Hyperlink1= my_Hyperlinks._Add(mySelection.Range, refArg myAddress, none) 
app.ActiveWindow.Selection.InsertAfter("\n"); 
Thread.Sleep(5000); 
 
// Open a window to Hyperlink  
let ie = Process.Start("iexplore.exe", my_Hyperlink1.Address)  
 
// Wait for a short spell to allow the page to be examined  
Thread.Sleep(10000); 
 
// Display "The End"  
 
app.ActiveWindow.Selection.InsertAfter("\nThe End"); 
app.Activate(); 
Thread.Sleep(5000) 
 
// Close Microsoft Word  
 
app.ActiveWindow.Close(refArg Word.WdSaveOptions.wdDoNotSaveChanges,none); 
app.Quit(none, none, none) 
