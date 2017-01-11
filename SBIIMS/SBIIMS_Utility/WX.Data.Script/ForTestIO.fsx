

open System.IO
open System.Text
let filePath= @"D:\Common Workspace\Received\BasicResearch\BasicResearch\3D.fsx"
let  readFile =
     async { use  fs = new  FileStream(filePath,FileMode.Open, FileAccess.Read, FileShare.Read)
             let  data = Array.create (int fs.Length) 0uy
             let!  bytesRead = fs. AsyncRead(data, 0, data.Length)
             let enc=ASCIIEncoding()
             return enc.GetString(data)
      }

Async.RunSynchronously readFile

let readFile1=
  use rs=new StreamReader(filePath,Encoding.Default)
  rs.ReadToEnd()
readFile1

let f=FileInfo( @"\dfdsfdsf\\\;kjl///dfdf")
f.Directory


///////////////////////////////////////////////////////////////////////////////

let WriteFile (sourceText:string) filePath=
  let fileInfo=FileInfo(filePath)
  match fileInfo.Directory with
  | x when not  x.Exists ->x.Create()
  | _ ->()
  (* Right, Why???
  let bytes=System.Text.Encoding.ASCII.GetBytes(sourceText)
  //or
  let enc=System.Text.ASCIIEncoding()
  let bytes=enc.GetBytes(sourceText)
  *)
  let bytes=ASCIIEncoding.ASCII.GetBytes(sourceText)
  let task=
    async{
      use fs=fileInfo.Create()
      do! fs.AsyncWrite(bytes)
    }
  Async.RunSynchronously task

let filePath= @"c:/wx/wx.cs"
WriteFile readFile1 filePath

let WriteFile1 (sourceText:string) filePath=
  let fileInfo=FileInfo(filePath)
  match fileInfo.Directory with
  | x when not  x.Exists ->x.Create()
  | _ ->()
  //if fileInfo.Exists then fileInfo.Delete(), Right
  (* Right, Why???
  let bytes=System.Text.Encoding.ASCII.GetBytes(sourceText)
  //or
  let enc=System.Text.ASCIIEncoding()
  let bytes=enc.GetBytes(sourceText)
  *)
  let bytes=ASCIIEncoding.ASCII.GetBytes(sourceText)
  async{
    use fs=fileInfo.Create()
    do! fs.AsyncWrite(bytes)
  }
let filePath= @"c:/wx/wx.cs"
WriteFile1 readFile1 filePath
|>Async.RunSynchronously 


let WriteFile2 (sourceText:string) filePath=
  let fileInfo=FileInfo(filePath)
  match fileInfo.Directory with
  | x when not  x.Exists ->x.Create()
  | _ ->()
  use writer= fileInfo.CreateText()
  writer.Write(sourceText)
let filePath= @"c:/wx/wx.cs"
WriteFile2 readFile1 filePath


let WriteAndReadFile (sourceText:string) filePath=
  let fileInfo=FileInfo(filePath)
  match fileInfo.Directory with
  | x when not  x.Exists ->x.Create()
  | _ ->()
  (* Right, Why???
  let bytes=System.Text.Encoding.ASCII.GetBytes(sourceText)
  //or
  let enc=System.Text.ASCIIEncoding()
  let bytes=enc.GetBytes(sourceText)
  *)
  let bytes=ASCIIEncoding.ASCII.GetBytes(sourceText)
  let task=
    async{
      use fs=fileInfo.Create()
      do! fs.AsyncWrite(bytes)
    }
  Async.RunSynchronously task

let filePath= @"c:/wx/wx.cs"
WriteFile readFile1 filePath


let directoryInfo=DirectoryInfo( @"c:/wx/wx.cs")
let fileInfo=FileInfo(@"c:/wx/wx.cs")
match fileInfo.Directory with
| x when not  x.Exists ->x.Create()
| _ ->()
let enc=System.Text.Encoding.ASCII
let bytes=enc.GetBytes(readFile1)
let fileStream= fileInfo.Create()
fileStream.Close()

Write(bytes,0,bytes.Length)


(*
let streamWriter=StreamWriter(

fileInfo.Directory.Exists
directoryInfo.Exists
directoryInfo.Create()
let WriteFile (sourceText:string) (filePath:string) =
  let  flag = false
  let directoryInfo=DirectoryInfo(filePath)
  if not directoryInfo.Exists then
    Directory.CreateDirectory(directoryInfo

			if (!Directory.Exists(path)) {
				Directory.CreateDirectory(path);
			}
            
			path += fileNamePath;
            if (File.Exists(path)) {
                File.Delete(path);
            }

			//
			//Builder the channels for read or write file
			//,it similarity with [saveFileDialog.ShowDialog()],
			//it has sameness pupose of them
			//
			using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
				using (StreamWriter sw = new StreamWriter(fs, Encoding.Default)) {
					sw.Write(sourceText);
					flag = true;
				}
			}
			return flag;
		}

*)
///////////////////////////////////////////////////////////////////////////////
(*
open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.CommonExtensions
let ProcessImageAsync(i) =
  async { use inStream = File.OpenRead(sprintf "Image%d.tmp" i)
  let! pixels = inStream.ReadAsync(numPixels)
  let pixels' = TransformImage(pixels,i)
  use outStream = File.OpenWrite(sprintf "Image%d.done" i)
  do! outStream.WriteAsync(pixels') }
let ProcessImagesAsync() =
  printfn "ProcessImagesAsync...";
  let tasks = [ for i in 1 .. numImages -> ProcessImageAsync(i) ]
  Async.Run (Async.Parallel tasks) |> ignore
  printfn "ProcessImagesAsync finished!";
*)