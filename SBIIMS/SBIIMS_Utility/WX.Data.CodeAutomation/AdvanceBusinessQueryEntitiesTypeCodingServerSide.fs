namespace WX.Data.CodeAutomation
open System
open System.IO
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceBusinessQueryEntitiesTypeCodingServerSide=
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
open System.Runtime.Serialization
[<DataContract>]
type BQ_{0}_Advance()=
  inherit BQ_{0}()",
                match a.TableName with
                | x when x.StartsWith("T_") ->x.Remove(0,2)
                | x -> x)|>ignore
              (Path.Combine(baseDirectory,CodeLayerPath.QueryEntitiesServerAdvance assemblySuffix ,"BQAdvance",
                String.Format("BQ_{0}_Advance.fs",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)))
              |>File.WriteFileCreateOnly (sb.ToString().TrimStart())
          )
      Path.Combine(baseDirectory,CodeLayerPath.QueryEntitiesServerAdvance assemblySuffix,"BQAdvance","BQ_X_Advance")
    with e ->ObjectDumper.Write e; raise e


      
