using UnityEngine;
using UnityEngine.UI;

public class ClickListerner : MonoBehaviour
{
    public Button leftButton;

    public Button rightButton;

    private void Start()
    {
        leftButton.onClick.AddListener(OnLeftButtonClick);
        rightButton.onClick.AddListener(OnRightButtonClick);
    }

    private void OnRightButtonClick()
    {
        Debug.Log("Right Button Clicked");
    }

    private void OnLeftButtonClick()
    {
        Debug.Log("Left Button Clicked");
    }
}