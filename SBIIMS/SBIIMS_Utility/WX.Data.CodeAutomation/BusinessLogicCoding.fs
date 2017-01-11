namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type BusinessLogicCoding=
  static member GetCode (tableNames:string list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.BusinessLogic
open WX.Data
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.DataAccess") |>ignore
    for tableName in tableNames do
      sb.AppendFormat( @"{0}
type BL_{1} =
  static member public INS=BL_{1}()
  private new()={{}}
  member x.Get{1}s (queryEntity:BQ_{1})=
    (DA_{1}.INS:>IDA_{1}).Get{1}s queryEntity
  member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Create{1} executeContent
  member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Update{1} executeContent
  member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1} executeContent
  member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1}s executeContent",
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

type BusinessLogicCodingAdvance=
  static member GetCode (tableNames:string list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.BusinessLogic
open WX.Data
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.DataAccess

type BL_SBIIMS =
  static member public INS=BL_SBIIMS()
  private new()={}"
      ) |>ignore
    for tableName in tableNames do
      sb.AppendFormat( @"{0}
  member x.Get{1}s (queryEntity:BQ_{1})=
    (DA_{1}.INS:>IDA_{1}).Get{1}s queryEntity
  member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Create{1} executeContent
  member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Update{1} executeContent
  member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1} executeContent
  member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1}s executeContent",
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