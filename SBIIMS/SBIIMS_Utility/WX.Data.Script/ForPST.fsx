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
#r "office.dll"
 
open System
open System.Collections.Generic
open System.Reflection
open System.Text
open System.Text.RegularExpressions
open System.Data
open System.Runtime.Serialization
open System.Data.SqlClient
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Data
open Microsoft.Practices.ObjectBuilder2
open System.Windows.Forms
//let projectPath= @"D:\Workspace\SBIIMS"
 
//It must load on sequence
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"

 
open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataOperate
open WX.Data.PSeq
open WX.Data
 
ConfigHelper.INS.LoadDefaultServiceConfigToManager
///////////////////////////////////////////////////////////////////////////////////////////////////
type Subsource=
 {
 AccountKey:string
 }
 


#r "Microsoft.Office.Interop.Excel.dll"
open Microsoft.Office.Interop.Excel
open System.Runtime.InteropServices
 
//---------------------------------------------------
 
// Create new Excel.Application
let app = new ApplicationClass(Visible = true)

// Get the workbooks collection
let workbooks = app.Workbooks

let filePath= @"D:\PST\Tickets\Bell\P090902084823\WMS Resets 09.01.09.xls"
let workbook=workbooks.Open(filePath)
let sheets = workbook.Worksheets
//For open
let worksheet1 =(sheets.[box 1] :?> _Worksheet)

worksheet1.Range("A8").Value2
worksheet1.Range("A14").Value2


//Read data from Excel
let dataSourceOriginal=
  seq{
      let index:ref int=ref 8
      while worksheet1.Range("A"+(string !index)).Value2<>null do
        yield
          {
          SBSRC_ID=string<| worksheet1.Range("A"+(string (!index))).Value2
          }

    }
    |>pseq
    |>toNetList




 
///////////////////////////////////////////////////////////////////////////////////////////////////

type Subsource=
 {
 SBSRC_ID:string
 SBSRC_NM:string
 CMNTS:string
 mutable GRP:string
 mutable REMEDY_GRP:string
 mutable ESCALATE_GRP:string
 }
 
//////////////////////////////////////////////////////////////////////////////////////////////////////
#r "Microsoft.Office.Interop.Excel.dll"
open Microsoft.Office.Interop.Excel
open System.Runtime.InteropServices
 
//---------------------------------------------------
 
// Create new Excel.Application
let app = new ApplicationClass(Visible = true)
 
// Get the workbooks collection
let workbooks = app.Workbooks

let filePath= @"D:\Common Workspace\Send\Subsource004.xlsx"
let workbook=workbooks.Open(filePath)
let sheets = workbook.Worksheets
//For open
let worksheet5 =(sheets.[box 1] :?> _Worksheet)
worksheet5.Name<-"Additional Comments"
let worksheet6 = (sheets.[box 2] :?> _Worksheet)
worksheet6.Name<-"Critical Action"
let worksheet7 =(sheets.[box 3] :?> _Worksheet)
worksheet7.Name<-"Warning Action"
let worksheet8 =(sheets.[box 4] :?> _Worksheet)
worksheet8.Name<-"Harmless Action"

//--------------------------------
//For new
let appNew = new ApplicationClass(Visible = true)
 
// Get the workbooks collection
let workbooksNew = appNew.Workbooks

// Add a new workbook
let workbookNew = workbooksNew.Add(XlWBATemplate.xlWBATWorksheet)

// Get the worksheets collection
let sheetsNew = workbookNew.Worksheets
for i=0 to 3 do
  sheetsNew.Add()|>ignore

//For Create
let worksheet1 =(sheetsNew.[box 1] :?> _Worksheet)
worksheet1.Name<-"Additional Comments"
let worksheet2 = (sheetsNew.[box 2] :?> _Worksheet)
worksheet2.Name<-"Critical Action"
let worksheet3 =(sheetsNew.[box 3] :?> _Worksheet)
worksheet3.Name<-"Warning Action"
let worksheet4 =(sheetsNew.[box 4] :?> _Worksheet)
worksheet4.Name<-"Harmless Action"



//---------------------
//Read data from Excel
let dataSourceOriginal=
  seq{
      //for i=0 to 2806-1 do
      for i=0 to 610-1 do
        yield
          {SBSRC_ID=string<| worksheet5.Range("A"+(string (i+2))).Value2
           SBSRC_NM=string <|worksheet5.Range("B"+(string (i+2))).Value2
           CMNTS=string<| worksheet5.Range("C"+(string (i+2))).Value2
           GRP= String.Empty //string<| worksheet1.Range("D"+(string (i+2))).Value2
           REMEDY_GRP=String.Empty
           ESCALATE_GRP=String.Empty}
    }
    |>pseq
    |>toNetList

let group=
  dataSourceOriginal
  |>Seq.groupBy (fun a->a.SBSRC_ID)
  

let  dataSource=
  let sb=StringBuilder()
  let sbTem=StringBuilder()
  seq{
  for a,b in group do
    sb.Remove(0,sb.Length)|>ignore
    for c in b do 
      sb.Append(c.GRP+", ")|>ignore
    match sb with
    | x when x.Length>0 ->
        x.Remove(x.Length - 2,2)|>ignore
        sbTem.Remove(0,sbTem.Length)|>ignore  
        for d in 
          (x.ToString().Split([|", "|],StringSplitOptions.RemoveEmptyEntries))
          |>Seq.distinct
          do
          sbTem.Append(d+", ")|>ignore
    | _ -> ()
    yield 
       {
        SBSRC_ID=(b|>Seq.head).SBSRC_ID
        SBSRC_NM=(b|>Seq.head).SBSRC_NM
        CMNTS=String.Empty
        GRP= 
          match sbTem with
          | y when y.Length>0 -> y.Remove(y.Length - 2,2)|>ignore;y.ToString()
          | _ ->String.Empty
          
        REMEDY_GRP=String.Empty
        ESCALATE_GRP=String.Empty}
  }  
  |>pseq
  |>toNetList   
     
ObjectDumper.Write(dataSource,1)    

//---------------------------------------------------
 
///////////////////////////////////////////////////
 
 
 
//Escalation - Adtnl Cmnts 
let sbsrc_ADTNLs=
    seq{
      let db=DatabaseFactory.CreateDatabase("External")
      let sqlText=
        @"
        select sbsrc_id, sbsrc_nm, adtnl_cmnts from t_sbsrc
        where adtnl_cmnts like '%escalate%' or adtnl_cmnts like '%remedy%' or adtnl_cmnts like '%assign%'
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield
          {SBSRC_ID=string reader.["SBSRC_ID"];
          SBSRC_NM=string reader.["SBSRC_NM"];
          CMNTS= string reader.["adtnl_cmnts"];
          GRP=String.Empty;
          REMEDY_GRP=String.Empty;
          ESCALATE_GRP=String.Empty}
      }
    |>pseq
    |>toNetList
   
//Escalation - Critical action   
let sbsrc_CRICs=
    seq{
      let db=DatabaseFactory.CreateDatabase("External")
      let sqlText=
        @"
        select a.sbsrc_id, a.sbsrc_nm, b.crtcl_actn from t_sbsrc a,
        t_sbsrc_mntrs b where a.sbsrc_id=b.sbsrc_id
        and (b.crtcl_actn like '%escalate%' or b.crtcl_actn like '%remedy%' or b.crtcl_actn like '%assign%')
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield
          {SBSRC_ID=string reader.["SBSRC_ID"];
          SBSRC_NM=string reader.["SBSRC_NM"];
          CMNTS= string reader.["crtcl_actn"];
          GRP=String.Empty;
          REMEDY_GRP=String.Empty;
          ESCALATE_GRP=String.Empty}
      }
    |>pseq
    |>toNetList
  
 
sbsrc_CRICs.Count
(*   
let sbsrc_CRICs=
    seq{
      let db=DatabaseFactory.CreateDatabase("External")
      let sqlText=
        @"
        select a.sbsrc_id, a.sbsrc_nm, b.crtcl_actn from t_sbsrc a,
        t_sbsrc_mntrs b where a.sbsrc_id=b.sbsrc_id
        and (b.crtcl_actn like '%escalate%' or b.crtcl_actn like '%remedy%' or b.crtcl_actn like '%assign%')
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield
          {SBSRC_ID=string reader.["SBSRC_ID"];
          SBSRC_NM=string reader.["SBSRC_NM"];
          CMNTS= string reader.["crtcl_actn"];
          REMEDY_GRP=String.Empty;
          ESCALATE_GRP=String.Empty}
      }
    |>pseq
    |>take 100
    |>toNetList
*)
   
   
//Escalation - Warning action  
let sbsrc_WRNs=
    seq{
      let db=DatabaseFactory.CreateDatabase("External")
      let sqlText=
        @"
        select a.sbsrc_id, a.sbsrc_nm, b.wrng_actn from t_sbsrc a,
        t_sbsrc_mntrs b where a.sbsrc_id=b.sbsrc_id
        and (b.wrng_actn like '%escalate%' or b.wrng_actn like '%remedy%' or b.wrng_actn like '%assign%')
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield
          {SBSRC_ID=string reader.["SBSRC_ID"];
          SBSRC_NM=string reader.["SBSRC_NM"];
          CMNTS= string reader.["wrng_actn"];
          GRP=String.Empty;
          REMEDY_GRP=String.Empty;
          ESCALATE_GRP=String.Empty}
      }
    |>pseq
    |>toNetList
   
sbsrc_WRNs.Count
   
//Escalation - Harmless action
let sbsrc_HAMLs=
    seq{
      let db=DatabaseFactory.CreateDatabase("External")
      let sqlText=
        @"
        select a.sbsrc_id, a.sbsrc_nm, b.hrmlss_actn from t_sbsrc a,
        t_sbsrc_mntrs b where a.sbsrc_id=b.sbsrc_id
        and (b.hrmlss_actn like '%escalate%' or b.hrmlss_actn like '%remedy%' or b.hrmlss_actn like '%assign%')
        "
      use cmd=new SqlCommand(sqlText)
      use reader=db.ExecuteReader cmd
      while reader.Read() do
        yield
          {SBSRC_ID=string reader.["SBSRC_ID"];
          SBSRC_NM=string reader.["SBSRC_NM"];
          CMNTS= string reader.["hrmlss_actn"];
          GRP=String.Empty;
          REMEDY_GRP=String.Empty;
          ESCALATE_GRP=String.Empty}
      }
    |>pseq
    |>toNetList
 
for a in sbsrc_ADTNLs do
  match a.CMNTS,(a.CMNTS.IndexOf(@""""),a.CMNTS.LastIndexOf(@"""")) with
  | x,(y,z) when z>y ->a.REMEDY_GRP<-x.Substring(y+2,z-1-(y+2))
  | _ ->()
 
 
////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////
 
let dataSource=
  //sbsrc_ADTNLs
  //sbsrc_HAMLs
  sbsrc_WRNs
  //sbsrc_CRICs

  |>pseq
  |>take 100
  |>toNetList
 
 

 

//let regexPattersForGRP=
//  [@"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)"; //@"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)"  //(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)  //(?<=^.*\n*.*[Ee]scalate.+\n*.*"")\b.+\b(?="".*)//For escalate to " Group Name"
//   @"(?<=^.*\n*.*[Ee]scalation\s+to.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)";  //@"((?<=^.*\n*.*[Ee]scalation\s+to.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)"; //for escalation to " Group Name"
//  //@"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+-.+\b(?=\s*"".*)"; //For escalate to " Group-Name"
//  @"(?<=^.*\n*.*[Ee]scalate.+\n*.*')\b.+\b(?='.*)"; //For escalate to 'Group Name'
//  @"(?<=^.*[Ee]scalate.*\s+to\s+""{0})\b.+\b(?=""{0}\.{0}.*)";  //For escalate to Group Name.
//  ]
//
//let regexPattersForGRP=
//  [
//  @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<="")\s*\w.+\w\s*(?="")";
//  @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=')\s*\w.+\w\s*(?=')";
//  @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=to\s*)(?<!"")(?<!')\s*\w.+\w\s*(?!')(?!"")(?=\n)";
//  ]
 
dataSourceOriginal.Count
for a in dataSourceOriginal do
 a.GRP<-String.Empty
 
let regexPattersForGRP=
  [
  @"((?<=[Ee]scalat[\s\w\n\:\(\)\-\&\d\,]*[""\']+)|(?<=[Rr]emedy[\s\w\n\:\(\)\-\&\d\,]*[""\']+)|(?<=[Aa]ssign[\s\w\n\:\(\)\-\&\d\,]*[""\']+))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(\)\-\&\d\,]*[""\']+)[\s\w\d\-\&]+(?=[""\']+)";
  //@"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup|[Qq]ueue)[\s\n\:]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup|[Qq]ueue)[\s\n\:]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup|[Qq]ueue)[\s\n\:]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=[\.\,\<\n\(\)$]+)"
  @"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=[\.\,\<\n\(\)]+)";
  @"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=$)"
  ]
  
let sb=StringBuilder()
sb.Remove(0,sb.Length)|>ignore
for a in dataSourceOriginal do
  sb.Remove(0,sb.Length)|>ignore
  seq{for pattern in regexPattersForGRP do
      match Regex.Matches(a.CMNTS, pattern,RegexOptions.Multiline)  with
      | x when x.Count>0->
          for b in x do
            if b.Groups.Count>0 then
              match b.Groups.[0].Value.Split([|"if ";"see "|],StringSplitOptions.RemoveEmptyEntries) with
              | y ->
                  match y.[0].Trim() with
                  | u when u.ToLower().StartsWith("the") ->yield u.Remove(0,3)
                  | u when u.ToLower().StartsWith("the appropriate") ->yield u.Remove(0,15)
                  | u ->yield u  //yield b.Groups.[0].Value.Trim()  //yield b.Groups.[0].Value.Split
      | _ ->()}
  |>Seq.distinct
  |>Seq.iter (fun b ->
      match b.Trim() with
      | x when x|>String.IsNullOrEmpty|>not && x.ToLower()<>"the" && x.ToLower()<>"group" && x.ToLower()<>"remedy queue for" -> x^", "|>sb.Append|>ignore
      | _ ->())
  if sb.Length>0 then
    sb.Remove(sb.Length - 2,2) |>ignore
    a.GRP<-sb.ToString()
 
//  SBSRC_ID:string
// SBSRC_NM:string
// CMNTS:string
// mutable GRP:string
// mutable REMEDY_GRP:string
// mutable ESCALATE_GRP:string
 
 dataSource
 |>Seq.groupBy (fun a->a.SBSRC_ID)
 
 
"Distributed and Web Operations".Split([|"and "|],StringSplitOptions.RemoveEmptyEntries).Length



@"
sdff

  wx".Trim()
 
//----------------------------------------------
 
for a in dataSource do
  a.ESCALATE_GRP<-String.Empty
 
 
let regexPattersForEscalate=
  [@"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)"; //@"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)"  //(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)  //(?<=^.*\n*.*[Ee]scalate.+\n*.*"")\b.+\b(?="".*)//For escalate to " Group Name"
   @"(?<=^.*\n*.*[Ee]scalation\s+to.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)";  //@"((?<=^.*\n*.*[Ee]scalation\s+to.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)"; //for escalation to " Group Name"
  //@"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+-.+\b(?=\s*"".*)"; //For escalate to " Group-Name"
  @"(?<=^.*\n*.*[Ee]scalate.+\n*.*')\b.+\b(?='.*)"; //For escalate to 'Group Name'
  @"(?<=^.*[Ee]scalate.*\s+to\s+""{0})\b.+\b(?=""{0}\.{0}.*)";  //For escalate to Group Name.
  ]
let sb=StringBuilder()
sb.Remove(0,sb.Length)|>ignore
for a in dataSource do
  sb.Remove(0,sb.Length)|>ignore
  for pattern in regexPattersForEscalate do
    match Regex.Matches(a.CMNTS, pattern,RegexOptions.Multiline)  with
    | x when x.Count>0->
          seq{ for b in x do
                   if b.Groups.Count>0 then
                     yield b.Groups.[0].Value}
          |>Seq.distinct
          |>Seq.iter (fun b -> b.Split([|".";","|],StringSplitOptions.RemoveEmptyEntries).[0].ToString()^","|>sb.Append|>ignore)
          if sb.Length>0 then
            sb.Remove(sb.Length - 1,1) |>ignore
            if String.IsNullOrEmpty <| a.ESCALATE_GRP.Trim() then
              a.ESCALATE_GRP<-sb.ToString()
    | _ ->()   
  
  
//-------------------------------------
  
for a in dataSource do
  a.ESCALATE_GRP<-String.Empty
 
 
let regexPattersForRemedy=
  [@"(?<=^.*\n*.*[Rr]emedy.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)"; ////For Remedy  " Group Name" without scalate
   @"(?<=^.*\n*.*[Aa]ssign.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Rr]emedy.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Ee]scalation.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)"; //For assign to " Group Name" without escalate and remedy
   @"(?<=^.*[Aa]ssign.*\s+to\s+""{0})(?<!^.*[Rr]emedy.*\s+to\s+""{0})(?<!^.*[Ee]scalate.*\s+to\s+""{0})(?<!^.*[Ee]scalation.*\s+to\s+""{0})\b.+\b(?=""{0}\.{0}.*)"; //For assign to Group Name without escalate and remedy
  ]
sb.Remove(0,sb.Length)|>ignore
for a in dataSource do
  sb.Remove(0,sb.Length)|>ignore
  for pattern in regexPattersForRemedy do
    match Regex.Matches(a.CMNTS, pattern,RegexOptions.Multiline)  with
    | x when x.Count>0->
          seq{ for b in x do
                   if b.Groups.Count>0 then
                     yield b.Groups.[0].Value}
          |>Seq.distinct
          |>Seq.iter (fun b -> b.Split([|".";","|],StringSplitOptions.RemoveEmptyEntries).[0].ToString()^","|>sb.Append|>ignore)
          if sb.Length>0 then
            sb.Remove(sb.Length - 1,1) |>ignore
            if String.IsNullOrEmpty <| a.REMEDY_GRP.Trim() then
              a.REMEDY_GRP <-sb.ToString()
    | _ ->()  
   
//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////
 
//"1.q".Split('.').[0]
 
let remedyGroup=
  match Regex.Matches(a.CMNTS, @".+""(.+)"".+$",RegexOptions.Multiline) with
  | x when x.Count>0->
        seq{ for a in x do
                 if a.Groups.Count>0 then
                   yield a.Groups.[0].Value}
        |>Seq.distinct
        |>Seq.iter (fun b ->sb.Append(b^",")|>ignore)
        if sb.Length>0 then
          sb.Remove(sb.Length - 2,1) |>ignore
          a.ESCALATE_GRP<-sb.ToString()
  | _ ->()   
if sb.Length>0 then
  a.REMEDY_GRP<-sb.ToString()
 
ObjectDumper.Write(sbsrc_ADTNLs)
 
 
let wx= "wx"
wx.IndexOf('"')
 
////////////////////////////////////////////////////////////////////
 
 
 
(*
test.IndexOfAny([|'"'|])
//test.
//let reg=Regex( @"s+\""s+\""")
let reg=Regex( @"^.+escalate to.+""")
reg.Match(test).Value
reg.Match(test).Groups.Count
reg.Match(test).Groups.[0].Value
reg.Match(test).Groups.[1].Value
ObjectDumper.Write(reg.Match(test).Groups,1)
*)
 
let test= @"Create a severity 2B Remedy ticket and refer to ROS Guide
"

let rex= Regex.Split(test,@"[Aa]ssign\s*\n*\s*to",RegexOptions.Multiline)
rex.Length

let rex= Regex.Split(rex.[0],@"""",RegexOptions.Multiline)
rex.Length
rex.[1].Trim()
match rex.[1] with
| x when x.StartWith(@"""") ->
 
//let reg2=Regex.Matches(test, @"(?<=^.+escalate.+\n*.*("")|(\sto\s+))\b.+\b(?=("")|(\sto\s+).*)",RegexOptions.Multiline)  //Wrong
//let reg2=Regex.Matches(test, @"(?<=^.+escalate.+\n*.*[""|\sto\s+])\b.+\b(?=[""|\sto\s+].*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.+escalate.+\n*.*[""|(\bto\s+)])\b.+\b(?=[""|.?].*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.+escalate.+\n*.*"")\b.+\b(?="".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*escalate.*\s+to\s+""{0})\b.+\b(?=""{0}\.)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*[Ee]scalate.*\s+to\s+""{0})\b.+\b(?=\.\s*\w+.+)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*[Ee]scalate.*\s+to\s+""{0})\b.+\b(?=\.?.+)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*[Ee]scalate.*\s+to\s+""{0})\b.+\b(?=""{0}\..*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Rr]emedy.*(?<!.*[Ee]scalate)\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+-.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Rr]emedy.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"Write(?!.*Line.*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*[Aa]ssign.*\s+to\s+""{0})(?<!^.*[Rr]emedy.*\s+to\s+""{0})(?<!^.*[Ee]scalate.*\s+to\s+""{0})\b.+\b(?=""{0}\.{0}.*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Aa]ssign.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Rr]emedy.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*[Ee]scalation.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Aa]ssign.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*""\s*)\b.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Aa]ssign.*\n*\t*.*\n*\t*.*""\s*)(?<!^.*\n*.*"".*\n*.*"".*\n*.*"".*)\b.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"((?<=^.*\n*.*[Ee]scalate.*\n*\t*.*\n*\t*.*""\s*)|(?<=^.*\n*.*[Aa]ssign.*\n*\t*.*\n*\t*.*""\s*)|(?<=^.*\n*.*[Rr]emedy.*\n*\t*.*\n*\t*.*""\s*))\b.+\b(?=\s*"".*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=^.*\n*.*[Aa]ssign.*\n*\t*.*\n*\t*.*""\s*)\b.+\b(?=\s*"".*\n*.*)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+\n*\.*)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+\n*\.*\n*\.*))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<="")\s*\w.+\w\s*(?="")",RegexOptions.Multiline)
let reg2=Regex.Matches(test, @"(?<=[Aa]ssign.*\n*\.*\n*.*)(?<!"")""\s*\w.+\w\s*""",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"(?<=[Aa]ssign\s*\w*\s*\n*\s*\w*\s*\n*\s*\w*\s*"")\s*\d*\w*\s*\-*\&*\s*s*(?="")",RegexOptions.Multiline)
let reg2=Regex.Matches(test, @"(?<=[Aa]ssign\s*\w*\s*\n*\s*\w*\s*\n*\s*\w*\s*"")[a-zA-Z0-9\-\&\s]*(?="")",RegexOptions.Multiline)
let reg2=Regex.Matches(test, @"(?<=[Aa]ssign[\s\w\n]*"")[a-zA-Z0-9\-\&\s]*(?="")",RegexOptions.Multiline)
let reg2=Regex.Matches(test, @"(?<=[Aa]ssign[\s\w\n]*"")[\s\w\d\-\&]*(?="")",RegexOptions.Multiline)


let regexPattersForGRP=
  [
  @"((?<=[Ee]scalat[\s\w\n\:\(\)\-\&\d\,]*[""\']+)|(?<=[Rr]emedy[\s\w\n\:\(\)\-\&\d\,]*[""\']+)|(?<=[Aa]ssign[\s\w\n\:\(\)\-\&\d\,]*[""\']+))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(\)\-\&\d\,]*[""\']+)[\s\w\d\-\&]+(?=[""\']+)";
  //@"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup|[Qq]ueue)[\s\n\:]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup|[Qq]ueue)[\s\n\:]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup|[Qq]ueue)[\s\n\:]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=[\.\,\<\n\(\)$]+)"
  @"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=[\.\,\<\n\(\)]+)";
  @"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=$)"
  ]
let reg2=Regex.Matches(test, @"((?<=[Ee]scalat[\s\w\n\:\(\)\-\&\d\,]*[""\']+)|(?<=[Rr]emedy[\s\w\n\:\(\)\-\&\d\,]*[""\']+)|(?<=[Aa]ssign[\s\w\n\:\(\)\-\&\d\,]*[""\']+))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(\)\-\&\d\,]*[""\']+)[\s\w\d\-\&]+(?=[""\']+)",RegexOptions.Multiline) //Right
let reg2=Regex.Matches(test, @"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=[\.\,\<\n\(\)]+)",RegexOptions.Multiline)
let reg2=Regex.Matches(test, @"((?<=[Ee]scalate[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Rr]emedy[\s\w\n\d\(\)]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*)|(?<=[Aa]ssign[\s\w\n]*([Tt]o|[Gg]roup\s*\:)[\s\n\:]*[""]*))(?<![Rr]emedy[\s\n]+[Aa]sset[\s\n]+[Nn]ame[\s\n\:\(]*)[\s\w\d\-\&]+(?=$)",RegexOptions.Multiline)


let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=').+(?=')",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=to\s*)(?<!"")(?<!').+(?!')(?!"")(?(\.)(?=\.)|(?(\s*\n)(?=\s*\n)|(?=\s*$)))",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=to\s*)(?<!"")(?<!').+(?!')(?!"")(?=\.)",RegexOptions.Multiline)
let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=to\s*)(?<!"")(?<!')\s*\w.+\w\s*(?!')(?!"")(?=\n)",RegexOptions.Multiline) //Wrong ???
//let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=to\s*)(?<!"")(?<!').+(?!')(?!"")(?=\n)",RegexOptions.Multiline)
//let reg2=Regex.Matches(test, @"((?<=[Ee]scalate.+)|(?<=[Rr]emedy.+)|(?<=[Aa]ssign.+))(?<![Rr]emedy\s+[Aa]sset\s+[Nn]ame.+)(?<=to\s*)(?<!"")(?<!').+(?!')(?!"")(?!(\.))(?=\n)",RegexOptions.Multiline) //Wrong ???
 
let sstring ="wx"
 
reg2.Count
reg2.[0].Groups.Count
reg2.[0].Captures.Count
reg2.[0].Captures.[0]
reg2.[0].Captures.[1]
reg2.[0].Groups.[0].Value
reg2.[0].Groups.[1].Value
reg2.[0].Groups.[2].Value
reg2.[1].Groups.[0].Value
reg2.[1].Groups.[1].Value
reg2.[2].Groups.[0].Value
reg2.[2].Groups.[1].Value
reg2.[2].Groups.[0].Value
 
reg2.[1].Groups.Count
 
ObjectDumper.Write(reg2,1)
 
 
 

(*
worksheet1.Range("A1").Value2<-"Subsource ID"
for i=0 to sbsrc_ADTNLs.Count-1 do
  worksheet1.Range("A"^(string (i+2))).Value2<- sbsrc_ADTNLs.[i].SBSRC_ID
 
 
worksheet1.Range("B1").Value2<-"Subsource Name"
for i=0 to sbsrc_ADTNLs.Count-1 do
  worksheet1.Range("B"^(string (i+2))).Value2<- sbsrc_ADTNLs.[i].SBSRC_NM
 
 
worksheet1.Range("C1").Value2<-"Comments"
for i=0 to sbsrc_ADTNLs.Count-1 do
  worksheet1.Range("C"^(string (i+2))).Value2<- sbsrc_ADTNLs.[i].CMNTS
 
worksheet1.Range("D1").Value2<-"Remedy Group or Escalation"
for i=0 to sbsrc_ADTNLs.Count-1 do
  worksheet1.Range("D"^(string (i+2))).Value2<-sbsrc_ADTNLs.[i].REMEDY_GRP
 
*) 
//-------------------------------------------------------

let dataSource=
  dataSource
  |>pseq
  |>toNetList
  
worksheet1.Range("A1").Value2<-"Subsource ID"
for i=0 to dataSource.Count-1 do
  worksheet1.Range("A"^(string (i+2))).Value2<- dataSource.[i].SBSRC_ID
 
 
worksheet1.Range("B1").Value2<-"Subsource Name"
for i=0 to dataSource.Count-1 do
  worksheet1.Range("B"^(string (i+2))).Value2<- dataSource.[i].SBSRC_NM
  
worksheet1.Range("D1").Value2<-"Escalation or Remedy Group"
for i=0 to dataSource.Count-1 do
  worksheet1.Range("D"^(string (i+2))).Value2<- dataSource.[i].GRP
 
 
worksheet2.Range("C1").Value2<-"Comments"
for i=0 to dataSource.Count-1 do
  worksheet2.Range("C"^(string (i+2))).Value2<- dataSource.[i].CMNTS
 
 

 
 
 
 
worksheet1.Range("E1").Value2<-"Escalation Group"
for i=0 to dataSource.Count-1 do
  worksheet1.Range("E"^(string (i+2))).Value2<-dataSource.[i].ESCALATE_GRP
 
worksheet1.Range("F1").Value2<-"Remedy Group"
for i=0 to dataSource.Count-1 do
  worksheet1.Range("F"^(string (i+2))).Value2<- dataSource.[i].REMEDY_GRP
 
 

 
//------------------------------------------------------------------
//Escalation - Critical action   
 
worksheet2.Range("A1").Value2<-"Subsource ID"
for i=0 to dataSource.Count-1 do
  worksheet2.Range("A"^(string (i+2))).Value2<- dataSource.[i].SBSRC_ID
 
 
worksheet2.Range("B1").Value2<-"Subsource Name"
for i=0 to dataSource.Count-1 do
  worksheet2.Range("B"^(string (i+2))).Value2<- dataSource.[i].SBSRC_NM
 
 
worksheet2.Range("C1").Value2<-"Comments"
for i=0 to dataSource.Count-1 do
  worksheet2.Range("C"^(string (i+2))).Value2<- dataSource.[i].CMNTS
 
worksheet2.Range("D1").Value2<-"Escalation Group"
for i=0 to dataSource.Count-1 do
  worksheet2.Range("D"^(string (i+2))).Value2<-dataSource.[i].ESCALATE_GRP
 
worksheet2.Range("E1").Value2<-"Remedy Group"
for i=0 to dataSource.Count-1 do
  worksheet2.Range("E"^(string (i+2))).Value2<- dataSource.[i].REMEDY_GRP



(*
Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx
Regular Expression Language Elements
http://msdn.microsoft.com/en-us/library/az24scfc.aspx

如何使用正则表达式搜索
http://technet.microsoft.com/zh-cn/library/ms174214.aspx

正则表达式语言元素
http://msdn.microsoft.com/zh-cn/library/az24scfc.aspx


常用正则表达式大全！（例如：匹配中文、匹配html） 
http://www.cnblogs.com/guiliangfeng/archive/2009/04/13/1434696.html
*)