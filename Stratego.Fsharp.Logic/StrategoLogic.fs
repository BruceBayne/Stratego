﻿namespace Stratego.Logic 
open StrategoTypes
open XUtils
open Game

module StrategoLogic =
 
 
   


 
 
    
 let private getFigure (slots : FieldSlot[,]) (position:FigurePosition) =  
  let (x,y) = position.Get  
  match slots.[x,y] with
  | Figure f ->Some f
  | _ -> None

   

  
 let private getFrontBackLeftRightMove (position:FigurePosition) =
  seq {   
   let (x,y)= position.Get   
   
   let generatedMoves=[
    FigurePosition.Create (x+1, y);    
    FigurePosition.Create (x-1, y);
    FigurePosition.Create (x, y+1);
    FigurePosition.Create (x, y-1);
   ]  
   yield! Seq.collect (fun x-> Option.toList x) generatedMoves   
  }

 
 let private getMovesForScout (slots:FieldSlot[,]) (position:FigurePosition) =
  
  let fieldSizeX,fieldSizeY= GameRules.FieldSize
  let (currentX,currentY)= position.Get   
  
  let allPossibleXToEnd=[currentX+1..fieldSizeX]
  let allPossibleXToStart=[currentX-1..-1..0]


  let allPossibleYToTop=   [currentY+1..fieldSizeY] 
  let allPossibleYToBottom=[currentY-1..-1..0] 

  
  let owner = match slots.[currentX,currentY] with
               | Figure f -> Some f.Owner
               | _ -> None
  
     

  let xFunc localX = FigurePosition.Create(localX,currentY)  
  let yFunc localY = FigurePosition.Create(currentX,localY)
  
  let moves xOrY seq  = Seq.map xOrY seq
                     |> Seq.collect Option.toList
                     |> Seq.takeWhile (fun (t : FigurePosition)->
                      let (nx,ny) = t.Get
                      match slots.[nx,ny] with 
                       | Empty -> true
                       | Figure f when owner.IsSome && f.Owner<> owner.Value -> true
                       | _ -> false
                       )
  
  seq {
  yield! moves xFunc allPossibleXToEnd
  yield! moves xFunc allPossibleXToStart

  yield! moves yFunc allPossibleYToTop
  yield! moves yFunc allPossibleYToBottom   
  }



 let private getPossibleMoves (slots : FieldSlot[,]) (position:FigurePosition) rule =     

  
  let (x,y) = position.Get
  
  match (slots.[x,y]) with
   | Figure movingFigure -> 
        match rule with 
         | LineMove -> Ok(getMovesForScout slots position)
         | AlwaysStand -> Ok (Seq.empty)
         | ForwardBackRightLeft  -> Ok(seq{         
          let fbrlPos= getFrontBackLeftRightMove position                    
          for some in fbrlPos do
           let (nx,ny)=some.Get
           match slots.[nx,ny] with                    
           | Figure f when f.Owner <> movingFigure.Owner  
            -> yield some           
           | Empty -> yield some
           | c -> c |> ignore
         })                                  
                    
   | _ -> Error NoFigureToMove

   
  

 let private getMovingRule (slots : FieldSlot[,]) (position:FigurePosition) =
  let (x,y) = position.Get
  match slots.[x,y] with 
     | Empty -> Error NoFigureToMove
     | Obstacle -> Error ObstaclesCantMove
     | Figure f  -> match Map.tryFind f.Rank GameRules.AllowedTurns  with 
                     | Some rule -> Ok rule
                     | None -> Error NoFigureMovementInformation
    

 
 
 
 let public CalculateAvailableMoves(slots : FieldSlot[,]) (position:FigurePosition) =   
  
  let _calcMoves  = getPossibleMoves slots position   
  let _getMovingRule = getMovingRule slots
  
  
  position |> _getMovingRule 
           >>= _calcMoves
          
 
 

 
 let private (|SourceStronger|SourceWeaker|SamePower|) (source: Figure,destination:Figure) =   
  
  if source.Rank = Marshal && destination.Rank = Spy  then SourceStronger
  elif source.Rank = Spy && destination.Rank=Marshal then SourceStronger
  elif source.Rank = Miner && destination.Rank = Mine then SourceStronger    
  elif source.Rank > destination.Rank then SourceStronger 
  elif source.Rank < destination.Rank then SourceWeaker
  else
  SamePower
    

  

 let private moveFigure (slots : FieldSlot[,])  moveIntent sourceFigure =   
  let (x,y) = moveIntent.MoveTo.Get  
    
  let turnInfo ={OldPosition= moveIntent.MovingFrom; NewPosition=moveIntent.MoveTo };
  
  match slots.[x,y] with  
  | Empty  -> Ok (JustMoveCase turnInfo )  
  | Figure destinationFigure -> match (sourceFigure,destinationFigure) with 
                                | SourceStronger -> Ok (DeathCase(turnInfo, IAmKiller , destinationFigure.Rank))
                                | SourceWeaker -> Ok (DeathCase(turnInfo, MyFigureDead , destinationFigure.Rank))
                                | SamePower -> Ok (DeathCase(turnInfo, BothDead, destinationFigure.Rank))
  | _ -> Error NoFigureToMove
  
 
 let private GetPlayerFigure currentPlayer slots position =   
  let ownFigure = getFigure slots position
  match ownFigure with 
  | Some figure when figure.Owner <> currentPlayer-> Error CurrentPlayerError
  | Some figure when figure.Owner = currentPlayer-> Ok (figure)
  | _ -> Error NoFigureToMove

 
 

 let private updateGameInfo(gameInfo : GameInformation) (moveIntent:MoveIntent) (turnInfo:TurnSuccessInfo)=
  
  let  newPlayer = if gameInfo.CurrentPlayer = Red then Blue else Red    
  let clone=Array2D.copy gameInfo.GameField.Field  
  let (old_x,old_y) = moveIntent.MovingFrom.Get
  let (new_x,new_y) = moveIntent.MoveTo.Get

  match turnInfo with 
               | JustMoveCase _ -> clone.[new_x,new_y] <- clone.[old_x,old_y]
                                   clone.[old_x,old_y] <- Empty                                                                                      
               | DeathCase (_,killInfo,_) ->                                                        
                                               match killInfo with 
                                               | IAmKiller -> clone.[new_x,new_y] <- clone.[old_x,old_y]
                                                              clone.[old_x,old_y] <- Empty                                                                                      
                                               | MyFigureDead -> clone.[old_x,old_y] <- Empty                                                                                      
                                               | BothDead  -> clone.[old_x,old_y] <- Empty  
                                                              clone.[new_x,new_y] <- Empty    

  let newDoska = {CurrentPlayer = newPlayer; GameField = {Field= clone}}
  let turnInfo = {GameInfo = newDoska; TurnInfo= turnInfo}
  Ok(turnInfo)

 
 
 
 let private validateMoveIntent moveIntent validMoves= 
   if Seq.contains moveIntent.MoveTo validMoves  then
    Ok true
   else
    Error NoFigureToMove
  

 let MakeMove (gameInfo : GameInformation) (moveIntent:MoveIntent) =                    
  
  let slots = gameInfo.GameField.Field   
  let getFigure = GetPlayerFigure gameInfo.CurrentPlayer slots moveIntent.MovingFrom 
  let moveFigure = moveFigure slots moveIntent
  let getAvailableMoves = CalculateAvailableMoves slots moveIntent.MovingFrom    
  let updateGameInfo = updateGameInfo  gameInfo moveIntent  
  let _validate= validateMoveIntent moveIntent  

  getAvailableMoves >! _validate >!! getFigure >>= moveFigure >>= updateGameInfo
   


 
  

  
  
     
  

  

  
  