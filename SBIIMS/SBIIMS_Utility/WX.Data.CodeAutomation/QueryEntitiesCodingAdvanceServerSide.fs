namespace WX.Data.CodeAutomation

open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

//不涉及客户端使用的部分
type QueryEntitiesCodingAdvanceServerSide=
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
            |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not && a.COLUMN_NAME<>"C_FBID")  //为便于分布式设计，将"C_FBID"设计到BQ_Base中
          sb.AppendFormat( @"
[<Sealed>]
[<DataContract>]
type BQ_{0}()=
  inherit BQ_Base()
  {1}
  {2}",
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
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match a.DATA_TYPE with
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
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match a.DATA_TYPE with
                | EndsWithIn NullableTypeConditions _ ->"null"
                |  x -> "new System.Nullable<"+x+">()"
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
              sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=_{1}<-v",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore

            for a in !columns do
              match a.DATA_TYPE.ToLowerInvariant() with
              | EndsWithIn QueryRangeTypeConditions x ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1}Second 
    with get ()=_{1}Second 
    and set v=_{1}Second<-v",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    )|>ignore
              | _ ->()
            sbTem.ToString().Trim()
            )
          )|>ignore

          sb.AppendLine()|>ignore
    sb.ToString()

////////////////////////////////////////////////////////////////////////////////////////////

(*使用了OnPropertyChanged
type QueryEntitiesCodingAdvanceServerSide=
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
      | PCLSHTable ->()
      | _ ->
          columns:=
            DatabaseInformation.GetColumnSchemal4Way a.TableName
            |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          sb.AppendFormat( @"
[<DataContract>]
type BQ_{0}()=
  inherit BQ_Base()
  {1}
  {2}",
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
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match a.DATA_TYPE with
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
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match a.DATA_TYPE with
                | EndsWithIn NullableTypeConditions _ ->"null"
                |  x -> "new System.Nullable<"+x+">()"
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

            for a in !columns do
              sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then _{1}<-v
      x.OnPropertyChanged ""{1}""",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore

            for a in !columns do
              match a.DATA_TYPE.ToLowerInvariant() with
              | EndsWithIn QueryRangeTypeConditions x ->
                  sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1}Second 
    with get ()=_{1}Second 
    and set v=
      if _{1}Second<>v then _{1}Second<-v
      x.OnPropertyChanged ""{1}Second""",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    )|>ignore
              | _ ->()
            sbTem.ToString().Trim()
            )
          )|>ignore

          sb.AppendLine()|>ignore
    sb.ToString()

*)