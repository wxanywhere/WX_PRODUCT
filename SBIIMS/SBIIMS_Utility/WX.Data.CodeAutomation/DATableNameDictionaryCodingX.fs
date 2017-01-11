namespace WX.Data.CodeAutomation
open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

(*
使用type的问题是，如果有一个模块同时引用两个模块数据字典，那么类型名相同的话，将有一个模块的数据库字典被屏蔽
*)
type DATableNameDictionaryCodingX=
  static member GetCode (tableRelatedInfos:TableRelatedInfoX[])= 
    let sb=StringBuilder()
    seq{
      for tableRelatedInfo in tableRelatedInfos do
        yield tableRelatedInfo.TableInfo.TABLE_NAME
    }
    |>Seq.sort
    |>Seq.iter (fun a->  
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
