﻿namespace WX.Data.ClientChannel
open System
open System.ServiceModel
open Microsoft.FSharp.Collections
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts

type WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel() =
  let _EndpointName="WSHttpBinding_IWS_SBIIMS_JXC_TJBB_CGYCG_Advance"
  let channelFactory = new ChannelFactory<IWS_SBIIMS_JXC_TJBB_CGYCG_Advance>(_EndpointName)
  let client = channelFactory.CreateChannel()
  static member public INS= WS_SBIIMS_JXC_TJBB_CGYCG_AdvanceChannel()
  //==================================================================
  //TJBB_CGYCG_QueryAdvance
  member this.GetTJBB_CGYCG_CGDJView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.GetTJBB_CGYCG_CGDJView queryEntity

  member this.GetTJBB_CGYCG_SPHZView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.GetTJBB_CGYCG_SPHZView queryEntity

  member this.GetTJBB_CGYCG_SPMXView (queryEntity:BQ_TJBB_CGYCG_Advance)=
    use channel=client:?>IClientChannel  
    channel.Open()
    client.GetTJBB_CGYCG_SPMXView queryEntity


