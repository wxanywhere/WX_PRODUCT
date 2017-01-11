namespace WX.Data.CodeAutomation
open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

(*
使用type的问题是，如果有一个模块同时引用两个模块数据字典，那么类型名相同的话，将有一个模块的数据库字典被屏蔽
*)
type TableNameDictionaryCoding=
  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)= //(tableNames:string list)=  
    let sb=StringBuilder()
    seq{
      for tableRelatedInfo in tableRelatedInfos do
        yield tableRelatedInfo.TableName
    }
    |>PSeq.sort
    |>Seq.iter (fun a->  //使用PSeq.Iter时，有时会出错？？？
        sb.AppendFormat(@"
  let {0}=""{0}""",
          //{0}
          a
          )|>ignore)
    if sb.Length>0 then
      sb.Insert(0, @"namespace WX.Data.DataAccess
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TN=") |>ignore //外键表实例别名
    else
      sb.Insert(0, @"namespace WX.Data.DataAccess
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module TN=()") |>ignore //外键表实例别名
    (string sb).TrimStart()
