//#I  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"
#r "System.dll"
#r "System.Core.dll"
//#r "System.Threading.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"


open System
open System.Collections.Generic
open System.Linq
open System.Data
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Validation
open Microsoft.Practices.EnterpriseLibrary.Logging
open Microsoft.FSharp.Collections


//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.CModule.dll"
#r "WX.Data.BusinessDataEntities.JXC.dll"
#r "WX.Data.CViewModelBase.dll"
#r "WX.Data.CViewModelBase.dll"
#r "WX.Data.FViewModel.JXC.JBXX.GHSWH.dll"
#r "WX.Data.FViewModelAdvance.JXC.JBXX.GHSWH.dll"
#r "PresentationCore.dll"
#r "WX.Data.ClientHelper.dll"
#r "WX.Data.CClientHelper.dll"
#r "WX.Data.BusinessEntitiesAdvance.JXC.JBXX.GHSWH.dll"
#r "WX.Data.BusinessEntitiesAdvance.JXC.GGHC.dll"

open WX.Data.Helper
open WX.Data.DataOperate
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.ViewModel

///////////////////////////////////////////////////////////////////////////////////////////////////

//ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings //不能使用该语句，有时会将加载到内存的配置信息重置？？？
//ConfigurationManager.ConnectionStrings.["SBIIMSEntities"] 

///////////////////////////////////////////////////////////////////////////////////////////////////

let x01=new BD_T_SP() 
x01.GetType().GetProperty("PropertyDictionary").DeclaringType=typeof<BD_Base>
x01.GetType()=typeof<BD_Base>

let x02=new FVM_JBXX_GHSWH_Add_Advance() 

x02.GetType().GetProperty("D_GHSAdd").PropertyType.IsSubclassOf(typeof<BD_Base>)