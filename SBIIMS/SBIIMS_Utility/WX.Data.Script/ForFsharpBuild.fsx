

#I @"C:\Program Files (x86)\Microsoft SDKs\F#\3.1\Framework\v4.0"
#r "FSharp.Build.dll"
#r "Microsoft.Build.Utilities.v12.0"
#r "Microsoft.Build"
#r "Microsoft.Build.Engine"
#r "Microsoft.Build.Framework"


#I @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\FSharp\FSPackages"
#r "FSharp.ProjectSystem.FSharp"

open Microsoft.FSharp.Build
open Microsoft.Build
open Microsoft.Build.Evaluation
open Microsoft.Build.Framework

let fsProjFile = @"D:\Workspace\SBIIMS\SBIIMS_Frame\Frame\WX.Data.BusinessEntities.Frame\WX.Data.BusinessEntities.Frame.fsproj";
let pc=new ProjectCollection()
let project = new Project(fsProjFile);
project.SetProperty("Configuration", "Debug");

let success = project.Build();



let build=new Microsoft.FSharp.Build.Fsc()
