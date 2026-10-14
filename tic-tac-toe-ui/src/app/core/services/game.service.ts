import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  GameState,
  Scoreboard,
  CreateGameRequest,
  MoveRequest
} from '../models/game.model';

@Injectable({
  providedIn: 'root'
})
export class GameService {


  private apiUrl = 'https://localhost:5088/api';
private scoreboardUrl = 'https://localhost:5088/api/scoreboard';

  constructor(private http: HttpClient) {}

  createGame(request: CreateGameRequest): Observable<GameState> {
    return this.http.post<GameState>(
      `${this.apiUrl}/games`,
      request
    );
  }

  getGame(gameId: string): Observable<GameState> {
    return this.http.get<GameState>(
      `${this.apiUrl}/games/${gameId}`
    );
  }

  makeMove(
    gameId: string,
    request: MoveRequest
  ): Observable<GameState> {
    return this.http.post<GameState>(
      `${this.apiUrl}/games/${gameId}/moves`,
      request
    );
  }

  undoMove(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(
      `${this.apiUrl}/games/${gameId}/undo`,
      {}
    );
  }

  resetGame(gameId: string): Observable<GameState> {
    return this.http.post<GameState>(
      `${this.apiUrl}/games/${gameId}/reset`,
      {}
    );
  }

  getScoreboard(): Observable<Scoreboard> {
    return this.http.get<Scoreboard>(
      `${this.apiUrl}/scoreboard`
    );
  }

  resetScoreboard(): Observable<Scoreboard> {
    return this.http.post<Scoreboard>(
      `${this.apiUrl}/scoreboard/reset`,
      {}
    );
  }
}