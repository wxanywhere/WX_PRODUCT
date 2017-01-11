
#r "FSharp.PowerPack.Parallel.Seq.dll"
open Microsoft.FSharp.Collections

let data=seq{yield "wx1";yield "wx2"}

let x01=
  data
  |>PSeq.map (fun a->a+"00000000000")
  |>PSeq.map (fun a->a+"xxx")