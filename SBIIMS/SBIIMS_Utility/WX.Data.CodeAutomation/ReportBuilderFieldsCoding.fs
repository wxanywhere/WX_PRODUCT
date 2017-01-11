namespace WX.Data.CodeAutomation

open System
open System.Linq
open System.Text
open Microsoft.FSharp.Linq
open FSharp.Collections.ParallelSeq
open WX.Data.Helper
open WX.Data
open WX.Data.CodeAutomationHelper

type ReportBuilderFieldsCoding=
  static member GetCode (columnConditions:(string*string*bool*string) seq)=  
    let isValidated()=
      let flag=ref true
      columnConditions
      |>Seq.iteri (fun i (a,b,_,_)->
          if TypeShortNames|>Seq.exists (fun c->b=c)|>not then
            ObjectDumper.Write ("----------------------------------------------") 
            ObjectDumper.Write (string (i+1)+" of elements has some problems , The type name of field ["+a+"] is not right!") 
            flag:=false
          )
      !flag
    match isValidated() with
    | false ->
        ObjectDumper.Write ("----------------------------------------------") 
        "Wrong!"  
    | _ ->
        let sb=new StringBuilder()
        sb.Clear()|>ignore
        sb.Append(@"
      <Fields>")|>ignore
        for (a,b,_,_) in columnConditions do
          sb.AppendFormat (@"
        <Field Name=""{0}"">
          <DataField>{0}</DataField>
          <rd:TypeName>{1}</rd:TypeName>
        </Field>",
            a,TypeShortLongDic.[b])|>ignore
        sb.Append(@"
      </Fields>")|>ignore
        sb.Remove(0,sb.ToString().IndexOf("\n")+1)|>ignore
        sb.ToString()
