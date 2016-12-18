namespace WX.Data.ViewModel
open System.Windows
open System.Windows.Input
open WX.Data

type FVM_TJBB_CGYCG() as this=
  inherit WorkspaceViewModel()

  [<DV>]
  val  mutable  _CMD_RQD:ICommand
  abstract CMD_RQD:ICommand with get
  default this.CMD_RQD 
    with get()=
      if this._CMD_RQD=null then
        this._CMD_RQD<-new RelayCommand(fun e->
          match e with
          | :? string as xe ->
              match this.RetrieveInstance xe with
              | Some vm ->
                  this.[ContextMenu]<-vm
                  vm.RequestClose (fun _ ->())
              | _ ->()
          | _ ->())
      this._CMD_RQD

  //-----------------------------------------------------------------------------------------


  [<DV>]
  val mutable _CMD_YG:ICommand
  abstract CMD_YG:ICommand with get
  default this.CMD_YG
    with get ()=
      if this._CMD_YG=null then
        this._CMD_YG<-new RelayCommand(fun e->
          match e with
          | :? string as xe -> 
              match this.RetrieveInstance xe with
              | Some vm ->
                  this.[PopupSub]<-vm
                  vm.RequestClose (fun _ ->())
              | _ ->()
          | _ ->())
      this._CMD_YG   

  [<DV>]
  val mutable _CMD_DJLX:ICommand
  abstract CMD_DJLX:ICommand with get
  default this.CMD_DJLX
    with get ()=
      if this._CMD_DJLX=null then
        this._CMD_DJLX<-new RelayCommand(fun e->
          match e with
          | :? string as xe ->
              match this.RetrieveInstance xe with
              | Some vm ->
                  this.[PopupSub]<-vm
                  vm.RequestClose (fun _ ->())
              | _ ->()
          | _ ->())     
      this._CMD_DJLX   

  //-----------------------------------------------------------------------------------------
  [<DV>]
  val  mutable _D_DJPaging:PagingViewModel
  abstract D_DJPaging:PagingViewModel with get 
  default this.D_DJPaging
    with get()=
      if this._D_DJPaging=Null()  then
        match new FVM_Paging () with
        | vm ->
            this._D_DJPaging<-vm
      this._D_DJPaging

  [<DV>]
  val  mutable _D_HZPaging:PagingViewModel
  abstract D_HZPaging:PagingViewModel with get 
  default this.D_HZPaging
    with get()=
      if this._D_HZPaging=Null()  then
        match new FVM_Paging () with
        | vm ->
            this._D_HZPaging<-vm
      this._D_HZPaging

  [<DV>]
  val  mutable _D_MXPaging:PagingViewModel
  abstract D_MXPaging:PagingViewModel with get 
  default this.D_MXPaging
    with get()=
      if this._D_MXPaging=Null()  then
        match new FVM_Paging () with
        | vm ->
            this._D_MXPaging<-vm
      this._D_MXPaging

  //-----------------------------------------------------------------------------------------


  do 
    this.DisplayName<-"采购员采购统计"
//-----------------------------------------------------------------------