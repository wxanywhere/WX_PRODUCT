#r "System.dll"
#r "System.Runtime.Serialization.dll"
#r "System.ServiceModel.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"

open System
open Microsoft.FSharp.Text
open System.ServiceModel
open System.Configuration


//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"

open WX.Data.WcfService
open WX.Data.ServiceContracts
open WX.Data.Helper
open WX.Data.FHelper

let str01:string=null
str01.Length

//Double.TryParse(parts.[0])
let (successX, x) = Double.TryParse("100")
//要比这样好 
let x:float ref=ref 0.0
let successX = Double.TryParse("100",x)


"100"
|>Double.TryParse //it's right

match Double.TryParse("100") with
| true,x ->ObjectDumper.Write x
| _ ->()


(*
let b=ref Unchecked.defaultof<_>
sb.TryGetObjectByKey(a,b), !b with
可以写成, 
let (flag,result)=sb.TryGetObjectByKey(a) //参考变量可以不需要作为参数, 而是可以在结果元数据中接收 
同样的 let (successX, x) = Double.TryParse("100")
*)

let mutable array=Array.create 2 byte
let array:byte array=Array.zeroCreate 2
let x=array.[0]- byte '\000'
let x=int16 (array.[0]- byte '\000')*256s+ int16 (array.[1]- byte '\000')

let x=System.Text.Encoding.Default.GetBytes("周涛")


//It's right, tested
let  GetChinesePYString01 (chineseStr:string)=
        let tempStr:string ref=ref String.Empty
        //let mutable array:byte[]=null //let array:byte []=Array.zeroCreate 2
        for a in chineseStr do
          match a with
          | x when int x>=33 && int x<=126->tempStr:=!tempStr+x.ToString()
          | x ->
              tempStr:=!tempStr+
              match x.ToString()|>System.Text.Encoding.Default.GetBytes with
              | y ->
                  match int (y.[0]- byte '\000')*256+ int (y.[1]- byte '\000') with   //可用char 0，在C#中为(char)0='\0'
                  | z when z < 0xB0A1 -> "*"
                  | z when z < 0xB0C5 -> "a"
                  | z when z < 0xB2C1 -> "b"
                  | z when z < 0xB4EE -> "c"
                  | z when z < 0xB6EA -> "d"
                  | z when z < 0xB7A2 -> "e"
                  | z when z < 0xB8C1 -> "f"
                  | z when z < 0xB9FE -> "g"
                  | z when z < 0xBBF7 -> "h"
                  | z when z < 0xBFA6 -> "j"
                  | z when z < 0xC0AC -> "k"
                  | z when z < 0xC2E8 -> "l"
                  | z when z < 0xC4C3 -> "m"
                  | z when z < 0xC5B6 -> "n"
                  | z when z < 0xC5BE -> "o"
                  | z when z < 0xC6DA -> "p"
                  | z when z < 0xC8BB -> "q"
                  | z when z < 0xC8F6 -> "r"
                  | z when z < 0xCBFA -> "s"
                  | z when z < 0xCDDA -> "t"
                  | z when z < 0xCEF4 -> "w"
                  | z when z < 0xD1B9 -> "x"
                  | z when z < 0xD4D1 -> "y"
                  | z when z < 0xD7FA -> "z"
                  | _ -> "*"
        !tempStr

let sm=GetChinesePYString01 "在"


55290=0xD7FA
int 0xB0A1

let result=
    match int (byte 214- byte '\000')*256+ int (byte 220- byte '\000') with   //可用char 0，在C#中为(char)0='\0'
    | z when z < 0xB0A1 -> "*"
    | z when z < 0xB0C5 -> "a"
    | z when z < 0xB2C1 -> "b"
    | z when z < 0xB4EE -> "c"
    | z when z < 0xB6EA -> "d"
    | z when z < 0xB7A2 -> "e"
    | z when z < 0xB8C1 -> "f"
    | z when z < 0xB9FE -> "g"
    | z when z < 0xBBF7 -> "h"
    | z when z < 0xBFA6 -> "g"
    | z when z < 0xC0AC -> "k"
    | z when z < 0xC2E8 -> "l"
    | z when z < 0xC4C3 -> "m"
    | z when z < 0xC5B6 -> "n"
    | z when z < 0xC5BE -> "o"
    | z when z < 0xC6DA -> "p"
    | z when z < 0xC8BB -> "q"
    | z when z < 0xC8F6 -> "r"
    | z when z < 0xCBFA -> "s"
    | z when z < 0xCDDA -> "t"
    | z when z < 0xCEF4 -> "w"
    | z when z < 0xD1B9 -> "x"
    | z when z < 0xD4D1 -> "y"
    | z when z < 0xD7FA -> "z"
    | _ -> "*"


////It's right, tested
let GetChinesePYStringWithRec (chineseStr:string)=
        let rec GetChinesePYStringRec (chineseChars:char[]) (index:int)=
          match chineseChars,index with
          | x,i when i=x.Length->""
          | x,i when int x.[i]>=33 && int x.[i]<=126->x.[i].ToString()+GetChinesePYStringRec x (i+1)
          | x,i ->
              match x.[i].ToString()|>System.Text.Encoding.Default.GetBytes with
              | y ->
                  match int (y.[0]- byte '\000')*256+ int (y.[1]- byte '\000') with   //可用char 0，在C#中为(char)0='\0'
                  | z when z < 0xB0A1 -> "*"
                  | z when z < 0xB0C5 -> "a"
                  | z when z < 0xB2C1 -> "b"
                  | z when z < 0xB4EE -> "c"
                  | z when z < 0xB6EA -> "d"
                  | z when z < 0xB7A2 -> "e"
                  | z when z < 0xB8C1 -> "f"
                  | z when z < 0xB9FE -> "g"
                  | z when z < 0xBBF7 -> "h"
                  | z when z < 0xBFA6 -> "g"
                  | z when z < 0xC0AC -> "k"
                  | z when z < 0xC2E8 -> "l"
                  | z when z < 0xC4C3 -> "m"
                  | z when z < 0xC5B6 -> "n"
                  | z when z < 0xC5BE -> "o"
                  | z when z < 0xC6DA -> "p"
                  | z when z < 0xC8BB -> "q"
                  | z when z < 0xC8F6 -> "r"
                  | z when z < 0xCBFA -> "s"
                  | z when z < 0xCDDA -> "t"
                  | z when z < 0xCEF4 -> "w"
                  | z when z < 0xD1B9 -> "x"
                  | z when z < 0xD4D1 -> "y"
                  | z when z < 0xD7FA -> "z"
                  | _ -> "*"
              + GetChinesePYStringRec x (i+1)
        GetChinesePYStringRec (chineseStr.ToCharArray()) 0

GetChinesePYString "aaaa我们一起走遍最美"

        | x 
        for a in chineseStr do
          match a with
          | x when int x>=33 && int x<=126->tempStr+x.ToString()|>ignore
          | x ->
              ignore<|
              tempStr+
              match x.ToString()|>System.Text.Encoding.Default.GetBytes with
              | y ->
                  match int (y.[0]- byte '\000')*256+ int (y.[1]- byte '\000') with   //可用char 0，在C#中为(char)0='\0'
                  | z when z < 0xB0A1 -> "*"
                  | z when z < 0xB0C5 -> "a"
                  | z when z < 0xB2C1 -> "b"
                  | z when z < 0xB4EE -> "c"
                  | z when z < 0xB6EA -> "d"
                  | z when z < 0xB7A2 -> "e"
                  | z when z < 0xB8C1 -> "f"
                  | z when z < 0xB9FE -> "g"
                  | z when z < 0xBBF7 -> "h"
                  | z when z < 0xBFA6 -> "g"
                  | z when z < 0xC0AC -> "k"
                  | z when z < 0xC2E8 -> "l"
                  | z when z < 0xC4C3 -> "m"
                  | z when z < 0xC5B6 -> "n"
                  | z when z < 0xC5BE -> "o"
                  | z when z < 0xC6DA -> "p"
                  | z when z < 0xC8BB -> "q"
                  | z when z < 0xC8F6 -> "r"
                  | z when z < 0xCBFA -> "s"
                  | z when z < 0xCDDA -> "t"
                  | z when z < 0xCEF4 -> "w"
                  | z when z < 0xD1B9 -> "x"
                  | z when z < 0xD4D1 -> "y"
                  | z when z < 0xD7FA -> "z"
                  | _ -> "*"
        tempStr

(*

(*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//http://www.cnblogs.com/youxiang/archive/2007/03/08/668199.htmlnamespace WX.Data.ViewModelBase
{
  public class ChineseToPY
  {
    public string GetPYString(string str)
    {
      string tempStr = "";
      foreach (char c in str)
      {
        if ((int)c >= 33 && (int)c <= 126)
        {//字母和符号原样保留
          tempStr += c.ToString();
        }
        else
        {//累加拼音声母
          tempStr += GetPYChar(c.ToString());
        }
      }
      return tempStr;
    }

    public string GetPYChar(string c)
    {
      byte[] array = new byte[2];
      array = System.Text.Encoding.Default.GetBytes(c);
      int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));

      if (i < 0xB0A1) return "*";
      if (i < 0xB0C5) return "a";
      if (i < 0xB2C1) return "b";
      if (i < 0xB4EE) return "c";
      if (i < 0xB6EA) return "d";
      if (i < 0xB7A2) return "e";
      if (i < 0xB8C1) return "f";
      if (i < 0xB9FE) return "g";
      if (i < 0xBBF7) return "h";
      if (i < 0xBFA6) return "g";
      if (i < 0xC0AC) return "k";
      if (i < 0xC2E8) return "l";
      if (i < 0xC4C3) return "m";
      if (i < 0xC5B6) return "n";
      if (i < 0xC5BE) return "o";
      if (i < 0xC6DA) return "p";
      if (i < 0xC8BB) return "q";
      if (i < 0xC8F6) return "r";
      if (i < 0xCBFA) return "s";
      if (i < 0xCDDA) return "t";
      if (i < 0xCEF4) return "w";
      if (i < 0xD1B9) return "x";
      if (i < 0xD4D1) return "y";
      if (i < 0xD7FA) return "z";

      return "*";
    }
  }
}

*)
