# Blackjack

A simple game of Blackjack for up to 4 players.

## Features

- Betting!
- Classic moves such as Hit, Stay, and Double Down!
- Save and resume a game even if you're in the middle of a round!
- Choose starting cash and Blackjack payout!
- Automatic Ace Analysis (AAA) calculates the best value for your Aces!

## Outline

Made in Unity 2019.4.11f1.

**BlackjackGame.cs**
- Handles game flow, saving, player input

**CardDatabase.cs**
- A scriptable object containing card data

**CardData.cs**
- Contains a sprite, value, and suit

**Card.cs**
- Represents a physical card in play
- Can be face up or face down

**Deck.cs**
- Represents a "shoe" that can shuffle and deal cards
- Object pool repurposes cards that have been discarded
- Entire deck can be saved and restored

**Player.cs**
- Represents a player who can play Blackjack (includes dealer)
- Card slots are for neatly placing cards in hand without having to think about it
- Calculates value of cards in hand, accounting for Aces

**BetUI.cs**
- Controls the betting amount during the betting phase
- Can increase or decrease bet by a fixed amount
