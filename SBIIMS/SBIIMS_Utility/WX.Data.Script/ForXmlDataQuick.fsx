

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

#load "ForXmlDataForVisualStudioLibrary.fsx" 
open WX.Data.Script

//=======================================
//增加或移除项目的Referenc,当然也可以使用正则表达式进行替换处理，不过正则表达式比较适合于Reference的移除，且只能全部一起替换，无条件控制
//收集所指定目录及指定文件特征的所有文件

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

//------------------------------------------------------------------
//在VS2010的项目中在条件节点前插入指定内容(主要针对组件引用)

(
[  //sourceDirectoryPaths
@"D:\Workspace\SBIIMS"
//@"D:\Workspace\SBIIMS\SBIIMS_AC "
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
//@"D:\Workspace\SBIIMS\SBIIMS_Base"
//@"D:\Workspace\SBIIMS\SBIIMS_Integration"
//@"D:\TempWorkspace\xml"
//@"D:\Workspace\Temp\AC"
],
[  //指定文件
//@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$"       //不包含"ViewModelTemplate", http://msdn.microsoft.com/en-us/library/az24scfc.aspx，
//@"^WX\.Data\.View\.JXC\.JBXX\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.JHGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.KCGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.XSGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.TJBB\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.ZHGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.FViewModel\.JXC\.JBXX\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.JHGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.KCGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.XSGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.TJBB\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.ZHGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.JBXX\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.JHGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.KCGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.XSGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.TJBB\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.ZHGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.JBXX\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.View\.ViewModelTemplateAdvance\.[A-Z]+\.[A-Z]+\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.View\.ViewModelTemplate\.[A-Z]+\.[A-Z]+\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.[A-Z]+\.[A-Z]+\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel[a-zA-Z]*\.JXC\.[A-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.[a-zA-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.[a-zA-Z\.]+\.[cf]sproj$"
@"^[a-zA-Z\.]+\.[cf]sproj$" 
//@"^WX\.Data\.FViewModelAdvance\.AC\.JSGNGLX\.fsproj$"
],
//locationElementContent
(*
<TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <TargetFrameworkVersion>v4.5.1</TargetFrameworkVersion>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration> //只使用第一个子元素作为查询条件，配合使用 正则表达式验正是否有遗漏"""^\<Project\s+ToolsVersion\=\"12\.0\"[\w\W\s]+2003\"\>\s*\n\s*\<PropertyGroup\>\s*$"""
  </PropertyGroup>
*)
"""
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
  </PropertyGroup>
"""
,   
//Insertable Content
"""                                                                                                        
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
""",
true   //true为插前， false为插后
)
|>InsertElement
|>Seq.iter (fun a->ObjectDumper.Write a)

//-------------------------------------------------------------------------------------------------------
//修改VS2010的项目属性组
(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
// @"D:\Workspace\SBIIMS\SBIIMS_AC "
// @"D:\Workspace\SBIIMS\SBIIMS_JXC"
// @"D:\Workspace\SBIIMS\SBIIMS_Frame"
// @"D:\Workspace\SBIIMS\SBIIMS_Link"
// @"D:\Workspace\SBIIMS\SBIIMS_Base"
// @"D:\Workspace\SBIIMS\SBIIMS_Integration"
@"K:\Workspace\XN\Git\XN\xnrpt"
],
[
//@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
@"^[a-zA-Z\.]+\.[c]sproj$" 
],  //插入时前面的元素。如果需要新增，将插入到该元素名称的后面
@"    
<PlatformTarget>AnyCPU</PlatformTarget>
" 
, 
//newPropertyElementContentt
@"                                                                                                        
<PlatformTarget>x86</PlatformTarget>
"
,
false //true为插前，false为插后
)
|>ModifyVSProjectPropertyGroup
|>Seq.iter (fun a->ObjectDumper.Write a)

//-------------------------------------------------------------------------------------------------------
@"
<A P1=""wx""></A>
"
|>fun a->
    match XElement.Parse(a) with
    | x ->ObjectDumper.Write x.IsEmpty

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

//-----------------------------------------------------------------------------------
//替换指定的元素，多个相同的元素将替换为一个元素，所以可用于删除重复的元素
(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
@"D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.Research.TypeProvider.Java"
//@"D:\Workspace\SBIIMS\SBIIMS_AC "
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
//@"D:\Workspace\SBIIMS\SBIIMS_Base"
//@"D:\Workspace\SBIIMS\SBIIMS_Integration"
//@"D:\Workspace\SBIIMS\SBIIMS_Utility"
//@"D:\Workspace\SBIIMS\SBIIMS_VC"
//@"D:\Workspace\SBIIMS\SBIIMS_FK"
//@"D:\Workspace\SBIIMS\SBIIMS_APC"
//@"D:\TempWorkspace\c"
],
[
//@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$" 
//@"^[a-zA-Z\.]+View[a-zA-Z\.]+\.[cf]sproj$" 
@"^[a-zA-Z\.]+\.[cf]sproj$" 
//@"^WX\.Data\.FViewModelAdvance\.AC\.JSGNGLX\.fsproj$"
],
[   // oldNewElementContents, 旧元素和新元素相同时，可用于删除重复的节点元素
"""
<SccProjectName>SAK</SccProjectName>
  """,
  """
  <SccProjectName>SAK</SccProjectName>
  """
]
)
|>ReplaceElements
|>Seq.iter (fun a->ObjectDumper.Write a)

//替换指定的元素，多个相同的元素将替换为一个元素，所以可用于删除重复的元素
(
[  //sourceDirectoryPaths
@"D:\Workspace\SBIIMS"
//@"D:\Workspace\SBIIMS\SBIIMS_AC "
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
//@"D:\Workspace\SBIIMS\SBIIMS_Base"
//@"D:\Workspace\SBIIMS\SBIIMS_Integration"
//@"D:\Workspace\SBIIMS\SBIIMS_Utility"
//@"D:\Workspace\SBIIMS\SBIIMS_VC"
//@"D:\Workspace\SBIIMS\SBIIMS_FK"
//@"D:\Workspace\SBIIMS\SBIIMS_APC"
//@"D:\TempWorkspace\c"
],
[
//@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$" 
//@"^[a-zA-Z\.]+View[a-zA-Z\.]+\.[cf]sproj$" 
//@"^[a-zA-Z\.]+\.[cf]sproj$" 
@"^[a-zA-Z\.]+\.fsproj$"
//@"^WX\.Data\.FViewModelAdvance\.AC\.JSGNGLX\.fsproj$"
],
[   // oldNewElementContents, 旧元素和新元素相同时，可用于删除重复的节点元素
(*
    <OutputPath>D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug\</OutputPath>
    <OutputPath>D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug\</OutputPath>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <TargetFSharpCoreVersion>4.3.0.0</TargetFSharpCoreVersion>
*)
"""
  <Choose>
    <When Condition="'$(VisualStudioVersion)' == '11.0'">
      <PropertyGroup Condition="Exists('$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.1\Framework\v4.0\Microsoft.FSharp.Targets')">
        <FSharpTargetsPath>$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.1\Framework\v4.0\Microsoft.FSharp.Targets</FSharpTargetsPath>
      </PropertyGroup>
    </When>
    <Otherwise>
      <PropertyGroup Condition="Exists('$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets')">
        <FSharpTargetsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets</FSharpTargetsPath>
      </PropertyGroup>
    </Otherwise>
  </Choose>
  """,
  """
  <Choose>
    <When Condition="'$(VisualStudioVersion)' == '11.0'">
      <PropertyGroup Condition="Exists('$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets')">
        <FSharpTargetsPath>$(MSBuildExtensionsPath32)\..\Microsoft SDKs\F#\3.0\Framework\v4.0\Microsoft.FSharp.Targets</FSharpTargetsPath>
      </PropertyGroup>
    </When>
    <Otherwise>
      <PropertyGroup Condition="Exists('$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets')">
        <FSharpTargetsPath>$(MSBuildExtensionsPath32)\Microsoft\VisualStudio\v$(VisualStudioVersion)\FSharp\Microsoft.FSharp.Targets</FSharpTargetsPath>
      </PropertyGroup>
    </Otherwise>
  </Choose>
  """
]
)
|>ReplaceElements
|>Seq.iter (fun a->ObjectDumper.Write a)


//批量替换
let entryDirectory=DirectoryInfo @"D:\Workspace\SBIIMS\SBIIMS_JXC"
seq{
  for n in entryDirectory.GetDirectories() do
    match n.Name.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries) with
    | [|a;b|] when a.ToUpper()=a && b.ToUpper()=b ->
        yield String.Format (".{0}.{1}",a,b), String.Format ("_{0}_{1}",a,b)
    | [|a;b;c|] when a.ToUpper()=a && b.ToUpper()=b && c.ToUpper()=c ->
        yield String.Format (".{0}.{1}.{2}",a,b,c), String.Format ("_{0}_{1}_{2}",a,b,c)
    | _ ->()
}
|>Seq.toArray
|>Seq.iter (fun (a,b) ->    
    //替换指定的元素，多个相同的元素将替换为一个元素，所以可用于删除重复的元素
    (
    [  //sourceDirectoryPaths
    Path.Combine(@"D:\Workspace\SBIIMS\SBIIMS_JXC",b.Remove(0,1))
    ],
    [
    //@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
    //@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$" 
    //@"^[a-zA-Z\.]+View[a-zA-Z\.]+\.[cf]sproj$" 
    @"^[a-zA-Z\.]+\.[cf]sproj$" 
    //@"^WX\.Data\.FViewModelAdvance\.AC\.JSGNGLX\.fsproj$"
    ],
    [   // oldNewElementContents, 旧元素和新元素相同时，可用于删除重复的节点元素
    """
    <Reference Include="WX.Data.BusinessDataEntities.JXC, Version=0.0.0.0, Culture=neutral, processorArchitecture=MSIL" />
      """,
    String.Format(
      """
    <Reference Include="WX.Data.BusinessEntities{0}">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.BusinessEntities{0}.dll</HintPath>
    </Reference>
      """,a)
    ]
    )
    |>ReplaceElements
    |>Seq.iter (fun a->ObjectDumper.Write a)
)

//-------------------------------------------------------------------------------------------


(
[  //sourceDirectoryPaths
@"D:\Workspace\SBIIMS\SBIIMS_AC "
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_DJCX"
//@"D:\TempWorkspace\c"
],
[ //匹配的文件文件模式
//@"^WX\.Data\.I?DataAccessAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.IDataAccessAdvance\.[A-Z]+\.[A-Z]+\.[A-Z]+\.[cf]sproj$"  
@"^WX\.Data\.IDataAccessAdvance\.[A-Z]+\.[A-Z]+\.[cf]sproj$"  
],
[   // oldNewElementContents, 旧元素和新元素相同时，可用于删除重复的节点元素
@"
    <Content Include=""Design.txt"" />
   ",
  @"
    <Content Include=""CodeAutomation.txt"" />
    "
]
)
|>ReplaceElements
|>Seq.iter (fun a->ObjectDumper.Write a)

//---------------------------------------------------------------------------------------------------
//删除指定的元素

(
[  //sourceDirectoryPaths
//@"D:\Workspace\SBIIMS"
"""J:\Workspace\SBMIIS_ProjectStructure"""
//@"D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.Research.TypeProvider.Java"
//@"D:\Workspace\SBIIMS\SBIIMS_AC"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC"
//@"D:\Workspace\SBIIMS\SBIIMS_Frame"
//@"D:\Workspace\SBIIMS\SBIIMS_Link"
//@"D:\Workspace\SBIIMS\SBIIMS_Base"
//@"D:\Workspace\SBIIMS\SBIIMS_Integration"
//@"D:\Workspace\SBIIMS\SBIIMS_VC"
//@"D:\Workspace\SBIIMS\SBIIMS_FK"
//@"D:\Workspace\SBIIMS\SBIIMS_APC"
],
[
//@"^WX\.Data\.View\.(?!ViewModelTemplate)[a-zA-Z\.]+\.[cf]sproj$" 
//@"^WX\.Data\.View\.JXC\.JBXX\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.JHGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.KCGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.XSGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.TJBB\.[A-Z]+\.csproj$"
//@"^WX\.Data\.View\.JXC\.ZHGL\.[A-Z]+\.csproj$"
//@"^WX\.Data\.FViewModel\.JXC\.JBXX\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.JHGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.KCGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.XSGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.TJBB\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.JXC\.ZHGL\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.View\.ViewModelTemplateAdvance\.[A-Z]+\.[A-Z]+\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.View\.AC\.[A-Z]+\.[A-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel\.[A-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModel[a-zA-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.(?!JBXX)[a-zA-Z]+\.[a-zA-Z]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.JXC\.[a-zA-Z\.]+\.[cf]sproj$"
//@"^WX\.Data\.FViewModelAdvance\.[a-zA-Z\.]+\.[cf]sproj$"
@"^[a-zA-Z0-9\.]+\.[cf]sproj$" 
],

[
(*
    <Reference Include="WX.Data.BusinessQueryEntities.Frame">
      <HintPath>..\..\..\SBIIMS_Assemblies\ClientDebug\WX.Data.BusinessQueryEntities.Frame.dll</HintPath>
    </Reference>
*)
"""
    <SccProjectName>SAK</SccProjectName>
"""
"""
    <SccProvider>SAK</SccProvider>
"""
"""
    <SccAuxPath>SAK</SccAuxPath>
"""
"""
    <SccLocalPath>SAK</SccLocalPath>
"""
]
)
|>DeleteElements
|>Seq.iter (fun a->ObjectDumper.Write a)

//---------------------------------------------------------------------------------------------------
Regex.IsMatch ("WX.Data.View.ViewMode.JXC.ZHGL.YWYGL.csproj",@"^WX\.Data\.View\.(?!ViewModel)[a-zA-Z\.]+\.[cf]sproj$")  //报

//==================================================

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
      String.Format (@"VC_SJLX=""FVM_{0}"" VC_GJSJLX=""FVM_{0}_Advance"" VC_ZPM=""WX.Data.FViewModel.{1}.{0}"" VC_GJZPM=""WX.Data.FViewModelAdvance.{1}.{0}"" VCS_FGNID=""14AF0F82-5E90-4b02-9381-2D7E16398E23"" VCS_JYBZ=""False"" VCS_TSBZ=""false""  VC_ZPURI=""/WX.Data.View.ViewModelTemplate.{1}.{0};Component/VMT_{1}_{0}.xaml"" VC_GJZPURI=""/WX.Data.View.ViewModelTemplateAdvance.{1}.{0};Component/VMT_{1}_{0}_Advance.xaml""",n,m)
      |>ObjectDumper.Write


let parentNodeTemplate= @"<Record  VCS_GNID=""{0}"" VC_GNMC=""{1}"" VC_SJLX="" ""  VC_GJSJLX="" "" VC_ZPM="" "" VC_GJZPM="" ""  VCS_FGNID=""{2}"" VCS_JYBZ=""false""  VCS_TSBZ=""false"" VC_ZPURI="" "" VC_GJZPURI="" "" VCS_GNZPQYZTID=""2"" VC_GNBZ="" ""  VC_CS="" "" VCS_CTBZ=""false"" VC_CTLX="" "" VC_CTZPM="" ""  VC_CTURI="" ""  VC_JDXH=""{3}"">"
let childNodeTemplate= @"    <Record  VCS_GNID=""{0}"" VC_GNMC=""{1}"" VC_SJLX=""FVM_{2}""  VC_GJSJLX=""FVM_{2}_Advance"" VC_ZPM=""WX.Data.FViewModel.{3}"" VC_GJZPM=""WX.Data.FViewModelAdvance.{3}"" VCS_FGNID=""{4}"" VCS_JYBZ=""false"" VCS_TSBZ=""false"" VC_ZPURI=""/WX.Data.View.ViewModelTemplate.{3};Component/VMT_{6}.xaml"" VC_GJZPURI=""/WX.Data.View.ViewModelTemplateAdvance.{3};Component/VMT_{6}_Advance.xaml"" VCS_GNZPQYZTID=""2"" VC_GNBZ="" ""  VC_CS="" ""  VCS_CTBZ=""false"" VC_CTLX="" "" VC_CTZPM="" ""  VC_CTURI="" ""  VC_JDXH=""{5}""/>"
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
//"JXC","KCGL","SPBY","库存管理","商品报损" 
//"JXC","KCGL","SPBS","库存管理","商品报溢"        
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

"JXC","ZHBB","DJBB","综合报表管理","单据报表"          
"JXC","ZHBB","QDBB","综合报表管理","清单报表"       
"JXC","ZHBB","TBBB","综合报表管理","图表报表"    
"JXC","ZHBB","DCBB","综合报表管理","导出"    

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