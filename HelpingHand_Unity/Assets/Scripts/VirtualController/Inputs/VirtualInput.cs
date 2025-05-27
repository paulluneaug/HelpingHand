using System;

using UnityEngine;

public abstract class VirtualInput<T> : MonoBehaviour, IVirtualInput
{
    public T Value => m_value;
    public event Action<T> OnValueChanged;
    public event Action OnActivate;
    public event Action OnDeactivate;

    protected abstract BaseVariable<T> InputEvent { get; }

    public bool IsActive => InputEvent.IsActive;

    [NonSerialized] private T m_value;

    protected virtual void OnEnable()
    {
        InputEvent.OnActivate -= OnInputActivate;
        InputEvent.OnActivate += OnInputActivate;
        InputEvent.OnDeactivate -= OnInputDeactivate;
        InputEvent.OnDeactivate += OnInputDeactivate;
    }

    protected virtual void OnDisable()
    {
        InputEvent.OnActivate -= OnInputActivate;
    }

    private void OnInputActivate()
    {
        SetValue(InputEvent.Value);
        OnActivate?.Invoke();
    }

    private void OnInputDeactivate()
    {
        OnDeactivate?.Invoke();
    }

    protected void SetValue(T newValue)
    {
        T oldValue = m_value;
        m_value = newValue;

        if (InputEvent != null)
        {
            InputEvent.Value = newValue;
        }

        if (!oldValue.Equals(newValue))
        {
            if (InputEvent.IsActive)
            {
                OnValueChanged?.Invoke(newValue);
            }
        }
    }

    protected virtual void SetValueWithoutNotify(T newValue)
    {
        m_value = newValue;

        if (InputEvent != null)
        {
            InputEvent.SetValueWithoutNotify(newValue);
        }
    }
}
