namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceBDFilesGenerator=
  (*
  调用示例
(
@"D:\Workspace\SBIIMS\WX.Data.BusinessDataEntitiesAdvance.JXC.ZHGL.KHJH",
[ //BD类型名*BD基类名*子类型*类型备注
"BD_TV_ZHGL_KHJH_JHDJ_Advance","BD_T_DJSP_KHJH","综合管理-客户借货-进货单据"
"BD_V_ZHGL_KHJH_KHHZ_Advance",String.Empty,"综合管理-客户借货-客户汇总"
]
)
|>AdvanceBDFilesGenerator.GenerateCodeFiles 
  *)
  static member GenerateCodeFiles (targetDirectory:string,typeSources:(string*string*string*string) seq)= //目标目录*BD类型名*BD基类名*子类型*类型备注   (targetDirectory:string) (typeName:string,baseTypeName:string,childrenTypeName:string,comment:string)
    try 
      let sb=new StringBuilder()
      let sbTem=new StringBuilder()
      seq{
        sb.AppendFormat(@"
//--------------------------------------------------------------------------------
//BD
(
@""{0}"",
[ //BD类型名*BD基类名*子类型*类型备注
{1}
]
)
|>AdvanceBDFilesGenerator.GenerateCodeFiles 
|>ObjectDumper.Write
//--------------------------------------------------------------------------------",
          //{0}
          targetDirectory,
          //{1}
          (
          sbTem.Clear()|>ignore
          for (typeName,baseTypeName,childrenTypeName,comment) in typeSources do 
            sbTem.AppendFormat(@"
""{0}"",""{1}"",""{2}"",""{3}""",
              typeName,baseTypeName,childrenTypeName,comment
              )|>ignore
          sbTem.ToString().TrimStart()
          )
          )|>ignore
        match Path.Combine(targetDirectory,String.Format("{0}.txt","BD_CodeAutomation")) with
        | y ->
            y|>File.WriteFile (sb.ToString().TrimStart()) //可覆盖
            yield y
        sb.Clear()|>ignore
        sbTem.Clear()|>ignore
        for (typeName,baseTypeName,childrenTypeName,comment) in typeSources do
          sb.Clear()|>ignore
          match typeName with
          | x when x.StartsWith("BD_T")-> 
              sb.AppendFormat(@"
namespace WX.Data.BusinessEntities
open System
open System.ComponentModel
open System.Runtime.Serialization
open WX.Data
open WX.Data.BusinessBase

//{0}
[<Sealed>]
[<DataContract>]
type {1}()=
  inherit {2}()

  {3}",
                //{0}
                comment,
                //{1}
                x,
                //{2}
                baseTypeName,
                //{3}
                (
                sbTem.Clear()|>ignore
                match childrenTypeName|>String.IsNullOrWhiteSpace with
                | true ->()
                | _ ->
                    sbTem.AppendFormat (@"
  //--------------------------------------------------------------------------------------
  [<DV>]
  val mutable private _{0}View:{0}[]
  [<DataMember>]
  member x.{0}View
    with get()=x._{0}View
    and set v=x._{0}View<-v",
                      //{0}
                      childrenTypeName
                      )|>ignore
                sbTem.ToString().TrimStart()
                )
                )|>ignore
          | x ->
              sb.AppendFormat(@"
namespace WX.Data.BusinessEntities
open System
open System.ComponentModel
open System.Runtime.Serialization
open WX.Data
open WX.Data.BusinessBase

//{0}
[<DataContract>]
type {1}()=
  inherit BD_ViewBase()

  {2}",
                //{0}
                comment,
                //{1}
                x,
                //{2}
                (
                sbTem.Clear()|>ignore
                match childrenTypeName|>String.IsNullOrWhiteSpace with
                | true ->()
                | _ ->
                    sbTem.AppendFormat (@"
  //--------------------------------------------------------------------------------------
  [<DV>]
  val mutable private _{0}View:{0}[]
  [<DataMember>]
  member x.{0}View
    with get()=x._{0}View
    and set v=x._{0}View<-v",
                      //{0}
                      childrenTypeName
                      )|>ignore
                sbTem.ToString().TrimStart()
                )
                )|>ignore
          match Path.Combine(targetDirectory,String.Format("{0}.fs",typeName)) with
          | y ->
              y|>File.WriteFileCreateOnly (sb.ToString().TrimStart()) //注意，数据访问层只能创建
              yield y
      }
      |>Seq.toArray //须执行
    with e -> ObjectDumper.Write e; raise e