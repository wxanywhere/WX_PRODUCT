namespace WX.Data.CodeAutomation
open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceServerCacheProcessLayerCoding=

  (*
//调用示例
@"
  abstract AuditKCGL_SPCF:BD_TV_KCGL_SPCF_DJ_Advance[] ->BD_ExecuteResult
"
|>AdvanceServerCacheProcessLayerCoding.GetDataAccessCodeText
|>Clipboard.SetText
  *)
  static member GetDataAccessCodeText (systemAlias:string,interfaceMemberName:string)=
    try
    let sb=new StringBuilder()
    match interfaceMemberName with
    | x ->
        match Regex.Split(x.Trim(),@"\s*\n\s*",RegexOptions.Multiline) with
        | y ->
            seq{
              for c in y do
                if Regex.Split(c,@"->").Length>2 then failwith (String.Format(@"The member can't use with currying parameters of ""{0}"".",c))
                match Regex.Matches(c, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*\(*\s*([a-zA-Z_\s\<\>\[\]\:\?\*\#]+)\s*\)*\s*\-\>\s*([a-zA-Z_\s\<\>\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格， 已经考虑了可选参数 * ?Parameter
                | y when y.Count>0 && y.[0].Groups.Count>3 ->
                    yield 
                      y.[0].Groups.[1].Value, //方法名 b
                      seq{
                        match Regex.Split (y.[0].Groups.[2].Value,@"\s*\*\s*",RegexOptions.Singleline) with  //解析多个参数
                        | v ->
                            if v.Length>7 then failwith (String.Format(@"The  quantity of without name parameters is more than 7, it's ""{0}"".",c)) 
                            for i in 0..v.Length-1 do
                              match Regex.Split (v.[i],@"\s*\:\s*",RegexOptions.Singleline) with //参数名:参数类型
                              | u when u.Length=2 ->yield u.[0],u.[1]
                              | u when u.Length=1 -> //只有参数类型，无参数名的情况
                                  match u.[0].Replace(" ","") with //条件名称*条件类型, 并消除"[]"前的空格 c
                                  | y when y.StartsWith "BQ" ->yield BQParameterNames.[i],y
                                  | y when y.StartsWith "BD_ExecuteContent" ->yield BDExecuteContentParameterNames.[i],y
                                  | y when y.StartsWith "BD_ExecuteBase" ->yield BDExecuteBaseParameterNames.[i],y
                                  | y when y.StartsWith "BD_" && y.EndsWith @"]" ->yield BDEntityArrayParameterNames.[i],y
                                  | y when y.StartsWith "BD_" ->yield BDEntityParameterNames.[i],y
                                  | y when y.Equals "unit" ->()
                                  | y ->yield DefaultParameterNames.[i],y
                              | _ ->()
                      }
                      ,
                      match y.[0].Groups.[3].Value.Trim() with //结果类型 
                      | v ->
                          match Regex.Split (v,@"[\<\>]",RegexOptions.Singleline) with
                          | u ->
                              match u.Length with
                              | 1 ->v,v
                              | 3 ->v,u.[1].Replace(" ","") //消除"[]"前的空格
                              | _ -> failwith (String.Format(@"Result paragraph is not right, it's ""{0}"".",v))
                | _ ->()
              }
    |>fun a->
        for (b,c,(dx,dy)) in a do
          sb.AppendFormat (@"
    //获取
    member this.{0} ({1})=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<{2}[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        let sb=new {3}EntitiesAdvance()
        |>Seq.filter (fun a->
            true
            )
        |>Seq.pageTake pagingInfo
        |>Seq.map (fun a->
            match new {2}() with
            | x->
                x.MappingFrom a
                x)
        |>Seq.toResult result
      with 
      | e -> this.AttachError(e,-1,this,GetEntitiesAdvance,result)",
            b,
            //{1}
            (
            c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
            ),
            //{2}
            match dy.EndsWith "]" with true ->dy.Substring(0,dy.Length-2) | _ ->dy
            ,
            //{3}
            match b.ToLowerInvariant().Contains ("table"),systemAlias with
            | true, x ->x
            | _, x ->x.Insert(x.LastIndexOf('_')+1,"Query")   //需改进SBIIMS_JXC+Query的命名方法
            )|>ignore
          sb.AppendLine()|>ignore
    sb.ToString()
    with e -> ObjectDumper.Write e; raise e

  (*
//为所有接口组件生成代码文件
(
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS",  //接口文件目录名称
TwoPhaseGroup  //接口文件名成组片断段级别
)
|>AdvanceServerCacheProcessLayerCoding.GenerateCodingFilesFromComponents
|>ObjectDumper.Write
  *)
  static member GenerateCodingFilesFromComponents (systemAlias:string,baseDirectory:string,maxInterfaceGroupPhaseLevel:GroupPhaseLevel)=
    try
    let codeLayerNames=
      [
      "WX.Data.IDataAccessAdvanceX"
      "WX.Data.DataAccessAdvanceX"
      "WX.Data.ServerCaching"
      "WX.Data.Caching"
      ]
    seq{
      let interfaceComponentBaseName="WX.Data.IDataAccessAdvanceX" 
      let interfaceDirectories=
        match DirectoryInfo baseDirectory,interfaceComponentBaseName with
        | x,y->
            let rec GetInterfaceDirectories (sourceDirectory:DirectoryInfo)=    
              seq{   
                  match sourceDirectory with
                  | x when x.Name.StartsWith y ->yield x   
                  | _ ->()  
                  for m in sourceDirectory.GetDirectories() do
                    yield! GetInterfaceDirectories m
              }
            GetInterfaceDirectories x 
            |>Seq.filter (fun a->
                match a.Name.Replace(y,String.Empty) with
                | w ->
                    match codeLayerNames|>Seq.map (fun b->match b, w with EndsWithIn ["Caching"] _, EndsWithIn [".GGHC"] _ ->b+w.Replace(".GGHC",String.Empty) | _ -> b+w) with
                    | u ->
                        u|>Seq.forall (fun b->a.Parent.GetDirectories() |>Seq.exists (fun c->c.Name=b))
                )
      for m in interfaceDirectories do
        match m.Name.Replace(interfaceComponentBaseName,String.Empty) with
        | x ->
            match 
              m.GetFiles() 
              |>Seq.filter (fun a->Regex.IsMatch (a.Name,@"IDA_([a-zA-Z_]+)_[a-zA-Z]*Advance.[fsFS]"))
              with
            | y ->
                match y|>Seq.forall (fun a->a.IsReadOnly|>not) with
                | false ->yield String.Format("[Read Only!!!] --- {0}",m.FullName) //有符合条件的接口文件为只读时，其所属目录下的所有接口文件，将不执行代码生成，源代码控制时有用
                | _  ->
                    yield m.FullName  
                    y
                    |>Seq.map (fun a->a.Name.Replace(a.Extension,""))
                    |>Seq.toList 
                    |>fun a->
                        match a with
                        | _ when a.Length>0 |>not->()
                        | _ -> 
                            ObjectDumper.Write a
                            AdvanceServerCacheProcessLayerCoding.GenerateCodingFiles (x,systemAlias,m.FullName,m.Parent.FullName,a,maxInterfaceGroupPhaseLevel) |>ignore  
    }
    |>Seq.toArray
    with e -> ObjectDumper.Write e; raise e


  (*
//调用示例
(
".JXC.KCGL.SPCF", //组件装配名后缀
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF",  //接口文件目录名称
@"D:\Workspace\SBIIMS",  //目标基本路径或入口路径
[
"IDA_KCGL_SPCF_BusinessAdvance"
"IDA_KCGL_SPCF_QueryAdvance"
]
,
TwoPhaseGroup   //接口文件名成组片断段级别
)
|>AdvanceServerCacheProcessLayerCoding.GenerateCodingFiles
|>ObjectDumper.Write
  *)
  static member GenerateCodingFiles  (assemblySuffix:string,systemAlias:string,sourceDirectory:string,targetBaseDirectory:string,inferfaceTypeNames:string seq,maxInterfaceGroupPhaseLevel:GroupPhaseLevel) =
    let sb=new StringBuilder()
    let sbTem=new StringBuilder()
    try
    inferfaceTypeNames
    |>Seq.groupBy (fun a->
        match a.Remove(a.LastIndexOf('_')).Split([|'_'|],StringSplitOptions.RemoveEmptyEntries),maxInterfaceGroupPhaseLevel with
        | x,FourPhaseGroup ->
            match x.Length with
            | y when y>3->x.[0]+"_"+x.[1]+"_"+x.[2]+"_"+x.[3]+"_"
            | y when y>2->x.[0]+"_"+x.[1]+"_"+x.[2]+"_"
            | y when y>1->x.[0]+"_"+x.[1]+"_"
            | _ ->x.[0]+"_"
        | x, ThreePhaseGroup ->
            match x.Length with
            | y when y>2->x.[0]+"_"+x.[1]+"_"+x.[2]+"_"
            | y when y>1->x.[0]+"_"+x.[1]+"_"
            | _ ->x.[0]+"_"
        | x, TwoPhaseGroup ->
            match x.Length with
            | y when y>1->x.[0]+"_"+x.[1]+"_"
            | _ ->x.[0]+"_"
        | x, OnePhaseGroup ->x.[0]+"_")
    |>fun a ->
        seq{
        for (mx,my) in a do
          sb.Clear()|>ignore
          let ServiceTypeNameSuffix=
            match mx.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries),maxInterfaceGroupPhaseLevel with
            | x, FourPhaseGroup->
                match x.Length with
                | y when y>3 ->x.[1]+"_"+x.[2]+"_"+x.[3]+"_Advance" 
                | y when y>2 ->x.[1]+"_"+x.[2]+"_Advance" 
                | y when y>1 ->x.[1]+"_Advance" 
                | _ ->"Advance"
            | x, ThreePhaseGroup->
                match x.Length with
                | y when y>2 ->x.[1]+"_"+x.[2]+"_Advance" 
                | y when y>1 ->x.[1]+"_Advance" 
                | _ ->"Advance"
            | x, TwoPhaseGroup->
                match x.Length with
                | y when y>1 ->x.[1]+"_Advance" 
                | _ ->"Advance"
            | x, OnePhaseGroup->"Advance"
          match 
            systemAlias.Trim(), //系统别名
            systemAlias.Trim()+"_"+ServiceTypeNameSuffix.Trim(),  
            GetInterfaceTypeCodingInfo sourceDirectory my
            with
          | i,k,m ->
              sb.AppendFormat(@"
//--------------------------------------------------------------------------------
//Multi layer
(
""{0}"",  //组件装配名后缀
""{1}"",  //系统简称
@""{2}"",  //接口文件目录名称
@""{3}"",  //目标基本路径或入口路径
[
{4}  //接口类型名组
]
,
{5}  //接口文件名成组片断段级别
)
|>AdvanceServerCacheProcessLayerCoding.GenerateCodingFiles
|>ObjectDumper.Write
//--------------------------------------------------------------------------------",
                //{0}{1}{2}{3}
                assemblySuffix,systemAlias,sourceDirectory,targetBaseDirectory,
                //{4}
                (
                sbTem.Clear()|>ignore
                for typeName in inferfaceTypeNames do 
                  sbTem.AppendFormat(@"
""{0}""",
                    typeName
                    )|>ignore
                sbTem.ToString().TrimStart()
                ),
                //{5}
                maxInterfaceGroupPhaseLevel.ToString() //ToString()已被重载
                )|>ignore
              match Path.Combine(sourceDirectory,String.Format("{0}.txt","CodeAutomation")) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart()) //可覆盖
                  yield y
              sb.Clear()|>ignore
              sbTem.Clear()|>ignore
              //WX.Data.DataAccessAdvance... , 只能创建，不能覆盖
              for ((n,p),a) in m do    //((注释,数据访问类型名),其他)
                sb.Clear()|>ignore
                sb.AppendFormat( @"
namespace WX.Data.DataAccess
open System
open System.Data
open Microsoft.Practices.EnterpriseLibrary.Logging
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess

[<Sealed>]
type {0}=
  inherit DA_Base
  static member public INS = {0}() 
  private new () = {{inherit DA_Base()}}
  interface I{0} with",
                  //{0}
                  p)|>ignore
                for (b,c,(dx,dy)) in a do
                  sb.AppendFormat (@"
    //获取
    member this.{0} ({1})=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<{2}[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        let sb=new {3}EntitiesAdvance()
        |>Seq.filter (fun a->
            true
            )
        |>Seq.pageTake pagingInfo
        |>Seq.map (fun a->
            match new {2}() with
            | x->
                x.MappingFrom a
                x)
        |>Seq.toResult result
      with 
      | e -> this.AttachError(e,-1,this,GetEntitiesAdvance,result)",
                    b,
                    //{1}
                    (
                    c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    )
                    ,
                    //{2}
                    match dy.EndsWith "]" with 
                    | true ->dy.Substring(0,dy.Length-2) | _ ->dy
                    ,
                    //{3}
                    match p.ToLowerInvariant().Contains ("query") with
                    | true ->i.Insert(i.LastIndexOf('_')+1,"Query")   //需改进SBIIMS_JXC+Query的命名方法
                    | _ ->i
                    )|>ignore
                  sb.AppendLine()|>ignore
                match Path.Combine(targetBaseDirectory,CodeLayerPath.DataAccessAdvanceX assemblySuffix,String.Format("{0}.fs",p)) with
                | y ->
                    y|>File.WriteFileCreateOnly (sb.ToString().TrimStart()) //注意，数据访问层只能创建
                    yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ServerCaching... ServerCacheKeys
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data
open System

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ServerCacheKey={0}",
                //{0}
                if Seq.length m >0|>not then "()" else String.Empty)|>ignore
              for ((n,p),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  let SCK_{0} = ""SCK_{0}""",
                    match Regex.Match (b, @"^\s*Get([A-Z_]+)[T|V][a-z]+[\w\W]*$",RegexOptions.Singleline)  with
                    | y  when y.Groups.Count>1 ->y.Groups.[1].Value
                    | _ ->String.Empty
                    )|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.ServerCaching assemblySuffix,String.Format("{0}.fs","ServerCacheKeys")) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ServerCaching... ServerCacheRefreshAction
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ServerCaching
open System
open System.Collections
open System.Collections.ObjectModel
open FSharp.Collections.ParallelSeq
open Microsoft.Practices.EnterpriseLibrary.Caching
open Microsoft.Practices.EnterpriseLibrary.Caching.Expirations
open WX.Data
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServerCacheManager
open WX.Data.IDataAccess
open WX.Data.DataAccess{0}",
                //{0}
                String.Empty)|>ignore
              for ((n,p),a) in m do
                sb.AppendFormat ( @"
//==================================================================
//{0}",n
                )|>ignore
                sb.AppendLine ()|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
[<Serializable>]
type private {0}CacheRefreshAction= 
  static member INS=new {0}CacheRefreshAction()
  private new ()={{}} 
  interface ICacheItemRefreshAction with
    member this.Refresh(key:string, expiredValue:obj, removalReason:CacheItemRemovedReason)=
      match IsoCache,key with
      | x,y ->
          x.Add (y,expiredValue,CacheItemPriority.NotRemovable,{0}CacheRefreshAction.INS,shortExpiration)
          match new {1}() with
          | z ->
              z.IsReturnQueryError<-Nullable<_> false
              ({2}.INS:>I{2}).{3} (z)
          |>fun a->
              match unbox<Generic.List<_>> expiredValue with
              | z -> 
                  z.Clear()
                  z.AddRange a.ResultData",
                    b.Remove (0,3),
                    //{1}
                    c|>Seq.head|>snd 
                    ,
                    //{2}
                    p,
                    //{3}
                    b)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.ServerCaching assemblySuffix,String.Format("{0}.fs","ServerCacheRefreshAction")) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ServerCaching... ServerIsolateCacheModule
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ServerCaching
open System
open System.Collections
open System.Collections.ObjectModel
open FSharp.Collections.ParallelSeq
open Microsoft.Practices.EnterpriseLibrary.Caching
open Microsoft.Practices.EnterpriseLibrary.Caching.Expirations
open WX.Data
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServerCacheManager
open WX.Data.IDataAccess
open WX.Data.DataAccess

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module ServerCache={0}",
                //{0}
                if Seq.length m >0|>not then "()" else String.Empty)|>ignore
              for ((n,p),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                sb.AppendLine ()|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  let  {0}Cache()=
    try 
      match IsoCache,ServerCacheKey.SCK_{1} with
      | x,y ->
          let GetData()=
            match new {2}() with
            |  z ->
                z.IsReturnQueryError<-Nullable<_> false
                ({3}.INS:>I{3}).{0} (z)
          if x.[y]=null then
            match GetData() with
            | z ->new Generic.List<_>(z.ResultData)   //使用List有利于缓存的更新
            |>fun a->
                x.Add(y,a,CacheItemPriority.NotRemovable,{4}CacheRefreshAction.INS,shortExpiration)
          match unbox<Generic.List<_>>  x.[y] with
          | u when u.Count>0 |>not ->
              match GetData() with
              | v ->u.AddRange v.ResultData;u
          | u -> u
    with 
    | e -> new Generic.List<_>()
    
  let ResetCache{1}()=
    ServerCacheManager.IsoCache.Remove ServerCacheKey.SCK_{1}
    {0}Cache() |>ignore
    
  //-------------------------------------------------------------------------",
                    b,
                    //{1}
                    match Regex.Match (b, @"^\s*Get([A-Z_]+)[T|V][a-z]+[\w\W]*$",RegexOptions.Singleline)  with 
                    | y  when y.Groups.Count>1 ->y.Groups.[1].Value
                    | _ ->String.Empty
                    ,
                    //{2}
                    c|>Seq.head|>snd 
                    ,
                    //{3}
                    p,
                    //{4}
                    b.Remove(0,3))|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.ServerCaching assemblySuffix,String.Format("{0}.fs","ServerIsolateCacheModule")) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              (*
              //--------------------------------------------------------------------------------------------------
              //WX.Data.Caching... ServerIsolateCacheModule
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.Caching
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ClientChannel

[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module  ServerCache={0}",
                //{0}
                String.Empty)|>ignore
              for ((n,p),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                sb.AppendLine ()|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  let {0}Cache queryEntity=
    let client=WS_{1}Channel.INS
    client.{0}Cache queryEntity
    
  let AsyncResetCache{2}()=
    async{{
      let client=WS_{1}Channel.INS
      client.ResetCache{2} ()|>ignore
    }}
    |>Async.Start
    
  //-------------------------------------------------------------------------",
                    b,
                    //{1}
                    k,
                    //{2}
                    match Regex.Match (b, @"^\s*Get([A-Z_]+)[T|V][a-z]+[\w\W]*$",RegexOptions.Singleline)  with 
                    | y  when y.Groups.Count>1 ->y.Groups.[1].Value
                    | _ ->String.Empty
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.Caching assemblySuffix,String.Format("{0}.fs","ServerIsolateCacheModule")) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
        *)
        }
        |>Seq.toArray //须执行
    with e -> ObjectDumper.Write e; raise e
