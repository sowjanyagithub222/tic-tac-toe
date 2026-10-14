
import {
  Component,
  EventEmitter,
  Input,
  Output,
  OnChanges,
  SimpleChanges
} from '@angular/core';

import {
  GameState,
  MoveRequest,
  Scoreboard
} from '../../../core/models/game.model';

import { GameService } from '../../../core/services/game.service';

@Component({
  selector: 'app-game-board',
  standalone: false,
  templateUrl: './game-board.html',
  styleUrls: ['./game-board.css']
})
export class GameBoard implements OnChanges {

  @Input() gameState: GameState | null = null;
  @Input() isLoading = false;

  @Output() move = new EventEmitter<MoveRequest>();
  @Output() gameStateChange = new EventEmitter<GameState>();

  errorMessage = '';
  scoreboard: Scoreboard | null = null;

  constructor(private gameService: GameService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['gameState']) {
      console.log(
        'GameBoard received state:',
        this.gameState?.currentPlayer,
        this.gameState?.board
      );

      // Refresh scoreboard when a game finishes.
      if (
        this.gameState?.status === 'Won' ||
        this.gameState?.status === 'Draw'
      ) {
        this.loadScoreboard();
      }
    }
  }


onCellClick(index: number): void {
  const game = this.gameState;

  if (!game || this.isLoading) {
    return;
  }

  if (game.status !== 'InProgress') {
    return;
  }

  if (game.board[index] !== null) {
    return;
  }

  const request: MoveRequest = {
    player: game.currentPlayer,
    row: Math.floor(index / 3),
    column: index % 3
  };

  console.log('Cell clicked:', index);
  console.log('Sending move:', request);

  this.move.emit(request);
}
getCellLabel(index: number): string {
  const row = Math.floor(index / 3) + 1;
  const column = (index % 3) + 1;

  return `Row ${row}, Column ${column}`;
}

  loadScoreboard(): void {
    this.gameService.getScoreboard().subscribe({
      next: (result: Scoreboard) => {
        this.scoreboard = result;
      },
      error: (error) => {
        console.error('Failed to load scoreboard', error);
        this.errorMessage = 'Failed to load scoreboard.';
      }
    });
  }

  isWinningCell(index: number): boolean {
    return this.gameState?.winningCells?.includes(index) ?? false;
  }

  getStatusMessage(): string {
    if (!this.gameState) {
      return 'Start a new game to play.';
    }

    if (this.gameState.status === 'Won') {
      return `Player ${this.gameState.winner} wins!`;
    }

    if (this.gameState.status === 'Draw') {
      return "It's a draw!";
    }

    return `Player ${this.gameState.currentPlayer}'s turn`;
  }
}
