namespace WX.Data.CodeAutomation
open System
open System.IO
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceBusinessDataEntitiesTypeCoding=
  static member GenerateCodeFile   (assemblySuffix:string)   (baseDirectory:string) (tableRelatedInfos:TableRelatedInfo seq) =
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
open System.Collections.Generic
open System.Runtime.Serialization
open System.ComponentModel
open WX.Data
[<DataContract>]
type BD_{0}_Advance()=
  inherit BD_{0}()",
                a.TableName)|>ignore
              (Path.Combine(baseDirectory,CodeLayerPath.DataEntitiesAdvance assemblySuffix,"BDAdvance",String.Format("BD_{0}_Advance.fs",a.TableName)))
              |>File.WriteFileCreateOnly (sb.ToString().TrimStart())
          )  
      Path.Combine(baseDirectory,CodeLayerPath.DataEntitiesAdvance assemblySuffix,"BDAdvance","BD_T_X_Advance")
    with e ->ObjectDumper.Write e; raise e