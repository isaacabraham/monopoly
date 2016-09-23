module Monopoly.Runner

open System

let buildDisposable newColor = 
    let oldColor = Console.ForegroundColor
    do Console.ForegroundColor <- newColor
    { new IDisposable with member __.Dispose() = Console.ForegroundColor <- oldColor }

/// Gets the textual representation of a position on the board.
let printPosition (state : MovementEvent) = 
    let color, text = 
        match state.MovementData.Destination with
        | Property(Brown, name) -> ConsoleColor.DarkYellow, name
        | Property(Blue, name) -> ConsoleColor.Blue, name
        | Property(Pink, name) -> ConsoleColor.Magenta, name
        | Property(Orange, name) -> ConsoleColor.DarkMagenta, name
        | Property(Red, name) -> ConsoleColor.Red, name
        | Property(Yellow, name) -> ConsoleColor.Yellow, name
        | Property(Green, name) -> ConsoleColor.Green, name
        | Property(Purple, name) -> ConsoleColor.DarkMagenta, name
        | Utility name -> ConsoleColor.Gray, name
        | Station name -> ConsoleColor.White, name
        | Tax name -> ConsoleColor.White, name
        | Chance number -> ConsoleColor.DarkCyan, sprintf "Chance %d" number
        | CommunityChest number -> ConsoleColor.Cyan, sprintf "Community Chest %d" number
        | _ -> ConsoleColor.Gray, sprintf "%A" state.MovementData.Destination
    let text =
        match state with
        | LandedOn (movement, dice) -> sprintf "Rolled a %A and landed on %A (%d doubles)" dice movement.Destination movement.DoubleCount 
        | MovedTo movement -> sprintf "Moved to %A (%d doubles)" movement.Destination movement.DoubleCount
    
    use x = buildDisposable color
    printfn "%s" text

[<EntryPoint>]
let main _ = 
    let controller = Monopoly.Controller()
    let history = controller.PlayGame(100)
    for entry in history do
        printPosition entry
    Console.Read() |> ignore
    0
