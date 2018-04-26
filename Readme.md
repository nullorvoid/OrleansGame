# Orleans Game Project Example
First attempt at writing a small sample game in Orleans, I am in no way experienced with Orleans, so don't take this as the right way to do anything :D

## Prerequisite
This project is built on .Net Core 2.0 with Orleans 2.0.
In the future this project will use Docker and this will become a requirement.

## Getting started
1. Clone the project
2. Open a terminal and run `dotnet restore` in the top level folder
3. Open a second terminal, cd into `Server` folder, run `dotnet run`
4. On original terminal, cd into `Client` folder, run `dotnet run`

# Design
The project is structured into 4 main parts following the Orleans samples format.

* Client - At the moment this is just a test console
* Server - This is the Orleans Silo
* GrainInterfaces - This is Grain interfaces plus any shared types that are used on both the server and the client for messages between the two
* GrainImplementations - This is the grain implmentations that the client can call apon

## Game Message Implementation Structure
![Structure](/Docs/OrleansStructure.png)

## Overview
The main grains are

* PlayerGrain
* GameGrain

The game grain controls the players coming in and going out of the game, whilst forwarding messages to the internal grains which deal with the game state and send messages to the connected clients via the `GameStream`.

[TODO] The `GameStream` is currently key'd by the GameId, like the `GameActionMessageHandlerGrain` and the `GameState`. This could be extended into namespaced messages such as namespacing the stream by player id for certain player only actions.

```
IStreamProvider streamProvider = GetStreamProvider("GameStream");
secretStream = streamProvider.GetStream<GameMessage>(gameId, playerId);
```

This could also be extended to different areas of the game if for instance there are different grains that make up the game state.

```
IStreamProvider streamProvider = GetStreamProvider("GameStream");
boardStream = streamProvider.GetStream<GameMessage>(gameId, "Board");
```

## GameActionMessageHandler
This class is one of the most important because it decouples what is a `Game` from the implementation of the game itself.

This simple proxy maps game specific messages to the `GameState`. In the simple example in this repo (at present) the `GameState` is just one grain, however because the `GameActionMessageHandler` exists it could map to multiple grains and still retain the single message at a time by awaiting all the responses.

A simple message handler which maps a single message to the state
```
handlers.Add(typeof(MoveMessage), async (msg) => await state.PlayerMove(msg));
```

A complex message handler which allows to map to multiple grains from a single message
```
handlers.Add(typeof(PlaceCardMessage), async (msg) => {
	PlaceCardMessage message = msg as PlaceCardMessage;
	await hand.RemoveCard(message.CardFromHandMessage);
	await board.PlaceCardOnBoaed(message.PlaceCardOnBoardMessage);
});

```
This allows chaining of messages over different state elements but makes sure because of the awaiting it is always processed in one go from the `Game` grain.
