import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { GameBoard } from './features/game/game-board/game-board';
import { GameControls } from './features/game/game-controls/game-controls';
import { MoveHistory } from './features/game/move-history/move-history';
import { Scoreboard } from './features/game/scoreboard/scoreboard';
import { HttpClientModule } from '@angular/common/http';
@NgModule({
  declarations: [App, GameBoard, GameControls, MoveHistory, Scoreboard],
  imports: [BrowserModule, AppRoutingModule],
  providers: [provideBrowserGlobalErrorListeners()],
  bootstrap: [App],
})
export class AppModule {}
