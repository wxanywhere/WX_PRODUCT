#r "PresentationCore.dll"
#r "PresentationFramework.dll"
#r "System.Xaml.dll"
#r "WindowsBase.dll"
#r "System.Configuration.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
open System.Configuration
open System.Collections.Generic
open Microsoft.FSharp.Collections
open System.Windows.Markup
open System.Windows
open System.Windows.Controls

//=================================================================================
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"


let x=new Button()
(box x) :? FrameworkElement    //True

(box x) :? DependencyObject   //True

//----------------------------------------------------------------------------------------------
open System.Reflection
open System.IO
open System.Xml
open System.Windows.Markup
let createWindow (file : string) =
  using (XmlReader.Create(file))
    (fun (xmlRdr : XmlReader) -> 
    (XamlReader.Load(xmlRdr) :?> Window))   
    
let createWindow1(file:string)=
  let assembly=Assembly.GetExecutingAssembly()
  use sr=new StreamReader(assembly.GetManifestResourceStream(file))
  XamlReader.Load(sr.BaseStream):?>Window 
  //System.Windows.Markup.XamlInstanceCreator(
  
let window=createWindow "page.xaml"

//置前
window.Topmost<-true

