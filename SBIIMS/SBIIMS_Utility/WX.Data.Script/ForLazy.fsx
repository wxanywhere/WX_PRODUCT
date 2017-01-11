


let x = Lazy<int>.Create(fun () -> printfn "Evaluating x..."; 10)
let y = lazy (printfn "Evaluating y..."; x.Value + x.Value);;
let z=printfn "Evaluating y..."; x.Value + x.Value
x.Value
y.Value
x.Force()

val x : Lazy<int> = <unevaluated>
val y : Lazy<int> = <unevaluated>
