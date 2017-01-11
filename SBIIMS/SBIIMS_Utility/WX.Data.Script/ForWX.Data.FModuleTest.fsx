open System
#r "FSharp.PowerPack.Parallel.Seq.dll"
open Microsoft.FSharp.Collections
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open  WX.Data
open WX.Data.ActiveModule

let x01=
 match (box true):?>Nullable<bool> with
 | IsTrue x->ObjectDumper.Write x
 | _ ->ObjectDumper.Write "xxxxxxxx" 
 