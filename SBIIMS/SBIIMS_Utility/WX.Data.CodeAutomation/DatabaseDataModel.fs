namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.Linq
open System.Data
open System.Data.SqlClient
open Microsoft.FSharp.Linq
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data

open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type T_DJLX=
  {
   C_DM:byte
   C_LX:string
   C_QZ:string
   C_BZ:string
  }

type TableColumnDescription=
  {
  TABLE_NAME                       :string
  TABLE_DESCRIPTION           : string
  COLUMN_ORDER                : string
  COLUMN_NAME                 :string
  COLUMN_DESCRIPTION     :string
 }

type TableInfo=
  {
  TABLE_CATALOG :string
  TABLE_SCHEMA   :string
  TABLE_NAME       :string
  TABLE_TYPE         :string
  }


type ViewInfo=
 {
 TABLE_CATALOG   :string
 TABLE_SCHEMA    :string
 TABLE_NAME        :string
 CHECK_OPTION    :string
 IS_UPDATABLE      :string
 }

//http://msdn.microsoft.com/en-us/library/ms254969.aspx ,Schema dictionary 
//This one only for SQL Server 2005
//原始表字段结构
type DbColumnSchemalOR=
  {
  TABLE_CATALOG:string;
  TABLE_SCHEMA:string;
  TABLE_NAME:string;
  COLUMN_NAME:string;
  ORDINAL_POSITION:int;
  COLUMN_DEFAULT:string;
  IS_NULLABLE:string
  DATA_TYPE:string;
  DB_DATA_TYPE:string;
  CHARACTER_MAXIMUM_LENGTH:int;
  CHARACTER_OCTET_LENGTH:int;
  NUMERIC_PRECISION:byte;
  NUMERIC_PRECISION_RADIX:Int16;
  NUMERIC_SCALE:int;
  DATETIME_PRECISION:Int16;
  CHARACTER_SET_CATALOG:string;
  CHARACTER_SET_SCHEMA:string;
  CHARACTER_SET_NAME:string;
  COLLATION_CATALOG:string;
  }
  
//自定义
type DbColumnSchemalR=
  {
  TABLE_CATALOG:string;
  TABLE_SCHEMA:string;
  TABLE_NAME:string;
  COLUMN_NAME:string;
  ORDINAL_POSITION:int;
  COLUMN_DEFAULT:string;
  IS_NULLABLE:string;
  DATA_TYPE:string;
  DB_DATA_TYPE:string;
  CHARACTER_MAXIMUM_LENGTH:int;
  CHARACTER_OCTET_LENGTH:int;
  NUMERIC_PRECISION:byte;
  NUMERIC_PRECISION_RADIX:Int16;
  NUMERIC_SCALE:int;
  DATETIME_PRECISION:Int16;
  CHARACTER_SET_CATALOG:string;
  CHARACTER_SET_SCHEMA:string;
  CHARACTER_SET_NAME:string;
  COLLATION_CATALOG:string;
  IS_NULLABLE_TYPED:bool
  DATA_TYPE_TYPED:System.Data.DbType;
  }

//http://msdn.microsoft.com/en-us/library/ms254969.aspx   ,Schema dictionary 
type DbColumnSchemal()=
    [<DefaultValue>]
    val mutable _TABLE_CATALOG:string
    [<DefaultValue>]
    val mutable _TABLE_SCHEMA:string
    [<DefaultValue>]
    val mutable _TABLE_NAME:string
    [<DefaultValue>]
    val mutable _COLUMN_NAME:string
    [<DefaultValue>]
    val mutable _ORDINAL_POSITION:int
    [<DefaultValue>]
    val mutable _COLUMN_DEFAULT:string
    [<DefaultValue>]
    val mutable _IS_NULLABLE:string
    [<DefaultValue>]
    val mutable _DATA_TYPE:string
    [<DefaultValue>]
    val mutable _DB_DATA_TYPE:string
    [<DefaultValue>]
    val mutable _CHARACTER_MAXIMUM_LENGTH:int
    [<DefaultValue>]
    val mutable _CHARACTER_OCTET_LENGTH:int
    [<DefaultValue>]
    val mutable _NUMERIC_PRECISION:byte
    [<DefaultValue>]
    val mutable _NUMERIC_PRECISION_RADIX:Int16
    [<DefaultValue>]
    val mutable _NUMERIC_SCALE:int
    [<DefaultValue>]
    val mutable _DATETIME_PRECISION:Int16
    [<DefaultValue>]
    val mutable _CHARACTER_SET_CATALOG:string
    [<DefaultValue>]
    val mutable _CHARACTER_SET_SCHEMA:string
    [<DefaultValue>]
    val mutable _CHARACTER_SET_NAME:string
    [<DefaultValue>]
    val mutable _COLLATION_CATALOG:string
    [<DefaultValue>]
    val mutable _IS_NULLABLE_TYPED:bool
    [<DefaultValue>]
    val mutable _DATA_TYPE_TYPED:DbType
    
    member x.TABLE_CATALOG
      with get ()=x._TABLE_CATALOG
      and set v=x._TABLE_CATALOG<-v

    member x.TABLE_SCHEMA
      with get ()=x._TABLE_SCHEMA
      and set v=x._TABLE_SCHEMA<-v

    member x.TABLE_NAME
      with get ()=x._TABLE_NAME
      and set v=x._TABLE_NAME<-v

    member x.COLUMN_NAME
      with get ()=x._COLUMN_NAME
      and set v=x._COLUMN_NAME<-v

    member x.ORDINAL_POSITION
      with get ()=x._ORDINAL_POSITION
      and set v=x._ORDINAL_POSITION<-v

    member x.COLUMN_DEFAULT
      with get ()=x._COLUMN_DEFAULT
      and set v=x._COLUMN_DEFAULT<-v

    member x.IS_NULLABLE
      with get ()=x._IS_NULLABLE
      and set v=x._IS_NULLABLE<-v

    member x.DATA_TYPE
      with get ()=x._DATA_TYPE
      and set v=x._DATA_TYPE<-v

    member x.DB_DATA_TYPE
      with get ()=x._DB_DATA_TYPE
      and set v=x._DB_DATA_TYPE<-v

    member x.CHARACTER_MAXIMUM_LENGTH
      with get ()=x._CHARACTER_MAXIMUM_LENGTH
      and set v=x._CHARACTER_MAXIMUM_LENGTH<-v

    member x.CHARACTER_OCTET_LENGTH
      with get ()=x._CHARACTER_OCTET_LENGTH
      and set v=x._CHARACTER_OCTET_LENGTH<-v

    member x.NUMERIC_PRECISION
      with get ()=x._NUMERIC_PRECISION
      and set v=x._NUMERIC_PRECISION<-v

    member x.NUMERIC_PRECISION_RADIX
      with get ()=x._NUMERIC_PRECISION_RADIX
      and set v=x._NUMERIC_PRECISION_RADIX<-v

    member x.NUMERIC_SCALE
      with get ()=x._NUMERIC_SCALE
      and set v=x._NUMERIC_SCALE<-v

    member x.DATETIME_PRECISION
      with get ()=x._DATETIME_PRECISION
      and set v=x._DATETIME_PRECISION<-v

    member x.CHARACTER_SET_CATALOG
      with get ()=x._CHARACTER_SET_CATALOG
      and set v=x._CHARACTER_SET_CATALOG<-v

    member x.CHARACTER_SET_SCHEMA
      with get ()=x._CHARACTER_SET_SCHEMA
      and set v=x._CHARACTER_SET_SCHEMA<-v

    member x.CHARACTER_SET_NAME
      with get ()=x._CHARACTER_SET_NAME
      and set v=x._CHARACTER_SET_NAME<-v
      
    member x.COLLATION_CATALOG
      with get ()=x._COLLATION_CATALOG
      and set v=x._COLLATION_CATALOG<-v
      
    member x.IS_NULLABLE_TYPED
      with get ()=x._IS_NULLABLE_TYPED
      and set v=x._IS_NULLABLE_TYPED<-v
      
    member x.DATA_TYPE_TYPED
      with get ()=x._DATA_TYPE_TYPED
      and set v=x._DATA_TYPE_TYPED<-v

type DbColumnSchemalString()=
    [<DefaultValue>]
    val mutable _TABLE_CATALOG:string
    [<DefaultValue>]
    val mutable _TABLE_SCHEMA:string
    [<DefaultValue>]
    val mutable _TABLE_NAME:string
    [<DefaultValue>]
    val mutable _COLUMN_NAME:string
    [<DefaultValue>]
    val mutable _ORDINAL_POSITION:string
    [<DefaultValue>]
    val mutable _COLUMN_DEFAULT:string
    [<DefaultValue>]
    val mutable _IS_NULLABLE:string
    [<DefaultValue>]
    val mutable _DATA_TYPE:string
    [<DefaultValue>]
    val mutable _DB_DATA_TYPE:string
    [<DefaultValue>]
    val mutable _CHARACTER_MAXIMUM_LENGTH:string
    [<DefaultValue>]
    val mutable _CHARACTER_OCTET_LENGTH:string
    [<DefaultValue>]
    val mutable _NUMERIC_PRECISION:string
    [<DefaultValue>]
    val mutable _NUMERIC_PRECISION_RADIX:string
    [<DefaultValue>]
    val mutable _NUMERIC_SCALE:string
    [<DefaultValue>]
    val mutable _DATETIME_PRECISION:string
    [<DefaultValue>]
    val mutable _CHARACTER_SET_CATALOG:string
    [<DefaultValue>]
    val mutable _CHARACTER_SET_SCHEMA:string
    [<DefaultValue>]
    val mutable _CHARACTER_SET_NAME:string
    [<DefaultValue>]
    val mutable _COLLATION_CATALOG:string
    
    member x.TABLE_CATALOG
      with get ()=x._TABLE_CATALOG
      and set v=x._TABLE_CATALOG<-v

    member x.TABLE_SCHEMA
      with get ()=x._TABLE_SCHEMA
      and set v=x._TABLE_SCHEMA<-v

    member x.TABLE_NAME
      with get ()=x._TABLE_NAME
      and set v=x._TABLE_NAME<-v

    member x.COLUMN_NAME
      with get ()=x._COLUMN_NAME
      and set v=x._COLUMN_NAME<-v

    member x.ORDINAL_POSITION
      with get ()=x._ORDINAL_POSITION
      and set v=x._ORDINAL_POSITION<-v

    member x.COLUMN_DEFAULT
      with get ()=x._COLUMN_DEFAULT
      and set v=x._COLUMN_DEFAULT<-v

    member x.IS_NULLABLE
      with get ()=x._IS_NULLABLE
      and set v=x._IS_NULLABLE<-v

    member x.DATA_TYPE
      with get ()=x._DATA_TYPE
      and set v=x._DATA_TYPE<-v

    member x.DB_DATA_TYPE
      with get ()=x._DB_DATA_TYPE
      and set v=x._DB_DATA_TYPE<-v

    member x.CHARACTER_MAXIMUM_LENGTH
      with get ()=x._CHARACTER_MAXIMUM_LENGTH
      and set v=x._CHARACTER_MAXIMUM_LENGTH<-v

    member x.CHARACTER_OCTET_LENGTH
      with get ()=x._CHARACTER_OCTET_LENGTH
      and set v=x._CHARACTER_OCTET_LENGTH<-v

    member x.NUMERIC_PRECISION
      with get ()=x._NUMERIC_PRECISION
      and set v=x._NUMERIC_PRECISION<-v

    member x.NUMERIC_PRECISION_RADIX
      with get ()=x._NUMERIC_PRECISION_RADIX
      and set v=x._NUMERIC_PRECISION_RADIX<-v

    member x.NUMERIC_SCALE
      with get ()=x._NUMERIC_SCALE
      and set v=x._NUMERIC_SCALE<-v

    member x.DATETIME_PRECISION
      with get ()=x._DATETIME_PRECISION
      and set v=x._DATETIME_PRECISION<-v

    member x.CHARACTER_SET_CATALOG
      with get ()=x._CHARACTER_SET_CATALOG
      and set v=x._CHARACTER_SET_CATALOG<-v

    member x.CHARACTER_SET_SCHEMA
      with get ()=x._CHARACTER_SET_SCHEMA
      and set v=x._CHARACTER_SET_SCHEMA<-v

    member x.CHARACTER_SET_NAME
      with get ()=x._CHARACTER_SET_NAME
      and set v=x._CHARACTER_SET_NAME<-v
      
    member x.COLLATION_CATALOG
      with get ()=x._COLLATION_CATALOG
      and set v=x._COLLATION_CATALOG<-v
      
type DbFKPK=
  {
  FOREIGN_KEY:string
  FK_TABLE:string
  FK_COLUMN_NAME:string
  PK_TABLE:string
  PK_COLUMN_NAME:string
  mutable PK_TABLE_ALIAS:string
  }

and DbPKColumn=
  {  
  CONSTRAINT_CATALOG:string
  CONSTRAINT_SCHEMA:string
  CONSTRAINT_NAME:string
  TABLE_CATALOG:string
  TABLE_SCHEMA:string
  TABLE_NAME:string
  COLUMN_NAME:string
  ORDINAL_POSITION:int
  KEYTYPE:byte
  INDEX_NAME:string
  }