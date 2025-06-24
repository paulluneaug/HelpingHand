using System;

using UnityEngine;
using UnityEngine.UI;

public class CreditsMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_canvasGroup;
    [SerializeField] private Button m_backToMenuButton;

    [SerializeField] private Selectable m_notNavigable;

    [NonSerialized] private bool m_open = true;
    
    public void OpenCreditsMenu()
    {
        if (m_open)
        {
            return;
        }
        
        m_canvasGroup.alpha = 1;
        m_canvasGroup.interactable = true;
        m_canvasGroup.blocksRaycasts = true;

        m_backToMenuButton.onClick.AddListener(OnBackToMenuClicked);

        m_backToMenuButton.Select();

        m_open = true;
    }

    public void CloseCreditsMenu()
    {
        if (!m_open)
        {
            return;
        }
        
        m_canvasGroup.alpha = 0;
        m_canvasGroup.interactable = false;
        m_canvasGroup.blocksRaycasts = false;

        m_backToMenuButton.onClick.RemoveListener(OnBackToMenuClicked);
        m_notNavigable.Select();

        m_open = false;
    }

    private void OnBackToMenuClicked()
    {
        GameManager.Instance.CanvasManager.CloseCredits();
    }
}
