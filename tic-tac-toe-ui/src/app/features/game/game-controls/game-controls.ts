
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { GameMode, GameState } from '../../../core/models/game.model';

@Component({
  selector: 'app-game-controls',
  standalone: false,
  templateUrl: './game-controls.html',
  styleUrls: ['./game-controls.css']
})
export class GameControls {
  @Input() gameState: GameState | null = null;
@Input() selectedMode: GameMode = 'TwoPlayer';
  @Output() newGame = new EventEmitter<GameMode>();
  @Output() undo = new EventEmitter<void>();
  @Output() reset = new EventEmitter<void>();
   @Output() selectedModeChange = new EventEmitter<GameMode>();
  //selectedMode: GameMode = 'TwoPlayer';

  onModeChange(mode: GameMode): void {
    this.selectedMode = mode as GameMode;
    this.selectedModeChange.emit(this.selectedMode);
    console.log('Selected mode:', this.selectedMode);

  }

  onNewGame(): void {
        console.log('Starting new game with mode:', this.selectedMode);
    this.newGame.emit(this.selectedMode);
  }

  onUndo(): void {
    this.undo.emit();
  }

  onReset(): void {
    this.reset.emit();
  }

  get canUndo(): boolean {
    return !!this.gameState
      && this.gameState.status === 'InProgress'
      && this.gameState.moveHistory.length > 0;
  }

  get canReset(): boolean {
    return !!this.gameState;
  }
}
