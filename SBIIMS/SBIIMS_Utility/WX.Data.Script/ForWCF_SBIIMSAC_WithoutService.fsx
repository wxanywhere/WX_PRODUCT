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
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.BusinessEntities.AccessControl.dll"
#r "WX.Data.ServiceContracts.AccessControl.dll"
#r "WX.Data.WcfService.AccessControl.dll"

open WX.Data.BusinessEntities
open WX.Data.WcfService
open WX.Data.ServiceContracts
open WX.Data.Helper
open WX.Data.FHelper

///////////////////////////////////////////////////////////////////////////////////////////////////

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings
//ConfigurationManager.GetSection "system.serviceModel/services"

let service=WS_SBIIMSAC():>IWS_SBIIMSAC
let queryEntity=BQ_AC_CZY()
let result=service.GetAC_CZYs(queryEntity)
ObjectDumper.Write result


//////////////////////////////////////////////////////////////////////////////////////////////////

let host=new ServiceHost(typeof<WS_SBIIMSAC>)
host.Open()
ObjectDumper.Write(host.Description.Endpoints,2)
Console.WriteLine("Press <ENTER> to terminate the host application")
Console.ReadLine() |> ignore
host.Close()

//////////////////////////////////////////////////////////////////////////////////////////////////
