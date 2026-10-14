import { ComponentFixture, TestBed } from '@angular/core/testing';
import { GameBoard } from './game-board';

describe('GameBoard', () => {
  let component: GameBoard;
  let fixture: ComponentFixture<GameBoard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [GameBoard],
    }).compileComponents();

    fixture = TestBed.createComponent(GameBoard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
