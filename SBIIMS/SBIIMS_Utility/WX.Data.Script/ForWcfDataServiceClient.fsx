#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Services.Client.dll"
//#r "FSharp.PowerPack.Linq.dll"

open System
open System.Linq
open System.Configuration
open System.Data.Services.Client



//They must import by orderly
//Or it will be like this "--> Referenced 'D:\Workspace\SBIIMS\WX.Data.WcfService\bin\Debug\WX.Data.Helper.dll'"
#I @"D:\Workspace\SBIIMS\WX.Data.Helper\bin\Debug"
#r "WX.Data.Helper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FHelper\bin\Debug"
#r "WX.Data.FHelper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.DataModel\bin\Debug"
#r "WX.Data.DataModel.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.ServiceProxy\bin\Debug"
#r "WX.Data.ServiceProxy.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FModule\bin\Debug"
#r "WX.Data.dll"

open WX.Data.DataOperate
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.ServiceProxy.DS_SBIIMS

////////////////////////////////////////////////


ConfigHelper.INS.LoadDefaultClientConfigToManager
let stopWatch=System.Diagnostics.Stopwatch()
let uri=new Uri("http://localhost:8080/DS_SBIIMS")
let sb=new SBIIMSEntities(uri)
let query=
  sb.T_CK
  |>Seq.toList



////////////////////////////////////////////////

(*

//////////////////////////////////////////////////
//Overral
let stopWatch=System.Diagnostics.Stopwatch()
let uri=new Uri("http://localhost:8080/SBIIMSDataService/")

//let select  f coll =  System.Query.Sequence.Select(coll,new Func<_,_>(f)) //on old version
let qselect (selector:'Source->'Result) (source:seq<'Source>)=System.Linq.Enumerable.Select(source,new Func<_,_>(selector))
let qselect2d (selector:'Source->int->'Result) (source:seq<'Source>)=System.Linq.Enumerable.Select(source,new Func<_,_,_>(selector))
//let genericList=Microsoft.FSharp.Collections.ResizeArray()

//Inner Join
let qjoinTem outer inner outerKeySelector innerKeySelector resultSelector=System.Linq.Enumerable.Join(outer,inner,new Func<_,_>(outerKeySelector),new Func<_,_>(innerKeySelector),new Func<_,_,_>(resultSelector))
let qjoint ((outer:seq<'Outer>),(inner:seq<'Inner>),(outerKeySelector:'Outer->'Key),(innerKeySelector:'Inner->'Key),(resultSelector:'Outer->'Inner->'Result))=System.Linq.Enumerable.Join(outer,inner,new Func<_,_>(outerKeySelector),new Func<_,_>(innerKeySelector),new Func<_,_,_>(resultSelector))
let qjoin (outer:seq<'Outer>) (inner:seq<'Inner>) (outerKeySelector:'Outer->'Key) (innerKeySelector:'Inner->'Key) (resultSelector:'Outer->'Inner->'Result)=System.Linq.Enumerable.Join(outer,inner,new Func<_,_>(outerKeySelector),new Func<_,_>(innerKeySelector),new Func<_,_,_>(resultSelector))

//Left Outer Join
let qgroupjoin (outer:seq<'Outer>) (inner:seq<'Inner>) (outerKeySelector:'Outer->'Key) (innerKeySelector:'Inner->'Key) (resultSelector:'Outer->seq<'Inner>->'Result)=System.Linq.Enumerable.GroupJoin(outer,inner,new Func<_,_>(outerKeySelector),new Func<_,_>(innerKeySelector),new Func<_,_,_>(resultSelector))

//Group By
let groupby (source:'Source) (keySelector:'Source->'Key)=System.Linq.Enumerable.GroupBy(source,new Func<_,_>(keySelector))
//Microsoft.FSharp.Linq.Query.groupBy
//Seq.groupBy

//Take
let qtake count (source:seq<'Source>) =System.Linq.Enumerable.Take(source,count)
//System.Linq.Queryable.Take


/////////////////////////////////////////////////


//Unfortunately, it can't be serialized
[<System.Runtime.Serialization.DataContract>]
type VT_CK=
  {
  [<DataMember>] C_ID:Guid; 
  [<DataMember>] C_MC:string;
  [<DataMember>] [<DefaultValue>] mutable C_BZ:string;
//  [<DataMember>] [<DefaultValue>] mutable C_BZ01:string;
  //[<DataMember>] mutable C_BZ:string
  }
  
//The field sign with [<DefaultValue>] cann't distinguish the type 
type VT_CK001=
  {
  C_ID:Guid; 
  C_MC001:string;//It cann't be C_MC, for they cann't have the same fields indentity between two record type 
  mutable C_BZ001:string; 
  }
  
  
let configFilePath= @"D:\Workspace\SBIIMS\WX.Data.ServiceProxy\App.config"
//let configFilePath= @"D:\Work for myself\F# Research\BusinessArchitecture\SBIIMS\WX.Data.ServiceProxy\App.config"

let channel=new CustomClientChannel<IBusinessEntityService>(configFilePath)
let client=channel.CreateChannel()
//let query0051=

let vt_CK=
   let vt_CK={new VT_CK with  C_ID=Guid.NewGuid() and C_MC="wx"}
   vt_CK.C_BZ<-"wx1111"
   vt_CK
   
let vt_CK001=
   let vt_CK={C_ID=Guid.NewGuid();C_MC="wx"}
   vt_CK.C_BZ<-"wx11112222222222"
   vt_CK
  
let query0031=
  let sb=new SBIIMSEntities(uri)
  sb.T_CK
  //|>Seq.map (fun a->{{C_ID=a.C_ID;C_MC=a.C_MC} with C_BZ="wx" }) //Wrong, but compiled right
  //|>Seq.map (fun a-> {C_ID=a.C_ID;C_MC=a.C_MC}.C_BZ="wx")
  |>Seq.map (fun a-> {new VT_CK with  C_ID=a.C_ID and C_MC=a.C_MC })
  |>qtake 10
  //|>Seq.take 10 //When the cout more than result sequence, the error will occured
  //|>Seq.toArray
ObjectDumper.Write(query0031,1)



 
let entity=new T_JYTZ_GHS()
entity.C_CJRQ<-DateTime.Now

//Inner Join use with linq library
let query0071=
  let sb=new SBIIMSEntities(uri)
  //qjoinTem sb.T_JYTZ_GHS_TEST sb.T_YG_TEST (fun a->a.C_JBR) (fun b->Nullable<Guid> b.C_ID)  (fun a  b ->a.C_ID,a.C_CJRQ,b.C_XM)
  qjoin sb.T_JYTZ_GHS_TEST sb.T_YG_TEST (fun a->a.C_JBR) (fun b->Nullable<Guid> b.C_ID)  (fun a  b -> a.C_ID,a.C_CJRQ,b.C_XM)
  //System.Linq.Enumerable.Join(sb.T_JYTZ_GHS_TEST,sb.T_YG_TEST,(fun a->a.C_JBR),(fun b->Nullable<Guid> b.C_ID),(fun a  b ->a.C_ID,a.C_CJRQ,b.C_XM)) //Right
  //System.Linq.Enumerable.Join(sb.T_JYTZ_GHS_TEST,sb.T_YG_TEST,(fun (a:T_JYTZ_GHS_TEST)->a.C_JBR),(fun (b:T_YG_TEST)->Nullable<Guid> b.C_ID),(fun (a:T_JYTZ_GHS_TEST)  (b:T_YG_TEST) ->a.C_ID,a.C_CJRQ,b.C_XM))
ObjectDumper.Write(query0071,1)

//INNER JOIN
let query007=
  let sb=new SBIIMSEntities(uri)
  join  sb.T_JYTZ_GHS_TEST sb.T_YG_TEST  (fun a->a.C_JBR ) (fun b->Nullable<Guid> b.C_ID) (fun a b ->a.C_ID,a.C_CJRQ,b.C_XM)
  //join  sb.T_JYTZ_GHS_TEST sb.T_YG_TEST  (fun a->if a.C_JBR.HasValue then a.C_JBR.Value else new Guid() ) (fun b->b.C_ID) (fun a b ->a.C_ID,a.C_CJRQ,b.C_XM)
  //join  sb.T_JYTZ_GHS_TEST sb.T_YG_TEST  (fun a->a.C_JBR.GetValueOrDefault() ) (fun b->b.C_ID) (fun a b ->a.C_ID,a.C_CJRQ,b.C_XM) //DefualtValue is "00000000-0000-0000-0000-000000000000"
  //join  sb.T_YG_TEST sb.T_JYTZ_GHS_TEST  (fun a->Nullable<Guid> a.C_ID) (fun b->b.C_JBR ) (fun a b ->b.C_ID,b.C_CJRQ,a.C_XM)
  //join  sb.T_JYTZ_GHS_TEST sb.T_YG_TEST  (fun a->a.C_CZY) (fun b->b.C_ID) (fun a b ->a.C_ID,a.C_CJRQ,b.C_XM)
ObjectDumper.Write(query007,1)

//LEFT OUTTER JOIN
let query008=
  let sb=new SBIIMSEntities(uri)
  //Right backup
  //The relation key is between Nullable<T> and T
  groupJoin sb.T_JYTZ_GHS_TEST sb.T_YG_TEST  (fun a->a.C_JBR) (fun b->Nullable<Guid> b.C_ID)
     (fun a c->a.C_CJRQ,a.C_ID,(if c.FirstOrDefault()=null then "" else c.FirstOrDefault().C_XM)) //In that 'c' is  filtered already by '(fun a->a.C_CZY) (fun b->b.C_ID)'
     
(*
//Left Outer Join in Linq
//LinqSamples from micrsoft
        [Category("Join")]
        [Title("GroupJoin - Left Outer Join")] 
        [Description("This sample shows how to get LEFT OUTER JOIN by using DefaultIfEmpty(). The DefaultIfEmpty() method returns null when there is no Order for the Employee." )]
        public void LinqToSqlJoin07() {
            var q =
                from e in db.Employees
                join o in db.Orders on e equals o.Employee into ords
                from o in ords.DefaultIfEmpty()
                select new {e.FirstName, e.LastName, Order = o};

            ObjectDumper.Write(q);
        }

*)
     
     
  //Right backup 
  //Normal left outter join
  //groupJoin sb.T_JYTZ_GHS_TEST sb.T_YG_TEST  (fun a->a.C_CZY) (fun b->b.C_ID)
  //   (fun a c->a.C_CJRQ,a.C_ID,(if c.FirstOrDefault()=null then "" else c.FirstOrDefault().C_XM)) //In that 'c' is  filtered already by '(fun a->a.C_CZY) (fun b->b.C_ID)'
 
ObjectDumper.Write(query008,1)  




let query0020=
  let sb=new SBIIMSEntities(uri)
  sb.T_JYTZ_GHS
  |>Seq.toList
  
let query009=
  let sb=new SBIIMSEntities(uri)
  sb.T_SZLX
  |>Seq.toList
  
ObjectDumper.Write(query007,1)

///////////////////////////////////////

//>>
//let objectExpression=
//  {
//    new T_JYTZ_GHS() with
//       C_FYLB=1
//  }


//执行时要使用两个fsi的Session，一个fsi实例作为服务端，另外一个fsi实例作为客户端， 即服务端Session和客户端Session不能使用同一个fsiSession

let configFilePath= @"D:\Workspace\SBIIMS\WX.Data.ServiceProxy\App.config"
//let configFilePath= @"D:\Work for myself\F# Research\BusinessArchitecture\SBIIMS\WX.Data.ServiceProxy\App.config"
//let config=ConfigHelper.INS.OpenConfig configFilePath

let channel=new CustomClientChannel<IBusinessEntityService>(configFilePath)
let client=channel.CreateChannel()
ObjectDumper.Write(channel,2)
let query001=client.GetT_CKs()

let query002=
  use serviceProxy=new BusinessEntityServiceClient()
  ObjectDumper.Write(serviceProxy.Endpoint,2)
  let result001=serviceProxy.GetT_CKs()
  ObjectDumper.Write(result001,2)
  result001

//let uri=new Uri("http://localhost:8080/SBIIMSDataService/")

let query003=
  let uri=new Uri("http://localhost:8080/SBIIMSDataService/")
  let sb=new SBIIMSEntities(uri)
  sb.T_CK
  |>Seq.groupBy (fun a->a.C_CKDZ) 
  |>Seq.toArray

  


let fJoin=Microsoft.FSharp.Linq.Query.join


//right
let fQuery=Microsoft.FSharp.Linq.Query.query
stopWatch.Reset()
stopWatch.Start()  
let query004=
  let sb=new SBIIMSEntities(uri)
  fQuery 
    <@   
      seq {
        for a in sb.T_CK do
          yield a 
      }
    @>
  |>Seq.toList
stopWatch.Stop()

//Operate in memory  
stopWatch.Reset()
stopWatch.Start()  
let query005=
  let sb=new SBIIMSEntities(uri) 
  let genericList=Microsoft.FSharp.Collections.ResizeArray<T_CK>()
  for a in sb.T_CK do
     if a.C_CKDZ="九门里"  then
       genericList.Add(a)
  genericList
stopWatch.Stop()

stopWatch.Reset()
stopWatch.Start()  
let query006=
  let sb=new SBIIMSEntities(uri) 
  seq{for a in sb.T_CK do
      if a.C_CKDZ="九门里"  then
        yield a} 
  |>Seq.toList
stopWatch.Stop()

  
let netList=new System.Collections.Generic.List<String>()
for i in 0..100 do
  netList.Add("wx"^i.ToString());
let x=Seq.toList netList

let endpoints=new System.ServiceModel.Configuration.ChannelEndpointElementCollection()
let endpointsEnum=endpoints.GetEnumerator()



//endpointsEnum
//|>select (fun a->a.

*)