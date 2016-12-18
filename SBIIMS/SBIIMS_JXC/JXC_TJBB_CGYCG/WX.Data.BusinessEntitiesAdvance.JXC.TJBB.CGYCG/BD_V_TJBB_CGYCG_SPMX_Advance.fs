namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open System.ComponentModel
open WX.Data
open WX.Data.BusinessBase

//统计-采购员采购-商品明细
[<DataContract>]
type BD_V_TJBB_CGYCG_SPMX_Advance()=
  inherit BD_ViewBase()

(* Template
[
"VC_DJH","string",false,"单据号"
"VC_CJRQ","DateTime",false,"开单日期"
"VC_DJLX","string",false,"单据类型"
"VC_GHS","string",false,"供货商"
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPPC","decimal",false,"商品批次"
"VC_SPSCRQ","DateTime",false,"商品生产日期"
"VC_SPDW","string",false,"商品单位"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPYS","string",false,"商品颜色"
"VC_SPDJ","decimal",false,"商品单价"
"VC_SPSL","decimal",false,"商品数量"
"VC_SPZJE","decimal",false,"商品总金额"
"VC_CK","string",false,"仓库"
"VC_JBR","string",false,"经办人"
]
*)
  //-------------------------------------------------

  [<DV>]
  val mutable private _VC_DJH:string
  [<DataMember>]
  member x.VC_DJH 
    with get ()=x._VC_DJH 
    and set v=
      if  x._VC_DJH<>v  then
        x._VC_DJH <- v
        x.OnPropertyChanged "VC_DJH"

  [<DV>]
  val mutable private _VC_CJRQ:DateTime
  [<DataMember>]
  member x.VC_CJRQ 
    with get ()=x._VC_CJRQ 
    and set v=
      if  x._VC_CJRQ<>v  then
        x._VC_CJRQ <- v
        x.OnPropertyChanged "VC_CJRQ"

  [<DV>]
  val mutable private _VC_DJLX:string
  [<DataMember>]
  member x.VC_DJLX 
    with get ()=x._VC_DJLX 
    and set v=
      if  x._VC_DJLX<>v  then
        x._VC_DJLX <- v
        x.OnPropertyChanged "VC_DJLX"

  [<DV>]
  val mutable private _VC_GHS:string
  [<DataMember>]
  member x.VC_GHS 
    with get ()=x._VC_GHS 
    and set v=
      if  x._VC_GHS<>v  then
        x._VC_GHS <- v
        x.OnPropertyChanged "VC_GHS"

  [<DV>]
  val mutable private _VC_SPXBH:decimal
  [<DataMember>]
  member x.VC_SPXBH 
    with get ()=x._VC_SPXBH 
    and set v=
      if  x._VC_SPXBH<>v  then
        x._VC_SPXBH <- v
        x.OnPropertyChanged "VC_SPXBH"

  [<DV>]
  val mutable private _VC_SP:string
  [<DataMember>]
  member x.VC_SP 
    with get ()=x._VC_SP 
    and set v=
      if  x._VC_SP<>v  then
        x._VC_SP <- v
        x.OnPropertyChanged "VC_SP"

  [<DV>]
  val mutable private _VC_SPPC:decimal
  [<DataMember>]
  member x.VC_SPPC 
    with get ()=x._VC_SPPC 
    and set v=
      if  x._VC_SPPC<>v  then
        x._VC_SPPC <- v
        x.OnPropertyChanged "VC_SPPC"

  [<DV>]
  val mutable private _VC_SPSCRQ:DateTime
  [<DataMember>]
  member x.VC_SPSCRQ 
    with get ()=x._VC_SPSCRQ 
    and set v=
      if  x._VC_SPSCRQ<>v  then
        x._VC_SPSCRQ <- v
        x.OnPropertyChanged "VC_SPSCRQ"

  [<DV>]
  val mutable private _VC_SPDW:string
  [<DataMember>]
  member x.VC_SPDW 
    with get ()=x._VC_SPDW 
    and set v=
      if  x._VC_SPDW<>v  then
        x._VC_SPDW <- v
        x.OnPropertyChanged "VC_SPDW"

  [<DV>]
  val mutable private _VC_SPGGXH:string
  [<DataMember>]
  member x.VC_SPGGXH 
    with get ()=x._VC_SPGGXH 
    and set v=
      if  x._VC_SPGGXH<>v  then
        x._VC_SPGGXH <- v
        x.OnPropertyChanged "VC_SPGGXH"

  [<DV>]
  val mutable private _VC_SPYS:string
  [<DataMember>]
  member x.VC_SPYS 
    with get ()=x._VC_SPYS 
    and set v=
      if  x._VC_SPYS<>v  then
        x._VC_SPYS <- v
        x.OnPropertyChanged "VC_SPYS"

  [<DV>]
  val mutable private _VC_SPDJ:decimal
  [<DataMember>]
  member x.VC_SPDJ 
    with get ()=x._VC_SPDJ 
    and set v=
      if  x._VC_SPDJ<>v  then
        x._VC_SPDJ <- v
        x.OnPropertyChanged "VC_SPDJ"

  [<DV>]
  val mutable private _VC_SPSL:decimal
  [<DataMember>]
  member x.VC_SPSL 
    with get ()=x._VC_SPSL 
    and set v=
      if  x._VC_SPSL<>v  then
        x._VC_SPSL <- v
        x.OnPropertyChanged "VC_SPSL"

  [<DV>]
  val mutable private _VC_SPZJE:decimal
  [<DataMember>]
  member x.VC_SPZJE 
    with get ()=x._VC_SPZJE 
    and set v=
      if  x._VC_SPZJE<>v  then
        x._VC_SPZJE <- v
        x.OnPropertyChanged "VC_SPZJE"

  [<DV>]
  val mutable private _VC_CK:string
  [<DataMember>]
  member x.VC_CK 
    with get ()=x._VC_CK 
    and set v=
      if  x._VC_CK<>v  then
        x._VC_CK <- v
        x.OnPropertyChanged "VC_CK"

  [<DV>]
  val mutable private _VC_JBR:string
  [<DataMember>]
  member x.VC_JBR 
    with get ()=x._VC_JBR 
    and set v=
      if  x._VC_JBR<>v  then
        x._VC_JBR <- v
        x.OnPropertyChanged "VC_JBR"


