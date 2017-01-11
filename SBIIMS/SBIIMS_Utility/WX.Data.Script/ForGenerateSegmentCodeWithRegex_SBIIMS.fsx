

#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Windows.Forms.dll"

open System
open System.IO
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Text.RegularExpressions
open System.Data
open System.Windows.Forms


#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"

open WX.Data.Helper
open WX.Data

#load "ForGenerateSegmentCodeWithRegexCommon_SBIIMS.fsx"
open WX.Data.Script

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
//========================================================
//Generate code from IDataAccess Layer
//========================================================

//-----------------------------------------------------------------------------------------------------------
//从数据访问接口方法获取代码
(
//文件名须按代码层顺，只需提供数据访问层类型名称及业务逻辑层类型名称
[  
"DA_KCGL_SPCF_BusinessAdvance"   //数据访问层类型名
"BL_KCGL_SPCF_Advance"          //业务逻辑层类型名
]
,
//数据访问层的接口代码
@"
  abstract AuditKCGL_SPCF:BD_TV_KCGL_SPCF_DJ_Advance[] ->BD_Result
"
)
|>generateCodeFromMembers
|>Clipboard.SetText


//-----------------------------------------------------------------------------------------------------------
//从数据访问接口文件获取全类型代码
(
"SBIIMS_JXC",
"BL_KCGL_SPCF_Advance", //业务逻辑层类型名
@"D:\Workspace\SBIIMS\WX.Data.IDataAccessAdvance.JXC.KCGL.SPCF",  //接口文件目录名称
[
"IDA_KCGL_SPCF_BusinessAdvance"
"IDA_KCGL_SPCF_QueryAdvance"
]
)
|>generateFullTypeCodeFromFiles
|>Clipboard.SetText

//===========================================
//从现有BD文件中提取字段模板
DirectoryInfo  @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC\WX.Data.BusinessDataEntitiesAdvance.JXC\BDAdvance"
|>fun a->a.GetFiles()
|>Seq.map (fun a->
    seq{
      yield a.Name
      match File.ReadFile a.FullName with
      | x ->
          match Regex.Match (x, @"^\s+(val\s+[\w\W\n]+)$",RegexOptions.Multiline)  with
          | y  when y.Groups.Count>1 ->
              match y.Groups.[1].Value with
              | z ->
                   for m in Regex.Split (z,@"\(\*[\w\W\n]*\*\)",RegexOptions.Multiline) do //先去除(*...*)的注释
                     for n in  Regex.Split (m.Trim(),@"\s*\n\s*",RegexOptions.Multiline) do //注释"//"通过行匹配去除
                       if String.IsNullOrWhiteSpace n|>not  then //yield n.Trim()
                         match Regex.Match (n, @"^\s*val\s+mutable\s+[a-zA-Z\s]*_([a-z-A-Z_]+)\:([a-zA-Z_]+)",RegexOptions.Singleline)  with     //数组[]前可以有空格, 已经考虑了可选参数 * ?Parameter
                         | w when w.Groups.Count>2 ->
                             match w.Groups.[1].Value,w.Groups.[2].Value with
                             | v1,v2 ->
                                 yield String.Format(@"""{0}"",""{1}"",{2},""{3}""",v1,v2,
                                   match v2 with
                                   | StartsWithIn ["null"] _ ->"true"
                                   | _ ->"false"
                                   ,
                                   String.Empty)
                         | _ ->()
          | _ ->()
      yield "//========================================="
    }
    )
|>Seq.iter (fun a->ObjectDumper.Write a) 





