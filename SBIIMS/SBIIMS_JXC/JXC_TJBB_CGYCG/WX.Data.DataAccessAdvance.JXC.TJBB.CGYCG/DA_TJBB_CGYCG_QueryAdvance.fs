namespace WX.Data.DataAccess
open System
open System.Data
open Microsoft.Practices.EnterpriseLibrary.Logging
open Microsoft.FSharp.Collections
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess

[<Sealed>]
type DA_TJBB_CGYCG_QueryAdvance=
  inherit DA_Base
  static member public INS = DA_TJBB_CGYCG_QueryAdvance() 
  private new () = {inherit DA_Base()}
  interface IDA_TJBB_CGYCG_QueryAdvance with
    //统计-采购员采购-采购单据
    member this.GetTJBB_CGYCG_CGDJView (queryEntity:BQ_TJBB_CGYCG_Advance)=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_V_TJBB_CGYCG_CGDJ_Advance[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        let sb=new SBIIMS_JXC_TJBB_CGYCGEntitiesAdvance()
        sb.T_DJ_JHGL
          .Include(FTN.T_GHS)
          .Include(FTN.T_QFMX_JHGL)
          .Include(FTN.T_DJLX)
          .Include(FTN.T_CK1).Include(FTN.T_CK)
          .Include(FTN.T_YG1).Include(FTN.T_YG)
        |>PSeq.filter (fun a->
            match a.C_CJRQ,queryEntity.VC_CJRQ,queryEntity.VC_CJRQSecond with
            | x,y,z when y.HasValue && z.HasValue && y.Value=DateTime.MinValue ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value=DateTime.MaxValue ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.C_JBR,queryEntity.VC_JBRID with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.C_DJLX, queryEntity.VC_DJLXID with
            | x, y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.T_GHS.C_MCJM ,queryEntity.VC_GHSJM with     
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_GHS.C_XBH,queryEntity.VC_GHSXBH with //可以通过选择窗体选择
            | x,y when y.HasValue ->x =y.Value
            | _ ->true
            && 
            match a.C_CKBZ,a.C_RKBZ,a.T_CK.C_MCJM ,a.T_CK1.C_MCJM,queryEntity.VC_CKJM with
            | true, true, x, y, z when z<>null -> z.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith || z.ToLowerInvariant()|>y.ToLowerInvariant().StartsWith  //出库或入库均可
            | true, false, x, _, z when z<>null ->z.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith
            | false, true, _, y, z when z<>null ->z.ToLowerInvariant()|>y.ToLowerInvariant().StartsWith
            | _ ->true 
            && 
            match a.C_DJH,queryEntity.VC_DJH with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            )
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>Seq.map (fun a->
            match new BD_V_TJBB_CGYCG_CGDJ_Advance() with
            | x->
                x.VC_JBR<-a.T_YG1.C_XM 
                x.VC_CJRQ<-a.C_CJRQ
                x.VC_DJH<-a.C_DJH
                x.VC_DJLXID<-a.C_DJLX
                x.VC_DJLX<-a.T_DJLX.C_LX
                x.VC_GHS<-a.T_GHS.C_MC
                x.VC_CK<-
                  match a.C_RKBZ with
                  | true ->a.T_CK1.C_MC
                  | _ ->a.T_CK.C_MC
                x.VC_WFYFJE<-a.C_DJYZQJE |>op_UnaryNegation  
                x.VC_DJYHJE<-a.C_DJYHJE
                match a.C_KDJQBZ with //开单即结清标志，提高关联性能
                | true ->
                    x.VC_WFWFJE<-0M    //不能使用台账按查询单据的时间段来查询欠费，使用单据最终的欠费金额才符合实际要求
                    x.VC_WFSFJE<-x.VC_WFYFJE-x.VC_DJYHJE 
                | _ ->
                    x.VC_WFWFJE<-match a.T_QFMX_JHGL with null ->0M | y ->y.C_QFJE |>op_UnaryNegation //正负设计和单据一致   
                    x.VC_WFSFJE<-x.VC_WFYFJE-x.VC_DJYHJE-x.VC_WFWFJE 
                x.VC_CZY<-a.T_YG.C_XM
                x.VC_DJZT<-
                  match a.C_DJZT with
                  | 1uy ->"待审核"
                  | 2uy -> "审核中"
                  | 3uy -> "已审核"
                  | 4uy -> "取消审核"
                  | _ ->String.Empty
                x.VC_BZ<-a.C_BZ
                x.BD_V_TJBB_CGYCG_CGDJSP_AdvanceView <-
                  a.T_DJSP_JHGL.CreateSourceQuery()
                    .Include(FTN.T_SP)
                    .Include(FTN.T_SP.+FTN.T_SPYS)
                    .Include(FTN.T_SP.+FTN.T_SPDW)
                  |>Seq.map (fun b->
                      match new BD_V_TJBB_CGYCG_CGDJSP_Advance() with
                      | y ->
                          match b.T_SP with
                          | z ->
                              y.VC_SP<-z.C_MC
                              y.VC_SPXBH<-z.C_XBH
                              y.VC_SPJM<-z.C_MCJM
                              y.VC_SPYS<-z.T_SPYS.C_YS
                              y.VC_SPGGXH<-z.C_GGXH
                              y.VC_SPDW<-z.T_SPDW.C_DW
                          y.VC_SPPC<-b.C_PC
                          y.VC_SPSCRQ<-b.C_SCRQ
                          y.VC_SPDJ<-b.C_ZHJ
                          y.VC_SPSL<-b.C_VSL  //显示为正
                          y.VC_SPZJE<-b.C_VZHJE
                          y)
                  |>Seq.toArray
                x)
        |>Seq.toArray 
        |>Seq.toResult result
      with
      | e -> this.AttachError(e,-1,this,GetEntitiesAdvance,result)

    //统计-商品采购-商品明细
    member this.GetTJBB_CGYCG_SPMXView (queryEntity:BQ_TJBB_CGYCG_Advance)=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_V_TJBB_CGYCG_SPMX_Advance[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        let sb=new SBIIMS_JXC_TJBB_CGYCGEntitiesAdvance()
        sb.T_DJSP_JHGL
          .Include(FTN.T_GHS)
          .Include(FTN.T_DJLX)
          .Include(FTN.T_CK1).Include(FTN.T_CK)
          .Include(FTN.T_YG1)
          .Include(FTN.T_SP)
          .Include(FTN.T_SP.+FTN.T_SPYS)
          .Include(FTN.T_SP.+FTN.T_SPDW)
        |>PSeq.filter (fun a->
            match a.C_CJRQ,queryEntity.VC_CJRQ,queryEntity.VC_CJRQSecond with
            | x,y,z when y.HasValue && z.HasValue && y.Value=DateTime.MinValue ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value=DateTime.MaxValue ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_JBR,queryEntity.VC_JBRID with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.T_GHS.C_MCJM ,queryEntity.VC_GHSJM with     
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_GHS.C_XBH,queryEntity.VC_GHSXBH with //可以通过选择窗体选择
            | x,y when y.HasValue ->x =y.Value
            | _ ->true
            && 
            match a.T_SP.C_MCJM,queryEntity.VC_SPJM with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_SP.C_GGXH,queryEntity.VC_SPGGXH with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_SP.C_XBH,queryEntity.VC_SPXBH,queryEntity.VC_SPXBHSecond with
            | x,y,z when y.HasValue && z.HasValue && y.Value=Decimal.MinValue ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value=Decimal.MaxValue ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_PC,queryEntity.VC_SPPC,queryEntity.VC_SPPCSecond with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            )
        |>Seq.sortBy (fun a->DateTimeDefaultValue-a.C_CJRQ)
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>Seq.map (fun a->
            match new BD_V_TJBB_CGYCG_SPMX_Advance() with
            | x ->
                x.VC_CJRQ<-a.C_CJRQ
                x.VC_DJH<-a.C_DJH
                x.VC_DJLX<-a.T_DJLX.C_LX
                match a.T_SP with
                | y -> 
                    x.VC_SP<-y.C_MC
                    x.VC_SPXBH<-y.C_XBH
                    x.VC_SPYS<-y.T_SPYS.C_YS
                    x.VC_SPGGXH<-y.C_GGXH
                    x.VC_SPDW<-y.T_SPDW.C_DW
                x.VC_SPPC<-a.C_PC 
                x.VC_SPSCRQ<-a.C_SCRQ
                x.VC_SPDJ<-a.C_ZHJ
                x.VC_SPSL<-a.C_SL |>op_UnaryNegation //显示的正负刚好与后台正负设计相反
                x.VC_SPZJE<-a.C_ZHJE |>op_UnaryNegation
                x.VC_GHS<-a.T_GHS.C_MC
                x.VC_CK<-
                  match a.C_RKBZ with
                  | true ->a.T_CK1.C_MC
                  | _ ->a.T_CK.C_MC 
                x.VC_JBR<-a.T_YG1.C_XM
                x)
        |>Seq.toResult result
      with
      | e -> this.AttachError(e,-1,this,GetEntitiesAdvance,result)

   //统计-商品采购-商品汇总
    member this.GetTJBB_CGYCG_SPHZView (queryEntity:BQ_TJBB_CGYCG_Advance)=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_V_TJBB_CGYCG_SPHZ_Advance[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        let sb=new SBIIMS_JXC_TJBB_CGYCGEntitiesAdvance()
        sb.T_DJSP_JHGL
          .Include(FTN.T_SP)
          .Include(FTN.T_GHS)
          .Include(FTN.T_SP.+FTN.T_SPYS)
          .Include(FTN.T_SP.+FTN.T_SPDW)
          .Include(FTN.T_SP.+FTN.T_KCSP_DQZT)
          .Include(FTN.T_YG1)
        |>PSeq.filter (fun a->
            match a.C_CJRQ,queryEntity.VC_CJRQ,queryEntity.VC_CJRQSecond with
            | x,y,z when y.HasValue && z.HasValue && y.Value=DateTime.MinValue ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value=DateTime.MaxValue ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.C_JBR,queryEntity.VC_JBRID with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.T_GHS.C_MCJM ,queryEntity.VC_GHSJM with     
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_GHS.C_XBH,queryEntity.VC_GHSXBH with //可以通过选择窗体选择
            | x,y when y.HasValue ->x =y.Value
            | _ ->true
            && 
            match a.T_SP.C_MCJM,queryEntity.VC_SPJM with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_SP.C_GGXH,queryEntity.VC_SPGGXH with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.T_SP.C_XBH,queryEntity.VC_SPXBH,queryEntity.VC_SPXBHSecond with
            | x,y,z when y.HasValue && z.HasValue && y.Value=Decimal.MinValue ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value=Decimal.MaxValue ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            )
        |>Seq.groupBy (fun a->a.C_SP,a.C_CGGHS)  //按商品，采购供货商分组
        |>Seq.sortBy (fun (_,a)->(a|>Seq.head).T_SP.C_XBH|>op_UnaryNegation) //须测试排序方案
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>Seq.map (fun (a,b)->
            match new BD_V_TJBB_CGYCG_SPHZ_Advance() with
            | x ->
                match b|>Seq.head with
                | y ->
                    x.VC_GHS<-y.T_GHS.C_MC
                    match y.T_SP with
                    | z -> 
                        x.VC_SP<-z.C_MC
                        x.VC_SPXBH<-z.C_XBH
                        x.VC_SPYS<-z.T_SPYS.C_YS
                        x.VC_SPGGXH<-z.C_GGXH
                        x.VC_SPDW<-z.T_SPDW.C_DW
                        x.VC_SPSCCS<-z.C_SCCS
                        match z.T_KCSP_DQZT with
                        | w->
                            x.VC_DQKCSL<-w.C_CKSL
                match b|>Seq.filter (fun c->c.C_RKBZ) with
                | y ->
                    x.VC_CGSL<-y|>Seq.sumBy (fun c->c.C_SL) |>op_UnaryNegation 
                    x.VC_CGJE<-y|>Seq.sumBy (fun c->c.C_ZJE) |>op_UnaryNegation 
                match b|>Seq.filter (fun c->c.C_CKBZ) with
                | y ->
                    x.VC_CGTHSL<-y|>Seq.sumBy (fun c->c.C_SL) 
                    x.VC_CGTHJE<-y|>Seq.sumBy (fun c->c.C_ZJE) 
                x.VC_CGHJSL<-x.VC_CGSL-x.VC_CGTHSL
                x.VC_CGHJJE<- x.VC_CGJE-x.VC_CGTHJE
                x)
        |>Seq.toArray 
        |>Seq.page pagingInfo
        |>Seq.toResult result
      with
      | e -> this.AttachError(e,-1,this,GetEntitiesAdvance,result)