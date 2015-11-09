namespace Monopoly

open Monopoly.Data
open System

/// Represents the roll of two dice.
type DiceData = (int * int)

/// A movement that has occurred for a player.
type MovementData = 
    { Destination : Position
      DoubleCount : int }

/// A type of movement event.
type MovementEvent = 
    /// When a player lands on a position from a throw of the dice.
    | LandedOn of MovementData * DiceData
    /// When a player lands on a position as a result of an external event e.g. chance card.
    | MovedTo of MovementData
    override this.ToString() =
        match this with
        | LandedOn _ -> "Landed on"
        | MovedTo _ -> "Moved to"
    member this.MovementData =
        match this with
        | LandedOn (movementData, _) -> movementData
        | MovedTo movementData -> movementData

/// Manages a game.
type Controller() =
    let moveBy dice currentPosition = 
        let diceValue = fst dice + snd dice
        let currentIndex = Board |> List.findIndex ((=) currentPosition)
        let newIndex = currentIndex + diceValue
        Board.[newIndex % 40]
    
    let picker = Random()

    let tryMoveFromDeck position = 
        let tryMoveFromDeck deck = 
            match deck |> List.item (picker.Next(0, 16)) with
            | GoTo destination -> Some destination
            | Move numberOfSpaces -> Some(position |> moveBy (numberOfSpaces, 0))
            | Other -> None
        match position with
        | Chance _ -> ChanceDeck |> tryMoveFromDeck
        | CommunityChest _ -> CommunityChestDeck |> tryMoveFromDeck
        | GoToJail -> Some Jail
        | _ -> None

    (* These two Active Patterns allow us to reason about the roll of dice a 
       little more easily so that we can pattern match over some higher
       abstractions (see below) *)
    let (|ThreeDoubles|LessThanThreeDoubles|) numberOfDoubles =
        match numberOfDoubles with
        | 3 -> ThreeDoubles
        | _ -> LessThanThreeDoubles

    let (|Double|NotADouble|) dice =
        match dice with
        | a,b when a = b -> Double
        | _ -> NotADouble
    
    let onMovedEvent = new Event<MovementEvent>()
    
    let playGame turnsToPlay doRoll =
        ([], [ 1 .. turnsToPlay ])
        ||> List.scan(fun previousMoves _ ->
            let currentPosition =
                match previousMoves |> List.rev with
                | [] -> { Destination = Go; DoubleCount = 0 } // no history - start at Go.
                | (MovedTo movementData) :: _
                | (LandedOn (movementData, _) :: _) -> movementData // last move made

            let currentThrow = doRoll(), doRoll()
            let currentDoubleCount =
                match (currentPosition.DoubleCount, currentThrow) with
                | LessThanThreeDoubles, Double -> currentPosition.DoubleCount + 1
                | ThreeDoubles, Double -> 1
                | LessThanThreeDoubles, NotADouble | ThreeDoubles, NotADouble -> 0
            
            let generateMovement movingTo throw = 
                let movementData = { Destination = movingTo; DoubleCount = currentDoubleCount }
                let movement =
                    match throw with
                    | Some throw -> LandedOn (movementData, throw)
                    | None -> MovedTo movementData
                onMovedEvent.Trigger(movement)
                movement
            
            match currentDoubleCount with
            | ThreeDoubles -> [ generateMovement Jail (Some currentThrow) ]
            | LessThanThreeDoubles -> 
                let newPosition = currentPosition.Destination |> moveBy currentThrow
                let initialMove = generateMovement newPosition (Some currentThrow)
                match tryMoveFromDeck newPosition with
                | Some forcedMove ->
                    let secondaryMove = generateMovement forcedMove None
                    [ initialMove; secondaryMove ]
                | None -> [ initialMove ])
    
    (* A CLI Event is an event that can be consumed by e.g. C# *)
    /// Fired whenever a move occurs.
    [<CLIEvent>]
    member __.OnMoved = onMovedEvent.Publish
    
    /// Plays the game of Monopoly
    member __.PlayGame turnsToPlay =
        let doRoll = 
            let die = new Random()
            fun () -> die.Next(1, 7)
        playGame turnsToPlay doRoll
        |> List.collect id
