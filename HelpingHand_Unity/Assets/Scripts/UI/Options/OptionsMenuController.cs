using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;

public class OptionsMenuController : MonoBehaviour 
{
    public bool IsOpened => m_isOpened;

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    [SerializeField] private InputActionReference m_pauseAction;

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
    [SerializeField] private UIAbstractOption<float> m_optionGameSpeed;

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
            m_optionGameSpeed 
        };
    }

    private void Start()
    {
        m_gameOptions = GameManager.Instance.GameOptionsManager;

        m_pauseAction.action.performed += OnGamePaused;
        m_saveButton.onClick.AddListener(OnSaveButtonClicked);
        m_defaultButton.onClick.AddListener(OnDefaultButtonClicked);
        m_quitButton.onClick.AddListener(OnQuitButtonClicked);

        m_optionVolumeGlobal.OnValueChangedEvent    += OnOptionVolumeGlobalChanged;
        m_optionVolumeSFX.OnValueChangedEvent       += OnOptionVolumeSFXChanged;
        m_optionVolumeMusic.OnValueChangedEvent     += OnOptionVolumeMusicChanged;
        m_optionScreenMode.OnValueChangedEvent      += OnOptionScreenModeChanged;
        m_optionGameSpeed.OnValueChangedEvent       += OnOptionGameSpeedChanged;

    }

    private void OnDestroy()
    {
        m_pauseAction.action.performed -= OnGamePaused;
        m_saveButton.onClick.RemoveListener(OnSaveButtonClicked);
        m_defaultButton.onClick.RemoveListener(OnDefaultButtonClicked);
        m_quitButton.onClick.RemoveListener(OnQuitButtonClicked);

        m_optionVolumeGlobal.OnValueChangedEvent    -= OnOptionVolumeGlobalChanged;
        m_optionVolumeSFX.OnValueChangedEvent       -= OnOptionVolumeSFXChanged;
        m_optionVolumeMusic.OnValueChangedEvent     -= OnOptionVolumeMusicChanged;
        m_optionScreenMode.OnValueChangedEvent      -= OnOptionScreenModeChanged;
        m_optionGameSpeed.OnValueChangedEvent       -= OnOptionGameSpeedChanged;
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
        var volume = value == 0 ? -80 : Mathf.Log10(value / 100f) * 20;
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

    private void OnOptionGameSpeedChanged(float gameSpeed)
    {
        m_gameOptions.GameSpeed.Value = gameSpeed;
    }

    private void OnGamePaused(InputAction.CallbackContext ctx)
    {
        if (m_isOpened)
        {
            return;
        }

        OpenOptionMenu();
    }

    public void OpenOptionMenu()
    {
        m_menuOptions.alpha = 1;
        m_menuOptions.interactable = true;
        m_menuOptions.blocksRaycasts = true;
        m_isOpened = true;
        m_firstSelectable.Select();
        OnMenuOpened?.Invoke();
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

    public void CloseOptionMenu()
    {
        m_menuOptions.alpha = 0;
        m_menuOptions.interactable = false;
        m_menuOptions.blocksRaycasts = false;
        m_isOpened = false;
        OnMenuClosed?.Invoke();
    }

    private void OnQuitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}
