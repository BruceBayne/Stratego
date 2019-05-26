module Utils

//let (>>=) m f  =
//  match m with
//  | Error x -> 
//      x
//  | Ok x -> 
//      x |> f
 
let (>>=) m f  =
 match m with
 | Error x -> 
     Error x
 | Ok x -> f x
 
 
let bind = (>>=)


 
