namespace WX.Data.CodeAutomation

open System
open System.Text

open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

//单独生成子表的代码时，只生成查询代码，因为它不能单独进行更新
type DataAccessCoding=
  static member private VariableNames=
    ['x';'y';'z';'u';'v';'w';'a';'b';'c']
    
    (* Dynamic
    let intTem=ref 0
    intTem:=120 //char 120=x
    [for i=0 to 20 do
       yield char !intTem
       incr intTem
       match !intTem with  // x,y,z,u,v,w,a,b,c....
       | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
       | u when u=120 ->intTem:=97 // int 'a'=97
       | _ -> ()
       ]
    *)
       
  static member GetCode (tableNames:string list)=
    let sb=StringBuilder()
    try
      DataAccessCoding.GenerateNamespaceCode
      |>box|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      for tableName in tableNames do
        let tableColumns=
          DatabaseInformation.GetColumnSchemal4Way tableName
          |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
        let tableAsFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
        let tableKeyColumns=DatabaseInformation.GetPKColumns tableName
        
        (* 已前置验证
        match DatabaseInformation.ValidateForeignKeyColumnDesign tableName tableAsFKRelationships tableColumns with
        | x when x.Length>0 ->ObjectDumper.Write(x); failwith "请看验证信息" //raise (Exception("请看输出信息"))
        | _ ->()
        *)
        
        box <| DataAccessCoding.GenerateTypeCode tableName
        |>sb.Append|>ignore //Generate type code
        sb.AppendLine()|>ignore
        
        let tableAsPKRelationships=DatabaseInformation.GetAsPKRelationship tableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
        //let tableKeyColumns= DatabaseInformation.GetPKColumns tableName //KeyColumns

        let mainChildCondition=tableName.Contains("_DJ_") || tableName.Contains("_FDJ_") //只有表名称严格的符合条件时才进行主子表的代码的处理，能改进否？？？
        let childTableRelationships=    //获取信息子表，一般认为信息子表最多只有一个
          tableAsPKRelationships
          |>PSeq.filter (fun a->
              let pkColumns=
                a.FK_TABLE 
                |>DatabaseInformation.GetPKColumns  |>PSeq.toList
              pkColumns.Length >1  //子表有多个主键列,说明是一对多的关系
              &&
              pkColumns|>PSeq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME))   //
          |>PSeq.toList
        match mainChildCondition,childTableRelationships with
        | true,x when x.Length>0 ->
            let childTableName=x.Head.FK_TABLE
            let childTableColumns=
              DatabaseInformation.GetColumnSchemal4Way childTableName
              |>PSeq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
            let childTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship childTableName
            let childTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship childTableName
            DataAccessCoding.GenerateQueryCodeForMainChildTables tableName tableColumns tableAsFKRelationships tableAsPKRelationships childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
            |>box|>sb.Append |>ignore //QueryCode
            sb.AppendLine()|>ignore
            DataAccessCoding.GenerateCreateCodeForMainChildTables  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
            |>box|>sb.Append |>ignore //CreateCode
            sb.AppendLine()|>ignore
            DataAccessCoding.GenerateUpdateCodeForMainChildTables  tableName tableColumns tableAsFKRelationships tableAsPKRelationships tableKeyColumns childTableName childTableColumns childTableAsFKRelationships childTableAsPKRelationships 
            |>box|>sb.Append |>ignore //UpdateCode
            sb.AppendLine()|>ignore
            DataAccessCoding.GenerateDeleteCodeForMainChildTables  tableName    tableKeyColumns childTableName
            |>box|>sb.Append |>ignore //Delete
            sb.AppendLine()|>ignore
            DataAccessCoding.GenerateMultiDeleteCodeForMainChildTables  tableName    tableKeyColumns childTableName
            |>box|>sb.Append |>ignore //MultiDelete
            sb.AppendLine()|>ignore
        | _ ->() //单表查询
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
      
  static member private GenerateNamespaceCode=
    @"namespace WX.Data.DataAccess
open System
open System.Collections.Generic
open Microsoft.Practices.EnterpriseLibrary.Logging
open WX.Data.Helper
open WX.Data
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess"

   /// note: internal new () as this= {{}}    //it's right in the Format(...), it's must be like that, 关于 {{}}，http://msdn.microsoft.com/zh-cn/library/txafckwd(VS.95).aspx
  static member private GenerateTypeCode (mainTableName:string)=
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
type  DA_{1}=
  static member public INS= DA_{1}() 
  internal new () = {{}}
    
  interface IDA_{1} with",
        //{0}
        String.Empty
        ,
        //{1}
        match mainTableName with
        | x when x.StartsWith("T_") ->x.Remove(0,2)
        | x -> x
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e
      
  static member private GenerateQueryCodeForMainChildTables mainTableName  mainTableColumns (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list)  childTableName childTableColumns (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    let sbTem=StringBuilder()
    let sbTemSub=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Get{1}s (queryEntity:BQ_{1})=
      try
        use sb=new SBIIMSEntitiesAdvance()
        sb.{2}
        |>PSeq.filter (fun a->
            {3})
        |>PSeq.map (fun a->
            let entity=
              BD_{2}
                ({4})
            {5}
            entity.BD_{6}s<-
              a.{6}
              |>PSeq.map (fun b->
                  let childEntity=
                    BD_{6}
                      ({7})
                  {8}
                  childEntity,result)
              |>PSeq.toNetList
            entity)
        |>PSeq.toNetList  
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),""General""); BD_QueryResult<BD_{2}[]>()",
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
        (*
        //{3}
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
              match a.DATA_TYPE with
              | z when z.ToLowerInvariant().EndsWith("string")  || z.EndsWith("[]")->
                     sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                     //{0}
                       x
                       )|>ignore
              | _ ->
                  sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            && ",
                  //{0}
                    x
                    )|>ignore
          | x,y ->
              match y |>PSeq.find (fun b->b.FK_COLUMN_NAME=x ) ,a.DATA_TYPE with
              | z,u when u.ToLowerInvariant().EndsWith("string")  || u.EndsWith("[]") ->
                     sbTem.AppendFormat( @"
            match queryEntity.{0} with
            | x when x<>null ->a.{1}Reference.Load();a.{1}.{2}.Equals(x)
            | _ ->true
            && ",
                       //{0}
                       x
                       ,
                       //{1}
                       z.PK_TABLE_ALIAS
                       ,
                       //{2}
                       z.PK_COLUMN_NAME
                       )|>ignore
              | z,_ ->
                     sbTem.AppendFormat( @"
            match queryEntity.{0} with
            | x when x.HasValue ->a.{1}Reference.Load();a.{1}.{2} =x.Value
            | _ ->true
            && ",
                       //{0}
                       x
                       ,
                       //{1}
                       z.PK_TABLE_ALIAS
                       ,
                       //{2}
                       z.PK_COLUMN_NAME
                       )|>ignore
        (*        
        match sbTem,sbTem.ToString() with //不能解决问题
        | x,y when x.Length>0 && y.IndexOf('\n')> -1  && y.Substring(0,y.IndexOf('\n')).Trim() |>String.IsNullOrEmpty ->x.Remove(0,y.IndexOf('\n')+1) |>ignore //'\n' ='\010'
        | _ ->()
        *)
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-3,2)|>ignore //Remove the last of "&& "
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        *)
        //{3}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in 
          mainTableColumns
          |>PSeq.filter (fun b->mainTableAsFKRelationships|>PSeq.exists (fun c->c.FK_COLUMN_NAME=b.COLUMN_NAME) |>not) do
          match a.COLUMN_NAME, a.DATA_TYPE with
          | x,y when y.ToLowerInvariant().EndsWith("string")  || y.EndsWith("[]")->
              sbTem.AppendFormat( @"
            match a.{0},queryEntity.{0} with
            | x,y when y<>null ->x.Equals(y)
            | _ ->true
            && ",
                //{0}
                x
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

        (* case
              //该表作为当前表的外键列全为空时
              if queryEntity.C_SHR.HasValue || queryEntity.C_SHR1.HasValue then
                a.T_YG2Reference.Load() 
                match a.T_YG2 with
                | x when x<>null -> 
                  match queryEntity.C_SHR with
                  | y when y.HasValue->
                       x.C_ID =y.Value
                  | _ ->true
                  &&
                  match queryEntity.C_SHR1 with
                  | y when y.HasValue->
                       x.C_ID =y.Value
                  | _ ->true
                | _ ->true
              else
                true 
            &&  
              //该表作为当前表的外键列全不为空时
              if queryEntity.C_SHR.HasValue || queryEntity.C_SHR1.HasValue then
                a.T_YG2Reference.Load() 
                match a.T_YG2,queryEntity.C_SHR with
                | x,y when y.HasValue->
                       x.C_ID =y.Value
                | _ ->true
                &&
                match a.T_YG2,queryEntity.C_SHR1 with
                | x,y when y.HasValue->
                       x.C_ID =y.Value
                | _ ->true
              else
                true 
            &&  
        *)
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              match y|>PSeq.toList|>List.length with
              | z when z>1 ->
                  sbTem.AppendFormat(@"  
              if {0} then
                a.{1}Reference.Load() 
                {2}
              else
                true 
            && ",
                    //{0}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                          sbTemSub.AppendFormat(@"queryEntity.{0}<>null || "
                            ,
                            //{0}
                            v.COLUMN_NAME //在高级查询类中，如果使用了子表查询实体，那么这里判断条件中的字段应该使用这个子表的的字段名称而不是外键列名
                            )|>ignore
                      | _,v  ->
                          sbTemSub.AppendFormat(@"queryEntity.{0}.HasValue || " 
                            ,
                            //{0}
                            v.COLUMN_NAME
                             )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of "|| "
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    ,
                    //{1}
                    match  y|>PSeq.head with  u,_ ->u.PK_TABLE_ALIAS
                ,
                    //{2}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y do 
                      match b with
                      | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                          sbTemSub.AppendFormat(@"
                match a.{0},queryEntity.{1} with
                | x,y when y<>null->
                    x.{2} =y
                | _ ->true
                && ",
                            //{0}
                            u.PK_TABLE_ALIAS
                            ,
                            //{1}
                            u.FK_COLUMN_NAME
                            ,
                            //{2}
                            u.PK_COLUMN_NAME
                            )|>ignore
                      | u,_ ->
                          sbTemSub.AppendFormat(@"
                match a.{0},queryEntity.{1} with
                | x,y when y.HasValue->
                    x.{2} =y.Value
                | _ ->true
                && ",
                            //{0}
                            u.PK_TABLE_ALIAS
                            ,
                            //{1}
                            u.FK_COLUMN_NAME
                            ,
                            //{2}
                            u.PK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of "&& "
                    | _ ->()
                    sbTemSub.ToString().Trim()
                    )
                  )|>ignore      
              | _ -> //外键只有一个字段构成时，简化程序
                  (* 使用 if ... then , Right!!!
                  match y|>PSeq.head with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTem.AppendFormat(@"  
              if queryEntity.{0}<>null  then
                a.{1}Reference.Load() 
                a.{1}.{2}=queryEntity.{0}
              else
                true 
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                      )|>ignore   
                  | u,_ ->
                      sbTem.AppendFormat(@"  
              if queryEntity.{0}.HasValue  then
                a.{1}Reference.Load() 
                a.{1}.{2}=queryEntity.{0}.Value
              else
                true 
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                      )|>ignore  
                  *)    
                  match y|>PSeq.head with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTem.AppendFormat(@"  
            match queryEntity.{0} with
            | x when x<>null ->
                a.{1}Reference.Load() 
                a.{1}.{2}=x
            | _ ->true
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                      )|>ignore   
                  | u,_ ->
                      sbTem.AppendFormat(@"  
            match queryEntity.{0} with
            | x when x.HasValue ->
                a.{1}Reference.Load() 
                a.{1}.{2}=x.Value
            | _ ->true
            && ",
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
              match y|>PSeq.toList|>List.length with
              | z when z>1 ->
                  sbTem.AppendFormat(@"  
              if {0} then
                a.{1}Reference.Load() 
                match a.{1} with
                | x when x<>null -> 
                    {2}
                | _ ->true
              else
                true 
            && ",
                    //{0}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y  do
                      match b with
                      | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                          sbTemSub.AppendFormat(@"queryEntity.{0}<>null || "
                            ,
                            //{0}
                            v.COLUMN_NAME //在高级查询类中，如果使用了子表查询实体，那么这里判断条件中的字段应该使用这个子表的的字段名称而不是外键列名
                            )|>ignore
                      | _,v  ->
                          sbTemSub.AppendFormat(@"queryEntity.{0}.HasValue || " 
                            ,
                            //{0}
                            v.COLUMN_NAME
                             )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of "|| "
                    | _ ->()
                    sbTemSub.ToString().TrimStart()
                    )
                    ,
                    //{1}
                    match  y|>PSeq.head with  u,_ ->u.PK_TABLE_ALIAS
                    ,
                    //{2}
                    (
                    sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                    for b in y do
                      match b with
                      | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                          sbTemSub.AppendFormat(@"
                    match queryEntity.{0} with
                    | y when y<>null->
                        x.{1} =y
                    | _ ->true
                    && ",
                            //{0}
                            u.FK_COLUMN_NAME
                            ,
                            //{1}
                            u.PK_COLUMN_NAME
                            )|>ignore
                      | u,_ ->
                          sbTemSub.AppendFormat(@"
                    match queryEntity.{0} with
                    | y when y.HasValue->
                        x.{1} =y.Value
                    | _ ->true
                    && ",
                            //{0}
                            u.FK_COLUMN_NAME
                            ,
                            //{1}
                            u.PK_COLUMN_NAME
                            )|>ignore
                    match sbTemSub with
                    | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of "&& "
                    | _ ->()
                    sbTemSub.ToString().Trim()
                    )
                  )|>ignore
              | _ -> //外键只有一个字段构成时，简化程序
                  (* 使用 if ... then, Right!!!
                  match y|>PSeq.head with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTem.AppendFormat(@"  
              if queryEntity.{0}<>null  then
                a.{1}Reference.Load() 
                match a.{1}, queryEntity.{0} with
                | x,y when x<>null -> 
                        x.{2} =y
                | _ ->true
              else
                true 
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,_ ->
                      sbTem.AppendFormat(@"  
              if queryEntity.{0}.HasValue  then
                a.{1}Reference.Load() 
                match a.{1}, queryEntity.{0} with
                | x,y when x<>null -> 
                        x.{2} =y.Value
                | _ ->true
              else
                true 
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  *) 
                  match y|>PSeq.head with
                  | u,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                      sbTem.AppendFormat(@"  
            match queryEntity.{0} with
            | x when x<>null ->
                a.{1}Reference.Load() 
                match a.{1} with
                | y when y<>null -> 
                        y.{2} =x
                | _ ->true
            | _ ->true
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
                        ,
                        //{2}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,_ ->
                      sbTem.AppendFormat(@"  
            match queryEntity.{0} with
            | x when x.HasValue->
                a.{1}Reference.Load() 
                match a.{1} with
                | y when y<>null -> 
                        y.{2} =x.Value
                | _ ->true
            | _ ->true
            && ",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_TABLE_ALIAS
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
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
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
        (*
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
                     a.T_CK1Reference.Load()
                     entity.C_CCK<-a.T_CK1.C_ID
        *)
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships with
          | x,y when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x) ->
              match y|>PSeq.find (fun b->b.FK_COLUMN_NAME =x),a.IS_NULLABLE_TYPED,a.DATA_TYPE with
              | z,u,v when not u || v.ToLowerInvariant().EndsWith("string")  || v.EndsWith("[]")->
                  sbTem.AppendFormat( @"
            a.{0}Reference.Load()
            entity.{1}<-a.{0}.{2}",
                    //{0}
                    z.PK_TABLE_ALIAS
                     ,
                     //{1}
                    x //或者 b.FK_COLUMN_NAME
                    ,
                    //{2}
                    z.PK_COLUMN_NAME
                    )|>ignore
              | z,_,v->
                  sbTem.AppendFormat( @"
            a.{0}Reference.Load()
            entity.{1}<-Nullable<{2}>(a.{0}.{3})",
                    //{0}
                    z.PK_TABLE_ALIAS
                     ,
                     //{1}
                    x //或者 b.FK_COLUMN_NAME
                    ,
                    //{2}
                    v
                    ,
                    //{3}
                    z.PK_COLUMN_NAME
                    )|>ignore
          | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        *)
        //{5}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        (*
                     a.T_CK1Reference.Load()
                     entity.C_CCK<-a.T_CK1.C_ID
        *)
        for _,a in 
          (mainTableAsFKRelationships,mainTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              (* 使用 if
            if not a.{0}Reference.IsLoaded  then 
              a.{0}Reference.Load()
              *)
              sbTem.AppendFormat( @"
            match a.{0}Reference with 
            | x when not x.IsLoaded -> x.Load() | _ ->()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                )|>ignore
              for b in y do
                match b with
                | u,v  ->
                    sbTem.AppendFormat(@"
            entity.{0}<-a.{1}.{2}",
                      //{0}, CZY
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
            match a.{0}Reference with 
            | x when not x.IsLoaded -> x.Load() | _ ->()
            match a.{0} with
            | x when x<>null ->{1} | _ ->()",
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
        childTableName
        ,
        //{7}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in childTableColumns do
          match a.COLUMN_NAME,childTableAsFKRelationships,mainTableAsPKRelationships with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ||  z|>PSeq.exists (fun b ->b.FK_TABLE=childTableName && b.FK_COLUMN_NAME=x) -> //除了与主表关联的键外， 只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们最好是在后续的外键实体加载中进行赋值，这样便于结果集的扩展
              sbTem.AppendFormat( @"
                      {0}=b.{0},",
                a.COLUMN_NAME
              )|>ignore
          | _ ->()
        match sbTem with
        | x when x.Length>0 ->x.Remove(x.Length-1,1)|>ignore
        | _ ->()
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        ,
        //{8}
        (*
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for b in childTableAsFKRelationships do
          (*
                     b.T_DWBM1Reference.Load()
                     childEntity.C_FBID<-b.T_DWBM1.C_ID
          *)          
          sbTem.AppendFormat( @"
                  b.{0}Reference.Load()
                  childEntity.{1}<- b.{0}.{2}",
            //{0}
            b.PK_TABLE_ALIAS
            ,
            //{1}
            b.FK_COLUMN_NAME
            ,
            //{2}
            b.PK_COLUMN_NAME
          )|>ignore
        sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
        )
        *)
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for _,a in 
          (childTableAsFKRelationships  |>PSeq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不要加载
            childTableColumns)
          |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
          |>PSeq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
          match a with 
          | y  when y|>PSeq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
              sbTem.AppendFormat( @"
                  b.{0}Reference.Load()",
                //{0}
                match  y|>PSeq.head with u,_ ->u.PK_TABLE_ALIAS
                )|>ignore
              for b in y do
                match b with
                | u,v  ->
                    sbTem.AppendFormat(@"
                  childEntity.{0}<-b.{1}.{2}",
                      //{0}, CZY
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
                  b.{0}Reference.Load()
                  match b.{0} with
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
                      childEntity.{0} <- x.{1}",
                        //{0}
                        u.FK_COLUMN_NAME
                        ,
                        //{1}
                        u.PK_COLUMN_NAME
                        )|>ignore
                  | u,v->
                      sbTemSub.AppendFormat(@"
                      childEntity.{0} <- Nullable<{1}>(x.{2})",
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
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    
    
///////////////////////////////////////////

  static member private GenerateCreateCodeForMainChildTables (mainTableName:string)  (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(  @"
    member x.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        match (""{2}"",{2}({0}))|>sb.CreateEntityKey with
        | x when x<>null -> failwith ""The record is exist！"" | _ ->()
        let {3}=
          {2}
            ({4})
        {5}    
        for childEntity in businessEntity.BD_{6}s do
          let {7}=
            {6}
              ({8})
          {9}
          {3}.{6}.Add({7})
        sb.{2}.AddObject({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1",
      
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
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
              | z when z.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
            {0}=DateTime.Now,",
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
        {0}.{1} <-
          (""{2}"",{2}({3}))
          |>sb.CreateEntityKey
          |>sb.GetObjectByKey
          |>unbox<{2}>",
                    //{0},t_DJ_JHGL
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
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
                    string DataAccessCoding.VariableNames.[a]
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
                        string DataAccessCoding.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCoding.VariableNames.[a]+".HasValue"
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
            {0}.{1} <-
              (""{2}"",{2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>
        | _ ->()",
                    //{0}
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
                            string DataAccessCoding.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCoding.VariableNames.[a]
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
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) ->
                  sbTem.AppendFormat( @"
              {0}=DateTime.Now,",
                    x
                    )|>ignore
              | _  ->
                  sbTem.AppendFormat( @"
              {0}=childEntity.{0},",
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
          {0}.{1} <-
            (""{2}"",{2}({3}))
            |>sb.CreateEntityKey
            |>sb.GetObjectByKey
            |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
                          sbTemSub.AppendFormat(@"{0}=childEntity.{1},",
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
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
          match {0) with
          | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                for b in y  do
                  match b with
                  | u,_  ->
                      sbTemSub.AppendFormat(@"businessEntity.{0},"
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
                    string DataAccessCoding.VariableNames.[a]
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
                        string DataAccessCoding.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCoding.VariableNames.[a]+".HasValue"
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
              {0}.{1} <-
                (""{2}"",{2}({3}))
                |>sb.CreateEntityKey
                |>sb.GetObjectByKey
                |>unbox<{2}>
          | _ ->()",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
                            string DataAccessCoding.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCoding.VariableNames.[a]
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
    
  //Update for Main Child Tables
  //子表更新最好还是加入跟踪信息，这样在单据子项很多的情况下，可根据子项不同记录的跟踪信息，进行插入，更新或是删除操作
  static member private GenerateUpdateCodeForMainChildTables (mainTableName:string)   (mainTableColumns:DbColumnSchemalR seq) (mainTableAsFKRelationships:DbFKPK list) (mainTableAsPKRelationships:DbFKPK list) (mainTableKeyColumns:DbPKColumn seq)  (childTableName:string)  (childTableColumns:DbColumnSchemalR seq) (childTableAsFKRelationships:DbFKPK list) (childTableAsPKRelationships:DbFKPK list)=
    try
      let sbTem=StringBuilder()
      let sbTemSub=StringBuilder()
      let sb=StringBuilder()
      sb.AppendFormat(  @"{3}
    member x.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try 
        let businessEntity=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        let originalEntityKey=(""{2}"",{2}({0}))|>sb.CreateEntityKey
        match originalEntityKey with
        | x when x=null ->  failwith ""The record is not exist！！"" | _ ->()
        let original=
          originalEntityKey
          |>sb.GetObjectByKey
          |>unbox<{2}>
        {4}
        {5}    
        if businessEntity.BD_{6}s.Count>0 then
          original.{6}.Load()
          original.{6} |>Seq.iter (fun a->sb.DeleteObject(a))
          businessEntity.BD_{6}s|>Seq.iter (fun a->
            let {7}=
              {6}
                ({8})
            {9}
            original.{6}.Add({7}))
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,1);-1",
      
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
        (*
        match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
        *)
        String.Empty
        ,
        //{4}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in mainTableColumns do
          match a.COLUMN_NAME,mainTableAsFKRelationships,mainTableKeyColumns with
          | x,y,z when y|>PSeq.exists(fun b->b.FK_COLUMN_NAME =x)|>not &&  z|>PSeq.exists (fun b->b.COLUMN_NAME=x)|>not->
              match a.DATA_TYPE with
              | z when z.ToLowerInvariant().EndsWith("datetime") &&  x.Equals("C_GXRQ") ->
                  sbTem.AppendFormat( @"
        original.{0}<-DateTime.Now",
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
          (""{2}"",{2}({3}))
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
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
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
                    string DataAccessCoding.VariableNames.[a]
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
                        string DataAccessCoding.VariableNames.[a]+"<>null"
                        )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && " 
                        ,
                        //{0}
                        string DataAccessCoding.VariableNames.[a]+".HasValue"
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
              (""{2}"",{2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>
        | _ ->()",
                    //{0}
                    (*
                    match mainTableName,mainTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
                    *)
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
                            string DataAccessCoding.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCoding.VariableNames.[a]
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
        //{7}, t_DJSP_JHGL
        match childTableName,childTableName.Split('_') with  //update it to t_DJ...
        | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
              | u when u.ToLowerInvariant().EndsWith("datetime") && (x.Equals("C_CJRQ") || x.Equals("C_GXRQ")) -> //???,如果子表确实需要使用 创建时间和更新时间字段，那么子表记录就不能简单的先删除再新增了
                  sbTem.AppendFormat( @"
                {0}=DateTime.Now,",
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
            {0}.{1} <-
              (""{2}"",{2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>",
                //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
          | y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
              sbTem.AppendFormat(@"
            match {0) with
            | {1} when {2} ->",
                //{0}
                (
                sbTemSub.Remove(0,sbTemSub.Length) |>ignore
                y|>Seq.iter (fun (a,_)->
                  sbTemSub.AppendFormat(@"businessEntity.{0},"
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
                    string  DataAccessCoding.VariableNames.[a]
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
                        string DataAccessCoding.VariableNames.[a]+"<>null"
                      )|>ignore
                  | _  ->
                      sbTemSub.AppendFormat(@"{0} && "
                        ,
                        //{0}
                        string DataAccessCoding.VariableNames.[a]+".HasValue"
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
                {0}.{1} <-
                  (""{2}"",{2}({3}))
                  |>sb.CreateEntityKey
                  |>sb.GetObjectByKey
                  |>unbox<{2}>
            | _ ->()",
                    //{0}
                    match childTableName,childTableName.Split('_') with
                    | v,w-> w.[0].ToLowerInvariant()+v.Remove(0,w.[0].Length)
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
                            string DataAccessCoding.VariableNames.[a]
                            )|>ignore
                      | w,_->
                          sbTemSub.AppendFormat(@"{0}={1}.Value,",
                            //{0}
                            w.PK_COLUMN_NAME
                            ,
                            //{1}
                            string DataAccessCoding.VariableNames.[a]
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
    
  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member  GenerateDeleteCodeForMainChildTables (mainTableName:string)    (mainTablePKColumns:DbPKColumn seq)   (childTableName:string) =
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
      try
        use sb=new SBIIMSEntitiesAdvance()
        let businessEntity=executeContent.ExecuteData
        match (""{2}"",{2}({3}))|>sb.CreateEntityKey with
        | x when x=null ->  failwith ""The record is not exist！"" 
        | x ->
            x
            |>sb.GetObjectByKey
            |>unbox<{2}>
            |>fun x -> x.{4}.Load();x
            |>sb.DeleteObject
        sb.SaveChanges()
      with
      | e -> ObjectDumper.Write(e,1); -1",
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
          (mainTablePKColumns,mainTableColumns)
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
        for a in  mainTablePKColumns do
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
        )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
    
  //删除实体不应该使用查询实体作为条件，因为只有商业实体才能保证实体键都不为空
  static member  GenerateMultiDeleteCodeForMainChildTables (mainTableName:string)    (mainTablePKColumns:DbPKColumn seq)   (childTableName:string) =
    let sb=StringBuilder()
    let sbTem=StringBuilder()
    try
      sb.AppendFormat(  @"{0}
    member x.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
      try
        let businessEntities=executeContent.ExecuteData
        use sb=new SBIIMSEntitiesAdvance()
        for businessEntity in businessEntities do
          match (""{2}"",{2}({3}))|>sb.CreateEntityKey with
          | x when x=null ->  failwith ""The record is not exist！"" 
          | x ->
              x
              |>sb.GetObjectByKey
              |>unbox<{2}>
              |>fun x -> x.{4}.Load();x
              |>sb.DeleteObject
        sb.SaveChanges()
      with
      | e -> ObjectDumper.Write(e,1); -1",
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
        for a in  mainTablePKColumns do
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
      )|>ignore
      string sb
    with 
    | e -> ObjectDumper.Write(e,2); raise e
 
(*

            &&
            match a.C_RKSL,queryEntity.C_RKSL with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match queryEntity.C_SHR,queryEntity.C_SHR1 with
            | x,y when x.HasValue || y.HasValue->
                a.T_YG2Reference.Load() 
                match a.T_YG2 with
                | xs when xs<>null -> 
                  match x with
                  | xss when xss.HasValue->
                       xs.C_ID =x.Value
                  | _ ->true
                  &&
                  match y with
                  | xss when xss.HasValue->
                       xs.C_ID1 =x.Value
                  | _ ->true
                | _ ->true
            | _ ->true
            &&
            match a.C_SZQJE,queryEntity.C_SZQJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match a.C_THBZ,queryEntity.C_THBZ with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&
            match queryEntity.C_WFDW,queryEntity.C_WFDW1 with
            | x,y when x.HasValue ||  y.hasValue ->
                a.T_DWBM1Reference.Load()
                match x with
                | xs when xs.HasValue ->
                    a.T_DWBM1.C_ID =xs.Value
                &&
                match y with
                | xs when xs.HasValue ->
                    a.T_DWBM1.C_ID1 =xs.Value
            | _ ->true
            &&
            match a.C_YHJE,queryEntity.C_YHJE with
            | x,y when y.HasValue ->x=y.Value
            | _ ->true
            &&

*)