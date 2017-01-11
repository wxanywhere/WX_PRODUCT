
#r "WindowsBase.dll"
#r "System.Configuration.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
open System
open System.Configuration
open System.Collections.Generic
open Microsoft.FSharp.Collections
open System.Windows.Forms

#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data

//============================================

let window=new Form()
window.TopMost<-true
window.Show()
let xa=window.Click.Subscribe(fun a->window.Text<-window.Text+"wx")
xa.Dispose()
window.Close()

window.Click
|>Observable.

let xb=
  window.Click
  |>Observable.subscribe (fun a->window.Text<-window.Text+"wx")
xb.Dispose()

let evt=
  let x:IDisposable ref=ref null
  (fun () ->
    x:=window.Click.Subscribe(fun a->window.Text<-window.Text+"wx";if !x<>null then x.Value.Dispose()))
evt()
