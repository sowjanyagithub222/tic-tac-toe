
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Scoreboard as ScoreboardModel } from '../../../core/models/game.model';

@Component({
  selector: 'app-scoreboard',
  standalone: false,
  templateUrl: './scoreboard.html',
  styleUrls: ['./scoreboard.css']
})
export class Scoreboard {
  @Input() scoreboard: ScoreboardModel | null = null;

  @Output() resetScoreboard = new EventEmitter<void>();

  onResetScoreboard(): void {
    this.resetScoreboard.emit();
  }
}
