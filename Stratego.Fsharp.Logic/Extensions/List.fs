module List

open System
 
let shuffle list =
 list |> List.sortBy (fun _ -> (Guid.NewGuid()))
let shuffleSeq list =
 list |> Seq.toList  |> shuffle