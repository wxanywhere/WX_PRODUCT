namespace WX.Data.CodeAutomation

open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

//不要格式化代码，最好和QueryEntitiesCodingAdvance在格式上保持一致，这样便于修改同步
type QueryEntitiesSegmentCodingServerAndClient=
  static member GetCode (propertyName:string,propertyTypeStr:string,isNullable:bool)=
          let blankSapce="  "
          let sb=StringBuilder()
          let sbTem=StringBuilder()

          //客户端代码部分
          sb.Append(@"
  //客户端部分") |>ignore
          sb.AppendFormat( @"{0}
  {1}
  {2}
  {3}
  {4}
  {5}
  {6}
  {7}
  {8}",
            //{0}
            String.Empty
            ,
            //{1}
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  let mutable _{1}:{2}={3}",
                //{0}
                String.Empty
                ,
                //{1}
                propertyName
                ,
                //{2}
                match propertyTypeStr with
                | EndsWithIn NullableTypeConditions x ->x
                | x -> "System.Nullable<"+x+">"
                ,
                //{3}
                match propertyTypeStr with
                | EndsWithIn NullableTypeConditions _ ->"null"
                |  x -> "new System.Nullable<"+x+">()"
                )|>ignore

            if isNullable then
              sbTem.AppendFormat(@"{0}
  let mutable _IsQueryableNullOf{1}=false",
                //{0}
                String.Empty
                ,
                //{1}
                propertyName
                )|>ignore

            //范围条件的第二个参数
            match propertyTypeStr.ToLowerInvariant() with
            | EndsWithIn QueryRangeTypeConditions x ->
              sbTem.AppendFormat(@"{0}
  let mutable _{1}Second:{2}={3}",
                //{0}
                String.Empty
                ,
                //{1}
                propertyName
                ,
                //{2}
                match x with
                | EndsWithIn NullableTypeConditions _ ->x
                | _ -> "System.Nullable<"+x+">"
                ,
                //{3}
                match x with
                | EndsWithIn NullableTypeConditions _ ->"null"
                |  _ -> "new System.Nullable<"+x+">()"
                )|>ignore
            | _ ->()

            sbTem.ToString().Trim()
            )
            ,
            //{2} //生成 _IsCheckedC_MC, 用于绑定条件选择项, 查询条件可能一个都没选择，所以IsChecked应该使用3态
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  let mutable _IsChecked{1}=new Nullable<bool>(false)",
                //{0}
                String.Empty
                ,
                //{1}
                propertyName
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{3} //生成 _IsDefaultC_MC, 用于构造条件的中间变量，在多个查询中同时又存在可选条件组时，保存可选条件组的默认选项 
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  let mutable _IsDefault{1}=false",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{4} //生成 _VisibilityC_MC, 用于处理条件的多个可选项的可见性 Visibility.Hidden不可见但需要占位，所以应该使用Visibility.Collapsed
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  let mutable _Visibility{1}=Visibility.Collapsed",
                //{0}
                String.Empty
                ,
                //{1}
                propertyName
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{5} //属性中的方法必须声明，否则不能在WCF中序列化
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            if isNullable then
              sbTem.AppendFormat(@"{0}
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
                propertyName
                )|>ignore

            //注意，但范围条件的第一个值改变时，第二个值应该自动与第一个同步
            match propertyTypeStr.ToLowerInvariant() with
            | EndsWithIn QueryRangeTypeConditions _ ->
                sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set (v:{2})=
      match v with
      | y when y.HasValue ->
          if _{1}Second.HasValue && _{1}Second.Value>=y.Value|>not then
            x.{1}Second<-y
          _{1}<-y
          x.IsChecked{1}<-new Nullable<bool>(true)
          x.Visibility{1}<-Visibility.Visible
          x.IsDefault{1}<-false
      | y ->
          x.{1}Second<-y 
          _{1}<-y
          x.IsChecked{1}<-new Nullable<bool>(false)
          x.Visibility{1}<-Visibility.Collapsed
      x.OnPropertyChanged ""{1}""",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    propertyName
                    ,
                    //{2}
                    match propertyTypeStr with
                    | EndsWithIn NullableTypeConditions y  -> y
                    |  y -> "System.Nullable<"+y+">"
                    )|>ignore
            | x ->
                match x with
                | EndsWithIn NullableTypeConditions _ ->
                      match x with
                      |  EndsWithIn StringConditions _ ->
                          match propertyName with
                          | EndsWithIn NeedToUpperColumnNames _ ->
                              sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        match v with
        | HasLength _ -> 
            _{1} <-v.ToUpper()
            x.IsChecked{1}<-new Nullable<bool>(true)
            x.Visibility{1}<-Visibility.Visible
            x.IsDefault{1}<-false
        | _ ->
            _{1} <-v
            x.IsChecked{1}<-new Nullable<bool>(false)
            x.Visibility{1}<-Visibility.Collapsed
        x.OnPropertyChanged ""{1}""",
                                //{0}
                                String.Empty
                                ,
                                //{1}
                                propertyName
                                )|>ignore
                          | _ ->
                              sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        match v with
        | HasLength _ -> 
            x.IsChecked{1}<-new Nullable<bool>(true)
            x.Visibility{1}<-Visibility.Visible
            x.IsDefault{1}<-false
        | _ ->
            x.IsChecked{1}<-new Nullable<bool>(false)
            x.Visibility{1}<-Visibility.Collapsed
        x.OnPropertyChanged ""{1}""",
                                //{0}
                                String.Empty
                                ,
                                //{1}
                                propertyName
                                )|>ignore
                      | _ ->
                          sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        match v with
        | HasLength _ -> 
            x.IsChecked{1}<-new Nullable<bool>(true)
            x.Visibility{1}<-Visibility.Visible
            x.IsDefault{1}<-false
        | _ ->
            x.IsChecked{1}<-new Nullable<bool>(false)
            x.Visibility{1}<-Visibility.Collapsed
        x.OnPropertyChanged ""{1}""",
                            //{0}
                            String.Empty
                            ,
                            //{1}
                            propertyName
                            )|>ignore
                | _ ->
                    sbTem.AppendFormat(@"{0}
  member x.{1} 
    with get ()=_{1} 
    and set v=
      if _{1}<>v then
        _{1} <-v
        match v with
        | y when y.HasValue  -> 
            x.IsChecked{1}<-new Nullable<bool>(true)
            x.Visibility{1}<-Visibility.Visible
            x.IsDefault{1}<-false
        | _ ->
            x.IsChecked{1}<-new Nullable<bool>(false)
            x.Visibility{1}<-Visibility.Collapsed
        x.OnPropertyChanged ""{1}""",
                      //{0}
                      String.Empty
                      ,
                      //{1}
                      propertyName
                      )|>ignore

            match propertyTypeStr.ToLowerInvariant() with
            | EndsWithIn QueryRangeTypeConditions x ->
                match x with
                | EndsWithIn DateTimeConditions _ ->
                    sbTem.AppendFormat(@"{0}
  member x.{1}Second 
    with get ()=_{1}Second 
    and set (v:{2})=
      match v with
      | y when y.HasValue->
          if _{1}.HasValue |>not then
            x.{1}<-y
            _{1}Second<-y
          elif  y.Value>=_{1}.Value |>not then 
            _{1}Second<-_{1}
          else _{1}Second<-y
          _{1}Second.Value
          |>fun a -> 
              if  a.Hour=0 && a.Minute=0 && a.Second=0 && a.Millisecond=0 then
                _{1}Second<-Nullable<DateTime>(a.AddDays(1.0).AddMilliseconds(-1.0))
      | y ->
          _{1}Second<-y
      x.OnPropertyChanged ""{1}Second""",
                        //{0}
                        String.Empty
                        ,
                        //{1}
                        propertyName
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
      match v with
      | y when y.HasValue->
          if _{1}.HasValue |>not then
            x.{1}<-y
            _{1}Second<-y
          elif  y.Value>=_{1}.Value |>not  then 
            _{1}Second<-_{1}
          else _{1}Second<-y
      | y ->
          _{1}Second<-y
      x.OnPropertyChanged ""{1}Second""",
                        //{0}
                        String.Empty
                        ,
                        //{1}
                        propertyName
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
            match propertyTypeStr with
            | EndsWithIn NullableTypeConditions _ ->
                sbTem.AppendFormat(@"{0}
  member x.IsChecked{1} 
    with get ()=_IsChecked{1} 
    and set v=
      if _IsChecked{1}<>v then
        match _{1} with
        | HasLength _ ->
            _IsChecked{1} <-v 
        | _ ->
            _IsChecked{1} <-new Nullable<bool>(false)
        x.OnPropertyChanged ""IsChecked{1}""",
                  //{0}
                  String.Empty
                  ,
                  //{1}
                  propertyName
                  )|>ignore
            | _ ->
                sbTem.AppendFormat(@"{0}
  member x.IsChecked{1} 
    with get ()=_IsChecked{1} 
    and set v=
      if _IsChecked{1}<>v then
        match _{1} with
        | y when y.HasValue ->
            _IsChecked{1} <-v 
        | _ ->
            _IsChecked{1} <-new Nullable<bool>(false)
        x.OnPropertyChanged ""IsChecked{1}""",
                  //{0}
                  String.Empty
                  ,
                  //{1}
                  propertyName
                  )|>ignore

            (*
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
                propertyName
                )|>ignore
            *)

            sbTem.ToString().Trim()
            )
            ,
            //{7} //生成 VisibilityC_MC, 属于客户端处理部分，所以不需要 [<DataMember>]
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
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
                propertyName
                )|>ignore
            sbTem.ToString().Trim()
            )
            ,
            //{8} //生成 IsDefaultC_MC, 属于客户端处理部分，所以不需要 [<DataMember>]
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  member x.IsDefault{1} 
    with get ()=_IsDefault{1} 
    and set v=_IsDefault{1} <-v",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              )|>ignore
            sbTem.ToString().Trim()
            )

          )|>ignore

//---------------------------------------------------------------------------
          sb.AppendLine()|>ignore
          sb.AppendLine()|>ignore
          sb.Append(@"
  //服务端部分") |>ignore
          //服务端代码
          sb.AppendFormat( @"{0}
  {1}
  {2}",
            //{0}
            String.Empty
            ,
            //{1}
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  let mutable _{1}:{2}={3}",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              ,
              //{2}
              match propertyTypeStr with
              | EndsWithIn NullableTypeConditions x ->x
              | x -> "System.Nullable<"+x+">"
              ,
              //{3}
              match propertyTypeStr with
              | EndsWithIn NullableTypeConditions _ ->"null"
              |  x -> "new System.Nullable<"+x+">()"
              )|>ignore

            sbTem.AppendFormat(@"{0}
  let mutable _IsQueryableNullOf{1}=false",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              )|>ignore

            //范围条件的第二个参数
            match propertyTypeStr.ToLowerInvariant() with
            | EndsWithIn QueryRangeTypeConditions _ ->
                sbTem.AppendFormat(@"{0}
  let mutable _{1}Second:{2}={3}",
                  //{0}
                  String.Empty
                  ,
                  //{1}
                  propertyName
                  ,
                  //{2}
                  match propertyTypeStr with
                  | EndsWithIn NullableTypeConditions x ->x
                  | x -> "System.Nullable<"+x+">"
                  ,
                  //{3}
                  match propertyTypeStr with
                  | EndsWithIn NullableTypeConditions _ ->"null"
                  |  x -> "new System.Nullable<"+x+">()"
                  )|>ignore
            | _ ->()

            sbTem.ToString().Trim()
            )
            ,
            //{2} //属性中的方法必须声明，否则不能在WCF中序列化
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.IsQueryableNullOf{1} 
    with get ()=_IsQueryableNullOf{1} 
    and set v=_IsQueryableNullOf{1}<-v",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              )|>ignore

            sbTem.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=_{1}<-v",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              )|>ignore

            match propertyTypeStr.ToLowerInvariant() with
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
                  propertyName
                  )|>ignore
            | _ ->()
            sbTem.ToString().Trim()
            )
          )|>ignore


//--------------------------------------------------------------------------

          sb.AppendLine()|>ignore
          sb.ToString()



(*  使用OnPropertyChanged
          //服务端代码
          sb.AppendFormat( @"{0}
  {1}
  {2}",
            //{0}
            String.Empty
            ,
            //{1}
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
            sbTem.AppendFormat(@"{0}
  let mutable _{1}:{2}={3}",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              ,
              //{2}
              match propertyTypeStr with
              | EndsWithIn NullableTypeConditions x ->x
              | x -> "System.Nullable<"+x+">"
              ,
              //{3}
              match propertyTypeStr with
              | EndsWithIn NullableTypeConditions _ ->"null"
              |  x -> "new System.Nullable<"+x+">()"
              )|>ignore

            sbTem.AppendFormat(@"{0}
  let mutable _IsQueryableNullOf{1}=false",
              //{0}
              String.Empty
              ,
              //{1}
              propertyName
              )|>ignore

            //范围条件的第二个参数
            match propertyTypeStr.ToLowerInvariant() with
            | EndsWithIn QueryRangeTypeConditions _ ->
                sbTem.AppendFormat(@"{0}
  let mutable _{1}Second:{2}={3}",
                  //{0}
                  String.Empty
                  ,
                  //{1}
                  propertyName
                  ,
                  //{2}
                  match propertyTypeStr with
                  | EndsWithIn NullableTypeConditions x ->x
                  | x -> "System.Nullable<"+x+">"
                  ,
                  //{3}
                  match propertyTypeStr with
                  | EndsWithIn NullableTypeConditions _ ->"null"
                  |  x -> "new System.Nullable<"+x+">()"
                  )|>ignore
            | _ ->()

            sbTem.ToString().Trim()
            )
            ,
            //{2} //属性中的方法必须声明，否则不能在WCF中序列化
            (
            sbTem.Remove(0,sbTem.Length)|>ignore
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
              propertyName
              )|>ignore

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
              propertyName
              )|>ignore

            match propertyTypeStr.ToLowerInvariant() with
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
                  propertyName
                  )|>ignore
            | _ ->()
            sbTem.ToString().Trim()
            )
          )|>ignore
*)