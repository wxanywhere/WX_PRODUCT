namespace WX.Data.ViewModel
open System
open System.ComponentModel
open System.Collections.ObjectModel
open System.Collections
open System.Windows
open System.Windows.Input
open Microsoft.FSharp.Collections
open WX.Data
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ClientChannel

type FVM_TJBB_CGYCG_Advance() =
  inherit FVM_TJBB_CGYCG()
  let mutable _DJQueryInfo=new BQ_TJBB_CGYCG_Advance()
  let mutable _HZQueryInfo=new BQ_TJBB_CGYCG_Advance()
  let mutable _MXQueryInfo=new BQ_TJBB_CGYCG_Advance()

  //----------------------------------------------------------------------------
  [<DV>]val mutable private _TD_DJSPView:Generic.List<BD_V_TJBB_CGYCG_CGDJSP_Advance> //由于null的问题，单独使用字段不好 
  member private this.TD_DJSPView
    with get ()=
      if this._TD_DJSPView=null then
        this._TD_DJSPView<-new Generic.List<BD_V_TJBB_CGYCG_CGDJSP_Advance>()
      this._TD_DJSPView

  [<DV>]val mutable private _D_DJView:ObservableCollection<BD_V_TJBB_CGYCG_CGDJ_Advance>
  member  this.D_DJView
    with get ()=
      if this._D_DJView=null then
        this._D_DJView<-new ObservableCollection<BD_V_TJBB_CGYCG_CGDJ_Advance>()
      this._D_DJView

  [<DV>]val mutable private _D_DJSPView:ObservableCollection<BD_V_TJBB_CGYCG_CGDJSP_Advance>
  member  this.D_DJSPView
    with get ()=
      if this._D_DJSPView=null then
        this._D_DJSPView<-new ObservableCollection<BD_V_TJBB_CGYCG_CGDJSP_Advance>()
      this._D_DJSPView

  [<DV>]val mutable private _D_HZView:ObservableCollection<BD_V_TJBB_CGYCG_SPHZ_Advance>
  member  this.D_HZView
    with get ()=
      if this._D_HZView=null then
        this._D_HZView<-new ObservableCollection<BD_V_TJBB_CGYCG_SPHZ_Advance>()
      this._D_HZView

  [<DV>]val mutable private _D_MXView:ObservableCollection<BD_V_TJBB_CGYCG_SPMX_Advance>
  member  this.D_MXView
    with get ()=
      if this._D_MXView=null then
        this._D_MXView<-new ObservableCollection<BD_V_TJBB_CGYCG_SPMX_Advance>()
      this._D_MXView

  [<DV>]val mutable private _D_DJSelected:BD_V_TJBB_CGYCG_CGDJ_Advance
  member this.D_DJSelected
    with get()=this._D_DJSelected
    and set v=
      if this._D_DJSelected<>v then
        this._D_DJSelected<-v
        this.OnPropertyChanged "D_DJSelected"  
        if v<>Null() then
          this.TD_DJSPView.Clear()
          this.TD_DJSPView.AddRange v.BD_V_TJBB_CGYCG_CGDJSP_AdvanceView
          this.DJSPQuery()

  [<DV>]val mutable private _D_YGSelected:KeyValue<Guid,string>
  member this.D_YGSelected
    with get()=this._D_YGSelected
    and set v=
      if this._D_YGSelected<>v then
        this._D_YGSelected<-v
        this.OnPropertyChanged "D_YGSelected"  

//-------------------------------------------------------------------------------------------------

  [<DV>]val mutable private _D_Query2Entity:BQ_TJBB_CGYCG_Client_Advance
  member this.D_Query2Entity
    with get()=
      if this._D_Query2Entity=Null() then
        this._D_Query2Entity<-new BQ_TJBB_CGYCG_Client_Advance()
        match this._D_Query2Entity, new Nullable<_>(DateTime.Now.Date) with
        | x,v -> 
            x.VC_CJRQ<-v
            x.VC_CJRQSecond<-v 
            x.IsReturnQueryError<-Nullable<_> true 
      this._D_Query2Entity

  [<DV>]val mutable private _CMD_Query:ICommand
  member this.CMD_Query
    with get ()=
      if this._CMD_Query=null then
        this._CMD_Query<-new RelayCommand(fun _ ->
          this.UpdateEffect Query true
          match new AsyncWorker<_>(async{
            return 
              match _DJQueryInfo with
              | x ->
                  this.D_Query2Entity.BuildQueryEntity (x,true)|>ignore
                  let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                  client.GetTJBB_CGYCG_CGDJView x
              ,
              match _HZQueryInfo with
              | x ->
                  this.D_Query2Entity.BuildQueryEntity  (x,true)|>ignore
                  let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                  client.GetTJBB_CGYCG_SPHZView x
              ,
              match _MXQueryInfo with
              | x ->
                  this.D_Query2Entity.BuildQueryEntity (x,true)|>ignore
                  let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                  client.GetTJBB_CGYCG_SPMXView x})  with
          | wk ->
              wk.Completed.Add(fun (r1,r2,r3) ->
                match r1,r2,r3 with
                | HasError x, _, _ | _, HasError x,_ | _,_,HasError x -> this.UpdateResult Query x
                | HasResult xr1, HasResult xr2, HasResult xr3 ->
                    this.D_DJQueryStr<-String.Empty
                    this.D_DJSPQueryStr<-String.Empty
                    this.D_HZQueryStr<-String.Empty
                    this.D_MXQueryStr<-String.Empty
                    this.D_DJView.Clear()
                    this.TD_DJSPView.Clear()
                    this.D_DJSPView.Clear()
                    this.D_HZView.Clear()
                    this.D_MXView.Clear()
                    this.D_DJView.AddRange xr1
                    this.D_HZView.AddRange xr2
                    this.D_MXView.AddRange xr3
                    match this.D_DJPaging:?>FVM_Paging_Advance with
                    | vm ->
                        match r1 with
                        | HasPagingInfo y ->
                            vm.LoadData y
                        | _ ->
                            vm.LoadData _DJQueryInfo.PagingInfo 
                    match this.D_HZPaging:?>FVM_Paging_Advance with
                    | vm ->
                        match r2 with
                        | HasPagingInfo y ->
                            vm.LoadData y
                        | _ ->
                            vm.LoadData _HZQueryInfo.PagingInfo 
                    match this.D_MXPaging:?>FVM_Paging_Advance with
                    | vm ->
                        match r3 with
                        | HasPagingInfo y ->
                            vm.LoadData y
                        | _ ->
                            vm.LoadData _MXQueryInfo.PagingInfo 
                this.UpdateEffect Query false)
              wk.RunAsync() |>ignore
          )
      this._CMD_Query   

//-------------------------------------------------------------------------------------------------

  [<DV>]val mutable private _IsOpen_DJConditionTip:bool
   member this.IsOpen_DJConditionTip
     with get()=
       this._IsOpen_DJConditionTip
     and set v=
       if this._IsOpen_DJConditionTip<>v then
         this._IsOpen_DJConditionTip<-v
         base.OnPropertyChanged "IsOpen_DJConditionTip"

  [<DV>]val mutable private _IsChecked_DJCondition:bool
   member this.IsChecked_DJCondition
     with get()=
       this._IsChecked_DJCondition
     and set v=
       if this._IsChecked_DJCondition<>v then
         this._IsChecked_DJCondition<-v
         base.OnPropertyChanged "IsChecked_DJCondition"

  [<DV>]val mutable private _D_DJQueryEntity:BQ_TJBB_CGYCG_Client_Advance
  member this.D_DJQueryEntity
    with get()=
      if this._D_DJQueryEntity=Null() then
        this._D_DJQueryEntity<-new BQ_TJBB_CGYCG_Client_Advance()   
      this._D_DJQueryEntity 

  [<DV>]val mutable private _CMD_DJConditionOk:ICommand
  member this.CMD_DJConditionOk
    with get ()=
      if this._CMD_DJConditionOk=null then
        this._CMD_DJConditionOk<-new RelayCommand(fun _ ->
          this.IsChecked_DJCondition<-false
          this.D_DJQueryStr<-this.D_DJQueryEntity.GetQueryStr()
          this.D_DJQueryStr<-this.D_DJQueryEntity.GetQueryStr()  //执行2次可保证输入条件和实际查询条件匹配
          match this.D_DJQueryStr with
          | NotNullOrWhiteSpace _ -> this.IsOpen_DJConditionTip<-true
          | _ -> this.IsOpen_DJConditionTip<-false
          )
      this._CMD_DJConditionOk  

  [<DV>]val mutable private _D_DJQueryStr:string
  member this.D_DJQueryStr
    with get()=this._D_DJQueryStr
    and set v=
      if this._D_DJQueryStr<>v then
        this._D_DJQueryStr<-v
        this.OnPropertyChanged "D_DJQueryStr"
        if this.IsChecked_DJCondition|>not then this.IsOpen_DJConditionTip<-true 
        this.D_DJQueryEntity.ResetAll()
        match this.D_DJQueryEntity,Comm.decision v with
        | m, IsZS (_,_,(x,y),l,_) when l<4 ->     //查询条件不能和主查询条件重复
            m.VC_DJLXID<-x
        | m, IsZMKT (x,_,n) ->
            m.VC_CKJM<-x
            m.VC_GHSJM<-x  //最后一个为默认
            if Comm.isUsed m.IsDefaultVC_GHSJM 1 n then m.VC_GHSJM<-x
            elif Comm.isUsed m.IsDefaultVC_CKJM 2 n then m.VC_CKJM<-x
        | m, IsDJH (x,_,_)->
            m.VC_DJH<-x
        | m, _ ->
            m.ResetAll()
            this.IsOpen_DJConditionTip<-false

  [<DV>]val mutable private _CMD_DJQuery:ICommand
  member this.CMD_DJQuery
    with get ():ICommand=
      if this._CMD_DJQuery=null then
        this._CMD_DJQuery<-new RelayCommand(fun _ ->
          this.IsOpen_DJConditionTip<-false
          this.IsChecked_DJCondition<-false 
          this.UpdateEffect Query2 true
          match new AsyncWorker<_>(async{
            match _DJQueryInfo with
            | x ->
                if this.D_DJQueryEntity.BuildQueryEntity  (x,true)|>not then this.D_DJQueryStr<-String.Empty //没有条件，将查询所有条件，必要时可在此提示至少需要一个条件
                this.D_Query2Entity.BuildQueryEntity x|>ignore
                let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                return client.GetTJBB_CGYCG_CGDJView x})  with
          | wk ->
              wk.Completed.Add(fun r ->
                match r with
                | HasError x -> this.UpdateResult Query2 x
                | HasResult xr ->
                    this.TD_DJSPView.Clear()
                    this.D_DJSPView.Clear()
                    this.D_DJView.Clear()
                    this.D_DJView.AddRange xr
                    match this.D_DJPaging:?>FVM_Paging_Advance with
                    | vm ->
                        match r with
                        | HasPagingInfo y ->
                            vm.LoadData y
                        | _ ->
                            vm.LoadData _DJQueryInfo.PagingInfo 
                this.UpdateEffect Query2 false)
              wk.RunAsync() |>ignore
          )
      this._CMD_DJQuery   

//-------------------------------------------------------------------------------------------------

  [<DV>]val mutable private _IsOpen_DJSPConditionTip:bool
   member this.IsOpen_DJSPConditionTip
     with get()=
       this._IsOpen_DJSPConditionTip
     and set v=
       if this._IsOpen_DJSPConditionTip<>v then
         this._IsOpen_DJSPConditionTip<-v
         base.OnPropertyChanged "IsOpen_DJSPConditionTip"

  [<DV>]val mutable private _IsChecked_DJSPCondition:bool
   member this.IsChecked_DJSPCondition
     with get()=
       this._IsChecked_DJSPCondition
     and set v=
       if this._IsChecked_DJSPCondition<>v then
         this._IsChecked_DJSPCondition<-v
         base.OnPropertyChanged "IsChecked_DJSPCondition"

  [<DV>]val mutable private _D_DJSPQueryEntity:BQ_TJBB_CGYCG_Client_Advance
  member this.D_DJSPQueryEntity
    with get()=
      if this._D_DJSPQueryEntity=Null() then
        this._D_DJSPQueryEntity<-new BQ_TJBB_CGYCG_Client_Advance()   
      this._D_DJSPQueryEntity

  [<DV>]val mutable private _CMD_DJSPConditionOk:ICommand
  member this.CMD_DJSPConditionOk
    with get ()=
      if this._CMD_DJSPConditionOk=null then
        this._CMD_DJSPConditionOk<-new RelayCommand(fun _ ->
          this.IsChecked_DJSPCondition<-false
          this.D_DJSPQueryStr<-this.D_DJSPQueryEntity.GetQueryStr()
          this.D_DJSPQueryStr<-this.D_DJSPQueryEntity.GetQueryStr()  //执行2次可保证输入条件和实际查询条件匹配
          match this.D_DJSPQueryStr with
          | NotNullOrWhiteSpace _ -> this.IsOpen_DJSPConditionTip<-true
          | _ -> this.IsOpen_DJSPConditionTip<-false
          )
      this._CMD_DJSPConditionOk  

  [<DV>]val mutable private _D_DJSPQueryStr:string
  member this.D_DJSPQueryStr
    with get()=this._D_DJSPQueryStr
    and set v=
      if this._D_DJSPQueryStr<>v then
        this._D_DJSPQueryStr<-v
        this.OnPropertyChanged "D_DJSPQueryStr"
        if this.IsChecked_DJSPCondition|>not then this.IsOpen_DJSPConditionTip<-true 
        this.D_DJSPQueryEntity.ResetAll()
        match this.D_DJSPQueryEntity,Comm.decision v with
        | m, IsZMKT (x,_,_)->
            m.VC_SPJM<-x
        | m, IsZS ((x,y),_,_,l,_) when l=7->
            m.VC_SPXBHSecond<-y
            m.VC_SPXBH<-x
        | m, IsTDZFC (x,_,_) ->
            m.VC_SPGGXH<-x
        | m, _ ->
            m.ResetAll()
            this.IsOpen_DJSPConditionTip<-false
        if this.D_DJSelected<>Null() then this.DJSPQuery() 
        
  member this.DJSPQuery()=
    let Filter (queryEntity:BQ_TJBB_CGYCG_Advance) (source:seq<BD_V_TJBB_CGYCG_CGDJSP_Advance>)=
        source
        |>PSeq.filter (fun a->
            match a.VC_SPJM,queryEntity.VC_SPJM with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.VC_SPGGXH,queryEntity.VC_SPGGXH with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && 
            match a.VC_SPXBH,queryEntity.VC_SPXBH,queryEntity.VC_SPXBHSecond with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            )
    match new BQ_TJBB_CGYCG_Advance(),this.D_DJSPView with
    | x,y ->
        this.D_DJSPQueryEntity.BuildQueryEntity x|>ignore
        y.Clear()
        Filter x this.TD_DJSPView
        |>PSeq.sortBy (fun a->a.VC_SPXBH)   
        |>y.AddRange

  [<DV>]val mutable private _CMD_DJSPQuery:ICommand
  member this.CMD_DJSPQuery
    with get ()=
      if this._CMD_DJSPQuery=null then
        this._CMD_DJSPQuery<-new RelayCommand(fun _ ->
          this.IsOpen_DJSPConditionTip<-false
          this.IsChecked_DJSPCondition<-false 
          )
      this._CMD_DJSPQuery   

//-------------------------------------------------------------------------------------------------

  [<DV>]val mutable private _IsOpen_MXConditionTip:bool
   member this.IsOpen_MXConditionTip
     with get()=
       this._IsOpen_MXConditionTip
     and set v=
       if this._IsOpen_MXConditionTip<>v then
         this._IsOpen_MXConditionTip<-v
         base.OnPropertyChanged "IsOpen_MXConditionTip"

  [<DV>]val mutable private _IsChecked_MXCondition:bool
   member this.IsChecked_MXCondition
     with get()=
       this._IsChecked_MXCondition
     and set v=
       if this._IsChecked_MXCondition<>v then
         this._IsChecked_MXCondition<-v
         base.OnPropertyChanged "IsChecked_MXCondition"

  [<DV>]val mutable private _D_MXQueryEntity:BQ_TJBB_CGYCG_Client_Advance
  member this.D_MXQueryEntity
    with get()=
      if this._D_MXQueryEntity=Null() then
        this._D_MXQueryEntity<-new BQ_TJBB_CGYCG_Client_Advance()   
      this._D_MXQueryEntity 

  [<DV>]val mutable private _CMD_MXConditionOk:ICommand
  member this.CMD_MXConditionOk
    with get ()=
      if this._CMD_MXConditionOk=null then
        this._CMD_MXConditionOk<-new RelayCommand(fun _ ->
          this.IsChecked_MXCondition<-false
          this.D_MXQueryStr<-this.D_MXQueryEntity.GetQueryStr()
          this.D_MXQueryStr<-this.D_MXQueryEntity.GetQueryStr()  //执行2次可保证输入条件和实际查询条件匹配
          match this.D_MXQueryStr with
          | NotNullOrWhiteSpace _ -> this.IsOpen_MXConditionTip<-true
          | _ -> this.IsOpen_MXConditionTip<-false
          )
      this._CMD_MXConditionOk  

  [<DV>]val mutable private _D_MXQueryStr:string
  member this.D_MXQueryStr
    with get()=this._D_MXQueryStr
    and set v=
      if this._D_MXQueryStr<>v then
        this._D_MXQueryStr<-v
        this.OnPropertyChanged "D_MXQueryStr"
        if this.IsChecked_MXCondition|>not then this.IsOpen_MXConditionTip<-true 
        this.D_MXQueryEntity.ResetAll()
        match this.D_MXQueryEntity,Comm.decision v with
        | m, IsZMKT (x,_,n)->
            m.VC_GHSJM<-x
            m.VC_SPJM<-x
            if Comm.isUsed m.IsDefaultVC_SPJM 1 n then m.VC_SPJM<-x
            elif Comm.isUsed m.IsDefaultVC_GHSJM 2 n then m.VC_GHSJM<-x
        | m, IsZS ((x,y),_,_,l,_) when l=7->
            m.VC_SPXBHSecond<-y
            m.VC_SPXBH<-x
        | m, IsZS ((x,y),_,_,l,_) when l=13->
            m.VC_SPPCSecond<-y
            m.VC_SPPC<-x
        | m, IsTDZFC (x,_,_) ->
            m.VC_SPGGXH<-x
        | m, _ ->
            m.ResetAll()
            this.IsOpen_MXConditionTip<-false

  [<DV>]val mutable private _CMD_MXQuery:ICommand
  member this.CMD_MXQuery
    with get ():ICommand=
      if this._CMD_MXQuery=null then
        this._CMD_MXQuery<-new RelayCommand(fun _ ->
          this.IsOpen_MXConditionTip<-false
          this.IsChecked_MXCondition<-false 
          this.UpdateEffect Query2 true
          match new AsyncWorker<_>(async{
            match _MXQueryInfo with
            | x ->
                if this.D_MXQueryEntity.BuildQueryEntity  (x,true)|>not then this.D_MXQueryStr<-String.Empty //没有条件，将查询所有条件，必要时可在此提示至少需要一个条件
                this.D_Query2Entity.BuildQueryEntity x|>ignore
                let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                return client.GetTJBB_CGYCG_SPMXView x})  with
          | wk ->
              wk.Completed.Add(fun r ->
                match r with
                | HasError x -> this.UpdateResult Query2 x
                | HasResult xr ->
                    this.D_MXView.Clear()
                    this.D_MXView.AddRange xr
                    match this.D_MXPaging:?>FVM_Paging_Advance with
                    | vm ->
                        match r with
                        | HasPagingInfo y ->
                            vm.LoadData y
                        | _ ->
                            vm.LoadData _MXQueryInfo.PagingInfo 
                this.UpdateEffect Query2 false)
              wk.RunAsync() |>ignore
          )
      this._CMD_MXQuery   

//-------------------------------------------------------------------------------------------------

  [<DV>]val mutable private _IsOpen_HZConditionTip:bool
   member this.IsOpen_HZConditionTip
     with get()=
       this._IsOpen_HZConditionTip
     and set v=
       if this._IsOpen_HZConditionTip<>v then
         this._IsOpen_HZConditionTip<-v
         base.OnPropertyChanged "IsOpen_HZConditionTip"

  [<DV>]val mutable private _IsChecked_HZCondition:bool
   member this.IsChecked_HZCondition
     with get()=
       this._IsChecked_HZCondition
     and set v=
       if this._IsChecked_HZCondition<>v then
         this._IsChecked_HZCondition<-v
         base.OnPropertyChanged "IsChecked_HZCondition"

  [<DV>]val mutable private _D_HZQueryEntity:BQ_TJBB_CGYCG_Client_Advance
  member this.D_HZQueryEntity
    with get()=
      if this._D_HZQueryEntity=Null() then
        this._D_HZQueryEntity<-new BQ_TJBB_CGYCG_Client_Advance()   
      this._D_HZQueryEntity 

  [<DV>]val mutable private _CMD_HZConditionOk:ICommand
  member this.CMD_HZConditionOk
    with get ()=
      if this._CMD_HZConditionOk=null then
        this._CMD_HZConditionOk<-new RelayCommand(fun _ ->
          this.IsChecked_HZCondition<-false
          this.D_HZQueryStr<-this.D_HZQueryEntity.GetQueryStr()
          this.D_HZQueryStr<-this.D_HZQueryEntity.GetQueryStr()  //执行2次可保证输入条件和实际查询条件匹配
          match this.D_HZQueryStr with
          | NotNullOrWhiteSpace _ -> this.IsOpen_HZConditionTip<-true
          | _ -> this.IsOpen_HZConditionTip<-false
          )
      this._CMD_HZConditionOk  

  [<DV>]val mutable private _D_HZQueryStr:string
  member this.D_HZQueryStr
    with get()=this._D_HZQueryStr
    and set v=
      if this._D_HZQueryStr<>v then
        this._D_HZQueryStr<-v
        this.OnPropertyChanged "D_HZQueryStr"
        if this.IsChecked_HZCondition|>not then this.IsOpen_HZConditionTip<-true 
        this.D_HZQueryEntity.ResetAll()
        match this.D_HZQueryEntity,Comm.decision v with
        | m, IsZMKT (x,_,n)->
            m.VC_GHSJM<-x
            m.VC_SPJM<-x
            if Comm.isUsed m.IsDefaultVC_SPJM 1 n then m.VC_SPJM<-x
            elif Comm.isUsed m.IsDefaultVC_GHSJM 2 n then m.VC_GHSJM<-x
        | m, IsZS ((x,y),_,_,l,_) when l=7->
            m.VC_SPXBHSecond<-y
            m.VC_SPXBH<-x
        | m, IsTDZFC (x,_,_) ->
            m.VC_SPGGXH<-x
        | m, _ ->
            m.ResetAll()
            this.IsOpen_HZConditionTip<-false

  [<DV>]val mutable private _CMD_HZQuery:ICommand
  member this.CMD_HZQuery
    with get ():ICommand=
      if this._CMD_HZQuery=null then
        this._CMD_HZQuery<-new RelayCommand(fun _ ->
          this.IsOpen_HZConditionTip<-false
          this.IsChecked_HZCondition<-false 
          this.UpdateEffect Query2 true
          match new AsyncWorker<_>(async{
            match _HZQueryInfo with
            | x ->
                if this.D_HZQueryEntity.BuildQueryEntity  (x,true)|>not then this.D_HZQueryStr<-String.Empty //没有条件，将查询所有条件，必要时可在此提示至少需要一个条件
                this.D_Query2Entity.BuildQueryEntity x|>ignore
                let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                return client.GetTJBB_CGYCG_SPHZView x})  with
          | wk ->
              wk.Completed.Add(fun r ->
                match r with
                | HasError x -> this.UpdateResult Query2 x
                | HasResult xr ->
                    this.D_HZView.Clear()
                    this.D_HZView.AddRange xr
                    match this.D_HZPaging:?>FVM_Paging_Advance with
                    | vm ->
                        match r with
                        | HasPagingInfo y ->
                            vm.LoadData y
                        | _ ->
                            vm.LoadData _HZQueryInfo.PagingInfo 
                this.UpdateEffect Query2 false)
              wk.RunAsync() |>ignore
          )
      this._CMD_HZQuery   

//-------------------------------------------------------------------------------------------------

  override this.CMD_RQD 
    with get()=
      if this._CMD_RQD=null then
        this._CMD_RQD<-new RelayCommand(fun e->
          match e with
          | :? string as xe ->
              match this.RetrieveInstance xe with
              | Some vm ->
                  this.[ContextMenu]<-vm
                  vm.RequestClose(fun r->
                    match this.D_Query2Entity,r.Data with
                    | x, (:? (DateTime*DateTime) as y) ->
                        x.VC_CJRQ<-Nullable<_>(fst y)
                        x.VC_CJRQSecond<-Nullable<_>(snd y)
                    | _ ->())
              | _ ->()
          | _ ->())
      this._CMD_RQD

  override this.CMD_YG
    with get ()=
      if this._CMD_YG=null then
        this._CMD_YG<-new RelayCommand(fun e->
          match e with
          | :? string as xe ->
              match this.RetrieveInstance xe with
              | Some vm ->
                  this.[PopupSub]<-vm  
                  vm.LoadSpecific ((GuidDefaultValue,true, YWY,GuidString)) 
                  vm.RequestClose(fun r-> 
                    match r.Data, this.D_Query2Entity with
                    | :? (Guid*string) as x, y  ->
                        match x with
                        | xa, xb ->
                            this.D_YGSelected<-new KeyValue<_,_> (xa,xb)
                            y.VC_JBRID<- Nullable<_> xa
                    | IsFalse, y ->
                        this.D_YGSelected<-Null()
                        y.VC_JBRID<- Nullable<_> ()
                    | _ ->())
              | _ ->()
          | _ ->())
      this._CMD_YG 

  override this.CMD_DJLX
    with get ()=
      if this._CMD_DJLX=null then
        this._CMD_DJLX<-new RelayCommand(fun e->
          match e with
          | :? string as xe ->
              match this.RetrieveInstance xe with
              | Some vm ->
                  this.[PopupSub]<-vm  
                  vm.LoadSpecific JHGL
                  vm.RequestClose(fun r-> 
                    match r.Data with
                    | :? (byte*string) as y->
                        this.D_DJQueryEntity.VC_DJLXID <-Nullable<_> (fst y)
                        this.CMD_DJConditionOk.Execute()
                    | _ ->())
              | _ ->()
          | _ ->())
      this._CMD_DJLX 

//-------------------------------------------------------------------------------------------------

  override this.D_DJPaging
    with get()=
      if this._D_DJPaging=Null()  then
        match new FVM_Paging_Advance () with
        | vm ->
            this._D_DJPaging<-vm
            vm.PagingChanged.Add(fun e->
              match _DJQueryInfo with
              | x ->
                  x.PagingInfo<-e.PagingInfo 
                  let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                  client.GetTJBB_CGYCG_CGDJView x
              |>fun r->
                  match r with
                  | HasError x ->vm.PagingResult<-x
                  | HasResult xr ->
                      vm.ResultFlag <-true
                      this.TD_DJSPView.Clear()
                      this.D_DJSPView.Clear()
                      this.D_DJView.Clear()
                      this.D_DJView.AddRange xr
              )
      this._D_DJPaging

  override this.D_HZPaging
    with get()=
      if this._D_HZPaging=Null()  then
        match new FVM_Paging_Advance () with
        | vm ->
            this._D_HZPaging<-vm
            vm.PagingChanged.Add(fun e->
              match _HZQueryInfo with
              | x ->
                  x.PagingInfo<-e.PagingInfo 
                  let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                  client.GetTJBB_CGYCG_SPHZView x
              |>fun r->
                  match r with
                  | HasError x ->vm.PagingResult<-x
                  | HasResult xr ->
                      vm.ResultFlag <-true
                      this.D_HZView.Clear()
                      this.D_HZView.AddRange xr
              )
      this._D_HZPaging

  override this.D_MXPaging
    with get()=
      if this._D_MXPaging=Null()  then
        match new FVM_Paging_Advance () with
        | vm ->
            this._D_MXPaging<-vm
            vm.PagingChanged.Add(fun e->
              match _MXQueryInfo with
              | x ->
                  x.PagingInfo<-e.PagingInfo 
                  let client=new WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
                  client.GetTJBB_CGYCG_SPMXView x
              |>fun r->
                  match r with
                  | HasError x ->vm.PagingResult<-x
                  | HasResult xr ->
                      vm.ResultFlag <-true
                      this.D_MXView.Clear()
                      this.D_MXView.AddRange xr
              )
      this._D_MXPaging




