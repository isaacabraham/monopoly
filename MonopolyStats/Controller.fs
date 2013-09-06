namespace Monopoly

open Monopoly.Data
open System
open System.Collections.Generic

/// Gets the state of the game at a given time
type MovementEvent = 
    { ///<summary>Gets the value of the dice rolled.</summary>
      Rolled: int * int
      ///<summary>Gets the position the player is moving to.</summary>
      MovingTo: Position
      ///<summary>Gets the number of consequitive doubles rolled.</summary>
      DoubleCount: int
      ///<summary>Gets the type of movement that has occurred.</summary>
      MovementType: String }

type Controller() = 
    
    /// Gets the textual representation of a position on the board.
    let picker = new Random()
    
    let moveBy currentPosition rolls = 
        let totalDie = fst rolls + snd rolls
        let currentIndex = Board |> Array.findIndex((=) currentPosition)
        let newIndex = currentIndex + totalDie
        Board.[if newIndex >= 40 then newIndex - 40
               else if newIndex < 0 then newIndex + 40
               else newIndex]
    
    let pickFromDeck currentPosition (deck: Card list) = 
        match deck.[picker.Next(0,16)] with
        | GoTo(pos) -> Some(pos)
        | Move(by) -> Some(moveBy currentPosition (by,0))
        | Other -> None
    
    let checkForMovement position = 
        match position with
        | Chance(_) -> pickFromDeck position ChanceDeck
        | CommunityChest(_) -> pickFromDeck position CommunityChestDeck
        | GoToJail -> Some(Jail)
        | _ -> None
    
    let calculatesDoubles position doublesInARow rolls = 
        match position with
        | Jail -> 0,false
        | _ -> 
            let double = (fst rolls = snd rolls)
            
            let doublesInARow = 
                if double then doublesInARow + 1
                else 0
            if doublesInARow = 3 then (0,true)
            else (doublesInARow,false)
    
    let onMovedEvent = new Event<MovementEvent>()
    
    let rec playTurn originalPosition (die: Random) doublesInARow turnsToPlay = 
        if turnsToPlay = 0 then ignore()
        else 
            let doRoll() = die.Next(1,7)
            let dice = doRoll(),doRoll()
            let doublesInARow,goToJail = calculatesDoubles originalPosition doublesInARow dice
            
            let doPrintPosition movementType movingTo = 
                onMovedEvent.Trigger({ Rolled = dice
                                       MovingTo = movingTo
                                       DoubleCount = doublesInARow
                                       MovementType = movementType })
            
            let newPosition = 
                if goToJail then 
                    doPrintPosition "moved to" Jail
                    Jail
                else 
                    let newPosition = moveBy originalPosition dice
                    doPrintPosition "landed on" newPosition
                    match checkForMovement newPosition with
                    | Some(movedTo) -> 
                        doPrintPosition "moved to" movedTo
                        movedTo
                    | None -> newPosition
            
            playTurn newPosition die doublesInARow (turnsToPlay - 1)
    
    /// <summary>Fired whenever a move occurs</summary>
    [<CLIEvent>]
    member x.OnMoved = onMovedEvent.Publish
    
    /// <summary>Gets the display name for the supplied position.</summary>
    static member GetName position = 
        match position with
        | Property(name) | Station(name) | Utility(name) | Tax(name) -> name
        | Chance(number) -> sprintf "Chance #%d" number
        | CommunityChest(number) -> sprintf "Community Chest #%d" number
        | _ -> sprintf "%A" position
    
    /// <summary>Plays the game of Monopoly</summary>
    /// <param name="turnsToPlay">The number of turns to play</param>
    member x.PlayGame turnsToPlay = 
        let die = new Random()
        playTurn Go die 0 turnsToPlay
