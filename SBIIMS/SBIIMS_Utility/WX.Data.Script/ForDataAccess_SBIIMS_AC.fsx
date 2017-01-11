//#I  @"C:\Program Files\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Threading.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"


open System
open System.Collections.Generic
open System.Linq
open System.Data
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Validation
open Microsoft.Practices.EnterpriseLibrary.Logging
open Microsoft.FSharp.Collections

//let projectPath= @"D:\Workspace\SBIIMS"

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.DataModel.AccessControl.dll"
#r "WX.Data.BusinessEntities.AccessControl.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.IDataAccess.AccessControl.dll"
#r "WX.Data.DataAccess.AccessControl.dll"
#r "WX.Data.CViewModelBase.dll"
#r "WX.Data.CClientHelper.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.IDataAccess
open WX.Data.DataAccess
open WX.Data.DataOperate
open WX.Data.PSeq
open WX.Data.BusinessEntities
open WX.Data.BusinessBase
open WX.Data.ClientHelper

///////////////////////////////////////////////////////////////////////////////////////////////////

ConfigHelper.INS.LoadDefaultServiceConfigToManager
//ConfigurationManager.ConnectionStrings //不能使用该语句，有时会将加载到内存的配置信息重置？？？
//ConfigurationManager.ConnectionStrings.["SBIIMSEntities"] 


let rec Process (BD_T_AC_JSGN_Advance seq)=
  


let sb=new SBIIMSACEntitiesAdvance()
(sb.T_AC_JSGN
|>pseq
|>filter (fun a->a.C_JSID =queryEntity.C_ID.Value)   //ExistedJSGN
,
  sb.T_AC_JSGN.Include(FTN.T_AC_GNJD)
  |>pseq
  |>filter (fun b->b.C_JSID=queryEntity.C_FID.Value))
|>fun (a,b)->
  b
  |>append a
  |>filter (fun c->c



|>fun a-> 
    sb.T_AC_JSGN.Include(FTN.T_AC_GNJD)
    |>pseq
    |>filter (fun b->
        b.C_JSID=queryEntity.C_FID.Value &&
        a|>exists (fun c->c.C_GNID=b.C_GNID)|>not) //NewJSGN
|>map (fun a->
    let entity=
      new BD_T_AC_JSGN_Advance
        (C_CJRQ=a.C_CJRQ,
        C_GXRQ=a.C_GXRQ,
        C_QXBZ=a.C_QXBZ,
        C_XZBZ=a.C_XZBZ)
    entity.C_GNID<-a.T_AC_GNJD.C_ID
    entity.C_JSID<-queryEntity.C_ID.Value //a.T_AC_JS.C_ID
    entity.VC_FGNID<-a.T_AC_GNJD.C_FID
    entity.VC_FJSID<-queryEntity.C_FID.Value //a.T_AC_JS.C_FID
    entity.VC_GNMC<-a.T_AC_GNJD.C_MC
    entity.VC_JSMC<-queryEntity.C_MC //a.T_AC_JS.C_MC
    entity)



let sb=new SBIIMSACEntitiesAdvance()
let rec FindAllDeleteChildJSGNs (sourceJSGNs:T_AC_JSGN array) (deleteJSGNs:BD_T_AC_JSGN[]) (role:Guid)= //let rec FindAllDeleteChildJSGNs (sourceJSGNs:T_AC_JSGN seq) (deleteJSGNs:BD_T_AC_JSGN[]) (role:Guid)=
  seq{
      match sourceJSGNs,deleteJSGNs,role with
      | x,y,z ->
          for a in 
            x|>Seq.filter (fun a->a.T_AC_JS.C_FID=z && deleteJSGNs|>Seq.exists (fun b->b.C_GNID=a.C_GNID)) do
            yield a
            yield! FindAllDeleteChildJSGNs sourceJSGNs y a.C_JSID}
FindAllDeleteChildJSGNs (sb.T_AC_JSGN.Include(FTN.T_AC_JS)|>Seq.toArray) deleteJSGNs (deleteJSGNs|>Seq.head).C_JSID



let sb=new SBIIMSACEntitiesAdvance()
sb.T_AC_JSGN
|>Seq.toArray
|>fun a-> 
  ObjectDumper.Write(a,2)


let DeleteJSGNForCurrentRole (deleteJSGNs:BD_T_AC_JSGN[])=
      try
        use sb=new SBIIMSACEntitiesAdvance()
        let rec FindAllDeleteChildJSGNs (sourceJSGNs:T_AC_JSGN seq) (deleteJSGNs:BD_T_AC_JSGN[]) (role:Guid)=
          seq{
              match sourceJSGNs,deleteJSGNs,role with
              | x,y,z ->
                  for a in 
                    x|>pseq|>filter (fun a->a.T_AC_JS.C_FID=z && deleteJSGNs|>pseq|>exists (fun b->b.C_GNID=a.C_GNID)) do
                    yield a
                    FindAllDeleteChildJSGNs x y a.C_JSID |>ignore}
        FindAllDeleteChildJSGNs sb.T_AC_JSGN deleteJSGNs (deleteJSGNs|>Seq.head).C_JSID
        |>Seq.append deleteJSGNs
        |>Seq.iter (fun a->sb.DeleteObject entity)
        sb.SaveChanges()
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); -1




//Right 
let rec FindAllChildRoles (roleEntities:T_AC_JS seq) (role:Guid)=
      seq{
          match roleEntities,role with
          | x,y when x|>Seq.exists (fun a->a.C_FID=y) ->
               for a in x|>Seq.filter (fun a->a.C_FID=y) do
                 yield a.C_ID
                 yield FindAllChildRoles x a.C_ID


          | _ -> ()}

//Right, tested
let rec FindAllChildRoles01 (roleEntities:T_AC_JS seq) (role:Guid)=
          match roleEntities,role with
          | x,y when x|>Seq.exists (fun a->a.C_FID=y) ->
              [
               for a in x|>Seq.filter (fun a->a.C_FID=y) do
                 yield a.C_ID
              ]
              @
              ([for a in x|>Seq.filter (fun a->a.C_FID=y) do
                  yield FindAllChildRoles01 x a.C_ID]|>List.concat)
          | _ -> []

//Right, tested
let rec FindAllChildRoles02 (roleEntities:T_AC_JS seq) (role:Guid)=
          match roleEntities,role with
          | x,y when x|>Seq.exists (fun a->a.C_FID=y) ->
              [
               for a in x|>Seq.filter (fun a->a.C_FID=y) do
                 yield a.C_ID
                 yield! FindAllChildRoles02 x a.C_ID
              ]
          | _ -> []
                
let sb=new SBIIMSACEntitiesAdvance()
sb.T_AC_JS
//|>Seq.
|>Seq.toArray
|>fun a->
  FindAllChildRoles01 a (new Guid("d4d3baea-f693-41d9-be75-a73d3dea09b7"))
|>ObjectDumper.Write



let DeleteJSGNForCurrentRole (deleteJSGNs:BD_T_AC_JSGN[])=
      try
        use sb=new SBIIMSACEntitiesAdvance()
        let rec FindAllChildRoles (sourceJSGNs:T_AC_JSGN seq) (deleteJSGNs:BD_T_AC_JSGN[]) (role:Guid)=
          seq{
              match sourceJSGNs,deleteJSGNs,role with
              | x,y,z ->
                   x
                   |>Seq.filter (fun a->a.T_AC_JS.C_FID=z && deleteJSGNs|>pseq|>exists (fun b->b.C_GNID=a.C_GNID))
                   |>iter (fun a ->
                     yield a
                     FindAllChildRoles x a.C_JSID |>ignore)}
        
        (sb.T_AC_JSGN.Include(FTN.T_AC_JS)
        |>Seq.distinctBy (fun a->a.C_JSID)
         ,
          deleteJSGNs
          |>Seq.head)
        |>fun (a,b)->
            FindAllChildRoles a b.C_JSID
        |>fun a->
            sb.T_AC_JSGN
            |>pseq
            |>filter (fun b->a|>pseq|>exists (fun c->c=b.C_JSID) && deleteJSGNs|>pseq|>exists (fun c->c.C_GNID=b.C_GNID) )
        |>fun a->
          for entity in a do
            sb.DeleteObject entity
        sb.SaveChanges()
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); -1

/////////////////////////////////////////////////////////////////////////////

let DeleteJSGNForCurrentRole (deleteJSGNs:BD_T_AC_JSGN[])=
      try
        use sb=new SBIIMSACEntitiesAdvance()
        let rec FindAllChildRoles (sourceJSGNs:T_AC_JSGN seq) (role:Guid)=
          seq{
              match sourceJSGNs,role with
              //| x,y when x=null || (Seq.head x).T_AC_JS=null ->() //不好
              | x,y when x|>Seq.exists (fun a->a.T_AC_JS.C_FID=y) ->
                   for a in x|>Seq.filter (fun a->a.T_AC_JS.C_FID=y) do
                     yield a.C_JSID
                     FindAllChildRoles x a.C_JSID |>ignore
              | _ -> ()}
        
        (sb.T_AC_JSGN.Include(FTN.T_AC_JS)
        |>Seq.distinctBy (fun a->a.C_JSID)
         ,
          deleteJSGNs
          |>Seq.head)
        |>fun (a,b)->
            FindAllChildRoles a b.C_JSID
        |>fun a->
            sb.T_AC_JSGN
            |>pseq
            |>filter (fun b->a|>pseq|>exists (fun c->c=b.C_JSID) && deleteJSGNs|>pseq|>exists (fun c->c.C_GNID=b.C_GNID) )
        |>fun a->
          for entity in a do
            sb.DeleteObject entity
        sb.SaveChanges()
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); -1


let GetAvailableJSGNWithCurrentRole (queryEntity:BQ_AC_JS)=
      try
        use sb=new SBIIMSACEntitiesAdvance()
        match 
          //queryEntity.C_FID.Value, when ="00000000-0000-0000-0000-000000000001",为根角色 //Right. 不通用
          sb.T_AC_JSGN
          |>pseq
          |>filter (fun a->a.C_JSID=queryEntity.C_FID.Value) //角色ID作为其他角色的父Id
          |>length with
        | x when x >0-> //不是根节点
            sb.T_AC_JSGN
            |>pseq
            |>filter (fun a->
                a.C_JSID =queryEntity.C_ID.Value) //ExistedJSGN
            |>fun a-> 
                sb.T_AC_JSGN.Include(FTN.T_AC_GNJD)
                |>pseq
                |>filter (fun b->
                    b.C_JSID=queryEntity.C_FID.Value &&
                    a|>exists (fun c->c.C_GNID=b.C_GNID)|>not) //NewJSGN
            |>map (fun a->
                let entity=
                  new BD_T_AC_JSGN_Advance
                    (C_CJRQ=a.C_CJRQ,
                    C_GXRQ=a.C_GXRQ,
                    C_QXBZ=a.C_QXBZ,
                    C_XZBZ=a.C_XZBZ)
                entity.C_GNID<-a.T_AC_GNJD.C_ID
                entity.C_JSID<-queryEntity.C_ID.Value //a.T_AC_JS.C_ID
                entity.VC_FGNID<-a.T_AC_GNJD.C_FID
                entity.VC_FJSID<-queryEntity.C_FID.Value //a.T_AC_JS.C_FID
                entity.VC_GNMC<-a.T_AC_GNJD.C_MC
                entity.VC_JSMC<-queryEntity.C_MC //a.T_AC_JS.C_MC
                entity)
        | x when x=0-> //是根节点
            sb.T_AC_JSGN
            |>pseq
            |>filter (fun a->
                a.C_JSID =queryEntity.C_ID.Value) //ExistedJSGN
            |>fun a-> 
                sb.T_AC_GNJD
                |>pseq
                |>filter (fun b-> a|>exists (fun c->c.C_GNID=b.C_ID)|>not)
                |>map (fun b->
                    new BD_T_AC_JSGN_Advance
                      (C_CJRQ=b.C_CJRQ,
                      C_GXRQ=b.C_GXRQ,
                      C_QXBZ="-1",
                      C_XZBZ="-1")
                    |>fun c->
                        c.C_GNID<-b.C_ID
                        c.C_JSID<-queryEntity.C_ID.Value
                        c.VC_FGNID<-b.C_FID
                        c.VC_FJSID<-queryEntity.C_FID.Value
                        c.VC_GNMC<-b.C_MC
                        c.VC_JSMC<-queryEntity.C_MC
                        c)
        | _ -> failwith "The condition has some problem"
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_AC_JSGN_Advance>()

new BQ_AC_JS()
|>fun a -> 
   a.C_ID<-new Nullable<Guid>(new Guid("af07380f-9ef4-4dc0-b5fe-f8aaae1339b4"))
   a.C_FID<-new Nullable<Guid>(new Guid("00000000-0000-0000-0000-000000000001"))
   a
|>GetAvailableJSGNWithCurrentRole 
|>WX.Data.Helper.ObjectDumper.Write 

//|>Seq.length
|>fun a ->
   TreeNodeBuildAdvance.GetEntityNodes<BD_T_AC_JSGN_Advance,Guid>(a,"VC_FGNID",new Guid("00000000-0000-0000-0000-000000000001"),"C_GNID")
//
let GetDeleteCheckedJSGNs (checkedJSGNs:BD_T_AC_JSGN_Advance[])=

  
//Get delete checked nodes
let GetDeleteCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
    let rec GetDeleteCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when x.IsChecked.Value=true-> //等于false或null时不删除
          x.Entity::  
            (if x.HasChildNode then
              [for child in x.ChildNodes ->
                GetDeleteCheckedNodes child
              ]|>List.concat
             else [])
      | _ -> []
    GetDeleteCheckedNodes node|>List.toArray

//Create
let CreateJSGNs (businessEntities:BD_T_AC_JSGN[])=
      try 
        use sb=new SBIIMSACEntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            ("T_AC_JSGN",new T_AC_JSGN(C_GNID=businessEntity.C_GNID,C_JSID=businessEntity.C_JSID))
            |>sb.CreateEntityKey 
            |>fun a ->sb.TryGetObjectByKey(a,ref Unchecked.defaultof<_>) with
          | true -> failwith "The record is exist?" | _ ->()
          let t_AC_JSGN=
            new T_AC_JSGN
              (C_CJRQ=DateTime.Now,
              C_GXRQ=DateTime.Now,
              C_QXBZ=businessEntity.C_QXBZ,
              C_XZBZ=businessEntity.C_XZBZ)
          t_AC_JSGN.T_AC_GNJD <-
            ("T_AC_GNJD",new T_AC_GNJD(C_ID=businessEntity.C_GNID))
            |>sb.CreateEntityKey
            |>sb.GetObjectByKey
            |>unbox<T_AC_GNJD>
          t_AC_JSGN.T_AC_JS <-
            ("T_AC_JS",new T_AC_JS(C_ID=businessEntity.C_JSID))
            |>sb.CreateEntityKey
            |>sb.GetObjectByKey
            |>unbox<T_AC_JS>    
          sb.AddToT_AC_JSGN(t_AC_JSGN)
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1


//Get Added Nodes
let GetNewCheckedJSGNs (checkedJSGNs:BD_T_AC_JSGN_Advance[]) (existedJSGNs:BD_T_AC_JSGN_Advance[])=
  checkedJSGNs
  |>pseq
  |>filter (fun a->existedJSGNs|>pseq|>exists (fun b->b.C_GNID=a.C_GNID && b.C_JSID=a.C_JSID) |>not)
  |>toArray




//Get add checked nodes
let GetAddCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
    let rec GetAddCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when x.IsChecked.HasValue|>not || x.IsChecked.Value=true->
          x.Entity::  
            (if x.HasChildNode then
              [for child in x.ChildNodes ->
                GetAddCheckedNodes child
              ]|>List.concat
             else [])
      | _ -> []
    GetAddCheckedNodes node|>List.toArray

//Right
let GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
    let rec GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when x.IsChecked.HasValue|>not || x.IsChecked.Value=true->
          seq{yield x.Entity}  //Seq{x.Entity}, It can but not right.
          |>Seq.append
            (if x.HasChildNode then
              seq{for child in x.ChildNodes ->
                  GetCheckedNodes child
              }|>Seq.concat
             else seq[])
      | _ ->seq []
    GetCheckedNodes node|>Seq.toArray


//Wrong reference
let GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
    let rec GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when node.HasChildNode ->
            if x.IsChecked.HasValue|>not && x.IsChecked.Value=true then
              seq{yield x.Entity}  //Seq{x.Entity}, It can but not right.
              |>Seq.append
                  (seq{
                    for child in x.ChildNodes ->
                      GetCheckedNodes child
                }|>Seq.concat)
            else seq []
      | _ ->seq []
    GetCheckedNodes node|>Seq.toArray



//Right Reference, 常规方式,使用中间参考变量
let GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
    let checkedNodes=new List<BD_T_AC_JSGN_Advance>()
    let rec GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when x.IsChecked.HasValue|>not && x.IsChecked.Value ->
          checkedNodes.Add x.Entity
          if x.HasChildNode then
            for child in x.ChildNodes do
              GetCheckedNodes child
      | _ ->()
    GetCheckedNodes node
    checkedNodes|>Seq.toArray

//Right Reference, 常规方式,使用中间参考变量
let AddRoleFunctions (node:Node<BD_T_AC_JSGN_Advance>)=
    let checkedNodes=new List<BD_T_AC_JSGN_Advance>()
    let rec GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>) (checkedNodes:List<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when x.IsChecked.HasValue|>not && x.IsChecked.Value ->
          checkedNodes.Add x.Entity
          if x.HasChildNode then
            for child in x.ChildNodes do
              GetCheckedNodes child
      | _ ->()
    GetCheckedNodes node checkedNodes

//Right reference
//    let  rec GetSectionNames (sectionGroup:ConfigurationSectionGroup)=
//      seq{if sectionGroup.Sections.Count>0 then
//                 for section in sectionGroup.Sections do
//                   yield section.SectionInformation.SectionName} 
//     |>Seq.append 
//             (seq{if sectionGroup.SectionGroups.Count>0 then
//                     for sectionGroupChild in sectionGroup.SectionGroups ->
//                       GetSectionNames  sectionGroupChild} |>Seq.concat)


//Wrong
let AddRoleFunctions (node:Node<BD_T_AC_JSGN_Advance>)=
    let rec GetCheckedNodes (node:Node<BD_T_AC_JSGN_Advance>)=
      match node with
      | x when node.HasChildNode ->
            if x.IsChecked.HasValue|>not && x.IsChecked.Value=true then
              node.Entity::
                for child in node.ChildNodes do
                  GetCheckedNodes child
      | _ ->node.Entity
  


let GetAvailableJSGN (queryEntity:BQ_AC_JS)=
      try
        use sb=new SBIIMSACEntitiesAdvance()
        match 
          sb.T_AC_JSGN
          |>pseq
          |>filter (fun a->a.C_JSID=queryEntity.C_FID.Value) 
          |>length with
        | x when x >0-> //不是根节点
            sb.T_AC_JSGN
            |>pseq
            |>filter (fun a->
                a.C_JSID =queryEntity.C_ID.Value) //ExistedJSGN
            |>fun a-> 
                sb.T_AC_JSGN.Include(FTN.T_AC_GNJD)
                |>pseq
                |>filter (fun b->
                    b.C_JSID=queryEntity.C_FID.Value &&
                    a|>exists (fun c->c.C_GNID=b.C_GNID)|>not) //NewJSGN
            |>map (fun a->
                let entity=
                  new BD_T_AC_JSGN_Advance
                    (C_CJRQ=a.C_CJRQ,
                    C_GXRQ=a.C_GXRQ,
                    C_QXBZ=a.C_QXBZ,
                    C_XZBZ=a.C_XZBZ)
                entity.C_GNID<-a.T_AC_GNJD.C_ID
                entity.C_JSID<-queryEntity.C_ID.Value //a.T_AC_JS.C_ID
                entity.VC_FGNID<-a.T_AC_GNJD.C_FID
                entity.VC_FJSID<-queryEntity.C_FID.Value //a.T_AC_JS.C_FID
                entity.VC_GNMC<-a.T_AC_GNJD.C_MC
                entity.VC_JSMC<-queryEntity.C_MC //a.T_AC_JS.C_MC
                entity)
        | x when x=0-> //是根节点
            sb.T_AC_JSGN
            |>pseq
            |>filter (fun a->
                a.C_JSID =queryEntity.C_ID.Value) //ExistedJSGN
            |>fun a-> 
                sb.T_AC_GNJD
                |>pseq
                |>filter (fun b-> a|>exists (fun c->c.C_GNID=b.C_ID)|>not)
                |>map (fun b->
                    new BD_T_AC_JSGN_Advance
                      (C_CJRQ=b.C_CJRQ,
                      C_GXRQ=b.C_GXRQ,
                      C_QXBZ="-1",
                      C_XZBZ="-1")
                    |>fun c->
                        c.C_GNID<-b.C_ID
                        c.C_JSID<-queryEntity.C_ID.Value
                        c.VC_FGNID<-b.C_FID
                        c.VC_FJSID<-queryEntity.C_FID.Value
                        c.VC_GNMC<-b.C_MC
                        c.VC_JSMC<-queryEntity.C_MC
                        c)
        | _ -> failwith "The condition has some problem"
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_AC_JSGN_Advance>()


let queryEntity=
   new BQ_AC_JS()
   |>fun a->
       a.C_ID<-new Nullable<Guid>(new Guid("af07380f-9ef4-4dc0-b5fe-f8aaae1339b4"))
       a.C_FID<-new Nullable<Guid>(new Guid("00000000-0000-0000-0000-000000000001"))
       a
let sb=new SBIIMSACEntitiesAdvance()

//Existed JSGN
sb.T_AC_JSGN
|>pseq
|>filter (fun a->
            a.C_JSID =queryEntity.C_ID.Value) 
|>iter (fun a->ObjectDumper.Write a)

//available JSGN
sb.T_AC_JSGN
|>pseq
|>filter (fun a->
            a.C_JSID =queryEntity.C_FID.Value) 
//|>map (fun a->a)
|>iter (fun a->ObjectDumper.Write a)

//New 
GetAvailableJSGN queryEntity
|>Seq.iter (fun b->ObjectDumper.Write b)

//Add function
//let newJSGNNodes=
////  let queryJSGN (c_JSID:Guid)=
////  let queryEntity=new BQ_AC_JSGN()
////  queryEntity.C_JSID <-Nullable<Guid>(c_JSID)
//  TreeNodeBuildAdvance.GetEntityNodes<BD_T_AC_JSGN_Advance,Guid>
//    (clientJSGNWH.GetAC_JSGNView queryEntity,"VC_FID",new Guid("00000000-0000-0000-0000-000000000001"),"C_GNID")


  




//Remove functon



let result=
  new BQ_AC_JS()
  |>fun a->
      a.C_ID<-new Nullable<Guid>(new Guid("7367ed78-a542-4ddd-828d-18d775b905a7"))
      a.C_FID<-new Nullable<Guid>(new Guid("fbe9fe76-58a9-4ab0-ad8d-c56cc5c91f9b"))
      GetNewJSGN a


let GetJSGNNew1 (queryEntity:BQ_AC_JS)=
      try
        use sb=new SBIIMSACEntitiesAdvance()
        let existedJSGNs=
          sb.T_AC_JSGN
          |>pseq
          |>filter (fun a->
              a.C_JSID =queryEntity.C_ID.Value)
        sb.T_AC_JSGN
        |>pseq
        |>filter (fun a->
            a.C_JSID=queryEntity.C_FID.Value &&
            existedJSGNs|>exists (fun b->b.C_GNID=a.C_GNID)|>not)
        |>map (fun a->
            let entity=
              new BD_T_AC_JSGN_Advance
                (C_CJRQ=a.C_CJRQ,
                C_GXRQ=a.C_GXRQ,
                C_QXBZ=a.C_QXBZ,
                C_XZBZ=a.C_XZBZ)
            entity.C_GNID<-a.T_AC_GNJD.C_ID
            entity.C_JSID<-a.T_AC_JS.C_ID
            entity.VC_FID<-a.T_AC_GNJD.C_FID
            entity.VC_MC<-a.T_AC_GNJD.C_MC
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_AC_JSGN_Advance>()


///////////////////////////////////////////////////////////////////////////////////////////////////
//LianXiRen
let GetGHSFZR=
      let queryEntity=new BQ_GHSFZR()
    //member x.GetGHSFZRs (queryEntity:BQ_GHSFZR)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHSFZR.Include(FTN.T_DWBM).Include(FTN.T_GHS)
        |>pseq
        |>filter (fun a->
            match a.T_GHS,queryEntity.C_GHS with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              BD_T_GHSFZR
                (C_XM=a.C_XM)
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHSFZR>()

//地区
let GetDQ=
      let queryEntity=new BQ_DQ()
    //member x.GetDQs (queryEntity:BQ_DQ)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_DQ.Include(FTN.T_DWBM)
        |>pseq
        |>sortBy (fun a->a.C_XH)
        |>map (fun a->
            let entity=
              BD_T_DQ
                (C_MC=a.C_MC)
            entity.C_FBID<-a.T_DWBM.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_DQ>()

ObjectDumper.Write(GetDQ)

//供货商等级选项
//member x.GetGHSDJs (queryEntity:BQ_GHSDJ)=
let GetGHSDJ=
      let queryEntity=new BQ_GHSDJ()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHSDJ
        |>pseq
        |>filter (fun a->
            match a.C_DJ,queryEntity.C_DJ with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_DM,queryEntity.C_DM with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              BD_T_GHSDJ
                (C_DJ=a.C_DJ,
                C_DM=a.C_DM)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHSDJ>()

ObjectDumper.Write(GetGHSDJ)

//GHS 信息 
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun (b,ghslx) c ->
            b,ghslx,
            match c|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->""
            | x ->x.C_XM) 
        |>fun a->
            join a (sb.T_DQ|>pseq) (fun (b,_,_)->b.C_DQ) (fun c->c.C_ID) (fun (b,ghslx,lxr) c->b,ghslx,lxr, c.C_MC)
        |>fun a->
            groupJoin a (sb.T_QFMX_GHS
            |>pseq)
            ////|>groupByO (fun b->b.C_GHS)  //使用 Seq.groupBy,Right
            ////|>map (fun (b,c)->b,c|>pseq|>sumByDecimal (fun b->b.C_QFJE))) //Right
            //|>groupBy (fun b->b.C_GHS) //使用ParallelEnumerable.Sum,  Right
            //|>map (fun b->b.Key,b|>pseq |>sumByDecimal (fun c->c.C_QFJE))) //Right
            //    (fun (((b,_),_),_)->b.C_ID) (fun (c,d)->c) (fun b c->
                (fun (b,_,_,_)->b.C_ID) (fun c->c.C_GHS) (fun (b,ghslx,lxr,dq) c->
                b,ghslx,lxr,dq,
                match c|>pseq  with
                | x when length x=0 ->0M //可使用 decimal 0 进行测试 
                //| x->x|>headOrDefault|>snd) //与前面的groupBy配合，Right
                | x->x|>sumByDecimal (fun c->c.C_QFJE))
        |>map (fun (a,lx,lxr,dq,yfje)->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                C_LXDH=a.C_LXDH,
                C_LXDZ=a.C_LXDZ,
                C_MR=a.C_MR,
                C_JY=a.C_JY,
                C_XYJF=a.C_XYJF,
                C_GHJF=a.C_GHJF,
                C_DJ=a.C_DJ,
                C_BZ=a.C_BZ,
                VC_LX=lx,
                VC_LXR=lxr,
                VC_DQ=dq,
                VC_YFJE=yfje)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)


//GHS
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun b c ->
            b,
            match c|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->""
            | x ->x.C_XM) 
        |>fun a->
            join a (sb.T_DQ|>pseq) (fun ((b,_),_)->b.C_DQ) (fun c->c.C_ID) (fun b c->b, c.C_MC)
        |>fun a->
            groupJoin a (sb.T_QFMX_GHS
            |>pseq)
            ////|>groupByO (fun b->b.C_GHS)  //使用 Seq.groupBy,Right
            ////|>map (fun (b,c)->b,c|>pseq|>sumByDecimal (fun b->b.C_QFJE))) //Right
            //|>groupBy (fun b->b.C_GHS) //使用ParallelEnumerable.Sum,  Right
            //|>map (fun b->b.Key,b|>pseq |>sumByDecimal (fun c->c.C_QFJE))) //Right
            //    (fun (((b,_),_),_)->b.C_ID) (fun (c,d)->c) (fun b c->
                (fun (((b,_),_),_)->b.C_ID) (fun c->c.C_GHS) (fun b c->
                b,
                match c|>pseq  with
                | x when length x=0 ->0M //可使用 decimal 0 进行测试 
                //| x->x|>headOrDefault|>snd) //与前面的groupBy配合，Right
                | x->x|>sumByDecimal (fun c->c.C_QFJE))
        |>map (fun ((((a,lx),lxr),dq),yfje)->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=lx,
                VC_LXR=lxr,
                VC_DQ=dq,
                VC_YFJE=yfje)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)

//GHS
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun b c ->
            b,
            match c|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->""
            | x ->x.C_XM) 
        |>fun a->
            join a (sb.T_DQ|>pseq) (fun ((b,_),_)->b.C_DQ) (fun c->c.C_ID) (fun b c->b, c.C_MC)
        |>fun a->
            groupJoin a (sb.T_QFMX_GHS
            |>pseq
            //|>groupByO (fun b->b.C_GHS)  //使用 Seq.groupBy,Right
            //|>map (fun (b,c)->b,c|>pseq|>sumByDecimal (fun b->b.C_QFJE))) //Right
            |>groupBy (fun b->b.C_GHS) //使用ParallelEnumerable.Sum,  Right
            |>map (fun b->b.Key,b|>pseq |>sumByDecimal (fun c->c.C_QFJE))) //Right
                (fun (((b,_),_),_)->b.C_ID) (fun (c,d)->c) (fun b c->
                b,
                match c|>pseq  with
                | x when length x=0 ->0M //可使用 decimal 0 进行测试 
                | x->x|>headOrDefault|>snd)
        |>map (fun ((((a,lx),lxr),dq),yfje)->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=lx,
                VC_LXR=lxr,
                VC_DQ=dq,
                VC_YFJE=yfje)
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)



//GHS
let GetGHS=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR|>pseq) (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun (b,c) d ->
            b,c,
            match d|>pseq|>headxOrDefault (fun d->int d.C_FZYW=0) with 
            | null ->new Guid(),null 
            | x ->x.C_ID,x.C_XM)
        |>map (fun (a,b,(c,d))->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=b,
                VC_LXR=d)
            entity.C_DQ<-a.T_DQ.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS)

//第二种方案，不好
let GetGHS01=
      let queryEntity=new BQ_GHS_Advance()
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_GHS.Include(FTN.T_DQ).Include(FTN.T_DWBM)
        |>pseq
        |>filter (fun a->
            match a.C_LXDH,queryEntity.C_LXDH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_MCJM,queryEntity.C_MCJM with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true)
        |>fun a->join a (sb.T_GHSLX|>pseq) (fun b->b.C_LX) (fun c->c.C_DM) (fun b c->b,c.C_LX)
        |>fun a ->
            groupJoin a (sb.T_GHSFZR
            |>pseq
            |>filter (fun b->int b.C_FZYW=0) //使用默认业务负责人
            |>distinctBy (fun b->b.C_GHS)) //去除冗余的业务负责人，或者在数据库中约束每个供应商只对应一个默认负责人
                (fun (b,_)->b.C_ID) (fun c->c.C_GHS)  (fun (b,c) d ->
            b,c,
            match d|>pseq|>headOrDefault with
            | null ->new Guid(),null 
            | x ->x.C_ID,x.C_XM)
        |>map (fun (a,b,(c,d))->
            let entity=
              BD_T_GHS_Advance
                (C_ID=a.C_ID,
                C_MC=a.C_MC,
                VC_LX=b,
                VC_LXR=d)
            entity.C_DQ<-a.T_DQ.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity)
        |>toNetList 
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_GHS_Advance>()

ObjectDumper.Write(GetGHS01)

////////////////////////////////////////////////////////////////////////////////////////////////////

let queryEntity=BQ_YG()
let result=GetYGs queryEntity
ObjectDumper.Write result

////////////////////////////////////////////////////////////////////////////////////////////////////

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let query=(DA_DJ_GHS.INS:>IDA_DJ_GHS).GetDJ_GHSs queryEntity
ObjectDumper.Write( query,1)
query.[0].C_ID<-Guid("153BC105-1E6C-4113-9AB8-1D4E181809FA")
let businessEntity=query.[0]
businessEntity.C_SHR<-Nullable<Guid>()
businessEntity.C_BZ<-"Main2009-11-08Updated"
businessEntity.BD_T_DJSP_GHSs.[0].C_BZ<-"2009-11-08Updated"

businessEntity.C_SHR<-Nullable<Guid>(Guid("154f5e0f-63d4-4d25-9093-50a7ae929c5c"))

(DA_DJ_GHS.INS:>IDA_DJ_GHS).CreateDJ_GHS  businessEntity

(DA_DJ_GHS.INS:>IDA_DJ_GHS).UpdateDJ_GHS  businessEntity

(DA_DJ_GHS.INS:>IDA_DJ_GHS).DeleteDJ_GHS  businessEntity

(DA_DJ_GHS.INS:>IDA_DJ_GHS).DeleteDJ_GHSs  [|businessEntity|]

///////////////////////////////////////////////////////////////////////////////////////

let DeleteDJ_GHSs (businessEntities:BD_T_DJ_GHS[])=
      use sb=new SBIIMSEntitiesAdvance()
      try
        for businessEntity in businessEntities do
          match
            ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
            |>sb.CreateEntityKey
            |>fun a ->
                let b=ref Unchecked.defaultof<_>
                sb.TryGetObjectByKey(a,b), !b with
          | false,_ -> failwith "One of records is not exist!"
          | _,x ->
              x
              |>unbox<T_DJ_GHS>
              |>fun x -> 
                  //x.T_DJSP_GHS.Load()， 默认情况下已经加载，再次加载则是起到刷新的作用
                  x.T_DJSP_GHS.ToArray() //此种情况下x.T_DJSP_GHS的项也会随sb的Delete操作而删除, 此时迭代操作的数据源是一个新的数组实例，因而不受迭代删除操作的影响
                  |>Seq.iter (fun a->sb.DeleteObject(a))
                  ObjectDumper.Write(x.T_DJSP_GHS.Count) //结果为0
                  (*Wrong!!!, sb每执行一次Delete操作， x.T_DJSP_GHS中的相应的记录将会被删除
                  x.T_DJSP_GHS
                  |>Seq.iter (fun a->sb.DeleteObject(a) )
                  *)
                  (*
                  //Right
                  let count=x.T_DJSP_GHS.Count
                  for a=0 to count-1 do
                    sb.DeleteObject(x.T_DJSP_GHS.First())
                  *)
                  x
              |>sb.DeleteObject
        sb.SaveChanges()
      with
      | e -> ObjectDumper.Write(e,3); -1

let DeleteDJ_GHS (businessEntity:BD_T_DJ_GHS)=
      use sb=new SBIIMSEntitiesAdvance()
      try
        match
          ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
          |>sb.CreateEntityKey
          |>fun a ->
              let b=ref Unchecked.defaultof<_>
              sb.TryGetObjectByKey(a,b), !b with
        | false,_ -> failwith "The record is not exist!"
        | _,x ->
            x
            |>unbox<T_DJ_GHS>
            |>fun x -> 
                //x.T_DJSP_GHS.Load()， 默认情况下已经加载，再次加载则是起到刷新的作用
                x.T_DJSP_GHS.ToArray() //此种情况下x.T_DJSP_GHS的项也会随sb的Delete操作而删除, 此时迭代操作的数据源是一个新的数组实例，因而不受迭代删除操作的影响
                |>Seq.iter (fun a->sb.DeleteObject(a))
                ObjectDumper.Write(x.T_DJSP_GHS.Count) //结果为0
                (*Wrong!!!, sb每执行一次Delete操作， x.T_DJSP_GHS中的相应的记录将会被删除
                x.T_DJSP_GHS
                |>Seq.iter (fun a->sb.DeleteObject(a) )
                *)
                (*
                //Right
                let count=x.T_DJSP_GHS.Count
                for a=0 to count-1 do
                  sb.DeleteObject(x.T_DJSP_GHS.First())
                *)
                x
            |>sb.DeleteObject
            sb.SaveChanges()
      with
      | e -> ObjectDumper.Write(e,3); -1

DeleteDJ_GHS businessEntity


(DA_DJ_GHS.INS:>IDA_DJ_GHS).DeleteDJ_GHS  businessEntity

let GetDJ_GHSs (queryEntity:BQ_DJ_GHS)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_DJ_GHS.Include(FTN.T_CK).Include(FTN.T_CK1).Include(FTN.T_DWBM).Include(FTN.T_DWBM1).Include(FTN.T_GHS).Include(FTN.T_YG).Include(FTN.T_YG1).Include(FTN.T_YG2)
        |>pseq
        |>filter (fun a->
            match a.C_BZ,queryEntity.C_BZ with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_CJRQ,queryEntity.C_CJRQ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_CKJE,queryEntity.C_CKJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_CKSL,queryEntity.C_CKSL with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DJH,queryEntity.C_DJH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_DJLX,queryEntity.C_DJLX with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DJZQJE,queryEntity.C_DJZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DJZT,queryEntity.C_DJZT with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_DYBZ,queryEntity.C_DYBZ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_FKD,queryEntity.C_FKD with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_ID,queryEntity.C_ID with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_RKJE,queryEntity.C_RKJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_RKSL,queryEntity.C_RKSL with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_SZQJE,queryEntity.C_SZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_THBZ,queryEntity.C_THBZ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_YHJE,queryEntity.C_YHJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.C_YSDJH,queryEntity.C_YSDJH with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && 
            match a.C_YZQJE,queryEntity.C_YZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && 
            match a.T_CK,queryEntity.C_CCK with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_CK1,queryEntity.C_RCK with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_DWBM,queryEntity.C_FBID with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_DWBM1,queryEntity.C_WFDW with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_GHS,queryEntity.C_GHS with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_YG,queryEntity.C_CZY with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_YG1,queryEntity.C_JBR with
            | x,y when y.HasValue ->x.C_ID =y.Value
            | _ ->true
            && 
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              BD_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID)
            | _ ->()
            entity.BD_T_DJSP_GHSs.AddRange(
              a.T_DJSP_GHS.CreateSourceQuery().Include(FTN.T_DWBM).Include(FTN.T_SP)
              |>pseq 
              |>map (fun b->
                  let childEntity=
                    BD_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  childEntity.C_FBID<-b.T_DWBM.C_ID
                  childEntity.C_SP<-b.T_SP.C_ID
                  childEntity)
              |>toNetList)
            entity)
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),"General"); List<BD_T_DJ_GHS>()

let businessEntities= GetDJ_GHSs queryEntity
ObjectDumper.Write(businessEntities ,1)

////////////////////////////////////////////////////////////////////////////


let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let GetDJ_GHSs2 (queryEntity:BQ_DJ_GHS)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.T_DJ_GHS.Include("T_CK").Include("T_CK1").Include("T_DWBM").Include("T_GHS").Include("T_YG").Include("T_YG1").Include("T_YG2")
        |>pseq
        |>filter (fun a->
            match queryEntity.C_JBR with
            | x when x.HasValue ->
                a.T_YG1Reference.Load() 
                a.T_YG1.C_ID=x.Value
            | _ ->true
            )
        |>map (fun a->
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            match a.T_YG2 with
            | x when x<>null ->entity.C_SHR <- Nullable<System.Guid>(x.C_ID) | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS.CreateSourceQuery().Include("T_SP").Include("T_DWBM")
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  //b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                 // b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  detailEntity)
              |>toNetList
            entity)
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,2); Logger.Write(e.ToString(),"General"); List<B_T_DJ_GHS>()


let businessEntities= GetDJ_GHSs2 queryEntity
ObjectDumper.Write(businessEntities ,1)

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))


let GetDJ_GHSs3 (queryEntity:BQ_DJ_GHS)=
      use sb=new SBIIMSEntitiesAdvance()
      try
        sb.T_DJ_GHS
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG1Reference.Load() 
                a.T_YG1.C_ID=queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                a.T_YG2Reference.Load() 
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                        x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
            a.T_DWBM1Reference.Load()
            a.T_DWBMReference.Load()
            a.T_GHSReference.Load()
            a.T_YG1Reference.Load()
            a.T_YGReference.Load()
            a.T_DJSP_GHS.Load()
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE,
                C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID))
            a.T_CK1Reference.Load()
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            //Modified
            a.T_YG2Reference.Load()
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<Guid>(x.C_ID)
            | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_SP=b.C_SP,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                  detailEntity)
              |>toNetList
            entity
            )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,3); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS>()

let businessEntities= GetDJ_GHSs3 queryEntity
ObjectDumper.Write(businessEntities ,1)

////////////////////////////////////////////////////////////////////////////////////////////////////
//Right Reference
let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

//加载方法sb.ContextOptions.LazyLoadingEnabled<-true, a.T_CKReference.Load(),.Include(...)都可以，且只有Include(...)支持Parallel
//由于需要使用Parallel方式，所以只能采用InClude方式，且将sb.ContextOptions.LazyLoadingEnabled<-false， 其默认为True
let GetDJ_GHSs1 (queryEntity:BQ_DJ_GHS)=
        use sb=new SBIIMSEntitiesAdvance()
        //sb.ContextOptions.LazyLoadingEnabled<-true //默认已经为True， 特别的LazyLoading不支持Parallel？？？
        sb.ContextOptions.LazyLoadingEnabled<-false
        sb.T_DJ_GHS.Include("T_CK").Include("T_CK1").Include("T_DWBM").Include("T_GHS").Include("T_YG").Include("T_YG1").Include("T_YG2") //Right, 且Include方式支持Parallel方式
        |>pseq  //LazyLoading不支持Parallel？？？
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
        |>map (fun a-> //LazyLoading不支持Parallel？？？
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE)
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
        |>toNetList  
        
let businessEntities= GetDJ_GHSs1 queryEntity
ObjectDumper.Write(businessEntities ,1)

//////////////////////////////////////////////////////////////////////////////////////////////////////
//Right,  直接使用外键字段，只有新版ADO.NET 才支持

let queryEntity=BQ_DJ_GHS()
queryEntity.C_JBR <-Nullable<Guid>(Guid("bf7520a6-84c6-4c15-9d23-d87e188ff0fe"))
queryEntity.C_ID <-Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a"))

let GetDJ_GHSs1 (queryEntity:BQ_DJ_GHS)=
        use sb=new SBIIMSEntitiesAdvance()
        //sb.ContextOptions.LazyLoadingEnabled<-true
        sb.T_DJ_GHS//.Include("T_CK").Include("T_DWBM").Include("T_GHS").Include("T_YG")
        |>pseq
        |>filter (fun a->
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
                 //a.T_YG1.C_ID =queryEntity.C_JBR .Value
                 a.C_JBR =queryEntity.C_JBR .Value
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
        |>map (fun a->
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.C_CCK
            entity.C_RCK<-a.C_RCK
            entity.C_FBID<-a.C_FBID
            entity.C_WFDW<-a.C_WFDW
            entity.C_GHS<-a.C_GHS
            entity.C_CZY<-a.C_CZY
            entity.C_JBR<-a.C_JBR
            entity
            )
        |>toNetList  

let businessEntities= GetDJ_GHSs1 queryEntity
ObjectDumper.Write(businessEntities ,1)

//////////////////////////////////////////////////////////////////////////////////////////////////////

let GetDJ_GHSs (queryEntity:BQ_DJ_GHS)=
      use sb=new SBIIMSEntitiesAdvance()
      sb.ContextOptions.LazyLoadingEnabled<-true
      try
        sb.T_DJ_GHS
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                //a.T_YG1Reference.Load() 
                a.T_YG1.C_ID =queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                //a.T_YG2Reference.Load() 
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                        x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
//            if not a.T_DWBM1Reference.IsLoaded then
//              a.T_DWBM1Reference.Load()
//            if not a.T_DWBM1Reference.IsLoaded then
//              a.T_DWBMReference.Load()
//            if not a.T_GHSReference.IsLoaded then
//              a.T_GHSReference.Load()
//            if not a.T_YG1Reference.IsLoaded then
//              a.T_YG1Reference.Load()
//            if not a.T_YGReference.IsLoaded then
//              a.T_YGReference.Load()
//            if not a.T_DJSP_GHS.IsLoaded then
//              a.T_DJSP_GHS.Load()
//            if not a.T_CK1Reference.IsLoaded then
//              a.T_CK1Reference.Load()
//            if not a.T_CKReference.IsLoaded then
//              a.T_CKReference.Load()
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE)
            entity.C_CCK<-a.T_CK.C_ID
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            //Modified
            a.T_YG2Reference.Load()
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<Guid>(x.C_ID)
            | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_SP=b.C_SP,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  //b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  //b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                  detailEntity)
              |>toNetList
            entity
            )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,3); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS>()

let businessEntities= GetDJ_GHSs queryEntity
ObjectDumper.Write(businessEntities ,1)


let sb=new SBIIMSEntitiesAdvance()
let result=
  sb.T_DJ_GHS
  |>pseq
  |>filter (fun a->a.T_YGReference.Load(); a.T_YG.C_ID=queryEntity.C_CZY.Value)
  |>map (fun a->a.T_YG.C_XM )

sb.Dispose()


let query =
      let queryEntity=BQ_DJ_GHS()
      queryEntity.C_CZY<-Nullable<Guid>(Guid("797f2b9f-3f41-4b31-90a9-bcd2a8c281a3"))
      use sb=new SBIIMSEntitiesAdvance()
      try
        sb.T_DJ_GHS
        |>pseq
        |>filter (fun a->
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&   
              if queryEntity.C_CZY.HasValue  then
                a.T_YG1Reference.Load() 
                a.T_YG1.C_ID=queryEntity.C_CZY.Value
              else
                true 
            &&   
              if queryEntity.C_SHR.HasValue  then
                a.T_YG2Reference.Load() 
                match a.T_YG2, queryEntity.C_SHR with
                | x,y when x<>null -> 
                        x.C_ID =y.Value
                | _ ->true
              else
                true 
              )
        |>map (fun a->
            a.T_DWBM1Reference.Load()
            a.T_DWBMReference.Load()
            a.T_GHSReference.Load()
            a.T_YG1Reference.Load()
            a.T_YGReference.Load()
            a.T_DJSP_GHS.Load()
            let entity=
              B_T_DJ_GHS
                (C_BZ=a.C_BZ,
                C_CJRQ=a.C_CJRQ,
                C_CKJE=a.C_CKJE,
                C_CKSL=a.C_CKSL,
                C_DJH=a.C_DJH,
                C_DJLX=a.C_DJLX,
                C_DJZQJE=a.C_DJZQJE,
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
                C_YZQJE=a.C_YZQJE,
                C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID))
            a.T_CK1Reference.Load()
            entity.C_RCK<-a.T_CK1.C_ID
            entity.C_FBID<-a.T_DWBM.C_ID
            entity.C_WFDW<-a.T_DWBM1.C_ID
            entity.C_GHS<-a.T_GHS.C_ID
            entity.C_CZY<-a.T_YG.C_ID
            entity.C_JBR<-a.T_YG1.C_ID
            //Modified
            a.T_YG2Reference.Load()
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<Guid>(x.C_ID)
            | _ ->()
            entity.B_T_DJSP_GHSs<-
              a.T_DJSP_GHS
              |>pseq 
              |>map (fun b->
                  let detailEntity=
                    B_T_DJSP_GHS
                      (BZ=b.BZ,
                      C_BZQ=b.C_BZQ,
                      C_DJ=b.C_DJ,
                      C_DJID=b.C_DJID,
                      C_PC=b.C_PC,
                      C_SCRQ=b.C_SCRQ,
                      C_SL=b.C_SL,
                      C_SP=b.C_SP,
                      C_TM=b.C_TM,
                      C_XH=b.C_XH,
                      C_ZHJ=b.C_ZHJ,
                      C_ZHJE=b.C_ZHJE,
                      C_ZJE=b.C_ZJE,
                      C_ZKL=b.C_ZKL)
                  b.T_SPReference.Load()
                  detailEntity.C_SP<-b.T_SP.C_ID
                  b.T_DWBMReference.Load()
                  detailEntity.C_FBID<-b.T_DWBM.C_ID
                  detailEntity)
              |>toNetList
            entity
            )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e,3); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS>()

///////////////////////////////////////////////////////////////////////////////////////////////////

(DA_DJ_GHS.INS:>IDA_DJ_GHS)
let da=
  DA_DJ_GHS.INS |>unbox<IDA_DJ_GHS>
  
let queryEntity=BQ_DJ_GHS()
queryEntity.C_ID<- Nullable<Guid>(Guid("83706028-2459-4c6f-9e13-2b9ab85e7470"))
let businessEntities=da.GetDJ_GHSs queryEntity
ObjectDumper.Write(businessEntities,1)
ObjectDumper.Write(businessEntities.[0].B_T_DJSP_GHSs)

let creatEntity=businessEntities.[0]
creatEntity.C_ID<- Guid("7F09563C-F027-45b3-BCFC-2C454BC1F567")
for a in creatEntity.B_T_DJSP_GHSs do
  a.C_DJID <- Guid("7F09563C-F027-45b3-BCFC-2C454BC1F567")

ObjectDumper.Write(creatEntity,1)

try
  da.CreateDJ_GHS creatEntity
with 
| e -> ObjectDumper.Write(e);-1



  

///////////////////////////////////////////////////////////////////////////////////////////////////
//For Read
let GetDJ_DJSP_GHSs(queryEntity:BQ_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    sb.T_DJ_GHS
    |>pseq
    |>filter (fun a->
        match a.C_ID,queryEntity.C_ID with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
        &&
        match a.C_GXRQ,queryEntity.C_GXRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
          )
    |>map (fun a->
           a.T_DWBM1Reference.Load()
           a.T_DWBMReference.Load()
           a.T_GHSReference.Load()
           a.T_YG1Reference.Load()
           a.T_YG2Reference.Load()
           a.T_YGReference.Load()
           a.T_DJSP_GHS.Load()
           let entity=
             B_T_DJ_GHS_Advance
               (C_BZ=a.C_BZ,
               C_CJRQ=a.C_CJRQ,
               C_CKJE=a.C_CKJE,
               C_CKSL=a.C_CKSL,
               C_DJH=a.C_DJH,
               C_DJLX=a.C_DJLX,
               C_DJZQJE=a.C_DJZQJE,
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
               C_YZQJE=a.C_YZQJE,
               C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID))
           a.T_CK1Reference.Load()
           entity.C_RCK<-a.T_CK1.C_ID
           entity.C_FBID<-a.T_DWBM.C_ID
           entity.C_WFDW<-a.T_DWBM1.C_ID
           entity.C_GHS<-a.T_GHS.C_ID
           entity.C_CZY<-a.T_YG.C_ID
           entity.C_JBR<-a.T_YG1.C_ID
           entity.C_SHR <-Nullable<Guid>( a.T_YG2.C_ID)
           entity.B_T_DJSP_GHSs<-
             a.T_DJSP_GHS
             |>pseq 
             |>map (fun b->
                    let detailEntity=
                      B_T_DJSP_GHS_Advance
                        (BZ=b.BZ,
                        C_BZQ=b.C_BZQ,
                        C_DJ=b.C_DJ,
                        C_DJID=b.C_DJID,
                        C_PC=b.C_PC,
                        C_SCRQ=b.C_SCRQ,
                        C_SL=b.C_SL,
                        C_SP=b.C_SP,
                        C_TM=b.C_TM,
                        C_XH=b.C_XH,
                        C_ZHJ=b.C_ZHJ,
                        C_ZHJE=b.C_ZHJE,
                        C_ZJE=b.C_ZJE,
                        C_ZKL=b.C_ZKL)
                    b.T_SPReference.Load()
                    detailEntity.C_SP<-b.T_SP.C_ID
                    b.T_DWBMReference.Load()
                    detailEntity.C_FBID<-b.T_DWBM.C_ID
                    detailEntity)
             |>toNetList
           entity
           )
    |>toNetList  
  with 
  | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),""); List<B_T_DJ_GHS_Advance>()


//Excute query
let queryEntity=BQ_DJ_GHS_Advance(C_ID=Nullable<Guid>(Guid("e50a6378-d313-4e34-8020-7ce88944810a")))
let mainDetailEntity=GetDJ_DJSP_GHSs queryEntity
ObjectDumper.Write (mainDetailEntity,2)
mainDetailEntity.Count
mainDetailEntity.[0].B_T_DJSP_GHSs
ObjectDumper.Write (mainDetailEntity.[0].B_T_DJSP_GHSs,0)


//For Create
let CreateT_CK (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try 
    let t_DJ_GHS=
      T_DJ_GHS(
        C_ID=Guid.NewGuid(),
        C_BZ="wxtest",//businessEntity.C_BZ,
        C_CJRQ=DateTime.Now,
        C_CKJE=businessEntity.C_CKJE,
        C_CKSL=businessEntity.C_CKSL,
        C_DJH=businessEntity.C_DJH,
        C_DJLX=businessEntity.C_DJLX,
        C_DJZQJE=businessEntity.C_DJZQJE,
        C_DJZT=businessEntity.C_DJZT,
        C_DYBZ=businessEntity.C_DYBZ,
        C_FKD=businessEntity.C_FKD,
        C_GXRQ=DateTime.Now,
        C_RKJE=businessEntity.C_RKJE,
        C_RKSL=businessEntity.C_RKSL,
        C_SZQJE=businessEntity.C_SZQJE,
        C_THBZ=businessEntity.C_THBZ,
        C_YHJE=businessEntity.C_YHJE,
        C_YSDJH=businessEntity.C_YSDJH,
        C_YZQJE=businessEntity.C_YZQJE)
    t_DJ_GHS.T_CK<- 
      let entityKey=sb.CreateEntityKey("T_CK",T_CK(C_ID=businessEntity.C_CCK))
      sb.GetObjectByKey(entityKey):?>T_CK

      (* Right, not good
      sb.T_CK
      |>pseq
      |>filter (fun a->a.C_ID=businessEntity.C_CCK)
      |>head
      *)  
        
      (*// Right, not good
      ((("T_CK",T_CK(C_ID=businessEntity.C_CCK))
      |>sb.CreateEntityKey 
      |>sb.GetObjectByKey)
      :?>T_CK)
      *)
    t_DJ_GHS.T_CK1<-
      ((sb.CreateEntityKey("T_CK",T_CK(C_ID=businessEntity.C_RCK))
      |>sb.GetObjectByKey):?>T_CK)
    t_DJ_GHS.T_DWBM<-
       ("T_DWBM",T_DWBM(C_ID=businessEntity.C_FBID))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>fun x-> x:?>T_DWBM
    t_DJ_GHS.T_DWBM1<-
       ("T_DWBM",T_DWBM(C_ID=businessEntity.C_WFDW))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>fun x->x:?>T_DWBM
    t_DJ_GHS.T_GHS<-
      (* Right
       //let  original:obj ref=ref null //Right
       let  original=ref Unchecked.defaultof<obj>
       let entityKey=sb.CreateEntityKey ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
       if sb.TryGetObjectByKey(entityKey,original)  then
          !original :?>T_GHS
       else null
       *)
       //let  original:obj ref=ref null //Right
       let original=ref Unchecked.defaultof<obj>
       ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
       |>sb.CreateEntityKey
       |>fun x->if sb.TryGetObjectByKey(x,original) then !original else null
       |>unbox<T_GHS>
       (* Right
       ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>unbox<T_GHS>
       *)
    t_DJ_GHS.T_YG <-
       ("T_YG",T_YG(C_ID=businessEntity.C_CZY))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>unbox<T_YG>
    t_DJ_GHS.T_YG1 <-
       ("T_YG",T_YG(C_ID=businessEntity.C_JBR))
       |>sb.CreateEntityKey
       |>sb.GetObjectByKey
       |>unbox<T_YG>
    match businessEntity.C_SHR with
    | a when a.HasValue->
        t_DJ_GHS.T_YG1 <-
          ("T_YG",T_YG(C_ID=a.Value))
          |>sb.CreateEntityKey
          |>sb.GetObjectByKey
          |>unbox<T_YG>
    | _ ->()
    
    for detailEntity in businessEntity.B_T_DJSP_GHSs do
      let t_DJSP_GHS=
        T_DJSP_GHS
          (BZ=detailEntity.BZ,
          C_BZQ=detailEntity.C_BZQ,
          C_DJ=detailEntity.C_DJ,
          C_DJID=detailEntity.C_DJID,
          C_PC=detailEntity.C_PC,
          C_SCRQ=detailEntity.C_SCRQ,
          C_SL=detailEntity.C_SL,
          C_SP=detailEntity.C_SP,
          C_TM=detailEntity.C_TM,
          C_XH=detailEntity.C_XH,
          C_ZHJ=detailEntity.C_ZHJ,
          C_ZHJE=detailEntity.C_ZHJE,
          C_ZJE=detailEntity.C_ZJE,
          C_ZKL=detailEntity.C_ZKL)
      t_DJSP_GHS.T_DWBM<-
        ("T_DWBM",T_DWBM(C_ID=detailEntity.C_FBID))
        |>sb.CreateEntityKey
        |>sb.GetObjectByKey
        |>unbox<T_DWBM>
      t_DJ_GHS.T_DJSP_GHS.Add(t_DJSP_GHS)
    sb.AddToT_DJ_GHS(t_DJ_GHS)
    sb.SaveChanges()
  with
  | e ->ObjectDumper.Write(e,0);-1
  

//mainDetailEntity.[0].C_GHS<-Guid.NewGuid()    
let create=CreateT_CK mainDetailEntity.[0]


//For Update
let UpdateT_DJ_DJSP01 (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    let original=
      ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_DJ_GHS>
    original.C_BZ<-"================"
    original.T_YG<-
      ("T_YG",T_YG(C_ID=businessEntity.C_CZY))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_YG>
    sb.ApplyPropertyChanges("T_DJ_GHS",original) 
    sb.SaveChanges()
  with
  | e ->ObjectDumper.Write(e);-1
  
  
let UpdateT_DJ_DJSP (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    let original=
      ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_DJ_GHS>
    original.C_BZ<-"wx11111111111111111111111"
    original.C_CJRQ<-businessEntity.C_CJRQ
    original.C_CKJE<-businessEntity.C_CKJE
    original.C_CKSL<-businessEntity.C_CKSL
    original.C_DJH<-businessEntity.C_DJH
    original.C_DJLX<-businessEntity.C_DJLX
    original.C_DJZQJE<-businessEntity.C_DJZQJE
    original.C_DJZT<-businessEntity.C_DJZT
    original.C_DYBZ<-businessEntity.C_DYBZ
    original.C_FKD<-businessEntity.C_FKD
    original.C_GXRQ<-DateTime.Now
    original.C_RKJE<-businessEntity.C_RKJE
    original.C_RKSL<-businessEntity.C_RKSL
    original.C_SZQJE<-businessEntity.C_SZQJE
    original.C_THBZ<-businessEntity.C_THBZ
    original.C_YHJE<-businessEntity.C_YHJE
    original.C_YSDJH<-businessEntity.C_YSDJH
    original.C_YZQJE<-businessEntity.C_YZQJE

//    let t_DJ_GHS=
//      T_DJ_GHS(
//        C_ID=businessEntity.C_ID,
//        C_BZ="wxtest=================="//businessEntity.C_BZ,
//        C_CJRQ=DateTime.Now,
//        C_CKJE=businessEntity.C_CKJE,
//        C_CKSL=businessEntity.C_CKSL,
//        C_DJH=businessEntity.C_DJH,
//        C_DJLX=businessEntity.C_DJLX,
//        C_DJZQJE=businessEntity.C_DJZQJE,
//        C_DJZT=businessEntity.C_DJZT,
//        C_DYBZ=businessEntity.C_DYBZ,
//        C_FKD=businessEntity.C_FKD,
//        C_GXRQ=DateTime.Now,
//        C_RKJE=businessEntity.C_RKJE,
//        C_RKSL=businessEntity.C_RKSL,
//        C_SZQJE=businessEntity.C_SZQJE,
//        C_THBZ=businessEntity.C_THBZ,
//        C_YHJE=businessEntity.C_YHJE,
//        C_YSDJH=businessEntity.C_YSDJH,
//        C_YZQJE=businessEntity.C_YZQJE)
    original.T_CK<- 
      ("T_CK",T_CK(C_ID=businessEntity.C_CCK))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_CK>
    original.T_CK1<-
      ("T_CK",T_CK(C_ID=businessEntity.C_RCK))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_CK>
    original.T_DWBM<-
      ("T_DWBM",T_DWBM(C_ID=businessEntity.C_FBID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>fun x-> x:?>T_DWBM
    original.T_DWBM1<-
      ("T_DWBM",T_DWBM(C_ID=businessEntity.C_WFDW))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>fun x->x:?>T_DWBM
    original.T_GHS<-
      let original=ref Unchecked.defaultof<obj>
      ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
      |>sb.CreateEntityKey
      |>fun x->if sb.TryGetObjectByKey(x,original) then !original else null
      |>unbox<T_GHS>
    original.T_YG <-
      ("T_YG",T_YG(C_ID=businessEntity.C_CZY))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_YG>
    original.T_YG1 <-
      ("T_YG",T_YG(C_ID=businessEntity.C_JBR))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_YG>
    sb.ApplyPropertyChanges("T_DJ_GHS",original) 
    
    //对于子表记录来说,如果商业实体中有子表记录,先删现有子表中所有记录,再插入所有商业实体中的子表记录
    //还有一中方法是, 比较现有子表和商业实体中的子表记录,需要新增的才新增,需要删除的才删除,需要更新的才更新,
    //哪一种方法更好???
    if businessEntity.B_T_DJSP_GHSs.Count>0 then
      //Delete all detail records
      original.T_DJSP_GHS.Load()
      for originalDetail in original.T_DJSP_GHS do
        ("T_DJSP_GHS",T_DJSP_GHS(C_DJID=originalDetail.C_DJID,C_SP=originalDetail.C_SP,C_PC=originalDetail.C_PC))
        |>sb.CreateEntityKey
        |>sb.GetObjectByKey
        |>sb.DeleteObject
      //Insert detail records  
      for businessDetail in  businessEntity.B_T_DJSP_GHSs do
        let t_DJSP_GHS=
          T_DJSP_GHS
            (BZ=businessDetail.BZ,
            C_BZQ=businessDetail.C_BZQ,
            C_DJ=businessDetail.C_DJ,
            C_DJID=businessDetail.C_DJID,
            C_PC=businessDetail.C_PC,
            C_SCRQ=businessDetail.C_SCRQ,
            C_SL=businessDetail.C_SL,
            C_SP=businessDetail.C_SP,
            C_TM=businessDetail.C_TM,
            C_XH=businessDetail.C_XH,
            C_ZHJ=businessDetail.C_ZHJ,
            C_ZHJE=businessDetail.C_ZHJE,
            C_ZJE=businessDetail.C_ZJE,
            C_ZKL=businessDetail.C_ZKL)
        t_DJSP_GHS.T_DWBM<-
          ("T_DWBM",T_DWBM(C_ID=businessDetail.C_FBID))
          |>sb.CreateEntityKey
          |>sb.GetObjectByKey
          |>unbox<T_DWBM>
        original.T_DJSP_GHS.Add(t_DJSP_GHS)
    sb.SaveChanges()
  with
  | e ->ObjectDumper.Write(e);-1
  
  
let businessEntity=B_T_DJ_GHS_Advance(C_ID=Guid("83706028-2459-4c6f-9e13-2b9ab85e7470"))
businessEntity.C_CZY<-Guid("154f5e0f-63d4-4d25-9093-50a7ae929c5c")
UpdateT_DJ_DJSP01 businessEntity

UpdateT_DJ_DJSP businessEntity
  
let original=
  use sb=new SBIIMSEntitiesAdvance()
  ("T_DJ_GHS",T_DJ_GHS(C_ID=Guid("83706028-2459-4c6f-9e13-2b9ab85e7470")))
  |>sb.CreateEntityKey
  |>sb.GetObjectByKey
  |>unbox<T_DJ_GHS>


//----------------------------------

//For Delete
let DeleteT_DJ_DJSP (businessEntity:B_T_DJ_GHS_Advance)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
    |>sb.CreateEntityKey
    |>sb.GetObjectByKey
    |>unbox<T_DJ_GHS>
    |>fun x -> x.T_DJSP_GHS.Load();x
    |>sb.DeleteObject
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e); -1
let businessEntityForDelete=B_T_DJ_GHS_Advance(C_ID=Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0"))
businessEntity.C_CZY<-Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0")
DeleteT_DJ_DJSP businessEntityForDelete


//For Delete  multiple records
let DeleteT_DJ_DJSPs (businessEntitys:B_T_DJ_GHS_Advance [])=
  use sb=new SBIIMSEntitiesAdvance()
  try
    for businessEntity in businessEntitys do
      ("T_DJ_GHS",T_DJ_GHS(C_ID=businessEntity.C_ID))
      |>sb.CreateEntityKey
      |>sb.GetObjectByKey
      |>unbox<T_DJ_GHS>
      |>fun x -> x.T_DJSP_GHS.Load();x
      |>sb.DeleteObject
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e);-1
  
  
  
  
let businessEntityForDeleteMulti=B_T_DJ_GHS_Advance(C_ID=Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0"))
businessEntity.C_CZY<-Guid("eb8bdb2b-458a-4ca7-9e54-42045fad58e0")
DeleteT_DJ_DJSP businessEntityForDeleteMulti

/////////////////////

let  original=Unchecked.defaultof<T_GHS>
let  original1=null
let  original2:Object ref=ref null
let  original3:T_GHS ref=ref null
//let entityKey=sb.CreateEntityKey ("T_GHS",T_GHS(C_ID=businessEntity.C_GHS))
//if sb.TryGetObjectByKey(entityKey,original)  then
//   !original :?>T_GHS
//else null


let entity =box (T_GHS())
unbox<T_GHS> null
/////////////////////////////////////////////////

(*
//For Update ???
let UpdateT_CK01 (entity:T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    let entitySetName = entity.EntityKey.EntitySetName
    sb.T_CK.Context.ApplyPropertyChanges(entitySetName,entity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1


let UpdateT_CK02 (sourceEntity:B_T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  //let forUpdateEntity=
  let entityKey = sb.CreateEntityKey("T_CK",T_CK(C_ID=sourceEntity.C_ID))
  let orginalEntity = sb.GetObjectByKey(entityKey) :?> T_CK
  orginalEntity.C_CJRQ<-sourceEntity.C_CJRQ
  //...需要更新每一个成员
  try
    let entitySetName = orginalEntity.EntityKey.EntitySetName
    sb.T_CK.Context.ApplyPropertyChanges(entitySetName,orginalEntity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1

//For Delete
let DeleteT_CK01 (entity:T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  try
    //sb.DeleteObject(entity)
    sb.T_CK.Context.DeleteObject(entity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1
  
let DeleteT_CK02 (queryEntity:BQ_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  let deleteEntity=
    sb.T_CK
    |>pseq
    |>filter (fun a->
        match a.C_CJRQ,queryEntity.C_CJRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
        &&
        match a.C_GXRQ,queryEntity.C_GXRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
          )
     |>head
  try
    sb.DeleteObject(deleteEntity)
    sb.SaveChanges()
  with
  | e -> ObjectDumper.Write(e,1);-1

//For Query
let queryT_CK01 (queryEntity:BQ_CK)=
  //let x=(DA_T_CK.INS:>IDA_T_CK).GetT_CKs()
    use sb=new SBIIMSEntitiesAdvance()
    sb.T_CK
    |>pseq
    |>filter (fun a->
         match queryEntity with
         | x when x=Unchecked.defaultof<_> ->true
         | x  ->
            match a.C_CJRQ,x.C_CJRQ with
            | ax,y when y.HasValue ->ax=y.Value
            | _ ->true
            &&
            match a.C_GXRQ,x.C_GXRQ with
            | ax,y when y.HasValue ->ax=y.Value
            | _ ->true
          )
    |>toList  
    
let queryT_CK02(queryEntity:BQ_CK)=
    use sb=new SBIIMSEntitiesAdvance()
    sb.T_CK
    |>pseq
    |>filter (fun a->
        match a.C_CJRQ,queryEntity.C_CJRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
        &&
        match a.C_GXRQ,queryEntity.C_GXRQ with
        | b,c when c.HasValue ->b=c.Value
        | _ ->true
          )
    |>toList  

let queryT_CK03 (queryEntity:BQ_CK)=
    use sb=new SBIIMSEntitiesAdvance()
    sb.T_CK
    |>pseq
    |>filter (fun a->
         (if queryEntity.C_CJRQ.HasValue then a.C_CJRQ=queryEntity.C_CJRQ.Value else true) &&
         (if queryEntity.C_CJRQ.HasValue then a.C_GXRQ=queryEntity.C_GXRQ.Value else true )
         )
    |>toList



let x=(DA_T_CK.INS:>IDA_T_CK).GetT_CKs()
ObjectDumper.Write(x,0)
//WpfObjectDumper

//let y=System.Linq.Queryable<T_CK>(
//let y=System.Data.Objects.ObjectQuery

let path=System.AppDomain.CurrentDomain.BaseDirectory

let x=
  use sb=new SBIIMSEntitiesAdvance()
  sb.T_CK
  //|>Seq.toList
  |>pseq
  |>toList

let GetT_DWBM id=
  use sb=new SBIIMSEntitiesAdvance()
  sb.T_DWBM
  |>pseq
  |>filter (fun a->a.C_ID=id)
  |>toList
  
///Add, Rgiht  
let AddT_CK (entity:T_CK)=
  use sb=new SBIIMSEntitiesAdvance()
  //let t_DWBM=GetT_DWBM (Guid("69896fad-c145-4b3a-b122-36df6b6d2c87"))
//  let entity=
//          T_CK(C_BZ = null,
//               C_CJRQ = DateTime.Now,
//               C_CKDZ = "九门里",
//               C_DQ = "001001",
//               C_GXRQ = DateTime.Now,
//               C_ID = Guid.NewGuid(),
//               C_JY = false,
//               C_LXDH = "九门里",
//               C_MC = "九门里1号仓库",
//               C_MR = false )
  let t_DWBM=
    sb.T_DWBM
    |>pseq
    |>filter (fun a->a.C_ID=Guid "69896fad-c145-4b3a-b122-36df6b6d2c87")
    |>head
  let t_YG=
    sb.T_YG
    |>pseq
    |>filter (fun a->a.C_ID=Guid "b3548cf3-e9a1-4dd8-bb07-086de2dcd5f3")
    |>head
    
  entity.T_DWBM<-t_DWBM
  entity.T_YG<- t_YG
  try
      sb.AddToT_CK(entity) //EntityKey如果在"use sb"加载其它参考对象时，已经生成, 只是其值为空，所以，所有加载行为应该在同一个"use sb"中完成
      //sb.T_CK.Context.Attach entity
      ObjectDumper.Write ("wx")
      sb.T_CK.Context.SaveChanges()
   with
   | e ->ObjectDumper.Write(e,1);-1
  
let entity=T_CK.CreateT_CK(DateTime.Now, DateTime.Now,Guid.NewGuid(),"九门里1号仓库","00100100001001","0871","九门里",false,false)
let key=new EntityKey()

let t_DWBM=GetT_DWBM (Guid("69896fad-c145-4b3a-b122-36df6b6d2c87"))

//Wrong, 不能在不同的"use sb=new SBIIMSEntitiesAdvance()"中加载信息
let entity01=
        T_CK(C_BZ = null,
             C_CJRQ = DateTime.Now,
             C_CKDZ = "九门里",
             C_DQ = "001001",
             C_GXRQ = DateTime.Now,
             C_ID = Guid.NewGuid(),
             C_JY = false,
             C_LXDH = "九门里",
             C_MC = "九门里1号仓库",
             C_MR = false,
             T_DWBM=t_DWBM.Head )
             
entity01.EntityKey
entity01.EntityState
             
let entity02=
        T_CK(C_BZ = null,
             C_CJRQ = DateTime.Now,
             C_CKDZ = "九门里",
             C_DQ = "001001",
             C_GXRQ = DateTime.Now,
             C_ID = Guid.NewGuid(),
             C_JY = false,
             C_LXDH = "九门里",
             C_MC = "九门里1号仓库",
             C_MR = false )
AddT_CK entity02
  
  
*)