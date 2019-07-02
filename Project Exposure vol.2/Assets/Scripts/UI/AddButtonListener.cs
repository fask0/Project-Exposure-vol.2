using UnityEngine.UI;
using UnityEngine;

public class AddButtonListener : MonoBehaviour
{
    private enum ButtonState
    {
        Play,
        Stop,
        CreatureIcon,
        GoToMenu,
        ExitMenu,
        MinimapZoomIn,
        MinimapZoomOut,
        RestartButton,
        ShowDailyHighscores,
        ShowYearlyHighscores,
        Pause,
        Unpause,
        NextQuestion
    }

    [SerializeField] private ButtonState _buttonState;

    void Start()
    {
        Button button = GetComponent<Button>();

        switch (_buttonState)
        {
            case ButtonState.Play:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.PlayAudioSample(); });
                break;
            case ButtonState.Stop:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.StopAudioSample(); });
                break;
            case ButtonState.CreatureIcon:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.GotoDescription(gameObject); });
                break;
            case ButtonState.GoToMenu:
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.GotoFirstNewDescription(); });
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.ReduceAllVolume(); });
                break;
            case ButtonState.ExitMenu:
                button.onClick.AddListener(() => { SingleTons.SoundWaveManager.ResetPlayerTextures(); });
                button.onClick.AddListener(() => { SingleTons.CollectionsManager.IncreaseAllVolume(); });
                break;
            case ButtonState.MinimapZoomIn:
                button.onClick.AddListener(() => { SingleTons.MinimapManager.ZoomIn(); });
                break;
            case ButtonState.MinimapZoomOut:
                button.onClick.AddListener(() => { SingleTons.MinimapManager.ZoomOut(); });
                break;
            case ButtonState.RestartButton:
                button.onClick.AddListener(() => { SingleTons.GameController.ResetGame(); });
                break;
            case ButtonState.ShowDailyHighscores:
                button.onClick.AddListener(() => { SingleTons.ScoreManager.ShowDaily(); });
                break;
            case ButtonState.ShowYearlyHighscores:
                button.onClick.AddListener(() => { SingleTons.ScoreManager.ShowYearly(); });
                break;
            case ButtonState.Pause:
                button.onClick.AddListener(() => { SingleTons.GameController.PauseGame(); });
                break;
            case ButtonState.Unpause:
                button.onClick.AddListener(() => { SingleTons.GameController.UnpauseGame(); });
                break;
            case ButtonState.NextQuestion:
                button.onClick.AddListener(() => { SingleTons.ScoreManager.NextQuestion(gameObject); });
                break;
        }
    }
}
