using Signals;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameUI : MonoBehaviour, IDisposable
{
    [SerializeField] private CanvasGroup _endGameScreenGroup;
    [SerializeField] private Image _endGameBackground;
    [SerializeField] private TextMeshProUGUI _endGameScreenText;

    private bool _levelCompleted;
    private IEventBus _eventBus;

    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;

        _eventBus.Subscribe<LevelFinishedSignal>(OnCompleteLevel);
        _eventBus.Subscribe<LevelFailedSignal>(OnLevelFail);
        _eventBus.Subscribe<LevelReloadSignal>(OnLevelReload);
    }

    private void OnCompleteLevel(LevelFinishedSignal signal)
    {
        if (_levelCompleted) return;

        _levelCompleted = true;
        ShowEndGameScreen(Color.green, "Level Completed!");
    }

    private void OnLevelFail(LevelFailedSignal signal)
    {
        if (_levelCompleted) return;

        _levelCompleted = true;
        ShowEndGameScreen(Color.red, "Level Failed!");
    }

    private void OnLevelReload(LevelReloadSignal signal)
    {
        _endGameScreenGroup.alpha = 0;
        _levelCompleted = false;
    }

    private void ShowEndGameScreen(Color color, string text)
    {
        _endGameScreenGroup.alpha = 1;
        _endGameBackground.color = color;
        _endGameScreenText.text = text;
    }

    public void Dispose()
    {
        _eventBus.Unsubscribe<LevelFinishedSignal>(OnCompleteLevel);
        _eventBus.Unsubscribe<LevelFailedSignal>(OnLevelFail);
    }
}
