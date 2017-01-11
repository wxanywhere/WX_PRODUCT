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
type DataAccessCodingMainPart=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //结果为，命名空间部分*Seq<表名,代码>]static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      DataAccessCodingMainPart.GenerateNamespaceCode
      ,
      seq{
        for a in tableRelatedInfos do
          sb.Clear()|>ignore
          match a.TableTemplateType with
          | MainTableWithOneLevel 
          | MainTableWithTwoLevels
          | IndependentTable
          | ChildTable
          | LeafTable ->
              DataAccessCodingMainPart.GenerateTypeCode  a.TableName
              |>string|>sb.Append|>ignore
          | _ -> ()
          match a.TableTemplateType with
          | MainTableWithOneLevel ->
              DataAccessCodingMainChildOneLevelTablePart.GetCodeWithMainChildTableOneLevelTemplate databaseInstanceName a.TableName a.LevelOneChildTableName a
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | MainTableWithTwoLevels ->
              DataAccessCodingMainChildTwoLevelTablePart.GetCodeWithMainChildTableTwoLevelTemplate databaseInstanceName a.TableName a.LevelOneChildTableName a.LevelTwoChildTableName 
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | IndependentTable ->
              DataAccessCodingIndependentTablePart.GetCodeWithIndependentTable databaseInstanceName a.TableName a
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | ChildTable ->  //只能查询当前表和叶子表信息
              DataAccessCodingChildTablePart.GetCodeWithChildTableTemplate databaseInstanceName a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | LeafTable -> 
              DataAccessCodingLeafTablePart.GetCodeWithLeafTableTemplate databaseInstanceName a.TableName a
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | _ ->()    //不独立生成代码
          (*
          | DJLSHTable -> //单据流水号表
              () //不单独生成代码
          | LSHTable -> //基本信息流水号表
              () //不单独生成代码
          *)
          if sb.Length>0  then yield a.TableName, string sb 
      }
    with 
    | e ->ObjectDumper.Write(e,2); raise e
    
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

  static member private GenerateNamespaceCode=
    @"namespace WX.Data.DataAccess
open System
open System.Data
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open Microsoft.Practices.EnterpriseLibrary.Logging
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.ServerHelper"


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

  //--------------------------------------------------------------------------------------------------------------------------------------
    
///////////////////////////////////////////

(*子模板空格不正确
  static member private GenerateSingleCreateCodeForMainChildOneLevelTables (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    @"
    member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
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
            for child in businessEntity.BD_{6}s do
              new {6}
                ({8})
              |>fun {7} ->
                  {9}
                  {3}.{6}.Add({7})
            sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
    |>DataAccessCodingMainPart.GenerateCreateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 

  static member private GenerateMultiCreateCodeForMainChildOneLevelTables (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    @"
    member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
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
              for child in businessEntity.BD_{6}s do
                new {6}
                  ({8})
                |>fun {7} ->
                    {9}
                    {3}.{6}.Add({7})
              sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
    |>DataAccessCodingMainPart.GenerateCreateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
*)

//-------------------------------------------------------------------------------------------------------------------------------


//==================================================================================

//Do not use with   Temp variable
(*
member this.UpdateDJ_SH (businessEntity:BD_T_DJ_JXCSH)=
      try 
        use sb=new SBIIMSEntitiesAdvance()
        match
          ("T_DJ_JXCSH",new T_DJ_JXCSH(C_ID=businessEntity.C_ID))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith "The record is not exist!"
        | _,x -> unbox<T_DJ_JXCSH> x
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
            if businessEntity.BD_T_HZDJSPs.Length>0 then
              original.T_HZDJSP |>Seq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
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
        //|>Seq.filter(fun a->   //各种加载方式都支持
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
        //|>Seq.map (fun a->    //各种加载方式都支持
        |>Seq.map (fun a-> //LazyLoading不支持Parallel？？？
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
        //|>Seq.toList  
        |>Seq.toArray  
        
let businessEntities= GetDJ_JHGLs1 queryEntity
ObjectDumper.Write(businessEntities ,1)

*)