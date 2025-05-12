using System;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using UnityUtility.Singletons;

public class UIOptionsMenuController : MonoBehaviourSingleton<UIOptionsMenuController>
{
    [SerializeField] private AudioMixer m_mixer;
    [SerializeField] private InputActionReference m_pauseAction;

    [Header("UI components")]
    [SerializeField] private CanvasGroup m_menuOptions;
    [SerializeField] private Selectable m_firstSelectable;
    [SerializeField] private Button m_saveButton;
    [SerializeField] private Button m_defaultButton;
    [SerializeField] private Button m_quitButton;

    [Header("Options")]
    [SerializeField] private UIAbstractOption<string> m_optionInvincibility;
    [SerializeField] private UIAbstractOption<float> m_optionVolumeGlobal;
    [SerializeField] private UIAbstractOption<float> m_optionVolumeSFX;
    [SerializeField] private UIAbstractOption<float> m_optionVolumeMusic;
    [SerializeField] private UIAbstractOption<float> m_optionGameSpeed;
    [SerializeField] private UIAbstractOption<string> m_optionScreenMode;
    [SerializeField] private UIAbstractOption<float> m_optionSensitivity;

    public event Action OnMenuOpened;
    public event Action OnMenuClosed;

    private UIAbstractDefaultable[] m_options;
    private bool m_isOpened;
    public bool IsOpened => m_isOpened;

    public override void Initialize()
    {
        base.Initialize();
        m_options = new UIAbstractDefaultable[] { m_optionInvincibility, m_optionVolumeGlobal, m_optionVolumeSFX, m_optionVolumeMusic, m_optionGameSpeed, m_optionScreenMode, m_optionSensitivity };
    }

    protected override void Start()
    {
        m_pauseAction.action.performed += OnGamePaused;
        m_saveButton.onClick.AddListener(OnSaveButtonClicked);
        m_defaultButton.onClick.AddListener(OnDefaultButtonClicked);
        m_quitButton.onClick.AddListener(OnQuitButtonClicked);

        m_optionInvincibility.OnValueChangedEvent += OptionInvincibilityChanged;
        m_optionVolumeGlobal.OnValueChangedEvent += OptionVolumeGlobalChanged;
        m_optionVolumeSFX.OnValueChangedEvent += OptionVolumeSFXChanged;
        m_optionVolumeMusic.OnValueChangedEvent += OptionVolumeMusicChanged;
        m_optionGameSpeed.OnValueChangedEvent += OptionGameSpeedChanged;
        m_optionScreenMode.OnValueChangedEvent += OptionScreenModeChanged;
        m_optionSensitivity.OnValueChangedEvent += OptionSensitivityChanged;
    }


    private void OptionInvincibilityChanged(string value)
    {
        GameOptionsManager.Instance.IsInvincible = value switch
        {
            "Enabled" => true,
            _ => false
        };
    }

    private void OptionVolumeGlobalChanged(float value)
    {
        SetVolume("GlobalVolume", value);
    }

    private void OptionVolumeSFXChanged(float value)
    {
        SetVolume("SFXVolume", value);
    }

    private void OptionVolumeMusicChanged(float value)
    {
        SetVolume("MusicVolume", value);
    }

    private void SetVolume(string group, float value)
    {
        var volume = value == 0 ? -80 : Mathf.Log10(value / 100f) * 20;
        _ = m_mixer.SetFloat(group, volume);
    }

    private void OptionGameSpeedChanged(float value)
    {
        GameOptionsManager.Instance.GameSpeed.Value = value;
    }

    private void OptionSensitivityChanged(float value)
    {
        GameOptionsManager.Instance.Sensitivity.Value = (int)value;
    }

    private void OptionScreenModeChanged(string value)
    {
        GameOptionsManager.Instance.IsWindowed.Value = value switch
        {
            "Windowed" => true,
            _ => false
        };
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
