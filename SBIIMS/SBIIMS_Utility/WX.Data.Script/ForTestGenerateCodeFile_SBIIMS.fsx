(*
如果因版本设计变更，不能将配置信息绑定到Fsi Session 的话，则将依赖的组件拷贝至"C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0"即可
*)

#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Windows.Forms.dll"
#r "FSharp.PowerPack.Linq.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
#r "Microsoft.ServiceBus.dll"
//----------------------------------------------------------------------------------------------
// #I @"C:\Program Files (x86)\Microsoft Enterprise Library 4.1 - October 2008\Bin"
// #I @"C:\WINDOWS\assembly\GAC\"
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
//----------------------------------------------------------------------------------------------
open Microsoft.FSharp.Collections
open System
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open System.Diagnostics
open Microsoft.FSharp.Linq
open Microsoft.ServiceBus
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

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//System.Configuration.ConfigurationManager.ConnectionStrings
//先将WX.Data.FHelper.Service.Config,中的默认数据库配置改为<dataConfiguration defaultDatabase="SBIIMS" />
//=================================================================================


(*
let db=DatabaseFactory.CreateDatabase()
use conn=db.CreateConnection()
conn.Open()
let restrictionValues=[|null;"dbo";"T_DJ_KH";null|] 
let dataTable=conn.GetSchema("IndexColumns",restrictionValues)
ObjectDumper.Write (dataTable,2)
conn.Close()
*)

DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->
      DatabaseInformation.GetColumnSchemal4Way a.TABLE_NAME
      |>Seq.exists (fun a->a.COLUMN_NAME="C_XBH" )
      )
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.sortBy (fun a->a)
|>ObjectDumper.Write


DatabaseInformation.GetIndexedColumns "T_SP"
|>ObjectDumper.Write

DatabaseInformation.GetPKColumns "T_DJ_KH"
|>ObjectDumper.Write

//生成QueryEntity的属性片段代码
("VC_FZR","string",true)
|>QueryEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

"T_ZZ_DWBM"
//|>DatabaseInformation.GetColumnSchemal4Way
//|>DatabaseInformation.GetAsPKRelationship
|>DatabaseInformation.GetAsFKRelationship
//|>DatabaseInformation.GetPKColumns
//|>PSeq.filter (fun a->a.FK_TABLE="T_ZZ_DWBM")
|>ObjectDumper.Write

//---------------------------------------------------------------------------------------------------------------
//单个表代码
//["T_BJ_XSGL";"T_DJ_JHGL"]
["T_SP"]
//|>fun a ->
//    match DatabaseInformation.ValidateTablesDesign a|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
//    | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
//    | x ->ObjectDumper.Write(x,1)
//    a
|>Generator.AttachRelatedInfo
//|>DataEntitiesCodingAdvanceWithArray.GetCode
//|>DataAccessCodingAdvance.GetCode 
//|>TableFKInstanceDictionaryCoding.GetCode
//|>TableNameDictionaryCoding.GetCode
//|>IDataAccessCodingAdvanceWithArray.GetCode "SBIIMS_JXC"
|>DataAccessCodingAdvanceWithoutVariableWithArray.GetCode  "SBIIMS_JXC"
//|>DataAccessModuleSignatureCodingAdvance.GetCode  "SBIIMS_JXC"
//|>DataAccessModuleCodingAdvance.GetCode  "SBIIMS_JXC"
//|>DataAccessExtensionCodingAdvance.GetCode  "SBIIMS_JXC"
//|>BusinessLogicCodingAdvance.GetCode
//|>ServiceContractCodingAdvanceWithArray.GetCode "SBIIMS_JXC"
//|>QueryEntitiesCodingAdvance.GetCode
//|>QueryEntitiesCodingAdvanceServerSide.GetCode
//|>QueryEntitiesCodingAdvanceClientSide.GetCode
|>Clipboard.SetText

//---------------------------------------------------------------------------------------------------------------

let x=
  DatabaseInformation.GetColumnSchemal2Way "T_YG_TEST"
  |>Seq.filter (fun a->a.COLUMN_NAME="C_ZP")
//---------------------------------------------------------------------------------------------------------------

//验证
DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>fun a ->
    match DatabaseInformation.ValidateTablesDesign a|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
    | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
    | x ->ObjectDumper.Write(x,1)
    a
|>ignore

DatabaseInformation.ValidateTablesDesign ["T_SP"]


//---------------------------------------------------------------------------------------------------------------

let codeText=
  //单个代码文件生成
  DatabaseInformation.GetTableInfo
  |>Seq.filter (fun a->a.TABLE_NAME|>Comm.isEqualsIn ["sysdiagrams";"T_A";"T_AB"] |>not)
  |>Seq.map (fun a->a.TABLE_NAME)
  |>Seq.toList
  //validation
  //|>fun a ->
  //    match DatabaseInformation.ValidateTablesDesign a|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
  //    | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
  //    | x ->ObjectDumper.Write(x,1)
  //    a
  //|>ignore
  |>Generator.AttachRelatedInfo
  //|>Seq.map (fun a->ObjectDumper.Write(a.TableName)|>ignore,ObjectDumper.Write(a.ColumnConditionType)|>ignore)
  //|>DataEntitiesCodingAdvanceWithArray.GetCode
  //|>DataAccessCodingAdvance.GetCode 
  //|>TableFKInstanceDictionaryCoding.GetCode
  //|>TableNameDictionaryCoding.GetCode
  //|>IDataAccessCodingAdvanceWithArray.GetCode "SBIIMS_JXC"
  //|>DataAccessCodingAdvanceWithoutVariableWithArray.GetCode  "SBIIMS_JXC"
  //|>DataAccessModuleSignatureCodingAdvance.GetCode  "SBIIMS_JXC"
  |>DataAccessModuleCodingAdvance.GetCode  "SBIIMS_JXC"
  //|>DataAccessExtensionCodingAdvance.GetCode  "SBIIMS_JXC"
  //|>BusinessLogicCodingAdvance.GetCode
  //|>ServiceContractCodingAdvanceWithArray.GetCode "SBIIMS_JXC"
  //|>QueryEntitiesCodingAdvance.GetCode
  //|>QueryEntitiesCodingAdvanceServerSide.GetCode
  //|>QueryEntitiesCodingAdvanceClientSide.GetCode
codeText |> Clipboard.SetText

//======================================================================
//生成或更新所有的代码文件
//DatabaseInformation.GetViewInfo
let watch=new Stopwatch()
watch.Restart()
DatabaseInformation.GetTableInfo
//|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
//|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
//|>Seq.filter (fun a->false)
|>Seq.filter (fun a->a.TABLE_NAME|>Comm.isEqualsIn ["sysdiagrams";"T_A";"T_AB"] |>not)
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".JXC" "SBIIMS_JXC" @"D:\Workspace\SBIIMS\SBIIMS_JXC\BaseAutomation" (Some 10)
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds

//----------------------------------------------------------------------------------------------------------------------------

DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".Frame" "SBIIMS_Frame" @"D:\Workspace\SBIIMS\SBIIMS_Frame\BaseAutomation" None

//----------------------------------------------------------------------------------------------------------------------------

DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".VC" "SBIIMS_VC" @"D:\Workspace\SBIIMS\SBIIMS_VC\BaseAutomation" None

//----------------------------------------------------------------------------------------------------------------------------

DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".FK" "SBIIMS_FK" @"D:\Workspace\SBIIMS\SBIIMS_FK\BaseAutomation" None

DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".APC" "SBIIMS_APC" @"D:\Workspace\SBIIMS\SBIIMS_APC\BaseAutomation" None


//----------------------------------------------------------------------------------------------------------------------------

DatabaseInformation.GetTableInfo
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.map (fun a->a.TABLE_NAME)
|>Seq.toList
|>Generator.GenerateCodeFile ".LT" "SBIIMS_LT" @"D:\Workspace\SBIIMS\SBIIMS_LT\BaseAutomation" None

//======================================================================

//高级类生成
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


let tableNames=["BD_T_DJ_SH"]
DatabaseInformation.ValidateTablesDesign tableNames
ObjectDumper.Write (tableAsPKRelationships,1)

let tableName="T_DJ_SH"
//DatabaseInformation.GetColumnSchemal2Way tableName
DatabaseInformation.GetColumnSchemal4Way tableName
|>fun a->ObjectDumper.Write a
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

