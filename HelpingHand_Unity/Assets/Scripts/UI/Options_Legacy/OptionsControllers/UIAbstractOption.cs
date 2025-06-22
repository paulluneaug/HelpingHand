using System;

public abstract class UIAbstractOption<T> : UIAbstractDefaultable
{
    public event Action<T> OnValueChangedEvent;

    protected void TriggerValueChanged(T value)
    {
        OnValueChangedEvent?.Invoke(value);
    }

}
