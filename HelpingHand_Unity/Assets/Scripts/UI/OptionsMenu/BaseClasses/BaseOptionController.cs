using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityUtility.CustomAttributes;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseOptionController<T> : Selectable, IOptionController, ISubmitHandler
{
    [Serializable]
    private struct SpriteAndColor
    {
        [SerializeField] private Sprite m_sprite;
        [SerializeField] private Color m_color;
        [SerializeField] private Image.Type m_imageType;
        [SerializeField] private float m_pixelsPerUnitMultiplier;

        public void Apply(Image target)
        {
            target.sprite = m_sprite;
            target.color = m_color;
            target.type = m_imageType;
            target.pixelsPerUnitMultiplier = m_pixelsPerUnitMultiplier;
        }
    }

    public enum OptionControllerState
    {
        Deselected,
        Selected,
        Validated,
    }


    public string Description => m_description;

    public T Value => m_selectedOption.Value;
    public event Action<IOptionController> OnSelected;
    public event Action<T> OnValueChanged;

    [Title("Selection")]
    [SerializeField] private Image m_background;
    [SerializeField] private SpriteAndColor m_deselectedSprite;
    [SerializeField] private SpriteAndColor m_selectedSprite;
    [SerializeField] private SpriteAndColor m_validatedSprite;

    [Title("Childs")]
    [SerializeField] private BaseSelectedOption<T> m_selectedOption;

    [Title("Player Pref")]
    [SerializeField] protected string m_playerPrefName;
    [SerializeField] private T m_defaultValue;

    [Title("Description")]
    [SerializeField, TextArea] private string m_description;

    [NonSerialized] protected OptionControllerState m_currentState;

    [NonSerialized] private RectTransform m_rectTransform;
    [NonSerialized] private AutoScroller m_parentAutoScroller;


    protected override void Awake()
    {
        base.Awake();
        m_rectTransform = (RectTransform)transform;
        m_parentAutoScroller = GetComponentInParent<AutoScroller>();

        m_selectedOption.OnValueChanged += OnOptionValueChanged;
        m_selectedOption.Init(this, ReadFromPlayerPrefs(m_defaultValue));
        ForceState(OptionControllerState.Deselected);
    }

    protected override void OnDestroy()
    {
        m_selectedOption.Dispose();
    }

    private void OnOptionValueChanged(T value)
    {
        WriteToPlayerPrefs(value);

        OnValueChanged?.Invoke(value);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        if (m_parentAutoScroller != null)
        {
            m_parentAutoScroller.FocusOnChild(m_rectTransform);
        }

        if (m_currentState == OptionControllerState.Deselected)
        {
            OnSelected?.Invoke(this);
        }

        m_currentState = OptionControllerState.Selected;
        UpdateBackgroundSprite();
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (m_currentState == OptionControllerState.Validated)
        {
            return;
        }
        m_currentState = OptionControllerState.Deselected;
        UpdateBackgroundSprite();
    }

    public void ForceState(OptionControllerState state)
    {
        m_currentState = state;
        UpdateBackgroundSprite();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Validate();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Validate();
    }

    protected virtual void Validate()
    {
        m_currentState = OptionControllerState.Validated;
        UpdateBackgroundSprite();

        m_selectedOption.Select();
    }

    protected void UpdateBackgroundSprite()
    {
        SpriteAndColor fittingSprite = m_currentState switch
        {
            OptionControllerState.Deselected => m_deselectedSprite,
            OptionControllerState.Selected => m_selectedSprite,
            OptionControllerState.Validated => m_validatedSprite,
            _ => throw new ArgumentOutOfRangeException(),
        };

        fittingSprite.Apply(m_background);
    }

    public void SetDefault()
    {
        m_selectedOption.SetValue(m_defaultValue);
    }

    public abstract T ReadFromPlayerPrefs(T defaultValue);
    public abstract void WriteToPlayerPrefs(T value);
}
