namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type ServiceContractCoding=
  static member GetCode (tableNames:string list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.ServiceContracts
open System
open System.ServiceModel
open System.Collections.Generic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
[<ServiceContract>]
type IWS_SBIIMS ="
      ) |>ignore
    for tableName in tableNames do
    (*
如果使用  [<OperationContract>] abstract GetCKs:BQ_CK->List<BD_T_CK>, , 编译时正确，但在启动服务时将报错
System.ArgumentNullException: All parameter names used in operations that make up a service contract must not be null.
Parameter name: name
应该增加参数名称，以下是正确的
[<OperationContract>] abstract GetCKs:queryEntity:BQ_CK->List<BD_T_CK>
    *)
      sb.AppendFormat( @"{0}
  [<OperationContract>] abstract Get{1}s:queryEntity:BQ_{1}->BD_QueryResult<BD_{2}[]>
  [<OperationContract>] abstract Create{1}:executeContent:BD_ExecuteContent<#BD_{2}>->int
  [<OperationContract>] abstract Update{1}:executeContent:BD_ExecuteContent<#BD_{2}>->int
  [<OperationContract>] abstract Delete{1}:executeContent:BD_ExecuteContent<#BD_{2}>->int
  [<OperationContract>] abstract Delete{1}s:executeContent:BD_ExecuteContent<#BD_{2}[]>->int",
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

