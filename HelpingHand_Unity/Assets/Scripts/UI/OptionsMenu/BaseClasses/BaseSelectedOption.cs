using System;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public abstract class BaseSelectedOption<T> : Selectable, ISubmitHandler
{
    public T Value => m_value;

    public event Action<T> OnValueChanged;

    [SerializeField] private TMP_Text m_valueText;

    [NonSerialized] protected T m_value;
    [NonSerialized] protected BaseOptionController<T> m_controller;

    [NonSerialized] private bool m_submited;
    [NonSerialized] private bool m_hasSelection;

    public virtual void Init(BaseOptionController<T> controller, T startValue)
    {
        m_controller = controller;
        SetValue(startValue);
    }

    public virtual void Dispose()
    {

    }

    public virtual void OnSubmit(BaseEventData eventData)
    {
        m_controller.Select();
        m_submited = true;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        m_submited = false;
        m_hasSelection = true;
    }

    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        if (!m_submited)
        {
            m_controller.ForceState(BaseOptionController<T>.OptionControllerState.Deselected);
        }

        m_hasSelection = false;
    }

    public override void OnMove(AxisEventData eventData)
    {
        switch (eventData.moveDir)
        {
            case MoveDirection.Left:
                OnMoveLeft();
                break;
            case MoveDirection.Up:
                OnMoveUp();
                break;
            case MoveDirection.Right:
                OnMoveRight();
                break;
            case MoveDirection.Down:
                OnMoveDown();
                break;
            case MoveDirection.None:
                break;
            default:
                break;
        }
    }

    public virtual void SetValue(T value)
    {
        OnValueSet(value);
        OnValueChanged?.Invoke(value);
    }

    public virtual void SetValueWithoutNotify(T value)
    {
        OnValueSet(value);
    }

    protected virtual void OnValueSet(T value)
    {
        m_value = value;
        if (m_valueText != null)
        {
            m_valueText.text = ValueToDisplayString(value); 
        }

    }

    protected virtual void OnMoveLeft()
    {

    }

    protected virtual void OnMoveUp()
    {

    }

    protected virtual void OnMoveRight()
    {

    }

    protected virtual void OnMoveDown()
    {

    }

    protected virtual string ValueToDisplayString(T value)
    {
        return value.ToString();
    }

    protected void SelectIfNeeded()
    {
        if (!m_hasSelection)
        {
            Select();
            m_controller.ForceState(BaseOptionController<T>.OptionControllerState.Validated);
        }
    }
}
