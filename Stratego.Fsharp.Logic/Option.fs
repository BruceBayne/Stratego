module Option

let emit (o : 'T option)= 
    if(o.IsSome) then
     seq {yield o.Value}
     else
     seq<'T>[] 
