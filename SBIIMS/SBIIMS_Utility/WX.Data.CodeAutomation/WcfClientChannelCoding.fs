namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type WcfClientChannelCoding=
  static member GetCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.AppendFormat( @"namespace WX.Data.ClientChannel
open System
open System.Configuration
open System.Runtime.Serialization
open System.ServiceModel
open FSharp.Collections.ParallelSeq
//open Microsoft.ServiceBus
open WX.Data
open WX.Data.ClientHelper
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.BusinessLogic
open WX.Data.ServiceContracts

type WS_{0}Channel() =
  let _EndpointName =
    match ClientChannel with
    | FromServer -> ""WSHttpBinding_IWS_{0}""
    | FromAzure -> ""Azure_WS_{0}"" 
    | _ ->null
  let serviceNamespaceDomain = 
    match ClientChannel with
    | FromAzure -> ConfigurationManager.AppSettings.[""{1}""]
    | _ ->null
  let serviceUri: Uri =
    match ClientChannel with
    | FromAzure ->Null() //ServiceBus不能使用FrameworkClientProfile, ServiceBusEnvironment.CreateServiceUri(""sb"", serviceNamespaceDomain, ""WS_{0}"")
    | _ -> Null() 
  let channelFactory: ChannelFactory<IWS_{0}> =
    match ClientChannel with
    | FromServer -> new ChannelFactory<IWS_{0}>(_EndpointName)
    | FromAzure -> new ChannelFactory<IWS_{0}>(_EndpointName, new EndpointAddress(serviceUri))
    | _ -> Null()
  let client: IWS_{0} = 
    match ClientChannel with
    | FromServer
    | FromAzure ->channelFactory.CreateChannel()  
    | _ -> Null() 
  static member public INS= WS_{0}Channel()"
      ,
      databaseInstanceName
      ,
      "ServiceNamespaceDomain"
      )|>ignore
    sb.AppendLine()|>ignore
    for a in tableRelatedInfos do
      match a.TableTemplateType with
      | MainTableWithOneLevel
      | MainTableWithTwoLevels->
          WcfClientChannelCoding.GetCodeWithTemplateOne a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | IndependentTable ->
          match a.ColumnConditionTypes with
          | ColumnConditionTypeContains [HasXH] _ ->
              WcfClientChannelCoding.GetCodeWithTemplateThree a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
          | _ ->
              WcfClientChannelCoding.GetCodeWithTemplateOne a.TableName
              |>string |>sb.Append |>ignore
              sb.AppendLine()|>ignore
      | ChildTable 
      | LeafTable -> 
          WcfClientChannelCoding.GetCodeWithTemplateTwo a.TableName
          |>string |>sb.Append |>ignore
          sb.AppendLine()|>ignore
      | _ ->()
    string sb

  static member private GetCodeWithTemplateOne (tableName:string)=
    String.Format( @"{0}
  member this.Get{1}s (queryEntity:BQ_{1})=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Get{1}s queryEntity
    | _ ->
        BL_{1}.INS.Get{1}s queryEntity
  member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Create{1} executeContent
    | _ ->
        BL_{1}.INS.Create{1} executeContent
  member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Create{1}s executeContent
    | _ ->
        BL_{1}.INS.Create{1}s executeContent
  member this.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Update{1} executeContent
    | _ ->
        BL_{1}.INS.Update{1} executeContent
  member this.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Update{1}s executeContent
    | _ ->
        BL_{1}.INS.Update{1}s executeContent
  member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Delete{1} executeContent
    | _ ->
        BL_{1}.INS.Delete{1} executeContent
  member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Delete{1}s executeContent
    | _ ->
        BL_{1}.INS.Delete{1}s executeContent{3}",
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
      match tableName with
      | EqualsIn [JXCSHDJTableName;JHGLDJTableName] x->
          String.Format(@"{0}
  member this.Create{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Create{1}_CGJH executeContent
    | _ ->
        BL_{1}.INS.Create{1}_CGJH executeContent
  member this.Update{1}_CGJH (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Update{1}_CGJH executeContent
    | _ ->
        BL_{1}.INS.Update{1}_CGJH executeContent",
            //{0}
            String.Empty
            ,
            //{1}
            match x with
            | _ when x.StartsWith("T_") ->x.Remove(0,2)
            | _ -> x
            ,
            //{2}
            x)
      | _ ->String.Empty
       )

  static member private GetCodeWithTemplateTwo (tableName:string)=
    String.Format( @"{0}
  member this.Get{1}s (queryEntity:BQ_{1})=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Get{1}s queryEntity
    | _ ->
        BL_{1}.INS.Get{1}s queryEntity",
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
       )

  static member private GetCodeWithTemplateThree (tableName:string)=
    String.Format( @"{0}
  member this.Get{1}s (queryEntity:BQ_{1})=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Get{1}s queryEntity
    | _ ->
        BL_{1}.INS.Get{1}s queryEntity
  member this.Create{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Create{1} executeContent
    | _ ->
        BL_{1}.INS.Create{1} executeContent
  member this.Create{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Create{1}s executeContent
    | _ ->
        BL_{1}.INS.Create{1}s executeContent
  member this.Update{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Update{1} executeContent
    | _ ->
        BL_{1}.INS.Update{1} executeContent
  member this.Update{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Update{1}s executeContent
    | _ ->
        BL_{1}.INS.Update{1}s executeContent
  member this.Delete{1} (executeContent:BD_ExecuteContent<#BD_{2}>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Delete{1} executeContent
    | _ ->
        BL_{1}.INS.Delete{1} executeContent
  member this.Delete{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Delete{1}s executeContent
    | _ ->
        BL_{1}.INS.Delete{1}s executeContent
  member this.Batch{1}s (executeContent:BD_ExecuteContent<#BD_{2}[]>)=
    match ClientChannel with
    | FromServer
    | FromAzure ->
        use channel=client:?>IClientChannel  
        channel.Open()
        client.Batch{1}s executeContent
    | _ ->
        BL_{1}.INS.Batch{1}s executeContent",
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
       )