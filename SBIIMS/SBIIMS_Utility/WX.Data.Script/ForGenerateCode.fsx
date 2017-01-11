//#I  @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.0"
#r "System.dll"
#r "System.Core.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Threading.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Data.dll"
#r "Microsoft.Practices.ObjectBuilder2.dll"
#r "System.Runtime.Serialization.dll"
#r "System.Windows.Forms.dll"
#r "FSharp.PowerPack.Linq.dll"

open System
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open Microsoft.FSharp.Linq
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.Practices.ObjectBuilder2
open System.Windows.Forms
//let projectPath= @"D:\Workspace\SBIIMS"

//It must load on sequence
#I  @"D:\Workspace\SBIIMS\WX.Data.Helper\bin\Debug"
#r "WX.Data.Helper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FHelper\bin\Debug"
#r "WX.Data.FHelper.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.FModule\bin\Debug"
#r "WX.Data.dll"
#I @"D:\Workspace\SBIIMS\WX.Data.CodeAutomation\bin\Debug"
#r "WX.Data.CodeAutomation.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.PSeq
open WX.Data
open WX.Data.CodeAutomation

type System.Text.StringBuilder  with
  member x.AppendN (str:string)=
    x.Append(str) |>ignore
  member x.AppendFormatN (str:string)=
    x.AppendFormat(str) |>ignore

ConfigHelper.INS.LoadDefaultServiceConfigToManager

//////////////////////////////////////////////////////////////////////////////////////////////////
let mainTableName="T_DJ_GHS"
let mainTableColumns=
  DatabaseInformation.GetColumnSchemal4Way mainTableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
let mainTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship mainTableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
ObjectDumper.Write mainTableAsFKRelationships
let mainTableKeyColumns=DatabaseInformation.GetPKColumns mainTableName
ObjectDumper.Write mainTableKeyColumns
let group=
  (mainTableAsFKRelationships,mainTableColumns)
  |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
  |>Seq.groupBy (fun (a,_) ->a.FOREIGN_KEY)
  
ObjectDumper.Write(group,2)


let tableName="Test_Main"
let rela =DatabaseInformation.GetAsFKRelationship tableName |>Seq.groupBy (fun a->a.FOREIGN_KEY)

let tableName="T_DJ_GHS"
DataAccessCoding.GetCode tableName |> Clipboard.SetText




ObjectDumper.Write(rela,1)

//////////////////////////////////////////////////////////////////////////////////////////////////////

let mainTableName="T_DJ_GHS"

let mainTableColumns=
  DatabaseInformation.GetColumnSchemal4Way mainTableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
let mainTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship mainTableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
ObjectDumper.Write(mainTableAsFKRelationships)
let mainTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship mainTableName //获取指定表作为其它表外键关系的主键表时的关系，即该表关联到其它表的关系
ObjectDumper.Write(mainTableAsPKRelationships)
let detailTableRelationships=    //获取信息子表，一般认为信息子表最多只有一个
  mainTableAsPKRelationships
  |>pseq
  |>filter (fun a->
      let pkColumns=
        a.FK_TABLE 
        |>DatabaseInformation.GetPKColumns |>pseq |>toList
      pkColumns.Length >1  //说明是一对多的关系
      &&
      pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME))   //
  |>toList

let detailTableName="T_DJSP_GHS" //x.Head.FK_TABLE
let detailTableColumns=
  DatabaseInformation.GetColumnSchemal4Way detailTableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
let detailTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship detailTableName
ObjectDumper.Write(detailTableAsFKRelationships)
let detailTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship detailTableName

let mainTableKeyColumns= DatabaseInformation.GetPKColumns mainTableName
ObjectDumper.Write(mainTableKeyColumns)
let detailTableKeyColumns= DatabaseInformation.GetPKColumns detailTableName
ObjectDumper.Write(detailTableKeyColumns)


match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
| v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)

sb.ToString() |>Clipboard.SetText

let sbTem=StringBuilder()
let sbTemSub=StringBuilder()
let sb=StringBuilder()
let intTem=ref 0


let code=
  (*
  raise <| Exception("The record is exist")  
  failwith "The record is exist！"
  rethrow 
  *)
  sb.AppendFormat(  @"
    member x.Create{1} (businessEntity:B_{2})=
      try 
        use sb=new SBIIMSEntitiesAdvance()
        match (""{2}"",{2}({0}))|>sb.CreateEntityKey with
        | x when x<>null ->   
            failwith ""The record is exist！""
        | _ ->()
        let {3}=
          {2}
            ({4})
        {5}    
        for detailEntity in businessEntity.B_{6}s do
          let {7}=
            {6}
              ({8})
          {9}
          {3}.{6}.Add({7})
        sb.AddTo{2}({3})
        sb.SaveChanges()
      with
      | e ->ObjectDumper.Write(e,0);-1
      ",
      
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
    //{1},DJ_GHS
    match mainTableName with
    | x when x.StartsWith("T_") ->x.Remove(0,2)
    | x -> x
    ,
    //{2},T_DJ_GHS
    mainTableName
    ,
    //{3} ,t_DJ_GHS
    match mainTableName,mainTableName.Split('_') with  //update it to t_DJ...
    | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
    ,
    //{4}
    (
    sbTem.Remove(0,sbTem.Length) |>ignore
    for a in mainTableColumns do
      match a.COLUMN_NAME,mainTableAsFKRelationships with
      | x,y when y|>Seq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
          match a.DATA_TYPE with
          (*
          match a.DATA_TYPE, mainTableKeyColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.COLUMN_NAME ) with
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
    for a in 
      (mainTableAsFKRelationships,mainTableColumns)
      |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
      |>Seq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
      match a with
      | _,y  when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
          match y|>Seq.head with
          | u,_ ->
              sbTem.AppendFormat( @"
        {0}.{1} <-
          (""{2}"",{2}({3}))
          |>sb.CreateEntityKey
          |>sb.GetObjectByKey
          |>unbox<{2}>",
                //{0},t_DJ_GHS
                match mainTableName,mainTableName.Split('_') with
                | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
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
      | _,y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
          sbTem.AppendFormat(@"
        match {0} with
        | {1} when {2} ->",
            //{0}
            (
            sbTemSub.Remove(0,sbTemSub.Length) |>ignore
            for b in y  do
              match b with
              | u,_  ->
                  sbTemSub.AppendFormat(@"
                        businessEntity.{0},"
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
            intTem:=120 //char 120=x
            for b in y  do
              match b with
              | u,v  ->
                  sbTemSub.AppendFormat(@"
                        {0},"
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
            ,
            //{2}
            (
            sbTemSub.Remove(0,sbTemSub.Length) |>ignore
            intTem:=120 //char 120=x
            for b in y  do
              match b with
              | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                  sbTemSub.AppendFormat(@"
                        {0},"
                    ,
                    //{0}
                    string (char !intTem)^"<>null && "
                    )|>ignore
              | _  ->
                  sbTemSub.AppendFormat(@"
                        {0},"
                    ,
                    //{0}
                    string (char !intTem)^".HasValue && "
                     )|>ignore
              incr intTem
              match !intTem with  // x,y,z,u,v,w,a,b,c....
              | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
              | u when u=120 ->intTem:=97 // int 'a'=97
              | _ -> ()
            match sbTemSub with
            | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
            | _ ->()
            sbTemSub.ToString().TrimStart()
           )
           )|>ignore
           
          match y|>Seq.head with
          | u,_ ->
              sbTem.AppendFormat( @"
            {0}.{1} <-
              (""{2}"",{2}({3}))
              |>sb.CreateEntityKey
              |>sb.GetObjectByKey
              |>unbox<{2}>
        | _ ->()",
                //{0}
                match mainTableName,mainTableName.Split('_') with
                | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
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
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"{0}={1},",
                        //{0}
                        w.PK_COLUMN_NAME
                        ,
                        //{1}
                        string (char !intTem)
                        )|>ignore
                  | w,_->
                      sbTemSub.AppendFormat(@"{0}={1}.Value,",
                        //{0}
                        w.PK_COLUMN_NAME
                        ,
                        //{1}
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
              )|>ignore
    sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
    )
    ,
    //{6}, T_DJSP_GHS
    detailTableName
    ,
    //{7}, t_DJSP_GHS
    match detailTableName,detailTableName.Split('_') with  //update it to t_DJ...
    | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
    ,
    //{8}
    (
    sbTem.Remove(0,sbTem.Length) |>ignore
    for a in detailTableColumns do
      (*
      match a.COLUMN_NAME,detailTableAsFKRelationships,detailTableKeyColumns with
      | x,y,z when y|>Seq.exists(fun b->b.FK_COLUMN_NAME =x)|>not || z|>Seq.exists (fun b->b.COLUMN_NAME=x ) ->  
      *)
      match a.COLUMN_NAME,detailTableAsFKRelationships with
      | x,y when y|>Seq.exists(fun b->b.FK_COLUMN_NAME =x)|>not -> //只要是该表的外键都不需要赋值，即时这些字段列同时作为该表的主键列而存在于该表的实体中时，也不需要赋值操作，它们将在后续的外键实体加载中自动赋值
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
              {0}=detailEntity.{0},",
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
    for a in 
      (detailTableAsFKRelationships |>Seq.filter (fun b ->b.PK_TABLE<>mainTableName), // 子表的主键列同时是父表的主键时,不需要加载
        detailTableColumns)
      |>fun (a,b) ->Query.join a b (fun a->a.FK_COLUMN_NAME) (fun b->b.COLUMN_NAME) (fun a b ->a,b)
      |>Seq.groupBy (fun (a,_) ->a.FOREIGN_KEY) do
      match a  with 
      | _,y  when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) |>not -> //说明商业实体中对应字段都必须有值 
          match y|>Seq.head with
          | u,_ ->
              sbTem.AppendFormat( @"
         {0}.{1} <-
           (""{2}"",{2}({3}))
           |>sb.CreateEntityKey
           |>sb.GetObjectByKey
           |>unbox<{2}>",
                //{0}
                match detailTableName,detailTableName.Split('_') with
                | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
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
                      sbTemSub.AppendFormat(@"{0}=detailEntity.{1},",
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
      | _,y   -> //这里认为，一个外键所对应的所有外键列, 只有一个允许空，其它均认为允许为空，验证方法为 DatabaseInformation.ValidateForeignKeyColumnDesign
          sbTem.AppendFormat(@"
         match {0) with
         | {1} when {2} ->",
            //{0}
            (
            sbTemSub.Remove(0,sbTemSub.Length) |>ignore
            for b in y  do
              match b with
              | u,v  ->
                  sbTemSub.AppendFormat(@"
                        businessEntity.{0},"
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
            intTem:=120 //char 120=x
            for b in y  do
              match b with
              | u,v  ->
                  sbTemSub.AppendFormat(@"
                        {0},"
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
            ,
            //{2}
            (
            sbTemSub.Remove(0,sbTemSub.Length) |>ignore
            intTem:=120 //char 120=x
            for b in y  do
              match b with
              | _,v  when v.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || v.DATA_TYPE.EndsWith("[]") ->
                  sbTemSub.AppendFormat(@"
                        {0},"
                    ,
                    //{0}
                    string (char !intTem)^"<>null && "
                  )|>ignore
              | _  ->
                  sbTemSub.AppendFormat(@"
                        {0},"
                    ,
                    //{0}
                    string (char !intTem)^".HasValue && "
                  )|>ignore
              incr intTem
              match !intTem with  // x,y,z,u,v,w,a,b,c....
              | u when u>122 ->intTem:=117  // int 'u'=117 char 122='z' 
              | u when u=120 ->intTem:=97 // int 'a'=97
              | _ -> ()
            match sbTemSub with
            | w when w.Length>0 ->w.Remove(w.Length-3,3)|>ignore //Remove the last of '&& '
            | _ ->()
            sbTemSub.ToString().TrimStart()
            ) 
           )|>ignore
           
          match y|>Seq.head with
          | u,_ ->
              sbTem.AppendFormat( @"
             {0}.{1} <-
               (""{2}"",{2}({3}))
               |>sb.CreateEntityKey
               |>sb.GetObjectByKey
               |>unbox<{2}>
         | _ ->()",
                //{0}
                match detailTableName,detailTableName.Split('_') with
                | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
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
                intTem:=120 //char 120=x
                for b in y  do
                  match b with
                  | w,r when r.DATA_TYPE.ToLowerInvariant().EndsWith("string")  || r.DATA_TYPE.EndsWith("[]")->
                      sbTemSub.AppendFormat(@"{0}={1},",
                        //{0}
                        w.PK_COLUMN_NAME
                        ,
                        //{1}
                        string (char !intTem)
                        )|>ignore
                  | w,_->
                      sbTemSub.AppendFormat(@"{0}={1}.Value,",
                        //{0}
                        w.PK_COLUMN_NAME
                        ,
                        //{1}
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
                )|>ignore
    sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
    )
  )|>ignore
    
sbTem
    
//    for a in mainTableAsFKRelationships|>Seq.groupBy (fun b -> b.FOREIGN_KEY) do //每个外键可对应多个外键列
//      match a.FK_COLUMN_NAME,mainTableColumns|>Seq.find (fun b ->b.COLUMN_NAME=a.FK_COLUMN_NAME ) with 
//      | x,y when y.IS_NULLABLE_TYPED |>not ->
//          sbTem.AppendFormat( @"
//        {0t_DJ_GHS}.{1T_YG1} <-
//           (""{2}"",{2}({3C_ID=businessEntity.C_JBR))
//           |>sb.CreateEntityKey
//           |>sb.GetObjectByKey
//           |>unbox<{2}>",
//                //{0}
//            match mainTableName,mainTableName.Split('_') with
//            | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
//            ,
//            //{1}
//            z.PK_TABLE_ALIAS
//            ,
//            //2
//            z.PK_TABLE
//           )|>ignore
//    
//    
//    for a in mainTableColumns do
//      match a.COLUMN_NAME,mainTableAsFKRelationships with
//      | x,y when y|>Seq.exists(fun b->b.FK_COLUMN_NAME =x) ->
//          match y|>Seq.find (fun b->b.FK_COLUMN_NAME =x),a.IS_NULLABLE_TYPED with
//          | z,u when not u->
//              sbTem.AppendFormat( @"
//        {0t_DJ_GHS}.{1T_YG1} <-
//           (""{2}"",{2}({3C_ID=businessEntity.C_JBR))
//           |>sb.CreateEntityKey
//           |>sb.GetObjectByKey
//           |>unbox<{2}>",
//                //{0}
//                match mainTableName,mainTableName.Split('_') with
//                | v,w-> w.[0].ToLowerInvariant()^v.Remove(0,w.[0].Length)
//                ,
//                //{1}
//                z.PK_TABLE_ALIAS
//                ,
//                //2
//                z.PK_TABLE
                  
                    
    
    
   // )|>ignore


)


///////////////////////////////////////////////////////////////////////////////////////////////////

let tableName="Test_Main"
let asFKRelationships= DatabaseInformation.GetAsFKRelationship tableName //获取指定表的作为该表所有外键关系的外键表时的关系，即其它表关联到该表的关系
let columns=
  DatabaseInformation.GetColumnSchemal4Way tableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
  
let issues= 
  (*
  let sbTem=StringBuilder()
  *)
  (asFKRelationships|>pseq, columns|>pseq)
  |>fun(a,b) ->join a b (fun a->a.FK_COLUMN_NAME ) (fun b->b.COLUMN_NAME ) (fun a b ->a,b)
  |>as_seq
  |>Seq.groupBy (fun (a,b) ->a.FOREIGN_KEY )
  |>Seq.map (fun a->
      match a with
      | _,y when y|>Seq.exists (fun (_,b)->b.IS_NULLABLE_TYPED) && y|>Seq.exists (fun (_,b)->not b.IS_NULLABLE_TYPED)-> //如果设计正确, 必然有一种情况不存在
          (*
          sbTem.Remove(0,sbTem.Length)|>ignore
          y|>Seq.iter (fun (_,b)->sbTem.Append(b.COLUMN_NAME^",") |>ignore)
          *)
          let rec GetColumnNames (columns:(DbFKPK*DbColumnSchemalR) list)=
            match columns with
            | (_,v)::t-> v.COLUMN_NAME^","^GetColumnNames t
            | _ ->String.Empty
            (*
            match columns with
            | h::t-> 
                match h with 
                | u,v -> h .COLUMN_NAME^","^GetColumnNames t
            | _ ->()
            *)
          match y|>Seq.head,GetColumnNames (y|>Seq.toList) with
          | (u,v),w -> false, "该表"^tableName^"与主键表"^u.PK_TABLE ^"以外键"^u.FOREIGN_KEY^"关联的外键列"^GetColumnNames (y|>Seq.toList)^"要么全部允许为空，要么全部都不允许为空，请更正！"
      | _ ->true,String.Empty)
  |>Seq.filter (fun (a,_)->not a)
  |>Seq.toList


/////////////////////////////////////////////////////////////////////////////////////////////////////

let tableName="T_DJ_GHS"
DataAccessCoding.GetQueryCode tableName |>Clipboard.SetText

let sb=new SBIIMSEntitiesAdvance()
sb.T_DJ_GHS
|>Seq.filter (fun a->a.T_CK1Reference.Load(); a.T_CK1.C_ID =Guid("40a85414-4665-4bc5-97d6-f35a76147ae8") )
|>Seq.toList

"&&".Length

"{}"
String. Format(@"wx{0}", "wx")

//////////////////////////////////////////////////////////////////////////////////////////////////

let tableName="T_DJ_GHS"
let columns=
  DatabaseInformation.GetColumnSchemal4Way tableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
let asFKRelationships= DatabaseInformation.GetAsFKRelationship tableName
let asPKRelationships=DatabaseInformation.GetAsPKRelationship tableName
  
let detailTableName="T_DJSP_GHS"  
let detailTableColumns=
  DatabaseInformation.GetColumnSchemal4Way detailTableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)
let detailTableAsFKRelationships= DatabaseInformation.GetAsFKRelationship detailTableName
let detailTableAsPKRelationships=DatabaseInformation.GetAsPKRelationship detailTableName

ObjectDumper.Write(asFKRelationships ,1)
ObjectDumper.Write(asPKRelationships ,1)

(*
for a in asPKRelationships do
  if
    a.FK_TABLE 
    |>DatabaseInformation.GetPKColumns
    |>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME) //既是外键又是主键 
  then
    a.FK_TABLE

let entity=    
  asPKRelationships
  |>pseq
  |>filter (fun a->
                a.FK_TABLE
                |>DatabaseInformation.GetPKColumns
                |>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME))
  |>toList

*)

let detailTableRelations=    
  asPKRelationships
  |>pseq
  |>filter (fun a->
                let pkColumns=
                   a.FK_TABLE 
                   |>DatabaseInformation.GetPKColumns |>pseq |>toList
                pkColumns.Length >1  //说明是一对多的关系
                &&
                pkColumns|>Seq.exists (fun b->b.COLUMN_NAME=a.FK_COLUMN_NAME))   
  |>toList
entity.Length
entity.IsEmpty

  
  
//  
  
  
ObjectDumper.Write(entity ,1)
  
let pkColumns=DatabaseInformation.GetPKColumns "T_DJSP_GHS"
ObjectDumper.Write(pkColumns ,1)



ObjectDumper.Write(asPKRelationship,1)


let db=DatabaseFactory.CreateDatabase()
let conn=db.CreateConnection()
conn.Open()
let restrictionValues=[|null;"dbo";tableName;null|] 
let dataTable=conn.GetSchema("IndexColumns",restrictionValues)
ObjectDumper.Write(dataTable,1)
dataTable.Rows.Count
for a in dataTable.Columns do 
  ObjectDumper.Write((string a.DataType,a.ColumnName.ToUpper()))
  
  
conn.Close()

ObjectDumper.Write(asFKRelationship,1)
ObjectDumper.Write(asPKRelationship,1)

//Generate data read code
let sbTem=StringBuilder()
let sb=StringBuilder()

let code=
  sb.AppendFormat(  @"
      {0}member x.Get{1}s (queryEntity:BQ_{1})=
        use sb=new SBIIMSEntitiesAdvance()
        try
          sb.{2}
          |>pseq
          |>filter (fun a->
              {3}
                )
          |>map (fun a->
                 let entity=
                   B_{2}
                     ({4}))
                 {5}
                 entity.B_{6}s<-
                   a.{6}
                   |>pseq 
                   |>map (fun b->
                          let detailEntity=
                            B_{6}
                              ({7})
                          {8}
                          detailEntity)
                   |>toNetList
                 entity
                 )
          |>toNetList  
        with 
        | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),""); List<B_{2}>()
      ",
    //{0}
    String.Empty
    ,
    //{1}
    String.Empty
    ,
    //{2}
    String.Empty
    ,
    //{3}
    String.Empty
    ,
    //{4}
    String.Empty
    ,
    //{5}
    String.Empty
    ,
    //{6}
    String.Empty
    ,
    //{7}
    String.Empty
    ,
    //{8}
    String.Empty
    )|>ignore
    
    
(*
let codeTemplate= @"
    {0}member x.Get{1 DJ_GHS}s (queryEntity:BQ_{1DJ_GHS})=
      use sb=new SBIIMSEntitiesAdvance()
      try
        sb.{2T_DJ_GHS}
        |>pseq
        |>filter (fun a->
            {3
            match a.C_ID,queryEntity.C_ID with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true
            &&
            match a.C_GXRQ,queryEntity.C_GXRQ with
            | b,c when c.HasValue ->b=c.Value
            | _ ->true}
              )
        |>map (fun a->
               let entity=
                 B_{2 T_DJ_GHS}
                   ({4 C_BZ=a.C_BZ,
                   C_CJRQ=a.C_CJRQ,
                   C_CKJE=a.C_CKJE,
                   C_CKSL=a.C_CKSL,
                   C_DJH=a.C_DJH,
                   C_DJLX=a.C_DJLX,
                   C_DJZQJE=a.C_DJZQJE,
                   C_DJZT=a.C_DJZT,
                   C_DYBZ=a.C_DYBZ,
                   C_FKD=a.C_FKD,
                   C_GXRQ=a.C_GXRQ,
                   C_ID=a.C_ID,
                   C_RKJE=a.C_RKJE,
                   C_RKSL=a.C_RKSL,
                   C_SZQJE=a.C_SZQJE,
                   C_THBZ=a.C_THBZ,
                   C_YHJE=a.C_YHJE,
                   C_YSDJH=a.C_YSDJH,
                   C_YZQJE=a.C_YZQJE,
                   C_CCK=(a.T_CKReference.Load();a.T_CK.C_ID)})
               {5 a.T_CK1Reference.Load()
               entity.C_RCK<-a.T_CK1.C_ID
               entity.C_FBID<-a.T_DWBM.C_ID
               entity.C_WFDW<-a.T_DWBM1.C_ID
               entity.C_GHS<-a.T_GHS.C_ID
               entity.C_CZY<-a.T_YG.C_ID
               entity.C_JBR<-a.T_YG1.C_ID
               entity.C_SHR <-Nullable<Guid>( a.T_YG2.C_ID)}
               entity.B_{6 T_DJSP_GHS}s<-
                 a.{6 T_DJSP_GHS}
                 |>pseq 
                 |>map (fun b->
                        let detailEntity=
                          B_{6T_DJSP_GHS}
                            ({7 BZ=b.BZ,
                            C_BZQ=b.C_BZQ,
                            C_DJ=b.C_DJ,
                            C_DJID=b.C_DJID,
                            C_PC=b.C_PC,
                            C_SCRQ=b.C_SCRQ,
                            C_SL=b.C_SL,
                            C_SP=b.C_SP,
                            C_TM=b.C_TM,
                            C_XH=b.C_XH,
                            C_ZHJ=b.C_ZHJ,
                            C_ZHJE=b.C_ZHJE,
                            C_ZJE=b.C_ZJE,
                            C_ZKL=b.C_ZKL})
                        {8 b.T_SPReference.Load()
                        detailEntity.C_SP<-b.T_SP.C_ID
                        b.T_DWBMReference.Load()
                        detailEntity.C_FBID<-b.T_DWBM.C_ID}
                        detailEntity)
                 |>toNetList
               entity
               )
        |>toNetList  
      with 
      | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),""); List<B_{2T_DJ_GHS}>()
    "
*)
    
sb.Remove(0,sb.Length) |>ignore
let code=
  sb.AppendFormat(  @"
      {0}member x.Get{1}s (queryEntity:BQ_{1})=
        use sb=new SBIIMSEntitiesAdvance()
        try
          sb.{2}
          |>pseq
          |>filter (fun a->
              {3})
          |>map (fun a->
              let entity=
                B_{2}
                  ({4}))
              {5}
              entity.B_{6}s<-
                a.{6}
                |>pseq 
                |>map (fun b->
                    let detailEntity=
                      B_{6}
                        ({7})
                    {8}
                    detailEntity)
                |>toNetList
              entity)
          |>toNetList  
        with 
        | e ->ObjectDumper.Write(e); Logger.Write(e.ToString(),""General""); List<B_{2}>()
      ",
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
    for a in columns do
      match a.COLUMN_NAME,a.DATA_TYPE with
      | x,y when y.ToLowerInvariant().EndsWith("string") ->
          sbTem.AppendFormat( @"
              match a.{0},queryEntity.{0} with
              | b,c when c<>null ->b.Equals(c)
              | _ ->true
              &&",
            //{0}
            x
            )|>ignore
      | x,_ ->
          sbTem.AppendFormat( @"
              match a.{0},queryEntity.{0} with
              | b,c when c.HasValue ->b=c.Value
              | _ ->true
              &&",
            //{0}
            x
            )|>ignore
    (*        
    match sbTem,sbTem.ToString() with //不能解决问题
    | x,y when x.Length>0 && y.IndexOf('\n')> -1  && y.Substring(0,y.IndexOf('\n')).Trim() |>String.IsNullOrEmpty ->x.Remove(0,y.IndexOf('\n')+1) |>ignore //'\n' ='\010'
    | _ ->()
    *)
    sbTem.ToString().TrimStart() //TrimStart(), 移出子代码模板第一行格式化时的所有空格，包括换行符'\n'等都能移出，并由主模板代码文本中的占位符{0}的位置来决定第一行代码的真实起始位置
    )
    ,
    //{4}
    (
    sbTem.Remove(0,sbTem.Length) |>ignore
    for a in columns do
      match a.COLUMN_NAME,asFKRelationships with
      | x,y when y|>Seq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
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
                 a.T_CK1Reference.Load()
                 entity.C_CCK<-a.T_CK1.C_ID
    *)
    for b in asFKRelationships do
      sbTem.AppendFormat( @"
              a.{0}Reference.Load()
              entity.{1}<-a.{0}.{2}",
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
    ,
    //{6}
    (
    sbTem.Remove(0,sbTem.Length) |>ignore
    detailTableRelations.Head.FK_TABLE
    )
    ,
    //{7}
    (
    sbTem.Remove(0,sbTem.Length) |>ignore
    for a in detailTableColumns do
      match a.COLUMN_NAME,detailTableAsFKRelationships with
      | x,y when y|>Seq.exists(fun b->b.FK_COLUMN_NAME =x)|>not ->
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
    (
    sbTem.Remove(0,sbTem.Length) |>ignore
    for b in detailTableAsFKRelationships do
      (*
                 b.T_DWBM1Reference.Load()
                 detailEntity.C_FBID<-b.T_DWBM1.C_ID
      *)          
      sbTem.AppendFormat( @"                   
                    b.{0}Reference.Load()
                    detailEntity.{1}<-b.{0}.{2} ",
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
    )|>ignore
    
sb.ToString() |> Clipboard.SetText
  
 
let str= @"       

wx"
str.Substring(0,str.IndexOf('\n')+1).Trim()|>String.IsNullOrEmpty
str.Remove(0,str.IndexOf('\n')+1).TrimStart()
str.TrimStart()

""|>String.IsNullOrEmpty

 '\n'

/////////////////////////////////////////////////////////


let UpdateFKRelationships (relationships:DbFKPK list)=
  let count=ref 0
  //let  previous=ref Unchecked.defaultof<DbFKPK>
  let previousPkTableName=ref Unchecked.defaultof<String>
  for a in relationships do
    match !previousPkTableName,a with
    | x,y when String.IsNullOrEmpty(x) |>not && y.PK_TABLE=x->
        incr count
        y.PK_TABLE_ALIAS<-y.PK_TABLE^string !count
    | x,y ->
        count:=0
        y.PK_TABLE_ALIAS<-y.PK_TABLE
    previousPkTableName:=a.PK_TABLE 
  relationships

UpdateFKRelationships asFKRelationships

///////////////////////////////////////////////////////////



//(*
//Wrong, This field is not mutable of snd.PK_TABLE
let rec UpdateFKRelationships01 (relationships:DbFKPK list) (count:int ref)=
  match relationships with
  | head::snd::tail when head.PK_TABLE.StartsWith(snd.PK_TABLE) || head.PK_TABLE.Replace(snd.PK_TABLE,String.Empty).Length<=2 ->
       incr count;  //count:=!count+1, --, decr count
       snd.PK_TABLE<-snd.PK_TABLE^string !count;UpdateFKRelationships (snd::tail) count
  | head::tail -> count:=0;UpdateFKRelationships tail count
  | []->()
  
  
  
let rec UpdateFKRelationships (relationships:DbFKPK list) (count:int ref)=
  match relationships with
  | head::snd::tail when head.PK_TABLE.Equals(snd.PK_TABLE) || head.PK_TABLE.Replace(snd.PK_TABLE,String.Empty).ToCharArray() |>Seq.exists (fun a->not <| Char.IsDigit(a) ) |>not->
       if !count=0 then head.PK_TABLE_ALIAS<-head.PK_TABLE
       incr count  //count:=!count+1, --, decr count
       snd.PK_TABLE_ALIAS<-snd.PK_TABLE^string !count
       UpdateFKRelationships (snd::tail) count
  | head::tail -> 
      if !count>0 then head.PK_TABLE_ALIAS<-head.PK_TABLE^string !count; count:=0
      else head.PK_TABLE_ALIAS<-head.PK_TABLE  
      UpdateFKRelationships tail count
  | []->()
  
  
//Right, use with fsharp feature  
let UpdateFKRelationships (relationships:DbFKPK list)=
  let count=ref 0
  let rec UpdateFKRelationshipsA (relationshipsA:DbFKPK list)  (count:int ref)=
    match relationshipsA with
    | head::snd::tail when snd.PK_TABLE.Equals(head.PK_TABLE) ->   //head.PK_TABLE.Replace(snd.PK_TABLE,String.Empty).ToCharArray() |>Seq.exists (fun a->not <| Char.IsDigit(a) ) |>not-> //当更新PK_TABLE字段时
         if !count=0 then head.PK_TABLE_ALIAS<-head.PK_TABLE  //出现相等的第一对元素
         incr count  //count:=!count+1, --, decr count
         snd.PK_TABLE_ALIAS<-snd.PK_TABLE^string !count
         UpdateFKRelationshipsA (snd::tail) count
    | head::tail -> 
        if !count>0 then head.PK_TABLE_ALIAS<-head.PK_TABLE^string !count; count:=0 //最后一个相等的元素，此时后面一个元素和当前元素不等
        else head.PK_TABLE_ALIAS<-head.PK_TABLE  
        UpdateFKRelationshipsA tail count
    | []->()
  UpdateFKRelationshipsA relationships count
  
UpdateFKRelationships asFKRelationships
  
//Right, use with fsharp feature  
let UpdateFKRelationships01 (relationships:DbFKPK list)=
  let count=ref 0
  let previousPkTableName=ref Unchecked.defaultof<String>
  let rec UpdateFKRelationshipsA (relationshipsA:DbFKPK list)  (count:int ref) (previousPkTableName:string ref)=
    match !previousPkTableName,relationshipsA with
    | x, head::tail when String.IsNullOrEmpty(x) |>not && head.PK_TABLE.Equals(x) ->
         incr count  //count:=!count+1, --, decr count
         head.PK_TABLE_ALIAS<-head.PK_TABLE^string !count
         previousPkTableName:=head.PK_TABLE
         UpdateFKRelationshipsA tail count previousPkTableName
    | _,head::tail -> 
        count:=0 
        head.PK_TABLE_ALIAS<-head.PK_TABLE  
        previousPkTableName:=head.PK_TABLE
        UpdateFKRelationshipsA tail count previousPkTableName
    | _->() // or |_,[]
  UpdateFKRelationshipsA relationships count previousPkTableName
  
UpdateFKRelationships01 asFKRelationships

//Right, use with index
let UpdateFKRelationships (relationships:DbFKPK list)=
  let count=ref 0
  for i=0 to asFKRelationships.Length-1 do
    match i,asFKRelationships with
    | x,y when x>0 && y.[x].PK_TABLE =y.[x-1].PK_TABLE ->
        incr count
        y.[x].PK_TABLE_ALIAS<-y.[x].PK_TABLE^string !count
    | x,y ->
        count:=0
        y.[x].PK_TABLE_ALIAS<- y.[x].PK_TABLE
        
//Right use with foreach
let UpdateFKRelationshipsGeneral (relationships:DbFKPK list)=
  let count=ref 0
  //let  previous=ref Unchecked.defaultof<DbFKPK>
  let previousPkTableName=ref Unchecked.defaultof<String>
  for a in asFKRelationships do
    match !previousPkTableName,a with
    | x,y when String.IsNullOrEmpty(x) |>not && y.PK_TABLE=x->
        incr count
        y.PK_TABLE_ALIAS<-y.PK_TABLE^string !count
    | x,y ->
        count:=0
        y.PK_TABLE_ALIAS<-y.PK_TABLE
    previousPkTableName:=a.PK_TABLE
    
UpdateFKRelationshipsGeneral asFKRelationships
      
//    match i,asFKRelationships with
//    | x,y when x>0 && y.[x].PK_TABLE =y.[x-1].PK_TABLE ->
//        incr count
//        y.[x].PK_TABLE_ALIAS<-y.[x].PK_TABLE^string !count
//    | x,y ->
//        count:=0
//        y.[x].PK_TABLE_ALIAS<- y.[x].PK_TABLE
        
        
    
  
  
let UpdateFKRelationships asFKRelationships
  
ObjectDumper.Write(asFKRelationships,1)
  

//*)
let mutable current=String.Empty
let mutable pre=String.Empty
let count=ref 0
for i=0 to asFKRelationships.Length-1 do
  match i,asFKRelationships with
  | x,y when x>0 && y.[x].PK_TABLE =y.[x-1].PK_TABLE ->
      incr count
      y.[x].PK_TABLE_ALIAS<-y.[x].PK_TABLE^string !count
  | x,y ->
      count:=0
      y.[x].PK_TABLE_ALIAS<- y.[x].PK_TABLE
    


///////////////////////////////////////////////////////

let tableName="T_DJ_GHS"
let columns= DatabaseInformation.GetColumnSchemal2Way tableName

//为更新时,新建实体初始化字段
let sb=StringBuilder()
let asm=Assembly.LoadFrom( @"D:\Workspace\SBIIMS\WX.Data.DataModel\bin\Debug\WX.Data.DataModel.dll")
asm.GetTypes()
let entityType=asm.GetType(string.Format("{0}.{1}",asm.GetName().Name, tableName))
for a in columns do 
  match a.COLUMN_NAME,entityType with
  | x,y when y.GetProperty(x)<>null ->
    sb.AppendFormat( @"
       {0}=businessEntity.{0},"
      ,
      a.COLUMN_NAME
    )|>ignore
  | _ ->()
  
//为查询时获取实体
let tableName="T_DJSP_GHS"
let columns= DatabaseInformation.GetColumnSchemal2Way tableName
let sb=StringBuilder()
let asm=Assembly.LoadFrom( @"D:\Workspace\SBIIMS\WX.Data.DataModel\bin\Debug\WX.Data.DataModel.dll")
asm.GetTypes()
let entityType=asm.GetType(string.Format("{0}.{1}",asm.GetName().Name, tableName))
for a in columns do 
  match a.COLUMN_NAME,entityType with
  | x,y when y.GetProperty(x)<>null ->
    sb.AppendFormat( @"
       {0}=b.{0},"
      ,
      a.COLUMN_NAME
    )|>ignore
  | _ ->()
  
  
  
  
  ObjectDumper.Write(a.COLUMN_NAME)




////////////////////////////////////


let db=DatabaseFactory.CreateDatabase() 
let con=db.CreateConnection()
con.Open()

let dataTable=con.GetSchema("Columns",restrictionValues)

//For Tables
let tableName="T_CK"
let restrictionValues=[|null;"dbo";tableName;null|] 
let dataTableForTables=con.GetSchema("Tables",restrictionValues)
dataTableForTables.Columns.Count
dataTableForTables.Rows.Count
ObjectDumper.Write(dataTableForTables.Columns.[0],1)
ObjectDumper.Write(dataTableForTables.Rows.[0],1)
for a in dataTableForTables.Rows do 
  for i=0 to dataTableForTables.Columns.Count-1 do
    ObjectDumper.Write(a.[i].ToString(),0)


//For ForeignKeys
let dataTableForForeignKeys=con.GetSchema("ForeignKeys",restrictionValues)
dataTableForForeignKeys.Columns.Count
dataTableForForeignKeys.Rows.Count
ObjectDumper.Write(dataTableForForeignKeys.Columns.[2].DataType.ToString(),1)
ObjectDumper.Write(dataTableForForeignKeys.Rows.[0],1)

for a in dataTableForForeignKeys.Columns do 
  ObjectDumper.Write(a.ColumnName,0)
for a in dataTableForForeignKeys.Rows do 
  for i=0 to dataTableForForeignKeys.Columns.Count-1 do
    ObjectDumper.Write(a.[i].ToString(),0)
(*
  ItemArray: dbo
  ItemArray: FK_T_CK_C_FBID_T_DWBM
  ItemArray: SBIIMS0001
  ItemArray: dbo
  ItemArray: T_CK
  ItemArray: FOREIGN KEY
  ItemArray: NO
  ItemArray: NO

*)

//For ForeignKeyColumns, It's not right ???
let dataTableForForeignKeyColumns=con.GetSchema("ForeignKeyColumns",restrictionValues)
dataTableForForeignKeyColumns.Columns.Count
dataTableForForeignKeyColumns.Rows.Count
ObjectDumper.Write(dataTableForForeignKeyColumns.Columns.[0],1)
ObjectDumper.Write(dataTableForForeignKeyColumns.Rows.[0],1)


//For Indexes
let dataTableForIndexes=con.GetSchema("Indexes",restrictionValues)
dataTableForIndexes.Columns.Count
dataTableForIndexes.Rows.Count
ObjectDumper.Write(dataTableForIndexes.Columns.[0],1)
ObjectDumper.Write(dataTableForIndexes.Rows.[0],1)
(*
  ItemArray: dbo
  ItemArray: PK_T_CK
  ItemArray: SBIIMS0001
  ItemArray: dbo
  ItemArray: T_CK
  ItemArray: PK_T_CK

*)

//For IndexColumns
let dataTableForIndexColumns=con.GetSchema("IndexColumns",restrictionValues)
dataTableForIndexColumns.Columns.Count
dataTableForIndexColumns.Rows.Count
ObjectDumper.Write(dataTableForIndexColumns.Columns.[0],1)
ObjectDumper.Write(dataTableForIndexColumns.Rows.[0],1)

(*
  ItemArray: dbo
  ItemArray: PK_T_CK
  ItemArray: SBIIMS0001
  ItemArray: dbo
  ItemArray: T_CK
  ItemArray: C_ID
  ItemArray: 1
  ItemArray: 36
  ItemArray: PK_T_CK
*)


let tableName=null
let restrictionValues=[|null;"dbo";tableName;"FK_T_CK_C_FBID_T_DWBM"|] 
let dataTableForForeignKeys=con.GetSchema("ForeignKeys",restrictionValues)
dataTableForForeignKeys.Columns.Count
dataTableForForeignKeys.Rows.Count
for a in dataTableForForeignKeys.Rows do 
  for i=0 to dataTableForForeignKeys.Columns.Count-1 do
    ObjectDumper.Write(a.[i],0)

let dataTableAll=con.GetSchema()
ObjectDumper.Write(dataTableAll.Columns,1)

dataTableAll.Columns.Count

conn.Close()


let tableName ="T_DJ_GHS"
Clipboard.Clear()
QueryEntitiesCoding.GetCode tableName |>Clipboard.SetText
BusinessEntitiesCoding.GetCode tableName |>Clipboard.SetText

let columns= DatabaseInformation.GetColumnSchemal2Way tableName
ObjectDumper.Write(columns,0)
//public static class Clipboard
//    Member of System.Windows





(* Right!!!
//BusinessEntitiesCoding.GetCode "T_DJ_GHS"
let tableName ="T_DJ_GHS"
let columnsSeq=
  DatabaseInformation.GetColumnSchemal2Way tableName
  |>Seq.filter(fun a ->a.COLUMN_NAME.EndsWith("?") |>not)

let blankSapce="  "

let sb=StringBuilder()
sb.AppendFormat( @"namespace WX.Data.BusinessEntities

open System
open System.Runtime.Serialization

type BQ_{0}()=",tableName.Replace("T_",String.Empty))|>ignore
sb.AppendLine() |>ignore

for a in columnsSeq do
  sb.AppendFormat(@"{0}
    let  mutable _{1}:{2}={3}",
    //{0}
    String.Empty
    ,
    //{1}
    a.COLUMN_NAME
    ,
    //{2}
    match a.DATA_TYPE with
    |  x when x.ToLowerInvariant().EndsWith("string") -> x
    | x -> "System.Nullable<"^x^">"
    ,
    //{3}
    match a.DATA_TYPE with
    |  x when x.ToLowerInvariant().EndsWith("string") -> "null"
    |  x -> "System.Nullable<"^x^">()"
    )|>ignore
  //sb.AppendLine() |>ignore
   
sb.AppendLine() |>ignore
    
for a in columnsSeq do
  sb.AppendFormat(@"{0}
  [<DataMember>]
  member x.{1} 
    with get ()=_{1} 
    and set v=  _{1} <-v ",
    //{0}
    String.Empty
    ,
    //{1}
    a.COLUMN_NAME
    )|>ignore
  sb.AppendLine() |>ignore
  //sb.Append(Environment.NewLine) |>ignore


let x=System.Nullable<System.String>() 

type BQ_CK01()=
  //全部类型都使用System.Nullable<'T>, 只要有值都认为需要查询, 这样可省去 "IsQueryFieldName"成员
  let  mutable _C_BZ:System.String=null
  let  mutable _C_MR:System.Nullable<System.Boolean> =System.Nullable<System.Boolean>()

  [<DataMember>]
  member x.C_BZ 
    with get ()=_C_BZ 
    and set v=  _C_BZ <-v

  [<DataMember>]
  member x.C_MR 
    with get ()=_C_MR 
    and set v=  _C_MR <-v


type BQ_CK()=
  let  int16DefaultValue=Int16.MaxValue
  let  int32DefaultValue=Int32.MaxValue
  let int64DefaultValue=Int64.MaxValue
  
  let  mutable _C_BZ:System.String=null
  let mutable _IsQueryC_BZ=false
  let  mutable _C_MR:System.Nullable<System.Boolean> =System.Nullable<System.Boolean>()
  let mutable _IsQueryC_MR=false
  
  [<DataMember>]
  member  x.IsQueryC_BZ 
    with get ()=_IsQueryC_BZ
    and private  set v=_IsQueryC_BZ<-v
  [<DataMember>]
  member x.C_BZ 
    with get ()=_C_BZ 
    and set v=  _C_BZ <-v; if v<>null then x.IsQueryC_BZ<-true else x.IsQueryC_BZ<-false   

  [<DataMember>]
  member  x.IsQueryC_MR 
    with get ()=_IsQueryC_MR
    and private  set v=_IsQueryC_MR<-v
  [<DataMember>]
  member x.C_MR 
    with get ()=_C_MR 
    and set v=  _C_MR <-v; if v.Value then x.IsQueryC_MR<-true else x.IsQueryC_MR<-false  

Boolean.Parse("false")
    
//  [<DefaultValue>]
//  val mutable _C_CJRQ:System.Int32
//  [<DataMember>]
//  member x.C_CJRQ 
//    with get ()=x._C_CJRQ 
//    and set v= x._C_CJRQ <-v
    
let entity=BQ_CK()
entity.C_MR.HasValue <- null
entity.IsQueryC_BZ<-true

*)

(*
//Right!!!  Generate code for BusinessEntities, 
let tableName= "T_CK"
let columnsSeq=DatabaseInformation.GetColumnSchemal2Way tableName

 (*
 let columns=
   columnsSeq|>Seq.toArray
   columns.Length
 ObjectDumper.Write(columns,0)
 *)
let blankSapce="  "
let sb=StringBuilder()
sb.AppendFormat( @"namespace WX.Data.BusinessEntities

open System
open System.Runtime.Serialization
open Microsoft.Practices.EnterpriseLibrary.Validation.Validators

type B_{0}()=",
  tableName) |>ignore

for a in columnsSeq do
  sb.AppendFormat(@"
  {0}
  [<DefaultValue>]
  val mutable _{1}:{2}
  [<DataMember>]{3}{4}{5}{6}{7}{8}
  member x.{1} 
    with get ()=x._{1} 
    and set v= x._{1} <-v",
    //{0}
    String.Empty
    ,
    //{1}
    a.COLUMN_NAME
    ,
    //{2}
    match a.IS_NULLABLE,a.DATA_TYPE with
    | "YES",b when b.ToLowerInvariant().EndsWith("string") -> b
    | "YES",b -> "System.Nullable<"^b^">"
    | _,b ->b
    ,
    //{3}
    match a.IS_NULLABLE with 
    | "YES" ->String.Empty
    | _ -> Environment.NewLine^blankSapce^"[<NotNullValidator>]"
    ,
    //{4}
    match a.DATA_TYPE.ToLowerInvariant(),a.CHARACTER_MAXIMUM_LENGTH with
    | x,y when x.Contains("byte") || x.EndsWith("string") -> Environment.NewLine^blankSapce^"[<StringLengthValidator(1,"^y.ToString()^")>]"
    | _ -> String.Empty
    ,
    //{5}
    match a.DATA_TYPE.ToLowerInvariant().EndsWith("datetime") with
    | true -> Environment.NewLine^blankSapce^ @"[<DateTimeRangeValidator("""^DateTime.Parse("2000-01-01 00:00:00").ToString() ^ @""","""^DateTime.Parse("2099-01-01 00:00:00").ToString() ^ @""")>]"
    | _ ->String.Empty 
    ,
    //{6}
    (* Right, not good
    match a.DATA_TYPE.ToLowerInvariant(),a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE with
    | x,  _,_ when x.EndsWith("int16") -> Environment.NewLine^blankSapce^"[<RangeValidator(0s,RangeBoundaryType.Inclusive,"^Int16.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x,  _,_ when x.EndsWith("int32") -> Environment.NewLine^blankSapce^"[<RangeValidator("^Int32.MinValue.ToString()^",RangeBoundaryType.Exclusive,"^Int32.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x,  _,_ when x.EndsWith("int64") -> Environment.NewLine^blankSapce^"[<RangeValidator("^Int64.MinValue.ToString()^",RangeBoundaryType.Exclusive,"^Int64.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x,  _,_ when x.EndsWith("double") -> Environment.NewLine^blankSapce^"[<RangeValidator("^Double.MinValue.ToString()^",RangeBoundaryType.Exclusive,"^Double.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x, y,z when x.EndsWith("decimal") -> 
          let maxValue=10.0**float y-10.0**float -z
          Environment.NewLine^blankSapce^"[<RangeValidator("^(-maxValue).ToString()^",RangeBoundaryType.Exclusive,"^maxValue.ToString()^",RangeBoundaryType.Exclusive)>]" //??
    | _ ->String.Empty
    *)
    match a.DATA_TYPE.ToLowerInvariant(),(a.NUMERIC_PRECISION_RADIX,a.NUMERIC_SCALE) with
    | x,  _ when x.EndsWith("int16") -> Environment.NewLine^blankSapce^"[<RangeValidator(0s,RangeBoundaryType.Inclusive,"^Int16.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x,  _ when x.EndsWith("int32") -> Environment.NewLine^blankSapce^"[<RangeValidator("^Int32.MinValue.ToString()^",RangeBoundaryType.Exclusive,"^Int32.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x,  _ when x.EndsWith("int64") -> Environment.NewLine^blankSapce^"[<RangeValidator("^Int64.MinValue.ToString()^",RangeBoundaryType.Exclusive,"^Int64.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x,  _ when x.EndsWith("double") -> Environment.NewLine^blankSapce^"[<RangeValidator("^Double.MinValue.ToString()^",RangeBoundaryType.Exclusive,"^Double.MaxValue.ToString()^",RangeBoundaryType.Exclusive)>]"
    | x, (y,z) when x.EndsWith("decimal") -> 
          let maxValueString=
            match 10.0**float y-10.0**float -z with
            | zx when z>0 ->zx |>string
            | zx -> (zx|>string)^".0"
          Environment.NewLine^blankSapce^"[<RangeValidator(-"^maxValueString^",RangeBoundaryType.Exclusive,"^maxValueString^",RangeBoundaryType.Exclusive)>]" //??
    | _ ->String.Empty
    ,
    //{7}, RegexValidator, for mail address...
    //http://msdn.microsoft.com/en-us/library/ms998267.aspx
    //http://www.regular-expressions.info/tutorial.html
    match a.COLUMN_NAME with
    | "C_DY" ->  Environment.NewLine^blankSapce^ @"[<RegexValidator(@""^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$"")>]"   //Mial
    //| "C_LXDH" ->  Environment.NewLine^blankSapce^ @"[<RegexValidator(@""^\d{5}$"")>]"   //Phone
    | "C_ZJID" -> Environment.NewLine^blankSapce^ @"[<RegexValidator(@""^\d{18}$"")>]"   //证件ID
    | _ ->String.Empty
    ,
    //{8}
    match a.DATA_TYPE.ToLowerInvariant() with
    | x when x.EndsWith("decimal") -> Environment.NewLine^blankSapce^ @"[<RegexValidator(@""^(-)?\d+(\.\d\d)?$"")>]"  //1.20
    | _ ->String.Empty
    )|>ignore
  sb.Append(Environment.NewLine) |>ignore
 
*)
  
10.0**(-4.0)

-10 |>float

10.0**4.0

10.0**(-4 |>float) //right
10.0**-4 |>float //Wrong
10.0**float -4 //right
  
let dt=DateTime()  
let de=new Decimal(100.00)
let x=Decimal.Floor(100.01m)
let x=Decimal.Ceiling(100.01m)
let x=Decimal.GetBits(100.001m)
ObjectDumper.Write(de,2)
  
for a in columns do
  ObjectDumper.Write(a.DATA_TYPE,1)
    


//////////////////////////////////////////////////////////////////////////////////////////

let db=DatabaseFactory.CreateDatabase() 
let con=db.CreateConnection()
con.Open()
let restrictionValues=Array.zeroCreate<string> 4
//let restrictionValues01=Array.init 4 (fun i->String.Empty) //right
//let restrictionValues01=Array.create 4 String.Empty //Right
(*
Catalog TABLE_CATALOG 1
Owner TABLE_SCHEMA 2
Table TABLE_NAME 3
Table TypeTABLE_TYPE 4
*)
restrictionValues.[1]<-"dbo"
restrictionValues.[2]<-"T_CK"
restrictionValues.[3]<-"BASE TABLE"
let dataTableAll=con.GetSchema("Tables") //Get all Table
dataTableAll.Rows.Count
let dataTable=con.GetSchema("Tables",restrictionValues)
dataTable.Rows.Count
ObjectDumper.Write(dataTable.Rows.[0].ItemArray,1)


let restrictionValuesForColumn=[|null;"dbo";"T_CK";null|]
(*
Catalog TABLE_CATALOG 1
Owner TABLE_SCHEMA 2
Table TABLE_NAME 3
Table COLUMN_NAME 4
*)
let tableName="T_CK"
let dataTableAllColumns=con.GetSchema("Columns") //Get all columns from all tables
dataTableAllColumns.Rows.Count
let dataRows=dataTableAllColumns.Select("TABLE_NAME = '" + tableName + "'")

//GetDataType(Convert.ToInt32(drColVal[0]["DATA_TYPE"]))
dataRows.[0]
dataRows.[0].["IS_NULLABLE"].ToString()
dataRows.[1].["IS_NULLABLE"].ToString()
ObjectDumper.Write(dataRows.[0],2)
let dataTableColumns=con.GetSchema("Columns",restrictionValuesForColumn) 
dataTableColumns.Rows.Count
let x=dataTableColumns.Rows.[0]

for a in dataTableColumns.Rows do
  for b in dataTableColumns.Columns do 
   match a.[b.ColumnName] with
   | null ->()
   | c -> ()
   ObjectDumper.Write((a.[b.ColumnName].ToString(),b.ColumnName,b.DataType.ToString(),b.AllowDBNull),0)

for a in dataTableColumns.Columns do
  ObjectDumper.Write(a.ColumnName^"=")
  
for a in dataTableColumns.Columns do
  ObjectDumper.Write((a.ColumnName,a.DataType.ToString()),0)

for a in dataTableColumns.Columns do
  let code=String.Format(
             @"    member x.{0}
      with get ()=x._{0}
      and set v=x._{0}<-v",a.ColumnName)
  ObjectDumper.Write(code)
  
for a in dataTableColumns.Columns do
  let code=String.Format(
             @"[<DefaultValue>]
val mutable _{0}:string",a.ColumnName)
  ObjectDumper.Write(code)
  
for a in dataTableColumns.Columns do
  let code=String.Format(
             @"{0}=a.[""{0}""].ToString(),",a.ColumnName)
  ObjectDumper.Write(code)
  
  
  //ObjectDumper.Write(Environment.NewLine)

let dataRows=dataTableColumns.Select("TABLE_NAME = '" + tableName + "'")
dataRows.[0].[1]
ObjectDumper.Write(dataTableColumns.Rows.[0],1)
ObjectDumper.Write(System.Environment)


con.Close()

(*
 string strDataType = GetDataType(Convert.ToInt32(drColVal[0]["DATA_TYPE"]));
 string strIsnullable = drColVal[0]["IS_NULLABLE"].ToString();
 string strMaxLength = drColVal[0]["CHARACTER_MAXIMUM_LENGTH"].ToString();
 string strNumberiPre = drColVal[0]["NUMERIC_PRECISION"].ToString();
 string strDesc = drColVal[0]["Description"].ToString();
*)


let cmd=new SqlCommand("select * from T_CK")
let ds=cmd|>db.ExecuteDataSet
ds.Tables.[0].Columns.["C_ID"]
ds.Tables.[0].


let GenerateCode=
  let setion=ConfigurationManager.GetSection("dataConfiguration")
  let db=DatabaseFactory.CreateDatabase() //Set default database in advance
  let cmd=new SqlCommand("select * from T_CK")
  let ds=cmd|>db.ExecuteDataSet
  let sb=StringBuilder()
  for a in ds.Tables.[0].Columns do
    sb.Append("public ") |>ignore 
    sb.Append(a.DataType.FullName)  |>ignore 
    sb.Append(" ") |>ignore 
    sb.Append(a.ColumnName) |>ignore 
    sb.Append("{get;set}") |>ignore 
    sb.AppendLine() |>ignore
  sb.ToString()

//let GenerateCode01=
//  let setion=ConfigurationManager.GetSection("dataConfiguration")
//  let db=DatabaseFactory.CreateDatabase() //Set default database in advance
//  let cmd=new SqlCommand("select * from T_CK")
//  let ds=cmd|>db.ExecuteDataSet
//  let sb=StringBuilder()
//  for a in ds.Tables.[0].Columns do
//    sb.AppendFormat(
//            @"[<DefaultValue>]
//            val mutable _{0}:{1}
//            [<DataMember>]{2}
//            member x.C_FBID 
//              with get ()=x._{0}
//              and set v= x._{0}<-v",
//              a.ColumnName,a.DataType,
//              if(a.
//              System.Environment.NewLine^"[<NotNullValidator>]"
//                )|>ignore
//    sb.AppendLine()
//  sb.ToString()


(*
Table 1. Common Regular Expressions

Field  Expression Format Samples Description 
Name ^[a-zA-Z''-'\s]{1,40}$ John Doe
O'Dell Validates a name. Allows up to 40 uppercase and lowercase characters and a few special characters that are common to some names. You can modify this list. 
Social Security Number ^\d{3}-\d{2}-\d{4}$ 111-11-1111 Validates the format, type, and length of the supplied input field. The input must consist of 3 numeric characters followed by a dash, then 2 numeric characters followed by a dash, and then 4 numeric characters. 
Phone Number ^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$ (425) 555-0123
425-555-0123
425 555 0123
1-425-555-0123 Validates a U.S. phone number. It must consist of 3 numeric characters, optionally enclosed in parentheses, followed by a set of 3 numeric characters and then a set of 4 numeric characters.  
E-mail  ^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$ someone@example.com Validates an e-mail address. 
URL ^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$ http://www.microsoft.com Validates a URL  
ZIP Code ^(\d{5}-\d{4}|\d{5}|\d{9})$|^([a-zA-Z]\d[a-zA-Z] \d[a-zA-Z]\d)$ 12345 Validates a U.S. ZIP Code. The code must consist of 5 or 9 numeric characters. 
Password (?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,10})$   Validates a strong password. It must be between 8 and 10 characters, contain at least one digit and one alphabetic character, and must not contain special characters. 
Non- negative integer ^\d+$ 0
986 Validates that the field contains an integer greater than zero. 
Currency (non- negative) ^\d+(\.\d\d)?$ 1.00 Validates a positive currency amount. If there is a decimal point, it requires 2 numeric characters after the decimal point. For example, 3.00 is valid but 3.1 is not. 
Currency (positive or negative) ^(-)?\d+(\.\d\d)?$ 1.20 Validates for a positive or negative currency amount. If there is a decimal point, it requires 2 numeric characters after the decimal point. 

*)