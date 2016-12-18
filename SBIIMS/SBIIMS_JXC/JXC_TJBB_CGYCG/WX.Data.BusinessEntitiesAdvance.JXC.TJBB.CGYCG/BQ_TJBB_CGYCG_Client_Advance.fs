namespace WX.Data.BusinessEntities
open System
open System.Windows
open WX.Data
open WX.Data.BusinessBase

//统计报表-采购员采购
type BQ_TJBB_CGYCG_Client_Advance()=
  inherit BQ_ClientBase()

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
  let mutable _VC_CJRQSecond:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _VC_GHSID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _VC_GHS:string=null
  let mutable _VC_GHSJM:string=null
  let mutable _VC_GHSXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_GHSXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_GHSXZID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSXZIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSLXID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSLXIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSLBID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSLBIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSDJID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSDJIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_GHSDQID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _VC_SPID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _VC_SPJM:string=null
  let mutable _VC_SPXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_SPXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_SPLBJM:string=null
  let mutable _VC_SPGGXH:string=null
  let mutable _VC_SPPC:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_SPPCSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_SPSCRQ:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _VC_SPSCRQSecond:System.Nullable<DateTime>=new System.Nullable<DateTime>()
  let mutable _VC_SPLBID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _VC_SPLBIDs:Guid[]=null
  let mutable _VC_CKID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _VC_CKJM:string=null
  let mutable _VC_CKXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_CKXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_DJH:string=null
  let mutable _VC_DJLXID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_DJLXIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_DJZQLXID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_DJZQLXIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_JBRID:System.Nullable<Guid>=new System.Nullable<Guid>()
  let mutable _VC_JBRJM:string=null
  let mutable _VC_JBRXBH:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_JBRXBHSecond:System.Nullable<decimal>=new System.Nullable<decimal>()
  let mutable _VC_TS:System.Nullable<int>=new System.Nullable<int>()
  let mutable _VC_TSSecond:System.Nullable<int>=new System.Nullable<int>()
  let mutable _VC_BID:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_BIDSecond:System.Nullable<byte>=new System.Nullable<byte>()
  let mutable _VC_NF:System.Nullable<int>=new System.Nullable<int>()
  let mutable _VC_NFSecond:System.Nullable<int>=new System.Nullable<int>()
  let mutable _IsCheckedVC_CJRQ=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSID=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHS=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSJM=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSXBH=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSXZID=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSLXID=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSLBID=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSDJID=new Nullable<_>(false)
  let mutable _IsCheckedVC_GHSDQID=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPID=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPJM=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPXBH=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPLBJM=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPGGXH=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPPC=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPSCRQ=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPLBID=new Nullable<_>(false)
  let mutable _IsCheckedVC_SPLBIDs=new Nullable<_>(false)
  let mutable _IsCheckedVC_CKID=new Nullable<_>(false)
  let mutable _IsCheckedVC_CKJM=new Nullable<_>(false)
  let mutable _IsCheckedVC_CKXBH=new Nullable<_>(false)
  let mutable _IsCheckedVC_DJH=new Nullable<_>(false)
  let mutable _IsCheckedVC_DJLXID=new Nullable<_>(false)
  let mutable _IsCheckedVC_DJZQLXID=new Nullable<_>(false)
  let mutable _IsCheckedVC_JBRID=new Nullable<_>(false)
  let mutable _IsCheckedVC_JBRJM=new Nullable<_>(false)
  let mutable _IsCheckedVC_JBRXBH=new Nullable<_>(false)
  let mutable _IsCheckedVC_TS=new Nullable<_>(false)
  let mutable _IsCheckedVC_BID=new Nullable<_>(false)
  let mutable _IsCheckedVC_NF=new Nullable<_>(false)
  let mutable _IsDefaultVC_CJRQ=false
  let mutable _IsDefaultVC_GHSID=false
  let mutable _IsDefaultVC_GHS=false
  let mutable _IsDefaultVC_GHSJM=false
  let mutable _IsDefaultVC_GHSXBH=false
  let mutable _IsDefaultVC_GHSXZID=false
  let mutable _IsDefaultVC_GHSLXID=false
  let mutable _IsDefaultVC_GHSLBID=false
  let mutable _IsDefaultVC_GHSDJID=false
  let mutable _IsDefaultVC_GHSDQID=false
  let mutable _IsDefaultVC_SPID=false
  let mutable _IsDefaultVC_SPJM=false
  let mutable _IsDefaultVC_SPXBH=false
  let mutable _IsDefaultVC_SPLBJM=false
  let mutable _IsDefaultVC_SPGGXH=false
  let mutable _IsDefaultVC_SPPC=false
  let mutable _IsDefaultVC_SPSCRQ=false
  let mutable _IsDefaultVC_SPLBID=false
  let mutable _IsDefaultVC_SPLBIDs=false
  let mutable _IsDefaultVC_CKID=false
  let mutable _IsDefaultVC_CKJM=false
  let mutable _IsDefaultVC_CKXBH=false
  let mutable _IsDefaultVC_DJH=false
  let mutable _IsDefaultVC_DJLXID=false
  let mutable _IsDefaultVC_DJZQLXID=false
  let mutable _IsDefaultVC_JBRID=false
  let mutable _IsDefaultVC_JBRJM=false
  let mutable _IsDefaultVC_JBRXBH=false
  let mutable _IsDefaultVC_TS=false
  let mutable _IsDefaultVC_BID=false
  let mutable _IsDefaultVC_NF=false
  let mutable _VisibilityVC_CJRQ=Visibility.Collapsed
  let mutable _VisibilityVC_GHSID=Visibility.Collapsed
  let mutable _VisibilityVC_GHS=Visibility.Collapsed
  let mutable _VisibilityVC_GHSJM=Visibility.Collapsed
  let mutable _VisibilityVC_GHSXBH=Visibility.Collapsed
  let mutable _VisibilityVC_GHSXZID=Visibility.Collapsed
  let mutable _VisibilityVC_GHSLXID=Visibility.Collapsed
  let mutable _VisibilityVC_GHSLBID=Visibility.Collapsed
  let mutable _VisibilityVC_GHSDJID=Visibility.Collapsed
  let mutable _VisibilityVC_GHSDQID=Visibility.Collapsed
  let mutable _VisibilityVC_SPID=Visibility.Collapsed
  let mutable _VisibilityVC_SPJM=Visibility.Collapsed
  let mutable _VisibilityVC_SPXBH=Visibility.Collapsed
  let mutable _VisibilityVC_SPLBJM=Visibility.Collapsed
  let mutable _VisibilityVC_SPGGXH=Visibility.Collapsed
  let mutable _VisibilityVC_SPPC=Visibility.Collapsed
  let mutable _VisibilityVC_SPSCRQ=Visibility.Collapsed
  let mutable _VisibilityVC_SPLBID=Visibility.Collapsed
  let mutable _VisibilityVC_SPLBIDs=Visibility.Collapsed
  let mutable _VisibilityVC_CKID=Visibility.Collapsed
  let mutable _VisibilityVC_CKJM=Visibility.Collapsed
  let mutable _VisibilityVC_CKXBH=Visibility.Collapsed
  let mutable _VisibilityVC_DJH=Visibility.Collapsed
  let mutable _VisibilityVC_DJLXID=Visibility.Collapsed
  let mutable _VisibilityVC_DJZQLXID=Visibility.Collapsed
  let mutable _VisibilityVC_JBRID=Visibility.Collapsed
  let mutable _VisibilityVC_JBRJM=Visibility.Collapsed
  let mutable _VisibilityVC_JBRXBH=Visibility.Collapsed
  let mutable _VisibilityVC_TS=Visibility.Collapsed
  let mutable _VisibilityVC_BID=Visibility.Collapsed
  let mutable _VisibilityVC_NF=Visibility.Collapsed
  member x.VC_CJRQ 
    with get ()=_VC_CJRQ 
    and set (v:System.Nullable<DateTime>)=
      if _VC_CJRQ<>v then
        _VC_CJRQ<-v
        x.OnPropertyChanged "VC_CJRQ"
      else x.IsDefaultVC_CJRQ<-false
      match v with
      | _ when v.HasValue ->
          if _VC_CJRQSecond.HasValue && _VC_CJRQSecond.Value>=v.Value|>not then
            x.VC_CJRQSecond<-v
          x.IsCheckedVC_CJRQ<-new Nullable<_>(true)
          x.VisibilityVC_CJRQ<-Visibility.Visible
      | _ ->
          x.VC_CJRQSecond<-v 
          x.IsCheckedVC_CJRQ<-new Nullable<_>(false)
          x.VisibilityVC_CJRQ<-Visibility.Collapsed
  member x.VC_CJRQSecond 
    with get ()=_VC_CJRQSecond 
    and set (v:System.Nullable<DateTime>)=
      if _VC_CJRQSecond<>v then
        _VC_CJRQSecond<-v
        x.OnPropertyChanged "VC_CJRQSecond"
      match v with
      | _ when v.HasValue->
          if _VC_CJRQ.HasValue |>not then
            x.VC_CJRQ<-v
          elif  v.Value>=_VC_CJRQ.Value |>not  then 
            _VC_CJRQSecond<-_VC_CJRQ
            x.OnPropertyChanged "VC_CJRQSecond"
          match v.Value with
          | y -> 
              if  y.Hour=0 && y.Minute=0 && y.Second=0 && y.Millisecond=0 then
                _VC_CJRQSecond<-Nullable<_>(y.AddDays(1.0).AddMilliseconds(-1.0))
                x.OnPropertyChanged "VC_CJRQSecond"
      | _ ->()
  member x.VC_GHSID 
    with get ()=_VC_GHSID 
    and set v=
      if _VC_GHSID<>v then
        _VC_GHSID <-v
        x.OnPropertyChanged "VC_GHSID"
      else x.IsDefaultVC_GHSID<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsCheckedVC_GHSID<-new Nullable<_>(true)
          x.VisibilityVC_GHSID<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_GHSID<-new Nullable<_>(false)
          x.VisibilityVC_GHSID<-Visibility.Collapsed
  member x.VC_GHS 
    with get ()=_VC_GHS 
    and set v=
      if _VC_GHS<>v then
        _VC_GHS <-v
        x.OnPropertyChanged "VC_GHS"
      else x.IsDefaultVC_GHS<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_GHS<-new Nullable<_>(true)
          x.VisibilityVC_GHS<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_GHS<-new Nullable<_>(false)
          x.VisibilityVC_GHS<-Visibility.Collapsed
  member x.VC_GHSJM 
    with get ()=_VC_GHSJM 
    and set v=
      if _VC_GHSJM<>v then
        _VC_GHSJM <-v
        x.OnPropertyChanged "VC_GHSJM"
      else x.IsDefaultVC_GHSJM<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_GHSJM<-new Nullable<_>(true)
          x.VisibilityVC_GHSJM<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_GHSJM<-new Nullable<_>(false)
          x.VisibilityVC_GHSJM<-Visibility.Collapsed
  member x.VC_GHSXBH 
    with get ()=_VC_GHSXBH 
    and set (v:System.Nullable<decimal>)=
      if _VC_GHSXBH<>v then
        _VC_GHSXBH<-v
        x.OnPropertyChanged "VC_GHSXBH"
      else x.IsDefaultVC_GHSXBH<-false
      match v with
      | _ when v.HasValue ->
          if _VC_GHSXBHSecond.HasValue && _VC_GHSXBHSecond.Value>=v.Value|>not then
            x.VC_GHSXBHSecond<-v
          x.IsCheckedVC_GHSXBH<-new Nullable<_>(true)
          x.VisibilityVC_GHSXBH<-Visibility.Visible
      | _ ->
          x.VC_GHSXBHSecond<-v 
          x.IsCheckedVC_GHSXBH<-new Nullable<_>(false)
          x.VisibilityVC_GHSXBH<-Visibility.Collapsed
  member x.VC_GHSXBHSecond 
    with get ()=_VC_GHSXBHSecond 
    and set (v:System.Nullable<decimal>)=
      if _VC_GHSXBHSecond<>v then
        _VC_GHSXBHSecond<-v
        x.OnPropertyChanged "VC_GHSXBHSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_GHSXBH.HasValue |>not then
            x.VC_GHSXBH<-v
          elif  v.Value>=_VC_GHSXBH.Value |>not  then 
            _VC_GHSXBHSecond<-_VC_GHSXBH
            x.OnPropertyChanged "VC_GHSXBHSecond"
      | _ ->()
  member x.VC_GHSXZID 
    with get ()=_VC_GHSXZID 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSXZID<>v then
        _VC_GHSXZID<-v
        x.OnPropertyChanged "VC_GHSXZID"
      else x.IsDefaultVC_GHSXZID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_GHSXZIDSecond.HasValue && _VC_GHSXZIDSecond.Value>=v.Value|>not then
            x.VC_GHSXZIDSecond<-v
          x.IsCheckedVC_GHSXZID<-new Nullable<_>(true)
          x.VisibilityVC_GHSXZID<-Visibility.Visible
      | _ ->
          x.VC_GHSXZIDSecond<-v 
          x.IsCheckedVC_GHSXZID<-new Nullable<_>(false)
          x.VisibilityVC_GHSXZID<-Visibility.Collapsed
  member x.VC_GHSXZIDSecond 
    with get ()=_VC_GHSXZIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSXZIDSecond<>v then
        _VC_GHSXZIDSecond<-v
        x.OnPropertyChanged "VC_GHSXZIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_GHSXZID.HasValue |>not then
            x.VC_GHSXZID<-v
          elif  v.Value>=_VC_GHSXZID.Value |>not  then 
            _VC_GHSXZIDSecond<-_VC_GHSXZID
            x.OnPropertyChanged "VC_GHSXZIDSecond"
      | _ ->()
  member x.VC_GHSLXID 
    with get ()=_VC_GHSLXID 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSLXID<>v then
        _VC_GHSLXID<-v
        x.OnPropertyChanged "VC_GHSLXID"
      else x.IsDefaultVC_GHSLXID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_GHSLXIDSecond.HasValue && _VC_GHSLXIDSecond.Value>=v.Value|>not then
            x.VC_GHSLXIDSecond<-v
          x.IsCheckedVC_GHSLXID<-new Nullable<_>(true)
          x.VisibilityVC_GHSLXID<-Visibility.Visible
      | _ ->
          x.VC_GHSLXIDSecond<-v 
          x.IsCheckedVC_GHSLXID<-new Nullable<_>(false)
          x.VisibilityVC_GHSLXID<-Visibility.Collapsed
  member x.VC_GHSLXIDSecond 
    with get ()=_VC_GHSLXIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSLXIDSecond<>v then
        _VC_GHSLXIDSecond<-v
        x.OnPropertyChanged "VC_GHSLXIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_GHSLXID.HasValue |>not then
            x.VC_GHSLXID<-v
          elif  v.Value>=_VC_GHSLXID.Value |>not  then 
            _VC_GHSLXIDSecond<-_VC_GHSLXID
            x.OnPropertyChanged "VC_GHSLXIDSecond"
      | _ ->()
  member x.VC_GHSLBID 
    with get ()=_VC_GHSLBID 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSLBID<>v then
        _VC_GHSLBID<-v
        x.OnPropertyChanged "VC_GHSLBID"
      else x.IsDefaultVC_GHSLBID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_GHSLBIDSecond.HasValue && _VC_GHSLBIDSecond.Value>=v.Value|>not then
            x.VC_GHSLBIDSecond<-v
          x.IsCheckedVC_GHSLBID<-new Nullable<_>(true)
          x.VisibilityVC_GHSLBID<-Visibility.Visible
      | _ ->
          x.VC_GHSLBIDSecond<-v 
          x.IsCheckedVC_GHSLBID<-new Nullable<_>(false)
          x.VisibilityVC_GHSLBID<-Visibility.Collapsed
  member x.VC_GHSLBIDSecond 
    with get ()=_VC_GHSLBIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSLBIDSecond<>v then
        _VC_GHSLBIDSecond<-v
        x.OnPropertyChanged "VC_GHSLBIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_GHSLBID.HasValue |>not then
            x.VC_GHSLBID<-v
          elif  v.Value>=_VC_GHSLBID.Value |>not  then 
            _VC_GHSLBIDSecond<-_VC_GHSLBID
            x.OnPropertyChanged "VC_GHSLBIDSecond"
      | _ ->()
  member x.VC_GHSDJID 
    with get ()=_VC_GHSDJID 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSDJID<>v then
        _VC_GHSDJID<-v
        x.OnPropertyChanged "VC_GHSDJID"
      else x.IsDefaultVC_GHSDJID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_GHSDJIDSecond.HasValue && _VC_GHSDJIDSecond.Value>=v.Value|>not then
            x.VC_GHSDJIDSecond<-v
          x.IsCheckedVC_GHSDJID<-new Nullable<_>(true)
          x.VisibilityVC_GHSDJID<-Visibility.Visible
      | _ ->
          x.VC_GHSDJIDSecond<-v 
          x.IsCheckedVC_GHSDJID<-new Nullable<_>(false)
          x.VisibilityVC_GHSDJID<-Visibility.Collapsed
  member x.VC_GHSDJIDSecond 
    with get ()=_VC_GHSDJIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_GHSDJIDSecond<>v then
        _VC_GHSDJIDSecond<-v
        x.OnPropertyChanged "VC_GHSDJIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_GHSDJID.HasValue |>not then
            x.VC_GHSDJID<-v
          elif  v.Value>=_VC_GHSDJID.Value |>not  then 
            _VC_GHSDJIDSecond<-_VC_GHSDJID
            x.OnPropertyChanged "VC_GHSDJIDSecond"
      | _ ->()
  member x.VC_GHSDQID 
    with get ()=_VC_GHSDQID 
    and set v=
      if _VC_GHSDQID<>v then
        _VC_GHSDQID <-v
        x.OnPropertyChanged "VC_GHSDQID"
      else x.IsDefaultVC_GHSDQID<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsCheckedVC_GHSDQID<-new Nullable<_>(true)
          x.VisibilityVC_GHSDQID<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_GHSDQID<-new Nullable<_>(false)
          x.VisibilityVC_GHSDQID<-Visibility.Collapsed
  member x.VC_SPID 
    with get ()=_VC_SPID 
    and set v=
      if _VC_SPID<>v then
        _VC_SPID <-v
        x.OnPropertyChanged "VC_SPID"
      else x.IsDefaultVC_SPID<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsCheckedVC_SPID<-new Nullable<_>(true)
          x.VisibilityVC_SPID<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_SPID<-new Nullable<_>(false)
          x.VisibilityVC_SPID<-Visibility.Collapsed
  member x.VC_SPJM 
    with get ()=_VC_SPJM 
    and set v=
      if _VC_SPJM<>v then
        _VC_SPJM <-v
        x.OnPropertyChanged "VC_SPJM"
      else x.IsDefaultVC_SPJM<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_SPJM<-new Nullable<_>(true)
          x.VisibilityVC_SPJM<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_SPJM<-new Nullable<_>(false)
          x.VisibilityVC_SPJM<-Visibility.Collapsed
  member x.VC_SPXBH 
    with get ()=_VC_SPXBH 
    and set (v:System.Nullable<decimal>)=
      if _VC_SPXBH<>v then
        _VC_SPXBH<-v
        x.OnPropertyChanged "VC_SPXBH"
      else x.IsDefaultVC_SPXBH<-false
      match v with
      | _ when v.HasValue ->
          if _VC_SPXBHSecond.HasValue && _VC_SPXBHSecond.Value>=v.Value|>not then
            x.VC_SPXBHSecond<-v
          x.IsCheckedVC_SPXBH<-new Nullable<_>(true)
          x.VisibilityVC_SPXBH<-Visibility.Visible
      | _ ->
          x.VC_SPXBHSecond<-v 
          x.IsCheckedVC_SPXBH<-new Nullable<_>(false)
          x.VisibilityVC_SPXBH<-Visibility.Collapsed
  member x.VC_SPXBHSecond 
    with get ()=_VC_SPXBHSecond 
    and set (v:System.Nullable<decimal>)=
      if _VC_SPXBHSecond<>v then
        _VC_SPXBHSecond<-v
        x.OnPropertyChanged "VC_SPXBHSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_SPXBH.HasValue |>not then
            x.VC_SPXBH<-v
          elif  v.Value>=_VC_SPXBH.Value |>not  then 
            _VC_SPXBHSecond<-_VC_SPXBH
            x.OnPropertyChanged "VC_SPXBHSecond"
      | _ ->()
  member x.VC_SPLBJM 
    with get ()=_VC_SPLBJM 
    and set v=
      if _VC_SPLBJM<>v then
        _VC_SPLBJM <-v
        x.OnPropertyChanged "VC_SPLBJM"
      else x.IsDefaultVC_SPLBJM<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_SPLBJM<-new Nullable<_>(true)
          x.VisibilityVC_SPLBJM<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_SPLBJM<-new Nullable<_>(false)
          x.VisibilityVC_SPLBJM<-Visibility.Collapsed
  member x.VC_SPGGXH 
    with get ()=_VC_SPGGXH 
    and set v=
      if _VC_SPGGXH<>v then
        _VC_SPGGXH <-v
        x.OnPropertyChanged "VC_SPGGXH"
      else x.IsDefaultVC_SPGGXH<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_SPGGXH<-new Nullable<_>(true)
          x.VisibilityVC_SPGGXH<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_SPGGXH<-new Nullable<_>(false)
          x.VisibilityVC_SPGGXH<-Visibility.Collapsed
  member x.VC_SPPC 
    with get ()=_VC_SPPC 
    and set (v:System.Nullable<decimal>)=
      if _VC_SPPC<>v then
        _VC_SPPC<-v
        x.OnPropertyChanged "VC_SPPC"
      else x.IsDefaultVC_SPPC<-false
      match v with
      | _ when v.HasValue ->
          if _VC_SPPCSecond.HasValue && _VC_SPPCSecond.Value>=v.Value|>not then
            x.VC_SPPCSecond<-v
          x.IsCheckedVC_SPPC<-new Nullable<_>(true)
          x.VisibilityVC_SPPC<-Visibility.Visible
      | _ ->
          x.VC_SPPCSecond<-v 
          x.IsCheckedVC_SPPC<-new Nullable<_>(false)
          x.VisibilityVC_SPPC<-Visibility.Collapsed
  member x.VC_SPPCSecond 
    with get ()=_VC_SPPCSecond 
    and set (v:System.Nullable<decimal>)=
      if _VC_SPPCSecond<>v then
        _VC_SPPCSecond<-v
        x.OnPropertyChanged "VC_SPPCSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_SPPC.HasValue |>not then
            x.VC_SPPC<-v
          elif  v.Value>=_VC_SPPC.Value |>not  then 
            _VC_SPPCSecond<-_VC_SPPC
            x.OnPropertyChanged "VC_SPPCSecond"
      | _ ->()
  member x.VC_SPSCRQ 
    with get ()=_VC_SPSCRQ 
    and set (v:System.Nullable<DateTime>)=
      if _VC_SPSCRQ<>v then
        _VC_SPSCRQ<-v
        x.OnPropertyChanged "VC_SPSCRQ"
      else x.IsDefaultVC_SPSCRQ<-false
      match v with
      | _ when v.HasValue ->
          if _VC_SPSCRQSecond.HasValue && _VC_SPSCRQSecond.Value>=v.Value|>not then
            x.VC_SPSCRQSecond<-v
          x.IsCheckedVC_SPSCRQ<-new Nullable<_>(true)
          x.VisibilityVC_SPSCRQ<-Visibility.Visible
      | _ ->
          x.VC_SPSCRQSecond<-v 
          x.IsCheckedVC_SPSCRQ<-new Nullable<_>(false)
          x.VisibilityVC_SPSCRQ<-Visibility.Collapsed
  member x.VC_SPSCRQSecond 
    with get ()=_VC_SPSCRQSecond 
    and set (v:System.Nullable<DateTime>)=
      if _VC_SPSCRQSecond<>v then
        _VC_SPSCRQSecond<-v
        x.OnPropertyChanged "VC_SPSCRQSecond"
      match v with
      | _ when v.HasValue->
          if _VC_SPSCRQ.HasValue |>not then
            x.VC_SPSCRQ<-v
          elif  v.Value>=_VC_SPSCRQ.Value |>not  then 
            _VC_SPSCRQSecond<-_VC_SPSCRQ
            x.OnPropertyChanged "VC_SPSCRQSecond"
          match v.Value with
          | y -> 
              if  y.Hour=0 && y.Minute=0 && y.Second=0 && y.Millisecond=0 then
                _VC_SPSCRQSecond<-Nullable<_>(y.AddDays(1.0).AddMilliseconds(-1.0))
                x.OnPropertyChanged "VC_SPSCRQSecond"
      | _ ->()
  member x.VC_SPLBID 
    with get ()=_VC_SPLBID 
    and set v=
      if _VC_SPLBID<>v then
        _VC_SPLBID <-v
        x.OnPropertyChanged "VC_SPLBID"
      else x.IsDefaultVC_SPLBID<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsCheckedVC_SPLBID<-new Nullable<_>(true)
          x.VisibilityVC_SPLBID<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_SPLBID<-new Nullable<_>(false)
          x.VisibilityVC_SPLBID<-Visibility.Collapsed
  member x.VC_SPLBIDs 
    with get ()=_VC_SPLBIDs 
    and set v=
      if _VC_SPLBIDs<>v then
        _VC_SPLBIDs <-v
        x.OnPropertyChanged "VC_SPLBIDs"
      else x.IsDefaultVC_SPLBIDs<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_SPLBIDs<-new Nullable<_>(true)
          x.VisibilityVC_SPLBIDs<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_SPLBIDs<-new Nullable<_>(false)
          x.VisibilityVC_SPLBIDs<-Visibility.Collapsed
  member x.VC_CKID 
    with get ()=_VC_CKID 
    and set v=
      if _VC_CKID<>v then
        _VC_CKID <-v
        x.OnPropertyChanged "VC_CKID"
      else x.IsDefaultVC_CKID<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsCheckedVC_CKID<-new Nullable<_>(true)
          x.VisibilityVC_CKID<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_CKID<-new Nullable<_>(false)
          x.VisibilityVC_CKID<-Visibility.Collapsed
  member x.VC_CKJM 
    with get ()=_VC_CKJM 
    and set v=
      if _VC_CKJM<>v then
        _VC_CKJM <-v
        x.OnPropertyChanged "VC_CKJM"
      else x.IsDefaultVC_CKJM<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_CKJM<-new Nullable<_>(true)
          x.VisibilityVC_CKJM<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_CKJM<-new Nullable<_>(false)
          x.VisibilityVC_CKJM<-Visibility.Collapsed
  member x.VC_CKXBH 
    with get ()=_VC_CKXBH 
    and set (v:System.Nullable<decimal>)=
      if _VC_CKXBH<>v then
        _VC_CKXBH<-v
        x.OnPropertyChanged "VC_CKXBH"
      else x.IsDefaultVC_CKXBH<-false
      match v with
      | _ when v.HasValue ->
          if _VC_CKXBHSecond.HasValue && _VC_CKXBHSecond.Value>=v.Value|>not then
            x.VC_CKXBHSecond<-v
          x.IsCheckedVC_CKXBH<-new Nullable<_>(true)
          x.VisibilityVC_CKXBH<-Visibility.Visible
      | _ ->
          x.VC_CKXBHSecond<-v 
          x.IsCheckedVC_CKXBH<-new Nullable<_>(false)
          x.VisibilityVC_CKXBH<-Visibility.Collapsed
  member x.VC_CKXBHSecond 
    with get ()=_VC_CKXBHSecond 
    and set (v:System.Nullable<decimal>)=
      if _VC_CKXBHSecond<>v then
        _VC_CKXBHSecond<-v
        x.OnPropertyChanged "VC_CKXBHSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_CKXBH.HasValue |>not then
            x.VC_CKXBH<-v
          elif  v.Value>=_VC_CKXBH.Value |>not  then 
            _VC_CKXBHSecond<-_VC_CKXBH
            x.OnPropertyChanged "VC_CKXBHSecond"
      | _ ->()
  member x.VC_DJH 
    with get ()=_VC_DJH 
    and set v=
      if _VC_DJH<>v then
        _VC_DJH <-v
        x.OnPropertyChanged "VC_DJH"
      else x.IsDefaultVC_DJH<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_DJH<-new Nullable<_>(true)
          x.VisibilityVC_DJH<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_DJH<-new Nullable<_>(false)
          x.VisibilityVC_DJH<-Visibility.Collapsed
  member x.VC_DJLXID 
    with get ()=_VC_DJLXID 
    and set (v:System.Nullable<byte>)=
      if _VC_DJLXID<>v then
        _VC_DJLXID<-v
        x.OnPropertyChanged "VC_DJLXID"
      else x.IsDefaultVC_DJLXID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_DJLXIDSecond.HasValue && _VC_DJLXIDSecond.Value>=v.Value|>not then
            x.VC_DJLXIDSecond<-v
          x.IsCheckedVC_DJLXID<-new Nullable<_>(true)
          x.VisibilityVC_DJLXID<-Visibility.Visible
      | _ ->
          x.VC_DJLXIDSecond<-v 
          x.IsCheckedVC_DJLXID<-new Nullable<_>(false)
          x.VisibilityVC_DJLXID<-Visibility.Collapsed
  member x.VC_DJLXIDSecond 
    with get ()=_VC_DJLXIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_DJLXIDSecond<>v then
        _VC_DJLXIDSecond<-v
        x.OnPropertyChanged "VC_DJLXIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_DJLXID.HasValue |>not then
            x.VC_DJLXID<-v
          elif  v.Value>=_VC_DJLXID.Value |>not  then 
            _VC_DJLXIDSecond<-_VC_DJLXID
            x.OnPropertyChanged "VC_DJLXIDSecond"
      | _ ->()
  member x.VC_DJZQLXID 
    with get ()=_VC_DJZQLXID 
    and set (v:System.Nullable<byte>)=
      if _VC_DJZQLXID<>v then
        _VC_DJZQLXID<-v
        x.OnPropertyChanged "VC_DJZQLXID"
      else x.IsDefaultVC_DJZQLXID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_DJZQLXIDSecond.HasValue && _VC_DJZQLXIDSecond.Value>=v.Value|>not then
            x.VC_DJZQLXIDSecond<-v
          x.IsCheckedVC_DJZQLXID<-new Nullable<_>(true)
          x.VisibilityVC_DJZQLXID<-Visibility.Visible
      | _ ->
          x.VC_DJZQLXIDSecond<-v 
          x.IsCheckedVC_DJZQLXID<-new Nullable<_>(false)
          x.VisibilityVC_DJZQLXID<-Visibility.Collapsed
  member x.VC_DJZQLXIDSecond 
    with get ()=_VC_DJZQLXIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_DJZQLXIDSecond<>v then
        _VC_DJZQLXIDSecond<-v
        x.OnPropertyChanged "VC_DJZQLXIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_DJZQLXID.HasValue |>not then
            x.VC_DJZQLXID<-v
          elif  v.Value>=_VC_DJZQLXID.Value |>not  then 
            _VC_DJZQLXIDSecond<-_VC_DJZQLXID
            x.OnPropertyChanged "VC_DJZQLXIDSecond"
      | _ ->()
  member x.VC_JBRID 
    with get ()=_VC_JBRID 
    and set v=
      if _VC_JBRID<>v then
        _VC_JBRID <-v
        x.OnPropertyChanged "VC_JBRID"
      else x.IsDefaultVC_JBRID<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsCheckedVC_JBRID<-new Nullable<_>(true)
          x.VisibilityVC_JBRID<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_JBRID<-new Nullable<_>(false)
          x.VisibilityVC_JBRID<-Visibility.Collapsed
  member x.VC_JBRJM 
    with get ()=_VC_JBRJM 
    and set v=
      if _VC_JBRJM<>v then
        _VC_JBRJM <-v
        x.OnPropertyChanged "VC_JBRJM"
      else x.IsDefaultVC_JBRJM<-false
      match v with
      | HasLength _ -> 
          x.IsCheckedVC_JBRJM<-new Nullable<_>(true)
          x.VisibilityVC_JBRJM<-Visibility.Visible
      | _ ->
          x.IsCheckedVC_JBRJM<-new Nullable<_>(false)
          x.VisibilityVC_JBRJM<-Visibility.Collapsed
  member x.VC_JBRXBH 
    with get ()=_VC_JBRXBH 
    and set (v:System.Nullable<decimal>)=
      if _VC_JBRXBH<>v then
        _VC_JBRXBH<-v
        x.OnPropertyChanged "VC_JBRXBH"
      else x.IsDefaultVC_JBRXBH<-false
      match v with
      | _ when v.HasValue ->
          if _VC_JBRXBHSecond.HasValue && _VC_JBRXBHSecond.Value>=v.Value|>not then
            x.VC_JBRXBHSecond<-v
          x.IsCheckedVC_JBRXBH<-new Nullable<_>(true)
          x.VisibilityVC_JBRXBH<-Visibility.Visible
      | _ ->
          x.VC_JBRXBHSecond<-v 
          x.IsCheckedVC_JBRXBH<-new Nullable<_>(false)
          x.VisibilityVC_JBRXBH<-Visibility.Collapsed
  member x.VC_JBRXBHSecond 
    with get ()=_VC_JBRXBHSecond 
    and set (v:System.Nullable<decimal>)=
      if _VC_JBRXBHSecond<>v then
        _VC_JBRXBHSecond<-v
        x.OnPropertyChanged "VC_JBRXBHSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_JBRXBH.HasValue |>not then
            x.VC_JBRXBH<-v
          elif  v.Value>=_VC_JBRXBH.Value |>not  then 
            _VC_JBRXBHSecond<-_VC_JBRXBH
            x.OnPropertyChanged "VC_JBRXBHSecond"
      | _ ->()
  member x.VC_TS 
    with get ()=_VC_TS 
    and set (v:System.Nullable<int>)=
      if _VC_TS<>v then
        _VC_TS<-v
        x.OnPropertyChanged "VC_TS"
      else x.IsDefaultVC_TS<-false
      match v with
      | _ when v.HasValue ->
          if _VC_TSSecond.HasValue && _VC_TSSecond.Value>=v.Value|>not then
            x.VC_TSSecond<-v
          x.IsCheckedVC_TS<-new Nullable<_>(true)
          x.VisibilityVC_TS<-Visibility.Visible
      | _ ->
          x.VC_TSSecond<-v 
          x.IsCheckedVC_TS<-new Nullable<_>(false)
          x.VisibilityVC_TS<-Visibility.Collapsed
  member x.VC_TSSecond 
    with get ()=_VC_TSSecond 
    and set (v:System.Nullable<int>)=
      if _VC_TSSecond<>v then
        _VC_TSSecond<-v
        x.OnPropertyChanged "VC_TSSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_TS.HasValue |>not then
            x.VC_TS<-v
          elif  v.Value>=_VC_TS.Value |>not  then 
            _VC_TSSecond<-_VC_TS
            x.OnPropertyChanged "VC_TSSecond"
      | _ ->()
  member x.VC_BID 
    with get ()=_VC_BID 
    and set (v:System.Nullable<byte>)=
      if _VC_BID<>v then
        _VC_BID<-v
        x.OnPropertyChanged "VC_BID"
      else x.IsDefaultVC_BID<-false
      match v with
      | _ when v.HasValue ->
          if _VC_BIDSecond.HasValue && _VC_BIDSecond.Value>=v.Value|>not then
            x.VC_BIDSecond<-v
          x.IsCheckedVC_BID<-new Nullable<_>(true)
          x.VisibilityVC_BID<-Visibility.Visible
      | _ ->
          x.VC_BIDSecond<-v 
          x.IsCheckedVC_BID<-new Nullable<_>(false)
          x.VisibilityVC_BID<-Visibility.Collapsed
  member x.VC_BIDSecond 
    with get ()=_VC_BIDSecond 
    and set (v:System.Nullable<byte>)=
      if _VC_BIDSecond<>v then
        _VC_BIDSecond<-v
        x.OnPropertyChanged "VC_BIDSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_BID.HasValue |>not then
            x.VC_BID<-v
          elif  v.Value>=_VC_BID.Value |>not  then 
            _VC_BIDSecond<-_VC_BID
            x.OnPropertyChanged "VC_BIDSecond"
      | _ ->()
  member x.VC_NF 
    with get ()=_VC_NF 
    and set (v:System.Nullable<int>)=
      if _VC_NF<>v then
        _VC_NF<-v
        x.OnPropertyChanged "VC_NF"
      else x.IsDefaultVC_NF<-false
      match v with
      | _ when v.HasValue ->
          if _VC_NFSecond.HasValue && _VC_NFSecond.Value>=v.Value|>not then
            x.VC_NFSecond<-v
          x.IsCheckedVC_NF<-new Nullable<_>(true)
          x.VisibilityVC_NF<-Visibility.Visible
      | _ ->
          x.VC_NFSecond<-v 
          x.IsCheckedVC_NF<-new Nullable<_>(false)
          x.VisibilityVC_NF<-Visibility.Collapsed
  member x.VC_NFSecond 
    with get ()=_VC_NFSecond 
    and set (v:System.Nullable<int>)=
      if _VC_NFSecond<>v then
        _VC_NFSecond<-v
        x.OnPropertyChanged "VC_NFSecond" 
      match v with
      | _ when v.HasValue->
          if _VC_NF.HasValue |>not then
            x.VC_NF<-v
          elif  v.Value>=_VC_NF.Value |>not  then 
            _VC_NFSecond<-_VC_NF
            x.OnPropertyChanged "VC_NFSecond"
      | _ ->()
  member x.IsCheckedVC_CJRQ 
    with get ()=_IsCheckedVC_CJRQ 
    and set v=
      if _IsCheckedVC_CJRQ<>v then
        _IsCheckedVC_CJRQ<-v
        x.OnPropertyChanged "IsCheckedVC_CJRQ"
      match _VC_CJRQ with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_CJRQ<>new Nullable<_>(false) then 
            _IsCheckedVC_CJRQ <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_CJRQ"
  member x.IsCheckedVC_GHSID 
    with get ()=_IsCheckedVC_GHSID 
    and set v=
      if _IsCheckedVC_GHSID<>v then
        _IsCheckedVC_GHSID<-v
        x.OnPropertyChanged "IsCheckedVC_GHSID"
      match _VC_GHSID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSID<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSID"
  member x.IsCheckedVC_GHS 
    with get ()=_IsCheckedVC_GHS 
    and set v=
      if _IsCheckedVC_GHS<>v then
        _IsCheckedVC_GHS<-v
        x.OnPropertyChanged "IsCheckedVC_GHS"
      match _VC_GHS with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_GHS <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHS"
  member x.IsCheckedVC_GHSJM 
    with get ()=_IsCheckedVC_GHSJM 
    and set v=
      if _IsCheckedVC_GHSJM<>v then
        _IsCheckedVC_GHSJM<-v
        x.OnPropertyChanged "IsCheckedVC_GHSJM"
      match _VC_GHSJM with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_GHSJM <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSJM"
  member x.IsCheckedVC_GHSXBH 
    with get ()=_IsCheckedVC_GHSXBH 
    and set v=
      if _IsCheckedVC_GHSXBH<>v then
        _IsCheckedVC_GHSXBH<-v
        x.OnPropertyChanged "IsCheckedVC_GHSXBH"
      match _VC_GHSXBH with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSXBH<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSXBH <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSXBH"
  member x.IsCheckedVC_GHSXZID 
    with get ()=_IsCheckedVC_GHSXZID 
    and set v=
      if _IsCheckedVC_GHSXZID<>v then
        _IsCheckedVC_GHSXZID<-v
        x.OnPropertyChanged "IsCheckedVC_GHSXZID"
      match _VC_GHSXZID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSXZID<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSXZID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSXZID"
  member x.IsCheckedVC_GHSLXID 
    with get ()=_IsCheckedVC_GHSLXID 
    and set v=
      if _IsCheckedVC_GHSLXID<>v then
        _IsCheckedVC_GHSLXID<-v
        x.OnPropertyChanged "IsCheckedVC_GHSLXID"
      match _VC_GHSLXID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSLXID<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSLXID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSLXID"
  member x.IsCheckedVC_GHSLBID 
    with get ()=_IsCheckedVC_GHSLBID 
    and set v=
      if _IsCheckedVC_GHSLBID<>v then
        _IsCheckedVC_GHSLBID<-v
        x.OnPropertyChanged "IsCheckedVC_GHSLBID"
      match _VC_GHSLBID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSLBID<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSLBID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSLBID"
  member x.IsCheckedVC_GHSDJID 
    with get ()=_IsCheckedVC_GHSDJID 
    and set v=
      if _IsCheckedVC_GHSDJID<>v then
        _IsCheckedVC_GHSDJID<-v
        x.OnPropertyChanged "IsCheckedVC_GHSDJID"
      match _VC_GHSDJID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSDJID<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSDJID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSDJID"
  member x.IsCheckedVC_GHSDQID 
    with get ()=_IsCheckedVC_GHSDQID 
    and set v=
      if _IsCheckedVC_GHSDQID<>v then
        _IsCheckedVC_GHSDQID<-v
        x.OnPropertyChanged "IsCheckedVC_GHSDQID"
      match _VC_GHSDQID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_GHSDQID<>new Nullable<_>(false) then 
            _IsCheckedVC_GHSDQID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_GHSDQID"
  member x.IsCheckedVC_SPID 
    with get ()=_IsCheckedVC_SPID 
    and set v=
      if _IsCheckedVC_SPID<>v then
        _IsCheckedVC_SPID<-v
        x.OnPropertyChanged "IsCheckedVC_SPID"
      match _VC_SPID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_SPID<>new Nullable<_>(false) then 
            _IsCheckedVC_SPID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPID"
  member x.IsCheckedVC_SPJM 
    with get ()=_IsCheckedVC_SPJM 
    and set v=
      if _IsCheckedVC_SPJM<>v then
        _IsCheckedVC_SPJM<-v
        x.OnPropertyChanged "IsCheckedVC_SPJM"
      match _VC_SPJM with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_SPJM <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPJM"
  member x.IsCheckedVC_SPXBH 
    with get ()=_IsCheckedVC_SPXBH 
    and set v=
      if _IsCheckedVC_SPXBH<>v then
        _IsCheckedVC_SPXBH<-v
        x.OnPropertyChanged "IsCheckedVC_SPXBH"
      match _VC_SPXBH with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_SPXBH<>new Nullable<_>(false) then 
            _IsCheckedVC_SPXBH <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPXBH"
  member x.IsCheckedVC_SPLBJM 
    with get ()=_IsCheckedVC_SPLBJM 
    and set v=
      if _IsCheckedVC_SPLBJM<>v then
        _IsCheckedVC_SPLBJM<-v
        x.OnPropertyChanged "IsCheckedVC_SPLBJM"
      match _VC_SPLBJM with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_SPLBJM <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPLBJM"
  member x.IsCheckedVC_SPGGXH 
    with get ()=_IsCheckedVC_SPGGXH 
    and set v=
      if _IsCheckedVC_SPGGXH<>v then
        _IsCheckedVC_SPGGXH<-v
        x.OnPropertyChanged "IsCheckedVC_SPGGXH"
      match _VC_SPGGXH with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_SPGGXH <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPGGXH"
  member x.IsCheckedVC_SPPC 
    with get ()=_IsCheckedVC_SPPC 
    and set v=
      if _IsCheckedVC_SPPC<>v then
        _IsCheckedVC_SPPC<-v
        x.OnPropertyChanged "IsCheckedVC_SPPC"
      match _VC_SPPC with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_SPPC<>new Nullable<_>(false) then 
            _IsCheckedVC_SPPC <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPPC"
  member x.IsCheckedVC_SPSCRQ 
    with get ()=_IsCheckedVC_SPSCRQ 
    and set v=
      if _IsCheckedVC_SPSCRQ<>v then
        _IsCheckedVC_SPSCRQ<-v
        x.OnPropertyChanged "IsCheckedVC_SPSCRQ"
      match _VC_SPSCRQ with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_SPSCRQ<>new Nullable<_>(false) then 
            _IsCheckedVC_SPSCRQ <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPSCRQ"
  member x.IsCheckedVC_SPLBID 
    with get ()=_IsCheckedVC_SPLBID 
    and set v=
      if _IsCheckedVC_SPLBID<>v then
        _IsCheckedVC_SPLBID<-v
        x.OnPropertyChanged "IsCheckedVC_SPLBID"
      match _VC_SPLBID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_SPLBID<>new Nullable<_>(false) then 
            _IsCheckedVC_SPLBID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPLBID"
  member x.IsCheckedVC_SPLBIDs 
    with get ()=_IsCheckedVC_SPLBIDs 
    and set v=
      if _IsCheckedVC_SPLBIDs<>v then
        _IsCheckedVC_SPLBIDs<-v
        x.OnPropertyChanged "IsCheckedVC_SPLBIDs"
      match _VC_SPLBIDs with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_SPLBIDs <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_SPLBIDs"
  member x.IsCheckedVC_CKID 
    with get ()=_IsCheckedVC_CKID 
    and set v=
      if _IsCheckedVC_CKID<>v then
        _IsCheckedVC_CKID<-v
        x.OnPropertyChanged "IsCheckedVC_CKID"
      match _VC_CKID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_CKID<>new Nullable<_>(false) then 
            _IsCheckedVC_CKID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_CKID"
  member x.IsCheckedVC_CKJM 
    with get ()=_IsCheckedVC_CKJM 
    and set v=
      if _IsCheckedVC_CKJM<>v then
        _IsCheckedVC_CKJM<-v
        x.OnPropertyChanged "IsCheckedVC_CKJM"
      match _VC_CKJM with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_CKJM <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_CKJM"
  member x.IsCheckedVC_CKXBH 
    with get ()=_IsCheckedVC_CKXBH 
    and set v=
      if _IsCheckedVC_CKXBH<>v then
        _IsCheckedVC_CKXBH<-v
        x.OnPropertyChanged "IsCheckedVC_CKXBH"
      match _VC_CKXBH with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_CKXBH<>new Nullable<_>(false) then 
            _IsCheckedVC_CKXBH <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_CKXBH"
  member x.IsCheckedVC_DJH 
    with get ()=_IsCheckedVC_DJH 
    and set v=
      if _IsCheckedVC_DJH<>v then
        _IsCheckedVC_DJH<-v
        x.OnPropertyChanged "IsCheckedVC_DJH"
      match _VC_DJH with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_DJH <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_DJH"
  member x.IsCheckedVC_DJLXID 
    with get ()=_IsCheckedVC_DJLXID 
    and set v=
      if _IsCheckedVC_DJLXID<>v then
        _IsCheckedVC_DJLXID<-v
        x.OnPropertyChanged "IsCheckedVC_DJLXID"
      match _VC_DJLXID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_DJLXID<>new Nullable<_>(false) then 
            _IsCheckedVC_DJLXID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_DJLXID"
  member x.IsCheckedVC_DJZQLXID 
    with get ()=_IsCheckedVC_DJZQLXID 
    and set v=
      if _IsCheckedVC_DJZQLXID<>v then
        _IsCheckedVC_DJZQLXID<-v
        x.OnPropertyChanged "IsCheckedVC_DJZQLXID"
      match _VC_DJZQLXID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_DJZQLXID<>new Nullable<_>(false) then 
            _IsCheckedVC_DJZQLXID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_DJZQLXID"
  member x.IsCheckedVC_JBRID 
    with get ()=_IsCheckedVC_JBRID 
    and set v=
      if _IsCheckedVC_JBRID<>v then
        _IsCheckedVC_JBRID<-v
        x.OnPropertyChanged "IsCheckedVC_JBRID"
      match _VC_JBRID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_JBRID<>new Nullable<_>(false) then 
            _IsCheckedVC_JBRID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_JBRID"
  member x.IsCheckedVC_JBRJM 
    with get ()=_IsCheckedVC_JBRJM 
    and set v=
      if _IsCheckedVC_JBRJM<>v then
        _IsCheckedVC_JBRJM<-v
        x.OnPropertyChanged "IsCheckedVC_JBRJM"
      match _VC_JBRJM with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsCheckedVC_JBRJM <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_JBRJM"
  member x.IsCheckedVC_JBRXBH 
    with get ()=_IsCheckedVC_JBRXBH 
    and set v=
      if _IsCheckedVC_JBRXBH<>v then
        _IsCheckedVC_JBRXBH<-v
        x.OnPropertyChanged "IsCheckedVC_JBRXBH"
      match _VC_JBRXBH with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_JBRXBH<>new Nullable<_>(false) then 
            _IsCheckedVC_JBRXBH <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_JBRXBH"
  member x.IsCheckedVC_TS 
    with get ()=_IsCheckedVC_TS 
    and set v=
      if _IsCheckedVC_TS<>v then
        _IsCheckedVC_TS<-v
        x.OnPropertyChanged "IsCheckedVC_TS"
      match _VC_TS with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_TS<>new Nullable<_>(false) then 
            _IsCheckedVC_TS <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_TS"
  member x.IsCheckedVC_BID 
    with get ()=_IsCheckedVC_BID 
    and set v=
      if _IsCheckedVC_BID<>v then
        _IsCheckedVC_BID<-v
        x.OnPropertyChanged "IsCheckedVC_BID"
      match _VC_BID with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_BID<>new Nullable<_>(false) then 
            _IsCheckedVC_BID <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_BID"
  member x.IsCheckedVC_NF 
    with get ()=_IsCheckedVC_NF 
    and set v=
      if _IsCheckedVC_NF<>v then
        _IsCheckedVC_NF<-v
        x.OnPropertyChanged "IsCheckedVC_NF"
      match _VC_NF with
      | y when y.HasValue ->()
      | _ ->
          if _IsCheckedVC_NF<>new Nullable<_>(false) then 
            _IsCheckedVC_NF <-new Nullable<_>(false)
            x.OnPropertyChanged "IsCheckedVC_NF"
  member x.VisibilityVC_CJRQ 
    with get ()=_VisibilityVC_CJRQ 
    and set v=
      if _VisibilityVC_CJRQ<>v then
        _VisibilityVC_CJRQ <-v 
        x.OnPropertyChanged "VisibilityVC_CJRQ"
  member x.VisibilityVC_GHSID 
    with get ()=_VisibilityVC_GHSID 
    and set v=
      if _VisibilityVC_GHSID<>v then
        _VisibilityVC_GHSID <-v 
        x.OnPropertyChanged "VisibilityVC_GHSID"
  member x.VisibilityVC_GHS 
    with get ()=_VisibilityVC_GHS 
    and set v=
      if _VisibilityVC_GHS<>v then
        _VisibilityVC_GHS <-v 
        x.OnPropertyChanged "VisibilityVC_GHS"
  member x.VisibilityVC_GHSJM 
    with get ()=_VisibilityVC_GHSJM 
    and set v=
      if _VisibilityVC_GHSJM<>v then
        _VisibilityVC_GHSJM <-v 
        x.OnPropertyChanged "VisibilityVC_GHSJM"
  member x.VisibilityVC_GHSXBH 
    with get ()=_VisibilityVC_GHSXBH 
    and set v=
      if _VisibilityVC_GHSXBH<>v then
        _VisibilityVC_GHSXBH <-v 
        x.OnPropertyChanged "VisibilityVC_GHSXBH"
  member x.VisibilityVC_GHSXZID 
    with get ()=_VisibilityVC_GHSXZID 
    and set v=
      if _VisibilityVC_GHSXZID<>v then
        _VisibilityVC_GHSXZID <-v 
        x.OnPropertyChanged "VisibilityVC_GHSXZID"
  member x.VisibilityVC_GHSLXID 
    with get ()=_VisibilityVC_GHSLXID 
    and set v=
      if _VisibilityVC_GHSLXID<>v then
        _VisibilityVC_GHSLXID <-v 
        x.OnPropertyChanged "VisibilityVC_GHSLXID"
  member x.VisibilityVC_GHSLBID 
    with get ()=_VisibilityVC_GHSLBID 
    and set v=
      if _VisibilityVC_GHSLBID<>v then
        _VisibilityVC_GHSLBID <-v 
        x.OnPropertyChanged "VisibilityVC_GHSLBID"
  member x.VisibilityVC_GHSDJID 
    with get ()=_VisibilityVC_GHSDJID 
    and set v=
      if _VisibilityVC_GHSDJID<>v then
        _VisibilityVC_GHSDJID <-v 
        x.OnPropertyChanged "VisibilityVC_GHSDJID"
  member x.VisibilityVC_GHSDQID 
    with get ()=_VisibilityVC_GHSDQID 
    and set v=
      if _VisibilityVC_GHSDQID<>v then
        _VisibilityVC_GHSDQID <-v 
        x.OnPropertyChanged "VisibilityVC_GHSDQID"
  member x.VisibilityVC_SPID 
    with get ()=_VisibilityVC_SPID 
    and set v=
      if _VisibilityVC_SPID<>v then
        _VisibilityVC_SPID <-v 
        x.OnPropertyChanged "VisibilityVC_SPID"
  member x.VisibilityVC_SPJM 
    with get ()=_VisibilityVC_SPJM 
    and set v=
      if _VisibilityVC_SPJM<>v then
        _VisibilityVC_SPJM <-v 
        x.OnPropertyChanged "VisibilityVC_SPJM"
  member x.VisibilityVC_SPXBH 
    with get ()=_VisibilityVC_SPXBH 
    and set v=
      if _VisibilityVC_SPXBH<>v then
        _VisibilityVC_SPXBH <-v 
        x.OnPropertyChanged "VisibilityVC_SPXBH"
  member x.VisibilityVC_SPLBJM 
    with get ()=_VisibilityVC_SPLBJM 
    and set v=
      if _VisibilityVC_SPLBJM<>v then
        _VisibilityVC_SPLBJM <-v 
        x.OnPropertyChanged "VisibilityVC_SPLBJM"
  member x.VisibilityVC_SPGGXH 
    with get ()=_VisibilityVC_SPGGXH 
    and set v=
      if _VisibilityVC_SPGGXH<>v then
        _VisibilityVC_SPGGXH <-v 
        x.OnPropertyChanged "VisibilityVC_SPGGXH"
  member x.VisibilityVC_SPPC 
    with get ()=_VisibilityVC_SPPC 
    and set v=
      if _VisibilityVC_SPPC<>v then
        _VisibilityVC_SPPC <-v 
        x.OnPropertyChanged "VisibilityVC_SPPC"
  member x.VisibilityVC_SPSCRQ 
    with get ()=_VisibilityVC_SPSCRQ 
    and set v=
      if _VisibilityVC_SPSCRQ<>v then
        _VisibilityVC_SPSCRQ <-v 
        x.OnPropertyChanged "VisibilityVC_SPSCRQ"
  member x.VisibilityVC_SPLBID 
    with get ()=_VisibilityVC_SPLBID 
    and set v=
      if _VisibilityVC_SPLBID<>v then
        _VisibilityVC_SPLBID <-v 
        x.OnPropertyChanged "VisibilityVC_SPLBID"
  member x.VisibilityVC_SPLBIDs 
    with get ()=_VisibilityVC_SPLBIDs 
    and set v=
      if _VisibilityVC_SPLBIDs<>v then
        _VisibilityVC_SPLBIDs <-v 
        x.OnPropertyChanged "VisibilityVC_SPLBIDs"
  member x.VisibilityVC_CKID 
    with get ()=_VisibilityVC_CKID 
    and set v=
      if _VisibilityVC_CKID<>v then
        _VisibilityVC_CKID <-v 
        x.OnPropertyChanged "VisibilityVC_CKID"
  member x.VisibilityVC_CKJM 
    with get ()=_VisibilityVC_CKJM 
    and set v=
      if _VisibilityVC_CKJM<>v then
        _VisibilityVC_CKJM <-v 
        x.OnPropertyChanged "VisibilityVC_CKJM"
  member x.VisibilityVC_CKXBH 
    with get ()=_VisibilityVC_CKXBH 
    and set v=
      if _VisibilityVC_CKXBH<>v then
        _VisibilityVC_CKXBH <-v 
        x.OnPropertyChanged "VisibilityVC_CKXBH"
  member x.VisibilityVC_DJH 
    with get ()=_VisibilityVC_DJH 
    and set v=
      if _VisibilityVC_DJH<>v then
        _VisibilityVC_DJH <-v 
        x.OnPropertyChanged "VisibilityVC_DJH"
  member x.VisibilityVC_DJLXID 
    with get ()=_VisibilityVC_DJLXID 
    and set v=
      if _VisibilityVC_DJLXID<>v then
        _VisibilityVC_DJLXID <-v 
        x.OnPropertyChanged "VisibilityVC_DJLXID"
  member x.VisibilityVC_DJZQLXID 
    with get ()=_VisibilityVC_DJZQLXID 
    and set v=
      if _VisibilityVC_DJZQLXID<>v then
        _VisibilityVC_DJZQLXID <-v 
        x.OnPropertyChanged "VisibilityVC_DJZQLXID"
  member x.VisibilityVC_JBRID 
    with get ()=_VisibilityVC_JBRID 
    and set v=
      if _VisibilityVC_JBRID<>v then
        _VisibilityVC_JBRID <-v 
        x.OnPropertyChanged "VisibilityVC_JBRID"
  member x.VisibilityVC_JBRJM 
    with get ()=_VisibilityVC_JBRJM 
    and set v=
      if _VisibilityVC_JBRJM<>v then
        _VisibilityVC_JBRJM <-v 
        x.OnPropertyChanged "VisibilityVC_JBRJM"
  member x.VisibilityVC_JBRXBH 
    with get ()=_VisibilityVC_JBRXBH 
    and set v=
      if _VisibilityVC_JBRXBH<>v then
        _VisibilityVC_JBRXBH <-v 
        x.OnPropertyChanged "VisibilityVC_JBRXBH"
  member x.VisibilityVC_TS 
    with get ()=_VisibilityVC_TS 
    and set v=
      if _VisibilityVC_TS<>v then
        _VisibilityVC_TS <-v 
        x.OnPropertyChanged "VisibilityVC_TS"
  member x.VisibilityVC_BID 
    with get ()=_VisibilityVC_BID 
    and set v=
      if _VisibilityVC_BID<>v then
        _VisibilityVC_BID <-v 
        x.OnPropertyChanged "VisibilityVC_BID"
  member x.VisibilityVC_NF 
    with get ()=_VisibilityVC_NF 
    and set v=
      if _VisibilityVC_NF<>v then
        _VisibilityVC_NF <-v 
        x.OnPropertyChanged "VisibilityVC_NF"
  member x.IsDefaultVC_CJRQ 
    with get ()=_IsDefaultVC_CJRQ 
    and set v=_IsDefaultVC_CJRQ <-v
  member x.IsDefaultVC_GHSID 
    with get ()=_IsDefaultVC_GHSID 
    and set v=_IsDefaultVC_GHSID <-v
  member x.IsDefaultVC_GHS 
    with get ()=_IsDefaultVC_GHS 
    and set v=_IsDefaultVC_GHS <-v
  member x.IsDefaultVC_GHSJM 
    with get ()=_IsDefaultVC_GHSJM 
    and set v=_IsDefaultVC_GHSJM <-v
  member x.IsDefaultVC_GHSXBH 
    with get ()=_IsDefaultVC_GHSXBH 
    and set v=_IsDefaultVC_GHSXBH <-v
  member x.IsDefaultVC_GHSXZID 
    with get ()=_IsDefaultVC_GHSXZID 
    and set v=_IsDefaultVC_GHSXZID <-v
  member x.IsDefaultVC_GHSLXID 
    with get ()=_IsDefaultVC_GHSLXID 
    and set v=_IsDefaultVC_GHSLXID <-v
  member x.IsDefaultVC_GHSLBID 
    with get ()=_IsDefaultVC_GHSLBID 
    and set v=_IsDefaultVC_GHSLBID <-v
  member x.IsDefaultVC_GHSDJID 
    with get ()=_IsDefaultVC_GHSDJID 
    and set v=_IsDefaultVC_GHSDJID <-v
  member x.IsDefaultVC_GHSDQID 
    with get ()=_IsDefaultVC_GHSDQID 
    and set v=_IsDefaultVC_GHSDQID <-v
  member x.IsDefaultVC_SPID 
    with get ()=_IsDefaultVC_SPID 
    and set v=_IsDefaultVC_SPID <-v
  member x.IsDefaultVC_SPJM 
    with get ()=_IsDefaultVC_SPJM 
    and set v=_IsDefaultVC_SPJM <-v
  member x.IsDefaultVC_SPXBH 
    with get ()=_IsDefaultVC_SPXBH 
    and set v=_IsDefaultVC_SPXBH <-v
  member x.IsDefaultVC_SPLBJM 
    with get ()=_IsDefaultVC_SPLBJM 
    and set v=_IsDefaultVC_SPLBJM <-v
  member x.IsDefaultVC_SPGGXH 
    with get ()=_IsDefaultVC_SPGGXH 
    and set v=_IsDefaultVC_SPGGXH <-v
  member x.IsDefaultVC_SPPC 
    with get ()=_IsDefaultVC_SPPC 
    and set v=_IsDefaultVC_SPPC <-v
  member x.IsDefaultVC_SPSCRQ 
    with get ()=_IsDefaultVC_SPSCRQ 
    and set v=_IsDefaultVC_SPSCRQ <-v
  member x.IsDefaultVC_SPLBID 
    with get ()=_IsDefaultVC_SPLBID 
    and set v=_IsDefaultVC_SPLBID <-v
  member x.IsDefaultVC_SPLBIDs 
    with get ()=_IsDefaultVC_SPLBIDs 
    and set v=_IsDefaultVC_SPLBIDs <-v
  member x.IsDefaultVC_CKID 
    with get ()=_IsDefaultVC_CKID 
    and set v=_IsDefaultVC_CKID <-v
  member x.IsDefaultVC_CKJM 
    with get ()=_IsDefaultVC_CKJM 
    and set v=_IsDefaultVC_CKJM <-v
  member x.IsDefaultVC_CKXBH 
    with get ()=_IsDefaultVC_CKXBH 
    and set v=_IsDefaultVC_CKXBH <-v
  member x.IsDefaultVC_DJH 
    with get ()=_IsDefaultVC_DJH 
    and set v=_IsDefaultVC_DJH <-v
  member x.IsDefaultVC_DJLXID 
    with get ()=_IsDefaultVC_DJLXID 
    and set v=_IsDefaultVC_DJLXID <-v
  member x.IsDefaultVC_DJZQLXID 
    with get ()=_IsDefaultVC_DJZQLXID 
    and set v=_IsDefaultVC_DJZQLXID <-v
  member x.IsDefaultVC_JBRID 
    with get ()=_IsDefaultVC_JBRID 
    and set v=_IsDefaultVC_JBRID <-v
  member x.IsDefaultVC_JBRJM 
    with get ()=_IsDefaultVC_JBRJM 
    and set v=_IsDefaultVC_JBRJM <-v
  member x.IsDefaultVC_JBRXBH 
    with get ()=_IsDefaultVC_JBRXBH 
    and set v=_IsDefaultVC_JBRXBH <-v
  member x.IsDefaultVC_TS 
    with get ()=_IsDefaultVC_TS 
    and set v=_IsDefaultVC_TS <-v
  member x.IsDefaultVC_BID 
    with get ()=_IsDefaultVC_BID 
    and set v=_IsDefaultVC_BID <-v
  member x.IsDefaultVC_NF 
    with get ()=_IsDefaultVC_NF 
    and set v=_IsDefaultVC_NF <-v

