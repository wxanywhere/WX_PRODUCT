namespace WX.Data
open System
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomation

(*
注意不要使用PSeq.iteri 和PSeq.iter,否则会引起随机的丢失迭代的元素
*)

module CodeAutomationHelper=
  //开始使用没有问题，但后来报错TypeName=<StartupCode$WX-Data-CodeAutomation>.$CodeAutomationHelper     Message=The type initializer for '<StartupCode$WX-Data-CodeAutomation>.$CodeAutomationHelper' threw an exception.       Data=...        InnerException={ }      TargetSite={ }  StackTrace=   at WX.Data.CodeAutomationHelper.get_TableColumnDescriptionInfo()
  //发现是 DatabaseInformation.GetDJLX() 导致的问题，需要判断数据库是否有表T_DJLX
  //let TableColumnDescriptionInfo=DatabaseInformation.GetTableColumnDescription()   
  let DJLXInfo=
    match DatabaseInformation.GetTableInfo |>Seq.exists (fun a->a.TABLE_NAME="T_DJLX" ) with
    | true ->
        DatabaseInformation.GetDJLX()
    | _ -> []
  let getTableDescription tableName=DatabaseInformation.GetTableColumnDescription () |>Seq.find (fun a->a.TABLE_NAME=tableName)|>fun a->a.TABLE_DESCRIPTION
  let getColumnDescription tableName columnName= 
    match 
      //TableColumnDescriptionInfo
      DatabaseInformation.GetTableColumnDescription()
      |>Seq.tryFind(fun a->a.TABLE_NAME=tableName && a.COLUMN_NAME=columnName)  with
    | Some y ->
        match y.COLUMN_DESCRIPTION with
        | x when String.IsNullOrEmpty x |>not && String.IsNullOrWhiteSpace x|>not->x
        | _ ->columnName 
    | _ ->String.Empty
  let getDJHPrefix code=DJLXInfo|>Seq.find (fun a->a.C_DM=code)|>fun a->a.C_QZ
  let getDJName code= DJLXInfo|>Seq.find (fun a->a.C_DM=code)|>fun a->a.C_LX

  let VariableNames=
    ['x';'y';'z';'u';'v';'w';'a';'b';'c']
