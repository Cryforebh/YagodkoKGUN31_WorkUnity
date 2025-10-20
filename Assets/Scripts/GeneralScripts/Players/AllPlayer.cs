using System.Collections.Generic;
using UnityEngine;

public class AllPlayer : MonoBehaviour
{
    private Dictionary<EnumPlayers, List<IUnitMain>> _playersCollection = new Dictionary<EnumPlayers, List<IUnitMain>>();
    public Dictionary<EnumPlayers, List<IUnitMain>> PlayersCollection => _playersCollection;

    public void AddUnitOnPlayer(EnumPlayers playerEnum, IUnitMain unit)
    {
        if (!_playersCollection.ContainsKey(playerEnum))
        {
            //Debug.Log($"Внимание: Был создан новый список Unit'тов для {playerEnum}!");
            _playersCollection[playerEnum] = new List<IUnitMain>();
        }
        _playersCollection[playerEnum].Add(unit);
    }

    public void RemoveUnitOnPlayer(EnumPlayers playersEnum, IUnitMain unit)
    {
        if (!_playersCollection.ContainsKey(playersEnum))
            Debug.LogError($"Ошибка: {playersEnum} - Такого списка не существует в {_playersCollection}!");

        if (!_playersCollection[playersEnum].Contains(unit))
            Debug.LogError($"Ошибка: {unit} - Такого обьекта не существует в {playersEnum}!");

        if (_playersCollection.ContainsKey(playersEnum))
            _playersCollection[playersEnum].Remove(unit);

        if (_playersCollection[playersEnum].Count == 0)
        {
            Debug.LogWarning($"Внимание: Список {playersEnum} - Пуст!");
        }
    }

    public EnumPlayers GetPlayerToWhomUnitBelongs(IUnitMain unit)
    {
        // Если юнит уже хранит информацию о своём владельце
        if (unit.Player != EnumPlayers.None)
            return unit.Player; // Просто возвращаем значение из свойства Unit

        Debug.LogWarning($"Внимание: Списков с {unit} не обнаруженно! Присвоенно значение {EnumPlayers.None}!");
        return EnumPlayers.None;
    }

}
