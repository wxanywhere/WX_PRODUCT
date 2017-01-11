

//#I  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
#r "FSharp.PowerPack.Linq.dll"
#r "FSharp.PowerPack.dll"

open System
open System.Linq
open System.Configuration
open System.Data.Objects

//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CModule.dll"
#r "WX.Data.DataModel.JXC.dll"
#r "WX.Data.DataModel.Query.JXC.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.DataAccessBase.dll"
#r "WX.Data.BusinessDataEntities.JXC"
#r "WX.Data.BusinessQueryEntities.JXC"
#r "WX.Data.DatabaseDictionary.JXC"
#r "WX.Data.IDataAccess.JXC"
#r "WX.Data.DataAccess.JXC"
#r "WX.Data.ClientHelper.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data
open WX.Data.DataModel
open WX.Data.DataModel.Query
open WX.Data.BusinessBase
open WX.Data.DataAccess
open WX.Data.ClientHelper

//=========================================================
ConfigHelper.INS.LoadDefaultClientConfigToManager
//SBIIMS_JXCEntities 须使用完整路径配置，metadata=D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug\SBIIMS_JXC.csdl|D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug\SBIIMS_JXC.ssdl|D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug\SBIIMS_JXC.msl
//=========================================================

//***********加载代码文件后，须打开其namespace才可用, 如果先批量加载，而后批量打开namespace, 则会引起类型不匹配的问题
#load "D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH\WX.Data.BusinessEntitiesAdvance.JXC.JHGL.CGJH\BQ_JHGL_CGJH_Advance.fs"
open WX.Data.BusinessEntities
#load "D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH\WX.Data.BusinessEntitiesAdvance.JXC.JHGL.CGJH\BD_TV_JHGL_CGJH_SHDJSP_Advance.fs"
open WX.Data.BusinessEntities
#load "D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH\WX.Data.BusinessEntitiesAdvance.JXC.JHGL.CGJH\BD_TV_JHGL_CGJH_SHDJ_Advance.fs"
open WX.Data.BusinessEntities
#load "D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH\WX.Data.IDataAccessAdvance.JXC.JHGL.CGJH\IDA_JHGL_CGJH_BusinessAdvance.fs"
open WX.Data.IDataAccess
#load "D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH\WX.Data.DataAccessAdvance.JXC.JHGL.CGJH\DA_JHGL_CGJH_BusinessAdvance.fs"
open WX.Data.DataAccess
#load "D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH\WX.Data.BusinessLogicAdvance.JXC.JHGL.CGJH\BL_JHGL_CGJH_Advance.fs"
open WX.Data.BusinessLogic


//=========================================================

let sb=new SBIIMS_QueryJXCEntitiesAdvance()

PSeq.join (sb.T_DJSP_JHGL) (sb.T_DJSP_KCGL ) (fun a->a.C_SP) (fun b->b.C_SP) (fun a b->a,b)
|>Seq.length

let x= new BQ_JHGL_CGJH_Advance()
x.IsReturnQueryError<-Nullable<_> true
let r=BL_JHGL_CGJH_Advance.INS.GetJHGL_CGJH_SHDJTableView(x)
ObjectDumper.Write r



let d=(DA_JHGL_CGJH_BusinessAdvance.INS:>IDA_JHGL_CGJH_BusinessAdvance).GetJHGL_CGJH_SHDJTableView(x)

//=========================================================

module LinqHelper =
    open Microsoft.FSharp.Quotations
    open Microsoft.FSharp.Linq.QuotationEvaluation
    open System.Linq.Expressions
    let ToLinq (exp : Expr<'a -> 'b>) =
        let linq = exp.ToLinqExpression()
        let call = linq :?> MethodCallExpression
        let lambda = call.Arguments.[0] :?> LambdaExpression
        Expression.Lambda<Func<'a, 'b>>(lambda.Body, lambda.Parameters) 


ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings //不能使用该语句，有时会将加载到内存的配置信息重置？？？
//ConfigurationManager.ConnectionStrings.["SBIIMSEntities"] 
//let sb=new SBIIMS_JXCEntitiesAdvance()
//let sb=new Query.SBIIMS_QueryJXCEntitiesAdvance()
let watch=new System.Diagnostics.Stopwatch()
//=========================================================
let sb=new SBIIMS_QueryJXCEntitiesAdvance()
watch.Restart()
sb.T_CK.MergeOption<-MergeOption.NoTracking
sb.T_CK
|>Seq.toArray
|>ObjectDumper.Write 
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds




//---------------------------------------------------------------------------
//http://stackoverflow.com/questions/1987477/how-to-call-c-method-taking-a-parameter-of-type-expressiont-from-f
//let exp:Expr<(SBIIMS_JXCEntitiesAdvance->T_CK array)> = <@ fun (sb:SBIIMS_JXCEntitiesAdvance)->sb.T_CK|>Seq.toArray @>

let expx01 = <@ fun (a:int)->1 @> //Rright!!!


let expx = <@ fun (sb:SBIIMS_JXCEntitiesAdvance)->sb.T_CK|>Seq.toArray @> //编译通过，但执行错误


expx|>LinqHelper.ToLinq 

CompiledQuery.Compile(expx|>LinqHelper.ToLinq )


//Wrong???
let sameCountry =
   query <@ seq { for e in sb.T_CK do  
                           yield (e) } @>


//------------------------------------------------------------------------------------




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
"JXC","KCGL","BSBY","库存管理","报损报溢" //合并商品报损和商品报溢                               
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
"JXC","ZHGL","JJGL","综合管理","均价管理"  
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

"JXC","FXYC","CGFX","分析预测","商品采购分析"  
"JXC","FXYC","CGJH","分析预测","采购商品计划"  
"JXC","FXYC","XSFX","分析预测","销售分析"  
"JXC","FXYC","XSYC","分析预测","销售预测"  
"JXC","FXYC","KCFX","分析预测","库存分析"  
"JXC","FXYC","KCYC","分析预测","库存积压风险预测"  
"JXC","FXYC","JGFX","分析预测","价格分析"   
"JXC","FXYC","JGYC","分析预测","价格趋势预测"    
"JXC","FXYC","YLFX","分析预测","盈利分析"  
"JXC","FXYC","YLYC","分析预测","盈利预测"  
"JXC","FXYC","GZLFX","分析预测","业务员工作量分析"          
"JXC","FXYC","GZLYC","分析预测","业务员工作量预测"       
                              
"JXC","XTGL","XTSZ","系统管理","系统设置"    
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
            xbh:= !xbh+1M
            y.C_XBH<- !xbh
            y.C_MC<-a 
            y.C_MCJM<- a.ToChinesePY()
            y.C_JDJB<-1uy 
            y.C_JDXH<- i 
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
                match new T_GNJD() with
                | z ->
                    z.C_ID<-Guid.NewGuid()
                    z.C_FID<-x 
                    z.C_GXRQ<-DateTime.Now
                    z.C_CJRQ<-DateTime.Now
                    xbh:= !xbh+1M
                    z.C_XBH<- !xbh
                    z.C_MC<-m5 
                    z.C_MCJM<-m5.ToChinesePY()
                    z.C_JDJB<-2uy 
                    z.C_JDXH<- j 
                    z.C_YZBZ<-true 
                    z.C_JYBZ<-false
                    z.C_ZPQYZT<- 1uy
                    match 
                      (match m2 with NotNullOrWhiteSpace _ -> m2+"_"+m3 | _ ->m3),
                      (match m2 with NotNullOrWhiteSpace _ -> m1+"."+m2+"."+m3 | _ ->m1+"."+m3),
                      (match m2 with NotNullOrWhiteSpace _ -> m1+"_"+m2+"_"+m3 | _ ->m1+"_"+m3) with
                    | u,v,w ->
                        z.C_SJLX<- String.Format(@"FVM_{0}",u)
                        z.C_GJSJLX<-String.Format(@"FVM_{0}_Advance",u)
                        z.C_LJ<-"" 
                        z.C_CS<-""
                        z.C_ZPM<-String.Format(@"WX.Data.FViewModel.{0}",v)
                        z.C_GJZPM<-String.Format(@"WX.Data.FViewModelAdvance.{0}",v)
                        z.C_ZPURI<-String.Format(@"/WX.Data.View.ViewModelTemplate.{0};Component/VMT_{1}.xaml",v,w)
                        z.C_GJZPURI<-String.Format(@"/WX.Data.View.ViewModelTemplateAdvance.{0};Component/VMT_{1}_Advance.xaml",v,w)
                        z.C_ZPLJ<-""
                        z.C_MMKJ<-""
                        z.C_BZ<-""
                    sb01.AddToT_GNJD(z)
            )
    )
sb01.SaveChanges()|>ignore

match new T_GNJD() with
| y ->
    y.C_ID<-parentGuid
    y.C_FID<-DefaultGuidValue 
    y.C_GXRQ<-DateTime.Now
    y.C_CJRQ<-DateTime.Now
    y.C_XBH<- 1000M
    y.C_MC<-"系统功能" 
    y.C_MCJM<-"系统功能".ToChinesePY()
    y.C_JDJB<- 0uy 
    y.C_JDXH<- 0 
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

    


watch.Reset()
watch.Start()

