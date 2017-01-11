namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper

(*
已停用
该扩展的方法只能用特定的对象，故而应使用对象式方法扩展，即使用type...with进行方法扩展

1. 由于module下let绑定的方法无法显示参数名称，因此不适用于EntityKey源数据的获取
*)
type DataAccessExtensionCodingAdvance=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      DataAccessExtensionCodingAdvance.GenerateNameSpaceCommonCode()
      |>string|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      for a in tableRelatedInfos do
        match a.TableTemplateType with
        | MainTableWithOneLevel 
        | MainTableWithTwoLevels
        | IndependentTable
        | ChildTable
        | LeafTable ->
            DataAccessExtensionCodingAdvance.GenerateGetEntityKeySourceCode  a.TableName
            |>string|>sb.Append|>ignore
        | _ -> ()
      string sb
    with 
    | e ->ObjectDumper.Write(e,2); raise e

  static member private GenerateNameSpaceCommonCode ()=
    @"namespace WX.Data.DataAccess
open System
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities

type DA="

  static member private GenerateGetEntityKeySourceCode (tableName:string)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let tableKeyColumns=DatabaseInformation.GetPKColumns tableName 
    sb.AppendFormat(@"
  static member Get{0}EntityKeySource {1} = ""{0}"",new {0}({2})",
      //{0}
      tableName,
      //{1}
      (
      sbTem.Clear()|>ignore
      for m in tableKeyColumns  do
        sbTem.AppendFormat(@"{0},",
          //{0}
          m.COLUMN_NAME.FirstLetterToLower()
          )|>ignore
      match sbTem with
      | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
      | _ ->()
      match sbTem.ToString().TrimStart() with
      | w when w|>Seq.exists (fun a->a=',') ->"("+w+")"
      | w ->w 
      ),
      (*Right reference, 方案二，使用类型约束
      //{1}
      (
      sbTem.Clear()|>ignore
      let tableColumns=
        DatabaseInformation.GetColumnSchemal4Way tableName
        |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
      for m in tableKeyColumns  do
        sbTem.AppendFormat(@"{0}:{1},",
          //{0}
          m.COLUMN_NAME.FirstLetterToLower(),
          //{1}
          match tableColumns|>Seq.find (fun b->b.COLUMN_NAME=m.COLUMN_NAME ) with
          | x ->
              match x.IS_NULLABLE_TYPED,x.DATA_TYPE with 
              | true, EndsWithIn ["string";"[]"]  y  -> y
              | true,y -> "System.Nullable<"+y+">"
              | _,y ->y
          )|>ignore
      match sbTem with
      | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
      | _ ->()
      sbTem.ToString().TrimStart()
      ),
      *)
      //{2}
      (
      sbTem.Clear()|>ignore
      for m in tableKeyColumns  do
        sbTem.AppendFormat(@"{0}={1},",
          //{0}
          m.COLUMN_NAME,
          //{1}
          match m.COLUMN_NAME with
          | x ->x.FirstLetterToLower()
          )|>ignore
      match sbTem with
      | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
      | _ ->()
      sbTem.ToString().TrimStart()
      )
      )|>ignore
    sb.ToString()

