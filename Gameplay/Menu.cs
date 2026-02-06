namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Логика меню и простых текстовых экранов.
/// </summary>
public sealed class Menu
{
    public GameState ShowMainMenu()
    {
        Console.Clear();
        Console.WriteLine("CORE SIEGE: Last Defense");
        Console.WriteLine("1) Играть");
        Console.WriteLine("2) Улучшения");
        Console.WriteLine("3) Выход");
        Console.Write("Выбор: ");

        return Console.ReadLine() switch
        {
            "1" => GameState.MapSelect,
            "2" => GameState.Upgrades,
            "3" => GameState.Exit,
            _ => GameState.MainMenu
        };
    }

    public MapDefinition? SelectMap()
    {
        Console.Clear();
        Console.WriteLine("Выбор карты:");
        var maps = MapDefinition.CreateDefaultMaps();
        for (var index = 0; index < maps.Count; index++)
        {
            Console.WriteLine($"{index + 1}) {maps[index].Name}");
        }

        Console.WriteLine("0) Назад");
        Console.Write("Выбор: ");
        var input = Console.ReadLine();
        if (input == "0")
        {
            return null;
        }

        if (int.TryParse(input, out var mapIndex) && mapIndex >= 1 && mapIndex <= maps.Count)
        {
            return maps[mapIndex - 1];
        }

        return null;
    }

    public GameState ShowUpgrades()
    {
        Console.Clear();
        Console.WriteLine("Экран улучшений мета-прогрессии.");
        Console.WriteLine("В этой демо-версии улучшения башен доступны во время игры.");
        Console.WriteLine("Нажмите Enter, чтобы вернуться.");
        Console.ReadLine();
        return GameState.MainMenu;
    }

    public GameState ShowGameOver()
    {
        Console.Clear();
        Console.WriteLine("Энергетическое ядро разрушено.");
        Console.WriteLine("1) Перезапуск");
        Console.WriteLine("2) Главное меню");
        Console.Write("Выбор: ");
        return Console.ReadLine() switch
        {
            "1" => GameState.MapSelect,
            _ => GameState.MainMenu
        };
    }

    public void ShowBuildMenu(Level level)
    {
        Console.WriteLine();
        Console.WriteLine("Постройка башни: выберите тип и слот (формат X,Y).");
        Console.WriteLine($"Доступные средства: {level.Money}");
        Console.WriteLine("1) Пулемётная");
        Console.WriteLine("2) Лазерная");
        Console.WriteLine("3) Ракетная");
        Console.WriteLine("4) Замедляющая");
        Console.WriteLine("5) Электрическая цепь");
        Console.WriteLine("6) Огнемёт");
        Console.Write("Тип: ");
        var typeInput = Console.ReadLine();
        Console.Write("Слот: ");
        var slotInput = Console.ReadLine();

        if (!TryParseSlot(slotInput, out var position))
        {
            Console.WriteLine("Неверный формат.");
            return;
        }

        var tower = typeInput switch
        {
            "1" => new MachineGunTower(position),
            "2" => new LaserTower(position),
            "3" => new RocketTower(position),
            "4" => new SlowTower(position),
            "5" => new ChainLightningTower(position),
            "6" => new FlamethrowerTower(position),
            _ => null
        };

        if (tower is null || !level.BuildTower(tower))
        {
            Console.WriteLine("Не удалось построить башню. Проверьте слот и ресурсы.");
        }
    }

    public void ShowTowerUpgradeMenu(Level level)
    {
        Console.WriteLine();
        Console.WriteLine("Улучшение башни: введите позицию X,Y.");
        Console.WriteLine($"Доступные средства: {level.Money}");
        Console.Write("Позиция: ");
        var input = Console.ReadLine();
        if (!TryParseSlot(input, out var position))
        {
            Console.WriteLine("Неверный формат.");
            return;
        }

        var tower = level.Towers.FirstOrDefault(t => t.Position == position);
        if (tower is null)
        {
            Console.WriteLine("Башня не найдена.");
            return;
        }

        if (!level.UpgradeTower(tower))
        {
            Console.WriteLine("Улучшение недоступно.");
        }
    }

    private static bool TryParseSlot(string? input, out GridPosition position)
    {
        position = default;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var parts = input.Split(',');
        if (parts.Length != 2)
        {
            return false;
        }

        if (!int.TryParse(parts[0], out var x) || !int.TryParse(parts[1], out var y))
        {
            return false;
        }

        position = new GridPosition(x, y);
        return true;
    }
}
