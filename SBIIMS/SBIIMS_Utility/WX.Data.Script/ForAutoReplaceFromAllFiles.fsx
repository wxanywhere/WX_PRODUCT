#nowarn "40"

//=========================================================

#r "System.dll"
open System
open System.IO
open System.Text
open System.Text.RegularExpressions
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
open WX.Data.Helper
#r "WX.Data.dll"
open WX.Data
#r "WX.Data.CodeAutomation.dll"
open WX.Data.CodeAutomation

System.AppDomain.CurrentDomain.BaseDirectory

let rootDirectoryInfo=DirectoryInfo @"D:\Common Workspace\Send\SBIIMS"
System.IO.Directory.GetDirectories    @"D:\Common Workspace\Send\SBIIMS"
System.Environment.GetEnvironmentVariable("PATH")
//=========================================================

//替换WX.Data.View的文件内容 2010-08-29
let FileReplaceModify (sourPath:string) (sourceContent:string) (targetContent:string)=
  let sourDirectory=DirectoryInfo sourPath
  let rec FileReplaceModify (sourDirectory:DirectoryInfo,sourceContent:string,targetContent:string)=       
    for b in sourDirectory.GetFiles() do
      match b.Extension.ToLower() with
      | EndsWithIn [".xaml"] _ ->
          match File.ReadFile b.FullName with
          | x ->File.ModifyFile (x.Replace(sourceContent,targetContent)) b.FullName
      | _ ->()
    for a in sourDirectory.GetDirectories() do
        FileReplaceModify(a,sourceContent,targetContent)
  FileReplaceModify(sourDirectory,sourceContent,targetContent) 

let sourcePath= @"D:\Common Workspace\SBIIMS_SL\WX.Data.Silverlight.View.GGXZ\View"
let sourceContent= @"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"""
let targetContent= @"xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
      xmlns:sdk=""http://schemas.microsoft.com/winfx/2006/xaml/presentation/sdk"""
FileReplaceModify sourcePath sourceContent targetContent


//---------------------------------------------------------------------------------------------

//Windwos文件系统的名称修改拷贝
let RenameCopy (sourPath:string) (targetPath:string) (renameStr:string) (renamedStr:string)=
  let sourDirectory=DirectoryInfo sourPath
  let targetDirectory=new DirectoryInfo(targetPath) 
  let rec RenameCopy (sourDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,renameStr:string,renamedStr:string)=       
    for b in sourDirectory.GetFiles() do
      match b.IsReadOnly with 
      | x ->
          b.IsReadOnly<-false //It's needed
          b.CopyTo(Path.Combine(targetDirectory.FullName,b.Name.Replace(renameStr,renamedStr))) |>ignore
          b.IsReadOnly<-x
    for a in sourDirectory.GetDirectories() do
      match targetDirectory.CreateSubdirectory(a.Name.Replace(renameStr,renamedStr))  with
      | x ->  RenameCopy(a,x,renameStr,renamedStr)
  RenameCopy(sourDirectory,targetDirectory,renameStr,renamedStr) 
        
let sourPath= @"D:\Common Workspace\For Research\WX.Data.BusinessModuleTemplate"
let targetPath= @"D:\Common Workspace\For Research\WX.Data.BusinessModuleTemplateNiew"
RenameCopy sourPath targetPath  "System" "Link"  

//---------------------------------------------------------------------------------------------
//删除Bin及Obj目录
let  DeleteBinObj (directoryPath:string)=
  let rootDirectoryInfo=DirectoryInfo directoryPath
  let rec DeleteDirectories (directoryInfo:DirectoryInfo)=
    for a in directoryInfo.GetDirectories() do
      match a with
      | x when x.Name.Equals("WX.Data.CodeAutomation") || x.Name.Equals("WX.Data.Helper")->()
      | x when x.Name.Equals("bin") || x.Name.Equals("obj") ->x.Delete(true)
      | x -> DeleteDirectories x
  DeleteDirectories rootDirectoryInfo


//let path= @"D:\Common Workspace\Send\SBIIMS"
//let path= @"D:\Common Workspace\Send\SBIIMSClient"
let path= @"D:\Common Workspace\Send\2010-02-28"
DeleteBinObj path   

//---------------------------------------------------------------------------------------------
//为文件增加代码
let  AddForCodeFile (directoryPath:string) (newContent:string)=
  let rootDirectoryInfo=DirectoryInfo directoryPath
  //ObjectDumper.Write rootDirectoryInfo
  let rec foreachDirectories (directoryInfo:DirectoryInfo)=
    for b in directoryInfo.GetFiles() do
      match b with
      | x when x.Extension.ToLower()=".fs" ->
          let  codeText=ref <| FileHelper.ReadFile x.FullName
          codeText:=Regex.Replace(!codeText,@"[\W\w\s\n]*namespace[\w\s\.\/\n\[\<\>\]\\]*type\s",newContent.Trim()+" ",RegexOptions.Multiline)   //[<DataContract>]
          FileHelper.WriteFile !codeText x.FullName
      | x ->()
    for a in directoryInfo.GetDirectories() do
      foreachDirectories a
  foreachDirectories rootDirectoryInfo

//let projectPath= @"D:\Workspace\SBIIMS\WX.Data.FViewModelData\FVM"
let projectPath= @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\BDAdvance"

let codeContent=
  @"namespace WX.Data.IDataAccess
open System.Collections.Generic
open WX.Data.BusinessEntities
type IDA_GHS_Advance=
  inherit IDA_GHS
  abstract GetGHSView:BQ_GHS_Advance->List<BD_T_GHS_Advance>"

AddForCodeFile   projectPath  codeContent


//---------------------------------------------------------------------------------------------
//所有类文件替换指定的多行内容
let  ReplaceCodeFile (directoryPath:string) (newContent:string)=
  let rootDirectoryInfo=DirectoryInfo directoryPath
  //ObjectDumper.Write rootDirectoryInfo
  let rec foreachDirectories (directoryInfo:DirectoryInfo)=
    for b in directoryInfo.GetFiles() do
      match b with
      | x when x.Extension.ToLower()=".fs" ->
          let  codeText=ref <| FileHelper.ReadFile x.FullName
          codeText:=Regex.Replace(!codeText,@"[\W\w\s\n]*namespace[\w\s\.\/\n\[\<\>\]\\]*type\s",newContent.Trim()+" ",RegexOptions.Multiline)   //[<DataContract>]
          FileHelper.WriteFile !codeText x.FullName
      | x ->()
    for a in directoryInfo.GetDirectories() do
      foreachDirectories a
  foreachDirectories rootDirectoryInfo


let projectPath= @"D:\Workspace\SBIIMS\WX.Data.FViewModelData\FVM"
let newContent=
  @"namespace WX.Data.ViewModelData
open System
open System.ComponentModel
open System.Collections.ObjectModel
open System.Collections
open System.Windows
open System.Windows.Input
open Microsoft.FSharp.Collections
open WX.Data
open WX.Data.ActiveModule
open WX.Data.TypeAlias
open WX.Data.ClientHelper
open WX.Data.BusinessEntities
open WX.Data.ServiceProxy.WS_SBIIMS

type"

ReplaceCodeFile projectPath   newContent

//---------------------------------------------------------------------------------------------

let projectPath= @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\BDAdvance"
let newContent=
  @"namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open WX.Data.TypeAlias
[<DataContract>]
type"

ReplaceCodeFile projectPath   newContent
//=========================================================
//复制所有类后，自动产出高级类模板
let  ModiyCodeFile (directoryPath:string)=
  //ObjectDumper.Write directoryPath
  let rootDirectoryInfo=DirectoryInfo directoryPath
  let newCodePartOne= @"namespace WX.Data.ViewModelData
open System.Collections.Generic
open System.Windows
open System.Windows.Input
open Microsoft.FSharp.Collections
open WX.Data
open WX.Data.TypeAlias
open WX.Data.ClientHelper
open WX.Data.ViewModel
open WX.Data.BusinessEntities
open WX.Data.ServiceProxy.WS_SBIIMS

type"
  let newCodePartTwo= @"inherit WX.Data.ViewModel.{0}()"
  let rec foreachDirectories (directoryInfo:DirectoryInfo)=
    for b in directoryInfo.GetFiles() do
      //ObjectDumper.Write b.Extension
      match b with
      | x when x.Extension.ToLower()=".fs" ->
          let  codeText=ref <| FileHelper.ReadFile x.FullName
          //ObjectDumper.Write x
          //ObjectDumper.Write !codeText
          codeText:=Regex.Replace(!codeText,@"[\W\w\s\n]*namespace[\w\s\.\/\n\[\<\>\]\\]*type\s",newCodePartOne.Trim()+" ",RegexOptions.Multiline)
          //ObjectDumper.Write !codeText
          match Regex.Matches(!codeText, "(?<=[\w\s\.\n]*type\s+)\w+((?=\s*\()|(?=\s*\=))",RegexOptions.Multiline)  with
          | y when y.Count>0 && y.[0].Groups.Count>0 ->
              let indent="  "
              String.Format(newCodePartTwo,y.[0].Groups.[0].Value)
              |>fun c ->codeText:=Regex.Replace(!codeText,@"^.*inherit\s[\w\W\s\n]*",indent+c,RegexOptions.Multiline)
              FileHelper.WriteFile !codeText x.FullName
          | _ ->()
      | x ->()
    for a in directoryInfo.GetDirectories() do
      foreachDirectories a
  foreachDirectories rootDirectoryInfo

let projectPath= @"D:\Workspace\SBIIMSClient\WX.Data.FViewModelData\FVM"
ModiyCodeFile projectPath


let testText= @"
//#I @""C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0""
//open Systemopen System.Windowsopen System.Windows.Inputnamespace WX.Data.ViewModelData
open System.Collections.Generic
open System.Windows.Input
open Microsoft.FSharp.Collections
open WX.Data
open WX.Data.TypeAlias
open WX.Data.ClientHelper
open WX.Data.ViewModel
open WX.Data.BusinessEntities
open WX.Data.ServiceProxy.WS_SBIIMS

type FVM_KC_SPCB () = 
  inherit WorkspaceViewModel ()
  let getnull()=Unchecked.defaultof<_>
  let mutable _isUserControlEnable=true"
//let x=Regex.Matches(testText, "(?<=[.\n]*type\s+)\w+(?=\s*\()",RegexOptions.Multiline)
let x=Regex.Matches(testText, @"namespace[\w\s\n\.]*type\s",RegexOptions.None)
let x=Regex.Matches(testText, @"namespace[\w\s\n\.]*type\s",RegexOptions.None)
x.[0].Groups.[0].Value
//=========================================================
 
let y=Regex.Replace(testText,@"[\W\w\s\n]*namespace[\w\s\.\/]*type\s"," ",RegexOptions.Multiline)




//Current
System.AppDomain.CurrentDomain.BaseDirectory



let directoryPath= @"D:\Workspace\SBIIMS\WX.Data.View.ViewModelTemplate\bin\Debug"
let assemblyDirectory=new DirectoryInfo(directoryPath)
assemblyDirectory.FullName
let file=assemblyDirectory.GetFiles()
file.[0].FullName
file.[0].Name
file.[0].Name.Remove(file.[0].Name.Length-file.[0].Extension.Length)

AppDomain.CurrentDomain.GetAssemblies().[0].FullName
AppDomain.CurrentDomain.GetAssemblies().[0].GetName().Name
