using UnityEngine;

public class RunConclusionUI : MonoBehaviour
{
    [SerializeField]
    GameObject winText;

    [SerializeField]
    GameObject loseText;

    public void ShowRunConclusionText(bool success)
    {
        if (success)
            EnableWinText();
        else
            EnableLoseText();
    }

    private void EnableWinText()
    {
        loseText.SetActive(false);
        winText.SetActive(true);
    }

    private void EnableLoseText()
    {
        winText.SetActive(false);
        loseText.SetActive(true);
    }
}
