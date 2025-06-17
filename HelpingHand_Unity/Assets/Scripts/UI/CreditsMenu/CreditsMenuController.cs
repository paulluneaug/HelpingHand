using System;

using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Button m_backToMenuButton;

    public void OpenCreditsMenu()
    {
        m_canvasGroup.alpha = 1;
        m_canvasGroup.interactable = true;
        m_canvasGroup.blocksRaycasts = true;

        m_backToMenuButton.onClick.AddListener(OnBackToMenuClicked);

        m_backToMenuButton.Select();
    }

    public void CloseCreditsMenu()
    {
        m_canvasGroup.alpha = 0;
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;

        m_backToMenuButton.onClick.RemoveListener(OnBackToMenuClicked);
    }

    private void OnBackToMenuClicked()
    {
        GameManager.Instance.CanvasManager.CloseCredits();
    }
}
