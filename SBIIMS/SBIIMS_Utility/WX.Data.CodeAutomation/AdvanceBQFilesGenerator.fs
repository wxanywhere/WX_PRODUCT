namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceBQFilesGenerator=
  (*
  调用示例
(
@"D:\Workspace\SBIIMS\WX.Data.BusinessEntitiesAdvance.JXC.ZHGL.KHJH",
[
"BQ_ZHGL_KHJH_Advance","综合管理-客户借货"
]
)
|>AdvanceBQFilesGenerator.GenerateCodeFiles
|>Clipboard.SetText
  *)
  static member GenerateCodeFiles (targetServerBQDirectory:string,typeSources:(string*string) seq)= 
    try 
      let sb=new StringBuilder()
      let sbTem=new StringBuilder()
      seq{
        sb.AppendFormat(@"
//--------------------------------------------------------------------------------
//BQ
(
@""{0}"",
[
{1}
]
)
|>AdvanceBQFilesGenerator.GenerateCodeFiles
//--------------------------------------------------------------------------------",
          //{0}
          targetServerBQDirectory,
          //{1}
          (
          sbTem.Clear()|>ignore
          for (typeName,comment) in typeSources do 
            sbTem.AppendFormat(@"
""{0}"",""{1}""",
              typeName,comment
              )|>ignore
          sbTem.ToString().TrimStart()
          )
          )|>ignore
        match Path.Combine(targetServerBQDirectory,String.Format("{0}.txt","BQ_CodeAutomation")) with
        | y ->
            y|>File.WriteFile (sb.ToString().TrimStart()) //可覆盖
            yield y
        sb.Clear()|>ignore
        sbTem.Clear()|>ignore
        for (typeName,comment) in typeSources do
          sb.Clear()|>ignore
          sb.AppendFormat(@"
namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open WX.Data.BusinessBase

//{0}
[<Sealed>]
[<DataContract>]
type {1}()=
  inherit BQ_Base()",
            //{0}
            comment,
            //{1}
            typeName
            )|>ignore
          match Path.Combine(targetServerBQDirectory,String.Format("{0}.fs",typeName)) with
          | y ->
              y|>File.WriteFileCreateOnly (sb.ToString().TrimStart()) //注意，数据访问层只能创建
              yield y
          //ClientBQ
          sb.Clear()|>ignore
          sb.AppendFormat(@"
namespace WX.Data.BusinessEntities
open System
open System.Windows
open WX.Data
open WX.Data.BusinessBase

//{0}
type {1}()=
  inherit BQ_ClientBase()",
            //{0}
            comment,
            //{1}
            typeName.Replace("_Advance","_Client_Advance")
            )|>ignore

          (*使用不同的组件时使用
          match Path.Combine(targetServerBQDirectory.Replace("BusinessQueryEntitiesAdvance","BusinessQueryEntitiesClientAdvance"),String.Format("{0}.fs",typeName.Replace("_Advance","_Client_Advance"))) with
          *)
          match Path.Combine(targetServerBQDirectory,String.Format("{0}.fs",typeName.Replace("_Advance","_Client_Advance"))) with
          | y ->
              y|>File.WriteFileCreateOnly (sb.ToString().TrimStart()) //注意，数据访问层只能创建
              yield y
      }
      |>Seq.toArray //须执行
    with e -> ObjectDumper.Write e; raise e