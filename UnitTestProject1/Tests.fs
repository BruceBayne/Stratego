namespace UnitTestProject1

open Microsoft.VisualStudio.TestTools.UnitTesting
open FsCheck
open StrategoTypes

module Probes =

    let some (i:int) =  
     
     let marshalCount=FieldGenerator.CreateRandomBoard 40
                        |> array2d.Flat
                        
                        //|> Seq.ofType<Figure>
                        //|> Seq.where (fun p-> p.Owner = Blue)
                        //|> Seq.where (fun x-> x.Rank = Marshal)
                        //|> Seq.length               

     
     //marshalCount = 1 
     true
     
     
     
    [<TestClass>]
    type TestClass () =

        [<TestMethod>]
        member this.TestMethodPassing () =     
         Check.QuickThrowOnFailure some
     
