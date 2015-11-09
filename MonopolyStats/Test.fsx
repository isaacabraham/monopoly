#load @"Scripts\load-project.fsx"
open Monopoly

let printMove index move =
    sprintf "%d: %A" index move

let controller = Controller()
let history = controller.PlayGame(50)

history
|> List.map (fun m ->
    match m with
    | LandedOn (movement, dice) -> sprintf "Rolled a %A and landed on %A (%d doubles)" dice movement.Destination movement.DoubleCount 
    | MovedTo movement -> sprintf "Moved to %A (%d doubles)" movement.Destination movement.DoubleCount)





