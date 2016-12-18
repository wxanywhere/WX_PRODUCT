namespace WX.Data.ServiceContracts
open System
open System.ServiceModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities

[<ServiceContract>]
type IWS_SBIIMS_JXC_TJBB_CGYCG_Advance =
  //==================================================================
  //TJBB_CGYCG_QueryAdvance
  [<OperationContract>] abstract GetTJBB_CGYCG_CGDJView: queryEntity:BQ_TJBB_CGYCG_Advance -> BD_QueryResult<BD_V_TJBB_CGYCG_CGDJ_Advance[]>
  [<OperationContract>] abstract GetTJBB_CGYCG_SPHZView: queryEntity:BQ_TJBB_CGYCG_Advance -> BD_QueryResult<BD_V_TJBB_CGYCG_SPHZ_Advance[]>
  [<OperationContract>] abstract GetTJBB_CGYCG_SPMXView: queryEntity:BQ_TJBB_CGYCG_Advance -> BD_QueryResult<BD_V_TJBB_CGYCG_SPMX_Advance[]>

