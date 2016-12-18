namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open System.ComponentModel
open WX.Data
open WX.Data.BusinessBase

//统计-采购员采购-商品汇总
[<DataContract>]
type BD_V_TJBB_CGYCG_SPHZ_Advance()=
  inherit BD_ViewBase()

(* Template
[
"VC_SPXBH","decimal",false,"商品编号"
"VC_SP","string",false,"商品名称"
"VC_SPYS","string",false,"商品颜色"
"VC_SPGGXH","string",false,"商品规格型号"
"VC_SPDW","string",false,"商品单位"
"VC_SPSCCS","string",false,"商品生产厂商"
"VC_GHS","string",false,"供货商"
"VC_CGSL","decimal",false,"采购数量"
"VC_CGJE","decimal",false,"采购金额"
"VC_CGTHSL","decimal",false,"采购退货数量"
"VC_CGTHJE","decimal",false,"采购退货金额"
"VC_CGHJSL","decimal",false,"采购合计数量"
"VC_CGHJJE","decimal",false,"采购合计金额"
"VC_DQKCSL","decimal",false,"商品当前库存数量"
]
*)
  //-------------------------------------------------

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
  val mutable private _VC_SPYS:string
  [<DataMember>]
  member x.VC_SPYS 
    with get ()=x._VC_SPYS 
    and set v=
      if  x._VC_SPYS<>v  then
        x._VC_SPYS <- v
        x.OnPropertyChanged "VC_SPYS"

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
  val mutable private _VC_SPDW:string
  [<DataMember>]
  member x.VC_SPDW 
    with get ()=x._VC_SPDW 
    and set v=
      if  x._VC_SPDW<>v  then
        x._VC_SPDW <- v
        x.OnPropertyChanged "VC_SPDW"

  [<DV>]
  val mutable private _VC_SPSCCS:string
  [<DataMember>]
  member x.VC_SPSCCS 
    with get ()=x._VC_SPSCCS 
    and set v=
      if  x._VC_SPSCCS<>v  then
        x._VC_SPSCCS <- v
        x.OnPropertyChanged "VC_SPSCCS"

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
  val mutable private _VC_CGSL:decimal
  [<DataMember>]
  member x.VC_CGSL 
    with get ()=x._VC_CGSL 
    and set v=
      if  x._VC_CGSL<>v  then
        x._VC_CGSL <- v
        x.OnPropertyChanged "VC_CGSL"

  [<DV>]
  val mutable private _VC_CGJE:decimal
  [<DataMember>]
  member x.VC_CGJE 
    with get ()=x._VC_CGJE 
    and set v=
      if  x._VC_CGJE<>v  then
        x._VC_CGJE <- v
        x.OnPropertyChanged "VC_CGJE"

  [<DV>]
  val mutable private _VC_CGTHSL:decimal
  [<DataMember>]
  member x.VC_CGTHSL 
    with get ()=x._VC_CGTHSL 
    and set v=
      if  x._VC_CGTHSL<>v  then
        x._VC_CGTHSL <- v
        x.OnPropertyChanged "VC_CGTHSL"

  [<DV>]
  val mutable private _VC_CGTHJE:decimal
  [<DataMember>]
  member x.VC_CGTHJE 
    with get ()=x._VC_CGTHJE 
    and set v=
      if  x._VC_CGTHJE<>v  then
        x._VC_CGTHJE <- v
        x.OnPropertyChanged "VC_CGTHJE"

  [<DV>]
  val mutable private _VC_CGHJSL:decimal
  [<DataMember>]
  member x.VC_CGHJSL 
    with get ()=x._VC_CGHJSL 
    and set v=
      if  x._VC_CGHJSL<>v  then
        x._VC_CGHJSL <- v
        x.OnPropertyChanged "VC_CGHJSL"

  [<DV>]
  val mutable private _VC_CGHJJE:decimal
  [<DataMember>]
  member x.VC_CGHJJE 
    with get ()=x._VC_CGHJJE 
    and set v=
      if  x._VC_CGHJJE<>v  then
        x._VC_CGHJJE <- v
        x.OnPropertyChanged "VC_CGHJJE"

  [<DV>]
  val mutable private _VC_DQKCSL:decimal
  [<DataMember>]
  member x.VC_DQKCSL 
    with get ()=x._VC_DQKCSL 
    and set v=
      if  x._VC_DQKCSL<>v  then
        x._VC_DQKCSL <- v
        x.OnPropertyChanged "VC_DQKCSL"


