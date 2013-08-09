module Controller

open Monopoly
open MonopolyData
open System
open System.Collections.Generic

/// Gets the state of the game at a given time
type State = {
    /// Gets the value of the dice rolled.
    rolled:int*int
    /// Gets the position the player is moving to.
    movingTo:Position
    /// Gets the number of consequitive doubles rolled.
    doubleCount:int
    /// Gets the type of movement that has occurred.
    movementType:String
}

/// Gets the textual representation of a position on the board.
let getName position =
    match position with
    | Property(name)
    | Station(name) 
    | Utility(name) 
    | Tax(name) -> name
    | Chance(number) -> sprintf "Chance #%d" number
    | CommunityChest(number) -> sprintf "Community Chest #%d" number
    | _ -> sprintf "%A" position

let private picker = new Random()

let private moveBy currentPosition rolls =
    let totalDie = fst rolls + snd rolls
    let currentIndex = (Board |> List.findIndex ((=) currentPosition))
    let newIndex = currentIndex + totalDie
    Board.[if newIndex >= 40 then newIndex - 40 else if newIndex < 0 then newIndex + 40 else newIndex]

let private pickFromDeck currentPosition (deck:Card list) =
    match deck.[picker.Next(0, 16)] with
    | GoTo(pos) -> Some(pos)
    | Move(by) -> Some(moveBy currentPosition (by,0))
    | Other -> None
    
let private checkForMovement position =
    match position with
    | Chance(_) -> pickFromDeck position ChanceDeck
    | CommunityChest(_) -> pickFromDeck position CommunityChestDeck
    | GoToJail -> Some(Jail)
    | _ -> None
    
let private calculatesDoubles position doublesInARow rolls =
    match position with
    | Jail -> 0, false
    | _ -> let double = (fst rolls = snd rolls)
           let doublesInARow = if double then doublesInARow + 1 else 0
           if doublesInARow = 3 then (0, true) else (doublesInARow, false)

let rec private playTurn originalPosition (die:Random) doublesInARow turnsToPlay printPosition =
    if turnsToPlay = 0 then
        ignore()
    else
        let doRoll() = die.Next(1, 7)
        let dice = doRoll(), doRoll()
        let doublesInARow, goToJail = calculatesDoubles originalPosition doublesInARow dice
        let doPrintPosition movementType movingTo = printPosition { rolled = dice; movingTo = movingTo; doubleCount = doublesInARow; movementType = movementType };
        let newPosition = if goToJail then
                               doPrintPosition "moved to" Jail
                               Jail
                          else let newPosition = moveBy originalPosition dice
                               doPrintPosition "landed on" newPosition
                               match checkForMovement newPosition with
                               | Some(movedTo) -> doPrintPosition "moved to" movedTo
                                                  movedTo
                               | None -> newPosition
        playTurn newPosition die doublesInARow (turnsToPlay - 1) printPosition

/// <summary>Plays the game of Monopoly</summary>
/// <param name="turnsToPlay">The number of turns to play</param>
/// <param name="printPosition">The code to execute whenever a turn is made.</param>
let playGame turnsToPlay printPosition =
    let die = new System.Random()
    playTurn Go die 0 turnsToPlay printPosition