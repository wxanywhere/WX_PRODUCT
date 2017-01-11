#r "System.dll"
#r "System.Runtime.Serialization.dll"
#r "System.ServiceModel.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"

//They must import by orderly
//Or it will be like this "--> Referenced 'D:\Workspace\SBIIMS\WX.Data.WcfService\bin\Debug\WX.Data.Helper.dll'"
#I @"D:\Workspace\SBIIMS\WX.Data.Helper\bin\Debug"
#r "WX.Data.Helper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FHelper\bin\Debug"
#r "WX.Data.FHelper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.DataModel\bin\Debug"
#r "WX.Data.DataModel.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\bin\Debug"
#r "WX.Data.BusinessEntities.dll"
//#I @"D:\Workspace\SBIIMS\WX.Data.BusinessEntitiesCS\bin\Debug"
//#r "WX.Data.BusinessEntitiesCS.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.ServiceProxy\bin\Debug"
#r "WX.Data.ServiceProxy.dll"

open System
open System.Collections.Generic
open Microsoft.Practices.EnterpriseLibrary.Validation

open WX.Data.DataModel
open WX.Data.BusinessEntities
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.ServiceProxy.WS_SBIIMS
open WX.Data.ServiceProxy.WS_YWST
ConfigHelper.INS.LoadDefaultClientConfigToManager

///////////////////////////////////////////////////////////

let client=new WS_SBIIMSClient()

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let query=client.GetDJ_GHSs queryEntity
ObjectDumper.Write(query,3)
query.[0].C_ID<-Guid("153BC105-1E6C-4113-9AB8-1D4E181809FA")
let businessEntity=query.[0]
businessEntity.C_SHR<-Nullable<Guid>()
businessEntity.C_BZ<-"Main2009-11-07"
businessEntity.BD_T_DJSP_GHSs.[0].C_BZ<-"2009-11-07Modify"

businessEntity.C_SHR<-Nullable<Guid>(Guid("154f5e0f-63d4-4d25-9093-50a7ae929c5c"))

client.CreateDJ_GHS  businessEntity

client.UpdateDJ_GHS  businessEntity

client.DeleteDJ_GHS  businessEntity

//List<BD_T_DJ_GHS>(seq{yield businessEntity})
//List<BD_T_DJ_GHS>([businessEntity])
List<BD_T_DJ_GHS>([|businessEntity|])
|>client.DeleteDJ_GHSs  

//////////////////////////////////////////////////////////

//(*
let result=
  use client=new WS_SBIIMSClient()
  //ObjectDumper.Write(client.Endpoint,2)
  let queryEntity=BQ_CK()

  let ck=client.GetCKs queryEntity
  ck

let result=
  use client=new WS_SBIIMSClient()
  let queryEntity=BQ_YG()
  queryEntity.C_BZ<-null
  queryEntity.IsQueryableNullOfC_BZ<-true
  let yg=client.GetYGs queryEntity
  yg

ObjectDumper.Write result.Count

let result1=
  let  client=new WS_SBIIMSClient()
  let queryEntity=BQ_YG_TEST()
  //queryEntity.C_BZ <-null
  //queryEntity.C_ID<-Nullable<Guid>(Guid("294dc5cf-c1c8-4f06-96c4-4670f5872324"))
  let ck=client.GetYG_TESTs queryEntity
  

ObjectDumper.Write result1.Count
//*)

(* Right
let client=new WS_YWSTClient()
let ck=client.GetCKs
*)


(*

The runtime has encountered a fatal error. The address of the error was at 0xe97e8d50, on thread 0x2578. The error code is 0xc0000005. This error may be a bug in the CLR or in the unsafe or non-verifiable portions of user code. Common sources of this bug include user marshaling errors for COM-interop or PInvoke, which may corrupt the stack.
是下列的enum定义导致的
 
[<DataContract>]
type TrackingInfo=
  | [<EnumMember>] Unchanged=1 //编译时不报错，但在WCF中将会引起fatal error. 应该从0开始
  | [<EnumMember>] Created=2
  | [<EnumMember>] Updated=3
  | [<EnumMember>] Deleted=4

正确的定义是
[<DataContract>]
type TrackingInfo=
  | [<EnumMember>] Unchanged=0 
  | [<EnumMember>] Created=1
  | [<EnumMember>] Updated=2
  | [<EnumMember>] Deleted=3

在C#中也是类似的
The runtime has encountered a fatal error. The address of the error was at 0xe97e8d50, on thread 0x2578. The error code is 0xc0000005. This error may be a bug in the CLR or in the unsafe or non-verifiable portions of user code. Common sources of this bug include user marshaling errors for COM-interop or PInvoke, which may corrupt the stack.
  [Serializable]
  [DataContract]
  public enum TrackingInfo
  {
    [EnumMember]
    Unchanged=1,
    [EnumMember]
    Created=2,
    [EnumMember]
    Updated=3,
    [EnumMember]
    Deleted=4
}

以下是正确的
  [Serializable]
  [DataContract]
  public enum TrackingInfo
  {
    [EnumMember]
    Unchanged,
    [EnumMember]
    Created,
    [EnumMember]
    Updated,
    [EnumMember]
    Deleted
}
 或
  [Serializable]
  [DataContract]
  public enum TrackingInfo
  {
    [EnumMember]
    Unchanged=0,
    [EnumMember]
    Created=1,
    [EnumMember]
    Updated=2,
    [EnumMember]
    Deleted=3
}

*)


(*
//执行时要使用两个fsi的Session，一个fsi实例作为服务端，另外一个fsi实例作为客户端， 即服务端Session和客户端Session不能使用同一个fsiSession

let configFilePath= @"D:\Workspace\SBIIMS\WX.Data.ServiceProxy\App.config"
//let configFilePath= @"D:\Work for myself\F# Research\BusinessArchitecture\SBIIMS\WX.Data.ServiceProxy\App.config"
//let config=ConfigHelper.INS.OpenConfig configFilePath

let channel=new WX.Data.Helper.CustomClientChannel<IBusinessEntityService>(configFilePath)
let client=channel.CreateChannel()
ObjectDumper.Write(channel,2)
let query001=client.GetT_CKs()

let query002=
  use serviceProxy=new BusinessEntityServiceClient()
  ObjectDumper.Write(serviceProxy.Endpoint,2)
  let result001=serviceProxy.GetT_CKs()
  ObjectDumper.Write(result001,2)
  result001
  
*)
