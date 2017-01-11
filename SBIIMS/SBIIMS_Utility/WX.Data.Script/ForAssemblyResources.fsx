#r "FSharp.PowerPack.dll"
#r "PresentationCore.dll"
#r "PresentationFramework.dll"
#r "System.Xaml.dll"
#r "WindowsBase.dll"
#r "System.Configuration.dll"
#r "FSharp.PowerPack.Parallel.Seq.dll"
open System.Configuration
open System.Collections.Generic
open Microsoft.FSharp.Collections
open System.Windows.Markup
open System.Windows
open System.Windows.Controls
open System
open System.IO
open System.Windows
open System.Collections
open System.Resources
open System.Reflection
open Microsoft.FSharp.Collections


#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug" 
//#r "WX.Data.AssemblyLoader.dll"
//open WX.Data.AssemblyLoader
#r "WX.Data.Helper.dll"
open WX.Data.Helper
#r "WX.Data.CModule.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.BusinessDataEntities.System.dll"
#r "WX.Data.BusinessDataEntitiesAdvance.System.dll"
open WX.Data.BusinessEntities

//=========================================================



let directory=String.Format( @"{0}\api",@"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug")         //AppDomain.CurrentDomain.BaseDirectory
let id=ref 0
let fid=ref 0
let  GetThemeDlls (directory:string)=
  let directoryInfo=DirectoryInfo directory
  let rec GetThemeDlls (directoryInfo:DirectoryInfo)=
    seq{
      for b in directoryInfo.GetFiles() do
        if b.Name.ToLowerInvariant().Contains("theme") && b.Extension.ToLowerInvariant() =".dll" then
          yield b
      for a in directoryInfo.GetDirectories() do
        yield! GetThemeDlls  a
      }
  GetThemeDlls directoryInfo
GetThemeDlls  directory
|>Seq.map (fun a->Assembly.LoadFile a.FullName)
|>Seq.iteri (fun i a->
    String.Format ("{0}.g.resources",a.GetName().Name)
    |>a.GetManifestResourceStream
    |>fun a->new ResourceReader (a)
    |>Seq.cast<DictionaryEntry>
    |>Seq.filter (fun a->a.Key.ToString().EndsWith ("theme.baml"))
    |>Seq.iteri (fun k b->
        if k=0 then
          fid:=!id
          match new BD_T_JMZT_Advance() with
          | x ->
              x.C_ID<- !id
              x.C_FID<- -1
              x.C_MC<- a.GetName().Name
              x.C_LJ<-String.Empty
              x.C_CS<-String.Empty
              x.C_JYBZ<-false
              incr id
              ObjectDumper.Write ((x.C_ID,x.C_FID))
        match new BD_T_JMZT_Advance(), b.Key.ToString() with
        | x,y ->
            x.C_ID<- !id
            x.C_FID<- !fid
            x.C_MC<-y.Remove(y.Length-11).Replace('/','_')
            x.C_LJ<- String.Format("/{0};Component/{1}.xaml",asm.GetName().Name,y.Remove(y.Length-5))
            x.C_CS<-String.Empty
            x.C_JYBZ<-false
            incr id
            ObjectDumper.Write  ((x.C_ID,x.C_FID))
    ))


let directory=String.Format(@"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug")         //AppDomain.CurrentDomain.BaseDirectory
let dll=File.ReadAllBytes (Path.Combine(directory, "WPF.Themes.dll"))
let pdb=File.ReadAllBytes (Path.Combine(directory,"WPF.Themes.pdb"))
let asm= AppDomain.CurrentDomain.Load(dll,pdb) 
//asm.GetName.Name
//let a=Application.Current.get
let x=asm.GetManifestResourceNames()
let y=asm.GetManifestResourceInfo("WPF.Themes.g.resources")
let z=asm.GetManifestResourceStream ("WPF.Themes.g.resources")
let res=new ResourceReader(z)
res
|>PSeq.cast<System.Collections.DictionaryEntry>
|>PSeq.filter (fun a->a.Key.ToString().EndsWith ("theme.baml"))
|>PSeq.iteri (fun i a->
    match new BD_T_JMZT_Advance(), a.Key.ToString() with
    | x0,x1 ->
        x0.C_ID<-i
        x0.C_MC<-x1.Remove(x1.Length-11).Replace('/','_')
        x0.C_LJ<- String.Format("/{0};Component/{1}.xaml",asm.GetName().Name,x1.Remove(x1.Length-5))
        ObjectDumper.Write (x0,1)
        )



//==============================================================

//Application.Current.
z.ToString()
y.ResourceLocation
ObjectDumper.Write y
let res=asm.GetManifestResourceStream("xceedoffice2007silver/theme.baml")
|>fun a->a.GetManifestResourceInfo("theme")
|>fun a->
  let b=a.
|>ObjectDumper.Write

AppDomain.CurrentDomain.GetAssemblies()
|>PSeq.iter (fun a->a.

var directory = ConfigurationManager.AppSettings["ViewModelAdvanceAssemblyDirectory"];
var pathDLL = Path.Combine(directory, "WX.Data.FViewModel.dll");
var pathPDB = Path.Combine(directory, "WX.Data.FViewModel.pdb");
var bytesDLL = File.ReadAllBytes(pathDLL);
var bytesPDB = File.ReadAllBytes(pathPDB);
AppDomain.CurrentDomain.Load(bytesDLL, bytesPDB);

pathDLL = Path.Combine(directory, "WX.Data.FViewModelData.dll");
pathPDB = Path.Combine(directory, "WX.Data.FViewModelData.pdb");
bytesDLL = File.ReadAllBytes(pathDLL);
bytesPDB = File.ReadAllBytes(pathPDB);
AppDomain.CurrentDomain.Load(bytesDLL, bytesPDB);




let loader=new Loader()

let directory= @"D:\Workspace\SBIIMS\WX.Data.FViewModelData\bin\Debug\"

loader.LoadAssembly("WX.Data.FViewModelData.dll")

let asm=System.AppDomain.CurrentDomain.GetAssemblies()
asm.Length
asm.[0].GetName().Name

#I  @"D:\Workspace\SBIIMSClient\WX.Data.FViewModel\bin\Debug"
#r "WX.Data.FViewModel.dll"
open System.Reflection
let file= @"D:\Workspace\SBIIMSClient\WX.Data.FViewModel\bin\Debug\WX.Data.FViewModel.dll"
let asm=Assembly.LoadFrom(file)
asm.GetTypes()

System.AppDomain.CurrentDomain
let xtype=asm.GetType("WX.Data.ViewModel.FVM_TJ_YYFX")
workspace = Activator.CreateInstance(currentType.First()) as WorkspaceViewModel;