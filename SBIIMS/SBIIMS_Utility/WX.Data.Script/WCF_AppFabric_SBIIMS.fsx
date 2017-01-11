
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
#r "Microsoft.ServiceBus.dll"

open System
open Microsoft.FSharp.Text
open System.ServiceModel
open System.Configuration
open Microsoft.ServiceBus


//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.BusinessDataEntities.JXC.dll"
#r "WX.Data.BusinessQueryEntities.JXC.dll"
#r "WX.Data.ServiceContracts.JXC"
#r "WX.Data.WcfService.JXC.dll"
#r "WX.Data.ServiceContractsAdvance.JXC"
#r "WX.Data.WcfServiceAdvance.JXC.dll"

open WX.Data.FHelper
open WX.Data.Helper
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts
open WX.Data.WcfService

ConfigHelper.INS.LoadDefaultServiceConfigToManager

//=============================================================

let serviceNamespaceDomain = ConfigurationManager.AppSettings.["ServiceNamespaceDomain"]
let address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespaceDomain, "WS_SBIIMS_JHGL_Advance")
let host = new ServiceHost(typeof<WS_SBIIMS_JHGL_Advance>, address)
// create the ServiceRegistrySettings behavior for the endpoint
let serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public)
// add the Service Bus credentials to all endpoints specified in configuration
for endpoint in host.Description.Endpoints do
  endpoint.Behaviors.Add(serviceRegistrySettings)
host.Open()

//=============================================================

let host=new  ServiceHost(typeof<WS_SBIIMS_JBXX_Advance>)
host.Open()

(*
            Console.Title = "Service";

            // retrieve service namespace domain from the configuration file
            string serviceNamespaceDomain = ConfigurationManager.AppSettings["serviceNamespaceDomain"];

            // create the service URI based on the service namespace
            Uri address = ServiceBusEnvironment.CreateServiceUri("sb", serviceNamespaceDomain, "EchoService");

            // create the service host reading the configuration
            ServiceHost host = new ServiceHost(typeof(EchoService), address);

            // create the ServiceRegistrySettings behavior for the endpoint
            IEndpointBehavior serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Public);

            // add the Service Bus credentials to all endpoints specified in configuration
            foreach (ServiceEndpoint endpoint in host.Description.Endpoints)
            {
                endpoint.Behaviors.Add(serviceRegistrySettings);
            }

            // open the service
            host.Open();

            foreach (ChannelDispatcherBase channelDispatcherBase in host.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                {
                    Console.WriteLine("Listening at: {0}", endpointDispatcher.EndpointAddress);
                }
            }

            Console.WriteLine("Service address: " + address);
            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            // close the service
            host.Close();
*)