# Tic Tac Toe Game

## Overview

A Tic Tac Toe application built using **Angular, TypeScript, and ASP.NET Core Web API**. The application supports two-player gameplay, playing against the computer, move undo, game reset, win/draw detection, and scoreboard management.

Game data is stored in memory and is not persisted after the API restarts.

## Technologies Used

* Frontend: Angular, TypeScript, HTML, CSS
* Backend: ASP.NET Core Web API, C#
* Testing: xUnit
* Storage: In-memory storage

## Prerequisites

Install the following:

* Node.js and npm
* Angular CLI
* .NET SDK compatible with the API project
* Visual Studio or Visual Studio Code

## Project Structure


TicTacToe/
  TicTacToe.Api/       # .NET Web API
  TicTacToe.Tests/     # xUnit unit tests
  tic-tac-toe-ui/      # Angular frontend
  



## Setup and Run

### 1. Run the .NET API

Open a terminal in the solution root and run:

 open the solution in Visual Studio, set `TicTacToe.Api` as the startup project, and press **F5**.

Note the API URL displayed in the terminal. Open the Swagger URL, https://localhost:44303/swagger/index.html and test the endpoints.

### 2. Configure and Run Angular

Open a separate terminal and navigate to the Angular project folder:

```bash
cd tic-tac-toe-ui
npm install
ng serve
```

Open the local URL displayed in the terminal, usually:

`http://localhost:4200`

Ensure the API base URL configured in the Angular service matches the running API URL.

In service, src/app/core/services/game.service.ts ,in GameService class change port numbers to the webapi running port in apiUrl and scoreboard url


private apiUrl = 'https://localhost:44303/api';
private scoreboardUrl = 'https://localhost:44303/api/scoreboard';



  
### 3. Run Unit Tests for apis.

1)From the solution root, open TicTacToe.Tests in visual studio.
2)Right click on the solution , and click on run tests.


This executes the automated .NET unit tests.

## Features

* Create a new game in two-player or computer mode
* Make moves on a 3 × 3 board
* Automatically detect wins and draws
* Highlight winning cells
* Undo moves
* Reset the current game
* View and reset the scoreboard
* Validate moves and prevent invalid game actions

The below strategy is used while designing this application.

Option A: 
Disable Undo After Completion
•	Once a game is won or drawn, Undo is disabled.
•	The scoreboard remains final for that game.


How the game works:-

1)Click on the option in the dropdown Two Player or Play against Computer.
2)Click on New Game: This starts the game.

Note:-Clicking on dropdown alone wont start the game. New Game button starts the new game.


## Notes

* Start the backend API before using the Angular application.
* Game and scoreboard data are held in memory and reset when the API restarts.

