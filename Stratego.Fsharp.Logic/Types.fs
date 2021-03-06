module StrategoTypes 
 open System.Runtime.InteropServices

 type Player = Red | Blue
 
 type TurnErrorInfo = NotImplemented | CurrentPlayerError | NoFigureToMove | ObstaclesCantMove | PositionOutOfBounds | NoFigureMovementInformation
 
 type FigureRank =
      | Flag     
      | Spy
      | Miner
      | Scout
      | Sergeant
      | Leitenant
      | Captain
      | Major
      | Colonel
      | General      
      | Marshal
      | Mine
      
      
        
    
 [<StructuralEquality;StructuralComparison>] 
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
 
 type FieldSlot =  
 | Empty 
 | Figure of Figure
 | Obstacle
 with 
  member this.TryGetFigure( [<Out>] result : Figure byref ) =
   match this with
   | Figure f ->
       result <-f
       true
   | _ -> false
 

 type GameField =  {
  Field : FieldSlot[,]   
 }



 type GameInformation ={
  CurrentPlayer: Player
  GameField : GameField
 }

 
 

 type MoveInfo = {
  OldPosition: FigurePosition
  NewPosition : FigurePosition
 }
 
 
 
 type KillInfo = IAmKiller  | MyFigureDead  | BothDead
  
 type TurnSuccessInfo =  
 |JustMoveCase of MoveInfo
 |DeathCase of MoveInfo * KillInfo * FigureRank
  
  //C# jerk off  
  member this.TryGetJustMoveCase( [<Out>] result : MoveInfo byref ) =
   match this with
   | JustMoveCase f ->
       result <-f
       true
   | _ -> false

   //C# jerk off  
   member this.TryGetDeathCase([<Out>] result : (MoveInfo*KillInfo*FigureRank) byref) =
   match this with
   | DeathCase (m,f,k) ->
       result <- (m,f,k)       
       true
   | _ -> false
      

 
 type ExternalTurnResult = {
  GameInfo : GameInformation 
  TurnInfo : TurnSuccessInfo
  }


 //type MoveDirection = Forward | Back | Left | Right   
 // [<System.FlagsAttribute>]
 // type AvailableDirections = Forward = 1 |  Back = 2 | Left = 4 | Right = 8 

  type RuleAllowedTurns =  | AlwaysStand | ForwardBackRightLeft | LineMove
  
  type AllowedTurns = FigureRank * RuleAllowedTurns   

 type GameRules = {
 StartPlayer : Player
 AllowedTurns : Map<FigureRank,RuleAllowedTurns>
 FiguresStayRevealed : bool
 FieldSize :  int*int
 }

 
 
  type MoveIntent = {
   Player :Player
   MovingFrom: FigurePosition
   MoveTo : FigurePosition
  }
 
 
 

