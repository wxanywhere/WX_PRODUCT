namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type IDataAccessCoding=
  static member private GetCode (tableNames:string list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.IDataAccess
open System.Collections.Generic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities") |>ignore
    for tableName in tableNames do
      sb.AppendFormat( @"{0}
type IDA_{1}=
  abstract Get{1}s:BQ_{1}->BD_QueryResult<List<BD_{2}>>
  abstract Create{1}:BD_{2}->int  
  abstract Update{1}:BD_{2}->int
  abstract Delete{1}:BD_{2}->int
  abstract Delete{1}s:BD_{2}[]->int",
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
