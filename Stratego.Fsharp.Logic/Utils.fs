﻿module XUtils
 
let (>>=) m f  =
 Result.bind f m
 

let (>!) m f  =
  match m with
  | Error x -> 
     Error x
  | Ok x -> f x
 

let (>!!) m f  =
  match m with
  | Error x -> 
     Error x
  | Ok x -> f
 
 
let bind = (>>=)

let (>>>=) m f  =
 match m with
 | Some x -> f x     
 | None -> None
 
