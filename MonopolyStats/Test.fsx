#load "Types.fs"
open Monopoly

#load "Data.fs"
open Monopoly.Data
open System

#load "Controller.fs"
open Monopoly

let printMove (index,move) = sprintf "%d %A %s %s (doubles %d)" index move.Rolled move.MovementType (Controller.GetName(move.MovingTo)) move.DoubleCount

let controller = Controller()
let history = controller.PlayGame(10000)
              |> Seq.toArray

//for item in history do
//    printfn "%A, %s %A" item.Rolled item.MovementType item.MovingTo

let diceCombos x = x
                   |> Seq.map(fun h -> h.Rolled)
                   |> Seq.choose(id)
                   |> Seq.map(fun (a,b) -> if b > a then b,a else a,b)

diceCombos history
|> Seq.map(fun (a,b) -> a + b)
|> Seq.groupBy(fun h -> h)
|> Seq.map(fun (g,c) -> g, c |> Seq.length)
|> Seq.sortBy(fun (_,c) -> -c)
|> Seq.toArray

diceCombos history
|> Seq.groupBy(fun h -> h)
|> Seq.map(fun (g,c) -> g, c |> Seq.length)
|> Seq.sortBy(fun (_,c) -> -c)
|> Seq.toArray