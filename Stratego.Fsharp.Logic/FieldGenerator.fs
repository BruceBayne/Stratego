module FieldGenerator

open StrategoTypes
open Game

let private cellGenerator maxX minY maxY = seq { 
     for x in [0..maxX] do
      for y in [minY..maxY] do
       yield (x,y)    
     }
 
let private CreateEmptyField() =   
 let emptyGen=Seq.map (fun _-> Empty)   
 let (maxSizeX,maxSizeY) = GameRules.FieldSize
 Seq.map (fun _-> emptyGen [1..maxSizeX]) [1..maxSizeY] |> array2D  



let StartGame gameField = 
 {CurrentPlayer= GameRules.StartPlayer; GameField = {Field=gameField}}

let CreateRandomBoard maxFigures =
 let field = CreateEmptyField()
 let (maxSizeX,_) = GameRules.FieldSize
 let horizontalGenerator = cellGenerator maxSizeX
  
 let GenerateFigures player allowedPositions= 
  let figures= Seq.collect (fun f->  [for _ in 1..f.Count -> {Owner=player; Rank=f.Rank}]) config                
               |> List.shuffleSeq
               |> Seq.take maxFigures              
  
  Seq.iter (fun t->
                    let (x,y),figure = t
                    field.[x,y] <- Figure figure
                    ()) <|  Seq.zip allowedPositions figures  
                    |> ignore
                      
 GenerateFigures Player.Blue (horizontalGenerator 0 3)
 GenerateFigures Player.Red (horizontalGenerator 6 9)
 field

 


let CreatePredefinedBoard() =   

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
             
 field