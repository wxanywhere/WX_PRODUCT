namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type IDataAccessCodingAdvance=
  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.IDataAccess
open System.Collections.Generic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities") |>ignore
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels
      | IndependentTable ->
          IDataAccessCodingAdvance.GetCodeWithTemplateOne a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          IDataAccessCodingAdvance.GetCodeWithTemplateTwo a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb
       
  static member private GetCodeWithTemplateOne (tableName:string)=
    String.Format(@"{0}
type IDA_{1}=
  abstract Get{1}s:BQ_{1}->BD_QueryResult<List<BD_{2}>>
  abstract Create{1}:BD_{2}->BD_ExecuteResult
  abstract Create{1}s:BD_{2}[]->BD_ExecuteResult
  abstract Update{1}:BD_{2}->BD_ExecuteResult
  abstract Update{1}s:BD_{2}[]->BD_ExecuteResult
  abstract Delete{1}:BD_{2}->BD_ExecuteResult
  abstract Delete{1}s:BD_{2}[]->BD_ExecuteResult",
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
      )

  static member private GetCodeWithTemplateTwo (tableName:string)=
    String.Format(@"{0}
type IDA_{1}=
  abstract Get{1}s:BQ_{1}->BD_QueryResult<List<BD_{2}>>",
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
      )