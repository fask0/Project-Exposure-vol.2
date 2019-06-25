using UnityEngine;

public class MainCanavasManager : MonoBehaviour
{
    public static GameObject Joystick;
    public static GameObject Scanprogress;
    public static GameObject Spectrograms;
    public static GameObject Codex;
    public static GameObject ScoreText;
    public static GameObject Minimap;
    public static GameObject ResolutionScreen;
    public static GameObject PauseMenu;
    public static GameObject HelpMenu;

    private void Awake()
    {
        SingleTons.MainCanavasManager = this;

        for (int i = 0; i < transform.childCount; i++)
        {
            switch (transform.GetChild(i).name.ToLower())
            {
                case "joystick":
                    Joystick = transform.GetChild(i).gameObject;
                    break;

                case "scanprogress":
                    Scanprogress = transform.GetChild(i).gameObject;
                    break;

                case "spectrograms":
                    Spectrograms = transform.GetChild(i).gameObject;
                    break;

                case "codex":
                    Codex = transform.GetChild(i).gameObject;
                    break;

                case "minimap":
                    Minimap = transform.GetChild(i).gameObject;
                    break;

                case "resolutionscreen":
                    ResolutionScreen = transform.GetChild(i).gameObject;
                    break;

                case "pausemenu":
                    PauseMenu = transform.GetChild(i).gameObject;
                    break;

                case "helpmenu":
                    HelpMenu = transform.GetChild(i).gameObject;
                    break;

                default:
                    break;
            }
        }
    }
}
