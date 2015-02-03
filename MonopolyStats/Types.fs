namespace Monopoly

type Set =
    | Brown
    | Blue
    | Pink
    | Orange
    | Red
    | Yellow
    | Green
    | Purple

type Position = 
    | Property of Set * Name : string
    | Station of Name : string
    | Utility of Type : string
    | Chance of Id : int
    | CommunityChest of Id : int
    | Tax of Type : string
    | GoToJail
    | Go
    | FreeParking
    | Jail
    override x.ToString() = 
        match x with
        | Property(_, name) | Station name | Utility name | Tax name -> name
        | Chance number -> sprintf "Chance #%d" number
        | CommunityChest number -> sprintf "Community Chest #%d" number
        | FreeParking -> "Free Parking"
        | GoToJail -> "Go to Jail"
        | position -> sprintf "%A" position

type Card = 
    | Other
    | Move of Distance:int
    | GoTo of Destination:Position