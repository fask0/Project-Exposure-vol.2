using UnityEngine.UI;
using UnityEngine;

public class CorrectTheFuckingScrollbar : MonoBehaviour
{
    void Start()
    {
        GetComponent<Scrollbar>().direction = Scrollbar.Direction.BottomToTop;
    }
}
