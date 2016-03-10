API Draft Brettspiel v2.0
=================================

###Message structure
Every message is encoded in JSON.

It should look like so:

{
  "type": "type_string",
  ...some args
}

- All id fields are integers, not strings!!

###The server sends the following commands:
- game_created: game_id => A new game instance was successfully created.
- registered: player_id, player_name => Player has joined the game

- text: text => Just print the text.
- character: character_id, text => A character with id character_id says some text.

- move: player_id, location_id => A player with id player_id moves to the location with id location_id

- next: player_id => Player is next, the current turn is over

- question: player_id, charakter_id, question, options=["option1","option2"] => Character asks player question and gives more than one answer options

- honor: player_id, amount => Ruhm = Ruhm + amount. amount could also be negative.
- change_cards: player_id, swords, shields, supply => all three values are added to the corresponding fields of player. could also be negative.

- item_gained: player_id, item_name => Notifies the player that he has gained item item_name
- item_lost: player_id, item_name => Notifies the player that he has lost item item_name

- win: player_id => Player with player_id wins

###The Client sends the following commands:
(game_id is a string needed by the server to find the correct game instance)
- new_game => asks the server to create a new game instance.

- join: game_id, name => Player with name want's to join

- start: game_id => All players are registered, the game can start

- answer: game_id, answer => The answer of the asked player, an INTEGER from 0 to amount of possibilities of the question - 1

- move_request: game_id, player_id, location_id => Player want's to move to the localisation with localisation_id. "0" means stay.

###Initialization:
1. server listens on a socket, clients send messages to it.
2. one client sends a "new_game" message and gets a game_created message back
3. clients send join messages and get registered messages.
4. one client sends the start message, the game starts
5. the begin of first turn (and all other turns) is marked by a next message

###Procedure of one turn:
1. server sends next message to client
2. client sends move_request to server
3. server accepts it by sending a move message
4. server sends messages. client answers if they recieve a question message.
5. eventually the client recieve's a win message, the game is over then.
6. server sends next message to client
