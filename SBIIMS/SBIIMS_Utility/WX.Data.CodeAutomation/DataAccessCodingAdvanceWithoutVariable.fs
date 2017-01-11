namespace WX.Data.CodeAutomation

(*
ADO.NET Entity Framework Exception Process

System.Data..::.UpdateException
Namespace:  System.Data
Assembly:  System.Data.Entity (in System.Data.Entity.dll) 
http://msdn.microsoft.com/en-us/library/system.data.updateexception.aspx

http://msdn.microsoft.com/en-us/library/bb738618.aspx
Saving Changes and Managing Concurrency (Entity Framework)
System.Data.OptimisticConcurrencyException


Adding, Modifying, and Deleting Objects (Entity Framework)
http://msdn.microsoft.com/en-us/library/bb738695.aspx


.NET Framework Data Providers (ADO.NET) Search with Exception
http://msdn.microsoft.com/en-us/library/a6cd7c08.aspx

System.InvalidOperationException: The type 'enum1' has no settable properties. at 

      | :? System.Data.Common.
      | :? System.Data.ObjectNotFoundException -> -2
      | :? System.Data.UpdateException ->
      |: ? System.Data.in
      | :? InvalidOperationException -> -2

*)

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper

//单独生成子表的代码时，只生成查询代码，因为它不能单独进行更新
type DataAccessCodingAdvanceWithoutVariable=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      DataAccessCodingAdvanceWithoutVariable.GenerateNamespaceCode
      |>string|>sb.Append|>ignore
      DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperCode databaseInstanceName tableRelatedInfos    //获取单据名名称
      |>string|>sb.Append|>ignore
      for a in tableRelatedInfos do
        match a.TableTemplateType with
        | MainTableWithOneLevel 
        | MainTableWithTwoLevels
        | IndependentTable
        | ChildTable
        | LeafTable ->
            DataAccessCodingAdvanceWithoutVariable.GenerateTypeCode  a.TableName
            |>string|>sb.Append|>ignore
        | _ -> ()
        match a.TableTemplateType with
        | MainTableWithOneLevel ->
            DataAccessCodingAdvanceWithoutVariable.GetCodeWithMainChildTableOneLevelTemplate databaseInstanceName a.TableName a.LevelOneChildTableName a
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | MainTableWithTwoLevels ->
            DataAccessCodingAdvanceWithoutVariable.GetCodeWithMainChildTableTwoLevelTemplate databaseInstanceName a.TableName a.LevelOneChildTableName a.LevelTwoChildTableName 
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | IndependentTable ->
            DataAccessCodingAdvanceWithoutVariable.GetCodeWithIndependentTable databaseInstanceName a.TableName a
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | ChildTable ->  //只能查询当前表和叶子表信息
            DataAccessCodingAdvanceWithoutVariable.GetCodeWithChildTableTemplate databaseInstanceName a.TableName
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | LeafTable -> //只能查询当前表
            DataAccessCodingAdvanceWithoutVariable.GetCodeWithLeafTableTemplate databaseInstanceName a.TableName
            |>string |>sb.Append |>ignore
            sb.AppendLine()|>ignore
        | _ ->()
        (*
        | DJLSHTable -> //单据流水号表
            () //不单独生成代码
        | LSHTable -> //基本信息流水号表
            () //不单独生成代码
        *)
      string sb
    with 
    | e ->ObjectDumper.Write(e,2); raise e

  static member private VariableNames=
    ['x';'y';'z';'u';'v';'w';'a';'b';'c']
    
    (* Dynamic
    let intTem=ref 0
    intTem:=120 //char 120=x
    [for i=0 to 20 do
       yield char !intTem
       incr intTem
       match !intTem with  // x,y,z,u,v,w,a,b,c....
       | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
       | u when u=120 ->intTem:=97 // int 'a'=97
       | _ -> ()
       ]
    *)


  static member private GetCodeWithLeafTableTemplate  (databaseInstanceName:string) (tableName:string)=
    let sb=StringBuilder()
    let tableColumns=
      DatabaseInformation.GetColumnSchemal4Way tableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
    let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
    let tableKeyColumns=DatabaseInformation.GetPKColumns tableName

    DataAccessCodingAdvanceWithoutVariable.GenerateQueryCodeForLeafTable databaseInstanceName tableName tableColumns tableAsFKRelationships tableAsPKRelationships
    |>box|>sb.Append |>ignore //QueryCode
    sb.AppendLine()|>ignore
    string sb
     
  static member private GetCodeWithChildTableTemplate (databaseInstanceName:string) (tableName:string)=
    String.Empty

  static member private GetCodeWithIndependentTable  (databaseInstanceName:string) (tableName:string)(tableRelatedInfo:TableRelatedInfo)=  //(columnConditionType:ColumnConditionType)=
    let sb=StringBuilder()
    let tableColumns=
      DatabaseInformation.GetColumnSchemal4Way tableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
    let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
    let tableKeyColumns=DatabaseInformation.GetPKColumns tableName

    match tableRelatedInfo.ColumnConditionTypes with
    | ColumnConditionTypeContains [HasFID] _ ->
        DataAccessCodingAdvanceWithoutVariable.GenerateQueryCodeForIndependentTableWithoutPaging databaseInstanceName tableName tableColumns tableAsFKRelationships tableAsPKRelationships
        |>box|>sb.Append |>ignore //QueryCode
        sb.AppendLine()|>ignore
    | _ ->
        DataAccessCodingAdvanceWithoutVariable.GenerateQueryCodeForIndependentTable databaseInstanceName tableName tableColumns tableAsFKRelationships tableAsPKRelationships
        |>box|>sb.Append |>ignore //QueryCode
        sb.AppendLine()|>ignore
    match tableRelatedInfo.ColumnConditionTypes with
    | ColumnConditionTypeContains [HasLSH] _->
        match tableRelatedInfo.TableRelationshipTypes with
        | TableRelationshipTypeContains [WithZZB] _ ->
            //总账表处理
            DataAccessCodingAdvanceWithoutVariable.GenerateSingleCreateCodeForIndependentTableWithLSHWithZZ databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DataAccessCodingAdvanceWithoutVariable.GenerateMultiCreateCodeForIndependentTableWithLSHWithZZ databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore
        | _ ->
            DataAccessCodingAdvanceWithoutVariable.GenerateSingleCreateCodeForIndependentTableWithLSH databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DataAccessCodingAdvanceWithoutVariable.GenerateMultiCreateCodeForIndependentTableWithLSH databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore
    | _ ->
        match tableRelatedInfo.TableRelationshipTypes with
        | TableRelationshipTypeContains [WithZZB] _->
            //总账表处理
            DataAccessCodingAdvanceWithoutVariable.GenerateSingleCreateCodeForIndependentTableWithZZ databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DataAccessCodingAdvanceWithoutVariable.GenerateMultiCreateCodeForIndependentTableWithZZ databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore
        | _ ->
            DataAccessCodingAdvanceWithoutVariable.GenerateSingleCreateCodeForIndependentTable databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DataAccessCodingAdvanceWithoutVariable.GenerateMultiCreateCodeForIndependentTable databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore

    DataAccessCodingAdvanceWithoutVariable.GenerateSingleUpdateCodeForIndependentTable databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //SingleUpdateCode
    sb.AppendLine()|>ignore
    DataAccessCodingAdvanceWithoutVariable.GenerateMultiUpdateCodeForIndependentTable databaseInstanceName  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //MultiUpdateCode
    sb.AppendLine()|>ignore
    match tableRelatedInfo.TableRelationshipTypes with
    | TableRelationshipTypeContains [WithZZB] _ ->
        DataAccessCodingAdvanceWithoutVariable.GenerateDeleteCodeForIndependentTableWithZZ databaseInstanceName  tableName tableColumns tableKeyColumns tableAsPKRelationships tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //Delete
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateMultiDeleteCodeForIndependentTableWithZZ databaseInstanceName  tableName tableColumns tableKeyColumns tableAsPKRelationships tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //MultiDelete
        sb.AppendLine()|>ignore
    | _ ->
        DataAccessCodingAdvanceWithoutVariable.GenerateDeleteCodeForIndependentTable databaseInstanceName  tableName tableColumns tableKeyColumns tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //Delete
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateMultiDeleteCodeForIndependentTable databaseInstanceName  tableName tableColumns tableKeyColumns tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //MultiDelete
        sb.AppendLine()|>ignore
    string sb

  static member private GetCodeWithMainChildTableTwoLevelTemplate   (databaseInstanceName:string)  (mainTableName:string) (childTableName:string) (leafChildTableName:string)=
    String.Empty


  static member private GetCodeWithMainChildTableOneLevelTemplate  (databaseInstanceName:string) (mainTableName:string) (childTableName:string) (tableRelatedInfo:TableRelatedInfo)=
    let sb=StringBuilder()
    let mainTableColumns=
      DatabaseInformation.GetColumnSchemal4Way mainTableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let mainTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship mainTableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
    let mainTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship mainTableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
    let mainTableKeyColumns=DatabaseInformation.GetPKColumns mainTableName

    let childTableName=childTableName
    let childTableColumns=
      DatabaseInformation.GetColumnSchemal4Way childTableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let childTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship childTableName
    let childTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship childTableName

    DataAccessCodingAdvanceWithoutVariable.GenerateQueryCodeForMainChildOneLevelTables databaseInstanceName mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
    |>box|>sb.Append |>ignore //QueryCode
    sb.AppendLine()|>ignore
    match tableRelatedInfo.ColumnConditionTypes with
    | ColumnConditionTypeContains [HasDJLSH] _ ->
        DataAccessCodingAdvanceWithoutVariable.GenerateSingleCreateCodeForMainChildOneLevelTablesWithDJLSH databaseInstanceName  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //SingleCreateCode
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateMultiCreateCodeForMainChildOneLevelTablesWithDJLSH databaseInstanceName  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //MultiCreateCode
        sb.AppendLine()|>ignore
    | _ ->
        DataAccessCodingAdvanceWithoutVariable.GenerateSingleCreateCodeForMainChildOneLevelTables databaseInstanceName  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //SingleCreateCode
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateMultiCreateCodeForMainChildOneLevelTables databaseInstanceName  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //MultiCreateCode
        sb.AppendLine()|>ignore

    DataAccessCodingAdvanceWithoutVariable.GenerateSingleUpdateCodeForMainChildOneLevelTables databaseInstanceName  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships  tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //SingleUpdateCode
    sb.AppendLine()|>ignore
    DataAccessCodingAdvanceWithoutVariable.GenerateMultiUpdateCodeForMainChildOneLevelTables databaseInstanceName  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships  tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //MultiUpdateCode
    sb.AppendLine()|>ignore
    DataAccessCodingAdvanceWithoutVariable.GenerateDeleteCodeForMainChildOneLevelTables databaseInstanceName  mainTableName mainTableColumns mainTableKeyColumns childTableName  tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //Delete
    sb.AppendLine()|>ignore
    DataAccessCodingAdvanceWithoutVariable.GenerateMultiDeleteCodeForMainChildOneLevelTables databaseInstanceName  mainTableName mainTableColumns mainTableKeyColumns childTableName tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //MultiDelete
    sb.AppendLine()|>ignore
    string sb
      

  static member private GenerateNamespaceCode=
    @"namespace WX.Data.DataAccess
open System
open System.Data
open System.Text.RegularExpressions
open System.Collections.Generic
open FSharp.Collections.ParallelSeq
open Microsoft.Practices.EnterpriseLibrary.Logging
open WX.Data.Helper
open WX.Data
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess"

  static member private GenerateDAHelperCode (databaseInstanceName:string)  (tableRelatedInfos:TableRelatedInfo seq)= 
    let sb=StringBuilder()
    sb.AppendFormat(@"
type  DA_{0}Helper=",databaseInstanceName)|>ignore
    match tableRelatedInfos|>PSeq.exists (fun a->a.TableName="T_DJLX") with
    | true  ->
        DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperGetDJHCode databaseInstanceName //单据号
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperGetLSHCode databaseInstanceName //流水号
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperGetDJH2MCode() //单据号2M
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperUpdateLSHCode databaseInstanceName //更新流水号
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperDJNameCode()
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
    | _ ->()
    DataAccessCodingAdvanceWithoutVariable.GenerateDAHelperTrackingInfoCode()  //BusinessEntity公共的跟踪信息
    |>string |>sb.Append |>ignore
    sb.AppendLine()|>ignore
    string sb

  static member private GenerateDAHelperDJNameCode()=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetDJName (code:byte)=
    match code with
    {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
    | {0}uy->""{1}""",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_LX
            )|>ignore
        sbTem.Append(@"
    | _ ->String.Empty")|>ignore
        sbTem.ToString().TrimStart()
        )
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperTrackingInfoCode()=
    @"
  static member GetTrackInfo (now:DateTime) (businessEntityBase:BD_EditBase)=
    new T_RZ(
       C_ID=Guid.NewGuid(),
       C_FBID=businessEntityBase.C_FBIDBase,
       C_CJRQ=now,
       C_CZY=businessEntityBase.C_CZYBase,
       C_CZYXM=businessEntityBase.C_CZYXMBase,
       C_HOST=businessEntityBase.C_HOSTBase,
       C_IP=businessEntityBase.C_IPBase)"

  static member private GenerateDAHelperGetDJHCode  (databaseInstanceName:string)=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetDJH (sb:{1}EntitiesAdvance) (c_DJLX:byte) (c_DJH:string)=
    match c_DJH with 
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)->
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->""{1}""+System.DateTime.Now.ToString(""yyyyMMdd"")+string (sb.T_DJLSH_{1}|>Seq.head).C_LSH",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->c_DJH
    | _ ->c_DJH")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperGetLSHCode  (databaseInstanceName:string)=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetLSH (sb:{1}EntitiesAdvance) (c_DJLX:byte) (c_DJH:string)=
    match c_DJH with //单据号的流水号应该每天复位为 1000..., 每一天的序号都重新开始, 复位应该由表的触发器来完成
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)->
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->(sb.T_DJLSH_{1}|>Seq.head).C_LSH",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->0M
    | _ ->0M")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperGetDJH2MCode()=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetDJH2M (c_DJLX:byte) (c_DJH:string) (c_LSH:decimal)=
    match c_DJH with //单据号的流水号应该每天复位为 1000..., 每一天的序号都重新开始, 复位应该由表的触发器来完成
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)->
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->""{1}""+System.DateTime.Now.ToString(""yyyyMMdd"")+string c_LSH",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->c_DJH                                                                                      
    | _ ->c_DJH")|>ignore
        sbTem.ToString().TrimStart()
        )
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperUpdateLSHCode  (databaseInstanceName:string)=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member UpdateLSH (sb:{1}EntitiesAdvance) (c_DJLX:byte) (c_DJH:string) (accStep:decimal)=
    match c_DJH with 
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)-> //更新条件和获取条件应该一致, 最好使用正则表达式
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->sb.T_DJLSH_{1}|>Seq.head|>fun a->a.C_LSH<-a.C_LSH+accStep",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->()
    | _ ->()")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

   /// note: internal new () as this= {{}}    //it's right in the Format(...), it's must be like that, 关于 {{}}，http://msdn.microsoft.com/zh-cn/library/txafckwd(VS.95).aspx
  static member private GenerateTypeCode (mainTableName:string)=
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
type  DA_{1}=
  inherit DA_Base
  static member public INS= DA_{1}() 
  public new () = {{inherit DA_Base()}}
  interface IDA_{1} with",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

      
  static member private GenerateQueryCodeForMainChildOneLevelTables (databaseInstanceName:string) mainTableName  mainTableColumns (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list)  childTableName childTableColumns (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Get{1}s (queryEntity:BQ_{1})=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_{2}[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        use sb=new {11}EntitiesAdvance()
        sb.{2}{9}
        |>PSeq.filter (fun a->
            {3})
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>PSeq.map (fun a->
            new BD_{2}
              ({4})
            |>fun entity->
                {5}
                entity.BD_{6}s.AddRange(
                  a.{6}{10}
                  |>PSeq.map (fun b->
                      new BD_{6}
                        ({7})
                      |>fun childEntity->
                          {8}
                          childEntity,result)
                  |>PSeq.toNetList)
                entity)
        |>Seq.toResult result
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e,""GetEntities""); BD_QueryResult<BD_{2}[]>()",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        mainTableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          mainTableColumns
          |>PSeq.filter (fun b->mainTableAsFKRelationships|>PSeq.exists (fun c->c.FK_COLUMN_NAME=b.COLUMN_NAME) |>not) do
          match a.IS_NULLABLE_TYPED with
          | false ->
              match a.COLUMN_NAME, a.DATA_TYPE.ToLowerInvariant() with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.StartsWith y
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
          | _ ->
              (* 该代码模板已停用
            match queryEntity.IsQueryableNullOfC_FID with
            | false -> 
                match a.C_FID,queryEntity.C_FID with
                | x,y when y.HasValue && x.HasValue->x.Value=y.Value
                | x,y when y.HasValue && not x.HasValue->false
                | _ ->true
            | _ ->
                match a.C_FID with
                | x when not x.HasValue ->true
                | _ -> false
            && 
            match queryEntity.IsQueryableNullOfC_BZ with
            | false -> 
                match a.C_BZ,queryEntity.C_BZ with
                | x,y when y<>null && x<>null ->x.Equals(y)
                | x,y when y<>null && x=null ->false
                | _ ->true
            | _ ->
                match a.C_BZ with
                | x when x=null ->true
                | _ -> false
            && 
              *)
              (* 正确的参考
            match queryEntity.{0} with
            | x when x<>null ->
                match a.{0} with
                | y when y<>null -y.Equals(x)
                | _ ->false
            | _ ->
                match queryEntity.IsQueryableNullOf{0} with
                | true ->
                    match a.{0} with
                    | y when y<>null ->false
                    | _ ->true
                | flase ->true
                  
            match queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,_ when x<>null ->
                match a.{0} with
                | y when y<>null -y.Equals(x)
                | _ ->false
            | _,true ->
                match a.{0} with
                | y when y<>null ->false
                | _ ->true
            | _ ->true
              *)
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.StartsWith y
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.Equals(y)
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  (*对可空处理x.Equals(y)的验证 
                  let x01=System.Nullable<decimal>(1M)
                  let x02=System.Nullable<decimal>()
                  x01.Equals(x02)
                  *)
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second,queryEntity.IsQueryableNullOf{0} with
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && z.Value>y.Value ->x.Value>=y.Value && x.Value<=z.Value
            | x,y,z,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,z,_ when y.HasValue && not x.HasValue ->false
            | x,_,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,_ when y.HasValue && not x.HasValue ->false
            | x,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore

        (* case
            //该表作为当前表的外键列全为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&  
            //该表作为当前表的外键列全不为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&  
        *)
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x.{2}>=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore

          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value && x<>null->x.{2} >=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-3,3)|>ignore //Remove the last of "&& "
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              sbTem.AppendFormat( @"
              {0}=a.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        //if sbTem.Length>0 then sbTem.Remove(sbTem.Length-1,1)
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
            entity.C_JBR<-a.T_YG1.C_ID            
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID) 
                entity.C_SHR1 <- Nullable<System.Guid>(x.C_ID1) 
            | _ ->()
        *)
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do
                match b with
                | u,_  ->
                    sbTem.AppendFormat(@"
                entity.{0}<-a.{1}.{2}",
                      //{0}
                      u.FK_COLUMN_NAME
                      ,
                      //{1}
                      u.PK_TABLE_ALIAS
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat( @"
                match a.{0} with
                | x when x<>null ->
                    {1}
                | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- Nullable<{1}>(x.{2})",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        v.DATA_TYPE
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                sbTemSub.ToString().TrimStart()
                )
                )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        childTableName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          match a.COLUMN_NAME,childTableAsFKRelationships,mainTableAsPKRelationships with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ||  z|>PSeq.exists (fun b ->b.FK_TABLE=childTableName && b.FK_COLUMN_NAME=x) -> //除了与主表关联的键外， 只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们最好是在后续的外键实体加载中进行赋值，这样便于结果集的扩展
              sbTem.AppendFormat( @"
                        {0}=b.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{8}
        (*
            entity.C_JBR<-a.T_YG1.C_ID            
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID) 
                entity.C_SHR1 <- Nullable<System.Guid>(x.C_ID1) 
            | _ ->()
        *)
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships  |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do
                match b with
                | u,_  ->
                    sbTem.AppendFormat(@"
                          childEntity.{0}<-b.{1}.{2}",
                      //{0}, 
                      u.FK_COLUMN_NAME
                      ,
                      //{1}
                      u.PK_TABLE_ALIAS
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat( @"
                          match b.{0} with
                          | x when x<>null ->
                              {1}
                          | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"
                              childEntity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                              childEntity.{0} <- Nullable<{1}>(x.{2})",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        v.DATA_TYPE
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                sbTemSub.ToString().TrimStart()
                )
                )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        mainTableAsFKRelationships
        |>PSeq.map (fun a->a. PK_TABLE_ALIAS)
        |>PSeq.distinct //一个主键可能由两个或两个以上键列构成
        |>Seq.iter (fun a->
            sbTem.AppendFormat(@".Include(FTN.{0})",
              //{0}
              a
              )|>ignore)
        sbTem.ToString()
        )
        ,
        //{10}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        match childTableAsFKRelationships  |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName) with
        | x when PSeq.length x>0 ->
            sbTem.Append(".CreateSourceQuery()")|>ignore
            x
            |>PSeq.map (fun a->a. PK_TABLE_ALIAS)
            |>PSeq.distinct //一个主键可能由两个或两个以上键列构成
            |>Seq.iter (fun a->
                sbTem.AppendFormat(@".Include(FTN.{0})",
                  //{0}
                  a
                  )|>ignore)
        | x ->()
        sbTem.ToString()
        )
        ,
        //{11}
        databaseInstanceName
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    
    
///////////////////////////////////////////

(*子模板空格不正确
  static member private GenerateSingleCreateCodeForMainChildOneLevelTables (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    @"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            for childEntity in businessEntity.BD_{6}s do
              new {6}
                ({8})
              |>fun {7} ->
                  {9}
                  {3}.{6}.Add({7})
            sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
    |>DataAccessCodingAdvanceWithoutVariable.GenerateCreateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 

  static member private GenerateMultiCreateCodeForMainChildOneLevelTables (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    @"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              for childEntity in businessEntity.BD_{6}s do
                new {6}
                  ({8})
                |>fun {7} ->
                    {9}
                    {3}.{6}.Add({7})
              sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
    |>DataAccessCodingAdvanceWithoutVariable.GenerateCreateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
*)

  static member private GenerateSingleCreateCodeForMainChildOneLevelTables (databaseInstanceName:string) (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {10}EntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            for childEntity in businessEntity.BD_{6}s do
              new {6}
                ({8})
              |>fun {7} ->
                  {9}
                  {3}.{6}.Add({7})
            sb.{2}.AddObject({3})
        businessEntity
        |>DA_{10}Helper.GetTrackInfo now
        |>fun a->
            {11}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -6; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            {0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                {0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=childEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=childEntity.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0) with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      {0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        databaseInstanceName
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_ZBM<- {9}
            a.C_ZBBM<-{10}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}
            a.C_ZBSL<-new Nullable<int>({7})
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          "3uy"
          ,
          //{4}
          @""""+"父子表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          @"""新增"+ getTableDescription mainTableName + @"父子记录"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  //Adding, Modifying, and Deleting Objects (Entity Framework)
  // http://msdn.microsoft.com/en-us/library/bb738695.aspx
  static member private GenerateSingleCreateCodeForMainChildOneLevelTablesWithDJLSH (databaseInstanceName:string) (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {10}EntitiesAdvance()
        businessEntity|>fun a->a.C_DJH<-DA_{10}Helper.GetDJH sb a.C_DJLX a.C_DJH
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            for childEntity in businessEntity.BD_{6}s do
              new {6}
                ({8})
              |>fun {7} ->
                  {9}
                  {3}.{6}.Add({7})
            sb.{2}.AddObject({3})
        businessEntity
        |>fun a->
            DA_{10}Helper.UpdateLSH sb a.C_DJLX a.C_DJH 1M
            a
        |>DA_{10}Helper.GetTrackInfo now
        |>fun a->
            {11}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -6; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              (* 已置前
              | z when z.ToLowerInvariant().EndsWith("string") && x.Equals(DJHColumnName) ->
                  sbTem.AppendFormat( @"
          {0}=LSHHelper.GetDJH businessEntity.C_DJLX businessEntity.{0},",
                    x
                    )|>ignore
              *)
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            {0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                {0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=childEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=childEntity.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0) with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      {0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        databaseInstanceName
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_ZBM<- {9}
            a.C_ZBBM<-{10}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}
            a.C_ZBSL<-new Nullable<int>({7})
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            (*
            match b.DATA_TYPE.ToLowerInvariant() with
            | EndsWithIn NullableTypeConditions _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            | _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}.Value+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            *)
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          "2uy"
          ,
          //{4}
          @""""+"单据处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          @"""新增"+ getTableDescription mainTableName + @"的""+(businessEntity.C_DJLX|>DA_"+databaseInstanceName+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(a.C_ZBSL.Value|>string)+""种商品"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e


  static member private GenerateMultiCreateCodeForMainChildOneLevelTables (databaseInstanceName:string) (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntities=executeContent.ExecuteData
        use sb=new {10}EntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              for childEntity in businessEntity.BD_{6}s do
                new {6}
                  ({8})
                |>fun {7} ->
                    {9}
                    {3}.{6}.Add({7})
              sb.{2}.AddObject({3})
          executeContent.TrackingInfo
          |>DA_{10}Helper.GetTrackInfo now
          |>fun a->
              {11}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -7; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
            {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
              {0}.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->()",
                    //{0}
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
                  {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                  {0}=childEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                    {0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=childEntity.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                    match {0) with
                    | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                        {0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>
                    | _ ->()",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        databaseInstanceName
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_ZBM<- {9}
              a.C_ZBBM<-{10}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}
              a.C_ZBSL<-new Nullable<int>({7})
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          "3uy"
          ,
          //{4}
          @""""+"父子表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          @"""新增"+ getTableDescription mainTableName + @"父子记录"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e


  static member private GenerateMultiCreateCodeForMainChildOneLevelTablesWithDJLSH (databaseInstanceName:string) (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new {10}EntitiesAdvance()
        let c_LSH=ref (match businessEntities|>PSeq.head with y-> DA_{10}Helper.GetLSH sb y.C_DJLX y.C_DJH)
        for businessEntity in businessEntities do
          businessEntity
          |>fun a->
              a.C_DJH<-DA_{10}Helper.GetDJH2M a.C_DJLX a.C_DJH !c_LSH
              c_LSH:=!c_LSH+1M
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              for childEntity in businessEntity.BD_{6}s do
                new {6}
                  ({8})
                |>fun {7} ->
                    {9}
                    {3}.{6}.Add({7})
              sb.{2}.AddObject({3})
          executeContent.TrackingInfo
          |>DA_{10}Helper.GetTrackInfo now
          |>fun a->
              {11}
              a
          |>sb.T_RZ.AddObject
        businessEntities
        |>fun a->
            DA_{10}Helper.UpdateLSH sb (a|>PSeq.head).C_DJLX (a|>PSeq.head).C_DJH (a.Length|>decimal)
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -7; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
            {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
              {0}.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->()",
                    //{0}
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
                  {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                  {0}=childEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                    {0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=childEntity.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                    match {0) with
                    | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                        {0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>
                    | _ ->()",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        databaseInstanceName
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_ZBM<- {9}
              a.C_ZBBM<-{10}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}
              a.C_ZBSL<-new Nullable<int>({7})
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          "2uy"
          ,
          //{4}
          @""""+"单据处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          @"""新增"+ getTableDescription mainTableName + @"的""+(businessEntity.C_DJLX|>DA_"+databaseInstanceName+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(a.C_ZBSL.Value|>string)+""种商品"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

(* 子模板空格问题
  static member private GenerateSingleUpdateCodeForMainChildOneLevelTables (mainTableName:string)   (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    @"{3}
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}    
            if businessEntity.BD_{6}s.Count>0 then
              original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
              businessEntity.BD_{6}s|>Seq.iter (fun a->
                new {6}
                  ({8})
                |>fun {7} -> 
                    {9}
                    original.{6}.Add({7}))
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DataAccessCodingAdvanceWithoutVariable.GenerateUpdateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
    
  static member private GenerateMultiUpdateCodeForMainChildOneLevelTables (mainTableName:string)   (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    @"{3}
    member x.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do 
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}    
              if businessEntity.BD_{6}s.Count>0 then
                original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
                businessEntity.BD_{6}s|>Seq.iter (fun a->
                  new {6}
                    ({8})
                  |>fun {7} ->
                      {9}
                      original.{6}.Add({7}))
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DataAccessCodingAdvanceWithoutVariable.GenerateUpdateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 

*)

  //Update for Main Child Tables
  //子表更新最好还是加入跟踪信息，这样在单据子项很多的情况下，可根据子项不同记录的跟踪信息，进行插入，更新或是删除操作
  //加入跟踪后，客户端交叉操作可能导致子表跟踪信息有误？？？
  static member private GenerateSingleUpdateCodeForMainChildOneLevelTables (databaseInstanceName:string) (mainTableName:string)   (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)  (columnConditionTypes:ColumnConditionType seq)= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {10}EntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}    
            if businessEntity.BD_{6}s.Count>0 then
              original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
              businessEntity.BD_{6}s|>Seq.iter (fun a->
                new {6}
                  ({8})
                |>fun {7} -> 
                    {9}
                    original.{6}.Add({7}))
        businessEntity
        |>DA_{10}Helper.GetTrackInfo now
        |>fun a->
            {11}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""UpdateEntity""); result.ResultLength<- -15; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        String.Empty
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | z when z.ToLowerInvariant().EndsWith("datetime") &&  x.Equals("C_GXRQ") ->
                  sbTem.AppendFormat( @"
            original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
            original.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                original.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->
                original.{1}Reference.Load() 
                original.{1}<-null",
                    //{0}
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) -> //???,如果子表确实需要使用 创建时间和更新时间字段，那么子表记录就不能简单的先删除再新增了
                  sbTem.AppendFormat( @"
                  {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                  {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                    {0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_)->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}
                        a.PK_COLUMN_NAME
                        ,
                        //{1}
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                    match {0) with
                    | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}
                    a.FK_COLUMN_NAME
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string  DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                        {0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>
                    | _ ->
                        {0}.{1}Reference.Load() 
                        {0}.{1}<-null",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b ->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        databaseInstanceName
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_ZBM<- {9}
            a.C_ZBBM<-{10}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}
            a.C_ZBSL<-new Nullable<int>({7})
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""更新"+ getTableDescription mainTableName + @"的""+(businessEntity.C_DJLX|>DA_"+databaseInstanceName+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(a.C_ZBSL.Value|>string)+""种商品"""
          | _ ->
              @"""更新"+ getTableDescription mainTableName + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    

  static member private GenerateMultiUpdateCodeForMainChildOneLevelTables (databaseInstanceName:string)  (mainTableName:string)   (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)  (columnConditionTypes:ColumnConditionType seq)= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member x.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new {10}EntitiesAdvance()
        for businessEntity in businessEntities do 
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}    
              if businessEntity.BD_{6}s.Count>0 then
                original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
                businessEntity.BD_{6}s|>Seq.iter (fun a->
                  new {6}
                    ({8})
                  |>fun {7} ->
                      {9}
                      original.{6}.Add({7}))
          executeContent.TrackingInfo
          |>DA_{10}Helper.GetTrackInfo now
          |>fun a->
              {11}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""UpdateEntities""); result.ResultLength<- -15; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        String.Empty
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | z when z.ToLowerInvariant().EndsWith("datetime") &&  x.Equals("C_GXRQ") ->
                  sbTem.AppendFormat( @"
              original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
              original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
              original.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                  original.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->
                  original.{1}Reference.Load() 
                  original.{1}<-null",
                    //{0}
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) -> //???,如果子表确实需要使用 创建时间和更新时间字段，那么子表记录就不能简单的先删除再新增了
                  sbTem.AppendFormat( @"
                    {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                    {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      {0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_)->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}
                        a.PK_COLUMN_NAME
                        ,
                        //{1}
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0) with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}
                    a.FK_COLUMN_NAME
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string  DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                          {0}.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          {0}.{1}Reference.Load() 
                          {0}.{1}<-null",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b ->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        databaseInstanceName
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_ZBM<- {9}
              a.C_ZBBM<-{10}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}
              a.C_ZBSL<-new Nullable<int>({7})
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""更新"+ getTableDescription mainTableName + @"的""+(businessEntity.C_DJLX|>DA_"+databaseInstanceName+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(a.C_ZBSL.Value|>string)+""种商品"""
          | _ ->
              @"""更新"+ getTableDescription mainTableName + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

(*
let b=ref Unchecked.defaultof<_>
sb.TryGetObjectByKey(a,b), !b with
可以写成, 
let (flag,result)=sb.TryGetObjectByKey(a) //参考变量可以不需要作为参数, 而是可以在结果元数据中接收 
同样的 let (successX, x) = Double.TryParse("100")
Exception of Adding, Modifying, and Deleting Objects (Entity Framework)
http://msdn.microsoft.com/en-us/library/bb738695.aspx
*)
  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member  GenerateDeleteCodeForMainChildOneLevelTables (databaseInstanceName:string) (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq)  (mainTableKeyColumns:DbPKColumn seq)   (childTableName:string)  (columnConditionTypes:ColumnConditionType seq)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntity=executeContent.ExecuteData
        use sb=new {5}EntitiesAdvance()
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->
            x
            |>unbox<{2}>
            |>fun x -> 
                x.{4}|>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
                x
            |>sb.DeleteObject
        businessEntity
        |>DA_{5}Helper.GetTrackInfo now
        |>fun a->
            {6}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? UpdateException as e -> ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntity""); result.ResultLength<- -16; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntity""); result.ResultLength<- -20; result",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        mainTableName
        ,
        //{3}
        (* 商业实体的实体键必然有值
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          (mainTableKeyColumns,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          do
          match a with
          | x,y when y.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || y.DATA_TYPE.EndsWith("[]")->
              sbTem.AppendFormat(@"{0}=businessEntity.{0},",
                //{0}
                x.COLUMN_NAME
                )|>ignore
          | x,_->
              sbTem.AppendFormat(@"{0}=businessEntity.{0}.Value,",
                //{0}
                x.COLUMN_NAME
                )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        *)
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  mainTableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        childTableName
        ,
        //{5}
        databaseInstanceName
        ,
        //{6}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_ZBM<- {9}
            a.C_ZBBM<-{10}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}
            a.C_ZBSL<-new Nullable<int>({7})
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "3uy"
          ,
          //{6}
          @"""删除"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""删除"+ getTableDescription mainTableName + @"的""+(businessEntity.C_DJLX|>DA_"+databaseInstanceName+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(a.C_ZBSL.Value|>string)+""种商品"""
          | _ ->
              @"""删除"+ getTableDescription mainTableName + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    
(*
let b=ref Unchecked.defaultof<_>
sb.TryGetObjectByKey(a,b), !b with
可以写成, 
let (flag,result)=sb.TryGetObjectByKey(a) //参考变量可以不需要作为参数, 而是可以在结果元数据中接收 
同样的 let (successX, x) = Double.TryParse("100")
*)
  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member  GenerateMultiDeleteCodeForMainChildOneLevelTables (databaseInstanceName:string) (mainTableName:string)   (mainTableColumns:DbColumnSchemalR seq)    (mainTableKeyColumns:DbPKColumn seq)   (childTableName:string)  (columnConditionTypes:ColumnConditionType seq)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntities=executeContent.ExecuteData
        use sb=new {5}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _,x ->
              x
              |>unbox<{2}>
              |>fun x -> 
                  x.{4} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
                  x
              |>sb.DeleteObject
          executeContent.TrackingInfo
          |>DA_{5}Helper.GetTrackInfo  now
          |>fun a->
              {6}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? UpdateException as e -> ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntities""); result.ResultLength<- -17; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntities""); result.ResultLength<- -20; result",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        mainTableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  mainTableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        childTableName
        ,
        //{5}
        databaseInstanceName
        ,
        //{6}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_ZBM<- {9}
              a.C_ZBBM<-{10}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}
              a.C_ZBSL<-new Nullable<int>({7})
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription mainTableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "3uy"
          ,
          //{6}
          @"""删除"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Count"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""删除"+ getTableDescription mainTableName + @"的""+(businessEntity.C_DJLX|>DA_"+databaseInstanceName+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(a.C_ZBSL.Value|>string)+""种商品"""
          | _ ->
              @"""删除"+ getTableDescription mainTableName + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription childTableName + @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
 

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  static member private GenerateQueryCodeForIndependentTable (databaseInstanceName:string) tableName tableColumns tableAsFKRelationships tableAsPKRelationships=
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
    member x.Get{1}s (queryEntity:BQ_{1})=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_{2}[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        use sb=new {6}EntitiesAdvance()
        sb.{2}{0}
        |>PSeq.filter (fun a->
            {3})
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>PSeq.map (fun a->
            new BD_{2}
              ({4})
            |>fun entity ->
                {5}
                entity)
        |>Seq.toResult result
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e,""GetEntities""); BD_QueryResult<BD_{2}[]>()",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        tableAsFKRelationships
        |>PSeq.map (fun a->a. PK_TABLE_ALIAS)
        |>PSeq.distinct //一个主键可能由两个或两个以上键列构成
        |>Seq.iter (fun a->    //使用PSeq.Iter时，有时会出错？？？
            sbTem.AppendFormat(@".Include(FTN.{0})",
              //{0}
              a
              )|>ignore)
        sbTem.ToString()
        )
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          tableColumns
          |>PSeq.filter (fun b->tableAsFKRelationships|>PSeq.exists (fun c->c.FK_COLUMN_NAME=b.COLUMN_NAME) |>not) do
          match a.IS_NULLABLE_TYPED with
          | false ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.StartsWith y
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
          | _ ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.StartsWith y
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.Equals(y)
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second,queryEntity.IsQueryableNullOf{0} with
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && z.Value>y.Value ->x.Value>=y.Value && x.Value<=z.Value
            | x,y,z,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,z,_ when y.HasValue && not x.HasValue ->false
            | x,_,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,_ when y.HasValue && not x.HasValue ->false
            | x,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore

        (* case
            //该表作为当前表的外键列全为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&  
            //该表作为当前表的外键列全不为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&  
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x.{2}>=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore

          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value && x<>null->x.{2} >=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-3,3)|>ignore //Remove the last of "&& "
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              sbTem.AppendFormat( @"
              {0}=a.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        //if sbTem.Length>0 then sbTem.Remove(sbTem.Length-1,1)
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
            entity.C_JBR<-a.T_YG1.C_ID            
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID) 
                entity.C_SHR1 <- Nullable<System.Guid>(x.C_ID1) 
            | _ ->()
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do
                match b with
                | u,_  ->
                    sbTem.AppendFormat(@"
                entity.{0}<-a.{1}.{2}",
                      //{0}
                      u.FK_COLUMN_NAME
                      ,
                      //{1}
                      u.PK_TABLE_ALIAS
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat( @"
                match a.{0} with
                | x when x<>null ->
                    {1}
                | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- Nullable<{1}>(x.{2})",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        v.DATA_TYPE
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                sbTemSub.ToString().TrimStart()
                )
                )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e


  static member private GenerateQueryCodeForIndependentTableWithoutPaging (databaseInstanceName:string) tableName tableColumns tableAsFKRelationships tableAsPKRelationships=
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
    member x.Get{1}s (queryEntity:BQ_{1})=
      let result=new BD_QueryResult<BD_{2}[]>(ExecuteDateTime=DateTime.Now)
      try
        use sb=new {6}EntitiesAdvance()
        sb.{2}{0}
        |>PSeq.filter (fun a->
            {3})
        |>PSeq.map (fun a->
            new BD_{2}
              ({4})
            |>fun entity ->
                {5}
                entity)
        |>Seq.toResult result
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e,""GetEntities""); BD_QueryResult<BD_{2}[]>()",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        tableAsFKRelationships
        |>PSeq.map (fun a->a. PK_TABLE_ALIAS)
        |>PSeq.distinct //一个主键可能由两个或两个以上键列构成
        |>Seq.iter (fun a->   //使用PSeq.Iter时，有时会出错？？？
            sbTem.AppendFormat(@".Include(FTN.{0})",
              //{0}
              a
              )|>ignore)
        sbTem.ToString()
        )
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          tableColumns
          |>PSeq.filter (fun b->tableAsFKRelationships|>PSeq.exists (fun c->c.FK_COLUMN_NAME=b.COLUMN_NAME) |>not) do
          match a.IS_NULLABLE_TYPED with
          | false ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.StartsWith y
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
          | _ ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.StartsWith y
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.Equals(y)
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second,queryEntity.IsQueryableNullOf{0} with
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && z.Value>y.Value ->x.Value>=y.Value && x.Value<=z.Value
            | x,y,z,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,z,_ when y.HasValue && not x.HasValue ->false
            | x,_,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,_ when y.HasValue && not x.HasValue ->false
            | x,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore

        (* case
            //该表作为当前表的外键列全为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&  
            //该表作为当前表的外键列全不为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&  
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x.{2}>=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore

          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value && x<>null->x.{2} >=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-3,3)|>ignore //Remove the last of "&& "
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              sbTem.AppendFormat( @"
              {0}=a.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        //if sbTem.Length>0 then sbTem.Remove(sbTem.Length-1,1)
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
            entity.C_JBR<-a.T_YG1.C_ID            
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID) 
                entity.C_SHR1 <- Nullable<System.Guid>(x.C_ID1) 
            | _ ->()
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do
                match b with
                | u,_  ->
                    sbTem.AppendFormat(@"
                entity.{0}<-a.{1}.{2}",
                      //{0}
                      u.FK_COLUMN_NAME
                      ,
                      //{1}
                      u.PK_TABLE_ALIAS
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat( @"
                match a.{0} with
                | x when x<>null ->
                    {1}
                | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- Nullable<{1}>(x.{2})",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        v.DATA_TYPE
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                sbTemSub.ToString().TrimStart()
                )
                )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

(*
match 
  (""{2}"",new {2}({0}))
  |>sb.CreateEntityKey 
  |>fun a ->sb.TryGetObjectByKey(a,ref Unchecked.defaultof<_>) with
| true -> failwith ""The record is exist！"" | _ ->()
可改写成
match 
  (""{2}"",new {2}({0}))
  |>sb.CreateEntityKey 
  |>sb.TryGetObjectByKey with
| true, _ -> failwith ""The record is exist！"" | _ ->()

*)

(*只模板空格问题 
  static member private GenerateSingleCreateCodeForIndependentTable (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq) =
    @"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
  |>DataAccessCodingAdvanceWithoutVariable.GenerateCreateCodeForIndependentTable tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns

  static member private GenerateMultiCreateCodeForIndependentTable (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq) =
    @"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
  |>DataAccessCodingAdvanceWithoutVariable.GenerateCreateCodeForIndependentTable tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns

*)

  static member private GenerateSingleCreateCodeForIndependentTable (databaseInstanceName:string)  (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            sb.{2}.AddObject({3})
        businessEntity
        |>DA_{6}Helper.GetTrackInfo now
        |>fun a->
            {7}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -6; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            {0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                {0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"记录"""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateSingleCreateCodeForIndependentTableWithZZ (databaseInstanceName:string)  (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            sb.{2}.AddObject({3})
        {8}
        businessEntity
        |>DA_{6}Helper.GetTrackInfo now
        |>fun a->
            {7}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -6; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            {0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                {0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName+","+getTableDescription zzTableName+ @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"记录，同时新增总帐表"+getTableDescription zzTableName+ @""""

          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          DatabaseInformation.GetColumnSchemal4Way zzTableName
          |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        new {0}({2})
        |>fun {1} ->
            {3}    
            sb.{0}.AddObject({1})"
          ,
          //{0},
          zzTableName
          ,
          //{1} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{2}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{3}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when DatabaseInformation.GetPKColumns zzTableName|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not->  //不是主键列
                sbTemSub.AppendFormat(@"
            {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if DatabaseInformation.GetColumnSchemal4Way tableName|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match 
                      DatabaseInformation.GetAsFKRelationship zzTableName
                      |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                    | Some x  when x.PK_TABLE=tableName ->x.PK_COLUMN_NAME
                    | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
            {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e


  static member private GenerateSingleCreateCodeForIndependentTableWithLSH (databaseInstanceName:string)  (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        sb.T_LSH_{1}|>PSeq.head
        |>fun entityLSH->
            businessEntity.C_XBH<- entityLSH.C_LSH
            match 
              (""{2}"",new {2}({0}))
              |>sb.CreateEntityKey 
              |>sb.TryGetObjectByKey with
            | true, _ -> failwith ""The record is exist！"" | _ ->()
            new {2}
              ({4})
            |>fun {3} ->
                {5}    
                sb.{2}.AddObject({3})
            entityLSH.C_LSH<-entityLSH.C_LSH+1M
        businessEntity
        |>DA_{6}Helper.GetTrackInfo now
        |>fun a->
            {7}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -6; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
              {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
              {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                {0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                match {0} with
                | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                    {0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>
                | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateSingleCreateCodeForIndependentTableWithLSHWithZZ (databaseInstanceName:string)  (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        sb.T_LSH_{1}|>PSeq.head
        |>fun entityLSH->
            businessEntity.C_XBH<- entityLSH.C_LSH
            match 
              (""{2}"",new {2}({0}))
              |>sb.CreateEntityKey 
              |>sb.TryGetObjectByKey with
            | true, _ -> failwith ""The record is exist！"" | _ ->()
            new {2}
              ({4})
            |>fun {3} ->
                {5}    
                sb.{2}.AddObject({3})
            entityLSH.C_LSH<-entityLSH.C_LSH+1M
        {8}
        businessEntity
        |>DA_{6}Helper.GetTrackInfo now
        |>fun a->
            {7}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -6; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntity""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
              {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
              {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                {0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                match {0} with
                | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                    {0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>
                | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName+","+getTableDescription zzTableName+ @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时新增总帐表"+getTableDescription zzTableName+ @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          DatabaseInformation.GetColumnSchemal4Way zzTableName
          |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        //let zzTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship zzTableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
        //let zzTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship zzTableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
        //let zzTableKeyColumns=DatabaseInformation.GetPKColumns zzTableName
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        new {0}({2})
        |>fun {1} ->
            {3}    
            sb.{0}.AddObject({1})"
          ,
          //{0},
          zzTableName
          ,
          //{1} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{2}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          (*正确，但限制较多，必须保证主表和总账表仅有1一个主键字段
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          *)
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{3}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when DatabaseInformation.GetPKColumns zzTableName|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not->  //不是主键列
                sbTemSub.AppendFormat(@"
            {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if DatabaseInformation.GetColumnSchemal4Way tableName|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match 
                      DatabaseInformation.GetAsFKRelationship zzTableName
                      |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                    | Some x  when x.PK_TABLE=tableName ->x.PK_COLUMN_NAME
                    | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
            {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          (*正确，但出现了具体的表字段
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _ when x="C_FBID"->
                sbTemSub.AppendFormat(@"
            {0}.{1}<-businessEntity.{1}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1} 该总账表的主表是T_DWBM时需要做额外处理
                  if DatabaseInformation.GetColumnSchemal4Way tableName
                    |>PSeq.exists (fun b->b.COLUMN_NAME="C_FBID")
                    |>not then
                    match 
                      DatabaseInformation.GetAsFKRelationship zzTableName
                      |>PSeq.find (fun b->b.FK_COLUMN_NAME="C_FBID") with //已经在验证环节验证，所以可以不用PSeq.tryFind
                    | x ->x.PK_COLUMN_NAME
                  else x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ when x="C_CJRQ" || x="C_GXRQ"->
                sbTemSub.AppendFormat(@"
            {0}.{1}<-businessEntity.{1}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          *)
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateMultiCreateCodeForIndependentTable (databaseInstanceName:string)   (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              sb.{2}.AddObject({3})
          executeContent.TrackingInfo
          |>DA_{6}Helper.GetTrackInfo now
          |>fun a->
              {7}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -7; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
            {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
              {0}.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}{7}
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"记录"""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateMultiCreateCodeForIndependentTableWithZZ (databaseInstanceName:string)   (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              sb.{2}.AddObject({3})
          {8}
          executeContent.TrackingInfo
          |>DA_{6}Helper.GetTrackInfo now
          |>fun a->
              {7}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -7; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
            {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
              {0}.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}{7}
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName+","+getTableDescription zzTableName+ @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"记录，同时新增总帐表"+getTableDescription zzTableName+ @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          DatabaseInformation.GetColumnSchemal4Way zzTableName
          |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          new {0}({2})
          |>fun {1} ->
              {3}    
              sb.{0}.AddObject({1})"
          ,
          //{0},
          zzTableName
          ,
          //{1} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{2}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{3}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when DatabaseInformation.GetPKColumns zzTableName|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not->  //不是主键列
                sbTemSub.AppendFormat(@"
              {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if DatabaseInformation.GetColumnSchemal4Way tableName|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match 
                      DatabaseInformation.GetAsFKRelationship zzTableName
                      |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                    | Some x  when x.PK_TABLE=tableName ->x.PK_COLUMN_NAME
                    | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
              {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e


  static member private GenerateMultiCreateCodeForIndependentTableWithLSH (databaseInstanceName:string)   (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        sb.T_LSH_{1}|>PSeq.head
        |>fun entityLSH->
            for businessEntity in businessEntities do
              businessEntity.C_XBH<- entityLSH.C_LSH
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              new {2}
                ({4})
              |>fun {3} ->
                  {5}    
                  sb.{2}.AddObject({3})
              entityLSH.C_LSH<-entityLSH.C_LSH+1M
              executeContent.TrackingInfo
              |>DA_{6}Helper.GetTrackInfo now
              |>fun a->
                  {7}
                  a
              |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -7; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
                {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0} with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                      {0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                  a.C_CJRQ<-now
                  a.C_JLID<- {0}
                  a.C_BM<- {1}
                  a.C_BBM<-{2}
                  a.C_YWLX<-{3}
                  a.C_YWLXMC<-{4}
                  a.C_CZLX<-{5}
                  a.C_CZLXMC<-{6}{7}
                  a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e


  static member private GenerateMultiCreateCodeForIndependentTableWithLSHWithZZ (databaseInstanceName:string)   (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member x.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntities=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        sb.T_LSH_{1}|>PSeq.head
        |>fun entityLSH->
            for businessEntity in businessEntities do
              businessEntity.C_XBH<- entityLSH.C_LSH
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              new {2}
                ({4})
              |>fun {3} ->
                  {5}    
                  sb.{2}.AddObject({3})
              entityLSH.C_LSH<-entityLSH.C_LSH+1M
              {8}
              executeContent.TrackingInfo
              |>DA_{6}Helper.GetTrackInfo  now
              |>fun a->
                  {7}
                  a
              |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? InvalidOperationException as e->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -7; result 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""CreateEntities""); result.ResultLength<- -10; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
                {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  {0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0} with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                      {0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                  a.C_CJRQ<-now
                  a.C_JLID<- {0}
                  a.C_BM<- {1}
                  a.C_BBM<-{2}
                  a.C_YWLX<-{3}
                  a.C_YWLXMC<-{4}
                  a.C_CZLX<-{5}
                  a.C_CZLXMC<-{6}{7}
                  a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName+","+getTableDescription zzTableName+ @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时新增总帐表"+getTableDescription zzTableName+ @""""
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          DatabaseInformation.GetColumnSchemal4Way zzTableName
          |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              new {0}({2})
              |>fun {1} ->
                  {3}    
                  sb.{0}.AddObject({1})"
          ,
          //{0},
          zzTableName
          ,
          //{1} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{2}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{3}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when DatabaseInformation.GetPKColumns zzTableName|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not->  //不是主键列
                sbTemSub.AppendFormat(@"
                  {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if DatabaseInformation.GetColumnSchemal4Way tableName|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match 
                      DatabaseInformation.GetAsFKRelationship zzTableName
                      |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                    | Some x  when x.PK_TABLE=tableName ->x.PK_COLUMN_NAME
                    | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                  {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

(*空格问题
  static member private GenerateSingleUpdateCodeForIndependentTable   (tableName:string)   (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq) =
    @"{3}
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}    
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DataAccessCodingAdvanceWithoutVariable.GenerateUpdateCodeForIndependentTable  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns

  static member private GenerateMultiUpdateCodeForIndependentTable   (tableName:string)   (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq) =
    @"{3}
    member x.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}    
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DataAccessCodingAdvanceWithoutVariable.GenerateUpdateCodeForIndependentTable  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
*)

  static member private GenerateSingleUpdateCodeForIndependentTable (databaseInstanceName:string)   (tableName:string)   (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)  (columnConditionTypes:ColumnConditionType seq)= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false, _ -> failwith ""The record is not exist!""
        | _, x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}    
            ()
        businessEntity
        |>DA_{6}Helper.GetTrackInfo now
        |>fun a->
            {7}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""UpdateEntity""); result.ResultLength<- -15; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        String.Empty
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships,tableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | z when z.ToLowerInvariant().EndsWith("datetime") &&  x.Equals("C_GXRQ") ->
                  sbTem.AppendFormat( @"
            original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
            original.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                original.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->
                original.{1}Reference.Load() 
                original.{1}<-null",
                    //{0}
                    (*
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""更新"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""更新"+ getTableDescription tableName + @"的记录"""
          )
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateMultiUpdateCodeForIndependentTable  (databaseInstanceName:string)     (tableName:string)   (tableColumns:DbColumnSchemalR seq) (tableAsFKRelationships:DbFKPK list) (tableAsPKRelationships:DbFKPK list) (tableKeyColumns:DbPKColumn seq)   (columnConditionTypes:ColumnConditionType seq)= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member x.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new {6}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}  
              () 
          executeContent.TrackingInfo
          |>DA_{6}Helper.GetTrackInfo now
          |>fun a->
              {7}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""UpdateEntities""); result.ResultLength<- -15; result",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} ,t_DJ_JHGL
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        String.Empty
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships,tableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | z when z.ToLowerInvariant().EndsWith("datetime") &&  x.Equals("C_GXRQ") ->
                  sbTem.AppendFormat( @"
              original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
              original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
              original.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                  original.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->
                  original.{1}Reference.Load() 
                  original.{1}<-null",
                    //{0}
                    (*
                    match tableName,tableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCodingAdvanceWithoutVariable.VariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}{7}
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""更新"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""更新"+ getTableDescription tableName + @"的记录"""
          )
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

(*
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>fun a ->
              let b=ref Unchecked.defaultof<_>
              sb.TryGetObjectByKey(a,b), !b with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->

可该写成
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""

*)
  static member  GenerateDeleteCodeForIndependentTable (databaseInstanceName:string) (tableName:string)  (tableColumns:DbColumnSchemalR seq)   (tableKeyColumns:DbPKColumn seq)  (columnConditionTypes:ColumnConditionType seq)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntity=executeContent.ExecuteData
        use sb=new {4}EntitiesAdvance()
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->
            sb.DeleteObject x
        businessEntity
        |>DA_{4}Helper.GetTrackInfo now
        |>fun a->
            {5}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? UpdateException as e -> ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntity""); result.ResultLength<- -16; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntity""); result.ResultLength<- -20; result",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        databaseInstanceName
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "3uy"
          ,
          //{6}
          @"""删除"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""删除"+ getTableDescription tableName + @"的记录"""
          )
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member  GenerateDeleteCodeForIndependentTableWithZZ (databaseInstanceName:string) (tableName:string)  (tableColumns:DbColumnSchemalR seq)   (tableKeyColumns:DbPKColumn seq) (tableAsPKRelationships:DbFKPK seq)  (columnConditionTypes:ColumnConditionType seq)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntity=executeContent.ExecuteData
        use sb=new {4}EntitiesAdvance()
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->
            sb.DeleteObject x
        {6}
        businessEntity
        |>DA_{4}Helper.GetTrackInfo now
        |>fun a->
            {5}
            a
        |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? UpdateException as e -> ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntity""); result.ResultLength<- -16; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntity""); result.ResultLength<- -20; result",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        databaseInstanceName
        ,
        //{5}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            a.C_CJRQ<-now
            a.C_JLID<- {0}
            a.C_BM<- {1}
            a.C_BBM<-{2}
            a.C_YWLX<-{3}
            a.C_YWLXMC<-{4}
            a.C_CZLX<-{5}
            a.C_CZLXMC<-{6}{7}
            a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName+","+getTableDescription zzTableName+ @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "5uy"
          ,
          //{6}
          @"""删除(同时删除总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时删除总帐表"+getTableDescription zzTableName+ @""""
          | _ ->
              @"""删除"+ getTableDescription tableName + @"的记录，同时删除总帐表"+getTableDescription zzTableName+ @""""
          )
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{6} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableKeyColumns=DatabaseInformation.GetPKColumns zzTableName
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        match
          (""{0}"",new {0}({1}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->
            sb.DeleteObject x"
          ,
          //{0},
          zzTableName
          ,
          //{1}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          (* 正确，但需要限制主表和总账表都只有单一主键列
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          *)
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member  GenerateMultiDeleteCodeForIndependentTable (databaseInstanceName:string) (tableName:string)  (tableColumns:DbColumnSchemalR seq)   (tableKeyColumns:DbPKColumn seq)   (columnConditionTypes:ColumnConditionType seq)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntities=executeContent.ExecuteData
        use sb=new {4}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _,x ->
              sb.DeleteObject x
          executeContent.TrackingInfo
          |>DA_{4}Helper.GetTrackInfo now
          |>fun a->
              {5}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? UpdateException as e -> ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntities""); result.ResultLength<- -17; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntities""); result.ResultLength<- -20; result",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        databaseInstanceName
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}{7}
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "3uy"
          ,
          //{6}
          @"""删除"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""删除"+ getTableDescription tableName + @"的记录"""
          )
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member  GenerateMultiDeleteCodeForIndependentTableWithZZ (databaseInstanceName:string) (tableName:string)  (tableColumns:DbColumnSchemalR seq)   (tableKeyColumns:DbPKColumn seq) (tableAsPKRelationships:DbFKPK seq)   (columnConditionTypes:ColumnConditionType seq)=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      let now=DateTime.Now
      let result=new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntities=executeContent.ExecuteData
        use sb=new {4}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _,x ->
              sb.DeleteObject x 
          {6}
          executeContent.TrackingInfo
          |>DA_{4}Helper.GetTrackInfo now
          |>fun a->
              {5}
              a
          |>sb.T_RZ.AddObject
        result.ResultLength<-sb.SaveChanges()
        result
      with
      | :? UpdateException as e -> ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntities""); result.ResultLength<- -17; result
      | e ->ObjectDumper.Write(e,2); Logger.Write(e,""DeleteEntities""); result.ResultLength<- -20; result",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        databaseInstanceName
        ,
        //{5}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              a.C_CJRQ<-now
              a.C_JLID<- {0}
              a.C_BM<- {1}
              a.C_BBM<-{2}
              a.C_YWLX<-{3}
              a.C_YWLXMC<-{4}
              a.C_CZLX<-{5}
              a.C_CZLXMC<-{6}{7}
              a.C_NR<-{8}",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ getTableDescription tableName+","+getTableDescription zzTableName+ @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "5uy"
          ,
          //{6}
          @"""删除(同时删除总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ getTableDescription tableName + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时删除总帐表"+getTableDescription zzTableName+ @""""
          | _ ->
              @"""删除"+ getTableDescription tableName + @"的记录，同时删除总帐表"+getTableDescription zzTableName+ @""""
          )
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{6} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableKeyColumns=DatabaseInformation.GetPKColumns zzTableName
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          match
            (""{0}"",new {0}({1}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x ->
              sb.DeleteObject x"
          ,
          //{0},
          zzTableName
          ,
          //{1}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e


////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  static member private GenerateQueryCodeForLeafTable (databaseInstanceName:string) tableName tableColumns tableAsFKRelationships tableAsPKRelationships=
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
    member x.Get{1}s (queryEntity:BQ_{1})=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_{2}[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        use sb=new {6}EntitiesAdvance()
        sb.{2}{0}
        |>PSeq.filter (fun a->
            {3})
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>PSeq.map (fun a->
            new BD_{2}
              ({4})
            |>fun entity ->
                {5}
                entity)
        |>Seq.toResult result
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e,""GetEntities""); BD_QueryResult<BD_{2}[]>()",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        tableAsFKRelationships
        |>PSeq.map (fun a->a. PK_TABLE_ALIAS)
        |>PSeq.distinct //一个主键可能由两个或两个以上键列构成
        |>Seq.iter (fun a->
            sbTem.AppendFormat(@".Include(FTN.{0})",
              //{0}
              a
              )|>ignore)
        sbTem.ToString()
        )
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          tableColumns
          |>PSeq.filter (fun b->tableAsFKRelationships|>PSeq.exists (fun c->c.FK_COLUMN_NAME=b.COLUMN_NAME) |>not) do
          match a.IS_NULLABLE_TYPED with
          | false ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.StartsWith y
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
          | _ ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.StartsWith y
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.Equals(y)
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second,queryEntity.IsQueryableNullOf{0} with
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && z.Value>y.Value ->x.Value>=y.Value && x.Value<=z.Value
            | x,y,z,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,z,_ when y.HasValue && not x.HasValue ->false
            | x,_,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,_ when y.HasValue && not x.HasValue ->false
            | x,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore

        (* case
            //该表作为当前表的外键列全为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&  
            //该表作为当前表的外键列全不为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&  
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x.{2}>=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore

          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2}.StartsWith y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value && x<>null->x.{2} >=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-3,3)|>ignore //Remove the last of "&& "
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              sbTem.AppendFormat( @"
              {0}=a.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        //if sbTem.Length>0 then sbTem.Remove(sbTem.Length-1,1)
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
            entity.C_JBR<-a.T_YG1.C_ID            
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID) 
                entity.C_SHR1 <- Nullable<System.Guid>(x.C_ID1) 
            | _ ->()
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do
                match b with
                | u,_  ->
                    sbTem.AppendFormat(@"
                entity.{0}<-a.{1}.{2}",
                      //{0}
                      u.FK_COLUMN_NAME
                      ,
                      //{1}
                      u.PK_TABLE_ALIAS
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat( @"
                match a.{0} with
                | x when x<>null ->
                    {1}
                | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- Nullable<{1}>(x.{2})",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        v.DATA_TYPE
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                sbTemSub.ToString().TrimStart()
                )
                )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
   

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//Do not use with   Temp variable
(*
member x.UpdateDJ_SH (businessEntity:BD_T_DJ_SH)=
      try 
        use sb=new SBIIMSEntitiesAdvance()
        match
          ("T_DJ_SH",new T_DJ_SH(C_ID=businessEntity.C_ID))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith "The record is not exist!"
        | _,x -> unbox<T_DJ_SH> x
        |>fun original->
            original.C_BZ<-businessEntity.C_BZ
            original.C_CJRQ<-businessEntity.C_CJRQ
            original.C_CKJE<-businessEntity.C_CKJE
            original.C_CKSL<-businessEntity.C_CKSL
            original.C_DJH<-businessEntity.C_DJH
            original.C_DJLX<-businessEntity.C_DJLX
            original.C_DJSZQJE<-businessEntity.C_DJSZQJE
            original.C_DJZT<-businessEntity.C_DJZT
            original.C_DYBZ<-businessEntity.C_DYBZ
            original.C_FKD<-businessEntity.C_FKD
            original.C_GHSLXR<-businessEntity.C_GHSLXR
            original.C_GXRQ<-now
            original.C_KHLXR<-businessEntity.C_KHLXR
            original.C_LR<-businessEntity.C_LR
            original.C_RKJE<-businessEntity.C_RKJE
            original.C_RKSL<-businessEntity.C_RKSL
            original.C_SZQJE<-businessEntity.C_SZQJE
            original.C_THBZ<-businessEntity.C_THBZ
            original.C_YHJE<-businessEntity.C_YHJE
            original.C_YSDJH<-businessEntity.C_YSDJH
            original.C_DJYZQJE<-businessEntity.C_DJYZQJE
            original.T_CK <-
              ("T_CK",new T_CK(C_ID=businessEntity.C_CCK))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_CK> 
            original.T_CK1 <-
              ("T_CK",new T_CK(C_ID=businessEntity.C_RCK))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_CK> 
            original.T_DWBM <-
              ("T_DWBM",new T_DWBM(C_ID=businessEntity.C_FBID))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_DWBM> 
            original.T_DWBM1 <-
              ("T_DWBM",new T_DWBM(C_ID=businessEntity.C_WFDW))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_DWBM> 
            original.T_GHS <-
              ("T_GHS",new T_GHS(C_ID=businessEntity.C_GHS))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_GHS> 
            original.T_KH <-
              ("T_KH",new T_KH(C_ID=businessEntity.C_KH))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_KH> 
            original.T_YG <-
              ("T_YG",new T_YG(C_ID=businessEntity.C_CZY))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_YG> 
            original.T_YG1 <-
              ("T_YG",new T_YG(C_ID=businessEntity.C_JBR))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<T_YG>
            match businessEntity.C_SHR with
            | x when x.HasValue  ->
                original.T_YG2 <-
                  ("T_YG",new T_YG(C_ID=x.Value))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<T_YG>
            | _ ->
                original.T_YG2Reference.Load() 
                original.T_YG2<-null    
            if businessEntity.BD_T_HZDJSPs.Count>0 then
              original.T_HZDJSP |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
              businessEntity.BD_T_HZDJSPs|>Seq.iter (fun a->
                new T_HZDJSP
                  (C_BZ=a.C_BZ,
                  C_BZQ=a.C_BZQ,
                  C_DJ=a.C_DJ,
                  C_SCRQ=a.C_SCRQ,
                  C_SL=a.C_SL,
                  C_XH=a.C_XH,
                  C_ZJE=a.C_ZJE,
                  C_ZKL=a.C_ZKL)
                |>fun b->
                    b.T_DWBM <-
                      ("T_DWBM",new T_DWBM(C_ID=a.C_FBID))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<T_DWBM>
                    b.T_SP <-
                      ("T_SP",new T_SP(C_ID=a.C_SP))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<T_SP>
                    original.T_HZDJSP.Add(b))
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1

*)

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

(* 关于 sb.ContextOptions.LazyLoadingEnabled, ...Reference.Load(), 及Include(...)
let queryEntity=BQ_DJ_JHGL()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

//加载方法sb.ContextOptions.LazyLoadingEnabled<-true, a.T_CKReference.Load(),.Include(...)都可以，且只有Include(...)支持Parallel
//由于需要使用Parallel方式，所以只能采用InClude方式，且将sb.ContextOptions.LazyLoadingEnabled<-false， 其默认为True
let GetDJ_JHGLs1 (queryEntity:BQ_DJ_JHGL)=
        use sb=new SBIIMSEntitiesAdvance()
        //sb.ContextOptions.LazyLoadingEnabled<-true //默认已经为True， 特别的LazyLoading不支持Parallel？？？
        sb.ContextOptions.LazyLoadingEnabled<-false
        sb.T_DJ_JHGL.Include("T_CK").Include("T_CK1").Include("T_DWBM").Include("T_GHS").Include("T_YG").Include("T_YG1").Include("T_YG2") //Right, 且Include方式支持Parallel方式
          //LazyLoading不支持Parallel？？？
        //|>PSeq.filter(fun a->   //各种加载方式都支持
        |> filter (fun a-> //LazyLoading不支持Parallel？？？
//            a.T_CKReference.Load()   //Right, 但Load()方法不支持Parallel
//            a.T_CK1Reference.Load()
//            a.T_DWBMReference.Load()
//            a.T_DWBM1Reference.Load()
//            a.T_GHSReference.Load()
//            a.T_YGReference.Load()
//            a.T_YG1Reference.Load()
//            a.T_YG2Reference.Load()
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG.C_ID =queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_JBR.HasValue  then
                 //true
                 a.T_YG1.C_ID =queryEntity.C_JBR .Value
                 //a.C_JBR =queryEntity.C_JBR .Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                    x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        //|>PSeq.map (fun a->    //各种加载方式都支持
        |>PSeq.map (fun a-> //LazyLoading不支持Parallel？？？
            let entity=
              B_T_DJ_JHGL
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJSZQJE=a.C_DJSZQJE,
                C_DJZT=a.C_DJZT,
                C_DYBZ=a.C_DYBZ,
                C_FKD=a.C_FKD,
                C_GXRQ=a.C_GXRQ,
                C_ID=a.C_ID,
                C_RKJE=a.C_RKJE,
                C_RKSL=a.C_RKSL,
                C_SZQJE=a.C_SZQJE,
                C_THBZ=a.C_THBZ,
                C_YHJE=a.C_YHJE,
                C_YSDJH=a.C_YSDJH,
                C_DJYZQJE=a.C_DJYZQJE)
//            entity.C_CCK<-a.C_CCK  //Right
//            entity.C_RCK<-a.C_RCK
//            entity.C_FBID<-a.C_FBID
//            entity.C_WFDW<-a.C_WFDW
//            entity.C_GHS<-a.C_GHS
//            entity.C_CZY<-a.C_CZY
//            entity.C_JBR<-a.C_JBR
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            entity
            )
        //|>PSeq.toList  
        |>PSeq.toNetList  
        
let businessEntities= GetDJ_JHGLs1 queryEntity
ObjectDumper.Write(businessEntities ,1)

*)