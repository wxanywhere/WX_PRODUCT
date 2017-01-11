namespace WX.Data.CodeAutomation
open System
open System.IO
open System.Collections
open System.Data.Mapping
open System.Data.Entity.Design
open Microsoft.Practices.EnterpriseLibrary.Data
open WX.Data.Helper
open WX.Data.Database.DataAccess

type  EdmGenX=
  static member Generator (databaseInstanceName:string, namespaceName:string,targetDirectory:string)=
    try
      match GetDatabaseInfo databaseInstanceName with
      | Some x ->
          match
            ( 
            seq{
              yield! x.TableInfos|>Array.map (fun a->a.TABLE_SCHEMA, a.TABLE_NAME, 1uy)
              yield! x.ViewInfos|>Array.map (fun a->a.TABLE_SCHEMA, a.TABLE_NAME, 2uy)
              yield! x.StoredProcedureInfos|>Array.map (fun a->a.ROUTINE_SCHEMA, a.ROUTINE_NAME, 3uy)
            }
            |>Seq.toArray)
            with
          | y ->    
              EdmGenX.Generator (databaseInstanceName, x.ConnectionString, namespaceName, y, targetDirectory)
      | _ ->ObjectDumper.Write "指定的数据库信息错误"; [||]
    with e ->[||]

  static member Generator (fileNamePrefix:string, connectionString:string, namespaceName:string,objectSchema_Name_TypeIDs:(string*string*byte)[],targetDirectory:string)=
    try
      seq{ 
        match new EntityStoreSchemaGenerator("System.Data.SqlClient", connectionString, String.Format("{0}.Store",namespaceName)), Path.Combine (targetDirectory,fileNamePrefix) with
        | x, f  ->
            x.GenerateForeignKeyProperties<-true
            match 
              match objectSchema_Name_TypeIDs|>Seq.exists (fun (_,a,_)->a="T_RZ")|>not with
              | true ->objectSchema_Name_TypeIDs|>Seq.append [|"dbo","T_RZ",1uy|]|>Seq.toArray
              | _ ->objectSchema_Name_TypeIDs
              |>Seq.map (fun (a,b,_)->new EntityStoreSchemaFilterEntry(null, a, b, EntityStoreSchemaFilterObjectTypes.All, EntityStoreSchemaFilterEffect.Allow))
              |>Seq.toArray
              with                                                                  
            | y ->
                yield! x.GenerateStoreMetadata(y,EntityFrameworkVersions.Version3)
                x.WriteStoreSchema(String.Format ("{0}.ssdl",f))
                match new EntityModelSchemaGenerator(x.EntityContainer, namespaceName, String.Format("{0}Entities",fileNamePrefix)) with
                | z ->
                    z.GenerateForeignKeyProperties<-true
                    yield! z.GenerateMetadata(EntityFrameworkVersions.Version3)
                    z.WriteModelSchema(String.Format ("{0}.csdl",f))
                    z.WriteStorageMapping(String.Format ("{0}.msl",f))
                    match new EntityCodeGenerator(LanguageOption.GenerateCSharpCode) with
                    | u ->
                        u.LanguageOption<-LanguageOption.GenerateCSharpCode
                        yield! u.GenerateCode(String.Format ("{0}.csdl",f),String.Format ("{0}Entities.cs",f),EntityFrameworkVersions.Version3) 
                    (*
                    View现已经用预编译生成，用以下方式生成将会报错??
                    Erro Message:
                    The mapping and metadata information for EntityContainer 'SBIIMS_VCEntities' no longer matches the information used to create the pre-generated views.
                    match new EntityViewGenerator(LanguageOption.GenerateCSharpCode) with
                    | u ->
                        u.LanguageOption<-LanguageOption.GenerateCSharpCode
                        yield! u.GenerateViews(new StorageMappingItemCollection(z.EdmItemCollection,x.StoreItemCollection,String.Format ("{0}.msl",f)), String.Format ("{0}Views.cs",f))
                    *)
      }
      |>Seq.toArray
    with e ->ObjectDumper.Write e; [|null|]
