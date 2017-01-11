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
//#r "Microsoft.ServiceBus.dll"
//----------------------------------------------------------------------------------------------
//#I @"C:\Program Files (x86)\Microsoft Enterprise Library 4.1 - October 2008\Bin"
// #I @"C:\WINDOWS\assembly\GAC\"
//#I @"c:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
//----------------------------------------------------------------------------------------------
open Microsoft.FSharp.Collections
open System
open System.IO
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open System.Diagnostics
open System.Configuration
open Microsoft.FSharp.Linq
//open Microsoft.ServiceBus
open System.Windows.Forms
//----------------------------------------------------------------------------------------------
//It must load on sequence
//#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
#r "WX.Data.Database.dll"
//----------------------------------------------------------------------------------------------
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation
open WX.Data.Database

//----------------------------------------------------------------------------------------------
(*
let workPath= @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
AppDomain.CurrentDomain.SetData("APPBASE", workPath)
AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", Path.Combine(workPath,"Service.config"))
AppDomain.CurrentDomain.BaseDirectory
ConfigurationManager.ConnectionStrings.["SBIIMS_APC"].ConnectionString
*)

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//----------------------------------------------------------------------------------------------

let watch=new Stopwatch()
//System.Configuration.ConfigurationManager.ConnectionStrings
//先将WX.Data.FHelper.Service.Config,中的默认数据库配置改为<dataConfiguration defaultDatabase="SBIIMS" />
//=================================================================================

watch.Restart()
(
".JXC.JBXX.SPWH",
"SBIIMS_JXC",
@"D:\TempWorkspace\AutomationTesting",
false,
[
"T_SP";"T_SPDW";"T_SPLB";"T_SPXH"
]
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds

(
".JXC.JBXX.YGWH",
"SBIIMS_JXC",
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH",
false,
[
"T_LSH_YG";"T_RZ";"T_WF";"T_LSH_WF";"T_WFYWLX";"T_XL";"T_XX";"T_YG";"T_ZW"
]
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers


//--------------------------------------------------------------------------------------

(
".Frame",
"SBIIMS_Frame",
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame",
false
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers

(
".AC",
"SBIIMS_AC",
@"D:\Workspace\SBIIMS\SBIIMS_AC\AC",
false
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers


(
".APC",
"SBIIMS_APC",
@"D:\Workspace\SBIIMS\SBIIMS_APC\APC",
false
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers

(
".FK",
"SBIIMS_FK",
@"D:\Workspace\SBIIMS\SBIIMS_FK\FK",
false
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers

(
".VC",
"SBIIMS_VC",
@"D:\Workspace\SBIIMS\SBIIMS_VC\VC",
false
)
|>DAGeneratorX.GenerateCodeFile
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForDataAccessAutomationLayers


//-----------------------------------------------
watch.Restart()
(
"SBIIMS_APC", 
@"Data Source=192.168.2.199;Initial Catalog=SBIIMS_APC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True",
 "WX.Data.DataModel",
 @"D:\Workspace\SBIIMS\SBIIMS_APC\APC\WX.Data.DataModel.APC"
 )
|>EdmGenX.Generator
watch.Stop()
watch

//-----------------------------------------------

[@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame"]
|>MSBuild.INS.BuildForDataAccessAutomationLayers

[@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.BusinessEntities.Frame"]
|>MSBuild.INS.BuildForDataAccessAutomationLayers

[@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.DataAccess.Frame"]
|>MSBuild.INS.BuildForDataAccessAutomationLayers

[@"C:\Users\zhou\Documents\Visual Studio 2013\Projects\WX.Data.BusinessEntities.Frame"]
|>MSBuild.INS.BuildForDataAccessAutomationLayers

[@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.DataModel.Frame"]
|>MSBuild.INS.BuildForDataAccessAutomationLayers
