namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.Runtime.Serialization
open System.ServiceModel
open Microsoft.FSharp.Collections
//open Microsoft.ServiceBus
open WX.Data
open WX.Data.ClientHelper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

type WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel() =
  let _EndpointName =
    match ClientChannel with
    | FromServer -> "WSHttpBinding_IWS_SBIIMS_JXC_TJBB_CGYCG_Advance"
    | FromAzure -> "Azure_WS_SBIIMS_JXC_TJBB_CGYCG_Advance" 
    | _ ->null
  let serviceNamespaceDomain = 
    match ClientChannel with
    | FromAzure -> ConfigurationManager.AppSettings.["ServiceNamespaceDomain"] 
    | _ ->null
  let serviceUri: Uri =
    match ClientChannel with
    | FromAzure -> Null() //ServiceBus不能使用FrameworkClientProfile, ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespaceDomain, "WS_SBIIMS_JXC_TJBB_CGYCG_Advance")
    | _ -> Null() 
  let channelFactory: ChannelFactory<IWS_SBIIMS_JXC_TJBB_CGYCG_Advance> =
    match ClientChannel with
    | FromServer -> new ChannelFactory<IWS_SBIIMS_JXC_TJBB_CGYCG_Advance>(_EndpointName)
    | FromAzure -> new ChannelFactory<IWS_SBIIMS_JXC_TJBB_CGYCG_Advance>(_EndpointName, new EndpointAddress(serviceUri))
    | _ -> Null()
  let client: IWS_SBIIMS_JXC_TJBB_CGYCG_Advance = 
    match ClientChannel with
    | FromServer
    | FromAzure ->channelFactory.CreateChannel()  
    | _ -> Null() 
  static member public INS= WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
  //==================================================================
  //TJBB_CGYCG_QueryAdvance
  member this.GetTJBB_CGYCG_CGDJView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.GetTJBB_CGYCG_CGDJView queryEntity
    | _ ->
        BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_CGDJView queryEntity

  member this.GetTJBB_CGYCG_SPHZView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.GetTJBB_CGYCG_SPHZView queryEntity
    | _ ->
        BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_SPHZView queryEntity

  member this.GetTJBB_CGYCG_SPMXView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.GetTJBB_CGYCG_SPMXView queryEntity
    | _ ->
        BL_TJBB_CGYCG_Advance.INS.GetTJBB_CGYCG_SPMXView queryEntity


