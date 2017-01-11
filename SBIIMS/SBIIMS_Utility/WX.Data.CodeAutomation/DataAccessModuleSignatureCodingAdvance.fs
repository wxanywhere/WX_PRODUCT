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
*)
//单独生成子表的代码时，只生成查询代码，因为它不能单独进行更新
type DataAccessModuleSignatureCodingAdvance=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      DataAccessModuleSignatureCodingAdvance.GenerateModuleCommonCode()
      |>string|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      for a in tableRelatedInfos do
        match a.TableTemplateType with
        | MainTableWithOneLevel 
        | MainTableWithTwoLevels
        | IndependentTable
        | ChildTable
        | LeafTable ->
            DataAccessModuleSignatureCodingAdvance.GenerateMapingToBusinessEntitySignatureCode  a.TableName
            |>string|>sb.Append|>ignore
        | _ -> ()
      string sb
    with 
    | e ->ObjectDumper.Write(e,2); raise e

  static member private GenerateModuleCommonCode ()=
    @"namespace WX.Data.DataAccess
open System
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities

[<AutoOpen>]
module DataAccessModule="

  static member private GenerateMapingToBusinessEntitySignatureCode (tableName:string)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let tableColumns=
      DatabaseInformation.GetColumnSchemal4Way tableName
      |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    sb.AppendFormat( @"
  val MappingToBD_{0}: dataModel:{0} -> businessEntity:BD_{0} -> unit",
      //{0}
      tableName
      )|>ignore
    sb.ToString()

  static member private GenerateGetEntityKeySourceSignatureCode (tableName:string)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let tableKeyColumns=DatabaseInformation.GetPKColumns tableName 
    let tableColumns=
      DatabaseInformation.GetColumnSchemal4Way tableName
      |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    sb.AppendFormat(@"
  val Get{0}EntityKeySource:{1} string*{0}",
      //{0}
      tableName,
      //{1}
      (
      sbTem.Clear()|>ignore
      for m in tableKeyColumns  do
        sbTem.AppendFormat(@"{0}:{1} ->",
          //{0}
          m.COLUMN_NAME.FirstLetterToLower(),
          //{1}
          match tableColumns|>Seq.find (fun b->b.COLUMN_NAME=m.COLUMN_NAME ) with
          | x ->
              match x.IS_NULLABLE_TYPED,x.DATA_TYPE.Replace("System.",String.Empty) with 
              | true, EndsWithIn ["string";"[]"]  y  -> y
              | true,y -> "Nullable<"+y+">"
              | _,y ->y
          )|>ignore
      sbTem.ToString().TrimStart()
      )
      )|>ignore
    sb.ToString()

