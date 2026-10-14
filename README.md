# Tic Tac Toe Game

## Overview

A Tic Tac Toe application built using **Angular, TypeScript, and ASP.NET Core Web API**. The application supports two-player gameplay, playing against the computer, move undo, game reset, win/draw detection, and scoreboard management.

Game data is stored in memory and is not persisted after the API restarts.

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

## Technologies Used

* Frontend: Angular, TypeScript, HTML, CSS
* Backend: ASP.NET Core Web API, C#
* Testing: xUnit
* Storage: In-memory storage
* Local API: HTTPS

## Prerequisites

Install the following before running the application:

* Node.js and npm
* Angular CLI
* .NET SDK compatible with the API project
* Visual Studio or Visual Studio Code

Verify the installations:

dotnet --version
node --version
npm --version

## Project Structure

```text
tic-tac-toe/
├── run.bat
├── README.md
├── TicTacToe.Api/
│   └── TicTacToe.Api/
│       ├── TicTacToe.Api.csproj
│       └── Properties/
│           └── launchSettings.json
├── TicTacToe.Tests/
│   └── TicTacToe.Tests/
│       └── TicTacToe.Tests.csproj
└── tic-tac-toe-ui/
    ├── package.json
    └── src/
```
## Configuration

The API uses the HTTPS launch profile on port 5088.

The Angular application runs on port 4200.

Ensure the Angular service uses these API URLs:

private apiUrl = 'https://localhost:5088/api';
private scoreboardUrl = 'https://localhost:5088/api/scoreboard';

If the HTTPS development certificate is not trusted, run:

dotnet dev-certs https --trust

## Run the Application Using the BAT File

A Windows batch file is provided to restore and build the API, check and build the Angular application, and start both applications.
run-tic-tac-toe.bat

## Expected Results

When you execute the BAT file:

The .NET API restore should complete successfully.

The .NET API build should succeed.

Angular dependencies should be installed if node_modules is missing.

The Angular production build should succeed.

Two terminal windows should open.

Open the application in your browser:

Frontend: http://localhost:4200

Backend HTTPS base URL: https://localhost:5088

You can check the API endpoints using Swagger if Swagger is enabled in the API project.


## Setup and Run Projects Manually

### 1. Run the .NET API

Open a terminal in the solution root and run:

 open the solution in Visual Studio, set `TicTacToe.Api` as the startup project, and press **F5**.

Note the API URL displayed in the terminal. Open the Swagger URL, https://localhost:5088/swagger/index.html and test the endpoints.

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


private apiUrl = 'https://localhost:5088/api';
private scoreboardUrl = 'https://localhost:5088/api/scoreboard';



  
### 3. Run Unit Tests for apis.

1)From the solution root, open TicTacToe.Tests in visual studio.
2)Right click on the solution , and click on run tests.


This executes the automated .NET unit tests.

