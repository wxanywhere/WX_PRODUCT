module WX.Data.Script

#r "PresentationFramework"
#r "System.Xml.dll"
#r "System.Xml.Linq.dll"
open System
open System.Linq
open System.IO
open System.Xml
open System.Xml.XPath  //必须的
open System.Xml.Linq
open System.Text.RegularExpressions


#I  @"K:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"K:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data
//=========================================
(*
Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx
Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx

如何使用正则表达式搜索
http://technet.microsoft.com/zh-cn/library/ms174214.aspx

正则表达式语言元素
http://msdn.microsoft.com/zh-cn/library/az24scfc.aspx


常用正则表达式大全！（例如：匹配中文、匹配html） 
http://www.cnblogs.com/guiliangfeng/archive/2009/04/13/1434696.html
*)

(*
//xml Query
http://msdn.microsoft.com/en-us/library/bb308960.aspx

XPath Examples@@@@@@@@@
http://msdn.microsoft.com/en-us/library/ms256086.aspx

How to: Query LINQ to XML Using XPath
http://msdn.microsoft.com/en-us/library/bb387057.aspx

.NET Language-Integrated Query for XML Data
http://msdn.microsoft.com/en-us/library/bb308960.aspx
*)

//======================================

//=======================================
//增加或移除项目的Referenc,当然也可以使用正则表达式进行替换处理，不过正则表达式比较适合于Reference的移除，且只能全部一起替换，无条件控制
//收集所指定目录及指定文件特征的所有文件
let CollectFilesWithRegex (sourceDirectoryPaths:string seq,fileNamePatterns:string seq) =
  let rec GetFileInfo (sourceDirectories:DirectoryInfo seq)=    
    seq{   
        for a in sourceDirectories do
          for b in a.GetFiles() do
            match fileNamePatterns|>Regex.IsMatchIn b.Name with //不能直接比较两个实例sourceDirectory，targetDirectory
            | true -> 
                yield b
            | _ ->()
          yield! GetFileInfo (a.GetDirectories())
    }
  match sourceDirectoryPaths|>Seq.map (fun a->DirectoryInfo a) with
  | x ->GetFileInfo x

(*
(
[  //sourceDirectoryPaths
@"D:\Workspace\SBIIMS\SBIIMS_Link"
],
[
@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$"   //不包含"ViewModelTemplate", http://msdn.microsoft.com/en-us/library/az24scfc.aspx，
]
)
|>CollectFilesWithRegex
|>Seq.iter (fun a->ObjectDumper.Write a.FullName)
*)

//在VS2010的项目中在条件节点前插入指定内容(主要针对组件引用)
let InsertElement  (sourceDirectoryPaths:string seq, fileNamePatterns:string seq, locationElementContent:string, insertableContent:string, insertBefore:bool)= 
  seq{
    for m in CollectFilesWithRegex (sourceDirectoryPaths,fileNamePatterns) do
      match XElement.Load (m.FullName) with
      | x ->
          match x.Name.NamespaceName with
          | y ->
              let ns=XNamespace.Get(y)
              let nt = new NameTable()
              let nsmgr = new XmlNamespaceManager(nt)
              nsmgr.AddNamespace("wx",y)
              match XElement.Parse(insertableContent,LoadOptions.None) with
              | Null ->()
              | za ->
                  match 
                    match 
                      za.Name.LocalName,
                      za.Attributes()
                      |>Seq.fold (fun r a->r+(match r with NotNullOrWhiteSpace _ ->"and" | _ ->String.Empty)+String.Format(@"@{0}=""{1}""",a.Name.LocalName,a.Value)) String.Empty
                      ,
                      match za.HasElements,za.IsEmpty with  //<A P1=""wx""></A> IsEmpty为false, <A P1=""wx""/> IsEmpty为True
                      | y1, y2 when y1 || y2 ->null
                      | _ ->za.Value
                      with
                    | x1, NotNullOrWhiteSpace x2, NotNull x3 -> //x3不会为空
                        String.Format(@"{0}[{1} and text()=""{2}""]",x1,x2,x3)   //text()可以改为'.'
                    | x1, NotNullOrWhiteSpace x2, _ ->
                        String.Format(@"{0}[{1}]",x1,x2)   
                    | x1,  _, NotNull x3->
                        String.Format(@"{0}[text()=""{1}""]",x1,x3)
                    | x1,  _, _->
                        String.Format(@"{0}",x1)
                    with
                  | xc ->
                      match x.XPathSelectElement( String.Format(@"//wx:{0}",xc),nsmgr) with //Right, x.XPathSelectElement( @"//wx:Reference[@Include=""WX.Data.View.Controls.LoadingControl""]",nsmgr)
                      | NotNull _ ->() //存在时不插入
                      | Null ->
                          match XElement.Parse(locationElementContent,LoadOptions.None) with
                          | Null ->()
                          | xa ->
                              match 
                                match 
                                  xa.Name.LocalName,
                                  xa.Attributes()
                                  |>Seq.fold (fun r a->r+(match r with NotNullOrWhiteSpace _ ->"and" | _ ->String.Empty)+String.Format(@"@{0}=""{1}""",a.Name.LocalName,a.Value)) String.Empty
                                  ,
                                  match xa.HasElements,xa.IsEmpty with  //<A P1=""wx""></A> IsEmpty为false, <A P1=""wx""/> IsEmpty为True
                                  | y1, y2 when y1 || y2 ->null
                                  | _ ->xa.Value
                                  with
                                | x1, NotNullOrWhiteSpace x2, NotNull x3 -> //x3不会为空
                                    String.Format(@"{0}[{1} and text()=""{2}""]",x1,x2,x3)   //text()可以改为'.'
                                | x1, NotNullOrWhiteSpace x2, _ ->
                                    String.Format(@"{0}[{1}]",x1,x2)   
                                | x1,  _, NotNull x3->
                                    String.Format(@"{0}[text()=""{1}""]",x1,x3)
                                | x1,  _, _->
                                    String.Format(@"{0}",x1)
                                with
                              | yc->
                                  match x.XPathSelectElement( String.Format(@"//wx:{0}",yc),nsmgr) with //Right, x.XPathSelectElement( @"//wx:Reference[@Include=""WX.Data.View.Controls.LoadingControl""]",nsmgr)
                                  | Null ->()
                                  | z ->
                                      let rec ModifyElementNamespace (sourceXElement:XElement)=
                                        match sourceXElement with
                                        | s ->
                                            s.Name<-ns+s.Name.LocalName //XName={XNamespace}+"LocalName"(较为特殊), 或使用XName.Get("LocalName","namespace")   
                                            for m in s.Elements() do ModifyElementNamespace m
                                      ModifyElementNamespace za
                                      match insertBefore with
                                      | true ->z.AddBeforeSelf za
                                      | _ ->z.AddAfterSelf za
                                      x.Save(m.FullName,SaveOptions.OmitDuplicateNamespaces)
                                      yield m.FullName
  }
  |>Seq.toArray

(*
(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
@"D:\Workspace\SBIIMS\SBIIMS_AC "
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
],
[
@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$"       //不包含"ViewModelTemplate", http://msdn.microsoft.com/en-us/library/az24scfc.aspx，
],
//locationElementContent
@"
    <Reference Include=""WX.Data.View.Controls.Command"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.Command.dll</HintPath>
    </Reference>
"
,   
//Insertable Content
@"                                                                                                        
    <Reference Include=""WX.Data.View.Controls.LoadingControl"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
    </Reference>
"
)
|>InsertElement
|>Seq.iter (fun a->ObjectDumper.Write a)
*)
//-------------------------------------------------------------------------------------------------------
//修改VS2010的项目属性组
let ModifyVSProjectPropertyGroup  (sourceDirectoryPaths:string seq, fileNamePatterns:string seq, locationElementContent:string, newPropertyElementContent:string, insertBefore:bool)= 
  seq{
    for m in CollectFilesWithRegex (sourceDirectoryPaths,fileNamePatterns) do
      match XElement.Load (m.FullName) with
      | x ->
          match x.Name.NamespaceName with
          | y ->
              let ns=XNamespace.Get(y)
              let nt = new NameTable()
              let nsmgr = new XmlNamespaceManager(nt)
              nsmgr.AddNamespace("wx",y)
              match XElement.Parse(newPropertyElementContent,LoadOptions.None) with
              | Null ->()
              | z ->
                  match x.XPathSelectElement( String.Format(@"//wx:{0}",z.Name.LocalName),nsmgr) with
                  | Null -> //没有该属性时插入该属性
                      match XElement.Parse(locationElementContent,LoadOptions.None) with
                      | Null ->()
                      | xa ->
                          match 
                            xa.Name.LocalName,
                            xa.Attributes()
                            |>Seq.fold (fun r a->r+(match r with NotNullOrWhiteSpace _ ->"and" | _ ->String.Empty)+String.Format(@"@{0}=""{1}""",a.Name.LocalName,a.Value)) String.Empty
                            ,
                            match xa.HasElements,xa.IsEmpty with  //<A P1=""wx""></A> IsEmpty为false, <A P1=""wx""/> IsEmpty为True
                            | y1, y2 when y1 || y2 ->null
                            | _ ->xa.Value
                            with
                          | x1, NotNullOrWhiteSpace x2, NotNull x3 -> //x3不会为空
                              String.Format(@"{0}[{1} and text()=""{2}""]",x1,x2,x3)   //text()可以改为'.'
                          | x1, NotNullOrWhiteSpace x2, _ ->
                              String.Format(@"{0}[{1}]",x1,x2)   
                          | x1,  _, NotNull x3->
                              String.Format(@"{0}[text()=""{1}""]",x1,x3)
                          | x1,  _, _->
                              String.Format(@"{0}",x1)
                          |>fun a->
                              ObjectDumper.Write (a+"------------------------")
                              match x.XPathSelectElement( String.Format(@"//wx:{0}",a),nsmgr) with
                              | Null ->()
                              | ya ->
                                  z.Name<-ns+z.Name.LocalName //使该元素的命名空间和当前项目文件的命名空间保持一致，XName={XNamespace}LocalName, 或使用XName.Get("LocalName","namespace")
                                  match insertBefore with
                                  | true ->ya.AddBeforeSelf z
                                  | _ ->ya.AddAfterSelf z
                  | ya -> //修改存在的属性元素
                      for m in ya.Attributes()|>Seq.toArray do
                        m.Value<-z.Attribute(XName.Get(m.Name.LocalName)).Value //获取属性时不需要命名空间，无属性时应报错
                      ya.Value<-z.Value
                  x.Save(m.FullName,SaveOptions.OmitDuplicateNamespaces)
                  yield m.FullName
  }
  |>Seq.toArray
(*
(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
@"D:\Workspace\SBIIMS\SBIIMS_AC "
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
@"D:\Workspace\SBIIMS\SBIIMS_Frame"
@"D:\Workspace\SBIIMS\SBIIMS_Link"
//@"D:\TempWorkspace\c"
],
[
@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
],  //插入时前面的元素。如果需要新增，将插入到该元素名称的后面
@"    
<TargetFrameworkVersion>v4.0</TargetFrameworkVersion>
" 
, 
//newPropertyElementContentt
@"                                                                                                        
<TargetFrameworkProfile>Client</TargetFrameworkProfile>
"
)
|>ModifyVSProjectPropertyGroup
|>Seq.iter (fun a->ObjectDumper.Write a)
*)
//-------------------------------------------------------------------------------------------------------
(*
@"
<A P1=""wx"">
 <B P2=""sssss"">..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.BusinessBase.dll</B>
</A>
"
|>fun a->
    match XElement.Parse(a) with
    | x ->
       match x.XPathSelectElement(@"//B[@P2=""sssss"" and text()=""..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.BusinessBase.dll""]") with  //text() 和. 
       | y -> 
         ObjectDumper.Write (@"//B[text()=""..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.BusinessBase.dll""]")
         ObjectDumper.Write (y.Name)
       ObjectDumper.Write (x.Attribute(XName.Get("P1")).Value) 
       ObjectDumper.Write (x.d)
*)
//-----------------------------------------------------------------------------------
//替换指定的元素，多个相同的元素将替换为一个元素，所以可用于删除重复的元素
let ReplaceElements  (sourceDirectoryPaths:string seq, fileNamePatterns:string seq, oldNewElementContents:(string*string) seq)= 
  seq{
    for m in CollectFilesWithRegex (sourceDirectoryPaths,fileNamePatterns) do
      match XElement.Load (m.FullName) with
      | x ->
          match x.Name.NamespaceName with
          | y ->
              let ns=XNamespace.Get(y)
              let nt = new NameTable()
              let nsmgr = new XmlNamespaceManager(nt)
              nsmgr.AddNamespace("wx",y)
              for (mx,my) in oldNewElementContents do
                match XElement.Parse(mx,LoadOptions.None),XElement.Parse(my,LoadOptions.None) with 
                | Null, _
                | _, Null ->()
                | xa,xb ->
                    let rec ModifyElementNamespace (sourceXElement:XElement)=
                      match sourceXElement with
                      | s ->
                          s.Name<-ns+s.Name.LocalName //XName={XNamespace}+"LocalName"(较为特殊), 或使用XName.Get("LocalName","namespace")   
                          for m in s.Elements() do ModifyElementNamespace m
                    ModifyElementNamespace xa
                    ModifyElementNamespace xb
                    match 
                      match 
                        xa.Name.LocalName,
                        xa.Attributes()
                        |>Seq.fold (fun r a->r+(match r with NotNullOrWhiteSpace _ ->"and " | _ ->String.Empty)+String.Format(@"@{0}=""{1}""",a.Name.LocalName,a.Value)) String.Empty
                        ,
                        match xa.HasElements,xa.IsEmpty with  //<A P1=""wx""></A> IsEmpty为false, <A P1=""wx""/> IsEmpty为True
                        | y1, y2 when y1 || y2 ->null
                        | _ ->xa.Value
                        with
                      | x1, NotNullOrWhiteSpace x2, NotNull x3 -> //x3不会为空
                          String.Format(@"{0}[{1} and text()=""{2}""]",x1,x2,x3)   //text()可以改为'.'
                      | x1, NotNullOrWhiteSpace x2, _ ->
                          String.Format(@"{0}[{1}]",x1,x2)   
                      | x1,  _, NotNull x3->
                          String.Format(@"{0}[text()=""{1}""]",x1,x3)
                      | x1,  _, _->
                          String.Format(@"{0}",x1)
                      with
                    | zc->
                        match x.XPathSelectElements(String.Format( @"//wx:{0}",zc),nsmgr) with
                        | HasNotElement _ ->()
                        | ya ->
                            match ya|>Seq.toList with //节点更新时，须先toArray或toList, 否则操作不能完全正确
                            | h::[] ->
                                h.ReplaceWith xb   
                            | h::t ->
                                t|>Seq.iter (fun a->a.Remove ())   
                                h.ReplaceWith xb
                            | _ ->()
              yield m.FullName               
              x.Save(m.FullName,SaveOptions.OmitDuplicateNamespaces)  
  }
  |>Seq.toArray
(*
(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
@"D:\Workspace\SBIIMS\SBIIMS_AC "
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
@"D:\TempWorkspace\c"
],
[
//@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$"  
],
[   // oldNewElementContents, 旧元素和新元素相同时，可用于删除重复的节点元素
@"
    <Reference Include=""WX.Data.View.Controls.LoadingControl"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
    </Reference>
   ",
  @"
    <Reference Include=""WX.Data.View.Controls.LoadingControl"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.View.Controls.LoadingControl.dll</HintPath>
    </Reference>
    "
]
)
|>ReplaceElements
|>Seq.iter (fun a->ObjectDumper.Write a)
*)
//---------------------------------------------------------------------------------------------------
//删除指定的元素, 只可操作两个个层级的节点？？？ 慎用！！！
let DeleteElements  (sourceDirectoryPaths:string seq, fileNamePatterns:string seq, deletionElementContents:string seq)= 
  seq{
    for m in CollectFilesWithRegex (sourceDirectoryPaths,fileNamePatterns) do
      match XElement.Load (m.FullName) with
      | x ->
          match x.Name.NamespaceName with
          | y ->
              let ns=XNamespace.Get(y)
              let nt = new NameTable()
              let nsmgr = new XmlNamespaceManager(nt)
              nsmgr.AddNamespace("wx",y)
              for mx in deletionElementContents do
                match XElement.Parse(mx,LoadOptions.None) with 
                | Null ->()
                | xa ->
                    match 
                      xa.Name.LocalName,
                      xa.Attributes()
                      |>Seq.fold (fun r a->r+(match r with NotNullOrWhiteSpace _ ->"and" | _ ->String.Empty)+String.Format(@"@{0}=""{1}""",a.Name.LocalName,a.Value)) String.Empty
                      ,
                      match xa.HasElements,xa.IsEmpty with  //<A P1=""wx""></A> IsEmpty为false, <A P1=""wx""/> IsEmpty为True
                      | y1, y2 when y1 || y2 ->null
                      | _ ->xa.Value
                      with
                    | x1, NotNullOrWhiteSpace x2, NotNull x3 -> //x3不会为空
                        String.Format(@"{0}[{1} and text()=""{2}""]",x1,x2,x3)   //text()可以改为'.'
                    | x1, NotNullOrWhiteSpace x2, _ ->
                        String.Format(@"{0}[{1}]",x1,x2)   
                    | x1,  _, NotNull x3->
                        String.Format(@"{0}[text()=""{1}""]",x1,x3)
                    | x1,  _, _->
                        String.Format(@"{0}",x1)
                    |>fun a->
                        //ObjectDumper.Write a
                        x.XPathSelectElements(String.Format(@"//wx:{0}",a),nsmgr) 
                        |>Seq.toArray //节点更新时，须先toArray或toList, 否则操作不能完全正确
                        |>Array.iter (fun b->b.Remove())
              x.Save(m.FullName,SaveOptions.OmitDuplicateNamespaces)
              yield m.FullName
  }
  |>Seq.toArray

(*
(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
//@"D:\Workspace\SBIIMS\SBIIMS_AC "
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
@"D:\TempWorkspace\c"
],
[
@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
],
[
@"
    <Reference Include=""WX.Data.BusinessBase"">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.BusinessBase.dll</HintPath>
    </Reference>"
]
)
|>DeleteElements
|>Seq.iter (fun a->ObjectDumper.Write a)
*)
//---------------------------------------------------------------------------------------------------