module Some
open StrategoTypes

  
//let MakeMove (x:int,y:int) direction = 
let foo2 = seq { yield 0; } 
let foo = seq { yield 1; yield! foo2 } 
 
// 0
 
//let someFunc() =
//  let some = [ { Rank = Flag; Owner = Blue } ];
//  let s = Array2D.create 8 8 { Rank = Flag; Owner = Red }
    
//  let i = Array2D.init 10 10
//  let z = i (fun x y -> { Rank = Scout; Owner = Blue })

//  let my2DArray = array2D [
//      [
//          [ { Rank = Scout; Owner = Blue } ]
//      ]
//      [
//          [ { Rank = Scout; Owner = Red } ]
//      ]
//  ]

//  0