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
open Microsoft.FSharp.Collections

#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug" 
//#r "WX.Data.AssemblyLoader.dll"
//open WX.Data.AssemblyLoader
#r "WX.Data.Helper.dll"
open WX.Data.Helper

//==============================================================
(*Config Way
<runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <probing privatePath="Castle;Core;Module;UI;Misc;"/>
  </assemblyBinding>
</runtime>
http://msdn.microsoft.com/zh-cn/library/system.appdomain.getdata.aspx
AppDomain.CurrentDomain.GetData or SetData 方法, 获取或更新当前AppDomain的AppDomainSetup信息
"APPBASE"ApplicationBase
"LOADER_OPTIMIZATION" LoaderOptimization
"APP_CONFIG_FILE" ConfigurationFile
"DYNAMIC_BASE"DynamicBase
"DEV_PATH"（无属性）
"APP_NAME"ApplicationName
"PRIVATE_BINPATH"PrivateBinPath 
"BINPATH_PROBE_ONLY"PrivateBinPathProbe
"SHADOW_COPY_DIRS"ShadowCopyDirectories
"FORCE_CACHE_INSTALL"ShadowCopyFiles
"CACHE_BASE"CachePath（应用程序特定的）LicenseFile
"APP_LAUNCH_URL"（无属性）
*)
AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "C:\Program Files (x86)\Microsoft F#\v4.0\fsi.exe.Config")
AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE")
AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", "Castle;Core;Module;UI;Misc;") //Castl等为主程序所在目录的子目录名
AppDomain.CurrentDomain.GetData("PRIVATE_BINPATH")
AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", "Castle;Core;Module;UI;Misc;") //
AppDomain.CurrentDomain.GetData("BINPATH_PROBE_ONLY")

//=========================================================

let x=AppDomain.CreateDomain("API")
AppDomain.CurrentDomain.SetData("API",x)
AppDomain.CurrentDomain.GetData("API")

let directory= @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
let dll=File.ReadAllBytes (Path.Combine(directory, "WPF.Themes.dll"))
let pdb=File.ReadAllBytes (Path.Combine(directory,"WPF.Themes.pdb"))
let asm= AppDomain.CurrentDomain.Load(dll,pdb) 
//let a=Application.Current.get
let x=asm.GetManifestResourceNames()
let y=asm.GetManifestResourceInfo("WPF.Themes.g.resources")
let z=asm.GetManifestResourceStream ("WPF.Themes.g.resources")
z.

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

System.AppDomain.CurrentDomain.na
asm.GetType("WX.Data.ViewModel.FVM_TJ_YYFX")
workspace = Activator.CreateInstance(currentType.First()) as WorkspaceViewModel;

AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE")

//============================================================
(*
http://www.cnblogs.com/nullreference/archive/2010/02/09/set_appdomain_privatebinpath_and_configurationfile_location.html
代码控制PrivateBinPath和ConfigurationFile的位置

.Net的WinForm程序有的时候让人很烦的是，在执行目录下总是一大堆的DLL，配置文件，最少则是个以下，多的时候怕有四五十个吧……，自己程序中的类库，第三方的类库……加载一起让人感觉乱糟糟的，非常不爽。在下虽然在个人卫生上没有什么洁癖，可是对于应用程序的这个样子确实没有一点容忍力的，是可忍孰不可忍啊！
 
处理这些DLL还是比较简单的，Configuration文件里就可以配置了。先将DLL分门别类，Core, Module, Misc等等，然后将这几个目录名称加入App.config中。如：
<runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <probing privatePath="Castle;Core;Module;UI;Misc;"/>
  </assemblyBinding>
</runtime>
嗯，现在看上去就舒服多了，各就各位的，清爽！慢……在主目录下，出了我们的应用程序Exe可执行文件外，还有一个例外，app.config…… 多少还是有点儿不爽，怎么办呢？我们建个Data目录，将config文件移动进去吧~！说做就做，我们就把app.config移动到下级目录了，开心啊，看看程序能运行不？ 杯具发生了，程序运行毫无反应…… 用VS调试之，发现是找不到其他类库中的类型导致的。也是，我们刚刚在config文件中加了privatePath，现在有把这个文件移动了，怎么能加载到其他目录下的DLL呢？
 
 
知道原因了，当然就好解决。据说configuration文件的位置是可以指定的。AppDomainSetup.ConfiguraitonFile属性就是指的这个。让我们来试试……
 
分析一下：
要改config的路径，当然是要在使用config之前咯，而且越早越好…… 哪里比较早呢？嗯，Main函数，程序的入口是个不错的选择……
static class Program
{
     /// <summary>
    /// 应用程序的主入口点。
    /// </summary>
    [STAThread]
    static void Main()
    {
        AppDomain.CurrentDomain.SetupInformation.ConfigurationFile = "Data\MyApp.Config";
 
        Application.Run(new MyAppForm());
    }
}
 
好，编译，信心满满的再次运行程序，可是杯具再一次啊发生了，依然没有反应……
 
 
……在此调试，惊奇的发现这个属性设置居然完全没有效果，语句执行完后，该属性值依然是默认值。于是查文档啊，百度啊，谷歌啊~ 最后终于知道设置config的正确方法。
AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", path);
 
嗯，这个这个……不是微软在害人吗…… 好了，停下抱怨继续工作先…… 嗯，看到了，config文件的位置生效了，指向了我们预定的位置，可是………… 杯具毫无意外的再次发生……DLL依然不能加载！
 
 
虽然我已经在AppDomainSetup里看到了PrivateBinPath属性，可是我却一点兴奋感觉都没有，应该跟前面的结果类似把！
想想程序运行的过程，PrivateBinPath是AppDomain.Current.SetupInformation里的属性，而这个属性只有在AppDomain.Create的时候才会生效，在AppDomain已经建立后更改config的路径再去设置这个值，应该是不行的。看来只好代码里来设置了。可是如何下手呢？
 
关门， 放Reflector！呵呵，让我们看看AppDomain内部的情况。果其不然啊！SetupInforamtion只是其内部配置的一个Copy，这也就解释了为什么更改属性却没有生效的原因！
public AppDomainSetup SetupInformation
{
    get
    {
        return new AppDomainSetup(this.FusionStore, true);
    }
}
 
看起来不动用终极手段是不行的了！反射！更改内部字段来达到我们的目的！仔细检查了以下代码，发现AppDomain内部的Setup信息是保存在一个FunsionStore的Internal属性里的。好，我们就从这里动手把！
 
AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", "Castle;Core;Module;UI;Misc;");
AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", "Castle;Core;Module;UI;Misc;");
 
 
这段代码确实就更改了AppDomain.CurrentDomain.SetupInformation.PrivateBinPath的值了，可是运行发现类型还是不能加载！可能内部还有个什么RefrefshConfiguraiton或UpdateCache之类的方法来刷新！继续找啊找的~ 终于发现了。是一个static extern的方法。呵呵……这下简单了，看代码！
 
 
在调试过程中，我们还发现Main方法不是最好的地方，所以我们将代码放在了Program的静态构造方法中，这里是除了静态字段外的最早的起始地了。
view sourceprint?
static Program()
{
    AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", "Castle;Core;Module;UI;Misc;");
    AppDomain.CurrentDomain.SetData("BINPATH_PROBE_ONLY", "Castle;Core;Module;UI;Misc;");
    var m = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
    var funsion = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance);
    m.Invoke(null, new object[] { funsion.Invoke(AppDomain.CurrentDomain, null), "PRIVATE_BINPATH", "Castle;Core;Module;UI;Misc;" });
}
嗯，迫不及待的编译，运行！哇！毫不意外的，程序正常了，再看看应用程序目录，只有一个干干净净的exe文件的存在，真是爽到极点~
 
 
告诉你，exe文件就一个，我可以！哈哈！
 
最后提醒，如果Program里有静态字段，不要定义为用其他类库的类型，因为这个时候我们上述的方法还没有执行到，会因为找不到类型而出错的哦~
*)