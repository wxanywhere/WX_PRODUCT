﻿namespace WX.Data.CodeAutomation

open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

//BQ的分离设计，为减少和服务端的交互数据量,现有的WX.Data.BusinessBase和WX.Data.BusinessEntitiesWindows也可以取下WindowsBase.dll和PresentationCore.dll的引用
type QueryEntitiesCodingAdvanceClientSideNew=
  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)=

    let blankSapce="  "
    let sb=StringBuilder()
    let sbTem=StringBuilder()

    sb.Append( @"namespace WX.Data.BusinessEntities
open System
open System.Windows
open System.Runtime.Serialization
open WX.Data
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
type BQ_{0}_Client()=
  inherit BQ_ClientBase()
  {1}
  {2}
  {3}
  {4}
  {5}
  {6}
  {7}
  {8}",
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
                match a.DATA_TYPE with
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
            //{2} //生成 _IsCheckedC_MC, 用于绑定条件选择项, 查询条件可能一个都没选择，所以IsChecked应该使用3态
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  let mutable _IsChecked{1}=new Nullable<_>(false)",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{3} //生成 _IsDefaultC_MC, 用于构造条件的中间变量，在多个查询中同时又存在可选条件组时，保存可选条件组的默认选项 
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  let mutable _IsDefault{1}=false",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{4} //生成 _VisibilityC_MC, 用于处理条件的多个可选项的可见性 Visibility.Hidden不可见但需要占位，所以应该使用Visibility.Collapsed
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  let mutable _Visibility{1}=Visibility.Collapsed",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{5} //属性中的方法必须声明，否则不能在WCF中序列化
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
              match a.DATA_TYPE with
              | EndsWithIn QueryRangeTypeConditions x ->
                  sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set (v:{2})=
      if _{1}<>v then
        _{1}<-v
        x.OnPropertyChanged ""{1}""
      else x.IsDefault{1}<-false
      match v with
      | _ when v.HasValue ->
          if _{1}Second.HasValue && _{1}Second.Value>=v.Value|>not then
            x.{1}Second<-v
          x.IsChecked{1}<-new Nullable<_>(true)
          x.Visibility{1}<-Visibility.Visible
      | _ ->
          x.{1}Second<-v 
          x.IsChecked{1}<-new Nullable<_>(false)
          x.Visibility{1}<-Visibility.Collapsed",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    ,
                    //{2}
                    match x with
                    |  EndsWithIn NullableTypeConditions _ -> x
                    |  _ -> "System.Nullable<"+x+">"
                    )|>ignore
              | x ->
                  match x with
                  | EndsWithIn NullableTypeConditions _ ->
                      match x with
                      |  EndsWithIn StringConditions _  ->
                          match a.COLUMN_NAME with
                          | EndsWithIn NeedToUpperColumnNames _ ->
                              sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        x.OnPropertyChanged ""{1}""
      else x.IsDefault{1}<-false
      match v with
      | HasLength _ ->
          if v<>v.ToUpper() then 
            _{1} <-v.ToUpper()
            x.OnPropertyChanged ""{1}""
          x.IsChecked{1}<-new Nullable<_>(true)
          x.Visibility{1}<-Visibility.Visible
      | _ ->
          x.IsChecked{1}<-new Nullable<_>(false)
          x.Visibility{1}<-Visibility.Collapsed",
                                //{0}
                                String.Empty
                                ,
                                //{1}
                                a.COLUMN_NAME
                                )|>ignore
                          | _ ->
                              sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        x.OnPropertyChanged ""{1}""
      else x.IsDefault{1}<-false
      match v with
      | HasLength _ -> 
          x.IsChecked{1}<-new Nullable<_>(true)
          x.Visibility{1}<-Visibility.Visible
      | _ ->
          x.IsChecked{1}<-new Nullable<_>(false)
          x.Visibility{1}<-Visibility.Collapsed",
                                //{0}
                                String.Empty
                                ,
                                //{1}
                                a.COLUMN_NAME
                                )|>ignore
                      | _ ->
                          //可能是byte[]类型的
                          sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        x.OnPropertyChanged ""{1}""
      else x.IsDefault{1}<-false
      match v with
      | HasLength _ -> 
          x.IsChecked{1}<-new Nullable<_>(true)
          x.Visibility{1}<-Visibility.Visible
      | _ ->
          x.IsChecked{1}<-new Nullable<_>(false)
          x.Visibility{1}<-Visibility.Collapsed",
                            //{0}
                            String.Empty
                            ,
                            //{1}
                            a.COLUMN_NAME
                            )|>ignore
                  | _ ->
                      sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        x.OnPropertyChanged ""{1}""
      else x.IsDefault{1}<-false 
      match v with
      | _ when v.HasValue  -> 
          x.IsChecked{1}<-new Nullable<_>(true)
          x.Visibility{1}<-Visibility.Visible
      | _ ->
          x.IsChecked{1}<-new Nullable<_>(false)
          x.Visibility{1}<-Visibility.Collapsed",
                        //{0}
                        String.Empty
                        ,
                        //{1}
                        a.COLUMN_NAME
                        )|>ignore

            for a in !columns do
              match a.DATA_TYPE with
              (*
              | x when x.EndsWith("byte") || x.EndsWith("int16") || x.EndsWith("int32") || x.EndsWith("int64")
                  || x.EndsWith("double") || x.EndsWith("single") || x.EndsWith("decimal")  ->
              *)
              | EndsWithIn QueryRangeTypeConditions x ->
                  match x with
                  | EndsWithIn DateTimeConditions _ ->
                      sbTem.AppendFormat(@"{0}
  member x.{1}Second 
    with get ()=_{1}Second 
    and set (v:{2})=
      if _{1}Second<>v then
        _{1}Second<-v
        x.OnPropertyChanged ""{1}Second""
      match v with
      | _ when v.HasValue->
          if _{1}.HasValue |>not then
            x.{1}<-v
          elif  v.Value>=_{1}.Value |>not  then 
            _{1}Second<-_{1}
            x.OnPropertyChanged ""{1}Second""
          match v.Value with
          | y -> 
              if  y.Hour=0 && y.Minute=0 && y.Second=0 && y.Millisecond=0 then
                _{1}Second<-Nullable<_>(y.AddDays(1.0).AddMilliseconds(-1.0))
                x.OnPropertyChanged ""{1}Second""
      | _ ->()",
                        //{0}
                        String.Empty
                        ,
                        //{1}
                        a.COLUMN_NAME
                        ,
                        //{2}
                        match x with
                        |  EndsWithIn NullableTypeConditions _ -> x
                        |  _ -> "System.Nullable<"+x+">"
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat(@"{0}
  member x.{1}Second 
    with get ()=_{1}Second 
    and set (v:{2})=
      if _{1}Second<>v then
        _{1}Second<-v
        x.OnPropertyChanged ""{1}Second"" 
      match v with
      | _ when v.HasValue->
          if _{1}.HasValue |>not then
            x.{1}<-v
          elif  v.Value>=_{1}.Value |>not  then 
            _{1}Second<-_{1}
            x.OnPropertyChanged ""{1}Second""
      | _ ->()",
                        //{0}
                        String.Empty
                        ,
                        //{1}
                        a.COLUMN_NAME
                        ,
                        //{2}
                        match x with
                        |  EndsWithIn NullableTypeConditions _ -> x
                        |  _ -> "System.Nullable<"+x+">"
                        )|>ignore
              | _ ->
                  ()

            sbTem.ToString().Trim()
            )
            ,
            //{6} //生成 IsCheckedC_MC, 用于绑定条件选择项, 属于客户端处理部分，所以不需要 [<DataMember>]
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              match a.DATA_TYPE with
              | EndsWithIn NullableTypeConditions _ ->
                  sbTem.AppendFormat(@"{0}
  member x.IsChecked{1} 
    with get ()=_IsChecked{1} 
    and set v=
      if _IsChecked{1}<>v then
        _IsChecked{1}<-v
        x.OnPropertyChanged ""IsChecked{1}""
      match _{1} with
      | HasLength _ ->() 
      | _ ->
          if v<>new Nullable<_>(false)  then
            _IsChecked{1} <-new Nullable<_>(false)
            x.OnPropertyChanged ""IsChecked{1}""",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    )|>ignore
              | _ ->
                  sbTem.AppendFormat(@"{0}
  member x.IsChecked{1} 
    with get ()=_IsChecked{1} 
    and set v=
      if _IsChecked{1}<>v then
        _IsChecked{1}<-v
        x.OnPropertyChanged ""IsChecked{1}""
      match _{1} with
      | y when y.HasValue ->()
      | _ ->
          if _IsChecked{1}<>new Nullable<_>(false) then 
            _IsChecked{1} <-new Nullable<_>(false)
            x.OnPropertyChanged ""IsChecked{1}""",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    a.COLUMN_NAME
                    )|>ignore

            sbTem.ToString().Trim()
            )
            ,
            //{7} //生成 VisibilityC_MC, 属于客户端处理部分，所以不需要 [<DataMember>]
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  member x.Visibility{1} 
    with get ()=_Visibility{1} 
    and set v=
      if _Visibility{1}<>v then
        _Visibility{1} <-v 
        x.OnPropertyChanged ""Visibility{1}""",
                //{0}
                String.Empty
                ,
                //{1}
                a.COLUMN_NAME
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{8} //生成 IsDefaultC_MC, 属于客户端处理部分，所以不需要 [<DataMember>]
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            for a in !columns do
              sbTem.AppendFormat(@"{0}
  member x.IsDefault{1} 
    with get ()=_IsDefault{1} 
    and set v=_IsDefault{1} <-v",
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

