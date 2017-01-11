

//#I  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Data.Entity.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
#r "System.Windows.Forms.dll"

open Microsoft.FSharp.Collections
open System
open System.Windows.Forms

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation

(*
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("y."+a+"<-")) 

|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

|>ReportBuilderFieldsCoding.GetCode
|>Clipboard.SetText
*)
//============================================================================

[  //BD类型名*BD基类名*子类型*类型备注
"BD_TV_GGHC_KHDHC_ZW_Advance","BD_T_ZW","","公共缓存-客户端缓存-职务信息"
"BD_TV_GGHC_KHDHC_XL_Advance","BD_T_XL","","公共缓存-客户端缓存-学历信息"
"BD_TV_GGHC_KHDHC_XX_Advance","BD_T_XX","","公共缓存-客户端缓存-血型信息"
"BD_TV_GGHC_KHDHC_KHYWY_Advance","BD_T_KHYWY","","公共缓存-客户端缓存-客户业务员"
"BD_TV_GGHC_KHDHC_KHYHZH_Advance","BD_T_KHYHZH","","公共缓存-客户端缓存-客户银行账户信息"
"BD_TV_GGHC_KHDHC_GHSYWY_Advance","BD_T_GHSYWY","","公共缓存-客户端缓存-供货商业务员"
"BD_TV_GGHC_KHDHC_GHSYHZH_Advance","BD_T_GHSYHZH","","公共缓存-客户端缓存-供货商银行账户信息"
"BD_TV_GGHC_KHDHC_JYF_Advance","BD_T_JYJYF","","公共缓存-客户端缓存-经营交易方信息"
"BD_TV_GGHC_KHDHC_JYFYWY_Advance","BD_T_JYJYFYWY","","公共缓存-客户端缓存-经营交易方业务员"
"BD_TV_GGHC_KHDHC_JYFYHZH_Advance","BD_T_JYJYFYHZH","","公共缓存-客户端缓存-经营交易方银行账户信息"
"BD_TV_GGHC_KHDHC_XFP_Advance","BD_T_XFP","","公共缓存-客户端缓存-消费品信息"
"BD_TV_GGHC_KHDHC_FJP_Advance","BD_T_FJP","","公共缓存-客户端缓存-废旧品信息"
"BD_TV_GGHC_KHDHC_GDZC_Advance","BD_T_GDZC","","公共缓存-客户端缓存-固定资产信息"
"BD_TV_GGHC_KHDHC_GDZCLB_Advance","BD_T_GDZCLB","","公共缓存-客户端缓存-固定资产类别信息"
"BD_TV_GGHC_KHDHC_YHZH_Advance","BD_T_YHZH","","公共缓存-客户端缓存-我方银行账户信息"
]
|>Seq.iter (fun (a,_,_,_) -> 
  ObjectDumper.Write (
    String.Format (@"
let CCK_{0}=""CCK_{0}"" ",a.Replace("BD_TV_GGHC_KHDHC_","").Replace("_Advance",""))))


[
"UC_JBXX_YGWH_Modify"
"UC_JBXX_YGWH_YWLX"
"UC_JBXX_YGWH_YWLX_Add"
"UC_JBXX_YGWH_YWLX_Modify"
"UC_JBXX_YGWH_ZW"
"UC_JBXX_YGWH_ZW_Add"
"UC_JBXX_YGWH_ZW_Modify"
] 
|>fun b->
  ObjectDumper.Write "<!--PopupSub-->"
  b
  |>Seq.iter (fun a->
      String.Format (@"
      <DataTemplate DataType=""{{x:Type vm:FVM_{0}}}"">
          <v:UC_{0}   />
      </DataTemplate>", a.Remove(0,3))
      |>ObjectDumper.Write 
      )
  ObjectDumper.Write "//---------------------------"
  ObjectDumper.Write "<!--PopupSub-->"
  b
  |>Seq.iter (fun a->
      String.Format (@"
      <DataTemplate DataType=""{{x:Type vm:FVM_{0}_Advance}}"">
          <v:UC_{0}   />
      </DataTemplate>", a.Remove(0,3))
      |>ObjectDumper.Write 
      )













[
"DH_JXC_FXYC","DH.JXC.FXYC"
"DH_JXC_JBXX","DH.JXC.JBXX"
"DH_JXC_JHGL","DH.JXC.JHGL"
"DH_JXC_KCGL","DH.JXC.KCGL"
"DH_JXC_TJBB","DH.JXC.TJBB"
"DH_JXC_XSGL","DH.JXC.XSGL"
"DH_JXC_XTGL","DH.JXC.XTGL"
"DH_JXC_ZHBB","DH.JXC.ZHBB"
"DH_JXC_ZHGL","DH.JXC.ZHGL"
"DH_AC","DH.AC"
"DH_AGN","DH.AGN"
"DH_AMK","DH.AMK"
"DH_JXC","DH.JXC"
"DH","DH"
]
|>Seq.iter (fun (a,b)->ObjectDumper.Write( String.Format( @"    <DataTemplate DataType=""{{x:Type vm:FVM_{0}_Advance}}"">
        <v:UC_{0} />
    </DataTemplate>", a)))
//|>Clipboard.SetText

[
"VC_BTFYX","string",false,"补贴费用项"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


let f (x:'a seq when 'a:>string)=()

[
"wx"
]
|>Seq.nth 0

[
"VC_CJRQ","DateTime",false,"创建日期，及批次创建日期"
"VC_CKID","Guid",false,"仓库ID"
"VC_CK","string",false,"仓库名称"
"VC_SPID","Guid",false,"库存商品ID"
"VC_SPPC","decimal",false,"商品批次"
"VC_CGDJID","Guid",false,"采购单据ID"
"VC_CGGHSID","Guid",false,"采购供货商ID"
"VC_SCRQ","DateTime",false,"生产日期"
"VC_YXRQ","DateTime",false,"有效日期"
"VC_SPPH","decimal",false,"商品批号"
"VC_SPPCSL","decimal",false,"商品批次数量，当前批次当前仓库的数量"
"VC_SPPCJJ","decimal",false,"商品批次进价，当前批次的进价"
"VC_SPPCCBJE","decimal",false,"商品批次成本金额，当前批次的成本金额"
"VC_CGDJH","string",false,"采购单据号"
"VC_CGGHS","string",false,"采购供货商"
"VC_CGGHSJM","string",false,"采购供货商简码"
"VC_CGGHSXBH","decimal",false,"采购供货商编号"
"VC_XSBJ","decimal",false,"销售报价"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPJM","string",false,"商品简码"
"VC_SPDW","string",false,"商品单位"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPYS","string",false,"商品颜色"
"VC_SCCS","string",false,"商品生产厂商"
"VC_SPLB","string",false,"商品类别"
"VC_SPLBID","Guid",false,"商品类别ID"
"VC_SPJYBZ","bool",false,"商品禁用标志"
"VC_SPBZ","string",false,"商品备注"
"VC_SPCKSL","decimal",false,"商品仓库的数量，当前仓库所有批次的数量，该字段不能直接使用，须在客户端更新使用"
"VC_SPCKCBJJ","decimal",false,"商品仓库成本均价，当前仓库所有批次的成本均价，用于售价参考，该字段不能直接使用，须在客户端更新使用"
"VC_SPPCKXSL","decimal",false,"商品批次可选数量, 客户端使用"
"VC_SPCKKXSL","decimal",false,"商品仓库可选数量，客户端使用"
"VC_XNSPBZ","bool",false,"虚拟商品标志，用于过滤"
"VC_PDRQ","DateTime",false,"盘点日期"
"VC_PDZTID","byte",false,"盘点状态ID"
"VC_PDZT","string",false,"盘点状态, 0. 未盘，1. 盘点中，2. 已盘"
"VC_SYBZ","bool",false,"使用标志，用于控制已经使用的记录，主要用于盘点处理"
"VC_SPPCs","decimal[]",false,"商品批次组，按商品成组时，每个商品对应的所有批次，用于盘点商品选择控制"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode (Some "BD_V_GGHC_FWDHC_KCSPPC_Advance")
|>Clipboard.SetText


[
"VC_GXJID","Guid",false,"更新集Id"
"VC_GXRQ","DateTime",false,"更新日期"
"VC_GXJBH","string",false,"更新集编号"
"VC_GXJB","string",false,"更新级别"
"VC_GXJS","string",false,"更新简述"
"VC_GXSM","string",false,"更新说明"
]
|>DataEntitiesSegmentCoding.GetCode 
    (Some "BD_TV_GGXZ_WF_Advance") 
    (Some "BD_T_WF")
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_GXRQ","DateTime",false,"更新日期"
"VC_JYBZ","bool",false,"禁用标志"
"VC_GNID","Guid",false,"功能ID"
"VC_GNXBH","decimal",false,"功能编号"
"VC_GNJM","string",false,"功能简码"
"VC_GNMC","string",false,"功能名称"
"VC_SJLX","string",false,"数据类型"
]
//|>Seq.map (fun (a,b,c,d)->a.ToUpper(),b,c,d)
|>DataEntitiesSegmentCoding.GetCode None None
|>Clipboard.SetText

Guid.NewGuid()

[
"SPECIFIC_CATALOG","string",false,"此参数所属的过程的编录名称"
"SPECIFIC_SCHEMA","string",false,"包含此参数所属的过程的架构"
"SPECIFIC_NAME","string",false,"此参数所属的过程的名称"
"ORDINAL_POSITION","Int16",false,"参数的序号位置,从 1 开始 对于过程的返回值,此位置为 0"
"PARAMETER_MODE","string",false,"如果是输入参数,返回 IN,如果是输出参数,返回 OUT,如果是输入/输出参数,返回 INOUT"
"IS_RESULT","string",false,"如果指示属于函数的过程的结果,返回 YES 否则,返回 NO"
"AS_LOCATOR","string",false,"如果声明为定位符,返回 YES 否则,返回 NO"
"PARAMETER_NAME","string",false,"参数的名称 如果对应于函数的返回值,返回 NULL"
"DATA_TYPE","string",false,"系统提供的数据类型"
"CHARACTER_MAXIMUM_LENGTH","Int32",false,"二进制或字符数据类型的最大长度（字符数）否则,返回 NULL"
"CHARACTER_OCTET_LENGTH","Int32",false,"二进制或字符数据类型的最大长度（字节数）否则,返回 NULL"
"COLLATION_CATALOG","string",false,"参数排序规则的编录名称 如果不属于某种字符类型,返回 NULL"
"COLLATION_SCHEMA","string",false,"始终返回 NULL"
"COLLATION_NAME","string",false,"参数排序规则的名称 如果不属于某种字符类型,返回 NULL"
"CHARACTER_SET_CATALOG","string",false,"参数字符集的编录名称 如果不属于某种字符类型,返回 NULL"
"CHARACTER_SET_SCHEMA","string",false,"始终返回 NULL"
"CHARACTER_SET_NAME","string",false,"参数字符集的名称 如果不属于某种字符类型,返回 NULL"
"NUMERIC_PRECISION","Byte",false,"近似数值数据、精确数值数据、整数数据或货币数据的精度 否则,返回 NULL"
"NUMERIC_PRECISION_RADIX","Int16",false,"近似数值数据、精确数值数据、整数数据或货币数据的精度基数 否则,返回 NULL"
"NUMERIC_SCALE","Int32",false,"近似数值数据、精确数值数据、整数数据或货币数据的小数位数 否则,返回 NULL"
"DATETIME_PRECISION","Int16",false,"如果参数类型为 datetime 或 smalldatetime,则为小数秒数的精度 否则,返回 NULL"
"INTERVAL_TYPE","string",false,"NULL 保留供 SQL Server 以后使用"
"INTERVAL_PRECISION","Int16",false,"NULL保留供 SQL Server 以后使用"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write (String.Format ( """xg.{0}<-m.Attributes.["{0}"].Value""",a)))
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write (String.Format ( """xf.SetAttribute ("{0}",k.{0})""",a)))

[
"VC_XSHZSL","decimal",false,"销售汇总数量"
"VC_XSHZJE","decimal",false,"销售汇总金额"
"VC_XSHZLR","decimal",false,"销售汇总利润"
]
|>DataEntitiesSegmentCoding.GetCode None None
|>Clipboard.SetText

[
"VC_CZY","string",false,"操作员姓名"
"VC_CZYID","Guid",false,"操作员ID"
"VC_XBH","decimal",false,"操作员序编号，及用户代码"
"VC_MRJSID","Guid",false,"默认角色ID,配合BD_JS使用"
"VC_JSID","Guid",false,"当前角色ID"
"VC_FJSID","Guid",false,"父角色ID,用于操作角色权限"
"VC_JS","string",false,"当前角色名称"
"VC_FBID","Guid",false,"分部ID"
"VC_BMID","Guid",false,"部门ID"
"VC_BM","string",false,"部门名称"
"VC_DQID","Guid",false,"地区ID"
"VC_DQ","string",false,"地区名称"
"VC_DY","string",false,"电邮"
"VC_LXDH","string",false,"联系电话"
//--------------
"BD_V_ZH_CZY_JS_AdvanceView","BD_V_ZH_CZY_JS_Advance[]",false,"操作员脚色组"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("w."+a+"<-y."+a.Remove(0,1))) 
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("w.WriteAttributeString ("+a+")"+a.Remove(0,1))) 


[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_GXRQ","DateTime",false,"更新日期"
"VC_JYBZ","bool",false,"禁用标志"
"VC_GNID","Guid",false,"功能ID"
"VC_GNXBH","decimal",false,"功能编号"
"VC_GNJM","string",false,"功能简码"
"VC_GNMC","string",false,"功能名称"
"VC_SJLX","string",false,"数据类型"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText



[
"VC_GXJID","Guid",false,"更新集ID"
"VC_DXID","Guid",false,"对象ID"
"VC_DXIDs","Guid[]",false,"对象ID组"
"VC_CJRQ","DateTime",false,"创建日期"
"VC_GXRQ","DateTime",false,"更新日期"
"VC_DBID","byte",false,"数据库ID"
"VC_AUID","byte",false,"自动单元ID"
"VC_GXJBH","string",false,"更新集编号"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[

"VC_CKID","Guid",false,"仓库ID"
"VC_FJPID","Guid",false,"废旧品D"
"VC_FJPJM","string",false,"废旧品简码"
"VC_FJPXBH","decimal",false,"废旧品编号"
"VC_FJPGGXH","string",false,"废旧品规格型号"
"VC_FJPLBIDs","Guid[]",false,"废旧品类别ID组"
"VC_XFPID","Guid",false,"消费品D"
"VC_XFPJM","string",false,"消费品简码"
"VC_XFPXBH","decimal",false,"消费品编号"
"VC_XFPGGXH","string",false,"消费品规格型号"
"VC_XFPLBIDs","Guid[]",false,"消费品类别ID组"
"VC_ZCID","Guid",false,"资产D"
"VC_ZCJM","string",false,"资产简码"
"VC_ZCXBH","decimal",false,"资产编号"
"VC_ZCLBID","Guid",false,"资产类别ID"
"VC_JYFID","Guid",false,"交易方ID"
"VC_JYFJM","string",false,"交易方简码"
"VC_JYFXBH","decimal",false,"交易方编号"

] 
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


""
|>fun a->
    a.Remove(0,a.IndexOf("\n")+1)



[

]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_JYF","string",false,"供货商"
"VC_CK","string",false,"仓库名称"
"VC_JBR","string",false,"经办人"
"VC_SP","string",false,"商品名称"
"VC_SPPC","decimal",false,"商品批次"
"VC_SL","decimal",false,"退货数量"
"VC_DJ","decimal",false,"退货单价"
"VC_ZJE","decimal",false,"退货总金额"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

|>ReportBuilderFieldsCoding.GetCode
|>Clipboard.SetText

|>Seq.fold (fun acc (a,b,_,_) ->
    acc+
    String.Format (@"
        <Field Name=""{0}"">
          <DataField>{0}</DataField>
          <rd:TypeName>{1}</rd:TypeName>
        </Field>",
        a,TypeShortLongDic.[b])
    ) ""
|>Clipboard.SetText

[
"wx"
"wx1"
]
|>Seq.fold (fun acc a ->acc+a) ""

    |>Clipboard.SetText
    //|>ObjectDumper.Write 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_CK","string",false,"仓库名称"
"VC_WFDW","string",false,"我方单位"
"VC_WFLXDH","string",false,"我方联系电话"
"VC_WFLXDZ","string",false,"我方联系地址"
"VC_JYF","string",false,"供货商"
"VC_JYFLXR","string",false,"供货商联系人"
"VC_JYFLXDH","string",false,"供货商联系电话"
"VC_JYFLXDZ","string",false,"供货商联系地址"
"VC_YFJE","decimal",false,"我方应付金额"
"VC_SFJE","decimal",false,"我方实付金额"
"VC_WFJE","decimal",false,"我方未付金额"
"VC_DJYHJE","decimal",false,"单据优惠金额，我方优惠金额和供货商优惠金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_DJZT","string",false,"单据状态"
"VC_BZ","string",false,"单据备注"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


System.DateTime.Now.ToString("yyyyMM")
[
"VC_WFZHJMXL","string",false,"我方组合简码系列，公司名(WX)-地域(YN，云南)-部门(XS，销售)"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_ZCLB","string",false,"资产类别"
"VC_ZCBH","string",false,"资产编号"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_WFZHJMXL","string",false,"我方组合简码系列，公司名(WX)-地域(YN，云南)-部门(XS，销售)，DA必须"
"VC_JYF","string",false,"交易方名称"
"VC_CK","string",false,"仓库名称"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_YG","string",false,"员工姓名"
"VC_JKLB","string",false,"借款类别，C_FYLB"
"VC_YGYHZHYH","string",false,"员工银行账户银行名称"
"VC_YGYHZHHM","string",false,"员工银行账户户名"
"VC_YGYHZHZH","string",false,"员工银行账户帐号"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_XFPID","Guid",false,"消费品ID"
"VC_XFP","string",false,"消费品名称"
"VC_XFPXBH","decimal",false,"消费品序编号"
"VC_PFSL","decimal",false,"配发数量"
"VC_PFZJE","decimal",false,"配发总金额"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_XFPID","string",false,"消费品ID"
"VC_XFP","string",false,"消费品名称"
"VC_XFPXBH","decimal",false,"消费品序编号"
"VC_SHSL","decimal",false,"损耗数量"
"VC_SHZJE","decimal",false,"损耗总金额"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_YGID","Guid",false,"员工ID"
"VC_YG","string",false,"员工姓名"
"VC_YGXBH","decimal",false,"员序编号"
"VC_PFSL","decimal",false,"配发数量"
"VC_PFZJE","decimal",false,"配发总金额"
]

//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_DJLX","string",false,"单据类型"
"VC_JYF","string",false,"交易方名称"
"VC_CK","string",false,"仓库名称"
"VC_ZCLB","string",false,"资产类别名称"
"VC_ZCBH","string",false,"资产编号"
"VC_SCCS","string",false,"生产厂商"
"VC_SL","decimal",false,"数量"
"VC_DJ","decimal",false,"单价"
"VC_ZJE","decimal",false,"总金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_BZ","string",false,"备注"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText



[
"VC_FYLBID","byte",false,"费用类别ID"
"VC_FYLB","string",false,"费用类别"
"VC_HZJE","decimal",false,"汇总金额"
"VC_SZLX","string",false,"收支类型"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CJRQ","DateTime",false,"开单日期"
"VC_DJH","string",false,"单据号"
"VC_JYF","string",false,"交易方名称"
"VC_FKXJS","string",false,"付款项简述"
"VC_FKJE","decimal",false,"付款金额"
"VC_JQBZ","bool",false,"结清标志，未结清的付款项，可在VC_BZ里显示C_QFJE(欠费金额)"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_BZ","string",false,"付款金额"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJID","Guid",false,"单据ID，用于单据显示"
"VC_DJH","string",false,"单据号"
"VC_CK","string",false,"仓库名称"
"VC_FJP","string",false,"废旧品名称"
"VC_FJPXBH","decimal",false,"废旧品序编号"
"VC_SL","decimal",false,"数量"
"VC_DJ","decimal",false,"单价"
"VC_ZJE","decimal",false,"总金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_BZ","string",false,"备注"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_JYFID","Guid",false,"交易方ID"
"VC_JYFJM","string",false,"交易方简码"
"VC_JYFXBH","decimal",false,"交易方编号"
"VC_JBRID","Guid",false,"经办人ID"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_FYLBID","byte",false,"费用类别ID"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

999999999
let x01:int list=[]
x01
|>Seq.max

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_YGID","Guid",false,"员工ID"
"VC_YGJM","string",false,"员工简码"
"VC_YGXBH","decimal",false,"员工编号"
"VC_JBRID","Guid",false,"经办人ID"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_ZFZT","byte",false,"支付状态，或结清状态，1-未支付，2-部分支付，3-全部支付"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

let x:int16=2

[
"VC_YWY","decimal",false,"业报月"
"VC_KCSL","decimal",false,"库存金额"
"VC_KCCBJE","decimal",false,"库存成本金额"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_KCSHSL","decimal",false,"库存损耗数量，同时做为精确计算业务月库存数量的依据"
"VC_KCSHCBJE","decimal",false,"库存损耗成本金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSCBJE","decimal",false,"销售成本合计金额"
"VC_XSLRJE","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_JYFXZID","byte",false,"交易方性质ID，包括1-供货商，2-客户，3-经营交易方，4-员工，用于区分对应的台账表,用于关联"
"VC_YSYFLXID","byte",false,"应收应付类型ID,用于过滤"
"VC_FYLBID","byte",false,"费用类别ID，对应台账表中的T_FYLB,用于关联"
"VC_FYLB","string",false,"费用类别"
"VC_CJRQ","DateTime",false,"创建日期"
"VC_JE","decimal",false,"金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_BZ","string",false,"操作员"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_YWY","DateTime",false,"业务月"
"VC_XSLRJE","decimal",false,"销售利润"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText




[
"VC_XFP","string",false,"消费品名称"
"VC_XFPXBH","decimal",false,"消费品编号"
"VC_XFPGGXH","string",false,"消费品规格型号"
"VC_KCSL","decimal",false,"库存数量"     
"VC_CBJJ","decimal",false,"成本均价" 
"VC_CBJE","decimal",false,"成本金额"
"VC_GZZSL","decimal",false,"购置总数量，从总账中获取"
"VC_GZZJE","decimal",false,"购置总金额，从总账中获取"
"VC_PFZSL","decimal",false,"配发总数量，从总账中获取"
"VC_PFZJE","decimal",false,"配发总数量，从总账中获取"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-a."+a)) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CK","string",false,"库存名称"
"VC_KCSL","decimal",false,"库存数量"     
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-a."+a)) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_ZCID","Guid",false,"固定资产ID，用于关联"
"VC_CJRQ","DateTime",false,"开单日期"
"VC_DJH","string",false,"单据号"
"VC_DJLX","string",false,"单据类型"
"VC_ZSBL","decimal",false,"折损比例"
"VC_ZSQJE","decimal",false,"折损前金额"
"VC_ZSJE","decimal",false,"折损金额"
"VC_ZSHJE","decimal",false,"折损后金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_SHR","string",false,"审核人"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-a."+a)) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLX","string",false,"单据类型"
"VC_DJID","Guid",false,"单据ID,用于关联"
"VC_ZCSL","decimal",false,"资产数量"
"VC_ZSQZJE","decimal",false,"资产折损前总金额"
"VC_ZSZJE","decimal",false,"资产折损总金额"
"VC_ZSHZJE","decimal",false,"资产折损总金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_SHR","string",false,"审核人"
"VC_DJZT","string",false,"单据状态"
"VC_BZ","string",false,"单据备注"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("y."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_ZC","string",false,"资产名称"
"VC_ZCXBH","decimal",false,"固定资产编号"
"VC_ZCLB","string",false,"固定资产类别"
"VC_ZSBL","decimal",false,"资产折损比例"
"VC_ZSQJE","decimal",false,"资产折损前金额"
"VC_ZSJE","decimal",false,"资产折损金额"
"VC_ZSHJE","decimal",false,"资产折损后总金额"
"VC_GHZBDJS","decimal",false,"更换质保倒计时，以天记"
"VC_WXZBDJS","decimal",false,"维修质保倒计时，以天记"
"VC_SCCS","string",true,"生产产商"
"VC_SCCSLXR","string",true,"生产产商联系人"
"VC_SCCSLXDH","string",true,"生产产商联系电话"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("y."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"开单日期"
"VC_DJH","string",false,"单据号"
"VC_JYF","string",false,"交易方"
"VC_CK","string",false,"仓库名称"
"VC_ZCLB","string",false,"资产类别名称"
"VC_SL","decimal",false,"数量"
"VC_DJ","decimal",false,"单价"
"VC_ZJE","decimal",false,"总金额"
"VC_GHZBQ","decimal",false,"更换质保期"
"VC_WXZBQ","decimal",false,"维修质保期"
"VC_SCCS","string",true,"生产产商"
"VC_SCCSLXR","string",true,"生产产商联系人"
"VC_SCCSLXDH","string",true,"生产产商联系电话"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_CKID","Guid",false,"仓库ID"
"VC_SPID","Guid",false,"商品ID"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_GHSID","Guid",false,"供货商ID"
"VC_GHSJM","string",false,"供货商简码"
"VC_GHSXBH","decimal",false,"供货商编号"
"VC_KHID","Guid",false,"客户ID"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,"客户编号"
"VC_SL","decimal",false,"数量，商品数量或库存数量"
"VC_SCRQ","DateTime",false,"商品生产日期"
"VC_SPLBIDs","Guid[]",false,"商品类别ID组"
"VC_PDZTID","byte",false,"盘点状态ID"
"VC_PDZTIDs","byte[]",false,"盘点状态ID组，用于查询多个状态的记录"
"VC_FJPCBZ","bool",false,"附加批次标志，以商品成组时，附加批次信息组，用于盘点商品选择等"
"VC_SPPCs","decimal[]",false,"商品批次组"
"VC_SPIDs","Guid[]",false,"商品ID组"
"VC_ZHLBID","byte",false,"账户类别ID"
"VC_YHWDID","Guid",false,"银行网点ID"
"VC_JYFID","Guid",false,"交易方ID"
"VC_YGID","Guid",false,"员工ID"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期，用于排序"
"VC_CKID","Guid",false,"仓库ID"
"VC_CK","string",false,"仓库名称"
"VC_SPID","Guid",false,"库存商品ID"
"VC_SPSL","decimal",false,"商品数量，当前仓库的商品数量"
"VC_SPCBJJ","decimal",false,"商品成本均价"
"VC_SPCBJE","decimal",false,"商品成本金额"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPJM","string",false,"商品简码"
"VC_SPDW","string",false,"商品单位"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPYS","string",false,"商品颜色"
"VC_SCCS","string",false,"生产厂商"
"VC_SPLB","string",false,"商品类别"
"VC_SPLBID","Guid",false,"商品类别ID"
"VC_SPJYBZ","bool",false,"商品禁用标志"
"VC_SPBZ","string",false,"商品备注"
"VC_XNSPBZ","bool",false,"虚拟商品标志，用于过滤"
"VC_PDRQ","DateTime",false,"盘点日期"
"VC_PDZTID","byte",false,"盘点状态ID"
"VC_PDZT","string",false,"盘点状态, 0. 未盘，1. 盘点中，2. 已盘"
"VC_SYBZ","bool",false,"使用标志，用于控制已经使用的记录，主要用于盘点处理"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CK","string",false,"仓库名称"
"VC_CKID","Guid",false,"仓库ID"
"VC_CKJM","string",false,"仓库简码"
"VC_DQ","string",false,"地区名称"
"VC_DQID","Guid",false,"地区ID"
"VC_DQIDs","Guid[]",false,"地区ID集"
"VC_DQJM","string",false,"地区简码"
"VC_GDZC","string",false,"固定资产名称"
"VC_GDZCID","Guid",false,"固定资产ID"
"VC_GDZCJM","string",false,"固定资产简码"
"VC_GDZCXBH","decimal",false,"固定资产编号"
"VC_JYF","string",false,"供货商名称"
"VC_JYFID","Guid",false,"供货商ID"
"VC_JYFJM","string",false,"供货商简码"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_KH","string",false,"客户名称"
"VC_KHID","Guid",false,"客户ID"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,"客户编号"
"VC_SP","string",false,"商品名称"
"VC_SPID","Guid",false,"商品ID"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,"商品编号"
"VC_YG","string",false,"员工名称"
"VC_YGID","Guid",false,"员工ID"
"VC_YGJM","string",false,"员工简码"
"VC_YGXBH","decimal",false,"员工编号"
"VC_YHZH","string",false,"银行账户名称"
"VC_YHZHID","Guid",false,"银行账户ID"
"VC_YHZHJM","string",false,"银行账户简码"
"VC_YHZHXBH","decimal",false,"银行账户编号"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("y."+a+"<-")) 

[
"VC_JYF","string",false,"供货商"
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLX","string",false,"单据类型"
"VC_CK","string",false,"仓库名称"
"VC_WFYFJE","decimal",false,"我方应付金额"
"VC_WFSFJE","decimal",false,"我方实付金额"
"VC_WFWFJE","decimal",false,"我方未付金额"
"VC_DJYHJE","decimal",false,"单据优惠金额，我方优惠金额和供货商优惠金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_JQBZ","bool",false,"是否结清"
"VC_BZ","string",false,"备注"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,"开单日期"
"VC_DJLX","string",false,"交易凭据类型，即单据类型"
"VC_JYF","DateTime",false,"供货商"
"VC_WFYFJE","decimal",false,"我方应付金额，退货时供货商应付为负"
"VC_DJYHJE","decimal",false,"单据优惠金额，供货商优惠为正，我方优惠为负"
"VC_WFSFJE","decimal",false,"我方实付金额"
"VC_WFWFJE","decimal",false,"我方未付金额"
"VC_JBR","string",false,"单据经办人"
"VC_CZY","string",false,"操作员"
]


|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-")) 

[
"VC_YWY","DateTime",false,"业务月"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_JBR","string",false,"经办人即业务员。记录应原子化到经办人和客户的成组，此时，日报数据和月报数据单独以经办人和日报数据成组获取，如果原子化到单据的话，获取到客户端的数据较大，尽管所有的统计数据可以一次性获取"
"VC_JBRID","Guid",false,"用于过滤时报(日报+月报)"
"VC_JBRTCBL","decimal",false,"经办人提出比例"
"VC_KH","string",false,"客户名称"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,"客户编号"
"VC_KHXFSL","decimal",false,"客户消费数量"
"VC_KHTHSL","decimal",false,"客户退货数量"
"VC_KHXFHJSL","decimal",false,"客户消费合计数量"
"VC_KHXFJE","decimal",false,"客户消费金额"
"VC_KHTHJE","decimal",false,"客户退货金额"
"VC_KHXFHJJE","decimal",false,"客户消费合计金额"
"VC_KHDJYHJE","decimal",false,"单据优惠客户的金额，与利润有关"
"VC_KHSPYHJE","decimal",false,"商品优惠客户的金额，与利润无关"
"VC_KHSFJE","decimal",false,"客户实付金额"
"VC_KHQFJE","decimal",false,"客户欠费金额"
"VC_XSHZSL","decimal",false,"销售汇总数量"
"VC_THHZSL","decimal",false,"退货汇总数量"
"VC_XSHJHZSL","decimal",false,"销售合计汇总数量"
"VC_XSHZJE","decimal",false,"销售汇总金额"
"VC_THHZJE","decimal",false,"退货汇总金额"
"VC_XSHJHZJE","decimal",false,"销售合计汇总金额"
"VC_YSKHZJE","decimal",false,"已收款汇总金额"
"VC_WSKHZJE","decimal",false,"未收款汇总金额"
"VC_JBRTCJE","decimal",false,"经办人提成金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPLB","string",false,"商品类别"
"VC_CGSL","decimal",false,"采购数量"
"VC_CGJE","decimal",false,"采购金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_JYF","string",false,"供货商"
"VC_CGSL","decimal",false,"采购数量"
"VC_CGJE","decimal",false,"采购金额"
"VC_DQKCSL","decimal",false,"商品当前库存数量"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,"开单日期"
"VC_JYF","string",false,"供货商"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPLB","string",false,"商品类别"
"VC_SPDW","string",false,"商品单位"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPYS","string",false,"商品颜色"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_CK","string",false,"仓库"
"VC_JBR","string",false,"经办人"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false
"VC_KDRQ","DateTime",false
"VC_JYF","string",false
"VC_JYFJM","string",false
"VC_SP","string",false
"VC_PC","decimal",false
"VC_SCRQ","DateTime",false
"VC_SPJM","string",false
"VC_SPXBH","decimal",false
"VC_SPLB","string",false
"VC_SPLBID","Guid",false
"VC_DW","string",false
"VC_GGXH","string",false
"VC_YS","string",false
"VC_DJ","decimal",false
"VC_SL","decimal",false
"VC_ZJE","decimal",false
"VC_CK","string",false
"VC_CKXBH","decimal",false
"VC_JBR","string",false
"VC_CZY","string",false
"VC_SHR","string",false
"VC_HZSL","decimal",false
"VC_HZJE","decimal",false
"VC_FLHZSL","decimal",false
"VC_FLHZJE","decimal",false
]

[
"VC_SPID","Guid",false,"商品ID，用来关联汇总明细"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_XFTHSL","decimal",false,"消费退货数量"
"VC_XFTHJE","decimal",false,"消费退货金额"
"VC_DQKCSL","decimal",false,"商品当前库存数量"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_KH","string",false,"客户名称"
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,"开单日期"
"VC_SPID","Guid",false,"商品ID，用来关联"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_CK","string",false,"仓库"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_KH","string",false,"客户名称"
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,"开单日期"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_CK","string",false,"仓库"
"VC_SPSCCS","string",false,"商品生产厂商"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText



[
"VC_FKPJ","string",false,"付款凭据"
"VC_FKRQ","DateTime",false,"付款日期"
"VC_FKPJLX","string",false,"付款凭据类型，即单据类型"
"VC_FKJE","decimal",false,"付款金额"
"VC_QFJE","decimal",false,"欠费金额"
"VC_JYJBR","string",false,"付款经办人"
"VC_JYCZY","string",false,"付款操作员"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText




[
"VC_FKPJ","string",false,"付款凭据"
"VC_FKRQ","DateTime",false,"付款日期"
"VC_FKPJLX","string",false,"付款凭据类型，即单据类型"
"VC_FKJE","decimal",false,"付款金额"
"VC_QFJE","decimal",false,"欠费金额"
"VC_FKMBDJ","string",false,"付款目标单据"
"VC_FKMBDJLX","string",false,"付款目标单据类型"
"VC_KH","string",false,"客户名称"
"VC_JBR","string",false,"单据经办人"
"VC_CZY","string",false,"操作员"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


"VC_GLPJ","Guid",false,"关联凭据，即为单据付款的单据"
"VC_GLPJLSH","string",false,"关联凭据流水号"

"VC_JYRQ","DateTime",false,""
"VC_JYF","string",false,""
"VC_JYJE","decimal",false,""
"VC_JYJBR","string",false,""
"VC_JYCZY","string",false,""
"VC_BZ","string",false,""
"VC_YZQJE","decimal",false,""
"VC_DJYHJE","decimal",false,""
"VC_DJZQJE","decimal",false,""
"VC_SZQJE","decimal",false,""
"VC_QFJE","decimal",false,""
"VC_HJZQJE","decimal",false,""
"VC_DJJBR","string",false,""
"VC_DJCZY","string",false,""
"VC_JYBZ","string",false,""
]


[
"VC_JYPJ","Guid",false,"交易凭据"
"VC_JYPJLSH","string",false,"交易凭据流水号"
"VC_JYPJLX","string",false,"交易凭据类型，即单据类型"
"VC_GLPJ","Guid",false,"关联凭据，即为单据付款的单据"
"VC_GLPJLSH","string",false,"关联凭据流水号"
"VC_KDRQ","DateTime",false,"开单日期"
"VC_JYRQ","DateTime",false,""
"VC_JYF","string",false,""
"VC_JYJE","decimal",false,""
"VC_JYJBR","string",false,""
"VC_JYCZY","string",false,""
"VC_BZ","string",false,""
"VC_YZQJE","decimal",false,""
"VC_DJYHJE","decimal",false,""
"VC_DJZQJE","decimal",false,""
"VC_SZQJE","decimal",false,""
"VC_QFJE","decimal",false,""
"VC_HJZQJE","decimal",false,""
"VC_DJJBR","string",false,""
"VC_DJCZY","string",false,""
"VC_JYBZ","string",false,""
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_JYPJ","Guid",false,""
"VC_JYPJLSH","string",false,""
"VC_JYPJLX","string",false,""
"VC_GLPJ","Guid",false,""
"VC_GLPJLSH","string",false,""
"VC_KDRQ","DateTime",false,""
"VC_JYRQ","DateTime",false,""
"VC_JYF","string",false,""
"VC_JYJE","decimal",false,""
"VC_JYJBR","string",false,""
"VC_JYCZY","string",false,""
"VC_BZ","string",false,""
"VC_YZQJE","decimal",false,""
"VC_DJYHJE","decimal",false,""
"VC_DJZQJE","decimal",false,""
"VC_SZQJE","decimal",false,""
"VC_QFJE","decimal",false,""
"VC_HJZQJE","decimal",false,""
"VC_DJJBR","string",false,""
"VC_DJCZY","string",false,""
"VC_JYBZ","string",false,""
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_JYFID","Guid",false,"供货商ID"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_JYF","string",false,"供货商名称"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_CGJE","decimal",false,"采购金额"
"VC_CGTHJE","decimal",false,"采购退货金额"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_CGYHJE","decimal",false,"采购优惠金额"
"VC_WFYFJE","decimal",false,"我方应付金额，从台账获取，应=VC_CGHJJE"
"VC_WFSFJE","decimal",false,"我方实付金额，从台账获取"
"VC_WFWFJE","decimal",false,"我方未付金额，从台账获取"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLX","string",false,"单据类型"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_XSLRJE","decimal",false,"销售利润金额"
"VC_XSLRL","decimal",false,"销售利润率"
"VC_JYF","string",false,"供货商名称"
"VC_CK","string",false,"仓库名称"
"VC_JBR","string",false,"经办人"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPYS","decimal",false,"商品颜色"
"VC_SPGGXH","decimal",false,"商品规格型号"
"VC_SPDW","decimal",false,"商品单位"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_DQKCSL","decimal",false,"当库库存数量，该数量并不以时间来统计"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPYS","decimal",false,"商品颜色"
"VC_SPGGXH","decimal",false,"商品规格型号"
"VC_SPDW","decimal",false,"商品单位"
"VC_JYFL","decimal",false,"供货数量"
"VC_GHJE","decimal",false,"供货金额"
"VC_GHTHSL","decimal",false,"供货退货数量"
"VC_GHTHJE","decimal",false,"供货退货金额"
"VC_GHHJSL","decimal",false,"供货合计数量"
"VC_GHHJJE","decimal",false,"供货合计金额"
"VC_DQKCSL","decimal",false,"当库库存数量，该数量并不以时间来统计"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLX","string",false,"单据类型"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_JYF","string",false,"供货商名称"
"VC_CK","string",false,"仓库名称"
"VC_JBR","string",false,"经办人"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CGYWY","string",false,"采购业务员姓名"
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_DJLX","string",false,"单据类型"
"VC_JYF","string",false,"供货商名称"
"VC_CK","string",false,"仓库名称"
"VC_WFYFJE","decimal",false,"我方应付金额"
"VC_WFSSJE","decimal",false,"我方实付金额"
"VC_WFWFJE","decimal",false,"我方未付金额"
"VC_DJYHJE","decimal",false,"单据优惠金额，我方优惠金额和供货商优惠金额"
"VC_CZY","string",false,"操作员"
"VC_DJZT","string",false,"单据状态"
"VC_BZ","string",false,"单据备注"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码，用于单据商品过滤"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPZJE","decimal",false,"商品总金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText





[
"VC_JYF","string",false,"供货商"
"VC_CJRQ","DateTime",false,"创建日期"
"VC_DJH","string",false,"单据号"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_DJLX","string",false,"单据类型"
"VC_CK","string",false,"仓库名称"
"VC_WFYFJE","decimal",false,"我方应付金额"
"VC_WFSFJE","decimal",false,"我方实付金额"
"VC_WFWFJE","decimal",false,"我方未付金额"
"VC_DJYHJE","decimal",false,"单据优惠金额，我方优惠金额和供货商优惠金额"
"VC_JBR","string",false,"经办人"
"VC_CZY","string",false,"操作员"
"VC_DJZT","string",false,"单据状态"
"VC_BZ","string",false,"单据备注"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码，用于单据商品过滤"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPZJE","decimal",false,"商品总金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPID","decimal",false,"商品ID，用于join"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_DQKCSL","decimal",false,"当前库存数量"
"VC_CGSL","decimal",false,"采购数量"
"VC_CGTHSL","decimal",false,"采购退货数量"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_BSSL","decimal",false,"报损数量"
"VC_BYSL","decimal",false,"报溢数量"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write (a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_SPID","Guid",false,"商品ID，用于关联父查询"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_CKID","Guid",false,"仓库ID"
"VC_CKJM","string",false,"仓库简码"
"VC_CKXBH","decimal",false,"仓库编号"
"VC_SPLBID","Guid",false,"商品类别ID"
"VC_SPLBIDs","Guid[]",false,"商品类别ID数组"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPLB","string",false,"商品类别名称"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPJYBZ","bool",false,"商品禁用标志"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_KCSL","decimal",false,"当前库存数量"
"VC_YSJJ","decimal",false,"预设进价"
"VC_SCJJ","decimal",false,"上次进价"
"VC_YSSJ","decimal",false,"预设售价"
"VC_DSZJE","decimal",false,"待售总金额"
"VC_CBJJ","decimal",false,"成本均价"
"VC_KCCBJE","decimal",false,"库存成本金额"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write (a+"<-")) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPLB","string",false,"商品类别名称"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPJYBZ","bool",false,"商品禁用标志"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_CK","string",false,"仓库名称"
"VC_KCSL","decimal",false,"当前库存数量"
"VC_JJ","decimal",false,"当前进价"
"VC_CBJE","decimal",false,"成本金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_ZWRQ","decimal",false,"账务日期"
"VC_CK","string",false,""
"VC_CKID","Guid",false,"仓库ID"
"VC_KCSL","decimal",false,"库存数量"
"VC_KCCBJE","decimal",false,"库存成本金额"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSCBJE","decimal",false ,"销售成本金额"
"VC_XSLRJE","decimal",false,"销售利润"
"VC_BSSL","decimal",false,"报损数量"
"VC_BSJE","decimal",false,"报损金额"
"VC_BYSL","decimal",false,"报溢数量"
"VC_BYJE","decimal",false,"报溢金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,"日期"
"VC_CKID","Guid",false,"仓库ID"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_YBR","DateTime",false,"业务日"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSCBJE","decimal",false,"销售成本合计金额"
"VC_XSYHJE","decimal",false,"销售优惠金额"
"VC_XSLRJE","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_YBY","DateTime",false,"业报月"
"VC_KCSL","decimal",false,"库存金额"
"VC_KCJE","decimal",false,"库存金额"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSCBJE","decimal",false,"销售成本合计金额"
"VC_XSYHJE","decimal",false,"销售优惠金额"
"VC_XSLRJE","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_KHID","Guid",false,""
"VC_KHXBH","decimal",false,""
"VC_KH","string",false,""
"VC_XFJE","decimal",false,"消费金额"
"VC_XFTHJE","decimal",false,"消费退货金额"
"VC_XFHJJE","decimal",false,"消费合计金额"
"VC_XFYHHJJE","decimal",false,"消费优惠合计金额，从单据获取，应和VC_WFYHJE相等"
"VC_WFYSJE","decimal",false,"我方应收金额，应和VC_XFHJJE相等"
"VC_WFYHJE","decimal",false,"我方优惠金额"
"VC_WFSSJE","decimal",false,"我方实收金额"
"VC_WFWSJE","decimal",false,"我方未收金额"

]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CGJHDJSL","decimal",false,"采购进货单据数据"
"VC_CGTHDJSL","decimal",false,"采购退货单据数量"
"VC_CGHHDJSL","decimal",false,"采购换货单据数量"
"VC_CGSL","decimal",false,"采购数量"
"VC_CGTHSL","decimal",false,"采购退货数量"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGJE","decimal",false,"采购金额"
"VC_CGTHJE","decimal",false,"采购退货金额"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_XSLSDJSL","decimal",false,"销售零售单据数量"
"VC_XSPOSDJSL","decimal",false,"POS销售单据数量"
"VC_XSPFDJSL","decimal",false,"销售批发单据数量"
"VC_XSTHDJSL","decimal",false,"销售退货单据数量"
"VC_XSHHDJSL","decimal",false,"销售换货单据数量"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJJE","decimal",false,"销售合计金额"


"VC_KCDBDJSL","decimal",false,"库存调拨单据数量"
"VC_KCBSDJSL","decimal",false,"库存报损单据数量"
"VC_KCBYDJSL","decimal",false,"库存报溢单据数量"
"VC_KCHBDJSL","decimal",false,"库存合并单据数据量"
"VC_KCCFDJSL","decimal",false,"库存拆分单据数据量"
"VC_KCSPHBDJSL","decimal",false,"库存商品合并单据数据量"
"VC_KCSPCFDJSL","decimal",false,"库存商品拆分单据数据量"
"VC_KCPDDJSL","decimal",false,"库存盘点单据数量"
"VC_KCPCBGDJSL","decimal",false,"库存批次变更单据数量"

"VC_KCDBCKSL","decimal",false,"库存调拨出库数量"
"VC_KCDBRKSL","decimal",false,"库存调拨入库数量"


"VC_KCBSSL","decimal",false,"库存报损数量"
"VC_KCBSJE","decimal",false,"库存报损金额"


"VC_KCBYSL","decimal",false,"库存报溢数量"
"VC_KCBYJE","decimal",false,"库存报溢金额"



"VC_KCSL","decimal",false,"库存数量"
"VC_KCJE","decimal",false,"库存金额"

"VC_WFFKDJSL","decimal",false,"我方付款单据数量"
"VC_WFYFJE","decimal",false,"我方应付金额,从单据中获取"
"VCX_WFYFJE","decimal",false,"我方应付金额，临时，用于对账，从台账中获取，应等于VC_WFYFJE"
"VC_JYFYHJE","decimal",false,"供货商优惠我方的金额,从单据获取 "
"VCX_JYFYHJE","decimal",false,"供货商优惠我方的金额,临时,用于对账，从交易台账中获取应和VC_JYFYHJE相等"
"VC_WFSFJE","decimal",false,"我方实付金额"
"VC_WFWFJE","decimal",false,"我方未付金额，因为是按日期统计，不能算是欠费"

"VC_WFSKDJSL","decimal",false,"我方收款单据数据"
"VC_WFYSJE","decimal",false,"我方应收金额,从单据中获取"
"VCX_WFYSJE","decimal",false,"我方应收金额，临时，用于对账，从台账中获取，应等于VC_WFYSJE"
"VC_WFYHJE","decimal",false,"我方优惠金额,从单据中获取"
"VCX_WFYHJE","decimal",false,"我方优惠金额，临时，用于对账，从台账中获取，应等于VC_WFYSJE"
"VC_WFSSJE","decimal",false,"我方实收金额"
"VC_WFWSJE","decimal",false,"我方未收金额，客户未付金额，，因为是按日期统计，不能算是欠费"

"VC_XSCBJE","decimal",false,"销售成本"
"VC_XSLRJE","decimal",false,"销售利润金额=销售总额-我方对客户的单据部分的优惠金额"
"VC_XSLRL","decimal",false,"销售利润率"

]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPID","Guid",false,"商品ID，用户Join"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_XSHJSL","decimal",false,"销售合计金额"
"VC_KCSL","decimal",false,"当前库存数量"
"VC_ZZL","decimal",false,"周转率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CJRQ","DateTime",false,""
"VC_JYFID","Guid",false,"供货商ID"
"VC_JYFJM","string",false,""
"VC_JYFXBH","decimal",false,""
"VC_JYFLXID","byte",false,"供货商类型ID"
"VC_JYFLBID","byte",false,"供货商类别ID"
"VC_JYFDJID","byte",false,"供货商等级ID"
"VC_KHID","Guid",false,"客户ID"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,""
"VC_KHLXID","byte",false,"客户类型ID"
"VC_KHDJID","byte",false,"客户等级ID"
"VC_KHDQID","Guid",false,"客户地区ID"
"VC_SPID","Guid",false,"商品ID，用于关联父查询"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,""
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_CKID","Guid",false,"仓库ID"
"VC_CKJM","string",false,"仓库简码"
"VC_CKXBH","decimal",false,"仓库编号"
"VC_SPLBID","Guid",false,"商品类别ID"
"VC_SPLBIDs","Guid[]",false,"商品类别ID数组"
"VC_DJH","string",false,"单据号"
"VC_JBRID","Guid",false,"经办人ID"
"VC_JBRJM","string",false,""
"VC_DJLXID","byte",false,"单据类型ID"
"VC_TS","int",false,"天数"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_XSHJSL","decimal",false,"销售合计金额"
"VC_KCSL","decimal",false,"当前库存数量"
"VC_ZZL","decimal",false,"周转率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_KHDQ","string",false,"客户地区"
"VC_XFSL","decimal",false,"消费数量"
"VC_XFJE","decimal",false,"消费金额"
"VC_XFTHSL","decimal",false,"消费退货数量"
"VC_XFTHJE","decimal",false,"消费退货金额"
"VC_XFHJSL","decimal",false,"消费合计数量"
"VC_XFHJJE","decimal",false,"消费合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_KHLX","string",false,"客户类型"
"VC_XFSL","decimal",false,"消费数量"
"VC_XFJE","decimal",false,"消费金额"
"VC_XFTHSL","decimal",false,"消费退货数量"
"VC_XFTHJE","decimal",false,"消费退货金额"
"VC_XFHJSL","decimal",false,"消费合计数量"
"VC_XFHJJE","decimal",false,"消费合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_KH","string",false,"客户名称"
"VC_XFSL","decimal",false,"消费数量"
"VC_XFJE","decimal",false,"消费金额"
"VC_XFTHSL","decimal",false,"消费退货数量"
"VC_XFTHJE","decimal",false,"消费退货金额"
"VC_XFHJSL","decimal",false,"消费合计数量"
"VC_XFHJJE","decimal",false,"消费合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_SP","string",false,"商品名称"
"VC_SPID","Guid",false,"商品ID,用于客户端关联"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_KCSL","decimal",false,"库存数量"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-"+a))
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPLB","string",false,"商品类别名称"
"VC_SPLBID","Guid",false,"商品类别ID,用于类别关联"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-"+a))
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_JYF","string",false,"供货商名称"
"VC_JYFID","Guid",false,"供货商ID"
"VC_CGSL","decimal",false,"采购数量"
"VC_CGJE","decimal",false,"采购金额"
"VC_CGTHSL","decimal",false,"采购退货数量"
"VC_CGTHJE","decimal",false,"采购退货金额"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
"VC_XSWCL","decimal",false,"销售完成率"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write ("x."+a+"<-"+a))
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_JYF","string",false,"供货商名称"
"VC_JYFJM","string",false,"供货商简码"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,""
"VC_CK","string",false,""
"VC_CKJM","string",false,"仓库简码"
"VC_DJLX","string",false,"单据类型"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_KH","string",false,"客户名称"
"VC_JYF","string",false,"供货商名称"
"VC_JYFJM","string",false,"供货商简码"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_JBR","string",false,"经办人，即销售业务员"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_CZY","string",false,"操作员"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_SPLRJE","decimal",false,"商品利润金额"
"VC_SPJYLRL","decimal",false,"商品利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText



[
"VC_KH","string",false,"客户名称"
"VC_KHID","Guid",false,"客户ID"
"VC_XFSL","decimal",false,"消费数量"
"VC_XFJE","decimal",false,"消费金额"
"VC_XFTHSL","decimal",false,"消费退货数量"
"VC_XFTHJE","decimal",false,"消费退货金额"
"VC_XFHJSL","decimal",false,"消费合计数量"
"VC_XFHJJE","decimal",false,"消费合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CJRQ","DateTime",false,""
"VC_JYFID","Guid",false,"供货商ID"
"VC_JYFJM","string",false,""
"VC_JYFXBH","decimal",false,""
"VC_JYFLXID","byte",false,"供货商类型ID"
"VC_JYFLBID","byte",false,"供货商类别ID"
"VC_JYFDJID","byte",false,"供货商等级ID"
"VC_KHID","Guid",false,"客户ID"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,""
"VC_KHLXID","byte",false,"客户类型ID"
"VC_KHDJID","byte",false,"客户等级ID"
"VC_KHDQID","Guid",false,"客户地区ID"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,""
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_CKID","Guid",false,"仓库ID"
"VC_CKJM","string",false,"仓库简码"
"VC_CKXBH","decimal",false,"仓库编号"
"VC_SPLBID","Guid",false,"商品类别ID"
"VC_SPLBIDs","Guid[]",false,"商品类别ID数组"
"VC_DJH","string",false,"单据号"
"VC_JBRID","Guid",false,"经办人ID"
"VC_JBRJM","string",false,""
"VC_DJLXID","byte",false,"单据类型ID"
"VC_TS","int",false,"天数"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,""
"VC_KH","string",false,"客户名称"
"VC_KHJM","string",false,"客户名称简码"
"VC_KHXBH","decimal",false,"客户编号"
"VC_JYF","string",false,"供货商"
"VC_JYFJM","string",false,"供货商简码"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_CK","string",false,""
"VC_CKJM","string",false,"仓库简码"
"VC_DJLX","string",false,"单据类型"
"VC_DJLXID","byte",false,"单据类型"
"VC_SP","string",false,"商品名称"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_SPLRJE","decimal",false,"商品利润金额"
"VC_SPJYLRL","decimal",false,"商品交易利润率"
"VC_JBR","string",false,"经办人"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false,"商品名称"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPXSSL","decimal",false,"商品销售数量"
"VC_SPTHSL","decimal",false,"商品退货数量"
"VC_SPXSHJSL","decimal",false,"商品销售合计数量"
"VC_SPXSJE","decimal",false,"商品销售金额"
"VC_SPTHJE","decimal",false,"商品退货金额"
"VC_SPXSHJJE","decimal",false,"商品销售合计金额"
"VC_SPLRJE","decimal",false,"商品利润金额"
"VC_SPJYLRL","decimal",false,"商品交易利润率，即销售利润率"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPLB","string",false,"类别"
"VC_SPLBSJ","string",false,"带缩进的商品类别名称"
"VC_SPLBJB","byte",false,"商品类别层次级别，用于图表按相同类别层次分组显示"
"VC_XSSL","decimal",false,"销售数量"
"VC_THSL","decimal",false,"退货数量"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_THJE","decimal",false,"退货金额"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_LRJE","decimal",false,"利润金额"
"VC_JYLRL","decimal",false,"交易利润率，即销售利润率"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,""
"VC_CK","string",false,""
"VC_CKJM","string",false,"仓库简码"
"VC_DJLX","string",false,"单据类型"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_JYF","string",false,"供货商名称"
"VC_KH","string",false,"客户名称"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,"客户编号"
"VC_JBR","string",false,"经办人，即销售业务员"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_CZY","string",false,"操作员"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_SPLRJE","decimal",false,"商品利润金额"
"VC_SPJYLRL","decimal",false,"商品利润率"

"VC_SPHZSL","decimal",false,"商品汇总数量"
"VC_SPHZJE","decimal",false,"商品汇总金额"
"VC_SPHZLRJE","decimal",false,"商品汇总利润"
"VC_SPJYHZLRL","decimal",false,"商品交易汇总利润率"

"VC_CGSL","decimal",false,"采购数量"
"VC_CGJE","decimal",false,"采购金额"
"VC_CGTHSL","decimal",false,"采购退货数量"
"VC_CGTHJE","decimal",false,"采购退货金额"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
"VC_XSWCL","decimal",false,"销售完成率"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,""
"VC_CK","string",false,""
"VC_CKJM","string",false,"仓库简码"
"VC_DJLX","string",false,"单据类型"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_KH","string",false,"客户名称"
"VC_JYF","string",false,"供货商名称"
"VC_JYFJM","string",false,"供货商简码"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_JBR","string",false,"经办人，即销售业务员"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_CZY","string",false,"操作员"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_SPLRJE","decimal",false,"商品利润金额"
"VC_SPJYLRL","decimal",false,"商品利润率"

"VC_SPHZSL","decimal",false,"商品汇总数量"
"VC_SPHZJE","decimal",false,"商品汇总金额"
"VC_SPHZLRJE","decimal",false,"商品汇总利润"
"VC_SPJYHZLRL","decimal",false,"商品交易汇总利润率"

"VC_XSSL","decimal",false,"销售数量"
"VC_XSJE","decimal",false,"销售金额"
"VC_XSTHSL","decimal",false,"销售退货数量"
"VC_XSTHJE","decimal",false,"销售退货金额"
"VC_XSHJSL","decimal",false,"销售合计数量"
"VC_XSHJJE","decimal",false,"销售合计金额"
"VC_XSLR","decimal",false,"销售利润"
"VC_XSLRL","decimal",false,"销售利润率"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_JBR","string",false,"经办人即业务员。记录应原子化到经办人和客户的成组，此时，日报数据和月报数据单独以经办人和日报数据成组获取，如果原子化到单据的话，获取到客户端的数据较大，尽管所有的统计数据可以一次性获取"
"VC_JBRID","Guid",false,"用于过滤时报(日报+月报)"
"VC_JBRTCBL","decimal",false,"经办人提出比例"

"VC_KH","string",false,"客户名称"
"VC_KHJM","string",false,"客户简码"
"VC_KHXBH","decimal",false,"客户编号"
"VC_KHXFSL","decimal",false,"客户消费数量"
"VC_KHTHSL","decimal",false,"客户退货数量"
"VC_KHXFHJSL","decimal",false,"客户消费合计数量"
"VC_KHXFJE","decimal",false,"客户消费金额"
"VC_KHTHJE","decimal",false,"客户退货金额"
"VC_KHXFHJJE","decimal",false,"客户消费合计金额"
"VC_KHDJYHJE","decimal",false,"单据优惠客户的金额，与利润有关"
"VC_KHSPYHJE","decimal",false,"商品优惠客户的金额，与利润无关"
"VC_KHSFJE","decimal",false,"客户实付金额"
"VC_KHQFJE","decimal",false,"客户欠费金额"

"VC_XSHZSL","decimal",false,"销售汇总数量"
"VC_THHZSL","decimal",false,"退货汇总数量"
"VC_XSHJHZSL","decimal",false,"销售合计汇总数量"
"VC_XSHZJE","decimal",false,"销售汇总金额"
"VC_THHZJE","decimal",false,"退货汇总金额"
"VC_XSHJHZJE","decimal",false,"销售合计汇总金额"
"VC_YSKHZJE","decimal",false,"已收款汇总金额"
"VC_WSKHZJE","decimal",false,"未收款汇总金额"
"VC_JBRTCJE","decimal",false,"经办人提成金额"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_JBRID","Guid",false,"经办人ID，用于成组" 
"VC_SYR","DateTime",false,"商业日"
"VC_RXSSL","decimal",false,"日销售数量"
"VC_RTHSL","decimal",false,"日退货数量"
"VC_RXSHJSL","decimal",false,"日销售合计数量"
"VC_RXSJE","decimal",false,"日销售金额"
"VC_RTHJE","decimal",false,"日退货金额"
"VC_RXSHJJE","decimal",false,"日销售合计金额"

"VC_XSHZSL","decimal",false,"销售汇总数量, 对于月报或年报"
"VC_THHZSL","decimal",false,"退货汇总数量"
"VC_XSHJHZSL","decimal",false,"销售合计汇总数量"
"VC_XSHZJE","decimal",false,"销售汇总金额"
"VC_THHZJE","decimal",false,"退货汇总金额"
"VC_XSHJHZJE","decimal",false,"销售合计汇总金额"
]
|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_JBR","",false,""

"VC_DJYZQJE","",false,"单据应支取金额，应还有统计日报和月报，所以需要获取单据级别的记录"
"VC_DJYHJE","",false,"单据优惠金额"
"VC_DJSZQJE","",false,"单据实支取金额"
"VC_DJQFJE","",false,"单据欠费金额"
"VC_DJCKJE","",false,"单据出库金额，即销售金额"
"VC_DJRKJE","",false,"单据入库金额，即退货金额"




"VC_XSHZJE","",false,"销售汇总金额"
"VC_THHZJE","",false,"退货汇总金额"
"VC_XSHJHZJE","",false,"销售合计汇总金额"
"VC_YSKHZJE","",false,"应收款汇总金额"
"VC_WSKHZJE","",false,"未收款汇总金额"

"VC_TCJE","",false,""

"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
"VC_","",false,""
]


[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,""
"VC_CK","string",false,""
"VC_CKJM","string",false,"仓库简码"
"VC_DJLX","string",false,"单据类型"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_DJYZQJE","decimal",false,"应收金额"
"VC_DJSZQJE","decimal",false,""
"VC_DJYHJE","decimal",false,"单据优惠金额"
"VC_DJQFJE","decimal",false,"欠费金额"
"VC_DJLRJE","decimal",false,"单据利润金额"
"VC_DJXSLRL","decimal",false,"单据销售利润率"
"VC_DJBZ","string",false,"单据备注"
"VC_KH","string",false,"客户名称"
"VC_JYF","string",false,"供货商名称"
"VC_JYFJM","string",false,"供货商简码"
"VC_JYFXBH","decimal",false,"供货商编号"
"VC_JBR","string",false,"经办人，即销售业务员"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_CZY","string",false,"操作员"
"VC_SP","string",false,"商品名称"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPJM","string",false,"商品简码"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"

"VC_SPSL","decimal",false,"商品数量"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_SPLRJE","decimal",false,"商品利润金额"

"VC_SPHZSL","decimal",false,"商品汇总数量"
"VC_SPHZJE","decimal",false,"商品汇总金额"
"VC_SPHZLR","decimal",false,"商品汇总利润"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText




[
"VC_KH","string",false,"客户名称"
"VC_KHLXR","string",false,"客户联系人"
"VC_KHLXDH","string",false,"客户联系电话"
"VC_KHLX","string",false,"客户类型"
"VC_KHDJ","string",false,"客户等级"
"VC_KHDQ","string",false,"客户地区"
"VC_LSXFZSL","decimal",false,"历史消费总数量"
"VC_LSXFZJE","decimal",false,"历史消费总金额"
"VC_LSXFZLR","decimal",false,"历史消费总利润"
]
//|>Seq.iter (fun (a,_,_,_)->ObjectDumper.Write a) 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_DJH","string",false,"单据号"
"VC_KDRQ","DateTime",false,"开单日期"
"VC_SP","string",false,""
"VC_SPXBH","decimal",false,""
"VC_SPGGXH","string",false,""
"VC_SPYS","string",false,""
"VC_SPDW","string",false,""
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPZJE","decimal",false,"商品总金额"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_KHDQ","string",false,"客户地区"
"VC_KH","string",false,""
"VC_KHJM","string",false,"客户简码，用于明细过滤"
"VC_KHXBH","decimal",false,"用于明细过滤"
"VC_XFSL","decimal",false,"消费数量"
"VC_XFJE","decimal",false,"消费金额"
"VC_YHJE","decimal",false,"优惠金额"
"VC_LR","decimal",false,"利润"
"VC_HZXFSL","decimal",false,"汇总消费数量"
"VC_HZXFJE","decimal",false,""
"VC_HZYHJE","decimal",false,"汇总优惠金额"
"VC_HZLR","decimal",false,"汇总利润，用于汇总"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_KDRQ","DateTime",false,""
"VC_DJLXDM","byte",false,""
"VC_DJH","string",false,""
"VC_JYFJM","string",false,""
"VC_JYFID","Guid",false,""
"VC_JYFXBH","decimal",false,""
"VC_CGJYFJM","string",false,""
"VC_CGJYFID","Guid",false,""
"VC_CCKJM","string",false,""
"VC_RCKJM","string",false,""
"VC_CCKID","Guid",false,""
"VC_RCKID","Guid",false,""
"VC_JBRJM","string",false,""
"VC_JBRID","Guid",false,""
"VC_SPJM","string",false,""
"VC_GGXH","string",false,""
"VC_XBH","decimal",false,""
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false,""
"VC_JYF","string",false,""
"VC_JYFJM","string",false,""
"VC_JYFXBH","decimal",false,""
"VC_JYFLXID","byte",false,""
"VC_JYFLBID","byte",false,""
"VC_JYFDJID","byte",false,""
"VC_KHJM","string",false,""
"VC_KHXBH","decimal",false,""
"VC_KHLXID","byte",false,"客户类型ID"
"VC_KHDJID","byte",false,"客户等级ID"
"VC_KHDQID","Guid",false,"客户地区ID"
"VC_SPJM","string",false,""
"VC_SPGGXH","string",false,""
"VC_SPXBH","decimal",false,""
"VC_SPPC","decimal",false,""
"VC_SCRQ","DateTime",false,""
"VC_CKXBH","decimal",false,""
"VC_SPLBID","Guid",false,""
"VC_SPLBIDs","Guid[]",false,""
"VC_SPLB","string",false,""
"VC_DJH","string",false,""
"VC_CKJM","string",false,""
"VC_JBRID","Guid",false,""
"VC_JBRJM","string",false,""
"VC_DJLXID","byte",false,""
"VC_TS","int",false,"天数"
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_KHLX","string",false,""
"VC_KH","string",false,""
"VC_KHJM","string",false,"客户简码，用于明细过滤"
"VC_KHXBH","decimal",false,"用于明细过滤"
"VC_XFSL","decimal",false,""
"VC_XFJE","decimal",false,""
"VC_LR","decimal",false,""
"VC_HZXFSL","decimal",false,"汇总消费数量"
"VC_HZXFJE","decimal",false,""
"VC_HZLR","decimal",false,"汇总利润，用于汇总"
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_CJRQ","DateTime",false,""
"VC_JYF","string",false,""
"VC_JYFJM","string",false,""
"VC_JYFXBH","decimal",false,""
"VC_KH","string",false,""
"VC_KHJM","string",false,""
"VC_KHXBH","decimal",false,""
"VC_JYF","string",false,"交易方"
"VC_JYFJM","string",false,""
"VC_JYFXBH","decimal",false,""
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_JYPJ","Guid",false,"交易凭据"
"VC_JYPJLSH","string",false,""
"VC_JYPJLX","string",false,""
"VC_GLPJ","Guid",false,""
"VC_GLPJLSH","string",false,""
"VC_KDRQ","DateTime",false,""
"VC_JYRQ","DateTime",false,""
"VC_KH","string",false,""
"VC_JYJE","decimal",false,""
"VC_JYJBR","string",false,""
"VC_JYCZY","string",false,""
"VC_JYBZ","string",false,""
"VC_YZQJE","decimal",false,""
"VC_YHJE","decimal",false,""
"VC_DJZQJE","decimal",false,""
"VC_SZQJE","decimal",false,"并非单据中C_SZQJE, 而是来自于T_QFMX.."
"VC_QFJE","decimal",false,""
"VC_HJZQJE","decimal",false,"汇总支取金额，以单据汇总时应和VC_SZQJE相等"
"VC_DJJBR","string",false,""
"VC_DJCZY","string",false,""
"VC_DJSHR","string",false,""
]
//|>Seq.iter (fun (a,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_KHID","Guid",false
"VC_KHXBH","decimal",false
"VC_KH","string",false
"VC_XFJE","decimal",false
"VC_XFTHJE","decimal",false
"VC_XFHJJE","decimal",false
"VC_WFYSJE","decimal",false
"VC_WFYHJE","decimal",false
"VC_WFSSJE","decimal",false
"VC_KHQFJE","decimal",false
]
//|>Seq.iter (fun (a,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false
"VC_JYF","string",false
"VC_JYFJM","string",false
"VC_JYFXBH","decimal",false
"VC_KHJM","string",false
"VC_KHXBH","decimal",false
"VC_SPJM","string",false
"VC_SPGGXH","string",false
"VC_SPXBH","decimal",false
"VC_SPPC","decimal",false
"VC_SCRQ","DateTime",false
"VC_CKXBH","decimal",false
"VC_SPLBID","Guid",false
"VC_SPLBIDs","Guid[]",false
"VC_SPLB","string",false
"VC_DJH","string",false
"VC_CKJM","string",false
"VC_JBRID","Guid",false
"VC_JBRJM","string",false
"VC_DJLXID","byte",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false
"VC_CJRQ","DateTime",false
"VC_DJLX","string",false
"VC_KH","string",false
"VC_KHJM","string",false
"VC_KHXBH","decimal",false
"VC_JYF","string",false
"VC_CK","string",false
"VC_JBR","string",false
"VC_SPXBH","decimal",false
"VC_SPJM","string",false
"VC_SP","string",false
"VC_SPPC","decimal",false
"VC_SPDW","string",false
"VC_SPYS","string",false
"VC_SPGGXH","string",false
"VC_SPSL","decimal",false
"VC_SPDJ","decimal",false
"VC_SPZJE","decimal",false
"VC_SPSCCS","string",false
"VC_SPHZSL","decimal",false
"VC_SPHZJE","decimal",false
]
//|>Seq.iter (fun (a,_,_)->ObjectDumper.Write a)
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText



[
"VC_KHXBH","decimal",false
"VC_KHJM","string",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_ZWRQ","decimal",false
"VC_CKID","Guid",false
"VC_DQID","Guid",false
"VC_DWBMID","Guid",false
"VC_GDZCID","Guid",false
"VC_JYFID","Guid",false
"VC_KHID","Guid",false
"VC_SPID","Guid",false
"VC_YGID","Guid",false
"VC_YHZHID","Guid",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_ZWRQ","decimal",false
"VC_CKID","Guid",false
"VC_DQID","Guid",false
"VC_DWBMID","Guid",false
"VC_GDZCID","Guid",false
"VC_JYFID","Guid",false
"VC_KHID","Guid",false
"VC_SPID","Guid",false
"VC_YGID","Guid",false
"VC_YHZHID","Guid",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
  "VC_ZWRQ","decimal",false
  "VC_CK","string",false
  "VC_CKID","Guid",false
  "VC_KCSL","decimal",false
  "VC_KCCBJE","decimal",false
  "VC_CGSL","decimal",false
  "VC_CGJE","decimal",false
  "VC_CGTHSL","decimal",false
  "VC_CGTHJE","decimal",false
  "VC_CGHJSL","decimal",false
  "VC_CGHJJE","decimal",false
  "VC_XSSL","decimal",false
  "VC_XSJE","decimal",false
  "VC_XSTHSL","decimal",false
  "VC_XSTHJE","decimal",false
  "VC_XSHJSL","decimal",false
  "VC_XSHJJE","decimal",false
  "VC_XSLR","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
  "VC_CJRQ","DateTime",false
  "VC_DJH","string",false
  "VC_DJID","Guid",false
  "VC_JBR","string",false
  "VC_JBRJM","string",false
  "VC_DJLXID","byte",false
  "VC_DJLX","string",false
  "VC_JYF","string",false
  "VC_JYFJM","string",false
  "VC_CK","string",false
  "VC_CKXBH","decimal",false
  "VC_CKJM","string",false
  "VC_YZQJE","decimal",false
  "VC_YHJE","decimal",false
  "VC_SZQJE","decimal",false
  "VC_QFJE","decimal",false
  "VC_CZY","string",false
  "VC_SPXBH","decimal",false
  "VC_SPJM","string",false
  "VC_SP","string",false
  "VC_SPPC","decimal",false
  "VC_SPDW","string",false
  "VC_SPYS","string",false
  "VC_SPGGXH","string",false
  "VC_SPSL","decimal",false
  "VC_SPDJ","decimal",false
  "VC_SPZJE","decimal",false
  "VC_SPSCRQ","DateTime",false
  "VC_SPHZSL","decimal",false
  "VC_SPHZJE","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false
"VC_JYF","string",false
"VC_JYFJM","string",false
"VC_JYFXBH","decimal",false
"VC_SPJM","string",false
"VC_SPGGXH","string",false
"VC_SPXBH","decimal",false
"VC_SPPC","decimal",false
"VC_SCRQ","DateTime",false
"VC_CKXBH","decimal",false
"VC_SPLBID","Guid",false
"VC_SPLBIDs","Guid[]",false
"VC_SPLB","string",false //仅用于显示
"VC_DJH","string",false
"VC_CKJM","string",false
"VC_JBRID","Guid",false
"VC_JBRJM","string",false
"VC_DJLXID","byte",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_DJH","string",false
"VC_KDRQ","DateTime",false
"VC_JYF","string",false
"VC_JYFJM","string",false
"VC_SP","string",false
"VC_PC","decimal",false
"VC_SCRQ","DateTime",false
"VC_SPJM","string",false
"VC_SPXBH","decimal",false
"VC_SPLB","string",false
"VC_SPLBID","Guid",false
"VC_DW","string",false
"VC_GGXH","string",false
"VC_YS","string",false
"VC_DJ","decimal",false
"VC_SL","decimal",false
"VC_ZJE","decimal",false
"VC_CK","string",false
"VC_CKXBH","decimal",false
"VC_JBR","string",false
"VC_CZY","string",false
"VC_SHR","string",false
"VC_HZSL","decimal",false
"VC_HZJE","decimal",false
"VC_FLHZSL","decimal",false
"VC_FLHZJE","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
  "VC_JYPJ","Guid",false
  "VC_JYPJLSH","string",false
  "VC_JYPJLX","string",false
  "VC_GLPJ","Guid",false
  "VC_GLPJLSH","string",false
  "VC_KDRQ","DateTime",false
  "VC_JYRQ","DateTime",false
  "VC_JYF","string",false
  "VC_JYJE","decimal",false
  "VC_JYJBR","string",false
  "VC_JYCZY","string",false
  "VC_BZ","string",false
  "VC_YZQJE","decimal",false
  "VC_YHJE","decimal",false
  "VC_DJZQJE","decimal",false
  "VC_SZQJE","decimal",false
  "VC_QFJE","decimal",false
  "VC_HJZQJE","decimal",false
  "VC_DJJBR","string",false
  "VC_DJCZY","string",false
  "VC_JYBZ","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false
"VC_JYF","string",false
"VC_JYFJM","string",false
"VC_JYFXBH","decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_CJRQ","DateTime",false
"VC_DJLXDM","byte",false
"VC_DJH","string",false
"VC_CGJYFJM","string",false
"VC_CGJYFID","Guid",false
"VC_CGJYFXBH","decimal",false
"VC_CCKJM","string",false
"VC_RCKJM","string",false
"VC_CCKID","Guid",false
"VC_RCKID","Guid",false
"VC_JBRJM","string",false
"VC_JBRID","Guid",false
"VC_SPJM","string",false
"VC_GGXH","string",false
"VC_XBH","decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_JYFID","Guid",false
"VC_JYFXBH","decimal",false
"VC_JYF","string",false
"VC_XSJE","decimal",false
"VC_XSTHJE","decimal",false
"VC_XSHJJE","decimal",false
"VC_CGJE","decimal",false
"VC_CGTHJE","decimal",false
"VC_CGYHJE","decimal",false
"VC_WFYFJE","decimal",false
"VC_WFSFJE","decimal",false
"VC_WFWFJE","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_JYFXBH","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_JYFXBH","decimal",false
"VC_JYFJM","string",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText



[
"VC_FGNID","Guid",false
"VC_GN","string",false
"VC_LJ","string",false
"VC_ZPM","string",false
"VC_ZPM_GJ","string",false
"VC_CS","string",false
"VC_JYBZ","string",false
"VC_GNBZ","string",true
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText



[
"VC_XSSL"    ,  "decimal"   ,false
"VC_XSJE"     ,  "decimal"  ,false
"VC_CGSL"    ,  "decimal" ,false
"VC_CGJE"    ,  "decimal"  ,false
"VC_XSLR"    ,  "decimal"  ,false
"VC_XSCBJE" ,  "decimal" ,false
"VC_BSSL"    ,  "decimal"  ,false
"VC_BSJE"     ,  "decimal"  ,false
"VC_BYSL"    ,  "decimal"  ,false
"VC_BYJE"     ,  "decimal"  ,false
"VC_KCSL"    ,  "decimal"  ,false
"VC_KCJE"     ,  "decimal" ,false
"VC_DBCKSL",  "decimal" ,false
"VC_DBCKJE",  "decimal"  ,false
"VC_DBRKSL",  "decimal" ,false
"VC_DBRKJE",  "decimal"  ,false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_XBH","decimal",false
"VC_SPJM","string",false
"VC_GGXH","string",false
"VC_SPLB" ,"string",false
"VC_JHRQ","DateTime",false
"VC_JYFJM","string",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_SP","string",false
"VC_SPJM","string",false
"VC_GGXH","string",false
"VC_XBH","decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_JYFJM", "string",false
"VC_JYF", "string",false
"VC_KHJM","string",false
"VC_KH","string",false
"VC_RCKJM", "string",false
"VC_RCK", "string",false
"VC_CCKJM","string",false
"VC_CCK","string",false
"VC_QFJE", "decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_CGJYFID","Guid",false
"VC_CGDJID","Guid",false
"VC_SCRQ","DateTime",false
"VC_JJ","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_XBH","decimal",false
"VC_SP","string",false
"VC_DW","string",false
"VC_GGXH","string",false
"VC_YS","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_SPHZSL","decimal",false
"VC_SPHZJE","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_KDRQ","DateTime",false
"VC_DJLXDM","byte",false
"VC_DJH","string",false
"VC_JYFJM","string",false
"VC_JYFID","Guid",false
"VC_JYFXBH","decimal",false
"VC_CGJYFJM","string",false
"VC_CGJYFID","Guid",false
"VC_CCKJM","string",false
"VC_RCKJM","string",false
"VC_CCKID","Guid",false
"VC_RCKID","Guid",false
"VC_JBRJM","string",false
"VC_JBRID","Guid",false
"VC_SPJM","string",false
"VC_GGXH","string",false
"VC_XBH","decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_DJLX","string",false
"VC_DJH","string",false
"VC_DJID","Guid",false
"VC_KDRQ","DateTime",false
"VC_JYF","string",false
"VC_JYFJM","string",false
"VC_JYFID","Guid",false
"VC_CCK","string",false
"VC_CCKJM","string",false
"VC_CCKID","Guid",false
"VC_RCK","string",false
"VC_RCKJM","string",false
"VC_RCKID","Guid",false
"VC_YZQJE","decimal",false
"VC_SZQJE","decimal",false
"VC_QFJE","decimal",false
"VC_YHJE","decimal",false
"VC_JBR","string",false
"VC_JBRJM","string",false
"VC_CZY","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_QFJE","decimal",false
"VC_YZQJE","decimal",false
"VC_SZQJE","decimal",false
"VC_YFJE","decimal",false
"VC_YHJE","decimal", false
"VC_THJE","decimal",false
"VC_CGJE","decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_QXXBH","decimal",false
"VC_QXMCJM","string",false
"VC_QXCJRQ","DateTime",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_CZYXBH","decimal",false
"VC_CZYXMJM","string",false
"VC_CZYCJRQ","DateTime",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_XMJM","string",false
"VC_XBH" ,"decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_FZR","string",false
"VC_FZRJM","string",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_XBH","decimal",false
"VC_SPJM","string",false
"VC_GGXH","string",false
"VC_SPLB" ,"string",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_CZYJM","string",false
"VC_CZY","string",false
"VC_JBRJM","string",false
"VC_JBR","string",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText


[
"VC_DJH", "string",false
"VC_XBH", "decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

//生成QueryEntity的属性片段代码
("VC_FZR","string",false)
|>QueryEntitiesSegmentCodingServerAndClient.GetCode
|>Clipboard.SetText

("VC_CZY","string",false)
|>QueryEntitiesSegmentCodingServerAndClient.GetCode
|>Clipboard.SetText


[
"VC_XBH",         "decimal",false
"VC_MCJM",     "string",false
"VC_GGXH",      "string",false
"VC_SPLB",        "string",false
]
[
"VC_JYFJM", "string",false
"VC_JYF", "string",false
"VC_RCKJM", "string",false
"VC_RCK", "string",false
"VC_JBRJM","string",false
"VC_JBR","string",false
"VC_QFJE", "decimal",false
]
|>QueryEntitiesSegmentCodingServerAndClientNew.GetCode
|>Clipboard.SetText

[
"VC_JYFJM", "string",false
"VC_JYF", "string",false
"VC_RCKJM", "string",false
"VC_RCK", "string",false
"VC_JBRJM","string",false
"VC_JBR","string",false
"VC_QFJE", "decimal",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText




("VC_JBR","string",false)
|>QueryEntitiesSegmentCodingServerAndClient.GetCode
|>Clipboard.SetText





[
"VC_XBH","decimal",false
"VC_MC","string",false
"VC_MCJM","string",false
"VC_DW","string",false
"VC_GGXH","string",false
"VC_YS","string",false
"VC_SPLB","string",false
"VC_SCCS","string",false
"VC_KCSX","decimal",false
"VC_KCXX","decimal",false
"VC_JYBZ","bool",false
] 
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_SPLB","string",false
"VC_SPYS","string",false
"VC_SPDW","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_KH","string",false
"VC_FZYW","string",false
"VC_XB","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText


[
"VC_XB","string",false
"VC_XX","string",false
"VC_XL","string",false
"VC_HF","string",false
"VC_ZN","string",false
"VC_DWBM","string",false
"VC_ZW","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText

[
"VC_FZR","string",false
"VC_DQ","string",false
"VC_DQLJ","string",false
]
|>DataEntitiesSegmentCoding.GetCode
|>Clipboard.SetText




