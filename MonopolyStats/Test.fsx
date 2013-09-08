#load "Types.fs"
open Monopoly

#load "Data.fs"
open Monopoly.Data
open System

#load "Controller.fs"
open Monopoly

let printMove (index,move) = sprintf "%d %A %s %s (doubles %d)" index move.Rolled move.MovementType (Controller.GetName(move.MovingTo)) move.DoubleCount

let controller = Controller()
let history = controller.PlayGame(1000)
              |> Seq.toArray

for item in history do
    printfn "%A, %s %A" item.Rolled item.MovementType item.MovingTo