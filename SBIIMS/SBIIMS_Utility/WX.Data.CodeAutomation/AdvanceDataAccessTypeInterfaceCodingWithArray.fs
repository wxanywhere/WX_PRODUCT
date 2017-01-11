namespace WX.Data.CodeAutomation
open System
open System.IO
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceDataAccessTypeInterfaceCodingWithArray=
  static member GenerateCodeFile   (assemblySuffix:string)  (baseDirectory:string) (tableRelatedInfos:TableRelatedInfo seq)  = 
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
              sb.AppendFormat( @"namespace WX.Data.IDataAccess
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
type IDA_{0}_Advance=
  inherit IDA_{0}
  abstract Get{0}View:BQ_{0}_Advance->BD_QueryResult<BD_{1}_Advance[]>",    
                //{0}
                match a.TableName with
                | x when x.StartsWith("T_") ->x.Remove(0,2)
                | x -> x
                ,
                //{1}
                a.TableName
                )|>ignore
              (Path.Combine(baseDirectory,CodeLayerPath.IDataAccessAdvance assemblySuffix,"Advance",
                String.Format("IDA_{0}_Advance.fs",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)))
              |>File.WriteFileCreateOnly (sb.ToString().TrimStart())
          )
      Path.Combine(baseDirectory,CodeLayerPath.IDataAccessAdvance assemblySuffix,"Advance","IDA_X_Advance")
    with e ->ObjectDumper.Write e; raise e