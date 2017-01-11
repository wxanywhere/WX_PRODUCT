namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type GroupPhaseLevel= //接口文件名成组片断段级别,如IDA_XX_XX_XX_Advance,多个接口文件成组为BL_...业务逻辑文件的片段数
  | OnePhaseGroup
  | TwoPhaseGroup
  | ThreePhaseGroup
  | FourPhaseGroup
  override this.ToString()=
    match this with
    | OnePhaseGroup ->"OnePhaseGroup" 
    | TwoPhaseGroup->"TwoPhaseGroup"
    | ThreePhaseGroup->"ThreePhaseGroup"
    | FourPhaseGroup->"FourPhaseGroup"
  member this.Level
    with get ()=
      match this with
      | OnePhaseGroup ->1
      | TwoPhaseGroup ->2
      | ThreePhaseGroup ->3
      | FourPhaseGroup ->4

[<AutoOpen>]
module InterfaceParse=
  let MatchAliasNames=
    ["x";"y";"z";"u";"v";"w";"r"]
  let BDExecuteContentParameterNames=
    ["executeContent";"executeContentTwo";"executeContentThree";"executeContentFour";"executeContentFive";"executeContentSix";"executeContentSeven"]
  let BDExecuteBaseParameterNames=
    ["executeBase";"executeBaseTwo";"executeBaseThree";"executeBaseFour";"executeBaseFive";"executeBaseSix";"executeBaseSeven"]
  let BDEntityParameterNames=
    ["businessEntity";"businessEntityTwo";"businessEntityThree";"businessEntityFour";"businessEntityFive";"businessEntitySix";"businessEntitySeven"]
  let BDEntityArrayParameterNames=
    ["businessEntities";"businessEntitiesTwo";"businessEntitiesThree";"businessEntitiesFour";"businessEntitiesFive";"businessEntitiesSix";"businessEntitiesSeven"]
  let BQParameterNames=
    ["queryEntity";"queryEntityTwo";"queryEntityThree";"queryEntityFour";"queryEntityFive";"queryEntitySix";"queryEntitySeven"]
  let DefaultParameterNames=
    ["parameter";"parameterTwo";"parameterThree";"parameterFour";"parameterFive";"parameterSix";"parameterSeven"]

  (*
  已停用
  只能解析一个入参
  *)
  let GetInterfaceTypeCodingInfoBackup (sourceDirectory:string) (inferfaceTypeNames:string seq) = //系统简称(如SBIIMS_JXC)*业务逻辑层类型名称*数据访问接口文件目录*数据访问接口类型名组
    try 
    match sourceDirectory,inferfaceTypeNames with
    | c,d ->
        seq{
          for cm in d do
            match Path.Combine(c,cm+".fs")|>File.ReadFile  with
            | x ->
                match Regex.Match (x, @"^\s+(abstract[\w\W\n]+)$",RegexOptions.Multiline)  with   //@"[\w\W\n]*type\s+I([a-zA-Z_]+)\s*\=[\n\w\W]+(abstract[\w\W\n]+)$，第一个abstract前有注释时该匹配有问题
                | y  when y.Groups.Count>1 ->
                    yield 
                      match cm.Remove(0,1) with  //简码数据访问组注释*数据访问层类型名
                      | z when z.Length>3 ->z.Remove(0,3),z  
                      | z ->String.Empty,z
                    ,
                    match y.Groups.[1].Value with
                    | z ->
                        seq{
                          for m in Regex.Split (z,@"\(\*[\w\W\n]*\*\)",RegexOptions.Multiline) do //先去除(*...*)的注释
                            for n in  Regex.Split (m.Trim(),@"\s*\n\s*",RegexOptions.Multiline) do //注释"//"通过行匹配去除
                              if String.IsNullOrWhiteSpace n|>not  then //yield n.Trim()
                                match Regex.Match (n, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*\(*\s*([a-zA-Z_\s\<\>\[\]\:\?\*\#]+)\s*\)*\s*\-\>\s*([a-zA-Z_\s\<\>\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //数组[]前可以有空格, 已经考虑了可选参数 * ?Parameter
                                | w when w.Groups.Count>3 ->
                                    yield 
                                      w.Groups.[1].Value, //方法名称 b
                                      match w.Groups.[2].Value.Replace(" ","").Trim() with //条件名称*条件类型, 并消除"[]"前及"]>"等的空格 c
                                      | y when y.StartsWith "BQ_" ->"queryEntity",y
                                      | y when y.StartsWith "BD_ExecuteContent" ->"executeContent",y
                                      | y when y.StartsWith "BD_ExecuteBase" ->"BD_ExecuteBase",y
                                      | y when y.StartsWith "BD_" && y.EndsWith @"]"  ->"businessEntities",y
                                      | y when y.StartsWith "BD_" ->"businessEntity",y
                                      | y ->String.Empty,y
                                      ,
                                      match w.Groups.[3].Value.Trim() with //结果类型  d
                                      | v ->
                                          match Regex.Split (v,@"[\<\>]",RegexOptions.Singleline) with
                                          | u ->
                                              match u.Length with
                                              | 1 ->v,v
                                              | 3 ->v,u.[1].Replace(" ","") //消除"[]"前的空格
                                              | _ -> failwith (String.Format(@"Result paragraph is not right, it's ""{0}"".",v))
                                | _ ->()
                        }
                | _ ->()
        }
        |>Seq.sortBy (fun ((a,_),_)->a.Remove(0,a.LastIndexOf('_')+1))  //按接口类型名的后缀排序
    with e -> ObjectDumper.Write e; raise e

  (*
  可解析多个一级元组参数
  不支持Currying化参数
  不支持一级以上元组参数类型如 abstract f:string*string*x:(string*(string*string))->string
  *)
  let GetInterfaceTypeCodingInfo (sourceDirectory:string) (inferfaceTypeNames:string seq) = //系统简称(如SBIIMS_JXC)*业务逻辑层类型名称*数据访问接口文件目录*数据访问接口类型名组
    try 
    match sourceDirectory,inferfaceTypeNames with
    | c,d ->
        seq{
          for cm in d do
            match Path.Combine(c,cm+".fs")|>File.ReadFile  with
            | x ->
                match Regex.Match (x, @"^\s+(abstract[\w\W\n]+)$",RegexOptions.Multiline)  with   //@"[\w\W\n]*type\s+I([a-zA-Z_]+)\s*\=[\n\w\W]+(abstract[\w\W\n]+)$，第一个abstract前有注释时该匹配有问题
                | y  when y.Groups.Count>1 ->
                    yield 
                      match cm.Remove(0,1) with  //简码数据访问组注释*数据访问层类型名
                      | z when z.Length>3 ->z.Remove(0,3),z  
                      | z ->String.Empty,z
                    ,
                    match y.Groups.[1].Value with
                    | z ->
                        seq{
                          for m in Regex.Split (z,@"\(\*[\w\W\n]*\*\)",RegexOptions.Multiline) do //先去除(*...*)的注释
                            for n in  Regex.Split (m.Trim(),@"\s*\n\s*",RegexOptions.Multiline) do //注释"//"通过行匹配去除
                              if String.IsNullOrWhiteSpace n|>not  then //yield n.Trim()
                                if Regex.Split(n,@"->").Length>2 then failwith (String.Format(@"The member can't use with currying parameters of ""{0}"" which is in the file of ""{1}"".",n,Path.Combine(c,cm+".fs")))
                                match Regex.Match (n, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*\(*\s*([a-zA-Z_\s\<\>\[\]\:\?\*\#]+)\s*\)*\s*\-\>\s*([a-zA-Z_\s\<\>\[\]]+)\s*.*$",RegexOptions.Singleline)  with     //1.数组[]前可以有空格, 2.已经考虑了可选参数 * ?Parameter,3已经考虑了泛型，4.参数不支持柯里化, 5,不支持多级元组参数类型，6. 已避免参数带括号的情况abstract f:(string*string)->string
                                | w when w.Groups.Count>3 ->
                                    yield 
                                      w.Groups.[1].Value, //方法名称 b
                                      seq{
                                        match Regex.Split (w.Groups.[2].Value,@"\s*\*\s*",RegexOptions.Singleline) with  //解析多个参数
                                        | v ->
                                            if v.Length>7 then failwith (String.Format(@"The  quantity of without name parameters is more than 7, it's ""{0}"" which is in the file of ""{1}"".",n,Path.Combine(c,cm+".fs"))) 
                                            for i in 0..v.Length-1 do
                                              match Regex.Split (v.[i],@"\s*\:\s*",RegexOptions.Singleline) with //参数名:参数类型
                                              | u when u.Length=2 ->yield u.[0],u.[1]
                                              | u when u.Length=1 -> //只有参数类型，无参数名的情况
                                                  match u.[0].Replace(" ","") with //条件名称*条件类型, 并消除"[]"前的空格 c
                                                  | y when y.StartsWith "BQ_" ->yield BQParameterNames.[i],y
                                                  | y when y.StartsWith "BD_ExecuteContent" ->yield BDExecuteContentParameterNames.[i],y
                                                  | y when y.StartsWith "BD_ExecuteBase" ->yield BDExecuteBaseParameterNames.[i],y
                                                  | y when y.StartsWith "BD_" && y.EndsWith @"]" ->yield BDEntityArrayParameterNames.[i],y
                                                  | y when y.StartsWith "BD_" ->yield BDEntityParameterNames.[i],y
                                                  | y when y.Equals "unit" ->()
                                                  | y ->yield DefaultParameterNames.[i],y
                                              | _ ->()
                                      }
                                      ,
                                      match w.Groups.[3].Value.Trim() with //结果类型  d
                                      | v ->
                                          match Regex.Split (v,@"[\<\>]",RegexOptions.Singleline) with
                                          | u ->
                                              match u.Length with
                                              | 1 ->v,v
                                              | 3 ->v,u.[1].Replace(" ","") //消除"[]"前的空格
                                              | _ -> failwith (String.Format(@"Result paragraph is not right, it's ""{0}"".",v))
                                | _ ->()
                        }
                | _ ->()
        }
        |>Seq.sortBy (fun ((a,_),_)->a.Remove(0,a.LastIndexOf('_')+1))  //按接口类型名的后缀排序
    with e -> ObjectDumper.Write e; raise e