namespace WX.Data.CodeAutomation
open System
open System.IO
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceDataAccessTypeCodingWithArray=
  static member GenerateCodeFile   (assemblySuffix:string)  (baseDirectory:string) (tableRelatedInfos:TableRelatedInfo seq) = 
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
              sb.AppendFormat( @"namespace WX.Data.DataAccess
open System
open System.Data
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Logging
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
[<Sealed>]
type  DA_{0}_Advance=
  inherit DA_{0}
  static member public INS= DA_{0}_Advance() 
  private new ()= {{inherit DA_{0}()}}
  //interface IDA_{0}_Advance with",
                match a.TableName with
                | x when x.StartsWith("T_") ->x.Remove(0,2)
                | x -> x)|>ignore
              (Path.Combine(baseDirectory,CodeLayerPath.DataAccessAdvance assemblySuffix,"Advance",
                String.Format("DA_{0}_Advance.fs",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)))
              |>File.WriteFileCreateOnly (sb.ToString().TrimStart())
          )
      Path.Combine(baseDirectory,CodeLayerPath.DataAccessAdvance assemblySuffix,"Advance","DA_X_Advance")
    with e ->ObjectDumper.Write e; raise e