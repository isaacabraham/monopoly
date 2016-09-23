(* Use this script to experiment with the Monopoly code *)

#load @"Scripts\load-project-debug.fsx"
open Monopoly

// Create a controller
let controller = Controller()

let printMovementEvent m =
    match m with
    | LandedOn (movement, dice) -> sprintf "Rolled a %A and landed on %A (%d doubles)" dice movement.Destination movement.DoubleCount 
    | MovedTo movement -> sprintf "Moved to %A (%d doubles)" movement.Destination movement.DoubleCount

// Subscribe to real-time events
// In F# we can use list-like operations (similar to RX) over events
controller.OnMoved
|> Event.filter(function MovedTo { Destination = Jail } -> true | _ -> false)
|> Event.add(fun _ -> printfn "YOU WENT TO JAIL!")

// Play a number of moves and get the history of the game.
let history = controller.PlayGame 50

// Print out the history
history
|> List.map printMovementEvent
