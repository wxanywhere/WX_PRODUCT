namespace WX.Data.CodeAutomation

open System
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

//还应增加对主键值的验证
type DataEntitiesCoding=
  //For key validation
  static member GetCode tableName=
    let columnsSeq=
      DatabaseInformation.GetColumnSchemal2Way tableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)

    (*
    let columns=
      columnsSeq|>PSeq.toArray
      columns.Length
    ObjectDumper.Write(columns,0)
    *)
    let blankSapce="  "
    let sb=StringBuilder()
    sb.AppendFormat( @"namespace WX.Data.BusinessEntities

open System
open System.Collections.Generic
open System.Runtime.Serialization
open Microsoft.Practices.EnterpriseLibrary.Validation.Validators

[<DataContract>]
type BD_{0}()=",
      tableName) |>ignore

    for a in columnsSeq do
      sb.AppendFormat(@"{0}
  [<DefaultValue>]
  val mutable _{1}:{2}
  [<DataMember>]{3}{4}{5}{6}{7}{8}
  member x.{1} 
    with get ()=x._{1} 
    and set v= x._{1} <-v",
        //{0}
        String.Empty
        ,
        //{1}
        a.COLUMN_NAME
        ,
        //{2}
        match a.IS_NULLABLE_TYPED,a.DATA_TYPE with 
        | true,b when b.ToLowerInvariant().EndsWith("string") || b.EndsWith("[]") -> b
        | true,b -> "System.Nullable<"+b+">"
        | _,b ->b
        ,
        //{3}
        match a.IS_NULLABLE_TYPED with 
        | true ->String.Empty
        | _ -> Environment.NewLine+blankSapce+"[<NotNullValidator>]"
        ,
        //{4}
        match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH with
        | x,y when x.Contains("byte") || x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+y.ToString()+")>]" //包括 Byte []
        | _ -> String.Empty
        ,
        //{5}
        match a.DATA_TYPE.ToLowerInvariant().EndsWith("datetime") with
        | true -> Environment.NewLine+blankSapce+ @"[<DateTimeRangeValidator("""+DateTime.Parse("2000-01-01 00:00:00").ToString() + @""","""+DateTime.Parse("2099-01-01 00:00:00").ToString() + @""")>]"
        | _ ->String.Empty 
        ,
        //{6}
        (* Right, not good
        match a.DATA_TYPE.ToLowerInvariant(),a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
        | x,  _,_ when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0s,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
        | x,  _,_ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int32.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Int32.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
        | x,  _,_ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int64.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Int64.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
        | x,  _,_ when x.EndsWith("double") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Double.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Double.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
        | x, y,z when x.EndsWith("decimal") -> 
              let maxValue=10.0**float y-10.0**float -z
              Environment.NewLine+blankSapce+"[<RangeValidator("+(-maxValue).ToString()+",RangeBoundaryType.Exclusive,"+maxValue.ToString()+",RangeBoundaryType.Exclusive)>]" //??
        | _ ->String.Empty
        *)
        match a.DATA_TYPE.ToLowerInvariant(),(a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE) with
        | x,  (y,z) when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0s,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
        | x,  _ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int32.MinValue+",RangeBoundaryType.Exclusive,"+string Int32.MaxValue+",RangeBoundaryType.Exclusive)>]"
        | x,  _ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int64.MinValue+",RangeBoundaryType.Exclusive,"+string Int64.MaxValue+",RangeBoundaryType.Exclusive)>]"
        | x,  _ when x.EndsWith("double") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Double.MinValue+",RangeBoundaryType.Exclusive,"+string Double.MaxValue+",RangeBoundaryType.Exclusive)>]"
        | x, (y,z) when x.EndsWith("decimal") -> 
              let maxValueString=
                match 10.0**float y-10.0**float -z with
                | zx when z>0 ->zx |>string
                | zx -> (zx|>string)+".0"
              Environment.NewLine+blankSapce+"[<RangeValidator(-"+maxValueString+",RangeBoundaryType.Exclusive,"+maxValueString+",RangeBoundaryType.Exclusive)>]" //??
        | _ ->String.Empty
        ,
        //{7}, RegexValidator, for mail address...
        //http://msdn.microsoft.com/en-us/library/ms998267.aspx
        //http://www.regular-expressions.info/tutorial.html
        match a.COLUMN_NAME with
        | "C_DY" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""+([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
        //| "C_LXDH" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""+\d{5}$"")>]"   //Phone
        | "C_ZJID" -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""+\d{18}$"")>]"   //证件ID
        | _ ->String.Empty
        ,
        //{8}
        (*
        //When it is not  a numeric
        NUMERIC_PRECISION = 0uy;
        NUMERIC_PRECISION_RADIX = -1s;
        NUMERIC_SCALE = -1;
        *)
        match a.DATA_TYPE.ToLowerInvariant(), a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
        | x,y,z when y> -1s ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""+(-)?\d{"+string y+ @"}(\.\d{"+string z+ @"})?$"")>]" 
        | _ ->String.Empty
        )|>ignore
      //sb.Append(Environment.NewLine) |>ignore
      sb.AppendLine() |>ignore
      
    sb.ToString()
    
    

(*
//正则表达式语言元素-Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx 
*)
    
(* http://www.tzwhx.com/newOperate/html/1/11/114/16908.html
public const string DateTime =
//-------------日期部分是牛人写的,可以判断闰月
             "+\\s*((((1[6-9]|[2-9]\\d)\\d{2})-(0[13578]|1[02])-(0[1-9]|[12]\\d|3[01]))|(((1[6-9]|[2-9]\\d)\\d{2})-(0[469]|11)-(0 [1-9]|[12]\\d|30))|(((1[6-9]|[2-9]\\d)\\d{2})-02-(0[1-9]|1\\d|2[0-8]))|(((1[6-9]|[2-9]\\d)[13579][26])-02-29)|(((1[6-9]|[2-9]\\d)[2468][048])-02-29)|(((1[6-9]|[2-9]\\d)0[48])-02-29)|(([13579]6)00-02-29)|(([2468][048])00-02-29)|(([3579]2)00-02-29))" +
//----------------下面是时间部分,属于我的原创.虽然写的不好,但也可以用了.

               "(" +   //-->把时间和上面的日期隔开
                       "((T)|(\\s))(" + //-->时间开始 可以为空格 或者T
                             "([0-1]\\d)|([2][0-4])" + //-->小时
                             "([:][0-5][0-9]){2}" +    //分,秒,重复2次
                       ")\\s*" +
             ") $";   //-->问号表示时间可以为>=1

*)