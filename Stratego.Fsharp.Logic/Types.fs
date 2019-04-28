module StrategoTypes 

 type Player = Red | Blue
 
 type FigureRank =
      | Flag
      | Marshal
      | General
      | Colonel
      | Major
      | Captain
      | Leitenant
      | Sergeant
      | Scout
      | Miner
      | Spy
      | Mine




 type FigurePosition =
     private | FigurePosition of int * int

     static member Create (x:int,y:int) : Option<FigurePosition> =
      
      match (x,y) with 
      |(x,y) when x < 0 || y <  0 -> None
      |(x,y) when x > 9 || y > 9 -> None      
      |(x,y) -> Some(FigurePosition(x,y))
      
     member this.Get : int =
         this |> fun (FigurePosition (x,y)) -> 0     



 type Figure = {  
  Owner : Player
  Rank : FigureRank 
 }
 
 
 type FigureSlot =  
 | Empty 
 | Figure of Figure


 type GameField = {
  Field : FigureSlot[,]   
 }

 type GameInformation =  
 | Turn of Player * GameField 
 | Winner of Player
 

 type MoveDirection = Forward | Left | Right
 type TurnInformation = MoveDirection * FigurePosition