namespace UnitTestProject1

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open FsCheck

module Probes =

let some (i:int) =  
 if i < 5 then
  true
 else
  false
     
[<TestClass>]
type TestClass () =

    [<TestMethod>]
    member this.TestMethodPassing () =     
     Check.QuickThrowOnFailure some
     
