(*
Schema Restrictions (ADO.NET)
http://msdn.microsoft.com/en-us/library/cc716722.aspx  for .NET Framework 3.5

Schema dictionary
http://msdn.microsoft.com/en-us/library/ms254969.aspx   for NET 3.5
http://msdn.microsoft.com/en-us/library/ms254969(VS.100).aspx  for NET 4.0

//WX, 其实数据库所有的Schema信息都开放的, 可从该位置查看, SBIIMS0001->Views->System Views->INFORMATION_SCHEMA....

Design:
1. 可以使用 Join 将 字段信息进行组合， 然后再创建相应的Recrod type

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
open WX.Data.Database

[<AutoOpen>]
module ValidateDatabaseDesignX =
  //====================================================================================

  //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况  
  let private ValidateForeignKeyColumnDesign (tableInfo:TableInfo)=
      (*
      let sbTem=StringBuilder()
      *)
    (tableInfo.TableForeignKeyRelationshipInfos, tableInfo.TableColumnInfos)
    |>fun(a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME ) (fun b->b.COLUMN_NAME ) (fun a b ->a,b)
    |>Seq.groupBy (fun (a,b) ->a.FOREIGN_KEY)
    |>Seq.map (fun a->
        match a with
        | _,y when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) && y|>Seq.exists (fun (_,b)->not b.IS_NULLABLE_TYPED)-> //如果设计正确, 必然有一种情况不存在
            let rec GetColumnNames (columns:(TableForeignKeyRelationshipInfo*TableColumnInfo) list)=
              match columns with
              | (_,v)::t-> v.COLUMN_NAME+","+GetColumnNames t
              | _ ->String.Empty
            match y|>Seq.head,GetColumnNames (y|>Seq.toList) with
            | (u,v),w -> false, "该表"+tableInfo.TABLE_NAME+"与主键表"+u.PK_TABLE+"以外键"+u.FOREIGN_KEY+"关联的外键列"+GetColumnNames (y|>Seq.toList)+"要么全部允许为空，要么全部都不允许为空，请更正！"
        | _ ->true,String.Empty)
    |>Seq.filter (fun (a,_)->not a)
    |>Seq.toList
 

  //验证是否有日志表
  let private ValidateKeyTables (tableInfos:TableInfo[])=
    seq{
      //match tableNames|>Seq.exists (fun a->a.StartsWith "T_" && a.EndsWith "_RZ") with
      match tableInfos|>Seq.exists (fun a->a.TABLE_NAME.Equals "T_RZ") with //所有数据库使用同一个日志表名，这样便于自动代码和日志数据合并
      | false ->
          yield false,"该数据库没有日志表T_RZ, 按照设计要求，每个数据库都应该拥有特定名称的日志表，请修改！"
      | _ ->()
    }

  let private ValidateZZTableColumn (tableInfos:TableInfo [])=
    seq{
      //验证总帐表和主表中的Guid字段和DateTime字段要一一对应
      for tableInfo in tableInfos do
        match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME="T_ZZ_"+tableInfo.TABLE_NAME.Remove(0,2)) with
        | Some x -> //有总账表
            for a in x.TableColumnInfos do
              match a.COLUMN_NAME,a.DATA_TYPE with
              | y, EndsWithIn DateTimeConditions _  ->
                    if tableInfo.TableColumnInfos|>Seq.exists (fun b->b.COLUMN_NAME=y)|>not then
                      yield false, "该表"+tableInfo.TABLE_NAME+"中没有时间字段"+y+"与的总账表"+x.TABLE_NAME+"的时间字段"+y+"一一对应，按照设计要求，总账表中的时间字段要和主表中的时间字段一一对应，请修改！"
              | y,EndsWithIn GuidConditions _  when  x.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME=y ) |>not -> //不是主键列
                    if tableInfo.TableColumnInfos|>Seq.exists (fun b->b.COLUMN_NAME=y)|>not then
                      match 
                        x.TableForeignKeyRelationshipInfos
                        |>Seq.tryFind (fun b->b.FK_COLUMN_NAME=y) with 
                      | Some w  when w.PK_TABLE=tableInfo.TABLE_NAME -> ()//x.PK_COLUMN_NAME
                      | _ -> yield false,"该表"+tableInfo.TABLE_NAME+"中没有Guid字段"+y+"与的总账表"+x.TABLE_NAME+"的Guid字段"+y+"一一对应，按照设计要求，总账表中的Guid字段除主键列外都要和主表中的Guid字段一一对应，请修改！"
              | _ ->()
        | _ ->()

      //验证总帐表，总账表中如果有String或byte[]字段，必须设计为可空
      for tableInfo in  tableInfos do
        match tableInfo.TABLE_NAME.StartsWith "T_ZZ_" with
        | true ->
            for column in 
              tableInfo.TableColumnInfos  do
              match column.COLUMN_NAME,column.DATA_TYPE,column.IS_NULLABLE_TYPED with
              | x, EndsWithIn NullableTypeConditions _,false ->
                  yield false,"该总账表"+tableInfo.TABLE_NAME+"的字段"+x+"不能设计为不可空，按照设计要求，总账表中的String类型和byte[]类型的字段必须设计为可空，因为总账表和基本表是同时创建的，非关键字段都使用默认值！请修改！"
              | _ ->()

        | _ ->()
    }


  //验证表是否存在主键
  let private ValidateKeyColumn  (tableInfos:TableInfo [])=
    seq{
      for tableInfo in  tableInfos do
        match
          tableInfo.TablePrimaryKeyInfos
          |>Seq.length  with
        | x when x>0 |>not ->
            yield false,"该表"+tableInfo.TABLE_NAME+"没有主键，按照设计要求，每个表都应该拥有主键，请修改！"
        | _ ->()
    }

  (*
   //总账表还应与主表有一致的主键，并且总账表中的外键也需要在主表中存在
  *)
  let private ValidateTableWithZZTable  (tableInfos:TableInfo [])=
    seq{
     let sb=new StringBuilder()
     let keyColumnStr:string ref=ref String.Empty
     let zzKeyColumnStr:string ref=ref String.Empty
     let relationShipStr:string ref=ref String.Empty
     for tableInfo in tableInfos do
       match 
         tableInfos
         |>Seq.tryFind (fun a->a.TABLE_NAME="T_ZZ_"+match tableInfo.TABLE_NAME with x ->x.Remove(0,2)) with
       | Some x->
           match
             tableInfo.TablePrimaryKeyRelationshipInfos
             |>Seq.filter (fun b->b.FK_TABLE=x.TABLE_NAME)
             |>Seq.distinctBy(fun b->b.PK_COLUMN_NAME), //主键在总账表中有两个对应外键时如T_DWBM和T_ZZ_DWBM
             tableInfo.TablePrimaryKeyInfos,
             x.TablePrimaryKeyInfos with
           | u,v,_ when Seq.length v>0 && Seq.length u>0|>not->
               yield false,"该表"+ tableInfo.TABLE_NAME+"和总账表"+x.TABLE_NAME+"没有建立主外键关系, 请修改!"
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
               yield false,"该表"+ tableInfo.TABLE_NAME+"和关联的总账表"+x.TABLE_NAME+"没有建立完整的主外键关系, 请修改！当前表和总账表的主外键关系为("+ !relationShipStr+"), 表"+tableInfo.TABLE_NAME+"的主键列为"+ !keyColumnStr+" 而总账表"+x.TABLE_NAME+"的主键列为"+ !zzKeyColumnStr
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
                   yield false,"该表"+ tableInfo.TABLE_NAME+"和关联的总账表"+x.TABLE_NAME+"主键不一致,请修改！ 当前表和总账表的主外键关系为("+ !relationShipStr+"), 表"+tableInfo.TABLE_NAME+"的主键列为"+ !keyColumnStr+" 而总账表"+x.TABLE_NAME+"的主键列为"+ !zzKeyColumnStr
       | _ ->()
    } 

  //互为外键的验证, 
  (* 对于True的提示逻辑, 没有考虑一个外键由多个键列构成的情况
  let private ValidateForeignKeyRelationshipDesign  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       for a,b in
         (tableInfo.TableForeignKeyRelationshipInfos,tableInfo.TableColumnInfos)
         |>fun (a,b)->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b->a,b)
         do
         match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.PK_TABLE),b.IS_NULLABLE_TYPED with
         | Some x,false -> 
             match x.TableForeignKeyRelationshipInfos|>Seq.filter (fun c ->c.PK_TABLE=tableInfo.TABLE_NAME) with
             | u when Seq.length u >0 ->
                  match 
                    x.TableColumnInfos
                    |>Seq.filter (fun c->c.COLUMN_NAME=(Seq.head u).FK_COLUMN_NAME)
                    |>Seq.head with
                  | v when not v.IS_NULLABLE_TYPED ->
                      yield false, "该表"+tableInfo.TABLE_NAME+"("+a.FOREIGN_KEY+")"+"与表"+x.TABLE_NAME+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表，并且表"+tableInfo.TABLE_NAME+"的外键列"+b.COLUMN_NAME+"和表"+x.TABLE_NAME+"的外键"+(Seq.head u).FK_COLUMN_NAME+"均不允许为空，这将造成这两个的互锁，请更正！"
                  | v ->
                     yield true, "该表"+tableInfo.TABLE_NAME+"("+a.FOREIGN_KEY+")"+"与表"+x.TABLE_NAME+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表, 其中表"+x.TABLE_NAME+"("+ (Seq.head u).FOREIGN_KEY+")的相关外键列"+v.COLUMN_NAME+"已设计为允许空，未互锁，该设计可行，但其中的一个表的删除操作将受到约束！"
             | _ ->()
         | _ ->()
    } 
  *)
  let private ValidateForeignKeyRelationshipDesign  (tableInfos:TableInfo [])=
    seq{
     let sb=StringBuilder()
     let columnStringFst=ref String.Empty
     let columnStringSnd=ref String.Empty
     for tableInfo in tableInfos do
       for a,b in
         (tableInfo.TableForeignKeyRelationshipInfos, tableInfo.TableColumnInfos)
         |>fun (a,b)->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b->a,b)
         |>Seq.groupBy (fun (a,_)->a.PK_TABLE)
         |>Seq.choose (fun (a,b) ->
             match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a) with
             | Some x ->Some (x, b)
             | _ ->None
             )
         |>Seq.toArray
         do
         match a.TableForeignKeyRelationshipInfos|>Seq.filter (fun c ->c.PK_TABLE=tableInfo.TABLE_NAME) with
         | u when Seq.length u >0 -> //存在两个表互为外键表
             match 
               b|>Seq.exists (fun (_,d)->d.IS_NULLABLE_TYPED) |>not, //第一个表的指定外键的外键列全部都设计为不允许为空
               a.TableColumnInfos
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
                   yield false, "该表"+tableInfo.TABLE_NAME+"("+(b|>Seq.head|>fst).FOREIGN_KEY+")"+"与表"+a.TABLE_NAME+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表，并且表"+tableInfo.TABLE_NAME+"的外键列"+ !columnStringFst+"和表"+a.TABLE_NAME+"的外键列"+ !columnStringSnd+"均设计为不允许为空，这将造成这两个的互锁，请更正！"
               | _ ->
                   yield false, "该表"+tableInfo.TABLE_NAME+"("+(b|>Seq.head|>fst).FOREIGN_KEY+")"+"与表"+a.TABLE_NAME+"("+ (Seq.head u).FOREIGN_KEY+")"+"互为外键表, 其中的一个表的相关外键列已设计为允许空，未互锁，该设计可行, 但自动编码模块暂时不支持该设计！！！ 此时外键关系的主表实体将是一个集合实例，而正常情况下外键关系的主表实体只是一个单一记录实例, 并且其中的一个表的删除操作将受到约束, 请考虑该设计是否必要！"
         | _ ->()
    } 

  (*
  //验证构成外键的字段不能同时为C_ID, （在实际设计中此种情况是可以的）
  let ValidateForeignKeyRelationship  (tableNames:string list)=
    seq[]
  *)
  let private ValidateSpecialColumn  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       for a in
         tableInfo.TableColumnInfos
         do
         match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),a.IS_NULLABLE_TYPED with
         | EqualsIn ["C_CJRQ";"C_GXRQ"] x, NotEndsWith "datetime" _, _ 
         | EqualsIn ["C_CJRQ";"C_GXRQ"] x, _, true ->
             yield false, "该表"+tableInfo.TABLE_NAME+"的字段"+x+"必须为日期类型, 并且不能为空，请更正！"
         | _ ->()
         match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),a.IS_NULLABLE_TYPED with
         | EqualsIn ["C_DWBM";"C_FBID"] x, NotEndsWith "guid" _, _ 
         | EqualsIn ["C_DWBM";"C_FBID"] x, _, true ->
             yield false, "该表"+tableInfo.TABLE_NAME+"的字段"+x+"必须为uniqueidentifier类型, 并且不能为空，请更正！"
         | _ ->()
         match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),a.IS_NULLABLE_TYPED with
         | EqualsIn ["C_PC"] x, NotEndsWith DecimalTypeName _, _ 
         | EqualsIn ["C_PC"] x, _, true ->
             yield false, "该表"+tableInfo.TABLE_NAME+"的字段"+x+"必须为decimal类型, 并且不能为空，请更正！"
         | _ ->()
    } 

  let private ValidateForeignKeyName  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       for a in 
         tableInfo.TableForeignKeyRelationshipInfos
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
                 yield false, "该表"+tableInfo.TABLE_NAME+"的外键名"+x+"不规范，按照设计要求, 1. 外键名构成为：'FK_'+当前表名+构成外键的键列名(多个键列名前后以'_'分隔)+构成外键的主键表名, 2. 外键关系的键列名与外键名称所包含的字段名不符"
             | _ ->()
         (*
         | x when not<| Regex.IsMatch(x,@"^FK_T_[A-Z_]+C+_+[A-Z_]+_T_[A-Z_]*[A-Z]$",RegexOptions.None) ->
             yield false, "该表"+tableInfo.TABLE_NAME+"的外键名"+x+"不规范，按照设计要求, 1. 外键名只能由大写字母和'_'构成, 2. 外键名只能以FK_T开头, 3. 外键名中间部分必须包含带C_的字段名, 4. 外键名结尾部分要带T_并以字母结尾"
         | _ ->()
         *)
    }

  let private ValidatePrimaryKeyName  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       for a in
         tableInfo.TablePrimaryKeyInfos 
         |>Seq.distinctBy (fun a->a.INDEX_NAME ) //一个索引对应多个键列
         do
         match a.INDEX_NAME with
         | x when (x.StartsWith("PK_") &&  x<>"PK_"+a.TABLE_NAME) || not<|Regex.IsMatch(x,@"^(PK|IX)_[A-Z_0-9]*[A-Z0-9]$",RegexOptions.None) ->
             yield false, "该表"+tableInfo.TABLE_NAME+"的索引名"+x+"不规范，按照设计要求, 1. 索引名只能由大写字母，'_'，或数字构成, 2. 索引名只能以PK_或IX_开头, 3. 索引名要以字母或者数字结尾,  4. 以PK_开头时必须以所在表的表名结尾"
         | _ ->()
    }

  let private ValidateIndexName  (tableInfos:TableInfo [])=
    seq{
      for tableInfo in tableInfos do
        for (a,b) in 
          tableInfo.TableIndexInfos
          |>Seq.groupBy (fun a ->a.INDEX_NAME )
          do
          match "IX_"+tableInfo.TABLE_NAME with
          | x when a.Length=x.Length || a.StartsWith x|>not || 
              match a.Substring(x.Length).Split([|"_C_"|],StringSplitOptions.RemoveEmptyEntries) with
              | y ->
                  Seq.length y<>Seq.length b ||
                  match y|>Seq.map (fun c->"C_"+c) with
                  | w ->
                      w|>Seq.forall (fun c->b|>Seq.exists (fun d->d.COLUMN_NAME=c))|>not ||
                      b|>Seq.forall (fun c->w|>Seq.exists (fun d->d=c.COLUMN_NAME))|>not
              ->
              yield false, "该表"+tableInfo.TABLE_NAME+"的索引名"+a+"不规范，按照设计要求, 1. 索引名构成为：'IX_'+当前表名+构成索引列名(多个索引列名前后以'_'分隔), 2. 索引列名与索引名称所包含的字段名称不符"
          | _ ->() 
       }

  let private ValidateTableName  (tableInfos:TableInfo [])=
    seq{
      for tableInfo in tableInfos do
        match tableInfo.TABLE_NAME with
        | x when not<|Regex.IsMatch(x,@"^T_[A-Z_0-9]*[A-Z0-9]$",RegexOptions.None) ->
            yield false, "该表"+x+"的表名不规范，按照设计要求, 1. 表名只能由大写字母，'_'，或数字构成, 2. 表名只能以T_开头, 3. 表名要以字母或数字结尾"
        | _ ->()
    }

  let private ValidateColumnName  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       match tableInfo.TableColumnInfos with
       | columns-> 
           for a in columns do
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant() with
             | NotMatch @"^C_[A-Z_]*[A-Z]$" x,_  ->
                 yield false, "该表"+tableInfo.TABLE_NAME+"的列名"+x+"不规范，按照设计要求, 1. 列名只能由大写字母和'_'构成, 2. 列名只能以C_开头, 3. 列名要以字母结尾"
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
                         yield false, "该表"+tableInfo.TABLE_NAME+"的列名"+x+"不规范，按照设计要求, 存在以'JM'后缀结尾的列名时，必须同时存在去掉该列名的'JM'后缀之后的列名，且他们的字段类型均为'String', 多个'JM'作为后缀也是不允许的 "
                 | _ ->()
             | _ -> ()
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant() with
             | NotEndsWithIn ["BZ";"_DM"] x,EndsWith BoolTypeName _-> //C_DM在流水号表中的类型为Boolean
                 yield false, "该表"+tableInfo.TABLE_NAME+"的列名"+x+"不规范，按照设计要求, 1. Bool类型字段的列名1. 只能由大写字母和'_'构成, 2. 列名只能以C_开头, 3. 列名要以BZ或_DM结尾"
             | _ ->()
             //流水号创建或更新后，需要将生成的编号返回, 所以需要的约束较多
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(), tableInfo.TABLE_NAME,tableInfo.TablePrimaryKeyInfos with
             | EqualsIn [XBHColumnName] x,NotEndsWith DecimalTypeName y, xt, _ -> //C_XBH在表中必须为decimal类型
                 yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为decimal类型(当前类型为"+y+"), 2. 不能在包含_DJ_,_FDJ_,_FKD_和_HZDJ的表中, 3. 所在表只能有一个主键, 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [XBHColumnName] x, _, ContainsIn ["_DJ_";"_FDJ_";"_HZDJ";"_FKD_"] xt, _->   //C_XBH不能在单据表中
                 yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为decimal类型, 2. 不能在包含_DJ_,_FDJ_和_HZDJ,_FKD_的表中(当前表名为"+xt+"), 3. 所在表只能有一个主键, 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [XBHColumnName] x, _, xt, y when Seq.length y >1  || Seq.length y<1-> 
                 yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为decimal类型, 2. 不能在包含_DJ_,_FDJ_和_HZDJ,_FKD_的表中, 3. 所在表只能有一个主键(当前主键列有"+(Seq.length y|>string)+"个), 4. 所在表的主键列类型必须为Guid"
             | EqualsIn [XBHColumnName] x, _, xt, y ->
                 match columns|>Seq.find (fun b->b.COLUMN_NAME=(Seq.head y).COLUMN_NAME)|>fun b->b.COLUMN_NAME,b.DATA_TYPE.ToLowerInvariant() with
                 | z, NotEndsWith GuidTypeName w ->
                     yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_XBH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表只能有一个主键,  4. 所在表的主键列类型必须为Guid(当前主键列"+z+"的类型为"+w+")"
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
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(),tableInfo.TABLE_NAME,tableInfo.TablePrimaryKeyInfos  with
             | EqualsIn [DJHColumnName] x, NotEndsWith "string" y, xt, _ -> //C_DJH在表中必须为string类型
                 yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型(当前类型为"+y+"), 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, NotContainsIn ["_DJ";"_FDJ";"_HZDJ";"_FKD";"_QFMX_"] xt, _ ->   //C_DJH只能在包含_DJ和_FDJ的表中,包括主表和字表
                 yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中(当前表名为"+xt+"), 3. 所在表的主键列类型必须为Guid"
             | EqualsIn [DJHColumnName] x, _, xt, y ->
                 match columns|>Seq.find (fun b->b.COLUMN_NAME=(Seq.head y).COLUMN_NAME)|>fun b->b.COLUMN_NAME,b.DATA_TYPE.ToLowerInvariant() with
                 | z, NotEndsWith "guid" w ->
                     yield false, "该表"+xt+"的列名"+x+"不规范，按照设计要求, 1. C_DJH在表中必须为string类型, 2, 只能设计在格式为T_DJ_XX, T_FDJ_XX, T_FKD_XX和T_HZDJ的表中, 3. 所在表的主键列类型必须为Guid(当前主键列"+z+"的类型为"+w+")"
                 | _ ->() 
             | _ ->()
             //序号验证
             match a.COLUMN_NAME,a.DATA_TYPE.ToLowerInvariant(), tableInfo.TablePrimaryKeyInfos with
             | EqualsIn [XHColumnName] x,NotEndsWith "byte" _, z when Seq.length z=1-> //C_XH,序号在表中必须为byte类型， T_DJSP_XX中的C_XH不受该约束，因为其主键列均大于1
                 yield false, "该表"+tableInfo.TABLE_NAME+"的列名"+x+"不规范，按照设计要求, 1. C_XH在独立表中必须为byte类型, 2. 所在表只能有一个主键;  具有序号的表可能同时要进行创建，更新和删除的批处理操作，处理量较大"
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
  let private ValidateLSHTable  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       match tableInfo.TableColumnInfos, tableInfo.TABLE_NAME.Remove(0,2) with
       | x,y when x|>Seq.exists (fun a->a.COLUMN_NAME=XBHColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith("decimal")) &&  tableInfos|>Seq.exists (fun a->a.TABLE_NAME="T_LSH_"+y) |>not ->
           yield false, "该表"+tableInfo.TABLE_NAME+"中存在列名C_XBH, 而缺少流水号表T_LSH_"+y+"与之对应"
       | _ ->()
    }

  //基本表交易流水号表验证
  let private ValidateJYLSHTable  (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       match tableInfo.TableColumnInfos,tableInfo.TABLE_NAME.Remove(0,2) with
       | x,y when x|>Seq.exists (fun a->a.COLUMN_NAME=JYHColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith("decimal")) && tableInfo.TABLE_NAME.EndsWithIn ["_ZWJQ";"_LS"]|>not  && tableInfos|>Seq.exists (fun a->a.TABLE_NAME="T_JYLSH_"+y) |>not ->  
           yield false, "该表"+tableInfo.TABLE_NAME+"中存在列名C_JYH, 而缺少交易流水号表T_JYLSH_"+y+"与之对应"
       | _ ->()
    }

  //单据流水号表验证
  let  private ValidateDJLSHTable  (tableInfos:TableInfo [], databaseName)=
    seq{
      match tableInfos with
      | x when x|>Seq.exists (fun a->a.TABLE_NAME="T_DJLX") ->
          match  DatabaseInformationX.GetDJLX(databaseName)
            with
          | a->
              for entity in a do
                match entity with
                | y when  x|>Seq.exists (fun b->b.TABLE_NAME="T_DJLSH_"+ y.C_QZ) |>not ->
                    yield false, "缺少单据流水号表T_DJLSH_"+y.C_QZ+"与存在的单据类型表T_DJLX对应, 在表T_DJLX中没有对应单据流水号表的行记录为C_DM="+(y.C_DM|>string)+",C_LX="+y.C_LX
                | _ ->()
      | _ ->()
    }
           
  let private ValidateForeignKeyColumnRelationshipDesign (tableInfos:TableInfo [])=
    seq{
      let sb=StringBuilder()
      for tableInfo in tableInfos do
        match 
          tableInfo.TableForeignKeyRelationshipInfos
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
              yield false, "该表"+tableInfo.TABLE_NAME+"的列"+a+"包含在"+string (Seq.length b)+"个外键中："+string sb+",  设计要求一个表列只能包含在该表的一个外键中，这个设计要求与ADO.NET Entity Framework是一致的，请修正设计！"
        | _ ->()
    }

  //验证主子表关系级别
  let private  ValidateMainChildRelationshipDesign (tableInfos:TableInfo [])=
    seq{
      for tableInfo in tableInfos do
        //这里验证的入口表不一定只有一个主键, 所以这里不作限制
        let tableAsPKRelationships=tableInfo.TablePrimaryKeyRelationshipInfos //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
        let mainChildRelationshipOneLevel=
          tableAsPKRelationships 
          |>Seq.filter (fun a->
              let pkColumns=
                match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                | Some x ->x.TablePrimaryKeyInfos
                | _ ->[||]
              Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
              &&
              pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键

        let mainChildRelationshipTwoLevels=
          match mainChildRelationshipOneLevel with
          | x when Seq.length x>0 ->
              match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME= (Seq.head x).FK_TABLE) with
              | Some y ->
                  y.TablePrimaryKeyRelationshipInfos 
                  |>Seq.filter (fun a->
                      let pkColumns=
                        match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                        | Some z ->z.TablePrimaryKeyInfos
                        | _ ->[||]
                      Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
                      &&
                      pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                  |>Some
              | _ ->None
          | _ ->None

        let mainChildRelationshipThreeLevels=
          match mainChildRelationshipTwoLevels with
          | Some(x) when Seq.length x>0  ->
              match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME= (Seq.head x).FK_TABLE) with
              | Some y ->
                  y.TablePrimaryKeyRelationshipInfos
                  |>Seq.filter (fun a->
                      let pkColumns=
                        match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                        | Some z ->z.TablePrimaryKeyInfos
                        | _ ->[||]
                      Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
                      &&
                      pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                  |>Some
              | _ ->None
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
        | x,Some(y),Some(z) when Seq.length z>0-> 
            match Seq.head x,Seq.head y, Seq.head z with
            |u,v,w ->yield false, "该表"+tableInfo.TABLE_NAME+"具有4层主子表关系："+tableInfo.TABLE_NAME+"->"+u.FK_TABLE+"("+u.FOREIGN_KEY+")->"+v.FK_TABLE+"("+v.FOREIGN_KEY+")->"+w.FK_TABLE+"("+w.FOREIGN_KEY+"), 主子表关系不能超过三级，请更正！"
        | _ ->() 
    }

  //验证复合外键列可空设计
  let private  ValidateForeignKeyColumnNullableDesign (tableInfos:TableInfo [])=
    seq{
      for tableInfo in tableInfos do
        let columns=
          tableInfo.TableColumnInfos
          |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        let asFKRelationships= tableInfo.TableForeignKeyRelationshipInfos//获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系  
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
                  let rec GetColumnNames (columns:(TableForeignKeyRelationshipInfo*TableColumnInfo) list)=
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
                  | (u,v),w -> false, "该表"+tableInfo.TABLE_NAME+"与主键表"+u.PK_TABLE+"以外键"+u.FOREIGN_KEY+"关联的外键列"+GetColumnNames (y|>Seq.toList)+"要么全部允许为空，要么全部都不允许为空，请更正！"
              | _ ->true,String.Empty)
          |>Seq.filter (fun (a,_)->not a) with
        | x when Seq.length x>0 ->
            for a in x do
              yield a
        | _ ->()}

  let private ValidateColumnDataType (tableInfos:TableInfo [])=
    seq{
     for tableInfo in tableInfos do
       match tableInfo.TableColumnInfos with
       | columns-> 
           for n in columns do
             match n.DB_DATA_TYPE, n.COLUMN_NAME with
             | EqualsIn ["ntext";"text";"image"] _, y ->
                 yield false, "该表"+tableInfo.TABLE_NAME+"的数据列"+y+"设计不规范，按照设计要求, 1. 数据列的类型不能是ntext, text, image, 请参考http://msdn.microsoft.com/en-us/library/ms187993.aspx"
             | _ -> ()
    }

  let ValidateTablesDesignX (tableInfos:TableInfo [], databaseName)=
    seq{
      yield! ValidateKeyTables tableInfos|>Seq.toArray
      yield! ValidateKeyColumn tableInfos|>Seq.toArray
      yield! ValidateTableName tableInfos|>Seq.toArray
      yield! ValidateColumnName tableInfos|>Seq.toArray
      yield! ValidateColumnDataType tableInfos|>Seq.toArray 
      yield! ValidateSpecialColumn tableInfos|>Seq.toArray
      yield! ValidatePrimaryKeyName tableInfos|>Seq.toArray
      yield! ValidateIndexName tableInfos|>Seq.toArray
      yield! ValidateForeignKeyRelationshipDesign tableInfos|>Seq.toArray
      yield! ValidateForeignKeyName tableInfos|>Seq.toArray
      yield! ValidateForeignKeyColumnNullableDesign tableInfos|>Seq.toArray
      //yield ValidateForeignKeyColumnRelationshipDesign tableInfos|>Seq.toArray  //暂时取消该验证，ADO.NET Entity Framework 4.0已经支持该设计
      yield! ValidateMainChildRelationshipDesign tableInfos|>Seq.toArray
      yield! ValidateLSHTable tableInfos|>Seq.toArray
      yield! ValidateDJLSHTable (tableInfos, databaseName)|>Seq.toArray
      yield! ValidateTableWithZZTable tableInfos|>Seq.toArray
      yield! ValidateZZTableColumn tableInfos|>Seq.toArray
      yield! ValidateJYLSHTable tableInfos |>Seq.toArray
    }
    |>Seq.toArray

