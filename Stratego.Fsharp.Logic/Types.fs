module StrategoTypes 

 type Player = Red | Blue
 
 type MoveErrorCodes = NotImplemented | NoFigureToMove | ObstaclesCantMove | PositionOutOfBounds
 
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
     
     member this.Forward  =
      this |> fun (FigurePosition (x,y)) -> 
       FigurePosition.Create (x+1,y)
     
      member this.Get  =
         this |> fun (FigurePosition (x,y)) -> (x,y)     



 type Figure = {  
  Owner : Player
  Rank : FigureRank

 }
 
 
 type FigureSlot =  
 | Empty 
 | Figure of Figure
 | Obstacle


 type GameField =  {
  Field : FigureSlot[,]   
 }

 
 type GameInformation =  
 | CurrentGameState of Player * GameField 
 | Winner of Player
 

 type MoveResult = 
  | FigureDead of Figure * GameField
  | TurnSuccess of GameField * Figure * FigurePosition
  | TurnNotAllowed of MoveErrorCodes



 
 type MoveDirection = Forward | Left | Right
 [<System.FlagsAttribute>]
 type AvailableDirections = Forward = 1 | Left = 2 | Right = 4 

 type MoveIntent = {CurrentPosition: FigurePosition ; Direction: MoveDirection}