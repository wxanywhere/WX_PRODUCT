namespace WX.Data.CodeAutomation
open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper
open WX.Data.CodeAutomationHelper

type DataAccessCodingDAHelper=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    try
      sb.Clear()|>ignore
      DataAccessCodingDAHelper.GenerateNamespaceCode
      |>string|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      DataAccessCodingDAHelper.GenerateDAHelperCode databaseInstanceName tableRelatedInfos    //获取单据名名称
      |>string|>sb.Append|>ignore
      sb.AppendLine()|>ignore
      string sb
    with 
    | e ->ObjectDumper.Write(e,2); raise e

  static member private GenerateNamespaceCode=
    @"namespace WX.Data.DataAccess
open System
open System.Data
open System.Text.RegularExpressions
open FSharp.Collections.ParallelSeq
open Microsoft.Practices.EnterpriseLibrary.Logging
open WX.Data
open WX.Data.Helper
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.ServerHelper"

  static member private GenerateDAHelperCode (databaseInstanceName:string)  (tableRelatedInfos:TableRelatedInfo seq)= 
    let sb=StringBuilder()
    sb.AppendFormat(@"
type  DA_{0}Helper=",databaseInstanceName)|>ignore
    match tableRelatedInfos|>PSeq.exists (fun a->a.TableName="T_DJLX") with
    | true  ->
        DataAccessCodingDAHelper.GenerateDAHelperGetDJHCode databaseInstanceName //单据号
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        DataAccessCodingDAHelper.GenerateDAHelperGetLSHCode databaseInstanceName //流水号
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        DataAccessCodingDAHelper.GenerateDAHelperGetDJH2MCode() //单据号2M
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
        (*已停用
        DataAccessCodingDAHelper.GenerateDAHelperUpdateLSHCode databaseInstanceName //更新流水号
        |>string |>sb.Append |>ignore
        *)
        DataAccessCodingDAHelper.GenerateDAHelperDJNameCode()
        |>string |>sb.Append |>ignore
        sb.AppendLine()|>ignore
    | _ ->()
    DataAccessCodingDAHelper.GenerateDAHelperTrackingInfoCode()  //BusinessEntity公共的跟踪信息
    |>string |>sb.Append |>ignore
    sb.AppendLine()|>ignore
    DataAccessCodingDAHelper.GenerateDAHelperWriteBusinessLogCode databaseInstanceName //写业务操作日志
    |>string |>sb.Append |>ignore
    sb.AppendLine()|>ignore
    string sb

  static member private GenerateDAHelperDJNameCode()=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetDJName (code:byte)=
    match code with
    {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
    | {0}uy->""{1}""",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_LX
            )|>ignore
        sbTem.Append(@"
    | _ ->String.Empty")|>ignore
        sbTem.ToString().TrimStart()
        )
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperTrackingInfoCode()=
    @"
  static member GetTrackInfo (now:DateTime) (trackingInfo:BD_Tracking)=
    match new T_RZ(), trackingInfo with
    | x, y -> //trackingInfo为空时需要报错
       x.C_ID<-Guid.NewGuid()
       x.C_FBID<-y.C_FBIDBase
       x.C_CJRQ<-now
       x.C_CZY<-y.C_CZYBase
       x.C_CZYXM<-y.C_CZYXMBase
       x.C_HOST<-y.C_HOSTBase
       x.C_IP<-y.C_IPBase
       x"

  static member private GenerateDAHelperGetDJHCode  (databaseInstanceName:string)=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetDJH (sb:{1}EntitiesAdvance) (c_DJLX:byte) (c_DJH:string) (now:DateTime)=
    match c_DJH with 
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2,5}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)->
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->
            match Seq.head sb.T_DJLSH_{1} with
            | y -> 
                if y.C_GXRQ.Date<>now.Date then y.C_LSH<-y.C_CSLSH+1M
                y.C_GXRQ<-now
                match y.C_LSH with
                | z ->
                    y.C_LSH<-z+1M
                    ""{1}""+now.ToString(""yyyyMMdd"")+string z",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->c_DJH
    | _ ->c_DJH")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperGetLSHCode  (databaseInstanceName:string)=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetLSH (sb:{1}EntitiesAdvance) (c_DJLX:byte) (c_DJH:string) (now:DateTime) (accStep:decimal)=
    match c_DJH with //单据号的流水号应该每天复位为 1000..., 每一天的序号都重新开始, 复位应该由表的触发器来完成
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)->
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->
            match Seq.head sb.T_DJLSH_{1} with
            | y ->
                if y.C_GXRQ.Date<>now.Date then y.C_LSH<-y.C_CSLSH+1M
                y.C_GXRQ<-now
                match y.C_LSH with
                | z ->y.C_LSH<-z+accStep; z",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->0M
    | _ ->0M")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperGetDJH2MCode()=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member GetDJH2M (c_DJLX:byte) (c_DJH:string) (c_LSH:decimal) (now:DateTime)=
    match c_DJH with //单据号的流水号应该每天复位为 1000..., 每一天的序号都重新开始, 复位应该由表的触发器来完成
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)->
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->""{1}""+now.ToString(""yyyyMMdd"")+string c_LSH",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->c_DJH                                                                                      
    | _ ->c_DJH")|>ignore
        sbTem.ToString().TrimStart()
        )
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  //已停用
  static member private GenerateDAHelperUpdateLSHCode  (databaseInstanceName:string)=
    let sbTem=StringBuilder()
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member UpdateLSH (sb:{1}EntitiesAdvance) (c_DJLX:byte) (c_DJH:string) (accStep:decimal) =
    match c_DJH with 
    | x when x=null || not <| Regex.IsMatch(x,@""^\w{{2}}[23456]\d{{3}}[01]\d[0123]\d\d{{3,}}$"",RegexOptions.None)-> //更新条件和获取条件应该一致, 最好使用正则表达式
        match c_DJLX with
        {0}",
        //{0}
        (
        sbTem.Remove(0,sbTem.Length) |>ignore
        for a in DJLXInfo do
          sbTem.AppendFormat(@"
        | {0}uy->sb.T_DJLSH_{1}|>Seq.head|>fun a->a.C_LSH<-a.C_LSH+accStep",
            //{0}
            a.C_DM
            ,
            //{1}
            a.C_QZ
            )|>ignore
        sbTem.Append(@"
        | _ ->()
    | _ ->()")|>ignore
        sbTem.ToString().TrimStart()
        )
        ,
        //{1}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e

  static member private GenerateDAHelperWriteBusinessLogCode  (databaseInstanceName:string)=
    let sb=StringBuilder()
    try
      sb.AppendFormat(  @"
  static member WriteBusinessLog (executeBase:BD_ExecuteBase,context:{0}EntitiesAdvance,now) (c_JLID,c_BM,c_BBM,c_YWLX,c_YWLXMC,c_CZLX,c_CZLXMC,c_ZBSL,c_NR,c_ZBM,c_ZBBM)=
    try 
      match executeBase.IsWriteBusinessLog with
      | x ->
          if (x.HasValue && x.Value) || (not x.HasValue && Config.isDefaultWriteBusinessLog) then
            executeBase.TrackingInfo
            |>DA_{0}Helper.GetTrackInfo now
            |>fun (a:T_RZ)->
                a.C_JLID<- c_JLID
                a.C_BM<- c_BM
                a.C_BBM<-c_BBM
                a.C_YWLX<-c_YWLX
                a.C_YWLXMC<-c_YWLXMC
                a.C_CZLX<-c_CZLX
                a.C_CZLXMC<-c_CZLXMC
                a.C_ZBSL<-c_ZBSL
                a.C_NR<-c_NR
                a.C_ZBM<- c_ZBM
                a.C_ZBBM<-c_ZBBM
                a
            |>context.T_RZ.AddObject
    with
    | e ->raise e",
        //{0}
        databaseInstanceName
        )|>ignore
      sb.ToString()
    with 
    | e -> ObjectDumper.Write(e,2); raise e