namespace Monopoly

open Monopoly.Data
open System

type DiceData = (int * int)

///// A movement that has occurred for a player.
type MovementData = 
    { Destination : Position
      DoubleCount : int }

type MovementEvent = 
    | LandedOn of MovementData * DiceData
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

    let (|ThreeDoubles|LessThanThreeDoubles|) = function
        | 3 -> ThreeDoubles
        | _ -> LessThanThreeDoubles

    let (|Double|NotADouble|) =
        function
        | a,b when a = b -> Double
        | _ -> NotADouble        
    
    let onMovedEvent = new Event<MovementEvent>()
    
    let rec playTurn currentPosition doRoll previousDoubleCount turnsToPlay history = 
        if turnsToPlay = 0 then List.rev history
        else
            let currentThrow = doRoll(), doRoll()
            let currentDoubleCount =
                match (previousDoubleCount, currentThrow) with
                | LessThanThreeDoubles, Double -> previousDoubleCount + 1
                | LessThanThreeDoubles, NotADouble -> 0
                | ThreeDoubles, Double -> 1
                | ThreeDoubles, NotADouble -> 0
            
            let generateMovement movementType movingTo throw = 
                let movementData = { Destination = movingTo; DoubleCount = currentDoubleCount }
                let movement =
                    match throw with
                    | Some throw -> LandedOn (movementData, throw)
                    | None -> MovedTo movementData
                onMovedEvent.Trigger(movement)
                movement
            
            let movementsThisTurn = 
                match currentDoubleCount with
                | ThreeDoubles -> [ generateMovement MovedTo Jail (Some currentThrow) ]
                | LessThanThreeDoubles -> 
                    let movingTo = currentPosition |> moveBy currentThrow
                    let initialMove = generateMovement LandedOn movingTo (Some currentThrow)
                    match tryAutoMove movingTo with
                    | Some destination -> 
                        let secondaryMove = generateMovement MovedTo destination None
                        [ secondaryMove; initialMove ]
                    | None -> [ initialMove ]
            
            playTurn movementsThisTurn.Head.MovementData.Destination doRoll currentDoubleCount (turnsToPlay - 1) 
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
