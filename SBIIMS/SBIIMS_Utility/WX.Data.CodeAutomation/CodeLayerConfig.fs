namespace WX.Data.CodeAutomation
open System
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Database

type Data =
  | Kind1 of int * int
  | Kind2 of string * string
//let data = Kind1(3,2)
//match data with
//| Kind1(a,b) -> a+b
//| Kind2(s1,s2) -> s1.Length + s2.Length


type CodeLayer=
  |DataEntities  //数据实体,业务实体的一部份
  |QueryEntities //查询实体, 业务实体的一部份
  |IDataAccess  
  |DataAccess 
  |BusinessLogic 
  |ServiceContract 
  |WcfService 
  |FViewData 

(*
type CodeLayerInfo=
  static member CodeLayerPath (codeLayer:CodeLayer)= 
    match codeLayer with
    | CodeLayer.DataEntities    -> @"WX.Data.BusinessEntities"
    | CodeLayer.QueryEntities   -> @"WX.Data.BusinessEntities"
    | CodeLayer.IDataAccess     -> @"WX.Data.IDataAccess"
    | CodeLayer.DataAccess      -> @"WX.Data.DataAccess"
    | CodeLayer.BusinessLogic   -> @"WX.Data.BusinessLogic"
    | CodeLayer.WcfService      -> @"WX.Data.WcfService"
    | CodeLayer.FViewData       -> @"WX.Data.FViewData"
*)

type CodeLayerInfo=
  static member CodeLayerPath (codeLayer:CodeLayer)= 
    match codeLayer with
    | DataEntities    -> @"WX.Data.BusinessEntities"
    | QueryEntities   -> @"WX.Data.BusinessEntities"
    | IDataAccess     -> @"WX.Data.IDataAccess"
    | DataAccess      -> @"WX.Data.DataAccess"
    | BusinessLogic   -> @"WX.Data.BusinessLogic"
    | ServiceContract      -> @"WX.Data.ServiceContracts"
    | WcfService      -> @"WX.Data.WcfService"
    | FViewData       -> @"WX.Data.FViewData"

(*
type CodeLayerPath=
  static member DataEntities ="WX.Data.BusinessEntities{0}" //BusinessDataEntities
  static member QueryEntities="WX.Data.BusinessEntities{0}" //BusinessQueryEntities
  static member QueryEntitiesCS="WX.Data.BusinessEntitiesCS{0}"
  static member IDataAccess="WX.Data.IDataAccess{0}"
  static member DataAccess="WX.Data.DataAccess{0}"
  static member BusinessLogic="WX.Data.BusinessLogic{0}"
  static member ServiceContract="WX.Data.ServiceContracts{0}"
  static member WcfService="WX.Data.WcfService{0}"
  static member FViewData="WX.Data.FViewData{0}"
  static member WcfServiceDevelopment="WX.Data.ServiceProxy{0}.Development"
*)

type CodeLayerPath=
  static member BusinessEntities (assemblySuffix:string)=String.Format("WX.Data.BusinessEntities{0}", assemblySuffix) //BusinessEntities
  static member DataEntities (assemblySuffix:string)=String.Format("WX.Data.BusinessDataEntities{0}", assemblySuffix) //BusinessDataEntities
  static member QueryEntities (assemblySuffix:string)=String.Format("WX.Data.BusinessEntities{0}", assemblySuffix) //BusinessQueryEntities
  static member QueryEntitiesServer (assemblySuffix:string)=String.Format("WX.Data.BusinessQueryEntities{0}", assemblySuffix) //BusinessQueryEntities的服务端部分，启用后"member QueryEntities"应停用
  static member QueryEntitiesClient (assemblySuffix:string)=String.Format("WX.Data.BusinessQueryEntitiesClient{0}", assemblySuffix) //BusinessQueryEntities的客户端部分
  static member QueryEntitiesCS (assemblySuffix:string)=String.Format("WX.Data.BusinessEntitiesCS{0}", assemblySuffix)  //临时
  static member IDataAccess (assemblySuffix:string)=String.Format("WX.Data.IDataAccess{0}", assemblySuffix)
  static member DataAccess (assemblySuffix:string)=String.Format("WX.Data.DataAccess{0}", assemblySuffix)
  static member BusinessLogic (assemblySuffix:string)=String.Format("WX.Data.BusinessLogic{0}", assemblySuffix)
  static member ServiceContract (assemblySuffix:string)=String.Format("WX.Data.ServiceContracts{0}", assemblySuffix)
  static member WcfService (assemblySuffix:string)=String.Format("WX.Data.WcfService{0}", assemblySuffix)
  static member WcfServiceWebIISHost (assemblySuffix:string)=String.Format("WX.Data.WcfService.WebIISHost{0}", assemblySuffix)
  static member WcfServiceWebIISHostAll (assemblySuffix:string)=String.Format("WX.Data.WcfService.WebIISHost{0}", String.Empty)
  static member FViewData (assemblySuffix:string)=String.Format("WX.Data.FViewData{0}", assemblySuffix)
  static member WcfServiceDevelopment (assemblySuffix:string)=String.Format("WX.Data.ServiceProxy{0}.Development", assemblySuffix)
  static member WcfClientChannelFromAzure (assemblySuffix:string)=String.Format("WX.Data.ClientChannel.FromAzure{0}", assemblySuffix)
  static member WcfClientChannelFromNative (assemblySuffix:string)=String.Format("WX.Data.ClientChannel.FromNative{0}", assemblySuffix)
  static member WcfClientChannelFromServer (assemblySuffix:string)=String.Format("WX.Data.ClientChannel.FromServer{0}", assemblySuffix)
  static member WcfClientChannel (assemblySuffix:string)=String.Format("WX.Data.ClientChannel{0}", assemblySuffix)
  static member DatabaseDictionary (assemblySuffix:string)=String.Format("WX.Data.DatabaseDictionary{0}", assemblySuffix) //数据库字典

  //Advance
  static member DataEntitiesAdvance (assemblySuffix:string)=String.Format("WX.Data.BusinessDataEntitiesAdvance{0}", assemblySuffix) //BusinessDataEntities
  static member QueryEntitiesAdvance (assemblySuffix:string)=String.Format("WX.Data.BusinessEntitiesAdvance{0}", assemblySuffix) //BusinessQueryEntities
  static member QueryEntitiesServerAdvance (assemblySuffix:string)=String.Format("WX.Data.BusinessQueryEntitiesAdvance{0}", assemblySuffix) //BusinessQueryEntities的服务端部分，启用后"member QueryEntities"应停用
  static member QueryEntitiesClientAdvance (assemblySuffix:string)=String.Format("WX.Data.BusinessQueryEntitiesClientAdvance{0}", assemblySuffix) //BusinessQueryEntities的客户端部分
  static member IDataAccessAdvance (assemblySuffix:string)=String.Format("WX.Data.IDataAccessAdvance{0}", assemblySuffix)
  static member DataAccessAdvance (assemblySuffix:string)=String.Format("WX.Data.DataAccessAdvance{0}", assemblySuffix)
  static member BusinessLogicAdvance (assemblySuffix:string)=String.Format("WX.Data.BusinessLogicAdvance{0}", assemblySuffix)
  static member ServiceContractAdvance (assemblySuffix:string)=String.Format("WX.Data.ServiceContractsAdvance{0}", assemblySuffix)
  static member WcfServiceAdvance (assemblySuffix:string)=String.Format("WX.Data.WcfServiceAdvance{0}", assemblySuffix)
  static member WcfServiceAdvanceWebIISHost (assemblySuffix:string)=String.Format("WX.Data.WcfServiceAdvance.WebIISHost{0}", assemblySuffix)
  static member WcfClientChannelAdvanceFromAzure (assemblySuffix:string)=String.Format("WX.Data.ClientChannelAdvance.FromAzure{0}", assemblySuffix)
  static member WcfClientChannelAdvanceFromNative (assemblySuffix:string)=String.Format("WX.Data.ClientChannelAdvance.FromNative{0}", assemblySuffix)
  static member WcfClientChannelAdvanceFromServer (assemblySuffix:string)=String.Format("WX.Data.ClientChannelAdvance.FromServer{0}", assemblySuffix)
  static member WcfClientChannelAdvance (assemblySuffix:string)=String.Format("WX.Data.ClientChannelAdvance{0}", assemblySuffix)
  static member FViewDataAdvance (assemblySuffix:string)=String.Format("WX.Data.FViewDataAdvance{0}", assemblySuffix)
  
  static member IDataAccessAdvanceX (assemblySuffix:string)=String.Format("WX.Data.IDataAccessAdvanceX{0}", assemblySuffix)
  static member DataAccessAdvanceX (assemblySuffix:string)=String.Format("WX.Data.DataAccessAdvanceX{0}", assemblySuffix)
  static member ServerCaching (assemblySuffix:string)=String.Format("WX.Data.ServerCaching{0}", match assemblySuffix with EndsWithIn [".GGHC"] x ->x.Replace(".GGHC",String.Empty) | x ->x)
  static member Caching (assemblySuffix:string)=String.Format("WX.Data.Caching{0}", match assemblySuffix with EndsWithIn [".GGHC"] x ->x.Replace(".GGHC",String.Empty) | x ->x)




(* Right!!! enum Type
type TableTemplateType=
  | IndependentTable =1 //独立表代码模板类型
  | LeafTable=2  //叶子表代码模板类型, 关系为 MainTable->ChildTable->LeafTable 或, MainTable->LeafTable 为商业实体的代码生成提供条件
  | ChildTable =3 //子表代码模板类型  关系为 MainTable->ChildTable->LeafTable
  | MainTableWithOneLevel =4 //主子表代码模板类型
  | MainTableWithTwoLevels=5  //主子表代码模板类型
*)

// Right!!! union Type, union项具有Tag值， 其大小按union项顺序累加，Tag=0..., 所以union项是可排序的
(* 测试Case
TableName=T_ZZ_JYFY     TableTemplateType={ }    LevelOneChildTableName=null     LevelTwoChildTableName=null
  TableTemplateType: Tag=0         IsMainTableWithTwoLevels=False  IsMainTableWithOneLevel=False   IsChildTable=False      IsLeafTable=False       IsIndependentTable=True
*)
type TableTemplateType=
  | IndependentTable  //独立表代码模板类型
  | LeafTable  //叶子表代码模板类型, 关系为 MainTable->ChildTable->LeafTable 或, MainTable->LeafTable 为商业实体的代码生成提供条件
  | ChildTable //子表代码模板类型  关系为 MainTable->ChildTable->LeafTable
  | MainTableWithOneLevel //主子表代码模板类型
  | MainTableWithTwoLevels  //主子表代码模板类型
  | DJLSHTable //单据流水号表
  | LSHTable //基本信息流水号表 
  | PCLSHTable //批次流水号表
  | JYLSHTable //批次流水号表

//----------------------------------------------------------------
(* Right Backup
//字段列条件类型
type ColumnConditionType=
  | HasNone  //独立表代码模板类型
  | HasDJLSH  //有单据流水号
  | HasLSH  //基本表流水号, 或独立表流水号
  | HasFID //存在FID, 存在父子记录时，不能分页
  | HasLSHAndFID  //同时具有流水号和父子记录(同时存在时，有些不合理)
 *)

type ColumnConditionType=
  //| HasNone  //独立表代码模板类型
  | HasDJLSH  //有单据流水号
  | HasPCInChild //在字表中有批次字段
  | HasLSH  //基本表流水号, 或独立表流水号,也就是有C_XBH
  | HasFID //存在FID, 存在父子记录时，不能分页
  | HasXH //存在序号，用于排序记录，该字段类型只适用于记录数量较小的选择表或树节点表
  | HasJYH //存在交易号，用于产生交易号 如 T_JYTZ_JHGL

//----------------------------------------------------------------

type TableRelationshipType=
  //| WithNone //没有关系表
  | WithForeignKeyRelatedTable //有外键表
  | WithZZB //有总账表
  | WithSameKeyParentTable //该表具有相同主键的父表，如T_DJ_JHGL和T_QFMX_JHGL, 主键相同，并且T_DJ_JHGL的主键也是T_QFMX_JHGL的外键，该表创建时要依赖于主表的创建

//----------------------------------------------------------------
(*
type TableRelatedInfo=
 {
 TableName:string
 TableTemplateType:TableTemplateType
 LevelOneChildTableName:string //主表->子表
 LevelTwoChildTableName:string //主表->子表->子表的子表
 mutable ColumnConditionType:ColumnConditionType //字段列条件类型
 mutable TableRelationshipType:TableRelationshipType //表关系类型
 }
 *)

type TableRelatedInfo=
 {
 TableName:string
 TableTemplateType:TableTemplateType //表模板类型
 LevelOneChildTableName:string //主表->子表
 LevelTwoChildTableName:string //主表->子表->子表的子表
 mutable ColumnConditionTypes:ColumnConditionType[] //字段列条件类型
 mutable TableRelationshipTypes:TableRelationshipType[] //表关系类型
 }
//----------------------------------------------------------------

type TableRelatedInfoX=
 {
 TableInfo:TableInfo
 TableTemplateType:TableTemplateType
 LevelOneChildTableInfo:TableInfo //主表->子表
 LevelTwoChildTableInfo:TableInfo //主表->子表->子表的子表
 mutable ColumnConditionTypes:ColumnConditionType[] //字段列条件类型
 mutable TableRelationshipTypes:TableRelationshipType[] //表关系类型
 }
     
type ViewRelatedInfo=class end

type StoredProcedureRelatedInfo=class end                                                    











