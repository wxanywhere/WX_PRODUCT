namespace WX.Data.CodeAutomation
open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Database

(* Code Temlate
type Table=
  static member T_CK1="T_CK1"
使用type的问题是，如果有一个模块同时引用两个模块数据字典，那么类型名相同的话，将有一个模块的数据库字典被屏蔽
*)

type DAFKTableInstanceDictionaryCodingX=
  static member GetCode (tableRelatedInfos:TableRelatedInfoX[]) (tableInfos:TableInfo[])=
    let sb=StringBuilder()
    seq{
      for tableRelatedInfo in tableRelatedInfos do
        for a in tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos|>Array.filter (fun a->tableInfos|>Array.exists (fun b->b.TABLE_NAME=a.PK_TABLE)) do
          yield a.PK_TABLE_ALIAS
        //所有的主键又都是外键的情况，此时两个表实际上可以合成一个表如T_GHS和T_ZZ_GHS的关系, 在这里产生的表名只有在DA_.._Advance中才会用到
        match 
          tableRelatedInfo.TableInfo.TablePrimaryKeyInfos, 
          tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos|>Array.filter (fun a->tableInfos|>Array.exists (fun b->b.TABLE_NAME=a.PK_TABLE)) with
        | x,y -> 
            if x|>Array.forall (fun a->y|>Array.exists (fun b->b.FK_COLUMN_NAME=a.COLUMN_NAME)) then  //表中所有的主键又都是外键的情况
              yield tableRelatedInfo.TableInfo.TABLE_NAME
    }
    |>Seq.distinct
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
module FTN=") |>ignore //外键表实例别名
    else
      sb.Insert(0, @"namespace WX.Data.DataAccess
[<RequireQualifiedAccess>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module FTN=()") |>ignore //外键表实例别名
    (string sb).TrimStart()
