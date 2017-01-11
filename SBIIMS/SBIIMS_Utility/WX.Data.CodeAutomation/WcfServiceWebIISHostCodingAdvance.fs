namespace WX.Data.CodeAutomation

open System
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type WcfServiceWebIISHostCodingAdvance=
  static member GetCode (databaseInstanceName:string)=  
    let sb=StringBuilder()
    sb.AppendFormat( @"<%@ ServiceHost Language=""C#"" Debug=""true"" Service=""WX.Data.WcfService.WS_{0}"" CodeBehind=""WS_{0}.fs"" %>"
      ,
      databaseInstanceName
      ) |>ignore
    string sb

  static member GetConfigPartCode (databaseInstanceName:string)=  
    let sb=StringBuilder()  //使用XQuery取出Service节点
    sb.AppendFormat( @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <system.serviceModel>
        <services>
            <service name=""WX.Data.WcfService.WS_{0}"">
                <endpoint address="""" binding=""wsHttpBinding"" bindingConfiguration=""""
                    contract=""WX.Data.ServiceContracts.IWS_{0}"" isSystemEndpoint=""false"">
                    <identity>
                        <dns value=""localhost"" />
                    </identity>
                </endpoint>
                <endpoint address=""mex"" binding=""mexHttpBinding"" contract=""IMetadataExchange"" />
                <host>
                    <baseAddresses>
                        <add baseAddress=""http://localhost:8080/WS_{0}"" />
                    </baseAddresses>
                    <timeouts openTimeout=""00:05:00"" />
                </host>
            </service>
    </system.serviceModel>
</configuration>",
      databaseInstanceName
      )|>ignore
    string sb

  static member GetConfigInitialCode (databaseInstanceName:string) (tableRelatedInfos:TableRelatedInfo seq)=  //static member GetCode (typedTableNames:(string*TableTemplateType) list)=
    let sb=StringBuilder()
    sb.AppendFormat( @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
    <system.serviceModel>
        <protocolMapping>
            <remove scheme=""http"" />
            <add scheme=""http""
                 binding=""wsHttpBinding"" />
        </protocolMapping>
        <bindings>
            <basicHttpBinding>
                <binding name=""""
                         closeTimeout=""00:01:00""
                         openTimeout=""00:01:00""
                         receiveTimeout=""00:10:00""
                         sendTimeout=""00:01:00""
                         allowCookies=""false""
                         bypassProxyOnLocal=""false""
                         hostNameComparisonMode=""StrongWildcard""
                         maxBufferSize=""6553600""
                         maxBufferPoolSize=""52428800""
                         maxReceivedMessageSize=""6553600""
                         messageEncoding=""Text""
                         textEncoding=""utf-8""
                         transferMode=""Buffered""
                         useDefaultWebProxy=""true"">
                    <readerQuotas maxDepth=""3200""
                                  maxStringContentLength=""819200""
                                  maxArrayLength=""1638400""
                                  maxBytesPerRead=""409600""
                                  maxNameTableCharCount=""1638400"" />
                    <security mode=""None"">
                        <transport clientCredentialType=""None""
                                   proxyCredentialType=""None""
                                   realm="""" />
                        <message clientCredentialType=""UserName""
                                 algorithmSuite=""Default"" />
                    </security>
                </binding>
            </basicHttpBinding>
            <wsHttpBinding>
                <binding name=""""
                         closeTimeout=""00:02:00""
                         openTimeout=""00:02:00""
                         receiveTimeout=""00:10:00""
                         sendTimeout=""00:01:00""
                         bypassProxyOnLocal=""false""
                         transactionFlow=""false""
                         hostNameComparisonMode=""StrongWildcard""
                         maxBufferPoolSize=""52428800""
                         maxReceivedMessageSize=""6553600""
                         messageEncoding=""Text""
                         textEncoding=""utf-8""
                         useDefaultWebProxy=""true""
                         allowCookies=""false"">
                    <readerQuotas maxDepth=""3200""
                                  maxStringContentLength=""819200""
                                  maxArrayLength=""1638400""
                                  maxBytesPerRead=""409600""
                                  maxNameTableCharCount=""1638400"" />
                    <reliableSession ordered=""true""
                                     inactivityTimeout=""00:10:00""
                                     enabled=""false"" />
                    <security mode=""Message"">
                        <transport clientCredentialType=""Windows""
                                   proxyCredentialType=""None""
                                   realm="""" />
                        <message clientCredentialType=""Windows""
                                 negotiateServiceCredential=""true""
                                 algorithmSuite=""Default"" />
                    </security>
                </binding>
            </wsHttpBinding>
        </bindings>
        <services>
            <service name=""WX.Data.WcfService.WS_{0}"">
                <endpoint address="""" binding=""wsHttpBinding"" bindingConfiguration=""""
                    contract=""WX.Data.ServiceContracts.IWS_{0}"" isSystemEndpoint=""false"">
                    <identity>
                        <dns value=""localhost"" />
                    </identity>
                </endpoint>
                <endpoint address=""mex"" binding=""mexHttpBinding"" contract=""IMetadataExchange"" />
                <host>
                    <baseAddresses>
                        <add baseAddress=""http://localhost:8080/WS_{0}"" />
                    </baseAddresses>
                    <timeouts openTimeout=""00:05:00"" />
                </host>
            </service>
        </services>
        <behaviors>
            <serviceBehaviors>
                <behavior name="""">
                    <serviceMetadata httpGetEnabled=""true"" />
                    <serviceDebug httpHelpPageEnabled=""true""
                                  includeExceptionDetailInFaults=""true"" />
                    <serviceThrottling maxConcurrentCalls=""1600""
                                       maxConcurrentSessions=""10000""
                                       maxConcurrentInstances=""11600"" />
                </behavior>
            </serviceBehaviors>
        </behaviors>
        <serviceHostingEnvironment multipleSiteBindingsEnabled=""true"" />
    </system.serviceModel>
</configuration>",
      databaseInstanceName
      )|>ignore
    string sb