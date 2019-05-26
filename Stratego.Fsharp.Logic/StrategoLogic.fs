

namespace Stratego.Logic 
open StrategoTypes
open Utils


module StrategoLogic =
  
 let private initialConfiguration = 
   [ 
    {|Rank=Flag; Count=1|}; 
    {|Rank=Marshal ;   Count=1|};
    {|Rank=General ;   Count=1|}; 
    {|Rank=Colonel ;   Count=2|};
    {|Rank=Major   ;   Count=3|};
    {|Rank=Captain ;   Count=4|};    
    {|Rank=Leitenant ; Count=4|};
    {|Rank=Sergeant ;  Count=4|};        
    {|Rank=Scout   ;   Count=8|};
    {|Rank=Miner   ;   Count=8|};
    {|Rank=Spy   ;     Count=1|};
    {|Rank=Mine   ;    Count=6|};
   ]  
 
   

 let private turnBased = List.map (fun x-> (x,ForwardBackRightLeft)) [Marshal; General; Colonel; Major;Captain; Leitenant; Sergeant ;Miner; Spy] 
 let private allowedTurns = List.sort ((Scout, LineMove) :: [(Flag, AlwaysStand); (Mine, AlwaysStand)] @ turnBased) |> Map.ofList
 let private gameRules = {StartPlayer = Player.Blue; AllowedTurns= allowedTurns; FiguresStayRevealed=true; FieldSize=(10,10) }

 let StartNewGame() = 
  let figureArray = 
   array2D [            
             [
               Figure {Owner= Blue; Rank=Flag }
               Figure {Owner= Blue; Rank=Spy }
             ];              
             [
                 Figure {Owner= Red; Rank=Flag }
                 Figure {Owner= Red; Rank=Spy }
             ]; 

           ]           
  {CurrentPlayer= gameRules.StartPlayer; GameField = {Field=figureArray}}

 
 let (|OutOfGameFieldBounds|_|) i =  
  let (x,y)=i
  if x < 0 || y<0 || i > gameRules.FieldSize then Some(i) else None

 
 let private checkDeskBounds (movePosition:FigurePosition) = 
  match movePosition.Get with
   | OutOfGameFieldBounds _ -> Error (PositionOutOfBounds)
   | _ -> Ok movePosition


 


 let private makeActualMove (slots : FieldSlot[,]) (position:FigurePosition) rule =   
  Ok (JustMoveCase {OldPosition= position; NewPosition=position} )


 let private generateLineSeq (position:FigurePosition) =
  seq {
   yield position
   }
 

 let private fbrl (position:FigurePosition) =
  seq {
  yield position
  }

 let private calcMoves (slots : FieldSlot[,]) (position:FigurePosition) rule =     
  let s=seq<FigurePosition>[]

  let (x,y) = position.Get
  
  match (slots.[x,y]) with
   | Figure movingFigure -> 
                match rule with 
                 | LineMove -> Ok (generateLineSeq position)
                 | AlwaysStand -> Ok (seq<FigurePosition>[]) //empty
                 | ForwardBackRightLeft -> 
                             let ((x1,y1),(x2,y2)) = (1,2),(3,4)
                             match slots.[x1,x2] with 
                               |Empty -> 
                                 Ok (seq {
                                  yield! s
                                 })
                               | Obstacle ->Ok s
                               | Figure targetCellFigure 
                                  when movingFigure.Owner <> targetCellFigure.Owner -> Ok s
   | _ -> Error NoFigureToMove


  
  

 let private getMovingRule (slots : FieldSlot[,]) (position:FigurePosition) =
  let (x,y) = position.Get
  match slots.[x,y] with 
     | Empty -> Error NoFigureToMove
     | Obstacle -> Error ObstaclesCantMove
     | Figure f  -> match Map.tryFind f.Rank gameRules.AllowedTurns  with 
                     | Some rule -> Ok rule
                     | None -> Error NoFigureMovementInformation
    

 
 
 
 let public CalculateAvailableMoves(slots : FieldSlot[,]) (position:FigurePosition) =   
 
  let _calcMoves  = calcMoves slots position   
  let _getMovingRule = getMovingRule slots
  position |> checkDeskBounds 
           >>= _getMovingRule 
           >>= _calcMoves
           

 let MakeMove (gameInfo : GameInformation) (moveIntent:MoveIntent) =       
  if (gameInfo.CurrentPlayer<> moveIntent.Player) then
    CurrentPlayerError
   else  
         
   //check figure type  for desired move type (Flag cant move e.t.c)
   //   
    NotImplemented

  
  