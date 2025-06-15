using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.Extensions;

[RequireComponent(typeof(RectTransform))]
public class DialoguePanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_dialoguePanel;

    [SerializeField] private TMP_Text m_subtitleText;
    [SerializeField] private Image m_subtitleBackground;

    [NonSerialized] private GameOptionsManager m_optionsManager;

    private void Awake()
    {
        m_optionsManager = GameManager.Instance.GameOptionsManager;
    }

    public void OpenPanel()
    {
        m_dialoguePanel.alpha = 1;
        m_dialoguePanel.interactable = true;
        m_dialoguePanel.blocksRaycasts = true;

        UpdateSubtitleSettings();

        SubscribeToEvents();
    }

    public void ClosePanel()
    {
        m_dialoguePanel.alpha = 0;
        m_dialoguePanel.interactable = false;
        m_dialoguePanel.blocksRaycasts = false;

        UnsubscribeFromEvents();
    }

    private void UpdateSubtitleSettings()
    {
        m_subtitleBackground.color = m_subtitleBackground.color.WhereA(m_optionsManager.SubtitleOpacity.Value / 100.0f);
        m_subtitleText.color = m_optionsManager.SubtitleColor.Value;
        m_subtitleText.fontSize = m_optionsManager.ToFontSize(m_optionsManager.SubtitleSize.Value);
    }

    private void SubscribeToEvents()
    {
        m_optionsManager.SubtitleOpacity.OnValueChanged += OnSubtitleOpacityChanged;
        m_optionsManager.SubtitleColor.OnValueChanged += OnSubtitleColorChanged;
        m_optionsManager.SubtitleSize.OnValueChanged += OnSubtitleSizeChanged;
    }

    private void UnsubscribeFromEvents()
    {
        m_optionsManager.SubtitleOpacity.OnValueChanged -= OnSubtitleOpacityChanged;
        m_optionsManager.SubtitleColor.OnValueChanged -= OnSubtitleColorChanged;
        m_optionsManager.SubtitleSize.OnValueChanged -= OnSubtitleSizeChanged;
    }

    private void OnSubtitleOpacityChanged(float subtitleOpacity)
    {
        m_subtitleBackground.color = m_subtitleBackground.color.WhereA(subtitleOpacity / 100.0f);
    }

    private void OnSubtitleColorChanged(Color subtitleColor)
    {
        m_subtitleText.color = subtitleColor;
    }

    private void OnSubtitleSizeChanged(SubtitleSize subtitleSize)
    {
        m_subtitleText.fontSize = m_optionsManager.ToFontSize(subtitleSize);
    }
}
