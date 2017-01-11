

#r "System.ServiceModel.dll"
#r "System.Configuration.dll"
#r "Microsoft.ServiceBus.dll"

open System
open System.ServiceModel
open Microsoft.ServiceBus

//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.dll"
#r "WX.Data.ClientHelper.dll"
#r "WX.Data.Helper.dll"
#r "WX.Data.CModule.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.BusinessBaseClient.dll"
#r "WX.Data.BusinessQueryEntities.JXC.dll"
#r "WX.Data.BusinessDataEntities.JXC.dll"

#r "WX.Data.BusinessEntitiesAdvance.JXC.JHGL.CGJH.dll"
#r "WX.Data.ServiceContractsAdvance.JXC.JHGL.CGJH.dll"
#r "WX.Data.ClientChannel.JXC.dll"
#r "WX.Data.ClientChannelAdvance.JXC.JHGL.CGJH.dll"

#r "WX.Data.BusinessEntitiesAdvance.JXC.GGHC.FWDHC.dll"
#r "WX.Data.BusinessEntitiesAdvance.JXC.GGHC.dll"
#r "WX.Data.ServiceContractsAdvance.JXC.GGHC.dll"
#r "WX.Data.ClientChannel.JXC.dll"
#r "WX.Data.ClientChannelAdvance.JXC.GGHC.dll"

open WX.Data.Helper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts
open WX.Data.ClientChannel

#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.FHelper.dll"
open WX.Data.FHelper

ConfigHelper.INS.LoadDefaultClientConfigToManager

//=============================================================


let channelFactory = new ChannelFactory<IWS_SBIIMS_JXC_GGHC_FWDHC_Advance>("WSHttpBinding_IWS_SBIIMS_JXC_GGHC_FWDHC_Advance")
let client =channelFactory.CreateChannel()  
let channel=client:?>IClientChannel  
channel.Open()
let x= new BQ_GGHC_Advance()
x.IsReturnQueryError<-Nullable<_> true
let result=client.GetGGHC_FWDHC_KCSPPC_SPView x
ObjectDumper.Write result


result.[0].BD_Result.Message
channel.Close()



let channelFactory = new ChannelFactory<IWS_SBIIMS_JXC_JHGL_CGJH_Advance>("WSHttpBinding_IWS_SBIIMS_JXC_JHGL_CGJH_Advance")
let client =channelFactory.CreateChannel()  
let channel=client:?>IClientChannel  
channel.Open()
let x= new BQ_JHGL_CGJH_Advance()
x.IsReturnQueryError<-Nullable<_> true
let result=client.GetJHGL_CGJH_SHDJTableView x
ObjectDumper.Write result


result.[0].BD_Result.Message
channel.Close()



//=============================================================


//=============================================================


//==============================================================
(*
            Console.Title = "Client";

            // retrieve service namespace domain from the configuration file
            string serviceNamespaceDomain = ConfigurationManager.AppSettings["serviceNamespaceDomain"];

            // create the service URI based on the service namespace
            Uri serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespaceDomain, "EchoService");

            // create the channel factory loading the configuration
            ChannelFactory<IEchoContract> channelFactory = new ChannelFactory<IEchoContract>("RelayEndpoint", new EndpointAddress(serviceUri));

            // create and open the client channel
            IEchoContract channel = channelFactory.CreateChannel();
            ((ICommunicationObject)channel).Open();

            IHybridConnectionStatus hybridConnectionStatus =
                            ((System.ServiceModel.Channels.IChannel)channel).GetProperty<IHybridConnectionStatus>();
            if (hybridConnectionStatus != null)
            {
                hybridConnectionStatus.ConnectionStateChanged += (o, e) =>
                {
                    Console.WriteLine("Connection state changed to: {0}.", e.ConnectionState);
                };
            }

            Console.WriteLine("Enter text to echo (or [Enter] to exit):");
            string input = Console.ReadLine();
            while (input != String.Empty)
            {
                try
                {
                    Console.WriteLine("Server echoed: {0}", channel.Echo(input));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }
                input = Console.ReadLine();
            }

            ((ICommunicationObject)channel).Close();
            channelFactory.Close();
*)

(*
//Right reference
#r "System.dll"
#r "System.Core.dll"
#r "System.Data.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Data.DataSetExtensions.dll"
#r "System.ServiceModel.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Xml.dll"
#r "System.Xml.Linq.dll"
#r "FSharp.Core.dll"

#r "Microsoft.ServiceBus.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"

open System
open Microsoft.FSharp.Text
open System.ServiceModel
open System.Configuration
open System.ServiceModel
open System.Reflection
open Microsoft.ServiceBus
open Microsoft.FSharp.Core

//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.dll"
#r "WX.Data.ClientHelper.dll"
#r "WX.Data.Helper.dll"
#r "WX.Data.CModule.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.BusinessBaseClient.dll"
#r "WX.Data.BusinessQueryEntities.JXC.dll"
#r "WX.Data.BusinessDataEntities.JXC.dll"
#r "WX.Data.BusinessEntitiesAdvance.JXC.JHGL.CGJH.dll"
#r "WX.Data.ServiceContractsAdvance.JXC.JHGL.CGJH.dll"
#r "WX.Data.ClientChannel.JXC.dll"
#r "WX.Data.ClientChannelAdvance.JXC.JHGL.CGJH.dll"


open WX.Data.Helper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts
open WX.Data.ClientChannel

#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.FHelper.dll"
open WX.Data.FHelper

ConfigHelper.INS.LoadDefaultClientConfigToManager

//=============================================================

let channelFactory = new ChannelFactory<IWS_SBIIMS_JXC_JHGL_CGJH_Advance>("WSHttpBinding_IWS_SBIIMS_JXC_JHGL_CGJH_Advance")
let client =channelFactory.CreateChannel()  
let channel=client:?>IClientChannel  
channel.Open()
let x= new BQ_JHGL_CGJH_Advance()
x.IsReturnQueryError<-Nullable<_> true
let result=client.GetJHGL_CGJH_SHDJTableView x
ObjectDumper.Write result


result.[0].BD_Result.Message
channel.Close()
*)