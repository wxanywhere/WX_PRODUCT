// Copyright (c) Microsoft Corporation 2005-2008. 
// This sample code is provided "as is" without warranty of any kind.  
// We disclaim all warranties, either express or implied, including the  
// warranties of merchantability and fitness for a particular purpose.  
 
#I @"C:\WINDOWS\assembly\GAC\"
#r "Office" 
#r "Microsoft.Office.Interop.Excel"  //注意直接引用GAC组件时，不加*.dll后缀
open Microsoft.Office.Interop.Excel 
 
 
open System 
open System.Runtime.InteropServices // For COMException  
 
// Create new Excel.Application 
let app = new ApplicationClass(Visible = true)  

 
// Get the workbooks collection 
let workbooks = app.Workbooks 
 
// Add a new workbook 
let workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet)  
 
// Get the worksheets collection 
let sheets = workbook.Worksheets  
 
let worksheet = (sheets.[box 1] :?> _Worksheet)  
 
Console.WriteLine ("Setting the value for cell") 
 
// This puts the value 5 to the cell G1  
worksheet.Range("G1").Value2 <- 5 
 
// This sends a single dimension array to Excel  
worksheet.Range("A1", "E1").Value2 <- [| for i in 0 .. 4 -> i * i |] 
worksheet.Range("A2", "E2").Value2 <- [| for i in 0 .. 4 -> sin (float i) |] 
 
// This sends a two dimension array to Excel  
let array3 = Array2D.init 4 5 (fun i j -> i*10 + j)  
worksheet.Range("A3", "E6").Value2 <- array3 
 
// This reads a two dimension array from Excel  
let array4 = (worksheet.Range("A3", "E6").Value2 :?> obj[,])  
 
printf "Low: %d\n" (array4.GetLowerBound(0)) 
for i=array4.GetLowerBound(0) to array4.GetUpperBound(0) do 
    for j=array4.GetLowerBound(1) to array4.GetUpperBound(1) do 
        printf "Compare %d, %d " i j; 
        if int (Array2D.get array4 i j :?> float) <> Array2D.get array3 (i-array4.GetLowerBound(0))  (j-array4.GetLowerBound(1)) then 
            Console.WriteLine ("ERROR: Comparison FAILED!") 
 
//  This fills two dimension array with points for two curves and sends it to Excel  
let range5 = worksheet.Range("A8", "J9")  
let array5 =  
    Array2D.init 2 10 
        (fun i j ->  
            let arg = (Math.PI / 10.0) * float j  
            if i = 0 then Math.Sin(arg) else Math.Cos(arg)) 
range5.Value2 <- array5 
 
// The following code would draw the chart if it wasn't for the Excel/CLR bug further below  
range5.Select() |> ignore 
 
let chartobjects = (worksheet.ChartObjects() :?> ChartObjects)  
let chartobject = chartobjects.Add(10.0, 100.0, 450.0, 250.0)  
 
// Make the chart 
chartobject.Chart.ChartWizard(Source = range5, 
                              Gallery = XlChartType .xl3DColumn, 
                              PlotBy = XlRowCol.xlRows, 
                              HasLegend = true, 
                              Title = "Sample Chart", 
                              CategoryTitle = "Sample Category Type", 
                              ValueTitle = "Sample Value Type") 
chartobject.Visible <- false 
chartobject.Visible <- true 
chartobject.Parent |> ignore 
chartobject.Chart.GetType() |> ignore 
 
 
#if COMPILED 
System.Threading.Thread.Sleep(1000) 
 
//  If user interacted with Excel it will not close when the app object is destroyed, so we close it explicitly  
let _ =  
  try  
    workbook.Saved <- true 
    app.UserControl <- false 
    app.Quit() 
  with e -> Console.WriteLine ("User closed Excel manually, so we don't have to do that") 
 
let _ = Console.WriteLine ("Sample successfully finished!") 
#endif 
