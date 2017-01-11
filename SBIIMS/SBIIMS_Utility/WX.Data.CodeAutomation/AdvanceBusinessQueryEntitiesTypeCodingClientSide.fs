namespace WX.Data.CodeAutomation
open System
open System.IO
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceBusinessQueryEntitiesTypeCodingClientSide=
  static member GenerateCodeFile  (assemblySuffix:string)  (baseDirectory:string) (tableRelatedInfos:TableRelatedInfo seq) = 
    try
      let sb=StringBuilder()
      tableRelatedInfos
      |>Seq.iter (fun a->
          match a.TableTemplateType with
          | DJLSHTable
          | LSHTable
          | PCLSHTable
          | JYLSHTable->()
          | _ ->
              let _=sb.Clear()
              sb.AppendFormat( @"namespace WX.Data.BusinessEntities
open System
open System.Windows
open WX.Data
type BQ_{0}_Client_Advance()=
  inherit BQ_{0}_Client()",
                match a.TableName with
                | x when x.StartsWith("T_") ->x.Remove(0,2)
                | x -> x)|>ignore
              (Path.Combine(baseDirectory,CodeLayerPath.QueryEntitiesClientAdvance assemblySuffix ,"BQClientAdvance",
                String.Format("BQ_{0}_Client_Advance.fs",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)))
              |>File.WriteFileCreateOnly (sb.ToString().TrimStart())
          )
      Path.Combine(baseDirectory,CodeLayerPath.QueryEntitiesClientAdvance assemblySuffix,"BQClientAdvance","BQ_X_Client_Advance")
    with e ->ObjectDumper.Write e; raise e


      
