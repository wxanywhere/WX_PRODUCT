﻿namespace WX.Data.CodeAutomation

open System
open System.Linq
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data.Helper
open WX.Data
open WX.Data.CodeAutomationHelper

(*
//格式化MessageTemplate 
http://msdn.microsoft.com/en-us/library/dd203228.aspx
*)

//还应增加对主键值的验证
//还需加入MessageTemplate到验证中
type BDCodingX=
  static member GetCode (tableRelatedInfos:TableRelatedInfoX[])=  
    let sb=StringBuilder()
    sb.Append( @"namespace WX.Data.BusinessEntities
open System
open System.Runtime.Serialization
open Microsoft.Practices.EnterpriseLibrary.Validation
open Microsoft.Practices.EnterpriseLibrary.Validation.Validators
open WX.Data
open WX.Data.BusinessBase") |>ignore
    for n in tableRelatedInfos do
      match  n.TableTemplateType  with
      | DJLSHTable
      | LSHTable
      | PCLSHTable
      | JYLSHTable ->()  //流水号表不生成代码
      | _ ->
          BDCodingX.GetCodeWithTemplateAndConditions n
          |>string |>sb.Append |>ignore

          match n.TableTemplateType with
          | MainTableWithOneLevel
          | MainTableWithTwoLevels
          | ChildTable ->
              BDCodingX.GetChildRelationshipsCode n.LevelOneChildTableInfo.TABLE_NAME
              |>string|>sb.Append|>ignore
          | LeafTable
          | IndependentTable  -> ()
          | _ ->()
          sb.AppendLine()|>ignore
    string sb

  //需要加入某些复合验证？？？
  static member private GetChildRelationshipsCode (childTableName:string)=
    let sb=StringBuilder()
    sb.AppendFormat(@"
  [<DV>]{0}
  val mutable private _BD_{1}s:BD_{1}[]
  [<DataMember>]
  member x.BD_{1}s
    with get()=x._BD_{1}s
    and set v=
      if x._BD_{1}s<>v then 
        x._BD_{1}s<-v",
      //{0}
      String.Empty
      ,
      //{1}
      childTableName
      )|>ignore
    string sb

  (*
  已停用
  *)
  static member private GetCodeWithTemplateBackup (tableRelatedInfo:TableRelatedInfoX)=
    let blankSapce="  "
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    sb.AppendFormat( @"
[<DataContract>]
type BD_{0}=
  inherit BD_EditBase
  new ()={inherit BD_EditBase()}  
  new (businessBase){1}={inherit BD_EditBase(businessBase)} {2} 
  {3}",
      //{0}
      tableRelatedInfo.TableInfo.TABLE_NAME
      ,
      //{1}
      match 
        tableRelatedInfo.TableInfo.TableColumnInfos
        |>PSeq.exists (fun a->a.COLUMN_NAME="C_FBID" && a.DATA_TYPE.ToLowerInvariant().EndsWith("guid"))
        with
      | true -> " as x" 
      | _ -> String.Empty
      ,
      //{2}
      match 
        tableRelatedInfo.TableInfo.TableColumnInfos
        |>PSeq.exists (fun a->a.COLUMN_NAME="C_FBID" && a.DATA_TYPE.ToLowerInvariant().EndsWith("guid"))
        with
      | true -> "then x.C_FBID<- businessBase.C_FBIDBase" 
      | _ -> String.Empty
      ,
      //{3}
      (
      sbTem.Remove(0,sbTem.Length)|>ignore
      let tableAsFKRelationships=tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos//获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
      let tableKeyColumns=tableRelatedInfo.TableInfo.TablePrimaryKeyInfos

      tableRelatedInfo.TableInfo.TableColumnInfos
      |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
      |>fun a->Query.groupJoin a tableKeyColumns (fun b->b.COLUMN_NAME) (fun c->c.COLUMN_NAME) (fun b d->b,d|>PSeq.headOrDefault)
      |>fun a->Query.groupJoin a tableAsFKRelationships (fun (b,c)->b.COLUMN_NAME) (fun d->d.FK_COLUMN_NAME) (fun (b,c) e->b,c,e|>PSeq.headOrDefault)
      |>fun z ->
          for a,b,c in z do
            sbTem.AppendFormat(@"{0}
  [<DV>]
  val mutable private _{1}:{2}
  [<DataMember>]
  [<TableField>]{3}{4}{5}{6}{7}{8}{9}{10}
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
              //| true,y when y.ToLowerInvariant().EndsWith("string") || y.EndsWith("[]") -> y  //Right backup
              | true, EndsWithIn ["string";"[]"]  y  -> y
              | true,y -> "System.Nullable<"+y+">"
              | _,y ->y
              ,
              //{3}
              match a.IS_NULLABLE_TYPED,a.DATA_TYPE.ToLowerInvariant() with 
              | false,x when x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<NotNullValidator>]"
              | false,x when x.EndsWith("byte[]") -> Environment.NewLine+blankSapce+"[<NotNullValidator>]"
              | true, _ -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]"
              (*
              | true,x when x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]"
              | true,x when x.EndsWith("byte[]") -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]"
              *)
              | _ -> String.Empty
              ,
              //{4}
              match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH,a.IS_NULLABLE_TYPED with
              | EndsWithIn ["byte[]";"string"] _, y, false ->
                  match y with
                  | -1 ->Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+Int32.MaxValue.ToString()+")>]"  //针对xml字段
                  | _ ->Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+y.ToString()+")>]" //包括 Byte []
              | _ -> String.Empty
              ,
              //{5}
              match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH,a.IS_NULLABLE_TYPED with
              | EndsWithIn ["byte[]";"string"] _, y, true ->
                  match y with
                  | -1 ->Environment.NewLine+blankSapce+"[<StringLengthValidator("+Int32.MaxValue.ToString()+")>]"  //针对xml字段
                  | _ ->Environment.NewLine+blankSapce+"[<StringLengthValidator("+y.ToString()+")>]"
                  (*
                  //部分正确的备份
                  Environment.NewLine+blankSapce+ @"[<ValidatorComposition(CompositionType.Or,MessageTemplate=""The string characters can not more than "+y.ToString()+  @" !"")>]"+  //包括 Byte []
                  Environment.NewLine+blankSapce+"[<StringLengthValidator("+y.ToString()+")>]"+  //包括 Byte []
                  Environment.NewLine+blankSapce+"[<NotNullValidator(Negated=true)>]" //包括 Byte []
                  *)
              | _ -> String.Empty
              ,
              //{6}
              match a.DATA_TYPE.ToLowerInvariant().EndsWith("datetime"),a.COLUMN_NAME with
              | true,x when x.Equals("C_GXRQ")|>not && x.Equals("C_CJRQ")|>not  -> Environment.NewLine+blankSapce+ @"[<DateTimeRangeValidator(""2000-01-01T00:00:00"",""2099-01-01T00:00:00"")>]" //@"[<DateTimeRangeValidator(DateTime.Parse(""2000-01-01 00:00:00""),DateTime.Parse(""2099-01-01 00:00:00""))>]"
              | _ ->String.Empty 
              ,
              //{7}
              (* Right, not good
              match a.DATA_TYPE.ToLowerInvariant(),a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
              | x,  _,_ when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
              | x,  _,_ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int32.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Int32.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
              | x,  _,_ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int64.MinValue.ToString()+"L,RangeBoundaryType.Exclusive,"+Int64.MaxValue.ToString()+"L,RangeBoundaryType.Exclusive)>]"
              | x,  _,_ when x.EndsWith("double") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<Double,"""+Double.MinValue.ToString()+ @""",RangeBoundaryType.Exclusive,"""+Double.MaxValue.ToString()+ @""",RangeBoundaryType.Exclusive)>]"
              | x, y,z when x.EndsWith("decimal") -> 
                    let maxValue=10.0**float y-10.0**float -z
                    Environment.NewLine+blankSapce+"[<RangeValidator("+(-maxValue).ToString()+",RangeBoundaryType.Exclusive,"+maxValue.ToString()+",RangeBoundaryType.Exclusive)>]" //??
              | _ ->String.Empty
              *)
              match a.DATA_TYPE.ToLowerInvariant(),(a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE) with
              | x,  _ when x.EndsWith("byte") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<byte>,"""+string Byte.MinValue+ @""",RangeBoundaryType.Exclusive,"""+string Byte.MaxValue+ @""",RangeBoundaryType.Exclusive)>]"
              | x,  (y,z) when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
              | x,  _ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int32.MinValue+",RangeBoundaryType.Exclusive,"+string Int32.MaxValue+",RangeBoundaryType.Exclusive)>]"
              | x,  _ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int64.MinValue+"L,RangeBoundaryType.Exclusive,"+string Int64.MaxValue+"L,RangeBoundaryType.Exclusive)>]"
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
              match a.IS_NULLABLE_TYPED,a.COLUMN_NAME with
              | false, "C_DY" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"",MessageTemplate=""The mail format should be like name@x.com!"")>]" //@"[<RegexValidator(@""+([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
              | false, "C_LXDH" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^[\d\,\-]{5,}[\d]$"",MessageTemplate=""The phone number is not right,  may be it has some blanks !"")>]"   //Phone
              | false, "C_ZJID" -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^(\d{15}|\d{18})$"",MessageTemplate=""The ID number is  should be  15 or 18 digits !"")>]"   //证件ID
              | true, "C_DY" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$))"",MessageTemplate=""The mail format should be like name@x.com!"")>]" //@"[<RegexValidator(@""+([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
              | true, "C_LXDH" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^[\d\,\-]{5,}[\d]$))"",MessageTemplate=""The phone number is not right,  may be it has some blanks !"")>]"   //Phone
              | true, "C_ZJID" -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^(\d{15}|\d{18})$))"",MessageTemplate=""The ID number is  should be  15 or 18 digits !"")>]"   //证件ID
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
              //| false,x,y,z when x.ToLower().EndsWith("guid") && (b<>Unchecked.defaultof<TablePrimaryKeyInfo> || c<>Unchecked.defaultof<TableForeignKeyRelationshipInfo>) ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive)>]"   //Environment.NewLine+blankSapce+ @"[<RangeValidator(new Guid(""00000000-0000-0000-0000-000000000000""),RangeBoundaryType.Exclusive,new Guid(""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF""),RangeBoundaryType.Exclusive)>]" 
              //| _, EndsWithIn ["guid"] _, _, _ when b.IsSome || c.IsSome ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive)>]"   //Environment.NewLine+blankSapce+ @"[<RangeValidator(new Guid(""00000000-0000-0000-0000-000000000000""),RangeBoundaryType.Exclusive,new Guid(""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF""),RangeBoundaryType.Exclusive)>]" 
              | _, EndsWithIn ["guid"] _, _, _  ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive)>]"   //没有主键外键的也需要验证
              | _ ->String.Empty

              )|>ignore
      sbTem.ToString().Trim()
      )
    ) |>ignore
    sb.ToString()
    
  static member private GetCodeWithTemplateAndConditions (tableRelatedInfo:TableRelatedInfoX)=
    let blankSapce="  "
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSecond=StringBuilder()
    let tableColumns=
      tableRelatedInfo.TableInfo.TableColumnInfos
      |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    sb.AppendFormat( @"
[<DataContract>]
type BD_{0}=
  inherit BD_EditBase
  new () = {{inherit BD_EditBase()}}  
  new (businessBase){1} = {{inherit BD_EditBase (businessBase)}} {2} 
  {3}

  {4}",
      //{0}
      tableRelatedInfo.TableInfo.TABLE_NAME
      ,
      //{1}
      String.Empty
      ,
      //{2}
      String.Empty
      ,
      //{3}
      (*
      本来可用类型构造函数替代该方法，不过由于使用[<DataContract>], new (...) as ...将会引起问题an object or value being accessed recursively before it was fully initialized
  new (instance:BD_T_XZX) as x={inherit BD_EditBase()} then
    match instance with
    | y -> 
        x.C_GZX<-y.C_GZX
        ...
      *)
      (
      sbTem.Clear()|>ignore
      sbTem.AppendFormat(@"
  new (instance:{0}) as x = {{inherit BD_EditBase (instance)}} then
    match instance with
    | y ->
        {1}",
        //{0}
        "BD_"+tableRelatedInfo.TableInfo.TABLE_NAME,
        //{1}
        (
        sbTemSecond.Clear()|>ignore
        for m in tableColumns do 
          sbTemSecond.AppendFormat (@"
        x.{0}<-y.{0}",
            //{0}
            m.COLUMN_NAME
            )|>ignore
        sbTemSecond.ToString().Trim()
        )
        )|>ignore
      sbTem.ToString().Trim()
      (*
      sbTem.Clear()|>ignore
      //当前由于new () as...的问题，将来应该改为构造函数方式
      sbTem.AppendFormat(@"
  member x.CloneFrom (instance:{0})=
    match instance with
    | y ->
        {1}",
        //{0}
        "BD_"+tableName,
        //{1}
        (
        sbTemSecond.Clear()|>ignore
        for m in tableColumns do 
          sbTemSecond.AppendFormat (@"
        x.{0}<-y.{0}",
            //{0}
            m.COLUMN_NAME
            )|>ignore
        sbTemSecond.ToString().Trim()
        )
        )|>ignore
      sbTem.ToString().Trim()
      *)
      ),
      //{4}
      (
      sbTem.Remove(0,sbTem.Length)|>ignore
      let tableAsFKRelationships=tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
      let tableKeyColumns=tableRelatedInfo.TableInfo.TablePrimaryKeyInfos

      tableColumns
      |>fun a->Query.groupJoin a tableKeyColumns (fun b->b.COLUMN_NAME) (fun c->c.COLUMN_NAME) (fun b d->b,d|>PSeq.headOrDefault)
      |>fun a->Query.groupJoin a tableAsFKRelationships (fun (b,c)->b.COLUMN_NAME) (fun d->d.FK_COLUMN_NAME) (fun (b,c) e->b,c,e|>PSeq.headOrDefault)
      |>fun z ->
          for a,b,c in z do
            if a.COLUMN_NAME<>"C_FBID" then //C_FBID统一设计到BD_EditBase中, 部门数据库中将取消所有"C_FBID"字段，以满足分布式设计的要求 
              sbTem.AppendFormat(@"
  [<DV>]
  val mutable private _{1}:{2}
  [<DataMember>]
  [<TableField>]{3}{4}{5}{6}{7}{8}{9}{10}
  member x.{1} 
    with get ()=x._{1} 
    and set v=
      if  x._{1}<>v  then
        x._{1} <- v
        x.OnPropertyChanged ""{1}""
        {0}",
                //{0}
                match  a.IS_NULLABLE_TYPED,a.DATA_TYPE.ToLowerInvariant(), a.COLUMN_NAME with
                | false, EndsWithIn [StringTypeName] _,  y ->
                    match z|>PSeq.exists (fun (d,_,_) ->d.IS_NULLABLE_TYPED=false && d.COLUMN_NAME=y+JMColumnSuffix && d.DATA_TYPE.ToLowerInvariant().EndsWith StringTypeName) with
                    | true ->String.Format ("x.{0}<-Comm.toChinesePY v",y+JMColumnSuffix)
                    | _ ->String.Empty
                | true, EndsWithIn [StringTypeName] _, NotEndsWithIn [JMColumnSuffix] y ->
                    match z|>PSeq.exists (fun (d,_,_) ->d.IS_NULLABLE_TYPED=true && d.COLUMN_NAME=y+JMColumnSuffix && d.DATA_TYPE.ToLowerInvariant().EndsWith StringTypeName) with
                    | true ->String.Format ("x.{0}<-Comm.toChinesePY v",y+JMColumnSuffix)
                    | _ ->String.Empty
                | _ ->String.Empty
                (*
                | false, x, y  when  x.EndsWith("string") && y="C_MC" && z|>PSeq.exists (fun (d,_,_) ->d.IS_NULLABLE_TYPED=false && d.DATA_TYPE.ToLowerInvariant().EndsWith"string" && d.COLUMN_NAME="C_MCJM")->
                    "x.C_MCJM<-Comm.toChinesePY v"
                | false, x, y  when  x.EndsWith("string") && y="C_XM" && z|>PSeq.exists (fun (d,_,_) ->d.IS_NULLABLE_TYPED=false && d.DATA_TYPE.ToLowerInvariant().EndsWith"string" && d.COLUMN_NAME="C_XMJM")->
                    "x.C_XMJM<-Comm.toChinesePY v"
                | _ ->String.Empty
                *)
                ,
                //{1}
                a.COLUMN_NAME
                ,
                //{2}
                match a.IS_NULLABLE_TYPED,a.DATA_TYPE with 
                //| true,y when y.ToLowerInvariant().EndsWith("string") || y.EndsWith("[]") -> y //Right
                | true, EndsWithIn ["string";"[]"]  y  -> y
                | true,y -> "System.Nullable<"+y+">"
                | _,y ->y
                ,
                //{3}
                match a.IS_NULLABLE_TYPED,a.DATA_TYPE.ToLowerInvariant(),a.COLUMN_NAME with 
                | false,x,y when x.EndsWith("string") &&  y<>DJHColumnName  -> Environment.NewLine+blankSapce+ @"[<NotNullValidator(MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" can not be null"")>]"
                | false,x,y when x.EndsWith("string") &&  y=DJHColumnName  -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]" //单据号在后台生成
                | false,x,_ when x.EndsWith("byte[]") -> Environment.NewLine+blankSapce+ @"[<NotNullValidator(MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" can not be null"")>]"
                | true, _,_ -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]"
                (*
                | true,x when x.EndsWith("string") -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]"
                | true,x when x.EndsWith("byte[]") -> Environment.NewLine+blankSapce+"[<IgnoreNulls>]"
                *)
                | _ -> String.Empty
                ,
                //{4}
                match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH,a.IS_NULLABLE_TYPED with 
                | EndsWithIn ["string"] _,y,false  -> 
                    match y with
                    | -1 ->Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+Int32.MaxValue.ToString()+ @",MessageTemplate= @""The length of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"   //针对xml字段
                    | _ ->Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+y.ToString()+ @",MessageTemplate= @""The length of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]" 
                | EndsWithIn ["byte[]"] _,y,false -> Environment.NewLine+blankSapce+"[<StringLengthValidator(1,"+y.ToString()+ @",MessageTemplate= @""The length of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]" //包括 Byte []
                | _ -> String.Empty
                ,
                //{5}
                match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH,a.IS_NULLABLE_TYPED with
                //| x,y,true when x.EndsWith("byte[]") || x.EndsWith("string") -> 
                | EndsWithIn ["byte[]";"string"] _ ,y,true ->  
                    match y with
                    | -1 ->Environment.NewLine+blankSapce+"[<StringLengthValidator("+Int32.MaxValue.ToString()+ @",MessageTemplate= @""The length of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]" //针对xml字段
                    | _ ->Environment.NewLine+blankSapce+"[<StringLengthValidator("+y.ToString()+ @",MessageTemplate= @""The length of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"
                    (*
                    //部分正确的备份
                    Environment.NewLine+blankSapce+ @"[<ValidatorComposition(CompositionType.Or,MessageTemplate=""The string characters can not more than "+y.ToString()+  @" !"")>]"+  //包括 Byte []
                    Environment.NewLine+blankSapce+"[<StringLengthValidator("+y.ToString()+")>]"+  //包括 Byte []
                    Environment.NewLine+blankSapce+"[<NotNullValidator(Negated=true)>]" //包括 Byte []
                    *)
                | _ -> String.Empty
                ,
                //{6}
                match a.DATA_TYPE.ToLowerInvariant().EndsWith("datetime"),a.COLUMN_NAME with
                | true,x when x.Equals("C_GXRQ")|>not && x.Equals("C_CJRQ")|>not  -> Environment.NewLine+blankSapce+ @"[<DateTimeRangeValidator(""2000-01-01T00:00:00"",""2099-01-01T00:00:00"",MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]" //@"[<DateTimeRangeValidator(DateTime.Parse(""2000-01-01 00:00:00""),DateTime.Parse(""2099-01-01 00:00:00""))>]"
                | _ ->String.Empty 
                ,
                //{7}
                (* Right, not good
                match a.DATA_TYPE.ToLowerInvariant(),a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
                | x,  _,_ when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
                | x,  _,_ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int32.MinValue.ToString()+",RangeBoundaryType.Exclusive,"+Int32.MaxValue.ToString()+",RangeBoundaryType.Exclusive)>]"
                | x,  _,_ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+Int64.MinValue.ToString()+"L,RangeBoundaryType.Exclusive,"+Int64.MaxValue.ToString()+"L,RangeBoundaryType.Exclusive)>]"
                | x,  _,_ when x.EndsWith("double") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<Double>,"""+Double.MinValue.ToString()+ @""",RangeBoundaryType.Exclusive,"""+Double.MaxValue.ToString()+ @""",RangeBoundaryType.Exclusive)>]"
                | x, y,z when x.EndsWith("decimal") -> 
                      let maxValue=10.0**float y-10.0**float -z
                      Environment.NewLine+blankSapce+"[<RangeValidator("+(-maxValue).ToString()+",RangeBoundaryType.Exclusive,"+maxValue.ToString()+",RangeBoundaryType.Exclusive)>]" //??
                | _ ->String.Empty
                *)
                match a.DATA_TYPE.ToLowerInvariant(),(a.NUMERIC_PRECISION,a.NUMERIC_SCALE) with
                | x,  _ when x.EndsWith("byte") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<byte>,"""+string Byte.MinValue+ @""",RangeBoundaryType.Exclusive,"""+string Byte.MaxValue+ @""",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"   //{0}参数为输入的值，暂不用；{3}为属性名，暂不用}    
                | x,  (y,z) when x.EndsWith("int16") -> Environment.NewLine+blankSapce+"[<RangeValidator(0,RangeBoundaryType.Inclusive,"+Int16.MaxValue.ToString()+ @",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"
                | x,  _ when x.EndsWith("int32") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int32.MinValue+",RangeBoundaryType.Exclusive,"+string Int32.MaxValue+ @",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"
                | x,  _ when x.EndsWith("int64") -> Environment.NewLine+blankSapce+"[<RangeValidator("+string Int64.MinValue+"L,RangeBoundaryType.Exclusive,"+string Int64.MaxValue+ @"L,RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"
                | x,  _ when x.EndsWith("double") -> Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<Double>,"""+string Double.MinValue+ @""",RangeBoundaryType.Exclusive,"""+string Double.MaxValue+ @""",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"
                | x, (y,z) when x.EndsWith("decimal") -> 
                      let maxValueString=
                        match 10.0**float (int y-z)-10.0**float -z with
                        | x ->string x
                      Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<decimal>,""-"+maxValueString+ @""",RangeBoundaryType.Exclusive,"""+maxValueString+ @""",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]" //??
                | _ ->String.Empty
                ,
                //{8}, RegexValidator, for mail address...
                //http://msdn.microsoft.com/en-us/library/ms998267.aspx
                //http://www.regular-expressions.info/tutorial.html
                match a.IS_NULLABLE_TYPED,a.DATA_TYPE.ToLowerInvariant(),a.COLUMN_NAME with
                | false, EndsWithIn [StringTypeName] _, "C_DY" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$"",MessageTemplate=""The"+a.COLUMN_DESCRIPTION+ @" should be like name@x.com!"")>]" //@"[<RegexValidator(@""+([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
                | false, EndsWithIn [StringTypeName] _, "C_LXDH" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^[\d\,\-]{5,}[\d]$"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" is not right,  may be it has some blanks !"")>]"   //Phone
                | false, EndsWithIn [StringTypeName] _, "C_ZJID" -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""^(\d{15}|\d{18})$"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" is  should be  15 or 18 digits !"")>]"   //证件ID
                | false, EndsWithIn [StringTypeName] _, EndsWithIn [JMColumnSuffix] _ -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^[\w\d]{1,}$))"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" should be  Letters or digital !"")>]"   //名称简码 
                | false, EndsWithIn [StringTypeName] _,  w when w=DJHColumnName -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^\w{2}[23456]\d{3}[01]\d[0123]\d\d{3,}$))"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" should be like XX201001100001 !"")>]"   //名称简码 
                | true, EndsWithIn [StringTypeName] _,  "C_DY" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$))"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" should be like name@x.com!"")>]" //@"[<RegexValidator(@""+([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
                | true, EndsWithIn [StringTypeName] _,  "C_LXDH" ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^[\d\,\-]{5,}[\d]$))"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" is not right,  may be it has some blanks !"")>]"   //Phone
                | true, EndsWithIn [StringTypeName] _,  "C_ZJID" -> Environment.NewLine+blankSapce+ @"[<RegexValidator(@""((^$)|(^(\d{15}|\d{18})$))"",MessageTemplate=""The "+a.COLUMN_DESCRIPTION+ @" is  should be  15 or 18 digits !"")>]"   //证件ID
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
                (*  @"^\[+-]?[\d{0,16}]*\.?\d{0,2}$", 
                    match z with
                    | z when z>0 ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""(-)?\d{0,"+string y+ @"}\.\d{"+string z+ @"}$"",MessageTemplate=""The number should be like 0-"+  string (10.0**float y-1.0)+"."+string (10.0**float z-1.0) + @"!"")>]" 
                    | z when z=0 ->  Environment.NewLine+blankSapce+ @"[<RegexValidator(@""(-)?\d{1,"+string y+ @"}$"",MessageTemplate=""The number should be like  0-"+  string (10.0**float y-1.0)+ @"!"")>]" 
                    | _ ->String.Empty
                *)
                | _ ->String.Empty
                ,
                //{10}
                match a.IS_NULLABLE_TYPED,a.DATA_TYPE,b,c with
                //| false,x,y,z when x.ToLower().EndsWith("guid") && (b<>Unchecked.defaultof<TablePrimaryKeyInfo> || c<>Unchecked.defaultof<TableForeignKeyRelationshipInfo>) ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive)>]"   //Environment.NewLine+blankSapce+ @"[<RangeValidator(new Guid(""00000000-0000-0000-0000-000000000000""),RangeBoundaryType.Exclusive,new Guid(""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF""),RangeBoundaryType.Exclusive)>]" 
                //| _,EndsWithIn ["guid"] _ ,_ ,_ when b.IsSome || c.IsSome ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"   //Environment.NewLine+blankSapce+ @"[<RangeValidator(new Guid(""00000000-0000-0000-0000-000000000000""),RangeBoundaryType.Exclusive,new Guid(""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF""),RangeBoundaryType.Exclusive)>]"       //(ObjectDumper.Write (a.TABLE_NAME+a.COLUMN_NAME);
                | _,EndsWithIn ["guid"] _ ,_ ,_ ->Environment.NewLine+blankSapce+ @"[<RangeValidator(typeof<System.Guid>,""00000000-0000-0000-0000-000000000000"",RangeBoundaryType.Exclusive,""FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"",RangeBoundaryType.Exclusive,MessageTemplate= @""The value of "+a.COLUMN_DESCRIPTION+ @" must fall within the range """"{3}"""" ({4}) - """"{5}"""" ({6}){2}"")>]"   //没有主键外键的也需要验证
                | _ ->String.Empty

                )|>ignore
      sbTem.ToString().Trim()
      )
    ) |>ignore
    sb.ToString()
