
#r "System.dll"
//#r "FSharp.PowerPack.dll"
open System
open System.IO
open System.Text
open Microsoft.FSharp.Control
open System.Text.RegularExpressions
#I  @"K:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
open WX.Data.Helper
#r "WX.Data.dll"
open WX.Data

//=================================================
//获取当前目录下的所有文件  Key, directoryInfo.DirectoryName, directoryInfo.Parent
//rootDirectoryInfo<-DirectoryInfo (rootDirectoryInfo.Parent.FullName+ @"\"+rootDirectoryInfo.Name)
//rootDirectoryInfo.Parent.FullName 无"\"后缀
//rootDirectoryInfo.GetDirectories().[0].FullName 无"\"后缀
let GetFileInfos =
  let mutable directoryPath= @"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.ZHYW"
  match directoryPath with
  | EndsWithIn [@"\"; "/"]  x->directoryPath<-x.Remove(x.Length-1)
  | _ ->()
  let  rootDirectoryInfo=DirectoryInfo directoryPath
  let rec collectFileInfos (template:string) (directoryInfo:DirectoryInfo)=
    seq{
      for b in directoryInfo.GetFiles() do
        if b.Extension=".fs" then
          match rootDirectoryInfo,directoryInfo with
          | x,y when x.FullName<>y.FullName ->
              yield String.Format(template, y.FullName.Remove(0,x.FullName.Length+1) ,b.Name ) 
          | _ ->
              yield String.Format(template, String.Empty,b.Name ) 
      for a in directoryInfo.GetDirectories() do
        yield! collectFileInfos template a
      }
  rootDirectoryInfo
  |>collectFileInfos @"    <Compile Include=""{1}"" />"     //@"    <Compile Include=""{0}\{1}"" />"
  |>ObjectDumper.Write 

//---------------------------------------------------------------------------------------------
//Windwos文件系统的名称修改拷贝-*********包括目录名称和文件名称的修改
let RenameCopy (sourceDirectoryPath:string) (targetPath:string) (renameStr:string) (renamedStr:string)=
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory=new DirectoryInfo(targetPath) 
  let rec RenameCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,renameStr:string,renamedStr:string)=       
    for b in sourceDirectory.GetFiles() do
      match b.IsReadOnly with 
      | x ->
          b.IsReadOnly<-false //It's needed
          b.CopyTo(Path.Combine(targetDirectory.FullName,b.Name.Replace(renameStr,renamedStr)),true) |>ignore
          b.IsReadOnly<-x
    for a in sourceDirectory.GetDirectories() do
      match targetDirectory.CreateSubdirectory(a.Name.Replace(renameStr,renamedStr))  with
      | x ->  RenameCopy(a,x,renameStr,renamedStr)
  RenameCopy(sourceDirectory,targetDirectory,renameStr,renamedStr) 
        
let sourceDirectoryPath= @"D:\Workspace\SBIIMS"
let targetPath= @"D:\Workspace\SBIIMS10100902"
RenameCopy sourceDirectoryPath targetPath  "Advance.JXC" "Advance.JXC"  

//---------------------------------------------------------------------------------------------
//*********************************************************************************************
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换
let ModifyCopy (sourceDirectoryPath:string,targetDirectoryPath:string,directoryOldNewNameGroups:(string*string) list,fileOldNewGroups:(string*string) list) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,directoryOldNewNameGroups:(string*string) list,fileOldNewGroups:(string*string) list)=       
    for b in sourceDirectory.GetFiles() do
      match File.ReadFile b.FullName with
      | x ->
          match 
            fileOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) b.Name,
            fileOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) x with
          | y,z ->
              Path.Combine(targetDirectory.FullName,y)
              |>File.WriteFileCreateOnly z     
    for a in sourceDirectory.GetDirectories() do
      match directoryOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) a.Name with
      | x ->
          match targetDirectory.CreateSubdirectory(x)  with
          | x ->  ModifyCopy(a,x,directoryOldNewNameGroups,fileOldNewGroups)
  ModifyCopy(sourceDirectory,targetDirectory,directoryOldNewNameGroups,fileOldNewGroups) 

//*********************************************************************************************
//Windwos文件系统 --拷贝并修改文件名及内容(需要排除的文件仅复制，不读取及写入), ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换,
//let file=FileInfo "D:\Workspace\YDTF\Resources\MyTask\RDT\M00_系统支撑_V03\系统支撑\M00\App.xaml"
//file.CopyTo "D:\Workspace\YDTF\Resources\MyTask\RDT\M00_系统支撑_V03\系统支撑\M00\App0wx.xaml"

let ModifyCopyExtraX (sourceDirectoryPath:string,targetDirectoryPath:string,fileContentReplaceExtensions:string seq,directoryOldNewNameGroups:(string*string) list,fileOldNewNameGroups:(string*string) list,fileOldNewContentGroups:(string*string) list) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,fileContentReplaceExtensions:string seq,directoryOldNewNameGroups:(string*string) list,fileOldNewNameGroups:(string*string) list,fileOldNewContentGroups:(string*string) list)=       
    for b in sourceDirectory.GetFiles() do
      match fileOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) b.Name with
      | x ->
          let fileNewPath=Path.Combine(targetDirectory.FullName,x)
          match b.Extension.ToLower() with
          | EqualsIn fileContentReplaceExtensions _->
              match File.ReadFile b.FullName with
              | y ->
                  match fileOldNewContentGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) y with
                  | z ->File.WriteFileCreateOnly z fileNewPath   
          | _ ->b.CopyTo fileNewPath|>ignore
    for a in sourceDirectory.GetDirectories() do
      match directoryOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) a.Name with
      | x ->
          match targetDirectory.CreateSubdirectory(x)  with
          | x ->  ModifyCopy(a,x,fileContentReplaceExtensions,directoryOldNewNameGroups,fileOldNewNameGroups,fileOldNewContentGroups)
  ModifyCopy(sourceDirectory,targetDirectory,fileContentReplaceExtensions,directoryOldNewNameGroups,fileOldNewNameGroups,fileOldNewContentGroups) 

//其中".flow"为SketchFlow的流程文件扩展名
let fileContentReplaceExtensions=[".fs";".fsx";".fsi";".cs";".c";".h";".vb";".java";".class";".txt";".xml";".xaml";".csproj";".fsproj";".sln";".flow"]
//--------------------------------------------------------------------------------------

(
@"K:\Workspace\XN\Git\XN\xnrpt\BusinessModule",  //sourceDirectory
@"K:\Workspace\XN\Git\XN\xnrpt\BusinessModule2",  //targetDirectory
fileContentReplaceExtensions,
[ //directoryOldNewNameGroups
"XN.RPT","XN.RPT.Business"
], 
[ //fileOldNewNameGroups
"XN.RPT","XN.RPT.Business"
],
[  //fileOldNewContentGroups
"XN.RPT","XN.RPT.Business"
]
)
|>ModifyCopyExtraX

(
@"J:\Workspace\YDTF\Dev\Archive\WorkflowClient",  //sourceDirectory
@"J:\Workspace\YDTF\Dev\Archive\WorkflowClient1",  //targetDirectory
fileContentReplaceExtensions,
[ //directoryOldNewNameGroups
"YDTF.WFA.","WX.WFA."   
], 
[ //fileOldNewNameGroups
"YDTF.WFA.","WX.WFA."  
],
[  //fileOldNewContentGroups
"YDTF.WFA.","WX.WFA."  
]
)
|>ModifyCopyExtraX


//*********************************************************************************************
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换
let ModifyCopyExtra (sourceDirectoryPath:string,targetDirectoryPath:string,directoryOldNewNameGroups:(string*string) list,fileOldNewNameGroups:(string*string) list,fileOldNewContentGroups:(string*string) list) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,directoryOldNewNameGroups:(string*string) list,fileOldNewNameGroups:(string*string) list,fileOldNewContentGroups:(string*string) list)=       
    for b in sourceDirectory.GetFiles() do
      match File.ReadFile b.FullName with
      | x ->
          match 
            fileOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) b.Name,
            fileOldNewContentGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) x with
          | y,z ->
              Path.Combine(targetDirectory.FullName,y)
              |>File.WriteFileCreateOnly z     
    for a in sourceDirectory.GetDirectories() do
      match directoryOldNewNameGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) a.Name with
      | x ->
          match targetDirectory.CreateSubdirectory(x)  with
          | x ->  ModifyCopy(a,x,directoryOldNewNameGroups,fileOldNewNameGroups,fileOldNewContentGroups)
  ModifyCopy(sourceDirectory,targetDirectory,directoryOldNewNameGroups,fileOldNewNameGroups,fileOldNewContentGroups) 

//*********************************************************************************************

(
@"D:\Workspace\SBIIMS",  //sourceDirectory
@"D:\Workspace\SBIIMS_ProjectStructure",
[ //directoryOldNewNameGroups 
], 
[ //fileOldNewNameGroups 
],
[  //fileOldNewContentGroups
"HasQueryResult","HasResult"   
]
)
|>ModifyCopyExtra


(
@"G:\temp\WX.Data.FViewModelAdvance.AC.CZYJSGL",  //sourceDirectory
@"G:\temp\WX.Data.FViewModelAdvance.AC.CZYJSGL1",
[ //directoryOldNewNameGroups 
], 
[ //fileOldNewNameGroups 
],
[  //fileOldNewContentGroups
"HasQueryResult","HasResult"   
]
)
|>ModifyCopyExtra

(
@"D:\Workspace\YDTF\Resources\MyTask\RPT\M00_系统支撑_V01\系统支撑",  //sourceDirectory
@"D:\Workspace\YDTF\Resources\MyTask\RPT\M00_系统支撑_V01\系统支撑1",  //targetDirectory
[ //directoryOldNewNameGroups
"M00_01","M00"   
], 
[ //fileOldNewNameGroups
"M00_01","M00"   
],
[  //fileOldNewContentGroups
"M00_01","M00"   
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c11",  //targetDirectory
[ //directoryOldNewNameGroups
".FK",".JXC"   
"_FK","_JXC" 
], 
[ //fileOldNewNameGroups
".FK",".JXC"   
"_FK","_JXC" 
],
[  //fileOldNewContentGroups
".FK",".JXC"   
"_FK","_JXC" 
]
)
|>ModifyCopyExtra

let entryDirectory=DirectoryInfo @"D:\Workspace\SBIIMS\SBIIMS_JXC"
seq{
  for n in entryDirectory.GetDirectories() do
    match n.Name.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries) with
    | [|a;b|] when a.ToUpper()=a && b.ToUpper()=b ->
        yield String.Format (".{0}.{1}",a,b), String.Format ("_{0}_{1}",a,b)
    | [|a;b;c|] when a.ToUpper()=a && b.ToUpper()=b && c.ToUpper()=c ->
        yield String.Format (".{0}.{1}.{2}",a,b,c), String.Format ("_{0}_{1}_{2}",a,b,c)
    | _ ->()
}
|>Seq.toArray
|>Seq.iter (fun (a,b) ->    
    (
    @"D:\TempWorkspace\c",  //sourceDirectory
    Path.Combine(@"D:\TempWorkspace\c1",b.Remove(0,1)),  //@"D:\TempWorkspace\c1",  //targetDirectory
    [ //directoryOldNewNameGroups
    //".FK",a   //.JXC.JBXX.CKWH
    //"_FK",b //SBIIMS_JXC_JBXX_CKWH
    ".JXC.FXYC.CGFX",a   //.JXC.JBXX.CKWH
    "_JXC_FXYC_CGFX",b //_JXC_JBXX_CKWH
    ], 
    [ //fileOldNewNameGroups
    ".JXC.FXYC.CGFX",a   //.JXC.JBXX.CKWH
    "_JXC_FXYC_CGFX",b //_JXC_JBXX_CKWH
    ],
    [  //fileOldNewContentGroups
    ".JXC.FXYC.CGFX",a   //.JXC.JBXX.CKWH
    "_JXC_FXYC_CGFX",b //_JXC_JBXX_CKWH
    ]
    )
    |>ModifyCopyExtra
  )


(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"_BBGL_BBWH","_DAAPGL_AUPZ"    
"_BBGL","_DAAPGL_AUPZ"   
"_BBLX","_AU"  
"_BBWJ","_AUDX"
"版本管理-版本维护","数据访问自动编程管理-自动单元配置"
"版本管理","数据访问自动编程管理-自动单元配置"
"版本类型","自动单元"
"版本文件","自动单元对象"
], 
[ //fileOldNewNameGroups
"_BBGL_BBWH","_DAAPGL_AUPZ"    
"_BBGL","_DAAPGL_AUPZ"   
"_BBLX","_AU"  
"_BBWJ","_AUDX"
"版本管理-版本维护","数据访问自动编程管理-自动单元配置"
"版本管理","数据访问自动编程管理-自动单元配置"
"版本类型","自动单元"
"版本文件","自动单元对象"
],
[  //fileOldNewContentGroups
"_BBGL_BBWH","_DAAPGL_AUPZ"    
"_BBGL","_DAAPGL_AUPZ"   
"_BBLX","_AU"  
"_BBWJ","_AUDX"
"版本管理-版本维护","数据访问自动编程管理-自动单元配置"
"版本管理","数据访问自动编程管理-自动单元配置"
"版本类型","自动单元"
"版本文件","自动单元对象"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"VC.GXFB","APC.DAAPGL"    
"VC_GXFB","APC_DAAPGL"    
"GXFB","DAAPGL"   
"XTGX","APGX"
"_FB","_FBGX"
"GXWJ","GXDX"
"GXBBWJ","GXAUDX"
"_BBWJ","_AUDX"
"版本文件","自动单元对象"
"_BBDY","_APDY"
"版本定义","自动编程定义"
"XTWJ","DBDX"
"系统文件","数据库对象"
"BBLX","DB"
"DYLX","AU"
"版本控制","自动编程控制"
"更新发布","数据访问自动编程管理"
"系统更新","自动编程更新"
"发布","发布更新"
"更新文件","更新对象"
"更新版本文件","更新自动单元对象AU-Automated-Unit"
"版本类型","数据库"
"定义类型","自动单元"

], 
[ //fileOldNewNameGroups
"VC.GXFB","APC.DAAPGL"    
"VC_GXFB","APC_DAAPGL"    
"GXFB","DAAPGL"   
"XTGX","APGX"
"_FB","_FBGX"
"GXWJ","GXDX"
"GXBBWJ","GXAUDX"
"_BBWJ","_AUDX"
"版本文件","自动单元对象"
"_BBDY","_APDY"
"版本定义","自动编程定义"
"XTWJ","DBDX"
"系统文件","数据库对象"
"BBLX","DB"
"DYLX","AU"
"版本控制","自动编程控制"
"更新发布","数据访问自动编程管理"
"系统更新","自动编程更新"
"发布","发布更新"
"更新文件","更新对象"
"更新版本文件","更新自动单元对象AU-Automated-Unit"
"版本类型","数据库"
"定义类型","自动单元"
],
[  //fileOldNewContentGroups
"VC.GXFB","APC.DAAPGL"    
"VC_GXFB","APC_DAAPGL"    
"GXFB","DAAPGL"   
"XTGX","APGX"
"_FB","_FBGX"
"GXWJ","GXDX"
"GXBBWJ","GXAUDX"
"_BBWJ","_AUDX"
"版本文件","自动单元对象"
"_BBDY","_APDY"
"版本定义","自动编程定义"
"XTWJ","DBDX"
"系统文件","数据库对象"
"BBLX","DB"
"DYLX","AU"
"版本控制","自动编程控制"
"更新发布","数据访问自动编程管理"
"系统更新","自动编程更新"
"发布","发布更新"
"更新文件","更新对象"
"更新版本文件","更新自动单元对象AU-Automated-Unit"
"版本类型","数据库"
"定义类型","自动单元"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"_BBXX","_SJJK"    
".BBXX",".SJJK"  
"版本信息","数据接口"
], 
[ //fileOldNewNameGroups
"_BBXX","_SJJK"    
".BBXX",".SJJK"  
"版本信息","数据接口"
],
[  //fileOldNewContentGroups
"_BBXX","_SJJK"    
".BBXX",".SJJK"  
"版本信息","数据接口"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"FKJL_CWJL","FKGL_CWFK"
"FKJL.CWJL","FKGL.CWFK"   
"FKJL_JYJL","FKGL_JYFK"
"FKJL.JYJL","FKGL.JYFK"   
".FKJL",".FKGL"
"_FKJL","_FKGL"
"_CWJL","_CWFK"
"_JYJL","_JYFK"
"CWJL","CWFK"
"JYJL","JYFK"
"综合管理-反馈交流","反馈管理"
"反馈交流","反馈管理"
"错误交流","错误反馈"
"建议交流","建议反馈"
], 
[ //fileOldNewNameGroups
"FKJL_CWJL","FKGL_CWFK"
"FKJL.CWJL","FKGL.CWFK"   
"FKJL_JYJL","FKGL_JYFK"
"FKJL.JYJL","FKGL.JYFK"   
".FKJL",".FKGL"
"_FKJL","_FKGL"
"_CWJL","_CWFK"
"_JYJL","_JYFK"
"CWJL","CWFK"
"JYJL","JYFK"
"综合管理-反馈交流","反馈管理"
"反馈交流","反馈管理"
"错误交流","错误反馈"
"建议交流","建议反馈"
],
[  //fileOldNewContentGroups
"FKJL_CWJL","FKGL_CWFK"
"FKJL.CWJL","FKGL.CWFK"   
"FKJL_JYJL","FKGL_JYFK"
"FKJL.JYJL","FKGL.JYFK"   
".FKJL",".FKGL"
"_FKJL","_FKGL"
"_CWJL","_CWFK"
"_JYJL","_JYFK"
"CWJL","CWFK"
"JYJL","JYFK"
"综合管理-反馈交流","反馈管理"
"反馈交流","反馈管理"
"错误交流","错误反馈"
"建议交流","建议反馈"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"JBXX_JYFWH","FKJL_JYJL"   
"JBXX.JYFWH","FKJL.JYJL"  
"交易方维护","建议交流"  
], 
[ //fileOldNewNameGroups
"JBXX_JYFWH","FKJL_JYJL"   
"JBXX.JYFWH","FKJL.JYJL"  
"交易方维护","建议交流"  
],
[  //fileOldNewContentGroups
"JBXX_JYFWH","FKJL_JYJL"   
"JBXX.JYFWH","FKJL.JYJL"  
"交易方维护","建议交流"  
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"FKGL","FKJL"   
"CWFK","CWJL"  
"GJFK","JYJL"  
"反馈管理","反馈交流"
"错误反馈","错误交流"
"改进反馈","建议交流"
], 
[ //fileOldNewNameGroups
"FKGL","FKJL"   
"CWFK","CWJL"  
"GJFK","JYJL"  
"反馈管理","反馈交流"
"错误反馈","错误交流"
"改进反馈","建议交流"
],
[  //fileOldNewContentGroups
"FKGL","FKJL"   
"CWFK","CWJL"  
"GJFK","JYJL"  
"反馈管理","反馈交流"
"错误反馈","错误交流"
"改进反馈","建议交流"
]
)
|>ModifyCopyExtra
(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"FKSJ_CWFK","ZDCW_CW"   
"FKSJ.CWFK","ZDCW.CW"  
"FKSJ","ZDCW"     
"CWFK","CW"  
"反馈收集","自动收集错误"
"错误反馈","错误"
], 
[ //fileOldNewNameGroups
"FKSJ_CWFK","ZDCW_CW"   
"FKSJ.CWFK","ZDCW.CW"  
"FKSJ","ZDCW"     
"CWFK","CW"  
"反馈收集","自动收集错误"
"错误反馈","错误"
],
[  //fileOldNewContentGroups
"FKSJ_CWFK","ZDCW_CW"   
"FKSJ.CWFK","ZDCW.CW"  
"FKSJ","ZDCW"     
"CWFK","CW"  
"反馈收集","自动收集错误"
"错误反馈","错误"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"JXC_ZHGL_BJGL","FK_FKGL"
"JXC.ZHGL.BJGL","FK.FKGL"
"JXC_","FK_"
"JXC.","FK."
"ZHGL_BJGL","FKGL"
"ZHGL.BJGL","FKGL"
"JHBJ","CWFK"
"XSBJ","GJFK"
"GHSBJ","CWFK"
"WFBJ","GJFK"
"进货报价","错误反馈"
"销售报价","改进反馈"
"报价管理","反馈管理"
"供货商报价","错误反馈"
"我方报价","改进反馈"
"供货商","错误"
"我方","改进"
], 
[ //fileOldNewNameGroups
"JXC_ZHGL_BJGL","FK_FKGL"
"JXC.ZHGL.BJGL","FK.FKGL"
"JXC_","FK_"
"JXC.","FK."
"ZHGL_BJGL","FKGL"
"ZHGL.BJGL","FKGL"
"JHBJ","CWFK"
"XSBJ","GJFK"
"GHSBJ","CWFK"
"WFBJ","GJFK"
"进货报价","错误反馈"
"销售报价","改进反馈"
"报价管理","反馈管理"
"供货商报价","错误反馈"
"我方报价","改进反馈"
"供货商","错误"
"我方","改进"
],
[  //fileOldNewContentGroups
"JXC_ZHGL_BJGL","FK_FKGL"
"JXC.ZHGL.BJGL","FK.FKGL"
"JXC_","FK_"
"JXC.","FK."
"ZHGL_BJGL","FKGL"
"ZHGL.BJGL","FKGL"
"JHBJ","CWFK"
"XSBJ","GJFK"
"GHSBJ","CWFK"
"WFBJ","GJFK"
"进货报价","错误反馈"
"销售报价","改进反馈"
"报价管理","反馈管理"
"供货商报价","错误反馈"
"我方报价","改进反馈"
"供货商","错误"
"我方","改进"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"VC_GXBS","FK_FKSJ"     
".VC.GXBS",".FK.FKSJ"
"GXBS","FKSJ"
"更新部署","反馈收集"
"更新信息","错误反馈"
"更新文件","改进反馈"
"_GXXX","_CWFK"
"_BBWJ","_GJFK"
"_V_","_TV_"
"WX.Data.Update","WX.Data.Feedback"
@"\Update",@"\Feedback"
@"/Update",@"/Feedback"
], 
[ //fileOldNewNameGroups
"VC_GXBS","FK_FKSJ"     
".VC.GXBS",".FK.FKSJ"
"GXBS","FKSJ"
"更新部署","反馈收集"
"更新信息","错误反馈"
"更新文件","改进反馈"
"_GXXX","_CWFK"
"_BBWJ","_GJFK"
"_V_","_TV_"
"WX.Data.Update","WX.Data.Feedback"
@"\Update",@"\Feedback"
@"/Update",@"/Feedback"
],
[  //fileOldNewContentGroups
"VC_GXBS","FK_FKSJ"     
".VC.GXBS",".FK.FKSJ"
"GXBS","FKSJ"
"更新部署","反馈收集"
"更新信息","错误反馈"
"更新文件","改进反馈"
"_GXXX","_CWFK"
"_BBWJ","_GJFK"
"_V_","_TV_"
"WX.Data.Update","WX.Data.Feedback"
@"\Update",@"\Feedback"
@"/Update",@"/Feedback"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"_KJCD","_WJGL"     
"_Frame_","_VC_"
".Frame.KJCD",".VC.WJGL"
".Frame",".VC"
], 
[ //fileOldNewNameGroups
"_KJCD","_WJGL"     
"_Frame_","_VC_"
".Frame.KJCD",".VC.WJGL"
".Frame",".VC"
],
[  //fileOldNewContentGroups
"_KJCD","_WJGL"     
"_Frame_","_VC_"
".Frame.KJCD",".VC.WJGL"
".Frame",".VC"
]
)
|>ModifyCopyExtra

[
"DH_JXC_FXYC","DH.JXC.FXYC"
"DH_JXC_JBXX","DH.JXC.JBXX"
"DH_JXC_JHGL","DH.JXC.JHGL"
"DH_JXC_KCGL","DH.JXC.KCGL"
"DH_JXC_TJBB","DH.JXC.TJBB"
"DH_JXC_XSGL","DH.JXC.XSGL"
"DH_JXC_XTGL","DH.JXC.XTGL"
"DH_JXC_ZHBB","DH.JXC.ZHBB"
"DH_JXC_ZHGL","DH.JXC.ZHGL"
"DH_AC","DH.AC"
"DH_AGN","DH.AGN"
"DH_AMK","DH.AMK"
"DH_JXC","DH.JXC"
"DH","DH"
]
|>fun r->
  for (n1,n2) in r do
    (
    @"D:\TempWorkspace\c",  //sourceDirectory
    @"D:\TempWorkspace\c1",  //targetDirectory
    [ //directoryOldNewNameGroups
    "Test",n1
    ".Test","."+n2
    ], 
    [ //fileOldNewNameGroups
    "Test",n1
    ".Test","."+n2
    ],
    [  //fileOldNewContentGroups
    "Test",n1
    ".Test","."+n2
    ]
    )
    |>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"_KCGL_SPCF","_DH"     
"_JXC_","_Frame_"
".JXC.KCGL.SPCF",".Frame.DH"
".JXC",".Frame"
], 
[ //fileOldNewNameGroups
"_KCGL_SPCF","_DH"     
"_JXC_","_Frame_"
".JXC.KCGL.SPCF",".Frame.DH"
".JXC",".Frame"
],
[  //fileOldNewContentGroups
"_KCGL_SPCF","_DH"     
"_JXC_","_Frame_"
".JXC.KCGL.SPCF",".Frame.DH"
".JXC",".Frame"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"JBXX_FJPWH","JBXX_JYFWH"
"JBXX.FJPWH","JBXX.JYFWH"
"FJP","JYF"
], 
[ //fileOldNewNameGroups
"JBXX_FJPWH","JBXX_JYFWH"
"JBXX.FJPWH","JBXX.JYFWH"
"FJP","JYF"
],
[  //fileOldNewContentGroups
"JBXX_FJPWH","JBXX_JYFWH"
"JBXX.FJPWH","JBXX.JYFWH"
"FJP","JYF"
]
)
|>ModifyCopyExtra

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryOldNewNameGroups
"DanJu_ShenHe","DJSH"
"DanJuShenHe","DJSH"
"ShangPinSelect","SPXZ"
"ShangPin_Modify","SPModify"
"ShangPinModify","SPModify"
"GongHuoQingKuang","GHQK"
"GongHuoShangZhangWu","GHSZW"
"GongHuoXiaoShou","GGXS"
"WoFangFuKuanQingKuang","WFFKQK"
"ShangPin_LieBiao","SPLB"
"ShangPinChaYi","SPCY"
"WangLaiQingKuang","WLQK"
"WangLaiTongJi","WLTJ"
"PopupWindow","DJJG"
"KeHuTuiHuo","KHTH"
"KeHuZhangWu","KHZW"
"KeHuZhiQu","KHZQ"
"AnDanJuHuiZong","ADJHZ"
"AnKeHuHuiZong","AKHHZ"
"XiaoFeiDanJu","XFDJ"
"XiaoFeiQingKuang","XFQK"
"XiaoFeiTongJi","XFTJ"
"YueTuBiao","YTB"
"AnKeHuLeiXing","AKHLX"
"AnKeHuLiuShi","ALSKH"
"FenLeiHuiZong","FLHZ"
"FenLeiTuBiao","FLTB"
"HuiZongTuBiao","HZTB"
"DanJuLieBiao","DJLB"
"FuKuanDan","FKD"
"GongHuoDanJu","GHDJ"
"DanJu","DJ"
"HuiZong","HZ"
"MingXi","MX"
"AnDiQu","ADQ"
"BiaoGe","BG"
"TuBiao","TB"
"RiBao","RB"
"YueBao","YB"
], 
[ //fileOldNewNameGroups
"z","z"
],
[  //fileOldNewContentGroups
"DanJu_ShenHe","DJSH"
"DanJuShenHe","DJSH"
"ShangPinSelect","SPXZ"
"ShangPin_Modify","SPModify"
"ShangPinModify","SPModify"
"GongHuoQingKuang","GHQK"
"GongHuoShangZhangWu","GHSZW"
"GongHuoXiaoShou","GGXS"
"WoFangFuKuanQingKuang","WFFKQK"
"ShangPin_LieBiao","SPLB"
"ShangPinChaYi","SPCY"
"WangLaiQingKuang","WLQK"
"WangLaiTongJi","WLTJ"
"PopupWindow","DJJG"
"KeHuTuiHuo","KHTH"
"KeHuZhangWu","KHZW"
"KeHuZhiQu","KHZQ"
"AnDanJuHuiZong","ADJHZ"
"AnKeHuHuiZong","AKHHZ"
"XiaoFeiDanJu","XFDJ"
"XiaoFeiQingKuang","XFQK"
"XiaoFeiTongJi","XFTJ"
"YueTuBiao","YTB"
"AnKeHuLeiXing","AKHLX"
"AnKeHuLiuShi","ALSKH"
"FenLeiHuiZong","FLHZ"
"FenLeiTuBiao","FLTB"
"HuiZongTuBiao","HZTB"
"DanJuLieBiao","DJLB"
"FuKuanDan","FKD"
"GongHuoDanJu","GHDJ"
"DanJu","DJ"
"HuiZong","HZ"
"MingXi","MX"
"AnDiQu","ADQ"
"BiaoGe","BG"
"TuBiao","TB"
"RiBao","RB"
"YueBao","YB"
]
)
|>ModifyCopyExtra


(
@"D:\Workspace\SBIIMS\SBIIMS_AC",  //sourceDirectory
@"D:\Workspace\SBIIMS\SBIIMS_AC1",  //targetDirectory
[ //directoryModifyStrOldNewGroups
"NewAdd","Add"
"AddNew","Add"
"JiaRu_Modify","JRModify"
"JiaRu_Edit","JRModify"
"JiaRuModify","JRModify"
"JiaRuEdit","JRModify"
"Edit","Modify"

], 
[  //fileModifyStrOldNewGroups
@"\NewAdd",@"\Add"
@"\AddNew",@"\Add"
@"\JiaRuModify",@"\JRModify"
@"\JiaRuEdit",@"\JRModify"
@"\Edit",@"\Modify"

@"NewAdd\",@"Add\"
@"AddNew\",@"Add\"
@"JiaRuModify\",@"JRModify\"
@"JiaRuEdit\",@"JRModify\"
@"Edit\",@"Modify\"
"_NewAdd","_Add"
"_AddNew","_Add"
"JiaRu_Modify","JRModify"
"JiaRu_Edit","JRModify"
"_JR_BJ","_JRModify"
"_JR.","_JRModify."
"_JR_","_JRModify_"
"_JiaRuModify","_JRModify"
"_JiaRuEdit","_JRModify"
"_JREdit","_JRModify"
"_JRBJ","_JRModify"

"_JRXG","_JRModify"
"_SPXG","_SP_Modify"
"_Edit","_Modify"
"_BJ.","_Modify."  //***
"_BJ_","_Modify_"
"_LSZ.","_MX."  //**
"_LSZ_","_MX_"
"_ALSZ.","_AMX."
"_ALSZ_","_AMX_"
"_JBXX_KH_","_JBXX_KHWH_"
"_JBXX_GHS_","_JBXX_GHSWH_"
"_JBXX_SP_","_JBXX_SPWH_"
"_JBXX_YG_","_JBXX_YGWH_"
"_JBXX_CK_","_JBXX_CKWH_"
"_SPXS_DJ_SH","_SPXS_DJSH"
"_DJ_SH.","_DJSH."
"_DJ_SH_","_DJSH_"
"_FeiYongChart","_FY_Chart"
"_LiRunChart","_LR_Chart"
"_ShouRu_","_SR_"
"_ZhiChu_","_ZC_"

"_FYZJ",""

]
)
|>ModifyCopy

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryModifyStrOldNewGroups
"NewAdd","Add"
"Edit","Modify"
], 
[  //fileModifyStrOldNewGroups
"NewAdd","Add"
"Edit","Modify"
"GGXZ_CK","JBXX_CKWH"
]
)
|>ModifyCopy

(
@"D:\Workspace\SBIIMS\SBIIMS_Reference",  //sourceDirectory
@"D:\Workspace\SBIIMS\SBIIMS_Reference01",  //targetDirectory
[ //directoryModifyStrOldNewGroups
"_GG_","_GGXZ_"
"_JB_","_JBXX_"
"_JH_","_JHGL_"
"_KC_","_KCGL_"
"_XS_","_XSGL_"
"_TJ_","_TJBB_"
"_ZH_","_ZHGL_"
"_XT_","_XTGL_"
], 
[  //fileModifyStrOldNewGroups
"_GG_","_GGXZ_"
"_JB_","_JBXX_"
"_JH_","_JHGL_"
"_KC_","_KCGL_"
"_XS_","_XSGL_"
"_TJ_","_TJBB_"
"_ZH_","_ZHGL_"
"_XT_","_XTGL_"
]
)
|>ModifyCopy

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryModifyStrOldNewGroups
"DWBM","WF"
], 
[  //fileModifyStrOldNewGroups
"DWBM","WF"
]
)
|>ModifyCopy

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryModifyStrOldNewGroups  //注意替换顺序，包含相同关键字时，大朝前，小朝后
".Link", ".Link.ZH"
], 
[  //fileModifyStrOldNewGroups    //注意替换顺序，包含相同关键字时，大朝前，小朝后
"_Link","_Link_ZH"     
".Link",".Link.ZH"
]
)
|>ModifyCopy

(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryModifyStrOldNewGroups  //注意替换顺序，包含相同关键字时，大朝前，小朝后
".JXC.KCGL.SPCF", ".JXC.GGHC"
"JXC_KCGL_SPCF","JXC_GGHC"  
"_JXC","_JXC" 
], 
[  //fileModifyStrOldNewGroups    //注意替换顺序，包含相同关键字时，大朝前，小朝后
"_KCGL_SPCF","_GGHC"     
"_JXC_","_JXC_"
".JXC.KCGL.SPCF",".JXC.GGHC"
".JXC",".JXC"
]
)
|>ModifyCopy


(
@"D:\TempWorkspace\c",  //sourceDirectory
@"D:\TempWorkspace\c1",  //targetDirectory
[ //directoryModifyStrOldNewGroups
".ClientChannel.From", ".ClientChannelAdvance.From"
], 
[  //fileModifyStrOldNewGroups
".ClientChannel.From", ".ClientChannelAdvance.From"
]
)
|>ModifyCopy

[
"JXC","JBXX","CKWH","",""
"JXC","JBXX","CZYWH","",""
"JXC","JBXX","FJPWH","",""
"JXC","JBXX","GHSWH","",""
"JXC","JBXX","JYFWH","",""
"JXC","JBXX","KHWH","",""
"JXC","JBXX","SPWH","",""
"JXC","JBXX","XFPWH","",""
"JXC","JBXX","YGWH","",""
"JXC","JBXX","ZCLBWH","",""
]

[
"JXC","JHGL","CGJH","进货管理","采购进货"                                    
"JXC","JHGL","CGTH","进货管理","采购退货"                                                                
"JXC","JHGL","WLZW","进货管理","往来帐务" 
"JXC","JHGL","DJCX","进货管理","采购单据查询"  
"JXC","KCGL","WLZW","进货管理","库存往来查询"   
      
"JXC","XSGL","SPXS","销售管理","商品销售"   
"JXC","XSGL","POS","销售管理","POS 销售"                                         
"JXC","XSGL","XSTH","销售管理","顾客退货"            
"JXC","XSGL","XSHH","销售管理","销售换货"                                  
"JXC","XSGL","WLZW","销售管理","销售往来帐务"      
"JXC","XSGL","DJCX","销售管理","销售单据查询"    
"JXC","KCGL","WLZW","销售管理","库存往来查询"    

"JXC","KCGL","SPCF","库存管理","商品拆分"                                   
"JXC","KCGL","SPKB","库存管理","商品捆绑"    
"JXC","KCGL","SPBY","库存管理","商品报损" 
"JXC","KCGL","SPBS","库存管理","商品报溢"        
"JXC","KCGL","BSBY","库存管理","报损报溢" //合并商品报损和商品报溢                               
"JXC","KCGL","KCPD","库存管理","库存盘点"    
"JXC","KCGL","KCPDX","库存管理","库存盘点X" 
"JXC","KCGL","KCDB","库存管理","库存调拨"                       
"JXC","KCGL","KCYJ","库存管理" ,"库存预警"                                                
"JXC","KCGL","WLZW","库存管理","库存往来帐"   
"JXC","KCGL","KCCX","库存管理","库存查询"       
"JXC","KCGL","DJCX","库存管理","库存单据查询"      
     
"JXC","JHGL","WLZW","统计报表","供货商供货统计"   
"JXC","TJBB","SPCG","统计报表","商品采购统计"   
"JXC","TJBB","CGYCG","统计报表","采购员采购统计"   
"JXC","TJBB","KCCB","统计报表","库存成本统计"   
"JXC","XSGL","WLZW","统计报表","客户消费统计"  
"JXC","TJBB","SPXS","统计报表","商品销售统计"       
"JXC","TJBB","XSYXS","统计报表","销售员销售统计"   
"JXC","TJBB","XSPH","统计报表","商品销售排行"                                                
"JXC","TJBB","YYFX","统计报表","营业分析"    

"JXC","ZHGL","CWGL","综合管理","财务综合管理"  
"JXC","ZHGL","XZGL","综合管理","薪资管理"  
"JXC","ZHGL","JYFYGL","综合管理","经营费用管理"  
"JXC","ZHGL","BJGL","综合管理","报价管理"  
"JXC","ZHGL","JJGL","综合管理","均价管理"  
"JXC","ZHGL","HTGL","综合管理","合同管理"         
"JXC","ZHGL","GHSGL","综合管理","供货商管理"    
"JXC","ZHGL","KHGL","综合管理","客户管理"      
"JXC","ZHGL","HYGL","综合管理","会员管理" 
"JXC","ZHGL","JYFGL","综合管理","经营交易方管理"     
"JXC","ZHGL","YWYGL","综合管理","业务员管理"         
"JXC","ZHGL","XFPGL","综合管理","消费品管理"         
"JXC","ZHGL","FJPGL","综合管理","废旧品管理"               
"JXC","ZHGL","GDZCGL","综合管理","固定资产管理"                                  
"JXC","ZHGL","KHJH","综合管理","客户借货管理"                            
"JXC","ZHGL","SJTX","综合管理","事件提醒"                                    

"JXC","JBXX","SPWH","基本信息管理","商品信息维护"                  
"JXC","JBXX","GHSWH","基本信息管理","供货商信息维护"       
"JXC","JBXX","KHWH","基本信息管理","客户信息维护"     
"JXC","JBXX","JYFWH","基本信息管理","交易方信息维护"    
"JXC","JBXX","YGWH","基本信息管理","员工信息维护"    
"JXC","JBXX","CZYWH","基本信息管理","操作员信息维护"         
"JXC","JBXX","CKWH","基本信息管理","仓库信息维护"                                         
"JXC","JBXX","XFPWH","基本信息管理","消费品信息维护"                  
"JXC","JBXX","FJPWH","基本信息管理","废旧品信息维护"                                         
"JXC","JBXX","ZCLBWH","基本信息管理","固定资产类别维护"          

"JXC","ZHBB","DJBB","综合报表管理","单据报表"          //使用较为频繁，且同一类型的报表显示的数据一致，因此单独处理
"JXC","ZHBB","QDBB","综合报表管理","清单报表"          //应该和相关功能合并？？？
"JXC","ZHBB","TBBB","综合报表管理","图表报表"           //应该和相关功能合并？？？
"JXC","ZHBB","DCBB","综合报表管理","导出"                //应该和相关功能合并？？？

"JXC","FXYC","CGFX","分析预测","商品采购分析"  
"JXC","FXYC","CGJH","分析预测","采购商品计划"  
"JXC","FXYC","XSFX","分析预测","销售分析"  
"JXC","FXYC","XSYC","分析预测","销售预测"  
"JXC","FXYC","KCFX","分析预测","库存分析"  
"JXC","FXYC","KCYC","分析预测","库存积压风险预测"  
"JXC","FXYC","JGFX","分析预测","价格分析"   
"JXC","FXYC","JGYC","分析预测","价格趋势预测"    
"JXC","FXYC","YLFX","分析预测","盈利分析"  
"JXC","FXYC","YLYC","分析预测","盈利预测"  
"JXC","FXYC","GZLFX","分析预测","业务员工作量分析"          
"JXC","FXYC","GZLYC","分析预测","业务员工作量预测"       
                              
"JXC","XTGL","XTSZ","系统管理","系统设置"    
"JXC","XTGL","BFHF","系统管理","备份恢复"             
"JXC","XTGL","YZWJS","系统管理","月账务结算"                         
"JXC","XTGL","NZWJS","系统管理","年账务结算"    
"JXC","XTGL","XTRZ","系统管理","系统日志"                                   
"JXC","XTGL","XTCSH","系统管理","系统初始化"                              
"JXC","XTGL","QCJZ","系统管理","期初建账"             
"JXC","XTGL","TMDY","系统管理","条码打印"      
]
|>fun a->
  for (m1,m2,m3,_,_) in a do
    (
    @"D:\TempWorkspace\c",  //sourceDirectory
    @"D:\TempWorkspace\c1",  //targetDirectory
    [ //directoryModifyStrOldNewGroups
    ".JXC.KCGL.SPCF", String.Format(".{0}.{1}.{2}",m1,m2,m3)
    "JXC_KCGL_SPCF", String.Format("{0}_{1}_{2}",m1,m2,m3)
    "_JXC", String.Format("_{0}",m1)
    ], 
    [  //fileModifyStrOldNewGroups
    "_JXC_",String.Format("_{0}_",m1)
    "_KCGL_SPCF",String.Format("_{0}_{1}",m2,m3)     
    ".JXC.KCGL.SPCF",String.Format(".{0}.{1}.{2}",m1,m2,m3)
    ]
    )
    |>ModifyCopy


[
"AC","CZYJSGL"
"AC","CZYWH"
"AC","GGXZ"
"AC","GG"
"AC","GNWH"
"AC","JSGNGL"
"AC","JSQXGL"
"AC","JSWH"
"AC","QXWH"
]
|>fun a->
  for (m1,m2) in a do
    (
    @"D:\TempWorkspace\c",  //sourceDirectory
    @"D:\TempWorkspace\c1",  //targetDirectory
    [ //directoryModifyStrOldNewGroups  //注意替换顺序，包含相同关键字时，大朝前，小朝后
    ".JXC.KCGL.SPCF",String.Format(".{0}.{1}",m1,m2)
    "JXC_KCGL_SPCF", String.Format("{0}_{1}",m1,m2)
    "_JXC", String.Format("_{0}",m1)
    ], 
    [  //fileModifyStrOldNewGroups  //注意替换顺序，包含相同关键字时，大朝前，小朝后
    "_KCGL_SPCF",String.Format("_{0}",m2)     
    ".JXC.KCGL.SPCF",String.Format(".{0}.{1}",m1,m2)
    "_JXC_",String.Format("_{0}_",m1)
    ".JXC",String.Format(".{0}",m1) 
    ]
    )
    |>ModifyCopy


//----------------------------------------------

//Windwos文件系统 --拷贝并修改文件名及内容, 但不包括例外扩展名的文件， ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换
//针对源码受第三方工具管理的情况，如Team Foundation Server
let ModifyCopyWithoutExceptionalExtension (sourceDirectoryPath:string,targetPath:string,directoryModifyStrOldNewGroups:(string*string) list,fileModifyStrOldNewGroups:(string*string) list, exceptionalExtensions:string seq) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopyWithoutExceptionalExtension (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,directoryModifyStrOldNewGroups:(string*string) list,fileModifyStrOldNewGroups:(string*string) list, exceptionalExtensions:string seq)=       
    for b in sourceDirectory.GetFiles() do
      match b.Extension with
      | EqualsIn exceptionalExtensions _ ->()
      | _ ->
          match File.ReadFile b.FullName with
          | x ->
              match 
                fileModifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) b.Name,
                fileModifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) x with
              | y,z ->
                  Path.Combine(targetDirectory.FullName,y)
                  |>File.WriteFileCreateOnly z     
    for a in sourceDirectory.GetDirectories() do
      match directoryModifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) a.Name with
      | x ->
          match targetDirectory.CreateSubdirectory(x)  with
          | x ->  ModifyCopyWithoutExceptionalExtension(a,x,directoryModifyStrOldNewGroups,fileModifyStrOldNewGroups,exceptionalExtensions)
  ModifyCopyWithoutExceptionalExtension(sourceDirectory,targetDirectory,directoryModifyStrOldNewGroups,fileModifyStrOldNewGroups,exceptionalExtensions) 

(
@"D:\TempWorkspace\b",  //sourceDirectory
@"D:\TempWorkspace\b1",  //targetDirectory
[ //directoryModifyStrOldNewGroups
".AccessControl", ".AC"
], 
[  //fileModifyStrOldNewGroups
"_AC_","_"
".AccessControl", ".AC"
],
[
".vspscc"   //Exceptional Extension
]
)
|>ModifyCopyWithoutExceptionalExtension

(
@"D:\Workspace\SBIIMS",  //sourceDirectory
@"D:\Workspace\SBIIMS1",  //targetDirectory
[ //directoryModifyStrOldNewGroups
".", "."
], 
[  //fileModifyStrOldNewGroups
".","."
],
[
".vspscc"   //Exceptional Extension
]
)
|>ModifyCopyWithoutExceptionalExtension


//---------------------------------------------------------------------------------------------
//*********************************************************************************************
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换
//使用正则表达式，例外目录实现有问题！！！！！！！！！！
let ModifyCopyWithRegex (sourceDirectoryPath:string,targetDirectoryPath:string,directoryOldNewNamePatterns:(string*string) list,fileOldNewNamePatterns:(string*string) list,fileOldNewContentPatterns:(string*string) list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,directoryOldNewNamePatterns:(string*string) list,fileOldNewNamePatterns:(string*string) list,fileOldNewContentPatterns:(string*string) list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list)=       
    for b in sourceDirectory.GetFiles() do
      match File.ReadFile b.FullName with
      | x ->
          match 
            match fileNamePatterns|>Regex.IsMatchIn b.Name, exceptionDirectoryNamePatterns|>Regex.IsMatchIn sourceDirectory.Name|>not with
            | true, true ->   
                fileOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) b.Name,
                fileOldNewContentPatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) x
            | _ ->b.Name,x 
            with
          | y,z ->
              Path.Combine(targetDirectory.FullName,y)
              |>File.WriteFileCreateOnly z     
    for a in sourceDirectory.GetDirectories() do
      match 
        match exceptionDirectoryNamePatterns|>Regex.IsMatchIn a.Name|>not with
        | true ->
            directoryOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) a.Name
        | _ ->a.Name
        with
      | x ->
          match targetDirectory.CreateSubdirectory(x)  with
          | y ->  ModifyCopy(a,y,directoryOldNewNamePatterns,fileOldNewNamePatterns,fileOldNewContentPatterns,exceptionDirectoryNamePatterns,fileNamePatterns)
  ModifyCopy(sourceDirectory,targetDirectory,directoryOldNewNamePatterns,fileOldNewNamePatterns,fileOldNewContentPatterns,exceptionDirectoryNamePatterns,fileNamePatterns) 
//*********************************************************************************************
(
@"D:\TempWorkspace\FunctionNodeTemplate",  //sourceDirectory
@"D:\TempWorkspace\FunctionNodeTemplate1",  //targetDirectory
[ //directoryOldNewNamePatterns       //关于正则表达式的替换http://msdn.microsoft.com/zh-cn/library/ewy2t5e0.aspx  http://msdn.microsoft.com/en-us/library/az24scfc.aspx
@"z", 
  @"z"
], 
[  //fileOldNewNamePatterns
@"z", 
  @"z"
],
[  //fileOldNewContentPatterns
@"\<Reference\s+Include\s*=\s*""(System)""\s*/>", 
  @"\<Reference Includex=""$1""/>"
],
[
"WX.Data.ServiceContracts[a-zA-Z\.]+"  //例外的文件目录片段
"WX.Data.WcfService[a-zA-Z\.]+"       
],
[
//".fsproj"      //只替换指定文件扩展名的文件
"[\w\W]+\.fs"
"[\w\W]+\.txt"
]
)
|>ModifyCopyWithRegex


//---------------------------------------------------------------------------------------------
//*********************************************************************************************
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换
//使用正则表达式,  使用例外目录
let ModifyCopyWithExceptionWithRegex (sourceDirectoryPaths:string seq,targetDirectoryPath:string,directoryOldNewNamePatterns:(string*string) list,fileOldNewNamePatterns:(string*string) list,fileOldNewContentPatterns:(string*string) list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list) =
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectories:DirectoryInfo seq,targetDirectory:DirectoryInfo,directoryOldNewNamePatterns:(string*string) list,fileOldNewNamePatterns:(string*string) list,fileOldNewContentPatterns:(string*string) list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list)=       
    for a in sourceDirectories do
      match exceptionDirectoryNamePatterns|>Regex.IsMatchIn a.Name|>not with
      | false ->()  //例外目录下的所有文件和子目录将忽略！ 例外的目录不一定是当前级别的目录
      | true ->
          match directoryOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) a.Name with
          | xa ->
              match targetDirectory.CreateSubdirectory(xa)  with
              | ya ->
                  for b in a.GetFiles() do
                    match File.ReadFile b.FullName with
                    | x ->
                        match 
                          match fileNamePatterns|>Regex.IsMatchIn b.Name with
                          | true ->   
                              fileOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) b.Name,
                              fileOldNewContentPatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) x
                          | _ ->b.Name,x 
                          with
                        | y,z ->
                            Path.Combine(ya.FullName,y)
                            |>File.WriteFileCreateOnly z    
                  ModifyCopy(a.GetDirectories(),ya,directoryOldNewNamePatterns,fileOldNewNamePatterns,fileOldNewContentPatterns,exceptionDirectoryNamePatterns,fileNamePatterns)
  let rec CopyOnly (sourceDirectories:DirectoryInfo seq,targetDirectory:DirectoryInfo,directoryOldNewNamePatterns:(string*string) list)=  
    for a in sourceDirectories do 
      match exceptionDirectoryNamePatterns|>Regex.IsMatchIn a.Name with
      | true -> //例外目录下所以文件和子目录都应拷贝
          let rec CopyAll (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo)=
            match targetDirectory.CreateSubdirectory(sourceDirectory.Name) with
            | x ->      
                for b in sourceDirectory.GetFiles() do
                  match Path.Combine(x.FullName,b.Name) with
                  | y ->b.CopyTo (y,true)|>ignore
                for b in sourceDirectory.GetDirectories() do
                  CopyAll (b,x) 
          CopyAll (a,targetDirectory)  
      | _ ->
         match directoryOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) a.Name with //目标目录须同步
         | x ->
             match DirectoryInfo (Path.Combine(targetDirectory.FullName,x)) with  //不在例外目录范围内目录已存在
             | y ->
                 CopyOnly (a.GetDirectories(),y,directoryOldNewNamePatterns)
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->
      ModifyCopy(x,targetDirectory,directoryOldNewNamePatterns,fileOldNewNamePatterns,fileOldNewContentPatterns,exceptionDirectoryNamePatterns,fileNamePatterns) 
      CopyOnly (x,targetDirectory,directoryOldNewNamePatterns)
//*********************************************************************************************
(
[ //sourceDirectory
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
],
@"D:\Workspace\SBIIMS\SBIIMS_JXC1",  //targetDirectory
[ //directoryOldNewNamePatterns       //关于正则表达式的替换http://msdn.microsoft.com/zh-cn/library/ewy2t5e0.aspx  http://msdn.microsoft.com/en-us/library/az24scfc.aspx
@"z", 
  @"z"
], 
[  //fileOldNewNamePatterns
@"z", 
  @"z"
],
[  //fileOldNewContentPatterns
//@"\<Reference\s+Include\s*=\s*""(System)""\s*/>", 
//  @"\<Reference Includex=""$1""/>"
"_Modify_","_BJ_"
],
[
"WX.Data.FViewModel[a-zA-Z\.]+"  //例外的文件目录片段
"WX.Data.View[a-zA-Z\.]+"       
],
[     //只替换指定文件
//"[\w\W]+\.fs"
//"[\w\W]+\.txt"
"[\w\W]+"   //所有
]
)
|>ModifyCopyWithExceptionWithRegex

//---------------------------------------------------------------------------------------------
//*********************************************************************************************
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改目录名,文件名和文件内容, ***只对平面文件, 实现一个目录或一个文件多次替换
//使用正则表达式,  使用例外目录
let ModifyCopyWithIncludesWithRegex (sourceDirectoryPaths:string seq,targetDirectoryPath:string,directoryOldNewNamePatterns:(string*string) list,fileOldNewNamePatterns:(string*string) list,fileOldNewContentPatterns:(string*string) list,includesDirectoryNamePatterns:string list,fileNamePatterns:string list) =
  //let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetDirectoryPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec ModifyCopy (sourceDirectories:DirectoryInfo seq,targetDirectory:DirectoryInfo,directoryOldNewNamePatterns:(string*string) list,fileOldNewNamePatterns:(string*string) list,fileOldNewContentPatterns:(string*string) list,includesDirectoryNamePatterns:string list,fileNamePatterns:string list)=       
    for a in sourceDirectories do
      match includesDirectoryNamePatterns|>Regex.IsMatchIn a.Name with
      | false ->()  //例外目录下的所有文件和子目录将忽略！ 例外的目录不一定是当前级别的目录
      | true ->
          match directoryOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) a.Name with
          | xa ->
              match targetDirectory.CreateSubdirectory(xa)  with
              | ya ->
                  for b in a.GetFiles() do
                    match File.ReadFile b.FullName with
                    | x ->
                        match 
                          match fileNamePatterns|>Regex.IsMatchIn b.Name with
                          | true ->   
                              fileOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) b.Name,
                              fileOldNewContentPatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) x
                          | _ ->b.Name,x 
                          with
                        | y,z ->
                            Path.Combine(ya.FullName,y)
                            |>File.WriteFileCreateOnly z    
                  ModifyCopy(a.GetDirectories(),ya,directoryOldNewNamePatterns,fileOldNewNamePatterns,fileOldNewContentPatterns,includesDirectoryNamePatterns,fileNamePatterns)
  let rec CopyOnly (sourceDirectories:DirectoryInfo seq,targetDirectory:DirectoryInfo,directoryOldNewNamePatterns:(string*string) list)=  
    for a in sourceDirectories do 
      match includesDirectoryNamePatterns|>Regex.IsMatchIn a.Name|>not with
      | true -> //例外目录下所以文件和子目录都应拷贝
          let rec CopyAll (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo)=
            match targetDirectory.CreateSubdirectory(sourceDirectory.Name) with
            | x ->      
                for b in sourceDirectory.GetFiles() do
                  match Path.Combine(x.FullName,b.Name) with
                  | y ->b.CopyTo (y,true)|>ignore
                for b in sourceDirectory.GetDirectories() do
                  CopyAll (b,x) 
          CopyAll (a,targetDirectory)  
      | _ ->
         match directoryOldNewNamePatterns|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) a.Name with //目标目录须同步
         | x ->
             match DirectoryInfo (Path.Combine(targetDirectory.FullName,x)) with  //不在例外目录范围内目录已存在
             | y ->
                 CopyOnly (a.GetDirectories(),y,directoryOldNewNamePatterns)
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->
      ModifyCopy(x,targetDirectory,directoryOldNewNamePatterns,fileOldNewNamePatterns,fileOldNewContentPatterns,includesDirectoryNamePatterns,fileNamePatterns) 
      CopyOnly (x,targetDirectory,directoryOldNewNamePatterns)
//*********************************************************************************************
(
[ //sourceDirectory
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
],
@"D:\Workspace\SBIIMS\SBIIMS_JXC1",  //targetDirectory
[ //directoryOldNewNamePatterns       //关于正则表达式的替换http://msdn.microsoft.com/zh-cn/library/ewy2t5e0.aspx  http://msdn.microsoft.com/en-us/library/az24scfc.aspx
@"z", 
  @"z"
], 
[  //fileOldNewNamePatterns
@"[a-zA-Z_]*\_GGXZ\_CD\_RQD\_XZ","_GGXZ_CD_RQD"
],
[  //fileOldNewContentPatterns
@"[a-zA-Z_]*\_GGXZ\_CD\_RQD\_XZ","_GGXZ_CD_RQD"
],
[ //包括的文件目录片段
"WX.Data.FViewModel[a-zA-Z\.]+"  
"WX.Data.View[a-zA-Z\.]+"       
],
[     //只替换指定文件
//"[\w\W]+\.fs"
//"[\w\W]+\.txt"
"[\w\W]+"
]
)
|>ModifyCopyWithIncludesWithRegex

//---------------------------------------------------------------------------------------------
//*********************************************************************************************
//Windwos文件系统 --只修改文件内容
//使用正则表达式
let ModifyFilesWithRegex (sourceDirectoryPaths:string seq,oldNewPatternGroups:(string*string) list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list) =
  let rec ModifyFiles (sourceDirectories:DirectoryInfo seq,oldNewPatternGroups:(string*string) list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list)=       
    seq{
      for a in sourceDirectories do 
        match exceptionDirectoryNamePatterns|>Regex.IsMatchIn a.Name with
        | true ->() //ObjectDumper.Write a.Name
        | _ ->
            for b in a.GetFiles() do
              match fileNamePatterns|>Regex.IsMatchIn b.Name with
              | true ->
                  //ObjectDumper.Write b.FullName
                  match File.ReadFile b.FullName with
                  | x -> 
                      match   
                        oldNewPatternGroups
                        |>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v,RegexOptions.Multiline)) x
                        with
                      | y ->
                          //ObjectDumper.Write y 
                          File.ModifyFile y b.FullName
                          yield b.FullName
              | _ ->() 
        yield! ModifyFiles(a.GetDirectories(),oldNewPatternGroups,exceptionDirectoryNamePatterns,fileNamePatterns)
    }
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->ModifyFiles(x,oldNewPatternGroups,exceptionDirectoryNamePatterns,fileNamePatterns) 
  |>Seq.toArray

(*
@"\<AssemblyName\>\s*WX\.Data\.FviewModel\.([a-zA-Z\.]+)\s*\<\/AssemblyName\>", 
  @"<AssemblyName>WX.Data.FViewModel.$1</AssemblyName>"
@"\<Name\>\s*WX\.Data\.FviewModel\.([a-zA-Z\.]+)\s*</Name>",
  @"<Name>WX.Data.FViewModel.$1</Name>"

//正确,换行只能通过"$x"来实现
"""^\<Project\s+ToolsVersion\=\"12\.0\"[\w\W\s]+2003\"\>(\s*\n\s*)\<PropertyGroup\>(\s*)$""",
  """<Project ToolsVersion="12.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">$1
<Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />$1
<PropertyGroup>$2""" 
"""^(\s*)abstract\s+(?!Get)([a-zA-Z_]+)\s*:\s*([a-zA-Z_\[\]]+)\s*\-\>\s*BD_Result(\s*)$""",
  """$1abstract $2:BD_ExecuteContent<$3> -> BD_ExecuteResult$4""" 
"""^(\s*)abstract\s+(?!Get)([a-zA-Z_]+)\s*:\s*([a-zA-Z_\[\]]+)\s*\-\>\s*BD_ResultBase(\s*)$""",
  """$1abstract $2:BD_ExecuteContent<$3> -> BD_ExecuteResult$4""" 
"""^(\s*)abstract\s+(Get[a-zA-Z_]+)\s*:\s*([a-zA-Z_]+)\s*\-\>\s*([a-zA-Z_\[\]]+)(\s*)$""",
  """$1abstract $2:$3 -> BD_QueryResult<$4>$5""" 
"""^open\s+WX\.Data\.BusinessEntities(\s*)$""",
  """open WX.Data.BusinessBase$1
open WX.Data.BusinessEntities$1""" 
"""^open\s+WX\.Data\.BusinessEntities(\s*)\nopen\s+WX\.Data\.BusinessBase\s*$""",
  """open WX.Data.BusinessBase$1
open WX.Data.BusinessEntities$1""" 
"""^(\s*)member\s+this\.(?!Get)([a-zA-Z_]+)\s*\(\s*[a-zA-Z_]+\s*:\s*([a-zA-Z_\[\]]+)\s*\)\s*\=(\s*)$""",
  """$1member this.$2 (executeContent:BD_ExecuteContent<$3>)=$4""" 
"""new\s+BD_Result\s*\(""",
  """new BD_ExecuteResult(""" 
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)(use\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*Seq\.pageAttach\s+queryEntity\.PagingInfo\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*)$""",
  """$1member this.$2 (queryEntity:$3)=$4
$5let pagingInfo=queryEntity.PagingInfo$4
$5let result=new BD_QueryResult<$8[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)$4
$5try$4
$6$7
$6|>Seq.toResult result$4
$5with$4
$5| e -> this.AttachError($9result)$10""" 
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)let\s+pagingInfo\s*=\s*queryEntity\.PagingInfo\s*\n\s+(let\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*fun\s+a\s*\-\>\s*\n\s+if\s+a\.Length\s*\>\s*0\s+then\s+a\.\[0\]\.PagingInfo\s*\<\-\s*pagingInfo\s*\n\s+a\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*\n\s*)$""",
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)let\s+pagingInfo\s*=\s*queryEntity\.PagingInfo\s*\n\s+(let\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*fun\s+a\s*\-\>\s*\n\s+if\s+a\.Length\s*\>\s*0\s+then\s+a\.\[0\]\.PagingInfo\s*\<\-\s*pagingInfo\s*\n(\s+\/\/.+\s*\n\s+async\{\s*\n(?:(?!member)[\w\W\n])+?)\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*\n\s*)$""",
  """$1member this.$2 (queryEntity:$3)=$4
$5let pagingInfo=queryEntity.PagingInfo$4
$5let result=new BD_QueryResult<$9[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)$4
$5try$4
$6$7
$6|>fun a ->
$8
$6|>Seq.toResult result$4
$5with$4
$5| e -> this.AttachError($10result)$11""" 
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)let\s+pagingInfo\s*=\s*queryEntity\.PagingInfo\s*\n\s+(let\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*Seq\.pageAttach\s+pagingInfo\s*\n\s+\|\>\s*fun\s+a\s*\-\>\s*\n(\s+\/\/.+\s*\n\s+async\{\s*\n(?:(?!member)[\w\W\n])+?)\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*\n\s*)$""",
  """$1member this.$2 (queryEntity:$3)=$4
$5let pagingInfo=queryEntity.PagingInfo$4
$5let result=new BD_QueryResult<$9[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)$4
$5try$4
$6$7
$6|>fun a ->
$8
$6|>Seq.toResult result$4
$5with$4
$5| e -> this.AttachError($10result)$11""" 
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)(use\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*PSeq\.toArray\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*)$""",
  """$1member this.$2 (queryEntity:$3)=$4
$5let result=new BD_QueryResult<$8[]>(ExecuteDateTime=DateTime.Now)$4
$5try$4
$6$7
$6|>Seq.toResult result$4
$5with$4
$5| e -> this.AttachError($9result)$10""" 
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)(let\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*Seq\.toArray\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*)$""",
  """$1member this.$2 (queryEntity:$3)=$4
$5let result=new BD_QueryResult<$8[]>(ExecuteDateTime=DateTime.Now)$4
$5try$4
$6$7
$6|>Seq.toResult result$4
$5with$4
$5| e -> this.AttachError($9result)$10"""
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)let\s+pagingInfo\s*=\s*queryEntity\.PagingInfo\s*\n\s+((?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*fun\s+a\s*\-\>\s*\n\s+if\s+a\.Length\s*\>\s*0\s+then\s+a\.\[0\]\.PagingInfo\s*\<\-\s*pagingInfo\s*\n\s+a\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*)$""" 
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)(let\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*Seq\.page\s+queryEntity\.PagingInfo\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*)$""",
"""^(\s+)member\s+this\.(Get[a-zA-Z_]+View)\s*\(\s*queryEntity\s*\:\s*([a-zA-Z_]+)\s*\)\s*\=(\s*)\n(\s+)try\s*\n(\s+)(let\s+sb\s*\=\s*new\s+SBIIMS_[a-zA-Z_]+EntitiesAdvance\s*\(\)\s*\n(?:(?!member)[\w\W\n])+?)\n\s+\|\>\s*Seq\.toArray\s*\n\s+with\s*\n\s+\|\s*e\s*\-\>\s*this\.QueryErrorProcess\<\s*([a-zA-Z_]+)\s*\>\s*\((\s*e\s*\,\s*\-1\s*\,\s*this\s*\,\s*GetEntitiesAdvance\s*\,)\s*queryEntity\.IsReturnQueryError\s*\)(\s*)$""",
  """$1member this.$2 (queryEntity:$3)=$4
$5let pagingInfo=queryEntity.PagingInfo$4
$5let result=new BD_QueryResult<$8[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)$4
$5try$4
$6$7
$6|>Seq.page pagingInfo$4
$6|>Seq.toResult result$4
$5with$4
$5| e -> this.AttachError($9result)$10""" 
"""^(\s+\|\s*e\s*\-\>\s*)this\.GenericErrorProcess\s*\((\s*e\s*\,\-[0-9]+\s*\,\s*this)\s*\,\s*GenericExecute\s*\,\s*result\s*\)(\s*)$""", //批量处理有问题
  """$1this.AttachError($2,result)$3"""
"""this\.GenericErrorProcess\s*\((\s*e\s*,\s*\-[0-9]+\s*,\s*this)\s*,\s*GenericExecute\s*,\s*result\s*\)""",
  """this.AttachError($1,result)"""
"""open\s+WX\.Data\.BusinessEntities(\s*)\n\s*open\s+WX\.Data\.BusinessBase\s*""",
  """open WX.Data.BusinessBase$1
open WX.Data.BusinessEntities$1
"""
"""open\s+WX\.Data\.BusinessBase(\s*)\n\s*open\s+WX\.Data\.BusinessBase\s*""",
  """open WX.Data.BusinessBase$1
"""
"""open\s+WX\.Data(\s*)\n\s*open\s+WX\.Data\.BusinessEntities\s*""",
  """open WX.Data.BusinessBase$1
"""
"""open\s+WX\.Data\.DataModel(\s*)\n\s*open\s+WX\.Data\.BusinessBase\s*\n\s*open\s+WX\.Data\.BusinessEntities\s*\n\s*open\s+WX\.Data\.IDataAccess\s*\n\s*open\s+WX\.Data\.Helper\s*\n\s*open\s+WX\.Data(\s*\n)""",
  """open WX.Data$1
open WX.Data.Helper$1
open WX.Data.DataModel$1
open WX.Data.BusinessBase$1
open WX.Data.BusinessEntities$1
open WX.Data.IDataAccess$2"""
"""open\s+WX\.Data\.BusinessBase(\s*)\n\s*open\s+WX\.Data(\s*\n)""",
  """open WX.Data$1
open WX.Data.BusinessBase$2"""
"""(?<!BusinessBase)(\s+)(\n\s*)open\s+WX\.Data\.BusinessEntities(\s*\n)""",
  """$1$2open WX.Data.BusinessBase$1
open WX.Data.BusinessEntities$3"""
"""^(\s{8})\|\s*_\s*\-\>\s*\(\s*\)(\s*)\n(\s{8})result\.ResultLength\s*\<\-\s*sb\.SaveChanges\s*\(\)\s*\n\s{8}result(\s*)$""",
  """$1    result.ResultLength<-sb.SaveChanges()$2
$3| _ ->()$2
$1result$4"""
*)
//------------------------------------------------------------------
(*
正则表达式匹配不包含某字符串的表达式
http://www.newxing.com/Tech/Program/Other/RegExp_730.html
*)
( 
[ //sourceDirectories
@"D:\Workspace\SBIIMS"
//@"g:\temp\1"
//@"G:\temp\WX.Data.FViewModelAdvance.AC.CZYJSGL"
//@"D:\Workspace\Temp\1"
//@"D:\Workspace\SBIIMS\SBIIMS_AC"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\TempWorkspace\c"
//@"D:\Workspace\Temp\AC"
],  
[  //oldNewPatternGroups //内容替换模式，对应xml加入"$"行结束符后可能不正确即使要加，前面最好使用\s*,通常才能匹配
"""^(\s{8})result\.ResultLength\s*\<\-\s*sb\.SaveChanges\s*\(\)(\s*)\n\s{8}result(\s*)$""",
  """$1    result.ResultLength<-sb.SaveChanges()$2
$1result$3"""
],
[ //例外的文件目录Pattern
//@"WX\.Data\.ServiceContracts[a-zA-Z\.]+"  
//@"WX\.Data\.WcfService[a-zA-Z\.]+"       
],
[  //只替换符合名称模式的文件
//"IDA_[a-zA-Z_]+Advance.fs"
//"IDA_[a-zA-Z_]+_QueryAdvance.fs"
//"IDA_[a-zA-Z_]+_BusinessAdvance.fs"
//"^DA_[a-zA-Z_]+_QueryAdvance.fs$"
//"^DA_[a-zA-Z_]+_BusinessAdvance.fs$"
//"BL_[a-zA-Z_]+_Advance.fs"
//"WS_[a-zA-Z_]+_AdvanceChannel.fs"
//"WS_[a-zA-Z_]+_Advance.fs"
//"[a-zA-Z_]+\.fs"     
//@"^WX\.Data\.FViewModel\.[a-zA-Z\.]+\.[cf]sproj$"  
//@"^WX\.Data\.FViewModelAdvance\.[a-zA-Z\.]+\.[cf]sproj$"  
//@"^FVM_[a-zA-Z_]+_Advance\.fs$" 
//@"^[a-zA-Z\.]+\.[cf]sproj$" 
//@"^DA_[a-zA-Z_]+Advance\.fs$"
//@"[a-zA-Z_]+\.fs$"
"^DA_[a-zA-Z_]+\.fs$"
]
)
|>ModifyFilesWithRegex
|>Seq.iter (fun a->ObjectDumper.Write a)

//------------------------------------------------------------------

let entryDirectory=DirectoryInfo @"D:\Workspace\SBIIMS\SBIIMS_JXC"
seq{
  for n in entryDirectory.GetDirectories() do
    match n.Name.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries) with
    | [|a;b|] when a.ToUpper()=a && b.ToUpper()=b ->
        yield String.Format (".{0}.{1}",a,b), String.Format ("_{0}_{1}",a,b)
    | [|a;b;c|] when a.ToUpper()=a && b.ToUpper()=b && c.ToUpper()=c ->
        yield String.Format (".{0}.{1}.{2}",a,b,c), String.Format ("_{0}_{1}_{2}",a,b,c)
    | _ ->()
}
|>Seq.toArray
|>Seq.iter (fun (a,b) ->    
    ( 
    [ //sourceDirectories
    //@"D:\Workspace\SBIIMS\SBIIMS_AC"
    //@"D:\Workspace\SBIIMS\SBIIMS_JXC"
    Path.Combine(@"D:\Workspace\SBIIMS\SBIIMS_JXC",b.Remove(0,1))
    ],  
    [  //oldNewPatternGroups //内容替换模式，对应xml加入"$"行结束符后可能不正确即使要加，前面最好使用\s*,通常才能匹配
    @"SBIIMS_QueryJXCEntitiesAdvance", 
      String.Format(@"SBIIMS{0}EntitiesAdvance", b)
    ],
    [ //例外的文件目录Pattern
    //@"WX\.Data\.ServiceContracts[a-zA-Z\.]+"  
    //@"WX\.Data\.WcfService[a-zA-Z\.]+"       
    ],
    [  //只替换符合名称模式的文件
    //"[a-zA-Z_]+\.fs"     
    @"^[I]*DA_[a-zA-Z_]+\.[cf]s$"  
    ]
    )
    |>ModifyFilesWithRegex
    |>Seq.iter (fun a->ObjectDumper.Write a)
    )

( 
[ //sourceDirectories
//@"D:\Workspace\SBIIMS\SBIIMS_AC"
@"D:\Temp\Dll\JXC_JBXX_CKWH"
],  
[  //oldNewPatternGroups //内容替换模式，对应xml加入"$"行结束符后可能不正确即使要加，前面最好使用\s*,通常才能匹配
@"SBIIMS_JXCEntitiesAdvance", 
  String.Format(@"SBIIMS{0}EntitiesAdvance", "_JXC_JBXX_CKWH")
],
[ //例外的文件目录Pattern
//@"WX\.Data\.ServiceContracts[a-zA-Z\.]+"  
//@"WX\.Data\.WcfService[a-zA-Z\.]+"       
],
[  //只替换符合名称模式的文件
//"[a-zA-Z_]+\.fs"     
@"^[I]*DA_[a-zA-Z_]+\.[cf]s$"  
]
)
|>ModifyFilesWithRegex
|>Seq.iter (fun a->ObjectDumper.Write a)

//---------------------------------------------------------------------------------------------
//*********************************************************************************************
let conetent=File.ReadFile """D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH\WX.Data.DataAccessAdvance.JXC.JBXX.SPWH\DA_JBXX_SPWH_BusinessAdvance.fs"""
let matchs=Regex.Matches(conetent, @"DA_[A-Z_]+\.",RegexOptions.None)
matchs
|>Seq.cast<Match>
|>Seq.distinctBy (fun a->a.Value)
|>ObjectDumper.Write

//Windwos文件系统 --查查指定的文件内容
//使用正则表达式
let FindWithRegex (sourceDirectoryPaths:string seq,contentPatterns:string list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list) =
  let rec Find (sourceDirectories:DirectoryInfo seq,contentPatterns:string list,exceptionDirectoryNamePatterns:string list,fileNamePatterns:string list)=       
    seq{
      for a in sourceDirectories do 
        match exceptionDirectoryNamePatterns|>Regex.IsMatchIn a.Name with
        | true ->() //ObjectDumper.Write a.Name
        | _ ->
            for b in a.GetFiles() do
              match fileNamePatterns|>Regex.IsMatchIn b.Name with
              | true ->
                  //ObjectDumper.Write b.Name
                  match File.ReadFile b.FullName with
                  | x -> 
                      //ObjectDumper.Write x
                      for n in contentPatterns do
                        for m in Regex.Matches(x, n,RegexOptions.Multiline)|>Seq.cast<Match>|>Seq.distinctBy (fun a->a.Value) do
                          //ObjectDumper.Write m
                          yield b.FullName,m.Value
              | _ ->() 
        yield! Find(a.GetDirectories(),contentPatterns,exceptionDirectoryNamePatterns,fileNamePatterns)
    }
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->Find(x,contentPatterns,exceptionDirectoryNamePatterns,fileNamePatterns) 
  |>Seq.toArray


( 
[ //sourceDirectories
@"D:\Workspace\SBIIMS"
//@"G:\temp\1"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CKWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CZYWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_FJPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_GHSWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_JYFWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHHYWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_XFPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_ZCLBWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_AC"
//@"D:\Workspace\SBIIMS\SBIIMS_APC"
//@"D:\Workspace\SBIIMS\SBIIMS_FK"
//@"D:\Workspace\SBIIMS\SBIIMS_VC"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
],  
[  //contentPatterns //内容模式，对应xml加入"$"行结束符后可能不正确即使要加，前面最好使用\s*,通常才能匹配
//@"DA_[A-Z_]+\.INS"
//"""this\.QueryErrorProcess"""
//"""this\.QuerySingleErrorProcess"""
//"""this\.GenericErrorProcess"""
//"""open\s+WX\.Data\.BusinessBase"""
//"""open\s+WX\.Data\s*\n\s*open\s+WX\.Data\.BusinessEntities"""
///"""\s*open\s+WX\.Data\.BusinessBase\s*\n\s*[a-zA-Z\.\n\s]*open\s+WX\.Data\.BusinessBase\s*"""
//"""this\.AttachError"""
//"""\>\(ExecuteDateTime\=DateTime\.Now\)"""
//"""new\s+BD_ExecuteContent"""
//"""^(\s{8})\|\s*_\s*\-\>\s*\(\s*\)(\s*)\n(\s{8})result\.ResultLength\s*\<\-\s*sb\.SaveChanges\s*\(\)\s*\n\s{8}result(\s*)$"""
//"""match\s+ServerCache\.Get"""
///"""^\s+do\s*\n\s*this\.Initialize\s*\(\s*\)\s*\n\s*do\s*\n\s*this\.Initialize\s*\(\s*\)"""
"""^\s+do\s*\n\s*this\.Initialize\s*\(\s*\)\s*\n\s*this\.Initialize\s*\(\s*\)"""
],
[ //例外的文件目录Pattern
//@"WX\.Data\.ServiceContracts[a-zA-Z\.]+"  
],
[  //只查找符合名称模式的文件
//@"""^DA_[a-zA-Z_]+\.fs"""
//"""^DA_[a-zA-Z_]+_QueryAdvance\.fs$"""
//"""^DA_[a-zA-Z_]+_BusinessAdvance\.fs$"""
//"""^DA_[a-zA-Z_]+\.fs$"""
"""^FVM_[a-zA-Z_]+_Advance\.fs$"""//
//"""^[a-zA-Z_]+\.fs$"""
]
)
|>FindWithRegex
|>Seq.iter (fun a->ObjectDumper.Write a)



( 
[ //sourceDirectories
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\TempWorkspace\c"
],  
[  //contentPatterns //内容模式，对应xml加入"$"行结束符后可能不正确即使要加，前面最好使用\s*,通常才能匹配
@"DA_[A-Z_]+\.INS"
@"\.T_[A-Z_]+"
@"\""T_[A-Z_]+"
@"BD_T_[A-Z_]+"
],
[ //例外的文件目录Pattern
//@"WX\.Data\.ServiceContracts[a-zA-Z\.]+"  
//@"WX\.Data\.WcfService[a-zA-Z\.]+"       
],
[  //只替换符合名称模式的文件
//"[a-zA-Z_]+\.fs"     
@"^DA_[a-zA-Z_]+\.fs"
@"^BD_[a-zA-Z_]+\.fs"
//@"^WX\.Data\.DataAccessAdvance[X]*\.[a-zA-Z\.]+\.fs$"  //WX.Data.DataAccessAdvance.JXC.JBXX.SPWH
//@"^WX\.Data\.FViewModel\.[a-zA-Z\.]+\.[cf]sproj$"  
]
)
|>FindWithRegex
|>Seq.map (fun a->
    match a with
    | StartsWithIn ["DA_"] _ ->a.Replace("DA_","T_").Replace(".INS",String.Empty)
    | StartsWithIn ["BD_T_"] _ ->a.Replace("BD_T_","T_")
    | StartsWithIn ["."] _->a.Replace(".",String.Empty)
    | StartsWithIn [@""""] _->a.Replace(".",String.Empty)
    | _ ->a
    )
|>Seq.distinct
|>Seq.iter (fun a->ObjectDumper.Write a)

#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
open Microsoft.FSharp.Data.TypeProviders
open System.Data.Linq
type SqlConnection = SqlDataConnection<ConnectionString = @"Data Source=192.168.2.199;Initial Catalog=SBIIMS_APC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True">
type DataTypes=SqlConnection.ServiceTypes
let now=DateTime.Now
let db = SqlConnection.GetDataContext()

let entryDirectory=DirectoryInfo @"D:\Workspace\SBIIMS\SBIIMS_JXC"
for ns in entryDirectory.GetDirectories() do
  if ns.Name.StartsWith ("JXC_") then
    match new DataTypes.T_AU() with
    | x ->
        x.C_CJRQ<-now
        x.C_GXRQ<-now
        x.C_ID<-Guid.NewGuid()
        x.C_DBID<-Guid.Parse "bdb15736-85ff-443c-b15a-234000d7d15a"
        x.C_MC<-ns.Name
        x.C_YXJB<-1uy
        x.C_GXBZ<-false
        db.T_AU.InsertOnSubmit x
        db.DataContext.SubmitChanges()
        //-----------------------------------
        ( 
        //sourceDirectories
        [|ns.FullName|]
        (**
        [
        @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
        ]
        *)
        ,  
        [  //contentPatterns //内容模式，对应xml加入"$"行结束符后可能不正确即使要加，前面最好使用\s*,通常才能匹配
        @"DA_[A-Z_]+\.INS"
        @"\.T_[A-Z_]+"
        @"\""T_[A-Z_]+"
        @"BD_T_[A-Z_]+"
        ],
        [ //例外的文件目录Pattern
        //@"WX\.Data\.ServiceContracts[a-zA-Z\.]+"  
        //@"WX\.Data\.WcfService[a-zA-Z\.]+"       
        ],
        [  //只替换符合名称模式的文件
        //"[a-zA-Z_]+\.fs"     
        @"^DA_[a-zA-Z_]+\.fs"
        @"^BD_[a-zA-Z_]+\.fs"
        //@"^WX\.Data\.DataAccessAdvance[X]*\.[a-zA-Z\.]+\.fs$"  //WX.Data.DataAccessAdvance.JXC.JBXX.SPWH
        //@"^WX\.Data\.FViewModel\.[a-zA-Z\.]+\.[cf]sproj$"  
        ]
        )
        |>FindWithRegex
        |>Seq.map (fun a->
            match a with
            | StartsWithIn ["DA_"] _ ->a.Replace("DA_","T_").Replace(".INS",String.Empty), false
            | StartsWithIn ["BD_T_"] _ ->a.Replace("BD_T_","T_"), true
            | StartsWithIn ["."] _->a.Replace(".",String.Empty), true
            | StartsWithIn [@""""] _->a.Replace(".",String.Empty),true
            | _ ->a,true
            )
        |>fun r ->
            match r|>Seq.filter (fun (_, a)->not a)|>Seq.distinctBy (fun (a, _)->a) with
            | x ->
                match r|>Seq.filter (fun (a,b)->b && x|>Seq.exists (fun (c,_)->c=a)|>not)|>Seq.distinctBy (fun (a,_)->a) with
                | y ->y|>Seq.append x
        //|>ObjectDumper.Write 
        |>Seq.iter (fun (a,b)->
            match new DataTypes.T_AUDX() with
            | y ->
                y.C_CJRQ<-now
                y.C_GXRQ<-now
                y.C_AUID<-x.C_ID
                y.C_DXID<-a
                y.C_DXLX<-1uy
                y.C_DXZ<-"dbo"
                y.C_JCXBZ<-b
                db.T_AUDX.InsertOnSubmit y
            )
        db.DataContext.SubmitChanges()


db.T_AUDX
|>Seq.filter (fun a->a.C_AUID=Guid.Parse "4867c995-a6bf-460b-b89a-7628ce8726eb")
|>ObjectDumper.Write

let x:obj []=[|null|]

x.Length


//*********************************************************************************************
//---------------------------------------------------------------------------------------------
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改文件名和文件内容, ***只对平面文件, 实现一个文件多次替换
let FileModifyCopy (sourceDirectoryPath:string,targetPath:string,modifyStrOldNewGroups:(string*string) list)=
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec FileModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,modifyStrOldNewGroups:(string*string) list)=       
    for b in sourceDirectory.GetFiles() do
      match File.ReadFile b.FullName with
      | x ->
          match 
            modifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) b.Name,
            modifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->r.Replace(u,v)) x with
          | y,z ->
              Path.Combine(targetDirectory.FullName,y)
              |>File.WriteFileCreateOnly z 
    for a in sourceDirectory.GetDirectories() do
      match targetDirectory.CreateSubdirectory(a.Name)  with
      | x ->  FileModifyCopy(a,x,modifyStrOldNewGroups)
  FileModifyCopy(sourceDirectory,targetDirectory,modifyStrOldNewGroups) 

(
@"D:\TempWorkspace\a",    //sourceDirectory
@"D:\TempWorkspace\a1",   //targetDirectory
[ //modifyStrOldNewGroups
"_SPCF","_SPKB"
])
|>FileModifyCopy

//---------------------------------------------------------------------------------------------
//使用正则表达式
//Windwos文件系统 --拷贝并修改文件名及内容, ********修改文件名和文件内容, ***只对平面文件, 实现一个文件多次替换
let FileModifyCopyWithRegex (sourceDirectoryPath:string,targetPath:string,modifyStrOldNewGroups:(string*string) list)=
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory= 
    match new DirectoryInfo(targetPath) with
    | x ->
        if x.Exists |>not then x.Create()
        x
  let rec FileModifyCopy (sourceDirectory:DirectoryInfo,targetDirectory:DirectoryInfo,modifyStrOldNewGroups:(string*string) list)=       
    for b in sourceDirectory.GetFiles() do
      match File.ReadFile b.FullName with
      | x ->
          match 
            modifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) b.Name,
            modifyStrOldNewGroups|>Seq.fold (fun (r:string) (u,v) ->Regex.Replace(r,u,v)) x with
          | y,z ->
              Path.Combine(targetDirectory.FullName,y)
              |>File.WriteFileCreateOnly z 
    for a in sourceDirectory.GetDirectories() do
      match targetDirectory.CreateSubdirectory(a.Name)  with
      | x ->  FileModifyCopy(a,x,modifyStrOldNewGroups)
  FileModifyCopy(sourceDirectory,targetDirectory,modifyStrOldNewGroups) 

(
@"D:\TempWorkspace\a",    //sourceDirectory
@"D:\TempWorkspace\a1",   //targetDirectory
[ //modifyStrOldNewGroups
"\s*(\w+)\s*","\1"
])
|>FileModifyCopyWithRegex


(*Wrong
|>Seq.mapi (fun i (a,b)->
    async{
      
      return FileModifyCopy (sourceDirectoryPath+string (i+1)) (targetPath+string (i+2))  a b 
    }
    )
|>Async.Parallel
|>Async.RunSynchronously
*)
//-------------------------------------------------------------------------------

let GetFileNames =
  let mutable directoryPath= @"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_DH\WX.Data.FViewModel.Frame.DH"
  match directoryPath with
  | EndsWithIn [@"\"; "/"]  x->directoryPath<-x.Remove(x.Length-1)
  | _ ->()
  let  rootDirectoryInfo=DirectoryInfo directoryPath
  let rec collectFileInfos (directoryInfo:DirectoryInfo)=
    seq{
      for b in directoryInfo.GetFiles() do
        match b.Extension.ToLower(),b.Name.Remove(b.Name.Length-3) with
        | ".fs", x when x.EndsWith("_Advance")|>not ->
            yield String.Format( @"
 ""{0}"""
              ,x).TrimStart() 
        | _ ->()
      for a in directoryInfo.GetDirectories() do
        yield! collectFileInfos a
      }
  rootDirectoryInfo
  |>collectFileInfos
  |>ObjectDumper.Write 

let x=new FileInfo( @"D:\TempWorkspace\a\WX.Data.BusinessDataEntitiesAdvance.JXC.ZHCX\BD_V_DJ_KH_XFSPHZ_AGHS.fs")
x.Name

//-------------------------------------------------------------------------------
//删除指定扩展名的文件
//DeleteFilesByExtension
let DeleteFilesByExtension (sourceDirectoryPath:string,fileExtensions:string seq) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let rec DeleteFilesByExtension (sourceDirectory:DirectoryInfo,fileExtensions:string seq)=       
    for b in sourceDirectory.GetFiles() do
      match b.Extension with
      | EqualsIn fileExtensions _ ->File.Delete b.FullName
      | _ ->()
    for a in sourceDirectory.GetDirectories() do
      DeleteFilesByExtension(a,fileExtensions)
  DeleteFilesByExtension(sourceDirectory,fileExtensions) 

(
@"D:\TempWorkspace\c",
[
".vssscc"
".vspscc"
]
)
|>DeleteFilesByExtension


//-------------------------------------------------------------------------------
//删除指定文件
//DeleteFilesByExtension
let DeleteFilesByKeywords (sourceDirectoryPath:string,keywords:string seq) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let rec DeleteFilesByKeywords (sourceDirectory:DirectoryInfo,keywords:string seq)=       
    for b in sourceDirectory.GetFiles() do
      match b.Name with
      | ContainsIn keywords _ ->File.Delete b.FullName
      | _ ->()
    for a in sourceDirectory.GetDirectories() do
      DeleteFilesByKeywords(a,keywords)
  DeleteFilesByKeywords(sourceDirectory,keywords) 

(
@"D:\Workspace\SBIIMS\SBIIMS_JXC",
[
"KCGL.SPCF.fsproj"
]
)
|>DeleteFilesByKeywords

//-------------------------------------------------------------------------------
//删除指定目录下的指定文件,通用
let DeleteFilesGeneric (sourceDirectoryPaths:string seq,fileDirectoryNamePatterns:string seq,fileNamePatterns:string seq) =
  let rec DeleteFiles (sourceDirectories:DirectoryInfo seq,fileDirectoryNamePatterns:string seq,fileNamePatterns:string seq)=       
    seq{
      for a in sourceDirectories do 
        match fileDirectoryNamePatterns|>Regex.IsMatchIn a.Name with
        | true ->
            for b in a.GetFiles() do
              match fileNamePatterns|>Regex.IsMatchIn b.Name with 
              | true ->
                  yield b.FullName
                  File.Delete b.FullName
              | _ ->()
        | _ ->()
        yield! DeleteFiles (a.GetDirectories(), fileDirectoryNamePatterns, fileNamePatterns)
    }
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->DeleteFiles(x,fileDirectoryNamePatterns,fileNamePatterns) 
  |>Seq.toArray

(
[ //入库路径
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
],
[ //文件所属目录名模式
@"^WX\.Data\.IDataAccessAdvance\.[A-Z]+\.[A-Z]+\.[A-Z]+$"
],
[ //文件名模式
@"Design\.txt"
]
)
|>DeleteFilesGeneric


//-------------------------------------------------------------------------------
//获取所有文件的路径全名
let GetFileFullNames (sourceDirectoryPath:string) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let rec GetFileFullNames (sourceDirectory:DirectoryInfo)=    
    seq{   
        for b in sourceDirectory.GetFiles() do
          yield b.FullName
        for a in sourceDirectory.GetDirectories() do
          yield! GetFileFullNames(a)
    }
  GetFileFullNames(sourceDirectory) 

"D:\Workspace\SBIIMS"
|>GetFileFullNames
|>Seq.maxBy (fun a->a.Length)
|>fun a->a.Length

//-------------------------------------------------------------------------------
//获取子目录

//"D:\Workspace\SBIIMS\SBIIMS_JXC"
"D:\Workspace\SBIIMS\SBIIMS_AC"
|>fun a->DirectoryInfo  a
|>fun a->a.GetDirectories()
|>Seq.filter (fun a->a.Name.StartsWith "WX.Data.View.ViewModelTemplate.")
|>Seq.map (fun a->a.Name.Replace("WX.Data.View.ViewModelTemplate.",""))
|>Seq.map (fun a->match a.Split([|'.'|]) with x ->x.[0],x.[1])
|>Seq.iter (fun (a,b)->ObjectDumper.Write  (String.Format(@"""{0}"",""{1}""",a,b )))


//将指定的目录组移到创建的子目录
match "D:\Workspace\SBIIMS\SBIIMS_JXC" with
| x ->
    match DirectoryInfo  x with
    | y ->
        y.GetDirectories()
        |>Seq.filter (fun a->Regex.IsMatch(a.Name,@".JXC.[A-Z]+.[A-Z]+$"))
        |>Seq.groupBy (fun a->a.Name.Remove(0,a.Name.IndexOf(".JXC.")))
        |>Seq.iter (fun (a,b)->
            match a.Split([|'.'|],StringSplitOptions.RemoveEmptyEntries) with 
            | y when y.Length>2 ->String.Format("{0}_{1}_{2}",y.[0],y.[1],y.[2])
            | y when y.Length>1->String.Format("{0}_{1}",y.[0],y.[1]) 
            | _ ->"Tem"
            |>y.CreateSubdirectory 
            |>fun a->
                for m in b do 
                  Path.Combine(a.FullName,m.Name)
                  |>m.MoveTo
            )

//----------------------------------------------------------------------------
//将指定的文件移入到指定的文件夹
let sourceDirectory=DirectoryInfo @"D:\Workspace\SBIIMS\SBIIMS_JXC"
let sourceDirectoryNameParts=["WX.Data.BusinessQueryEntitiesAdvance";"WX.Data.BusinessQueryEntitiesClientAdvance";"WX.Data.BusinessDataEntitiesAdvance"]
let targetDirectoryNamePart="WX.Data.BusinessEntitiesAdvance"
let rec GetFilesAndDirectories (sourceDirectory:DirectoryInfo)=    
  seq{   
      for b in sourceDirectory.GetFiles() do
        match Regex.IsMatch(b.Name,@"^[a-zA-Z_]+\.(fs|txt)$"), sourceDirectory.Name with
        | true, StartsWithIn sourceDirectoryNameParts xa  ->
            if (xa|>Seq.filter(fun a->a='.')|>Seq.length)>4 then //AC,Frame,Link模块 大于3, JXC需要大于4 !!!!!!!!
              yield b,sourceDirectory,
                match sourceDirectoryNameParts|>Seq.fold (fun (r:string) a->r.Replace (a,targetDirectoryNamePart))  sourceDirectory.Name with
                | x ->Path.Combine(sourceDirectory.Parent.FullName,x)
        | _ ->()
      for a in sourceDirectory.GetDirectories() do
        yield! GetFilesAndDirectories(a)
  }
match GetFilesAndDirectories(sourceDirectory)|>Seq.toArray with //toArray是必须的，这样才能删除目录
| x ->
    for (a,b,c) in x do
      match 
        (match a.Extension,b.Name with
        | EqualsIn [".txt"] _, StartsWithIn ["WX.Data.BusinessQueryEntitiesAdvance"] _ ->"BQ_CodeAutomation.txt"
        | EqualsIn [".txt"] _, StartsWithIn ["WX.Data.BusinessDataEntitiesAdvance"] _ ->"BD_CodeAutomation.txt"
        | _ ->a.Name)  with
      | y ->
          match Path.Combine(c,y) with
          | z ->a.CopyTo(z,true) |>ignore

//删除目录,不通用！！！！， 针对JXC.KCGL.SPCF样式的目录名称
let rec GetDirectories (sourceDirectory:DirectoryInfo)=    
  seq{   
      match sourceDirectory.Name with
      | StartsWithIn sourceDirectoryNameParts x ->
          if (x|>Seq.filter(fun a->a='.')|>Seq.length)>4   then   //AC,Frame,Link模块 大于3, JXC需要大于4 !!!!!!!!
            yield sourceDirectory
      | _ ->() 
      for a in sourceDirectory.GetDirectories() do
        yield! GetDirectories(a)
  }
GetDirectories(sourceDirectory) 
|>Seq.toArray
|>Seq.iter (fun a->a.Delete(true))

"WX.Data.BusinessQueryEntitiesAdvance.AC"|>Seq.filter(fun a->a='.')|>Seq.length

//----------------------------------------------------------------------------
//收集所指定的文件到指定的文件夹
//主要用于收集所有的WCF文件*svc及其配置文件

let CollectFilesToTargetWithRegex (sourceDirectoryPath:string,targetDirectoryPath,fileNamePatterns:string list) =
  let sourceDirectory=DirectoryInfo sourceDirectoryPath
  let targetDirectory=DirectoryInfo targetDirectoryPath
  let rec GetFileInfo (sourceDirectory:DirectoryInfo)=    
    seq{   
        for b in sourceDirectory.GetFiles() do
          match fileNamePatterns|>Regex.IsMatchIn b.Name,sourceDirectory.FullName<>targetDirectory.FullName with //不能直接比较两个实例sourceDirectory，targetDirectory
          | true,true -> 
              yield b
          | _ ->()
        for a in sourceDirectory.GetDirectories() do
          yield! GetFileInfo(a)
    }
  GetFileInfo(sourceDirectory) 
  |>Seq.distinctBy (fun a->a.Name)
  |>Seq.iter (fun a->
      match Path.Combine(targetDirectoryPath,a.Name) with
      | x ->a.CopyTo (x,true) |>ignore
      )

(
"D:\Workspace\SBIIMS", //sourceDirectoryPath
"D:\Workspace\SBIIMS\SBIIMS_Integration\WX.Data.WcfService.WebIISHost", //targetDirectoryPath
[
@"^[a-zA-Z_]+\.svc$"
@"^WS_[a-zA-Z_]+_WebConfig\.txt$"
]
)
|>CollectFilesToTargetWithRegex

//----------------------------------------------------------------------------
//收集所指定目录及指定文件特征的所有文件
//主要用于收集WCF服务的所所有配置文件,然后通过Xml的XQuery等组合这些配置信息
let CollectFilesWithRegex (sourceDirectoryPaths:string seq,fileNamePatterns:string seq) =
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


(
[  //sourceDirectoryPaths
@"D:\Workspace\SBIIMS\SBIIMS_AC"
@"D:\Workspace\SBIIMS\SBIIMS_Frame"
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
@"D:\Workspace\SBIIMS\SBIIMS_Link"
],
[
@"^WS_[a-zA-Z_]+_WebConfig\.txt$"
]
)
|>CollectFilesWithRegex

//-------------------------------------------------------------------

