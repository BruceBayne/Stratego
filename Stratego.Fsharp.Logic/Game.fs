module Game
open StrategoTypes


//anonymous types is BROKEN f*ck  
type  initialGameConfig =  {
 Count:int
 Rank :FigureRank
}

let internal config = [
               {Rank=Flag;Count=1};
               {Rank=Marshal;Count=1};
               {Rank=General;Count=1};
               {Rank=Colonel;Count=2};
               {Rank=Major;Count=3};
               {Rank=Captain;Count=4};
               {Rank=Leitenant;Count=4};
               {Rank=Sergeant;Count=4};
               {Rank=Scout;Count=8};
               {Rank=Miner;Count=5};
               {Rank=Spy;Count=1};
               {Rank=Mine;Count=6};
             ]



let private regularTurnFigures = List.map (fun x-> (x,ForwardBackRightLeft)) [Marshal; General; Colonel; Major;Captain; Leitenant; Sergeant ;Miner; Spy] 
let private specialTurnFigures = List.sort ((Scout, LineMove) :: [(Flag, AlwaysStand); (Mine, AlwaysStand)] @ regularTurnFigures) |> Map.ofList
let internal GameRules = {
                           StartPlayer = Player.Blue; 
                           AllowedTurns= specialTurnFigures; 
                           FiguresStayRevealed=true; FieldSize=(10,10) 
                        }
