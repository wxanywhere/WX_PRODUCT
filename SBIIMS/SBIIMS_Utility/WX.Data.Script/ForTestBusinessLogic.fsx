//#I  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Threading.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"

open System
open System.Collections.Generic
open System.Linq
open System.Data
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Validation
open Microsoft.Practices.EnterpriseLibrary.Logging

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
#I @"D:\Workspace\SBIIMS\WX.Data.BusinessLogic\bin\Debug"
#r "WX.Data.BusinessLogic.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.IDataAccess
open WX.Data.DataAccess
open WX.Data.DataOperate
open WX.Data.PSeq
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic

///////////////////////////////////////////////////////////////////////////////////////////////////

ConfigHelper.INS.LoadDefaultServiceConfigToManager

///////////////////////////////////////////////////////////////////////////////////////////////////

let queryEntity=BQ_CK()
BL_CK.INS.GetCKs queryEntity

let x="   "
let queryEntity=BQ_YG()
if x<>null && x.Trim()<>String.Empty then
  queryEntity.C_BZ<- x
let result=BL_YG.INS.GetYGs queryEntity
ObjectDumper.Write result.Count

let x=System.Nullable<Guid>()
x