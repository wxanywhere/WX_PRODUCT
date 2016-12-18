namespace WX.Data.DataAccess
open System
open System.Data
open System.Text.RegularExpressions
open Microsoft.FSharp.Collections
open Microsoft.Practices.EnterpriseLibrary.Logging
open WX.Data.Helper
open WX.Data
open WX.Data.DataModel
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
open WX.Data.IDataAccess
open WX.Data.ServerHelper

type  DA_SBIIMS_JXC_TJBB_CGYCGHelper=
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
       x

  static member WriteBusinessLog (executeBase:BD_ExecuteBase,context:SBIIMS_JXC_TJBB_CGYCGEntitiesAdvance,now) (c_JLID,c_BM,c_BBM,c_YWLX,c_YWLXMC,c_CZLX,c_CZLXMC,c_ZBSL,c_NR,c_ZBM,c_ZBBM)=
    try 
      match executeBase.IsWriteBusinessLog with
      | x ->
          if (x.HasValue && x.Value) || (not x.HasValue && Config.isDefaultWriteBusinessLog) then
            executeBase.TrackingInfo
            |>DA_SBIIMS_JXC_TJBB_CGYCGHelper.GetTrackInfo now
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
    | e ->raise e

