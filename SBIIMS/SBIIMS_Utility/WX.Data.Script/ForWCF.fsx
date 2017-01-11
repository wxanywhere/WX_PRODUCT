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
#I @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\bin\Debug"
#r "WX.Data.BusinessEntities.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.ServiceContracts\bin\Debug"
#r "WX.Data.ServiceContracts.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.WcfService\bin\Debug"
#r "WX.Data.WcfService.dll"

open WX.Data.WcfService
open WX.Data.ServiceContracts
open WX.Data.Helper
open WX.Data.FHelper

///////////////////////////////////////////////////////////////////////////////////////////////////

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings
//ConfigurationManager.GetSection "system.serviceModel/services"

//let host=new ServiceHost(typeof<WS_YWST>)
let host=new ServiceHost(typeof<WS_SBIIMS>)
host.Open()
ObjectDumper.Write(host.Description.Endpoints,2)
Console.WriteLine("Press <ENTER> to terminate the host application")
Console.ReadLine() |> ignore
host.Close()

//////////////////////////////////////////////////////////////////////////////////////////////////

(* 
if 
HTTP could not register URL http://+:8080/BusinessEntityService/. Your process does not have access rights to this namespace (see http://go.microsoft.com/fwlink/?LinkId=70353 for details).
then 
CMD run as Administrator
C:\Windows\system32>netsh http add urlacl url=http://+:8080/BusinessEntityService user=zhoutao(since it's not in a Domain)
*)