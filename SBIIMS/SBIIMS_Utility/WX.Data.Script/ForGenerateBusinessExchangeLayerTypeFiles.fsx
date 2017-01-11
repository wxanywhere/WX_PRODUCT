//#I  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0"

#r "System.dll"
#r "System.Windows.Forms.dll"
open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open Microsoft.FSharp.Collections
open System.Windows.Forms
open System.Text.RegularExpressions

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
#r "System.Windows.Forms.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation
//=================================================================================

match "JG_GC" with
| IsMatchIn [@"^[a-zA-Z_]+$"] _->true
| _ ->false

"JG_GC_XX".Split([|'_'|]).Length

//#load @"D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\AdvanceValidationBQ_BQClient.fs"
//open WX.Data.CodeAutomation

//验证 服务端查询实体类型是否和客户端查询实体类型一一对应，且都具有相同的数据成员
@"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
|>ValidationBQ_BQClient.Validate
|>ObjectDumper.Write

//BQ
(
@"D:\Workspace\SBIIMS\SBIIMS_FK\FK_FKJL\WX.Data.BusinessEntitiesAdvance.FK.FKJL",
[
"BQ_FKJL_Advance","反馈交流-错误交流"
]
)
|>AdvanceBQFilesGenerator.GenerateCodeFiles

//=================================================================================
//BD
(
@"D:\Workspace\SBIIMS\SBIIMS_FK\FK_FKJL\WX.Data.BusinessEntitiesAdvance.FK.FKJL",
[  //BD类型名*BD基类名*子类型*类型备注
"BD_V_FKJL_CWJL_FJ_Advance","BD_ViewBase","","反馈交流-错误交流-附件"
"BD_V_FKJL_JYJL_FJ_Advance","BD_ViewBase","","反馈交流-建议交流-附件"
]
)
|>AdvanceBDFilesGenerator.GenerateCodeFiles 
|>ObjectDumper.Write

//=================================================================================

//Multi layer
(
".JXC.ZHGL.JYFYGL", //组件装配名后缀
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.ZHGL.JYFYGL",  //接口文件目录名称
@"D:\Workspace\SBIIMS",  //目标基本路径或入口路径
[
"IDA_ZHGL_JYFYGL_BusinessAdvance"
"IDA_ZHGL_JYFYGL_QueryAdvance"
]
,
ThreePhaseGroup //接口文件名成组片断段级别
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFiles
|>ObjectDumper.Write

//Multi layer  ------------
(
".JXC.ZHGL.JYFYGL", //组件装配名后缀
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.ZHGL.JYFYGL",  //接口文件目录名称
@"D:\Workspace\SBIIMS",  //目标基本路径或入口路径
[
"IDA_ZHGL_JYFYGL_BusinessAdvance"
"IDA_ZHGL_JYFYGL_QueryAdvance"
]
,
ThreePhaseGroup //接口文件名成组片断段级别
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFiles
|>ObjectDumper.Write

//Multi layer
(
".Link.ZH", //组件装配名后缀
"SBIIMS_Link", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_Link\Link_ZH\WX.Data.IDataAccessAdvance.Link.ZH",  //接口文件目录名称
@"D:\Workspace\SBIIMS\SBIIMS_Link\Link_ZH",  //目标基本路径或入口路径
[
"IDA_ZH_JSGN_Advance"
]
,
TwoPhaseGroup //接口文件名成组片断段级别
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFiles
|>ObjectDumper.Write



//#load @"D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\AdvanceBusinessExchangeLayerTypeGenerator.fs"
//open WX.Data.CodeAutomation
//==============================================================
//为所有接口组件生成代码文件
(
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_JXC",  //接口文件目录名称
ThreePhaseGroup  //注意！！!！！！， JXC_GGXZ须单独处理
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
]
|>MSBuild.INS.BuildForBusinessExchangeLayers


(
"SBIIMS_JXC", //系统简称
[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGHC\WX.Data.IDataAccessAdvance.JXC.GGHC"  //接口文件目录名称
],  //接口文件目录名称
ThreePhaseGroup  //注意！！!！！！， 
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponentsX
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGXZ\WX.Data.IDataAccessAdvance.JXC.GGXZ",  //接口文件目录名称
TwoPhaseGroup  //注意！！!！！！， JXC_GGXZ须单独处理
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers


(
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH\WX.Data.IDataAccessAdvance.JXC.JBXX.SPWH",  //接口文件目录名称
ThreePhaseGroup 
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers


(
"SBIIMS_AC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_AC\AC_JSWH\WX.Data.IDataAccessAdvance.AC.JSWH",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers


(
"SBIIMS_Frame", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_Frame",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_Frame", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_ZH\WX.Data.IDataAccessAdvance.Frame.ZH",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_Frame", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_SJJK\WX.Data.IDataAccessAdvance.Frame.SJJK",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

//--------------------------------------------------------------------------

(
"SBIIMS_Link", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_Link\Link_ZH",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_Frame", //系统简称
[ //接口文件目录名称
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_KJCD"  
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_SJPZ"  
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_ZH"  
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_SJJK" //SmallBusinessBase  
],
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponentsX
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_AC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_AC",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_APC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_APC",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_FK", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_FK",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

(
"SBIIMS_VC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_VC",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers

//--------------------------------------------------------------------------

(
"SBIIMS_VC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_VC\VC_GXFB\WX.Data.IDataAccessAdvance.VC.GXFB",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers


(
"SBIIMS_APC", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_APC\APC_DAAPGL\WX.Data.IDataAccessAdvance.APC.DAAPGL",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>Seq.map (fun a->ObjectDumper.Write a; (Directory.GetParent a).FullName)
|>Seq.toArray
|>MSBuild.INS.BuildForBusinessExchangeLayers


(
"SBIIMS_FK", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_FK\FK_FKJL\WX.Data.IDataAccessAdvance.FK.FKJL",  //接口文件目录名称
TwoPhaseGroup  //注意！！！
)
|>AdvanceBusinessExchangeLayerCoding.GenerateCodingFilesFromComponents
|>ObjectDumper.Write



//==============================================================
(
"SBIIMS_JXC", //系统简称
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.ZHGL.JYFYGL",  //接口文件目录名称
[
"IDA_ZHGL_JYFYGL_BusinessAdvance"
]
,
ThreePhaseGroup
)
|>AdvanceBusinessExchangeLayerCoding.GetCodingText
|>Clipboard.SetText


(
"SBIIMS_Link", //系统简称
@"D:\Workspace\SBIIMS\SBIIMS_Link\Link_ZH\WX.Data.IDataAccessAdvance.Link.ZH",  //接口文件目录名称
[
"IDA_ZH_JSGN_Advance"
]
,
TwoPhaseGroup
)
|>AdvanceBusinessExchangeLayerCoding.GetCodingText
|>Clipboard.SetText


Path.Combine("D:\Workspace\SBIIMS","WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF")

//==============================================================
//数据访问层的接口代码
(
"SBIIMS_AC", //系统别名
@"
  abstract GetCZYJSGL_JSGNTableView:BQ_CZYJSGL_Advance->BD_TV_CZYJSGL_JSGN_Advance[]
"
)
|>AdvanceBusinessExchangeLayerCoding.GetDataAccessCodeText
|>Clipboard.SetText


//数据访问层的接口代码
(
"SBIIMS_JXC", //系统别名
@"
  abstract AddGNWH_GNJDs:addEntity:BD_T_GNJD*updateEntity:BD_T_GNJD->BD_Result
  abstract DeleteGNWH_GNJDs:deleteEntities:BD_T_GNJD[]*updateEntity:BD_T_GNJD->BD_Result
  abstract MoveGNWH_GNJDs:businessEntities:BD_T_GNJD[]*roleRelatedEntities:BD_T_GNJD[]->BD_Result
"
)
|>AdvanceBusinessExchangeLayerCoding.GetDataAccessCodeText
|>Clipboard.SetText

//数据访问层的接口代码
(
"SBIIMS_JXC", //系统别名
@"
  abstract MoveGNWH_GNJDs:wx:BD_ExecuteContent<BD_T_GNJD[]>*roleRelatedEntities:BD_T_GNJD[]->BD_ExecuteResult
"
)
|>AdvanceBusinessExchangeLayerCoding.GetDataAccessCodeText

//数据访问层的接口代码
(
"SBIIMS_JXC", //系统别名
@"
  abstract MoveGNWH_GNJDs:wx:BD_ExecuteContent<BD_T_GNJD[]>*roleRelatedEntities:BD_T_GNJD[]->BD_ExecuteResult
  abstract GetJSGNGL_JSTableView:BQ_JSGNGL_Advance -> BD_QueryResult<BD_TV_JSGNGL_JS_Advance[]>
"
)
|>AdvanceBusinessExchangeLayerCoding.GetDataAccessCodeText




@"
namespace WX.Data.IDataAccess
open WX.Data.BusinessEntities
open WX.Data.BusinessBase
type IDA_ZHCX_JH_WLZ_Advance=
  //供货往来帐
  abstract GetJH_WLZ_GHDJView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHDJ_Advance[]
  abstract GetJH_WLZ_GHQK_HZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHQK_HZ_Advance[]
  abstract GetJH_WLZ_GHQK_LSZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHQK_LSZ_Advance[]
  abstract GetJH_WLZ_GHXS_HZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHXS_HZ_Advance[]
  abstract GetJH_WLZ_GHXS_LSZView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHXS_LSZ_Advance[]
  abstract GetJH_WLZ_GHSZWView:BQ_ZHCX_DJ_Advance->BD_V_JH_WLZ_GHSZW_Advance[]
  abstract GetJH_WLZ_WFFKQK_ADJView:BQ_ZHCX_JYTZ_Advance->BD_V_JH_WLZ_WFFKQK_ADJ_Advance[]
  abstract GetJH_WLZ_WFFKQK_LSZView:BQ_ZHCX_JYTZ_Advance->BD_V_JH_WLZ_WFFKQK_LSZ_Advance[]"
|>fun a->
    match Regex.Match (a, @"^\s+(abstract[\w\W\n]+)$",RegexOptions.Multiline) with
    | y ->
      y.Groups.[1].Value
|>ObjectDumper.Write     

