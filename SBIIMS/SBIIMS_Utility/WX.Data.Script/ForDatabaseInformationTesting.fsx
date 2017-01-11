(*
如果因版本设计变更，不能将配置信息绑定到Fsi Session 的话，则将依赖的组件拷贝至"C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0"即可
*)

//It must load on sequence
#r "System.Configuration.dll"
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin" 
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
#r "System.Transactions.dll"
open System
open System.Data
open System.Data.SqlClient
open System.Configuration
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.FSharp.Collections

#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.Database.dll"
#r "WX.Data.CodeAutomation.dll"
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.Database
open WX.Data.CodeAutomation

ConfigHelper.INS.LoadDefaultServiceConfigToManager

//======================================================

let dbx=DataAccess.GetDatabaseInfo ("SBIIMS_JXC")
let db=dbx.Value
let r=
  db.TableInfos
  |>Seq.filter (fun a->a.TABLE_NAME="T_DJ_JHGL")
  |>Seq.toArray

ObjectDumper.Write r.[0].TableColumnInfos
ObjectDumper.Write r.[0].TablePrimaryKeyInfos
ObjectDumper.Write r.[0].TableIndexInfos
ObjectDumper.Write r.[0].TablePrimaryKeyRelationshipInfos
ObjectDumper.Write r.[0].TableForeignKeyRelationshipInfos

let db=DatabaseFactory.CreateDatabase("SBIIMS_VC")
let conn=db.CreateConnection()

let connStr="Data Source=192.168.2.199;Initial Catalog=SBIIMS_FK;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True"
let conn=new SqlConnection(connStr)
conn.Open()
let restrictionValues=[|null;"dbo";null;null|] 

conn.Close()
(*
let restrictionValues=[|null;"dbo";tableName;null|] 
*)
(*
http://msdn.microsoft.com/en-us/library/cc716722.aspx
Catalog TABLE_CATALOG 1
Owner TABLE_SCHEMA 2
Table TABLE_NAME 3
Table COLUMN_NAME 4
*)
(*
  Important********************************* 
  http://msdn.microsoft.com/en-us/library/ms187993.aspx 
  ntext , text, and image data types will be removed in a future version of Microsoft SQL Server. 
  Avoid using these data types in new development work, and plan to modify applications that currently use them. 
  Use nvarchar(max), varchar(max), and varbinary(max) instead. 
*)

let dataTable=conn.GetSchema("Columns",restrictionValues)
dataTable.Rows
|>Seq.cast<DataRow>
|>Seq.filter (fun a->
    match a.["DATA_TYPE"].ToString() with
    | EqualsIn ["ntext";"text";"image"]  _ ->true
    | _ ->false
    )
|>Seq.map (fun a->a.["TABLE_NAME"].ToString()+"."+a.["COLUMN_NAME"].ToString())
|>Seq.iter (fun a->ObjectDumper.Write (a,1))


"T_JYTZ_XZGL"
|>DatabaseInformation.GetColumnSchemal4Way
|>Seq.iter (fun a->ObjectDumper.Write  ("x."+a.COLUMN_NAME+"<-"))

//为Int的字段
for a in DatabaseInformation.GetTableInfo|>PSeq.sortBy (fun a->a.TABLE_NAME) do
  for b in 
    a.TABLE_NAME
    |>DatabaseInformation.GetColumnSchemal4Way do
    match b.DATA_TYPE.ToLowerInvariant(), b.NUMERIC_SCALE,b.COLUMN_NAME with
    | x,y,z when x.EndsWith("int32") -> ObjectDumper.Write (a.TABLE_NAME+"."+b.COLUMN_NAME)
    | _ ->() 

//为bigInt的字段
for a in DatabaseInformation.GetTableInfo|>PSeq.sortBy (fun a->a.TABLE_NAME) do
  for b in 
    a.TABLE_NAME
    |>DatabaseInformation.GetColumnSchemal4Way do
    match b.DATA_TYPE.ToLowerInvariant(), b.NUMERIC_SCALE,b.COLUMN_NAME with
    | x,y,z when x.EndsWith("int64") -> ObjectDumper.Write (a.TABLE_NAME+"."+b.COLUMN_NAME)
    | _ ->() 

//为Decimal的整数类型
for a in DatabaseInformation.GetTableInfo|>PSeq.sortBy (fun a->a.TABLE_NAME) do
  for b in 
    a.TABLE_NAME
    |>DatabaseInformation.GetColumnSchemal4Way do
    match b.DATA_TYPE.ToLowerInvariant(), b.NUMERIC_SCALE,b.COLUMN_NAME with
    | x,y,z when x.EndsWith("decimal") && y=0 -> ObjectDumper.Write (a.TABLE_NAME+"."+b.COLUMN_NAME)
    | _ ->() 

//数量字段
for a in DatabaseInformation.GetTableInfo|>PSeq.sortBy (fun a->a.TABLE_NAME) do
  for b in 
    a.TABLE_NAME
    |>DatabaseInformation.GetColumnSchemal4Way do
    match b.DATA_TYPE.ToLowerInvariant(), b.NUMERIC_SCALE,b.COLUMN_NAME with
    | x,y,z when x.EndsWith("decimal") && y=0 && z.EndsWith "SL" -> ObjectDumper.Write (a.TABLE_NAME+"."+b.COLUMN_NAME)
    | _ ->() 




let cd=DatabaseInformation.GetTableColumnDescription()

match 
  "T_FYLB_WF"
  |>DatabaseInformation.GetAsFKRelationship 
  |>PSeq.isEmpty with
| true ->true
| _ ->false

"T_FYLB_WF"
|>DatabaseInformation.GetAsFKRelationship
|>ObjectDumper.Write 

"T_AB"
|>DatabaseInformation.GetPKColumns
|>ObjectDumper.Write 

"T_AB"
|>DatabaseInformation.GetAsFKRelationship
|>ObjectDumper.Write 

//"T_GHS"
"T_ZZ_GHS"
//|>DatabaseInformation.GetColumnSchemal4Way
|>DatabaseInformation.GetAsFKRelationship 
//|>DatabaseInformation.GetAsPKRelationship
//|>DatabaseInformation.GetPKColumns
|>ObjectDumper.Write 


"T_ZZ_DWBM"
//|>DatabaseInformation.GetColumnSchemal4Way 
//|>DatabaseInformation.GetAsFKRelationship
|>DatabaseInformation.GetAsPKRelationship
//|>DatabaseInformation.GetPKColumns 
|>ObjectDumper.Write 

let tableColumns=
  DatabaseInformation.GetColumnSchemal4Way tableName
  |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
let tableKeyColumns=DatabaseInformation.GetPKColumns tableName


DatabaseInformation.GetTableInfo
|>PSeq.filter (fun a->a.TABLE_NAME="T_ZW")
|>fun a->ObjectDumper.Write a

"T_GHS"
//|>DatabaseInformation.GetPKColumns
//|>DatabaseInformation.GetColumnSchemal2Way
|>DatabaseInformation.GetTableInfo
|>fun a->
    ObjectDumper.Write a

let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship "T_DJSP_GHS"
