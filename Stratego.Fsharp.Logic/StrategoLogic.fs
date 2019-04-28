

namespace Stratego.Logic 
open StrategoTypes
open System


module StrategoLogic =
  

 let some = 
  
  let initialConfiguration = 
   [ 
    {|Rank=Flag ;      Count=1|} 
    {|Rank=Marshal ;   Count=1|};
    {|Rank=General ;   Count=1|}; 
    {|Rank=Colonel ;   Count=2|};
    {|Rank=Major   ;   Count=3|};
    {|Rank=Captain ;   Count=4|};
    {|Rank=Leitenant ; Count=5|};
    {|Rank=Sergeant ;  Count=6|};
    {|Rank=Scout   ;   Count=7|};
    {|Rank=Miner   ;   Count=8|};
    {|Rank=Spy   ;     Count=1|};
    {|Rank=Mine   ;    Count=9|};
   ]
  
    

  let shuffled = List.shuffle initialConfiguration  
 
  //0,1,2,3 6,7,8,9
  let i = Array2D.init 10 10 (fun i i2 -> 0)
  //let z = i (fun x y -> { Rank = Scout; Owner = Blue })
  
  
  let func=Array2D.map (fun i i2 -> 0) 




  //let x=List.map (fun e-> 1) z


  let result = List.map (fun element-> 0) initialConfiguration
  result
 

 


 let InitializeGame () = 
  let boardArray = Array2D.init 10 10
  let z = boardArray (fun x y -> FigureSlot.Figure { Rank = Scout; Owner = Blue;  })
 
  let board = {Field = z}
  let gf = Turn (Red,board)
  gf
  


 let CalculateTurn player deskState= 
  
  Turn(player,deskState)
  


 let MakeMove (gameInformation:GameInformation) (turnInfo:TurnInformation) =   
  match gameInformation with
  | Turn (player,field) -> CalculateTurn player field   
 

 //let MakeMove (gameInformation:GameInformation) (turnInfo:TurnInformation) =   
 // match gameInformation with
 // | Turn (player,field) -> CalculateTurn player field
 // | Winner p -> gameInformation