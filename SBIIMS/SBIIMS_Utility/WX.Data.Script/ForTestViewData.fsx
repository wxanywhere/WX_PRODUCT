#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Threading.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "Microsoft.Practices.ObjectBuilder2.dll"
#r "System.Runtime.Serialization.dll"

open System
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.Practices.ObjectBuilder2
//let projectPath= @"D:\Workspace\SBIIMS"

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\WX.Data.Helper\bin\Debug"
#r "WX.Data.Helper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FHelper\bin\Debug"
#r "WX.Data.FHelper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FModule\bin\Debug"
#r "WX.Data.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.DataModel\bin\Debug"
#r "WX.Data.DataModel.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\bin\Debug"
#r "WX.Data.BusinessEntities.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.IDataAccess\bin\Debug"
#r "WX.Data.IDataAccess.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.DataAccess\bin\Debug"
#r "WX.Data.DataAccess.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.CodeAutomation\bin\Debug"
#r "WX.Data.CodeAutomation.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.IDataAccess
open WX.Data.DataAccess
open WX.Data.DataOperate
open WX.Data.PSeq
open WX.Data
open WX.Data.CodeAutomation
open WX.Data.BusinessEntities

ConfigHelper.INS.LoadDefaultServiceConfigToManager

///////////////////////////////////////////////////////////////////


let queryEntity=BQ_DJ_GHS()
queryEntity.C_CCK.HasValue