//#I  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Threading.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Windows.Forms.dll"
#r "FSharp.PowerPack.Linq.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
//#I @"C:\Program Files (x86)\Microsoft Enterprise Library 4.1 - October 2008\Bin"
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "Microsoft.Practices.ObjectBuilder2.dll"

open System
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open Microsoft.FSharp.Linq
open Microsoft.FSharp.Collections
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.Practices.ObjectBuilder2
open System.Windows.Forms
//let projectPath= @"D:\Workspace\SBIIMS"

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation

type System.Text.StringBuilder  with
  member x.AppendN (str:string)=
    x.Append(str) |>ignore
  member x.AppendFormatN (str:string)=
    x.AppendFormat(str) |>ignore

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings
//先将WX.Data.FHelper.Service.Config,中的默认数据库配置改为<dataConfiguration defaultDatabase="SBIIMS" />
//////////////////////////////////////////////////////////////////////////////////////////////////

//////////////////////////////////////////////////////////////////////////////////////////////////

//生成或更新所有的代码文件
//DatabaseInformation.GetViewInfo
DatabaseInformation.GetTableInfo
//|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
//|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
|>Seq.filter (fun a->a.TABLE_NAME|>Comm.isEqualsIn ["sysdiagrams";"T_A";"T_AB"] |>not)
|>Seq.map (fun a->a.TABLE_NAME)
//For SBIIMS
|>AdvanceTypeGenerator.GenerateCodeFile ".JXC" @"D:\Workspace\SBIIMS" //不会修改存在的文件，只创建没有的文件
//For SBIIMSAC
//|>AdvanceTypeGenerator.GenerateCodeFile ".AccessControl" @"D:\Workspace\SBIIMS15"
//|>AdvanceTypeGenerator.GenerateCodeFile ".AccessControl" @"D:\Workspace\SBIIMS" //不会修改存在的文件，只创建没有的文件



//生成project config file
let sb=new StringBuilder()
DatabaseInformation.GetTableInfo
//|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
//|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
|>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
|>Seq.sortBy (fun a->a.TABLE_NAME)
|>Seq.map (fun a->a.TABLE_NAME)
|>Generator.AttachRelatedInfo
|>Seq.sortBy (fun a->a.TableName)
|>Seq.iter (fun a->
    match a.CodeTemplateType with
    | DJLSHTable
    | LSHTable ->()
    | _ ->
        //String.Format( @"    <Compile Include=""BDAdvance\BD_T_{0}_Advance.fs""/>",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)
        //String.Format( @"    <Compile Include=""BQAdvance\BQ_{0}_Advance.fs""/>",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)
        //String.Format( @"    <Compile Include=""BQClientAdvance\BQ_{0}_Client_Advance.fs""/>",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)
        //String.Format( @"    <Compile Include=""Advance\IDA_{0}_Advance.fs""/>",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)
        String.Format( @"    <Compile Include=""Advance\DA_{0}_Advance.fs""/>",match a.TableName with x when x.StartsWith("T_") ->x.Remove(0,2) | x -> x)
        |>sb.Append
        |>ignore
        sb.AppendLine()|>ignore)
sb.ToString()|>Clipboard.SetText

//sb.ToString() |>Clipboard.SetText
