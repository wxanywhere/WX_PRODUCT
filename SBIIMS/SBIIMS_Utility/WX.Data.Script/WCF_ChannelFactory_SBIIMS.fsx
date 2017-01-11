

#r "System.Configuration.dll"
#r "System.ServiceModel.dll"
open System.ServiceModel
//---------------------------------------------------------------------
//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.dll"
#r "WX.Data.Helper.dll"
open WX.Data
open WX.Data.Helper
//---------------------------------------------------------------------
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.FHelper.dll"
open WX.Data.FHelper
//---------------------------------------------------------------------
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
(*
#r "WX.Data.ServiceContractsAdvance.JXC.JHGL.CGJH.dll"
#r "WX.Data.WcfServiceAdvance.JXC.JHGL.CGJH.dll"
*)
#r "WX.Data.ServiceContractsAdvance.JXC.GGHC.dll"
#r "WX.Data.WcfServiceAdvance.JXC.GGHC.dll"
open WX.Data.WcfService

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//SBIIMS_JXCEntities 须使用搜索路径配置, metadata=.\SBIIMS_JXC.csdl|.\SBIIMS_JXC.ssdl|.\SBIIMS_JXC.msl
//=============================================================

//在普通用户模式下，需要在Administrator下注册URL http://+:8080/..., http://go.microsoft.com/fwlink/?LinkId=70353
//应单独使用Fsi运行，最好在Administrator下运行
//***********该服务启动后，可在该服务所在的项目下用Visual studio跟踪到服务端进行调试，注意如果服务实例和客户端实例不在同一VS工程中，很可能就不能进入服务实例进行调试
let host=new  ServiceHost(typeof<WS_SBIIMS_JXC_GGHC_FWDHC_Advance>)
host.Open()
(*
let host=new  ServiceHost(typeof<WS_SBIIMS_JXC_JHGL_CGJH_Advance>)
host.Open()
*)

(*
在Administrator下注册
C:\Windows\system32>netsh http add urlacl url=http://+:8080/WS_SBIIMS_JXC_JHGL_CGJH_Advance user=zhoutao
*)

//=============================================================
(* 
if 
HTTP could not register URL http://+:8080/BusinessEntityService/. Your process does not have access rights to this namespace (see http://go.microsoft.com/fwlink/?LinkId=70353 for details).
then 
CMD run as Administrator
C:\Windows\system32>netsh http add urlacl url=http://+:8080/BusinessEntityService user=zhoutao(since it's not in a Domain)
*)
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

(*Right Backup
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

open System
open Microsoft.FSharp.Text
open System.ServiceModel
open System.Configuration

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
#r "WX.Data.WcfServiceAdvance.JXC.JHGL.CGJH.dll"

open WX.Data.Helper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ServiceContracts
open WX.Data.ServiceContracts
open WX.Data.WcfService


#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.FHelper.dll"
open WX.Data.FHelper

ConfigHelper.INS.LoadDefaultServiceConfigToManager

//=============================================================

//在普通用户模式下，需要在Administrator下注册URL http://+:8080/..., http://go.microsoft.com/fwlink/?LinkId=70353
let host=new  ServiceHost(typeof<WS_SBIIMS_JXC_JHGL_CGJH_Advance>)
host.Open()
*)