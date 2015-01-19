#load "Types.fs"
open Monopoly

#load "Data.fs"
open Monopoly.Data
open System

#load "Controller.fs"
open Monopoly

let printMove index move = sprintf "%d: %A %s %s (doubles %d)" index move.Rolled move.MovementType (Controller.GetName(move.MovingTo)) move.DoubleCount

let controller = Controller()
let history = controller.PlayGame(100) |> Seq.toArray

for item in history do
    printfn "%A, %s %A" item.Rolled item.MovementType item.MovingTo

let diceCombos results = results
                         |> Seq.map(fun h -> h.Rolled)
                         |> Seq.choose(id)
                         |> Seq.map(fun (a,b) -> if b > a then b,a else a,b)

diceCombos history
|> Seq.map(fun dice -> fst dice + snd dice)
|> Seq.groupBy(fun total -> total)
|> Seq.map(fun (key,rolls) -> key, Seq.length rolls)
|> Seq.sortBy(fun (_,rolls) -> -rolls)
|> Seq.toArray

diceCombos history
|> Seq.groupBy(fun dice -> dice)
|> Seq.map(fun (key,rolls) -> key, Seq.length rolls)
|> Seq.sortBy(fun (_,rolls) -> -rolls)
|> Seq.toArray


controller.PlayGame(1000) |> Seq.mapi printMove |> Seq.toArray