

type T00()=
  class end

type T01()=
  inherit T00()

type T02()=
  inherit T01()

type T03()=
  member val X:T02=Unchecked.defaultof<_> with get,set
  member val Xs:T02[]=Unchecked.defaultof<_> with get,set
  member val X1:int=0 with get,set

let t03=new T03()
t03.X<-new T02()
t03.X1<- 1
t03.GetType().GetProperty("X").PropertyType.IsAssignableFrom(typeof<T02>)
typeof<T00>.IsAssignableFrom(t03.GetType().GetProperty("X").PropertyType)
typeof<T01>.IsAssignableFrom(t03.GetType().GetProperty("X").PropertyType)
typeof<T02>.IsAssignableFrom(t03.GetType().GetProperty("X").PropertyType)
typeof<T03>.IsAssignableFrom(t03.GetType().GetProperty("X").PropertyType)

typeof<T01[]>.IsAssignableFrom(t03.GetType().GetProperty("Xs").PropertyType)

t03.GetType().GetProperty("X").PropertyType.IsSubclassOf(typeof<T00>)
t03.GetType().GetProperty("X").PropertyType.IsSubclassOf(typeof<T01>)
t03.GetType().GetProperty("X").PropertyType.IsSubclassOf(typeof<T02>)


let p01=t03.GetType().GetProperty("X")

t03.GetType().GetProperty("X").PropertyType.

match p01.GetValue(t03) with
| :? T01 ->String.Empty
| _ ->"wx"
