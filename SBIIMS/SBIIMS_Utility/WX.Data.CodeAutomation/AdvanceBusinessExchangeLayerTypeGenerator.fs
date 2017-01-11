namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceBusinessExchangeLayerCoding=

  (*
//调用示例
@"
  abstract AuditKCGL_SPCF:BD_TV_KCGL_SPCF_DJ_Advance[] ->BD_ExecuteResult
"
|>AdvanceBusinessExchangeLayerCoding.GetDataAccessCodeText
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
          match dx with //c|>Seq.exists(fun (_,cy)->cy.StartsWith "BQ_"),不是很全面
          | "BD_ExecuteResult" ->
              sb.AppendFormat (@"
    (*

    客户端提供的原始条件实体为
    1. 
    *)
    member this.{0} ({1})=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        use sb=new {4}EntitiesAdvance()
        match {2} with
        | {3} ->
            //-------------------------------------------------------------
            //
            result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->this.AttachError(e,-21,this,BatchEntitiesAdvance,result)
      | :? UpdateException as e ->this.AttachError(e,-23,this,BatchEntitiesAdvance,result)
      | e ->this.AttachError(e,-30,this,BatchEntitiesAdvance,result)",
                b,
                //{1}
                (
                c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                ),
                //{2}
                (
                c
                |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                |>Seq.fold (fun acc (cx,_)->match acc with "" ->String.Format("{0}",cx) | _ ->String.Format("{0}, {1}",acc,cx)) ""
                ),
                //{3}
                (
                c
                |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                |>Seq.mapi (fun ix _ ->MatchAliasNames.[ix])
                |>Seq.fold (fun acc cx ->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                ),
                systemAlias)|>ignore
          | "BD_ResultBase" ->
                      sb.AppendFormat (@"
    member this.{0} ({1})=
      let result=new BD_ResultBase(ExecuteDateTime=DateTime.Now)
      try
        
        result.ResultLength<- -1
        result
      with
      | e ->this.AttachError(e,-50,this,result)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        )
                        )|>ignore
          | "unit" ->
              sb.AppendFormat (@"
    member this.{0} ({1})=
      try
      ()
      with
      | e ->this.RecordErrorInfo(e,-100,this)",
                b,
                //{1}
                (
                c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                )
                )|>ignore
          | _ ->
              (*
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>Seq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        ...
        |>Seq.toResult result
              *)
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
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>ObjectDumper.Write
  *)
  static member GenerateCodingFilesFromComponents (systemAlias:string,baseDirectory:string,maxInterfaceGroupPhaseLevel:GroupPhaseLevel)=
    try
    //let sb=new StringBuilder() //第二方案
    let codeLayerNames=
      [
      "WX.Data.BusinessEntitiesAdvance"
      "WX.Data.IDataAccessAdvance"
      "WX.Data.DataAccessAdvance"
      "WX.Data.BusinessLogicAdvance"
      "WX.Data.ServiceContractsAdvance"
      "WX.Data.WcfServiceAdvance"  //不起作用
      "WX.Data.WcfServiceAdvance.WebIISHost"
      "WX.Data.ClientChannelAdvance" //不起作用
      "WX.Data.ClientChannelAdvance.FromAzure"
      "WX.Data.ClientChannelAdvance.FromNative"
      "WX.Data.ClientChannelAdvance.FromServer"
      //"WX.Data.View.ViewModelTemplateAdvance" //临时启用，启用后没有包含该名称的组件组将不能生成高级自动化组件的代码
      //"WX.Data.View.ViewModelTemplate" //不起作用
      ]
    seq{
      let interfaceComponentBaseName="WX.Data.IDataAccessAdvance" 
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
                    match codeLayerNames|>Seq.map (fun b->b+w) with
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
                            AdvanceBusinessExchangeLayerCoding.GenerateCodingFiles (x,systemAlias,m.FullName,m.Parent.FullName,a,maxInterfaceGroupPhaseLevel) |>ignore  
    }
    |>Seq.toArray
    with e -> ObjectDumper.Write e; raise e

  (*
//为所有接口组件生成代码文件
(
"SBIIMS_JXC", //系统简称
[ //接口文件目录名称
@"D:\Workspace\SBIIMS" 
],
TwoPhaseGroup  //接口文件名成组片断段级别
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponentsX
|>ObjectDumper.Write
  *)
  static member GenerateCodingFilesFromComponentsX (systemAlias:string,baseDirectories:string seq,maxInterfaceGroupPhaseLevel:GroupPhaseLevel)=
    seq{
      for baseDirectory in baseDirectories do 
        yield! AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents (systemAlias,baseDirectory,maxInterfaceGroupPhaseLevel)
    }
    |>Seq.toArray

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
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFiles
|>ObjectDumper.Write
  *)
  static member GenerateCodingFiles  (assemblySuffix:string,systemAlias:string,sourceDirectory:string,targetBaseDirectory:string,inferfaceTypeNames:string seq,maxInterfaceGroupPhaseLevel:GroupPhaseLevel) =
    let generateCacheLayerFlag=ref false
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
          let businessTypeName=
            match mx.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries),maxInterfaceGroupPhaseLevel with
            | x, FourPhaseGroup->
                match x.Length with
                | y when y>3 ->"BL_"+x.[1]+"_"+x.[2]+"_"+x.[3]+"_Advance" 
                | y when y>2 ->"BL_"+x.[1]+"_"+x.[2]+"_Advance" 
                | y when y>1 ->"BL_"+x.[1]+"_Advance" 
                | _ ->"BL_Advance"
            | x, ThreePhaseGroup->
                match x.Length with
                | y when y>2 ->"BL_"+x.[1]+"_"+x.[2]+"_Advance" 
                | y when y>1 ->"BL_"+x.[1]+"_Advance" 
                | _ ->"BL_Advance"
            | x, TwoPhaseGroup->
                match x.Length with
                | y when y>1 ->"BL_"+x.[1]+"_Advance" 
                | _ ->"BL_Advance"
            | x, OnePhaseGroup->"BL_Advance"
          match 
            systemAlias.Trim(), //系统别名
            businessTypeName, //逻辑层类型名  
            systemAlias.Trim()+"_"+businessTypeName.Trim().Remove(0,3),  //带系统简称前缀的服务及客户端类名后缀
            GetInterfaceTypeCodingInfo sourceDirectory my with
          | i,j,k,m ->
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
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFiles
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
                match p.ToLowerInvariant().Contains ("query") with
                | true ->
                    sb.AppendFormat( @"
namespace WX.Data.DataAccess
open System
open System.Data
open Microsoft.Practices.EnterpriseLibrary.Logging
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel.Query
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
                | _ ->
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
                  match dx with //c|>Seq.exists(fun (_,cy)->cy.StartsWith "BQ_"),不是很全面
                  | "BD_ExecuteResult" ->
                      sb.AppendFormat (@"
    (*

    客户端提供的原始条件实体为
    1. 
    *)
    member this.{0} ({1})=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        use sb=new {4}EntitiesAdvance()
        match {2} with
        | {3} ->
            //-------------------------------------------------------------
            //
            result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->this.AttachError(e,-21,this,BatchEntitiesAdvance,result)
      | :? UpdateException as e ->this.AttachError(e,-23,this,BatchEntitiesAdvance,result)
      | e ->this.AttachError(e,-30,this,BatchEntitiesAdvance,result)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        ),
                        //{2}
                        (
                        c
                        |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                        |>Seq.fold (fun acc (cx,_)->match acc with "" ->String.Format("{0}",cx) | _ ->String.Format("{0}, {1}",acc,cx)) ""
                        ),
                        //{3}
                        (
                        c
                        |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                        |>Seq.mapi (fun ix _ ->MatchAliasNames.[ix])
                        |>Seq.fold (fun acc cx ->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                        ),
                        //{4}
                        i
                        )|>ignore
                  | "BD_ResultBase" ->
                      sb.AppendFormat (@"
    member this.{0} ({1})=
      let result=new BD_ResultBase(ExecuteDateTime=DateTime.Now)
      try
        
        result.ResultLength<- -1
        result
      with
      | e ->this.AttachError(e,-50,this,result)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        )
                        )|>ignore
                  | "unit" ->
                      sb.AppendFormat (@"
    member this.{0} ({1})=
      try
      ()
      with
      | e ->this.RecordErrorInfo(e,-100,this)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        )
                        )|>ignore
                  | _ ->
                      (*
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>Seq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        ...
        |>Seq.toResult result
                      *)
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
                match Path.Combine(targetBaseDirectory,CodeLayerPath.DataAccessAdvance assemblySuffix,String.Format("{0}.fs",p)) with
                | y ->
                    y|>File.WriteFileCreateOnly (sb.ToString().TrimStart()) //注意，数据访问层只能创建
                    yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.BusinessLogicAdvance...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.BusinessLogic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.DataAccess

[<Sealed>]
type {0} = 
  static member public INS = {0}()
  private new() = {{}}",
                //{0}
                j)|>ignore
              for ((n,p),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    ({3}.INS:>I{3}).{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    //{3}
                    p)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.BusinessLogicAdvance assemblySuffix,String.Format("{0}.fs",j)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ServiceContractsAdvance...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ServiceContracts
open System
open System.ServiceModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities

[<ServiceContract>]
type IWS_{0} ={1}",
                //{0}
                k,
                //{1}
                match m|>Seq.isEmpty|>not with
                | true ->String.Empty
                | _ ->"interface end" 
                )|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,(dx,_)) in a do
                  sb.AppendFormat (@"
  [<OperationContract>] abstract {0}: {1} -> {2}",
                    b,
                    //{1}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0} * {1}:{2}",acc,cx,cy)) ""
                      with
                    | NullOrWhiteSpace ->"unit"
                    | zc ->zc
                    ),
                    dx)|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.ServiceContractAdvance assemblySuffix,String.Format("IWS_{0}.fs",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.WcfServiceAdvance...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.WcfService
open System
open System.ServiceModel
open System.Runtime.Serialization
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

[<ServiceBehavior(Name=""WX.Data.WcfService.WS_{0}"",InstanceContextMode=InstanceContextMode.Single) >]
type WS_{0}() ={1}
  {2}interface IWS_{0} with",
                //{0}
                k,
                //{1}
                match m|>Seq.isEmpty|>not with
                | true ->String.Empty
                | _ ->"class end" 
                ,
                //{2}
                match m|>Seq.isEmpty|>not with
                | true ->String.Empty
                | _ -> @"//" 
                )|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
    //==================================================================
    //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
    member this.{0} ({1})= 
      {3}.INS.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    j)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfServiceAdvance assemblySuffix,String.Format("WS_{0}.fs",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.WcfServiceAdvance.WebIISHost...
              sb.Clear()|>ignore
              sb.AppendFormat( @"<%@ ServiceHost Language=""C#"" Debug=""true"" Service=""WX.Data.WcfService.WS_{0}"" CodeBehind=""WS_{0}.fs"" %>",
                k
                )|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfServiceAdvanceWebIISHost assemblySuffix,String.Format("WS_{0}.svc",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //Config part
              sb.Clear()|>ignore
              sb.AppendFormat( @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name=""WX.Data.WcfService.WS_{0}"">
                <endpoint address="""" binding=""wsHttpBinding"" bindingConfiguration=""""
                    contract=""WX.Data.ServiceContracts.IWS_{0}"" isSystemEndpoint=""false"">
                    <identity>
                        <dns value=""localhost"" />
                    </identity>
                </endpoint>
                <endpoint address=""mex"" binding=""mexHttpBinding"" contract=""IMetadataExchange"" />
                <host>
                    <baseAddresses>
                        <add baseAddress=""http://localhost:8080/WS_{0}"" />
                    </baseAddresses>
                    <timeouts openTimeout=""00:05:00"" />
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>",
                k
                )|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfServiceAdvanceWebIISHost assemblySuffix,String.Format("WS_{0}_WebConfig.txt",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ClientChannelAdvance...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.Runtime.Serialization
open System.ServiceModel
open FSharp.Collections.ParallelSeq
//open Microsoft.ServiceBus
open WX.Data
open WX.Data.ClientHelper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName =
    match ClientChannel with
    | FromServer -> ""WSHttpBinding_IWS_{0}""
    | FromAzure -> ""Azure_WS_{0}"" 
    | _ ->null
  let serviceNamespaceDomain = 
    match ClientChannel with
    | FromAzure -> ConfigurationManager.AppSettings.[""{1}""] 
    | _ ->null
  let serviceUri: Uri =
    match ClientChannel with
    | FromAzure -> Null() //ServiceBus不能使用FrameworkClientProfile, ServiceBusEnvironment.CreateServiceUri(""sb"", serviceNamespaceDomain, ""WS_{0}"")
    | _ -> Null() 
  let channelFactory: ChannelFactory<IWS_{0}> =
    match ClientChannel with
    | FromServer -> new ChannelFactory<IWS_{0}>(_EndpointName)
    | FromAzure -> new ChannelFactory<IWS_{0}>(_EndpointName, new EndpointAddress(serviceUri))
    | _ -> Null()
  let client: IWS_{0} = 
    match ClientChannel with
    | FromServer
    | FromAzure ->channelFactory.CreateChannel()  
    | _ -> Null() 
  static member public INS= WS_{0}Channel()",
                //{0}
                k,
                "ServiceNamespaceDomain"
                )|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}"      ,n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.{0} {2}
    | _ ->
        {3}.INS.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    //{3}
                    j
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfClientChannelAdvance assemblySuffix,String.Format("WS_{0}Channel.fs",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ClientChannelAdvance.FromAzure...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.ServiceModel
open FSharp.Collections.ParallelSeq
open Microsoft.ServiceBus
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName=""Azure_WS_{0}"" 
  let serviceNamespaceDomain = ConfigurationManager.AppSettings.[""ServiceNamespaceDomain""]
  let serviceUri = ServiceBusEnvironment.CreateServiceUri(""sb"", serviceNamespaceDomain, ""WS_{0}"")
  let channelFactory = new ChannelFactory<IWS_{0}>(_EndpointName, new EndpointAddress(serviceUri))
  let client = channelFactory.CreateChannel()
  static member public INS= WS_{0}Channel()",
                //{0}
                k)|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    )
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfClientChannelAdvanceFromAzure assemblySuffix,String.Format("WS_{0}Channel.fs",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ClientChannelAdvance.FromNative...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open FSharp.Collections.ParallelSeq
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic

type WS_{0}Channel() =
  static member public INS= WS_{0}Channel()",
                //{0}
                k)|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    {3}.INS.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    j)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfClientChannelAdvanceFromNative assemblySuffix,String.Format("WS_{0}Channel.fs",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.ClientChannelAdvance.FromServer...
              sb.Clear()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.ServiceModel
open FSharp.Collections.ParallelSeq
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName=""WSHttpBinding_IWS_{0}""
  let channelFactory = new ChannelFactory<IWS_{0}>(_EndpointName)
  let client = channelFactory.CreateChannel()
  static member public INS= WS_{0}Channel()",
                //{0}
                k)|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    )
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              match Path.Combine(targetBaseDirectory,CodeLayerPath.WcfClientChannelAdvanceFromServer assemblySuffix,String.Format("WS_{0}Channel.fs",k)) with
              | y ->
                  y|>File.WriteFile (sb.ToString().TrimStart())
                  yield y
              //--------------------------------------------------------------------------------------------------
              //WX.Data.Caching... ServerIsolateCacheModule
              generateCacheLayerFlag:=false
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
              for ((n,p),a) in m|>Seq.filter (fun mx->mx|>snd|>Seq.exists (fun (my,_,_)->my.EndsWith "Cache")) do
                generateCacheLayerFlag:=true
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                sb.AppendLine ()|>ignore
                for (b,c,_) in a|>Seq.filter (fun (ax,_,_)->ax.EndsWith "Cache") do
                  sb.AppendFormat (@"
  let {0} queryEntity=
    let client=WS_{1}Channel.INS
    client.{0} queryEntity",
                    b,
                    //{1}
                    k
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.Append (@"
  //-------------------------------------------------------------------------")|>ignore
                sb.AppendLine()|>ignore
                for (b,c,_) in a|>Seq.filter (fun (ax,_,_)->ax.StartsWith "ResetCache") do
                  sb.AppendFormat (@"
  let {0} ()=
    async{{
      let client=WS_{1}Channel.INS
      client.{0} ()|>ignore
    }}
    |>Async.Start",
                    b,
                    //{1}
                    k
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              if !generateCacheLayerFlag then
                match Path.Combine(targetBaseDirectory,CodeLayerPath.Caching assemblySuffix,String.Format("{0}.fs","ServerIsolateCacheModule")) with
                | y ->
                    y|>File.WriteFile (sb.ToString().TrimStart())
                    yield y
        }
        |>Seq.toArray //须执行
    with e -> ObjectDumper.Write e; raise e


  (*
//调用示例
(
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF",  //接口文件目录名称
[
"IDA_KCGL_SPCF_BusinessAdvance"
"IDA_KCGL_SPCF_QueryAdvance"
]
,
TwoPhaseGroup  //接口文件名成组片断段级别
)
|>AdvanceBusinessExchangeLayerCoding.GetCodingText
|>Clipboard.SetText
  *)
  static member GetCodingText (systemAlias:string,sourceDirectory:string,inferfaceTypeNames:string seq,maxInterfaceGroupPhaseLevel:GroupPhaseLevel) = //系统简称(如SBIIMS_JXC)*业务逻辑层类型名称*数据访问接口文件目录*数据访问接口类型名组
    try
    let sb=new StringBuilder()
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
        for (mx,my) in a do
          let businessTypeName=
            match mx.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries),maxInterfaceGroupPhaseLevel with
            | x, FourPhaseGroup->
                match x.Length with
                | y when y>3 ->"BL_"+x.[1]+"_"+x.[2]+"_"+x.[3]+"_Advance" 
                | y when y>2 ->"BL_"+x.[1]+"_"+x.[2]+"_Advance" 
                | y when y>1 ->"BL_"+x.[1]+"_Advance" 
                | _ ->"BL_Advance"
            | x, ThreePhaseGroup->
                match x.Length with
                | y when y>2 ->"BL_"+x.[1]+"_"+x.[2]+"_Advance" 
                | y when y>1 ->"BL_"+x.[1]+"_Advance" 
                | _ ->"BL_Advance"
            | x, TwoPhaseGroup->
                match x.Length with
                | y when y>1 ->"BL_"+x.[1]+"_Advance" 
                | _ ->"BL_Advance"
            | x, OnePhaseGroup->"BL_Advance"
          sb.AppendFormat(@"
//------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------
//------------------------------------------------------------------------------------------------
//{0}", businessTypeName
            )|>ignore
          sb.AppendLine()|>ignore
          match
            systemAlias.Trim(), //系统别名 
            businessTypeName, //逻辑层类型名
            systemAlias.Trim()+"_"+businessTypeName.Trim().Remove(0,3),  //带系统简称前缀的服务及客户端类名后缀
            GetInterfaceTypeCodingInfo sourceDirectory my with
          | i,j,k,m ->
              sb.Append (@"//1. WX.Data.DataAccessAdvance-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              for ((n,p),a) in m do
                match p.ToLowerInvariant().Contains ("query") with
                | true ->
                    sb.AppendFormat( @"
namespace WX.Data.DataAccess
open System
open System.Data
open Microsoft.Practices.EnterpriseLibrary.Logging
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel.Query
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
                | _ ->
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
                  match dx with //c|>Seq.exists(fun (_,cy)->cy.StartsWith "BQ_"),不是很全面
                  | "BD_ExecuteResult" ->
                      sb.AppendFormat (@"
    (*

    客户端提供的原始条件实体为
    1. 
    *)
    member this.{0} ({1})=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        use sb=new {4}EntitiesAdvance()
        match {2} with
        | {3} ->
            //-------------------------------------------------------------
            //
            result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->this.AttachError(e,-21,this,BatchEntitiesAdvance,result)
      | :? UpdateException as e ->this.AttachError(e,-23,this,BatchEntitiesAdvance,result)
      | e ->this.AttachError(e,-30,this,BatchEntitiesAdvance,result)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        ),
                        //{2}
                        (
                        c
                        |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                        |>Seq.fold (fun acc (cx,_)->match acc with "" ->String.Format("{0}",cx) | _ ->String.Format("{0}, {1}",acc,cx)) ""
                        ),
                        //{3}
                        (
                        c
                        |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                        |>Seq.mapi (fun ix _ ->MatchAliasNames.[ix])
                        |>Seq.fold (fun acc cx ->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                        ),
                        i)|>ignore
                  | "BD_ResultBase" ->
                      sb.AppendFormat (@"
    member this.{0} ({1})=
      let result=new BD_ResultBase(ExecuteDateTime=DateTime.Now)
      try
        
        result.ResultLength<- -1
        result
      with
      | e ->this.AttachError(e,-50,this,result)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        )
                        )|>ignore
                  | "unit" ->
                      sb.AppendFormat (@"
    member this.{0} ({1})=
      try
      ()
      with
      | e ->this.RecordErrorInfo(e,-100,this)",
                        b,
                        //{1}
                        (
                        c|>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                        )
                        )|>ignore
                  | _ ->
                      (*
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>Seq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        ...
        |>Seq.toResult result
                      *)
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
                        match dy.EndsWith "]" with 
                        | true ->dy.Substring(0,dy.Length-2) | _ ->dy
                        ,
                        //{3}
                        match p.ToLowerInvariant().Contains ("query") with
                        | true ->i.Insert(i.LastIndexOf('_')+1,"Query")   //需改进SBIIMS_JXC+Query的命名方法
                        | _ ->i
                        )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//2. WX.Data.BusinessLogicAdvance-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.BusinessLogic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.DataAccess

[<Sealed>]
type {0} = 
  static member public INS = {0}()
  private new() = {{}}",
                //{0}
                j)|>ignore
              for ((n,p),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    ({3}.INS:>I{3}).{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    p)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//3. WX.Data.ServiceContractsAdvance-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ServiceContracts
open System
open System.ServiceModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities

[<ServiceContract>]
type IWS_{0} ={1}",
                //{0}
                k,
                //{1}
                match m|>Seq.isEmpty|>not with
                | true ->String.Empty
                | _ ->"interface end" 
                )|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,(dx,_)) in a do
                  sb.AppendFormat (@"
  [<OperationContract>] abstract {0}: {1} -> {2}",
                    b,
                    //{1}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0} * {1}:{2}",acc,cx,cy)) ""
                      with
                    | NullOrWhiteSpace ->"unit"
                    | zc ->zc
                    ),
                    dx)|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//4. WX.Data.WcfServiceAdvance-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.WcfService
open System
open System.ServiceModel
open System.Runtime.Serialization
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

[<ServiceBehavior(Name=""WX.Data.WcfService.WS_{0}"",InstanceContextMode=InstanceContextMode.Single) >]
type WS_{0}() ={1}
  {2}interface IWS_{0} with",
                //{0}
                k,
                //{1}
                match m|>Seq.isEmpty|>not with
                | true ->String.Empty
                | _ ->"class end" 
                ,
                //{2}
                match m|>Seq.isEmpty|>not with
                | true ->String.Empty
                | _ -> @"//" 
                )|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
    //==================================================================
    //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
    member this.{0} ({1})= 
      {3}.INS.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    j)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//5. WX.Data.WcfServiceAdvance.WebIISHost-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"<%@ ServiceHost Language=""C#"" Debug=""true"" Service=""WX.Data.WcfService.WS_{0}"" CodeBehind=""WS_{0}.fs"" %>",
                k
                )|>ignore
              sb.AppendLine()|>ignore
              sb.Append ("//---------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name=""WX.Data.WcfService.WS_{0}"">
                <endpoint address="""" binding=""wsHttpBinding"" bindingConfiguration=""""
                    contract=""WX.Data.ServiceContracts.IWS_{0}"" isSystemEndpoint=""false"">
                    <identity>
                        <dns value=""localhost"" />
                    </identity>
                </endpoint>
                <endpoint address=""mex"" binding=""mexHttpBinding"" contract=""IMetadataExchange"" />
                <host>
                    <baseAddresses>
                        <add baseAddress=""http://localhost:8080/WS_{0}"" />
                    </baseAddresses>
                    <timeouts openTimeout=""00:05:00"" />
                </host>
            </service>
        </services>
    </system.serviceModel>
</configuration>",
                k
                )|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//6. WX.Data.ClientChannelAdvance-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.Runtime.Serialization
open System.ServiceModel
open FSharp.Collections.ParallelSeq
//open Microsoft.ServiceBus
open WX.Data
open WX.Data.ClientHelper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName =
    match ClientChannel with
    | FromServer -> ""WSHttpBinding_IWS_{0}""
    | FromAzure -> ""Azure_WS_{0}"" 
    | _ ->null
  let serviceNamespaceDomain = 
    match ClientChannel with
    | FromAzure -> ConfigurationManager.AppSettings.[""{1}""] 
    | _ ->null
  let serviceUri: Uri =
    match ClientChannel with
    | FromAzure -> Null() //ServiceBus不能使用FrameworkClientProfile, ServiceBusEnvironment.CreateServiceUri(""sb"", serviceNamespaceDomain, ""WS_{0}"")
    | _ -> Null() 
  let channelFactory: ChannelFactory<IWS_{0}> =
    match ClientChannel with
    | FromServer -> new ChannelFactory<IWS_{0}>(_EndpointName)
    | FromAzure -> new ChannelFactory<IWS_{0}>(_EndpointName, new EndpointAddress(serviceUri))
    | _ -> Null()
  let client: IWS_{0} = 
    match ClientChannel with
    | FromServer
    | FromAzure ->channelFactory.CreateChannel()  
    | _ -> Null() 
  static member public INS= WS_{0}Channel()",
                //{0}
                k,
                //{1}
                "ServiceNamespaceDomain"
                )|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.{0} {2}
    | _ ->
        {3}.INS.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    //{3}
                    j
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//7. WX.Data.ClientChannelAdvance.FromAzure-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.ServiceModel
open FSharp.Collections.ParallelSeq
open Microsoft.ServiceBus
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName=""Azure_WS_{0}"" 
  let serviceNamespaceDomain = ConfigurationManager.AppSettings.[""ServiceNamespaceDomain""]
  let serviceUri = ServiceBusEnvironment.CreateServiceUri(""sb"", serviceNamespaceDomain, ""WS_{0}"")
  let channelFactory = new ChannelFactory<IWS_{0}>(_EndpointName, new EndpointAddress(serviceUri))
  let client = channelFactory.CreateChannel()
  static member public INS= WS_{0}Channel()",
                //{0}
                k)|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    )
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//8. WX.Data.ClientChannelAdvance.FromNative-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open FSharp.Collections.ParallelSeq
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic

type WS_{0}Channel() =
  static member public INS= WS_{0}Channel()",
                //{0}
                k)|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    {3}.INS.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    ),
                    j)|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.Append ( @"//9. WX.Data.ClientChannelAdvance.FromServer-----------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
              sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.ServiceModel
open FSharp.Collections.ParallelSeq
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName=""WSHttpBinding_IWS_{0}""
  let channelFactory = new ChannelFactory<IWS_{0}>(_EndpointName)
  let client = channelFactory.CreateChannel()
  static member public INS= WS_{0}Channel()",
                //{0}
                k)|>ignore
              for ((n,_),a) in m do
                sb.AppendFormat ( @"
  //==================================================================
  //{0}",n
                )|>ignore
                for (b,c,_) in a do
                  sb.AppendFormat (@"
  member this.{0} ({1})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {2}",
                    b,
                    //{1}
                    (
                    c
                    |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not) //去掉可选参数
                    |>Seq.fold (fun acc (cx,cy)->match acc with "" ->String.Format("{0}:{1}",cx,cy) | _ ->String.Format("{0}, {1}:{2}",acc,cx,cy)) ""
                    ),
                    //{2}
                    (
                    match 
                      c
                      |>Seq.filter (fun (cx,_)->cx.Contains("?")|>not)
                      |>Seq.fold (fun acc (cx,_)->match acc with "" ->cx | _ ->String.Format("{0}, {1}",acc,cx)) ""
                      with
                    | NullOrWhiteSpace ->"()"
                    | xa when xa.Contains(",") ->String.Format ("({0})",xa)  //有两个以上的参数须加括号
                    | xa ->xa
                    )
                    )|>ignore
                  sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
                sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
              sb.AppendLine()|>ignore
        sb.ToString()
    with e -> ObjectDumper.Write e; raise e