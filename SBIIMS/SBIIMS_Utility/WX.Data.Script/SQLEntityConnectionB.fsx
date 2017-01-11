

#r "System.Data.Entity"
#r "EntityFramework"
#r "FSharp.Data.TypeProviders"
#r "System.Data.Linq"
//-----------------------------------------------------------------------
open System
open  System.Data.Linq
open System.Data.Entity
open System.Data.Entity
open System.Data.EntityClient
open System.Data.SqlClient
open System.Data.Metadata.Edm
open Microsoft.FSharp.Data.TypeProviders

open Microsoft.FSharp.Linq
open System.Diagnostics
//-----------------------------------------------------------------------
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
#r "Microsoft.Practices.EnterpriseLibrary.Common"
#r "Microsoft.Practices.EnterpriseLibrary.Data"
open Microsoft.Practices.EnterpriseLibrary.Common

//-----------------------------------------------------------------------
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WPFToolkit.Extended.dll"
open  Microsoft.Windows.Controls

(*
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
//#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
//#r "WX.Data.DataModel.JXC"
//#r "WX.Data.Helper"
//#r "WX.Data.FHelper"
*)
//ConfigHelper.INS.LoadDefaultServiceConfigToManager
let watch=new Stopwatch()
//-----------------------------------------------------------------------------------------------

let ecA="metadata=res://*/SBIIMS_Testing_One.csdl|res://*/SBIIMS_Testing_One.ssdl|res://*/SBIIMS_Testing_One.msl;provider=System.Data.SqlClient;provider connection string='Data Source=192.168.2.199;Initial Catalog=SBIIMS_Testing_One;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True'"
type TA = Microsoft.FSharp.Data.TypeProviders.EdmxFile< @"SBIIMS_Testing_One.edmx",@"D:\TempWorkspace" >
let sbA = new TA.WX.Data.DataModel.SBIIMS_Testing_OneEntities(ecA)
sbA.CommandTimeout<-Nullable<_> 180


let ecB="metadata=res://*/SBIIMS_Testing_Two.csdl|res://*/SBIIMS_Testing_Two.ssdl|res://*/SBIIMS_Testing_Two.msl;provider=System.Data.SqlClient;provider connection string='Data Source=192.168.2.199;Initial Catalog=SBIIMS_Testing_Two;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True'"
type TB = Microsoft.FSharp.Data.TypeProviders.EdmxFile< @"SBIIMS_Testing_Two.edmx",@"D:\TempWorkspace" >
let sbB = new TB.WX.Data.DataModel.SBIIMS_Testing_TwoEntities(ecB)
sbB.CommandTimeout<-Nullable<_> 180

let dbConnectionString= @"Data Source=192.168.2.199;Initial Catalog=SBIIMS_VC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;multipleactiveresultsets=True"

let getEDMConnectionString(connectionString) =
    let dbConnection = new SqlConnection(connectionString)
    let resourceArray = [| "res://*/" |]
    let assemblyList = [| System.Reflection.Assembly.GetCallingAssembly() |] 
    //let assemblyList=[|System.Reflection.Assembly.GetExecutingAssembly()|]
    let metaData = new MetadataWorkspace(resourceArray, assemblyList)
    //metaData.RegisterItemCollection(metaData.GetItemCollection(DataSpace.OCSpace))
    //metaData.RegisterItemCollection(new EdmItemCollection()
    Debug.Assert(metaData.GetItemCollection(DataSpace.CSpace).GetItems<EntityType>().Count <> 0)
    Debug.Assert(metaData.GetItemCollection(DataSpace.SSpace).GetItems<EntityType>().Count <> 0)
    new EntityConnection(metaData, dbConnection)

getEDMConnectionString dbConnectionString

"""connectionString="metadata=.\SBIIMS_VC.csdl|.\SBIIMS_VC.ssdl|.\SBIIMS_VC.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=192.168.2.199;Initial Catalog=SBIIMS_VC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True&quot;" providerName="System.Data.EntityClient" />"""


let GetEntityConnectinString (dbConnectionString, modelSchemaFileName)=
  String.Format ("metadata=.\{0}.csdl|.\{0}.ssdl|.\{0}.msl;provider=System.Data.SqlClient;provider connection string=&quot;{1}&quot;", modelSchemaFileName, dbConnectionString)

metadata=res://*/SBIIMS_JXC_Link.csdl|res://*/SBIIMS_JXC_Link.ssdl|res://*/SBIIMS_JXC_Link.msl;

let GetEmbededEdmxEntityConnectinString (dbConnectionString, modelSchemaFileName)=
  String.Format ("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=System.Data.SqlClient;provider connection string=&quot;{1}&quot;", modelSchemaFileName, dbConnectionString)

let GetEmbededResourceEntityConnectinString (dbConnectionString, modelSchemaFileName, assemblyName)=
  String.Format ("metadata=res://{0}/{1}.csdl|res://{0}/{1}.ssdl|res://{0}/{1}.msl;provider=System.Data.SqlClient;provider connection string=&quot;{2}&quot;", assemblyName,modelSchemaFileName, dbConnectionString)

GetEmbededResourceEntityConnectinString (dbConnectionString,"SBIIMS_VC","WX.Data.DataModel.VC")

GetEntityConnectinString (dbConnectionString, "SBIIMS_VC")

let getEDMConnectionString(dbConnectionString) =
    let dbConnection = new SqlConnection(dbConnectionString)
    let resourceArray = [| "res://*/" |]
    let assemblyList = [| System.Reflection.Assembly.GetCallingAssembly() |]
    let metaData = MetadataWorkspace(resourceArray, assemblyList)
    new EntityConnection(metaData, dbConnection)

getEDMConnectionString  dbConnectionString


(*内存计算
watch.Restart()
printfn "%s" (sbA.T_B|>Seq.length|>string) 
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)
*)

type BD_T_A()=
  member val C_ID:Guid=Unchecked.defaultof<Guid> with get, set
  member val C_BT:string=null with get, set
  member val C_NR:string=null with get, set


type QueryBuilder with
  member this.PageTake  ((source:QuerySource<'T,'Q>), (pageSize:int,pageIndex:int))=
    match this.Skip (source,(pageSize*pageIndex)) with
    | x ->this.Take (x,pageSize) 


watch.Restart()
query { 
  for n in sbA.T_A do
  where (n.C_NR.StartsWith "基本表处理")
  sortBy  n.C_BT
  skip 100
  take 50
  pageTake 
  }
|>Seq.map (fun a ->
    match new BD_T_A() with
    | x ->
        x.C_ID<-a.C_ID
        x.C_BT<-a.C_BT
        x.C_NR<-a.C_NR
        x
    ) 
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_NR a.C_BT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)



let qx=
  query{
    for n in sbA.T_B do
    select n
  }

watch.Restart()
query { 
  for n in sbA.T_B do
  //where (n.C_ZNR.StartsWith "基本表处理")
  //sortBy  n.C_ZBT
  //skip 100
  take 50
  //select n 
  //pageTake
}
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

let source:System.Data.Objects.ObjectSet<TableAType>=context.TableA

query {
  for n in source do
  sortBy n.ColumnA
  skip 10
  take 20
}
|>Seq.iter (fun a->printfn "%s" (string a.ColumnA))

source
|>query.Where (fun a->a.ColumnA)
|>query.SortBy (fun a->a.ColumnA)
|>query.Skip 10
|>query.Take 20
|>query.Run
|>Seq.iter (fun a->printfn "%s" (string a.ColumnA))

watch.Restart()
let xb:QuerySource<TA.WX.Data.DataModel.T_B,Linq.IQueryable>=query.Source( sbA.T_B)
match query.Take (xb,50)  with
| x ->
   let ya=query.Quote ( <@x@>)
   let xa=query.Run ya
   xa
   |>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

watch.Restart()
let xa:QuerySource<TA.WX.Data.DataModel.T_B,TA.WX.Data.DataModel.T_B>=query.Source( sbA.T_B)
match query.Take (xa,50) with
| x ->
    match query.YieldFrom x with
    | y ->
        y.Source
        |>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)



match query.Source( sbA.T_B) with
| x ->
    (query.Where (x, (fun a->a.C_ZNR.StartsWith "基本表处理")))
    |>fun r->
        query.Take (r,50)
        |>fun r->query.Run(<@r@>)
        |>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)

|>query.PageTake (50,2)


|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

//ToTraceString()

watch.Restart()
sbB.T_B
//|>Seq.filter (fun a->a.C_ZNR.StartsWith "基本表处理")
|>Seq.sortBy (fun a->a.C_ZBT)
//|>Seq.skip 100000
|>Seq.take 50
|>query.Source
|>query.PageTake
|>Array.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

watch.Restart()
query{
      for n in sbB.T_B do
      select n
}
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_ZNR a.C_ZBT)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)


(*
watch.Restart()
for u in 0..10 do
    for m in  0..10000 do
      match new TA.WX.Data.DataModel.T_A() with
      | x ->
          x.C_ID<-Guid.NewGuid()
          x.C_BT<-"基本表处理"
          x.C_NR<-"基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理基本表处理"
          seq {
            for n in 0..20 do
              match new TA.WX.Data.DataModel.T_B() with
              | y ->
                  y.C_ID<-Guid.NewGuid()
                  y.C_AID<-x.C_ID
                  y.C_ZBT<-"子表处理"
                  y.C_ZNR<-"子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理子表处理"
                  yield y
          }
          |>Seq.iter (fun a->x.T_B.Add a)
          sb.T_A.AddObject x
    sb.SaveChanges()|>ignore
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)
*)

//----------------------------------------------------------------------------------------------------
type T2 = Microsoft.FSharp.Data.TypeProviders.EdmxFile< @"SBIIMS_JXC.edmx",@"D:\TempWorkspace" >
let ecWrong="metadata=.\SBIIMS_JXC.csdl|.\SBIIMS_JXC.ssdl|.\SBIIMS_JXC.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True&quot;" 
let ecRightA="metadata=res://*/SBIIMS_JXC.csdl|res://*/SBIIMS_JXC.ssdl|res://*/SBIIMS_JXC.msl;provider=System.Data.SqlClient;provider connection string=\"Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True\""
let ecRightB="metadata=res://*/SBIIMS_JXC.csdl|res://*/SBIIMS_JXC.ssdl|res://*/SBIIMS_JXC.msl;provider=System.Data.SqlClient;provider connection string='Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True'"
let sb = new T2.WX.Data.DataModel.SBIIMS_JXCEntities(ecRightB)
watch.Restart()
query { 
  for n in sb.T_RZ do
  where (n.C_YWLXMC="基本表处理")
  sortBy  n.C_BM
  select n 
  }
|> Seq.iter (fun a -> printfn "Company: %s Contact: %s" a.C_YWLXMC a.C_BM)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

watch.Restart()
sb.T_RZ
|>Seq.filter (fun a->a.C_YWLXMC="基本表处理")
|>Seq.sortBy (fun a->a.C_BM)
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_YWLXMC a.C_BM)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

//----------------------------------------------------------------------------------------------------------

let sb=new SBIIMS_JXCEntitiesAdvance()

watch.Restart()
query { 
  for n in sb.T_RZ do
  where (n.C_YWLXMC="基本表处理")
  sortBy  n.C_BM
  select n 
  }
|> Seq.iter (fun a -> printfn "Company: %s Contact: %s" a.C_YWLXMC a.C_BM)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

watch.Restart()
sb.T_RZ
|>Seq.filter (fun a->a.C_YWLXMC="基本表处理")
|>Seq.sortBy (fun a->a.C_BM)
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_YWLXMC a.C_BM)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

//==================================================================================
// You can use Server Explorer to build your ConnectionString.
//type internal SqlConnection = Microsoft.FSharp.Data.TypeProviders.SqlEntityConnection<ConnectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=tempdb;Integrated Security=True">
//let internal db = SqlConnection.GetDataContext()

let getEDMConnectionString(connectionString) =
    let dbConnection = new SqlConnection(connectionString)
    let resourceArray = [| "res://*/" |]
    let assemblyList = [| System.Reflection.Assembly.GetCallingAssembly() |] 
    //let assemblyList=[|System.Reflection.Assembly.GetExecutingAssembly()|]
    let metaData = new MetadataWorkspace(resourceArray, assemblyList)
    //metaData.RegisterItemCollection(metaData.GetItemCollection(DataSpace.OCSpace))
    //metaData.RegisterItemCollection(new EdmItemCollection()
    Debug.Assert(metaData.GetItemCollection(DataSpace.CSpace).GetItems<EntityType>().Count <> 0)
    Debug.Assert(metaData.GetItemCollection(DataSpace.SSpace).GetItems<EntityType>().Count <> 0)
    new EntityConnection(metaData, dbConnection)

let dbConnectionString= @"Data Source=192.168.2.199;Initial Catalog=SBIIMS_VC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;multipleactiveresultsets=True"

let getEDMConnectionString(dbConnectionString) =
    let dbConnection = new SqlConnection(dbConnectionString)
    let resourceArray = [| "res://*/" |]
    let assemblyList = [| System.Reflection.Assembly.GetCallingAssembly() |]
    let metaData = MetadataWorkspace(resourceArray, assemblyList)
    new EntityConnection(metaData, dbConnection)


//type  sql=EdmxFile< @"D:\Workspace\SBIIMS\SBIIMS_JXC\BaseAutomation\WX.Data.DataModel.JXC\SBIIMS_JXC.edmx">
type sql=EdmxFile<"SBIIMS_JXC.edmx", ResolutionFolder = @"D:\Workspace\SBIIMS\SBIIMS_JXC\BaseAutomation\WX.Data.DataModel.JXC">
//System.ArgumentException: MetadataWorkspace must have EdmItemCollection pre-registered???
let edmConnectionString =getEDMConnectionString("Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True;")
let wk=new MetadataWorkspace([| "res://*/" |],[| System.Reflection.Assembly.GetCallingAssembly() |])
Debug.Assert(wk.GetItemCollection(DataSpace.CSpace).GetItems<EntityType>().Count <> 0)
Debug.Assert(wk.GetItemCollection(DataSpace.SSpace).GetItems<EntityType>().Count <> 0)
let sqlCnn=new SqlConnection("Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True;")
let entityCnn=new EntityConnection(wk,sqlCnn)
let sb=new sql.WX.Data.DataModel.SBIIMS_JXCEntities(entityCnn)


type sql01=Microsoft.FSharp.Data.TypeProviders.SqlDataConnection< @"Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True;">
let sb01=sql01.GetDataContext()

let watch=new Stopwatch()
watch.Restart()
query { 
  for n in sb01.T_RZ do
  where (n.C_YWLXMC="基本表处理")
  sortBy  n.C_BM
  select n 
  }
|> Seq.iter (fun a -> printfn "Company: %s Contact: %s" a.C_YWLXMC a.C_BM)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)

watch.Restart()
sb01.T_RZ
|>Seq.filter (fun a->a.C_YWLXMC="基本表处理")
|>Seq.sortBy (fun a->a.C_BM)
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.C_YWLXMC a.C_BM)
watch.Stop()
printfn "%s" (watch.ElapsedMilliseconds|>string)


//let internal table = query {
//    for r in db.Table do
//    select r
//    }
//
//for p in table do
//    printfn "%s" p.Property

(*
MetadataWorkspace workspace = new MetadataWorkspace(
  new string[] { "res://*/" }, 
  new Assembly[] { Assembly.GetExecutingAssembly() });

using (SqlConnection sqlConnection = new SqlConnection(connectionString))
using (EntityConnection entityConnection = new EntityConnection(workspace, sqlConnection))
using (NorthwindEntities context = new NorthwindEntities(entityConnection))
{
  foreach (var product in context.Products)
  {
    Console.WriteLine(product.ProductName);
  }
}

*)

