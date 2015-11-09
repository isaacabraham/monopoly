#load @"Scripts\load-project.fsx"
open Monopoly

// Create a controller
let controller = Controller()

// Play a number of moves and get the history of the game.
let history = controller.PlayGame 50

// Print out the history
history
|> List.map (fun m ->
    match m with
    | LandedOn (movement, dice) -> sprintf "Rolled a %A and landed on %A (%d doubles)" dice movement.Destination movement.DoubleCount 
    | MovedTo movement -> sprintf "Moved to %A (%d doubles)" movement.Destination movement.DoubleCount)





