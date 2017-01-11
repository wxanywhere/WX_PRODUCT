
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
//|>Event.sc
|>Observable.add (fun a->window.Text<-window.Text+"wx")|>ignore

let OneTimeEvent=
  let x:IDisposable ref=ref null
  (fun () ->
    x:=window.Click.Subscribe(fun a->window.Text<-window.Text+"wx";if !x<>null then x.Value.Dispose()))
OneTimeEvent()


(*
//-------------------------------------------------------------------
//http://msdn.microsoft.com/en-us/library/ee370313.aspx
open System
open System.Diagnostics

// Represents a stream of IObserver events.
type ObservableSource<'T>() =

    let protect function1 =
        let mutable ok = false
        try 
            function1()
            ok <- true
        finally
            Debug.Assert(ok, "IObserver method threw an exception.")

    let mutable key = 0

    // Use a Map, not a Dictionary, because callers might unsubscribe in the OnNext
    // method, so thread-safe snapshots of subscribers to iterate over are needed.
    let mutable subscriptions = Map.empty : Map<int, IObserver<'T>>

    let next(obs) = 
        subscriptions |> Seq.iter (fun (KeyValue(_, value)) -> 
            protect (fun () -> value.OnNext(obs)))

    let completed() = 
        subscriptions |> Seq.iter (fun (KeyValue(_, value)) -> 
            protect (fun () -> value.OnCompleted()))

    let error(err) = 
        subscriptions |> Seq.iter (fun (KeyValue(_, value)) -> 
            protect (fun () -> value.OnError(err)))

    let thisLock = new obj()

    let obs = 
        { new IObservable<'T> with
            member this.Subscribe(obs) =
                let key1 =
                    lock thisLock (fun () ->
                        let key1 = key
                        key <- key + 1
                        subscriptions <- subscriptions.Add(key1, obs)
                        key1)
                { new IDisposable with 
                    member this.Dispose() = 
                        lock thisLock (fun () -> 
                            subscriptions <- subscriptions.Remove(key1)) } }

    let mutable finished = false

    // The source ought to call these methods in serialized fashion (from
    // any thread, but serialized and non-reentrant).
    member this.Next(obs) =
        Debug.Assert(not finished, "IObserver is already finished")
        next obs

    member this.Completed() =
        Debug.Assert(not finished, "IObserver is already finished")
        finished <- true
        completed()

    member this.Error(err) =
        Debug.Assert(not finished, "IObserver is already finished")
        finished <- true
        error err

    // The IObservable object returned is thread-safe; you can subscribe 
    // and unsubscribe (Dispose) concurrently.
    member this.AsObservable = obs


let xa=new ObservableSource<int>()
let xb=xa.AsObservable
xb.Add (fun
*)