module List

open System

let shuffle list =
 List.groupBy (fun x->Guid.NewGuid()) list


