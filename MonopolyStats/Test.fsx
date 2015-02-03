#load "Types.fs"
open Monopoly

#load "Data.fs"
open Monopoly.Data
open System

#load "Controller.fs"
open Monopoly

let printMove index move =
    sprintf "%d: %A" index move

let controller = Controller()
let history = controller.PlayGame(10000) |> Seq.toArray

for item in history do
    printfn "%A" item

let diceCombos results = results
                         |> Seq.choose(function | LandedOn (_,b) -> Some b | _ -> None)
                         |> Seq.map(fun (a,b) -> if b > a then b,a else a,b)

diceCombos history
|> Seq.map(fun dice -> fst dice + snd dice)
|> Seq.countBy(fun total -> total)
|> Seq.sortBy(fun (_,rolls) -> -rolls)
|> Seq.toArray

diceCombos history
|> Seq.countBy id
|> Seq.sortBy(fun (_,rolls) -> -rolls)
|> Seq.toArray


controller.PlayGame(1000) |> Seq.mapi printMove |> Seq.toArray