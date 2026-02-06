namespace CoreSiegeDefense.Gameplay;

/// <summary>
/// Простые текстовые элементы UI, заменяющие полноценный HUD.
/// </summary>
public sealed class UI
{
    public void ShowMapIntro(MapDefinition map)
    {
        Console.Clear();
        Console.WriteLine($"Карта: {map.Name}");
        Console.WriteLine("Постройте башни на специальных плитках.");
        Console.WriteLine("B - построить башню, U - улучшить, ESC - пауза.");
        Console.WriteLine("Нажмите Enter для старта.");
        Console.ReadLine();
    }

    public void RenderHud(Level level)
    {
        Console.Clear();
        Console.WriteLine($"Ядро HP: {level.BaseHp} | Деньги: {level.Money}");
        Console.WriteLine($"Врагов на поле: {level.Enemies.Count} | Башен: {level.Towers.Count}");
        Console.WriteLine($"Пауза: ESC | Строить: B | Улучшать: U");
        Console.WriteLine("Башни:");
        foreach (var tower in level.Towers)
        {
            Console.WriteLine($"- {tower.Name} {tower.VisualVariant} ({tower.Position.X},{tower.Position.Y})");
        }
    }

    public void ShowPause(bool isPaused)
    {
        Console.WriteLine(isPaused ? "Пауза." : "Игра продолжена.");
    }
}
