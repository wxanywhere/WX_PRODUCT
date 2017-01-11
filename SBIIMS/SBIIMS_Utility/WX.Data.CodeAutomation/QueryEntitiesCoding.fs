namespace WX.Data.CodeAutomation

open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

//还应增加对 byte [] 的处理
type QueryEntitiesCoding=
  static member GetCode tableName=
    //let tableName ="T_DJ_JHGL"
    let columnsSeq=
      DatabaseInformation.GetColumnSchemal2Way tableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)

    let blankSapce="  "
    let sb=StringBuilder()
    sb.AppendFormat( @"namespace WX.Data.BusinessEntities

open System
open System.Runtime.Serialization

[<DataContract>]
type BQ_{0}()="
      ,
      tableName.Remove(0,2)  //移出表名的前缀'T_'
      )|>ignore
    sb.AppendLine() |>ignore

    for a in columnsSeq do
      sb.AppendFormat(@"{0}
  let  mutable _{1}:{2}={3}",
        //{0}
        String.Empty
        ,
        //{1}
        a.COLUMN_NAME
        ,
        //{2}
        match a.DATA_TYPE with
        |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]") -> x
        | x -> "System.Nullable<"+x+">"
        ,
        //{3}
        match a.DATA_TYPE with
        |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]")-> "null"
        |  x -> "System.Nullable<"+x+">()"
        )|>ignore
       
    sb.AppendLine() |>ignore
        
    for a in columnsSeq do
      sb.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=  _{1} <-v ",
        //{0}
        String.Empty
        ,
        //{1}
        a.COLUMN_NAME
        )|>ignore
      sb.AppendLine() |>ignore
      //sb.Append(Environment.NewLine) |>ignore
    sb.ToString()