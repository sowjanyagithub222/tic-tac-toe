import { Component, OnInit,ChangeDetectorRef ,NgZone  } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

import { GameService } from './core/services/game.service';

import {
  GameState,
  GameMode,
  Scoreboard,
  MoveRequest
} from './core/models/game.model';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './app.html',
  styleUrls: ['./app.css']
})
export class App implements OnInit {

  gameState: GameState | null = null;
  gameId: string | null = null;
selectedMode: GameMode = 'TwoPlayer';
  errorMessage = '';
  scoreboard: Scoreboard = {
  xWins: 0,
  oWins: 0,
  draws: 0
};


  isLoading = false;

  constructor(private gameService: GameService, private cdr: ChangeDetectorRef, private ngZone: NgZone) {}

  ngOnInit(): void {
    this.loadScoreboard();
  }

  
startNewGame(mode: GameMode): void {
  if (this.isLoading) {
    return;
  }
console.log('Creating game with mode:', mode);
  this.errorMessage = '';
  this.isLoading = true;

  this.gameService.createGame({ gameMode: mode }).subscribe({
    next: (game: GameState) => {
      console.log('New game received:', game);

      this.gameId = game.gameId;
      this.gameState = {
        ...game,
        board: [...game.board],
        winningCells: [...(game.winningCells ?? [])]
      };

      // Force the view to update after game creation.
      this.cdr.detectChanges();

      console.log('Game state assigned:', this.gameState);
    },
    error: (error: HttpErrorResponse) => {
      console.error('Could not create a game:', error);
      this.errorMessage =
        'Could not create a game. Check whether the API is running.';
    },
    complete: () => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  });
}


  



makeMove(request: MoveRequest): void {
  if (!this.gameId || this.isLoading || !this.gameState) {
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  const gameId = this.gameState.gameId;

  console.log('Sending move:', request);

  this.gameService.makeMove(gameId, request).subscribe({
    next: (state: GameState) => {
      console.log('API returned board:', state.board);

      this.gameState = {
        ...state,
        board: [...state.board],
        winningCells: [...(state.winningCells ?? [])]
      };

      this.gameId = state.gameId;

      console.log('Game mode:', state.mode);
      console.log('Current player:', state.currentPlayer);
      console.log('Game status:', state.status);

      this.cdr.detectChanges();

     const status = state.status?.toString().toLowerCase();

if (status === 'won' || status === 'draw') {
  console.log('Game finished. Refreshing scoreboard...');
  this.loadScoreboard();
  this.isLoading = false;
  this.cdr.detectChanges();
  return;
}

      if (
        state.mode?.toLowerCase() === 'computer' &&
        state.currentPlayer === 'O'
      ) {
        console.log('Computer turn triggered!');

        // Release the human-move loading lock before the computer move.
        this.isLoading = false;
        this.makeComputerMove(this.gameState);
      }
    },

    error: (error: HttpErrorResponse) => {
      console.error('Move failed:', error.status, error.error);
      this.errorMessage =
        error.error?.message ?? 'Could not make the move.';
      this.isLoading = false;
      this.cdr.detectChanges();
    },

    complete: () => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  });
}




private makeComputerMove(state: GameState): void {
  if (
    !this.gameId ||
    state.status !== 'InProgress' ||
    state.currentPlayer !== 'O'
  ) {
    return;
  }

  const emptyCells = state.board
    .map((cell, index) => cell === null ? index : -1)
    .filter(index => index !== -1);

  if (emptyCells.length === 0) {
    return;
  }

  const index =
    emptyCells[Math.floor(Math.random() * emptyCells.length)];

  const request: MoveRequest = {
    player: 'O',
    row: Math.floor(index / 3),
    column: index % 3
  };

  this.isLoading = true;

  console.log('Sending computer move:', request);

  this.gameService.makeMove(this.gameId, request).subscribe({
    next: (computerState: GameState) => {
      this.gameState = {
        ...computerState,
        board: [...computerState.board],
        winningCells: [...(computerState.winningCells ?? [])]
      };

      this.gameId = computerState.gameId;

      console.log('Board after computer move:', computerState.board);
      console.log('Next player:', computerState.currentPlayer);
      console.log('Game status:', computerState.status);

      if (
        computerState.status === 'Won' ||
        computerState.status === 'Draw'
      ) {
        this.loadScoreboard();
      }

      this.cdr.detectChanges();
    },

    error: (error: HttpErrorResponse) => {
      console.error(
        'Computer move API failed:',
        error.status,
        error.error
      );

      this.errorMessage =
        error.error?.message ?? 'Computer move failed.';
    },

    complete: () => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  });
}



 
undoLastMove(): void {
  if (!this.gameId || this.isLoading) {
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  this.gameService.undoMove(this.gameId).subscribe({
    next: (state: GameState) => {
      this.gameState = {
        ...state,
        board: [...state.board],
        winningCells: [...(state.winningCells ?? [])],
        moveHistory: [...(state.moveHistory ?? [])]
      };

      this.gameId = state.gameId;

      this.cdr.detectChanges();

      console.log('Undo successful:', this.gameState);

      if (state.status === 'Won' || state.status === 'Draw') {
        this.loadScoreboard();
      }
    },

    error: (error: HttpErrorResponse) => {
      console.error('Could not undo move:', error.status, error.error);

      this.errorMessage =
        error.error?.message ??
        error.error?.title ??
        (error.status === 409
          ? 'Undo is not allowed in the current game state.'
          : 'Could not undo the last move.');
  this.isLoading = false;
      this.cdr.detectChanges();
    },

    complete: () => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  });
}

 resetGame(): void {
  if (!this.gameId || this.isLoading) {
    return;
  }

  this.errorMessage = '';
  this.isLoading = true;

  this.gameService.resetGame(this.gameId).subscribe({
    next: (state: GameState) => {
      console.log('Reset API returned:', state);

      this.gameId = state.gameId;

      this.gameState = {
        ...state,
        board: [...state.board],
        winningCells: [...(state.winningCells ?? [])],
        moveHistory: [...(state.moveHistory ?? [])]
      };

      this.cdr.detectChanges();
    },

    error: (error: HttpErrorResponse) => {
      console.error('Could not reset game:', error.status, error.error);
      this.errorMessage = 'Could not reset the game.';
      this.isLoading = false;
      this.cdr.detectChanges();
    },

    complete: () => {
      this.isLoading = false;
      this.cdr.detectChanges();
    }
  });
}

  loadScoreboard(): void {
  this.gameService.getScoreboard().subscribe({
    next: (scoreboard: Scoreboard) => {
      console.log('Updated scoreboard from API:', scoreboard);

      this.scoreboard = { ...scoreboard };

      this.cdr.detectChanges();
    },
    error: (error: HttpErrorResponse) => {
      console.error('Failed to load scoreboard:', error);
    }
  });
}

 onResetScoreboard(): void {
this.gameService.resetScoreboard().subscribe({
next: (result) => {
if (result) {
this.scoreboard = { ...result };
this.cdr.detectChanges();
} else {
this.gameService.getScoreboard().subscribe({
next: (scoreboard: Scoreboard) => {
this.scoreboard = { ...scoreboard };
this.cdr.detectChanges();
},
error: (error: HttpErrorResponse) => {
console.error('Could not reload scoreboard:', error);
}
});
}
},
error: (error: HttpErrorResponse) => {
console.error('Could not reset scoreboard:', error);
}
});
}

}