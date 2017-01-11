namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper

//单独生成子表的代码时，只生成查询代码，因为它不能单独进行更新
type DataAccessQueryModuleCodingAdvance=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      DataAccessQueryModuleCodingAdvance.GenerateModuleCommonCode()
      |>string|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      for a in tableRelatedInfos do
        match a.TableTemplateType with
        | MainTableWithOneLevel 
        | MainTableWithTwoLevels
        | IndependentTable
        | ChildTable
        | LeafTable ->
            DataAccessQueryModuleCodingAdvance.GenerateMapingToBusinessEntityCode  a.TableName
            |>string|>sb.Append|>ignore
        | _ -> ()
      sb.AppendLine()|>ignore
      for a in tableRelatedInfos do
        match a.TableTemplateType with
        | MainTableWithOneLevel 
        | MainTableWithTwoLevels
        | IndependentTable
        | ChildTable
        | LeafTable ->
            DataAccessQueryModuleCodingAdvance.GenerateGetEntityKeySourceCode  a.TableName
            |>string|>sb.Append|>ignore
        | _ -> ()
      string sb
    with 
    | e ->ObjectDumper.Write(e,2); raise e

  static member private VariableNames=
    ['x';'y';'z';'u';'v';'w';'a';'b';'c']
    
    (* Dynamic
    let intTem=ref 0
    intTem:=120 //char 120=x
    [for i=0 to 20 do
       yield char !intTem
       incr intTem
       match !intTem with  // x,y,z,u,v,w,a,b,c....
       | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
       | u when u=120 ->intTem:=97 // int 'a'=97
       | _ -> ()
       ]
    *)
  static member private GenerateModuleCommonCode ()=
    @"namespace WX.Data.DataAccess
open System
open WX.Data.DataModel.Query
open WX.Data.BusinessBase
open WX.Data.BusinessEntities

[<AutoOpen>]
module DataAccessModule="

  static member private GenerateMapingToBusinessEntityCode (tableName:string)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let tableColumns=
      DatabaseInformation.GetColumnSchemal4Way tableName
      |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    sb.AppendFormat( @"
  type BD_{0} with  
    member x.MappingFrom (dataModel:{0})= 
      match dataModel with
      | y ->
          {1}",
      //{0}
      tableName,
      //{1}
      (
      sbTem.Clear()|>ignore
      for m in tableColumns do
        sbTem.AppendFormat( @" 
          x.{0}<-y.{0}",
          //{0}
          m.COLUMN_NAME  
          )|>ignore
      sbTem.ToString().TrimStart()  //移出子代码模板第一行格式化时的所有空格
      )
      )|>ignore
    sb.ToString()

  static member private GenerateGetEntityKeySourceCode (tableName:string)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let tableKeyColumns=DatabaseInformation.GetPKColumns tableName 
    sb.AppendFormat(@"
  type {0} with  
    static member GetEntityKeySource {1} = ""{0}"",new {0}({2})",
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
