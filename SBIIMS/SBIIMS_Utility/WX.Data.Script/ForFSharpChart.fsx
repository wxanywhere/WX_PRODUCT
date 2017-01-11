// Learn more about F# at http://fsharp.net
(*
http://code.msdn.microsoft.com/FSharpChart-1aa52c2c
http://blogs.msdn.com/b/dsyme/archive/2011/06/23/new-version-of-fsharpchart-now-available.aspx
*)

#I @"F:\ISO\Chart\FSharpChart-0.5\fsharpchart\bin\Debug" 
#r "MSDN.FSharp.Charting.dll"
#r "System.Windows.Forms.DataVisualization.dll"

open MSDN.FSharp.Charting
open System
open System.Drawing
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting


[("A",1); ("B",2); ("C",3)]
|> FSharpChart.Pie
|> FSharpChart.WithArea.Area3DStyle(Enable3D = true, Inclination = 60)
|> FSharpChart.WithCreate

[ for f in 1.0 .. 0.1 .. 10.0 -> sin (f * 2.0) + cos f ]
|> FSharpChart.Line
|> FSharpChart.WithCreate
|> FSharpChart.CopyToClipboard

let rnd=new System.Random()
FSharpChart.BoxPlot
     ( [ "Eggs", [| for i in 0 .. 20 -> float (rnd.Next 20) |]
         "Bacon", [| for i in 0 .. 20 -> float (rnd.Next 15 + 2) |]
         "Sausage", [| for i in 0 .. 20 -> float (rnd.Next 5 + 5) |]
         "Beans", [| for i in 0 .. 20 -> float (rnd.Next 10 + 3) |]
         "Mushroom", [| for i in 0 .. 20 -> float (rnd.Next 15 + 5) |] ],
         Name = "Breakfast Food BoxPlot")
|> FSharpChart.WithMargin(1.0f, 5.0f, 1.0f, 1.0f) 
|> FSharpChart.WithSeries.Style(Color = Color.SeaGreen)
|> FSharpChart.WithLegend(InsideArea = false, Font = new Font("Arial", 8.0f),
     Alignment = StringAlignment.Center, Docking = Docking.Top)
|>FSharpChart.WithCreate

let stocks = Data.brownianBarsAsList 1000 20 |> List.map snd
let chart2 = 
  FSharpChart.Candlestick(stocks)
  |> FSharpChart.Create
// Update data on the chart
chart2.SetData [ for h, l, o, c in stocks -> h - 9.0, l - 9.0, o - 9.0, c - 9.0 ]
chart2.SetData [ for h, l, o, c in stocks -> 12.0 - l, 12.0 - h, 12.0 - c, 12.0 - o ]



let chart=new FSharptChart()
chart.Margin <- (2.0f, 12.0f, 2.0f, 2.0f)
chart.Title <- Helper.CreateTitle("Chart Sin/Cosine", Font = new Font("Arial", 12.0f, FontStyle.Bold))
chart.Legend <- Helper.CreateLegend(InsideArea = false, Font = new Font("Arial", 8.0f),
                                     Alignment = StringAlignment.Center, Docking = Docking.Top)

let series = new MultiValue()  
series.BindSeries(chartControl.ChartSeries)
series.SetData([ "Cereal", [| for i in 0 .. 20 -> float (rnd.Next 20) |]
                  "Yogurt", [| for i in 0 .. 20 -> float (rnd.Next 15 + 2) |]
                  "Toast", [| for i in 0 .. 20 -> float (rnd.Next 15 + 2) |]
                  "Fruit", [| for i in 0 .. 20 -> float (rnd.Next 10 + 5) |] ],
                  chart.ChartBinder)
