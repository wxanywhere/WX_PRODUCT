namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper

type DataAccessCodingChildTablePart=

  static member GetCodeWithChildTableTemplate (databaseInstanceName:string) (tableName:string)=
    String.Empty