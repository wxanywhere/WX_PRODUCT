namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper
open WX.Data.Database

type DACodingMainChildOneLevelTablePartX=
  static member GetCodeWithMainChildTableOneLevelTemplate  (databaseInstanceName:string) (entityContextNamePrefix:string)  (tableRelatedInfo:TableRelatedInfoX) (tableInfos:TableInfo[])=
    let sb=StringBuilder()
    let mainTableName=tableRelatedInfo.TableInfo.TABLE_NAME
    let mainTableColumns=
      tableRelatedInfo.TableInfo.TableColumnInfos
      |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let mainTableAsFKRelationships=  //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
      tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.PK_TABLE)) 
    let mainTableAsPKRelationships= //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
      tableRelatedInfo.TableInfo.TablePrimaryKeyRelationshipInfos 
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.FK_TABLE)) 
    let mainTableKeyColumns= tableRelatedInfo.TableInfo.TablePrimaryKeyInfos
    let childTableName=tableRelatedInfo.LevelOneChildTableInfo.TABLE_NAME
    let childTableColumns=
      tableRelatedInfo.LevelOneChildTableInfo.TableColumnInfos
      |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let childTableAsFKRelationships=
      tableRelatedInfo.LevelOneChildTableInfo.TableForeignKeyRelationshipInfos
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.PK_TABLE)) 
    let childTableAsPKRelationships=
      tableRelatedInfo.LevelOneChildTableInfo.TablePrimaryKeyRelationshipInfos
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.FK_TABLE)) 
    let childTableKeyColumns=tableRelatedInfo.LevelOneChildTableInfo.TablePrimaryKeyInfos
    match tableRelatedInfo.ColumnConditionTypes with
    | ColumnConditionTypeContains [HasDJLSH] _ ->
        DACodingMainChildOneLevelTablePartX.GenerateSingleCreateCodeForMainChildOneLevelTablesWithDJLSH databaseInstanceName entityContextNamePrefix  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //SingleCreateCode
        sb.AppendLine()|>ignore
        DACodingMainChildOneLevelTablePartX.GenerateMultiCreateCodeForMainChildOneLevelTablesWithDJLSH databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //MultiCreateCode
        sb.AppendLine()|>ignore
    | _ ->
        DACodingMainChildOneLevelTablePartX.GenerateSingleCreateCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //SingleCreateCode
        sb.AppendLine()|>ignore
        DACodingMainChildOneLevelTablePartX.GenerateMultiCreateCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
        |>box|>sb.Append |>ignore //MultiCreateCode
        sb.AppendLine()|>ignore
    DACodingMainChildOneLevelTablePartX.GenerateSingleUpdateCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships childTableKeyColumns  tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //SingleUpdateCode
    sb.AppendLine()|>ignore
    DACodingMainChildOneLevelTablePartX.GenerateMultiUpdateCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships childTableKeyColumns  tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //MultiUpdateCode
    sb.AppendLine()|>ignore
    DACodingMainChildOneLevelTablePartX.GenerateDeleteCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableKeyColumns childTableName  tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //Delete
    sb.AppendLine()|>ignore
    DACodingMainChildOneLevelTablePartX.GenerateMultiDeleteCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableKeyColumns childTableName tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //MultiDelete
    sb.AppendLine()|>ignore
    match tableRelatedInfo.ColumnConditionTypes with
    | ColumnConditionTypeContains [HasDJLSH;HasPCInChild] x ->
        DACodingMainChildOneLevelTablePartX.GenerateSingleCreateCodeForMainChildOneLevelTablesWithPCProcess databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships
        |>box|>sb.Append |>ignore //SingleCreateCode with C_PC process
        sb.AppendLine()|>ignore
        DACodingMainChildOneLevelTablePartX.GenerateSingleUpdateCodeForMainChildOneLevelTablesWithPCProcess databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships childTableKeyColumns  tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //SingleUpdateCode with C_PC process
        sb.AppendLine()|>ignore
    | _ ->()
    //Special for CGJH 采购进货, 临时，可用Create..._WithPCProcess代替
    match mainTableName with
    | EqualsIn [JXCSHDJTableName;JHGLDJTableName] _ ->
        DACodingMainChildOneLevelTablePartX.GenerateCGJHSingleCreateCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships
        |>box|>sb.Append |>ignore //SingleCreateCode for CGJH 
        sb.AppendLine()|>ignore
        DACodingMainChildOneLevelTablePartX.GenerateCGJHSingleUpdateCodeForMainChildOneLevelTables databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships childTableKeyColumns  tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //SingleUpdateCode for CGJH
        sb.AppendLine()|>ignore
    | _ ->()
    //WriteLog for update //仅针对主子表的主表进行更新，
    DACodingMainChildOneLevelTablePartX.GenerateWriteLogForUpdateCode  databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns   mainTableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append|>ignore
    sb.AppendLine()|>ignore
    //WriteLog for Delete
    DACodingMainChildOneLevelTablePartX.GenerateWriteLogForDeleteCode  databaseInstanceName entityContextNamePrefix mainTableName mainTableColumns   mainTableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append|>ignore
    sb.AppendLine()|>ignore
    string sb

//------------------------------------------------------------------------------------------------------------------------------------------

  (*
        match businessEntity with
        | x ->
            DA_{10}Helper.UpdateLSH sb x.C_DJLX x.C_DJH 1M
            x
  //Adding, Modifying, and Deleting Objects (Entity Framework)
  // http://msdn.microsoft.com/en-us/library/bb738695.aspx
  *)
  static member private GenerateSingleCreateCodeForMainChildOneLevelTablesWithDJLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        match businessEntity with
        | x ->
            match DA_{10}Helper.GetDJH sb x.C_DJLX x.C_DJH now with
            | y ->
                x.C_DJH<-y
                (x.{12},x.C_DJH)|>result.GuidStrings.Add
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        match new {2}
         ({4}) with
        | entity{3} ->
            {5}  
            businessEntity.BD_{6}s|>Seq.iter(fun a->  
              match new {6}
               ({8}) with
              | child{7} ->
                  {9}
                  entity{3}.{6}.Add(child{7}))
            sb.{2}.AddObject(entity{3})
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} 
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=(match businessEntity.{0} with NotEquals DateTimeDefaultValue x ->x | _ ->now),",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              (* 已置前
              | z when z.ToLowerInvariant().EndsWith("string") && x.Equals(DJHColumnName) ->
                  sbTem.AppendFormat( @"
          {0}=LSHHelper.GetDJH businessEntity.C_DJLX businessEntity.{0},",
                    x
                    )|>ignore
              *)
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            entity{0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                entity{0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, 
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  child{0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=a.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0} with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"a.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            (*
            match b.DATA_TYPE.ToLowerInvariant() with
            | EndsWithIn NullableTypeConditions _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            | _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}.Value+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            *)
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "2uy"
          ,
          //{4}
          @""""+"单据处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          @"""新增"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (mainTableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  (*
        match PSeq.head businessEntities, decimal businessEntities.Length with
        | x, y ->DA_{10}Helper.UpdateLSH sb x.C_DJLX x.C_DJH y
  *)
  static member private GenerateMultiCreateCodeForMainChildOneLevelTablesWithDJLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        let lsh=ref (match businessEntities|>PSeq.head,decimal businessEntities.Length with y, z-> DA_{10}Helper.GetLSH sb y.C_DJLX y.C_DJH now z )
        for businessEntity in businessEntities do
          match businessEntity with
          | x->
              x.C_DJH<-DA_{10}Helper.GetDJH2M x.C_DJLX x.C_DJH !lsh now
              (x.{12},x.C_DJH)|>result.GuidStrings.Add
              lsh:=!lsh+1M
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          match new {2} 
           ({4}) with
          | entity{3} ->
              {5}    
              businessEntity.BD_{6}s|>Seq.iter(fun a->
                match new {6}
                 ({8}) with
                | child{7} ->
                    {9}
                    entity{3}.{6}.Add(child{7}))
              sb.{2}.AddObject(entity{3})
          {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} 
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
            {0}=(match businessEntity.{0} with NotEquals DateTimeDefaultValue x ->x | _ ->now),",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
            {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
              entity{0}.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                  entity{0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                  {0}=entity.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                  {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                  {0}=entity.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                  {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                    child{0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=a.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                    match {0} with
                    | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"a.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                        child{0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>
                    | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "2uy"
          ,
          //{4}
          @""""+"单据处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          @"""新增"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (mainTableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateSingleCreateCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        match new {2}
         ({4}) with
        | entity{3} ->
            {5}    
            businessEntity.BD_{6}s|>Seq.iter(fun a->
              match new {6}
               ({8}) with
              | child{7} ->
                  {9}
                  entity{3}.{6}.Add(child{7}))
            sb.{2}.AddObject(entity{3})
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} 
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              (*
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=(match businessEntity.{0} with NotEquals DateTimeDefaultValue x ->x | _ ->now),",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            entity{0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                entity{0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}, 
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  child{0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=a.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0} with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"a.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "3uy"
          ,
          //{4}
          @""""+"父子表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          @"""新增"+ String.Empty + @"父子记录"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateMultiCreateCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          match new {2}
           ({4}) with
          | entity{3} ->
              {5}   
              businessEntity.BD_{6}s|>Seq.iter(fun a-> 
                match new {6}
                 ({8}) with
                | child{7} ->
                    {9}
                    entity{3}.{6}.Add(child{7}))
              sb.{2}.AddObject(entity{3})
          {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3}
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
            {0}=(match businessEntity.{0} with NotEquals DateTimeDefaultValue x ->x | _ ->now),",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
            {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
              entity{0}.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                  entity{0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                  {0}=entity.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                  {0}=now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                  {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                    child{0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>sb.CreateEntityKey
                      |>sb.GetObjectByKey
                      |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=a.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                    match {0} with
                    | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"a.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                        child{0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>
                    | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "3uy"
          ,
          //{4}
          @""""+"父子表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          @"""新增"+ String.Empty + @"父子记录"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


(* 子模板空格问题
  static member private GenerateSingleUpdateCodeForMainChildOneLevelTables (mainTableName:string)   (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[])=
    @"{3}
    member this.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}    
            if businessEntity.BD_{6}s.Length>0 then
              original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
              businessEntity.BD_{6}s|>Seq.iter (fun a->
                new {6}
                  ({8})
                |>fun {7} -> 
                    {9}
                    original.{6}.Add({7}))
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DACodingMainChildOneLevelTablePartX.GenerateUpdateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
    
  static member private GenerateMultiUpdateCodeForMainChildOneLevelTables (mainTableName:string)   (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[])=
    @"{3}
    member this.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do 
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}    
              if businessEntity.BD_{6}s.Length>0 then
                original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
                businessEntity.BD_{6}s|>Seq.iter (fun a->
                  new {6}
                    ({8})
                  |>fun {7} ->
                      {9}
                      original.{6}.Add({7}))
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DACodingMainChildOneLevelTablePartX.GenerateUpdateCodeForMainChildOneLevelTables  mainTableName mainTableColumns mainTableAsFKRelationships mainTableAsPKRelationships mainTableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
*)
  (*
            if businessEntity.BD_{6}s.Length>0 then
              original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
              businessEntity.BD_{6}s|>Seq.iter (fun a->
                new {6}
                  ({8})
                |>fun {7} ->
                    {9}
                    original.{6}.Add({7}))

  //Update for Main Child Tables
  //子表更新最好还是加入跟踪信息，这样在单据子项很多的情况下，可根据子项不同记录的跟踪信息，进行插入，更新或是删除操作
  //加入跟踪后，客户端交叉操作可能导致子表跟踪信息有误？？？
  *)
  static member private GenerateSingleUpdateCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)   (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (childTableKeyColumns:TablePrimaryKeyInfo[])  (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member this.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}  
            if businessEntity.BD_{6}s.Length>0 then
              businessEntity.BD_{6}s 
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Created) 
              |>Seq.iter (fun a->
                  match new {6}
                   ({8}) with
                  | child{7} ->
                      {9}   
                      original.{6}.Add(child{7}))
              businessEntity.BD_{6}s
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Updated)
              |>Seq.iter (fun a->
                  match     
                    (""{6}"",new {6}({12}))
                    |>sb.CreateEntityKey
                    |>sb.TryGetObjectByKey with
                  | false,_ -> failwith ""The record is not exist!""
                  | _,x -> unbox<{6}> x
                  |>fun child ->
                      {13}
                      {14}  
                      ()) 
              businessEntity.BD_{6}s
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Deleted) 
              |>Seq.iter (fun a->
                  match 
                    (""{6}"",new {6}({12}))
                    |>sb.CreateEntityKey
                    |>sb.TryGetObjectByKey with
                  | false,_ -> failwith ""One of records is not exist!""
                  | _, x ->
                      sb.{6}.DeleteObject (x:?>{6}))
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-15,this,UpdateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              (*
              | z when z.ToLowerInvariant().EndsWith("datetime") &&  x.Equals("C_GXRQ") ->
              *)
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
            original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
            original.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                original.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->
                original.{1}Reference.Load() 
                original.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7},
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                    {0}=original.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                    {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                    {0}=original.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                    {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_)->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}
                        a.PK_COLUMN_NAME
                        ,
                        //{1}
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0} with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}
                    a.FK_COLUMN_NAME
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string  AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                          child{0}.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          child{0}.{1}Reference.Load() 
                          child{0}.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b ->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""更新"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          | _ ->
              @"""更新"+ String.Empty + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=a.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{13}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                      child.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                      child.{0}<-a.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{14}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships,childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
                      child.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0} with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                          child.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          child.{1}Reference.Load() 
                          child.{1}<-null",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  (*
              if businessEntity.BD_{6}s.Length>0 then
                original.{6} |>PSeq.toArray |>Seq.iter (fun a->sb.DeleteObject(a))
                businessEntity.BD_{6}s|>Seq.iter (fun a->
                  new {6}
                    ({8})
                  |>fun {7} ->
                      {9}
                      original.{6}.Add({7}))
  *)
  static member private GenerateMultiUpdateCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)   (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (childTableKeyColumns:TablePrimaryKeyInfo[])  (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member this.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try 
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        for businessEntity in businessEntities do 
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}    
              if businessEntity.BD_{6}s.Length>0 then
                businessEntity.BD_{6}s 
                |>PSeq.filter (fun a->a.TrackingState=TrackingState.Created) 
                |>Seq.iter (fun a->
                    match new {6}
                     ({8}) with
                    | child{7} ->
                        {9}   
                        original.{6}.Add(child{7}))
                businessEntity.BD_{6}s
                |>PSeq.filter (fun a->a.TrackingState=TrackingState.Updated)
                |>Seq.iter (fun a->
                    match     
                      (""{6}"",new {6}({12}))
                      |>sb.CreateEntityKey
                      |>sb.TryGetObjectByKey with
                    | false,_ -> failwith ""The record is not exist!""
                    | _,x -> unbox<{6}> x
                    |>fun child ->
                        {13}
                        {14}  
                        ()) 
                businessEntity.BD_{6}s
                |>PSeq.filter (fun a->a.TrackingState=TrackingState.Deleted) 
                |>Seq.iter (fun a->
                    match 
                      (""{6}"",new {6}({12}))
                      |>sb.CreateEntityKey
                      |>sb.TryGetObjectByKey with
                    | false,_ -> failwith ""One of records is not exist!""
                    | _, x ->
                        sb.{6}.DeleteObject  (x:?>{6}))
          {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-15,this,UpdateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
              original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
              original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
              original.{1} <-
                (""{2}"",new {2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
              match {0} with
              | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                  original.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>
              | _ ->
                  original.{1}Reference.Load() 
                  original.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                      {0}=original.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                      {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                      {0}=original.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                      {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                        child{0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_)->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}
                        a.PK_COLUMN_NAME
                        ,
                        //{1}
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                        match {0} with
                        | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}
                    a.FK_COLUMN_NAME
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string  AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                            child{0}.{1} <-
                              (""{2}"",new {2}({3}))
                              |>sb.CreateEntityKey
                              |>sb.GetObjectByKey
                              |>unbox<{2}>
                        | _ ->
                            child{0}.{1}Reference.Load() 
                            child{0}.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b ->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""更新"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          | _ ->
              @"""更新"+ String.Empty + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=a.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{13}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                        child.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                        child.{0}<-a.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{14}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships,childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
                        child.{1} <-
                          (""{2}"",new {2}({3}))
                          |>sb.CreateEntityKey
                          |>sb.GetObjectByKey
                          |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                        match {0} with
                        | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                            child.{1} <-
                              (""{2}"",new {2}({3}))
                              |>sb.CreateEntityKey
                              |>sb.GetObjectByKey
                              |>unbox<{2}>
                        | _ ->
                            child.{1}Reference.Load() 
                            child.{1}<-null",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

(*
let b=ref Unchecked.defaultof<_>
sb.TryGetObjectByKey(a,b), !b with
可以写成, 
let (flag,result)=sb.TryGetObjectByKey(a) //参考变量可以不需要作为参数, 而是可以在结果元数据中接收 
同样的 let (successX, x) = Double.TryParse("100")
Exception of Adding, Modifying, and Deleting Objects (Entity Framework)
http://msdn.microsoft.com/en-us/library/bb738695.aspx
*)
  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member private  GenerateDeleteCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[])  (mainTableKeyColumns:TablePrimaryKeyInfo[])   (childTableName:string)  (columnConditionTypes:ColumnConditionType[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      let childrenLength=ref 0
      try
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {5}EntitiesAdvance()
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->unbox<{2}> x
        |>fun a -> 
            a.{4}|>PSeq.toArray |>Seq.iter (fun b->incr childrenLength; sb.{4}.DeleteObject(b))
            a
        |>sb.{2}.DeleteObject
        {6}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? UpdateException as e -> match context with Some _ ->raise e | _ ->this.AttachError(e,-16,this,DeleteEntity,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-20,this,DeleteEntity,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        mainTableName
        ,
        //{3}
        (* 商业实体的实体键必然有值
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          (mainTableKeyColumns,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          do
          match a with
          | x,y when y.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || y.DATA_TYPE.EndsWith("[]")->
              sbTem.AppendFormat(@"{0}=businessEntity.{0},",
                //{0}
                x.COLUMN_NAME
                )|>ignore
          | x,_->
              sbTem.AppendFormat(@"{0}=businessEntity.{0}.Value,",
                //{0}
                x.COLUMN_NAME
                )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        *)
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  mainTableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        childTableName
        ,
        //{5}
        entityContextNamePrefix
        ,
        //{6}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "3uy"
          ,
          //{6}
          @"""删除"""
          ,
          //{7}
          //"businessEntity.BD_"+childTableName+"s.Length"
          "!childrenLength"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""删除"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(string !childrenLength)+ @""种商品"""
          | _ ->
              @"""删除"+ String.Empty + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

    
(*
let b=ref Unchecked.defaultof<_>
sb.TryGetObjectByKey(a,b), !b with
可以写成, 
let (flag,result)=sb.TryGetObjectByKey(a) //参考变量可以不需要作为参数, 而是可以在结果元数据中接收 
同样的 let (successX, x) = Double.TryParse("100")
*)
  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member private  GenerateMultiDeleteCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)   (mainTableColumns:TableColumnInfo[])    (mainTableKeyColumns:TablePrimaryKeyInfo[])   (childTableName:string)  (columnConditionTypes:ColumnConditionType[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      let childrenLength=ref 0
      try
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {5}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _,x ->unbox<{2}> x
          |>fun a -> 
              a.{4} |>PSeq.toArray |>Seq.iter (fun b->incr childrenLength; sb.{4}.DeleteObject(b))
              a
          |>sb.{2}.DeleteObject
          {6}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-17,this,DeleteEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-20,this,DeleteEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        mainTableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  mainTableKeyColumns do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().Trim()
        )
        ,
        //{4}
        childTableName
        ,
        //{5}
        entityContextNamePrefix
        ,
        //{6}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "3uy"
          ,
          //{6}
          @"""删除"""
          ,
          //{7}
          //"businessEntity.BD_"+childTableName+"s.Length"
          "!childrenLength"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""删除"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(string !childrenLength)+ @""种商品"""
          | _ ->
              @"""删除"+ String.Empty + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  //创建单据，且该子表的批次字段需要进行处理的代码
  static member private GenerateSingleCreateCodeForMainChildOneLevelTablesWithPCProcess (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.Create{1}_WithPCProcess (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        match businessEntity with
        | x ->
            match DA_{10}Helper.GetDJH sb x.C_DJLX x.C_DJH now with
            | y ->
                x.C_DJH<-y
                (x.{12},x.C_DJH)|>result.GuidStrings.Add{13}
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        match new {2}
         ({4}) with
        | entity{3} ->
            {5}    
            businessEntity.BD_{6}s|>Seq.iter(fun a->
              match new {6}
               ({8}) with
              | child{7} ->
                  {9}{14}
                  entity{3}.{6}.Add(child{7}))
            sb.{2}.AddObject(entity{3})
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3}
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=(match businessEntity.{0} with NotEquals DateTimeDefaultValue x ->x | _ ->now),",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              (* 已置前
              | z when z.ToLowerInvariant().EndsWith("string") && x.Equals(DJHColumnName) ->
                  sbTem.AppendFormat( @"
          {0}=LSHHelper.GetDJH businessEntity.C_DJLX businessEntity.{0},",
                    x
                    )|>ignore
              *)
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            entity{0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                entity{0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  child{0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=a.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0} with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"a.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            (*
            match b.DATA_TYPE.ToLowerInvariant() with
            | EndsWithIn NullableTypeConditions _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            | _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}.Value+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            *)
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "2uy"
          ,
          //{4}
          @""""+"单据处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          @"""新增"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (mainTableKeyColumns|>PSeq.head).COLUMN_NAME
        ,
        //{13}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        let entityPC=
          match PSeq.head sb.{0} with
          | x ->
              if x.C_GXRQ.Date<>now.Date then x.C_LSH<-x.C_CSLSH+1M
              x.C_GXRQ<-now
              x",
          //{0}
          match mainTableName with
          | x when x=JXCSHDJTableName->JXCSHDJ_PCLSHTableName
          | _ ->PCLSHTableName
          | _ ->String.Empty
          )|>ignore
        sbTem.ToString()
        )
        ,
        //{14}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                  match now.ToString(""yyyMMdd"")+string entityPC.C_LSH|>decimal with
                  | x ->
                      match child{0} with
                      | y when y.C_PC=0M -> 
                          y.C_PC<-x
                          result.GuidDecimals.Add (a.RowGuid,x)
                          entityPC.C_LSH<-entityPC.C_LSH+1M
                      | y ->
                          result.GuidDecimals.Add (a.RowGuid,y.C_PC)",
          //{0}
          String.Empty
          (*
          match childTableName,childTableName.Split('_') with
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          )|>ignore
        sbTem.ToString() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  //更新单据，且该子表的批次字段需要进行处理的代码
  static member private GenerateSingleUpdateCodeForMainChildOneLevelTablesWithPCProcess (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)   (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (childTableKeyColumns:TablePrimaryKeyInfo[])  (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member this.Update{1}_WithPCProcess (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance(){15}
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}  
            if businessEntity.BD_{6}s.Length>0 then
              businessEntity.BD_{6}s 
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Created) 
              |>Seq.iter (fun a->
                  match new {6}
                   ({8}) with
                  | child{7} ->
                      {9}{16}  
                      original.{6}.Add(child{7}))
              businessEntity.BD_{6}s
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Updated)
              |>Seq.iter (fun a->
                  match     
                    (""{6}"",new {6}({12}))
                    |>sb.CreateEntityKey
                    |>sb.TryGetObjectByKey with
                  | false,_ -> failwith ""The record is not exist!""
                  | _,x -> unbox<{6}> x
                  |>fun child ->
                      {13}
                      {14}  
                      ()) 
              businessEntity.BD_{6}s
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Deleted) 
              |>Seq.iter (fun a->
                  match 
                    (""{6}"",new {6}({12}))
                    |>sb.CreateEntityKey
                    |>sb.TryGetObjectByKey with
                  | false,_ -> failwith ""One of records is not exist!""
                  | _, x ->
                      sb.{6}.DeleteObject (x:?>{6}))
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-15,this,UpdateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
            original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
            original.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                original.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->
                original.{1}Reference.Load() 
                original.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                    {0}=original.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                    {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                    {0}=original.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                    {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_)->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}
                        a.PK_COLUMN_NAME
                        ,
                        //{1}
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0} with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}
                    a.FK_COLUMN_NAME
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string  AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                          child{0}.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          child{0}.{1}Reference.Load() 
                          child{0}.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b ->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""更新"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          | _ ->
              @"""更新"+ String.Empty + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=a.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{13}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                      child.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                      child.{0}<-a.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{14}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships,childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
                      child.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0} with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                          child.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          child.{1}Reference.Load() 
                          child.{1}<-null",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{15}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        let entityPC=
          match PSeq.head sb.{0} with
          | x ->
              if x.C_GXRQ.Date<>now.Date then x.C_LSH<-x.C_CSLSH+1M
              x.C_GXRQ<-now
              x",
          //{0}
          match mainTableName with
          | x when x=JXCSHDJTableName->JXCSHDJ_PCLSHTableName
          | _ ->PCLSHTableName
          | _ ->String.Empty
          )|>ignore
        sbTem.ToString()
        )
        ,
        //{16}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                      match now.ToString(""yyyMMdd"")+string entityPC.C_LSH|>decimal with
                      | x ->
                          match child{0} with
                          | y when y.C_PC=0M ->  
                              y.C_PC<-x
                              result.GuidDecimals.Add (a.RowGuid,x)  
                              entityPC.C_LSH<-entityPC.C_LSH+1M
                          | y ->
                              result.GuidDecimals.Add (a.RowGuid,y.C_PC)",
          //{0}
          String.Empty
          (*
          match childTableName,childTableName.Split('_') with
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          )|>ignore
        sbTem.ToString() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateCGJHSingleCreateCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)  (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) = //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.Create{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance()
        match businessEntity with
        | x ->
            match DA_{10}Helper.GetDJH sb x.C_DJLX x.C_DJH now with
            | y ->
                x.C_DJH<-y
                (x.{12},x.C_DJH)|>result.GuidStrings.Add{13}
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        match new {2}
         ({4}) with
        | entity{3} ->
            {5}    
            businessEntity.BD_{6}s|>Seq.iter(fun a->
              match new {6}
               ({8}) with
              | child{7} ->
                  {9}{14}
                  entity{3}.{6}.Add(child{7}))
            sb.{2}.AddObject(entity{3})
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3}
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, mainTableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=(match businessEntity.{0} with NotEquals DateTimeDefaultValue x ->x | _ ->now),",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
          {0}=now,",
                    x
                    )|>ignore
              (* 已置前
              | z when z.ToLowerInvariant().EndsWith("string") && x.Equals(DJHColumnName) ->
                  sbTem.AppendFormat( @"
          {0}=LSHHelper.GetDJH businessEntity.C_DJLX businessEntity.{0},",
                    x
                    )|>ignore
              *)
              | _  ->
                  sbTem.AppendFormat( @"
          {0}=businessEntity.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
            entity{0}.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,_ ->
                          sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                            //{0}, C_ID
                            w.PK_COLUMN_NAME
                            ,
                            //{1},C_CZY
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
                        ,
                        //{0}, CZY
                        u.FK_COLUMN_NAME
                        )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (*
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | u,v  ->
                      sbTemSub.AppendFormat(@"{0},"
                        ,
                        //{0}
                        string (char !intTem)
                        )|>ignore
                  incr intTem
                  match !intTem with  // x,y,z,u,v,w,a,b,c....
                  | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
                  | u when u=120 ->intTem:=97 // int 'a'=97
                  | _ -> ()
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                *)
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                entity{0}.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              (*
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
              {0}=Guid.NewGuid(),",
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                {0}=entity.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                  child{0}.{1} <-
                    (""{2}"",new {2}({3}))
                    |>sb.CreateEntityKey
                    |>sb.GetObjectByKey
                    |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | w,r ->
                          sbTemSub.AppendFormat(@"{0}=a.{1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            w.FK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                  match {0} with
                  | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"a.{0},"
                        ,
                        //{0}
                        u.FK_COLUMN_NAME
                      )|>ignore
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>
                  | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            (*
            match b.DATA_TYPE.ToLowerInvariant() with
            | EndsWithIn NullableTypeConditions _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            | _ ->
                sbTemSub.AppendFormat(@"{0}=""+businessEntity.{0}.Value+""|",
                  //{0}
                  a.COLUMN_NAME
                  )|>ignore
            *)
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "2uy"
          ,
          //{4}
          @""""+"单据处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          @"""新增"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (mainTableKeyColumns|>PSeq.head).COLUMN_NAME
        ,
        //{13}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        let entityPC=
          match PSeq.head sb.{0} with
          | x ->
              if x.C_GXRQ.Date<>now.Date then x.C_LSH<-x.C_CSLSH+1M
              x.C_GXRQ<-now
              x",
          //{0}
          match mainTableName with
          | x when x=JXCSHDJTableName->JXCSHDJ_PCLSHTableName
          | x when x=JHGLDJTableName ->PCLSHTableName
          | _ ->String.Empty
          )|>ignore
        sbTem.ToString()
        )
        ,
        //{14}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                  match now.ToString(""yyyMMdd"")+string entityPC.C_LSH|>decimal with
                  | x ->
                      child{0}.C_PC<-x
                      result.GuidDecimals.Add (a.RowGuid,x)
                  entityPC.C_LSH<-entityPC.C_LSH+1M",
          //{0}
          String.Empty
          (*
          match childTableName,childTableName.Split('_') with
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          )|>ignore
        sbTem.ToString() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateCGJHSingleUpdateCodeForMainChildOneLevelTables (databaseInstanceName:string) (entityContextNamePrefix:string) (mainTableName:string)   (mainTableColumns:TableColumnInfo[]) (mainTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (mainTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (mainTableKeyColumns:TablePrimaryKeyInfo[])  (childTableName:string)  (childTableColumns:TableColumnInfo[]) (childTableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (childTableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (childTableKeyColumns:TablePrimaryKeyInfo[])  (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}
    member this.Update{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try 
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {10}EntitiesAdvance(){15}
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _,x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}  
            if businessEntity.BD_{6}s.Length>0 then
              businessEntity.BD_{6}s 
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Created) 
              |>Seq.iter (fun a->
                  match new {6}
                   ({8}) with
                  | child{7} ->
                      {9}{16}  
                      original.{6}.Add(child{7}))
              businessEntity.BD_{6}s
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Updated)
              |>Seq.iter (fun a->
                  match     
                    (""{6}"",new {6}({12}))
                    |>sb.CreateEntityKey
                    |>sb.TryGetObjectByKey with
                  | false,_ -> failwith ""The record is not exist!""
                  | _,x -> unbox<{6}> x
                  |>fun child ->
                      {13}
                      {14}  
                      ()) 
              businessEntity.BD_{6}s
              |>PSeq.filter (fun a->a.TrackingState=TrackingState.Deleted) 
              |>Seq.iter (fun a->
                  match 
                    (""{6}"",new {6}({12}))
                    |>sb.CreateEntityKey
                    |>sb.TryGetObjectByKey with
                  | false,_ -> failwith ""One of records is not exist!""
                  | _, x ->
                      sb.{6}.DeleteObject (x:?>{6}))
        {11}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-15,this,UpdateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=businessEntity.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{1},DJ_JHGL
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        mainTableName
        ,
        //{3} ,t_DJ_JHGL
        String.Empty
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
            original.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
            original.{0}<-businessEntity.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
            original.{1} <-
              (""{2}"",new {2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=businessEntity.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0} with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                original.{1} <-
                  (""{2}"",new {2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->
                original.{1}Reference.Load() 
                original.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}, T_DJSP_JHGL
        childTableName
        ,
        //{7}
        String.Empty
        (*
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{8}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          (*
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>PSeq.exists (fun b->b.COLUMN_NAME=x ) ->  
          *)
          match a.COLUMN_NAME,childTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName ->
                  sbTem.AppendFormat( @"
                    {0}=original.{0},",
                    x
                    )|>ignore
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                    {0}=now,",
                    x
                    )|>ignore
              | EndsWith StringTypeName _ when x=DJHColumnName  &&  mainTableColumns|>Seq.exists (fun c->c.COLUMN_NAME=x ) -> //对字表冗余单据号的处理
                  sbTem.AppendFormat( @"
                    {0}=original.{0},",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                    {0}=a.{0},",
                    x
                    )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{9}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a  with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                      child{0}.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_)->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}
                        a.PK_COLUMN_NAME
                        ,
                        //{1}
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0} with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}
                    a.FK_COLUMN_NAME
                  )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string  AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                      )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"
                          child{0}.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          child{0}.{1}Reference.Load() 
                          child{0}.{1}<-null",
                    //{0}
                    String.Empty
                    (*
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b ->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{10}
        entityContextNamePrefix
        ,
        //{11}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (mainTableKeyColumns,mainTableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+mainTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->"2uy"
          | _ ->"3uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @""""+"单据处理"+ @""""
          | _ ->
              @""""+"父子表处理处理"+ @""""
          )
          ,
          //{5}
          "2uy"
          ,
          //{6}
          @"""更新"""
          ,
          //{7}
          "businessEntity.BD_"+childTableName+"s.Length"
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasDJLSH] _->
              @"""更新"+ String.Empty + @"的""+(businessEntity.C_DJLX|>DA_"+entityContextNamePrefix+ @"Helper.GetDJName)+""单""+(businessEntity.C_DJH|>string)+""，共""+(businessEntity.BD_"+childTableName+ @"s.Length|>string)+""种商品"""
          | _ ->
              @"""更新"+ String.Empty + @"的父子表记录"""
          )
          ,
          //{9}
          @""""+childTableName+ @""""
          ,
          //{10}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{12}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableKeyColumns  do
          sbTem.AppendFormat(@"{0}=a.{0},",
            //{0}, C_ID
            a.COLUMN_NAME
            )|>ignore
        match sbTem with
        | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
        | _ ->()
        sbTem.ToString().TrimStart()
        )
        ,
        //{13}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          match a.COLUMN_NAME,childTableAsFKRelationships,childTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | EndsWith DateTimeTypeName _ when x=UpdateDateColumnName ->
                  sbTem.AppendFormat( @"
                      child.{0}<-now",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
                      child.{0}<-a.{0}",
                    x
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{14}
        //一个外键对应多个外键列时，创建实体时，如果这个外键的全部外键列都允许为空，并且这些外键列只是部分有值，那么这些有值的外键列的值应该被忽略，实体能够被正常创建； 如果这个外键的部分外键列允许为空，并且此时所有外键列都有值，那么实体能够被正常创建，如果此时所有外键列只是部分有值，实体将不能创建新的记录,在数据库中，此种情况下记录能够新增，但只要一个外键多应的外键列中，有一个为空，其它不允许为空外键列值将不能被约束，除非所有外键列都有值，这些数据才能被约束，所以应该避免，一个外键的多个外键列部分为空的情况
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships,childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @" {0}
                      child.{1} <-
                        (""{2}"",new {2}({3}))
                        |>sb.CreateEntityKey
                        |>sb.GetObjectByKey
                        |>unbox<{2}>",
                    //{0}
                    String.Empty
                    ,
                    //{1},T_YG1
                    u.PK_TABLE_ALIAS
                    ,
                    //{2},T_YG
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iter (fun (a,_) ->
                      sbTemSub.AppendFormat(@"{0}=a.{1},",
                        //{0}, C_ID
                        a.PK_COLUMN_NAME
                        ,
                        //{1},C_CZY
                        a.FK_COLUMN_NAME
                        )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                   )|>ignore
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 ValidateDatabaseDesignX.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
                      match {0} with
                      | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_) ->
                  sbTemSub.AppendFormat(@"a.{0},"
                    ,
                    //{0}, CZY
                    a.FK_COLUMN_NAME
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                )
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a _ ->
                  sbTemSub.AppendFormat(@"{0},"
                    ,
                    //{0}
                    string AutoVariableNames.[a]
                    )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                | _ ->()
                sbTemSub.ToString().TrimStart()
                ) 
                ,
                //{2}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iteri (fun a b ->
                  match b with
                  | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string AutoVariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string AutoVariableNames.[a]+".HasValue"
                         )|>ignore)
                match sbTemSub with
                | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
                | _ ->()
                sbTemSub.ToString().TrimStart()
               )
               )|>ignore
               
              match y|>PSeq.head|>fst with
              | u ->
                  sbTem.AppendFormat( @"{0}
                          child.{1} <-
                            (""{2}"",new {2}({3}))
                            |>sb.CreateEntityKey
                            |>sb.GetObjectByKey
                            |>unbox<{2}>
                      | _ ->
                          child.{1}Reference.Load() 
                          child.{1}<-null",
                    //{0}
                    String.Empty
                    ,
                    //{1}
                    u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    u.PK_TABLE
                    ,
                    //{3}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    y|>Seq.iteri (fun a b->
                      match b with
                      | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                          sbTemSub.AppendFormat(@"{0}={1},",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string AutoVariableNames.[a]
                            )|>ignore)
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-1,1)|>ignore //Remove the last of ','
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                  )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{15}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        let entityPC=
          match PSeq.head sb.{0} with
          | x ->
              if x.C_GXRQ.Date<>now.Date then x.C_LSH<-x.C_CSLSH+1M
              x.C_GXRQ<-now
              x",
          //{0}
          match mainTableName with
          | x when x=JXCSHDJTableName->JXCSHDJ_PCLSHTableName
          | x when x=JHGLDJTableName ->PCLSHTableName
          | _ ->String.Empty
          )|>ignore
        sbTem.ToString()
        )
        ,
        //{16}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                      match now.ToString(""yyyMMdd"")+string entityPC.C_LSH|>decimal with
                      | x ->
                          child{0}.C_PC<-x
                          result.GuidDecimals.Add (a.RowGuid,x)  
                      entityPC.C_LSH<-entityPC.C_LSH+1M",
          //{0}
          String.Empty
          (*
          match childTableName,childTableName.Split('_') with
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          )|>ignore
        sbTem.ToString() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


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
        | ColumnConditionTypeAllEquals [HasDJLSH] _->
            @"""更新父表"+ String.Empty + @"，单据号为""+(modelInstance.C_DJH|>string)+""的记录"""
        | _ ->
            @"""更新父表"+ String.Empty + @"的记录"""
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

  static member private GenerateWriteLogForDeleteCode (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.WriteLogForDelete{12} (modelInstance:{13},executeBase,context,now)=
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
        "3uy"
        ,
        //{6}
        @"""删除"""
        ,
        //{7}
        String.Empty
        ,
        //{8}
        (
        match columnConditionTypes with
        | ColumnConditionTypeAllEquals [HasDJLSH] _->
            @"""删除父表为"+ String.Empty + @"，单据号为""+(modelInstance.C_DJH|>string)+""的相关记录"""
        | _ ->
            @"""删除父表为"+ String.Empty + @"的相关记录"""
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

//-------------------------------------------------------------------------------------------------------------------------------