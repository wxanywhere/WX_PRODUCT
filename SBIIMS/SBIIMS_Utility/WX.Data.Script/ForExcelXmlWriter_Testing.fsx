

#I @"F:\ISO\Report\Excel\ExcelXmlWriter"
#r "Office" 
#r "CarlosAg.ExcelXmlWriter.dll"  //注意直接引用GAC组件时，不加*.dll后缀
open CarlosAg.ExcelXmlWriter

//===================================================


let book=new Workbook()
let sheet=book.Worksheets.Add("wx")
let row=sheet.Table.Rows.Add()
row.Cells.Add ("Hello")
book.ExcelWorkbook.ProtectStructure<-true
book.Save(@"D:\Temp\WX2012-02-11.xls")



(*
Sample Code:
Private void ExportDatasettoExcel(DataSet dsExcel)
{
            //Add a workbook
	    string fileName = @"C:\Sample.xls";
            CarlosAg.ExcelXmlWriter.Workbook book = new CarlosAg.ExcelXmlWriter.Workbook();

            // Specify which Sheet should be opened and the size of window by default
            book.ExcelWorkbook.ActiveSheetIndex = 1;
            book.ExcelWorkbook.WindowTopX = 100;
            book.ExcelWorkbook.WindowTopY = 200;
            book.ExcelWorkbook.WindowHeight = 7000;
            book.ExcelWorkbook.WindowWidth = 8000;

            // Some optional properties of the Document
            book.Properties.Author = "Murali";
            book.Properties.Title = "Excel Export";
            book.Properties.Created = DateTime.Now;

	    //Add styles to the workbook 
            WorksheetStyle s31 =book.Styles.Add("s31");
            s31.Font.FontName = "Tahoma";
            s31.Font.Size = 8;
            s31.Font.Color = "#000000";
            s31.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            s31.Alignment.Vertical = StyleVerticalAlignment.Center;
            s31.Alignment.WrapText = true;
            s31.Borders.Add(StylePosition.Bottom, LineStyleOption.Continuous, 1);
            s31.Borders.Add(StylePosition.Left, LineStyleOption.Continuous, 1);
            s31.Borders.Add(StylePosition.Right, LineStyleOption.Continuous, 1);
            s31.Borders.Add(StylePosition.Top, LineStyleOption.Continuous, 1);
            s31.NumberFormat = "@";
            
            // Add styles for header of the Workbook
            WorksheetStyle style = book.Styles.Add("HeaderStyle");
            style.Font.FontName = "Tahoma";
            style.Font.Size = 12;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "White";
            style.Interior.Color = "Blue";
            style.Interior.Pattern = StyleInteriorPattern.DiagCross;

            
            // Add a Worksheet with some data
            Worksheet sheet = book.Worksheets.Add("Sample Data");
            // we can optionally set some column settings
            sheet.Table.Columns.Add(new WorksheetColumn(100));
            sheet.Table.Columns.Add(new WorksheetColumn(100));
            sheet.Table.Columns.Add(new WorksheetColumn(250));
            
            //Add row with some properties
            WorksheetRow row = sheet.Table.Rows.Add();
            row.Index = 0;
            row.Height = 26;
            row.AutoFitHeight = false;
           
	    //Add header text for the columns	
            WorksheetCell wcHeader = new WorksheetCell("Column 1", "HeaderStyle");
            row.Cells.Add(wcHeader);
            wcHeader = new WorksheetCell("Column 2", "HeaderStyle");
            row.Cells.Add(wcHeader);
            wcHeader = new WorksheetCell("Column 3", "HeaderStyle");
            row.Cells.Add(wcHeader);           
                       
	    //Loop through each table in dataset
            for (int i = 0; i < dsExcel.Tables.Count; i++)
            {
		//Loop through each row in datatable                
                    foreach (DataRow dtrrow in dsExcel.Tables[i].Rows)
                    {                   
			//Add row to the excel sheet
			row = sheet.Table.Rows.Add();
                        row.Height = 30;
                        row.AutoFitHeight = false;
			//Loop through each column
                        foreach (DataColumn col in dsExcel.Tables[i].Columns)
                        {
                            
                                WorksheetCell wc = new WorksheetCell(dtrrow[col.ColumnName].ToString(), DataType.String, "s31");
                                row.Cells.Add(wc);                            
                        }
                    }                
            }
	    //Save the work book
            book.Save(fileName);
}
//------------------------------------------------------------------------------------
namespace TestSuite {
    using System;
    using System.Diagnostics;
    using CarlosAg.ExcelXmlWriter;
    class TestApp {
        static void Main() {
            string filename = @"e:\test.xls";
            Workbook book = new Workbook();

            // Specify which Sheet should be opened and the size of window by default
            book.ExcelWorkbook.ActiveSheetIndex = 1;
            book.ExcelWorkbook.WindowTopX = 100;
            book.ExcelWorkbook.WindowTopY  = 200;
            book.ExcelWorkbook.WindowHeight = 7000;
            book.ExcelWorkbook.WindowWidth = 8000;

            // Some optional properties of the Document
            book.Properties.Author="CarlosAg";
            book.Properties.Title="My Document";
            book.Properties.Created=DateTime.Now;

            // Add some styles to the Workbook
            WorksheetStyle style = book.Styles.Add("HeaderStyle");
            style.Font.FontName = "Tahoma";
            style.Font.Size = 14;
            style.Font.Bold = true;
            style.Alignment.Horizontal = StyleHorizontalAlignment.Center;
            style.Font.Color = "White";
            style.Interior.Color = "Blue";
            style.Interior.Pattern = StyleInteriorPattern.DiagCross;

            // Create the Default Style to use for everyone
            style = book.Styles.Add("Default");
            style.Font.FontName = "Tahoma";
            style.Font.Size = 10;

            // Add a Worksheet with some data
            Worksheet sheet = book.Worksheets.Add("Some Data");

            // we can optionally set some column settings
            sheet.Table.Columns.Add(new WorksheetColumn(150));
            sheet.Table.Columns.Add(new WorksheetColumn(100));

            WorksheetRow row = sheet.Table.Rows.Add();
            row.Cells.Add(new WorksheetCell("Header 1", "HeaderStyle"));
            row.Cells.Add(new WorksheetCell("Header 2", "HeaderStyle"));
            WorksheetCell cell = row.Cells.Add("Header 3");
            cell.MergeAcross = 1;            // Merge two cells together
            cell.StyleID = "HeaderStyle";

            row = sheet.Table.Rows.Add();
            // Skip one row, and add some text
            row.Index = 3;
            row.Cells.Add("Data");
            row.Cells.Add("Data 1");
            row.Cells.Add("Data 2");
            row.Cells.Add("Data 3");

            // Generate 30 rows
            for (int i=0; i<30; i++) {
                row = sheet.Table.Rows.Add();
                row.Cells.Add("Row " + i.ToString());
                row.Cells.Add(new WorksheetCell(i.ToString(), DataType.Number));
            }

            // Add a Hyperlink
            row = sheet.Table.Rows.Add();
            cell = row.Cells.Add();
            cell.Data.Text = "Carlos Aguilar Mares";
            cell.HRef = "http://www.carlosag.net";
            // Add a Formula for the above 30 rows
            cell = row.Cells.Add();
            cell.Formula = "=SUM(R[-30]C:R[-1]C)";

            // Save the file and open it
            book.Save(filename);
            Process.Start(filename);
        }
    }
}
*)