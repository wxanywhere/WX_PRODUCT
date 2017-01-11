
#r "System.Xml.dll"
#r "System.Xml.Linq.dll"
open System
open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open System.IO

#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data

//======================================
//没有使用xmlns的情况，XElement..., XPathSelectElements...
(*
How to: Query LINQ to XML Using XPath
http://msdn.microsoft.com/en-us/library/bb387057.aspx

.NET Language-Integrated Query for XML Data
http://msdn.microsoft.com/en-us/library/bb308960.aspx

How to: Create a Document with Namespaces (C#) (LINQ to XML)
http://msdn.microsoft.com/en-us/library/bb387075.aspx

http://msdn.microsoft.com/en-us/library/bb387075.aspx
// Create an XML tree in a namespace.
XNamespace aw = "http://www.adventure-works.com";
XElement root = new XElement(aw + "Root",
    new XElement(aw + "Child", "child content")
);
Console.WriteLine(root);

<Root xmlns="http://www.adventure-works.com">
  <Child>child content</Child>
</Root>
//---------
// Create an XML tree in a namespace, with a specified prefix
XNamespace aw = "http://www.adventure-works.com";
XElement root = new XElement(aw + "Root",
    new XAttribute(XNamespace.Xmlns + "aw", "http://www.adventure-works.com"),
    new XElement(aw + "Child", "child content")
);
Console.WriteLine(root);

<aw:Root xmlns:aw="http://www.adventure-works.com">
  <aw:Child>child content</aw:Child>
</aw:Root>
//---------
// The http://www.adventure-works.com namespace is forced to be the default namespace.
XNamespace aw = "http://www.adventure-works.com";
XNamespace fc = "www.fourthcoffee.com";
XElement root = new XElement(aw + "Root",
    new XAttribute("xmlns", "http://www.adventure-works.com"),
    new XAttribute(XNamespace.Xmlns + "fc", "www.fourthcoffee.com"),
    new XElement(fc + "Child",
        new XElement(aw + "DifferentChild", "other content")
    ),
    new XElement(aw + "Child2", "c2 content"),
    new XElement(fc + "Child3", "c3 content")
);
Console.WriteLine(root);
<Root xmlns="http://www.adventure-works.com" xmlns:fc="www.fourthcoffee.com">
  <fc:Child>
    <DifferentChild>other content</DifferentChild>
  </fc:Child>
  <Child2>c2 content</Child2>
  <fc:Child3>c3 content</fc:Child3>
</Root>
//---------
XNamespace aw = "http://www.adventure-works.com";
XNamespace fc = "www.fourthcoffee.com";
XElement root = new XElement(aw + "Root",
    new XAttribute(XNamespace.Xmlns + "aw", aw.NamespaceName),
    new XAttribute(XNamespace.Xmlns + "fc", fc.NamespaceName),
    new XElement(fc + "Child",
        new XElement(aw + "DifferentChild", "other content")
    ),
    new XElement(aw + "Child2", "c2 content"),
    new XElement(fc + "Child3", "c3 content")
);
Console.WriteLine(root);
<aw:Root xmlns:aw="http://www.adventure-works.com" xmlns:fc="www.fourthcoffee.com">
  <fc:Child>
    <aw:DifferentChild>other content</aw:DifferentChild>
  </fc:Child>
  <aw:Child2>c2 content</aw:Child2>
  <fc:Child3>c3 content</fc:Child3>
</aw:Root>
//----------
// Create an XML tree in a namespace, with a specified prefix
XElement root = new XElement("{http://www.adventure-works.com}Root",
    new XAttribute(XNamespace.Xmlns + "aw", "http://www.adventure-works.com"),
    new XElement("{http://www.adventure-works.com}Child", "child content")
);
Console.WriteLine(root);
<aw:Root xmlns:aw="http://www.adventure-works.com">
  <aw:Child>child content</aw:Child>
</aw:Root>
*)

//======================================



//正确！ 直接修改插入节点元素的namespace
let path= @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX\WX.Data.View.JXC.JBXX\WX.Data.View.JXC.JBXX - Copy.csproj"
match XElement.Load (path) with
| x ->
    match x.Name.NamespaceName with
    | y ->
        let ns=XNamespace.Get(y)
        let nt = new NameTable()
        let nsmgr = new XmlNamespaceManager(nt)
        nsmgr.AddNamespace("wx",y)
        match x.XPathSelectElement( @"//wx:Reference[@Include=""WX.Data.View.Controls.LoadingControl""]",nsmgr) with //Right, wx
        | z ->
            //ObjectDumper.Write (y.Attribute(XName.Get("Include","")))
            //否则将会默地自动增加xmlns="",用于退出当前的命名空间Set the namespace on the Example and SubElement elements to the same as the RootElement. It is adding the xmlns="" to clear the namespace for these elements
            @"
            <Reference Include=""WX.Data.View.Controls.LoadingControl"">
              <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
            </Reference>"
            |>fun a->
                match XElement.Parse(a,LoadOptions.None)  with
                | w ->
                    let rec ModifyElementNamespace (sourceXElement:XElement)=
                      match sourceXElement with
                      | s ->
                          s.Name<-ns+s.Name.LocalName //XName={XNamespace}+"LocalName"(较为特殊), 或使用XName.Get("LocalName","namespace")   
                          for m in s.Elements() do ModifyElementNamespace m
                    ModifyElementNamespace w
                    z.AddBeforeSelf w
//------------------------------------------------
//正确，使用节点实例复制的方式插入新的xml节点，方式二
let path= @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX\WX.Data.View.JXC.JBXX\WX.Data.View.JXC.JBXX - Copy.csproj"
match XElement.Load(path) with
| x ->
  let ns= x.Name.NamespaceName
  ObjectDumper.Write ns
  let nsx=XNamespace.Get(x.Name.NamespaceName)
  let xname name=XName.Get(name,ns) 
  let nt = new NameTable()
  let nsmgr = new XmlNamespaceManager(nt)
  nsmgr.AddNamespace("wx", ns)   //
  match x.XPathSelectElement( @"//wx:Reference[@Include=""WX.Data.View.Controls.LoadingControl""]",nsmgr) with //Right, wx
  | y ->
    //ObjectDumper.Write (y.Attribute(XName.Get("Include","")))
    //加入xmlns="..."用于和项目xml文档的命名空间保持一致(xmlns是特殊的Attribute,通过Attribute方法不能操作xmlns)，否则将会默地自动增加xmlns="",用于退出当前的命名空间Set the namespace on the Example and SubElement elements to the same as the RootElement. It is adding the xmlns="" to clear the namespace for these elements
    @"
    <Reference Include=""WX.Data.View.Controls.LoadingControl"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
    </Reference>"
    |>fun a->
        match XElement.Parse(a,LoadOptions.None)  with
        | s ->
            let xeNew:XElement= //XElement和其中的Attribute不能分开创建
              new XElement(nsx+s.Name.LocalName,
                [|
                  for ma in s.Attributes() do 
                    yield new XAttribute(XName.Get(ma.Name.LocalName),ma.Value) //注意!!!!!!!!!!!!!!, 获取Attribute的XName，不能使用namespace，否则导致属性xmlns及当前Element下的属性加入命名空间的别名前缀如，p3:Attribute01="..." xmlns:p3="..."
                |])
            let rec CreateXElement (sourceXElement:XElement) (targetRootXElement:XElement) =
              match sourceXElement,targetRootXElement with
              | xa, ya ->
                  match (xa.Elements()|>Seq.length)>0 with
                  | true ->
                      for ma in xa.Elements() do
                        match 
                          new XElement(nsx+ma.Name.LocalName, 
                                        [|
                                          for na in ma.Attributes() do
                                            yield new XAttribute(XName.Get(na.Name.LocalName),na.Value)  //XName不能再使用nsx+na.Name.LocalName,否则会自动增加namespace的后缀，如xml:p3="..."  //注意!!!!!!!!!!!!!!, 获取Attribute的XName，不能使用namespace，否则导致属性xmlns及当前Element下的属性加入命名空间的别名前缀如，p3:Attribute01="..." xmlns:p3="..."
                                        |]) 
                          with
                        | za ->
                            ya.Add (za)
                            CreateXElement ma za
                  | _ ->ya.Value<-xa.Value
            CreateXElement s xeNew
            y.AddBeforeSelf xeNew
            x.Save(path,SaveOptions.None)  //OmitDuplicateNamespaces是必须的，否则会额外增加xmlns="..."


//--------------------------------------------------------------
//正确！使用节点实例复制的方式插入新的xml节点
let path= @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX\WX.Data.View.JXC.JBXX\WX.Data.View.JXC.JBXX - Copy.csproj"
match XElement.Load(path) with
| x ->
  let ns= x.Name.NamespaceName
  ObjectDumper.Write ns
  let nsx=XNamespace.Get(x.Name.NamespaceName)
  let xname name=XName.Get(name,ns) 
  let nt = new NameTable()
  let nsmgr = new XmlNamespaceManager(nt)
  nsmgr.AddNamespace("wx", ns)   //
  match x.XPathSelectElement( @"//wx:Reference[@Include=""WX.Data.View.Controls.LoadingControl""]",nsmgr) with //Right, wx
  | y ->
    //ObjectDumper.Write (y.Attribute(XName.Get("Include","")))
    //加入xmlns="..."用于和项目xml文档的命名空间保持一致(xmlns是特殊的Attribute,通过Attribute方法不能操作xmlns)，否则将会默地自动增加xmlns="",用于退出当前的命名空间Set the namespace on the Example and SubElement elements to the same as the RootElement. It is adding the xmlns="" to clear the namespace for these elements
    @"
    <Reference Include=""WX.Data.View.Controls.LoadingControl"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
    </Reference>"
    |>fun a->
        match XElement.Parse(a,LoadOptions.None)  with
        | s ->
            let xeNew=new XElement(nsx+s.Name.LocalName)
            let rec CreateXElement (sourceXElement:XElement) (targetRootXElement:XElement) =
              match sourceXElement,targetRootXElement with
              | xa,ya ->
                  for ma in xa.Attributes() do 
                    match ma.Name.LocalName with
                    | wa ->
                        ya.Add(new XAttribute(XName.Get(ma.Name.LocalName),ma.Value))  //注意!!!!!!!!!!!!!!, 获取Attribute的XName，不能使用namespace，否则导致属性xmlns及当前Element下的属性加入命名空间的别名前缀如，p3:Attribute01="..." xmlns:p3="..."
                  match (xa.Elements()|>Seq.length)>0 with
                  | true ->
                      for ma in xa.Elements() do
                        match ma.Name.LocalName with
                        | wa ->
                            match new XElement(nsx+wa) with
                            | ua ->
                                ya.Add (ua) 
                                CreateXElement ma ua
                  | _ ->ya.Value<-xa.Value
            CreateXElement s xeNew
            y.AddBeforeSelf xeNew
            x.Save(path,SaveOptions.None)  //OmitDuplicateNamespaces是必须的，否则会额外增加xmlns="..."


//-------------------------------------------------------------
@"
<Project ToolsVersion=""4.0"" DefaultTargets=""Build"">  //注意！！！！！！！！！！！！！！！无xmlns
  <ItemGroup>
    <Reference Include=""System"" />
  </ItemGroup>
  <ItemGroup>
    <Compile>
      <DependentUpon></DependentUpon>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <Compile>
      <SubType>Code</SubType>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <Reference>
      <HintPath></HintPath>
    </Reference>
  </ItemGroup>
</Project>"
|>fun a->XElement.Parse a
|>fun a->
    let xnameX name=XName.Get (name,"")
    for b in a.Elements(xnameX "ItemGroup") do //节点"Project"无xmlns时正确
      ObjectDumper.Write b.Name.LocalName
    for b in a.XPathSelectElements("//ItemGroup") do  //节点"Project"无xmlns时正确
      ObjectDumper.Write b.Name.LocalName
    for b in a.XPathSelectElements("//Compile") do  //节点"Project"无xmlns时正确
      ObjectDumper.Write b.Name.LocalName
    for b in a.XPathSelectElements("//HintPath") do  //节点"Project"无xmlns时正确
      ObjectDumper.Write b.Name.LocalName

//使用xmlns的情况，XElement..., XPathSelectElements...
@"
<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">  //注意！！！！！！！！！！！！！！！有xmlns
  <ItemGroup>
    <Reference Include=""System"" />
  </ItemGroup>
  <ItemGroup>
    <Compile>
      <DependentUpon></DependentUpon>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <Compile>
      <SubType>Code</SubType>
    </Compile>
  </ItemGroup>
  <ItemGroup>
    <Reference>
      <HintPath></HintPath>
    </Reference>
  </ItemGroup>
</Project>"
|>fun a->XElement.Parse a
|>fun a->
    let xnameY name=XName.Get (name,a.Name.NamespaceName)  //节点"Project"有xmlns时必须,如"http://schemas.microsoft.com/developer/msbuild/2003"
    let nt = new NameTable()
    let nsmgr = new XmlNamespaceManager(nt)
    nsmgr.AddNamespace("wx", a.Name.NamespaceName) //使用xmlns时必须
    for b in a.Elements(xnameY "ItemGroup") do //节点"Project"有xmlns时正确
      ObjectDumper.Write b.Name.LocalName
    for b in a.XPathSelectElements("//wx:ItemGroup",nsmgr) do  //节点"Project"无xmlns时正确
      ObjectDumper.Write b.Name.LocalName
    for b in a.XPathSelectElements("//wx:Compile",nsmgr) do  //节点"Project"无xmlns时正确
      ObjectDumper.Write b.Name.LocalName
    for b in a.XPathSelectElements("//wx:HintPath",nsmgr) do  //节点"Project"无xmlns时正确, 其中"//"是必须的
      ObjectDumper.Write b.Name.LocalName

match XElement.Load(@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX\WX.Data.View.JXC.JBXX\WX.Data.View.JXC.JBXX.csproj") with
| x ->
  let ns= @"http://schemas.microsoft.com/developer/msbuild/2003"
  let xname name=XName.Get(name,ns) 
  let nt = new NameTable()
  let nsmgr = new XmlNamespaceManager(nt)
  nsmgr.AddNamespace("wx", ns)   //
  for a in x.Elements(xname @"ItemGroup") do //Right
    ObjectDumper.Write a.Name.LocalName
  for a in x.XPathSelectElements("//wx:ItemGroup",nsmgr) do   //Right
    ObjectDumper.Write a.Name.LocalName   
  for a in x.XPathSelectElements("//wx:Reference",nsmgr) do //Right, wx
    ObjectDumper.Write a.Name.LocalName   

//=====================================================================
(*
XPath Examples@@@@@@@@@
http://msdn.microsoft.com/en-us/library/ms256086.aspx
*)
//-------------------------------------------------------
//XmlDocumnet方式下，在VisualStudio2010项目中增加Reference组件
let path= @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX\WX.Data.View.JXC.JBXX\WX.Data.View.JXC.JBXX - Copy.csproj"
match new XmlDocument() with
| x ->
    x.Load(path)
    //ObjectDumper.Write x.OuterXml
    let ns= @"http://schemas.microsoft.com/developer/msbuild/2003"
    let nt = new NameTable()
    let nsmgr = new XmlNamespaceManager(nt)
    nsmgr.AddNamespace("wx",ns)
    match x.SelectSingleNode(@"//wx:Reference[@Include=""WX.Data.View.Controls.LoadingControl""]",nsmgr) with   //x.SelectSingleNode(@"/Project/ItemGroup/Reference[@Include=""WX.Data.View.Controls.LoadingControl""]"), 用于无xmlns时正确
    | y ->
        ObjectDumper.Write y.Name
        //加入xmlns="..."用于和项目xml文档的命名空间保持一致(xmlns是特殊的Attribute,通过Attribute方法不能操作xmlns)，否则将会默地自动增加xmlns="",用于退出当前的命名空间Set the namespace on the Example and SubElement elements to the same as the RootElement. It is adding the xmlns="" to clear the namespace for these elements
        @"
        <Reference Include=""WX.Data.View.Controls.LoadingControl"" xmlns="""+ns+ @""">
          <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
        </Reference>"
        |>fun a->
            match new XmlDocument() with
            | z ->
                z.LoadXml(a)
                match z.SelectSingleNode(@"/Reference") with
                | w -> 
                    //y.ParentNode.InsertBefore(w,y)|>ignore //错误！！！！System.ArgumentException: The node to be inserted is from a different document context
                    match y.Clone() with
                    | u ->
                        (*该种方式节点会自动添加xmlns=""，用于退出当前的命名空间Set the namespace on the Example and SubElement elements to the same as the RootElement. It is adding the xmlns="" to clear the namespace for these elements
                        u.Attributes.["Include"].Value <-w.Attributes.["Include"].Value
                        u.InnerXml<-w.InnerXml //Right
                        *)
                        u.Attributes.["Include"].Value <-w.Attributes.["Include"].Value
                        u.ChildNodes.[0].InnerText<-w.ChildNodes.[0].InnerText
                        y.ParentNode.InsertBefore(u,y)|>ignore
                    x.Save(path)

//-----------------------------
//http://stackoverflow.com/questions/61084/empty-namespace-using-linq-xml
(*
XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),    
  new XElement(ns + "urlset",    
  new XElement("url",        
    new XElement("loc", "http://www.example.com/page"),        
    new XElement("lastmod", "2008-09-14"))));

The result is ...
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">  
  <url xmlns="">  //注意xmlns=""表明已退出根节点的命名空间   
    <loc>http://www.example.com/page</loc>    
    <lastmod>2008-09-14</lastmod>  
  </url>
</urlset>
*)
(*
XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "true"),
  new XElement(ns + "urlset",
  new XElement(ns + "url",    
    new XElement(ns + "loc", "http://www.example.com/page"),    
    new XElement(ns + "lastmod", "2008-09-14"))));

Same as your code, but with the "ns +" 
before every element name that needs to be in the sitemap namespace. 
It's smart enough not to put any unnecessary 
namespace declarations in the resulting XML, so the result is:
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">  
  <url>  //注意！！！！，namespace于根节点的命名空间一致 
    <loc>http://www.example.com/page</loc>    
    <lastmod>2008-09-14</lastmod>  
   </url>
 </urlset>
*)
//-------------------------------------------------------
let xdoc=XmlDocument()
xdoc.Load(@"D:\Workspace\SBIIMS\SBIIMS_Base\ViewBase\WX.Data.View.Resources\Data\XmlData\SystemData\SystemFunction.xml")
//xdoc.ChildNodes.[1].ChildNodes.[1].ChildNodes
xdoc.SelectNodes("/Info/View/Record")
|>fun a->
  seq{
    for m in a do
      for n in m.ChildNodes do
        yield n.Attributes.["VC_GNMC"].Value     
    }
|>Seq.iter (fun a->ObjectDumper.Write a)


[
"AC","CZYJSGL"
"AC","CZYWH"
"AC","GGXZ"
"AC","GG"
"AC","GNWH"
"AC","JSGNGL"
"AC","JSQXGL"
"AC","JSWH"
"AC","QXWH"
]
|>fun a->
    for (m,n) in a do
      String.Format (@"VC_SJLX=""FVM_{0}"" VC_GJSJLX=""FVM_{0}_Advance"" VC_ZPM=""WX.Data.FViewModel.{1}.{0}"" VC_GJZPM=""WX.Data.FViewModelAdvance.{1}.{0}"" VCS_FGNID=""14AF0F82-5E90-4b02-9381-2D7E16398E23"" VCS_JYBZ=""False""  VC_ZPURI=""/WX.Data.View.ViewModelTemplate.{1}.{0};Component/VMT_{1}_{0}.xaml"" VC_GJZPURI=""/WX.Data.View.ViewModelTemplateAdvance.{1}.{0};Component/VMT_{1}_{0}_Advance.xaml""",n,m)
      |>ObjectDumper.Write


let parentNodeTemplate= @"<Record  VCS_GNID=""{0}"" VC_GNMC=""{1}"" VC_SJLX="" ""  VC_GJSJLX="" "" VC_ZPM="" "" VC_GJZPM="" ""  VCS_FGNID=""{2}"" VCS_JYBZ=""false""  VC_ZPURI="" "" VC_GJZPURI="" "" VCS_GNZPQYZTID=""1"" VC_GNBZ="" ""  VC_CS="" "" VC_JDXH=""{3}"">"
let childNodeTemplate= @"    <Record  VCS_GNID=""{0}"" VC_GNMC=""{1}"" VC_SJLX=""FVM_{2}""  VC_GJSJLX=""FVM_{2}_Advance"" VC_ZPM=""WX.Data.FViewModel.{3}"" VC_GJZPM=""WX.Data.FViewModelAdvance.{3}""  VCS_FGNID=""{4}"" VCS_JYBZ=""false""  VC_ZPURI=""/WX.Data.View.ViewModelTemplate.{3};Component/VMT_{6}.xaml"" VC_GJZPURI=""/WX.Data.View.ViewModelTemplateAdvance.{3};Component/VMT_{6}_Advance.xaml"" VCS_GNZPQYZTID=""1"" VC_GNBZ="" ""  VC_CS="" "" VC_JDXH=""{5}""/>"
[
"JXC","JHGL","CGJH","进货管理","采购进货"                                    
"JXC","JHGL","CGTH","进货管理","采购退货"                                                                
"JXC","JHGL","WLZW","进货管理","往来帐务" 
"JXC","JHGL","DJCX","进货管理","采购单据查询"  
"JXC","KCGL","WLZW","进货管理","库存往来查询"   
      
"JXC","XSGL","SPXS","销售管理","商品销售"   
"JXC","XSGL","POS","销售管理","POS 销售"                                         
"JXC","XSGL","XSTH","销售管理","顾客退货"            
"JXC","XSGL","XSHH","销售管理","销售换货"                                  
"JXC","XSGL","WLZW","销售管理","销售往来帐务"      
"JXC","XSGL","DJCX","销售管理","销售单据查询"    
"JXC","KCGL","WLZW","销售管理","库存往来查询"    

"JXC","KCGL","SPCF","库存管理","商品拆分"                                   
"JXC","KCGL","SPKB","库存管理","商品捆绑"    
"JXC","KCGL","SPBY","库存管理","商品报损" 
"JXC","KCGL","SPBS","库存管理","商品报溢"        
"JXC","KCGL","BSBY","库存管理","报损报溢" //合并商品报损和商品报溢                               
"JXC","KCGL","KCPD","库存管理","库存盘点"    
"JXC","KCGL","KCPDX","库存管理","库存盘点X" 
"JXC","KCGL","KCDB","库存管理","库存调拨"                       
"JXC","KCGL","KCYJ","库存管理" ,"库存预警"                                                
"JXC","KCGL","WLZW","库存管理","库存往来帐"   
"JXC","KCGL","KCCX","库存管理","库存查询"       
"JXC","KCGL","DJCX","库存管理","库存单据查询"      
     
"JXC","JHGL","WLZW","统计报表","供货商供货统计"   
"JXC","TJBB","SPCG","统计报表","商品采购统计"   
"JXC","TJBB","CGYCG","统计报表","采购员采购统计"   
"JXC","TJBB","KCCB","统计报表","库存成本统计"   
"JXC","XSGL","WLZW","统计报表","客户消费统计"  
"JXC","TJBB","SPXS","统计报表","商品销售统计"       
"JXC","TJBB","XSYXS","统计报表","销售员销售统计"   
"JXC","TJBB","XSPH","统计报表","商品销售排行"                                                
"JXC","TJBB","YYFX","统计报表","营业分析"    

"JXC","ZHGL","CWGL","综合管理","财务综合管理"  
"JXC","ZHGL","XZGL","综合管理","薪资管理"  
"JXC","ZHGL","JYFYGL","综合管理","经营费用管理"  
"JXC","ZHGL","BJGL","综合管理","报价管理"  
"JXC","ZHGL","JJGL","综合管理","均价管理"  
"JXC","ZHGL","HTGL","综合管理","合同管理"         
"JXC","ZHGL","GHSGL","综合管理","供货商管理"    
"JXC","ZHGL","KHGL","综合管理","客户管理"      
"JXC","ZHGL","HYGL","综合管理","会员管理" 
"JXC","ZHGL","JYFGL","综合管理","经营交易方管理"     
"JXC","ZHGL","YWYGL","综合管理","业务员管理"         
"JXC","ZHGL","XFPGL","综合管理","消费品管理"         
"JXC","ZHGL","FJPGL","综合管理","废旧品管理"               
"JXC","ZHGL","GDZCGL","综合管理","固定资产管理"                                  
"JXC","ZHGL","KHJH","综合管理","客户借货管理"                            
"JXC","ZHGL","SJTX","综合管理","事件提醒"                                    

"JXC","JBXX","SPWH","基本信息管理","商品信息维护"                  
"JXC","JBXX","GHSWH","基本信息管理","供货商信息维护"       
"JXC","JBXX","KHWH","基本信息管理","客户信息维护"     
"JXC","JBXX","JYFWH","基本信息管理","交易方信息维护"    
"JXC","JBXX","YGWH","基本信息管理","员工信息维护"    
"JXC","JBXX","CZYWH","基本信息管理","操作员信息维护"         
"JXC","JBXX","CKWH","基本信息管理","仓库信息维护"                                         
"JXC","JBXX","XFPWH","基本信息管理","消费品信息维护"                  
"JXC","JBXX","FJPWH","基本信息管理","废旧品信息维护"                                         
"JXC","JBXX","ZCLBWH","基本信息管理","固定资产类别维护"          

"JXC","ZHBB","DJBB","综合报表管理","单据报表"          //使用较为频繁，且同一类型的报表显示的数据一致，因此单独处理
"JXC","ZHBB","QDBB","综合报表管理","清单报表"          //应该和相关功能合并？？？
"JXC","ZHBB","TBBB","综合报表管理","图表报表"           //应该和相关功能合并？？？
"JXC","ZHBB","DCBB","综合报表管理","导出"                //应该和相关功能合并？？？

"JXC","FXYC","CGFX","分析预测","商品采购分析"  
"JXC","FXYC","CGJH","分析预测","采购商品计划"  
"JXC","FXYC","XSFX","分析预测","销售分析"  
"JXC","FXYC","XSYC","分析预测","销售预测"  
"JXC","FXYC","KCFX","分析预测","库存分析"  
"JXC","FXYC","KCYC","分析预测","库存积压风险预测"  
"JXC","FXYC","JGFX","分析预测","价格分析"   
"JXC","FXYC","JGYC","分析预测","价格趋势预测"    
"JXC","FXYC","YLFX","分析预测","盈利分析"  
"JXC","FXYC","YLYC","分析预测","盈利预测"  
"JXC","FXYC","GZLFX","分析预测","业务员工作量分析"          
"JXC","FXYC","GZLYC","分析预测","业务员工作量预测"       
                              
"JXC","XTGL","XTSZ","系统管理","系统设置"    
"JXC","XTGL","BFHF","系统管理","备份恢复"             
"JXC","XTGL","YZWJS","系统管理","月账务结算"                         
"JXC","XTGL","NZWJS","系统管理","年账务结算"    
"JXC","XTGL","XTRZ","系统管理","系统日志"                                   
"JXC","XTGL","XTCSH","系统管理","系统初始化"                              
"JXC","XTGL","QCJZ","系统管理","期初建账"             
"JXC","XTGL","TMDY","系统管理","条码打印"                       
                      
"AC","","CZYJSGL","访问控制","操作员角色管理"
"AC","","JSGNGL","访问控制","角色功能管理"
"AC","","JSQXGL","访问控制","角色权限管理" 
"AC","","CZYWH","访问控制","操作员维护"
"AC","","GNWH","访问控制","功能维护" 
"AC","","JSWH","访问控制","角色维护" 
"AC","","QXWH","访问控制","权限维护" 
]
|>Seq.groupBy (fun (_,_,_,a,_)->a)
|>Seq.iteri (fun i (a,b)->
    match Guid.NewGuid() with
    | x ->
        String.Format(parentNodeTemplate, x,a,DefaultGuidValue,char (int 'A'+i))|>ObjectDumper.Write
        b
        |>Seq.iteri (fun j (m1,m2,m3,m4,m5) ->
            String.Format (childNodeTemplate,Guid.NewGuid(),m5,(match m2 with NotNullOrWhiteSpace _ -> m2+"_"+m3 | _ ->m3), (match m2 with NotNullOrWhiteSpace _ -> m1+"."+m2+"."+m3 | _ ->m1+"."+m3),x, j,(match m2 with NotNullOrWhiteSpace _ -> m1+"_"+m2+"_"+m3 | _ ->m1+"_"+m3))|>ObjectDumper.Write
            )
        @"</Record>"|>ObjectDumper.Write
    )

char 65

int 'A'

//=======================================
(*
    <Record  VCS_GNID="14AF0F82-5E90-4b02-9381-2D7E16398E79" VC_GNMC="单据报表" VC_SJLX=""  VC_GJSJLX="" VC_ZPM="" VC_GJZPM="" VCS_FGNID="00000000-0000-0000-0000-000000000001" VCS_JYBZ="False"  VC_ZPURI="" VC_GJZPURI="" VCS_GNZPQYZTID="1" VC_GNBZ=""  VC_CS="" VC_JDXH="H">
      <Record  VCS_GNID="14AF0F82-5E90-4b02-9381-3D7E16398E38" VC_GNMC="Report_GiftInfo" VC_SJLX="FVM_DJBB"  VC_GJSJLX="" VC_ZPM="WX.Data.FViewModel.JXC.DJBB" VC_GJZPM="WX.Data.FViewModelAdvance.JXC.DJBB" VCS_FGNID="14AF0F82-5E90-4b02-9381-2D7E16398E79" VCS_JYBZ="False"  VC_ZPURI="" VC_GJZPURI="" VCS_GNZPQYZTID="1" VC_GNBZ=""  VC_CS="" VC_JDXH="1"/>
    </Record>
    <Record  VCS_GNID="14AF0F82-5E90-4b02-9381-3D7E16398E79" VC_GNMC="单据报表Server" c="" VC_GJSJLX="" VC_ZPM="" VC_GJZPM=""  VCS_FGNID="00000000-0000-0000-0000-000000000001" VCS_JYBZ="False"  VC_ZPURI="" VC_GJZPURI="" VCS_GNZPQYZTID="1" VC_GNBZ=""  VC_CS=""  VC_JDXH="I">
       <Record  VCS_GNID="14AF0F82-5E90-4b02-9381-5D7E16398E38" VC_GNMC="SBIIMS Regressions" VC_SJLX="FVM_FWQBB" VC_GJSJLX="" VC_ZPM="WX.Data.FViewModel.JXC.DJBB" VC_GJZPM="WX.Data.FViewModelAdvance.JXC.DJBB"  VCS_FGNID="14AF0F82-5E90-4b02-9381-3D7E16398E79" VCS_JYBZ="False"  VC_ZPURI="" VC_GJZPURI="" VCS_GNZPQYZTID="1" VC_GNBZ=""  VC_CS="http://192.168.2.199/Reports/Pages/Report.aspx?ItemPath=%2fSBIIMS%2fRegressions" VC_JDXH="1"/>
       <Record  VCS_GNID="14AF0F82-5E90-4b02-9381-5D7E16398E39" VC_GNMC="SBIIMS Unplanned Work" VC_SJLX="FVM_FWQBB" VC_GJSJLX="" VC_ZPM="WX.Data.FViewModel.JXC.DJBB" VC_GJZPM="WX.Data.FViewModelAdvance.JXC.DJBB"  VCS_FGNID="14AF0F82-5E90-4b02-9381-3D7E16398E79" VCS_JYBZ="False"  VC_ZPURI="" VC_GJZPURI="" VCS_GNZPQYZTID="1" VC_GNBZ=""  VC_CS="http://192.168.2.199/Reports/Pages/Report.aspx?ItemPath=%2fSBIIMS%2fUnplanned+Work" VC_JDXH="2"/>
    </Record>
*)

(*
数据初始化
let parentGuid=Guid.NewGuid()
let xbh=ref 1000M
[
"JXC","JHGL","CGJH","进货管理","采购进货"                                    
"JXC","JHGL","CGTH","进货管理","采购退货"                                                                
"JXC","JHGL","WLZW","进货管理","往来帐务" 
"JXC","JHGL","DJCX","进货管理","采购单据查询"  
"JXC","KCGL","WLZW","进货管理","库存往来查询"   
      
"JXC","XSGL","SPXS","销售管理","商品销售"   
"JXC","XSGL","POS","销售管理","POS销售"                                         
"JXC","XSGL","XSTH","销售管理","顾客退货"            
"JXC","XSGL","XSHH","销售管理","销售换货"                                  
"JXC","XSGL","WLZW","销售管理","销售往来帐务"      
"JXC","XSGL","DJCX","销售管理","销售单据查询"    
"JXC","KCGL","WLZW","销售管理","库存往来查询"    

"JXC","KCGL","SPCF","库存管理","商品拆分"                                   
"JXC","KCGL","SPKB","库存管理","商品捆绑"    
"JXC","KCGL","SPBY","库存管理","商品报损" 
"JXC","KCGL","SPBS","库存管理","商品报溢"                                    
"JXC","KCGL","KCPD","库存管理","库存盘点"    
"JXC","KCGL","KCPDX","库存管理","库存盘点X" 
"JXC","KCGL","KCDB","库存管理","库存调拨"                       
"JXC","KCGL","KCYJ","库存管理" ,"库存预警"                                                
"JXC","KCGL","WLZW","库存管理","库存往来帐"   
"JXC","KCGL","KCCX","库存管理","库存查询"       
"JXC","KCGL","DJCX","库存管理","库存单据查询"      
     
"JXC","JHGL","WLZW","统计报表","供货商供货统计"   
"JXC","TJBB","SPCG","统计报表","商品采购统计"   
"JXC","TJBB","CGYCG","统计报表","采购员采购统计"   
"JXC","TJBB","KCCB","统计报表","库存成本统计"   
"JXC","XSGL","WLZW","统计报表","客户消费统计"  
"JXC","TJBB","SPXS","统计报表","商品销售统计"       
"JXC","TJBB","XSYXS","统计报表","销售员销售统计"   
"JXC","TJBB","XSPH","统计报表","商品销售排行"                                                
"JXC","TJBB","YYFX","统计报表","营业分析"    

"JXC","ZHGL","CWGL","综合管理","财务综合管理"  
"JXC","ZHGL","XZGL","综合管理","薪资管理"  
"JXC","ZHGL","JYFYGL","综合管理","经营费用管理"  
"JXC","ZHGL","BJGL","综合管理","报价管理"  
"JXC","ZHGL","HTGL","综合管理","合同管理"         
"JXC","ZHGL","GHSGL","综合管理","供货商管理"    
"JXC","ZHGL","KHGL","综合管理","客户管理"      
"JXC","ZHGL","HYGL","综合管理","会员管理" 
"JXC","ZHGL","JYFGL","综合管理","经营交易方管理"     
"JXC","ZHGL","YWYGL","综合管理","业务员管理"         
"JXC","ZHGL","XFPGL","综合管理","消费品管理"         
"JXC","ZHGL","FJPGL","综合管理","废旧品管理"               
"JXC","ZHGL","GDZCGL","综合管理","固定资产管理"                                  
"JXC","ZHGL","KHJH","综合管理","客户借货管理"                            
"JXC","ZHGL","SJTX","综合管理","事件提醒"                                    

"JXC","JBXX","SPWH","基本信息管理","商品信息维护"                  
"JXC","JBXX","GHSWH","基本信息管理","供货商信息维护"       
"JXC","JBXX","KHWH","基本信息管理","客户信息维护"     
"JXC","JBXX","JYFWH","基本信息管理","交易方信息维护"    
"JXC","JBXX","YGWH","基本信息管理","员工信息维护"    
"JXC","JBXX","CZYWH","基本信息管理","操作员信息维护"         
"JXC","JBXX","CKWH","基本信息管理","仓库信息维护"                                         
"JXC","JBXX","XFPWH","基本信息管理","消费品信息维护"                  
"JXC","JBXX","FJPWH","基本信息管理","废旧品信息维护"                                         
"JXC","JBXX","ZCLBWH","基本信息管理","固定资产类别维护"          

"JXC","ZHBB","DJBB","综合报表管理","单据报表"          
"JXC","ZHBB","QDBB","综合报表管理","清单报表"       
"JXC","ZHBB","TBBB","综合报表管理","图表报表"    
"JXC","ZHBB","DCBB","综合报表管理","导出"    
                              
"JXC","XTGL","BFHF","系统管理","备份恢复"             
"JXC","XTGL","YZWJS","系统管理","月账务结算"                         
"JXC","XTGL","NZWJS","系统管理","年账务结算"    
"JXC","XTGL","XTRZ","系统管理","系统日志"                                   
"JXC","XTGL","XTCSH","系统管理","系统初始化"                              
"JXC","XTGL","QCJZ","系统管理","期初建账"             
"JXC","XTGL","TMDY","系统管理","条码打印"                      
                      
"AC","","CZYJSGL","访问控制","操作员角色管理"
"AC","","JSGNGL","访问控制","角色功能管理"
"AC","","JSQXGL","访问控制","角色权限管理" 
"AC","","CZYWH","访问控制","操作员维护"
"AC","","GNWH","访问控制","功能维护" 
"AC","","JSWH","访问控制","角色维护" 
"AC","","QXWH","访问控制","权限维护" 
]
|>Seq.groupBy (fun (_,_,_,a,_)->a)
|>Seq.iteri (fun i (a,b)->
    match Guid.NewGuid() with
    | x ->
        match new T_GNJD() with
        | y ->
            y.C_ID<-x
            y.C_FID<-parentGuid 
            y.C_GXRQ<-DateTime.Now
            y.C_CJRQ<-DateTime.Now
            xbh:= !xbh+1M
            y.C_XBH<- !xbh
            y.C_MC<-a 
            y.C_MCJM<- a.ToChinesePY()
            y.C_JDJB<-1uy 
            y.C_JDXH<- i 
            y.C_YZBZ<-false 
            y.C_JYBZ<-false
            y.C_ZPQYZT<- 0uy
            y.C_SJLX<-""
            y.C_GJSJLX<-""
            y.C_LJ<-"" 
            y.C_CS<-""
            y.C_ZPM<-""
            y.C_GJZPM<-"" 
            y.C_ZPURI<-""
            y.C_GJZPURI<-""
            y.C_ZPLJ<-""
            y.C_MMKJ<-""
            y.C_BZ<-""
            sb01.AddToT_GNJD(y)
            b
            |>Seq.iteri (fun j (m1,m2,m3,m4,m5) ->
                match new T_GNJD() with
                | z ->
                    z.C_ID<-Guid.NewGuid()
                    z.C_FID<-x 
                    z.C_GXRQ<-DateTime.Now
                    z.C_CJRQ<-DateTime.Now
                    xbh:= !xbh+1M
                    z.C_XBH<- !xbh
                    z.C_MC<-m5 
                    z.C_MCJM<-m5.ToChinesePY()
                    z.C_JDJB<-2uy 
                    z.C_JDXH<- j 
                    z.C_YZBZ<-true 
                    z.C_JYBZ<-false
                    z.C_ZPQYZT<- 1uy
                    match 
                      (match m2 with NotNullOrWhiteSpace _ -> m2+"_"+m3 | _ ->m3),
                      (match m2 with NotNullOrWhiteSpace _ -> m1+"."+m2+"."+m3 | _ ->m1+"."+m3),
                      (match m2 with NotNullOrWhiteSpace _ -> m1+"_"+m2+"_"+m3 | _ ->m1+"_"+m3) with
                    | u,v,w ->
                        z.C_SJLX<- String.Format(@"FVM_{0}",u)
                        z.C_GJSJLX<-String.Format(@"FVM_{0}_Advance",u)
                        z.C_LJ<-"" 
                        z.C_CS<-""
                        z.C_ZPM<-String.Format(@"WX.Data.FViewModel.{0}",v)
                        z.C_GJZPM<-String.Format(@"WX.Data.FViewModelAdvance.{0}",v)
                        z.C_ZPURI<-String.Format(@"/WX.Data.View.ViewModelTemplate.{0};Component/VMT_{1}.xaml",v,w)
                        z.C_GJZPURI<-String.Format(@"/WX.Data.View.ViewModelTemplateAdvance.{0};Component/VMT_{1}_Advance.xaml",v,w)
                        z.C_ZPLJ<-""
                        z.C_MMKJ<-""
                        z.C_BZ<-""
                    sb01.AddToT_GNJD(z)
            )
    )
sb01.SaveChanges()|>ignore

match new T_GNJD() with
| y ->
    y.C_ID<-parentGuid
    y.C_FID<-DefaultGuidValue 
    y.C_GXRQ<-DateTime.Now
    y.C_CJRQ<-DateTime.Now
    y.C_XBH<- 1000M
    y.C_MC<-"系统功能" 
    y.C_MCJM<-"系统功能".ToChinesePY()
    y.C_JDJB<- 0uy 
    y.C_JDXH<- 0 
    y.C_YZBZ<-false 
    y.C_JYBZ<-false
    y.C_ZPQYZT<- 0uy
    y.C_SJLX<-""
    y.C_GJSJLX<-""
    y.C_LJ<-"" 
    y.C_CS<-""
    y.C_ZPM<-""
    y.C_GJZPM<-"" 
    y.C_ZPURI<-""
    y.C_GJZPURI<-""
    y.C_ZPLJ<-""
    y.C_MMKJ<-""
    y.C_BZ<-""
    sb01.AddToT_GNJD y
    sb01.SaveChanges()|>ignore

*)

