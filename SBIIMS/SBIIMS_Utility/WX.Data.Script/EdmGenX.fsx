

open System.Diagnostics

#r "System.Data.Entity"
#r "System.Data.Entity.Design"
open System.Data


#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data
open WX.Data.CodeAutomation

ConfigHelper.INS.LoadDefaultServiceConfigToManager

//----------------------------------------------------
let watch=new Stopwatch()
watch.Restart()
(
"SBIIMS_APC_DAAPGL", 
@"Data Source=192.168.2.199;Initial Catalog=SBIIMS_APC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True",
 "WX.Data.DataModel",
 [||],
 @"D:\TempWorkspace\AutomationTesting"
 )
|>EdmGenX.Generator
watch.Stop()
watch

//------------------------------------------------------------

(
"SBIIMS_Frame", 
"WX.Data.DataModel",
 @"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.DataModel.Frame"
 )
|>EdmGenX.Generator
[|@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.DataModel.Frame\WX.Data.DataModel.Frame.csproj"|]
|>MSBuild.INS.Build

(
"SBIIMS_AC", 
"WX.Data.DataModel",
 @"D:\Workspace\SBIIMS\SBIIMS_AC\AC\WX.Data.DataModel.AC"
 )
|>EdmGenX.Generator
[|@"D:\Workspace\SBIIMS\SBIIMS_AC\AC\WX.Data.DataModel.AC\WX.Data.DataModel.AC.csproj"|]
|>MSBuild.INS.Build

(
"SBIIMS_APC", 
"WX.Data.DataModel",
 @"D:\Workspace\SBIIMS\SBIIMS_APC\APC\WX.Data.DataModel.APC"
 )
|>EdmGenX.Generator
[|@"D:\Workspace\SBIIMS\SBIIMS_APC\APC\WX.Data.DataModel.APC\WX.Data.DataModel.APC.csproj"|]
|>MSBuild.INS.Build

(
"SBIIMS_FK", 
"WX.Data.DataModel",
 @"D:\Workspace\SBIIMS\SBIIMS_FK\FK\WX.Data.DataModel.FK"
 )
|>EdmGenX.Generator
[|@"D:\Workspace\SBIIMS\SBIIMS_FK\FK\WX.Data.DataModel.FK\WX.Data.DataModel.FK.csproj"|]
|>MSBuild.INS.Build

(
"SBIIMS_VC", 
"WX.Data.DataModel",
 @"D:\Workspace\SBIIMS\SBIIMS_VC\VC\WX.Data.DataModel.VC"
 )
|>EdmGenX.Generator
[|@"D:\Workspace\SBIIMS\SBIIMS_VC\VC\WX.Data.DataModel.VC\WX.Data.DataModel.VC.csproj"|]
|>MSBuild.INS.Build

//------------------------------------------------------------

watch.Restart()
(
"SBIIMS_JXC_JBXX_CZYWH", 
@"Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True",
 "WX.Data.DataModel",
 [|
 "dbo","T_RZ",1uy
 |],
 @"D:\TempWorkspace\AutomationTesting"
 )
|>EdmGenX.Generator
watch.Stop()
watch


(
"SBIIMS_JXC_JBXX_YGWH", 
@"Data Source=192.168.1.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True",
"WX.Data.DataModel",
[|
"dbo","T_LSH_YG",1uy
"dbo","T_RZ",1uy
"dbo","T_WF",1uy
"dbo","T_LSH_WF",1uy
"dbo","T_WFYWLX",1uy
"dbo","T_XL",1uy
"dbo","T_XX",1uy
"dbo","T_YG",1uy
"dbo","T_ZW",1uy
|],
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH\WX.Data.DataModel.JXC.JBXX.YGWH"
 )
|>EdmGenX.Generator
|>fun a->a|>Seq.iter(fun b->ObjectDumper.Write b)
[|@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH\WX.Data.DataModel.JXC.JBXX.YGWH\WX.Data.DataModel.JXC.JBXX.YGWH.csproj"|]
|>MSBuild.INS.Build

