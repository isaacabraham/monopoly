namespace Monopoly

type Position = 
    | Property of Name:string
    | Station of Name:string
    | Utility of Type:string
    | Chance of Id:int
    | CommunityChest of Id:int
    | Tax of Type:string
    | GoToJail
    | Go
    | FreeParking
    | Jail

type Card = 
    | Other
    | Move of Distance:int
    | GoTo of Destination:Position
