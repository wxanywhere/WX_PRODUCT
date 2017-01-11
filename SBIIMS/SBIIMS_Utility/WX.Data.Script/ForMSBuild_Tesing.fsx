
open System
open System.IO
open System.Text.RegularExpressions
//----------------------------------------------
#r "Microsoft.Build.dll"
#r "Microsoft.Build.Engine.dll"
#r "Microsoft.Build.Framework.dll"
#r "Microsoft.Build.Tasks.v4.0.dll"
#r "Microsoft.Build.Utilities.v4.0.dll"
open Microsoft.Build.Framework
open Microsoft.Build.Tasks
open Microsoft.Build.BuildEngine
//open Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.
open Microsoft.Build.Evaluation
//----------------------------------------------
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data
//=======================================

let eg = new Engine()
eg.BuildProjectFile ("D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_DJCX\WX.Data.FViewModel.JXC.JHGL.DJCX\WX.Data.FViewModel.JXC.JHGL.DJCX.fsproj")

let mc=new ProjectCollection()
let proj=mc.LoadProject( @"C:\Users\zhoutao\Documents\visual studio 2010\Projects\WpfApplication7\WpfApplication7\WpfApplication7.csproj")
proj.Build()
proj


let CollectProjectFilesWithRegex (sourceDirectoryPaths:string seq,fileNamePatterns:string seq) =
  let rec GetFileInfo (sourceDirectories:DirectoryInfo seq)=    
    seq{   
        for a in sourceDirectories do
          for b in a.GetFiles() do
            match fileNamePatterns|>Regex.IsMatchIn b.Name with //不能直接比较两个实例sourceDirectory，targetDirectory
            | true -> 
                yield b
            | _ ->()
          yield! GetFileInfo (a.GetDirectories())
    }
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->GetFileInfo x




(*Right ,but deprecated
let mb=new MSBuild()
let be=new Microsoft.Build.BuildEngine.Engine() //deprecated
be.BuildProjectFile ( @"C:\Users\zhoutao\Documents\visual studio 2010\Projects\WpfApplication7\WpfApplication7\WpfApplication7.csproj")
*)


(*
mb.BuildEngine.BuildProjectFile("C:\Users\zhoutao\Documents\visual studio 2010\Projects\WpfApplication7\WpfApplication7\WpfApplication7.csproj",null,null,null)
mb.BuildEngine.BuildProjectFile(
mb.Execute()
*)