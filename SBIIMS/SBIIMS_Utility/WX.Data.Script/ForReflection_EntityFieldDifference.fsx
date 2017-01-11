

#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Windows.Forms.dll"
#r "FSharp.PowerPack.Linq.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"

//#I @"C:\Program Files (x86)\Microsoft Enterprise Library 4.1 - October 2008\Bin"
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
//#r "Microsoft.Practices.ObjectBuilder2.dll"

open Microsoft.FSharp.Collections
open System
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Data
open System.Runtime.Serialization
open Microsoft.FSharp.Linq
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open System.Windows.Forms

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
#r "WX.Data.BusinessDataEntities.JXC.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.CModule.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation
open WX.Data.BusinessEntities
open WX.Data.BusinessBase

let GetDefferentMemberForTarget (template:string)  (reference:BD_Base,target:BD_Base)=
  match reference,target with
  | x ,y ->
    match 
      x.GetType().GetProperties(BindingFlags.Public||| BindingFlags.Instance ||| BindingFlags.SetProperty),
      y.GetType().GetProperties(BindingFlags.Public||| BindingFlags.Instance ||| BindingFlags.SetProperty) with
    | z,w ->
        seq{
          for m in w do
            if z|>Seq.exists (fun a->a.Name =m.Name) |>not then yield String.Format (template,m.Name)
        }
        |>Seq.toArray
//-------------------------------------------------------------------------


(
new BD_T_DJ_XZGL(),
new BD_T_QFMX_XZGL()
)
|>GetDefferentMemberForTarget "y.{0}<-"     
|>ObjectDumper.Write 



//======================================================================
