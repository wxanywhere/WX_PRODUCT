

//It must load on sequence
#r "System.Configuration.dll"
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
//#r "Microsoft.Practices.ObjectBuilder2.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
open System
open System.Configuration
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.FSharp.Collections

#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data
open WX.Data.CodeAutomation

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//===================================================

let GetDefferentColumnsForTarget (template:string)  (referenceTable:string,targetTable:string)=
  match referenceTable,targetTable with
  | x ,y ->
      match 
        x|>DatabaseInformation.GetColumnSchemal4Way,
        y|>DatabaseInformation.GetColumnSchemal4Way with
      | z,w ->     
          seq{
            for m in w do
              if z|>Seq.exists (fun a->a.COLUMN_NAME =m.COLUMN_NAME) |>not then yield String.Format (template,m.COLUMN_NAME)
          }
      |>Seq.toArray
//-------------------------------------------------------------------------
(
"T_JYTZ_JYGL",
"T_JYTZ_XFP"
)
|>GetDefferentColumnsForTarget "y.{0}<-"     
|>ObjectDumper.Write 