namespace WX.Data.IDataAccess
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
type IDA_TJBB_CGYCG_QueryAdvance=
  abstract GetTJBB_CGYCG_CGDJView:BQ_TJBB_CGYCG_Advance -> BD_QueryResult<BD_V_TJBB_CGYCG_CGDJ_Advance[]>
  abstract GetTJBB_CGYCG_SPHZView:BQ_TJBB_CGYCG_Advance -> BD_QueryResult<BD_V_TJBB_CGYCG_SPHZ_Advance[]>
  abstract GetTJBB_CGYCG_SPMXView:BQ_TJBB_CGYCG_Advance -> BD_QueryResult<BD_V_TJBB_CGYCG_SPMX_Advance[]>
