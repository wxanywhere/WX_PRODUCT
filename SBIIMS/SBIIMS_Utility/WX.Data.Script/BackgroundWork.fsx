
open System
open System.Drawing
open System.ComponentModel
open System.Windows.Forms

type Worker() as x =
    inherit BackgroundWorker(WorkerReportsProgress = true)
    
    do x.DoWork.Add 
        (fun _ ->
            let mutable timesThrough = 0
            while timesThrough < 10 do
                Threading.Thread.Sleep(500)
                timesThrough <- timesThrough + 1
                x.ReportProgress(timesThrough))

type WorkerForm(worker : BackgroundWorker) as x =
    inherit Form(Text = "BackgroundWorker How-To")
    
    let progressText = sprintf "# of times %i"
    let _workButton = new Button(Text = "Work!", Location = new Point(10, 10)) :> Control
    let _timesText = new TextBox(Text = progressText 0, 
                                 Location = new Point(10, 50),
                                 ReadOnly = true) :> Control
    do x.Controls.AddRange([|_workButton; _timesText|])
    do _workButton.Click.Add (fun _ -> worker.RunWorkerAsync())
    do worker.ProgressChanged.Add (fun e -> _timesText.Text <- progressText e.ProgressPercentage)
    do worker.RunWorkerCompleted.Add (fun _ -> _timesText.Text <- "Done!")

let frm = new WorkerForm(new Worker())
frm.Show()
//frm.ShowDialog()

//[<STAThread>]
//let runForm() =
//    let frm = new WorkerForm(new Worker())
//    Application.Run(frm)
//
//do runForm()
