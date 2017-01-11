namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type BusinessLogicCodingAdvance=
  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.BusinessLogic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.DataAccess") |>ignore
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels ->
          BusinessLogicCodingAdvance.GetCodeWithTemplateOne a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | IndependentTable ->
          match a.ColumnConditionTypes with
          | ColumnConditionTypeContains [HasXH] _ ->
              BusinessLogicCodingAdvance.GetCodeWithTemplateThree a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | _ ->
              BusinessLogicCodingAdvance.GetCodeWithTemplateOne a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          BusinessLogicCodingAdvance.GetCodeWithTemplateTwo a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb

  static member private GetCodeWithTemplateOne (tableName:string)=
    String.Format( @"{0}
type BL_{1} =
  static member public INS=BL_{1}()
  public new()={{}}
  member x.Get{1}s (queryEntity:BQ_{1})=
    (DA_{1}.INS:>IDA_{1}).Get{1}s queryEntity
  member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Create{1} executeContent
  member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Create{1}s executeContent
  member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Update{1} executeContent
  member x.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Update{1}s executeContent
  member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1} executeContent
  member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1}s executeContent{3}",
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
      ,
      //{3}
      match tableName with
      | EqualsIn [JXCSHDJTableName;JHGLDJTableName] x->
          String.Format(@"{0}
  member x.Create{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Create{1}_CGJH executeContent
  member x.Update{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Update{1}_CGJH executeContent",
            //{0}
            String.Empty
            ,
            //{1}
            match x with
            | _ when x.StartsWith("T_") ->x.Remove(0,2)
            | _ -> x
            ,
            //{2}
            x)
      | _ ->String.Empty
       )

  static member private GetCodeWithTemplateTwo (tableName:string)=
    String.Format( @"{0}
type BL_{1} =
  static member public INS=BL_{1}()
  public new()={{}}
  member x.Get{1}s (queryEntity:BQ_{1})=
    (DA_{1}.INS:>IDA_{1}).Get{1}s queryEntity",
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

  static member private GetCodeWithTemplateThree (tableName:string)=
    String.Format( @"{0}
type BL_{1} =
  static member public INS=BL_{1}()
  public new()={{}}
  member x.Get{1}s (queryEntity:BQ_{1})=
    (DA_{1}.INS:>IDA_{1}).Get{1}s queryEntity
  member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Create{1} executeContent
  member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Create{1}s executeContent
  member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Update{1} executeContent
  member x.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Update{1}s executeContent
  member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1} executeContent
  member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Delete{1}s executeContent
  member x.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    (DA_{1}.INS:>IDA_{1}).Batch{1}s executeContent",
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
