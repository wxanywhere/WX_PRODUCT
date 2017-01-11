

open System.Text.RegularExpressions

Regex.Split ("QueryResult<BD_TE[]>",@"[\<\>]",RegexOptions.Singleline)

let input= @"abstract CreateCD_RQD:BD_ExecuteContent<BD_T_CD_RQD>* ?context:SBIIMS_FrameEntitiesAdvance* ?currentDateTime:DateTime* ?bd_ExecuteResult:BD_ExecuteResult ->BD_ExecuteResult"

let reg=Regex.Matches(input, @"^\s*abstract\s+([a-zA-Z_]+)\s*:\s*\(*\s*([a-zA-Z_\s\<\>\[\]\:\?\*]+)\s*\)*\s*\-\>\s*([a-zA-Z_\s\<\>\[\]]+)\s*.*$",RegexOptions.Singleline)  
reg.[0].Groups.[1].Value
reg.[0].Groups.[2].Value


let x="IXX_CK_CK"
Regex.IsMatch(x,@"^T_[A-Z_]*[A-Z]$",RegexOptions.None) 

Regex.IsMatch(x,@"^(PK|IX)_[A-Z_]*[A-Z]$",RegexOptions.None) 

let x="FK_T_JHSP_FZ_C_WX_T_JHSP"
Regex.IsMatch(x,@"^FK_T_[A-Z_]+C+_+[A-Z_]+T_[A-Z_]*[A-Z]$",RegexOptions.None)

let x01="CJ20101131000"
Regex.IsMatch(x01,@"^\w{2}[23456]\d{3}[01]\d[0123]\d\d{3,}$",RegexOptions.None)


"VC_CKDQ","string",false,"仓库类型"
"VC_CKFZR","string",false,"仓库负责人"


//pattern:""""^\s*Global\s*\n\s*GlobalSection\(\s*TeamFoundationVersionControl\s*\)\s*\=\s*preSolution\s*\n([\w\W]*?)(?!EndGlobalSection)([\w\W]*?)\s*EndGlobalSection\s*$"""     
//([\w\W]*?)(?!EndGlobalSection)([\w\W]*?) //其中的“？”号 不能省 

//匹配字符串
"""
	GlobalSection(TeamFoundationVersionControl) = preSolution
		SccNumberOfProjects = 32
		SccEnterpriseProvider = {4CA58AB2-18FA-4F8D-95D4-32DDF27D184C}
		SccTeamFoundationServer = https://wx.visualstudio.com/defaultcollection
    ...
	EndGlobalSection
"""
//替换为
"""
Global
"""

"""
\n\s*\<None\s+Include\s*\=\s*\"[A-Z0-9\-_\.a-z\\]+\"\s*\/\>
\n\s*\<Content\s+Include\s*\=\s*\"[A-Z0-9\-_\.a-z\\]+\"\s*\/\>
\n\s*\<Compile\s+Include\s*\=\s*\"[A-Z0-9\-_\.a-z\\]+\"\s*\/\>

\n\s*\<Compile[\w\W]+\<\/Compile\>
\n\s*\<Page[\w\W]+\<\/Page\>

"""