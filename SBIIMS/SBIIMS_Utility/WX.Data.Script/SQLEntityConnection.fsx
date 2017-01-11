

#r "System.Data.Entity"
#r "FSharp.Data.TypeProviders"
#r "System.Data.Linq"
#r "System.Configuration"

//-----------------------------------------------------------------------
open  System.Data
open System.Data.Entity
open System.Data.EntityClient
open System.Data.SqlClient
open System.Data.Metadata.Edm
open Microsoft.FSharp.Data.TypeProviders
open System.Diagnostics
//-----------------------------------------------------------------------
#I @"C:\Program Files (x86)\Microsoft Enterprise Library 5.0\Bin"
#r "Microsoft.Practices.EnterpriseLibrary.Common"
#r "Microsoft.Practices.EnterpriseLibrary.Data"
//-----------------------------------------------------------------------
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.DataModel.JXC"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
ConfigHelper.INS.LoadDefaultServiceConfigToManager
let watch=new Stopwatch()
//-----------------------------------------------------------------------------------------------

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

type SqlConnection = Microsoft.FSharp.Data.TypeProviders.SqlDataConnection<ConnectionString = @"Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;persist security info=True;user id=sa;password=YZWX001@zhoutao.workspace;">
let db = SqlConnection.GetDataContext()
watch.Restart()
query{
  for n in db.T_DJSP_JHGL do
  take 5
}
|>Seq.iter (fun a->printfn "%A" (a.C_CCK,a.C_CBJE))
watch.Stop()
printfn "%A" (string watch.ElapsedMilliseconds)


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

//------------------------------------------------------------------------------------------------

type Northwind = ODataService<"http://services.odata.org/Northwind/Northwind.svc">
let db = Northwind.GetDataContext()

watch.Restart()
query { 
  for customer in db.Customers do
  where (customer.CompanyName="Ana Trujillo Emparedados y helados")
  sortBy  customer.CompanyName
  select customer 
  }
|> Seq.iter (fun customer -> printfn "Company: %s Contact: %s" customer.CompanyName customer.ContactName)
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds

watch.Restart()
db.Customers
|>Seq.filter (fun a->a.CompanyName="Ana Trujillo Emparedados y helados")
|>Seq.sortBy (fun a->a.CompanyName)
|>Seq.iter (fun a->printfn "Company: %s Contact: %s" a.CompanyName a.ContactName)
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds

watch.Restart()
// You can use Server Explorer to build your ConnectionString.
type internal SqlConnection = SqlEntityConnection<ConnectionString = @"Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True">
let internal db = SqlConnection.GetDataContext()
watch.Stop()
ObjectDumper.Write watch.ElapsedMilliseconds
//db.
type internal sql=TypeProviders.
type internal sql=Microsoft.FSharp.Data.TypeProviders.EdmxFile< @"D:\Workspace\SBIIMS\SBIIMS_JXC\BaseAutomation\WX.Data.DataModel.JXC\SBIIMS_JXC.edmx">

//let internal table = query {
//    for r in db.Table do
//    select r
//    }
//
//for p in table do
//    printfn "%s" p.Property

//============================================================================
(*
module SQLEntityConnection1
(*
#if INTERACTIVE
#r "System.Data.Entity"
#r "FSharp.Data.TypeProviders"
#endif
*)

#r "System.Data.Entity"
#r "FSharp.Data.TypeProviders"
//#r "System."

open System.Data.Entity
open System.Data.EntityClient
open System.Data.SqlClient
open System.Data.Metadata.Edm
open Microsoft.FSharp.Data.TypeProviders
open System.Diagnostics

// You can use Server Explorer to build your ConnectionString.
//type internal SqlConnection = Microsoft.FSharp.Data.TypeProviders.SqlEntityConnection<ConnectionString = @"Data Source=(LocalDB)\v11.0;Initial Catalog=tempdb;Integrated Security=True">
//let internal db = SqlConnection.GetDataContext()
//http://msdn.microsoft.com/en-us/library/hh361038%28v=vs.110%29.aspx
let getEDMConnectionString(connectionString) =
    let dbConnection = new SqlConnection(connectionString)
    let resourceArray = [| "res://*/" |]
    let assemblyList = [| System.Reflection.Assembly.GetCallingAssembly() |] 
    //let assemblyList=[|System.Reflection.Assembly.GetExecutingAssembly()|]
    let metaData = MetadataWorkspace(resourceArray, assemblyList)
    //metaData.RegisterItemCollection(metaData.GetItemCollection(DataSpace.OCSpace))
    //Debug.Assert(metaData.GetItemCollection(DataSpace.CSpace).GetItems<EntityType>().Count <> 0)
    //Debug.Assert(metaData.GetItemCollection(DataSpace.SSpace).GetItems<EntityType>().Count <> 0)
    new EntityConnection(metaData, dbConnection)
//type  sql=EdmxFile< @"D:\Workspace\SBIIMS\SBIIMS_JXC\BaseAutomation\WX.Data.DataModel.JXC\SBIIMS_JXC.edmx">
type sql=EdmxFile<"SBIIMS_JXC.edmx", ResolutionFolder = @"D:\Workspace\SBIIMS\SBIIMS_JXC\BaseAutomation\WX.Data.DataModel.JXC">
//System.ArgumentException: MetadataWorkspace must have EdmItemCollection pre-registered???
let edmConnectionString =getEDMConnectionString("Data Source=192.168.2.199;Initial Catalog=SBIIMS_JXC;Persist Security Info=True;User ID=sa;Password=YZWX001@zhoutao.workspace;MultipleActiveResultSets=True;")
let sb=new sql.WX.Data.DataModel.SBIIMS_JXCEntities(edmConnectionString)

let watch=new Stopwatch()
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



