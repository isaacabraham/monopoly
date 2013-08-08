#load "Types.fs"
#load "Data.fs"

open Monopoly
open MonopolyData

Board |> List.findIndex ((=) (Property "TrafalgarSquare"))