namespace WX.Data.CodeAutomation

open System
open System.Linq
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data.Helper
open WX.Data
open WX.Data.CodeAutomationHelper

type DataEntitiesSegmentCoding=
  (*
  当前由于new as indent在WCF环境下的问题,  将来isWithInstanceClone需要改为isWithInstanceConstructor,即须改为构造函数方式
  *)
  static member GetCode (instanceTypeName:string option) (inheritTypeName:string option) (columnConditions:(string*string*bool*string) seq)=   
    let isValidated()=
      let flag=ref true
      columnConditions
      |>Seq.iteri (fun i (a,b,_,_)->
          if TypeShortNames|>Seq.exists (fun c->b=c)|>not && b.Contains ("_") |>not && b.EndsWith ("]") |>not then //自定义BD实体类型忽略
            ObjectDumper.Write ("----------------------------------------------") 
            ObjectDumper.Write (string (i+1)+" of elements has some problems , The type name of field ["+a+"] is not right!") 
            flag:=false
          )
      match 
        columnConditions
        |>Seq.countBy (fun (a,_,_,_)->a) with
      | x when x|>Seq.exists (fun (_,b)->b>1) ->
          x
          |>Seq.filter (fun (_,b) ->b>1)
          |>Seq.iter (fun (a,b) ->ObjectDumper.Write ("The field of "+a+" has "+string b+" copies of the same."))
          flag:=false 
      | _ ->()
      !flag
    match isValidated() with
    | false ->
        ObjectDumper.Write ("----------------------------------------------") 
        "Wrong!"  
    | _ ->
        let sbTem=new StringBuilder()
        let sb=new StringBuilder()  
        match
          columnConditions|>Seq.filter (fun (_,a,_,_)->TypeShortNames|>Seq.exists (fun c->a=c)),
          columnConditions|>Seq.filter (fun (_,a,_,_)->TypeShortNames|>Seq.exists (fun c->a=c)|>not) with
        | xa, xb ->
            sb.AppendFormat( @"
(* Template
[
{0}
]
*)",
              //{0}
              (
              sbTem.Clear()|>ignore
              for columnName,columnTypeStr,isNullable,comment in xa do
                sbTem.AppendFormat(@"
""{0}"",""{1}"",{2},""{3}""",
                  //{0}
                  columnName
                  ,
                  //{1}
                  columnTypeStr
                  ,
                  //{2}
                  isNullable.ToString().ToLower()
                  ,
                  //{3}
                  comment
                  )|>ignore
              if Seq.isEmpty xb|>not then
                sbTem.Append(@"
//--------------")|>ignore
              for columnName,columnTypeStr,isNullable,comment in xb do
                sbTem.AppendFormat(@"
""{0}"",""{1}"",{2},""{3}""",
                  //{0}
                  columnName
                  ,
                  //{1}
                  columnTypeStr
                  ,
                  //{2}
                  isNullable.ToString().ToLower()
                  ,
                  //{3}
                  comment
                  )|>ignore
              sbTem.ToString().TrimStart()
              )
              )|>ignore

            sb.Append(@"
  //-------------------------------------------------") |>ignore
            sb.AppendLine()|>ignore

            match instanceTypeName with
            | Some y ->
                match columnConditions with
                | HasElement _ ->
                    sb.AppendFormat(@"
  new () = {{{3}}}
  new (instance:{0}) as x = {{{2}}} then
    match instance with
    | y ->
        {1}",
                      //{0}
                      y,
                      //{1}
                      (
                      sbTem.Clear()|>ignore
                      for (columnName,_,_,_) in columnConditions do 
                        sbTem.AppendFormat (@"
        x.{0}<-y.{0}",
                          //{0}
                          columnName
                          )|>ignore
                      sbTem.ToString().Trim()
                      ),
                      //{2}
                      (
                      match inheritTypeName with
                      | Some z ->
                          match z with
                          | EndsWithIn ["viewbase"] _ ->
                              String.Format ("inherit {0} ()", z)
                          | _ ->
                              String.Format ("inherit {0} (instance)", z)
                      | _ ->String.Empty
                      ),
                      //{3}
                      (
                      match inheritTypeName with
                      | Some z ->
                          String.Format ("inherit {0} ()", z)
                      | _ ->String.Empty
                      )
                      )|>ignore
                | _ ->
                    sb.AppendFormat(@"
  new () = {{{2}}}
  new (instance:{0}) = {{{1}}}",
                      //{0}
                      y,
                      //{1}
                      (
                      match inheritTypeName with
                      | Some z ->
                          match z with
                          | EndsWithIn ["viewbase"] _ ->
                              String.Format ("inherit {0} ()", z)
                          | _ ->
                              String.Format ("inherit {0} (instance)", z)
                      | _ ->String.Empty
                      ),
                      //{2}
                      (
                      match inheritTypeName with
                      | Some z ->
                          String.Format ("inherit {0} ()", z)
                      | _ ->String.Empty
                      )
                      )|>ignore
            | _ ->()
            sb.AppendLine()|>ignore

            for columnName,columnTypeStr,isNullable,_ in xa do
              sb.AppendFormat(@"{0}
  [<DV>]
  val mutable private _{1}:{2}
  [<DataMember>]
  member x.{1} 
    with get ()=x._{1} 
    and set v=
      if  x._{1}<>v  then
        x._{1} <- v
        x.OnPropertyChanged ""{1}""",
                //{0}
                String.Empty
                ,
                //{1}
                columnName
                ,
                //{2}
                match isNullable with
                | true ->
                    match columnTypeStr with
                    | EndsWithIn NullableTypeConditions _ ->columnTypeStr
                    | _ ->"Nullable<"+columnTypeStr+">"
                | _ ->columnTypeStr
                ) |>ignore
              sb.AppendLine()|>ignore
            if Seq.isEmpty xb|>not then
              sb.AppendLine()|>ignore
              sb.Append(@"
  //---------------------------------------------------------------")|>ignore
              sb.AppendLine()|>ignore
            for columnName,columnTypeStr,isNullable,_ in xb do
              match columnTypeStr with
              | EndsWithIn ["]"] _ ->
                  sb.AppendFormat(@"{0}
  [<DV>]
  val mutable private _{1}:{2}
  [<DataMember>]
  member x.{1} 
    with get ()=x._{1} 
    and set v=x._{1} <- v",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    columnName
                    ,
                    //{2}
                    columnTypeStr
                    ) |>ignore
              | _ ->
                  sb.AppendFormat(@"{0}
  [<DV>]
  val mutable private _{1}:{2}
  [<DataMember>]
  member x.{1} 
    with get ()=
      if x._{1}=Null() then x._{1} <-new {2}()
      x._{1}",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    columnName
                    ,
                    //{2}
                    columnTypeStr
                    ) |>ignore
              sb.AppendLine()|>ignore
        (*
        //当前由于new () as...的问题，将来应该改为构造函数方式
        match instanceTypeName with
        | Some y ->
            sb.AppendFormat(@"
  member x.CloneFrom (instance:{0})=
    match instance with
    | y ->
        {1}",
              //{0}
              y,
              //{1}
              (
              sbTem.Clear()|>ignore
              for (columnName,_,_,_) in columnConditions do 
                sbTem.AppendFormat (@"
        x.{0}<-y.{0}",
                  //{0}
                  columnName
                  )|>ignore
              sbTem.ToString().Trim()
              )
              )|>ignore
        | _ ->()
        *)

        sb.Append(@"
  //---------------------------------------------------------------")|>ignore
        sb.ToString()


(*
type ColumnCondtion=
  {
  ColumnName:string
  ColumnTypeStr:string
  IsNullable:bool
  }

type DataEntitiesSegmentCoding=
  static member GetCode (columnCondtion:ColumnCondtion seq)=  
    let sb=StringBuilder()
    for a in columnCondtion do
      sb.AppendFormat(@"{0}
  [<DV>]
  val mutable private _{1}:{2}
  [<DataMember>]
  member x.{1} 
    with get ()=x._{1} 
    and set v=
      if  x._{1}<>v  then
        x._{1} <- v
        x.OnPropertyChanged ""{1}""",
        //{0}
        String.Empty
        ,
        //{1}
        a.ColumnName
        ,
        //{2}
        a.ColumnTypeStr
        ) |>ignore
    sb.ToString()
*)