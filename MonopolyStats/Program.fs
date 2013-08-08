module Runner

open System
open System.Collections.Generic
open Monopoly
open MonopolyData

type disposableColor (newColor) =
    let oldColor = Console.ForegroundColor
    do Console.ForegroundColor <- newColor
    interface IDisposable with
        member this.Dispose() = Console.ForegroundColor <- oldColor

let printPosition (state:Controller.State) =
    let color, text = 
        match state.movingTo with
        | Property(name) -> ConsoleColor.Gray, name
        | Station(name) -> ConsoleColor.White, name
        | Utility(name) -> ConsoleColor.Gray, name
        | Tax(name) -> ConsoleColor.DarkCyan, name
        | Chance(number) -> ConsoleColor.DarkGreen, sprintf "Chance %d" number
        | CommunityChest(number) -> ConsoleColor.DarkGreen, sprintf "Community Chest %d" number
        | _ -> ConsoleColor.Gray, sprintf "%A" state.movingTo
    use temp = new disposableColor(color)
    printfn "%s %s" state.movementType text

let main argv =         
    let history = Controller.playGame 5000 printPosition

    printfn ""
    printfn "statistics"

//    let stats = history
//                |> Seq.groupBy (fun pos -> pos)
//                |> Seq.map (fun (key,items) -> key, items |> Seq.length)
//                |> Seq.sortBy (fun (_, count) -> -count)
//                |> Seq.map(fun (key,count) ->
//                    printf "landed %d times on " count
//                    printPosition "" key
//                )
//                |> Seq.toArray


    printfn "%A" argv
    0 // return an integer exit code