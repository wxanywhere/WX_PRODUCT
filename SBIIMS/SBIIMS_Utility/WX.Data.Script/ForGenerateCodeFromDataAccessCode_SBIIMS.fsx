﻿//#I  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Threading.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "Microsoft.Practices.ObjectBuilder2.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Windows.Forms.dll"
#r "FSharp.PowerPack.Linq.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"

open Microsoft.FSharp.Collections
open System
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open Microsoft.FSharp.Linq
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.Practices.ObjectBuilder2
open System.Windows.Forms
//let projectPath= @"D:\Workspace\SBIIMS"

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation

type System.Text.StringBuilder  with
  member x.AppendN (str:string)=
    x.Append(str) |>ignore
  member x.AppendFormatN (str:string)=
    x.AppendFormat(str) |>ignore

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings
//先将WX.Data.FHelper.Service.Config,中的默认数据库配置改为<dataConfiguration defaultDatabase="SBIIMS" />
//////////////////////////////////////////////////////////////////////////////////////////////////

let GenerateCodeFromDataAcceseQueryCode (dataAccessCode:string)=
  match dataAccessCode.Split([|"BQ_"|],StringSplitOptions.None) with
  | x -> 
      match x.[1].Split([|"_"|],StringSplitOptions.None) with
      | y ->
          y.[0]


"member x.GetKHView (queryEntity:BQ_KH_Advance)="
|>GenerateCodeFromDataAcceseQueryCode

//accessCode.Split([|"BQ_"|],StringSplitOptions.None)
//accessCode.Split([|"_"|],StringSplitOptions.None)

let businessCode= @"member x.GetGHSView (queryEntity:BQ_GHS_Advance)=
    (DA_GHS_Advance.INS:>IDA_GHS_Advance).GetGHSView queryEntity"

let  serviceContracts= @"[<OperationContract>] abstract GetGHSView:queryEntity:BQ_GHS_Advance->List<BD_T_GHS_Advance>"

let wcfServiceCode= @"member x.GetGHSView (queryEntity:BQ_GHS_Advance)=
      BL_JBXX_GHSWH_Advance.INS.GetGHSView queryEntity"

