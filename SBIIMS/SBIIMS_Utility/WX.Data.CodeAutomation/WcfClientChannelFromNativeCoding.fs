namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type WcfClientChannelFromNativeCoding=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.AppendFormat( @"namespace WX.Data.ClientChannel
open System
open System.Runtime.Serialization
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic

type WS_{0}Channel() =
  static member public INS= WS_{0}Channel()"
      ,
      databaseInstanceName
      ) |>ignore
    sb.AppendLine()|>ignore
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels->
          WcfClientChannelFromNativeCoding.GetCodeWithTemplateOne a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | IndependentTable ->
          match a.ColumnConditionTypes with
          | ColumnConditionTypeContains [HasXH] _ ->
              WcfClientChannelFromNativeCoding.GetCodeWithTemplateThree a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | _ ->
              WcfClientChannelFromNativeCoding.GetCodeWithTemplateOne a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          WcfClientChannelFromNativeCoding.GetCodeWithTemplateTwo a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb

  static member private GetCodeWithTemplateOne (tableName:string)=
    String.Format( @"{0}
  member this.Get{1}s (queryEntity:BQ_{1})=
    BL_{1}.INS.Get{1}s queryEntity
  member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Create{1} executeContent
  member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Create{1}s executeContent
  member this.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Update{1} executeContent
  member this.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Update{1}s executeContent
  member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Delete{1} executeContent
  member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Delete{1}s executeContent{3}",
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
  member this.Create{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Create{1}_CGJH executeContent
  member this.Update{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Update{1}_CGJH executeContent",
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
  member this.Get{1}s (queryEntity:BQ_{1})=
    BL_{1}.INS.Get{1}s queryEntity",
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
  member this.Get{1}s (queryEntity:BQ_{1})=
    BL_{1}.INS.Get{1}s queryEntity
  member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Create{1} executeContent
  member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Create{1}s executeContent
  member this.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Update{1} executeContent
  member this.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Update{1}s executeContent
  member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    BL_{1}.INS.Delete{1} executeContent
  member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Delete{1}s executeContent
  member this.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    BL_{1}.INS.Batch{1}s executeContent",
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