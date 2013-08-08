module Monopoly

type Set =
    | Brown
    | LightBlue
    | Pink
    | Orange
    | Red
    | Yellow
    | Green
    | DarkBlue

type Position =
    | Property of string
    | Station of string
    | Utility of string
    | Chance of int
    | CommunityChest of int
    | Tax of string
    | GoToJail
    | Go
    | FreeParking
    | Jail

type Card =
    | Other
    | Move of int
    | GoTo of Position