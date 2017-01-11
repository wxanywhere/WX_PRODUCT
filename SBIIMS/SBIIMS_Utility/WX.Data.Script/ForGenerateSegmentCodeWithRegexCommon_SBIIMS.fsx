module WX.Data.Script

#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"


open System
open System.IO
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Text.RegularExpressions
open System.Data

#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"

open WX.Data.Helper
open WX.Data

(*
Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx
Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx

如何使用正则表达式搜索
http://technet.microsoft.com/zh-cn/library/ms174214.aspx

正则表达式语言元素
http://msdn.microsoft.com/zh-cn/library/az24scfc.aspx


常用正则表达式大全！（例如：匹配中文、匹配html） 
http://www.cnblogs.com/guiliangfeng/archive/2009/04/13/1434696.html
*)

//==================================

(*
参数input的格式
(
//文件名须按代码层顺，只需提供数据访问层类型名称及业务逻辑层类型名称
[  
"DA_ZHCX_JH_WLZ_Advance"   //数据访问层类型名
"BL_JHGL_WLZ_Advance"          //业务逻辑层类型名
]
,
//数据访问层的接口代码
@"
  abstract AuditDJ_CGTH:BD_T_DJ_JHGL_Advance [] ->BD_Result
  abstract GetJH_WLZ_GHQK_HZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHQK_HZ_Advance[]
"
)
*)
let generateCodeFromMembers (input:string list*string)=
  let sb=new StringBuilder()
  match input with
  | a,b ->
    match Regex.Split(b.Trim(),@"\s*\n\s*",RegexOptions.Multiline) with
    | x ->
        seq{
          for c in x do
            match Regex.Matches(c, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*([a-zA-Z_\s\[\]]+)[\s\w\W]*\-\>\s*([a-zA-Z_\s\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格
            | x when x.Count>0 && x.[0].Groups.Count>3 ->
                yield 
                  x.[0].Groups.[1].Value, //方法名 b
                  match x.[0].Groups.[2].Value.Replace(" ","").Trim() with //条件名称,条件类型 c
                  | y when y.StartsWith "BQ" ->"queryEntity",y
                  | y when y.StartsWith "BD" && y.EndsWith @"]" -> "businessEntities",y
                  | y when y.StartsWith "BD" ->"businessEntity",y
                  | y ->String.Empty,y
                  ,
                  x.[0].Groups.[3].Value.Replace(" ","").Trim(), //结果类型  d
                  a.[0], //数据访问层类型名 e
                  a.[1] //业务逻辑层类型名 f
            | _ ->()
          }
        ,
        (*
        match a.[1].Split ([|'_'|],StringSplitOptions.RemoveEmptyEntries) with //简明注释
        | y when y.Length>3 -> y.[y.Length-3]+"_"+y.[y.Length-2]
        | _ ->String.Empty
        *)
        match a.[0].Split ([|'_'|],StringSplitOptions.RemoveEmptyEntries) with //简明注释
        | y when y.Length>3 -> y.[y.Length-3]+"_"+y.[y.Length-2]
        | _ ->String.Empty
|>fun (a,a0)->
    sb.Append (@"//WX.Data.DataAccessAdvance-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    for (b,(c1,c2),_,_,_) in a do
      sb.AppendFormat (@"
    member this.{0} ({1}:{2})=",
        b,c1,c2)|>ignore
      sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.Append ( @"//WX.Data.BusinessLogicAdvance-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    sb.AppendFormat ( @"
  //==================================================================
  //{0}",a0)|>ignore
    for (b,(c1,c2),_,e,_) in a do
      sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    ({3}.INS:>I{3}).{0} {1}",
        b,c1,c2,e)|>ignore
      sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.Append ( @"//WX.Data.ServiceContractsAdvance-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    sb.AppendFormat ( @"
  //==================================================================
  //{0}",a0)|>ignore
    for (b,(c1,c2),d,_,_) in a do
      sb.AppendFormat (@"
  [<OperationContract>] abstract {0}:{1}:{2}->{3}",
        b,c1,c2,d)|>ignore
    sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.Append ( @"//WX.Data.WcfServiceAdvance-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    sb.AppendFormat ( @"
    //==================================================================
    //{0}",a0)|>ignore
    for (b,(c1,c2),_,_,f) in a do
      sb.AppendFormat (@"
    member this.{0} ({1}:{2})= 
      {3}.INS.{0} {1}",
        b,c1,c2,f)|>ignore
      sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.Append ( @"//WX.Data.ClientChannel.FromAzure-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    sb.AppendFormat ( @"
  //==================================================================
  //{0}",a0)|>ignore
    for (b,(c1,c2),_,_,_) in a do
      sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
        b,c1,c2)|>ignore
      sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.Append ( @"//WX.Data.ClientChannel.FromNative-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    sb.AppendFormat ( @"
  //==================================================================
  //{0}",a0)|>ignore
    for (b,(c1,c2),_,_,f) in a do
      sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    {3}.INS.{0} {1}",
        b,c1,c2,f)|>ignore
      sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.AppendLine()|>ignore
    sb.Append ( @"//WX.Data.ClientChannel.FromServer-----------------------------------------------------")|>ignore
    sb.AppendLine()|>ignore
    sb.AppendFormat ( @"
  //==================================================================
  //{0}",a0)|>ignore
    for (b,(c1,c2),_,_,_) in a do
      sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
        b,c1,c2)|>ignore
      sb.AppendLine()|>ignore
    sb.ToString()


(*
//------------------------------------------------------------------------------------------------------
seq{
  yield 1,
  for a in 0..10 do
    yield a,
    seq{
      yield a
    }
}
//------------------------------------------------------------------------------------------------------
@"
abstract AuditKCGL_SPCF:BD_TV_KCGL_SPCF_DJ_Advance ->BD_Result1
  //abstract AuditKCGL_SPCF:BD_TV_KCGL_SPCF_DJ_Advance ->BD_Result2
  (**)
  (*   !@#$%^&*()_+|
  abstract AuditKCGL_SPCF:BD_TV_KCGL_SPCF_DJ_Advance ->BD_Result3
  *)
abstract GetKCGL_SPCF_DJView:BQ_KCGL_SPCF_DJ_Advance ->BD_V_KCGL_SPCF_DJ_Advance[]4"
|>fun a->
    seq{
      for b in Regex.Split (a,@"\(\*[\w\W\n]*\*\)",RegexOptions.Multiline) do //先去除(*...*)的注释
        for c in  Regex.Split (b.Trim(),@"\s*\n\s*",RegexOptions.Multiline) do //注释"//"通过行匹配去除
          if String.IsNullOrWhiteSpace c|>not  then yield c.Trim()
    }
//------------------------------------------------------------------------------------------------------
(
"BL_KCGL_SPCF_Advance",
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF",
[
"IDA_KCGL_SPCF_BusinessAdvance"
"IDA_KCGL_SPCF_QueryAdvance"
]
)
|>fun (a,b,c) ->
    for cx in c do
      match Path.Combine(b,cx+".fs")|>File.ReadFile  with
      | x ->
          match Regex.Match (x, @"[\w\W\n]*type\s+I([a-zA-Z_]+)\s*\=[\n\s]*(abstract[\w\W\n]+)$",RegexOptions.Multiline)  with
          | y  when y.Groups.Count>2 ->
              ObjectDumper.Write y.Groups.[1].Value
          | _ ->ObjectDumper.Write "None"
//------------------------------------------------------------------------------------------------------
//调用示例
//从数据访问接口文件获取代码
(
"BL_KCGL_SPCF_Advance", //业务逻辑层类型名
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF", //接口文件目录名称
[
"IDA_KCGL_SPCF_BusinessAdvance"
"IDA_KCGL_SPCF_QueryAdvance"
]
)
|>generateCodeFromFiles
|>Clipboard.SetText
*)
let generateCodeFromFiles (input:string*string*string list)= //业务逻辑层类型名称*数据访问接口文件目录*数据访问接口类型名组
  let sb=new StringBuilder()
  match input with
  | a,b,c ->
      seq{
        for cm in c do
          match Path.Combine(b,cm+".fs")|>File.ReadFile  with
          | x ->
              match Regex.Match (x, @"[\w\W\n]*type\s+I([a-zA-Z_]+)\s*\=[\n\s]*(abstract[\w\W\n]+)$",RegexOptions.Multiline)  with
              | y  when y.Groups.Count>2 ->
                  yield 
                    match y.Groups.[1].Value with  //简码数据访问组注释,
                    | z when z.Length>3 ->z.Remove(0,3)
                    | _ ->String.Empty
                  ,
                  match y.Groups.[2].Value with
                  | z ->
                      seq{
                        for m in Regex.Split (z,@"\(\*[\w\W\n]*\*\)",RegexOptions.Multiline) do //先去除(*...*)的注释
                          for n in  Regex.Split (m.Trim(),@"\s*\n\s*",RegexOptions.Multiline) do //注释"//"通过行匹配去除
                            if String.IsNullOrWhiteSpace n|>not  then //yield n.Trim()
                              match Regex.Match (n, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*([a-zA-Z_\s\[\]]+)[\s\w\W]*\-\>\s*([a-zA-Z_\s\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格
                              | w when w.Groups.Count>3 ->
                                  yield 
                                    w.Groups.[1].Value, //方法名称 b
                                    match w.Groups.[2].Value.Replace(" ","").Trim() with //条件名称*条件类型, 并消除"[]"前的空格 c
                                    | y when y.StartsWith "BQ" ->"queryEntity",y
                                    | y when y.StartsWith "BD" && y.EndsWith @"]" -> "businessEntities",y
                                    | y when y.StartsWith "BD" ->"businessEntity",y
                                    | y ->String.Empty,y
                                    ,
                                    w.Groups.[3].Value.Replace(" ","").Trim(), //结果类型名称，并消除"[]"前的空格  d
                                    y.Groups.[1].Value,  //数据访问类型名，去掉接口类型名的"I"即可 e
                                    a //业务逻辑层类型名 f
                              | _ ->()
                      }
              | _ ->()
      }
  |>Seq.sortBy (fun (a,_)->a.Remove(0,a.LastIndexOf('_')+1))
  |>fun m->
      sb.Append (@"//1. WX.Data.DataAccessAdvance-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        for (b,(c1,c2),_,_,_) in a do
          sb.AppendFormat (@"
    member this.{0} ({1}:{2})=",
            b,c1,c2)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//2. WX.Data.BusinessLogicAdvance-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),_,e,_) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    ({3}.INS:>I{3}).{0} {1}",
            b,c1,c2,e)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//3. WX.Data.ServiceContractsAdvance-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),d,_,_) in a do
          sb.AppendFormat (@"
  [<OperationContract>] abstract {0}:{1}:{2}->{3}",
            b,c1,c2,d)|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//4. WX.Data.WcfServiceAdvance-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        sb.AppendFormat ( @"
    //==================================================================
    //{0}",n)|>ignore
        for (b,(c1,c2),_,_,f) in a do
          sb.AppendFormat (@"
    member this.{0} ({1}:{2})= 
      {3}.INS.{0} {1}",
            b,c1,c2,f)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//5. WX.Data.ClientChannel.FromAzure-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),_,_,_) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
            b,c1,c2)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//6. WX.Data.ClientChannel.FromNative-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),_,_,f) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    {3}.INS.{0} {1}",
            b,c1,c2,f)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//7. WX.Data.ClientChannel.FromServer-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for (n,a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),_,_,_) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
            b,c1,c2)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.ToString()

(*
//从数据访问接口文件获取全类型代码
(
"SBIIMS_JXC",
"BL_KCGL_SPCF_Advance", //业务逻辑层类型名
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF",  //接口文件目录名称
[
"IDA_KCGL_SPCF_BusinessAdvance"
"IDA_KCGL_SPCF_QueryAdvance"
]
)
|>generateFullTypeCodeFromFiles
|>Clipboard.SetText
*)
let generateFullTypeCodeFromFiles (input:string*string*string*string list)= //系统简称(如SBIIMS_JXC)*业务逻辑层类型名称*数据访问接口文件目录*数据访问接口类型名组
  let sb=new StringBuilder()
  match input with
  | a,b,c,d ->
      b.Trim(), //逻辑层类型名
      a.Trim()+"_"+b.Trim().Remove(0,3), //带系统简称前缀的服务及客户端类名后缀
      seq{
        for cm in d do
          match Path.Combine(c,cm+".fs")|>File.ReadFile  with
          | x ->
              match Regex.Match (x, @"[\w\W\n]*type\s+I([a-zA-Z_]+)\s*\=[\n\s]*(abstract[\w\W\n]+)$",RegexOptions.Multiline)  with
              | y  when y.Groups.Count>2 ->
                  yield 
                    match y.Groups.[1].Value with  //简码数据访问组注释*数据访问层类型名
                    | z when z.Length>3 ->z.Remove(0,3),z
                    | z ->String.Empty,z
                  ,
                  match y.Groups.[2].Value with
                  | z ->
                      seq{
                        for m in Regex.Split (z,@"\(\*[\w\W\n]*\*\)",RegexOptions.Multiline) do //先去除(*...*)的注释
                          for n in  Regex.Split (m.Trim(),@"\s*\n\s*",RegexOptions.Multiline) do //注释"//"通过行匹配去除
                            if String.IsNullOrWhiteSpace n|>not  then //yield n.Trim()
                              match Regex.Match (n, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*([a-zA-Z_\s\[\]]+)\s*\-\>\s*([a-zA-Z_\s\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格
                              | w when w.Groups.Count>3 ->
                                  yield 
                                    w.Groups.[1].Value, //方法名称 b
                                    match w.Groups.[2].Value.Replace(" ","").Trim() with //条件名称*条件类型, 并消除"[]"前的空格 c
                                    | y when y.StartsWith "BQ" ->"queryEntity",y
                                    | y when y.StartsWith "BD" && y.EndsWith @"]" -> "businessEntities",y
                                    | y when y.StartsWith "BD" ->"businessEntity",y
                                    | y ->String.Empty,y
                                    ,
                                    w.Groups.[3].Value.Replace(" ","").Trim(), //结果类型名称，并消除"[]"前的空格  d
                                    y.Groups.[1].Value,  //数据访问类型名，去掉接口类型名的"I"即可 e
                                    b //业务逻辑层类型名 f
                              | _ ->()
                      }
              | _ ->()
      }
      |>Seq.sortBy (fun ((a,_),_)->a.Remove(0,a.LastIndexOf('_')+1))
  |>fun (j,k,m)->
      sb.Append (@"//1. WX.Data.DataAccessAdvance-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      for ((n,p),a) in m do
        sb.AppendFormat( @"
namespace WX.Data.DataAccess
open System
open System.Data
open Microsoft.Practices.EnterpriseLibrary.Logging
open Microsoft.FSharp.Collections
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.Helper
open WX.Data

[<Sealed>]
type {0}=
  inherit DA_Base
  static member public INS = {0}() 
  private new () = {{inherit DA_Base()}}
  interface I{0} with",
          //{0}
          p)|>ignore
        for (b,(c1,c2),_,_,_) in a do
          sb.AppendFormat (@"
    member this.{0} ({1}:{2})=",
            b,c1,c2)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//2. WX.Data.BusinessLogicAdvance-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      sb.AppendFormat( @"
namespace WX.Data.BusinessLogic
open WX.Data.IDataAccess
open WX.Data.DataAccess
open WX.Data.BusinessEntities

[<Sealed>]
type {0} = 
  static member public INS = {0}()
  private new() = {{}}",
        //{0}
        j)|>ignore
      for ((n,_),a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),_,e,_) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    ({3}.INS:>I{3}).{0} {1}",
            b,c1,c2,e)|>ignore
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
open WX.Data.BusinessEntities
open WX.Data.BusinessBase

[<ServiceContract>]
type IWS_{0} =",
        //{0}
        k)|>ignore
      for ((n,_),a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),d,_,_) in a do
          sb.AppendFormat (@"
  [<OperationContract>] abstract {0}:{1}:{2}->{3}",
            b,c1,c2,d)|>ignore
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
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

[<ServiceBehavior(Name=""WX.Data.WcfService.WS_{0}"",InstanceContextMode=InstanceContextMode.Single) >]
type WS_{0}() =
  interface IWS_{0} with",
        //{0}
        k)|>ignore
      for ((n,_),a) in m do
        sb.AppendFormat ( @"
    //==================================================================
    //{0}",n)|>ignore
        for (b,(c1,c2),_,_,f) in a do
          sb.AppendFormat (@"
    member this.{0} ({1}:{2})= 
      {3}.INS.{0} {1}",
            b,c1,c2,f)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//5. WX.Data.ClientChannel.FromAzure-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.ServiceModel
open Microsoft.FSharp.Collections
open Microsoft.ServiceBus
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
  //{0}",n)|>ignore
        for (b,(c1,c2),_,_,_) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
            b,c1,c2)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//6. WX.Data.ClientChannel.FromNative-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open Microsoft.FSharp.Collections
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic

type WS_{0}Channel() =
  static member public INS= WS_{0}Channel()",
        //{0}
        k)|>ignore
      for ((n,_),a) in m do
        sb.AppendFormat ( @"
  //==================================================================
  //{0}",n)|>ignore
        for (b,(c1,c2),_,_,f) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    {3}.INS.{0} {1}",
            b,c1,c2,f)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.AppendLine()|>ignore
      sb.Append ( @"//7. WX.Data.ClientChannel.FromServer-----------------------------------------------------")|>ignore
      sb.AppendLine()|>ignore
      sb.AppendFormat( @"
namespace WX.Data.ClientChannel
open System
open System.ServiceModel
open Microsoft.FSharp.Collections
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
  //{0}",n)|>ignore
        for (b,(c1,c2),_,_,_) in a do
          sb.AppendFormat (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
            b,c1,c2)|>ignore
          sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
        sb.AppendLine()|>ignore
      sb.ToString()

(*Right backup
(
//文件名须按代码层顺，只需提供数据访问层类型名称及业务逻辑层类型名称
[  
"DA_ZHCX_JH_WLZ_Advance"   //数据访问层类型名
"BL_JHGL_WLZ_Advance"          //业务逻辑层类型名
]
,
//数据访问层的接口代码
@"
  abstract AuditDJ_CGTH:BD_T_DJ_JHGL_Advance [] ->BD_Result
  abstract GetJH_WLZ_GHQK_HZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHQK_HZ_Advance[]
"
)
|>fun (a,b)->
    match Regex.Split(b.Trim(),@"\s*\n\s*",RegexOptions.Multiline) with
    | x ->
        seq{
          for c in x do
            match Regex.Matches(c, @"^.*abstract\s+([a-zA-Z_]+)\s*:\s*([a-zA-Z_\s\[\]]+)\s*\-\>\s*([a-zA-Z_\s\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格
            | x when x.Count>0 && x.[0].Groups.Count>3 ->
                yield 
                  x.[0].Groups.[1].Value, //方法名 b
                  match x.[0].Groups.[2].Value.Replace(" ","").Trim() with //条件名称,条件类型 c
                  | y when y.StartsWith "BQ" ->"queryEntity",y
                  | y when y.StartsWith "BD" && y.EndsWith @"]" -> "businessEntities",y
                  | y when y.StartsWith "BD" ->"businessEntity",y
                  | y ->String.Empty,y
                  ,
                  x.[0].Groups.[3].Value.Replace(" ","").Trim(), //结果类型  d
                  a.[0], //数据访问层类型名 e
                  a.[1]  //业务逻辑层类型名 f
            | _ ->()
          }
|>fun a->
    ObjectDumper.Write @"//WX.Data.DataAccessAdvance----------------------------------"
    for (b,(c1,c2),_,_,_) in a do
      String.Format (@"
    member this.{0} ({1}:{2})=",
        b,c1,c2)
      |>ObjectDumper.Write
    ObjectDumper.Write System.Environment.NewLine
    ObjectDumper.Write @"//WX.Data.BusinessLogicAdvance----------------------------------"
    for (b,(c1,c2),_,e,_) in a do
      String.Format (@"
    member this.{0} ({1}:{2})=
      ({3}.INS:>I{3}).{0} {1}",
        b,c1,c2,e)
      |>ObjectDumper.Write
    ObjectDumper.Write System.Environment.NewLine
    ObjectDumper.Write @"//WX.Data.ServiceContractsAdvance----------------------------------"
    for (b,(c1,c2),d,_,_) in a do
      String.Format (@"
  [<OperationContract>] abstract {0}:{1}:{2}->{3}",
        b,c1,c2,d)
      |>ObjectDumper.Write
    ObjectDumper.Write System.Environment.NewLine
    ObjectDumper.Write @"//WX.Data.WcfServiceAdvance----------------------------------"
    for (b,(c1,c2),_,_,f) in a do
      String.Format (@"
    member this.{0} ({1}:{2})= 
      {3}.INS.{0} {1}",
        b,c1,c2,f)
      |>ObjectDumper.Write
    ObjectDumper.Write System.Environment.NewLine
    ObjectDumper.Write @"//WX.Data.ClientChannel.FromAzure----------------------------------"
    for (b,(c1,c2),_,_,_) in a do
      String.Format (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
        b,c1,c2)
      |>ObjectDumper.Write
    ObjectDumper.Write System.Environment.NewLine
    ObjectDumper.Write @"//WX.Data.ClientChannel.FromNative----------------------------------"
    for (b,(c1,c2),_,_,f) in a do
      String.Format (@"
  member this.{0} ({1}:{2})=
    {3}.INS.{0} {1}",
        b,c1,c2,f)
      |>ObjectDumper.Write
    ObjectDumper.Write System.Environment.NewLine
    ObjectDumper.Write @"//WX.Data.ClientChannel.FromServer----------------------------------"
    for (b,(c1,c2),_,_,_) in a do
      String.Format (@"
  member this.{0} ({1}:{2})=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.{0} {1}",
        b,c1,c2)
      |>ObjectDumper.Write

(  
"DA_ZHCX_JH_WLZ_Advance"
,
@"
  abstract AuditDJ_CGTH:BD_T_DJ_JHGL_Advance [] ->BD_Result
  abstract GetJH_WLZ_GHQK_HZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHQK_HZ_Advance[]
"
)
|>fun (a,b)->
    match Regex.Split(b.Trim(),@"\s*\n\s*",RegexOptions.Multiline) with
    | x ->
          for c in x do
            match Regex.Matches(c, @"^.*abstract\s+([a-zA-Z_]+)\s*:\s*([a-zA-Z_\s\[\]]+)\s*\-\>\s*([a-zA-Z_\s\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格
            | x when x.Count>0 && x.[0].Groups.Count>2 ->
              match x.[0].Groups.[1].Value,x.[0].Groups.[2].Value.Replace(" ","").Trim(),x.[0].Groups.[3].Value.Replace(" ","").Trim()with
              | y,z,_ when z.StartsWith "BQ" ->
                  String.Format (@"
    member this.{0} (queryEntity:{1})=
      ({2}.INS:>I{2}).{0} queryEntity",
                    y,z,a)
                  |>ObjectDumper.Write
              | y,z,_ when z.StartsWith "BD" && z.EndsWith @"]" ->
                  String.Format (@"
    member this.{0} (businessEntities:{1})=
      ({2}.INS:>I{2}).{0} businessEntities",
                    y,z,a)
                  |>ObjectDumper.Write
              | y,z,_ when z.StartsWith "BD" ->
                  String.Format (@"
    member this.{0} (businessEntity:{1})=
      ({2}.INS:>I{2}).{0} businessEntity",
                    y,z,a)
                  |>ObjectDumper.Write
              | _ ->()
            | _ ->()



//----------------------------------------------------------------------------------------
//参考的正则表达式
@"(?<=^.*abstract\s+)([a-zA-Z_]+)(?=\s*:.*$)"  //x.[0].Groups.[0].Value 

//Right Bakcup
(  
"DA_ZHCX_JH_WLZ_Advance"
,
@"
  abstract GetJH_WLZ_GHDJView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHDJ_Advance[]
  abstract GetJH_WLZ_GHQK_HZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHQK_HZ_Advance[]
"
)
|>fun (a,b)->
    match Regex.Split(b.Trim(),@"\s*\n\s*",RegexOptions.Multiline) with
    | x ->
        seq{
          for c in x do
            match Regex.Matches(c, @"^.*abstract\s+([a-zA-Z_]+)\s*\:.*$",RegexOptions.Singleline)  with
            | x when x.Count>0->
                if x.Count>0 && x.[0].Groups.Count>1 then
                  yield x.[0].Groups.[1].Value
            | _ ->()
          }

*)