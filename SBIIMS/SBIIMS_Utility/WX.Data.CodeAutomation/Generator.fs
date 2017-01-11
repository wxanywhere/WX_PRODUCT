namespace WX.Data.CodeAutomation

open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data


(*
注意不要使用PSeq.iteri 和PSeq.iter,否则会引起随机的丢失迭代的元素
*)

//设计任务，将来可能需要用配置文件的方式来组合条件，使用表信息来组合条件过于复杂，而且很难维护
//例如，
//同时生成所有层的代码, 可以保证一致性
//如果自动生成的代码文件单独编译为Dll文件，那么其实每个依赖这些Dll的组件都需要重新编译，这样还不如把自动生成的代码放到现有的各个代码层，这样也便于调试跟踪
type Generator=
  static member GenerateCodeFile (assemblySuffix:string) (databaseInstanceName:string)  (projectRootPath:string) partGroupCount (tableNames:string list) =
    try
    let sb=new StringBuilder()
    match tableNames|>Seq.filter (fun a ->a<>"sysdiagrams")|>Seq.toList with
    | tableNames->
        match DatabaseInformation.ValidateTablesDesign tableNames|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
        | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
        | x ->ObjectDumper.Write(x,1)

        let tableRelatedInfos= Generator.AttachRelatedInfo tableNames

        [
        let codeText:string ref=ref String.Empty
        //let projectRootPath= @"D:\Common Workspace\For Research\BusinessArchitecture\SBIIMS"
        let filePath:string ref=ref String.Empty

        //For BusinessDataEntities，怎么保证messageTemplate(验证失败后的信息)的后期更新？？？，验证信息更新较为i频繁，应该放到UI层，这样便于商业实体和数据库约束保持一致，
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataEntities assemblySuffix,String.Format("BD_{0}.{1}",databaseInstanceName,"fs"))
        //ObjectDumper.Write !filePath 
        codeText:=DataEntitiesCodingAdvanceWithArray.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath

        (*
        //For BusinessQueryEntitiesCS
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.QueryEntitiesCS assemblySuffix,String.Format("BD_{0}.{1}",databaseInstanceName,"cs"))
        codeText:=DataEntitiesCodingCSharp.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        (*
        //For BusinessQueryEntities
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.QueryEntities assemblySuffix,String.Format("BQ_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=QueryEntitiesCodingAdvance.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        //For BusinessQueryEntities Service side and Client side
        //如果启用该BQ的分离设计，现有的BusinessQueryEntities（前一部分代码）应该停用，因为他们使用同一个目录
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.QueryEntitiesServer assemblySuffix,String.Format("BQ_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=QueryEntitiesCodingAdvanceServerSide.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.QueryEntitiesClient assemblySuffix,String.Format("BQ_{0}Client.{1}",databaseInstanceName,"fs"))
        codeText:=QueryEntitiesCodingAdvanceClientSideNew.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath

        //For IDataAccess
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.IDataAccess assemblySuffix,String.Format("IDA_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=IDataAccessCodingAdvanceWithArray.GetCode databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        (*
        //For DataAccess 所有类型代码放在一个表中，代码文件太大时，无法正确调试
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}.{1}",databaseInstanceName,"fs"))
        //codeText:=DataAccessCodingAdvance.GetCode(tableRelatedInfos)
        codeText:=DataAccessCodingAdvanceWithoutVariableWithArray.GetCode  databaseInstanceName tableRelatedInfos //do not use with let x=
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        //For DAHelper
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}Helper.{1}",databaseInstanceName,"fs"))
        codeText:=DataAccessCodingDAHelper.GetCode  databaseInstanceName tableRelatedInfos //do not use with let x=
        File.WriteFile !codeText !filePath
        yield !filePath

        //For DataAccess,获取[命名空间部分*Seq<表名,代码>], 将来每个表的不同代码层都可能会设计为单独的组件
        //相较生成一个文件的方案，改方案性能下降近一半???
        match DataAccessCodingMainPart.GetCode  databaseInstanceName tableRelatedInfos with //do not use with let x=
        | xn,x ->
            match partGroupCount with
            | Some xc -> 
                (*方案一，先整除平分，然后把余数加入到末尾的一组中
                let partSize=Seq.length x/xc
                let lastPartSize=partSize+Seq.length x%xc //最后一组可能分的较多，不太合理
                seq{
                  for partIndex in 0..xc-1 do
                    match partIndex with
                    | xa when xa=xc-1 ->yield x|>Seq.skip (partSize*partIndex) |>Seq.take lastPartSize
                    | _ -> yield x|>Seq.skip (partIndex*partSize) |>Seq.take partSize 
                }
                *)
                (*方案二，先整除平分，然后再把余数进行二次分配，每组加1，直到分配完为止*)
                seq{
                  let basePartSize=Seq.length x/xc
                  let rePartSize=basePartSize+1 //第一次按整除分配之后，如有余数，则按加1进行补分
                  let restPartSize=Seq.length x%xc
                  for (entrySize,partIndex,partSize) in 
                    seq{
                        for partIndex in 0..restPartSize-1 do //(restPartSize-1)相当于能够多分到1的最大索引
                          yield 0,partIndex, rePartSize    //分配起始数量，分配组索引，分配数量
                        for partIndex in 0..(xc-restPartSize)-1 do
                          yield rePartSize*restPartSize, partIndex, basePartSize 
                    } do
                    yield x|>Seq.skip (entrySize+partIndex*partSize) |>Seq.take partSize  
                }
                |>Seq.toArray //必须的，否则分组时，其中的元素有可能重复
                |>Array.iteri (fun i a->  //不能使用PSeq.iteri, 否则出现不可预知的错误
                    filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}{1}{2}.{3}",databaseInstanceName,"Part",char (65+i),"fs"))
                    sb.Clear()|>ignore
                    sb.Append xn|>ignore 
                    sb.AppendLine()|>ignore
                    for (_,codeContent) in a do
                      sb.Append codeContent|>ignore
                    File.WriteFile (string sb)  !filePath
                    )
            | _ ->
                filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}.{1}",databaseInstanceName,"fs"))
                sb.Clear()|>ignore
                sb.Append xn|>ignore 
                sb.AppendLine()|>ignore
                for (_,codeContent) in x do
                  sb.Append codeContent|>ignore
                File.WriteFile (string sb) !filePath   
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DA_{0}{1}.{2}",databaseInstanceName,"XXX","fs"))
        yield !filePath

        (*
        所涉及的方法只能用特定的对象，故而应使用对象式方法扩展，即使用type...with进行方法扩展，签名文件已无必要
        namespace WX.Data.DataAccess
        //For DataAccess Module signature
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DAM_{0}.{1}",databaseInstanceName,"fsi"))
        codeText:=DataAccessModuleSignatureCodingAdvance.GetCode databaseInstanceName tableRelatedInfos 
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        //For DataAccess Module
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DAM_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=DataAccessModuleCodingAdvance.GetCode databaseInstanceName tableRelatedInfos 
        File.WriteFile !codeText !filePath
        yield !filePath

        (*
        所涉及的方法只能用特定的对象，故而应使用对象式方法扩展，即使用type...with进行方法扩展
        //For DataAccess Extension
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DataAccess assemblySuffix,String.Format("DAE_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=DataAccessExtensionCodingAdvance.GetCode databaseInstanceName tableRelatedInfos 
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        //数据库字典->外键实例名
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DatabaseDictionary assemblySuffix,String.Format("FTN_{0}.{1}",databaseInstanceName,"fs")) //外键表实例名
        //codeText:=TableFKInstanceDictionaryCoding.GetCode(tableNames)
        codeText:=TableFKInstanceDictionaryCoding.GetCode tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //数据库字典->表名
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.DatabaseDictionary assemblySuffix,String.Format("TN_{0}.{1}",databaseInstanceName,"fs")) //表名
        codeText:=TableNameDictionaryCoding.GetCode tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For BusinessLogic
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.BusinessLogic assemblySuffix,String.Format("BL_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=BusinessLogicCodingAdvance.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath
        (*
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.BusinessLogic,String.Format("BL_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=BusinessLogicCodingAdvance.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        //For ServiceContract
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.ServiceContract assemblySuffix,String.Format("IWS_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=ServiceContractCodingAdvanceWithArray.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WcfService
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfService assemblySuffix,String.Format("WS_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=WcfServiceCodingAdvance.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WcfServiceWebIISHost
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfServiceWebIISHost assemblySuffix,String.Format("WS_{0}.{1}",databaseInstanceName,"svc"))
        codeText:=WcfServiceWebIISHostCodingAdvance.GetCode  databaseInstanceName
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WcfServiceWebIISHost Web config part
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfServiceWebIISHost assemblySuffix,String.Format("WS_{0}_WebConfig.{1}",databaseInstanceName,"txt"))
        codeText:=WcfServiceWebIISHostCodingAdvance.GetConfigPartCode  databaseInstanceName
        File.WriteFile !codeText !filePath
        yield !filePath

        (*须使用指定路径，暂时停用
        //For WcfServiceWebIISHostAll
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfServiceWebIISHostAll assemblySuffix,String.Format("WS_{0}.{1}",databaseInstanceName,"svc"))
        codeText:=WcfServiceWebIISHostCodingAdvance.GetCode  databaseInstanceName
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        (*
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfService,String.Format("WS_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=WcfServiceCodingAdvance.GetCode(tableRelatedInfos)
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        (*
        //For FViewData
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.FViewData assemblySuffix,String.Format("FV_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=FViewDataCoding.GetCode(tableNames)
        File.WriteFile !codeText !filePath
        yield !filePath
        *)

        //For WcfService  Development
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfServiceDevelopment assemblySuffix,String.Format("WS_{0}.{1}",databaseInstanceName,"fs"))
        codeText:=WcfServiceDevelopmentCodingAdvance.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WX.Data.ClientChannel.FromAzure
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfClientChannelFromAzure assemblySuffix,String.Format("WS_{0}Channel.{1}",databaseInstanceName,"fs"))
        codeText:=WcfClientChannelFromAzureCoding.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WX.Data.ClientChannel.FromNative
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfClientChannelFromNative assemblySuffix,String.Format("WS_{0}Channel.{1}",databaseInstanceName,"fs"))
        codeText:=WcfClientChannelFromNativeCoding.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WX.Data.ClientChannel.FromServer
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfClientChannelFromServer assemblySuffix,String.Format("WS_{0}Channel.{1}",databaseInstanceName,"fs"))
        codeText:=WcfClientChannelFromServerCoding.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath

        //For WX.Data.ClientChannel
        filePath:=Path.Combine(projectRootPath,CodeLayerPath.WcfClientChannel assemblySuffix,String.Format("WS_{0}Channel.{1}",databaseInstanceName,"fs"))
        codeText:=WcfClientChannelCoding.GetCode  databaseInstanceName tableRelatedInfos
        File.WriteFile !codeText !filePath
        yield !filePath
        ]
    with 
    | e ->ObjectDumper.Write(e); raise e

  //由于seq{}对函数的限制，如对fun a->的限制，所有在这里使用List<>对结果集进行收集较为合理
  static member AttachRelatedInfo (tableNames:string list)=
    try
      seq{
        let (|HasOneLevelLeafTable|_|) (tableAsPKRelationships:DbFKPK list) =
          match 
            tableAsPKRelationships
            |>Seq.tryFind (fun a->
                a.FK_TABLE.StartsWithIn OneLevelLeafTableFeatures &&  //OneLevelLeafTableFeaturePatterns|>Regex.IsMatchIn a.FK_TABLE && , 正确，但性能下降严重
                match DatabaseInformation.GetPKColumns a.FK_TABLE with
                | x ->
                    Seq.length x>1 && //子表有多个主键列,说明是一对多的关系
                    x|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                ) with
          | Some x ->Some x.FK_TABLE
          | _ ->None

        let (|HasOneLevelMainTable|_|) (tableAsFKRelationships:DbFKPK list) =
          match 
            tableAsFKRelationships
            |>Seq.tryFind (fun a->
                a.PK_TABLE.StartsWithIn OneLevelMainTableFeatures &&
                match DatabaseInformation.GetPKColumns a.PK_TABLE with
                | x -> 
                    //Seq.length x=1 //该表的上一级表，不一定只有一个主键
                    x|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                ) with
          | Some x ->Some x.PK_TABLE
          | _ ->None        
        //------------------------------------------------------------------------------------------------------
        let (|HasTwoLevelChildLeafTable|_|) (tableAsPKRelationships:DbFKPK list) =
          match 
            tableAsPKRelationships
            |>Seq.tryFind (fun a->
                a.FK_TABLE.StartsWithIn TwoLevelChildTableFeatures && 
                match DatabaseInformation.GetPKColumns a.FK_TABLE with
                | x ->
                    Seq.length x>1 && //子表有多个主键列,说明是一对多的关系
                    x|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                ) with
          | Some x ->
              match 
                DatabaseInformation.GetAsPKRelationship x.FK_TABLE
                |>Seq.tryFind (fun a->
                    a.FK_TABLE.StartsWithIn TwoLevelLeafTableFeatures &&
                    match DatabaseInformation.GetPKColumns a.FK_TABLE with
                    | y ->
                        Seq.length y>1 &&
                        y|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                    ) with
              | Some y ->Some (x.FK_TABLE, y.FK_TABLE)  //子表和叶子表
              | _ ->None
          | _ ->None
        let (|HasTwoLevelMainLeafTable|_|) (tableAsFKRelationships:DbFKPK list, tableAsPKRelationships:DbFKPK list)=
          match 
            tableAsFKRelationships
            |>Seq.tryFind (fun a->
                a.PK_TABLE.StartsWithIn TwoLevelMainTableFeatures &&
                match DatabaseInformation.GetPKColumns a.PK_TABLE with
                | x -> 
                    //Seq.length x=1 //该表的上一级表，不一定只有一个主键
                    x|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                ) with
          | Some x ->
              match 
                tableAsPKRelationships
                |>Seq.tryFind (fun a->
                    a.FK_TABLE.StartsWithIn TwoLevelLeafTableFeatures &&  //OneLevelLeafTableFeaturePatterns|>Regex.IsMatchIn a.FK_TABLE && , 正确，但性能下降严重
                    match DatabaseInformation.GetPKColumns a.FK_TABLE with
                    | y ->
                        Seq.length y>1 && //子表有多个主键列,说明是一对多的关系
                        y|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)   //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                    ) with
              | Some y ->Some (x.PK_TABLE, y.FK_TABLE)  //主表和叶子表
              | _ ->None
          | _ ->None       

        let (|HasTwoLevelMainChildTable|_|) (tableAsFKRelationships:DbFKPK list) =
          match 
            tableAsFKRelationships
            |>Seq.tryFind (fun a->
                a.PK_TABLE.StartsWithIn TwoLevelChildTableFeatures &&
                match DatabaseInformation.GetPKColumns a.PK_TABLE with
                | x -> 
                    Seq.length x>1 && //中间表须要有多个主键
                    x|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                ) with
          | Some x ->
              match 
                DatabaseInformation.GetAsFKRelationship x.PK_TABLE
                |>Seq.tryFind (fun a->
                    a.PK_TABLE.StartsWithIn TwoLevelMainTableFeatures &&
                    match DatabaseInformation.GetPKColumns a.PK_TABLE with
                    | y -> 
                        //Seq.length x=1 //该表的上一级表，不一定只有一个主键
                        y|>Seq.exists (fun b->b.COLUMN_NAME= a.PK_COLUMN_NAME)   //注意这里不是a.FK_COLUMN_NAME 该表的其中一个主键,也同时作为和主表关联的外键
                    ) with
              | Some y ->Some (x.PK_TABLE, y.PK_TABLE) //子表和主表
              | _ ->None      
          | _ ->None      

        for tableName in tableNames do
          let tableColumns=  //每循环一次都会产生一个栈吗？？？
            DatabaseInformation.GetColumnSchemal4Way tableName
            |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          let tablePKColumns=DatabaseInformation.GetPKColumns tableName
          let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
          let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系

          match tableName, ((tableAsFKRelationships, tableAsPKRelationships), tableColumns, tablePKColumns)  with
          | StartsWithIn ["T_DJLSH_"] x, _ -> //单据流水号表
              yield {TableName=x; TableTemplateType=DJLSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn ["T_LSH_"] x, _ ->  //基本表流水号表
              yield {TableName=x; TableTemplateType=LSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn ["T_PCLSH"] x, _ ->  //基本表批次流水号表
              yield {TableName=x; TableTemplateType=PCLSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn ["T_JYLSH_"] x, _ ->  //基本表批次流水号表
              yield {TableName=x; TableTemplateType=JYLSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | StartsWithIn OneLevelMainTableFeatures x, ((_, HasOneLevelLeafTable z), u, _)  ->   //一级关系的主子表
              match {TableName=x; TableTemplateType=MainTableWithOneLevel;LevelOneChildTableName=z; LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} with
              | p ->
                  p.ColumnConditionTypes<-
                    seq{
                      match x.StartsWithIn ["T_DJ_";"T_FKD_"], x.EndsWithIn ["_ZWJQ";"_LS"]|>not, u|>Seq.exists (fun a->a.COLUMN_NAME=DJHColumnName), u|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName) with
                      | true,true,true,true -> yield HasDJLSH
                      | _ ->()
                      match 
                        DatabaseInformation.GetColumnSchemal4Way p.LevelOneChildTableName 
                        |>Seq.exists (fun a->a.COLUMN_NAME=PCColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith DecimalTypeName && a.IS_NULLABLE_TYPED|>not)
                        with
                      | true -> yield HasPCInChild
                      | _ ->()
                    }
                    |>Seq.toArray
                  yield p
          | StartsWithIn OneLevelLeafTableFeatures x, ((HasOneLevelMainTable _ , _), _, _) -> //| IsMatchIn OneLevelLeafTableFeaturePatterns x, (HasOneLevelMainTable _ , _, _, _) -> //正确，但性能严重下降，一级关系的叶子表
              yield {TableName=x ;TableTemplateType=LeafTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} 
          | StartsWithIn TwoLevelMainTableFeatures x, ((_, HasTwoLevelChildLeafTable (za,zb)), _, _) ->
              yield {TableName=x;TableTemplateType=MainTableWithTwoLevels;LevelOneChildTableName=za;LevelTwoChildTableName=zb;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}  
          | StartsWithIn TwoLevelChildTableFeatures x, ((HasTwoLevelMainLeafTable _), _, _) ->
              yield {TableName=x;TableTemplateType=ChildTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} //存在MainTable->ChildTable->LeafTable
          | StartsWithIn TwoLevelLeafTableFeatures x, ((HasTwoLevelMainChildTable _, _), _, _) ->
              yield {TableName=x ;TableTemplateType=LeafTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} 
          | x, ((y, _), u, v) ->  //独立表
              match  
                {TableName=x; TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} 
                with
              | p ->
                  p.ColumnConditionTypes<-
                    seq{
                      for column in u do
                        match column.COLUMN_NAME,column.DATA_TYPE with
                        | q, EndsWithIn ["decimal"] _ when q=XBHColumnName->yield HasLSH
                        | q, EndsWithIn ["guid"] _ when q=FIDColumnName->yield HasFID
                        | q, EndsWithIn ["byte"] _ when q=XHColumnName->yield HasXH
                        | q, EndsWithIn ["decimal"] _ when q=JYHColumnName && p.TableName.EndsWithIn ["_ZWJQ";"_LS"]|>not ->yield HasJYH  
                        | _ ->()
                    }
                    |>Seq.toArray
                  p.TableRelationshipTypes<-
                    seq{
                      if Seq.isEmpty y|>not then yield WithForeignKeyRelatedTable
                      if tableNames|>Seq.exists (fun b->b="T_ZZ_"+tableName.Remove(0,2)) then yield WithZZB
                      elif v|>Seq.forall (fun a->y|>List.exists (fun b->b.FK_COLUMN_NAME=a.COLUMN_NAME)) then  //表中所有的主键又都是外键的情况, 总账表也是所有的主键又都是外键的情况，应排除
                        yield WithSameKeyParentTable
                    }
                    |>Seq.toArray
                  yield p  
      }
      |>Seq.sortBy (fun a->a.TableTemplateType) //Union Type 的Union 项具有Tag值，所以是可排序序，Tag值和Union项的顺序相关 
      |>Seq.toArray
    with e ->ObjectDumper.Write e; raise e

  //============================================================================================
          (*对于SBIIMS_AC, SBIIMS_JXC, SBIIMS_Frame 正确
          let mainChildRelationshipOneLevel=
            tableAsPKRelationships 
            |>Seq.filter (fun a->
                let pkColumns=
                  a.FK_TABLE 
                  |>DatabaseInformation.GetPKColumns
                Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
                &&
                pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键

          let mainChildRelationshipTwoLevels=
            match mainChildRelationshipOneLevel|>Seq.length >0 with
            | true ->
                (mainChildRelationshipOneLevel|>Seq.head).FK_TABLE
                |>DatabaseInformation.GetAsPKRelationship
                |>Seq.filter (fun a->
                    let pkColumns=
                      a.FK_TABLE 
                      |>DatabaseInformation.GetPKColumns
                    Seq.length pkColumns >1  //子表有多个主键列,说明是一对多的关系
                    &&
                    pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME)) //子表的其中一个主键列也是父表的主键, 这里外键列名就是主表的主键
                |>Some
            | _ ->None

          let childTableName=ref String.Empty
          match tablePKColumns,tableAsFKRelationships,mainChildRelationshipOneLevel,tableColumns with
          | _ when tableName.StartsWith("T_DJLSH_") -> //单据流水号表
              yield {TableName=tableName;TableTemplateType=DJLSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | _ when tableName.StartsWith("T_LSH_") ->  //基本表流水号表
              yield {TableName=tableName;TableTemplateType=LSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | _ when tableName.StartsWith("T_PCLSH") ->  //基本表批次流水号表
              yield {TableName=tableName;TableTemplateType=PCLSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | _ when tableName.StartsWith("T_JYLSH_") ->  //基本表批次流水号表
              yield {TableName=tableName;TableTemplateType=JYLSHTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
          | x,y,u,_ when  //Childtable or LeafTable
              Seq.length x > 1 //该表有多个主键列
              && y|>Seq.exists (fun a-> 
                a.PK_TABLE.ContainsIn ["_DJ_";"_FDJ_";"_HZDJ";"_FKD_"] //(a.PK_TABLE.Contains("_DJ_") || a.FK_TABLE.Contains("_FDJ_") ||  a.FK_TABLE.Contains("_HZDJ")) //只有表名称严格的符合条件时才进行主子表的代码的处理，能改进否？？？
                &&
                a.PK_TABLE
                |>DatabaseInformation.GetPKColumns
                |>Seq.length=1 //该表的主表只有一个主键, 
                && 
                x|>Seq.exists (fun b->b.COLUMN_NAME= a.FK_COLUMN_NAME))-> //这里应该是a.PK_COLUMN_NAME??? 该表的其中一个主键,也同时作为和主表关联的外键
                  if u|>Seq.length >0 //还有子表存在
                    && (u|>Seq.head).FK_TABLE.Contains("_?_")  then //需要通过子表的名称来进一步确认
                    yield {TableName=tableName;TableTemplateType=ChildTable;LevelOneChildTableName=(Seq.head u).FK_TABLE;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} //存在MainTable->ChildTable->LeafTable
                  else 
                    yield {TableName=tableName;TableTemplateType=LeafTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} //存在MainTable->LeafTable //Right!!! yield tableName,ChildTable
          | x,y,u,_ when //Leaf Table
              Seq.length x > 1 //该表有多个主键列
              && y|>Seq.exists (fun a->
                (a.PK_TABLE.Contains("_?_") || a.FK_TABLE.Contains("_?_")) //只有表名称严格的符合条件时才进行主子表的代码的处理，能改进否？？？
                &&
                a.PK_TABLE
                |>DatabaseInformation.GetPKColumns
                |>Seq.length >1 //该表的上一级表要有多个主键, 
                && 
                x|>Seq.exists (fun b->b.COLUMN_NAME= a.FK_COLUMN_NAME))-> //这里应该是a.PK_COLUMN_NAME??? 该表的其中一个主键,也同时作为和主表关联的外键
                    yield {TableName=tableName;TableTemplateType=LeafTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]} //存在MainTable->ChildTable->LeafTable, 这里认为只存三级表，更多层次的主子表关系，认为是设计缺陷，可在验证环节提示
          | x,_,u,v when  //MainChild table, 将来可能需要将主子表条件更改为单据表条件，因为主子表有时候并不需要像单据表那样同时处理
              tableName.ContainsIn ["_DJ_";"_FDJ_";"_HZDJ";"_FKD_"]  //(tableName.Contains("_DJ_") || tableName.Contains("_FDJ_") ||  tableName.Contains("_HZDJ")) //一级子表和二级子表都需满足该条件， 只有表名称严格的符合条件时才进行主子表的代码的处理，能改进否？？？
              && Seq.length x=1 //说明该表只有一个主键
              &&  
              u|>Seq.length >0-> //有一级子表存在
              match mainChildRelationshipTwoLevels with 
              | Some(w)  when w|>Seq.length >0 -> //有二级子表存在
                  //(u|>Seq.Head).Contains("_?_") || a.FK_TABLE.Contains("_?_")) //一级子表名称条件
                  (* Backup
                  match tableName.StartsWith("T_DJ_"), v|>Seq.exists (fun a->a.COLUMN_NAME =DJHColumnName), v|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName) with
                  | true,true,true ->yield {TableName=tableName;TableTemplateType=MainTableWithTwoLevels;LevelOneChildTableName=(Seq.head u).FK_TABLE;LevelTwoChildTableName=(Seq.head w).FK_TABLE;ColumnConditionType=HasDJLSH;TableRelationshipType=WithNone}  //二级主子表
                  | _ ->yield {TableName=tableName;TableTemplateType=MainTableWithTwoLevels;LevelOneChildTableName=(Seq.head u).FK_TABLE;LevelTwoChildTableName=(Seq.head w).FK_TABLE;ColumnConditionType=HasNone;TableRelationshipType=WithNone}  //二级主子表
                  *)
                  match  
                    {TableName=tableName;TableTemplateType=MainTableWithTwoLevels;LevelOneChildTableName=(Seq.head u).FK_TABLE;LevelTwoChildTableName=(Seq.head w).FK_TABLE;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}  
                    with
                  | p ->
                      p.ColumnConditionTypes<-
                        seq{
                          //match tableName.StartsWithIn ["T_DJ_";"T_FKD_"], v|>Seq.exists (fun a->a.COLUMN_NAME =DJHColumnName), v|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName),v|>Seq.exists (fun a->a.COLUMN_NAME=ZWYColumnName)|>not with
                          match tableName.StartsWithIn ["T_DJ_";"T_FKD_"], tableName.EndsWithIn ["_ZWJQ";"_LS"]|>not, v|>Seq.exists (fun a->a.COLUMN_NAME=DJHColumnName), v|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName) with
                          | true,true,true,true -> yield HasDJLSH
                          | _ ->()
                        }
                        |>Seq.toArray
                      yield p
              | _ ->
                  (*
                  match tableName.StartsWith("T_DJ_"), v|>Seq.exists (fun a->a.COLUMN_NAME=DJHColumnName), v|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName) with
                  | true,true,true ->yield {TableName=tableName;TableTemplateType=MainTableWithOneLevel;LevelOneChildTableName=(u|>Seq.head).FK_TABLE;LevelTwoChildTableName=null;ColumnConditionType=HasDJLSH;TableRelationshipType=WithNone}  //yield tableName,MainChildTable //一级主子表, 有单据流水号
                  | _ ->yield {TableName=tableName;TableTemplateType=MainTableWithOneLevel;LevelOneChildTableName=(u|>Seq.head).FK_TABLE;LevelTwoChildTableName=null;ColumnConditionType=HasNone;TableRelationshipType=WithNone}  //yield tableName,MainChildTable //一级主子表
                  *)
                  match 
                    {TableName=tableName;TableTemplateType=MainTableWithOneLevel;LevelOneChildTableName=(u|>Seq.head).FK_TABLE;LevelTwoChildTableName=null;ColumnConditionTypes=[||];TableRelationshipTypes=[||]}
                    with
                  | p ->
                      p.ColumnConditionTypes<-
                        seq{
                          //match tableName.StartsWithIn ["T_DJ_";"T_FKD_"], v|>Seq.exists (fun a->a.COLUMN_NAME=DJHColumnName), v|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName),v|>Seq.exists (fun a->a.COLUMN_NAME=ZWYColumnName)|>not with
                          match tableName.StartsWithIn ["T_DJ_";"T_FKD_"], tableName.EndsWithIn ["_ZWJQ";"_LS"]|>not, v|>Seq.exists (fun a->a.COLUMN_NAME=DJHColumnName), v|>Seq.exists (fun a->a.COLUMN_NAME=DJLXColumnName) with
                          | true,true,true,true -> yield HasDJLSH
                          | _ ->()
                          match 
                            DatabaseInformation.GetColumnSchemal4Way p.LevelOneChildTableName 
                            |>Seq.exists (fun a->a.COLUMN_NAME=PCColumnName && a.DATA_TYPE.ToLowerInvariant().EndsWith DecimalTypeName && a.IS_NULLABLE_TYPED|>not)
                            with
                          | true -> yield HasPCInChild
                          | _ ->()
                        }
                        |>Seq.toArray
                      yield p
          | x, _, _,v -> //独立表   
              (*  
              match v|>Seq.exists (fun a->a.COLUMN_NAME=XBHColumnName),  v|>Seq.exists (fun a->a.COLUMN_NAME=FIDColumnName) with
              | true,false -> yield {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasLSH;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表, 有流水号
              | false,true -> yield {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasFID;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表, 有流水号
              | true,true -> yield {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasLSHAndFID;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表, 有流水号
              | _ -> yield {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasNone;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表
              *)
              (*It's Right
              match 
                match v|>Seq.exists (fun a->a.COLUMN_NAME=XBHColumnName),  v|>Seq.exists (fun a->a.COLUMN_NAME=FIDColumnName) with
                | true,false ->  {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasLSH;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表, 有流水号
                | false,true ->  {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasFID;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表, 有流水号
                | true,true ->  {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasLSHAndFID;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表, 有流水号
                | _ ->  {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionType=HasNone;TableRelationshipType=WithNone} //Right!!! //yield tableName,IndependentTable //独立表
                with
              | p ->
                  //总账表的主键同时也是主表的外键，并且总账表中的外键也需要在主表中存在，需要验证
                  match  tableNames|>Seq.exists (fun b->b="T_ZZ_"+tableName.Remove(0,2)) with
                  | true ->p.TableRelationshipType<- WithZZB
                  | _ ->()
                  yield p
              *)
              match  
                {TableName=tableName;TableTemplateType=IndependentTable;LevelOneChildTableName=null;LevelTwoChildTableName=null;ColumnConditionTypes=Array.zeroCreate 0;TableRelationshipTypes=Array.zeroCreate 0} 
                with
              | p ->
                  p.ColumnConditionTypes<-
                    seq{
                      for column in v do
                        match column.COLUMN_NAME,column.DATA_TYPE with
                        | q, EndsWithIn ["decimal"] _ when q=XBHColumnName->yield HasLSH
                        | q, EndsWithIn ["guid"] _ when q=FIDColumnName->yield HasFID
                        | q, EndsWithIn ["byte"] _ when q=XHColumnName->yield HasXH
                        | q, EndsWithIn ["decimal"] _ when q=JYHColumnName && p.TableName.EndsWithIn ["_ZWJQ";"_LS"]|>not ->yield HasJYH   //| q, EndsWithIn ["decimal"] _ when q=JYHColumnName && v|>Seq.exists (fun a->a.COLUMN_NAME=ZWYColumnName)|>not ->yield HasJYH
                        | _ ->()
                    }
                    |>Seq.toArray
                  p.TableRelationshipTypes<-
                    seq{
                      if Seq.isEmpty tableAsFKRelationships|>not then yield WithForeignKeyRelatedTable
                      if tableNames|>Seq.exists (fun b->b="T_ZZ_"+tableName.Remove(0,2)) then yield WithZZB
                      else 
                        match   //总账表也是所有的主键又都是外键的情况，应排除
                          DatabaseInformation.GetPKColumns tableName, 
                          DatabaseInformation.GetAsFKRelationship tableName with
                        | x,y -> 
                            if x|>Seq.forall (fun a->y|>List.exists (fun b->b.FK_COLUMN_NAME=a.COLUMN_NAME)) then  //表中所有的主键又都是外键的情况
                              yield WithSameKeyParentTable
                    }
                    |>Seq.toArray
                  yield p  
          *)


    
  