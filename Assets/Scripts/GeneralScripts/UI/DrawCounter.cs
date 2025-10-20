using TMPro;
using UnityEngine;
using Zenject;

public class DrawCounter : MonoBehaviour
{
    [Inject] private IPlayerManager _playerManager;
    [Inject] private LocalizationManager _localizationManager;

    [SerializeField] private TMP_Text _counter;

    private string _nameText;
    private int _count;

    private void Start()
    {
        _localizationManager.ChangeLanguageEvent += UpdateText;
        _playerManager.OnActivePlayerChanged += CountUpdate;
        _playerManager.OnWinnerDeclared += CounterDisable;
        _nameText = _localizationManager.GetText(EnumTextLocalization.draw);
        _count = _playerManager.CountToDraw;
        _counter.text = $"{_nameText}: {_count}";

        _counter.enabled = false;
    }

    private void CounterDisable(EnumPlayers obj)
    {
        _counter.enabled = false;
    }

    private void CountUpdate(EnumPlayers obj)
    {
        if (_count > _playerManager.CountToDraw)
        {
            _count = _playerManager.CountToDraw;
            _counter.text = $"{_nameText}: {_count}";
            _counter.enabled = true;
        }
    }

    private void UpdateText(int obj)
    {
        _nameText = _localizationManager.GetText(EnumTextLocalization.draw);
        _counter.text = $"{_nameText}: {_count}";
    }

    private void OnDestroy()
    {
        _localizationManager.ChangeLanguageEvent -= UpdateText;
        _playerManager.OnActivePlayerChanged -= CountUpdate;
        _playerManager.OnWinnerDeclared -= CounterDisable;
    }
}
