namespace WX.Data

open System
open System.Collections
open System.IO
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.CodeAutomation

[<AutoOpen>]
module ActiveModule=
  (*已移植到WX.Data.FModule
  let (|EndsWithIn|_|) (conditionElements:string seq) (input:string) =
    if conditionElements|>PSeq.exists (fun a->input.ToLowerInvariant().EndsWith(a)) then Some(input)
    else None
  *)

//---------------------------------------------------
  let (|ColumnConditionTypeContains|_|) (columnConditionTypes:ColumnConditionType seq) (input:ColumnConditionType seq)=
    if columnConditionTypes|>PSeq.forall (fun a->input|>PSeq.exists (fun b->b=a)) then Some input
    else None

  let (|ColumnConditionTypeNotContains|_|) (columnConditionTypes:ColumnConditionType seq) (input:ColumnConditionType seq)=
    if columnConditionTypes|>PSeq.forall (fun a->input|>PSeq.exists (fun b->b=a) |>not) then Some input
    else None

  let (|ColumnConditionTypeAllEquals|_|) (columnConditionTypes:ColumnConditionType seq) (input:ColumnConditionType seq)=
    if columnConditionTypes|>PSeq.forall (fun a->input|>PSeq.exists (fun b->b=a)) 
      && input|>PSeq.forall (fun a->columnConditionTypes|>PSeq.exists (fun b->b=a)) then Some input
    else None

//---------------------------------------------------

  let (|TableRelationshipTypeContains|_|) (tableRelationshipTypes:TableRelationshipType seq) (input:TableRelationshipType seq)=
    if tableRelationshipTypes|>PSeq.forall (fun a->input|>PSeq.exists (fun b->b=a)) then Some input
    else None

  let (|TableRelationshipTypeNotContains|_|) (tableRelationshipTypes:TableRelationshipType seq) (input:TableRelationshipType seq)=
    if tableRelationshipTypes|>PSeq.forall (fun a->input|>PSeq.exists (fun b->b=a) |>not) then Some input
    else None

  let (|TableRelationshipTypeAllEquals|_|) (tableRelationshipTypes:TableRelationshipType seq) (input:TableRelationshipType seq)=
    if tableRelationshipTypes|>PSeq.forall (fun a->input|>PSeq.exists (fun b->b=a)) 
      && input|>PSeq.forall (fun a->tableRelationshipTypes|>PSeq.exists (fun b->b=a)) then Some input
    else None

//---------------------------------------------------


  let (|IsNullOrZeroLength|_|)  (input:string) =
    if String.IsNullOrWhiteSpace input then Some(input)
    else None

(*已移植到WX.Data.FModule.Common
module StringModule=
  let EndsWithIn (conditionElements:string list) (input:string) =
    if conditionElements|>PSeq.exists (fun a->input.ToLowerInvariant().EndsWith(a)) then true
    else false
*)

[<AutoOpen>]
module ConstantModule=
  let AutoVariableNames=
    ['x';'y';'z';'u';'v';'w';'a';'b';'c']

  let TypeShortNames=
    [|"string";"bool";"byte";"int";"int16";"int32";"int64";"double";"single";"decimal";
    "String";"Boolean";"Byte";"Int16";"Int32";"Int64";"Double";"Single";"Decimal";"DateTime";"Guid";
    "string[]";"bool[]";"byte[]";"int[]";"int16[]";"int32[]";"int64[]";"double[]";"single[]";"decimal[]";
    "String[]";"Boolean[]";"Byte[]";"Int16[]";"Int32[]";"Int64[]";"Double[]";"Single[]";"Decimal[]";"DateTime[]";"Guid[]"|]
  let TypeShortLongDic=
    let dic=new Generic.Dictionary<string,string>()
    dic.Add("string","System.String")
    dic.Add("bool","System.Boolean")
    dic.Add("byte","System.Byte")
    dic.Add("int","System.Int32")
    dic.Add("int16","System.Int16")
    dic.Add("int32","System.Int32")
    dic.Add("int64","System.Int64")
    dic.Add("double","System.Double")
    dic.Add("single","System.Single")
    dic.Add("decimal","System.Decimal")
    dic.Add("String","System.String")
    dic.Add("Boolean","System.Boolean")
    dic.Add("Byte","System.Byte")
    dic.Add("Int16","System.Int16")
    dic.Add("Int32","System.Int32")
    dic.Add("Int64","System.Int64")
    dic.Add("Double","System.Double")
    dic.Add("Single","System.Single")
    dic.Add("Decimal","System.Decimal")
    dic.Add("DateTime","System.DateTime")
    dic.Add("Guid","System.Guid")
    dic.Add("string[]","System.String[]")
    dic.Add("bool[]","System.Boolean[]")
    dic.Add("byte[]","System.Byte[]")
    dic.Add("int[]","System.Int32[]")
    dic.Add("int16[]","System.Int16[]")
    dic.Add("int32[]","System.Int32[]")
    dic.Add("int64[]","System.Int64[]")
    dic.Add("double[]","System.Double[]")
    dic.Add("single[]","System.Single[]")
    dic.Add("decimal[]","System.Decimal[]")
    dic.Add("String[]","System.String[]")
    dic.Add("Boolean[]","System.Boolean[]")
    dic.Add("Byte[]","System.Byte[]")
    dic.Add("Int16[]","System.Int16[]")
    dic.Add("Int32[]","System.Int32[]")
    dic.Add("Int64[]","System.Int64[]")
    dic.Add("Double[]","System.Double[]")
    dic.Add("Single[]","System.Single[]")
    dic.Add("Decimal[]","System.Decimal[]")
    dic.Add("DateTime[]","System.DateTime[]")
    dic.Add("Guid[]","System.Guid[]")
    dic
  let TypeNameInt16 ="System.Int16"
  let TypeNameInt32="System.Int32"
  let TypeNameInt64 = "System.Int64"
  let TypeNameDecimal="System.Decimal"
  let TypeNameSingle="System.Single"
  let TypeNameDouble="System.Double"
  let TypeNameByte="System.Byte"
  let TypeNameBytes=  "System.Byte[]"
  let TypeNameBool ="System.Boolean"
  let TypeNameString="System.String"
  let TypeNameDateTime="System.DateTime"
  let TypeNameGuid="System.Guid"
  let TypeNameObject="System.Object"
  let FuzzyQueryConditions=["string"]
  let ContainQueryConditions=["C_LXDH";"C_LXDZ"] //包含条件，联系电话，联系地址
  let NullableTypeConditions= ["string";"[]"]
  let QueryRangeTypeConditions= ["byte";"int";"int16";"int32";"int64";"double";"single";"decimal";"datetime"]
  let DateTimeConditions=["datetime"]
  let DateTimeTypeName="datetime"
  let StringTypeName="string"
  let DecimalTypeName="decimal"
  let GuidTypeName="guid"
  let BoolTypeName="boolean"
  let GuidConditions=["guid"]
  let DateTimeGuidConditions=["datetime";"guid"]
  let StringConditions=["string"]
  let NeedToUpperColumnNames=["JM"]  //NeedToUpperColumnNames=["C_MCJM";"C_XMJM"]
  let CreateDateColumnName="C_CJRQ"
  let UpdateDateColumnName="C_GXRQ"
  //特殊的业务字段，多出使用时才有必要定义
  let DJHColumnName="C_DJH" //单据号字段列
  let DJLXColumnName="C_DJLX" //单据类型字段列
  let XBHColumnName="C_XBH" //序编号即流水号字段列
  let XHColumnName="C_XH" //序号字段列
  let JYHColumnName="C_JYH" //交易号字段列
  let FIDColumnName="C_FID" //父ID
  let ZWYColumnName="C_ZWY"  //账务日期字段
  let PCColumnName="C_PC" //上品批次字段
  //特殊后缀
  let JMColumnSuffix="JM" //拼音简码后缀
  //主子表特征，
  let OneLevelMainTableFeatures=["T_DJ_";"T_FDJ_";"T_HZDJ_";"T_FKD_";"T_XTGX_"]
  let OneLevelLeafTableFeatures=["T_DJ";"T_FDJ";"T_HZDJ";"T_FKD";"T_XTGXX"]
  //let OneLevelLeafTableFeaturePatterns=["^T_DJ[A-Z]+[A-Z_0-9]+$";"^T_FDJ[A-Z]+[A-Z_0-9]+$";"^T_HZDJ[A-Z]+[A-Z_0-9]+$";"^T_FKD[A-Z]+[A-Z_0-9]+$";"^T_XTGX[A-Z]+[A-Z_0-9]+$"] //正确，但性能严重下降  
  let TwoLevelMainTableFeatures=["T_XXXX_"]
  let TwoLevelChildTableFeatures=["T_XXXX"]
  let TwoLevelLeafTableFeatures=["T_XXXX"]
  //特殊表
  let JXCSHDJTableName="T_DJ_JXCSH" //进销存审核表
  let JXCSHDJ_PCLSHTableName="T_PCLSH_JXCSH" //批次流水号进销存审核表, 所生成的批次可能被编辑，所以也可考虑审核单据不生成批次
  let JHGLDJTableName="T_DJ_JHGL" //进货管理单据, 自动生成的方法Create or Update..._CGJH (...)及Create or Update..._JXCSH应该删除，当前仍保留，使用Create or Update...WithPCProcess来替代 
  let PCLSHTableName="T_PCLSH" //批次流水号进货管理表, 审核后商品批次须重新生成, 采购进货，销售退货，商品拆分和捆绑都可能需要产生商品此次
  //let DateTimeDefaultValue=new DateTime()
  let RangeTypeMaxValueDic=
    let dic=new Generic.Dictionary<string,string>()
    dic.Add(TypeNameByte.ToLowerInvariant(),"Byte.MaxValue")
    dic.Add(TypeNameInt16.ToLowerInvariant(),"Int16.MaxValue")
    dic.Add(TypeNameInt32.ToLowerInvariant(),"Int32.MaxValue")
    dic.Add(TypeNameInt64.ToLowerInvariant(),"Int64.MaxValue")
    dic.Add(TypeNameDouble.ToLowerInvariant(),"Double.MaxValue")
    dic.Add(TypeNameSingle.ToLowerInvariant(),"Single.MaxValue")
    dic.Add(TypeNameDecimal.ToLowerInvariant(),"Decimal.MaxValue")
    dic.Add(TypeNameDateTime.ToLowerInvariant(),"DateTime.MaxValue")
    dic
  let RangeTypeMinValueDic=
    let dic=new Generic.Dictionary<string,string>()
    dic.Add(TypeNameByte.ToLowerInvariant(),"Byte.MinValue")
    dic.Add(TypeNameInt16.ToLowerInvariant(),"Int16.MinValue")
    dic.Add(TypeNameInt32.ToLowerInvariant(),"Int32.MinValue")
    dic.Add(TypeNameInt64.ToLowerInvariant(),"Int64.MinValue")
    dic.Add(TypeNameDouble.ToLowerInvariant(),"Double.MinValue")
    dic.Add(TypeNameSingle.ToLowerInvariant(),"Single.MinValue")
    dic.Add(TypeNameDecimal.ToLowerInvariant(),"Decimal.MinValue")
    dic.Add(TypeNameDateTime.ToLowerInvariant(),"DateTime.MinValue")
    dic
  