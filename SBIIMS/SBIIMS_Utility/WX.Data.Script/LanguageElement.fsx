open System
open System.Text

let x=[|"wx";"wx1"|]

let y=[|"wx";"wx12"|]

let dif=
  (x|>Set.ofSeq,y|>Set.ofSeq)
  |>fun (a,b) ->Set.difference a b

let x="FK_T_JHSP_FZ_C_JHSP_C_GHS_T_JHSP"
let u="FK_T_JHSP_FZ"
let v="T_JHSP"
x.Substring(u.Length,x.Length-u.Length-v.Length-1)

"_C_JHSP_C_GHS".Split([|"_C_"|],StringSplitOptions.RemoveEmptyEntries)

let x=["wx";"wx1";"wx2"]

x

|>Seq.fold (fun o (p:string) ->o) 