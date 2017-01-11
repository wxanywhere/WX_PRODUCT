// Learn more about F# at http://fsharp.net
namespace WX.Data
(*
open System
open System.Configuration
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.PSeq
open WX.Data.CodeAutomation
open WX.Data.IDataAccess
open WX.Data.DataAccess
open WX.Data.BusinessEntities



[<STAThread>]
[<EntryPoint>]
let main args=
  ConfigHelper.INS.LoadDefaultServiceConfigToManager
  let da=
    DA_DJ_JHGL.INS |>unbox<IDA_DJ_JHGL>
    
  let queryEntity=BQ_DJ_JHGL()
  queryEntity.C_JBR <-Nullable<Guid>(Guid("154f5e0f-63d4-4d25-9093-50a7ae929c5c"))
  let businessEntities=da.GetDJ_JHGLs queryEntity
  ObjectDumper.Write(businessEntities,1)
*)
//  ObjectDumper.Write(businessEntities.[0].B_T_DJSP_JHGLs)
//
//  let creatEntity=businessEntities.[0]
//  creatEntity.C_ID<- Guid("7F09563C-F027-45b3-BCFC-2C454BC1F567")
//  for a in creatEntity.B_T_DJSP_JHGLs do
//    a.C_DJID <- Guid("7F09563C-F027-45b3-BCFC-2C454BC1F567")
//
//  ObjectDumper.Write(creatEntity,1)
//
//  try
//    da.CreateDJ_JHGL creatEntity |>ignore
//  with 
//  | e -> ObjectDumper.Write(e,1)
  
  (*
  let tableName="T_DJ_JHGL"
  let asFKRelationships= DatabaseInformation.GetAsFKRelationship tableName
  *)
  
  ///////////////////////////////////////
  
  //let columnsSeq=DatabaseInformation.GetColumnSchemal2Way "T_CK"
  //let columns=columnsSeq|>Seq.toArray
  //let db=Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory.CreateDatabase("SBIIMS")

  //0
  
(*  Wrong,  新增一个实体信息时，其相关的参考实体应该在同一个“use sb=new SBIIMSEntitiesAdvance()”中进行
  let t_DWBM=GetT_DWBM (Guid("69896fad-c145-4b3a-b122-36df6b6d2c87"))

  let entity01=
            T_CK(C_BZ = null,
                 C_CJRQ = DateTime.Now,
                 C_CKDZ = "九门里",
                 C_DQ = "001001",
                 C_GXRQ = DateTime.Now,
                 //C_ID = Guid.NewGuid(),
                 C_JY = false,
                 C_LXDH = "九门里",
                 C_MC = "九门里1号仓库",
                 C_MR = false,
                 T_DWBM=t_DWBM.Head )
                 
  AddT_CK entity01 |>ignore
  0
*)
