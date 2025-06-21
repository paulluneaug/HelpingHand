using System;

using UnityEngine;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;

public class OptionsMenuController : MonoBehaviour
{
    public bool IsOpened => m_isOpened;

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    [Title("UI components")]
    [SerializeField] private CanvasGroup m_menuOptions;
    [SerializeField] private Selectable m_firstSelectable;
    [SerializeField] private Button m_saveButton;
    [SerializeField] private Button m_defaultButton;
    [SerializeField] private Button m_quitButton;

    [Title("Options")]
    [SerializeField] private UIAbstractOption<float> m_optionVolumeGlobal;
    [SerializeField] private UIAbstractOption<float> m_optionVolumeSFX;
    [SerializeField] private UIAbstractOption<float> m_optionVolumeMusic;
    [SerializeField] private UIAbstractOption<WindowMode> m_optionScreenMode;

    [SerializeField] private UIAbstractOption<Color> m_optionSubtitleColor;
    [SerializeField] private UIAbstractOption<SubtitleSize> m_optionSubtitleSize;
    [SerializeField] private UIAbstractOption<float> m_optionSubtitleOpacity;
    [SerializeField] private UIAbstractOption<DialogueReadMode> m_optionDialogueReadMode;



    private UIAbstractDefaultable[] m_options;
    private bool m_isOpened;

    private GameOptionsManager m_gameOptions;

    private void Awake()
    {
        m_options = new UIAbstractDefaultable[]
        {
            m_optionVolumeGlobal,
            m_optionVolumeSFX,
            m_optionVolumeMusic,
            m_optionScreenMode,
            m_optionSubtitleSize,
            m_optionSubtitleOpacity,
            m_optionDialogueReadMode
        };
    }

    private void Start()
    {
        m_gameOptions = GameManager.Instance.GameOptionsManager;
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    public void OpenOptionMenu()
    {
        m_menuOptions.alpha = 1;
        m_menuOptions.interactable = true;
        m_menuOptions.blocksRaycasts = true;

        SubscribeToEvents();

        m_isOpened = true;
        m_firstSelectable.Select();
        OnMenuOpened?.Invoke();
    }

    public void CloseOptionMenu()
    {
        m_menuOptions.alpha = 0;
        m_menuOptions.interactable = false;
        m_menuOptions.blocksRaycasts = false;

        UnsubscribeFromEvents();

        m_isOpened = false;
        OnMenuClosed?.Invoke();
    }

    private void OnOptionVolumeGlobalChanged(float value)
    {
        SetVolume("GlobalVolume", value);
    }

    private void OnOptionVolumeSFXChanged(float value)
    {
        SetVolume("SFXVolume", value);
    }

    private void OnOptionVolumeMusicChanged(float value)
    {
        SetVolume("MusicVolume", value);
    }

    private void SetVolume(string group, float value)
    {
        _ = value == 0 ? -80 : Mathf.Log10(value / 100f) * 20;
        // @TODO Set Volume
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
    }

    private void OnOptionSubtitleSizeChanged(SubtitleSize value)
    {
        m_gameOptions.SubtitleSize.Value = value;
    }

    private void OnOptionSubtitleOpacityChanged(float value)
    {
        m_gameOptions.SubtitleOpacity.Value = value;
    }

    private void OnOptionDialogueReadModeChanged(DialogueReadMode value)
    {
        m_gameOptions.DialogueReadMode.Value = value;
    }

    private void OnSaveButtonClicked()
    {
        // TODO save ?
        CloseOptionMenu();
    }

    private void OnDefaultButtonClicked()
    {
        foreach (var option in m_options)
        {
            option.SetDefault();
        }
    }

    private void OnQuitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }

    private void SubscribeToEvents()
    {
        m_saveButton.onClick.AddListener(OnSaveButtonClicked);
        m_defaultButton.onClick.AddListener(OnDefaultButtonClicked);
        m_quitButton.onClick.AddListener(OnQuitButtonClicked);

        m_optionVolumeGlobal.OnValueChangedEvent += OnOptionVolumeGlobalChanged;
        m_optionVolumeSFX.OnValueChangedEvent += OnOptionVolumeSFXChanged;
        m_optionVolumeMusic.OnValueChangedEvent += OnOptionVolumeMusicChanged;
        m_optionScreenMode.OnValueChangedEvent += OnOptionScreenModeChanged;

        m_optionSubtitleColor.OnValueChangedEvent += OnOptionSubtitleColorChanged;
        m_optionSubtitleSize.OnValueChangedEvent += OnOptionSubtitleSizeChanged;
        m_optionSubtitleOpacity.OnValueChangedEvent += OnOptionSubtitleOpacityChanged;
        m_optionDialogueReadMode.OnValueChangedEvent += OnOptionDialogueReadModeChanged;
    }

    private void UnsubscribeFromEvents()
    {
        m_saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        m_defaultButton.onClick.RemoveListener(OnDefaultButtonClicked);
        m_quitButton.onClick.RemoveListener(OnQuitButtonClicked);

        m_optionVolumeGlobal.OnValueChangedEvent -= OnOptionVolumeGlobalChanged;
        m_optionVolumeSFX.OnValueChangedEvent -= OnOptionVolumeSFXChanged;
        m_optionVolumeMusic.OnValueChangedEvent -= OnOptionVolumeMusicChanged;
        m_optionScreenMode.OnValueChangedEvent -= OnOptionScreenModeChanged;

        m_optionSubtitleColor.OnValueChangedEvent -= OnOptionSubtitleColorChanged;
        m_optionSubtitleSize.OnValueChangedEvent -= OnOptionSubtitleSizeChanged;
        m_optionSubtitleOpacity.OnValueChangedEvent -= OnOptionSubtitleOpacityChanged;
        m_optionDialogueReadMode.OnValueChangedEvent -= OnOptionDialogueReadModeChanged;
    }
}
