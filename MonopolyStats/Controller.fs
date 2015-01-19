namespace Monopoly

open Monopoly.Data
open System

type MovementType = 
    | LandedOn
    | MovedTo
    override x.ToString() =
        match x with
        | LandedOn -> "Landed on"
        | MovedTo -> "Moved to"

/// A movement that has occurred for a player.
type MovementEvent = 
    { Rolled : (int * int) option
      MovingTo : Position
      DoubleCount : int
      MovementType : MovementType }

/// Manages a game.
type Controller() = 
    
    let moveBy dice currentPosition = 
        let diceValue = fst dice + snd dice
        let currentIndex = Board |> List.findIndex ((=) currentPosition)
        let newIndex = currentIndex + diceValue
        Board.[newIndex % 40]
    
    let picker = Random()
    
    let tryAutoMove currentPosition = 
        let tryMoveFromDeck currentPosition deck = 
            match (picker.Next(0, 16)) |> List.nth deck with
            | GoTo destination -> Some destination
            | Move numberOfSpaces -> Some(currentPosition |> moveBy (numberOfSpaces, 0))
            | Other -> None
        match currentPosition with
        | Chance _ -> ChanceDeck |> tryMoveFromDeck currentPosition
        | CommunityChest _ -> CommunityChestDeck |> tryMoveFromDeck currentPosition
        | GoToJail -> Some Jail
        | _ -> None
    
    let calculateDoubles = 
        function 
        | 3, _ -> 0
        | doublesInARow, (first, second) when first = second -> doublesInARow + 1
        | _ -> 0
    
    let onMovedEvent = new Event<MovementEvent>()
    
    let rec playTurn currentPosition doRoll doublesInARow turnsToPlay history = 
        if turnsToPlay = 0 then List.rev history
        else 
            let dice = doRoll(), doRoll()
            let doublesInARow = calculateDoubles (doublesInARow, dice)
            
            let generateMovement movementType movingTo dice = 
                let movement = 
                    { Rolled = dice
                      MovingTo = movingTo
                      DoubleCount = doublesInARow
                      MovementType = movementType }
                onMovedEvent.Trigger(movement)
                movement
            
            let movementsThisTurn = 
                match doublesInARow with
                | 3 -> [ generateMovement MovedTo Jail (Some dice) ]
                | _ -> 
                    let initialMove = generateMovement LandedOn (currentPosition |> moveBy dice) (Some dice)
                    match tryAutoMove initialMove.MovingTo with
                    | Some automaticMove -> 
                        let secondaryMove = generateMovement MovedTo automaticMove None
                        [ secondaryMove; initialMove ]
                    | None -> [ initialMove ]
            
            playTurn movementsThisTurn.Head.MovingTo doRoll doublesInARow (turnsToPlay - 1) 
                (movementsThisTurn @ history)
    
    /// Fired whenever a move occurs
    [<CLIEvent>]
    member __.OnMoved = onMovedEvent.Publish
    
    /// Plays the game of Monopoly
    member __.PlayGame turnsToPlay = 
        let doRoll = 
            let die = new Random()
            fun () -> die.Next(1, 7)
        playTurn Go doRoll 0 turnsToPlay []
