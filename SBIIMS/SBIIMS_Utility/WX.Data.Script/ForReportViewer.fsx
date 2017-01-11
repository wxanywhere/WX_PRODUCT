//----------------------------------------------------------------
#I @"C:\WINDOWS\assembly\GAC\"
#r "Microsoft.ReportViewer.Common"  //注意直接引用GAC组件时，不加*.dll后缀
#r "Microsoft.ReportViewer.WinForms"
#r "System.Windows.Forms"
#r "WindowsFormsIntegration"
open System
open System.Windows.Forms
open Microsoft.Reporting.WinForms
//----------------------------------------------------------------
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data
//----------------------------------------------------------------
let form =new Form()
form.TopMost<-true
form.Show()
form.Close()
let report=new ReportViewer()
report.Dock<-DockStyle.Fill
form.Controls.Clear()
form.Controls.Add report

report.ProcessingMode<-ProcessingMode.Local
report.LocalReport.ReportPath<- @"D:\Workspace\SBIIMS\SBIIMS_Base\ViewBase\WX.Data.View.Resources.ReportTemplate\QDBB\R_QDBB_DesignTemplate.rdlc"

let paras=
  [|
  new ReportParameter("RP_DateTimeStart",string DateTime.Now)
  new ReportParameter("RP_DateTimeEnd",string DateTime.Now)
  |]
report.LocalReport.SetParameters paras
let node=report.Controls.Find("ReportTitle",true)
report.Controls.[0].Controls.[0].Controls.[2].Controls.[1].Name

let local=report.LocalReport

report.LocalReport.DataSources.Add(new ReportDataSource("DS",""))
report.FindForm().Controls.[0].Controls.[0].Controls.[0].Controls.[2].Controls.[1]
report.LocalReport.ListRenderingExtensions()
report.RefreshReport()


ObjectDumper.Write (node,1)

//==============================================
#r "System.Xml.Linq"
open System.Xml
open System.Xml.Linq
open System.Xml.XPath
//----------------------------------------------------------
let file= @"D:\Workspace\SBIIMS\SBIIMS_Base\ViewBase\WX.Data.View.Resources.ReportTemplate\QDBB\R_QDBB_Template.rdlc"
let xe=XElement.Load(file)
xe.XPathSelectElement( @"./Fields")
xe.XPathSelectElement( @"/Report")

let xml= @""
let x=XElement.Load ("D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.Script\Sample.xml")
x.se