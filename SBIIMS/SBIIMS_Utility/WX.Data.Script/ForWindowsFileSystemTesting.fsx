

#r "System.dll"
//#r "FSharp.PowerPack.dll"
open System
open System.IO
open System.Text
open Microsoft.FSharp.Control
open System.Text.RegularExpressions
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
open WX.Data.Helper
#r "WX.Data.dll"
open WX.Data
open System.Text.RegularExpressions

let RetrieveProjectStrcture (sourceDirectoryPath:string,targetDirectoryPath:string,directoryOldNewNameGroups:(string*string) list,fileOldNewNameGroups:(string*string) list,excludeDirectories:string list,includeFileExtensions:string list) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,directoryOldNewNameGroups:(string*string) list,fileOldNewNameGroups:(string*string) list,excludeDirectories:string list,includeFileExtensions:string list)=       
    for b in sourceDirectory.GetFiles()|>Seq.filter(fun c->includeFileExtensions|>Seq.exists(fun d->d.ToLower().Equals(c.Extension)))|>Seq.toArray do
      match File.ReadFile b.FullName with
      | x ->
          match 
            fileOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) b.Name with
          | y ->
              Path.Combine(targetDirectory.FullName,y)
              |>fun z->File.Copy(b.FullName,z)   
    for a in sourceDirectory.GetDirectories()|>Seq.filter(fun a->excludeDirectories|>Seq.exists(fun b->b.ToLower().Equals(a.Name))) do
      match directoryOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) a.Name with
      | x ->
          match targetDirectory.CreateSubdirectory(x)  with
          | x ->  ModifyCopy(a,x,directoryOldNewNameGroups,fileOldNewNameGroups,excludeDirectories,includeFileExtensions)
  ModifyCopy(sourceDirectory,targetDirectory,directoryOldNewNameGroups,fileOldNewNameGroups,excludeDirectories,includeFileExtensions) 

(
@"D:\Workspace\SBIIMS",  //sourceDirectory
@"D:\Workspace\SBIIMS_ProjectStructure",  //targetDirectory
[ //directoryOldNewNameGroups

], 
[ //fileOldNewNameGroups
],
[  //includeFileExtensions
".csproj"   
".fsproj" 
".sln"
]
)
|>RetrieveProjectStrcture


let ModifyFile (modifiedText:string) (filePath:string)=
   try
     match File.Exists filePath with
     | false->failwith "The file is not exsit!"
     | _ ->()
     use fs=File.OpenWrite(filePath)  //use fs=new FileStream(filePath,FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)
     Console.WriteLine(fs.Length)
     fs.SetLength(0L) //清空
     fs.Seek(0L, SeekOrigin.Begin)|>ignore
     use sw=new StreamWriter(fs,Encoding.UTF8)
     sw.Write modifiedText
     sw.Flush()
   with
   | _ ->reraise()

let ModifyFileX (modifiedText:string) (filePath:string)=
   try
     match File.Exists filePath with
     | false->failwith "The file is not exsit!"
     | _ ->()
     using (File.OpenWrite(filePath)) (fun fs->  //use fs=new FileStream(filePath,FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)
       Console.WriteLine(fs.Length)
       fs.SetLength(0L) //清空
       fs.Seek(0L, SeekOrigin.Begin)|>ignore
       using (new StreamWriter(fs,Encoding.UTF8)) (fun sw->
         sw.Write modifiedText
         sw.Flush()
         )
       )
   with
   | _ ->reraise()

ModifyFileX @"xxx" @"G:\temp\WX.Data.FViewModelAdvance.AC.CZYJSGL\FVM_CZYJSGL_Modify_Advance.fs"