namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type IDataAccessCodingAdvanceWithArray=
  static member GetCode  (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.IDataAccess
open System
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities") |>ignore
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels ->
          IDataAccessCodingAdvanceWithArray.GetCodeWithTemplateOne databaseInstanceName a.TableName a.ColumnConditionTypes
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | IndependentTable ->
          IDataAccessCodingAdvanceWithArray.GetCodeWithTemplateThree databaseInstanceName a
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          IDataAccessCodingAdvanceWithArray.GetCodeWithTemplateTwo databaseInstanceName a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb
       
  static member private GetCodeWithTemplateOne databaseInstanceName (tableName:string) (columnConditionTypes:ColumnConditionType[])=
    String.Format(@"
type IDA_{1}=
  abstract Get{1}s:BQ_{1}* ?context:{0}EntitiesAdvance->BD_QueryResult<BD_{2}[]>
  abstract Create{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Create{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Update{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Update{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract WriteLogForUpdate{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit
  abstract WriteLogForDelete{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit{3}{4}",
      //{0}
      databaseInstanceName
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
            databaseInstanceName
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
            databaseInstanceName
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

  static member private GetCodeWithTemplateTwo databaseInstanceName (tableName:string)=
    String.Format(@"
type IDA_{1}=
  abstract Get{1}s:BQ_{1}* ?context:{0}EntitiesAdvance->BD_QueryResult<BD_{2}[]>
  abstract WriteLogForUpdate{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit",
      //{0}
      databaseInstanceName
      ,
      //{1}
      match tableName with
      | x when x.StartsWith("T_") ->x.Remove(0,2)
      | x -> x
      ,
      //{2}
      tableName
      )

  static member private GetCodeWithTemplateThree databaseInstanceName (tableRelatedInfo:TableRelatedInfo)=
    String.Format(@"
type IDA_{1}=
  abstract Get{1}s:BQ_{1}* ?context:{0}EntitiesAdvance->BD_QueryResult<BD_{2}[]>
  abstract Create{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult
  abstract Create{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult{3}
  abstract Update{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Update{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}:BD_ExecuteContent<#BD_{2}>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult
  abstract Delete{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime ->BD_ExecuteResult{4}
  abstract WriteLogForUpdate{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit
  abstract WriteLogForDelete{1}:{2}*BD_ExecuteBase*{0}EntitiesAdvance*DateTime->unit",
      //{0}
      databaseInstanceName
      ,
      //{1}
      match tableRelatedInfo.TableName with
      | x when x.StartsWith("T_") ->x.Remove(0,2)
      | x -> x
      ,
      //{2}
      tableRelatedInfo.TableName
      ,
      //{3}
      match tableRelatedInfo.ColumnConditionTypes,tableRelatedInfo.TableRelationshipTypes,tableRelatedInfo.TableName with
      | ColumnConditionTypeNotContains [HasJYH;HasLSH] _, TableRelationshipTypeNotContains [WithZZB] x , y ->
          match x with
          | TableRelationshipTypeContains [WithForeignKeyRelatedTable] _ ->  
              String.Format(@"
  abstract CreateConcurrent{1}:BD_ExecuteContent<#BD_{2}>* context:{0}EntitiesAdvance* currentDateTime:DateTime ->{2} option
  abstract CreateConcurrent{1}s:BD_ExecuteContent<#BD_{2}[]>* context:{0}EntitiesAdvance* currentDateTime:DateTime ->{2}[]",
                //{0}
                databaseInstanceName
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
      match tableRelatedInfo.ColumnConditionTypes,tableRelatedInfo.TableName with
      | ColumnConditionTypeContains [HasXH] _, y->
          String.Format(@"
  abstract Batch{1}s:BD_ExecuteContent<#BD_{2}[]>* ?context:{0}EntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult",
            //{0}
            databaseInstanceName
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
