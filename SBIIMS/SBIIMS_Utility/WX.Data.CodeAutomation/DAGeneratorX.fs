namespace WX.Data.CodeAutomation

open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data
open WX.Data.Database

(*
注意不要使用PSeq.iteri 和PSeq.iter,否则会引起随机的丢失迭代的元素
*)

//设计任务，将来可能需要用配置文件的方式来组合条件，使用表信息来组合条件过于复杂，而且很难维护
//例如，
//同时生成所有层的代码, 可以保证一致性
//如果自动生成的代码文件单独编译为Dll文件，那么其实每个依赖这些Dll的组件都需要重新编译，这样还不如把自动生成的代码放到现有的各个代码层，这样也便于调试跟踪
type DAGeneratorX=
  static member GenerateCodeFile (assemblySuffix:string, databaseInstanceName:string, projectRootPath:string, sharedFlag, objectNames:string list) =
    try
      match GetDatabaseInfo databaseInstanceName with
      | Some x ->
          match 
            objectNames
            |>Seq.choose (fun a->
                match x.TableInfos|>Array.tryFind (fun b->b.TABLE_NAME=a) with
                | Some y ->
                    match new DatabaseUnitObjectInfo() with
                    | z ->z.ObjectName<-a; z.ObjectTypeID<-1uy; z.ProcessCategoryID<-1uy; Some z
                | _ ->
                    match x.ViewInfos|>Array.tryFind (fun b->b.TABLE_NAME=a) with
                    | Some y ->
                        match new DatabaseUnitObjectInfo() with
                        | z ->z.ObjectName<-a; z.ObjectTypeID<-2uy; z.ProcessCategoryID<-1uy; Some z    
                    | _ ->
                        match x.StoredProcedureInfos|>Array.tryFind (fun b->b.ROUTINE_NAME=a) with
                        | Some y ->
                            match new DatabaseUnitObjectInfo() with
                            | z ->z.ObjectName<-a; z.ObjectTypeID<-3uy; z.ProcessCategoryID<-1uy; Some z    
                        | _ ->None
                )
            |>Seq.toArray
            with
          |  y ->
              DAGeneratorX.GenerateCodeFileX (assemblySuffix, projectRootPath, sharedFlag, x, true) y
      | _ ->ObjectDumper.Write "指定的数据库信息"; [||]
    with e ->[||]

  static member GenerateCodeFile (assemblySuffix:string, databaseInstanceName:string, projectRootPath:string, sharedFlag, objectName_ProcessCategoryIDGroups:(string*byte)[])= 
    try
      match GetDatabaseInfo databaseInstanceName with
      | Some x ->
          match 
            objectName_ProcessCategoryIDGroups
            |>Seq.choose (fun (a,b) ->
                match x.TableInfos|>Array.tryFind (fun c->c.TABLE_NAME=a) with
                | Some y ->
                    match new DatabaseUnitObjectInfo() with
                    | z ->z.ObjectName<-a; z.ObjectTypeID<-1uy; z.ProcessCategoryID<-b; Some z
                | _ ->
                    match x.ViewInfos|>Array.tryFind (fun c->c.TABLE_NAME=a) with
                    | Some y ->
                        match new DatabaseUnitObjectInfo() with
                        | z ->z.ObjectName<-a; z.ObjectTypeID<-2uy; z.ProcessCategoryID<-b; Some z    
                    | _ ->
                        match x.StoredProcedureInfos|>Array.tryFind (fun c->c.ROUTINE_NAME=a) with
                        | Some y ->
                            match new DatabaseUnitObjectInfo() with
                            | z ->z.ObjectName<-a; z.ObjectTypeID<-3uy; z.ProcessCategoryID<-b; Some z    
                        | _ ->None
                )
            |>Seq.toArray
            with
          |  y ->
              DAGeneratorX.GenerateCodeFileX (assemblySuffix, projectRootPath,sharedFlag ,x, true) y
      | _ ->ObjectDumper.Write "指定的数据库信息"; [||]
    with e ->[||]

  static member GenerateCodeFile (assemblySuffix:string,databaseInstanceName:string, projectRootPath:string,sharedFlag, databaseUnitObjectInfos:DatabaseUnitObjectInfo[])= 
    try
      match GetDatabaseInfo databaseInstanceName with
      | Some x ->
          DAGeneratorX.GenerateCodeFileX (assemblySuffix, projectRootPath,sharedFlag, x, true) databaseUnitObjectInfos
      | _ ->ObjectDumper.Write "指定的数据库信息"; [||]
    with e ->[||]

  static member GenerateCodeFile (assemblySuffix:string, databaseInstanceName:string, projectRootPath:string, sharedFlag)= 
    try
      match GetDatabaseInfo databaseInstanceName with
      | Some x ->
          match
            ( 
            seq{
              yield! x.TableInfos|>Array.map (fun a->new DatabaseUnitObjectInfo(ObjectName=a.TABLE_NAME,ObjectTypeID=1uy,ProcessCategoryID=1uy))
              yield! x.ViewInfos|>Array.map (fun a->new DatabaseUnitObjectInfo(ObjectName=a.TABLE_NAME,ObjectTypeID=2uy,ProcessCategoryID=1uy))
              yield! x.StoredProcedureInfos|>Array.map (fun a->new DatabaseUnitObjectInfo(ObjectName=a.ROUTINE_NAME,ObjectTypeID=3uy,ProcessCategoryID=1uy))
            }
            |>Seq.toArray)
            with
          | y ->
              DAGeneratorX.GenerateCodeFileX (assemblySuffix, projectRootPath, sharedFlag, x, true) y
      | _ ->ObjectDumper.Write "指定的数据库信息错误"; [||]
    with e ->[||]

  static member GenerateCodeFileX (assemblySuffix:string,projectRootPath:string, sharedFlag, databaseInfo:DatabaseInfo, needValidate) (databaseUnitObjectInfos:DatabaseUnitObjectInfo[])=
    try
    match 
      databaseInfo.TableInfos
      |>Seq.filter (fun a ->a.TABLE_NAME<>"sysdiagrams")
      |>Seq.choose (fun a->
          match databaseUnitObjectInfos|>Seq.tryFind (fun b->b.ObjectTypeID=1uy && b.ObjectName=a.TABLE_NAME) with
          | Some x->a.ProcessCategoryID<-x.ProcessCategoryID; Some a
          | _ ->None
          )
      |>Seq.toArray 
      ,
      match databaseInfo.Name, assemblySuffix with
      | x, y -> 
        match x.Split([|'_'|]).[0], y.Replace('.','_') with        
        | u, v ->x,u+v  //数据实例名，EntityContext名称前缀
      with
    | x, (u,v)->
        if needValidate then
          match ValidateTablesDesignX (databaseInfo.TableInfos,u)|>Seq.filter (fun (a,_)->not a)|>Seq.toArray with //过滤！ 除了错误之外，还可以显示一些警告信息
          | y when y.Length >0 ->ObjectDumper.Write(y,1);failwith "The tables design have some problems! "
          | y ->ObjectDumper.Write(y,1)
        let tableRelatedInfosA= x|>Array.filter (fun a->a.ProcessCategoryID=1uy)|>DAGeneratorX.AttachRelatedInfo //查询且数据维护
        let tableRelatedInfosB =x|>Array.filter (fun a->a.ProcessCategoryID<>3uy)|>DAGeneratorX.AttachRelatedInfo 
        let tableRelatedInfosC =x|>DAGeneratorX.AttachRelatedInfo 
        let codeText:string ref=ref String.Empty
        let filePath:string ref=ref String.Empty
        match sharedFlag with
        | true ->
            [|
              //For DAHelper
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}Helper.{1}",v,"fs"))
              codeText:=DACodingHelperX.GetCode u tableRelatedInfosA
              File.WriteFile !codeText !filePath
              yield !filePath
            |]
        | _ ->
            [|
              //For BusinessEntities，怎么保证messageTemplate(验证失败后的信息)的后期更新？？？，验证信息更新较为i频繁，应该放到UI层，这样便于商业实体和数据库约束保持一致，
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.BusinessEntities assemblySuffix,String.Format("BD_{0}.{1}",v,"fs"))
              codeText:=BDCodingX.GetCode(tableRelatedInfosB)
              File.WriteFile !codeText !filePath
              yield !filePath

              //For IDataAccess
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.IDataAccess assemblySuffix,String.Format("IDA_{0}.{1}",v,"fs"))
              codeText:=IDACodingX.GetCode v tableRelatedInfosA
              File.WriteFile !codeText !filePath
              yield !filePath

              //For DABusinessHelper
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}Helper.{1}",v,"fs"))
              codeText:=DACodingBusinessHelperX.GetCode u v tableRelatedInfosC
              File.WriteFile !codeText !filePath
              yield !filePath

              //For DataAccess 所有类型代码放在一个文件中，
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}.{1}",v,"fs"))
              codeText:=DACodingMainPartX.GetCode u v tableRelatedInfosA x
              File.WriteFile !codeText !filePath
              yield !filePath

              //For DataAccess Module
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DAM_{0}.{1}",v,"fs"))
              codeText:=DAModuleCodingX.GetCode tableRelatedInfosB
              File.WriteFile !codeText !filePath
              yield !filePath

              //数据库字典->外键实例名
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.DatabaseDictionary assemblySuffix,String.Format("FTN_{0}.{1}",v,"fs")) //外键表实例名
              codeText:=DAFKTableInstanceDictionaryCodingX.GetCode tableRelatedInfosC x
              File.WriteFile !codeText !filePath
              yield !filePath

              //数据库字典->表名
              filePath:=Path.Combine(projectRootPath,CodeLayerPath.DatabaseDictionary assemblySuffix,String.Format("TN_{0}.{1}",v,"fs")) //表名
              codeText:=DATableNameDictionaryCodingX.GetCode tableRelatedInfosC
              File.WriteFile !codeText !filePath
              yield !filePath
            |]
    with 
    | e ->ObjectDumper.Write(e); raise e

  //由于seq{}对函数的限制，如对fun a->的限制，所有在这里使用List<>对结果集进行收集较为合理
  static member AttachRelatedInfo (tableInfos:TableInfo[])=
    try
      seq{
        let (|HasOneLevelLeafTable|_|) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) =
          match 
            tableAsPKRelationships
            |>Seq.tryFind (fun a->
                a.FK_TABLE.StartsWithIn OneLevelLeafTableFeatures &&  //OneLevelLeafTableFeaturePatterns|>Regex.IsMatchIn a.FK_TABLE && , 正确，但性能下降严重
                match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                | Some x ->                    
                    Seq.length x.TablePrimaryKeyInfos >1 && //子表有多个主键列,说明是一对多的关系
                    x.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                | _ ->false
                ) with
          | Some x ->
              match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=x.FK_TABLE) with
              | Some y ->Some y
              | _ ->None
          | _ ->None

        let (|HasOneLevelMainTable|_|) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) =
          match 
            tableAsFKRelationships
            |>Seq.tryFind (fun a->
                a.PK_TABLE.StartsWithIn OneLevelMainTableFeatures &&           
                match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.PK_TABLE)  with
                | Some x -> 
                    //Seq.length x.TablePrimaryKeyInfos=1 //该表的上一级表，不一定只有一个主键
                    x.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                | _ ->false
                ) with
          | Some x ->
              match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=x.PK_TABLE) with
              | Some y ->Some y
              | _ ->None
          | _ ->None        
        //------------------------------------------------------------------------------------------------------
        let (|HasTwoLevelChildLeafTable|_|) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) =
          match 
            tableAsPKRelationships
            |>Seq.tryFind (fun a->
                a.FK_TABLE.StartsWithIn TwoLevelChildTableFeatures && 
                match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                | Some x ->                    
                    Seq.length x.TablePrimaryKeyInfos>1 && //子表有多个主键列,说明是一对多的关系
                    x.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                | _ ->false
                ) with
          | Some x ->
              match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=x.FK_TABLE) with
              | Some y ->
                  match 
                    y.TablePrimaryKeyRelationshipInfos
                    |>Seq.tryFind (fun a->
                        a.FK_TABLE.StartsWithIn TwoLevelLeafTableFeatures &&
                        match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                        | Some z->  
                            Seq.length z.TablePrimaryKeyInfos>1 &&
                            z.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                        | _ ->false
                        ) with
                  | Some y ->
                      match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=x.FK_TABLE), tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=y.FK_TABLE) with   //子表和叶子表
                      | Some za, Some zb ->Some (za,zb)
                      | _ ->None
                  | _ ->None
              | _ ->None
          | _ ->None
        let (|HasTwoLevelMainLeafTable|_|) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[], tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[])=
          match 
            tableAsFKRelationships
            |>Seq.tryFind (fun a->
                a.PK_TABLE.StartsWithIn TwoLevelMainTableFeatures &&
                match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.PK_TABLE) with
                | Some x ->
                    //Seq.length x=1 //该表的上一级表，不一定只有一个主键
                    x.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                | _ ->false
                ) with
          | Some x ->
              match 
                tableAsPKRelationships
                |>Seq.tryFind (fun a->
                    a.FK_TABLE.StartsWithIn TwoLevelLeafTableFeatures &&  //OneLevelLeafTableFeaturePatterns|>Regex.IsMatchIn a.FK_TABLE && , 正确，但性能下降严重
                    match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.FK_TABLE) with
                    | Some y ->
                        Seq.length y.TablePrimaryKeyInfos>1 && //子表有多个主键列,说明是一对多的关系
                        y.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                    | _ ->false
                    ) with
              | Some y ->
                  match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=x.PK_TABLE), tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=y.FK_TABLE) with //主表和叶子表
                  | Some za, Some zb ->Some (za,zb)
                  | _ ->None
              | _ ->None
          | _ ->None       

        let (|HasTwoLevelMainChildTable|_|) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) =
          match 
            tableAsFKRelationships
            |>Seq.tryFind (fun a->
                a.PK_TABLE.StartsWithIn TwoLevelChildTableFeatures &&
                match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.PK_TABLE) with
                | Some x->
                    Seq.length x.TablePrimaryKeyInfos>1 && //中间表须要有多个主键
                    x.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                | _ ->false
                ) with
          | Some x ->
              match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=x.PK_TABLE) with
              | Some y -> 
                  match 
                    y.TableForeignKeyRelationshipInfos
                    |>Seq.tryFind (fun a->
                        a.PK_TABLE.StartsWithIn TwoLevelMainTableFeatures &&
                        match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=a.PK_TABLE) with
                        | Some z -> 
                            //Seq.length z.TablePrimaryKeyInfos=1 //该表的上一级表，不一定只有一个主键
                            z.TablePrimaryKeyInfos|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                        | _ ->false
                        ) with
                  | Some y ->
                      match tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=x.PK_TABLE), tableInfos|>Seq.tryFind (fun b->b.TABLE_NAME=y.PK_TABLE) with //子表和主表
                      | Some za, Some zb ->Some (za,zb)
                      | _ ->None
                  | _ ->None      
              | _ ->None
          | _ ->None      

        for tableInfo in tableInfos do
          let tableColumns=  //每循环一次都会产生一个栈吗？？？
            tableInfo.TableColumnInfos
            |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          let tablePKColumns=tableInfo.TablePrimaryKeyInfos
          let tableAsFKRelationships= tableInfo.TableForeignKeyRelationshipInfos //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
          let tableAsPKRelationships=tableInfo.TablePrimaryKeyRelationshipInfos //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系

          match tableInfo.TABLE_NAME, ((tableAsFKRelationships, tableAsPKRelationships), tableColumns, tablePKColumns)  with
          | StartsWithIn ["T_DJLSH_"] _, _ -> //单据流水号表
              yield {TableInfo=tableInfo; TableTemplateType=DJLSHTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn ["T_LSH_"] _, _ ->  //基本表流水号表
              yield {TableInfo=tableInfo; TableTemplateType=LSHTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn ["T_PCLSH"] _, _ ->  //基本表批次流水号表
              yield {TableInfo=tableInfo; TableTemplateType=PCLSHTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn ["T_JYLSH_"] _, _ ->  //基本表批次流水号表
              yield {TableInfo=tableInfo; TableTemplateType=JYLSHTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn OneLevelMainTableFeatures x, ((_, HasOneLevelLeafTable z), u, _)  ->   //一级关系的主子表
              match {TableInfo=tableInfo; TableTemplateType=MainTableWithOneLevel;LevelOneChildTableInfo=z; LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]} with
              | p ->
                  p.ColumnConditionTypes<-
                    seq{
                      match x.StartsWithIn ["T_DJ_";"T_FKD_"], x.EndsWithIn ["_ZWJQ";"_LS"]|>not, u|>Seq.exists (fun a->a.COLUMN_NAME=DJHColumnName), u|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName) with
                      | true,true,true,true -> yield HasDJLSH
                      | _ ->()
                      match 
                        match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=p.LevelOneChildTableInfo.TABLE_NAME) with
                        | Some v -> 
                            v.TableColumnInfos|>Seq.exists (fun a->a.COLUMN_NAME=PCColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith DecimalTypeName && a.IS_NULLABLE_TYPED|>not)
                        | _ ->false
                        with
                      | true -> yield HasPCInChild
                      | _ ->()
                    }
                    |>Seq.toArray
                  yield p
          | StartsWithIn OneLevelLeafTableFeatures _, ((HasOneLevelMainTable _ , _), _, _) -> //| IsMatchIn OneLevelLeafTableFeaturePatterns x, (HasOneLevelMainTable _ , _, _, _) -> //正确，但性能严重下降，一级关系的叶子表
              yield {TableInfo=tableInfo ;TableTemplateType=LeafTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]} 
          | StartsWithIn TwoLevelMainTableFeatures _, ((_, HasTwoLevelChildLeafTable (za,zb)), _, _) ->
              yield {TableInfo=tableInfo;TableTemplateType=MainTableWithTwoLevels;LevelOneChildTableInfo=za;LevelTwoChildTableInfo=zb;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}  
          | StartsWithIn TwoLevelChildTableFeatures _, ((HasTwoLevelMainLeafTable _), _, _) ->
              yield {TableInfo=tableInfo;TableTemplateType=ChildTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]} //存在MainTable->ChildTable->LeafTable
          | StartsWithIn TwoLevelLeafTableFeatures _, ((HasTwoLevelMainChildTable _, _), _, _) ->
              yield {TableInfo=tableInfo ;TableTemplateType=LeafTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]} 
          | _, ((y, _), u, v) ->  //独立表
              match  
                {TableInfo=tableInfo; TableTemplateType=IndependentTable;LevelOneChildTableInfo=Null();LevelTwoChildTableInfo=Null();ColumnConditionTypes=[||];TableRelationshipTypes=[||]} 
                with
              | p ->
                  p.ColumnConditionTypes<-
                    seq{
                      for column in u do
                        match column.COLUMN_NAME,column.DATA_TYPE with
                        | q, EndsWithIn ["decimal"] _ when q=XBHColumnName->yield HasLSH
                        | q, EndsWithIn ["guid"] _ when q=FIDColumnName->yield HasFID
                        | q, EndsWithIn ["byte"] _ when q=XHColumnName->yield HasXH
                        | q, EndsWithIn ["decimal"] _ when q=JYHColumnName && p.TableInfo.TABLE_NAME.EndsWithIn ["_ZWJQ";"_LS"]|>not ->yield HasJYH  
                        | _ ->()
                    }
                    |>Seq.toArray
                  p.TableRelationshipTypes<-
                    seq{
                      if Seq.isEmpty y|>not then yield WithForeignKeyRelatedTable
                      if tableInfos|>Seq.exists (fun b->b.TABLE_NAME="T_ZZ_"+tableInfo.TABLE_NAME.Remove(0,2)) then yield WithZZB
                      elif v|>Seq.forall (fun a->y|>Array.exists (fun b->b.FK_COLUMN_NAME=a.COLUMN_NAME)) then  //表中所有的主键又都是外键的情况, 总账表也是所有的主键又都是外键的情况，应排除
                        yield WithSameKeyParentTable
                    }
                    |>Seq.toArray
                  yield p  
      }
      |>Seq.sortBy (fun a->a.TableTemplateType) //Union Type 的Union 项具有Tag值，所以是可排序序，Tag值和Union项的顺序相关 
      |>Seq.toArray
    with e ->ObjectDumper.Write e; raise e

