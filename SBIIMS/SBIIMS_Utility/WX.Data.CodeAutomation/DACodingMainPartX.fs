
(*
ADO.NET Entity Framework Exception Process

System.Data..::.UpdateException
Namespace:  System.Data
Assembly:  System.Data.Entity (in System.Data.Entity.dll) 
http://msdn.microsoft.com/en-us/library/system.data.updateexception.aspx

http://msdn.microsoft.com/en-us/library/bb738618.aspx
Saving Changes and Managing Concurrency (Entity Framework)
System.Data.OptimisticConcurrencyException

Adding, Modifying, and Deleting Objects (Entity Framework)
http://msdn.microsoft.com/en-us/library/bb738695.aspx

.NET Framework Data Providers (ADO.NET) Search with Exception
http://msdn.microsoft.com/en-us/library/a6cd7c08.aspx

*)
namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper
open WX.Data.Database

//单独生成子表的代码时，只生成查询代码，因为它不能单独进行更新
type DACodingMainPartX=
  static member GetCode (databaseInstanceName:string) (entityContextNamePrefix:string) (tableRelatedInfos:TableRelatedInfoX[]) (tableInfos:TableInfo[])=  //结果为，命名空间部分*Seq<表名,代码>]static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      DACodingMainPartX.GenerateNamespaceCode
      |>string|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      for n in tableRelatedInfos do
        match n.TableTemplateType with
        | MainTableWithOneLevel 
        | MainTableWithTwoLevels
        | IndependentTable
        | ChildTable
        | LeafTable ->
            DACodingMainPartX.GenerateTypeCode  n.TableInfo.TABLE_NAME
            |>string|>sb.Append|>ignore
        | _ -> ()
        match n.TableTemplateType with
        | MainTableWithOneLevel ->
            DACodingMainChildOneLevelTablePartX.GetCodeWithMainChildTableOneLevelTemplate databaseInstanceName entityContextNamePrefix n tableInfos
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | MainTableWithTwoLevels ->
            DACodingMainChildTwoLevelTablePartX.GetCodeWithMainChildTableTwoLevelTemplate databaseInstanceName entityContextNamePrefix n tableInfos 
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | IndependentTable ->
            DACodingIndependentTablePartX.GetCodeWithIndependentTable databaseInstanceName entityContextNamePrefix n tableInfos
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | ChildTable ->  //只能查询当前表和叶子表信息
            DACodingChildTablePartX.GetCodeWithChildTableTemplate databaseInstanceName entityContextNamePrefix n tableInfos
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | LeafTable -> 
            DACodingLeafTablePartX.GetCodeWithLeafTableTemplate databaseInstanceName entityContextNamePrefix n tableInfos
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | _ ->()    //不独立生成代码
        (*
        | DJLSHTable -> //单据流水号表
            () //不单独生成代码
        | LSHTable -> //基本信息流水号表
            () //不单独生成代码
        *)
      string sb
    with 
    | e ->ObjectDumper.Write(e,2); raise e
   

  static member private GenerateNamespaceCode=
    @"namespace WX.Data.DataAccess
open System
open System.Data
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open Microsoft.Practices.EnterpriseLibrary.Logging
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.ServerHelper"


   /// note: internal new () as this= {{}}    //it's right in the Format(...), it's must be like that, 关于 {{}}，http://msdn.microsoft.com/zh-cn/library/txafckwd(VS.95).aspx
  static member private GenerateTypeCode (mainTableName:string)=
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
type  DA_{1}=
  inherit DA_Base
  static member public INS= DA_{1}() 
  public new () = {{inherit DA_Base()}}
  interface IDA_{1} with",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  //--------------------------------------------------------------------------------------------------------------------------------------
 