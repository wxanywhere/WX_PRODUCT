namespace WX.Data.CodeAutomation
open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open System.Diagnostics
open FSharp.Collections.ParallelSeq
open Microsoft.Build.Evaluation
open WX.Data
open WX.Data.Helper

type MSBuild()=
  let build (projectFilePath:string,codeLayerName:string,projectCollection:ProjectCollection)=
    try
      let tr = new TextWriterTraceListener(Console.Out);
      Debug.Listeners.Add(tr)|>ignore
      match projectCollection.LoadProject(projectFilePath) with
      | NotNull z -> 
          z.IsBuildEnabled<-true
          z.DisableMarkDirty<-false
          z.SkipEvaluation<-false
          match z.CreateProjectInstance() with  //也可在不创建项目实例的情况下直接编译，创建实例只是为了脱离对项目文件的依赖', //z.CreateProjectInstance().DeepCopy()也是正确的，但不适合于批量项目的编译
          | w ->
              match w.Build() with
              | false ->
                  Debug.Unindent()
                  Debug.Flush()
                  match new StackTrace(true) with
                  | u ->
                      for n in u.GetFrames() do
                        Console.WriteLine("Filename: {0} Method: {1} Line: {2} Column: {3}",n.GetFileName(),n.GetMethod(), n.GetFileLineNumber(),n.GetFileColumnNumber())
                  failwith <| String.Format ("编译项目失败---[{0}]", projectFilePath)
              | _ ->ObjectDumper.Write (String.Format ("{0}{1}{2}",codeLayerName, [for n in 0..40-codeLayerName.Length->"-"]|>Seq.fold (fun acc a->acc+a) String.Empty,projectFilePath))
              (*正确，但不能自动判断项目文件是否已被更改(IsDirty)，需要强制设置(MarkDirty())，但MarkDirty()后，每次编译的dll组件都被更新，这意味着其文件校验码(Md5)每次编译都被会更新
              z.MarkDirty() //需要额外判断项目文件是否被改变
              match z.IsDirty with
              | true ->
                  match z.Build() with
                  | false ->failwith <| String.Format ("编译项目失败---[{0}]", a.FullName)
                  | _ ->ObjectDumper.Write <| String.Format ("{0}{1}{2}",nb, [for n in 0..40-nb.Length->"-"]|>Seq.fold (fun acc a->acc+a) String.Empty,a.FullName)
              | _ ->ObjectDumper.Write <| String.Format ("{0}{1}It's update to date! {2}",nb, [for n in 0..40-nb.Length->"-"]|>Seq.fold (fun acc a->acc+a) String.Empty,a.FullName)  //failwith<| String.Format ("项目不允许编译---[{0}]", a.FullName)
              *)
      | _ ->failwith<| String.Format ("加载项目失败---[{0}]", projectFilePath)
    with e -> raise e

  let CollectProjectFileInfosByExtension (sourceDirectoryPaths:string seq,fileExtensions:string seq) =
    let rec GetFileInfos (sourceDirectories:DirectoryInfo seq)=    
      seq{   
          for n in sourceDirectories do
            for m in n.GetFiles() do
              match m.Extension with
              | EqualsIn fileExtensions _ ->yield m
              | _ ->()
            yield! GetFileInfos (n.GetDirectories())
      }
    match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
    | x ->GetFileInfos x

  let buildX sourceDirectoryPaths (projectFileNamePatternsOnOrder:(string*string) seq)=
    let pc=new ProjectCollection()
    pc.IsBuildEnabled<-true
    pc.DisableMarkDirty<-false
    pc.SkipEvaluation<-false
    //pc.DefaultToolsVersion<-"12.0" //MSbuild 的版本,在*.fsproj文件中
    let extensions=[|".fsproj";".csproj"|]
    try
      match CollectProjectFileInfosByExtension (sourceDirectoryPaths,extensions) with
      | x ->
          for na,nb in projectFileNamePatternsOnOrder do
            x
            |>Seq.filter (fun a->Regex.IsMatch (a.Name ,na))
            |>Seq.sortBy (fun a->a.Name|>Seq.filter (fun b->b='.')|>Seq.length) //先编译短后缀名称的项目，这样可以先编译GGXZ等项目
            |>Seq.iter (fun a->build (a.FullName,nb,pc))
    with e -> raise e

  static member INS=new MSBuild()
  member this.BuildAll (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.DataModel\.[a-zA-Z\.]+\.csproj$","DataModel"  //"WX.Data.DataModel"
      @"^WX\.Data\.BusinessEntities\.[a-zA-Z\.]+\.fsproj$","BusinessEntities" //"WX.Data.BusinessEntities"
      @"^WX\.Data\.DatabaseDictionary\.[a-zA-Z\.]+\.fsproj$","DatabaseDictionary" //"WX.Data.DatabaseDictionary"
      @"^WX\.Data\.IDataAccess\.[a-zA-Z\.]+\.fsproj$","IDataAccess" //"WX.Data.IDataAccess"
      @"^WX\.Data\.DataAccess\.[a-zA-Z\.]+\.fsproj$","DataAccess" //"WX.Data.DataAccess"
      @"^WX\.Data\.BusinessEntitiesAdvance\.[a-zA-Z\.]+\.fsproj$","BusinessEntitiesAdvance"  //"WX.Data.BusinessEntitiesAdvance"
      @"^WX\.Data\.BusinessEntitiesAdvanceX\.[a-zA-Z\.]+\.fsproj$","BusinessEntitiesAdvanceX" //"WX.Data.BusinessEntitiesAdvanceX"
      @"^WX\.Data\.IDataAccessAdvanceX\.[a-zA-Z\.]+\.fsproj$","IDataAccessAdvanceX" //"WX.Data.IDataAccessAdvanceX"
      @"^WX\.Data\.DataAccessAdvanceX\.[a-zA-Z\.]+\.fsproj$","DataAccessAdvanceX" //"WX.Data.DataAccessAdvanceX"
      @"^WX\.Data\.ServerCaching\.[a-zA-Z\.]+\.fsproj$","ServerCaching" //"WX.Data.ServerCaching"
      @"^WX\.Data\.IDataAccessAdvance\.[a-zA-Z\.]+\.fsproj$","IDataAccessAdvance" //"WX.Data.IDataAccessAdvance"
      @"^WX\.Data\.DataAccessAdvance\.[a-zA-Z\.]+\.fsproj$","DataAccessAdvance" //"WX.Data.DataAccessAdvance"
      @"^WX\.Data\.BusinessLogicAdvance\.[a-zA-Z\.]+\.fsproj$","BusinessLogicAdvance" //"WX.Data.BusinessLogicAdvance"
      @"^WX\.Data\.ServiceContractsAdvance\.[a-zA-Z\.]+\.fsproj$","ServiceContractsAdvance" //"WX.Data.ServiceContractsAdvance"
      @"^WX\.Data\.WcfServiceAdvance\.(?!WebIISHost)[a-zA-Z\.]+\.fsproj$","WcfServiceAdvance" //"WX.Data.WcfServiceAdvance"  
      //@"^WX\.Data\.WcfServiceAdvance\.WebIISHost\.[a-zA-Z\.]+\.[cf]sproj$","WebIISHost" //"WX.Data.WcfServiceAdvance.WebIISHost"
      @"^WX\.Data\.ClientChannelAdvance\.(?!FromAzure)[a-zA-Z\.]+\.fsproj$","ClientChannelAdvance" //"WX.Data.ClientChannelAdvance";"WX.Data.ClientChannelAdvance.FromAzure";"WX.Data.ClientChannelAdvance.FromNative";"WX.Data.ClientChannelAdvance.FromServer"
      @"^WX\.Data\.Caching\.[a-zA-Z\.]+\.fsproj$","Caching" //"WX.Data.Caching"
      @"^WX\.Data\.FViewModel\.[a-zA-Z\.]+\.fsproj$","FViewModel" //"WX.Data.FViewModel"
      @"^WX\.Data\.FViewModelAdvance\.[a-zA-Z\.]+\.fsproj$","FViewModelAdvance" //"WX.Data.FViewModelAdvance"
      @"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$","View" //WX.Data.View
      @"^WX\.Data\.View\.ViewModelTemplate\.[a-zA-Z\.]+\.[cf]sproj$","ViewModelTemplate" //"WX.Data.View.ViewModelTemplate"
      @"^WX\.Data\.View\.ViewModelTemplateAdvance\.[a-zA-Z\.]+\.[cf]sproj$","ViewModelTemplateAdvance"  //"WX.Data.View.ViewModelTemplateAdvance"
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write e

  member this.BuildAll (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildAll
    with
    | e ->ObjectDumper.Write e

  member this.BuildForDataAccessAutomationLayers (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.DataModel\.[a-zA-Z\.]+\.csproj$","DataModel"  //"WX.Data.DataModel"
      @"^WX\.Data\.BusinessEntities\.[a-zA-Z\.]+\.fsproj$","BusinessEntities" //"WX.Data.BusinessEntities"
      @"^WX\.Data\.DatabaseDictionary\.[a-zA-Z\.]+\.fsproj$","DatabaseDictionary" //"WX.Data.DatabaseDictionary"
      @"^WX\.Data\.IDataAccess\.[a-zA-Z\.]+\.fsproj$","IDataAccess" //"WX.Data.IDataAccess"
      @"^WX\.Data\.DataAccess\.[a-zA-Z\.]+\.fsproj$","DataAccess" //"WX.Data.DataAccess"
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write e

  member this.BuildForDataAccessAutomationLayers (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildForDataAccessAutomationLayers
    with
    | e ->ObjectDumper.Write e

  member this.BuildForBusinessExchangeLayers (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.BusinessEntitiesAdvance\.[a-zA-Z\.]+\.fsproj$","BusinessEntitiesAdvance"  //"WX.Data.BusinessEntitiesAdvance"
      @"^WX\.Data\.IDataAccessAdvance\.[a-zA-Z\.]+\.fsproj$","IDataAccessAdvance" //"WX.Data.IDataAccessAdvance"
      @"^WX\.Data\.DataAccessAdvance\.[a-zA-Z\.]+\.fsproj$","DataAccessAdvance" //"WX.Data.DataAccessAdvance"
      @"^WX\.Data\.BusinessLogicAdvance\.[a-zA-Z\.]+\.fsproj$","BusinessLogicAdvance" //"WX.Data.BusinessLogicAdvance"
      @"^WX\.Data\.ServiceContractsAdvance\.[a-zA-Z\.]+\.fsproj$","ServiceContractsAdvance" //"WX.Data.ServiceContractsAdvance"
      @"^WX\.Data\.WcfServiceAdvance\.(?!WebIISHost)[a-zA-Z\.]+\.fsproj$","WcfServiceAdvance" //"WX.Data.WcfServiceAdvance"  
      //@"^WX\.Data\.WcfServiceAdvance\.WebIISHost\.[a-zA-Z\.]+\.[cf]sproj$","WebIISHost" //"WX.Data.WcfServiceAdvance.WebIISHost"
      @"^WX\.Data\.ClientChannelAdvance\.(?!FromAzure)[a-zA-Z\.]+\.fsproj$","ClientChannelAdvance" //"WX.Data.ClientChannelAdvance";"WX.Data.ClientChannelAdvance.FromAzure";"WX.Data.ClientChannelAdvance.FromNative";"WX.Data.ClientChannelAdvance.FromServer"
      @"^WX\.Data\.Caching\.[a-zA-Z\.]+\.fsproj$","Caching" //"WX.Data.Caching"
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write e

  member this.BuildForBusinessExchangeLayers (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildForBusinessExchangeLayers
    with
    | e ->ObjectDumper.Write e

  member this.BuildForServerCacheProcessLayers (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.BusinessEntitiesAdvance\.[a-zA-Z\.]+\.fsproj$","BusinessEntitiesAdvance"  //"WX.Data.BusinessEntitiesAdvance"
      @"^WX\.Data\.BusinessEntitiesAdvanceX\.[a-zA-Z\.]+\.fsproj$","BusinessEntitiesAdvanceX" //"WX.Data.BusinessEntitiesAdvanceX"
      @"^WX\.Data\.IDataAccessAdvanceX\.[a-zA-Z\.]+\.fsproj$","IDataAccessAdvanceX" //"WX.Data.IDataAccessAdvanceX"
      @"^WX\.Data\.DataAccessAdvanceX\.[a-zA-Z\.]+\.fsproj$","DataAccessAdvanceX" //"WX.Data.DataAccessAdvanceX"
      @"^WX\.Data\.ServerCaching\.[a-zA-Z\.]+\.fsproj$","ServerCaching" //"WX.Data.ServerCaching"
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write e

  member this.BuildForServerCacheProcessLayers (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildForServerCacheProcessLayers
    with
    | e ->ObjectDumper.Write e

  member this.BuildForViewModelLayers (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.FViewModel\.[a-zA-Z\.]+\.fsproj$","FViewModel" //"WX.Data.FViewModel"
      @"^WX\.Data\.FViewModelAdvance\.[a-zA-Z\.]+\.fsproj$","FViewModelAdvance" //"WX.Data.FViewModelAdvance"
      @"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$","View" //WX.Data.View
      @"^WX\.Data\.View\.ViewModelTemplate\.[a-zA-Z\.]+\.[cf]sproj$","ViewModelTemplate" //"WX.Data.View.ViewModelTemplate"
      @"^WX\.Data\.View\.ViewModelTemplateAdvance\.[a-zA-Z\.]+\.[cf]sproj$","ViewModelTemplateAdvance"  //"WX.Data.View.ViewModelTemplateAdvance"
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write (e,2)

  member this.BuildForViewModelLayers (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildForViewModelLayers
    with
    | e ->ObjectDumper.Write e

  member this.BuildForViewLayers (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$","View" //WX.Data.View
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write (e,2)

  member this.BuildForViewLayers (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildForViewLayers
    with
    | e ->ObjectDumper.Write e

  member this.BuildForDataAccessLayers (sourceDirectoryPaths:string seq)=
    try
      [|
      @"^WX\.Data\.DataAccessAdvance\.[a-zA-Z\.]+\.fsproj$","DataAccessAdvance" //"WX.Data.DataAccessAdvance"
      |]
      |>buildX sourceDirectoryPaths
    with
    | e ->ObjectDumper.Write e

  member this.BuildForDataAccessLayers (sourceDirectoryEntryPath:string, dictionaryNamePaterns:string seq)=
    try
      (DirectoryInfo sourceDirectoryEntryPath).GetDirectories()
      |>Seq.choose (fun a->
          match dictionaryNamePaterns|>Regex.IsMatchIn a.Name with
          | true ->Some a.FullName
          | _ ->None
          )
      |>fun r->
          r
          |>this.BuildForDataAccessLayers
    with
    | e ->ObjectDumper.Write e

  member this.Build (projectFilePaths:string seq)=
    try
      let pc=new ProjectCollection()
      pc.IsBuildEnabled<-true
      pc.DisableMarkDirty<-false
      pc.SkipEvaluation<-false
      projectFilePaths|>Seq.iteri (fun i a->build (a,string i,pc))
    with
    | e ->ObjectDumper.Write e