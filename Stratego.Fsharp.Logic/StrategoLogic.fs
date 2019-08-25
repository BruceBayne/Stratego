namespace Stratego.Logic 
open StrategoTypes
open XUtils


module StrategoLogic =
 
 
  //anonymous types is BROKEN f*ck  
 type  initialGameConfig =  {
  Count:int
  Rank :FigureRank
 }


 

 let private config = [
                {Rank=Flag;Count=1};
                {Rank=Marshal;Count=1};
                {Rank=Marshal;Count=1};
                {Rank=General;Count=1};
                {Rank=Colonel;Count=2};
                {Rank=Major;Count=3};
                {Rank=Captain;Count=4};
                {Rank=Leitenant;Count=4};
                {Rank=Sergeant;Count=4};
                {Rank=Scout;Count=8};
                {Rank=Miner;Count=8};
                {Rank=Spy;Count=1};
                {Rank=Mine;Count=6};
              ]

 

   

 let private regularTurnFigures = List.map (fun x-> (x,ForwardBackRightLeft)) [Marshal; General; Colonel; Major;Captain; Leitenant; Sergeant ;Miner; Spy] 
 let private specialTurnFigures = List.sort ((Scout, LineMove) :: [(Flag, AlwaysStand); (Mine, AlwaysStand)] @ regularTurnFigures) |> Map.ofList
 let private gameRules = {
                            StartPlayer = Player.Blue; 
                            AllowedTurns= specialTurnFigures; 
                            FiguresStayRevealed=true; FieldSize=(10,10) 
                         }
 



 let private randomRefillSlots : FieldSlot[,] = 
  
  let (x,y)=gameRules.FieldSize;
  let field=Array2D.create x y (Empty)    
  
  Seq.iter (fun x-> 
                   field.[0,1] <- Figure ({Rank=x.Rank; Owner= Red})
                   ()) config

  field

 
 let CreateEmptyField() =   
   let emptyGen=Seq.map (fun _-> Empty)   
   let (maxSizeX,maxSizeY) = gameRules.FieldSize
   Seq.map (fun _-> emptyGen [1..maxSizeX]) [1..maxSizeY] |> array2D  
 

 let StartPredefinedGame() =   
  
  let field=CreateEmptyField()

  field.[0,0] <- Figure ({Rank= Flag; Owner=Blue})
  field.[1,0] <- Figure ({Rank= Scout; Owner=Blue})
  field.[2,0] <- Figure ({Rank= Spy; Owner=Blue})
  field.[3,0] <- Figure ({Rank= Colonel; Owner=Blue})
  field.[4,0] <- Figure ({Rank= Mine; Owner=Blue})
  field.[5,0] <- Figure ({Rank= Miner; Owner=Blue})
  field.[9,9] <- Figure ({Rank= Scout; Owner=Blue})
  
  field.[2,3] <- Obstacle
  field.[2,4] <- Obstacle
  field.[3,3] <- Obstacle
  field.[3,4] <- Obstacle

  field.[6,3] <- Obstacle
  field.[6,4] <- Obstacle
  field.[7,3] <- Obstacle
  field.[7,4] <- Obstacle




  field.[0,9] <- Figure ({Rank= Flag; Owner=Red})
  field.[1,9] <- Figure ({Rank= Scout; Owner=Red})
  field.[2,9] <- Figure ({Rank= Spy; Owner=Red})
  field.[3,9] <- Figure ({Rank= Colonel; Owner=Red})
  field.[4,9] <- Figure ({Rank= Mine; Owner=Red})
  field.[5,9] <- Figure ({Rank= Miner; Owner=Red})
             
  
  {CurrentPlayer= gameRules.StartPlayer; GameField = {Field=field}}
 
 
    
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
  
  let fieldSizeX,fieldSizeY= gameRules.FieldSize
  let (currentX,currentY)= position.Get   
  
  let allPossibleXToEnd=[currentX+1..fieldSizeX]
  let allPossibleXToStart=[currentX-1..-1..0]


  let allPossibleYToTop=   [currentY+1..fieldSizeY] 
  let allPossibleYToBottom=[currentY-1..-1..0] 

  
  let owner = match slots.[currentX,currentY] with
               | Figure f -> Some f.Owner
               | _ -> None
  
     

  let xFunc = fun localX -> FigurePosition.Create(localX,currentY)
  let yFunc = fun localY -> FigurePosition.Create(currentX,localY)
  
  let moves xOrY seq  = Seq.map (fun x-> xOrY x) seq
                     |> Seq.collect (fun o -> Option.toList o)
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
     | Figure f  -> match Map.tryFind f.Rank gameRules.AllowedTurns  with 
                     | Some rule -> Ok rule
                     | None -> Error NoFigureMovementInformation
    

 
 
 
 let public CalculateAvailableMoves(slots : FieldSlot[,]) (position:FigurePosition) =   
  
  let _calcMoves  = getPossibleMoves slots position   
  let _getMovingRule = getMovingRule slots
  
  
  position |> _getMovingRule 
           >>= _calcMoves
          
 
 

 
 let private (|SourceStronger|SourceWeaker|SamePower|) (source: Figure,destination:Figure) =   
  
  if source.Rank   = Marshal && destination.Rank= Spy then SourceWeaker else
  if destination.Rank = Mine && source.Rank<> Miner then SourceWeaker else
  if destination.Rank =  Flag  then SourceStronger else    
  if source.Rank > destination.Rank then SourceStronger 
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
   


 
  

  
  
     
  

  

  
  