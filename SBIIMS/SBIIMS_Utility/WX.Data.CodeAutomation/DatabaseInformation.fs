(*
Schema Restrictions (ADO.NET)
http://msdn.microsoft.com/en-us/library/cc716722.aspx  for .NET Framework 3.5

Schema dictionary
http://msdn.microsoft.com/en-us/library/ms254969.aspx   for NET 3.5
http://msdn.microsoft.com/en-us/library/ms254969(VS.100).aspx  for NET 4.0

//WX, 其实数据库所有的Schema信息都开放的, 可从该位置查看, SBIIMS0001->Views->System Views->INFORMATION_SCHEMA....

Design:
1. 可以使用 Join 将 字段信息进行组合， 然后再创建相应的Recrod type

1. 2011-05-22,  module DatabaseInformation，原先为type DatabaseInformation, 其成员均为Static member, 
    由于Type成员不能使用Closures特性, 而且静态字段也会每次都执行语句体
    WX.Data.Design.CodeSnippet.For FSharp.For Closures.txt
    经测试，其中的Closures特性也可使用 module下let 成员字段代替, 该类型成员也只在初始化时执行一次
      

*)
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
open WX.Data.CodeAutomation

module DatabaseInformation =
  //得到单据类型信息
  let GetDJLX=
    let sourceDataMemory:T_DJLX list ref=ref []
    (fun () ->   //closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        seq{
          let db=DatabaseFactory.CreateDatabase()
          let sqlText=
            @"
            SELECT C_DM,C_LX,C_QZ,C_BZ FROM T_DJLX 
            "
          use cmd=new SqlCommand(sqlText)
          use reader=db.ExecuteReader cmd
          while reader.Read() do
            yield 
              {C_DM=reader.["C_DM"].ToString()|>byte;
              C_LX=reader.["C_LX"].ToString();
              C_QZ=reader.["C_QZ"].ToString();
              C_BZ=reader.["C_BZ"].ToString()}
            } 
        |>Seq.toList //保证数据的即时可用性 
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      )

  (*
  http://msdn.microsoft.com/en-us/library/system.data.sqldbtype.aspx
  Important********************************* 
  http://msdn.microsoft.com/en-us/library/ms187993.aspx 
  ntext , text, and image data types will be removed in a future version of Microsoft SQL Server. 
  Avoid using these data types in new development work, and plan to modify applications that currently use them. 
  Use nvarchar(max), varchar(max), and varbinary(max) instead. 
  *)
  let GetDbType (dataType:string)=
    match dataType.ToLowerInvariant() with
     | "bigint" ->                          DbType.Int64
     | "binary" ->                         DbType.Byte //defined as  DbType.Byte[]
     | "bit" ->                               DbType.Boolean
     | "char" ->                            DbType.String
     | "datetime" ->                    DbType.DateTime
     | "decimal" ->                      DbType.Decimal
     | "float" ->                            DbType.Double
     | "image" ->                         DbType.Byte //defined as  DbType.Byte[] 
     | "int" ->                               DbType.Int32
     | "money" ->                       DbType.Decimal
     | "nchar" ->                          DbType.String
     | "ntext" ->                          DbType.String 
     | "nvarchar" ->                    DbType.String
     | "real" ->                             DbType.Single
     | "uniqueidentifier" ->        DbType.Guid
     | "smalldateTime" ->         DbType.DateTime
     | "smallInt" ->                       DbType.Int16
     | "smallmoney" ->              DbType.Decimal
     | "text" ->                            DbType.String 
     | "timestamp" ->                DbType.Byte  //defined as  DbType.Byte[]
     | "tinyint" ->                        DbType.Byte
     | "varbinary" ->                   DbType.Byte 
     | "varchar" ->                      DbType.String
     | "variant" //->                        DbType.Object //
     | "xml" //->                             DbType.Xml
     | "udt" //->                              DbType.Object      //???,用户自定义类型
     | "structured" //->                 DbType.Object   //???
     | "date" //->                          DbType.Date
     | "time"//->                           DbType.Time
     | "datetime2" //->               DbType.DateTime2
     | "datetimeoffset" ->        DbType.DateTimeOffset
     | _ ->DbType.String

  //http://msdn.microsoft.com/en-us/library/system.data.sqldbtype.aspx
  let GetTypeString (dataType:string)=
    match dataType.ToLowerInvariant() with
     | "bigint" ->                          TypeNameInt64
     | "binary" ->                         TypeNameBytes
     | "bit" ->                              TypeNameBool
     | "char" ->                            TypeNameString
     | "datetime" ->                     TypeNameDateTime
     | "decimal" ->                       TypeNameDecimal
     | "float" ->                           TypeNameDouble
     | "image" ->                         TypeNameBytes
     | "int" ->                               TypeNameInt32
     | "money" ->                        TypeNameDecimal
     | "nchar" ->                          TypeNameString
     | "ntext" ->                          TypeNameString
     | "nvarchar" ->                     TypeNameString
     | "real" ->                             TypeNameSingle
     | "uniqueidentifier" ->          TypeNameGuid
     | "smalldatetime" ->             TypeNameDateTime
     | "smallint" ->                       TypeNameInt16
     | "smallmoney" ->                TypeNameDecimal
     | "text" ->                             TypeNameString
     | "timestamp" ->                  TypeNameBytes
     | "tinyint" ->                         TypeNameByte
     | "varbinary" ->                     TypeNameBytes
     | "varchar" ->                       TypeNameString
     | "variant" ->                        TypeNameObject
     | "xml" 
     | "udt"
     | "structured"
     | "date" 
     | "time"
     | "datetime2"
     | "datetimeoffset" -> TypeNameString
     | _ ->TypeNameString


  //http://msdn.microsoft.com/en-us/library/system.data.sqldbtype.aspx
  (* Right Backup
  let GetTypeString (dataType:string)=
    match dataType.ToLowerInvariant() with
     | "bigint" ->                          "System.Int64"
     | "binary" ->                         "System.Byte[]"
     | "bit" ->                                 "System.Boolean"
     | "char" ->                            "System.String"
     | "datetime" ->                   "System.DateTime"
     | "decimal" ->                      "System.Decimal"
     | "float" ->                           "System.Double"
     | "image" ->                         "System.Byte[]"
     | "int" ->                                "System.Int32"
     | "money" ->                        "System.Decimal"
     | "nchar" ->                          "System.String"
     | "ntext" ->                         "System.String"
     | "nvarchar" ->                   "System.String"
     | "real" ->                             "System.Single"
     | "uniqueidentifier" ->        "System.Guid"
     | "smalldatetime" ->         "System.DateTime"
     | "smallint" ->                       "System.Int16"
     | "smallmoney" ->              "System.Decimal"
     | "text" ->                            "System.String"
     | "timestamp" ->                "System.Byte[]"
     | "tinyint" ->                        "System.Byte"
     | "varbinary" ->                   "System.Byte[]"
     | "varchar" ->                      "System.String"
     | "variant" ->                        "System.Object"
     | "xml" 
     | "udt"
     | "structured"
     | "date" 
     | "time"
     | "datetime2"
     | "datetimeoffset" ->"System.String"
     | _ ->"System.String"
  *)

 //http://msdn.microsoft.com/en-us/library/system.data.sqldbtype.aspx
  let GetSqlDbType (dataType:string)=
    match dataType.ToLowerInvariant() with
     | "bigint" ->                       System.Data.SqlDbType.BigInt
     | "binary" ->                      System.Data.SqlDbType.Binary
     | "bit" ->                            System.Data.SqlDbType.Bit
     | "char" ->                         System.Data.SqlDbType.Char   
     | "datetime" ->                System.Data.SqlDbType.DateTime   
     | "decimal" ->                  System.Data.SqlDbType.Decimal   
     | "float" ->                        System.Data.SqlDbType.Float    
     | "image" ->                      System.Data.SqlDbType.Image  
     | "int" ->                            System.Data.SqlDbType.Int   
     | "money" ->                    System.Data.SqlDbType.Money   
     | "nchar" ->                       System.Data.SqlDbType.NChar  
     | "ntext" ->                        System.Data.SqlDbType.NText  
     | "nvarchar" ->                    System.Data.SqlDbType.NVarChar
     | "real" ->                             System.Data.SqlDbType.Real
     | "uniqueidentifier" ->      System.Data.SqlDbType.UniqueIdentifier
     | "smalldatetime" ->        System.Data.SqlDbType.SmallDateTime 
     | "smallint" ->                     System.Data.SqlDbType.SmallInt
     | "smallmoney" ->              System.Data.SqlDbType.SmallMoney
     | "text" ->                            System.Data.SqlDbType.Text
     | "timestamp" ->                System.Data.SqlDbType.Timestamp
     | "tinyint" ->                        System.Data.SqlDbType.TinyInt 
     | "varbinary" ->                   System.Data.SqlDbType.VarBinary
     | "varchar" ->                      System.Data.SqlDbType.VarChar
     | "variant" ->                       System.Data.SqlDbType.Variant
     | "xml"  ->                            System.Data.SqlDbType.Xml
     | "udt" ->                             System.Data.SqlDbType.Udt
     | "structured" ->                 System.Data.SqlDbType.Structured
     | "date" ->                            System.Data.SqlDbType.Date
     | "time"->                             System.Data.SqlDbType.Time
     | "datetime2" ->                  System.Data.SqlDbType.DateTime2
     | "datetimeoffset" ->         System.Data.SqlDbType.DateTimeOffset
     | _ -> System.Data.SqlDbType.Text

  //new (databaseSourceName:string) as this={DatabaseSourceName=databaseSourceName}
  //static member DatabaseSourceName=String.Empty
  let GetTableInfo=
    let db=DatabaseFactory.CreateDatabase()
    use conn=db.CreateConnection()
    conn.Open()
    let restrictionValues=[|null;"dbo";null;"BASE TABLE"|] 
    let dataTable=conn.GetSchema("Tables",restrictionValues)
    conn.Close()
    seq{for a in dataTable.Rows ->
              {TABLE_CATALOG=string a.["TABLE_CATALOG"];
                TABLE_SCHEMA =string a.["TABLE_SCHEMA"];
                TABLE_NAME     =string a.["TABLE_NAME"];
                TABLE_TYPE        =string a.["TABLE_TYPE"]}}
    |>Seq.toArray

  (* Right!!!               
  let GetViewInfo=
    let db=DatabaseFactory.CreateDatabase()
    use conn=db.CreateConnection()
    conn.Open()
    let restrictionValues=[|null;"dbo";null;"VIEW"|] 
    let dataTable=conn.GetSchema("Tables",restrictionValues)
    conn.Close()
    seq{for a in dataTable.Rows ->
              {TABLE_CATALOG=string a.["TABLE_CATALOG"];
                TABLE_SCHEMA =string a.["TABLE_SCHEMA"];
                TABLE_NAME     =string a.["TABLE_NAME"];
                TABLE_TYPE        =string a.["TABLE_TYPE"]}}
   *)

  let GetViewInfo=
    let db=DatabaseFactory.CreateDatabase()
    use conn=db.CreateConnection()
    conn.Open()
    let restrictionValues=[|null;"dbo";null|] 
    let dataTable=conn.GetSchema("Views",restrictionValues)
    conn.Close()
    seq{for a in dataTable.Rows ->
              {TABLE_CATALOG=string a.["TABLE_CATALOG"];
                TABLE_SCHEMA =string a.["TABLE_SCHEMA"];
                TABLE_NAME     =string a.["TABLE_NAME"];
                CHECK_OPTION   =string a.["CHECK_OPTION"];
                IS_UPDATABLE   =string a.["IS_UPDATABLE"]}}
    |>Seq.toArray

  let GetColumnSchemal=
    let sourceDataMemory:DbColumnSchemalString array ref=ref [||]
    (fun (tableName) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        let db=DatabaseFactory.CreateDatabase()
        use conn=db.CreateConnection()
        conn.Open()
        let restrictionValues=[|null;"dbo";null;null|] 
        (*
        let restrictionValues=[|null;"dbo";tableName;null|] 
        *)
        (*
        http://msdn.microsoft.com/en-us/library/cc716722.aspx
        Catalog TABLE_CATALOG 1
        Owner TABLE_SCHEMA 2
        Table TABLE_NAME 3
        Table COLUMN_NAME 4
        *)
        let dataTable=conn.GetSchema("Columns",restrictionValues)
        conn.Close()
        seq{for a in dataTable.Rows ->
               let entity=DbColumnSchemalString()
               for b in  dataTable.Columns do
                 entity.GetType().GetProperty(b.ColumnName).SetValue(entity,a.[b.ColumnName],null)    //有时报错，须改进！System.ArgumentException: Object of type 'System.Int32' cannot be converted to type 'System.String'.
               entity}
        |>Seq.toArray
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>Array.filter (fun a->a.TABLE_NAME=tableName )
      )
    
  (*
  let GetColumnSchemal3Way tableName=
    let db=DatabaseFactory.CreateDatabase()
    use conn=db.CreateConnection()
    let restrictionValues=[|null;"dbo";tableName;null|] 
    (*
    Catalog TABLE_CATALOG 1
    Owner TABLE_SCHEMA 2
    Table TABLE_NAME 3
    Table COLUMN_NAME 4
    *)
    let dataTable=conn.GetSchema("Columns",restrictionValues)
    seq{for a in dataTable.Rows do
           //let entity=DbColumnSchemalR
           for b in  dataTable.Columns ->
             entity.GetType().GetProperty(b.ColumnName).SetValue(entity,a.[b.ColumnName],null)
             entity}
    let x=typeof<DbColumnSchemalR>
    let y={new DbColumnSchemalR with TABLE_CATALOG="wx"}
    y.GetType().GetProperty
    let fields=Microsoft.FSharp.Reflection.FSharpType.GetRecordFields(x)
    fields.[0].SetValue(
    {DbColumnSchemalR with fields.[0] ="wx"
    x.GetProperty("CHARACTER_SET_NAME").SetValue(x.,"wx",null)
    *)
    
  let GetColumnSchemal2Way=
    let sourceDataMemory:DbColumnSchemal array ref=ref [||]
    (fun (tableName) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        let db=DatabaseFactory.CreateDatabase()
        use conn=db.CreateConnection()
        conn.Open()
        let restrictionValues=[|null;"dbo";null;null|] 
        (*
        let restrictionValues=[|null;"dbo";tableName;null|] 
        *)
        (*
        Catalog TABLE_CATALOG 1
        Owner TABLE_SCHEMA 2
        Table TABLE_NAME 3
        Table COLUMN_NAME 4
        *)
        let dataTable=conn.GetSchema("Columns",restrictionValues)
        conn.Close()
        seq{for a in dataTable.Rows ->
                DbColumnSchemal
                  (TABLE_CATALOG=a.["TABLE_CATALOG"].ToString(),
                    TABLE_SCHEMA=a.["TABLE_SCHEMA"].ToString(),
                    TABLE_NAME=a.["TABLE_NAME"].ToString(),
                    COLUMN_NAME=a.["COLUMN_NAME"].ToString(),
                    ORDINAL_POSITION=(
                        match a.["ORDINAL_POSITION"].ToString() with 
                        | NullOrWhiteSpace _    -> -1
                        | b -> b|>Int32.Parse),
                    COLUMN_DEFAULT=a.["COLUMN_DEFAULT"].ToString(),
                    IS_NULLABLE=a.["IS_NULLABLE"].ToString(),
                    DATA_TYPE=(
                       match a.["DATA_TYPE"].ToString() with
                       | NullOrWhiteSpace _  ->String.Empty
                       | b -> GetTypeString(b)),
                    DB_DATA_TYPE=(
                       match a.["DATA_TYPE"].ToString() with
                       | NullOrWhiteSpace _  ->String.Empty
                       | b ->b),
                    CHARACTER_MAXIMUM_LENGTH=(
                       match a.["CHARACTER_MAXIMUM_LENGTH"].ToString() with
                       | NullOrWhiteSpace _   -> -1
                       | b ->b|>Int32.Parse),
                    CHARACTER_OCTET_LENGTH=(
                      match a.["CHARACTER_OCTET_LENGTH"].ToString() with
                      | NullOrWhiteSpace _   -> -1 
                      | b ->b|>Int32.Parse),
                    NUMERIC_PRECISION=(
                      match a.["NUMERIC_PRECISION"].ToString() with 
                      | NullOrWhiteSpace _  -> new Byte()
                      | b ->b|>Byte.Parse),
                    NUMERIC_PRECISION_RADIX=(
                      match a.["NUMERIC_PRECISION_RADIX"].ToString() with
                      | NullOrWhiteSpace _  -> -1s
                      | b -> b|>Int16.Parse),
                    NUMERIC_SCALE=(
                      match a.["NUMERIC_SCALE"].ToString() with
                      | NullOrWhiteSpace _   -> -1
                      | b ->b|>Int32.Parse),
                    DATETIME_PRECISION=(
                      match a.["DATETIME_PRECISION"].ToString() with
                      | NullOrWhiteSpace _   -> -1s
                      | b ->b|>Int16.Parse),
                    CHARACTER_SET_CATALOG=a.["CHARACTER_SET_CATALOG"].ToString(),
                    CHARACTER_SET_SCHEMA=a.["CHARACTER_SET_SCHEMA"].ToString(),
                    CHARACTER_SET_NAME=a.["CHARACTER_SET_NAME"].ToString(),
                    COLLATION_CATALOG=a.["COLLATION_CATALOG"].ToString(),
                    IS_NULLABLE_TYPED=(
                      match string a.["IS_NULLABLE"] with
                      | "YES" ->true
                      | _ ->false),
                    DATA_TYPE_TYPED=(
                      match string a.["DATA_TYPE"] with
                      | NullOrWhiteSpace _  ->DbType.Object
                      | b -> GetDbType(b)))}
        |>Seq.toArray
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>Array.filter (fun a->a.TABLE_NAME=tableName)
      )

  let GetColumnSchemal4Way=
    let sourceDataMemory:DbColumnSchemalR array ref=ref [||]
    (fun (tableName) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        //ObjectDumper.Write "执行"
        let db=DatabaseFactory.CreateDatabase()
        use conn=db.CreateConnection()
        conn.Open()
        let restrictionValues=[|null;"dbo";null;null|] 
        (*
        let restrictionValues=[|null;"dbo";tableName;null|] 
        *)
        (*
        Catalog TABLE_CATALOG 1
        Owner TABLE_SCHEMA 2
        Table TABLE_NAME 3
        Table COLUMN_NAME 4
        *)
        let dataTable=conn.GetSchema("Columns",restrictionValues)
        conn.Close()
        seq{for a in dataTable.Rows ->
                  { TABLE_CATALOG=string a.["TABLE_CATALOG"];
                    TABLE_SCHEMA=string a.["TABLE_SCHEMA"];
                    TABLE_NAME=string a.["TABLE_NAME"];
                    COLUMN_NAME=string a.["COLUMN_NAME"];
                    ORDINAL_POSITION=
                      match string a.["ORDINAL_POSITION"] with 
                      | NullOrWhiteSpace _    -> -1
                      | b -> int b;
                    COLUMN_DEFAULT=string a.["COLUMN_DEFAULT"];
                    IS_NULLABLE=string a.["IS_NULLABLE"];
                    DATA_TYPE=
                      match string a.["DATA_TYPE"] with
                      | NullOrWhiteSpace _  ->String.Empty
                      | b -> GetTypeString(b);
                    DB_DATA_TYPE=
                      match string a.["DATA_TYPE"] with
                      | NullOrWhiteSpace _  ->String.Empty
                      | b -> b;
                    CHARACTER_MAXIMUM_LENGTH=
                      match string a.["CHARACTER_MAXIMUM_LENGTH"] with
                      | NullOrWhiteSpace _   -> -1
                      | b ->int b;
                    CHARACTER_OCTET_LENGTH=
                      match string a.["CHARACTER_OCTET_LENGTH"] with
                      | NullOrWhiteSpace _   -> -1 
                      | b ->int b;
                    NUMERIC_PRECISION=
                      match string a.["NUMERIC_PRECISION"] with 
                      | NullOrWhiteSpace _  -> new Byte()
                      | b ->byte b;
                    NUMERIC_PRECISION_RADIX=
                      match string a.["NUMERIC_PRECISION_RADIX"] with
                      | NullOrWhiteSpace _  -> -1s
                      | b -> int16 b;
                    NUMERIC_SCALE=
                      match string a.["NUMERIC_SCALE"] with
                      | NullOrWhiteSpace _   -> -1
                      | b ->int b;
                    DATETIME_PRECISION=
                      match string a.["DATETIME_PRECISION"] with
                      | NullOrWhiteSpace _   -> -1s
                      | b ->int16 b;
                    CHARACTER_SET_CATALOG=string a.["CHARACTER_SET_CATALOG"];
                    CHARACTER_SET_SCHEMA=string a.["CHARACTER_SET_SCHEMA"];
                    CHARACTER_SET_NAME=string a.["CHARACTER_SET_NAME"];
                    COLLATION_CATALOG=string a.["COLLATION_CATALOG"];
                    IS_NULLABLE_TYPED=
                      match string a.["IS_NULLABLE"] with
                      | "YES" ->true
                      | _ ->false;
                    DATA_TYPE_TYPED=
                      match string a.["DATA_TYPE"] with
                      | NullOrWhiteSpace _  ->DbType.Object
                      | b -> GetDbType(b)}
                       }
        |>Seq.toArray
        |>fun a ->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>Array.filter (fun a->a.TABLE_NAME=tableName )
      )

  let GetPKColumns=
    let sourceDataMemory:DbPKColumn array ref=ref [||]
    (fun (tableName) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        let db=DatabaseFactory.CreateDatabase()
        use conn=db.CreateConnection()
        conn.Open()
        let restrictionValues=[|null;"dbo";null;null|] 
        (*
        let restrictionValues=[|null;"dbo";tableName;null|] 
        *)
        (*
        Catalog TABLE_CATALOG 1
        Owner TABLE_SCHEMA 2
        Table TABLE_NAME 3
        Table COLUMN_NAME 4
        *)
        let dataTable=conn.GetSchema("IndexColumns",restrictionValues)
        conn.Close()
        seq{for a in dataTable.Rows ->
                  {CONSTRAINT_CATALOG= string a.["CONSTRAINT_CATALOG"];
                  CONSTRAINT_SCHEMA= string a.["CONSTRAINT_SCHEMA"];
                  CONSTRAINT_NAME= string a.["CONSTRAINT_NAME"];
                  TABLE_CATALOG= string a.["TABLE_CATALOG"];
                  TABLE_SCHEMA= string a.["TABLE_SCHEMA"];
                  TABLE_NAME= string a.["TABLE_NAME"];
                  COLUMN_NAME= string a.["COLUMN_NAME"];
                  ORDINAL_POSITION=
                    (match string a.["ORDINAL_POSITION"] with
                    | NullOrWhiteSpace _  -> -1
                    | x -> int x);
                  KEYTYPE=
                    (match string a.["KEYTYPE"] with
                    | NullOrWhiteSpace _  ->Byte()
                    | x ->byte x);
                  INDEX_NAME=string a.["INDEX_NAME"]}
                  }
        |>Seq.filter (fun a->a.INDEX_NAME.ToLower().StartsWith "pk")  //临时解决方案，应该单独实现获取主键列的SQL语句 
        |>Seq.toArray
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>Array.filter (fun a->a.TABLE_NAME=tableName)
      )

  //获取所有的索引列(索引列包括了主键列)
  let GetIndexedColumns=
    let sourceDataMemory:DbPKColumn array ref=ref [||]
    (fun (tableName) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        let db=DatabaseFactory.CreateDatabase()
        use conn=db.CreateConnection()
        conn.Open()
        let restrictionValues=[|null;"dbo";null;null|] 
        (*
        let restrictionValues=[|null;"dbo";tableName;null|] 
        *)
        (*
        Catalog TABLE_CATALOG 1
        Owner TABLE_SCHEMA 2
        Table TABLE_NAME 3
        Table COLUMN_NAME 4
        *)
        let dataTable=conn.GetSchema("IndexColumns",restrictionValues)
        conn.Close()
        seq{for a in dataTable.Rows ->
                  {CONSTRAINT_CATALOG= string a.["CONSTRAINT_CATALOG"];
                  CONSTRAINT_SCHEMA= string a.["CONSTRAINT_SCHEMA"];
                  CONSTRAINT_NAME= string a.["CONSTRAINT_NAME"];
                  TABLE_CATALOG= string a.["TABLE_CATALOG"];
                  TABLE_SCHEMA= string a.["TABLE_SCHEMA"];
                  TABLE_NAME= string a.["TABLE_NAME"];
                  COLUMN_NAME= string a.["COLUMN_NAME"];
                  ORDINAL_POSITION=
                    (match string a.["ORDINAL_POSITION"] with
                    | NullOrWhiteSpace _  -> -1
                    | x -> int x);
                  KEYTYPE=    
                    (match string a.["KEYTYPE"] with
                    | NullOrWhiteSpace _  ->Byte()
                    | x ->byte x);
                  INDEX_NAME=string a.["INDEX_NAME"]}
                  }
        |>Seq.toArray
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>Array.filter (fun a->a.TABLE_NAME=tableName )
      )

  //获取除主键列以外的索引列
  let GetIndexedColumnsWithoutPK=
    let sourceDataMemory:DbPKColumn array ref=ref [||]
    (fun (tableName) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        let db=DatabaseFactory.CreateDatabase()
        use conn=db.CreateConnection()
        conn.Open()
        let restrictionValues=[|null;"dbo";null;null|] 
        (*
        let restrictionValues=[|null;"dbo";tableName;null|] 
        *)
        (*
        Catalog TABLE_CATALOG 1
        Owner TABLE_SCHEMA 2
        Table TABLE_NAME 3
        Table COLUMN_NAME 4
        *)
        let dataTable=conn.GetSchema("IndexColumns",restrictionValues)
        conn.Close()
        seq{for a in dataTable.Rows ->
                  {CONSTRAINT_CATALOG= string a.["CONSTRAINT_CATALOG"];
                  CONSTRAINT_SCHEMA= string a.["CONSTRAINT_SCHEMA"];
                  CONSTRAINT_NAME= string a.["CONSTRAINT_NAME"];
                  TABLE_CATALOG= string a.["TABLE_CATALOG"];
                  TABLE_SCHEMA= string a.["TABLE_SCHEMA"];
                  TABLE_NAME= string a.["TABLE_NAME"];
                  COLUMN_NAME= string a.["COLUMN_NAME"];
                  ORDINAL_POSITION=
                    (match string a.["ORDINAL_POSITION"] with
                    | NullOrWhiteSpace _  -> -1
                    | x -> int x);
                  KEYTYPE=    
                    (match string a.["KEYTYPE"] with
                    | NullOrWhiteSpace _  ->Byte()
                    | x ->byte x);
                  INDEX_NAME=string a.["INDEX_NAME"]}
                  }
        |>Seq.filter (fun a->a.INDEX_NAME.ToLower().StartsWith "pk"|>not)  //临时解决方案，应该单独实现获取主键列的SQL语句 
        |>Seq.toArray
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>Array.filter (fun a->a.TABLE_NAME=tableName )
      )

(*
//获取表的主键列,待完善
select A.COLUMN_NAME 
from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE A join 
(select * from sysobjects where xtype=N'PK' ) B 
on object_id(A.CONSTRAINT_NAME)=B.id 
where a.table_name='T_DJ_JHGL' 

//two way
SELECT syscolumns.name 
FROM syscolumns,sysobjects,sysindexes,sysindexkeys 
WHERE syscolumns.id = object_id('T_DJ_JHGL') AND sysobjects.xtype = 'PK' AND 
sysobjects.parent_obj = syscolumns.id AND sysindexes.id = syscolumns.id AND 
sysobjects.name = sysindexes.name AND sysindexkeys.id = syscolumns.id AND 
sysindexkeys.indid = sysindexes.indid AND syscolumns.colid = sysindexkeys.colid
*)
  
  
  let private UpdateFKRelationshipList (relationships:DbFKPK list)=
    //let newRelationships= relationships|>List.sortBy (fun a->a.PK_TABLE,a.FK_COLUMN_NAME)//只有升序
    //let newRelationships= relationships|>List.sortWith (fun a b->compare (a.PK_TABLE,a.FK_COLUMN_NAME)  (b.PK_TABLE,b.FK_COLUMN_NAME)) //升序
    //let newRelationships= relationships|>List.sortWith (fun a b->compare  (b.PK_TABLE,b.FK_COLUMN_NAME) (a.PK_TABLE,a.FK_COLUMN_NAME))//降序
    let count=ref 0
    for i=0 to relationships.Length-1 do //能够很好的表达第一个和上一个
      match i,relationships with
      | x,y when x>0 && y.[x].PK_TABLE =y.[x-1].PK_TABLE  && y.[x].FOREIGN_KEY<>y.[x-1].FOREIGN_KEY-> //y.[x].FOREIGN_KEY<>y.[x-1].FOREIGN_KEY, 一个外键对应多个外键列时，主键表应该使用同一个实例，反之不同的外键，应改使用不同的主键表实例
          incr count //排序是必须的,Seq.sortBy (fun a->a.PK_TABLE,a.FK_COLUMN_NAME) 
          y.[x].PK_TABLE_ALIAS<-y.[x].PK_TABLE+string !count
      | x,y ->
          count:=0
          y.[x].PK_TABLE_ALIAS<- y.[x].PK_TABLE
    relationships
 
    
  //[<OverloadID("1") >]  
  let private UpdateFKRelationshipsListTem (relationships:DbFKPK list)=
    let count=ref 0
    let rec UpdateFKRelationshipsA (relationshipsA:DbFKPK list)  (count:int ref)=
      match relationshipsA with
      | head::snd::tail when snd.PK_TABLE.Equals(head.PK_TABLE)  &&  snd.FOREIGN_KEY.Equals(head.FOREIGN_KEY) |>not ->   //head.PK_TABLE.Replace(snd.PK_TABLE,String.Empty).ToCharArray() |>Seq.exists (fun a->not <| Char.IsDigit(a) ) |>not-> //当更新PK_TABLE字段时
           if !count=0 then head.PK_TABLE_ALIAS<-head.PK_TABLE  //出现相等的第一对元素
           incr count  //count:=!count+1, --, decr count
           snd.PK_TABLE_ALIAS<-snd.PK_TABLE+string !count
           UpdateFKRelationshipsA (snd::tail) count
      | head::tail -> 
          if !count>0 then head.PK_TABLE_ALIAS<-head.PK_TABLE+string !count; count:=0 //最后一个相等的元素，此时后面一个元素和当前元素不等
          else head.PK_TABLE_ALIAS<-head.PK_TABLE  
          UpdateFKRelationshipsA tail count
      | []->()
    UpdateFKRelationshipsA relationships count
    
  //It can not update the seq records???  for they are different instance, 使用list时正确, 不太好
  let private UpdateFKRelationships (relationships:DbFKPK pseq)=
    let count=ref 0
    //let  previous=ref Unchecked.defaultof<DbFKPK>
    let previousPkTableName=ref Unchecked.defaultof<String>
    let previousForeignKey=ref Unchecked.defaultof<String>   // 一个外键对应多个外键列时，主键表应该使用同一个实例，反之不同的外键，应改使用不同的主键表实例
    for a in relationships do
      match !previousPkTableName,!previousForeignKey,a with
      | x,y,z when String.IsNullOrEmpty(x) |>not && z.PK_TABLE=x && z.FOREIGN_KEY<>y ->
          incr count
          a.PK_TABLE_ALIAS<-z.PK_TABLE+string !count
      | _,_,z ->
          count:=0
          a.PK_TABLE_ALIAS<-z.PK_TABLE
      previousPkTableName:=a.PK_TABLE 
      previousForeignKey:=a.FOREIGN_KEY
    relationships
    
  //It can update it, 较好
  let private UpdateFKRelationshipsTem (relationships:DbFKPK list)=
    let count=ref 0
    let  previous=ref Unchecked.defaultof<DbFKPK>
    for a in relationships do
      match !previous,a with
      | x,y when x=Unchecked.defaultof<DbFKPK> |>not && y.PK_TABLE=x.PK_TABLE && y.FOREIGN_KEY<>x.FOREIGN_KEY  -> // 一个外键对应多个外键列时，主键表应该使用同一个实例，反之不同的外键，应改使用不同的主键表实例,Unchecked.defaultof<DbFKPK> |>not 说明不是第一个
          incr count
          a.PK_TABLE_ALIAS<-y.PK_TABLE+string !count
      | _,y ->
          count:=0
          a.PK_TABLE_ALIAS<-y.PK_TABLE
      previous:=a
    relationships
    
(*Right
  let GetAllFKRelationships=
    seq{
      let db=DatabaseFactory.CreateDatabase()
      let sqlText=
        @"
          SELECT OBJECT_NAME(fk.constid) AS 'FOREIGN_KEY', OBJECT_NAME(fk.fkeyid) AS 
          'FK_TABLE', fc.[name] AS 'FK_COLUMN_NAME', OBJECT_NAME(fk.rkeyid) AS 'PK_TABLE', 
          pc.[name] AS 'PK_COLUMN_NAME'
          FROM sysforeignkeys fk
          INNER JOIN syscolumns fc
          ON fk.fkeyid = fc.[id]
          AND fk.fkey = fc.colid
          INNER JOIN syscolumns pc
          ON fk.rkeyid = pc.[id]
          AND fk.rkey = pc.colid
          ORDER BY OBJECT_NAME(fk.rkeyid) 
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield 
          {FOREIGN_KEY=reader.["FOREIGN_KEY"].ToString();
          FK_TABLE=reader.["FK_TABLE"].ToString();
          FK_COLUMN_NAME=reader.["FK_COLUMN_NAME"].ToString();
          PK_TABLE=reader.["PK_TABLE"].ToString();
          PK_TABLE_ALIAS=String.Empty;
          PK_COLUMN_NAME=reader.["PK_COLUMN_NAME"].ToString()}
      }
    |>Seq.toNetList //保证数据的即时可用性
  *)
  
  let GetAsFKRelationship=
    let sourceDataMemory:DbFKPK list ref=ref []
    (fun (asFKTableName:string) ->      //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        seq{
          let db=DatabaseFactory.CreateDatabase()
          let sqlText=
            @"
              SELECT OBJECT_NAME(fk.constid) AS 'FOREIGN_KEY', OBJECT_NAME(fk.fkeyid) AS 
              'FK_TABLE', fc.[name] AS 'FK_COLUMN_NAME', OBJECT_NAME(fk.rkeyid) AS 'PK_TABLE', 
              pc.[name] AS 'PK_COLUMN_NAME'
              FROM sysforeignkeys fk
              INNER JOIN syscolumns fc
              ON fk.fkeyid = fc.[id]
              AND fk.fkey = fc.colid
              INNER JOIN syscolumns pc
              ON fk.rkeyid = pc.[id]
              AND fk.rkey = pc.colid
              ORDER BY OBJECT_NAME(fk.rkeyid)"
              (*
              WHERE OBJECT_NAME(fk.fkeyid)=@FK_TABLE
              ORDER BY OBJECT_NAME(fk.rkeyid) 
              *)

          use cmd=new SqlCommand(sqlText)
          (*
          db.AddInParameter(cmd,"@FK_TABLE",DbType.String,asFKTableName)
          *)
          use reader=db.ExecuteReader cmd
          while reader.Read() do
            yield 
              {FOREIGN_KEY=reader.["FOREIGN_KEY"].ToString();
              FK_TABLE=reader.["FK_TABLE"].ToString();
              FK_COLUMN_NAME=reader.["FK_COLUMN_NAME"].ToString();
              PK_TABLE=reader.["PK_TABLE"].ToString();
              PK_TABLE_ALIAS=String.Empty;
              PK_COLUMN_NAME=reader.["PK_COLUMN_NAME"].ToString()}
        }
        |>Seq.toList //保证数据的即时可用性
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>List.filter (fun a->a.FK_TABLE=asFKTableName)
      |>List.sortBy (fun a->a.PK_TABLE,a.FK_COLUMN_NAME) //排序是必须的,按照ADO.net中Reference实体的生成顺序进行排序
      |>UpdateFKRelationshipList
      )

  let GetAsPKRelationship=
    let sourceDataMemory:DbFKPK list ref=ref []
    (fun (asPKTableName:string) ->   //Closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        seq{
          let db=DatabaseFactory.CreateDatabase()
          let sqlText=
            @"
              SELECT OBJECT_NAME(fk.constid) AS 'FOREIGN_KEY', OBJECT_NAME(fk.fkeyid) AS 
              'FK_TABLE', fc.[name] AS 'FK_COLUMN_NAME', OBJECT_NAME(fk.rkeyid) AS 'PK_TABLE', 
              pc.[name] AS 'PK_COLUMN_NAME'
              FROM sysforeignkeys fk
              INNER JOIN syscolumns fc
              ON fk.fkeyid = fc.[id]
              AND fk.fkey = fc.colid
              INNER JOIN syscolumns pc
              ON fk.rkeyid = pc.[id]
              AND fk.rkey = pc.colid
              ORDER BY OBJECT_NAME(fk.rkeyid)"
              (*
              WHERE OBJECT_NAME(fk.rkeyid)=@PK_TABLE
              ORDER BY OBJECT_NAME(fk.rkeyid)
              *)
          use cmd=new SqlCommand(sqlText)
          (*
          db.AddInParameter(cmd,"@PK_TABLE",DbType.String,asPKTableName)
          *)
          use reader=db.ExecuteReader cmd
          while reader.Read() do
            yield 
              {FOREIGN_KEY=reader.["FOREIGN_KEY"].ToString();
              FK_TABLE=reader.["FK_TABLE"].ToString();
              FK_COLUMN_NAME=reader.["FK_COLUMN_NAME"].ToString();
              PK_TABLE=reader.["PK_TABLE"].ToString();
              PK_TABLE_ALIAS=String.Empty;
              PK_COLUMN_NAME=reader.["PK_COLUMN_NAME"].ToString()}
          }
        |>Seq.toList //保证数据的即时可用性 //It can not update it??? for different instance
        |>fun a->sourceDataMemory:=a; a
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      |>List.filter (fun a->a.PK_TABLE=asPKTableName)
      |>UpdateFKRelationshipList
      )


  let GetTableColumnDescription=
    let sourceDataMemory:TableColumnDescription list ref=ref []
    (fun () ->   //closures, 也可用静态字段来或属性来代替
      let GetSourceData ()=
        seq{
          let db=DatabaseFactory.CreateDatabase()
          let sqlText=
            @"
            SELECT 
                TABLE_NAME         = b.name, 
                TABLE_DESCRIPTION  = isnull(d.value,''), 
                COLUMN_ORDER        = a.colorder, 
                COLUMN_NAME        = a.name, 
                COLUMN_DESCRIPTION = isnull(c.[value],'') 
            FROM 
        	    sys.syscolumns a 
        	    INNER JOIN 
        	    sys.sysobjects b 
        	    ON a.id=b.id  and b.xtype='U' and  b.name <>'dtproperties' 
        	    LEFT OUTER JOIN  
        	    sys.extended_properties  c 
        	    ON a.id=c.major_id and a.colid=c.minor_id  
        	    LEFT OUTER JOIN  
        	    sys.extended_properties d 
        	    ON b.id=d.major_id and d.minor_id=0
            order by 
                b.name,a.id,a.colorder 
            "
          use cmd=new SqlCommand(sqlText)
          use reader=db.ExecuteReader cmd
          while reader.Read() do
            yield 
              {TABLE_NAME=reader.["TABLE_NAME"].ToString();
              TABLE_DESCRIPTION=reader.["TABLE_DESCRIPTION"].ToString();
              COLUMN_ORDER=reader.["COLUMN_ORDER"].ToString();
              COLUMN_NAME=reader.["COLUMN_NAME"].ToString();
              COLUMN_DESCRIPTION=reader.["COLUMN_DESCRIPTION"].ToString()}
            } 
        |>Seq.toList //保证数据的即时可用性
        |>fun a->sourceDataMemory:=a; a 
      match sourceDataMemory.Value with
      | HasElement x-> x
      | _ ->GetSourceData ()
      )

  //====================================================================================

  //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况  
  let ValidateForeignKeyColumnDesign (tableName:string) (asFKRelationships:DbFKPK list) (columns:DbColumnSchemalR seq)=
      (*
      let sbTem=StringBuilder()
      *)
    (asFKRelationships, columns)
    |>fun(a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME ) (fun b->b.COLUMN_NAME ) (fun a b ->a,b)
    |>Seq.groupBy (fun (a,b) ->a.FOREIGN_KEY)
    |>Seq.map (fun a->
        match a with
        | _,y when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) && y|>Seq.exists (fun (_,b)->not b.IS_NULLABLE_TYPED)-> //如果设计正确, 必然有一种情况不存在
            (*
            sbTem.Remove(0,sbTem.Length)|>ignore
            y|>Seq.iter (fun (_,b)->sbTem.Append(b.COLUMN_NAME+",") |>ignore)
            *)
            let rec GetColumnNames (columns:(DbFKPK*DbColumnSchemalR) list)=
              match columns with
              | (_,v)::t-> v.COLUMN_NAME+","+GetColumnNames t
              | _ ->String.Empty
              (*
              match columns with
              | h::t-> 
                  match h with 
                  | u,v -> h .COLUMN_NAME+","+GetColumnNames t
              | _ ->()
              *)
            match y|>Seq.head,GetColumnNames (y|>Seq.toList) with
            | (u,v),w -> false, "该表"+tableName+"与主键表"+u.PK_TABLE+"以外键"+u.FOREIGN_KEY+"关联的外键列"+GetColumnNames (y|>Seq.toList)+"要么全部允许为空，要么全部都不允许为空，请更正！"
        | _ ->true,String.Empty)
    |>Seq.filter (fun (a,_)->not a)
    |>Seq.toList
 

  //验证是否有日志表
  let ValidateKeyTables  (tableNames:string list)=
    seq{
      //match tableNames|>Seq.exists (fun a->a.StartsWith "T_" && a.EndsWith "_RZ") with
      match tableNames|>Seq.exists (fun a->a.Equals "T_RZ") with //所有数据库使用同一个日志表名，这样便于自动代码和日志数据合并
      | false ->
          yield false,"该数据库没有日志表T_RZ, 按照设计要求，每个数据库都应该拥有特定名称的日志表，请修改！"
      | _ ->()
    }

  let ValidateZZTableColumn (tableNames:string seq)=
    seq{
      //验证总帐表和主表中的Guid字段和DateTime字段要一一对应
      for tableName in tableNames do
        match tableNames|>Seq.tryFind (fun b->b="T_ZZ_"+tableName.Remove(0,2)) with
        | Some x -> //有总账表
            for a in GetColumnSchemal4Way x do
              match a.COLUMN_NAME,a.DATA_TYPE with
              | y, EndsWithIn DateTimeConditions _  ->
                    if GetColumnSchemal4Way tableName|>Seq.exists (fun b->b.COLUMN_NAME=y)|>not then
                      yield false, "该表"+tableName+"中没有时间字段"+y+"与的总账表"+x+"的时间字段"+y+"一一对应，按照设计要求，总账表中的时间字段要和主表中的时间字段一一对应，请修改！"
              | y,EndsWithIn GuidConditions _  when GetPKColumns x|>Seq.exists (fun b->b.COLUMN_NAME=y ) |>not -> //不是主键列
                    if GetColumnSchemal4Way tableName|>Seq.exists (fun b->b.COLUMN_NAME=y)|>not then
                      match 
                        GetAsFKRelationship x
                        |>Seq.tryFind (fun b->b.FK_COLUMN_NAME=y) with 
                      | Some w  when w.PK_TABLE=tableName -> ()//x.PK_COLUMN_NAME
                      | _ -> yield false,"该表"+tableName+"中没有Guid字段"+y+"与的总账表"+x+"的Guid字段"+y+"一一对应，按照设计要求，总账表中的Guid字段除主键列外都要和主表中的Guid字段一一对应，请修改！"
              | _ ->()
        | _ ->()

      //验证总帐表，总账表中如果有String或byte[]字段，必须设计为可空
      for tableName in  tableNames do
        match tableName.StartsWith "T_ZZ_" with
        | true ->
            for column in 
              GetColumnSchemal4Way tableName  do
              match column.COLUMN_NAME,column.DATA_TYPE,column.IS_NULLABLE_TYPED with
              | x, EndsWithIn NullableTypeConditions _,false ->
                  yield false,"该总账表"+tableName+"的字段"+x+"不能设计为不可空，按照设计要求，总账表中的String类型和byte[]类型的字段必须设计为可空，因为总账表和基本表是同时创建的，非关键字段都使用默认值！请修改！"
              | _ ->()

        | _ ->()
    }


  //验证表是否存在主键
  let ValidateKeyColumn  (tableNames:string list)=
    seq{
      for tableName in  tableNames do
        match
          GetPKColumns tableName
          |>Seq.length  with
        | x when x>0 |>not ->
            yield false,"该表"+tableName+"没有主键，按照设计要求，每个表都应该拥有主键，请修改！"
        | _ ->()
    }

  (*
   //总账表还应与主表有一致的主键，并且总账表中的外键也需要在主表中存在
  *)
  let ValidateTableWithZZTable  (tableNames:string list)=
    seq{
     let sb=new StringBuilder()
     let keyColumnStr:string ref=ref String.Empty
     let zzKeyColumnStr:string ref=ref String.Empty
     let relationShipStr:string ref=ref String.Empty
     for tableName in tableNames do
       match 
         tableNames
         |>Seq.tryFind (fun a->a="T_ZZ_"+match tableName with x ->x.Remove(0,2)) with
       | Some x->
           match
             (* 
             GetAsPKRelationship tableName
             |>Seq.filter (fun b->b.FK_TABLE=x)
             |>Seq.map (fun b->b.PK_COLUMN_NAME),
             GetPKColumns tableName
             |>Seq.map (fun b->b.COLUMN_NAME),
             GetPKColumns x
             |>Seq.map (fun b->b.COLUMN_NAME) with
             *)
             GetAsPKRelationship tableName
             |>Seq.filter (fun b->b.FK_TABLE=x)
             |>Seq.distinctBy(fun b->b.PK_COLUMN_NAME), //主键在总账表中有两个对应外键时如T_DWBM和T_ZZ_DWBM
             GetPKColumns tableName,
             GetPKColumns x with
           | u,v,_ when Seq.length v>0 && Seq.length u>0|>not->
               yield false,"该表"+ tableName+"和总账表"+x+"没有建立主外键关系, 请修改!"
           | u,v,w when Seq.length u<>Seq.length v->
               sb.Clear()|>ignore 
               for keyColumn in v do
                 sb.Append(keyColumn.COLUMN_NAME)|>ignore 
                 sb.Append(",")|>ignore 
               keyColumnStr:=string sb
               sb.Clear()|>ignore 
               for keyColumn in w do
                 sb.Append(keyColumn.COLUMN_NAME)|>ignore 
                 sb.Append(",")|>ignore 
               zzKeyColumnStr:=string sb
               sb.Clear()|>ignore
               for keyColumn in u do
                 sb.Append(keyColumn.FOREIGN_KEY )|>ignore 
                 sb.Append(",")|>ignore
               relationShipStr:=string sb 
               yield false,"该表"+ tableName+"和关联的总账表"+x+"没有建立完整的主外键关系, 请修改！当前表和总账表的主外键关系为("+ !relationShipStr+"), 表"+tableName+"的主键列为"+ !keyColumnStr+" 而总账表"+x+"的主键列为"+ !zzKeyColumnStr
           | u,v,w  ->
               match 
                 Seq.append (u|>Seq.map(fun b->b.PK_COLUMN_NAME)) (v|>Seq.map(fun b->b.COLUMN_NAME))   //还可用Set.difference来对比两个集合是否相同
                 |>Seq.distinct with
               | p when Seq.length p=Seq.length u && Seq.length p=Seq.length w ->()  //合并该表查询两个集合后Distinct，其结果的长度和这两个集合的长度相等，并且同时也要等于总账表的主键列数
               | _ ->
                   sb.Clear()|>ignore 
                   for keyColumn in v do
                     sb.Append(keyColumn.COLUMN_NAME)|>ignore 
                     sb.Append(",")|>ignore 
                   keyColumnStr:=string sb
                   sb.Clear()|>ignore 
                   for keyColumn in w do
                     sb.Append(keyColumn.COLUMN_NAME)|>ignore 
                     sb.Append(",")|>ignore 
                   zzKeyColumnStr:=string sb
                   for keyColumn in u do
                     sb.Append(keyColumn.FOREIGN_KEY )|>ignore 
                     sb.Append(",")|>ignore
                   relationShipStr:=string sb 
                   yield false,"该表"+ tableName+"和关联的总账表"+x+"主键不一致,请修改！ 当前表和总账表的主外键关系为("+ !relationShipStr+"), 表"+tableName+"的主键列为"+ !keyColumnStr+" 而总账表"+x+"的主键列为"+ !zzKeyColumnStr
       | _ ->()
    } 

  //互为外键的验证, 
  (* 对于True的提示逻辑, 没有考虑一个外键由多个键列构成的情况
  let ValidateForeignKeyRelationshipDesign  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       for a,b in
         (GetAsFKRelationship tableName,GetColumnSchemal4Way tableName)
         |>fun (a,b)->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b->a,b)
         do
         match a.PK_TABLE,b.IS_NULLABLE_TYPED with
         | x,false -> 
             match GetAsFKRelationship x|>Seq.filter (fun c ->c.PK_TABLE=tableName) with
             | u when Seq.length u >0 ->
                  match 
                    GetColumnSchemal4Way x
                    |>Seq.filter (fun c->c.COLUMN_NAME=(Seq.head u).FK_COLUMN_NAME)
                    |>Seq.head with
                  | v when not v.IS_NULLABLE_TYPED ->
                      yield false, "该表"+tableName+"("+a.FOREIGN_KEY+")"+"与表"+x+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表，并且表"+tableName+"的外键列"+b.COLUMN_NAME+"和表"+x+"的外键"+(Seq.head u).FK_COLUMN_NAME+"均不允许为空，这将造成这两个的互锁，请更正！"
                  | v ->
                     yield true, "该表"+tableName+"("+a.FOREIGN_KEY+")"+"与表"+x+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表, 其中表"+x+"("+ (Seq.head u).FOREIGN_KEY+")的相关外键列"+v.COLUMN_NAME+"已设计为允许空，未互锁，该设计可行，但其中的一个表的删除操作将受到约束！"
             | _ ->()
         | _ ->()
    } 
  *)
  let ValidateForeignKeyRelationshipDesign  (tableNames:string list)=
    seq{
     let sb=StringBuilder()
     let columnStringFst=ref String.Empty
     let columnStringSnd=ref String.Empty
     for tableName in tableNames do
       for a,b in
         (GetAsFKRelationship tableName,GetColumnSchemal4Way tableName)
         |>fun (a,b)->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b->a,b)
         |>Seq.groupBy (fun (a,_)->a.PK_TABLE)
         do
         match GetAsFKRelationship a|>Seq.filter (fun c ->c.PK_TABLE=tableName) with
         | u when Seq.length u >0 -> //存在两个表互为外键表
             match 
               b|>Seq.exists (fun (_,d)->d.IS_NULLABLE_TYPED) |>not, //第一个表的指定外键的外键列全部都设计为不允许为空
               GetColumnSchemal4Way a
               |>fun v-> Query.join u v (fun u->u.FK_COLUMN_NAME) (fun v->v.COLUMN_NAME) (fun u v->v) //使用Join使逻辑更为清晰
               |>Seq.exists (fun v->v.IS_NULLABLE_TYPED) 
               |>not  //第二个表的指定外键的外键列全部都设计为不允许为空
               with
               | true,true-> //互锁
                   sb.Remove(0,sb.Length)|>ignore
                   b|>Seq.iteri (fun i (_,d)->
                     if i>0 then sb.Append(", ")|>ignore
                     d.COLUMN_NAME|>sb.Append|>ignore)
                   columnStringFst:=string sb 
                   sb.Remove(0,sb.Length)|>ignore
                   u|>Seq.iteri (fun i c->
                     if i>0 then sb.Append(", ")|>ignore
                     c.FK_COLUMN_NAME|>sb.Append|>ignore)
                   columnStringSnd:=string sb 
                   yield false, "该表"+tableName+"("+(b|>Seq.head|>fst).FOREIGN_KEY+")"+"与表"+a+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表，并且表"+tableName+"的外键列"+ !columnStringFst+"和表"+a+"的外键列"+ !columnStringSnd+"均设计为不允许为空，这将造成这两个的互锁，请更正！"
               | _ ->
                   yield false, "该表"+tableName+"("+(b|>Seq.head|>fst).FOREIGN_KEY+")"+"与表"+a+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表, 其中的一个表的相关外键列已设计为允许空，未互锁，该设计可行, 但自动编码模块暂时不支持该设计！！！ 此时外键关系的主表实体将是一个集合实例，而正常情况下外键关系的主表实体只是一个单一记录实例, 并且其中的一个表的删除操作将受到约束, 请考虑该设计是否必要！"
         | _ ->()
    } 

  (*
  //验证构成外键的字段不能同时为C_ID, （在实际设计中此种情况是可以的）
  let ValidateForeignKeyRelationship  (tableNames:string list)=
    seq[]
  *)
  let ValidateSpecialColumn  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       for a in
         GetColumnSchemal4Way tableName
         do
         (*
         match a.COLUMN_NAME,a.DATA_TYPE,a.IS_NULLABLE_TYPED with
         | x,y,z when 
             (x.Equals("C_CJRQ") || x.Equals("C_GXRQ"))
             &&
             ((string y).ToLowerInvariant().EndsWith("datetime")|>not 
             || z)
             ->
             yield false, "该表"+tableName+"的字段"+x+"必须为日期类型, 并且不能为空，请更正！"
         | _ ->()
         match a.COLUMN_NAME,a.DATA_TYPE,a.IS_NULLABLE_TYPED with
         | x,y,z when 
             (x.Equals("C_DWBM") || x.Equals("C_FBID"))
             &&
             ((string y).ToLowerInvariant().EndsWith("guid")|>not 
             || z)
             ->
             yield false, "该表"+tableName+"的字段"+x+"必须为uniqueidentifier类型, 并且不能为空，请更正！"
         | _ ->()
         *)
         match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),a.IS_NULLABLE_TYPED with
         | EqualsIn ["C_CJRQ";"C_GXRQ"] x, NotEndsWith "datetime" _, _ 
         | EqualsIn ["C_CJRQ";"C_GXRQ"] x, _, true ->
             yield false, "该表"+tableName+"的字段"+x+"必须为日期类型, 并且不能为空，请更正！"
         | _ ->()
         match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),a.IS_NULLABLE_TYPED with
         | EqualsIn ["C_DWBM";"C_FBID"] x, NotEndsWith "guid" _, _ 
         | EqualsIn ["C_DWBM";"C_FBID"] x, _, true ->
             yield false, "该表"+tableName+"的字段"+x+"必须为uniqueidentifier类型, 并且不能为空，请更正！"
         | _ ->()
         match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),a.IS_NULLABLE_TYPED with
         | EqualsIn ["C_PC"] x, NotEndsWith DecimalTypeName _, _ 
         | EqualsIn ["C_PC"] x, _, true ->
             yield false, "该表"+tableName+"的字段"+x+"必须为decimal类型, 并且不能为空，请更正！"
         | _ ->()
    } 

  let ValidateForeignKeyName  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       for a in 
         GetAsFKRelationship tableName
         |>Seq.groupBy (fun a->a.FOREIGN_KEY) //一个外键对应多个键列时
         do
         match a with
         | x,y ->
             match "FK_"+(Seq.head y).FK_TABLE, (Seq.head y).PK_TABLE with
             | u,v  when 
                 x.StartsWith u |>not
                 ||
                 x.EndsWith v |>not
                 ||  //外键名称的中间部分字段顺序可以不同
                 (x.Length>u.Length+v.Length+1 //1是为了最后一个'_', 
                 &&
                   match x.Substring(u.Length,x.Length-u.Length-v.Length-1).Split([|"_C_"|],StringSplitOptions.RemoveEmptyEntries) with //1是为了最后一个'_', RemoveEmptyEntries 是必须的,移出空的数组元素 
                   | w ->
                       Seq.length y<>w.Length
                       ||
                       match y|>Seq.map (fun c->c.FK_COLUMN_NAME)|>Set.ofSeq, w|>Seq.map (fun d->"C_"+d)|>Set.ofSeq with
                       | o,p -> o|>Set.difference p|>Set.count >0) 
                 ->
                 yield false, "该表"+tableName+"的外键名"+x+"不规范，按照设计要求, 1. 外键名构成为：'FK_'+当前表名+构成外键的键列名(多个键列名前后以'_'分隔)+构成外键的主键表名, 2. 外键关系的键列名与外键名称所包含的字段名不符"
             | _ ->()
         (*
         | x when not<| Regex.IsMatch(x,@"^FK_T_[A-Z_]+C+_+[A-Z_]+_T_[A-Z_]*[A-Z]$",RegexOptions.None) ->
             yield false, "该表"+tableName+"的外键名"+x+"不规范，按照设计要求, 1. 外键名只能由大写字母和'_'构成, 2. 外键名只能以FK_T开头, 3. 外键名中间部分必须包含带C_的字段名, 4. 外键名结尾部分要带T_并以字母结尾"
         | _ ->()
         *)
    }

  let ValidatePrimaryKeyName  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       for a in
         GetPKColumns tableName
         |>Seq.distinctBy (fun a->a.INDEX_NAME ) //一个索引对应多个键列
         do
         match a.INDEX_NAME with
         | x when (x.StartsWith("PK_") &&  x<>"PK_"+a.TABLE_NAME) || not<|Regex.IsMatch(x,@"^(PK|IX)_[A-Z_0-9]*[A-Z0-9]$",RegexOptions.None) ->
             yield false, "该表"+tableName+"的索引名"+x+"不规范，按照设计要求, 1. 索引名只能由大写字母，'_'，或数字构成, 2. 索引名只能以PK_或IX_开头, 3. 索引名要以字母或者数字结尾,  4. 以PK_开头时必须以所在表的表名结尾"
         | _ ->()
    }

  let ValidateIndexName  (tableNames:string list)=
    seq{
      for tableName in tableNames do
        for (a,b) in 
          GetIndexedColumnsWithoutPK tableName
          |>Seq.groupBy (fun a ->a.INDEX_NAME )
          do
          match "IX_"+tableName with
          | x when a.Length=x.Length || a.StartsWith x|>not || 
              match a.Substring(x.Length).Split([|"_C_"|],StringSplitOptions.RemoveEmptyEntries) with
              | y ->
                  Seq.length y<>Seq.length b ||
                  match y|>Seq.map (fun c->"C_"+c) with
                  | w ->
                      w|>Seq.forall (fun c->b|>Seq.exists (fun d->d.COLUMN_NAME=c))|>not ||
                      b|>Seq.forall (fun c->w|>Seq.exists (fun d->d=c.COLUMN_NAME))|>not
              ->
              yield false, "该表"+tableName+"的索引名"+a+"不规范，按照设计要求, 1. 索引名构成为：'IX_'+当前表名+构成索引列名(多个索引列名前后以'_'分隔), 2. 索引列名与索引名称所包含的字段名称不符"
          | _ ->() 
       }

  let ValidateTableName  (tableNames:string list)=
    seq{
      for tableName in tableNames do
        match tableName with
        | x when not<|Regex.IsMatch(x,@"^T_[A-Z_0-9]*[A-Z0-9]$",RegexOptions.None) ->
            yield false, "该表"+tableName+"的表名不规范，按照设计要求, 1. 表名只能由大写字母，'_'，或数字构成, 2. 表名只能以T_开头, 3. 表名要以字母或数字结尾"
        | _ ->()
    }

  let ValidateColumnName  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       match GetColumnSchemal4Way tableName with
       | columns-> 
           for a in columns do
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant() with
             | NotMatch @"^C_[A-Z_]*[A-Z]$" x,_  ->
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. 列名只能由大写字母和'_'构成, 2. 列名只能以C_开头, 3. 列名要以字母结尾"
             (*Right 
             | x,_ when not<|Regex.IsMatch(x,@"^C_[A-Z_]*[A-Z]$",RegexOptions.None) ->
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. 列名只能由大写字母和'_'构成, 2. 列名只能以C_开头, 3. 列名要以字母结尾"
             *)
             | EndsWithIn [JMColumnSuffix] x, EndsWithIn [StringTypeName] _ ->
                 match x.Substring(0,x.Length-JMColumnSuffix.Length) with
                 | NotEndsWithIn ["_"] z  ->
                     match columns|>Seq.exists (fun b->b.COLUMN_NAME=z && b.COLUMN_NAME.EndsWith JMColumnSuffix|>not  && b.DATA_TYPE.ToLowerInvariant().EndsWith StringTypeName ) with
                     | true ->()
                     | _ ->
                         yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 存在以'JM'后缀结尾的列名时，必须同时存在去掉该列名的'JM'后缀之后的列名，且他们的字段类型均为'String', 多个'JM'作为后缀也是不允许的 "
                 | _ ->()
             | _ -> ()
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant() with
             | NotEndsWithIn ["BZ";"_DM"] x,EndsWith BoolTypeName _-> //C_DM在流水号表中的类型为Boolean
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. Bool类型字段的列名1. 只能由大写字母和'_'构成, 2. 列名只能以C_开头, 3. 列名要以BZ或_DM结尾"
             | _ ->()
             //流水号创建或更新后，需要将生成的编号返回, 所以需要的约束较多
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(), tableName,GetPKColumns tableName  with
             | EqualsIn [XBHColumnName] x,NotEndsWith DecimalTypeName y, _, _ -> //C_XBH在表中必须为decimal类型
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为decimal类型(当前类型为"+y+"), 2. 不能在包含_DJ_,_FDJ_,_FKD_和_HZDJ的表中, 3. 所在表只能有一个主键, 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [XBHColumnName] x, _, ContainsIn ["_DJ_";"_FDJ_";"_HZDJ";"_FKD_"] _, _->   //C_XBH不能在单据表中
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为decimal类型, 2. 不能在包含_DJ_,_FDJ_和_HZDJ,_FKD_的表中(当前表名为"+tableName+"), 3. 所在表只能有一个主键, 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [XBHColumnName] x, _, _, y when Seq.length y >1  || Seq.length y<1-> 
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为decimal类型, 2. 不能在包含_DJ_,_FDJ_和_HZDJ,_FKD_的表中, 3. 所在表只能有一个主键(当前主键列有"+(Seq.length y|>string)+"个), 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [XBHColumnName] x, _, _, y ->
                 match columns|>Seq.find (fun b->b.COLUMN_NAME=(Seq.head y).COLUMN_NAME)|>fun b->b.COLUMN_NAME,b.DATA_TYPE.ToLowerInvariant() with
                 | z, NotEndsWith GuidTypeName w ->
                     yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表只能有一个主键,  4. 所在表的主键列类型必须为Guid(当前主键列"+z+"的类型为"+w+")"
                 | _ ->() 
             | _ ->()

             (*
             //单据创建或更新后，需要将生成的单据号返回, 所以需要的约束较多
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),tableName,GetPKColumns tableName  with
             | EqualsIn [DJHColumnName] x, NotEndsWith "string" y, _, _ -> //C_DJH在表中必须为string类型
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型(当前类型为"+y+"), 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表只能有一个主键, 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, NotContainsIn ["_DJ_";"_FDJ_";"_HZDJ";"_FKD_"] _, _ ->   //C_DJH只能在包含_DJ_和_FDJ_的表中
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中(当前表名为"+tableName+"), 3. 所在表只能有一个主键, 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, _, y when Seq.length y>1 || Seq.length y<1  ->
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表只能有一个主键(当前主键列有"+(Seq.length y|>string)+"个), 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, _, y ->
                 match columns|>Seq.find (fun b->b.COLUMN_NAME=(Seq.head y).COLUMN_NAME)|>fun b->b.COLUMN_NAME,b.DATA_TYPE.ToLowerInvariant() with
                 | z, NotEndsWith "guid" w ->
                     yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表只能有一个主键,  4. 所在表的主键列类型必须为Guid(当前主键列"+z+"的类型为"+w+")"
                 | _ ->() 
             *)
             //单据创建或更新后，需要将生成的单据号返回, 所以需要的约束较多, 还应加入约束，父子表中才能存在单据号???
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),tableName,GetPKColumns tableName  with
             | EqualsIn [DJHColumnName] x, NotEndsWith "string" y, _, _ -> //C_DJH在表中必须为string类型
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型(当前类型为"+y+"), 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, NotContainsIn ["_DJ";"_FDJ";"_HZDJ";"_FKD";"_QFMX_"] _, _ ->   //C_DJH只能在包含_DJ和_FDJ的表中,包括主表和字表
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中(当前表名为"+tableName+"), 3. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, _, y ->
                 match columns|>Seq.find (fun b->b.COLUMN_NAME=(Seq.head y).COLUMN_NAME)|>fun b->b.COLUMN_NAME,b.DATA_TYPE.ToLowerInvariant() with
                 | z, NotEndsWith "guid" w ->
                     yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表的主键列类型必须为Guid(当前主键列"+z+"的类型为"+w+")"
                 | _ ->() 
             | _ ->()
             //序号验证
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),GetPKColumns tableName with
             | EqualsIn [XHColumnName] x,NotEndsWith "byte" _, z when Seq.length z=1-> //C_XH,序号在表中必须为byte类型， T_DJSP_XX中的C_XH不受该约束，因为其主键列均大于1
                 yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_XH在独立表中必须为byte类型, 2. 所在表只能有一个主键;  具有序号的表可能同时要进行创建，更新和删除的批处理操作，处理量较大"
             | _ ->()
             (*
             match a.COLUMN_NAME,GetPKColumns tableName with  //可以同时显示设计问题
             | EqualsIn [XHColumnName] x, y when Seq.length y=1 -> 
                 if tableNames|>Seq.exists (fun b->b="T_ZZ_"+tableName.Remove(0,2)) && columns|>Seq.exists (fun b->b.COLUMN_NAME=FIDColumnName) |>not then  //有总账表
                   yield false, "该表"+tableName+"的列名"+x+"不规范，按照设计要求, 1. C_XH在独立表中必须为byte类型，2. 关联总账表的同时必须有C_FID字段("+"T_ZZ_"+tableName.Remove(0,2)+"), 3. 所在表只能有一个主键; 具有序号的表可能同时要进行创建，更新和删除的批处理操作，处理量较大"
             | _ ->()
             *)
    }


  //基本表流水号表验证
  let ValidateLSHTable  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       match GetColumnSchemal4Way tableName,tableName.Remove(0,2) with
       | x,y when x|>Seq.exists (fun a->a.COLUMN_NAME=XBHColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith("decimal")) &&  tableNames|>Seq.exists (fun a->a="T_LSH_"+y) |>not ->
           yield false, "该表"+tableName+"中存在列名C_XBH, 而缺少流水号表T_LSH_"+y+"与之对应"
       | _ ->()
    }

  //基本表交易流水号表验证
  let ValidateJYLSHTable  (tableNames:string list)=
    seq{
     for tableName in tableNames do
       match GetColumnSchemal4Way tableName,tableName.Remove(0,2) with
       | x,y when x|>Seq.exists (fun a->a.COLUMN_NAME=JYHColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith("decimal")) && tableName.EndsWithIn ["_ZWJQ";"_LS"]|>not  && tableNames|>Seq.exists (fun a->a="T_JYLSH_"+y) |>not ->   //| x,y when x|>Seq.exists (fun a->a.COLUMN_NAME=JYHColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith("decimal")) && x|>Seq.exists (fun a->a.COLUMN_NAME=ZWYColumnName)|>not  && tableNames|>Seq.exists (fun a->a="T_JYLSH_"+y) |>not ->
           yield false, "该表"+tableName+"中存在列名C_JYH, 而缺少交易流水号表T_JYLSH_"+y+"与之对应"
       | _ ->()
    }

  //单据流水号表验证
  let ValidateDJLSHTable  (tableNames:string list)=
    seq{
      match tableNames with
      | x when x|>Seq.exists (fun a->a="T_DJLX") ->
          match  GetDJLX()
            (*
            seq{
                let db=DatabaseFactory.CreateDatabase()
                let sqlText=
                  @"
                    SELECT C_DM,C_LX,C_QZ,C_BZ FROM T_DJLX 
                  "
                use cmd=new SqlCommand(sqlText)
                use reader=db.ExecuteReader cmd
                while reader.Read() do
                  yield 
                    {C_DM=reader.["C_DM"].ToString()|>byte;
                    C_LX=reader.["C_LX"].ToString();
                    C_QZ=reader.["C_QZ"].ToString();
                    C_BZ=reader.["C_BZ"].ToString()}
            }
            *)
            with
          | a->
              for entity in a do
                match entity with
                | y when  x|>Seq.exists (fun b->b="T_DJLSH_"+ y.C_QZ) |>not ->
                    yield false, "缺少单据流水号表T_DJLSH_"+y.C_QZ+"与存在的单据类型表T_DJLX对应, 在表T_DJLX中没有对应单据流水号表的行记录为C_DM="+(y.C_DM|>string)+",C_LX="+y.C_LX
                | _ ->()
      | _ ->()
    }
           


  //一个外键列, 不能同时对应两个或两个以上的外键， 场景如，单一外键和复合外键同时在一个表中时，数据库将允许一个外键列对应多个外键,  而在ADO.NET 中是此种情况不被支持
  (*Right backup
  let private ValidateForeignKeyColumnRelationshipDesign (tableNames:string list)=
    seq{
      let sb=StringBuilder()
      for tableName in tableNames do
        let asFKRelationships= GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系  
        match 
          asFKRelationships 
          |>Seq.sortBy (fun a->a.FK_COLUMN_NAME) 
          |>Seq.countBy (fun a->a.FK_COLUMN_NAME )  //和使用Seq.groupBy那个更好？？？
          |>Seq.filter (fun (a,b)->b>1) 
          with
        | x when Seq.length x>0 ->
            for a,b in x do //此处不能使用 x|>Seq.iter (fun (a,b)->
              sb.Remove(0,sb.Length)|>ignore
              asFKRelationships
              |>Seq.filter (fun c->c.FK_COLUMN_NAME=a)
              |>Seq.iteri (fun c d->
                  if c>0 then sb.Append(", ") |>ignore
                  sb.Append(d.FOREIGN_KEY)|>ignore)
              yield false, "该表"+tableName+"的列"+a+"包含在"+string b+"个外键中："+string sb+",  设计要求一个表列只能包含在该表的一个外键中，这个设计要求与ADO.NET Entity Framework是一致的，请修正设计！"
        | _ ->()
    }
  *)
  (*
  暂时取消该验证，ADO.NET Entity Framework 4.0已经支持该设计， 2010-07-12
一个外键列, 不能同时对应两个或两个以上的外键， 场景如，单一外键和复合外键同时在一个表中时，数据库将允许一个外键列对应多个外键,  而在ADO.NET 中是此种情况不被支持
  Item1=False     Item2=该表T_XSSP_FZ的列C_KH包含在2个外键中：FK_T_XSSP_FZ_C_KH_T_KH, FK_T_XSSP_FZ_C_XSSP_C_KH_T_XSSP,  设计要求一个表列只能包含在该表的一个外键中，这个设计要求与ADO.NET Entity Framework是一致的，请修正设计！
  Item1=False     Item2=该表T_JHSP_FZ的列C_GHS包含在2个外键中：FK_T_JHSP_FZ_C_GHS_T_GHS, FK_T_JHSP_FZ_C_JHSP_C_GHS_T_JHSP,  设计要求一个表列只能包含在该表的一个外键中，这个设计要求与ADO.NET Entity Framework是一致的，请修正设计！
  *)
  let private ValidateForeignKeyColumnRelationshipDesign (tableNames:string list)=
    seq{
      let sb=StringBuilder()
      for tableName in tableNames do
        match 
          GetAsFKRelationship tableName 
          |>Seq.groupBy (fun a->a.FK_COLUMN_NAME)
          |>Seq.filter (fun (a,b)->Seq.length b>1) 
          with
        | x when Seq.length x>0 ->
            for a,b in x do //此处不能使用 x|>Seq.iter (fun (a,b)->
              sb.Remove(0,sb.Length)|>ignore
              b
              |>Seq.iteri (fun c d->
                  if c>0 then sb.Append(", ") |>ignore
                  sb.Append(d.FOREIGN_KEY)|>ignore)
              yield false, "该表"+tableName+"的列"+a+"包含在"+string (Seq.length b)+"个外键中："+string sb+",  设计要求一个表列只能包含在该表的一个外键中，这个设计要求与ADO.NET Entity Framework是一致的，请修正设计！"
        | _ ->()
    }

  //验证主子表关系级别
  let private  ValidateMainChildRelationshipDesign (tableNames:string list)=
    seq{
      for tableName in tableNames do
        //这里验证的入口表不一定只有一个主键, 所以这里不作限制
        let tableAsPKRelationships=GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
        let mainChildRelationshipOneLevel=
          tableAsPKRelationships 
          |>Seq.filter (fun a->
              let pkColumns=
                a.FK_TABLE 
                |>GetPKColumns
              Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
              &&
              pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键

        let mainChildRelationshipTwoLevels=
          match mainChildRelationshipOneLevel with
          | x when Seq.length x>0 ->
              (Seq.head x).FK_TABLE
              |>GetAsPKRelationship
              |>Seq.filter (fun a->
                  let pkColumns=
                    a.FK_TABLE 
                    |>GetPKColumns
                  Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
                  &&
                  pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
              |>Some
          | _ ->None

        let mainChildRelationshipThreeLevels=
          match mainChildRelationshipTwoLevels with
          | Some(x) when Seq.length x>0  ->
              (Seq.head x).FK_TABLE
              |>GetAsPKRelationship
              |>Seq.filter (fun a->
                  let pkColumns=
                    a.FK_TABLE 
                    |>GetPKColumns
                  Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
                  &&
                  pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
              |>Some
          | _ ->None

        match mainChildRelationshipOneLevel,mainChildRelationshipTwoLevels,mainChildRelationshipThreeLevels with
        (* 如找出 T_DJ_JHGL->T_DJSP_JHGL
        | x, _,_ when Seq.length x >0 ->
            match Seq.head x with
            |u ->yield false, "该表"+tableName+"具有2层主子表关系："+tableName+"->"+u.FK_TABLE+"("+u.FOREIGN_KEY+"), 主子表关系有二级！"
        *)
        (* 如找出 T_CK->T_KCSP->T_KCSP_PC
        | x, Some(y),_ when Seq.length y >0 ->
            match Seq.head x,Seq.head y with
            |u,v ->yield false, "该表"+tableName+"具有3层主子表关系："+tableName+"->"+u.FK_TABLE+"("+u.FOREIGN_KEY+")->"+v.FK_TABLE+"("+v.FOREIGN_KEY+"), 主子表关系有三级！"
        *)
        //(*
        | x,Some(y),Some(z) when Seq.length z>0-> 
            match Seq.head x,Seq.head y, Seq.head z with
            |u,v,w ->yield false, "该表"+tableName+"具有4层主子表关系："+tableName+"->"+u.FK_TABLE+"("+u.FOREIGN_KEY+")->"+v.FK_TABLE+"("+v.FOREIGN_KEY+")->"+w.FK_TABLE+"("+w.FOREIGN_KEY+"), 主子表关系不能超过三级，请更正！"
        //*)
        | _ ->()  //| _ ->yield true,String.Empty
    }
    //|>Seq.filter (fun (a,_)->not a)

  //验证复合外键列可空设计
  let private  ValidateForeignKeyColumnNullableDesign (tableNames:string list)=
    seq{
      for tableName in tableNames do
        let columns=
          GetColumnSchemal4Way tableName
          |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        let asFKRelationships= GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系  
        match 
          (asFKRelationships, columns)
          |>fun(a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME ) (fun b->b.COLUMN_NAME ) (fun a b ->a,b)
          |>Seq.groupBy (fun (a,b) ->a.FOREIGN_KEY )
          |>Seq.map (fun (_,a)->
              match a with
              | y when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) && y|>Seq.exists (fun (_,b)->not b.IS_NULLABLE_TYPED)-> //如果设计正确, 必然有一种情况不存在
                  (*
                  sbTem.Remove(0,sbTem.Length)|>ignore
                  y|>Seq.iter (fun (_,b)->sbTem.Append(b.COLUMN_NAME+",") |>ignore)
                  *)
                  let rec GetColumnNames (columns:(DbFKPK*DbColumnSchemalR) list)=
                    match columns with
                    | (_,v)::t-> v.COLUMN_NAME+","+GetColumnNames t
                    | _ ->String.Empty
                    (*
                    match columns with
                    | h::t-> 
                        match h with 
                        | u,v -> h .COLUMN_NAME+","+GetColumnNames t
                    | _ ->()
                    *)
                  match y|>Seq.head,GetColumnNames (y|>Seq.toList) with
                  | (u,v),w -> false, "该表"+tableName+"与主键表"+u.PK_TABLE+"以外键"+u.FOREIGN_KEY+"关联的外键列"+GetColumnNames (y|>Seq.toList)+"要么全部允许为空，要么全部都不允许为空，请更正！"
              | _ ->true,String.Empty)
          |>Seq.filter (fun (a,_)->not a) with
        | x when Seq.length x>0 ->
            for a in x do
              yield a
        | _ ->()}

  let ValidateColumnDataType (tableNames:string list)=
    seq{
     for tableName in tableNames do
       match GetColumnSchemal4Way tableName with
       | columns-> 
           for n in columns do
             match n.DB_DATA_TYPE, n.COLUMN_NAME with
             | EqualsIn ["ntext";"text";"image"] _, y ->
                 yield false, "该表"+tableName+"的数据列"+y+"设计不规范，按照设计要求, 1. 数据列的类型不能是ntext, text, image, 请参考http://msdn.microsoft.com/en-us/library/ms187993.aspx"
             | _ -> ()
    }

  let ValidateTablesDesign (tableNames:string list)=
    seq{
      yield ValidateKeyTables tableNames
      yield ValidateKeyColumn tableNames
      yield ValidateTableName tableNames
      yield ValidateColumnName tableNames
      yield ValidateColumnDataType tableNames 
      yield ValidateSpecialColumn tableNames
      yield ValidatePrimaryKeyName tableNames
      yield ValidateIndexName tableNames
      yield ValidateForeignKeyRelationshipDesign tableNames
      yield ValidateForeignKeyName tableNames
      yield ValidateForeignKeyColumnNullableDesign tableNames
      //yield ValidateForeignKeyColumnRelationshipDesign tableNames  //暂时取消该验证，ADO.NET Entity Framework 4.0已经支持该设计
      yield ValidateMainChildRelationshipDesign tableNames
      yield ValidateLSHTable tableNames
      yield ValidateDJLSHTable tableNames
      yield ValidateTableWithZZTable tableNames
      yield ValidateZZTableColumn tableNames
      yield ValidateJYLSHTable tableNames
    }
    |>Seq.concat

  (*Right backup
  let ValidateTablesDesign (tableNames:string list)=
    //ValidateTableName tableNames
    //|>Seq.append (ValidateColumnName tableNames)

    match 
      ValidateTableName tableNames,
      ValidateColumnName tableNames,
      ValidateSpecialColumn tableNames,
      ValidatePrimaryKeyName tableNames,
      ValidateForeignKeyRelationshipDesign tableNames,
      ValidateForeignKeyName tableNames,
      ValidateForeignKeyColumnNullableDesign tableNames,
      ValidateForeignKeyColumnRelationshipDesign tableNames,
      ValidateMainChildRelationshipDesign tableNames,
      ValidateLSHTable tableNames,
      ValidateDJLSHTable tableNames
      with
      | x,y,z,u,v,w,o,p,q,r,s -> (Seq.append x<<Seq.append y<<Seq.append z<<Seq.append u<<Seq.append v<<Seq.append w<<Seq.append o<<Seq.append p<<Seq.append q<<Seq.append r)<| s |>Seq.toList //    Seq.append x <| Seq.append y < |z |>Seq.toList //都是按照x->y->z的顺序加入到Seq中
      *)

(*Wrong, Why ??? 
  let private  ValidateForeignKeyDesign1 (tableNames:string list)=
    seq{
      for tableName in tableNames do
        let columns=
          GetColumnSchemal4Way tableName
          |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        let asFKRelationships= GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系  
        (asFKRelationships, columns)
        |>fun(a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME ) (fun b->b.COLUMN_NAME ) (fun a b ->a,b)
        |>Seq.groupBy (fun (a,b) ->a.FOREIGN_KEY )
        |>Seq.map (fun (_,a)->
            match a with
            | y when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) && y|>Seq.exists (fun (_,b)->not b.IS_NULLABLE_TYPED)-> //如果设计正确, 必然有一种情况不存在
                (*
                sbTem.Remove(0,sbTem.Length)|>ignore
                y|>Seq.iter (fun (_,b)->sbTem.Append(b.COLUMN_NAME+",") |>ignore)
                *)
                let rec GetColumnNames (columns:(DbFKPK*DbColumnSchemalR) list)=
                  match columns with
                  | (_,v)::t-> v.COLUMN_NAME+","+GetColumnNames t
                  | _ ->String.Empty
                  (*
                  match columns with
                  | h::t-> 
                      match h with 
                      | u,v -> h .COLUMN_NAME+","+GetColumnNames t
                  | _ ->()
                  *)
                match y|>Seq.head,GetColumnNames (y|>Seq.toList) with
                | (u,v),w -> false, "该表"+tableName+"与主键表"+u.PK_TABLE+"以外键"+u.FOREIGN_KEY+"关联的外键列"+GetColumnNames (y|>Seq.toList)+"要么全部允许为空，要么全部都不允许为空，请更正！"
            | _ ->true,String.Empty)
        |>Seq.filter (fun (a,_)->not a)
        |> fun a -> 
            if Seq.length a>0 then
              for b in a do
                 yield b}
*)  

      
(* Right Reference
    
//Right, use with fsharp feature  
let UpdateFKRelationships (relationships:DbFKPK list)=
  let count=ref 0
  let rec UpdateFKRelationshipsA (relationshipsA:DbFKPK list)  (count:int ref)=
    match relationshipsA with
    | head::snd::tail when snd.PK_TABLE.Equals(head.PK_TABLE) ->   //head.PK_TABLE.Replace(snd.PK_TABLE,String.Empty).ToCharArray() |>Seq.exists (fun a->not <| Char.IsDigit(a) ) |>not-> //当更新PK_TABLE字段时
         if !count=0 then head.PK_TABLE_ALIAS<-head.PK_TABLE  //出现相等的第一对元素
         incr count  //count:=!count+1, --, decr count
         snd.PK_TABLE_ALIAS<-snd.PK_TABLE+string !count
         UpdateFKRelationshipsA (snd::tail) count
    | head::tail -> 
        if !count>0 then head.PK_TABLE_ALIAS<-head.PK_TABLE+string !count; count:=0 //最后一个相等的元素，此时后面一个元素和当前元素不等
        else head.PK_TABLE_ALIAS<-head.PK_TABLE  
        UpdateFKRelationshipsA tail count
    | []->()
  UpdateFKRelationshipsA relationships count
  
UpdateFKRelationships asFKRelationships
  
//Right, use with fsharp feature  
let UpdateFKRelationships01 (relationships:DbFKPK list)=
  let count=ref 0
  let previousPkTableName=ref Unchecked.defaultof<String>
  let rec UpdateFKRelationshipsA (relationshipsA:DbFKPK list)  (count:int ref) (previousPkTableName:string ref)=
    match !previousPkTableName,relationshipsA with
    | x, head::tail when String.IsNullOrEmpty(x) |>not && head.PK_TABLE.Equals(x) ->
         incr count  //count:=!count+1, --, decr count
         head.PK_TABLE_ALIAS<-head.PK_TABLE+string !count
         previousPkTableName:=head.PK_TABLE
         UpdateFKRelationshipsA tail count previousPkTableName
    | _,head::tail -> 
        count:=0 
        head.PK_TABLE_ALIAS<-head.PK_TABLE  
        previousPkTableName:=head.PK_TABLE
        UpdateFKRelationshipsA tail count previousPkTableName
    | _->() // or |_,[]
  UpdateFKRelationshipsA relationships count previousPkTableName
  
UpdateFKRelationships01 asFKRelationships

//Right, use with index
let UpdateFKRelationships (relationships:DbFKPK list)=
  let count=ref 0
  for i=0 to relationships.Length-1 do
    match i,relationships with
    | x,y when x>0 && y.[x].PK_TABLE =y.[x-1].PK_TABLE ->
        incr count
        y.[x].PK_TABLE_ALIAS<-y.[x].PK_TABLE+string !count
    | x,y ->
        count:=0
        y.[x].PK_TABLE_ALIAS<- y.[x].PK_TABLE
        
//Right use with foreach
let UpdateFKRelationshipsGeneral (relationships:DbFKPK list)=
  let count=ref 0
  //let  previous=ref Unchecked.defaultof<DbFKPK>
  let previousPkTableName=ref Unchecked.defaultof<String>
  for a in relationships do
    match !previousPkTableName,a with
    | x,y when String.IsNullOrEmpty(x) |>not && y.PK_TABLE=x->
        incr count
        y.PK_TABLE_ALIAS<-y.PK_TABLE+string !count
    | x,y ->
        count:=0
        y.PK_TABLE_ALIAS<-y.PK_TABLE
    previousPkTableName:=a.PK_TABLE 
    
    
*)
    
/////////////////////////////////////////////////////////////////////////////////////////
    
(* Right, but it can not dispose resources
  let GetAsFKRelationship asFKTableName=
    seq{
      let db=DatabaseFactory.CreateDatabase()
      let sqlText=
        @"
          SELECT OBJECT_NAME(fk.constid) AS 'FOREIGN_KEY', OBJECT_NAME(fk.fkeyid) AS 
          'FK_TABLE', fc.[name] AS 'FK_COLUMN_NAME', OBJECT_NAME(fk.rkeyid) AS 'PK_TABLE', 
          pc.[name] AS 'PK_COLUMN_NAME'
          FROM sysforeignkeys fk
          INNER JOIN syscolumns fc
          ON fk.fkeyid = fc.[id]
          AND fk.fkey = fc.colid
          INNER JOIN syscolumns pc
          ON fk.rkeyid = pc.[id]
          AND fk.rkey = pc.colid
          WHERE OBJECT_NAME(fk.fkeyid)='T_DJ_JHGL'
          ORDER BY OBJECT_NAME(fk.rkeyid) 
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield 
          {FOREIGN_KEY=reader.["FOREIGN_KEY"].ToString();
          FK_TABLE=reader.["FK_TABLE"].ToString();
          FK_COLUMN_NAME=reader.["FK_COLUMN_NAME"].ToString();
          PK_TABLE=reader.["PK_TABLE"].ToString();
          PK_TABLE_ALIAS=String.Empty;
          PK_COLUMN_NAME=reader.["PK_COLUMN_NAME"].ToString()}
      }
    |>Seq.toNetList //保证数据的即时可用性

  
    seq{
      let db=DatabaseFactory.CreateDatabase()
      let reader=
        @"
          SELECT OBJECT_NAME(fk.constid) AS 'FOREIGN_KEY', OBJECT_NAME(fk.fkeyid) AS 
          'FK_TABLE', fc.[name] AS 'FK_COLUMN_NAME', OBJECT_NAME(fk.rkeyid) AS 'PK_TABLE', 
          pc.[name] AS 'PK_COLUMN_NAME'
          FROM sysforeignkeys fk
          INNER JOIN syscolumns fc
          ON fk.fkeyid = fc.[id]
          AND fk.fkey = fc.colid
          INNER JOIN syscolumns pc
          ON fk.rkeyid = pc.[id]
          AND fk.rkey = pc.colid
          ORDER BY OBJECT_NAME(fk.rkeyid) 
        "
        |>fun x->new SqlCommand(x)
        |>db.ExecuteReader
      while reader.Read() do
        yield 
          {FOREIGN_KEY=reader.["FOREIGN_KEY"].ToString();
          FK_TABLE=reader.["FK_TABLE"].ToString();
          FK_COLUMN_NAME=reader.["FK_COLUMN_NAME"].ToString();
          PK_TABLE=reader.["PK_TABLE"].ToString();
          PK_COLUMN_NAME=reader.["PK_COLUMN_NAME"].ToString()}
     }
      

*)


/////////////////////////////////////////////////////////////////////////////////////////






(*
       | "BigInt" ->                              "System.Int64"
       | "Binary" ->                             "System.Byte[]"
       | "Bit" ->                                    "System.Boolean"
       | "Char" ->                                "System.String"
       | "DateTime" ->                       "System.DateTime"
       | "Decimal" ->                          "System.Decimal"
       | "Float" ->                               "System.Double"
       | "Image" ->                             "System.Byte[]"
       | "Int" ->                                    "System.Int32"
       | "Money" ->                            "System.Decimal"
       | "NChar" ->                             "System.String"
       | "NText" ->                             "System.String"
       | "NVarChar" ->                       "System.String"
       | "Real" ->                                 "System.Single"
       | "UniqueIdentifier" ->           "System.Guid"
       | "SmallDateTime" ->             "System.DateTime"
       | "SmallInt" ->                          "System.Int16"
       | "SmallMoney" ->                  "System.Decimal"
       | "Text" ->                                "System.String"
       | "Timestamp" ->                    "System.Byte[]"
       | "TinyInt" ->                            "System.Byte"
       | "VarBinary" ->                       "System.Byte[]"
       | "VarChar" ->                          "System.String"
       | "Variant" ->                            "System.Object"
       | "Xml" 
       | "Udt"
       | "Structured"
       | " Date" 
       | " Time"
       | " DateTime2"
       | " DateTimeOffset" ->"System.String"
       | _ ->"System.String"
*)
       
(*                                                      
       System.Data.DbType.
       System.Data.SqlDbType
       System.TypeCode                 
*)                                                     


(* 下面的代码可用来产生上面的成员
ConfigHelper.INS.LoadDefaultServiceConfigToManager
let db=DatabaseFactory.CreateDatabase() 
let con=db.CreateConnection()
con.Open()
  let restrictionValuesForColumn=[|null;"dbo";"T_CK";null|]
(*
Catalog TABLE_CATALOG 1
Owner TABLE_SCHEMA 2
Table TABLE_NAME 3
Table COLUMN_NAME 4
*)
let tableName="T_CK"
let dataTableAllColumns=con.GetSchema("Columns") //Get all columns from all tables
dataTableAllColumns.Rows.Count
let dataRows=dataTableAllColumns.Select("TABLE_NAME = '" + tableName + "'")
let dataTableColumns=con.GetSchema("Columns",restrictionValuesForColumn) 
dataTableColumns.Rows.Count
dataTableColumns.Rows.[0]

for a in dataTableColumns.Columns do
  ObjectDumper.Write(a.ColumnName)
  
con.Close()
*)

//WX, 其实数据库所有的Schema信息都开放的, 可从该位置查看, SBIIMS0001->Views->System Views->INFORMATION_SCHEMA....

////Database Schemal
//http://www.davidhayden.com/blog/dave/archive/2006/01/15/2734.aspx
//
//http://blog.csdn.net/KimmKing/archive/2009/01/17/3808337.aspx
//http://www.itgrass.com/a/csharp/C-jq/200901/17-8870.html
//
//C# 获取数据库表信息与列信息的方法
//作者：佚名 来源： 发布时间：09-01-17 浏览：12334 次 
//获取表的信息： 
//
//conn.Open(); 
//string[] restrictions = new string[4]; 
//restrictions[1] = "dbo"; 
//DataTable table = conn.GetSchema("Tables", restrictions); 
//conn.Close(); 
//返回的table是表的所有信息，而不仅仅是名字，可以通过如下语句查看这些信息： 
//foreach (System.Data.DataRow row in table.Rows) 
//{ 
//foreach (System.Data.DataColumn col in table.Columns) 
//{ 
//Console.WriteLine("{0} = {1}", col.ColumnName, row[col]); 
//} 
//} 
//要获取指定表的信息，关键是要设置数组restrictions的值。对于表而言，这个数组有如下的含义：
//http://msdn.microsoft.com/en-us/library/cc716722.aspx 
//Restriction[0]表示表所在的Catalog 
//Restriction[1]表示表的所有者 
//Restriction[2]表示表的名字 
//Restriction[3]表示表的类型： 

//上面的例子就获取了所有dbo拥有的表的信息。如果要获取所有的用户表，而非系统表，可用如下语句： 
//
//conn.Open(); 
//string[] restrictions = new string[4]; 
//restrictions[3] = “BASE TABLE"; 
//DataTable table = conn.GetSchema("Tables", restrictions); 
//conn.Close(); 
//
//获取列的信息： 
//conn.Open(); 
//string[] restrictions = new string[4]; 
//restrictions[1] = "dbo"; 
//DataTable table = conn.GetSchema("Columns", restrictions); 
//conn.Close(); 
//与获取表的代码很类似，只是GetSchema的第一个参数不同。同样，返回结果取决于restriction的值。此时， 
//Restriction[0]表示列所在的Catalog 
//Restriction[1]表示列的所有者 
//Restriction[2]表示列所在的表的名字 
//Restriction[3]表示列名 
//例如： 
//// restriction string array 
//string[] res = new string[4]; 
//
//// dbo拥有的所有表的所有列的信息 
//res[1] = "dbo"; 
//DataTable t1 = conn.GetSchema("Columns", res); 
//
//// 任意owner/schema所拥有的一个叫authors的表的列信息 
//res[2] = "authors"; 
//DataTable t2 = conn.GetSchema("Columns", res); 
//
////任意owner/schema所拥有的一个叫authors的表的列name的信息 
//res[2] = "authors"; res[3] = "name "; 
//DataTable t3 = conn.GetSchema("Columns", res); 
//
////任意owner/schema任意表中的一个列名是name的列的信息。 
//res[3] = "name"; 
//DataTable t4 = conn.GetSchema("Columns", res); 
//
//获取数据库的其它信息都可以使用GetSchema，只是第一个参数不同。这个参数在不同的数据库有差异： 
//1、在SQL Server中，可以获取的架构集合如下： http://msdn.microsoft.com/en-us/library/ms254969.aspx , Schema dictionary
//· Databases 
//· ForeignKeys 
//· Indexes 
//· IndexColumns 
//· Procedures 
//· ProcedureParameters 
//· Tables 
//· Columns 
//· Users 
//· Views 
//· ViewColumns 
//· UserDefinedTypes 
//
//2、在Oracle中，可以获取的架构集合如下： 
//· Columns 
//· Indexes 
//· IndexColumns 
//· Procedures 
//· Sequences 
//· Synonyms 
//· Tables 
//· Users 
//· Views 
//· Functions 
//· Packages 
//· PackageBodies 
//· Arguments 
//· UniqueKeys 
//· PrimaryKeys 
//· ForeignKeys 
//· ForeignKeyColumns 
//· ProcedureParameters 


(*
http://msdn.microsoft.com/en-us/library/ms136365(VS.85).aspx
Retrieving Primary Keys        Kathleen Dollard   |   Edit   |   Show History  
Please Wait   NOTE: This note explains how to work around apparent GetSchema limitations with the SQL Client provider. You should NOT resort to system tables when accessing metadata because these tables are likely to change between versions of SQL Server. The INFORMATION_SCHEMA approach discussed here is based on a standard, and as long as Microsoft supports this standard, your code will still work. 
?Primary Keys
It is not obvious how the GetSchema results lets you retrieve primary keys. If they are available, please update this note. Otherwise, you can access these keys through the old fashioned INFORMATION_SCHEMA tools of SQL Server. The Information Schema system is rather esoteric in its usage, so here is the SQL you'd need to retrieve the primary keys for a specific table named LANGUAGE. Replace "LANGUAGE" with your table name. 

SELECT Constraints.CONSTRAINT_NAME, Constraints.TABLE_NAME, COLUMN_NAME, ORDINAL_POSITION
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS Constraints
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS Keys 
ON Constraints.CONSTRAINT_CATALOG = Keys.CONSTRAINT_CATALOG AND
Constraints.CONSTRAINT_SCHEMA = Keys.CONSTRAINT_SCHEMA AND
Constraints.CONSTRAINT_NAME = Keys.CONSTRAINT_NAME 
WHERE CONSTRAINT_TYPE = 'PRIMARY KEY' AND
Constraints.TABLE_NAME = 'LANGUAGE'

You can paste this into a tool like Query Analyzer to test before including in your code. 
?Foreign Keys
I realize there is return information available for "ForiegnKeys" however I don't see how to use that. If someone knows how to use the GetSchema information to retrieve foreign keys, please updat this note.
Otherwise, you can fall back on the old fashioned INFORMATION_SCHEMA tools of SQL Server. The keys are retreived in a manner similar to the Primary keys, but this makes the TSQL nearly unreadable and the resulting denormalized result may not be useful. So, this block of TSQL simply illustrates how to get a list of foreign key values in your database. You can test this by pasting into Query Analyzer. 

SELECT Parent.TABLE_NAME AS ParentTableName, 
Child.TABLE_NAME AS ChildTableName
FROM INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE AS Parent 
INNER JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS As Constraints
ON Constraints.UNIQUE_CONSTRAINT_CATALOG = Parent.CONSTRAINT_CATALOG AND
Constraints.UNIQUE_CONSTRAINT_SCHEMA = Parent.CONSTRAINT_SCHEMA AND
Constraints.UNIQUE_CONSTRAINT_NAME = Parent.CONSTRAINT_NAME 
INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_TABLE_USAGE AS Child
ON Constraints.CONSTRAINT_CATALOG = Child.CONSTRAINT_CATALOG AND
Constraints.CONSTRAINT_SCHEMA = Child.CONSTRAINT_SCHEMA AND
Constraints.CONSTRAINT_NAME = Child.CONSTRAINT_NAME 


*)


/////////////////////////////////////////////////////////////////////////////////


(*  //For Foreign Key Columns 

Hi Dave,

As far as I know, ADO.net doesn't provide such a way to Get FK:PK 
relationships from a column. You may check system tables in SQL database. 
(Such as sys.foreign_key_columns table.) But, I'm afraid there's a lot of 
work to do here.....

For example: in SQL 2005
-- get constraint name
select Constraint_Name from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE 
where column_name='c2' and Table_Name='table_2'

-- get oject_id from constraintName
select object_id from sys.objects where name='ConstraintName'

-- get parent table, parent column, referenced table and column id.
select 
parent_object_id,parent_column_id,referenced_object_id,referenced_column_id 
from sys.foreign_key_columns where constraint_object_id='objectid'

-- get table and column name
select name from sys.objects where object_id='referenced_object_id'
select Column_Name from INFORMATION_SCHEMA.COLUMNS where table_name='Name' 
and Ordinal_position='ReferencedColumnId'

Hope this helps. Please let me know if you still have anything unclear. 
Best regards,
Wen Yuan
Microsoft Online Community Support
==================================================
This posting is provided "AS IS" with no warranties, and confers no rights.

//wx test
-- get constraint name
select Constraint_Name from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE 
where Table_Name='T_DJ_JHGL'

-- get oject_id from constraintName
select object_id from sys.objects where name='FK_T_DJ_JHGL_C_CZY_T_YG'

-- get parent table, parent column, referenced table and column id.
select 
parent_object_id,parent_column_id,referenced_object_id,referenced_column_id 
from sys.foreign_key_columns where constraint_object_id='388964512'

-- get table and column name
select name from sys.objects where object_id='1881109792'
select Column_Name from INFORMATION_SCHEMA.COLUMNS where table_name='T_YG' 
and Ordinal_position='4'

//----------------------------------------------

//************************************************************
Dave Got one better for u 
putting it all together for ya

works on sql server 2000 and 2005


SELECT OBJECT_NAME(f.constid) AS 'ForeignKey', OBJECT_NAME(f.fkeyid) AS 
'FKTable', c1.[name] AS 'FKColumnName', OBJECT_NAME(f.rkeyid) AS 'PKTable', 
c2.[name] AS 'PKColumnName'
FROM sysforeignkeys f
INNER JOIN syscolumns c1
ON f.fkeyid = c1.[id]
AND f.fkey = c1.colid
INNER JOIN syscolumns c2
ON f.rkeyid = c2.[id]
AND f.rkey = c2.colid
ORDER BY OBJECT_NAME(f.rkeyid) 


//wx updated it

SELECT OBJECT_NAME(fk.constid) AS 'ForeignKey', OBJECT_NAME(fk.fkeyid) AS 
'FKTable', fc.[name] AS 'FKColumnName', OBJECT_NAME(fk.rkeyid) AS 'PKTable', 
pc.[name] AS 'PKColumnName'
FROM sysforeignkeys fk
INNER JOIN syscolumns fc
ON fk.fkeyid = fc.[id]
AND fk.fkey = fc.colid
INNER JOIN syscolumns pc
ON fk.rkeyid = pc.[id]
AND fk.rkey = pc.colid
ORDER BY OBJECT_NAME(fk.rkeyid) 


//获取各种数据库的Schema,提供原代码下载， kailua_src.1.0.2.0.zip
//本地文件路径 D:\Work Source\AAAA-DotNetFramework\ADO.NET\Database Schema\kailua_src.1.0.2.0\Kailua
//http://www.windwardreports.com/open_source.htm

//************************************************************

*)


(* 包括表的描述信息(Description),字段的描述信息
http://topic.csdn.net/u/20100223/14/4cd225df-17af-4079-8925-3ca2d7d763d3.html
SELECT 
    表名      = case when a.colorder=1 then d.name else '' end, 
    表说明    = case when a.colorder=1 then isnull(f.value,'') else '' end, 
    字段序号  = a.colorder, 
    字段名    = a.name, 
    标识      = case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then '√'else '' end, 
    主键      = case when exists(SELECT 1 FROM sysobjects where xtype='PK' and parent_obj=a.id and name in ( 
                    SELECT name FROM sysindexes WHERE indid in( SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid))) then '√' else '' end, 
    类型      = b.name, 
    占用字节数 = a.length, 
    长度      = COLUMNPROPERTY(a.id,a.name,'PRECISION'), 
    小数位数  = isnull(COLUMNPROPERTY(a.id,a.name,'Scale'),0), 
    允许空    = case when a.isnullable=1 then '√'else '' end, 
    默认值    = isnull(e.text,''), 
    字段说明  = isnull(g.[value],'') 
FROM 
    syscolumns a 
left join 
    systypes b 
on 
    a.xusertype=b.xusertype 
inner join 
    sysobjects d 
on 
    a.id=d.id  and d.xtype='U' and  d.name <>'dtproperties' 
left join 
    syscomments e 
on 
    a.cdefault=e.id 
left join 
sys.extended_properties  g 
on 
    a.id=G.major_id and a.colid=g.minor_id  
left join 

sys.extended_properties f 
on 
    d.id=f.major_id and f.minor_id=0 
where 
    d.name='T_GHS'    --如果只查询指定表,加上此条件 
order by 
    a.id,a.colorder 
*)