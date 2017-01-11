namespace WX.Data.CodeAutomation
(*Right, 已移植到 WX.Data.FModule
open System.IO
open System.Text
open FSharp.Collections.ParallelSeq
open WX.Data

type FileHelper=
  static member ReadFile(filePath:string)=
    try
      match FileInfo(filePath) with
      | x when not x.Exists ->failwith "The file is not exsit!"
      | _ ->()
      async { 
        use! rs=File.AsyncOpenText filePath
        return! rs.AsyncReadToEnd() 
      }
      |>Async.RunSynchronously
    with
    | e ->reraise()

  static member WriteFile (sourceText:string) (filePath:string)=
      let fileInfo=FileInfo(filePath)
      match fileInfo.Directory with
      | x when not  x.Exists ->x.Create()
      | _ ->()
      let task=
        async{
          let buffer=Encoding.UTF8.GetBytes sourceText
          use fs=fileInfo.Create()
          do! fs.AsyncWrite(buffer)
        }
      Async.RunSynchronously task

  static member WriteFileCreateOnly (sourceText:string) (filePath:string)=
      let fileInfo=FileInfo(filePath)
      match fileInfo with
      | x when not  x.Exists ->
          if not x.Directory.Exists then
            x.Directory.Create()
          let task=
            async{
              let buffer=Encoding.UTF8.GetBytes sourceText
              use fs=fileInfo.Create()
              do! fs.AsyncWrite(buffer)
            }
          Async.RunSynchronously task
      | _ ->()

  static member PWriteFileCreateOnly (fileInfos:seq<string*string>)=
      fileInfos
      |>PSeq.map (fun (content,filePath)->
          let fileInfo=FileInfo(filePath)
          match fileInfo with
          | x when not  x.Exists ->
              if not x.Directory.Exists then
                x.Directory.Create()
              async{
                let buffer=Encoding.UTF8.GetBytes(content)
                use fs=fileInfo.Create()
                do! fs.AsyncWrite(buffer)
              }
          | _ ->async{()})
      |>Async.Parallel 
      |>Async.Ignore
      |>Async.Start 


  static member ModifyFile (modifiedText:string) (filePath:string)=
    try
      match FileInfo(filePath) with
      | x when not x.Exists ->failwith "The file is not exsit!"
      | _ ->()
      let task=
        async{
          let buffer=Encoding.UTF8.GetBytes modifiedText
          use! fs=File.AsyncOpenWrite(filePath)  //use fs=new FileStream(filePath,FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)
          do! fs.AsyncWrite(buffer)
        }
      Async.RunSynchronously task
    with
    | _ ->reraise()
*)


(* 会出现乱码
  static member ReadFile(filePath:string)=
    try
      match FileInfo(filePath) with
      | x when not x.Exists ->failwith "The file is not exsit!"
      | _ ->()
      async { use  fs = new  FileStream(filePath,FileMode.Open, FileAccess.Read, FileShare.Read)
              let  bytes = Array.create (int fs.Length) 0uy
              let!  bytesRead = fs. AsyncRead(bytes) //fs. AsyncRead(data, 0, data.Length)
              return bytes|>ASCIIEncoding.Unicode.GetString //ASCIIEncoding.ASCII.GetString
      }
      |>Async.RunSynchronously
    with
    | e ->reraise()

  static member WriteFile (sourceText:string) (filePath:string)=
      let fileInfo=FileInfo(filePath)
      match fileInfo.Directory with
      | x when not  x.Exists ->x.Create()
      | _ ->()
      //let bytes=ASCIIEncoding.ASCII.GetBytes(sourceText)
      let bytes=ASCIIEncoding.Unicode.GetBytes sourceText
      let task=
        async{
          use fs=fileInfo.Create()
          do! fs.AsyncWrite(bytes)
        }
      Async.RunSynchronously task

  static member WriteFileCreateOnly (sourceText:string) (filePath:string)=
      let fileInfo=FileInfo(filePath)
      match fileInfo with
      | x when not  x.Exists ->
          if not x.Directory.Exists then
            x.Directory.Create()
          //let bytes=ASCIIEncoding.ASCII.GetBytes(sourceText)
          let bytes=ASCIIEncoding.Unicode.GetBytes sourceText
          let task=
            async{
              use fs=fileInfo.Create()
              do! fs.AsyncWrite(bytes)
            }
          Async.RunSynchronously task
      | _ ->()

  //http://blogs.msdn.com/dsyme/archive/2010/01/10/async-and-parallel-design-patterns-in-f-reporting-progress-with-events-plus-twitter-sample.aspx
  //static member WriteFileCreateOnly (fileInfos:(string*string) seq)= //Right backup
  static member PWriteFileCreateOnly (fileInfos:seq<string*string>)=
      fileInfos
      |>PSeq.map (fun (content,filePath)->
          let fileInfo=FileInfo(filePath)
          match fileInfo with
          | x when not  x.Exists ->
              if not x.Directory.Exists then
                x.Directory.Create()
              let bytes=ASCIIEncoding.Unicode.GetBytes(content)
              async{
                use fs=fileInfo.Create()
                do! fs.AsyncWrite(bytes)
              }
          | _ ->async{()})
      |>Async.Parallel 
      |>Async.Ignore
      |>Async.Start 


  static member ModifyFile (modifiedText:string) (filePath:string)=
    try
      match FileInfo(filePath) with
      | x when not x.Exists ->failwith "The file is not exsit!"
      | _ ->()
      let bytes=ASCIIEncoding.Unicode.GetBytes(modifiedText)
      let task=
        async{
          use fs=new FileStream(filePath,FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)
          do! fs.AsyncWrite(bytes)
        }
      Async.RunSynchronously task
    with
    | _ ->reraise()

*)

/////////////////////////////////////////////////////////////////////////////////////
(*
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

*)

(*
http://www.infoq.com/articles/pickering-fsharp-async
//Now let's take a look at the equivalent using the asynchronous programming model:
#light

open System.IO

let openFile() =
     let fs = new FileStream(@"C:\Program Files\Internet Explorer\iexplore.exe",
                             FileMode.Open, FileAccess.Read, FileShare.Read)
     let data = Array.create (int fs.Length) 0uy
     let callback ar =
         let bytesRead = fs.EndRead(ar)
         fs.Dispose()
         printfn "Read Bytes: %i, First bytes were: %i %i %i ..."
             bytesRead data.(1) data.(2) data.(3)
     fs.BeginRead(data, 0, data.Length, (fun ar -> callback ar), null) |> ignore

  openFile()

Asynchronous Workflows
Asynchronous workflows were introduced to tackle this specific problem. So now let's take a look at the asynchronous workflow version:

#light

open System.IO
open  Microsoft.FSharp.Control.CommonExtensions

let  openFile =
     async { use  fs = new  FileStream(@"C:\Program Files\Internet Explorer\iexplore.exe",
                                     FileMode.Open, FileAccess.Read, FileShare.Read)
             let  data = Array.create (int fs.Length) 0uy
             let!  bytesRead = fs.ReadAsync(data, 0, data.Length)
             do  printfn "Read Bytes: %i, First bytes were: %i %i %i ..."
                 bytesRead data.(1) data.(2) data.(3) }

 Async.Run openFile

 let printThreadChilds() =
     Console.WriteLine("Thread ID {0}", Thread.CurrentThread.ManagedThreadId)
     Console.WriteLine((new StackTrace()).ToString())



Now let's examine the asynchronous version:

#light
open System
open System.IO
open System.Text.RegularExpressions
open Microsoft.FSharp.Control.CommonExtensions

let path = @"C:\Users\robert\Documents\Fielding"

let readFileAsync filePath =
     async { // open and read file
             let fileStream = File.OpenText(filePath)
             let! text = fileStream.ReadToEndAsync()

             // find all the "words" using a regex
             let word = new Regex("\w+")
             let matches = word.Matches(text)
             let words  = { for m in matches -> m.Value }

             // count unique words using a set
             let uniqueWords = Set.of_seq words
             // print the results
             let name = Path.GetFileNameWithoutExtension(filePath)
             do Console.WriteLine("{0} - Words: {1} Unique words: {2} ",
                                  name, matches.Count, uniqueWords.Count) }

let main() =
     let filePaths = Directory.GetFiles(path)
     let tasks = [ for filePath in filePaths -> readFileAsync filePath ]
     Async.Run (Async.Parallel tasks)

 main()
*)
(*
	public abstract class ReadWriteOperate {
		
		/// <summary>
		/// Read the file
		/// </summary>
		public static StringBuilder ReadFile(string fileName) {
			StringBuilder sb = new StringBuilder();
			if (fileName != string.Empty) {
				using (StreamReader sr = new StreamReader(fileName, Encoding.Default)) {
					//FileInfo fi = new FileInfo(fileName);
					//char[] buffer=new char[fi.Length];
					//sr.Read(buffer, 0, Convert.ToInt16(fi.Length - 1));
					//for(int i=0;i<buffer.Length;i++){
					//   RtxSourceText.Text += buffer[i];
					//}
					 sb.Append(sr.ReadToEnd());
				}
			}
			return sb;
		}

		
		/// <summary>
		/// Write code file to the folder
		/// </summary>
		public static bool WriteFile(string sourceText,string path,string fileNamePath) {
			bool flag = false;
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

        /// <summary>
        /// Write code file to the folder
        /// </summary>
        public static bool ModifyFile(string sourceText, string filePath) {
            bool flag = false;
            //
            //Builder the channels for read or write file
            //,it similarity with [saveFileDialog.ShowDialog()],
            //it has sameness pupose of them
            //
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Write)) {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.Default)) {
                    sw.Write(sourceText);
                    flag = true;
                }
            }
            return flag;
        }


        public char[] WriteAndReadFile(string filePath,string fileContent,int charLength) {
            char[] chars = null;
            try {
                if (File.Exists(filePath)) {
                    File.Delete(filePath);
                }

                using (StreamWriter sw = new StreamWriter(filePath)) {
                    sw.Write(fileContent);
                }

                using (StreamReader sr = new StreamReader(filePath)) {
                    while (sr.Peek() >= 0) {
                        chars = new char[charLength];
                        sr.Read(chars, 0, chars.Length);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return chars;
            }
            return chars;
        }

	}

*)

(*

			if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format(Resources.Culture, Resources.ExceptionConfigurationFileNotFound, section), fileName);
				if (!Path.IsPathRooted(configSource))
				{
					return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configSource);
				}
*)