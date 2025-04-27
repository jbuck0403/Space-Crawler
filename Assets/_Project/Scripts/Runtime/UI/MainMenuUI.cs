using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField]
    GameObject continueButton;

    private void Awake()
    {
        continueButton.SetActive(false);
        ShowContinueButton();
    }

    public void Initialize()
    {
        ShowContinueButton();
    }

    private void ShowContinueButton()
    {
        if (GameData.HasSaveData())
        {
            continueButton.SetActive(true);
        }
    }
}
