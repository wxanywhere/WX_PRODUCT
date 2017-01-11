#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"

open WX.Data.Helper

open System.IO
open System.Net
open System.Text.RegularExpressions
let url = @"http://localhost:2000/publish.htm"
// Download the webpage
let req = WebRequest.Create(url)
let resp = req.GetResponse()
let stream = resp.GetResponseStream()
let reader = new StreamReader(stream)
let html = reader.ReadToEnd()

ObjectDumper.Write(html)

//========================================================

System.Environment.MachineName 
System.Environment.UserDomainName
System.Environment.UserName

System.Net.Dns.GetHostName()
System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).GetValue(2).ToString()
System.Environment.UserDomainName
System.Environment.UserName

let x=System.Diagnostics.Process.GetProcessById(1)

//System.Diagnostics.Process.
//x.k

(*
　　2. 在网络编程中的通用方法： 
　　获取当前电脑名：static System.Net.Dns.GetHostName() 
　　根据电脑名取出全部IP地址：static System.Net.Dns.Resolve(电脑名).AddressList 
　　也可根据IP地址取出电脑名：static System.Net.Dns.Resolve(IP地址).HostName 
　　3. 系统环境类的通用属性： 
　　当前电脑名：static System.Environment.MachineName 
　　当前电脑所属网域：static System.Environment.UserDomainName 
　　当前电脑用户：static System.Environment.UserName

*)

System.Net.Dns.Resolve(System.Net.Dns.GetHostName()).AddressList


System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName()).GetValue(0).ToString();

let x01=System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
x01.GetValue(2).ToString()

System.Net.Dns.


for a in x01 do
 //a.get_Address()
 ObjectDumper.Write(a |>ignore)


for a in 
  System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList do
  ObjectDumper.Write(a.AddressFamily.ToString())

System.Net.Dns.GetHostName() 


System.Environment.MachineName

System.Environment.UserDomainName

System.Environment.UserName

System.Environment.i


System.Net.Dns.Resolve(System.Net.Dns.GetHostName() ).AddressList.SetValue( 

System.Net.Dns.GetHostAddresses(

System.Net.NetworkInformation.PhysicalAddress

System.Environment.UserDomainName 

