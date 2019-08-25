module FieldGenerator

open StrategoTypes
open Game

let CreateEmptyField() =   
  let emptyGen=Seq.map (fun _-> Empty)   
  let (maxSizeX,maxSizeY) = GameRules.FieldSize
  Seq.map (fun _-> emptyGen [1..maxSizeX]) [1..maxSizeY] |> array2D  

let CreateRandomBoard maxFigures=
 let field = CreateEmptyField()
 

 let createFiguresForPlayer player = Seq.collect (fun f->  [for _ in 0..f.Count -> {Owner=player; Rank=f.Rank}]) config

 let blueFigures= createFiguresForPlayer Player.Blue |> List.shuffleSeq
 let redFigures= createFiguresForPlayer Player.Red |> List.shuffleSeq
 let (maxSizeX,maxSizeY) = GameRules.FieldSize
  
 
 
 Seq.indexed redFigures |> Seq.iter (fun (index,figure)-> field.[index % 10,1] <- Figure {Owner= Red; Rank=Colonel}) 

 

 0



let private randomRefillSlots : FieldSlot[,] =   
 let (x,y)=GameRules.FieldSize;
 let field=Array2D.create x y (Empty)    
  
 Seq.iter (fun x-> 
                   field.[0,1] <- Figure ({Rank=x.Rank; Owner= Red})
                   ()) config

 field
   




let StartPredefinedGame() =   
 
 let field=CreateEmptyField()

 field.[0,0] <- Figure ({Rank= Flag; Owner=Blue})
 field.[1,0] <- Figure ({Rank= Scout; Owner=Blue})
 field.[2,0] <- Figure ({Rank= Spy; Owner=Blue})
 field.[3,0] <- Figure ({Rank= Colonel; Owner=Blue})
 field.[4,0] <- Figure ({Rank= Mine; Owner=Blue})
 field.[5,0] <- Figure ({Rank= Miner; Owner=Blue})

 field.[5,1] <- Figure ({Rank= Mine; Owner=Red})

 field.[9,9] <- Figure ({Rank= Scout; Owner=Blue})
 
 field.[2,4] <- Obstacle
 field.[2,5] <- Obstacle
 field.[3,4] <- Obstacle
 field.[3,5] <- Obstacle

 field.[6,4] <- Obstacle
 field.[6,5] <- Obstacle
 field.[7,4] <- Obstacle
 field.[7,5] <- Obstacle




 field.[0,9] <- Figure ({Rank= Flag; Owner=Red})
 field.[1,9] <- Figure ({Rank= Scout; Owner=Red})
 field.[2,9] <- Figure ({Rank= Spy; Owner=Red})
 field.[3,9] <- Figure ({Rank= Colonel; Owner=Red})
 field.[4,9] <- Figure ({Rank= Mine; Owner=Red})
 field.[5,9] <- Figure ({Rank= Miner; Owner=Red})
            
 
 {CurrentPlayer= GameRules.StartPlayer; GameField = {Field=field}}