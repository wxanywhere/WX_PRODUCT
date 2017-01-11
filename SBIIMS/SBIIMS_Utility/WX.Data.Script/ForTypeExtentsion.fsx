
open Microsoft.FSharp

open Microsoft.FSharp.Collections
open Microsoft.FSharp.Control

[<AutoOpen>]
module TypeExtension=
    type System.Collections.Generic.Dictionary<'K,'V> with
      member this.AddKeyValue (keyValuePair:System.Collections.Generic.KeyValuePair<'K,'V>)=
        this.Add (keyValuePair.Key,keyValuePair.Value) 

    type seq<'T> with
        member xs.RepeatElements(n: int) =
            seq { for x in xs do for i in 1 .. n do yield x }


[<AutoOpen>]
module TypeExtensionX=
  [<System.Runtime.CompilerServices.Extension>]
  type ExtraCSharpStyleExtensionMethodsInFSharp () =
      [<System.Runtime.CompilerServices.Extension>]
      static member inline Sum(xs: seq<'T>) = Seq.sum xs



let listOfIntegers = [ 1 .. 100 ]
let listOfBigIntegers = [ 1I .. 100I ]
let sum1 = listOfIntegers.Sum()
let sum2 = listOfBigIntegers.Sum()

open System
open System.Collections.ObjectModel
open System.Runtime.CompilerServices
[<Extension>]
type ObservableExtensions =   

      [<Extension>]
      [<CompiledName("PascalCase")>]
      static member inline retryAfterDelay<'TSource, 'TException when 'TException :> System.Exception>(source: IObservable<'TSource>, retryDelay: int -> TimeSpan, maxRetries, scheduler: IScheduler): IObservable<'TSource> =
            let rec go(source: IObservable<'TSource>, retryDelay: int -> TimeSpan, retries, maxRetries, scheduler: IScheduler): IObservable<'TSource> =
                    source.Catch<'TSource, 'TException>(fun ex -> 
                        if maxRetries <= 0 then
                            Observable.Throw<'TSource>(ex)
                        else
                            go(source.DelaySubscription(retryDelay(retries), scheduler), retryDelay, retries + 1, maxRetries - 1, scheduler))
            go(source, retryDelay, 1, maxRetries, scheduler)

type DataPumpOperations =
        static member dataPump(source: Async<_>): IObservable<_> = Async.StartAsTask(source).ToObservable()
        static member dataPump(source: seq<_>): IObservable<_> = source |> Observable.toObservable


(*
// C# way
[<System.Runtime.CompilerServices.Extension>]
module ExtensionMethods =
    [<System.Runtime.CompilerServices.Extension>]
    let Great(s : System.String) = "Great"

    // F# way
    type System.String with
        member this.Awesome() = "Awesome"
    let example = "foo".Awesome()


C#
using System;
using MyFSharp;  // reference the F# dll
class Program
{
    static void Main(string[] args)
    {
        var s = "foo";
        //s.Awesome(); // no
        Console.WriteLine(s.Great());  // yes
    }
}


*)