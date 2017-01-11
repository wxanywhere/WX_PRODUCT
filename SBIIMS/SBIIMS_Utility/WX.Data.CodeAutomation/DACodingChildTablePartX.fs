namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper
open WX.Data.Database

type DACodingChildTablePartX=

  static member GetCodeWithChildTableTemplate (databaseInstanceName:string) (entityContextNamePrefix:string) (tableRelatedInfo:TableRelatedInfoX) (tableInfos:TableInfo[])=
    String.Empty