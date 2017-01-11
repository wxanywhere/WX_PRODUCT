namespace WX.Data.CodeAutomation
open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

(* Code Temlate
type Table=
  static member T_CK1="T_CK1"
使用type的问题是，如果有一个模块同时引用两个模块数据字典，那么类型名相同的话，将有一个模块的数据库字典被屏蔽
*)

type TableFKInstanceDictionaryCoding=
  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)= //(tableNames:string list)=  
    let sb=StringBuilder()
    seq{
      for tableRelatedInfo in tableRelatedInfos do
        for a in DatabaseInformation.GetAsFKRelationship tableRelatedInfo.TableName do
          yield a.PK_TABLE_ALIAS
        //所有的主键又都是外键的情况，此时两个表实际上可以合成一个表如T_GHS和T_ZZ_GHS的关系, 在这里产生的表名只有在DA_.._Advance中才会用到
        match 
          DatabaseInformation.GetPKColumns tableRelatedInfo.TableName, 
          DatabaseInformation.GetAsFKRelationship tableRelatedInfo.TableName with
        | x,y -> 
            if x|>PSeq.forall (fun a->y|>List.exists (fun b->b.FK_COLUMN_NAME=a.COLUMN_NAME)) then  //表中所有的主键又都是外键的情况
              yield tableRelatedInfo.TableName
    }
    |>PSeq.distinct
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
module FTN=") |>ignore //外键表实例别名
    else
      sb.Insert(0, @"namespace WX.Data.DataAccess
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FTN=()") |>ignore //外键表实例别名
    (string sb).TrimStart()
