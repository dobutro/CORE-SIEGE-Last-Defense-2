namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Главный игровой цикл и переключение экранов.
/// </summary>
public sealed class Game
{
    private readonly Menu _menu = new();
    private readonly UI _ui = new();
    private Level? _level;
    private GameState _state = GameState.MainMenu;

    public void Run()
    {
        while (_state != GameState.Exit)
        {
            switch (_state)
            {
                case GameState.MainMenu:
                    _state = _menu.ShowMainMenu();
                    break;
                case GameState.MapSelect:
                    _state = HandleMapSelect();
                    break;
                case GameState.Upgrades:
                    _state = _menu.ShowUpgrades();
                    break;
                case GameState.Playing:
                    _state = RunLevel();
                    break;
                case GameState.GameOver:
                    _state = _menu.ShowGameOver();
                    break;
            }
        }
    }

    private GameState HandleMapSelect()
    {
        var map = _menu.SelectMap();
        if (map is null)
        {
            return GameState.MainMenu;
        }

        _level = new Level(map);
        _ui.ShowMapIntro(map);
        return GameState.Playing;
    }

    private GameState RunLevel()
    {
        if (_level is null)
        {
            return GameState.MainMenu;
        }

        var isPaused = false;
        var tick = new GameClock();

        while (!_level.IsGameOver && !_level.IsCompleted)
        {
            tick.Tick();
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                {
                    isPaused = !isPaused;
                    _ui.ShowPause(isPaused);
                }

                if (key == ConsoleKey.B)
                {
                    _menu.ShowBuildMenu(_level);
                }

                if (key == ConsoleKey.U)
                {
                    _menu.ShowTowerUpgradeMenu(_level);
                }
            }

            if (!isPaused)
            {
                _level.Update(tick.DeltaTime);
                _ui.RenderHud(_level);
            }

            Thread.Sleep(120);
        }

        return _level.IsGameOver ? GameState.GameOver : GameState.MainMenu;
    }
}

public enum GameState
{
    MainMenu,
    MapSelect,
    Playing,
    Upgrades,
    GameOver,
    Exit
}

public sealed class GameClock
{
    private DateTime _lastTick = DateTime.UtcNow;
    public float DeltaTime { get; private set; }

    public GameClock()
    {
        _lastTick = DateTime.UtcNow;
    }

    public void Tick()
    {
        var now = DateTime.UtcNow;
        DeltaTime = (float)(now - _lastTick).TotalSeconds;
        _lastTick = now;
    }
}
