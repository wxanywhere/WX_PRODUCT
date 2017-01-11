namespace WX.Data.CodeAutomation

open System
open System.Linq
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data

//还应增加对主键值的验证
//还需加入MessageTemplate到验证中
type DataEntitiesCodingAdvanceWithoutVariable=
  //let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
  //tableAsFKRelationships|>PSeq.headOrDefault
  //For key validation

  static member GetCode (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.BusinessEntities
open System
open System.Collections.Generic
open System.Runtime.Serialization
open Microsoft.Practices.EnterpriseLibrary.Validation.Validators") |>ignore
    for a in tableRelatedInfos do
      DataEntitiesCodingAdvanceWithoutVariable.GetCodeWithTemplate a.TableName
      |>string |>sb.Append |>ignore
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels
      | ChildTable ->
          DataEntitiesCodingAdvanceWithoutVariable.GetChildRelationshipsCode a.LevelOneChildTableName
          |>string|>sb.Append|>ignore
      | LeafTable
      | IndependentTable  -> ()
      | _ ->()
//      sb.Append(@"
//  [<DV>]
//  val mutable _TrackingState:TrackingInfo
//  [<DataMember>]
//  member x.TrackingState
//    with get ()=x._TrackingState
//    and set v= x._TrackingState <-v")|>ignore
      sb.AppendLine()|>ignore
    string sb

  //需要加入某些复合验证？？？
  static member private GetChildRelationshipsCode (childTableName:string)=
    let sb=StringBuilder()
    sb.AppendFormat(@"
  [<DV>]{0}
  val mutable _BD_{1}s:List<BD_{1}>
  [<DataMember>]
  member x.BD_{1}s
    with get()=
      if x._BD_{1}s=null then 
        x._BD_{1}s<- List<BD_{1}>()
      x._BD_{1}s",
      //{0}
      String.Empty
      ,
      //{1}
      childTableName
      )|>ignore
    string sb

  static member private GetCodeWithTemplate (tableName:string)=
    let blankSapce="  "
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    sb.AppendFormat( @"
[<DataContract>]
type BD_{0}()=
  {1}
  {2}",
      //{0}
      tableName
      ,
      //{1}
      "inherit BD_EditBase()"
      ,
      //{2}
      (
      sbTem.Remove(0,sbTem.Length)|>ignore
      let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
      let tableKeyColumns=DatabaseInformation.GetPKColumns tableName

      for a,b,c in 
        DatabaseInformation.GetColumnSchemal4Way tableName
        |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        |>fun a->Query.groupJoin a tableKeyColumns (fun b->b.COLUMN_NAME) (fun c->c.COLUMN_NAME) (fun b d->b,d|>PSeq.headOrDefault)
        |>fun a->Query.groupJoin a tableAsFKRelationships (fun (b,c)->b.COLUMN_NAME) (fun d->d.FK_COLUMN_NAME) (fun (b,c) e->b,c,e|>PSeq.headOrDefault)
        do
        sbTem.AppendFormat(@"{0}
  [<DV>]
  val mutable _{1}:{2}
  [<DataMember>]{3}{4}{5}{6}{7}{8}{9}{10}
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
          | true,y when y.ToLowerInvariant().EndsWith("string") || y.EndsWith("[]") -> y
          | true,y -> "System.Nullable<"+y+">"
          | _,y ->y
          ,
          //{3}
          match a.IS_NULLABLE_TYPED,a.DATA_TYPE.ToLowerInvariant() with 
          | false,x when x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<NotNullValidator>]"
          | false,x when x.EndsWith("byte[]") -> Environment.NewLine+blankSapce+"[<NotNullValidator>]"
          | _ -> String.Empty
          ,
          //{4}
          match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH,a.IS_NULLABLE_TYPED with
          | x,y,false when x.EndsWith("byte[]") || x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+y.ToString()+")>]" //包括 Byte []
          | _ -> String.Empty
          ,
          //{5}
          match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH,a.IS_NULLABLE_TYPED with
          | x,y,true when x.EndsWith("byte[]") || x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<StringLengthValidator("+y.ToString()+")>]" //包括 Byte []
          | _ -> String.Empty
          ,
          //{6}
          match a.DATA_TYPE.ToLowerInvariant().EndsWith("datetime") with
          | true -> Environment.NewLine+blankSapce+ @"[<DateTimeRangeValidator(""2000-01-01T00:00:00"",""2099-01-01T00:00:00"")>]" //@"[<DateTimeRangeValidator(DateTime.Parse(""2000-01-01 00:00:00""),DateTime.Parse(""2099-01-01 00:00:00""))>]"
          | _ ->String.Empty 
          ,
          //{7}
          (* Right, not good
          match a.DATA_TYPE.ToLowerInvariant(),a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
          | x,  _,_ when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
          | x,  _,_ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int32.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Int32.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
          | x,  _,_ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int64.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Int64.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
          | x,  _,_ when x.EndsWith("double") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<Double,"""+Double.MinValue.ToString()+ @""",RangeBoundaryType.Exclusive,"""+Double.MaxValue.ToString()+ @""",RangeBoundaryType.Exclusive)>]"
          | x, y,z when x.EndsWith("decimal") -> 
                let maxValue=10.0**float y-10.0**float -z
                Environment.NewLine+blankSapce+"[<RangeValidator("+(-maxValue).ToString()+",RangeBoundaryType.Exclusive,"+maxValue.ToString()+",RangeBoundaryType.Exclusive)>]" //??
          | _ ->String.Empty
          *)
          match a.DATA_TYPE.ToLowerInvariant(),(a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE) with
          | x,  _ when x.EndsWith("byte") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<byte>,"""+string Byte.MinValue+ @""",RangeBoundaryType.Inclusive,"""+string Byte.MaxValue+ @""",RangeBoundaryType.Exclusive)>]"
          | x,  (y,z) when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
          | x,  _ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int32.MinValue+",RangeBoundaryType.Exclusive,"+string Int32.MaxValue+",RangeBoundaryType.Exclusive)>]"
          | x,  _ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int64.MinValue+",RangeBoundaryType.Exclusive,"+string Int64.MaxValue+",RangeBoundaryType.Exclusive)>]"
          | x,  _ when x.EndsWith("double") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<Double,"""+string Double.MinValue+ @""",RangeBoundaryType.Exclusive,"""+string Double.MaxValue+ @""",RangeBoundaryType.Exclusive)>]"
          | x, (y,z) when x.EndsWith("decimal") -> 
                let maxValueString=
                  match 10.0**float y-10.0**float -z with
                  | zx when z>0 ->zx |>string
                  | zx -> (zx|>string)  //+".0"
                Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<decimal>,""-"+maxValueString+ @""",RangeBoundaryType.Exclusive,"""+maxValueString+ @""",RangeBoundaryType.Exclusive)>]" //??
          | _ ->String.Empty
          ,
          //{8}, RegexValidator, for mail address...
          //http://msdn.microsoft.com/en-us/library/ms998267.aspx
          //http://www.regular-expressions.info/tutorial.html
          match a.COLUMN_NAME with
          | "C_DY" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"",MessageTemplate=""The mail format should be like name@x.com!"")>]" //@"[<RegexValidator(@""+([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
          | "C_LXDH" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^[\d\,\-]{5,}[\d]$"",MessageTemplate=""The phone number is not right,  may be it has some blanks !"")>]"   //Phone
          | "C_ZJID" -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^(\d{15}|\d{18})$"",MessageTemplate=""The ID number is  should be  15 or 18 digits !"")>]"   //证件ID
          | _ ->String.Empty
          ,
          //{9}
          (*
          //When it is not  a numeric
          NUMERIC_PRECISION = 0uy;
          NUMERIC_PRECISION_RADIX = -1s;
          NUMERIC_SCALE = -1;
          *)
          match a.DATA_TYPE.ToLowerInvariant(), a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
          | _,y,z when y> -1s ->String.Empty
          (*
              match z with
              | z when z>0 ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""(-)?\d{0,"+string y+ @"}\.\d{"+string z+ @"}$"",MessageTemplate=""The number should be like 0-"+  string (10.0**float y-1.0)+"."+string (10.0**float z-1.0) + @"!"")>]" 
              | z when z=0 ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""(-)?\d{1,"+string y+ @"}$"",MessageTemplate=""The number should be like  0-"+  string (10.0**float y-1.0)+ @"!"")>]" 
              | _ ->String.Empty
          *)
          | _ ->String.Empty
          ,
          //{10}
          match a.IS_NULLABLE_TYPED,a.DATA_TYPE,b,c with
          | false,x,_,_ when x.ToLower().EndsWith("guid") && (b.IsSome || c.IsSome) ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive)>]"   //Environment.NewLine+blankSapce+ @"[<RangeValidator(new Guid(""00000000-0000-0000-0000-000000000000""),RangeBoundaryType.Exclusive,new Guid(""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF""),RangeBoundaryType.Exclusive)>]" 
          | _ ->String.Empty

          )|>ignore
      sbTem.ToString().Trim()
      )
    ) |>ignore
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