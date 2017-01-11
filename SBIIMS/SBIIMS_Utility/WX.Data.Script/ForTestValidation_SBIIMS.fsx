
#r "System.dll"
#r "System.Core.dll"
#r "System.Threading.dll"
#r "System.Configuration.dll"
#r "System.Data.Entity.dll"
#r "System.Runtime.Serialization.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Validation.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Common.dll"
#r "Microsoft.Practices.EnterpriseLibrary.Logging.dll"

open System
open System.Collections.Generic
open System.Linq
open System.Data
open System.Configuration
open System.Data.Objects
open Microsoft.Practices.EnterpriseLibrary.Common
open Microsoft.Practices.EnterpriseLibrary.Validation
open Microsoft.Practices.EnterpriseLibrary.Logging
open Microsoft.Practices.EnterpriseLibrary.Validation.Validators


//It must load on sequence
#I @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.FHelper.dll"
#r "WX.Data.dll"
#r "WX.Data.DataModel.dll"
#r "WX.Data.BusinessBase.dll"
#r "WX.Data.BusinessEntities.dll"
#r "WX.Data.IDataAccess.dll"
#r "WX.Data.DataAccess.dll"

open WX.Data.Helper
open WX.Data.FHelper
open WX.Data.DataModel
open WX.Data.IDataAccess
open WX.Data.DataAccess
open WX.Data.DataOperate
open WX.Data
open WX.Data.BusinessBase
open WX.Data.BusinessEntities
/////////////////////////////////////////////////////////////////////////////////////////////////////////////

([a-zA-Z]+[0-9]*[a-zA-Z0-9\.\*\/\-\b]*[a-zA-Z]*[0-9]*)|([a-zA-Z]+[0-9]*[a-zA-Z0-9\.\*\/\-\b]*[a-zA-Z]+[0-9]*)$


[a-zA-Z]+[a-zA-Z0-9\.\/\-\b]*| [a-zA-Z0-9\.\/\-\b]*[a-zA-Z]+


/////////////////////////////////////////////////////////////////////////////////////////////////////////////
let entity01=new BD_T_GHSDJ()


let entity=new BD_T_GHS()
entity.C_GHJF<-new Nullable<int>("")
let b=ValidationFactory.CreateValidator(entity.GetType(),"")
let x=
    b.Validate(entity)
    |>Seq.iter (fun a->ObjectDumper.Write a)


let sl01=StringLengthValidator(1,64)
sl01.Validate("")

let x01=RegexValidator(@"^(-)?[\d]{2,3}$")
x01.Validate("-203")

let x02=RegexValidator(@"((^$)|(^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$))")
x02.Validate "sc.c"

//[RangeValidator(typeof(decimal),"0.0", RangeBoundaryType.Exclusive, "20.0", RangeBoundaryType.Exclusive, Ruleset = "RuleSetA")]
//public decimal WXDecimal
//{
//  get { return _WXDecimal; }
//  set { _WXDecimal = value; }
//}
Validators.ValidatorCompositionAttribute(CompositionType.Or)
let x02=new StringLengthValidator(255,MessageTemplate="{1}")
x02.Validate(null)
|>fun a->a.IsValid
|>ObjectDumper.Write

let x=RangeValidator(0.0M, RangeBoundaryType.Exclusive, 20.0M, RangeBoundaryType.Exclusive, MessageTemplate = "wx")
decimal "9999999999"

let rv=RangeValidator(-9999999999M,RangeBoundaryType.Exclusive,9999999999M,RangeBoundaryType.Exclusive)
rv.Validate(99999M)
let rxv=RegexValidator(@"^[\d\,\-]{5,}[\d]$",MessageTemplate="The phone number is not right, may be it has some blanks!")
rxv.Validate("12345-2211,233333")
|>fun a->a.IsValid

let entity=new BD_T_GHS()
let b=ValidationFactory.CreateValidator(entity.GetType(),"")
let x=
    b.Validate(entity)
    |>Seq.iter (fun a->ObjectDumper.Write a)

System.Text.RegularExpressions.RegexOptions

let regexValidator01=new Validators.RegexValidator( @"^(?!(00000000-0000-0000-0000-000000000000$))") //正确
let regexValidatorResult01=regexValidator01.Validate("00000000-0000-0000-0000-000000000000") 
regexValidatorResult01.IsValid

let dt=DateTime.Parse("1960-01-01T00:00:00")  //("2000-01-01 00:00:00")
dt.ToUniversalTime()




///////////////////////////////////////////////////////////////////////////////////////////////////////////////

let dateTimeRangeValidator=new DateTimeRangeValidator(DateTime.Parse("2000-01-01 00:00:00"),DateTime.Parse("2099-01-01 00:00:00"))
let dateTimeRangeValidatorResult=dateTimeRangeValidator.Validate(DateTime.Now)
dateTimeRangeValidatorResult.IsValid

//let regexValidator=new RegexValidator(@"^(-)?\d{1,10}(\.\d{0})?$")
let regexValidator=new RegexValidator(@"^(\d{15}|\d{18})$")
let regexValidatorResult=regexValidator.Validate("111111111111111111111")  //111111111111111111
regexValidatorResult.IsValid


//StringLengthValidator(1,-1)
//DateTimeRangeValidator("2000-01-01 00:00:00","2099-01-01 00:00:00")
//let regexValidator=new Validators.RegexValidator( @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
let regexValidator=new Validators.RegexValidator( @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")
let regexValidatorResult=regexValidator.Validate("")
regexValidatorResult.IsValid

//let value=new ValueAccess(
let x=
  new Guid("00000000-0000-0000-0000-000000000001")> new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF")>new Guid("F0000000-0000-0000-0000-000000000000")

let rangeValidator01=new Validators.RangeValidator(new Guid("00000000-0000-0000-0000-000000000000"),Validators.RangeBoundaryType.Exclusive,new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),Validators.RangeBoundaryType.Exclusive)
let rangeValidatorResult01= rangeValidator01.Validate(new Guid("00000000-0000-0000-0000-000000000000"))
rangeValidatorResult01.IsValid

let regexValidator=new Validators.RegexValidator( @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$")

(new Guid("00000000-0000-0000-0000-000000000000")).ToString()

let regexValidator01=new Validators.RegexValidator( @"^00000000-0000-0000-0000-000000000000$",typeof<Guid>) //如何使用？？？
let regexValidatorResult01=regexValidator01.Validate(new Guid("00000000-0000-0000-0000-000000000001"))
regexValidatorResult01.IsValid

let result=regexValidator.Validate("00000000-0000-0000-0000-000000000001")  //("{8496D10F-0C05-41ED-B8EF-E49F22CB97FA}")
result.IsValid

let fieldValueValidator=new Validators.


//
//#load  @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\ChangeTracker.fs"
//open WX.Data.BusinessEntities
//#load  @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\BD_Base.fs"
//open WX.Data.BusinessEntities
//#load  @"D:\Workspace\SBIIMS\WX.Data.BusinessEntities\BD_SBIIMS.fs"
//open WX.Data.BusinessEntities


let entity=new BD_T_GHS_Advance()
let b=ValidationFactory.CreateValidator(entity.GetType(),"")
let x=
    b.Validate(entity)
    |>Seq.filter (fun a->a.Key="C_MCJM")

x
|>Seq.iter (fun b->
  ObjectDumper.Write (b,1))


let c=ValidationFactory.CreateValidatorFromAttributes(entity.GetType(),"")
c
let validatorWrapper = new ValidatorWrapper(c)

ValidatorWrapper
//validatorWrapper.GetType()validatorWrapper.ToString()

//let x=new AndCompositeValidator(
let d=validatorWrapper.GetInstrumentationEventProvider()
ObjectDumper.Write (d,3)

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
let va=new NotNullValidator()
let result=va.Validate(null)
result.IsValid
result
|>Seq.filter (fun a->a.Key="")
|>Seq.head
|>fun a->a.Message

//Microsoft.Practices.EnterpriseLibrary.Validation.ValidationFactory.CreateValidatorFromAttributes(
//Microsoft.Practices.EnterpriseLibrary.Validation.Validation.ValidateFromAttributes(

let va=new NotNullValidatorAttribute()
let result=va.Match("wx")
result
|>Seq.head
|>fun a->a.Message



(*
//http://www.codeproject.com/KB/cs/VAB.aspx
/// <summary>
/// Example business object class that uses 
/// attributes to define validation rules 
/// </summary> 
public class NoConfigPerson 
{ 
[RangeValidator(1, RangeBoundaryType.Inclusive, 500, 
               RangeBoundaryType.Inclusive)] 
private int m_ID; 

private string m_FirstName = null; 
private string m_LastName; 
private string m_Email; 
private DateTime m_DOB; 
private Address m_Address; 
private DateTime m_StartDate; 
private DateTime m_EndDate; 
public NoConfigPerson() 
{ 
} 
#region Properties 
public int ID 
{ 
get { return m_ID; } 
set { m_ID = value; } 
} 
[StringLengthValidator(1, 20)] 
[NotNullValidator] 
public string FirstName 
{ 
get { return m_FirstName; } 
set { m_FirstName = value; } 
} 
[NotNullValidator] 
[StringLengthValidator(1, 30)] 
public string LastName 
{ 
get { return m_LastName; } 
set { m_LastName = value; } 
} 
[NotNullValidator] 
public string Email 
{ 
get { return m_Email; } 
set { m_Email = value; } 
} 
[RelativeDateTimeValidator(18, DateTimeUnit.Year, 25, DateTimeUnit.Year)] 
[DateTimeRangeValidator("1960-01-01T00:00:00", "2007-01-01T00:00:00")] 
public DateTime DOB 
{ 
get { return m_DOB; } 
set { m_DOB = value; } 
} 
[ObjectValidator] 
public Address Address 
{ 
get { return m_Address; } 
set { m_Address = value; } 
} 
[PropertyComparisonValidator("EndDate", ComparisonOperator.LessThan)] 
[PropertyComparisonValidator("DOB", ComparisonOperator.GreaterThan)] 
public DateTime StartDate 
{ 
get { return m_StartDate; } 
set { m_StartDate = value; } 
} 
[PropertyComparisonValidator("StartDate", ComparisonOperator.GreaterThan)] 
public DateTime EndDate 
{ 
get { return m_EndDate; } 
set { m_EndDate = value; } 
} 
#endregion 
} 

*)