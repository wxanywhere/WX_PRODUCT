namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open WX.Data.BusinessBase

//统计报表-采购员采购
[<Sealed>]
[<DataContract>]
type BQ_TJBB_CGYCG_Advance()=
  inherit BQ_Base()


  //服务端部分
(* Template
[
"VC_CJRQ","DateTime",false,"创建日期"
"VC_GHSID","Guid",false,"供货商ID，包括供货商，客户，经营供货商，员工(薪资管理)"
"VC_GHS","string",false,"供货商名称"
"VC_GHSJM","string",false,"供货商简码"
"VC_GHSXBH","decimal",false,"供货商编号"
"VC_GHSXZID","byte",false,"供货商性质ID，1-供货商，2-客户，3-经营供货商，4-员工"
"VC_GHSLXID","byte",false,"供货商类型ID"
"VC_GHSLBID","byte",false,"供货商类别ID"
"VC_GHSDJID","byte",false,"供货商等级ID"
"VC_GHSDQID","Guid",false,"供货商地区ID"
"VC_SPID","Guid",false,"商品ID，包括商品，固定资产，消费品，用于关联父查询"
"VC_SPJM","string",false,"商品简码"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SPLBJM","string",false,"商品类别简码"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPLBID","Guid",false,"商品类别ID"
"VC_SPLBIDs","Guid[]",false,"商品类别ID数组"
"VC_CKID","Guid",false,"仓库ID"
"VC_CKJM","string",false,"仓库简码"
"VC_CKXBH","decimal",false,"仓库编号"
"VC_DJH","string",false,"单据号"
"VC_DJLXID","byte",false,"单据类型ID"
"VC_DJZQLXID","byte",false,"单据支取过滤类型ID，如1-不欠款单据，2-欠款单据"
"VC_JBRID","Guid",false,"经办人ID"
"VC_JBRJM","string",false,"经办人简码"
"VC_JBRXBH","decimal",false,"经办人编号"
"VC_TS","int",false,"天数"
"VC_BID","byte",false,"表ID"
"VC_NF","int",false,"年份"
]
*)
  //-------------------------------------------------

  let mutable _VC_CJRQ:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _IsQueryableNullOfVC_CJRQ=false
  let mutable _VC_CJRQSecond:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _VC_GHSID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _IsQueryableNullOfVC_GHSID=false
  let mutable _VC_GHS:string=null
  let mutable _IsQueryableNullOfVC_GHS=false
  let mutable _VC_GHSJM:string=null
  let mutable _IsQueryableNullOfVC_GHSJM=false
  let mutable _VC_GHSXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _IsQueryableNullOfVC_GHSXBH=false
  let mutable _VC_GHSXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_GHSXZID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_GHSXZID=false
  let mutable _VC_GHSXZIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSLXID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_GHSLXID=false
  let mutable _VC_GHSLXIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSLBID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_GHSLBID=false
  let mutable _VC_GHSLBIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSDJID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_GHSDJID=false
  let mutable _VC_GHSDJIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSDQID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _IsQueryableNullOfVC_GHSDQID=false
  let mutable _VC_SPID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _IsQueryableNullOfVC_SPID=false
  let mutable _VC_SPJM:string=null
  let mutable _IsQueryableNullOfVC_SPJM=false
  let mutable _VC_SPXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _IsQueryableNullOfVC_SPXBH=false
  let mutable _VC_SPXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_SPLBJM:string=null
  let mutable _IsQueryableNullOfVC_SPLBJM=false
  let mutable _VC_SPGGXH:string=null
  let mutable _IsQueryableNullOfVC_SPGGXH=false
  let mutable _VC_SPPC:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _IsQueryableNullOfVC_SPPC=false
  let mutable _VC_SPPCSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_SPSCRQ:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _IsQueryableNullOfVC_SPSCRQ=false
  let mutable _VC_SPSCRQSecond:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _VC_SPLBID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _IsQueryableNullOfVC_SPLBID=false
  let mutable _VC_SPLBIDs:Guid[]=null
  let mutable _IsQueryableNullOfVC_SPLBIDs=false
  let mutable _VC_CKID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _IsQueryableNullOfVC_CKID=false
  let mutable _VC_CKJM:string=null
  let mutable _IsQueryableNullOfVC_CKJM=false
  let mutable _VC_CKXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _IsQueryableNullOfVC_CKXBH=false
  let mutable _VC_CKXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_DJH:string=null
  let mutable _IsQueryableNullOfVC_DJH=false
  let mutable _VC_DJLXID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_DJLXID=false
  let mutable _VC_DJLXIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_DJZQLXID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_DJZQLXID=false
  let mutable _VC_DJZQLXIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_JBRID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _IsQueryableNullOfVC_JBRID=false
  let mutable _VC_JBRJM:string=null
  let mutable _IsQueryableNullOfVC_JBRJM=false
  let mutable _VC_JBRXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _IsQueryableNullOfVC_JBRXBH=false
  let mutable _VC_JBRXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_TS:System.Nullable<int>=new System.Nullable<int>()
  let mutable _IsQueryableNullOfVC_TS=false
  let mutable _VC_TSSecond:System.Nullable<int>=new System.Nullable<int>()
  let mutable _VC_BID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _IsQueryableNullOfVC_BID=false
  let mutable _VC_BIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_NF:System.Nullable<int>=new System.Nullable<int>()
  let mutable _IsQueryableNullOfVC_NF=false
  let mutable _VC_NFSecond:System.Nullable<int>=new System.Nullable<int>()
  [<DataMember>]
  member x.IsQueryableNullOfVC_CJRQ 
    with get ()=_IsQueryableNullOfVC_CJRQ 
    and set v=_IsQueryableNullOfVC_CJRQ<-v
  [<DataMember>]
  member x.VC_CJRQ 
    with get ()=_VC_CJRQ 
    and set v=_VC_CJRQ<-v
  [<DataMember>]
  member x.VC_CJRQSecond 
    with get ()=_VC_CJRQSecond 
    and set v=_VC_CJRQSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSID 
    with get ()=_IsQueryableNullOfVC_GHSID 
    and set v=_IsQueryableNullOfVC_GHSID<-v
  [<DataMember>]
  member x.VC_GHSID 
    with get ()=_VC_GHSID 
    and set v=_VC_GHSID<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHS 
    with get ()=_IsQueryableNullOfVC_GHS 
    and set v=_IsQueryableNullOfVC_GHS<-v
  [<DataMember>]
  member x.VC_GHS 
    with get ()=_VC_GHS 
    and set v=_VC_GHS<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSJM 
    with get ()=_IsQueryableNullOfVC_GHSJM 
    and set v=_IsQueryableNullOfVC_GHSJM<-v
  [<DataMember>]
  member x.VC_GHSJM 
    with get ()=_VC_GHSJM 
    and set v=_VC_GHSJM<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSXBH 
    with get ()=_IsQueryableNullOfVC_GHSXBH 
    and set v=_IsQueryableNullOfVC_GHSXBH<-v
  [<DataMember>]
  member x.VC_GHSXBH 
    with get ()=_VC_GHSXBH 
    and set v=_VC_GHSXBH<-v
  [<DataMember>]
  member x.VC_GHSXBHSecond 
    with get ()=_VC_GHSXBHSecond 
    and set v=_VC_GHSXBHSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSXZID 
    with get ()=_IsQueryableNullOfVC_GHSXZID 
    and set v=_IsQueryableNullOfVC_GHSXZID<-v
  [<DataMember>]
  member x.VC_GHSXZID 
    with get ()=_VC_GHSXZID 
    and set v=_VC_GHSXZID<-v
  [<DataMember>]
  member x.VC_GHSXZIDSecond 
    with get ()=_VC_GHSXZIDSecond 
    and set v=_VC_GHSXZIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSLXID 
    with get ()=_IsQueryableNullOfVC_GHSLXID 
    and set v=_IsQueryableNullOfVC_GHSLXID<-v
  [<DataMember>]
  member x.VC_GHSLXID 
    with get ()=_VC_GHSLXID 
    and set v=_VC_GHSLXID<-v
  [<DataMember>]
  member x.VC_GHSLXIDSecond 
    with get ()=_VC_GHSLXIDSecond 
    and set v=_VC_GHSLXIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSLBID 
    with get ()=_IsQueryableNullOfVC_GHSLBID 
    and set v=_IsQueryableNullOfVC_GHSLBID<-v
  [<DataMember>]
  member x.VC_GHSLBID 
    with get ()=_VC_GHSLBID 
    and set v=_VC_GHSLBID<-v
  [<DataMember>]
  member x.VC_GHSLBIDSecond 
    with get ()=_VC_GHSLBIDSecond 
    and set v=_VC_GHSLBIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSDJID 
    with get ()=_IsQueryableNullOfVC_GHSDJID 
    and set v=_IsQueryableNullOfVC_GHSDJID<-v
  [<DataMember>]
  member x.VC_GHSDJID 
    with get ()=_VC_GHSDJID 
    and set v=_VC_GHSDJID<-v
  [<DataMember>]
  member x.VC_GHSDJIDSecond 
    with get ()=_VC_GHSDJIDSecond 
    and set v=_VC_GHSDJIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_GHSDQID 
    with get ()=_IsQueryableNullOfVC_GHSDQID 
    and set v=_IsQueryableNullOfVC_GHSDQID<-v
  [<DataMember>]
  member x.VC_GHSDQID 
    with get ()=_VC_GHSDQID 
    and set v=_VC_GHSDQID<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPID 
    with get ()=_IsQueryableNullOfVC_SPID 
    and set v=_IsQueryableNullOfVC_SPID<-v
  [<DataMember>]
  member x.VC_SPID 
    with get ()=_VC_SPID 
    and set v=_VC_SPID<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPJM 
    with get ()=_IsQueryableNullOfVC_SPJM 
    and set v=_IsQueryableNullOfVC_SPJM<-v
  [<DataMember>]
  member x.VC_SPJM 
    with get ()=_VC_SPJM 
    and set v=_VC_SPJM<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPXBH 
    with get ()=_IsQueryableNullOfVC_SPXBH 
    and set v=_IsQueryableNullOfVC_SPXBH<-v
  [<DataMember>]
  member x.VC_SPXBH 
    with get ()=_VC_SPXBH 
    and set v=_VC_SPXBH<-v
  [<DataMember>]
  member x.VC_SPXBHSecond 
    with get ()=_VC_SPXBHSecond 
    and set v=_VC_SPXBHSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPLBJM 
    with get ()=_IsQueryableNullOfVC_SPLBJM 
    and set v=_IsQueryableNullOfVC_SPLBJM<-v
  [<DataMember>]
  member x.VC_SPLBJM 
    with get ()=_VC_SPLBJM 
    and set v=_VC_SPLBJM<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPGGXH 
    with get ()=_IsQueryableNullOfVC_SPGGXH 
    and set v=_IsQueryableNullOfVC_SPGGXH<-v
  [<DataMember>]
  member x.VC_SPGGXH 
    with get ()=_VC_SPGGXH 
    and set v=_VC_SPGGXH<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPPC 
    with get ()=_IsQueryableNullOfVC_SPPC 
    and set v=_IsQueryableNullOfVC_SPPC<-v
  [<DataMember>]
  member x.VC_SPPC 
    with get ()=_VC_SPPC 
    and set v=_VC_SPPC<-v
  [<DataMember>]
  member x.VC_SPPCSecond 
    with get ()=_VC_SPPCSecond 
    and set v=_VC_SPPCSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPSCRQ 
    with get ()=_IsQueryableNullOfVC_SPSCRQ 
    and set v=_IsQueryableNullOfVC_SPSCRQ<-v
  [<DataMember>]
  member x.VC_SPSCRQ 
    with get ()=_VC_SPSCRQ 
    and set v=_VC_SPSCRQ<-v
  [<DataMember>]
  member x.VC_SPSCRQSecond 
    with get ()=_VC_SPSCRQSecond 
    and set v=_VC_SPSCRQSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPLBID 
    with get ()=_IsQueryableNullOfVC_SPLBID 
    and set v=_IsQueryableNullOfVC_SPLBID<-v
  [<DataMember>]
  member x.VC_SPLBID 
    with get ()=_VC_SPLBID 
    and set v=_VC_SPLBID<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_SPLBIDs 
    with get ()=_IsQueryableNullOfVC_SPLBIDs 
    and set v=_IsQueryableNullOfVC_SPLBIDs<-v
  [<DataMember>]
  member x.VC_SPLBIDs 
    with get ()=_VC_SPLBIDs 
    and set v=_VC_SPLBIDs<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_CKID 
    with get ()=_IsQueryableNullOfVC_CKID 
    and set v=_IsQueryableNullOfVC_CKID<-v
  [<DataMember>]
  member x.VC_CKID 
    with get ()=_VC_CKID 
    and set v=_VC_CKID<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_CKJM 
    with get ()=_IsQueryableNullOfVC_CKJM 
    and set v=_IsQueryableNullOfVC_CKJM<-v
  [<DataMember>]
  member x.VC_CKJM 
    with get ()=_VC_CKJM 
    and set v=_VC_CKJM<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_CKXBH 
    with get ()=_IsQueryableNullOfVC_CKXBH 
    and set v=_IsQueryableNullOfVC_CKXBH<-v
  [<DataMember>]
  member x.VC_CKXBH 
    with get ()=_VC_CKXBH 
    and set v=_VC_CKXBH<-v
  [<DataMember>]
  member x.VC_CKXBHSecond 
    with get ()=_VC_CKXBHSecond 
    and set v=_VC_CKXBHSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_DJH 
    with get ()=_IsQueryableNullOfVC_DJH 
    and set v=_IsQueryableNullOfVC_DJH<-v
  [<DataMember>]
  member x.VC_DJH 
    with get ()=_VC_DJH 
    and set v=_VC_DJH<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_DJLXID 
    with get ()=_IsQueryableNullOfVC_DJLXID 
    and set v=_IsQueryableNullOfVC_DJLXID<-v
  [<DataMember>]
  member x.VC_DJLXID 
    with get ()=_VC_DJLXID 
    and set v=_VC_DJLXID<-v
  [<DataMember>]
  member x.VC_DJLXIDSecond 
    with get ()=_VC_DJLXIDSecond 
    and set v=_VC_DJLXIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_DJZQLXID 
    with get ()=_IsQueryableNullOfVC_DJZQLXID 
    and set v=_IsQueryableNullOfVC_DJZQLXID<-v
  [<DataMember>]
  member x.VC_DJZQLXID 
    with get ()=_VC_DJZQLXID 
    and set v=_VC_DJZQLXID<-v
  [<DataMember>]
  member x.VC_DJZQLXIDSecond 
    with get ()=_VC_DJZQLXIDSecond 
    and set v=_VC_DJZQLXIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_JBRID 
    with get ()=_IsQueryableNullOfVC_JBRID 
    and set v=_IsQueryableNullOfVC_JBRID<-v
  [<DataMember>]
  member x.VC_JBRID 
    with get ()=_VC_JBRID 
    and set v=_VC_JBRID<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_JBRJM 
    with get ()=_IsQueryableNullOfVC_JBRJM 
    and set v=_IsQueryableNullOfVC_JBRJM<-v
  [<DataMember>]
  member x.VC_JBRJM 
    with get ()=_VC_JBRJM 
    and set v=_VC_JBRJM<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_JBRXBH 
    with get ()=_IsQueryableNullOfVC_JBRXBH 
    and set v=_IsQueryableNullOfVC_JBRXBH<-v
  [<DataMember>]
  member x.VC_JBRXBH 
    with get ()=_VC_JBRXBH 
    and set v=_VC_JBRXBH<-v
  [<DataMember>]
  member x.VC_JBRXBHSecond 
    with get ()=_VC_JBRXBHSecond 
    and set v=_VC_JBRXBHSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_TS 
    with get ()=_IsQueryableNullOfVC_TS 
    and set v=_IsQueryableNullOfVC_TS<-v
  [<DataMember>]
  member x.VC_TS 
    with get ()=_VC_TS 
    and set v=_VC_TS<-v
  [<DataMember>]
  member x.VC_TSSecond 
    with get ()=_VC_TSSecond 
    and set v=_VC_TSSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_BID 
    with get ()=_IsQueryableNullOfVC_BID 
    and set v=_IsQueryableNullOfVC_BID<-v
  [<DataMember>]
  member x.VC_BID 
    with get ()=_VC_BID 
    and set v=_VC_BID<-v
  [<DataMember>]
  member x.VC_BIDSecond 
    with get ()=_VC_BIDSecond 
    and set v=_VC_BIDSecond<-v
  [<DataMember>]
  member x.IsQueryableNullOfVC_NF 
    with get ()=_IsQueryableNullOfVC_NF 
    and set v=_IsQueryableNullOfVC_NF<-v
  [<DataMember>]
  member x.VC_NF 
    with get ()=_VC_NF 
    and set v=_VC_NF<-v
  [<DataMember>]
  member x.VC_NFSecond 
    with get ()=_VC_NFSecond 
    and set v=_VC_NFSecond<-v