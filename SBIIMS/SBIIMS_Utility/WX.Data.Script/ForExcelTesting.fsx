
#I @"F:\ISO\Report\Excel\excellibrary_20110730_CodeGoogle_Response way"
#r "ExcelLibrary"
open ExcelLibrary.SpreadSheet
open System
//===============================================================

let file = @"d:/Temp/newdoc.xls"
let workbook = new Workbook()
let worksheet = new Worksheet("First Sheet")
//worksheet.Cells.[0, 1] = new Cell(1)
//worksheet.Cells.[2, 0] = new Cell(9999999)
//worksheet.Cells.[3, 3] = new Cell((decimal)3.45)
//worksheet.Cells.[2, 2] = new Cell("Text string")
//worksheet.Cells.[2, 4] = new Cell("Second string")
//worksheet.Cells.[4, 0] = new Cell(32764.5, "#,##0.00")
//worksheet.Cells.[5, 1] = new Cell(DateTime.Now, @"YYYY\-MM\-DD")
//worksheet.Cells.ColumnWidth.[0us, 1us] = uint16 3000
workbook.Worksheets.Add(worksheet)
workbook.Save(file)

//===============================================================
#I @"F:\ISO\Report\Excel\Export Excel without be installed\e表ebiao\ebiao\bin"
#r "ebexcel"

open ebexcel
match new ExcelFile() with
| x ->
    match x.Worksheets.Add("wx") with
    | y ->
        y.Cells.[0,0]<-new Cell("wx")
    x.SaveXls (@"d:/Temp/O.xls")

match new Workbook() with
| x ->
    match new Worksheet("wx") with
    | y ->
        y.Cells.[0,0]<-new Cell("wx")
        x.Worksheets.Add y
        x.Save (@"d:/Temp/Old.xls")

