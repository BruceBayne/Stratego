﻿module array2d
let Flat array2D = 
    seq { 
    for x in [0..(Array2D.length1 array2D) - 1] do 
     for y in [0..(Array2D.length2 array2D) - 1] do 
      yield array2D.[x, y] 
     }

