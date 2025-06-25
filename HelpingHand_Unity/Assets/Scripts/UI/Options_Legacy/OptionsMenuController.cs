using System;
using System.Collections.Generic;

using Sirenix.Serialization;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;
using UnityUtility.Extensions;

using SerializedMonoBehaviour = Sirenix.OdinInspector.SerializedMonoBehaviour;

public class OptionsMenuController : SerializedMonoBehaviour
{
    public bool IsOpened => m_open;

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    [Title("UI components")]
    [SerializeField] private CanvasGroup m_menuOptions;
    [SerializeField] private Selectable m_mainMenuFirstSelectable;
    [SerializeField] private Selectable m_gameplayFirstSelectable;
    [SerializeField] private Button m_resumeButton;
    [SerializeField] private Button m_defaultButton;
    [SerializeField] private Button m_mainMenuButton;

    [SerializeField] private Selectable m_notNavigable;

    [Title("Description")]
    [SerializeField] private TMP_Text m_descriptionText;
    [SerializeField] private Image m_descriptionBackground;

    [Title("Options")]
    [Title("Misc", separator: false)]
    [SerializeField] private BaseOptionController<WindowMode> m_optionWindowMode;
    [SerializeField] private BaseOptionController<DialogueReadMode> m_optionDialogueReadMode;

    [Title("Audio", separator: false)]
    [SerializeField] private BaseOptionController<float> m_optionMasterVolume;
    [SerializeField] private BaseOptionController<float> m_optionVoiceVolume;

    [Title("Subtitles", separator: false)]
    [SerializeField] private BaseOptionController<SubtitleSize> m_optionSubtitleSize;
    [SerializeField] private ColorOptionController m_optionSubtitleColor;
    [SerializeField] private BaseOptionController<float> m_optionSubtitleBackgroundOpacity;
    [SerializeField] private ColorOptionController m_optionSubtitleBackgroundColor;

    [Space]
    [OdinSerialize] private readonly IOptionController[] m_additionalOptionControllers;



    private List<IOptionController> m_optionControllers;

    private bool m_open = true;

    private GameOptionsManager m_gameOptions;

    private void Awake()
    {
        m_optionControllers = new List<IOptionController>()
        {
            m_optionWindowMode,
            m_optionDialogueReadMode,
            m_optionMasterVolume,
            m_optionVoiceVolume,
            m_optionSubtitleSize,
            m_optionSubtitleBackgroundOpacity,
        };
        m_optionControllers.AddRange(m_additionalOptionControllers);

        for (int iController = 0; iController < m_optionControllers.Count; iController++)
        {
            m_optionControllers[iController].OnSelected += OnOptionSelected;
        }
    }

    private void OnOptionSelected(IOptionController controller)
    {
        m_descriptionText.text = controller.Description;
    }

    private void Start()
    {
        m_gameOptions = GameManager.Instance.GameOptionsManager;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        for (int iController = 0; iController < m_optionControllers.Count; iController++)
        {
            m_optionControllers[iController].OnSelected -= OnOptionSelected;
        }
    }

    public void OpenOptionMenu()
    {
        if (m_open)
        {
            return;
        }

        m_menuOptions.alpha = 1;
        m_menuOptions.interactable = true;
        m_menuOptions.blocksRaycasts = true;

        SubscribeToEvents();

        GameManager.GameState currentGameState = GameManager.Instance.CurrentGameState;

        m_resumeButton.gameObject.SetActive(currentGameState != GameManager.GameState.MainMenu);

        Selectable firstSelectable = currentGameState switch
        {
            GameManager.GameState.MainMenu => m_mainMenuFirstSelectable,
            GameManager.GameState.Gameplay => m_gameplayFirstSelectable,
            _ => throw new ArgumentOutOfRangeException(),
        };
        firstSelectable.Select();

        UpdateDescriptionPanel();

        OnMenuOpened?.Invoke();

        m_open = true;
    }

    public void InitializeAllOptionValues()
    {
        OnOptionScreenModeChanged(m_optionWindowMode.Value);
        OnOptionDialogueReadModeChanged(m_optionDialogueReadMode.Value);
        OnOptionVolumeGlobalChanged(m_optionMasterVolume.Value);
        OnOptionVolumeVoiceChanged(m_optionVoiceVolume.Value);
        OnOptionSubtitleColorChanged(m_optionSubtitleColor.Value);
        OnOptionSubtitleSizeChanged(m_optionSubtitleSize.Value);
        OnOptionSubtitleBackgroundOpacityChanged(m_optionSubtitleBackgroundOpacity.Value);
        OnOptionSubtitleBackgroundColorChanged(m_optionSubtitleBackgroundColor.Value);
    }

    public void CloseOptionMenu()
    {
        if (!m_open)
        {
            return;
        }

        m_menuOptions.alpha = 0;
        m_menuOptions.interactable = false;
        m_menuOptions.blocksRaycasts = false;

        UnsubscribeFromEvents();

        OnMenuClosed?.Invoke();
        m_notNavigable.Select();

        m_open = false;
    }

    private void OnOptionVolumeGlobalChanged(float value)
    {
        AudioManager.Instance.RTPCManager.RTPC_MasterVolume.SetGlobalValue(value / 100.0f);
    }

    //private void OnOptionVolumeSFXChanged(float value)
    //{
    //    AudioManager.Instance.RTPCManager.RTPC_SFXVolume.SetGlobalValue(value);
    //}

    //private void OnOptionVolumeMusicChanged(float value)
    //{
    //    AudioManager.Instance.RTPCManager.RTPC_MusicVolume.SetGlobalValue(value);
    //}

    private void OnOptionVolumeVoiceChanged(float value)
    {
        AudioManager.Instance.RTPCManager.RTPC_VoiceVolume.SetGlobalValue(value / 100.0f);
    }



    private void SetVolume(string group, float value)
    {
        //_ = value == 0 ? -80 : Mathf.Log10(value / 100f) * 20;
        // Pas utilisé pour système avec Wwise
    }

    private void OnOptionScreenModeChanged(WindowMode value)
    {
        m_gameOptions.IsWindowed.Value = value switch
        {
            WindowMode.Windowed => true,
            WindowMode.FullScreen => false,
            _ => false
        };
    }

    private void OnOptionSubtitleColorChanged(Color value)
    {
        m_gameOptions.SubtitleColor.Value = value;
        UpdateDescriptionPanel();
    }

    private void OnOptionSubtitleSizeChanged(SubtitleSize value)
    {
        m_gameOptions.SubtitleSize.Value = value;
        UpdateDescriptionPanel();
    }

    private void OnOptionSubtitleBackgroundOpacityChanged(float value)
    {
        m_gameOptions.SubtitleBackgroundOpacity.Value = value;
        UpdateDescriptionPanel();
    }

    private void OnOptionSubtitleBackgroundColorChanged(Color color)
    {
        m_gameOptions.SubtitleBackgroundColor.Value = color;
        UpdateDescriptionPanel();
    }

    private void OnOptionDialogueReadModeChanged(DialogueReadMode value)
    {
        m_gameOptions.DialogueReadMode.Value = value;
    }

    private void OnResumeButtonClicked()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.CanvasManager.CloseOptions();
        gameManager.Paused.Value = false;
    }

    private void OnDefaultButtonClicked()
    {
        foreach (IOptionController optionController in m_optionControllers)
        {
            optionController.SetDefault();
        }
    }

    private void OnMainMenuButtonClicked()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    private void SubscribeToEvents()
    {
        m_resumeButton.onClick.AddListener(OnResumeButtonClicked);
        m_defaultButton.onClick.AddListener(OnDefaultButtonClicked);
        m_mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        m_optionWindowMode.OnValueChanged += OnOptionScreenModeChanged;
        m_optionDialogueReadMode.OnValueChanged += OnOptionDialogueReadModeChanged;

        m_optionMasterVolume.OnValueChanged += OnOptionVolumeGlobalChanged;
        m_optionVoiceVolume.OnValueChanged += OnOptionVolumeVoiceChanged;

        m_optionSubtitleColor.OnValueChanged += OnOptionSubtitleColorChanged;
        m_optionSubtitleSize.OnValueChanged += OnOptionSubtitleSizeChanged;
        m_optionSubtitleBackgroundOpacity.OnValueChanged += OnOptionSubtitleBackgroundOpacityChanged;
        m_optionSubtitleBackgroundColor.OnValueChanged += OnOptionSubtitleBackgroundColorChanged;
    }

    private void UnsubscribeFromEvents()
    {
        m_resumeButton.onClick.RemoveListener(OnResumeButtonClicked);
        m_defaultButton.onClick.RemoveListener(OnDefaultButtonClicked);
        m_mainMenuButton.onClick.RemoveListener(OnMainMenuButtonClicked);

        m_optionWindowMode.OnValueChanged -= OnOptionScreenModeChanged;
        m_optionDialogueReadMode.OnValueChanged -= OnOptionDialogueReadModeChanged;

        m_optionMasterVolume.OnValueChanged -= OnOptionVolumeGlobalChanged;
        m_optionVoiceVolume.OnValueChanged -= OnOptionVolumeVoiceChanged;

        m_optionSubtitleColor.OnValueChanged -= OnOptionSubtitleColorChanged;
        m_optionSubtitleSize.OnValueChanged -= OnOptionSubtitleSizeChanged;
        m_optionSubtitleBackgroundOpacity.OnValueChanged -= OnOptionSubtitleBackgroundOpacityChanged;
        m_optionSubtitleBackgroundColor.OnValueChanged -= OnOptionSubtitleBackgroundColorChanged;
    }

    private void UpdateDescriptionPanel()
    {
        m_descriptionText.fontSize = m_gameOptions.ToFontSize(m_optionSubtitleSize.Value);
        m_descriptionText.color = m_optionSubtitleColor.Value;
        m_descriptionBackground.color = m_optionSubtitleBackgroundColor.Value.WhereA(m_optionSubtitleBackgroundOpacity.Value);
    }
}
