namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper

type DataAccessCodingLeafTablePart=

  static member GetCodeWithLeafTableTemplate  (databaseInstanceName:string) (tableName:string) (tableRelatedInfo:TableRelatedInfo)=
    let sb=StringBuilder()
    let tableColumns=
      DatabaseInformation.GetColumnSchemal4Way tableName
      |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
    let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
    let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
    let tableKeyColumns=DatabaseInformation.GetPKColumns tableName

    DataAccessCodingLeafTablePart.GenerateQueryCodeForLeafTable databaseInstanceName tableName tableColumns tableAsFKRelationships tableAsPKRelationships
    |>box|>sb.Append |>ignore //QueryCode
    sb.AppendLine()|>ignore
    //WriteLog for update //子表更新写日志
    DataAccessCodingLeafTablePart.GenerateWriteLogForUpdateCode  databaseInstanceName  tableName tableColumns  tableKeyColumns tableRelatedInfo.ColumnConditionTypes
    |>box|>sb.Append|>ignore
    sb.AppendLine()|>ignore
    string sb

  static member private GenerateQueryCodeForLeafTable (databaseInstanceName:string) tableName tableColumns tableAsFKRelationships tableAsPKRelationships=
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
    member this.Get{1}s (queryEntity:BQ_{1},?context)=
      let pagingInfo=queryEntity.PagingInfo
      let result=new BD_QueryResult<BD_{2}[]>(PagingInfo=pagingInfo,ExecuteDateTime=DateTime.Now)
      try
        let sb=match context with Some x ->x | _ ->new {6}EntitiesAdvance()
        sb.{2}{0}
        |>PSeq.filter (fun a->
            {3})
        |>fun a->
            if pagingInfo.TotalCount=0 then pagingInfo.TotalCount<- a|>PSeq.length
            a  
        |>PSeq.skip (pagingInfo.PageSize * pagingInfo.PageIndex)
        |>PSeq.take pagingInfo.PageSize
        |>PSeq.map (fun a->
            match new BD_{2}
             ({4}) with
            | entity ->
                {5}
                entity)
        |>PSeq.toArray 
        |>fun a ->
            match context with Some _ ->() | _ ->sb.Dispose()
            a
        |>Seq.toResult result
      with 
      | e -> match context with Some _ ->raise e | _ ->this.AttachError(e,-1,this,GetEntities,result)",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        tableAsFKRelationships
        |>PSeq.map (fun a->a. PK_TABLE_ALIAS)
        |>PSeq.distinct //一个主键可能由两个或两个以上键列构成
        |>Seq.iter (fun a->
            sbTem.AppendFormat(@".Include(FTN.{0})",
              //{0}
              a
              )|>ignore)
        sbTem.ToString()
        )
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
        for a in 
          tableColumns
          |>PSeq.filter (fun b->tableAsFKRelationships|>PSeq.exists (fun c->c.FK_COLUMN_NAME=b.COLUMN_NAME) |>not) do
          match a.IS_NULLABLE_TYPED with
          | false ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      match x with
                      | EqualsIn ContainQueryConditions _ ->
                          sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().Contains 
            | _ ->true
            && ",
                            //{0}
                            x
                            )|>ignore
                      | _ ->
                          sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | _ ->true
            && ",
                            //{0}
                            x
                            )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions y ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second with
            | x,y,z when y.HasValue && z.HasValue && y.Value={1} ->x<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value={2} ->x>=y.Value 
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x>=y.Value && x<=z.Value
            | x,y,_ when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    ,
                    //{1}
                    RangeTypeMinValueDic.[y.ToLowerInvariant()]
                    ,
                    //{2}
                    RangeTypeMaxValueDic.[y.ToLowerInvariant()]
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore
          | _ ->
              match a.COLUMN_NAME, a.DATA_TYPE with
              (*
              | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              *)
              | x, EndsWithIn NullableTypeConditions y ->
                  match y with
                  | EndsWithIn FuzzyQueryConditions _ ->
                      match x with
                      | EqualsIn ContainQueryConditions _ ->
                          sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->y.ToLowerInvariant()|>x.ToLowerInvariant().Contains 
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                            //{0}
                            x
                            )|>ignore
                      | _ ->
                          sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->y.ToLowerInvariant()|>x.ToLowerInvariant().StartsWith 
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                            //{0}
                            x
                            )|>ignore
                  | _ ->
                      sbTem.AppendFormat( @"   
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y<>null && x<>null->x.Equals(y)
            | x,y,_ when y<>null && x=null ->false
            | x,_,true when x<>null->false
            | _ ->true
            && ",
                        //{0}
                        x
                        )|>ignore
              | x, EndsWithIn QueryRangeTypeConditions y ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.{0}Second,queryEntity.IsQueryableNullOf{0} with
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && y.Value={1} ->x.Value<=z.Value
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && z.Value={2} ->x.Value>=y.Value
            | x,y,z,_ when y.HasValue && z.HasValue && x.HasValue && z.Value>y.Value ->x.Value>=y.Value && x.Value<=z.Value
            | x,y,z,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,z,_ when y.HasValue && not x.HasValue ->false
            | x,_,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    ,
                    //{1}
                    RangeTypeMinValueDic.[y.ToLowerInvariant()]
                    ,
                    //{2}
                    RangeTypeMaxValueDic.[y.ToLowerInvariant()]
                    )|>ignore
              | x,_ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0},queryEntity.IsQueryableNullOf{0} with
            | x,y,_ when y.HasValue && x.HasValue->x.Equals(y)
            | x,y,_ when y.HasValue && not x.HasValue ->false
            | x,_,true when x.HasValue->false
            | _ ->true
            && ",
                    //{0}
                    x
                    )|>ignore

        (* case
            //该表作为当前表的外键列全为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue && x<>null->x.C_ID =y.Value
            | _ ->true
            &&  
            //该表作为当前表的外键列全不为空时
            match a.T_YG2,queryEntity.C_SHR with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&
            match a.T_YG2,queryEntity.C_SHR1 with
            | x,y when y.HasValue->x.C_ID =y.Value
            | _ ->true
            &&  
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        match v.COLUMN_NAME with
                        | EqualsIn ContainQueryConditions _ ->
                            sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.{2}.ToLowerInvariant().Contains 
            | _ ->true
            && ",
                              //{0
                              u.PK_TABLE_ALIAS
                              ,
                              //{1}
                              u.FK_COLUMN_NAME
                              ,
                              //{2}
                              u.PK_COLUMN_NAME
                              )|>ignore
                        | _ ->
                            sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->y.ToLowerInvariant()|>x.{2}.ToLowerInvariant().StartsWith 
            | _ ->true
            && ",
                              //{0
                              u.PK_TABLE_ALIAS
                              ,
                              //{1}
                              u.FK_COLUMN_NAME
                              ,
                              //{2}
                              u.PK_COLUMN_NAME
                              )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && y.Value={3} ->x.{2}<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value={4} ->x.{2}>=y.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value ->x.{2}>=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      ,
                      //{3}
                      RangeTypeMinValueDic.[v.DATA_TYPE.ToLowerInvariant()]
                      ,
                      //{4}
                      RangeTypeMaxValueDic.[v.DATA_TYPE.ToLowerInvariant()]
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue ->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore

          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              for b in y do 
                match b with
                (*
                | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                *)
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn NullableTypeConditions ->
                    match v.DATA_TYPE with
                    | EndsWithIn FuzzyQueryConditions _ ->
                        match v.COLUMN_NAME with
                        | EqualsIn ContainQueryConditions _ ->
                            sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->y.ToLowerInvariant()|>x.{2}.ToLowerInvariant().Contains 
            | _ ->true
            && ",
                              //{0
                              u.PK_TABLE_ALIAS
                              ,
                              //{1}
                              u.FK_COLUMN_NAME
                              ,
                              //{2}
                              u.PK_COLUMN_NAME
                              )|>ignore
                        | _ ->
                            sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->y.ToLowerInvariant()|>x.{2}.ToLowerInvariant().StartsWith 
            | _ ->true
            && ",
                              //{0
                              u.PK_TABLE_ALIAS
                              ,
                              //{1}
                              u.FK_COLUMN_NAME
                              ,
                              //{2}
                              u.PK_COLUMN_NAME
                              )|>ignore
                    | _ -> 
                        sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y<>null && x<>null ->x.{2} =y
            | _ ->true
            && ",
                          //{0
                          u.PK_TABLE_ALIAS
                          ,
                          //{1}
                          u.FK_COLUMN_NAME
                          ,
                          //{2}
                          u.PK_COLUMN_NAME
                          )|>ignore
                | u,v when v.DATA_TYPE|>Comm.isEndsWithIn QueryRangeTypeConditions ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1},queryEntity.{1}Second with
            | x,y,z when y.HasValue && z.HasValue && y.Value={3} && x<>null->x.{2}<=z.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value={4} && x<>null->x.{2} >=y.Value
            | x,y,z when y.HasValue && z.HasValue && z.Value>y.Value && x<>null->x.{2} >=y.Value && x.{2}<=z.Value
            | x,y,_ when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      ,
                      //{3}
                      RangeTypeMinValueDic.[v.DATA_TYPE.ToLowerInvariant()]
                      ,
                      //{4}
                      RangeTypeMaxValueDic.[v.DATA_TYPE.ToLowerInvariant()]
                      )|>ignore
                | u,_  ->
                    sbTem.AppendFormat( @"
            match a.{0},queryEntity.{1} with
            | x,y when y.HasValue && x<>null->x.{2} =y.Value
            | _ ->true
            && ",
                      //{0
                      u.PK_TABLE_ALIAS
                      ,
                      //{1}
                      u.FK_COLUMN_NAME
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-3,3)|>ignore //Remove the last of "&& "
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in tableColumns do
          match a.COLUMN_NAME,tableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              sbTem.AppendFormat( @"
              {0}=a.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        //if sbTem.Length>0 then sbTem.Remove(sbTem.Length-1,1)
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
            entity.C_JBR<-a.T_YG1.C_ID            
            match a.T_YG2 with
            | x when x<>null ->
                entity.C_SHR <- Nullable<System.Guid>(x.C_ID) 
                entity.C_SHR1 <- Nullable<System.Guid>(x.C_ID1) 
            | _ ->()
        *)
        for _,a in 
          (tableAsFKRelationships,tableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              for b in y do
                match b with
                | u,_  ->
                    sbTem.AppendFormat(@"
                entity.{0}<-a.{1}.{2}",
                      //{0}
                      u.FK_COLUMN_NAME
                      ,
                      //{1}
                      u.PK_TABLE_ALIAS
                      ,
                      //{2}
                      u.PK_COLUMN_NAME
                      )|>ignore
          | y -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat( @"
                match a.{0} with
                | x when x<>null ->
                    {1}
                | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                ,
                //{1}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                    entity.{0} <- Nullable<{1}>(x.{2})",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        v.DATA_TYPE
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                sbTemSub.ToString().TrimStart()
                )
                )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{6}
        databaseInstanceName
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  //---------------------------------------------------------------------------------------------

  static member private GenerateWriteLogForUpdateCode (databaseInstanceName:string)   (tableName:string)  (tableColumns:DbColumnSchemalR seq) (tableKeyColumns:DbPKColumn seq)   (columnConditionTypes:ColumnConditionType seq)=  //(codeTemplate:string)=
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
        @""""+ getTableDescription tableName + @""""
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
            @"""更新"+ getTableDescription tableName + @"，编号为""+(modelInstance.C_XBH|>string)+""的记录"""
        | ColumnConditionTypeAllEquals [HasDJLSH] _->
            @"""更新"+ getTableDescription tableName + @"，单据号为""+(modelInstance.C_DJH|>string)+""的子表记录"""
        | ColumnConditionTypeAllEquals [HasJYH] _->
            @"""更新"+ getTableDescription tableName + @"，交易号为""+(modelInstance.C_JYH|>string)+""的记录"""
        | _ ->
            @"""更新"+ getTableDescription tableName + @"的记录"""
        )
        ,
        //{9}
        "null"
        ,
        //{10}
        "null"
        ,
        //{11}
        databaseInstanceName
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