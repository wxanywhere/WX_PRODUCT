

open System
open System.IO
open System.Text.RegularExpressions
//----------------------------------------------
(**
#r "Microsoft.Build.dll"
#r "Microsoft.Build.Framework.dll"
open Microsoft.Build.Evaluation
**)
//----------------------------------------------
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
#r "WX.Data.CodeAutomation.dll"
open WX.Data.Helper
open WX.Data.CodeAutomation
open WX.Data

//=================================================
(*
#load @"D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\MSBuild.fs"
open WX.Data.CodeAutomation
*)
//=================================================


[
@"C:\Users\zhou\Documents\Visual Studio 2013\Projects\ConsoleApplication9\ClassLibrary1\ClassLibrary1.csproj"
]
|>MSBuild.INS.Build

[
@"D:\Workspace\SBIIMS\SBIIMS_VC\VC_GXFB\WX.Data.BusinessEntitiesAdvance.VC.GXFB"
]
|>MSBuild.INS.BuildAll

[
@"D:\Workspace\SBIIMS\SBIIMS_AC\AC_JSGNGLX"
]
|>MSBuild.INS.BuildAll

[
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_KJCD"
]
|>MSBuild.INS.BuildForBusinessExchangeLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_AC\AC_GNWH"
]
|>MSBuild.INS.BuildForBusinessExchangeLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
]
|>MSBuild.INS.BuildForViewLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_AC"
]
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewModelLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGHC"
]
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewModelLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGXZ"
]
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewModelLayers

[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CKWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CZYWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_FJPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_GHSWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_JYFWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHHYWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_XFPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_ZCLBWH"
]
|>MSBuild.INS.BuildAll


[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CKWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CZYWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_FJPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_GHSWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_JYFWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHHYWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_XFPWH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_ZCLBWH"
]
|>MSBuild.INS.BuildAll

//|>MSBuild.INS.BuildForBusinessExchangeLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_Frame"
]
|>MSBuild.INS.BuildAll

[
@"D:\Workspace\SBIIMS\SBIIMS_APC"
@"D:\Workspace\SBIIMS\SBIIMS_VC"
@"D:\Workspace\SBIIMS\SBIIMS_FK"
]
|>MSBuild.INS.BuildAll

//========================================================================================

(
@"D:\Workspace\SBIIMS\SBIIMS_JXC"
,
[
@"^JXC_JBXX_[A-Z]+$"
//@"^JXC_JHGL_[A-Z]+$"
//@"^JXC_KCGL_[A-Z]+$"
//@"^JXC_XSGL_[A-Z]+$"
//@"^JXC_TJBB_[A-Z]+$"
//@"^JXC_ZHGL_[A-Z]+$"
//@"^JXC_ZHBB_[A-Z]+$"
]
)
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewLayers
//|>MSBuild.INS.BuildForViewModelLayers

(
@"D:\Workspace\SBIIMS\SBIIMS_AC"
,
[
@"^AC_[A-Z]+$"
]
)
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewLayers

(
@"D:\Workspace\SBIIMS\SBIIMS_VC"
,
[
@"^VC_[A-Z]+$"
]
)
//|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewModelLayers
|>MSBuild.INS.BuildForViewLayers

[
@"D:\Workspace\SBIIMS\SBIIMS_Frame"

]
//|>MSBuild.INS.BuildForViewLayers
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewModelLayers

(
@"D:\Workspace\SBIIMS\SBIIMS_Frame"
,
[
@"^Frame_[A-Z]+$"
]
)
|>MSBuild.INS.BuildAll

[
@"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame_SJJK\WX.Data.DataModel.Frame.SJJK\WX.Data.DataModel.Frame.SJJK.csproj"
]
|>MSBuild.INS.Build

(
@"D:\Workspace\SBIIMS\SBIIMS_AC"
,
[
@"^AC_[A-Z]+$"
]
)
|>MSBuild.INS.BuildAll
//|>MSBuild.INS.BuildForViewModelLayers


(
@"D:\Workspace\SBIIMS\SBIIMS_FK"
,
[
@"^FK_[A-Z]+$"
]
)
|>MSBuild.INS.BuildAll

(
@"D:\Workspace\SBIIMS\SBIIMS_VC"
,
[
@"^VC_[A-Z]+$"
]
)
|>MSBuild.INS.BuildAll


let entryDirectory=DirectoryInfo @"D:\Workspace\SBIIMS\SBIIMS_JXC"
seq{
  for n in entryDirectory.GetDirectories() do
    match n.Name.Split([|'_'|],StringSplitOptions.RemoveEmptyEntries) with
    | [|a;b|] when a.ToUpper()=a && b.ToUpper()=b ->
        yield String.Format (".{0}.{1}",a,b), String.Format ("_{0}_{1}",a,b)
    | [|a;b;c|] when a.ToUpper()=a && b.ToUpper()=b && c.ToUpper()=c ->
        yield String.Format (".{0}.{1}.{2}",a,b,c), String.Format ("_{0}_{1}_{2}",a,b,c)
    | _ ->()
}
|>Seq.toArray
|>Seq.iter (fun (a,b) -> Path.Combine(@"D:\Workspace\SBIIMS\SBIIMS_JXC",b.Remove(0,1))|>ObjectDumper.Write)   

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_CGFX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_CGJH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_GZLFX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_GZLYC"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_JGFX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_JGYC"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_KCFX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_KCYC"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_XSFX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_XSYC"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_YLFX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_FXYC_YLYC"
]
|>MSBuild.INS.BuildAll

@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GG"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGHC"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGXZ"

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CKWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_CZYWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_FJPWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_GHSWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_JYFWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHHYWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_KHWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_SPWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_XFPWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_ZCLBWH"
]
|>MSBuild.INS.BuildAll

[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGJH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_CGTH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_DJCX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JHGL_WLZW"
]
|>MSBuild.INS.BuildAll


[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_BSBY"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_DJCX"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_DJXC"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_KCCX"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_KCDB"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_KCPD"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_KCPDX"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_KCYJ"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_SPBS"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_SPBY"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_SPCF"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_SPKB"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_KCGL_WLZW"
]
|>MSBuild.INS.BuildAll

@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_SY"

[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_CGYCG"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_KCCB"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_SPCG"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_SPXS"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_XSPH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_XSYXS"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_TJBB_YYFX"
]
|>MSBuild.INS.BuildAll

[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_DJCX"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_POS"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_SPXS"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_WLZW"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_XSHH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XSGL_XSTH"
]
|>MSBuild.INS.BuildAll

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_BFHF"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_NZWJS"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_QCJZ"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_TMDY"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_XTCSH"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_XTRZ"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_XTSZ"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_XTGL_YZWJS"
]
|>MSBuild.INS.BuildAll

@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZH"

[
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHBB_DCBB"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHBB_DJBB"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHBB_QDBB"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHBB_TBBB"
]
|>MSBuild.INS.BuildAll

[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_BJGL"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_CWGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_FJPGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_GDZCGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_GHSGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_HTGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_HYGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_JJGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_JYFGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_JYFYGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_KHGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_KHJH"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_SJTX"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_XFPGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_XZGL"
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_ZHGL_YWYGL"
]
|>MSBuild.INS.BuildAll


[
//@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_JBXX_YGWH\WX.Data.DataModel.JXC.JBXX.YGWH\WX.Data.DataModel.JXC.JBXX.YGWH.csproj"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGHC\WX.Data.DataAccessAdvanceX.JXC.GGHC\WX.Data.DataAccessAdvanceX.JXC.GGHC.fsproj"
@"D:\Workspace\SBIIMS\SBIIMS_JXC\JXC_GGHC\WX.Data.IDataAccessAdvanceX.JXC.GGHC\WX.Data.IDataAccessAdvanceX.JXC.GGHC.fsproj"
]
|>MSBuild.INS.Build

//==================================================================================
(*
问题
Message=Could not load file or assembly 'FSharp.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' or one of its dependencies. The system cannot find the file specified.         FileName=FSharp.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a         FusionLog=WRN: Assembly binding logging is turned OFF.
To enable assembly bind failure logging, set the registry value [HKLM\Software\Microsoft\Fusion!EnableLog] (DWORD) to 1.
Note: There is some performance penalty associated with assembly bind failure logging.
To turn this feature off, remove the registry value [HKLM\Software\Microsoft\Fusion!EnableLog].
     Data=...        InnerException={ }      TargetSite={ }  StackTrace=   at Microsoft.FSharp.Core.Operators.Raise[T](Exception exn)
   at WX.Data.CodeAutomation.MSBuild.Build(IEnumerable`1 sourceDirectoryPaths, IEnumerable`1 projectFileNamePatternsOnOrder) in D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\MSBuild.fs:line 54
   at <StartupCode$WX-Data-CodeAutomation>.$MSBuild.BuildAll@79.Invoke(IEnumerable`1 sourceDirectoryPaths, IEnumerable`1 projectFileNamePatternsOnOrder) in D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\MSBuild.fs:line 79
   at WX.Data.CodeAutomation.MSBuild.BuildAll(IEnumerable`1 sourceDirectoryPaths) in D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.CodeAutomation\MSBuild.fs:line 59     HelpLink=null   Source=FSharp.Core      HResult=-2147024894
val it : unit = ()
> exit 0;;
解决
在fsi.exe中("C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0\Fsi.exe.config"), 添加f#版本Remapping
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <runtime>
     <legacyUnhandledExceptionPolicy enabled="true" />
     <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
       <dependentAssembly>
         <assemblyIdentity name="FSharp.Core"  publicKeyToken="b03f5f7f11d50a3a"
                          culture="neutral"/>
         <bindingRedirect oldVersion="2.0.0.0" newVersion="4.3.0.0"/>
         <bindingRedirect oldVersion="4.0.0.0" newVersion="4.3.0.0"/>
       </dependentAssembly>
    </assemblyBinding>
  </runtime>
  <appSettings>
    <add key="EntryConfigPath" value="D:\Workspace\SBIIMS\SBIIMS_Utility\WX.Data.FHelper\Entry.config" />
  </appSettings>
</configuration>
*)