using UnityEngine;
using UnityEngine.UI;

public class WeaponIconUI : MonoBehaviour
{
    [SerializeField]
    private GameObject background;

    [SerializeField]
    private Image icon;

    public void EnableBackground(bool enable = true)
    {
        background.SetActive(enable);
    }

    public Image GetIcon()
    {
        return icon;
    }
}
