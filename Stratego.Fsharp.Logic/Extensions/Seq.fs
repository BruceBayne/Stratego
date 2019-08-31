module Seq
 
 //let ofType<'a> (items: _ seq)= items |> Seq.choose(fun i -> match box i with | :? 'a as a -> Some a |_ -> None)

  
 let ofType<'a> (source : System.Collections.IEnumerable) : seq<'a> =    
    seq {
       for item in source do
          match item with             
             | x when (x :? 'a)  ->
                                     yield downcast x
             |_ -> ()
    }
