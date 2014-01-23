namespace Monopoly

open Monopoly.Data
open System
open System.Collections.Generic

/// A movement that has occurred for a player.
type MovementEvent = 
    { Rolled : (int * int) option
      MovingTo : Position
      DoubleCount : int
      MovementType : string }

(*
The controller, which is the main entry point for clients. We have to create a full class rather than a basic module as
it seems that CLIEvents do not get consumed nicely in modules.
*)

/// Manages a game.
type Controller() =   
    let moveBy rolls currentPosition = 
        let totalDie = fst rolls + snd rolls
        let currentIndex = Board |> List.findIndex((=) currentPosition)
        let newIndex = currentIndex + totalDie
        Board.[if newIndex >= 40 then newIndex - 40
               else if newIndex < 0 then newIndex + 40
               else newIndex]
    
    let picker = new Random()
    let pickFromDeck (deck : Card list) currentPosition = 
        match (picker.Next(0, 16)) |> List.nth deck with
        | GoTo destination -> Some destination
        | Move numberOfSpaces -> Some(currentPosition |> moveBy(numberOfSpaces, 0))
        | Other -> None
    
    let checkForMovement currentPosition = 
        match currentPosition with
        | Chance _ -> ChanceDeck |> pickFromDeck currentPosition
        | CommunityChest _ -> CommunityChestDeck |> pickFromDeck currentPosition
        | GoToJail -> Some Jail
        | _ -> None
    
    let calculateDoubles position doublesInARow rolls = 
        let doublesInARow = 
            if (fst rolls = snd rolls) then doublesInARow + 1
            else 0
        if doublesInARow = 3 then (0, true)
        else (doublesInARow, false)
    
    let onMovedEvent = new Event<MovementEvent>()
    
    let rec playTurn currentPosition (die : Random) doublesInARow turnsToPlay history = 
        if turnsToPlay = 0 then List.rev history
        else 
            let doRoll() = die.Next(1, 7)
            let dice = doRoll(), doRoll()
            let doublesInARow, rolledThreeDoubles = calculateDoubles currentPosition doublesInARow dice
            
            let generateMovement movementType movingTo rolled = 
                let movement = 
                    { Rolled = rolled
                      MovingTo = movingTo
                      DoubleCount = doublesInARow
                      MovementType = movementType }
                onMovedEvent.Trigger(movement)
                movement
            
            let movementsThisTurn = 
                if rolledThreeDoubles then [ generateMovement "moved to" Jail (Some dice) ]
                else 
                    let initialMove = generateMovement "landed on" (currentPosition |> moveBy dice) (Some dice)
                    match checkForMovement initialMove.MovingTo with
                    | Some(movedTo) -> 
                        let secondaryMove = generateMovement "moved to" movedTo None
                        [ secondaryMove; initialMove ]
                    | None -> [ initialMove ]
            
            playTurn movementsThisTurn.Head.MovingTo die doublesInARow (turnsToPlay - 1) (movementsThisTurn @ history)
    
    /// Fired whenever a move occurs
    [<CLIEvent>]
    member x.OnMoved = onMovedEvent.Publish
    
    /// Gets the display name for the supplied position.
    static member GetName position = 
        match position with
        | Property(name) | Station(name) | Utility(name) | Tax(name) -> name
        | Chance(number) -> sprintf "Chance #%d" number
        | CommunityChest(number) -> sprintf "Community Chest #%d" number
        | _ -> sprintf "%A" position
    
    /// Plays the game of Monopoly
    member x.PlayGame turnsToPlay = 
        let die = new Random()
        playTurn Go die 0 turnsToPlay []
