namespace WX.Data.CodeAutomation

open System
open System.Text
open System.Text.RegularExpressions
open System.IO
open System.Reflection
open FSharp.Collections.ParallelSeq
open WX.Data
open WX.Data.Helper

type ValidationBQ_BQClient=
  (*
  Right Reference
  static member GetDefferentMemberForTarget<'a,'b when 'a:(new:unit->'a) and 'b:(new:unit->'b)> (_BQClient:'a, _BQ:'b)=
  *)
  static member Validate (businessQueryEntityAssemblyDirectoryPath:string)=
    let sourceDirectory=DirectoryInfo businessQueryEntityAssemblyDirectoryPath
    let rec GetAssemblyPaths (sourceDirectory:DirectoryInfo)=    
      seq{   
          for a in sourceDirectory.GetFiles() do
            match Regex.IsMatch(a.Name,@"WX\.Data\.Business[a-zA-Z]*Entities[a-zA-Z]*\.[a-zA-Z\.]+\.dll",RegexOptions.IgnoreCase) with
            | true ->yield a.FullName
            | _ ->()
          for a in sourceDirectory.GetDirectories() do
            yield! GetAssemblyPaths(a)
      }
    seq{
      for m in GetAssemblyPaths(sourceDirectory) do
        match File.ReadAllBytes(m) with
        | x ->
            for n in AppDomain.CurrentDomain.Load(x).GetTypes() do
              yield n,m
    }
    |>Seq.filter (fun (a,_)->a.Name.StartsWith ("BQ_"))
    |>Seq.distinctBy (fun (a,_)->a.Name)
    |>Seq.groupBy (fun (a,_)->a.Name.Replace("_Client",""))
    |>fun a->
        seq{
          for (_,m) in a do
            match m|>Seq.length with
            | 2 ->
                match m|>Seq.sortBy (fun (b,_)-> -b.Name.Length) with //确保BQ_Client排前，BQ排后
                | s ->
                    match s|>Seq.nth 0 , s|>Seq.nth 1 with
                    | (x,xa),(y,ya) ->
                        match 
                          x.GetProperties(BindingFlags.Public||| BindingFlags.Instance ||| BindingFlags.SetProperty)|>Seq.filter(fun b->b.Name.StartsWithIn ["VC_";"C_"]),
                          y.GetProperties(BindingFlags.Public||| BindingFlags.Instance ||| BindingFlags.SetProperty)|>Seq.filter(fun b->b.Name.StartsWithIn ["VC_";"C_"]) with
                        | z,w -> 
                            match 
                              seq{
                                  for m in z do
                                    if w|>Seq.exists (fun c->c.Name =m.Name)|>not then yield m.Name    //客户端查询实体类型的数据成员是否存在于服务端查询实体类型中
                                } 
                              with
                            | u when (Seq.length u)>0 ->
                                match u|>Seq.fold (fun acc c->match acc with NotNullOrWhiteSpace _  ->acc+","+c | _ ->c) String.Empty with
                                | v ->
                                    yield "* 客户端查询实体类型"+x.Name+"和服务端查询实体类型"+y.Name+ @"的数据成员有差异！"+Environment.NewLine+"    差异数据成员为："+v+"."+Environment.NewLine+"    其中, 客户端查询实体类型"+x.Name+"所属装配的路径为："+xa+ ";"+Environment.NewLine+"    服务端查询实体类型"+y.Name+"所属装配的路径为"+ya+"."  
                                    yield "----------------------------------------------------------------------------------"
                            | _ ->()
            | 1 ->
                match m|>Seq.nth 0 with
                | (x,xa) -> 
                    match  x.Name.Contains("_Client") with
                    | true -> 
                      yield "* 客户端查询实体类型"+x.Name+ @"无对应的服务端查询实体类型!"+Environment.NewLine+"    该实体类型所属装配的路径为："+xa+"."
                      yield "----------------------------------------------------------------------------------"
                    | _ ->
                      yield "* 服务端查询实体类型"+x.Name+ @"无对应的客户端查询实体类型!"+Environment.NewLine+"    该实体类型所属装配的路径为："+xa+"."
                      yield "----------------------------------------------------------------------------------"
            | _ ->()    
        } 