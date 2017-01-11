

#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Windows.Forms.dll"
#r "FSharp.PowerPack.Linq.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
open System
open System.Reflection
//----------------------------------------------------------------------------------------------
//#I @"C:\Program Files (x86)\Microsoft Enterprise Library 4.1 - October 2008\Bin"
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "Microsoft.Practices.Unity"
//#r "Microsoft.Practices.ObjectBuilder2.dll"
//let dllPath= @"file:////C:/Program Files (x86)/Microsoft Enterprise Library 5.0/Bin"
//AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", dllPath)
//let m = typeof<AppDomainSetup>.GetMethod("UpdateContextProperty", BindingFlags.NonPublic ||| BindingFlags.Static)
//let funsion = typeof<AppDomain>.GetMethod("GetFusionContext", BindingFlags.NonPublic ||| BindingFlags.Instance)
//m.Invoke(null, [| box (funsion.Invoke(AppDomain.CurrentDomain, null)); box "PRIVATE_BINPATH";dllPath|])  |>ignore
//----------------------------------------------------------------------------------------------
open Microsoft.FSharp.Collections
open System.Collections.Generic
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open System.Diagnostics
open Microsoft.FSharp.Linq
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.Practices.ObjectBuilder2
open System.Windows.Forms
//----------------------------------------------------------------------------------------------
//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
//----------------------------------------------------------------------------------------------
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation
//----------------------------------------------------------------------------------------------

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//System.AppDomain.CurrentDomain.BaseDirectory
//ConfigurationManager.ConnectionStrings
//先将WX.Data.FHelper.Service.Config,中的默认数据库配置改为<dataConfiguration defaultDatabase="SBIIMSAC" />
//===================================================================

let tableNames=["T_DJ_GHS"]
let typedTableInfoSeq= Generator.AttachRelatedInfo tableNames
DataAccessCodingAdvance.GetCode  typedTableInfoSeq |>Clipboard.SetText

"wx".Split ([|'_'|],StringSplitOptions.RemoveEmptyEntries)
//===================================================================

//单个表代码
["T_GHS"]
|>fun a ->
    match DatabaseInformation.ValidateTablesDesign a|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
    | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
    | x ->ObjectDumper.Write(x,1)
    a
|>Generator.AttachRelatedInfo
//|> DataAccessCodingAdvance.GetCode 
|>DataAccessCodingAdvanceWithoutVariable.GetCode  "SBIIMSAC"
|>Clipboard.SetText

//////////////////////////////////////////////////////////////////////////////////////////////////

//单个代码文件生成
DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
//|>fun a ->
//    match DatabaseInformation.ValidateTablesDesign a|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
//    | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
//    | x ->ObjectDumper.Write(x,1)
//    a
//|>ignore
|>Generator.AttachRelatedInfo
//|>Seq.map (fun a->ObjectDumper.Write(a.TableName)|>ignore,ObjectDumper.Write(a.ColumnConditionType)|>ignore)
//|>DataEntitiesCodingAdvance.GetCode
//|>DataAccessCodingAdvance.GetCode 
|>DataAccessCodingAdvanceWithoutVariableWithArray.GetCode  "SBIIMS_AC"
//|>BusinessLogicCodingAdvance.GetCode
//|>ServiceContractCodingAdvance.GetCode
//|>WcfServiceDevelopmentCodingAdvance.GetCode "SBIIMSAC"
//|>QueryEntitiesCodingAdvanceServerSide.GetCode
//|>QueryEntitiesCodingAdvanceClientSide.GetCode
|>Clipboard.SetText

//////////////////////////////////////////////////////////////////////////////////////////////////
//生成或更新所有的代码文件
//DatabaseInformation.GetViewInfo
let db=DatabaseFactory.CreateDatabase()

//#load "D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\DatabaseInformation.fs"
//open WX.Data.CodeAutomation

(*
DatabaseInformation.GetColumnSchemal4Way "T_GNJD"
|>ObjectDumper.Write

let watch=new Stopwatch()
watch.Restart()
DatabaseInformation.GetTableInfo
//|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
//|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".AC" "SBIIMS_AC" @"D:\Workspace\ACBaseAutomation1" None
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds
*)

DatabaseInformation.GetColumnSchemal4Way "T_GNJD"
|>ObjectDumper.Write

let watch=new Stopwatch()
watch.Restart()
DatabaseInformation.GetTableInfo
//|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
//|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".AC" "SBIIMS_AC" @"D:\Workspace\SBIIMS\SBIIMS_AC\BaseAutomation" None
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds

//////////////////////////////////////////////////////////////////////////////

//生成或更新所有的代码文件
//DatabaseInformation.GetViewInfo
DatabaseInformation.GetTableInfo
//|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
//|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>AdvanceTypeGenerator.GenerateCodeFile @"D:\Workspace\SBIIMS13"




////////////////////////////////////////////////////////////////////////////////////////////////////////////
//Update DataAccessCoding code
DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>fun a->(DatabaseInformation.ValidateTablesDesign a,1)|>ObjectDumper.Write;a
|>Generator.AttachRelatedInfo
|>DataAccessCodingAdvance.GetCode
//|>DataAccessCodingForAdoNet4.GetCode
//|>DataEntitiesCodingAdvance.GetCode
|>Clipboard.SetText

////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////

String.Format("{1}{0}","wx","wx1")

let tableNames=
  DatabaseInformation.GetTableInfo
  |>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
  |>Seq.map (fun a->a.TABLE_NAME)
  |>Seq.toList
let typedTableInfoSeq= Generator.AttachRelatedInfo tableNames
ObjectDumper.Write (tableNames,1)
ObjectDumper.Write (typedTableInfoSeq,1)

let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship "T_DJ_GHS"
ObjectDumper.Write (tableAsPKRelationships,1)
let mainChildRelationshipOneLevel=
  tableAsPKRelationships 
  |>Seq.filter (fun a->
      let pkColumns=
        a.FK_TABLE 
        |>DatabaseInformation.GetPKColumns
      Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
      &&
       pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) 


let tableNames=["T_DJ_GHS"]
DatabaseInformation.ValidateTablesDesign tableNames
ObjectDumper.Write (tableAsPKRelationships,1)



//////////////////////////////////////////////////////////////////////////////////////////////////

let db=DatabaseFactory.CreateDatabase()
let conn=db.CreateConnection()
conn.Open()
let restrictionValues=[|null;"dbo";null;"VIEW"|] 
let dataTable=conn.GetSchema("Tables",restrictionValues)
dataTable.Rows.Count

let restrictionValues=[|null;"dbo";null|] 
(*
Catalog TABLE_CATALOG 1
Owner TABLE_SCHEMA 2
Table TABLE_NAME 3
Table COLUMN_NAME 4
*)
let dataTable=conn.GetSchema("Views",restrictionValues)
dataTable.Rows.Count
conn.Close()
for a in dataTable.Columns do
  ObjectDumper.Write ((a.ColumnName,string a.DataType))
//ObjectDumper.Write dataTables

