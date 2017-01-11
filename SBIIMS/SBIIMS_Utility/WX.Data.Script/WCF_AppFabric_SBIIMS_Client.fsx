

#I @"C:\Windows\assembly\GAC_MSIL\Microsoft.ServiceBus\1.0.0.0__31bf3856ad364e35"
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
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.BusinessDataEntities.JXC.dll"
#r "WX.Data.BusinessDataEntitiesAdvance.JXC.dll"
#r "WX.Data.BusinessQueryEntities.JXC.dll"
#r "WX.Data.BusinessQueryEntitiesAdvance.JXC.dll"
#r "WX.Data.BusinessQueryEntitiesClient.JXC.dll"
#r "WX.Data.BusinessQueryEntitiesClientAdvance.JXC.dll"
#r "WX.Data.ServiceContracts.JXC"
#r "WX.Data.ServiceContractsAdvance.JXC"

open WX.Data.FHelper
open WX.Data.Helper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts

ConfigHelper.INS.LoadDefaultClientConfigToManager

//=============================================================

//AppFabric ServiceBus
let serviceNamespaceDomain = ConfigurationManager.AppSettings.["ServiceNamespaceDomain"]
let serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespaceDomain, "WS_SBIIMS_JHGL_Advance")
let  channelFactory = new ChannelFactory<IWS_SBIIMS_JHGL_Advance>("Azure_WS_SBIIMS_JHGL_Advance", new EndpointAddress(serviceUri))
let client = channelFactory.CreateChannel()
//let channel=client:?>ICommunicationObject
let channel=client:?>IClientChannel
channel.Open()
let queryEntity=new BQ_DJ_GHS_Advance()
let result=client.GetDJ_GHSView queryEntity
result|>ObjectDumper.Write 
channel.Close()

//=============================================================

//let  factory = new ChannelFactory<IWS_SBIIMS_JBXX_Advance>("WSHttpBinding_IWS_SBIIMS_JBXX_Advance")
let  factory = new ChannelFactory<IWS_SBIIMS_JBXX_Advance>("WSHttpBinding_IWS_SBIIMS_JBXX_Advance")
let client=factory.CreateChannel()
let channel=client:?>IClientChannel
channel.Open()
let queryEntity01=new BQ_GHS_Advance()
let result01=client.GetGHSView  queryEntity01
ObjectDumper.Write result01


//=============================================================
let binding = new WSHttpBinding( );
let address = new EndpointAddress("http://localhost:8080/WS_SBIIMS_JBXX_Advance");
let channel01=ChannelFactory<IWS_SBIIMS_JBXX_Advance>.CreateChannel(binding,address)
let channel02=channel01:?>IClientChannel 
channel02.Open()
let queryEntity01=new BQ_GHS_Advance()
let result01=channel01.GetGHSView  queryEntity01
ObjectDumper.Write result01

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