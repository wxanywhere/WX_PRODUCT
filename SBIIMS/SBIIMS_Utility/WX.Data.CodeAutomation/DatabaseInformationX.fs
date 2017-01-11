namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.Linq
open System.Data
open System.Data.SqlClient
open Microsoft.FSharp.Linq
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data

open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomation

[<RequireQualifiedAccess>]
module DatabaseInformationX=
  //得到单据类型信息
  let GetDJLX=
    let sourceDataMemory:T_DJLX list ref=ref []
    (fun (databaseName) ->   //closures, 也可用静态字段来或属性来代替
      let GetSourceData (databaseName)=
        seq{
          let db=DatabaseFactory.CreateDatabase(databaseName)
          let sqlText=
            @"
            SELECT C_DM,C_LX,C_QZ,C_BZ FROM T_DJLX 
            "
          use cmd=new SqlCommand(sqlText)
          use reader=db.ExecuteReader cmd
          while reader.Read() do
            yield 
              {C_DM=reader.["C_DM"].ToString()|>byte;
              C_LX=reader.["C_LX"].ToString();
              C_QZ=reader.["C_QZ"].ToString();
              C_BZ=reader.["C_BZ"].ToString()}
            } 
        |>Seq.toList //保证数据的即时可用性 
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData (databaseName)
      )