#r "System.dll"
#r "System.Runtime.Serialization.dll"
#r "System.ServiceModel.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"

open System
open Microsoft.FSharp.Text
open System.ServiceModel
open System.Configuration


//It must load on sequence
#I @"D:\Workspace\SBIIMS\WX.Data.Helper\bin\Debug"
#r "WX.Data.Helper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FHelper\bin\Debug"
#r "WX.Data.FHelper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities.AccessControl\bin\Debug"
#r "WX.Data.BusinessEntities.AccessControl.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.ServiceContracts.AccessControl\bin\Debug"
#r "WX.Data.ServiceContracts.AccessControl.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.WcfService.AccessControl\bin\Debug"
#r "WX.Data.WcfService.AccessControl.dll"

open WX.Data.WcfService
open WX.Data.ServiceContracts
open WX.Data.Helper
open WX.Data.FHelper

///////////////////////////////////////////////////////////////////////////////////////////////////

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings
//ConfigurationManager.GetSection "system.serviceModel/services"

let host=new ServiceHost(typeof<WS_SBIIMSAC>)
host.Open()
ObjectDumper.Write(host.Description.Endpoints,2)
Console.WriteLine("Press <ENTER> to terminate the host application")
Console.ReadLine() |> ignore
host.Close()

//////////////////////////////////////////////////////////////////////////////////////////////////
