namespace WX.Data.WcfService
open System
open System.ServiceModel
open System.Runtime.Serialization
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

[<ServiceBehavior(Name="WX.Data.WcfService.WS_SBIIMS_JXC_TJBB_CGYCG_Advance",InstanceContextMode=InstanceContextMode.Single) >]
type WS_SBIIMS_JXC_TJBB_CGYCG_Advance() =
  interface IWS_SBIIMS_JXC_TJBB_CGYCG_Advance with
    //==================================================================
    //TJBB_CGYCG_QueryAdvance
    member this.GetTJBB_CGYCG_CGDJView (queryEntity:BQ_TJBB_CGYCG_Advance)= 
      BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_CGDJView queryEntity

    member this.GetTJBB_CGYCG_SPHZView (queryEntity:BQ_TJBB_CGYCG_Advance)= 
      BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_SPHZView queryEntity

    member this.GetTJBB_CGYCG_SPMXView (queryEntity:BQ_TJBB_CGYCG_Advance)= 
      BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_SPMXView queryEntity


