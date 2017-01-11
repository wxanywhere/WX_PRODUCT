namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type IDACodingX=
  static member GetCode  (entityContextNamePrefix:string) (tableRelatedInfos:TableRelatedInfoX[])=  
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.IDataAccess
open System
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities") |>ignore
    for n in tableRelatedInfos do
      match n.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels ->
          IDACodingX.GetCodeWithTemplateOne entityContextNamePrefix n.TableInfo.TABLE_NAME n.ColumnConditionTypes
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | IndependentTable ->
          IDACodingX.GetCodeWithTemplateThree entityContextNamePrefix n
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          IDACodingX.GetCodeWithTemplateTwo entityContextNamePrefix n.TableInfo.TABLE_NAME
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb
       
  static member private GetCodeWithTemplateOne entityContextNamePrefix (tableName:string) (columnConditionTypes:ColumnConditionType[])=
    String.Format(@"
type IDA_{1}=
  abstract Create{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Create{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Update{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Update{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract WriteLogForUpdate{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit
  abstract WriteLogForDelete{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit{3}{4}",
      //{0}
      entityContextNamePrefix
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
      match columnConditionTypes with
      | ColumnConditionTypeContains [HasDJLSH;HasPCInChild] _ ->  
          String.Format(@"
  abstract Create{1}_WithPCProcess:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Update{1}_WithPCProcess:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult",
            //{0}
            entityContextNamePrefix
            ,
            //{1}
            match tableName with
            | x when x.StartsWith("T_") ->x.Remove(0,2)
            | x -> x
            ,
            //{2}
            tableName)
      | _ ->String.Empty
      ,
      //{4}  //考虑取消一下方法，使用...WithPCProcess方法即可
      match tableName with
      | EqualsIn [JXCSHDJTableName;JHGLDJTableName] x->
          String.Format(@"
  abstract Create{1}_CGJH:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Update{1}_CGJH:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult",
            //{0}
            entityContextNamePrefix
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

  static member private GetCodeWithTemplateTwo entityContextNamePrefix (tableName:string)=
    String.Format(@"
type IDA_{1}=
  abstract WriteLogForUpdate{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit",
      //{0}
      entityContextNamePrefix
      ,
      //{1}
      match tableName with
      | x when x.StartsWith("T_") ->x.Remove(0,2)
      | x -> x
      ,
      //{2}
      tableName
      )

  static member private GetCodeWithTemplateThree entityContextNamePrefix (tableRelatedInfo:TableRelatedInfoX)=
    String.Format(@"
type IDA_{1}=
  abstract Create{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Create{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult{3}
  abstract Update{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Update{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult{4}
  abstract WriteLogForUpdate{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit
  abstract WriteLogForDelete{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit",
      //{0}
      entityContextNamePrefix
      ,
      //{1}
      match tableRelatedInfo.TableInfo.TABLE_NAME with
      | x when x.StartsWith("T_") ->x.Remove(0,2)
      | x -> x
      ,
      //{2}
      tableRelatedInfo.TableInfo.TABLE_NAME
      ,
      //{3}
      match tableRelatedInfo.ColumnConditionTypes,tableRelatedInfo.TableRelationshipTypes,tableRelatedInfo.TableInfo.TABLE_NAME with
      | ColumnConditionTypeNotContains [HasJYH;HasLSH] _, TableRelationshipTypeNotContains [WithZZB] x , y ->
          match x with
          | TableRelationshipTypeContains [WithForeignKeyRelatedTable] _ ->  
              String.Format(@"
  abstract CreateConcurrent{1}:BD_ExecuteContent<#BD_{2}>* context:{0}EntitiesAdvance* currentDateTime:DateTime ->{2} option
  abstract CreateConcurrent{1}s:BD_ExecuteContent<#BD_{2}[]>* context:{0}EntitiesAdvance* currentDateTime:DateTime ->{2}[]",
                //{0}
                entityContextNamePrefix
                ,
                //{1}
                match y with
                | _ when y.StartsWith("T_") ->y.Remove(0,2)
                | _ -> y
                ,
                //{2}
                y
                )
          | _ ->String.Empty
      | _ ->String.Empty
      ,
      //{4}
      match tableRelatedInfo.ColumnConditionTypes,tableRelatedInfo.TableInfo.TABLE_NAME with
      | ColumnConditionTypeContains [HasXH] _, y->
          String.Format(@"
  abstract Batch{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult",
            //{0}
            entityContextNamePrefix
            ,
            //{1}
            match y with
            | _ when y.StartsWith("T_") ->y.Remove(0,2)
            | _ -> y
            ,
            //{2}
            y
            )
      | _ ->String.Empty
      )
