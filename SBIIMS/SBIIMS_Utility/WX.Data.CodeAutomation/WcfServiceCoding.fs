namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type WcfServiceCoding=
  static member GetCode (tableNames:string list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.WcfService
open System
open System.ServiceModel
open System.Runtime.Serialization
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts
[<ServiceBehavior(Name=""WX.Data.WcfService.WS_SBIIMS"",InstanceContextMode=InstanceContextMode.Single) >]
type WS_SBIIMS() =
  interface  IWS_SBIIMS with"
      ) |>ignore
    for tableName in tableNames do
      sb.AppendFormat( @"{0}
    member x.Get{1}s (queryEntity:BQ_{1})=
      BL_{1}.INS.Get{1}s queryEntity
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      BL_{1}.INS.Create{1} executeContent
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      BL_{1}.INS.Update{1} executeContent
    member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      BL_{1}.INS.Delete{1} executeContent
    member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      BL_{1}.INS.Delete{1}s executeContent",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
         )|>ignore
      sb.AppendLine()|>ignore
    sb.ToString()



(*
//Right!!! 所有业务逻辑方法都在一个类中namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq

open WX.Data
open WX.Data.Helper

type WcfServiceCodingAdvance=
  static member GetCode (tableNames:string list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.WcfService
open System
open System.ServiceModel
open System.Runtime.Serialization
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts
[<ServiceBehavior(Name=""WX.Data.WcfService.WS_SBIIMS"",InstanceContextMode=InstanceContextMode.Single) >]
type WS_SBIIMS() =
  interface  IWS_SBIIMS with"
      ) |>ignore
    for tableName in tableNames do
      sb.AppendFormat( @"{0}
    member x.Get{1}s (queryEntity:BQ_{1})=
      BL_SBIIMS.INS.Get{1}s queryEntity
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      BL_SBIIMS.INS.Create{1} executeContent
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      BL_SBIIMS.INS.Update{1} executeContent
    member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      BL_SBIIMS.INS.Delete{1} executeContent
    member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      BL_SBIIMS.INS.Delete{1}s executeContent",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
         )|>ignore
      sb.AppendLine()|>ignore
    sb.ToString()

*)