namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper
open WX.Data.Database

type DACodingLeafTablePartX=
  static member GetCodeWithLeafTableTemplate  (databaseInstanceName:string) (entityContextNamePrefix:string)  (tableRelatedInfo:TableRelatedInfoX)  (tableInfos:TableInfo[])=
    let sb=StringBuilder()
    let tableName=tableRelatedInfo.TableInfo.TABLE_NAME
    let tableColumns=
      tableRelatedInfo.TableInfo.TableColumnInfos
      |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let tableAsFKRelationships= //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
      tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos 
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.PK_TABLE)) 
    let tableAsPKRelationships= //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
      tableRelatedInfo.TableInfo.TablePrimaryKeyRelationshipInfos
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.FK_TABLE)) 
    let tableKeyColumns=tableRelatedInfo.TableInfo.TablePrimaryKeyInfos

    //WriteLog for update //子表更新写日志
    DACodingLeafTablePartX.GenerateWriteLogForUpdateCode  databaseInstanceName entityContextNamePrefix  tableName tableColumns  tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append|>ignore
    sb.AppendLine()|>ignore
    string sb

  //---------------------------------------------------------------------------------------------

  static member private GenerateWriteLogForUpdateCode (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.WriteLogForUpdate{12} (modelInstance:{13},executeBase,context,now)=
      match executeBase with
      | Null ->raise (new ArgumentNullException ""executeBase"")
      | x ->
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(x,context,now)"
        ,
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for (a,b) in 
          (tableKeyColumns,tableColumns)
          |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          do
          sbTem.AppendFormat(@"{0}=""+(modelInstance.{0}|>string)+""|",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
        | _ ->()
        sbTem.Insert(0,@"""")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        @""""+tableName+ @""""
        ,
        //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
        @""""+ String.Empty + @""""
        ,
        //{3} 
        (
        match columnConditionTypes with
        | ColumnConditionTypeContains [HasLSH;HasDJLSH;HasJYH] _->"5uy"   //无限制，均为单表操作
        | _ ->"5uy" 
        )
        ,
        //{4}
        (
        match columnConditionTypes with
        | ColumnConditionTypeContains [HasLSH;HasDJLSH;HasJYH] _-> //无限制，均为单表操作
            @""""+"单表处理"+ @""""
        | _ ->
            @""""+"单表处理"+ @""""      
        )
        ,
        //{5}
        "2uy"
        ,
        //{6}
        @"""更新"""
        ,
        //{7}
        String.Empty
        ,
        //{8}
        (
        match columnConditionTypes with
        | ColumnConditionTypeAllEquals [HasLSH] _->
            @"""更新"+ String.Empty + @"，编号为""+(modelInstance.C_XBH|>string)+""的记录"""
        | ColumnConditionTypeAllEquals [HasDJLSH] _->
            @"""更新"+ String.Empty + @"，单据号为""+(modelInstance.C_DJH|>string)+""的子表记录"""
        | ColumnConditionTypeAllEquals [HasJYH] _->
            @"""更新"+ String.Empty + @"，交易号为""+(modelInstance.C_JYH|>string)+""的记录"""
        | _ ->
            @"""更新"+ String.Empty + @"的记录"""
        )
        ,
        //{9}
        "null"
        ,
        //{10}
        "null"
        ,
        //{11}
        entityContextNamePrefix
        ,
        //{12}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{13}
        tableName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e