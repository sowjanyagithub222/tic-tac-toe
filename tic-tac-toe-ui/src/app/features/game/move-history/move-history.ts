
import { Component, Input } from '@angular/core';
import { Move } from '../../../core/models/game.model';

@Component({
  selector: 'app-move-history',
  standalone: false,
  templateUrl: './move-history.html',
  styleUrls: ['./move-history.css']
})
export class MoveHistory {
  @Input() moves: Move[] = [];
}