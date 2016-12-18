namespace WX.Data.ClientChannel
open System
open Microsoft.FSharp.Collections
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic

type WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel() =
  static member public INS= WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
  //==================================================================
  //TJBB_CGYCG_QueryAdvance
  member this.GetTJBB_CGYCG_CGDJView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_CGDJView queryEntity

  member this.GetTJBB_CGYCG_SPHZView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_SPHZView queryEntity

  member this.GetTJBB_CGYCG_SPMXView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_SPMXView queryEntity


