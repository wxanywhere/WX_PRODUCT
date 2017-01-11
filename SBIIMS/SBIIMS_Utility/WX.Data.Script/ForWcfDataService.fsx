//It must load on sequence
#I @"D:\Workspace\SBIIMS\WX.Data.Helper\bin\Debug"
#r "WX.Data.Helper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FHelper\bin\Debug"
#r "WX.Data.FHelper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.DataModel\bin\Debug"
#r "WX.Data.DataModel.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.ServiceContracts\bin\Debug"
#r "WX.Data.ServiceContracts.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.WcfService\bin\Debug"
#r "WX.Data.WcfService.dll"

#r "System.dll"
#r "System.ServiceModel.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.ServiceModel.Web.dll"
#r "System.Data.Services.Client.dll"

open System
open Microsoft.FSharp.Text
open System.ServiceModel
open System.Configuration
open System.Data.Services.Client
open System.ServiceModel.Web

open WX.Data.DataModel
open WX.Data.WcfService
open WX.Data.Helper
open WX.Data.FHelper

///////////////////////////////////////////////////////////////////////////////////////////////////

ConfigHelper.INS.LoadDefaultServiceConfigToManager
ConfigurationManager.ConnectionStrings
ConfigurationManager.GetSection "system.serviceModel/services"

let host=new WebServiceHost(typeof<DS_SBIIMS>)
host.Open()
ObjectDumper.Write(host.Description.Endpoints,2)
Console.WriteLine("Press <ENTER> to terminate the host application")
Console.ReadLine() |> ignore
host.Close()


//////////////////////////////////////////////////////////////

(*

//执行时要使用两个fsi的Session，一个fsi实例作为服务端，另外一个fsi实例作为客户端， 即服务端Session和客户端Session不能使用同一个fsiSession
//Open Config for ADO.NET Data Servies
//ConfigHelper.INS.EntryConfig
let host=new CustomWebServiceHost(typeof<SBIIMSDataService>)
host.Open()
let host001=new CustomServiceHost(typeof<BusinessEntityService>)
host001.Open()
//ObjectDumper.Write(host.Description.Endpoints,2)
//Don't copy below code lines to fsi console, or it will not work
Console.WriteLine("Press <ENTER> to terminate the host application")
Console.ReadLine() |> ignore
host.Close()

*)

