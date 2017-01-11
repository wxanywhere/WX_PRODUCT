
open System.Text.RegularExpressions
open System

type QueryConditionType=
    | IsDJH
    | IsMCJM  //名称简码
    | IsXM
    | IsXBH
    | IsSL
    | IsJE
    | IsRQ
    | IsMix

let (|Match|_|) pattern input=
  if Regex.IsMatch(input,pattern) then Some(input)
  else None


let Dispatch input=
  match input with
  | Match @"^[a-zA-Z]{2,3}\d{8,}$" x->IsDJH
  | Match @"^[a-zA-Z]" x->IsMCJM
  | Match @"^[\u4e00-\u9fa5]" x->IsXM
  | Match @"^[0-9]{6,}$" x->IsXBH  
  | Match @"^[0-9]{1,5}$" x->IsSL  
  | Match @"^(-)?[0-9]*\.[0-9]*$" x->IsJE
  | Match @"^[\d]{4}-[\d]{1,2}-[\d]{1,2}$" x ->
      match DateTime.TryParse(x) with
      | true, _ ->IsRQ
      | _ -> IsMix
  | _->IsMix

match Dispatch "10000" with
| IsDJH ->"IsDJH"
| IsMCJM->"IsMCJM"
| IsXM->"IsXM"
| IsXBH->"IsXBH"
| IsSL->"IsSL"
| IsJE->"IsJE"
| IsRQ->"IsRQ"
| IsMix->"IsMix"


//Wrong, Active patterns cannot return more than 7 possibilities
let (| IsDJH
    | IsMCJM  //名称简码
    | IsMC
    | IsXBH
    | IsSL
    | IsJE
    | IsRQ
    |IsMix
    |
    ) input=
    match input with
    | Match @"^[a-zA-Z]{2,3}\d{8,}$" x->IsDJH
    | Match @"^[a-zA-Z]" x->IsMCJM
    | Match @"^[\u4e00-\u9fa5]" x->IsMC
    | Match @"^[0-9]{6,}+$" x->IsXBH  
    | Match @"^[0-9]{1,5}+$" x->IsSL  
    | Match @"^(-)?[0-9]*\.[0-9]*$" x->IsJE
    | Match @"^[\d]{4}-[\d]{1,2}-[\d]{1,2}$" x ->
        match DateTime.TryParse(x) with
        | true, _ ->IsRQ
        | _ -> IsMix
    | _->IsMix

let x02 input=
  match input with 
  | IsStartsWithEnglishLetter-> "IsStartsWithEnglishLetter"
  | IsStartsWithChinese-> "IsStartsWithChinese"
  | IsInt-> "IsInt"
  | IsFloat-> "IsFloat"
  | IsDateTime-> "IsDateTime"
  | _ ->"None1"

x02 "2010-02-28"

let (|Match|_|) pattern input=
  if Regex.IsMatch(input,pattern) then Some(input)
  else None

let (| IsStartsWithEnglishLetter
    | IsStartsWithChinese
    | IsInt
    | IsFloat
    | IsDateTime
    |IsMix
    |
    ) input=
    match input with
    | Match @"^[a-zA-Z]" x->IsStartsWithEnglishLetter
    | Match @"^[\u4e00-\u9fa5]" x->IsStartsWithChinese
    | Match @"^[0-9]+$" x->IsInt  
    | Match @"^[0-9]*\.[0-9]*$" x->IsFloat
    | Match @"^[\d]{4}-[\d]{1,2}-[\d]{1,2}$" x ->
        match DateTime.TryParse(x) with
        | true, _ ->IsDateTime
        | _ -> IsMix
    | _->IsMix

let x02 input=
  match input with 
  | IsStartsWithEnglishLetter-> "IsStartsWithEnglishLetter"
  | IsStartsWithChinese-> "IsStartsWithChinese"
  | IsInt-> "IsInt"
  | IsFloat-> "IsFloat"
  | IsDateTime-> "IsDateTime"
  | _ ->"None1"

x02 "2010-02-28"


let (|Match|) pattern input=
  if Regex.IsMatch(input,pattern) then true
  else false

let (|IsStartsWithEnglishLetter| IsStartsWithChinese| IsInt| IsFloat| IsDateTime|IsMix|) input=
    match input with
    | Match @"^[a-zA-Z]" true->IsStartsWithEnglishLetter
    | Match @"^[\u4e00-\u9fa5]" true->IsStartsWithChinese
    | Match @"^[0-9]+$" true->IsInt  
    | Match @"^[0-9]*\.[0-9]*$" true->IsFloat
    | (Match @"^[\d]{4}-[\d]{1,2}-[\d]{1,2}$" true) ->IsDateTime
    (* 2月时有问题
    | Match @"((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|
                                   (^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|
                                   (^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|
                                   (^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|
                                   (^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))" true ->IsDateTime
    *)
    | _->IsMix

let x02 input=
  match input with 
  | IsStartsWithEnglishLetter-> "IsStartsWithEnglishLetter"
  | IsStartsWithChinese-> "IsStartsWithChinese"
  | IsInt-> "IsInt"
  | IsFloat-> "IsFloat"
  | IsDateTime-> "IsDateTime"
  | _ ->"None1"

x02 "2010.02.22"


let (|Match|_|) pattern input=
  if Regex.IsMatch(input,pattern) then Some(input.ToCharArray())  //Some的参数是任意的
  else None

let (|IsStartsWithEnglishLetter| IsStartsWithChinese| IsInt| IsFloat| IsDateTime|IsMix|) input=
    match input with
    | Match @"^[a-zA-Z]" x->IsStartsWithEnglishLetter       //x是  Match some到的值
    | Match @"^[\u4e00-\u9fa5]" x->IsStartsWithChinese
    | Match @"^[0-9]+$" x->IsInt  
    | Match @"^[0-9]*\.[0-9]*$" x->IsFloat
    | Match @"^[\d]{4}-[\d]{1,2}-[\d]{1,2}$" x->IsDateTime
    | _->IsMix


let (|Match|_|) pattern input=
  if Regex.IsMatch(input,pattern) then Some()
  else None

let (|IsStartsWithEnglishLetter| IsStartsWithChinese| IsInt| IsFloat| IsDateTime|IsMix|) input=
    match input with
    | Match @"^[a-zA-Z]"->IsStartsWithEnglishLetter
    | Match @"^[\u4e00-\u9fa5]"->IsStartsWithChinese
    | Match @"^[0-9]+$"->IsInt  
    | Match @"^[0-9]*\.[0-9]*$"->IsFloat
    | Match @"^[\d]{4}-[\d]{1,2}-[\d]{1,2}$"->IsDateTime
    | _->IsMix

//相当于对String的方法扩展
let x01 input=
  match input with 
  | IsStartsWithEnglishLetter-> "IsStartsWithEnglishLetter"
  | IsStartsWithChinese-> "IsStartsWithChinese"
  | IsInt-> "IsInt"
  | IsFloat-> "IsFloat"
  | IsDateTime-> "IsDateTime"
  | _ ->"None1"

x01 "1周"


let (|RegexMatch3|_|) (pattern : string) (input : string) =
  let result = Regex.Match(input, pattern)
  if result.Success then
    match (List.tail [ for g in result.Groups -> g.Value ]) with
    | fst :: snd :: trd :: []
        -> Some (fst, snd, trd)
    | [] -> failwith <| "Match succeeded, but no groups found.\n" + "Use '(.*)' to capture groups"
    | _ -> failwith "Match succeeded, but did not find exactly three groups."
  else
    None

let parseTime input =
  match input with
  // Match input of the form "6/20/2008"
  | RegexMatch3 "(\d+)/(\d+)/(\d\d\d\d)" (month, day, year)
  // Match input of the form "2004-12-8"
  | RegexMatch3 "(\d\d\d\d)-(\d+)-(\d+)" (year, month, day)
      -> Some( new DateTime(int year, int month, int day) )
  | _ -> None