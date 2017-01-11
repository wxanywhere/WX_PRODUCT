namespace WX.Data.CodeAutomation
open System
open WX.Data.Helper

(*
module main=
  [<STAThread>]
  [<EntryPoint>]
  let main args=
    [
    //@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_SJJK\WX.Data.DataModel.Frame.SJJK\WX.Data.DataModel.Frame.SJJK.csproj"
    //@"C:\Users\zhou\Documents\Visual Studio 2013\Projects\ConsoleApplication9\ClassLibrary1\ClassLibrary1.csproj"
    @"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.DataModel.Frame\WX.Data.DataModel.Frame.csproj"
    ]
    |>MSBuild.INS.Build
    (*
    [@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.BusinessEntities.Frame"]
    |>MSBuild.INS.BuildForDataAccessAutomationLayers
    *)
    (*
    (
    ".APC",
    "SBIIMS_APC",
    @"D:\TempWorkspace\AutomationTesting",
    false
    )
    |>DAGeneratorX.GenerateCodeFile
    |>ignore
    *)
    0
*)
(*
module main=
  [<STAThread>]
  [<EntryPoint>]
  let main args=
    DatabaseInformation.GetTableInfo
    |>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
    |>Seq.map (fun a->a.TABLE_NAME)
    |>Seq.toList
    |>Generator.GenerateCodeFile ".VC" "SBIIMS_VC" @"D:\Workspace\SBIIMS\SBIIMS_VC\BaseAutomation" None
    |>ignore
    0
*)
(*
module main=
  [<STAThread>]
  [<EntryPoint>]
  let main args=
    DatabaseInformation.GetTableInfo
    |>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
    |>Seq.map (fun a->a.TABLE_NAME)
    |>Seq.toList
    |>Generator.GenerateCodeFile ".AC" "SBIIMS_AC" @"D:\Workspace\SBIIMS1\SBIIMS_AC\BaseAutomation" None
    |>ignore
    0
*)
    (*
    [
    @"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_CGFX"
    ]
    |>MSBuild.INS.BuildForViewLayers
    0
*)
(*
module main=
  [<STAThread>]
  [<EntryPoint>]
  let main args=
    DatabaseInformation.GetTableInfo
    //|>Seq.filter (fun a->a.TABLE_TYPE<>"BASE TABLE")
    //|>Seq.filter (fun a->not<| a.TABLE_NAME.EndsWith("?"))
    |>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
    |>Seq.map (fun a->a.TABLE_NAME)
    |>Seq.toList
    |>Generator.GenerateCodeFile ".AccessControl" "SBIIMS_AC" @"D:\Workspace\SBIIMS"
    |>ignore
    0
*)

(*正确
module main=
  [<STAThread>]
  [<EntryPoint>]
  let main args=
    DatabaseInformation.GetTableInfo
    |>Seq.filter (fun a->a.TABLE_NAME<>"sysdiagrams")
    |>Seq.map (fun a->a.TABLE_NAME)
    |>Seq.toList
    |>fun a ->
        match DatabaseInformation.ValidateTablesDesign a|>Seq.filter (fun (a,_)->not a)|>Seq.toList with //过滤！ 除了错误之外，还可以显示一些警告信息
        | x when x.Length >0 ->ObjectDumper.Write(x,1);failwith "The tables design have some problems! "
        | x ->ObjectDumper.Write(x,1)
        a
    |>Generator.AttachRelatedInfo
    //|>Seq.map (fun a->ObjectDumper.Write(a.TableName)|>ignore,ObjectDumper.Write(a.ColumnConditionType)|>ignore)
    |>DataEntitiesCodingAdvance.GetCode
    //|>DataAccessCodingAdvance.GetCode 
    //|>DataAccessCodingAdvanceWithoutVariable.GetCode  "SBIIMSAC"
    //|>BusinessLogicCodingAdvance.GetCode
    //|>ServiceContractCodingAdvance.GetCode
    //|>Clipboard.SetText
    |>ignore
    0
*)

(* Generate code auto
//Genrate code tool navigation
http://davidhayden.com/blog/dave/category/15.aspx?Show=All

//Database Schemal
http://www.davidhayden.com/blog/dave/archive/2006/01/15/2734.aspx


http://msdn.microsoft.com/en-us/library/ms254934(VS.80).aspx
Working with the GetSchema Methods  
*)