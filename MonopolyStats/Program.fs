module Monopoly.Runner

open Data
open System
open System.Collections.Generic

type disposableColor(newColor) = 
    let oldColor = Console.ForegroundColor
    do Console.ForegroundColor <- newColor
    interface IDisposable with
        member this.Dispose() = Console.ForegroundColor <- oldColor

let printPosition(state: MovementEvent) = 
    let color,text = 
        match state.MovingTo with
        | Property(name) -> ConsoleColor.Gray,name
        | Station(name) -> ConsoleColor.White,name
        | Utility(name) -> ConsoleColor.Gray,name
        | Tax(name) -> ConsoleColor.DarkCyan,name
        | Chance(number) -> ConsoleColor.DarkGreen,sprintf "Chance %d" number
        | CommunityChest(number) -> ConsoleColor.DarkGreen,sprintf "Community Chest %d" number
        | _ -> ConsoleColor.Gray,sprintf "%A" state.MovingTo
    
    use temp = new disposableColor(color)
    printfn "%s %s" state.MovementType text

let main argv = 
    let controller = Monopoly.Controller()
    let history = controller.PlayGame(5000)
    printfn ""
    printfn "statistics"
    printfn "%A" argv
    0 // return an integer exit code
      
