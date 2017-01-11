namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper
open WX.Data.Database

type DACodingIndependentTablePartX=
  static member GetCodeWithIndependentTable  (databaseInstanceName:string)  (entityContextNamePrefix:string) (tableRelatedInfo:TableRelatedInfoX) (tableInfos:TableInfo[])=  
    let sb=StringBuilder()
    let tableName=tableRelatedInfo.TableInfo.TABLE_NAME
    let tableColumns=
      tableRelatedInfo.TableInfo.TableColumnInfos
      |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let tableAsFKRelationships= //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
      tableRelatedInfo.TableInfo.TableForeignKeyRelationshipInfos 
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.PK_TABLE)) 
    let tableAsPKRelationships=    //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
      tableRelatedInfo.TableInfo.TablePrimaryKeyRelationshipInfos 
      |>Array.filter (fun a->tableInfos|>Array.exists(fun b->b.TABLE_NAME=a.FK_TABLE)) 
    let tableKeyColumns=tableRelatedInfo.TableInfo.TablePrimaryKeyInfos

    match tableRelatedInfo.ColumnConditionTypes with
    | ColumnConditionTypeContains [HasJYH] _ ->
        DACodingIndependentTablePartX.GenerateSingleCreateCodeForIndependentTableWithJYLSH databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
        |>box|>sb.Append |>ignore //SingleCreateCode
        sb.AppendLine()|>ignore
        DACodingIndependentTablePartX.GenerateMultiCreateCodeForIndependentTableWithJYLSH databaseInstanceName entityContextNamePrefix  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
        |>box|>sb.Append |>ignore //MulitiCreateCode
        sb.AppendLine()|>ignore
    | ColumnConditionTypeContains [HasLSH] _->
        match tableRelatedInfo.TableRelationshipTypes with
        | TableRelationshipTypeContains [WithZZB] _ ->
            //总账表处理
            DACodingIndependentTablePartX.GenerateSingleCreateCodeForIndependentTableWithLSHWithZZ databaseInstanceName entityContextNamePrefix  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableInfos
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DACodingIndependentTablePartX.GenerateMultiCreateCodeForIndependentTableWithLSHWithZZ databaseInstanceName entityContextNamePrefix  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableInfos
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore
        | _ ->
            DACodingIndependentTablePartX.GenerateSingleCreateCodeForIndependentTableWithLSH databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DACodingIndependentTablePartX.GenerateMultiCreateCodeForIndependentTableWithLSH databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore
    | _ ->
        match tableRelatedInfo.TableRelationshipTypes with
        | TableRelationshipTypeContains [WithZZB] _->
            //总账表处理
            DACodingIndependentTablePartX.GenerateSingleCreateCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableInfos
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DACodingIndependentTablePartX.GenerateMultiCreateCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableInfos
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore
        | x ->
            DACodingIndependentTablePartX.GenerateSingleCreateCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //SingleCreateCode
            sb.AppendLine()|>ignore
            DACodingIndependentTablePartX.GenerateMultiCreateCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
            |>box|>sb.Append |>ignore //MulitiCreateCode
            sb.AppendLine()|>ignore

    match tableRelatedInfo.ColumnConditionTypes, tableRelatedInfo.TableRelationshipTypes with
    | ColumnConditionTypeNotContains [HasJYH;HasLSH] _, TableRelationshipTypeNotContains [WithZZB] y ->
        match y with   
        | TableRelationshipTypeContains [WithForeignKeyRelatedTable] _ -> //有外键关系才有必要
           DACodingIndependentTablePartX.GenerateSingleCreateConcurrentCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
           |>box|>sb.Append |>ignore //SingleCreateConcurrentCode  //和关联父表一起创建
           sb.AppendLine()|>ignore
           DACodingIndependentTablePartX.GenerateMultiCreateConcurrentCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
           |>box|>sb.Append |>ignore //MulitiCreateConcurrentCode //和关联父表一起创建
           sb.AppendLine()|>ignore
        | _ ->()
    | _ ->()

    DACodingIndependentTablePartX.GenerateSingleUpdateCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //SingleUpdateCode
    sb.AppendLine()|>ignore
    DACodingIndependentTablePartX.GenerateMultiUpdateCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append |>ignore //MultiUpdateCode
    sb.AppendLine()|>ignore
    match tableRelatedInfo.TableRelationshipTypes with
    | TableRelationshipTypeContains [WithZZB] _ ->
        DACodingIndependentTablePartX.GenerateDeleteCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableKeyColumns tableAsPKRelationships tableRelatedInfo.ColumnConditionTypes tableInfos
        |>box|>sb.Append |>ignore //Delete
        sb.AppendLine()|>ignore
        DACodingIndependentTablePartX.GenerateMultiDeleteCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableKeyColumns tableAsPKRelationships tableRelatedInfo.ColumnConditionTypes tableInfos
        |>box|>sb.Append |>ignore //MultiDelete
        sb.AppendLine()|>ignore
    | _ ->
        DACodingIndependentTablePartX.GenerateDeleteCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableKeyColumns tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //Delete
        sb.AppendLine()|>ignore
        DACodingIndependentTablePartX.GenerateMultiDeleteCodeForIndependentTable databaseInstanceName  entityContextNamePrefix tableName tableColumns tableKeyColumns tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append |>ignore //MultiDelete
        sb.AppendLine()|>ignore
    //Batch Processing
    match tableRelatedInfo.ColumnConditionTypes, tableRelatedInfo.TableRelationshipTypes with  //处理(C_XH)时常常需要批处理
    | ColumnConditionTypeContains [HasXH;HasLSH;HasFID] _, TableRelationshipTypeContains [WithZZB] _ ->  //同时具有序号字段(C_XH)和总账表(T_ZZ_XX)时，必须有C_FID子段
        DACodingIndependentTablePartX.GenerateBatchCodeForIndependentTableWithLSHWithZZ databaseInstanceName  entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes tableInfos
        |>box|>sb.Append|>ignore //BatchProcess  with C_XBH 和T_ZZ_XX
        sb.AppendLine()|>ignore
    | ColumnConditionTypeContains [HasXH;HasFID] _, TableRelationshipTypeContains [WithZZB] _ ->
        DACodingIndependentTablePartX.GenerateBatchCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes tableInfos
        |>box|>sb.Append|>ignore //BatchProcess  with T_ZZ_XX
        sb.AppendLine()|>ignore
    | ColumnConditionTypeContains [HasXH;HasLSH] _, _ ->
        DACodingIndependentTablePartX.GenerateBatchCodeForIndependentTableWithLSH databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append|>ignore //BatchProcess  with C_XBH
        sb.AppendLine()|>ignore
    | ColumnConditionTypeContains [HasXH] _, _ ->
        DACodingIndependentTablePartX.GenerateBatchCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableRelatedInfo.ColumnConditionTypes
        |>box|>sb.Append|>ignore //BatchProcess
        sb.AppendLine()|>ignore
    | _ ->()
    //WriteLog for update
    DACodingIndependentTablePartX.GenerateWriteLogForUpdateCode  databaseInstanceName entityContextNamePrefix tableName tableColumns   tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append|>ignore
    sb.AppendLine()|>ignore
    //WriteLog for Delete
    DACodingIndependentTablePartX.GenerateWriteLogForDeleteCode  databaseInstanceName entityContextNamePrefix tableName tableColumns   tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append|>ignore
    sb.AppendLine()|>ignore
    string sb

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateSingleCreateCodeForIndependentTableWithJYLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])= 
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match PSeq.head sb.T_JYLSH_{1} with
        |  x ->
            if x.C_GXRQ.Date<>now.Date then x.C_LSH<-x.C_CSLSH+1M
            x.C_GXRQ<-now
            businessEntity.C_JYH<- now.ToString(""yyyMMdd"")+string x.C_LSH|>decimal
            result.GuidDecimals.Add (businessEntity.{8},businessEntity.C_JYH)
            match 
              (""{2}"",new {2}({0}))
              |>sb.CreateEntityKey 
              |>sb.TryGetObjectByKey with
            | true, _ -> failwith ""The record is exist！"" | _ ->()
            match new {2}
             ({4}) with
            | entity{3} ->
                {5}    
                sb.{2}.AddObject(entity{3})
            x.C_LSH<-x.C_LSH+1M
        {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"交易号为""+(businessEntity.C_JYH|>string)+""的记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME      //
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateMultiCreateCodeForIndependentTableWithJYLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match PSeq.head sb.T_JYLSH_{1} with
        |  x ->
            if x.C_GXRQ.Date<>now.Date then x.C_LSH<-x.C_CSLSH+1M
            x.C_GXRQ<-now
            for businessEntity in businessEntities do
              businessEntity.C_JYH<- now.ToString(""yyyMMdd"")+string x.C_LSH|>decimal
              result.GuidDecimals.Add (businessEntity.{8},businessEntity.C_JYH)
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              match new {2}
               ({4}) with
              | entity{3} ->
                  {5}    
                  sb.{2}.AddObject(entity{3})
              x.C_LSH<-x.C_LSH+1M
              {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
              |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"交易号为""+(businessEntity.C_JYH|>string)+""的记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  //有流水号的表只能拥有一个主键，已验证
  static member private GenerateSingleCreateCodeForIndependentTableWithLSHWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) (tableInfos:TableInfo[])=  //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match sb.T_LSH_{1}|>PSeq.head with
        | entityLSH->
            (businessEntity.{9},entityLSH.C_LSH)|>result.GuidDecimals.Add
            businessEntity.C_XBH<- entityLSH.C_LSH
            match 
              (""{2}"",new {2}({0}))
              |>sb.CreateEntityKey 
              |>sb.TryGetObjectByKey with
            | true, _ -> failwith ""The record is exist！"" | _ ->()
            match new {2}
             ({4}) with
            | entity{3} ->
                {5} 
                {8} 
                sb.{2}.AddObject(entity{3})
            entityLSH.C_LSH<-entityLSH.C_LSH+1M
        {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时新增总帐表"+String.Empty+ @""""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->
              x.TableColumnInfos
              |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                entity{0}.{1}<-
                  match new {1}({3}) with
                  | {2} ->
                      {4}
                      {2}"
          , 
          //{0}  
          String.Empty
          (*
          match tableName,tableName.Split('_') with  //update it to t_DJ...
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          ,
          //{1},
          zzTableName
          ,
          //{2} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{3}
          (
          String.Empty
          (*主键值已由关联的父表自动提供
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          (*正确，但限制较多，必须保证主表和总账表仅有1一个主键字段
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          *)
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          *)
          )
          ,
          //{4}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when 
                match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                | Some y ->
                    y.TablePrimaryKeyInfos|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not 
                | _ ->false 
                ->  //不是主键列
                sbTemSub.AppendFormat(@"
                      {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if  tableColumns|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                    | Some y ->
                        match 
                          y.TableForeignKeyRelationshipInfos
                          |>Array.filter (fun b->tableInfos|>Array.exists(fun c->c.TABLE_NAME=b.PK_TABLE)) 
                          |>Array.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                        | Some z  when z.PK_TABLE=tableName ->z.PK_COLUMN_NAME
                        | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                    | _ ->String.Empty
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                      {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{9}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateMultiCreateCodeForIndependentTableWithLSHWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) (tableInfos:TableInfo[])= 
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match sb.T_LSH_{1}|>PSeq.head with
        | entityLSH->
            for businessEntity in businessEntities do
              (businessEntity.{9},entityLSH.C_LSH)|>result.GuidDecimals.Add
              businessEntity.C_XBH<- entityLSH.C_LSH
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              match new {2}
               ({4}) with
              | entity{3} ->
                  {5}
                  {8}    
                  sb.{2}.AddObject(entity{3})
              entityLSH.C_LSH<-entityLSH.C_LSH+1M
              {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
              |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时新增总帐表"+String.Empty+ @""""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->
              x.TableColumnInfos
              |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                  entity{0}.{1}<-
                    match new {1}({3}) with
                    | {2} ->
                        {4}
                        {2}"
          ,
          //{0}  
          String.Empty
          (*
          match tableName,tableName.Split('_') with  //update it to t_DJ...
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          ,
          //{1},
          zzTableName
          ,
          //{2} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{3}
          (
          String.Empty
          (*主键值已由关联的父表自动提供
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          *)
          )
          ,
          //{4}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when 
                match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                | Some y ->
                    y.TablePrimaryKeyInfos|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not 
                | _ ->false 
                ->  //不是主键列
                sbTemSub.AppendFormat(@"
                        {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if  tableColumns|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                    | Some y ->
                        match 
                          y.TableForeignKeyRelationshipInfos
                          |>Array.filter (fun b->tableInfos|>Array.exists(fun c->c.TABLE_NAME=b.PK_TABLE)) 
                          |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                        | Some z  when z.PK_TABLE=tableName ->z.PK_COLUMN_NAME
                        | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                    | _ ->String.Empty
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                        {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{9}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateSingleCreateCodeForIndependentTableWithLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match sb.T_LSH_{1}|>PSeq.head with
        | entityLSH->
            (businessEntity.{8},entityLSH.C_LSH)|>result.GuidDecimals.Add
            businessEntity.C_XBH<- entityLSH.C_LSH
            match 
              (""{2}"",new {2}({0}))
              |>sb.CreateEntityKey 
              |>sb.TryGetObjectByKey with
            | true, _ -> failwith ""The record is exist！"" | _ ->()
            match new {2}
             ({4}) with
            | entity{3} ->
                {5}    
                sb.{2}.AddObject(entity{3})
            entityLSH.C_LSH<-entityLSH.C_LSH+1M
        {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member  GenerateMultiDeleteCodeForIndependentTableWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[])   (tableKeyColumns:TablePrimaryKeyInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[])   (columnConditionTypes:ColumnConditionType[]) (tableInfos:TableInfo[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {4}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _, x ->
              sb.{2}.DeleteObject (x:?>{2})
          {6}
          {5}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-17,this,DeleteEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-20,this,DeleteEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
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
        entityContextNamePrefix
        ,
        //{5}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "5uy"
          ,
          //{6}
          @"""删除(同时删除总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时删除总帐表"+String.Empty+ @""""
          | _ ->
              @"""删除"+ String.Empty + @"的记录，同时删除总帐表"+String.Empty+ @""""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{6} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableKeyColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->x.TablePrimaryKeyInfos
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          match
            (""{0}"",new {0}({1}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _, x ->
              sb.{0}.DeleteObject (x:?>{0})"
          ,
          //{0},
          zzTableName
          ,
          //{1}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------



//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateMultiCreateCodeForIndependentTableWithLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match sb.T_LSH_{1}|>PSeq.head with
        | entityLSH->
            for businessEntity in businessEntities do
              (businessEntity.{8},entityLSH.C_LSH)|>result.GuidDecimals.Add
              businessEntity.C_XBH<- entityLSH.C_LSH
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              match new {2}
               ({4}) with
              | entity{3} ->
                  {5}    
                  sb.{2}.AddObject(entity{3})
              entityLSH.C_LSH<-entityLSH.C_LSH+1M
              {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
              |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateSingleCreateCodeForIndependentTableWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) (tableInfos:TableInfo[])= 
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        match new {2}
         ({4}) with
        | entity{3} ->
            {5}  
            {8}  
            sb.{2}.AddObject(entity{3})
        {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录，同时新增总帐表"+String.Empty+ @""""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->
              x.TableColumnInfos
              |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            entity{0}.{1}<-
              match new {1}({3}) with
              | {2} ->
                  {4}
                  {2}"
          ,
          //{0}  
          String.Empty
          (*
          match tableName,tableName.Split('_') with  //update it to t_DJ...
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          ,
          //{1},
          zzTableName
          ,
          //{2} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{3}
          (
          String.Empty
          (*主键值已由关联的父表自动提供
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          *)
          )
          ,
          //{4}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when 
                match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                | Some y ->
                    y.TablePrimaryKeyInfos
                    |>Array.exists (fun b->b.COLUMN_NAME=x ) |>not
                | _ ->false
                ->  //不是主键列
                sbTemSub.AppendFormat(@"
                  {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if tableColumns|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                    | Some y ->
                        match 
                          y.TableForeignKeyRelationshipInfos
                          |>Array.filter (fun b->tableInfos|>Array.exists(fun c->c.TABLE_NAME=b.PK_TABLE)) 
                          |>Array.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                        | Some z  when z.PK_TABLE=tableName ->z.PK_COLUMN_NAME
                        | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                    | _ ->String.Empty
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                  {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateMultiCreateCodeForIndependentTableWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])  (tableInfos:TableInfo[])= 
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
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
              {8}   
              sb.{2}.AddObject(entity{3})
          {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""      
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录，同时新增总帐表"+String.Empty+ @""""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->
              x.TableColumnInfos
              |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              entity{0}.{1}<-
                match new {1}({3}) with
                | {2} ->
                    {4}
                    {2}"
          ,
          //{0}  
          String.Empty
          (*
          match tableName,tableName.Split('_') with  //update it to t_DJ...
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          ,
          //{1},
          zzTableName
          ,
          //{2} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{3}
          (
          String.Empty
          (*主键值已由关联的父表自动提供 
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          *)
          )
          ,
          //{4}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when   //不是主键列
                match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                | Some y ->
                    y.TablePrimaryKeyInfos|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not 
                | _ ->false 
                ->  //不是主键列
                sbTemSub.AppendFormat(@"
                    {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if  tableColumns|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                    | Some y ->
                        match 
                          y.TableForeignKeyRelationshipInfos
                          |>Array.filter (fun b->tableInfos|>Array.exists(fun c->c.TABLE_NAME=b.PK_TABLE)) 
                          |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                        | Some z  when z.PK_TABLE=tableName ->z.PK_COLUMN_NAME
                        | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                    | _ ->String.Empty
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                    {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


(*
match 
  (""{2}"",new {2}({0}))
  |>sb.CreateEntityKey 
  |>fun a ->sb.TryGetObjectByKey(a,ref Unchecked.defaultof<_>) with
| true -> failwith ""The record is exist！"" | _ ->()
可改写成
match 
  (""{2}"",new {2}({0}))
  |>sb.CreateEntityKey 
  |>sb.TryGetObjectByKey with
| true, _ -> failwith ""The record is exist！"" | _ ->()

*)

(*只模板空格问题 
  static member private GenerateSingleCreateCodeForIndependentTable (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) =
    @"
    member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        new {2}
          ({4})
        |>fun {3} ->
            {5}    
            sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
  |>DACodingIndependentTablePartX.GenerateCreateCodeForIndependentTable tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns

  static member private GenerateMultiCreateCodeForIndependentTable (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) =
    @"
    member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try 
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          new {2}
            ({4})
          |>fun {3} ->
              {5}    
              sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1"
  |>DACodingIndependentTablePartX.GenerateCreateCodeForIndependentTable tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns

*)

  static member private GenerateSingleCreateCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match 
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey 
          |>sb.TryGetObjectByKey with
        | true, _ -> failwith ""The record is exist！"" | _ ->()
        match new {2}
         ({4}) with
        | entity{3} ->
            {5}    
            sb.{2}.AddObject(entity{3})
        {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-6,this,CreateEntity,result) 
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
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


  static member private GenerateMultiCreateCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
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
              sb.{2}.AddObject(entity{3})
          {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-7,this,CreateEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-10,this,CreateEntities,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
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


  static member private GenerateSingleCreateConcurrentCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.CreateConcurrent{1} (executeContent:BD_ExecuteContent<#BD_{2}>,context:{6}EntitiesAdvance, now)=
      try
        let businessEntity=executeContent.ExecuteData
        match 
          context.ObjectStateManager.GetObjectStateEntries(EntityState.Added)
          |>PSeq.map (fun a->a.EntityKey.EntitySetName)  with
        | HasNotElementIn [{8}] _ ->None
        | y ->
            match 
              (""{2}"",new {2}({0}))
              |>context.CreateEntityKey 
              |>context.TryGetObjectByKey with
            | true, _ -> failwith ""The record is exist！"" | _ ->()
            {7}
            match new {2}
             ({4}) with
            | entity{3} ->
                {5}
                Some entity{3}
      with
      | :? InvalidOperationException as e->raise e
      | e ->raise e",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName || x=UpdateDateColumnName  ->
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
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                match y|>PSeq.exists (fun a->a=TN.{2}) with
                | true -> ()
                | _ ->
                    entity{0}.{1} <-
                      (""{2}"",new {2}({3}))
                      |>context.CreateEntityKey
                      |>context.GetObjectByKey
                      |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match tableName,tableName.Split('_') with
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
                match y|>PSeq.exists (fun a->a=TN.{3}) with
                | true -> ()
                | _ ->
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
                ,
                //{3}
                (
                match y|>PSeq.head with
                | u,_ ->u.PK_TABLE
                )
                )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                        entity{0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>context.CreateEntityKey
                          |>context.GetObjectByKey
                          |>unbox<{2}>
                    | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
            ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
            |>DA_{11}Helper.WriteBusinessLog(executeContent,context,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (
        tableAsFKRelationships
        |>PSeq.distinctBy (fun a->a.PK_TABLE)
        |>PSeq.map (fun a->"TN."+a.PK_TABLE)
        |>PSeq.fold (fun acc a->match acc with "" ->a | _ ->acc+";"+a) "" //TN.T1;TN.T2
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateMultiCreateConcurrentCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
    member this.CreateConcurrent{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,context:{6}EntitiesAdvance, now)=
      try
        let businessEntities=executeContent.ExecuteData
        match 
          context.ObjectStateManager.GetObjectStateEntries(EntityState.Added)
          |>PSeq.map (fun a->a.EntityKey.EntitySetName)  with
        | HasNotElementIn [{8}] _ ->Array.zeroCreate 0
        | y ->
            [|for businessEntity in businessEntities do
                match 
                  (""{2}"",new {2}({0}))
                  |>context.CreateEntityKey 
                  |>context.TryGetObjectByKey with
                | true, _ -> failwith ""The record is exist！"" | _ ->()
                {7}
                match new {2}
                 ({4}) with
                | entity{3} ->
                    {5}
                    yield entity{3} |]
      with
      | :? InvalidOperationException as e->raise e
      | e ->raise e",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
              | z,true when z.ToLowerInvariant().EndsWith("guid")  ->
                  sbTem.AppendFormat( @"
            {0}=Guid.NewGuid(),",  //如果在这里新建Guid的话，那么客户端的同一张单据可以无数次的保存为新的单据, ！！！
                    x
                    )|>ignore
              *)
              | EndsWith DateTimeTypeName _ when x=CreateDateColumnName || x=UpdateDateColumnName  ->
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
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.head with
              | u,_ ->
                  sbTem.AppendFormat( @"
                    match y|>PSeq.exists (fun a->a=TN.{2}) with
                    | true -> ()
                    | _ ->
                        entity{0}.{1} <-
                          (""{2}"",new {2}({3}))
                          |>context.CreateEntityKey
                          |>context.GetObjectByKey
                          |>unbox<{2}>",
                    //{0}
                    String.Empty
                    (*
                    match tableName,tableName.Split('_') with
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
                    match y|>PSeq.exists (fun a->a=TN.{3}) with
                    | true -> ()
                    | _ ->
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
                ,
                //{3}
                (
                match y|>PSeq.head with
                | u,_ ->u.PK_TABLE
                )
                )|>ignore
               
              match y|>PSeq.head|>fst with
              | u->
                  sbTem.AppendFormat( @"
                            entity{0}.{1} <-
                              (""{2}"",new {2}({3}))
                              |>context.CreateEntityKey
                              |>context.GetObjectByKey
                              |>unbox<{2}>
                        | _ ->()",
                    //{0}
                    String.Empty
                    (*
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
                |>DA_{11}Helper.WriteBusinessLog(executeContent,context,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (
        tableAsFKRelationships
        |>PSeq.distinctBy (fun a->a.PK_TABLE)
        |>PSeq.map (fun a->"TN."+a.PK_TABLE)
        |>PSeq.fold (fun acc a->match acc with "" ->a | _ ->acc+";"+a) "" //TN.T1;TN.T2
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


(*空格问题
  static member private GenerateSingleUpdateCodeForIndependentTable   (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) =
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
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DACodingIndependentTablePartX.GenerateUpdateCodeForIndependentTable  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns

  static member private GenerateMultiUpdateCodeForIndependentTable   (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) =
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
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1"
  |>DACodingIndependentTablePartX.GenerateUpdateCodeForIndependentTable  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
*)

  static member private GenerateSingleUpdateCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])  (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        match
          (""{2}"",new {2}({0}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false, _ -> failwith ""The record is not exist!""
        | _, x -> unbox<{2}> x
        |>fun original ->
            {4}
            {5}    
            ()
        {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-15,this,UpdateEntity,result)",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships,tableKeyColumns with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
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
          | ColumnConditionTypeContains [HasLSH] _->
              @"""更新"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateMultiUpdateCodeForIndependentTable  (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
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
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
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
              () 
          {7}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-15,this,UpdateEntities,result)",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships,tableKeyColumns with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        entityContextNamePrefix
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
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
          | ColumnConditionTypeContains [HasLSH] _->
              @"""更新"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member  GenerateDeleteCodeForIndependentTableWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[])   (tableKeyColumns:TablePrimaryKeyInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[])  (columnConditionTypes:ColumnConditionType[])   (tableInfos:TableInfo[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {4}EntitiesAdvance()
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _, x ->
            sb.{2}.DeleteObject (x:?>{2})
        {6}
        {5}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? UpdateException as e -> match context with Some _ ->raise e | _ ->this.AttachError(e,-16,this,DeleteEntity,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-20,this,DeleteEntity,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
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
        entityContextNamePrefix
        ,
        //{5}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "5uy"
          ,
          //{6}
          @"""删除(同时删除总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时删除总帐表"+String.Empty+ @""""
          | _ ->
              @"""删除"+ String.Empty + @"的记录，同时删除总帐表"+String.Empty+ @""""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{6} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableKeyColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->x.TablePrimaryKeyInfos
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        match
          (""{0}"",new {0}({1}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _, x ->
            sb.{0}.DeleteObject (x:?>{0})"
          ,
          //{0},
          zzTableName
          ,
          //{1}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          (* 正确，但需要限制主表和总账表都只有单一主键列
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          *)
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


(*
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>fun a ->
              let b=ref Unchecked.defaultof<_>
              sb.TryGetObjectByKey(a,b), !b with
        | false,_ -> failwith ""The record is not exist!""
        | _,x ->

可该写成
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""

*)
  static member  GenerateDeleteCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[])   (tableKeyColumns:TablePrimaryKeyInfo[])  (columnConditionTypes:ColumnConditionType[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try
        let businessEntity=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {4}EntitiesAdvance()
        match
          (""{2}"",new {2}({3}))
          |>sb.CreateEntityKey
          |>sb.TryGetObjectByKey with
        | false,_ -> failwith ""The record is not exist!""
        | _, x ->
            sb.{2}.DeleteObject (x:?>{2})
        {5}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-16,this,DeleteEntity,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-20,this,DeleteEntity,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
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
        entityContextNamePrefix
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
        ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
        |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
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
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""删除"+ String.Empty + @"的记录"""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member  GenerateMultiDeleteCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[])   (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime)=
      let result=new BD_ExecuteResult()
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      result.ExecuteDateTime<-now
      try
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {4}EntitiesAdvance()
        for businessEntity in businessEntities do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _, x ->
              sb.{2}.DeleteObject (x:?>{2})
          {5}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-17,this,DeleteEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-20,this,DeleteEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
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
        entityContextNamePrefix
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
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
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""删除"+ String.Empty + @"的记录"""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateBatchCodeForIndependentTableWithLSHWithZZ  (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[]) (tableInfos:TableInfo[])= 
    try
      let sb=StringBuilder()
      sb.AppendFormat(@"{0}
    member this.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {3}EntitiesAdvance()
        {4}
        {5}
        {6}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-21,this,BatchEntities,result)
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-23,this,BatchEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-30,this,BatchEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        entityContextNamePrefix
        ,
        //{4}
        DACodingIndependentTablePartX.GenerateBatchCreateCodeForIndependentTableWithLSHWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableInfos
        |>fun (a:string)->a.Trim()
        ,
        //{5}
        DACodingIndependentTablePartX.GenerateBatchUpdateCodeForIndependentTable  databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns columnConditionTypes
        |>fun (a:string)->a.Trim()
        ,
        //{6}
        DACodingIndependentTablePartX.GenerateBatchDeleteCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName  tableColumns tableKeyColumns tableAsPKRelationships columnConditionTypes tableInfos
        |>fun (a:string)->a.Trim()
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateBatchCodeForIndependentTableWithZZ  (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) (columnConditionTypes:ColumnConditionType[]) (tableInfos:TableInfo[])= 
    try
      let sb=StringBuilder()
      sb.AppendFormat(@"{0}
    member this.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {3}EntitiesAdvance()
        {4}
        {5}
        {6}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-21,this,BatchEntities,result)
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-23,this,BatchEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-30,this,BatchEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        entityContextNamePrefix
        ,
        //{4}
        DACodingIndependentTablePartX.GenerateBatchCreateCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns tableInfos
        |>fun (a:string)->a.Trim()
        ,
        //{5}
        DACodingIndependentTablePartX.GenerateBatchUpdateCodeForIndependentTable  databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns columnConditionTypes
        |>fun (a:string)->a.Trim()
        ,
        //{6}
        DACodingIndependentTablePartX.GenerateBatchDeleteCodeForIndependentTableWithZZ databaseInstanceName entityContextNamePrefix tableName  tableColumns tableKeyColumns tableAsPKRelationships columnConditionTypes tableInfos
        |>fun (a:string)->a.Trim()
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateBatchCodeForIndependentTableWithLSH  (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sb=StringBuilder()
      sb.AppendFormat(@"{0}
    member this.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {3}EntitiesAdvance()
        {4}
        {5}
        {6}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-21,this,BatchEntities,result)
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-23,this,BatchEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-30,this,BatchEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        entityContextNamePrefix
        ,
        //{4}
        DACodingIndependentTablePartX.GenerateBatchCreateCodeForIndependentTableWithLSH databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
        |>fun (a:string)->a.Trim()
        ,
        //{5}
        DACodingIndependentTablePartX.GenerateBatchUpdateCodeForIndependentTable  databaseInstanceName entityContextNamePrefix tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns columnConditionTypes
        |>fun (a:string)->a.Trim()
        ,
        //{6}
        DACodingIndependentTablePartX.GenerateBatchDeleteCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName  tableColumns tableKeyColumns  columnConditionTypes
        |>fun (a:string)->a.Trim()
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateBatchCodeForIndependentTable  (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sb=StringBuilder()
      sb.AppendFormat(@"{0}
    member this.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>,?context, ?currentDateTime, ?bd_ExecuteResult)=
      let now=match currentDateTime with Some x->x | _ -> DateTime.Now
      let result=match bd_ExecuteResult with Some x->x | _ -> new BD_ExecuteResult(ExecuteDateTime=now)
      try 
        let businessEntities=executeContent.ExecuteData
        let sb=match context with Some x ->x | _ ->new {3}EntitiesAdvance()
        {4}
        {5}
        {6}
        match context  with Some _->() | _ ->result.ResultLength<-sb.SaveChanges(); sb.Dispose()
        result
      with
      | :? InvalidOperationException as e->match context with Some _ ->raise e | _ ->this.AttachError(e,-21,this,BatchEntities,result)
      | :? UpdateException as e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-23,this,BatchEntities,result)
      | e ->match context with Some _ ->raise e | _ ->this.AttachError(e,-30,this,BatchEntities,result)",
        //{0}
        String.Empty
        ,
        //{1}
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2}
        tableName
        ,
        //{3}
        entityContextNamePrefix
        ,
        //{4}
        DACodingIndependentTablePartX.GenerateBatchCreateCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName  tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns
        |>fun (a:string)->a.Trim()
        ,
        //{5}
        DACodingIndependentTablePartX.GenerateBatchUpdateCodeForIndependentTable  databaseInstanceName entityContextNamePrefix  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns columnConditionTypes
        |>fun (a:string)->a.Trim()
        ,
        //{6}
        DACodingIndependentTablePartX.GenerateBatchDeleteCodeForIndependentTable databaseInstanceName entityContextNamePrefix tableName  tableColumns tableKeyColumns  columnConditionTypes
        |>fun (a:string)->a.Trim()
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateBatchCreateCodeForIndependentTableWithLSH (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
        match sb.T_LSH_{1}|>PSeq.head with
        | entityLSH->
            for businessEntity in 
              businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Created)  do
              (businessEntity.{8},entityLSH.C_LSH)|>result.GuidDecimals.Add
              businessEntity.C_XBH<- entityLSH.C_LSH
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              match new {2}
               ({4}) with
              | entity{3} ->
                  {5}    
                  sb.{2}.AddObject(entity{3})
              entityLSH.C_LSH<-entityLSH.C_LSH+1M
              {7}{6}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        String.Empty
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
              |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateBatchCreateCodeForIndependentTableWithLSHWithZZ (databaseInstanceName:string)  (entityContextNamePrefix:string)  (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[]) (tableInfos:TableInfo[])=  
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"
        match sb.T_LSH_{1}|>PSeq.head with
        | entityLSH->
            for businessEntity in 
              businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Created)  do
              (businessEntity.{9},entityLSH.C_LSH)|>result.GuidDecimals.Add
              businessEntity.C_XBH<- entityLSH.C_LSH
              match 
                (""{2}"",new {2}({0}))
                |>sb.CreateEntityKey 
                |>sb.TryGetObjectByKey with
              | true, _ -> failwith ""The record is exist！"" | _ ->()
              match new {2}
               ({4}) with
              | entity{3} ->
                  {5}
                  {8}    
                  sb.{2}.AddObject(entity{3})
              entityLSH.C_LSH<-entityLSH.C_LSH+1M
              {7}{6}",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        match tableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        String.Empty
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
              |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          "4uy"
          ,
          //{4}
          @""""+"基本表处理"+ @""""
          ,
          //{5}
          "4uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时新增总帐表"+String.Empty+ @""""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->
              x.TableColumnInfos
              |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
                  entity{0}.{1}<-
                    match new {1}({3}) with
                    | {2} ->
                        {4}
                        {2}"
          ,
          //{0}  
          String.Empty
          (*
          match tableName,tableName.Split('_') with  //update it to t_DJ...
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          ,
          //{1},
          zzTableName
          ,
          //{2} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{3}
          (
          String.Empty
          (*主键值已由关联的父表自动提供 
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          *)
          )
          ,
          //{4}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when 
                match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                | Some y ->
                    y.TablePrimaryKeyInfos|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not 
                | _ ->false 
                ->  //不是主键列
                sbTemSub.AppendFormat(@"
                        {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if  tableColumns|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                    | Some y ->
                        match 
                          y.TableForeignKeyRelationshipInfos
                          |>Array.filter (fun b->tableInfos|>Array.exists(fun c->c.TABLE_NAME=b.PK_TABLE)) 
                          |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                        | Some z  when z.PK_TABLE=tableName ->z.PK_COLUMN_NAME
                        | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                    | _ ->String.Empty
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                        {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{9}
        (tableKeyColumns|>PSeq.head).COLUMN_NAME
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateBatchUpdateCodeForIndependentTable  (databaseInstanceName:string)  (entityContextNamePrefix:string)  (tableName:string)   (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])= //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{3}{1}
        for businessEntity in 
          businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Updated) do
          match
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _,x -> unbox<{2}> x
          |>fun original ->
              {4}
              {5}  
              ()
          {7}{6}",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        //{1},
        String.Empty
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships,tableKeyColumns with
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
          (tableAsFKRelationships,tableColumns)
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
        //{6}
        String.Empty
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
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
          | ColumnConditionTypeContains [HasLSH] _->
              @"""更新"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member  GenerateBatchDeleteCodeForIndependentTableWithZZ (databaseInstanceName:string)  (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[])   (tableKeyColumns:TablePrimaryKeyInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[])   (columnConditionTypes:ColumnConditionType[])  (tableInfos:TableInfo[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}{1}
        for businessEntity in 
          businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Deleted) do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _, x ->
              sb.{2}.DeleteObject (x:?>{2}) 
          {6}
          {5}{4}",
        //{0}
        String.Empty
        ,
        //{1}
        String.Empty
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
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
        String.Empty
        ,
        //{5}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
          | _ ->
              @""""+"单表处理"+ @""""
          )
          ,
          //{5}
          "5uy"
          ,
          //{6}
          @"""删除(同时删除总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录，同时删除总帐表"+String.Empty+ @""""
          | _ ->
              @"""删除"+ String.Empty + @"的记录，同时删除总帐表"+String.Empty+ @""""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{6} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableKeyColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->x.TablePrimaryKeyInfos
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          match
            (""{0}"",new {0}({1}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""The record is not exist!""
          | _, x ->
              sb.{0}.DeleteObject (x:?>{0})"
          ,
          //{0},
          zzTableName
          ,
          //{1}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          sbTemSub.AppendFormat(@"{0}=businessEntity.{1}"
              ,
              //{0},须验证主表和总账表都只有一个主键
              (zzTableKeyColumns|>PSeq.head).COLUMN_NAME
              ,
              //{1}
              (tableKeyColumns|>PSeq.head).COLUMN_NAME
              )|>ignore
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e


//-------------------------------------------------------------------------------------------------------------------------------


  static member private GenerateBatchCreateCodeForIndependentTableWithZZ (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])  (tableInfos:TableInfo[])= 
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{1}
        for businessEntity in 
          businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Created)  do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          match new {2}
           ({4}) with
          | entity{3} ->
              {5}  
              {8}  
              sb.{2}.AddObject(entity{3})
          {7}{6}",
      
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        //{1}
        String.Empty
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3}
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              (*
              match a.DATA_TYPE, tableKeyColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        String.Empty
        ,
        //{7}
        (
        let zzTableName="T_ZZ_"+match tableName with x ->x.Remove(0,2)
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
            |>fun (a,b) ->PSeq.join a b (fun a->a.COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
            do
            sbTemSub.AppendFormat(@"{0},{1}=""+(businessEntity.{0}|>string)+""|",
              //{0}
              a.COLUMN_NAME
              ,
              //{1}
              tableAsPKRelationships
              |>PSeq.find (fun c->c.FK_TABLE=zzTableName)
              |>fun a->a.FK_COLUMN_NAME 
              )|>ignore
          match sbTemSub with
          | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '+"|'
          | _ ->()
          sbTemSub.Insert(0,@"""")|>ignore
          sbTemSub.ToString().TrimStart()
          )
          ,
          //{1}
          @""""+tableName+","+zzTableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty+","+String.Empty+ @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增(同时新增总账表)"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录，同时新增总帐表"+String.Empty+ @""""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
          ,
          //{11}
          entityContextNamePrefix
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
        ,
        //{8} For T_ZZ_XX
        (
        let zzTableName=
          "T_ZZ_"+
          match tableName with
          | x when x.StartsWith("T_") ->x.Remove(0,2)
          | x -> x
        let zzTableColumns=
          match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
          | Some x ->
              x.TableColumnInfos
              |>Array.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
          | _ ->[||]
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
              entity{0}.{1}<-
                match new {1}({3}) with
                | {2} ->
                    {4}
                    {2}"
          ,
          //{0}  
          String.Empty
          (*
          match tableName,tableName.Split('_') with  //update it to t_DJ...
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          *)
          ,
          //{1},
          zzTableName
          ,
          //{2} 
          match zzTableName,zzTableName.Split('_') with  //
          | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
          ,
          //{3}
          (
          String.Empty
          (*主键值已由关联的父表自动提供 
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in tableAsPKRelationships|>PSeq.filter (fun a->a.FK_TABLE=zzTableName) do
            sbTemSub.AppendFormat(@"{0}=businessEntity.{1},"
              ,
              //{0}
              a.FK_COLUMN_NAME
              ,
              //{1}
              a.PK_COLUMN_NAME
              )|>ignore
          match sbTemSub with
          | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore  //Remove the last of ','
          | _ ->()
          sbTemSub.ToString().TrimStart()
          *)
          )
          ,
          //{4}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for a in zzTableColumns do
            match a.COLUMN_NAME,a.DATA_TYPE with
            | x, EndsWithIn GuidConditions _   when 
                match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                | Some y ->
                    y.TablePrimaryKeyInfos|>PSeq.exists (fun b->b.COLUMN_NAME=x ) |>not 
                | _ ->false 
                ->  //不是主键列
                sbTemSub.AppendFormat(@"
                    {0}.{1}<-businessEntity.{2}"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  ,
                  //{2}
                  if  tableColumns|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not then //在主表中不存在
                    match tableInfos|>Seq.tryFind (fun a->a.TABLE_NAME=zzTableName) with
                    | Some y ->
                        match 
                          y.TableForeignKeyRelationshipInfos
                          |>Array.filter (fun b->tableInfos|>Array.exists(fun c->c.TABLE_NAME=b.PK_TABLE)) 
                          |>PSeq.tryFind (fun b->b.FK_COLUMN_NAME=x) with //已经在验证环节验证，所以可以不用PSeq.find
                        | Some z  when z.PK_TABLE=tableName ->z.PK_COLUMN_NAME
                        | _ ->String.Empty //生成代码后，将在编译时报错，需要验证
                    | _ ->String.Empty
                  else
                    x
                  )|>ignore
            | x, EndsWithIn DateTimeConditions _ ->         //其实是对于C_GXRQ和C_CJRQ
                sbTemSub.AppendFormat(@"
                    {0}.{1}<-now"
                  ,
                  //{0}
                  match zzTableName,zzTableName.Split('_') with  
                  | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                  ,
                  //{1}
                  x
                  )|>ignore
            | _ ->()
          sbTemSub.ToString().TrimStart()
          )
          )|>ignore
        sbTem.ToString().TrimStart()
        )
      )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member  GenerateBatchDeleteCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[])   (tableKeyColumns:TablePrimaryKeyInfo[])   (columnConditionTypes:ColumnConditionType[])=
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    try
      sb.AppendFormat(  @"{0}{1}
        for businessEntity in 
          businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Deleted) do
          match
            (""{2}"",new {2}({3}))
            |>sb.CreateEntityKey
            |>sb.TryGetObjectByKey with
          | false,_ -> failwith ""One of records is not exist!""
          | _, x ->
              sb.{2}.DeleteObject (x:?>{2})
          {5}{4}",
        //{0}
        String.Empty
        ,
        //{1}
        String.Empty
        ,
        //{2}
        tableName
        ,
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in  tableKeyColumns do
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
        String.Empty
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->"4uy"
          | _ ->"5uy" 
          )
          ,
          //{4}
          (
          match columnConditionTypes with
          | ColumnConditionTypeContains [HasLSH] _->
              @""""+"基本表处理"+ @""""
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
          | ColumnConditionTypeContains [HasLSH] _->
              @"""删除"+ String.Empty + @"编号为""+(businessEntity.C_XBH|>string)+""的记录"""
          | _ ->
              @"""删除"+ String.Empty + @"的记录"""
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
          )|>ignore
        sbTem.ToString().TrimStart() 
        )
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

//-------------------------------------------------------------------------------------------------------------------------------

  static member private GenerateBatchCreateCodeForIndependentTable (databaseInstanceName:string) (entityContextNamePrefix:string) (tableName:string)  (tableColumns:TableColumnInfo[]) (tableAsFKRelationships:TableForeignKeyRelationshipInfo[]) (tableAsPKRelationships:TablePrimaryKeyRelationshipInfo[]) (tableKeyColumns:TablePrimaryKeyInfo[])=  //(codeTemplate:string)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(@"{1}
        for businessEntity in 
          businessEntities|>PSeq.filter (fun a->a.TrackingState=TrackingState.Created) do
          match 
            (""{2}"",new {2}({0}))
            |>sb.CreateEntityKey 
            |>sb.TryGetObjectByKey with
          | true, _ -> failwith ""The record is exist！"" | _ ->()
          match new {2}
           ({4}) with
          | entity{3} ->
              {5}    
              sb.{2}.AddObject(entity{3})
          {7}{6}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableKeyColumns  do
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
        String.Empty
        ,
        //{2},T_DJ_JHGL
        tableName
        ,
        //{3} 
        String.Empty
        (*
        match tableName,tableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
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
          (tableAsFKRelationships,tableColumns)
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
                    match tableName,tableName.Split('_') with
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
                    match tableName,tableName.Split('_') with
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
        //{6}
        String.Empty
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        sbTem.AppendFormat(@"
          ({0},{1},{2},{3},{4},{5},{6},new Nullable<_>({7}),{8},{9},{10})
          |>DA_{11}Helper.WriteBusinessLog(executeContent,sb,now)",
          //{0}
          (
          sbTemSub.Remove(0,sbTemSub.Length) |>ignore
          for (a,b) in 
            (tableKeyColumns,tableColumns)
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
          @""""+tableName+ @""""
          ,
          //{2}, 需要增加一个表的字典表，然后通过表明查询到表的中文名
          @""""+ String.Empty + @""""
          ,
          //{3} 
          "5uy"
          ,
          //{4}
          @""""+"单表处理"+ @""""
          ,
          //{5}
          "1uy"
          ,
          //{6}
          @"""新增"""
          ,
          //{7}
          String.Empty
          ,
          //{8}
          @"""新增"+ String.Empty + @"记录"""
          ,
          //{9}
          "null"
          ,
          //{10}
          "null"
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
            @"""更新"+ String.Empty + @"，单据号为""+(modelInstance.C_DJH|>string)+""的主表记录"""
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

//-------------------------------------------------------------------------------------------------------------------------------
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
        | ColumnConditionTypeAllEquals [HasLSH] _->
            @"""删除"+ String.Empty + @"，编号为""+(modelInstance.C_XBH|>string)+""的记录"""
        | ColumnConditionTypeAllEquals [HasJYH] _->
            @"""删除"+ String.Empty + @"，交易号为""+(modelInstance.C_JYH|>string)+""的记录"""
        | _ ->
            @"""删除"+ String.Empty + @"的记录"""
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


