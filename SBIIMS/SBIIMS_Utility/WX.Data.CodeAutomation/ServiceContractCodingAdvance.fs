namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type ServiceContractCodingAdvance=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.AppendFormat( @"namespace WX.Data.ServiceContracts
open System
open System.ServiceModel
open System.Collections.Generic
open WX.Data.BusinessBase 
open WX.Data.BusinessEntities
[<ServiceContract>]
type IWS_{0} ="
       ,
       databaseInstanceName
       ) |>ignore
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels
      | IndependentTable ->
          ServiceContractCodingAdvance.GetCodeWithTemplateOne a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          ServiceContractCodingAdvance.GetCodeWithTemplateTwo a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb

  static member private GetCodeWithTemplateOne (tableName:string)=
    (*
如果使用  [<OperationContract>] abstract GetCKs:BQ_CK->List<BD_T_CK>, , 编译时正确，但在启动服务时将报错
System.ArgumentNullException: All parameter names used in operations that make up a service contract must not be null.
Parameter name: name
应该增加参数名称，以下是正确的
[<OperationContract>] abstract GetCKs:queryEntity:BQ_CK->List<BD_T_CK>
    *)
    String.Format( @"{0}
  [<OperationContract>] abstract Get{1}s:queryEntity:BQ_{1}->BD_QueryResult<List<BD_{2}>>
  [<OperationContract>] abstract Create{1}:executeContent:BD_ExecuteContent<#BD_{2}>->BD_ExecuteResult
  [<OperationContract>] abstract Create{1}s:executeContent:BD_ExecuteContent<#BD_{2}[]>->BD_ExecuteResult
  [<OperationContract>] abstract Update{1}:executeContent:BD_ExecuteContent<#BD_{2}>->BD_ExecuteResult
  [<OperationContract>] abstract Update{1}s:executeContent:BD_ExecuteContent<#BD_{2}[]>->BD_ExecuteResult
  [<OperationContract>] abstract Delete{1}:executeContent:BD_ExecuteContent<#BD_{2}>->BD_ExecuteResult
  [<OperationContract>] abstract Delete{1}s:executeContent:BD_ExecuteContent<#BD_{2}[]>->BD_ExecuteResult",
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
    String.Format( @"{0}
  [<OperationContract>] abstract Get{1}s:queryEntity:BQ_{1}->BD_QueryResult<List<BD_{2}>>",
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
