module Monopoly.Data
    
let Board =
    [ Go;
      Property (Brown, "Old Kent Road"); CommunityChest 1; Property (Brown, "Whitechapel Road");
      Tax "Income Tax"; Station "Kings Cross";
      Property (Blue, "Angel Islington"); Chance 1; Property (Blue, "Euston Road"); Property (Blue, "Pentonville Road");
      Jail;
       
      Property (Pink, "Pall Mall"); Utility "Electric Company"; Property (Pink, "Whitehall"); Property (Pink, "Northumberland Avenue");
      Station "Marylebone Road";
      Property (Orange, "Bow Street"); CommunityChest 2; Property (Orange, "Marlborough Street"); Property (Orange, "Vine Street");
      FreeParking;
       
      Property (Red, "Strand"); Chance 2; Property (Red, "Fleet Street"); Property (Red, "Trafalgar Square");
      Station "Fenchurch Street";
      Property (Yellow, "Leicester Square"); Property (Yellow, "Coventry Street"); Utility "Water Works"; Property (Yellow, "Picaddilly");
      GoToJail;
       
      Property (Green, "Regent Street"); Property (Green, "Oxford Street"); CommunityChest 3; Property (Green, "Bond Street");
      Station "Liverpool Street"; Chance 3;
      Property (Purple, "Park Lane"); Tax "Super Tax"; Property (Purple, "Mayfair"); ]

let ChanceDeck = [GoTo <| Property (Purple, "Mayfair"); GoTo Go; GoTo Jail; GoTo <| Property (Red, "Trafalgar Square"); GoTo <| Station "Marylebone Road"; GoTo <| Property (Pink, "Pall Mall"); Move -3;] @ [for _ in 1..9 -> Card.Other]
let CommunityChestDeck = [GoTo Go; GoTo Jail; GoTo <| Property (Brown, "Old Kent Road");] @ [for _ in 1..13 -> Card.Other]
