

//#I  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"

open System
open System.Collections.Generic
open System.Linq
open System.Data
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Validation
open Microsoft.Practices.EnterpriseLibrary.Logging
open Microsoft.FSharp.Collections

//let projectPath= @"D:\Workspace\SBIIMS"

//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.DataModel.AC.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data
open WX.Data.DataModel

//=========================================================

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings //不能使用该语句，有时会将加载到内存的配置信息重置？？？
//ConfigurationManager.ConnectionStrings.["SBIIMSEntities"] 
//let sb=new SBIIMS_JXCEntitiesAdvance()
let sb=new Query.SBIIMS_QueryJXCEntitiesAdvance()
let watch=new System.Diagnostics.Stopwatch()
//=========================================================

let sb01=new SBIIMS_ACEntitiesAdvance()

//#load "...fs"
//open ....

let parentGuid=Guid.NewGuid()
let xbh=ref 1000M
[
"JXC","JHGL","CGJH","进货管理","采购进货"                                    
"JXC","JHGL","CGTH","进货管理","采购退货"                                                                
"JXC","JHGL","WLZW","进货管理","往来帐务" 
"JXC","JHGL","DJCX","进货管理","采购单据查询"  
"JXC","KCGL","WLZW","进货管理","库存往来查询"   
      
"JXC","XSGL","SPXS","销售管理","商品销售"   
"JXC","XSGL","POS","销售管理","POS 销售"                                         
"JXC","XSGL","XSTH","销售管理","顾客退货"            
"JXC","XSGL","XSHH","销售管理","销售换货"                                  
"JXC","XSGL","WLZW","销售管理","销售往来帐务"      
"JXC","XSGL","DJCX","销售管理","销售单据查询"    
"JXC","KCGL","WLZW","销售管理","库存往来查询"    

"JXC","KCGL","SPCF","库存管理","商品拆分"                                   
"JXC","KCGL","SPKB","库存管理","商品捆绑"    
"JXC","KCGL","SPBY","库存管理","商品报损" 
"JXC","KCGL","SPBS","库存管理","商品报溢"                                    
"JXC","KCGL","KCPD","库存管理","库存盘点" 
"JXC","KCGL","KCPDX","库存管理","库存盘点X"    
"JXC","KCGL","KCDB","库存管理","库存调拨"                       
"JXC","KCGL","KCYJ","库存管理" ,"库存预警"                                                
"JXC","KCGL","WLZW","库存管理","库存往来帐"   
"JXC","KCGL","KCCX","库存管理","库存查询"       
"JXC","KCGL","DJCX","库存管理","库存单据查询"      
     
"JXC","JHGL","WLZW","统计报表","供货商供货统计"   
"JXC","TJBB","SPCG","统计报表","商品采购统计"   
"JXC","TJBB","CGYCG","统计报表","采购员采购统计"   
"JXC","TJBB","KCCB","统计报表","库存成本统计"   
"JXC","XSGL","WLZW","统计报表","客户消费统计"  
"JXC","TJBB","SPXS","统计报表","商品销售统计"       
"JXC","TJBB","XSYXS","统计报表","销售员销售统计"   
"JXC","TJBB","XSPH","统计报表","商品销售排行"                                                
"JXC","TJBB","YYFX","统计报表","营业分析"    

"JXC","ZHGL","CWGL","综合管理","财务综合管理"  
"JXC","ZHGL","XZGL","综合管理","薪资管理"  
"JXC","ZHGL","JYFYGL","综合管理","经营费用管理"  
"JXC","ZHGL","BJGL","综合管理","报价管理"  
"JXC","ZHGL","HTGL","综合管理","合同管理"         
"JXC","ZHGL","GHSGL","综合管理","供货商管理"    
"JXC","ZHGL","KHGL","综合管理","客户管理"      
"JXC","ZHGL","HYGL","综合管理","会员管理" 
"JXC","ZHGL","JYFGL","综合管理","经营交易方管理"     
"JXC","ZHGL","YWYGL","综合管理","业务员管理"         
"JXC","ZHGL","XFPGL","综合管理","消费品管理"         
"JXC","ZHGL","FJPGL","综合管理","废旧品管理"               
"JXC","ZHGL","GDZCGL","综合管理","固定资产管理"                                  
"JXC","ZHGL","KHJH","综合管理","客户借货管理"                            
"JXC","ZHGL","SJTX","综合管理","事件提醒"                                    

"JXC","JBXX","SPWH","基本信息管理","商品信息维护"                  
"JXC","JBXX","GHSWH","基本信息管理","供货商信息维护"       
"JXC","JBXX","KHWH","基本信息管理","客户信息维护"     
"JXC","JBXX","JYFWH","基本信息管理","交易方信息维护"    
"JXC","JBXX","YGWH","基本信息管理","员工信息维护"    
"JXC","JBXX","CZYWH","基本信息管理","操作员信息维护"         
"JXC","JBXX","CKWH","基本信息管理","仓库信息维护"                                         
"JXC","JBXX","XFPWH","基本信息管理","消费品信息维护"                  
"JXC","JBXX","FJPWH","基本信息管理","废旧品信息维护"                                         
"JXC","JBXX","ZCLBWH","基本信息管理","固定资产类别维护"          

"JXC","ZHBB","DJBB","综合报表管理","单据报表"          
"JXC","ZHBB","QDBB","综合报表管理","清单报表"       
"JXC","ZHBB","TBBB","综合报表管理","图表报表"    
"JXC","ZHBB","DCBB","综合报表管理","导出"    
                              
"JXC","XTGL","BFHF","系统管理","备份恢复"             
"JXC","XTGL","YZWJS","系统管理","月账务结算"                         
"JXC","XTGL","NZWJS","系统管理","年账务结算"    
"JXC","XTGL","XTRZ","系统管理","系统日志"                                   
"JXC","XTGL","XTCSH","系统管理","系统初始化"                              
"JXC","XTGL","QCJZ","系统管理","期初建账"             
"JXC","XTGL","TMDY","系统管理","条码打印"                      
                      
"AC","","CZYJSGL","访问控制","操作员角色管理"
"AC","","JSGNGL","访问控制","角色功能管理"
"AC","","JSQXGL","访问控制","角色权限管理" 
"AC","","CZYWH","访问控制","操作员维护"
"AC","","GNWH","访问控制","功能维护" 
"AC","","JSWH","访问控制","角色维护" 
"AC","","QXWH","访问控制","权限维护" 
]
|>Seq.groupBy (fun (_,_,_,a,_)->a)
|>Seq.iteri (fun i (a,b)->
    match Guid.NewGuid() with
    | x ->
        match new T_GNJD() with
        | y ->
            y.C_ID<-x
            y.C_FID<-parentGuid 
            y.C_GXRQ<-DateTime.Now
            y.C_CJRQ<-DateTime.Now
            incr xbh
            y.C_XBH<- !xbh
            y.C_MC<-a 
            y.C_JDJB<-1uy 
            y.C_JDXH<- byte i 
            y.C_YZBZ<-false 
            y.C_JYBZ<-false
            y.C_ZPQYZT<- 0uy
            y.C_SJLX<-""
            y.C_GJSJLX<-""
            y.C_LJ<-"" 
            y.C_CS<-""
            y.C_ZPM<-""
            y.C_GJZPM<-"" 
            y.C_ZPURI<-""
            y.C_GJZPURI<-""
            y.C_ZPLJ<-""
            y.C_MMKJ<-""
            y.C_BZ<-""
            sb01.AddToT_GNJD(y)
            b
            |>Seq.iteri (fun j (m1,m2,m3,m4,m5) ->
                match new BD_T_GNJD() with
                | z ->
                    z.C_ID<-Guid.NewGuid()
                    z.C_FID<-x 
                    z.C_GXRQ<-DateTime.Now
                    z.C_CJRQ<-DateTime.Now
                    incr xbh
                    z.C_XBH<- !xbh
                    z.C_MC<-m5 
                    z.C_JDJB<-2uy 
                    z.C_JDXH<- byte j 
                    z.C_YZBZ<-true 
                    z.C_JYBZ<-false
                    z.C_ZPQYZT<- 1uy
                    match 
                      (match m2 with NotNullOrWhiteSpace _ -> m2+"_"+m3 | _ ->m3),
                      (match m2 with NotNullOrWhiteSpace _ -> m1+"."+m2+"."+m3 | _ ->m1+"."+m3) with
                    | u,v ->
                        z.C_SJLX<- String.Format(@"FVM_{0}",u)
                        z.C_GJSJLX<-String.Format(@"FVM_{0}_Advance",u)
                        z.C_LJ<-"" 
                        z.C_CS<-""
                        z.C_ZPM<-String.Format(@"WX.Data.FViewModel.{0}",v)
                        z.C_GJZPM<-String.Format(@"WX.Data.FViewModelAdvance.{0}",v)
                        z.C_ZPURI<-String.Format(@"/WX.Data.View.ViewModelTemplate.{0};Component/VMT_{1}.xaml",v,u)
                        z.C_GJZPURI<-String.Format(@"/WX.Data.View.ViewModelTemplateAdvance.{0};Component/VMT_{1}_Advance.xaml",v,u)
                        z.C_ZPLJ<-""
                        z.C_MMKJ<-""
                        z.C_BZ<-""
                    sb01.AddToT_GNJD(z)
            )
    )
sb01.SaveChanges()|>ignore

match new T_GNJD() with
| y ->
    y.C_ID<-Guid.NewGuid()
    y.C_FID<-DefaultGuidValue 
    y.C_GXRQ<-DateTime.Now
    y.C_CJRQ<-DateTime.Now
    y.C_XBH<- 1000M
    y.C_MC<-"系统功能" 
    y.C_JDJB<- 0uy 
    y.C_JDXH<- 0uy 
    y.C_YZBZ<-false 
    y.C_JYBZ<-false
    y.C_ZPQYZT<- 0uy
    y.C_SJLX<-""
    y.C_GJSJLX<-""
    y.C_LJ<-"" 
    y.C_CS<-""
    y.C_ZPM<-""
    y.C_GJZPM<-"" 
    y.C_ZPURI<-""
    y.C_GJZPURI<-""
    y.C_ZPLJ<-""
    y.C_MMKJ<-""
    y.C_BZ<-""
    sb01.AddToT_GNJD y
    sb01.SaveChanges()|>ignore

sb.lz

watch.Reset()
watch.Start()
CompiledQuery.Compile(fun sb->
sb.T_DJ_XSGL
|>Seq.toArray
)
//|>ObjectDumper.Write
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds

let GetDJBB_CGJHDView (queryEntity:BQ_ZHBB_DJBB_Advance)=
        sb.T_DJSP_JHGL
          .Include(FTN.T_DJ_JHGL)
          .Include(FTN.T_DJ_JHGL.+FTN.T_WF)
          .Include(FTN.T_DJ_JHGL.+FTN.T_QFMX_JHGL)
          .Include(FTN.T_GHS)
          .Include(FTN.T_GHS.+FTN.T_GHSYWY)
          .Include(FTN.T_CK1).Include(FTN.T_CK)
          .Include(FTN.T_YG1).Include(FTN.T_YG)
          .Include(FTN.T_SP)
          .Include(FTN.T_SP.+FTN.T_SPYS)
          .Include(FTN.T_SP.+FTN.T_SPDW)
        |>Seq.filter (fun a->
            match a.C_DJLX with
            | 1uy ->true
            | _ ->false
            && 
            match a.C_CJRQ,queryEntity.VC_CJRQ,queryEntity.VC_CJRQSecond with
            | x,y,z when y.HasValue && z.HasValue && y.Value=DateTime.MinValue ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value=DateTime.MaxValue ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.C_DJID,queryEntity.VC_DJID with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            )
        |>fun a->
            if queryEntity.TotalCount=0 then queryEntity.TotalCount<- a|>Seq.length
            a  
        |>PSeq.skip (queryEntity.PageSize * queryEntity.PageIndex)
        |>PSeq.take queryEntity.PageSize
        |>Seq.map (fun a->
            match new BD_V_DJBB_CGJHD_Advance() with
            | x->
                x.VC_DJID<-a.C_DJID
                x.VC_CJRQ<-a.C_CJRQ
                x.VC_DJH<-a.C_DJH
                x.VC_CK<-a.T_CK1.C_MC
                match a.T_DJ_JHGL.T_WF with
                | y -> 
                    x.VC_WFDW<-y.C_MC
                    x.VC_WFLXDH<-y.C_LXDH
                    x.VC_WFLXDZ<-y.C_LXDZ
                match a.T_GHS with
                | y ->
                    x.VC_GHS<-y.C_MC
                    x.VC_GHSLXDH<-y.C_LXDH
                    x.VC_GHSLXDZ<-y.C_LXDZ
                    x.VC_GHSLXR<-
                      let fa ()=
                          match y.T_GHSYWY|>PSeq.headOrDefault with
                          | Some y ->y.C_XM
                          | _ ->String.Empty
                      match y.C_LXR, y.T_GHSYWY with
                      | z,w when z.HasValue  -> 
                          match w|>Seq.tryFind (fun b->b.C_ID=z.Value) with
                          | Some z  ->z.C_XM
                          | _ ->fa ()
                      | _ ->fa ()
                match a.T_DJ_JHGL with
                | y ->
                    x.VC_YFJE<-y.C_DJYZQJE |>op_UnaryNegation  
                    x.VC_DJYHJE<-y.C_DJYHJE
                    match y.C_KDJQBZ with //开单即结清标志，提高关联性能
                    | true ->
                        x.VC_WFJE<-0M    //不能使用台账按查询单据的时间段来查询欠费，使用单据最终的欠费金额才符合实际要求
                        x.VC_SFJE<-x.VC_YFJE-x.VC_DJYHJE 
                    | _ ->
                        x.VC_WFJE<-match y.T_QFMX_JHGL with null ->0M | z ->z.C_QFJE |>op_UnaryNegation //正负设计和单据一致   
                        x.VC_SFJE<-x.VC_YFJE-x.VC_DJYHJE-x.VC_WFJE 
                x.VC_JBR<-a.T_YG1.C_XM
                x.VC_CZY<-a.T_YG.C_XM
                x.VC_DJZT<-
                  match a.T_DJ_JHGL.C_DJZT with
                  | 1uy ->"待审核"
                  | 2uy -> "审核中"
                  | 3uy -> "已审核"
                  | 4uy -> "取消审核"
                  | _ ->String.Empty
                x.VC_DJBZ<-a.C_BZ
                match a.T_SP with
                | y ->
                    x.VC_SP<-y.C_MC
                    x.VC_SPXBH<-y.C_XBH
                    x.VC_SPYS<-y.T_SPYS.C_YS
                    x.VC_SPGGXH<-y.C_GGXH
                    x.VC_SPDW<-y.T_SPDW.C_DW
                x.VC_SPPC<-a.C_PC
                x.VC_SPSCRQ<-a.C_SCRQ
                x.VC_SPDJ<-a.C_DJ
                x.VC_SPZKL<-a.C_ZKL
                x.VC_SPZHJ<-a.C_ZHJ
                x.VC_SPSL<-a.C_VSL  //显示为正
                x.VC_SPZHJE<-a.C_VZHJE
                x)
        |>Seq.toArray 
match new BQ_ZHBB_DJBB_Advance() with
| x ->
    x.VC_DJID<-Nullable<_> (Guid.Parse("0cb68750-23a5-4aa4-b424-a45cf227e791")) 
    GetDJBB_CGJHDView x
|>ObjectDumper.Write 


let x01=
  let rec GetData n (x:ObjectQuery<Query.T_DJ_KH>)=
    match n with
    | 0 ->x
    | _ ->GetData (n-1) (x.UnionAll (sb.T_DJ_KH.Include(FTN.T_KH).Include(FTN.T_QFMX_KH)))
  GetData 100 (sb.T_DJ_KH.Include(FTN.T_KH).Include(FTN.T_QFMX_KH))

x01
|>Seq.length
|>ObjectDumper.Write
  //|>Seq.length
  //|>ObjectDumper.Write

let x01=
  seq{
  for i=0 to 10 do
    yield sb.T_DJ_KH
  }
  |>Seq.concat
  //|>Seq.length
  //|>ObjectDumper.Write

x01
|>ObjectDumper.Write

watch.Reset()
watch.Start()
x01.Include(FTN.T_KH).Include(FTN.T_QFMX_KH)
|>PSeq.filter (fun a->true
    )
|>PSeq.groupBy (fun a->a.C_KH)
|>Seq.map (fun (a,b)->
    match new BD_V_XS_WLZ_KHZW_Advance() with
    | x->
        match b|>PSeq.head with
        | y ->
            x.VC_KHID<-y.C_KH
            x.VC_KHXBH<-y.T_KH.C_XBH
            x.VC_KH<-y.T_KH.C_MC //Group之后必然有元素
        x.VC_XFJE<-b|>PSeq.sumBy (fun c->c.C_CKJE)
        x.VC_XFTHJE<-b|>PSeq.sumBy (fun c->c.C_RKJE)
        x.VC_XFHJJE<-b|>PSeq.sumBy (fun c->c.C_CKJE-c.C_RKJE) //该金额应该和C_YZQJE的合计金额相等
        x.VC_WFYSJE<-b|>PSeq.sumBy (fun c->c.C_YZQJE)
        x.VC_WFYHJE<-b|>PSeq.sumBy (fun c->c.C_YHJE)
        x.VC_KHQFJE<-b|>PSeq.sumBy (fun c->match c.T_QFMX_KH with null ->0M | x ->x.C_QFJE )   //性能??, 不能是b|>PSeq.sumBy(fun c->c.C_SZQJE), 因单据表中的C_SZQJE只在以单据付费和帐务结算时更新
        x.VC_WFSSJE<-x.VC_WFYSJE-x.VC_WFYHJE-x.VC_KHQFJE
        x)
|>PSeq.toArray 
|>ObjectDumper.Write

watch.Stop()
watch.ElapsedMilliseconds
|>ObjectDumper.Write

//-------------------------------------------------------------------
watch.Reset()
watch.Start()
x01.Include(FTN.T_KH).Include(FTN.T_QFMX_KH)
|>Seq.filter (fun a->true
    )
|>Seq.groupBy (fun a->a.C_KH)
|>Seq.map (fun (a,b)->
    match new BD_V_XS_WLZ_KHZW_Advance() with
    | x->
        match b|>Seq.head with
        | y ->
            x.VC_KHID<-y.C_KH
            x.VC_KHXBH<-y.T_KH.C_XBH
            x.VC_KH<-y.T_KH.C_MC //Group之后必然有元素
        x.VC_XFJE<-b|>Seq.sumBy (fun c->c.C_CKJE)
        x.VC_XFTHJE<-b|>Seq.sumBy (fun c->c.C_RKJE)
        x.VC_XFHJJE<-b|>Seq.sumBy (fun c->c.C_CKJE-c.C_RKJE) //该金额应该和C_YZQJE的合计金额相等
        x.VC_WFYSJE<-b|>Seq.sumBy (fun c->c.C_YZQJE)
        x.VC_WFYHJE<-b|>Seq.sumBy (fun c->c.C_YHJE)
        x.VC_KHQFJE<-b|>Seq.sumBy (fun c->match c.T_QFMX_KH with null ->0M | x ->x.C_QFJE )   //性能??, 不能是b|>Seq.sumBy(fun c->c.C_SZQJE), 因单据表中的C_SZQJE只在以单据付费和帐务结算时更新
        x.VC_WFSSJE<-x.VC_WFYSJE-x.VC_WFYHJE-x.VC_KHQFJE
        x)
|>Seq.toArray 
|>ObjectDumper.Write

watch.Stop()
watch.ElapsedMilliseconds
|>ObjectDumper.Write

//---------------------------------------------------------------------------------------
sb.T_DJ_GHS_JQ.Count()
|>ObjectDumper.Write 

sbQuery.T_DJ_KH.Count()
|>ObjectDumper.Write 

sbQuery.T_DJ_KH
|>ObjectDumper.Write 

sbQuery.T_DJSP_KH
|>ObjectDumper.Write 

sbQuery.T_DJSP_KH.Count()
|>ObjectDumper.Write 

sbQuery.T_DJSP_KH.Include(FTN.T_DJ_KH)
|>Seq.map (fun a->
    a.T_DJ_KH.C_BZ
    )
|>ObjectDumper.Write 





//----------------------------------------------------------------------------------------------
//库存成本统计，也可作为i帐务结算的参考
let queryEntity=new BQ_ZWTJ_Advance()
(
[|
match new BD_V_TJ_KCCB_Advance() with
| x ->
    x.VC_ZWRQ<-10000000
    match 
      sb.T_KCSP  //库存
      |>PSeq.filter (fun a->
          match a.C_CK,queryEntity.VC_CKID with
          | x,y when y.HasValue ->x =y.Value
          | _ ->true
          ) 
      with
    | y ->
        x.VC_KCSL<- y|>PSeq.sumBy (fun a->a.C_KCSL)
        x.VC_KCCBJE<-y|>PSeq.sumBy (fun a->a.C_CBJJ*a.C_KCSL) //成本金额
    match 
      sb.T_DJ_GHS  //采购
      |>PSeq.filter (fun a->
          match a.C_CCK,a.C_RCK,queryEntity.VC_CKID with
          | x,y,z when z.HasValue ->x =z.Value || y=z.Value 
          | _ ->true
          ) 
      with
    | y ->
        x.VC_CGSL<-y|>PSeq.sumBy (fun a->a.C_RKSL)  //采购进货
        x.VC_CGJE<-y|>PSeq.sumBy (fun a->a.C_RKJE)
        x.VC_CGTHSL<-y|>PSeq.sumBy (fun a->a.C_CKSL)
        x.VC_CGTHJE<-y|>PSeq.sumBy (fun a->a.C_CKJE)
        x.VC_CGHJSL<-x.VC_CGSL-x.VC_CGTHSL
        x.VC_CGHJJE<-x.VC_CGJE-x.VC_CGTHJE
    match 
      sb.T_DJ_KH //销售
      |>PSeq.filter (fun a->
          match a.C_CCK,a.C_RCK,queryEntity.VC_CKID with
          | x,y,z when z.HasValue ->x =z.Value || y=z.Value 
          | _ ->true
          ) 
      with
    | y ->
        x.VC_XSSL<-y|>PSeq.sumBy (fun a->a.C_CKSL)  //销售
        x.VC_XSJE<-y|>PSeq.sumBy (fun a->a.C_CKJE)
        x.VC_XSTHSL<-y|>PSeq.sumBy (fun a->a.C_RKSL) //销售退货
        x.VC_XSTHJE<-y|>PSeq.sumBy (fun a->a.C_RKJE)
        x.VC_XSHJSL<-x.VC_XSSL-x.VC_XSTHSL
        x.VC_XSHJJE<-x.VC_XSJE-x.VC_XSTHJE
        x.VC_XSCBJE<-y|>PSeq.sumBy (fun a->a.C_CKCBJE-a.C_RKCBJE)
        x.VC_XSLR<-x.VC_XSHJJE-x.VC_XSCBJE  //销售合计金额-销售合计成本
    match 
      sb.T_DJ_WF //库存调拨
      |>PSeq.filter (fun a->
          a.C_DJLX=33uy
          &&
          match a.C_CCK,a.C_RCK,queryEntity.VC_CKID with
          | x,y,z when z.HasValue ->x =z.Value || y=z.Value 
          | _ ->true
          ) 
      with
    | y ->
        x.VC_DBCKSL<-y|>PSeq.sumBy (fun a->a.C.CKSL)
        x.VC_DBCKJE<-y|>PSeq.sumBy (fun a->a.C.CKJE)
        x.VC_DBRKSL<-y|>PSeq.sumBy (fun a->a.C.RKSL)
        x.VC_DBRKJE<-y|>PSeq.sumBy (fun a->a.C.RKJE)
    match 
      sb.T_DJ_WF //库存报损报溢
      |>PSeq.filter (fun a->
          match a.C_DJLX with
          | 36uy | 37uy ->true
          | _ ->false
          //(a.C_DJLX=36uy || a.C_DJLX=37uy)
          &&
          match a.C_CCK,a.C_RCK,queryEntity.VC_CKID with
          | x,y,z when z.HasValue ->x =z.Value || y=z.Value 
          | _ ->true
          ) 
      with
    | y ->
        x.VC_BSSL<-y|>PSeq.sumBy (fun a->a.C.CKSL)
        x.VC_BSJE<-y|>PSeq.sumBy (fun a->a.C.CKJE)
        x.VC_BYSL<-y|>PSeq.sumBy (fun a->a.C.RKSL)
        x.VC_BYJE<-y|>PSeq.sumBy (fun a->a.C.RKJE)
    x
|]
),
(
sb.T_YZW_CK
|>PSeq.filter (fun a->
    match a.C_CK,queryEntity.VC_CKID with
    | x,y when y.HasValue ->x =y.Value
    | _ ->true
    &&
    match a.C_ZWRQ,queryEntity.VC_ZWRQ,queryEntity.VC_ZWRQSecond with
    | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
    | x,y,_ when y.HasValue ->x=y.Value
    | _ ->true
    )
|>PSeq.groupBy (fun a->a.C_ZWRQ)
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_ZWRQ<-a
        x.VC_CGSL<-b|>PSeq.sumBy (fun c->c.C_CGSL)
        x.VC_CGJE<-b|>PSeq.sumBy (fun c->c.C_CGJE)
        x.VC_CGTHSL<-b|>PSeq.sumBy (fun c->c.C_CGTHSL)
        x.VC_CGTHJE<-b|>PSeq.sumBy (fun c->c.C_CGTHJE)
        x.VC_XSSL<-b|>PSeq.sumBy (fun c->c.C_XSSL)
        x.VC_XSJE<-b|>PSeq.sumBy (fun c->c.C_XSJE)
        x.VC_XSTHSL<-b|>PSeq.sumBy (fun c->c.C_XSTHSL)
        x.VC_XSTHJE<-b|>PSeq.sumBy (fun c->c.C_XSTHJE)
        x.VC_XSCBJE<-b|>PSeq.sumBy (fun c->c.C_XSCBJE)
        x.VC_XSLRJE<-b|>PSeq.sumBy (fun c->c.C_XSLRJE)
        x.VC_DBCKSL<-b|>PSeq.sumBy (fun c->c.C_DBCKSL)
        x.VC_DBCKJE<-b|>PSeq.sumBy (fun c->c.C_DBCKJE)
        x.VC_DBRKSL<-b|>PSeq.sumBy (fun c->c.C_DBRKSL)
        x.VC_DBRKJE<-b|>PSeq.sumBy (fun c->c.C_DBRKJE)
        x.VC_BSSL<-b|>PSeq.sumBy (fun c->c.C_BSSL)
        x.VC_BSJE<-b|>PSeq.sumBy (fun c->c.C_BSJE)
        x.VC_BYSL<-b|>PSeq.sumBy (fun c->c.C_BYSL)
        x.VC_BYJE<-b|>PSeq.sumBy (fun c->c.C_BYJE)
        x
    )
)
|>fun (a,b)->
    a
    |>PSeq.append b
    |>fun a->
        match queryEntity.VC_CKID with
        | y when y.HasValue->
           match sb.T_CK|>PSeq.tryFind (fun b->b.C_ID=y.Value) with
           |  Some z ->a|>Seq.iter (fun b->b.VC_CK<-z.C_MC)
           | _ ->()
        | _ ->a|>Seq.iter (fun b->b.VC_CK<-"所有仓库")
        a
    |>PSeq.sortBy (fun c-> -c.VC_ZWRQ)
    |>PSeq.toArray




let queryEntity=new BQ_ZWTJ_Advance()
sb.T_YZW_CK
|>PSeq.filter (fun a->
    match a.C_CK,queryEntity.VC_CKID with
    | x,y when y.HasValue ->x =y.Value
    | _ ->true
    &&
    match a.C_ZWRQ,queryEntity.VC_ZWRQ,queryEntity.VC_ZWRQSecond with
    | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
    | x,y,_ when y.HasValue ->x=y.Value
    | _ ->true
    )
|>PSeq.groupBy (fun a->a.C_ZWRQ)
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_ZWRQ<-a
        x.VC_CGSL<-b|>PSeq.sumBy (fun c->c.C_CGSL)
        x.VC_CGJE<-b|>PSeq.sumBy (fun c->c.C_CGJE)
        x.VC_CGTHSL<-b|>PSeq.sumBy (fun c->c.C_CGTHSL)
        x.VC_CGTHJE<-b|>PSeq.sumBy (fun c->c.C_CGTHJE)
        x.VC_XSSL<-b|>PSeq.sumBy (fun c->c.C_XSSL)
        x.VC_XSJE<-b|>PSeq.sumBy (fun c->c.C_XSJE)
        x.VC_XSTHSL<-b|>PSeq.sumBy (fun c->c.C_XSTHSL)
        x.VC_XSTHJE<-b|>PSeq.sumBy (fun c->c.C_XSTHJE)
        x.VC_XSCBJE<-b|>PSeq.sumBy (fun c->c.C_XSCBJE)
        x.VC_XSLRJE<-b|>PSeq.sumBy (fun c->c.C_XSLRJE)
        x.VC_DBCKSL<-b|>PSeq.sumBy (fun c->c.C_DBCKSL)
        x.VC_DBCKJE<-b|>PSeq.sumBy (fun c->c.C_DBCKJE)
        x.VC_DBRKSL<-b|>PSeq.sumBy (fun c->c.C_DBRKSL)
        x.VC_DBRKJE<-b|>PSeq.sumBy (fun c->c.C_DBRKJE)
        x.VC_BSSL<-b|>PSeq.sumBy (fun c->c.C_BSSL)
        x.VC_BSJE<-b|>PSeq.sumBy (fun c->c.C_BSJE)
        x.VC_BYSL<-b|>PSeq.sumBy (fun c->c.C_BYSL)
        x.VC_BYJE<-b|>PSeq.sumBy (fun c->c.C_BYJE)
        x
    )
|>fun a->
    match queryEntity.VC_CKID with
    | y when y.HasValue->
       match sb.T_CK|>PSeq.find (fun b->b.C_ID=y.Value) with
       |  z ->a|>Seq.iter (fun b->b.VC_CK<-z.C_MC)
    | _ ->a|>Seq.iter (fun b->b.VC_CK<-"所有仓库")
    a
       
|>ObjectDumper.Write

watch.Reset()
watch.Start()

match new BD_V_TJ_KCCB_Advance() with
| x ->
    x.VC_ZWRQ<-10000000
    match queryEntity.VC_CKID with
    | y when y.HasValue->
       match sb.T_CK|>PSeq.find (fun a->a.C_ID=y.Value) with
       |  z ->x.VC_CK<-z.C_MC
    | _ ->x.VC_CK<"所有仓库"
    match 
      sb.T_KCSP
      |>PSeq.filter (fun a->
          match a.C_CK,queryEntity.VC_CKID with
          | x,y when y.HasValue ->x =y.Value
          | _ ->true
          ) 
      with
    | y ->
        x.VC_KCSL<- y|>PSeq.sumBy (fun a->a.C_KCSL)
        x.VC_KCCBJE<-y|>PSeq.sumBy (fun a->a.C_CBJJ*a.C_KCSL) //成本金额
    match 
      sb.T_DJ_GHS
      |>PSeq.filter (fun a->
          match a.C_CCK,a.C_RCK,queryEntity.VC_CKID with
          | x,y,z when z.HasValue ->x =z.Value || y=z.Value 
          | _ ->true
          ) 
      with
    | y ->
        x.VC_CGSL<-y|>PSeq.sumBy (fun a->a.C_RKSL)  //采购进货
        x.VC_CGJE<-y|>PSeq.sumBy (fun a->a.C_RKJE)
        x.VC_CGTHSL<-y|>PSeq.sumBy (fun a->a.C_CKSL)
        x.VC_CGTHJE<-y|>PSeq.sumBy (fun a->a.C_CKJE)
        x.VC_CGHJSL<-x.VC_CGSL-x.VC_CGTHSL
        x.VC_CGHJJE<-x.VC_CGJE-x.VC_CGTHJE
    match 
      sb.T_DJ_KH
      |>PSeq.filter (fun a->
          match a.C_CCK,a.C_RCK,queryEntity.VC_CKID with
          | x,y,z when z.HasValue ->x =z.Value || y=z.Value 
          | _ ->true
          ) 
      with
    | y ->
        x.VC_XSSL<-y|>PSeq.sumBy (fun a->a.C_CKSL)  //销售
        x.VC_XSJE<-y|>PSeq.sumBy (fun a->a.C_CKJE)
        x.VC_XSTHSL<-y|>PSeq.sumBy (fun a->a.C_RKSL) //销售退货
        x.VC_XSTHJE<-y|>PSeq.sumBy (fun a->a.C_RKJE)
        x.VC_XSHJSL<-x.VC_XSSL-x.VC_XSTHSL
        x.VC_XSHJJE<-x.VC_XSJE-x.VC_XSTHJE
        x.VC_XSLR<-x.VC_XSHJJE-(y|>PSeq.sumBy (fun a->a.C_CKCBJE-a.C_RKCBJE))  //销售合计金额-销售合计成本
    x

|>ObjectDumper.Write

watch.Stop()
watch.ElapsedMilliseconds.ToString()
|>ObjectDumper.Write

//------------------------------------------------------------------------------------------------

watch.Reset()
watch.Start()
//性能较好
(
sb.T_KCSP
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a ->a.C_CK)
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-a
        x.VC_KCSL<-b|>PSeq.sumBy (fun c->c.C_KCSL)
        x.VC_KCCBJE<-b|>PSeq.sumBy (fun c->c.C_CBJJ*c.C_KCSL) //成本金额
        x
   )
,
sb.T_DJ_GHS
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a->match a.C_RCK with x when x=DefaultGuidValue ->a.C_CCK | _ ->a.C_RCK )
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-a
        x.VC_CGSL<-b|>PSeq.sumBy (fun c->c.C_RKSL)  //采购进货
        x.VC_CGJE<-b|>PSeq.sumBy (fun c->c.C_RKJE)
        x.VC_CGTHSL<-b|>PSeq.sumBy (fun c->c.C_CKSL)
        x.VC_CGTHJE<-b|>PSeq.sumBy (fun c->c.C_CKJE)
        x
    )
,
sb.T_DJ_KH
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a->match a.C_RCK with x when x=DefaultGuidValue ->a.C_CCK | _ ->a.C_RCK)
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-a
        x.VC_XSSL<-b|>PSeq.sumBy (fun c->c.C_CKSL)  //销售
        x.VC_XSJE<-b|>PSeq.sumBy (fun c->c.C_CKJE)
        x.VC_XSTHSL<-b|>PSeq.sumBy (fun c->c.C_RKSL) //销售退货
        x.VC_XSTHJE<-b|>PSeq.sumBy (fun c->c.C_RKJE)
        x
    )
)
|>fun (a,b,c) ->
    PSeq.groupJoin a b (fun d->d.VC_CKID) (fun e->e.VC_CKID) 
      (fun d e->
        match e|>PSeq.headOrDefault with
        | Some x -> 
            d.VC_CGSL<-x.VC_CGSL
            d.VC_CGJE<-x.VC_CGJE
            d.VC_CGTHSL<-x.VC_CGTHSL
            d.VC_CGTHJE<-x.VC_CGTHJE
        | _ ->()
        d),c
|>fun (a,b)->  
    PSeq.groupJoin a b (fun d->d.VC_CKID) (fun e->e.VC_CKID)
      (fun d e->
        match e|>PSeq.headOrDefault with
        | Some x ->
            d.VC_XSSL<-x.VC_XSSL
            d.VC_XSJE<-x.VC_XSJE
            d.VC_XSTHSL<-x.VC_XSTHSL
            d.VC_XSTHJE<-x.VC_XSTHJE
        | _ ->()
        d.VC_CGHJSL<-d.VC_CGSL-d.VC_CGTHSL
        d.VC_CGHJJE<-d.VC_CGJE-d.VC_CGTHJE
        d.VC_XSHJSL<-d.VC_XSSL-d.VC_XSTHSL
        d.VC_XSHJJE<-d.VC_XSJE-d.VC_XSTHJE
        d.VC_XSLR<-d.VC_XSHJJE-d.VC_CGHJJE //利润
        d)
|>fun a->
    PSeq.groupJoin a sb.T_CK (fun d->d.VC_CKID) (fun e->e.C_ID) 
      (fun d e->
        d.VC_CK<-match e|>PSeq.headOrDefault with Some x->x.C_MC | _ ->null
        d) 
|>ObjectDumper.Write

watch.Stop()
watch.ElapsedMilliseconds.ToString()
|>ObjectDumper.Write
//-------------------------------------------------------------------------------------------------

watch.Reset()
watch.Start()
//性能较好
(
sb.T_KCSP
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a ->a.C_CK)
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-a
        x.VC_KCSL<-b|>PSeq.sumBy (fun c->c.C_KCSL)
        x.VC_KCCBJE<-b|>PSeq.sumBy (fun c->c.C_CBJJ*c.C_KCSL) //成本金额
        x
   )
,
sb.T_DJ_GHS
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a->match a.C_RCK with x when x=DefaultGuidValue ->a.C_CCK | _ ->a.C_RCK )
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-a
        x.VC_CGSL<-b|>PSeq.sumBy (fun c->c.C_RKSL)  //采购进货
        x.VC_CGJE<-b|>PSeq.sumBy (fun c->c.C_RKJE)
        x.VC_CGTHSL<-b|>PSeq.sumBy (fun c->c.C_CKSL)
        x.VC_CGTHJE<-b|>PSeq.sumBy (fun c->c.C_CKJE)
        x
    )
,
sb.T_DJ_KH
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a->match a.C_RCK with x when x=DefaultGuidValue ->a.C_CCK | _ ->a.C_RCK)
|>Seq.map (fun (a,b)->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-a
        x.VC_XSSL<-b|>PSeq.sumBy (fun c->c.C_CKSL)  //销售
        x.VC_XSJE<-b|>PSeq.sumBy (fun c->c.C_CKJE)
        x.VC_XSTHSL<-b|>PSeq.sumBy (fun c->c.C_RKSL) //销售退货
        x.VC_XSTHJE<-b|>PSeq.sumBy (fun c->c.C_RKJE)
        x
    )
)
|>fun (a,b,c) ->
    PSeq.groupJoin a b (fun d->d.VC_CKID) (fun e->e.VC_CKID) 
      (fun d e->
        match e|>PSeq.headOrDefault with
        | Some x -> 
            d.VC_CGSL<-x.VC_CGSL
            d.VC_CGJE<-x.VC_CGJE
            d.VC_CGTHSL<-x.VC_CGTHSL
            d.VC_CGTHJE<-x.VC_CGTHJE
        | _ ->()
        d)
    |>fun d->  
        PSeq.groupJoin d c (fun e->e.VC_CKID) (fun f->f.VC_CKID)
          (fun e f->
            match f|>PSeq.headOrDefault with
            | Some x ->
                e.VC_XSSL<-x.VC_XSSL
                e.VC_XSJE<-x.VC_XSJE
                e.VC_XSTHSL<-x.VC_XSTHSL
                e.VC_XSTHJE<-x.VC_XSTHJE
            | _ ->()
            e.VC_CGHJSL<-e.VC_CGSL-e.VC_CGTHSL
            e.VC_CGHJJE<-e.VC_CGJE-e.VC_CGTHJE
            e.VC_XSHJSL<-e.VC_XSSL-e.VC_XSTHSL
            e.VC_XSHJJE<-e.VC_XSJE-e.VC_XSTHJE
            e.VC_XSLR<-e.VC_XSHJJE-e.VC_CGHJJE //利润
            e)
|>fun a->
    PSeq.groupJoin a sb.T_CK (fun d->d.VC_CKID) (fun e->e.C_ID) 
      (fun d e->
        d.VC_CK<-match e|>PSeq.headOrDefault with Some x->x.C_MC | _ ->null
        d) 
|>ObjectDumper.Write

watch.Stop()
watch.ElapsedMilliseconds.ToString()
|>ObjectDumper.Write

//------------------------------------------------------------------------------------------------

watch.Reset()
watch.Start()
//性能较差
(
sb.T_KCSP
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a ->a.C_CK)
|>Seq.map (fun (a,b)->
    a,
    (
    b|>PSeq.sumBy (fun c->c.C_KCSL),
    b|>PSeq.sumBy (fun c->c.C_CBJJ*c.C_KCSL) //成本金额
    ))
,
sb.T_DJ_GHS
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a->match a.C_RCK with x when x=DefaultGuidValue ->a.C_CCK | _ ->a.C_RCK )
|>Seq.map (fun (a,b)->
    a,
    (
    b|>PSeq.sumBy (fun c->c.C_RKSL),  //采购进货
    b|>PSeq.sumBy (fun c->c.C_RKJE),  
    b|>PSeq.sumBy (fun c->c.C_CKSL),  //采购退货
    b|>PSeq.sumBy (fun c->c.C_CKJE)
    ))
,
sb.T_DJ_KH
|>PSeq.filter (fun a->true)
|>PSeq.groupBy (fun a->match a.C_RCK with x when x=DefaultGuidValue ->a.C_CCK | _ ->a.C_RCK)
|>Seq.map (fun (a,b)->
    a,
    (
    b|>PSeq.sumBy (fun c->c.C_CKSL),  //销售
    b|>PSeq.sumBy (fun c->c.C_CKJE),
    b|>PSeq.sumBy (fun c->c.C_RKSL),  //销售退货
    b|>PSeq.sumBy (fun c->c.C_RKJE)
    ))
)
|>fun (a,b,c) ->
    match 
      PSeq.groupJoin a sb.T_CK (fun (d,_)->d) (fun e->e.C_ID) 
        (fun (vc_CKID,(vc_KCSL,vc_KCCBJE)) e->
          vc_CKID,((match e|>PSeq.headOrDefault with Some x->x.C_MC | _ ->null),vc_KCSL,vc_KCCBJE)) with
      | x ->
          match
            PSeq.groupJoin x b (fun (d,_)->d) (fun (e,_)->e) 
              (fun (vc_CKID,(vc_CK,vc_KCSL,vc_KCCBJE)) e->
                vc_CKID,(vc_CK,vc_KCSL,vc_KCCBJE),
                match e|>PSeq.headOrDefault with
                | Some (_,(vc_CGSL,vc_CGJE,vc_CGTHSL,vc_CGTHJE)) ->vc_CGSL,vc_CGJE,vc_CGTHSL,vc_CGTHJE
                | _ ->0M,0M,0M,0M) with
          | y ->
              PSeq.groupJoin y c (fun (d,_,_)->d) (fun (e,_)->e)
                (fun (vc_CKID,(vc_CK,vc_KCSL,vc_KCCBJE),(vc_CGSL,vc_CGJE,vc_CGTHSL,vc_CGTHJE)) e->
                  vc_CKID,(vc_CK,vc_KCSL,vc_KCCBJE),(vc_CGSL,vc_CGJE,vc_CGTHSL,vc_CGTHJE),
                  match e|>PSeq.headOrDefault with
                  | Some (_,(vc_XSSL,vc_XSJE,vc_XSTHSL,vc_XSTHJE))->vc_XSSL,vc_XSJE,vc_XSTHSL,vc_XSTHJE
                  | _ ->0M,0M,0M,0M)
|>Seq.map (fun (vc_CKID,(vc_CK,vc_KCSL,vc_KCCBJE),(vc_CGSL,vc_CGJE,vc_CGTHSL,vc_CGTHJE),(vc_XSSL,vc_XSJE,vc_XSTHSL,vc_XSTHJE))->
    match new BD_V_TJ_KCCB_Advance() with
    | x ->
        x.VC_CKID<-vc_CKID
        x.VC_CK<-vc_CK
        x.VC_KCSL<-vc_KCSL
        x.VC_KCCBJE<-vc_KCCBJE
        x.VC_CGSL<-vc_CGSL
        x.VC_CGJE<-vc_CGJE
        x.VC_CGTHSL<-vc_CGTHSL
        x.VC_CGTHJE<-vc_CGTHJE
        x.VC_XSSL<-vc_XSSL
        x.VC_XSJE<-vc_XSJE
        x.VC_XSTHSL<-vc_XSTHSL
        x.VC_XSTHJE<-vc_XSTHJE
        
        x.VC_CGHJSL<-x.VC_CGSL-x.VC_CGTHSL
        x.VC_CGHJJE<-x.VC_CGJE-x.VC_CGTHJE
        x.VC_XSHJSL<-x.VC_XSSL-x.VC_XSTHSL
        x.VC_XSHJJE<-x.VC_XSJE-x.VC_XSTHJE
        x)
|>ObjectDumper.Write
watch.Stop()
watch.ElapsedMilliseconds.ToString()
watch.Elapsed
watch.ElapsedTicks
    
let x="sss",1,3M
let y=Tuple.Create(


//--------------------------------------------------------------------------
let queryEntity=new BQ_DJ_GHS()
let sb=new SBIIMS_JXCEntitiesAdvance()
sb.T_DJSP_GHS.Include(FTN.T_DJ_GHS)
|>PSeq.filter (fun a ->
    a.T_DJ_GHS.C_DJLX=2uy //采购退货
    &&
    match a.T_DJ_GHS.C_CJRQ,queryEntity.C_CJRQ,queryEntity.C_CJRQSecond with
    | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
    | x,y,_ when y.HasValue ->x=y.Value
    | _ ->true
    )
|>PSeq.groupBy (fun a ->a.C_SP)
(*
|>PSeq.map (fun (a,b)->a,b|>PSeq.head,b|>PSeq.sumBy (fun c->c.C_SL),b|>PSeq.sumBy (fun c->c.C_ZHJE))
|>PSeq.map (fun (a,b,c,d)->
    match new BD_T_DJSP_GHS_Advance() with
    | x ->
        x.C_SP<-a
        x
    )
*)
|>ObjectDumper.Write


//----------------------------------------------------------------------------------------------------
let GetQFView (queryEntity:BQ_QFMX_KH_Advance)=
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_QFMX_KH
        |>PSeq.toNetList

new BQ_QFMX_KH_Advance()
|>GetQFView 
|>fun a->
    ObjectDumper.Write a

let GetKHView (queryEntity:BQ_KH_Advance)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_KH.Include(FTN.T_DQ).Include(FTN.T_DWBM).Include(FTN.T_KHLX).Include(FTN.T_YG1).Include(FTN.T_KHDJ).Include(FTN.T_YG)
        |>PSeq.filter (fun a->

            match a.C_XBH,queryEntity.C_XBH,queryEntity.C_XBHSecond with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
           
            )
        //|>fun a->PSeq.join a (sb.T_KHFZR) (fun b->b.C_LXR) (fun c->Nullable<Guid>(c.C_ID)) (fun b c->b,c.C_XM) //InnerJoin
        |>fun a->
            PSeq.groupJoin a (sb.T_KHFZR) (fun b->b.C_LXR) (fun c->Nullable<Guid>(c.C_ID)) 
              (fun b c->b,match c|>PSeq.headOrDefault with Some x->x.C_XM| _ ->null)
        (*
        |>fun a->  //应付金额, 应该使用总账表，T_QFMX_KH表数据量可能会很大，应该使用总账表
            PSeq.groupJoin a (sb.T_QFMX_KH)
              (fun (b,_)->b.C_ID) (fun c->c.C_KH) (fun (b,lxr) c->
              b,lxr,
              match c  with
              | x when PSeq.length x=0 ->0M 
              | x->x|>PSeq.sumBy(fun c->c.C_QFJE))
        *)
        (*
        |>fun a->  //应付金额, 和总账表应该是一一对应的，所以不应该用Left Outer Join
            PSeq.groupJoin a (sb.T_ZZ_KH)
              (fun (b,_)->b.C_ID) (fun c->c.C_KH) (fun (b,lxr) c->
              b,lxr,
              match c  with
              | x when PSeq.length x=0 ->0M 
              | x->x|>PSeq.sumBy(fun c->c.C_QFJE))
        *)
        |>fun a->PSeq.join a (sb.T_ZZ_KH)(fun (b,_)->b.C_ID) (fun c->c.C_KH) (fun (b,lxr) c->b,lxr,c.C_QFJE)
        |>fun a->
            if queryEntity.TotalCount=0 then queryEntity.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (queryEntity.PageSize * queryEntity.PageIndex)
        |>PSeq.take queryEntity.PageSize
        |>PSeq.map (fun (a,lxr,yfje)->
        //|>PSeq.map (fun (a,lxr)->
            new BD_T_KH_Advance
              (C_BZ=a.C_BZ,
              C_MR=a.C_MR,
              C_XBH=a.C_XBH,
              C_CJRQ=a.C_CJRQ,
              C_XFJF=a.C_XFJF,
              C_XYJF=a.C_XYJF,
              C_DY=a.C_DY,
              C_GXRQ=a.C_GXRQ,
              C_ID=a.C_ID,
              C_JY=a.C_JY,
              C_KH=a.C_KH,
              C_LXDH=a.C_LXDH,
              C_LXDZ=a.C_LXDZ,
              C_LXR=a.C_LXR,
              C_MC=a.C_MC,
              C_MCJM=a.C_MCJM)
            |>fun entity ->
                entity.C_FBID<-a.T_DWBM.C_ID
                entity.C_DQ<-a.T_DQ.C_ID
                entity.C_CZY<-a.T_YG.C_ID
                entity.C_DJ<-a.T_KHDJ.C_DM
                entity.C_LX<-a.T_KHLX.C_DM
                entity.C_JBR<-a.T_YG1.C_ID

                entity.VC_DQ<-a.T_DQ.C_MC
                entity.VC_CZY<-a.T_YG.C_XM
                entity.VC_KHDJ<-a.T_KHDJ.C_DJ
                entity.VC_KHLX<-a.T_KHLX.C_LX
                entity.VC_JBR<-a.T_YG1.C_XM
                entity.VC_LXR<-lxr
                entity.VC_YFJE<-yfje
                entity)
        |>PSeq.toNetList 
        |>fun a ->
            if a.Count>0 then a.[0].TotalCount<-queryEntity.TotalCount
            a
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e,"GetEntities"); List<BD_T_KH_Advance>()

new BQ_KH_Advance()
|>GetKHView
|>fun a->
    ObjectDumper.Write a


///////////////////////////////////////////////////////////////////////////////////////////////////
let GetZZ_JYFYs (queryEntity:BQ_ZZ_JYFY)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_ZZ_JYFY.Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_CJRQ,queryEntity.C_CJRQ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_JYJE,queryEntity.C_JYJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.T_DWBM,queryEntity.C_FBID with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            )
        |>map (fun a->
            new BD_T_ZZ_JYFY
              (C_CJRQ=a.C_CJRQ,
              C_GXRQ=a.C_GXRQ,
              C_JYJE=a.C_JYJE)
            |>fun entity ->
                entity.C_FBID<-a.T_DWBM.C_ID
                entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_ZZ_JYFY>()

int32.MaxValue
Int16.MaxValue
Int16.MinValue

//LianXiRen
let GetGHSFZR=
      let queryEntity=new BQ_GHSFZR()
    //member x.GetGHSFZRs (queryEntity:BQ_GHSFZR)=
      use sb=new SBIIMSEntitiesAdvance()
      sb.T_GHSFZR
      |>PSeq.pseq
      |>PSeq.headxOrDefault (fun a ->a.C_GHS=new Guid("00000000-0000-0000-0000-000000000001"))
      //|>PSeq.findOrDefault (fun a ->a.C_GHS=new Guid("00000000-0000-0000-0000-000000000001"))

ObjectDumper.Write GetGHSFZR



let GetGHSFZR=
      let queryEntity=new BQ_GHSFZR()
    //member x.GetGHSFZRs (queryEntity:BQ_GHSFZR)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHSFZR.Include(FTN.T_DWBM).Include(FTN.T_GHS)
        |>pseq
        |>filter (fun a->
            match a.T_GHS,queryEntity.C_GHS with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              BD_T_GHSFZR
                (C_XM=a.C_XM)
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHSFZR>()

//地区
let GetDQ=
      let queryEntity=new BQ_DQ()
    //member x.GetDQs (queryEntity:BQ_DQ)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_DQ.Include(FTN.T_DWBM)
        |>pseq
        |>sortBy (fun a->a.C_XH)
        |>map (fun a->
            let entity=
              BD_T_DQ
                (C_MC=a.C_MC)
            entity.C_FBID<-a.T_DWBM.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_DQ>()

ObjectDumper.Write(GetDQ)

//供货商等级选项
//member x.GetGHSDJs (queryEntity:BQ_GHSDJ)=
let GetGHSDJ=
      let queryEntity=new BQ_GHSDJ()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHSDJ
        |>pseq
        |>filter (fun a->
            match a.C_DJ,queryEntity.C_DJ with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_DM,queryEntity.C_DM with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              BD_T_GHSDJ
                (C_DJ=a.C_DJ,
                C_DM=a.C_DM)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHSDJ>()

ObjectDumper.Write(GetGHSDJ)

//GHS 信息 
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun (b,ghslx) c ->
            b,ghslx,
            match c|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->""
            | x ->x.C_XM) 
        |>fun a->
            join a (sb.T_DQ|>pseq) (fun (b,_,_)->b.C_DQ) (fun c->c.C_ID) (fun (b,ghslx,lxr) c->b,ghslx,lxr, c.C_MC)
        |>fun a->
            groupJoin a (sb.T_QFMX_GHS
            |>pseq)
            ////|>groupByO (fun b->b.C_GHS)  //使用 Seq.groupBy,Right
            ////|>map (fun (b,c)->b,c|>pseq|>sumByDecimal (fun b->b.C_QFJE))) //Right
            //|>groupBy (fun b->b.C_GHS) //使用ParallelEnumerable.Sum,  Right
            //|>map (fun b->b.Key,b|>pseq |>sumByDecimal (fun c->c.C_QFJE))) //Right
            //    (fun (((b,_),_),_)->b.C_ID) (fun (c,d)->c) (fun b c->
                (fun (b,_,_,_)->b.C_ID) (fun c->c.C_GHS) (fun (b,ghslx,lxr,dq) c->
                b,ghslx,lxr,dq,
                match c|>pseq  with
                | x when length x=0 ->0M //可使用 decimal 0 进行测试 
                //| x->x|>headOrDefault|>snd) //与前面的groupBy配合，Right
                | x->x|>sumByDecimal (fun c->c.C_QFJE))
        |>map (fun (a,lx,lxr,dq,yfje)->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                C_LXDH=a.C_LXDH,
                C_LXDZ=a.C_LXDZ,
                C_MR=a.C_MR,
                C_JY=a.C_JY,
                C_XYJF=a.C_XYJF,
                C_GHJF=a.C_GHJF,
                C_DJ=a.C_DJ,
                C_BZ=a.C_BZ,
                VC_LX=lx,
                VC_LXR=lxr,
                VC_DQ=dq,
                VC_YFJE=yfje)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)


//GHS
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun b c ->
            b,
            match c|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->""
            | x ->x.C_XM) 
        |>fun a->
            join a (sb.T_DQ|>pseq) (fun ((b,_),_)->b.C_DQ) (fun c->c.C_ID) (fun b c->b, c.C_MC)
        |>fun a->
            groupJoin a (sb.T_QFMX_GHS
            |>pseq)
            ////|>groupByO (fun b->b.C_GHS)  //使用 Seq.groupBy,Right
            ////|>map (fun (b,c)->b,c|>pseq|>sumByDecimal (fun b->b.C_QFJE))) //Right
            //|>groupBy (fun b->b.C_GHS) //使用ParallelEnumerable.Sum,  Right
            //|>map (fun b->b.Key,b|>pseq |>sumByDecimal (fun c->c.C_QFJE))) //Right
            //    (fun (((b,_),_),_)->b.C_ID) (fun (c,d)->c) (fun b c->
                (fun (((b,_),_),_)->b.C_ID) (fun c->c.C_GHS) (fun b c->
                b,
                match c|>pseq  with
                | x when length x=0 ->0M //可使用 decimal 0 进行测试 
                //| x->x|>headOrDefault|>snd) //与前面的groupBy配合，Right
                | x->x|>sumByDecimal (fun c->c.C_QFJE))
        |>map (fun ((((a,lx),lxr),dq),yfje)->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=lx,
                VC_LXR=lxr,
                VC_DQ=dq,
                VC_YFJE=yfje)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)

//GHS
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun b c ->
            b,
            match c|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->""
            | x ->x.C_XM) 
        |>fun a->
            join a (sb.T_DQ|>pseq) (fun ((b,_),_)->b.C_DQ) (fun c->c.C_ID) (fun b c->b, c.C_MC)
        |>fun a->
            groupJoin a (sb.T_QFMX_GHS
            |>pseq
            //|>groupByO (fun b->b.C_GHS)  //使用 Seq.groupBy,Right
            //|>map (fun (b,c)->b,c|>pseq|>sumByDecimal (fun b->b.C_QFJE))) //Right
            |>groupBy (fun b->b.C_GHS) //使用ParallelEnumerable.Sum,  Right
            |>map (fun b->b.Key,b|>pseq |>sumByDecimal (fun c->c.C_QFJE))) //Right
                (fun (((b,_),_),_)->b.C_ID) (fun (c,d)->c) (fun b c->
                b,
                match c|>pseq  with
                | x when length x=0 ->0M //可使用 decimal 0 进行测试 
                | x->x|>headOrDefault|>snd)
        |>map (fun ((((a,lx),lxr),dq),yfje)->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=lx,
                VC_LXR=lxr,
                VC_DQ=dq,
                VC_YFJE=yfje)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)



//GHS
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun (b,c) d ->
            b,c,
            match d|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->new Guid(),null 
            | x ->x.C_ID,x.C_XM)
        |>map (fun (a,b,(c,d))->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=b,
                VC_LXR=d)
            entity.C_DQ<-a.T_DQ.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)

//第二种方案，不好
let GetGHS01=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR
            |>pseq
            |>filter (fun b->int b.C_FZYW=0) //使用默认业务负责人
            |>distinctBy (fun b->b.C_GHS)) //去除冗余的业务负责人，或者在数据库中约束每个供应商只对应一个默认负责人
                (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun (b,c) d ->
            b,c,
            match d|>pseq|>headOrDefault with
            | null ->new Guid(),null 
            | x ->x.C_ID,x.C_XM)
        |>map (fun (a,b,(c,d))->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=b,
                VC_LXR=d)
            entity.C_DQ<-a.T_DQ.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS01)

////////////////////////////////////////////////////////////////////////////////////////////////////

let queryEntity=BQ_YG()
let result=GetYGs queryEntity
ObjectDumper.Write result

////////////////////////////////////////////////////////////////////////////////////////////////////

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let query=(DA_DJ_GHS.INS:>IDA_DJ_GHS).GetDJ_GHSs queryEntity
ObjectDumper.Write( query,1)
query.[0].C_ID<-Guid("153BC105-1E6C-4113-9AB8-1D4E181809FA")
let businessEntity=query.[0]
businessEntity.C_SHR<-Nullable<Guid>()
businessEntity.C_BZ<-"Main2009-11-08Updated"
businessEntity.BD_T_DJSP_GHSs.[0].C_BZ<-"2009-11-08Updated"

businessEntity.C_SHR<-Nullable<Guid>(Guid("154f5e0f-63d4-4d25-9093-50a7ae929c5c"))

(DA_DJ_GHS.INS:>IDA_DJ_GHS).CreateDJ_GHS  businessEntity

(DA_DJ_GHS.INS:>IDA_DJ_GHS).UpdateDJ_GHS  businessEntity

(DA_DJ_GHS.INS:>IDA_DJ_GHS).DeleteDJ_GHS  businessEntity

(DA_DJ_GHS.INS:>IDA_DJ_GHS).DeleteDJ_GHSs  [|businessEntity|]

///////////////////////////////////////////////////////////////////////////////////////

let DeleteDJ_GHSs (businessEntities:BD_T_DJ_GHS[])=
      use sb=new SBIIMSEntitiesAdvance()
      try
        for businessEntity in businessEntities do
          match
            ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
            |>sb.CreateEntityKey
            |>fun a ->
                let b=ref Unchecked.defaultof<_>
                sb.TryGetObjectByKey(a,b), !b with
          | false,_ -> failwith "One of records is not exist!"
          | _,x ->
              x
              |>unbox<T_DJ_GHS>
              |>fun x -> 
                  //x.T_DJSP_GHS.Load()， 默认情况下已经加载，再次加载则是起到刷新的作用
                  x.T_DJSP_GHS.ToArray() //此种情况下x.T_DJSP_GHS的项也会随sb的Delete操作而删除, 此时迭代操作的数据源是一个新的数组实例，因而不受迭代删除操作的影响
                  |>Seq.iter (fun a->sb.DeleteObject(a))
                  ObjectDumper.Write(x.T_DJSP_GHS.Count) //结果为0
                  (*Wrong!!!, sb每执行一次Delete操作， x.T_DJSP_GHS中的相应的记录将会被删除
                  x.T_DJSP_GHS
                  |>Seq.iter (fun a->sb.DeleteObject(a) )
                  *)
                  (*
                  //Right
                  let count=x.T_DJSP_GHS.Count
                  for a=0 to count-1 do
                    sb.DeleteObject(x.T_DJSP_GHS.First())
                  *)
                  x
              |>sb.DeleteObject
        sb.SaveChanges()
      with
      | e -> ObjectDumper.Write(e,3); -1

let DeleteDJ_GHS (businessEntity:BD_T_DJ_GHS)=
      use sb=new SBIIMSEntitiesAdvance()
      try
        match
          ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
          |>sb.CreateEntityKey
          |>fun a ->
              let b=ref Unchecked.defaultof<_>
              sb.TryGetObjectByKey(a,b), !b with
        | false,_ -> failwith "The record is not exist!"
        | _,x ->
            x
            |>unbox<T_DJ_GHS>
            |>fun x -> 
                //x.T_DJSP_GHS.Load()， 默认情况下已经加载，再次加载则是起到刷新的作用
                x.T_DJSP_GHS.ToArray() //此种情况下x.T_DJSP_GHS的项也会随sb的Delete操作而删除, 此时迭代操作的数据源是一个新的数组实例，因而不受迭代删除操作的影响
                |>Seq.iter (fun a->sb.DeleteObject(a))
                ObjectDumper.Write(x.T_DJSP_GHS.Count) //结果为0
                (*Wrong!!!, sb每执行一次Delete操作， x.T_DJSP_GHS中的相应的记录将会被删除
                x.T_DJSP_GHS
                |>Seq.iter (fun a->sb.DeleteObject(a) )
                *)
                (*
                //Right
                let count=x.T_DJSP_GHS.Count
                for a=0 to count-1 do
                  sb.DeleteObject(x.T_DJSP_GHS.First())
                *)
                x
            |>sb.DeleteObject
            sb.SaveChanges()
      with
      | e -> ObjectDumper.Write(e,3); -1

DeleteDJ_GHS businessEntity


(DA_DJ_GHS.INS:>IDA_DJ_GHS).DeleteDJ_GHS  businessEntity

let GetDJ_GHSs (queryEntity:BQ_DJ_GHS)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_DJ_GHS.Include(FTN.T_CK).Include(FTN.T_CK1).Include(FTN.T_DWBM).Include(FTN.T_DWBM1).Include(FTN.T_GHS).Include(FTN.T_YG).Include(FTN.T_YG1).Include(FTN.T_YG2)
        |>pseq
        |>filter (fun a->
            match a.C_BZ,queryEntity.C_BZ with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_CJRQ,queryEntity.C_CJRQ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_CKJE,queryEntity.C_CKJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_CKSL,queryEntity.C_CKSL with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DJH,queryEntity.C_DJH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_DJLX,queryEntity.C_DJLX with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DJZQJE,queryEntity.C_DJZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DJZT,queryEntity.C_DJZT with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DYBZ,queryEntity.C_DYBZ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_FKD,queryEntity.C_FKD with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_ID,queryEntity.C_ID with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_RKJE,queryEntity.C_RKJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_RKSL,queryEntity.C_RKSL with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_SZQJE,queryEntity.C_SZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_THBZ,queryEntity.C_THBZ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_YHJE,queryEntity.C_YHJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_YSDJH,queryEntity.C_YSDJH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_YZQJE,queryEntity.C_YZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.T_CK,queryEntity.C_CCK with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_CK1,queryEntity.C_RCK with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_DWBM,queryEntity.C_FBID with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_DWBM1,queryEntity.C_WFDW with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_GHS,queryEntity.C_GHS with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_YG,queryEntity.C_CZY with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_YG1,queryEntity.C_JBR with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              BD_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID)
            | _ ->()
            entity.BD_T_DJSP_GHSs.AddRange(
              a.T_DJSP_GHS.CreateSourceQuery().Include(FTN.T_DWBM).Include(FTN.T_SP)
              |>pseq 
              |>map (fun b->
                  let childEntity=
                    BD_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  childEntity.C_FBID<-b.T_DWBM.C_ID
                  childEntity.C_SP<-b.T_SP.C_ID
                  childEntity)
              |>toNetList)
            entity)
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_DJ_GHS>()

let businessEntities= GetDJ_GHSs queryEntity
ObjectDumper.Write(businessEntities ,1)

////////////////////////////////////////////////////////////////////////////


let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let GetDJ_GHSs2 (queryEntity:BQ_DJ_GHS)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_DJ_GHS.Include("T_CK").Include("T_CK1").Include("T_DWBM").Include("T_GHS").Include("T_YG").Include("T_YG1").Include("T_YG2")
        |>pseq
        |>filter (fun a->
            match queryEntity.C_JBR with
            | x when x.HasValue ->
                a.T_YG1Reference.Load() 
                a.T_YG1.C_ID=x.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            match a.T_YG2 with
            | x when x<>null ->entity.C_SHR <- Nullable<System.Guid>(x.C_ID) | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS.CreateSourceQuery().Include("T_SP").Include("T_DWBM")
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  //b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                 // b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  detailEntity)
              |>toNetList
            entity)
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e.ToString(),"General"); List<B_T_DJ_GHS>()


let businessEntities= GetDJ_GHSs2 queryEntity
ObjectDumper.Write(businessEntities ,1)

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))


let GetDJ_GHSs3 (queryEntity:BQ_DJ_GHS)=
      use sb=new SBIIMSEntitiesAdvance()
      try
        sb.T_DJ_GHS
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG1Reference.Load() 
                a.T_YG1.C_ID=queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                a.T_YG2Reference.Load() 
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                        x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
            a.T_DWBM1Reference.Load()
            a.T_DWBMReference.Load()
            a.T_GHSReference.Load()
            a.T_YG1Reference.Load()
            a.T_YGReference.Load()
            a.T_DJSP_GHS.Load()
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE,
                C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID))
            a.T_CK1Reference.Load()
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            //Modified
            a.T_YG2Reference.Load()
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<Guid>(x.C_ID)
            | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_SP=b.C_SP,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                  detailEntity)
              |>toNetList
            entity
            )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,3); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS>()

let businessEntities= GetDJ_GHSs3 queryEntity
ObjectDumper.Write(businessEntities ,1)

////////////////////////////////////////////////////////////////////////////////////////////////////
//Right Reference
let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

//加载方法sb.ContextOptions.LazyLoadingEnabled<-true, a.T_CKReference.Load(),.Include(...)都可以，且只有Include(...)支持Parallel
//由于需要使用Parallel方式，所以只能采用InClude方式，且将sb.ContextOptions.LazyLoadingEnabled<-false， 其默认为True
let GetDJ_GHSs1 (queryEntity:BQ_DJ_GHS)=
        use sb=new SBIIMSEntitiesAdvance()
        //sb.ContextOptions.LazyLoadingEnabled<-true //默认已经为True， 特别的LazyLoading不支持Parallel？？？
        sb.ContextOptions.LazyLoadingEnabled<-false
        sb.T_DJ_GHS.Include("T_CK").Include("T_CK1").Include("T_DWBM").Include("T_GHS").Include("T_YG").Include("T_YG1").Include("T_YG2") //Right, 且Include方式支持Parallel方式
        |>pseq  //LazyLoading不支持Parallel？？？
        //|>Seq.filter(fun a->   //各种加载方式都支持
        |> filter (fun a-> //LazyLoading不支持Parallel？？？
//            a.T_CKReference.Load()   //Right, 但Load()方法不支持Parallel
//            a.T_CK1Reference.Load()
//            a.T_DWBMReference.Load()
//            a.T_DWBM1Reference.Load()
//            a.T_GHSReference.Load()
//            a.T_YGReference.Load()
//            a.T_YG1Reference.Load()
//            a.T_YG2Reference.Load()
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG.C_ID =queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_JBR.HasValue  then
                 //true
                 a.T_YG1.C_ID =queryEntity.C_JBR .Value
                 //a.C_JBR =queryEntity.C_JBR .Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                    x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        //|>Seq.map (fun a->    //各种加载方式都支持
        |>map (fun a-> //LazyLoading不支持Parallel？？？
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE)
//            entity.C_CCK<-a.C_CCK  //Right
//            entity.C_RCK<-a.C_RCK
//            entity.C_FBID<-a.C_FBID
//            entity.C_WFDW<-a.C_WFDW
//            entity.C_GHS<-a.C_GHS
//            entity.C_CZY<-a.C_CZY
//            entity.C_JBR<-a.C_JBR
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            entity
            )
        //|>Seq.toList  
        |>toNetList  
        
let businessEntities= GetDJ_GHSs1 queryEntity
ObjectDumper.Write(businessEntities ,1)

//////////////////////////////////////////////////////////////////////////////////////////////////////
//Right,  直接使用外键字段，只有新版ADO.NET 才支持

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let GetDJ_GHSs1 (queryEntity:BQ_DJ_GHS)=
        use sb=new SBIIMSEntitiesAdvance()
        //sb.ContextOptions.LazyLoadingEnabled<-true
        sb.T_DJ_GHS//.Include("T_CK").Include("T_DWBM").Include("T_GHS").Include("T_YG")
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG.C_ID =queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_JBR.HasValue  then
                 //a.T_YG1.C_ID =queryEntity.C_JBR .Value
                 a.C_JBR =queryEntity.C_JBR .Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                    x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.C_CCK
            entity.C_RCK<-a.C_RCK
            entity.C_FBID<-a.C_FBID
            entity.C_WFDW<-a.C_WFDW
            entity.C_GHS<-a.C_GHS
            entity.C_CZY<-a.C_CZY
            entity.C_JBR<-a.C_JBR
            entity
            )
        |>toNetList  

let businessEntities= GetDJ_GHSs1 queryEntity
ObjectDumper.Write(businessEntities ,1)

//////////////////////////////////////////////////////////////////////////////////////////////////////

let GetDJ_GHSs (queryEntity:BQ_DJ_GHS)=
      use sb=new SBIIMSEntitiesAdvance()
      sb.ContextOptions.LazyLoadingEnabled<-true
      try
        sb.T_DJ_GHS
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                //a.T_YG1Reference.Load() 
                a.T_YG1.C_ID =queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                //a.T_YG2Reference.Load() 
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                        x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
//            if not a.T_DWBM1Reference.IsLoaded then
//              a.T_DWBM1Reference.Load()
//            if not a.T_DWBM1Reference.IsLoaded then
//              a.T_DWBMReference.Load()
//            if not a.T_GHSReference.IsLoaded then
//              a.T_GHSReference.Load()
//            if not a.T_YG1Reference.IsLoaded then
//              a.T_YG1Reference.Load()
//            if not a.T_YGReference.IsLoaded then
//              a.T_YGReference.Load()
//            if not a.T_DJSP_GHS.IsLoaded then
//              a.T_DJSP_GHS.Load()
//            if not a.T_CK1Reference.IsLoaded then
//              a.T_CK1Reference.Load()
//            if not a.T_CKReference.IsLoaded then
//              a.T_CKReference.Load()
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            //Modified
            a.T_YG2Reference.Load()
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<Guid>(x.C_ID)
            | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_SP=b.C_SP,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  //b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  //b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                  detailEntity)
              |>toNetList
            entity
            )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,3); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS>()

let businessEntities= GetDJ_GHSs queryEntity
ObjectDumper.Write(businessEntities ,1)


let sb=new SBIIMSEntitiesAdvance()
let result=
  sb.T_DJ_GHS
  |>pseq
  |>filter (fun a->a.T_YGReference.Load(); a.T_YG.C_ID=queryEntity.C_CZY.Value)
  |>map (fun a->a.T_YG.C_XM )

sb.Dispose()


let query =
      let queryEntity=BQ_DJ_GHS()
      queryEntity.C_CZY<-Nullable<Guid>(Guid("797f2b9f-3f41-4b31-90a9-bcd2a8c281a3"))
      use sb=new SBIIMSEntitiesAdvance()
      try
        sb.T_DJ_GHS
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG1Reference.Load() 
                a.T_YG1.C_ID=queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                a.T_YG2Reference.Load() 
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                        x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
            a.T_DWBM1Reference.Load()
            a.T_DWBMReference.Load()
            a.T_GHSReference.Load()
            a.T_YG1Reference.Load()
            a.T_YGReference.Load()
            a.T_DJSP_GHS.Load()
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_YZQJE=a.C_YZQJE,
                C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID))
            a.T_CK1Reference.Load()
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            //Modified
            a.T_YG2Reference.Load()
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<Guid>(x.C_ID)
            | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_SP=b.C_SP,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                  detailEntity)
              |>toNetList
            entity
            )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,3); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS>()

///////////////////////////////////////////////////////////////////////////////////////////////////

(DA_DJ_GHS.INS:>IDA_DJ_GHS)
let da=
  DA_DJ_GHS.INS |>unbox<IDA_DJ_GHS>
  
let queryEntity=BQ_DJ_GHS()
queryEntity.C_ID<- Nullable<Guid>(Guid("83706028-2459-4c6f-9e13-2b9ab85e7470"))
let businessEntities=da.GetDJ_GHSs queryEntity
ObjectDumper.Write(businessEntities,1)
ObjectDumper.Write(businessEntities.[0].B_T_DJSP_GHSs)

let creatEntity=businessEntities.[0]
creatEntity.C_ID<- Guid("7F09563C-F027-45b3-BCFC-2C454BC1F567")
for a in creatEntity.B_T_DJSP_GHSs do
  a.C_DJID <- Guid("7F09563C-F027-45b3-BCFC-2C454BC1F567")

ObjectDumper.Write(creatEntity,1)

try
  da.CreateDJ_GHS creatEntity
with 
| e -> ObjectDumper.Write(e);-1



  

///////////////////////////////////////////////////////////////////////////////////////////////////
//For Read
let GetDJ_DJSP_GHSs(queryEntity:BQ_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    sb.T_DJ_GHS
    |>pseq
    |>filter (fun a->
        match a.C_ID,queryEntity.C_ID with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
        &&
        match a.C_GXRQ,queryEntity.C_GXRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
          )
    |>map (fun a->
           a.T_DWBM1Reference.Load()
           a.T_DWBMReference.Load()
           a.T_GHSReference.Load()
           a.T_YG1Reference.Load()
           a.T_YG2Reference.Load()
           a.T_YGReference.Load()
           a.T_DJSP_GHS.Load()
           let entity=
             B_T_DJ_GHS_Advance
               (C_BZ=a.C_BZ,
               C_CJRQ=a.C_CJRQ,
               C_CKJE=a.C_CKJE,
               C_CKSL=a.C_CKSL,
               C_DJH=a.C_DJH,
               C_DJLX=a.C_DJLX,
               C_DJZQJE=a.C_DJZQJE,
               C_DJZT=a.C_DJZT,
               C_DYBZ=a.C_DYBZ,
               C_FKD=a.C_FKD,
               C_GXRQ=a.C_GXRQ,
               C_ID=a.C_ID,
               C_RKJE=a.C_RKJE,
               C_RKSL=a.C_RKSL,
               C_SZQJE=a.C_SZQJE,
               C_THBZ=a.C_THBZ,
               C_YHJE=a.C_YHJE,
               C_YSDJH=a.C_YSDJH,
               C_YZQJE=a.C_YZQJE,
               C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID))
           a.T_CK1Reference.Load()
           entity.C_RCK<-a.T_CK1.C_ID
           entity.C_FBID<-a.T_DWBM.C_ID
           entity.C_WFDW<-a.T_DWBM1.C_ID
           entity.C_GHS<-a.T_GHS.C_ID
           entity.C_CZY<-a.T_YG.C_ID
           entity.C_JBR<-a.T_YG1.C_ID
           entity.C_SHR <-Nullable<Guid>( a.T_YG2.C_ID)
           entity.B_T_DJSP_GHSs<-
             a.T_DJSP_GHS
             |>pseq 
             |>map (fun b->
                    let detailEntity=
                      B_T_DJSP_GHS_Advance
                        (BZ=b.BZ,
                        C_BZQ=b.C_BZQ,
                        C_DJ=b.C_DJ,
                        C_DJID=b.C_DJID,
                        C_PC=b.C_PC,
                        C_SCRQ=b.C_SCRQ,
                        C_SL=b.C_SL,
                        C_SP=b.C_SP,
                        C_TM=b.C_TM,
                        C_XH=b.C_XH,
                        C_ZHJ=b.C_ZHJ,
                        C_ZHJE=b.C_ZHJE,
                        C_ZJE=b.C_ZJE,
                        C_ZKL=b.C_ZKL)
                    b.T_SPReference.Load()
                    detailEntity.C_SP<-b.T_SP.C_ID
                    b.T_DWBMReference.Load()
                    detailEntity.C_FBID<-b.T_DWBM.C_ID
                    detailEntity)
             |>toNetList
           entity
           )
    |>toNetList  
  with 
  | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS_Advance>()


//Excute query
let queryEntity=BQ_DJ_GHS_Advance(C_ID=Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a")))
let mainDetailEntity=GetDJ_DJSP_GHSs queryEntity
ObjectDumper.Write (mainDetailEntity,2)
mainDetailEntity.Count
mainDetailEntity.[0].B_T_DJSP_GHSs
ObjectDumper.Write (mainDetailEntity.[0].B_T_DJSP_GHSs,0)


//For Create
let CreateT_CK (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try 
    let t_DJ_GHS=
      T_DJ_GHS(
        C_ID=Guid.NewGuid(),
        C_BZ="wxtest",//businessEntity.C_BZ,
        C_CJRQ=DateTime.Now,
        C_CKJE=businessEntity.C_CKJE,
        C_CKSL=businessEntity.C_CKSL,
        C_DJH=businessEntity.C_DJH,
        C_DJLX=businessEntity.C_DJLX,
        C_DJZQJE=businessEntity.C_DJZQJE,
        C_DJZT=businessEntity.C_DJZT,
        C_DYBZ=businessEntity.C_DYBZ,
        C_FKD=businessEntity.C_FKD,
        C_GXRQ=DateTime.Now,
        C_RKJE=businessEntity.C_RKJE,
        C_RKSL=businessEntity.C_RKSL,
        C_SZQJE=businessEntity.C_SZQJE,
        C_THBZ=businessEntity.C_THBZ,
        C_YHJE=businessEntity.C_YHJE,
        C_YSDJH=businessEntity.C_YSDJH,
        C_YZQJE=businessEntity.C_YZQJE)
    t_DJ_GHS.T_CK<- 
      let entityKey=sb.CreateEntityKey("T_CK",T_CK(C_ID=businessEntity.C_CCK))
      sb.GetObjectByKey(entityKey):?>T_CK

      (* Right, not good
      sb.T_CK
      |>pseq
      |>filter (fun a->a.C_ID=businessEntity.C_CCK)
      |>head
      *)  
        
      (*// Right, not good
      ((("T_CK",T_CK(C_ID=businessEntity.C_CCK))
      |>sb.CreateEntityKey 
      |>sb.GetObjectByKey)
      :?>T_CK)
      *)
    t_DJ_GHS.T_CK1<-
      ((sb.CreateEntityKey("T_CK",T_CK(C_ID=businessEntity.C_RCK))
      |>sb.GetObjectByKey):?>T_CK)
    t_DJ_GHS.T_DWBM<-
       ("T_DWBM",T_DWBM(C_ID=businessEntity.C_FBID))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>fun x-> x:?>T_DWBM
    t_DJ_GHS.T_DWBM1<-
       ("T_DWBM",T_DWBM(C_ID=businessEntity.C_WFDW))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>fun x->x:?>T_DWBM
    t_DJ_GHS.T_GHS<-
      (* Right
       //let  original:obj ref=ref null //Right
       let  original=ref Unchecked.defaultof<obj>
       let entityKey=sb.CreateEntityKey ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
       if sb.TryGetObjectByKey(entityKey,original)  then
          !original :?>T_GHS
       else null
       *)
       //let  original:obj ref=ref null //Right
       let original=ref Unchecked.defaultof<obj>
       ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
       |>sb.CreateEntityKey
       |>fun x->if sb.TryGetObjectByKey(x,original) then !original else null
       |>unbox<T_GHS>
       (* Right
       ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>unbox<T_GHS>
       *)
    t_DJ_GHS.T_YG <-
       ("T_YG",T_YG(C_ID=businessEntity.C_CZY))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>unbox<T_YG>
    t_DJ_GHS.T_YG1 <-
       ("T_YG",T_YG(C_ID=businessEntity.C_JBR))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>unbox<T_YG>
    match businessEntity.C_SHR with
    | a when a.HasValue->
        t_DJ_GHS.T_YG1 <-
          ("T_YG",T_YG(C_ID=a.Value))
          |>sb.CreateEntityKey
          |>sb.GetObjectByKey
          |>unbox<T_YG>
    | _ ->()
    
    for detailEntity in businessEntity.B_T_DJSP_GHSs do
      let t_DJSP_GHS=
        T_DJSP_GHS
          (BZ=detailEntity.BZ,
          C_BZQ=detailEntity.C_BZQ,
          C_DJ=detailEntity.C_DJ,
          C_DJID=detailEntity.C_DJID,
          C_PC=detailEntity.C_PC,
          C_SCRQ=detailEntity.C_SCRQ,
          C_SL=detailEntity.C_SL,
          C_SP=detailEntity.C_SP,
          C_TM=detailEntity.C_TM,
          C_XH=detailEntity.C_XH,
          C_ZHJ=detailEntity.C_ZHJ,
          C_ZHJE=detailEntity.C_ZHJE,
          C_ZJE=detailEntity.C_ZJE,
          C_ZKL=detailEntity.C_ZKL)
      t_DJSP_GHS.T_DWBM<-
        ("T_DWBM",T_DWBM(C_ID=detailEntity.C_FBID))
        |>sb.CreateEntityKey
        |>sb.GetObjectByKey
        |>unbox<T_DWBM>
      t_DJ_GHS.T_DJSP_GHS.Add(t_DJSP_GHS)
    sb.AddToT_DJ_GHS(t_DJ_GHS)
    sb.SaveChanges()
  with
  | e ->ObjectDumper.Write(e,0);-1
  

//mainDetailEntity.[0].C_GHS<-Guid.NewGuid()    
let create=CreateT_CK mainDetailEntity.[0]


//For Update
let UpdateT_DJ_DJSP01 (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    let original=
      ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_DJ_GHS>
    original.C_BZ<-"================"
    original.T_YG<-
      ("T_YG",T_YG(C_ID=businessEntity.C_CZY))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_YG>
    sb.ApplyPropertyChanges("T_DJ_GHS",original) 
    sb.SaveChanges()
  with
  | e ->ObjectDumper.Write(e);-1
  
  
let UpdateT_DJ_DJSP (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    let original=
      ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_DJ_GHS>
    original.C_BZ<-"wx11111111111111111111111"
    original.C_CJRQ<-businessEntity.C_CJRQ
    original.C_CKJE<-businessEntity.C_CKJE
    original.C_CKSL<-businessEntity.C_CKSL
    original.C_DJH<-businessEntity.C_DJH
    original.C_DJLX<-businessEntity.C_DJLX
    original.C_DJZQJE<-businessEntity.C_DJZQJE
    original.C_DJZT<-businessEntity.C_DJZT
    original.C_DYBZ<-businessEntity.C_DYBZ
    original.C_FKD<-businessEntity.C_FKD
    original.C_GXRQ<-DateTime.Now
    original.C_RKJE<-businessEntity.C_RKJE
    original.C_RKSL<-businessEntity.C_RKSL
    original.C_SZQJE<-businessEntity.C_SZQJE
    original.C_THBZ<-businessEntity.C_THBZ
    original.C_YHJE<-businessEntity.C_YHJE
    original.C_YSDJH<-businessEntity.C_YSDJH
    original.C_YZQJE<-businessEntity.C_YZQJE

//    let t_DJ_GHS=
//      T_DJ_GHS(
//        C_ID=businessEntity.C_ID,
//        C_BZ="wxtest=================="//businessEntity.C_BZ,
//        C_CJRQ=DateTime.Now,
//        C_CKJE=businessEntity.C_CKJE,
//        C_CKSL=businessEntity.C_CKSL,
//        C_DJH=businessEntity.C_DJH,
//        C_DJLX=businessEntity.C_DJLX,
//        C_DJZQJE=businessEntity.C_DJZQJE,
//        C_DJZT=businessEntity.C_DJZT,
//        C_DYBZ=businessEntity.C_DYBZ,
//        C_FKD=businessEntity.C_FKD,
//        C_GXRQ=DateTime.Now,
//        C_RKJE=businessEntity.C_RKJE,
//        C_RKSL=businessEntity.C_RKSL,
//        C_SZQJE=businessEntity.C_SZQJE,
//        C_THBZ=businessEntity.C_THBZ,
//        C_YHJE=businessEntity.C_YHJE,
//        C_YSDJH=businessEntity.C_YSDJH,
//        C_YZQJE=businessEntity.C_YZQJE)
    original.T_CK<- 
      ("T_CK",T_CK(C_ID=businessEntity.C_CCK))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_CK>
    original.T_CK1<-
      ("T_CK",T_CK(C_ID=businessEntity.C_RCK))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_CK>
    original.T_DWBM<-
      ("T_DWBM",T_DWBM(C_ID=businessEntity.C_FBID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>fun x-> x:?>T_DWBM
    original.T_DWBM1<-
      ("T_DWBM",T_DWBM(C_ID=businessEntity.C_WFDW))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>fun x->x:?>T_DWBM
    original.T_GHS<-
      let original=ref Unchecked.defaultof<obj>
      ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
      |>sb.CreateEntityKey
      |>fun x->if sb.TryGetObjectByKey(x,original) then !original else null
      |>unbox<T_GHS>
    original.T_YG <-
      ("T_YG",T_YG(C_ID=businessEntity.C_CZY))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_YG>
    original.T_YG1 <-
      ("T_YG",T_YG(C_ID=businessEntity.C_JBR))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_YG>
    sb.ApplyPropertyChanges("T_DJ_GHS",original) 
    
    //对于子表记录来说,如果商业实体中有子表记录,先删现有子表中所有记录,再插入所有商业实体中的子表记录
    //还有一中方法是, 比较现有子表和商业实体中的子表记录,需要新增的才新增,需要删除的才删除,需要更新的才更新,
    //哪一种方法更好???
    if businessEntity.B_T_DJSP_GHSs.Count>0 then
      //Delete all detail records
      original.T_DJSP_GHS.Load()
      for originalDetail in original.T_DJSP_GHS do
        ("T_DJSP_GHS",T_DJSP_GHS(C_DJID=originalDetail.C_DJID,C_SP=originalDetail.C_SP,C_PC=originalDetail.C_PC))
        |>sb.CreateEntityKey
        |>sb.GetObjectByKey
        |>sb.DeleteObject
      //Insert detail records  
      for businessDetail in  businessEntity.B_T_DJSP_GHSs do
        let t_DJSP_GHS=
          T_DJSP_GHS
            (BZ=businessDetail.BZ,
            C_BZQ=businessDetail.C_BZQ,
            C_DJ=businessDetail.C_DJ,
            C_DJID=businessDetail.C_DJID,
            C_PC=businessDetail.C_PC,
            C_SCRQ=businessDetail.C_SCRQ,
            C_SL=businessDetail.C_SL,
            C_SP=businessDetail.C_SP,
            C_TM=businessDetail.C_TM,
            C_XH=businessDetail.C_XH,
            C_ZHJ=businessDetail.C_ZHJ,
            C_ZHJE=businessDetail.C_ZHJE,
            C_ZJE=businessDetail.C_ZJE,
            C_ZKL=businessDetail.C_ZKL)
        t_DJSP_GHS.T_DWBM<-
          ("T_DWBM",T_DWBM(C_ID=businessDetail.C_FBID))
          |>sb.CreateEntityKey
          |>sb.GetObjectByKey
          |>unbox<T_DWBM>
        original.T_DJSP_GHS.Add(t_DJSP_GHS)
    sb.SaveChanges()
  with
  | e ->ObjectDumper.Write(e);-1
  
  
let businessEntity=B_T_DJ_GHS_Advance(C_ID=Guid("83706028-2459-4c6f-9e13-2b9ab85e7470"))
businessEntity.C_CZY<-Guid("154f5e0f-63d4-4d25-9093-50a7ae929c5c")
UpdateT_DJ_DJSP01 businessEntity

UpdateT_DJ_DJSP businessEntity
  
let original=
  use sb=new SBIIMSEntitiesAdvance()
  ("T_DJ_GHS",T_DJ_GHS(C_ID=Guid("83706028-2459-4c6f-9e13-2b9ab85e7470")))
  |>sb.CreateEntityKey
  |>sb.GetObjectByKey
  |>unbox<T_DJ_GHS>


//----------------------------------

//For Delete
let DeleteT_DJ_DJSP (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
    |>sb.CreateEntityKey
    |>sb.GetObjectByKey
    |>unbox<T_DJ_GHS>
    |>fun x -> x.T_DJSP_GHS.Load();x
    |>sb.DeleteObject
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e); -1
let businessEntityForDelete=B_T_DJ_GHS_Advance(C_ID=Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0"))
businessEntity.C_CZY<-Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0")
DeleteT_DJ_DJSP businessEntityForDelete


//For Delete  multiple records
let DeleteT_DJ_DJSPs (businessEntitys:B_T_DJ_GHS_Advance [])=
  use sb=new SBIIMSEntitiesAdvance()
  try
    for businessEntity in businessEntitys do
      ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_DJ_GHS>
      |>fun x -> x.T_DJSP_GHS.Load();x
      |>sb.DeleteObject
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e);-1
  
  
  
  
let businessEntityForDeleteMulti=B_T_DJ_GHS_Advance(C_ID=Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0"))
businessEntity.C_CZY<-Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0")
DeleteT_DJ_DJSP businessEntityForDeleteMulti

/////////////////////

let  original=Unchecked.defaultof<T_GHS>
let  original1=null
let  original2:Object ref=ref null
let  original3:T_GHS ref=ref null
//let entityKey=sb.CreateEntityKey ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
//if sb.TryGetObjectByKey(entityKey,original)  then
//   !original :?>T_GHS
//else null


let entity =box (T_GHS())
unbox<T_GHS> null
/////////////////////////////////////////////////

(*
//For Update ???
let UpdateT_CK01 (entity:T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    let entitySetName = entity.EntityKey.EntitySetName
    sb.T_CK.Context.ApplyPropertyChanges(entitySetName,entity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1


let UpdateT_CK02 (sourceEntity:B_T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  //let forUpdateEntity=
  let entityKey = sb.CreateEntityKey("T_CK",T_CK(C_ID=sourceEntity.C_ID))
  let orginalEntity = sb.GetObjectByKey(entityKey) :?> T_CK
  orginalEntity.C_CJRQ<-sourceEntity.C_CJRQ
  //...需要更新每一个成员
  try
    let entitySetName = orginalEntity.EntityKey.EntitySetName
    sb.T_CK.Context.ApplyPropertyChanges(entitySetName,orginalEntity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1

//For Delete
let DeleteT_CK01 (entity:T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    //sb.DeleteObject(entity)
    sb.T_CK.Context.DeleteObject(entity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1
  
let DeleteT_CK02 (queryEntity:BQ_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  let deleteEntity=
    sb.T_CK
    |>pseq
    |>filter (fun a->
        match a.C_CJRQ,queryEntity.C_CJRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
        &&
        match a.C_GXRQ,queryEntity.C_GXRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
          )
     |>head
  try
    sb.DeleteObject(deleteEntity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1

//For Query
let queryT_CK01 (queryEntity:BQ_CK)=
  //let x=(DA_T_CK.INS:>IDA_T_CK).GetT_CKs()
    use sb=new SBIIMSEntitiesAdvance()
    sb.T_CK
    |>pseq
    |>filter (fun a->
         match queryEntity with
         | x when x=Unchecked.defaultof<_> ->true
         | x  ->
            match a.C_CJRQ,x.C_CJRQ with
            | ax,y when y.HasValue ->ax=y.Value
            | _ ->true
            &&
            match a.C_GXRQ,x.C_GXRQ with
            | ax,y when y.HasValue ->ax=y.Value
            | _ ->true
          )
    |>toList  
    
let queryT_CK02(queryEntity:BQ_CK)=
    use sb=new SBIIMSEntitiesAdvance()
    sb.T_CK
    |>pseq
    |>filter (fun a->
        match a.C_CJRQ,queryEntity.C_CJRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
        &&
        match a.C_GXRQ,queryEntity.C_GXRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
          )
    |>toList  

let queryT_CK03 (queryEntity:BQ_CK)=
    use sb=new SBIIMSEntitiesAdvance()
    sb.T_CK
    |>pseq
    |>filter (fun a->
         (if queryEntity.C_CJRQ.HasValue then a.C_CJRQ=queryEntity.C_CJRQ.Value else true) &&
         (if queryEntity.C_CJRQ.HasValue then a.C_GXRQ=queryEntity.C_GXRQ.Value else true )
         )
    |>toList



let x=(DA_T_CK.INS:>IDA_T_CK).GetT_CKs()
ObjectDumper.Write(x,0)
//WpfObjectDumper

//let y=System.Linq.Queryable<T_CK>(
//let y=System.Data.Objects.ObjectQuery

let path=System.AppDomain.CurrentDomain.BaseDirectory

let x=
  use sb=new SBIIMSEntitiesAdvance()
  sb.T_CK
  //|>Seq.toList
  |>pseq
  |>toList

let GetT_DWBM id=
  use sb=new SBIIMSEntitiesAdvance()
  sb.T_DWBM
  |>pseq
  |>filter (fun a->a.C_ID=id)
  |>toList
  
///Add, Rgiht  
let AddT_CK (entity:T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  //let t_DWBM=GetT_DWBM (Guid("69896fad-c145-4b3a-b122-36df6b6d2c87"))
//  let entity=
//          T_CK(C_BZ = null,
//               C_CJRQ = DateTime.Now,
//               C_CKDZ = "九门里",
//               C_DQ = "001001",
//               C_GXRQ = DateTime.Now,
//               C_ID = Guid.NewGuid(),
//               C_JY = false,
//               C_LXDH = "九门里",
//               C_MC = "九门里1号仓库",
//               C_MR = false )
  let t_DWBM=
    sb.T_DWBM
    |>pseq
    |>filter (fun a->a.C_ID=Guid "69896fad-c145-4b3a-b122-36df6b6d2c87")
    |>head
  let t_YG=
    sb.T_YG
    |>pseq
    |>filter (fun a->a.C_ID=Guid "b3548cf3-e9a1-4dd8-bb07-086de2dcd5f3")
    |>head
    
  entity.T_DWBM<-t_DWBM
  entity.T_YG<- t_YG
  try
      sb.AddToT_CK(entity) //EntityKey如果在"use sb"加载其它参考对象时，已经生成, 只是其值为空，所以，所有加载行为应该在同一个"use sb"中完成
      //sb.T_CK.Context.Attach entity
      ObjectDumper.Write ("wx")
      sb.T_CK.Context.SaveChanges()
   with
   | e ->ObjectDumper.Write(e,1);-1
  
let entity=T_CK.CreateT_CK(DateTime.Now, DateTime.Now,Guid.NewGuid(),"九门里1号仓库","00100100001001","0871","九门里",false,false)
let key=new EntityKey()

let t_DWBM=GetT_DWBM (Guid("69896fad-c145-4b3a-b122-36df6b6d2c87"))

//Wrong, 不能在不同的"use sb=new SBIIMSEntitiesAdvance()"中加载信息
let entity01=
        T_CK(C_BZ = null,
             C_CJRQ = DateTime.Now,
             C_CKDZ = "九门里",
             C_DQ = "001001",
             C_GXRQ = DateTime.Now,
             C_ID = Guid.NewGuid(),
             C_JY = false,
             C_LXDH = "九门里",
             C_MC = "九门里1号仓库",
             C_MR = false,
             T_DWBM=t_DWBM.Head )
             
entity01.EntityKey
entity01.EntityState
             
let entity02=
        T_CK(C_BZ = null,
             C_CJRQ = DateTime.Now,
             C_CKDZ = "九门里",
             C_DQ = "001001",
             C_GXRQ = DateTime.Now,
             C_ID = Guid.NewGuid(),
             C_JY = false,
             C_LXDH = "九门里",
             C_MC = "九门里1号仓库",
             C_MR = false )
AddT_CK entity02
  
  
*)