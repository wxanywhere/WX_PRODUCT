namespace WX.Data.CodeAutomation

open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

//
type QueryEntitiesCodingAdvanceTwoStateChecked=
  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)=

    let blankSapce="  "
    let sb=StringBuilder()
    let sbTem=StringBuilder()

    sb.Append( @"namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open WX.Data.BusinessBase") |>ignore
    let columns=ref Unchecked.defaultof<DbColumnSchemalR pseq>
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | DJLSHTable 
      | LSHTable
      | PCLSHTable 
      | JYLSHTable->()
      | _ ->
          columns:=
            DatabaseInformation.GetColumnSchemal4Way a.TableName
            |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          sb.AppendFormat( @"
[<DataContract>]
type BQ_{0}()=
  inherit BQ_Base()
  {1}
  {2}
  {3}
  {4}",
            //{0}
            a.TableName.Remove(0,2) //移出表名的前缀'T_'
            ,
            //{1}
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  let mutable _{1}:{2}={3}",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                ,
                //{2}
                match a.DATA_TYPE with
                (*
                |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]") -> x
                *)
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match a.DATA_TYPE with
                (*
                |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]")-> "null"
                *)
                | EndsWithIn NullableTypeConditions _ ->"null"
                |  x -> "new System.Nullable<"+x+">()"
                )|>ignore

            for a in !columns|>PSeq.filter (fun a->a.IS_NULLABLE_TYPED) do
              sbTem.AppendFormat(@"{0}
  let mutable _IsQueryableNullOf{1}=false",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore

            //范围条件的第二个参数
            for a in
              !columns|>PSeq.filter (fun a->
                match a.DATA_TYPE.ToLowerInvariant() with
                (*
                | x when x.EndsWith("byte") || x.EndsWith("int16") || x.EndsWith("int32") || x.EndsWith("int64")
                    || x.EndsWith("double") || x.EndsWith("single") || x.EndsWith("decimal")  -> true
                *)
                | EndsWithIn QueryRangeTypeConditions _ ->true
                | _ ->false) do
              sbTem.AppendFormat(@"{0}
  let mutable _{1}Second:{2}={3}",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                ,
                //{2}
                match a.DATA_TYPE with
                (*
                |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]") -> x
                *)
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match a.DATA_TYPE with
                (*
                |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]")-> "null"
                *)
                | EndsWithIn NullableTypeConditions _ ->"null"
                |  x -> "new System.Nullable<"+x+">()"
                )|>ignore

            sbTem.ToString().Trim()
            )
            ,
            //{2} //生成 _IsCheckedC_MC, 用于绑定条件选择项
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  let mutable _IsChecked{1}=false",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{3} //属性中的方法必须声明，否则不能在WCF中序列化
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns|>PSeq.filter (fun a->a.IS_NULLABLE_TYPED) do
              sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.IsQueryableNullOf{1} 
    with get ()=_IsQueryableNullOf{1} 
    and set v=
      if _IsQueryableNullOf{1}<>v then
        _IsQueryableNullOf{1}<-v
        x.OnPropertyChanged ""IsQueryableNullOf{1}""",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore

            //注意，但范围条件的第一个值改变时，第二个值应该自动与第一个同步
            for a in !columns do
              match a.DATA_TYPE.ToLowerInvariant() with
              (*
              | x when x.EndsWith("byte") || x.EndsWith("int16") || x.EndsWith("int32") || x.EndsWith("int64")
                  || x.EndsWith("double") || x.EndsWith("single") || x.EndsWith("decimal")  ->
              *)
              | EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set (v:{2})=
      match v with
      | y when y.HasValue ->
          if _{1}Second.HasValue && _{1}Second.Value>=y.Value|>not then
            x.{1}Second<-y
          _{1}<-y
          x.IsChecked{1}<-true
          x.OnPropertyChanged ""{1}""
      | _ ->()",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    ,
                    //{2}
                    match a.DATA_TYPE with
                    |  y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]") -> y
                    |  y -> "System.Nullable<"+y+">"
                    )|>ignore
              | _ ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        x.IsChecked{1}<-true
        x.OnPropertyChanged ""{1}""",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    )|>ignore

            for a in !columns do
              match a.DATA_TYPE.ToLowerInvariant() with
              (*
              | x when x.EndsWith("byte") || x.EndsWith("int16") || x.EndsWith("int32") || x.EndsWith("int64")
                  || x.EndsWith("double") || x.EndsWith("single") || x.EndsWith("decimal")  ->
              *)
              | EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1}Second 
    with get ()=_{1}Second 
    and set (v:{2})=
      match v with
      | y when y.HasValue->
          if _{1}.HasValue |>not then
            x.{1}<-y
            _{1}Second<-y
          elif  y.Value>=_{1}.Value |>not  then 
            _{1}Second<-_{1}
          else _{1}Second<-y
          x.OnPropertyChanged ""{1}Second""
      | _ ->()",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    ,
                    //{2}
                    match a.DATA_TYPE with
                    |  y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]") -> y
                    |  y -> "System.Nullable<"+y+">"
                    )|>ignore
              | _ ->
                  ()

            sbTem.ToString().Trim()
            )
            ,
            //{4} //生成 IsCheckedC_MC, 用于绑定条件选择项, 属于客户端处理部分，所以不需要 [<DataMember>]
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  member x.IsChecked{1} 
    with get ()=_IsChecked{1} 
    and set v=
      if _IsChecked{1}<>v then
        _IsChecked{1} <-v 
        x.OnPropertyChanged ""IsChecked{1}""",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
            sbTem.ToString().Trim()
            )

          )|>ignore

          sb.AppendLine()|>ignore
    sb.ToString()

////////////////////////////////////////////////////////////////////////////////////////////

(* 
type QueryEntitiesCodingAdvanceTwoStateChecked=
  static member GetCode (tableNames:string list)=

    let blankSapce="  "
    let sb=StringBuilder()
    let sbTem=StringBuilder()

    sb.Append( @"namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization") |>ignore
    let columns=ref Unchecked.defaultof<DbColumnSchemalR seq>
    for tableName in tableNames do
      columns:=
        DatabaseInformation.GetColumnSchemal4Way tableName
        |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
      sb.AppendFormat( @"
[<DataContract>]
type BQ_{0}()=
  inherit BQ_Base()
  {1}
  {2}",
        //{0}
        tableName.Remove(0,2) //移出表名的前缀'T_'
        ,
        //{1}
        (
        sbTem.Remove(0,sbTem.Length)|>ignore
        for a in !columns do
          sbTem.AppendFormat(@"{0}
  let mutable _{1}:{2}={3}",
            //{0}
            String.Empty
            ,
            //{1}
            a.COLUMN_NAME
            ,
            //{2}
            match a.DATA_TYPE with
            |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]") -> x
            | x -> "System.Nullable<"+x+">"
            ,
            //{3}
            match a.DATA_TYPE with
            |  x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]")-> "null"
            |  x -> "System.Nullable<"+x+">()"
            )|>ignore

        for a in !columns|>PSeq.filter (fun a->a.IS_NULLABLE_TYPED) do
          sbTem.AppendFormat(@"{0}
  let mutable _IsQueryableNullOf{1}:System.Boolean=false",
            //{0}
            String.Empty
            ,
            //{1}
            a.COLUMN_NAME
            )|>ignore
        sbTem.ToString().Trim()
        )
        ,
        //{2} //属性中的方法必须声明，否则不能在WCF中序列化
        (
        sbTem.Remove(0,sbTem.Length)|>ignore
        for a in !columns|>PSeq.filter (fun a->a.IS_NULLABLE_TYPED) do
          sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.IsQueryableNullOf{1} 
    with get ()=_IsQueryableNullOf{1} 
    and set v=_IsQueryableNullOf{1}<-v",
            //{0}
            String.Empty
            ,
            //{1}
            a.COLUMN_NAME
            )|>ignore

        for a in !columns do
          match a.IS_NULLABLE_TYPED with
          | false ->
              sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=  _{1} <-v ",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
          | _ ->
              match a.DATA_TYPE with
              | x when x.ToLowerInvariant().EndsWith("string")  || x.EndsWith("[]") ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v= 
      if v=null then _IsQueryableNullOf{1}<-true
      else _IsQueryableNullOf{1}<-false
      _{1} <-v ",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    )|>ignore
              | x ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set (v:System.Nullable<{2}>)= 
      if not v.HasValue then _IsQueryableNullOf{1}<-true
      else _IsQueryableNullOf{1}<-false
      _{1} <-v ",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    ,
                    //{2}
                    x
                    )|>ignore
        sbTem.ToString().Trim()
        )

      )|>ignore

      sb.AppendLine()|>ignore
    sb.ToString()
*)

(* 以下代码模板是错误的, 在WCF中序列化时, 属性将被重新设置为默认值, 也就是说都会先调用属性的set 方法，所以序列化之后，IsQueryableNullOf{0}都将=true

[<DataContract>]
type BQ_CK()=
  let mutable _C_BZ:System.String=null
  let mutable _C_ID:System.Nullable<System.Guid>=System.Nullable<System.Guid>()
  let mutable _IsQueryableNullOfC_BZ:System.Boolean=false
  [<DataMember>]
  member x.IsQueryableNullOfC_BZ 
    with get ()=_IsQueryableNullOfC_BZ 
    and private set v=()
  [<DataMember>]
  member x.C_BZ 
    with get ()=_C_BZ 
    and set v= 
      if v=null then _IsQueryableNullOfC_BZ<-true
      else _IsQueryableNullOfC_BZ<-false
      _C_BZ <-v 
  [<DataMember>]
  member x.C_ID 
    with get ()=_C_ID 
    and set v=  _C_ID <-v 

*)

(*

sb.AppendLine() |>ignore
//sb.Append(Environment.NewLine) |>ignore

*)