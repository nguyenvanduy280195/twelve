
public enum GameState
{
    ChoosingPlayer = 0,
    Player1Turn,
    Player2Turn,
    Swapping,
    UndoSwapping,
    FindingMatches,
    RemovingMatches,
    SetupItemsFall,
    SpawningNewItems,
    ItemsFalling,
    ScanningMatchesInAlteredColumns,
    ExplosionAnimationWaiting,
    CheckingGameOver,
    CheckingNoSwappable, // TODO unused
    GameOver,
    WaitingForAnimation

}