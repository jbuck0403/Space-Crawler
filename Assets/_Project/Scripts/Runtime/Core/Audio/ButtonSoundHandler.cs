using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSoundHandler
    : MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler,
        IPointerClickHandler
{
    private Button button;
    private TextMeshProUGUI text;
    private Color defaultTextColor;
    private Color hoverTextColor = Color.white;

    private void Awake()
    {
        button = GetComponent<Button>();
        text = GetComponentInChildren<TextMeshProUGUI>();

        defaultTextColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            text.color = hoverTextColor;
            AudioManager.PlayButtonHoveredSound();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            text.color = defaultTextColor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (button.interactable)
        {
            AudioManager.PlayButtonClickedSound();
        }
    }
}
