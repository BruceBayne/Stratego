module List

open System
 
let K x y = x

let shuffle list =
 list |> List.groupBy (K (Guid.NewGuid()))   |> Seq.map (fun (_,b)->b.Head) 

let shuffleSeq list =
 list |> Seq.toList  |> shuffle