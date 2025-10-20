
using System;
using System.Collections.Generic;


public interface IPlayerManager
{
    public event Action<EnumPlayers> OnActivePlayerChanged; // Событие изменения активного игрока
    public event Action<EnumPlayers> OnWinnerDeclared; // Событие Победы

    public EnumPlayers ActivePlayer { get; set; }

    public int CountToDraw { get; }
    public bool Draw { get; }
    public bool Win { get; }

    /// <summary>
    /// Проверка на победителя (Если он есть).
    /// </summary>
    /// <returns></returns>
    public EnumPlayers? CheckWinner();

    /// <summary>
    /// Фильтрация Игроков с Юнитами.
    /// </summary>
    /// <returns></returns>
    public List<EnumPlayers> GetPlayersWhithUnits();

    /// <summary>
    /// Проверяет, есть ли игрок, у котого остался всего один юнит.
    /// </summary>
    /// <returns></returns>
    public bool IsCountOneUnit();

    /// <summary>
    /// Смена игрока
    /// </summary>
    public void ChangePlayer();
}
