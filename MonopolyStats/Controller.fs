namespace Monopoly

open Monopoly.Data
open System

/// Represents the roll of two dice.
type DiceData = (int * int)

/// A movement that has occurred for a player.
type MovementData = 
    { CurrentPosition : Position
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

/// Contains internal Monopoly functionality.
module Functions =
    let moveBy dice currentPosition = 
        let diceValue = fst dice + snd dice
        let currentIndex = Board |> List.findIndex ((=) currentPosition)
        let newIndex = currentIndex + diceValue
        Board.[newIndex % 40]
    
    let tryMoveFromDeck deck pickCard position = 
        match deck |> List.item (pickCard()) with
        | GoTo destination -> Some destination
        | Move numberOfSpaces -> Some(position |> moveBy (numberOfSpaces, 0))
        | Other -> None

    /// Picks a card from the Chance deck.
    let tryMoveFromChance = tryMoveFromDeck ChanceDeck
    /// Picks a card from the Community Chest deck.
    let tryMoveFromCommunityChest = tryMoveFromDeck CommunityChestDeck

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

    /// <summary>
    /// Plays a single turn, based on the previous movement.
    /// </summary>
    /// <param name="doRoll">Performs a roll of a single die.</param>
    /// <param name="pickCard">Selects a card to pick from 0 to 15.</param>
    /// <param name="publishMove">Publish a move to some side-effectful listener.</param>
    /// <param name="moveData">The movement data from the previous turn.</param>
    /// <returns>Returns a new position and optionally a secondary position (caused by e.g. rolling 3 doubles or chance etc. etc.)</returns>
    let playTurn doRoll pickCard publishMove moveData =
        let currentThrow = doRoll(), doRoll()
        let currentDoubleCount =
            match (moveData.DoubleCount, currentThrow) with
            | LessThanThreeDoubles, Double -> moveData.DoubleCount + 1
            | ThreeDoubles, Double -> 1
            | LessThanThreeDoubles, NotADouble | ThreeDoubles, NotADouble -> 0

        let generateMovement movingTo throw = 
            let movementData = { CurrentPosition = movingTo; DoubleCount = currentDoubleCount }
            let movement =
                match throw with
                | Some throw -> LandedOn (movementData, throw)
                | None -> MovedTo movementData
            publishMove movement
            movement
            
        match currentDoubleCount with
        | ThreeDoubles -> generateMovement Jail (Some currentThrow), None
        | LessThanThreeDoubles -> 
            let position = moveData.CurrentPosition |> moveBy currentThrow
            let initialMove = generateMovement position (Some currentThrow)
            let secondaryMove =
                match position with
                | Chance _ -> tryMoveFromChance pickCard position
                | CommunityChest _ -> tryMoveFromCommunityChest pickCard position
                | GoToJail -> Some Jail
                | _ -> None

            match secondaryMove with
            | Some secondaryMove ->
                let secondaryMove = generateMovement secondaryMove None
                initialMove, Some secondaryMove
            | None -> initialMove, None

    /// <summary>
    /// Plays a full game.
    /// </summary>
    /// <param name="turnsToPlay">The number of turns to play for.</param>
    /// <param name="onMove">Function called whenever a turn is played.</param>
    /// <param name="seed">Optional random seed.</param>
    let playGame turnsToPlay onMove seed =
        /// Plays a single turn.
        let playTurn =
            let random = match seed with | Some seed -> Random seed | None -> Random()
            let doRoll() = random.Next(1, 7)
            let pickCard() = random.Next(0, 16)
            playTurn doRoll pickCard onMove

        let getCurrentPosition (previousTurn:MovementEvent * MovementEvent option) =
            match previousTurn with
            | _, Some move | move, None -> move.MovementData

        ((MovedTo { CurrentPosition = Go; DoubleCount = 0 }, None), [ 1 .. turnsToPlay ])
        ||> List.scan(fun lastTurn _ -> lastTurn |> getCurrentPosition |> playTurn)
        |> List.collect(fun (mainRoll, optionalSecondaryMove) -> mainRoll :: Option.toList optionalSecondaryMove)

/// Manages a game.
type Controller() =
    let onMovedEvent = new Event<MovementEvent>()
    
    (* A CLI Event is an event that can be consumed by e.g. C# *)
    /// Fired whenever a move occurs.
    [<CLIEvent>]
    member __.OnMoved = onMovedEvent.Publish
    
    /// Plays the game of Monopoly
    member __.PlayGame turnsToPlay = Functions.playGame turnsToPlay onMovedEvent.Trigger None