(* Use this script to experiment with the Monopoly code *)

#load @"Scripts\load-project-debug.fsx"
open Monopoly

// Experiment with standalone functions
Go |> Functions.moveBy (5, 3)
(Position.Property(Set.Yellow, "Picaddilly")) |> Functions.moveBy (3, 1)
Functions.playTurn (fun () -> 3) (fun () -> 5) ignore { CurrentPosition = Go; DoubleCount = 0 }





// Create a controller to test out the public API
let controller = Controller()

let printMovementEvent m =
    match m with
    | LandedOn (movement, dice) -> sprintf "Rolled a %A and landed on %A (%d doubles)" dice movement.CurrentPosition movement.DoubleCount 
    | MovedTo movement -> sprintf "Moved to %A (%d doubles)" movement.CurrentPosition movement.DoubleCount

// Optional - subscribe to real-time events. In F# we can use list-like operations (similar to RX) over events
controller.OnMoved
|> Event.filter(function MovedTo { CurrentPosition = Jail } -> true | _ -> false)
|> Event.add(fun _ -> printfn "YOU WENT TO JAIL!")

// Play a number of moves and get the history of the game.
let history = controller.PlayGame 50

// Print out the history
history
|> List.map printMovementEvent