namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper
open WX.Data.Database

type DACodingMainChildTwoLevelTablePartX=
  static member GetCodeWithMainChildTableTwoLevelTemplate   (databaseInstanceName:string) (entityContextNamePrefix:string) (tableRelatedInfo:TableRelatedInfoX)  (tableInfos:TableInfo[])=
    String.Empty