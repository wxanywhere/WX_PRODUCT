namespace WX.Data.CodeAutomation

open System
open System.IO
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type AdvanceTypeGenerator=
  static member GenerateCodeFile  (assemblySuffix:string)  (baseDirectory:string) (tableNames:string list)=
    try
      let tableRelatedInfos= Generator.AttachRelatedInfo tableNames
      [
       AdvanceBusinessDataEntitiesTypeCoding.GenerateCodeFile assemblySuffix baseDirectory  tableRelatedInfos
       AdvanceBusinessQueryEntitiesTypeCodingServerSide.GenerateCodeFile assemblySuffix baseDirectory tableRelatedInfos
       AdvanceBusinessQueryEntitiesTypeCodingClientSide.GenerateCodeFile assemblySuffix baseDirectory tableRelatedInfos
       AdvanceDataAccessTypeCodingWithArray.GenerateCodeFile assemblySuffix  baseDirectory tableRelatedInfos
       AdvanceDataAccessTypeInterfaceCodingWithArray.GenerateCodeFile assemblySuffix  baseDirectory tableRelatedInfos
      ]
    with e -> ObjectDumper.Write e; raise e

//Right backup
//    [
//     yield AdvanceBusinessDataEntitiesTypeCoding.GenerateCodeFile tableNames baseDirectory
//     yield AdvanceBusinessDataEntitiesTypeCoding.GenerateCodeFile tableNames baseDirectory
//     yield AdvanceDataAccessTypeCoding.GenerateCodeFile  tableNames baseDirectory
//     yield AdvanceDataAccessTypeInterfaceCoding.GenerateCodeFile  tableNames baseDirectory
//    ]