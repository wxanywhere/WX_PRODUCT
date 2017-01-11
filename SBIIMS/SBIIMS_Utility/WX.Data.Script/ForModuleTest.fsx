open System
#r "FSharp.PowerPack.Parallel.Seq.dll"
open Microsoft.FSharp.Collections
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open  WX.Data

let x01=100
match x01 with
| x when x>1 ->ObjectDumper.Write "01"
| x when x>10 ->ObjectDumper.Write "02"
| x ->ObjectDumper.Write "00"

  //如果结果条件组为空，则认为是IsMix input
  let dispatcher input=
    if String.IsNullOrWhiteSpace input then [IsMix input]|>PSeq.ofList
    else
      let rec GetConditionGroup (condtionStrs:string list)=  
        match condtionStrs with
        | []->[]
        | h::t ->
            match Comm.decision h with
            | IsDJH x ->  IsDJH x::GetConditionGroup t
            | IsMCJM x ->IsMCJM x::GetConditionGroup t
            | IsXM x ->IsXM x::GetConditionGroup t
            | IsXBH (x,y) ->IsXBH (x,y) ::GetConditionGroup t
            | IsSL (x,y) ->IsSL (x,y) ::GetConditionGroup t
            | IsJE  (x,y)  ->IsJE (x,y) ::GetConditionGroup t
            | IsRQ  (x,y)  ->IsRQ (x,y) ::GetConditionGroup t
            | IsMix x ->GetConditionGroup t //不做为条件
      input.Split [|'+'|]
      |>PSeq.toList
      |>GetConditionGroup
      |>PSeq.distinct

dispatcher "1000000+XJXJ"
|>fun a->
    for b in a do
      match b with
      | IsMCJM x ->ObjectDumper.Write x
      | _ ->()



let x01:string []=[||]
x01
|>PSeq.take 10

x01
|>PSeq.skip 10

x01
|>PSeq.headOrDefault

x01
|>PSeq.head

