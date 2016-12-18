namespace WX.Data.BusinessLogic
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.DataAccess

[<Sealed>]
type BL_TJBB_CGYCG_Advance = 
  static member public INS = BL_TJBB_CGYCG_Advance()
  private new() = {}
  //==================================================================
  //TJBB_CGYCG_QueryAdvance
  member this.GetTJBB_CGYCG_CGDJView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    (DA_TJBB_CGYCG_QueryAdvance.INS:>IDA_TJBB_CGYCG_QueryAdvance).GetTJBB_CGYCG_CGDJView queryEntity

  member this.GetTJBB_CGYCG_SPHZView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    (DA_TJBB_CGYCG_QueryAdvance.INS:>IDA_TJBB_CGYCG_QueryAdvance).GetTJBB_CGYCG_SPHZView queryEntity

  member this.GetTJBB_CGYCG_SPMXView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    (DA_TJBB_CGYCG_QueryAdvance.INS:>IDA_TJBB_CGYCG_QueryAdvance).GetTJBB_CGYCG_SPMXView queryEntity


