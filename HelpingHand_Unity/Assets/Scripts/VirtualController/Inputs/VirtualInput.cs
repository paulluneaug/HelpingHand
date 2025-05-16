using System;
using System.Linq;

using UnityEngine;

public abstract class VirtualInput<T> : MonoBehaviour, IVirtualInput
{
    public T Value => m_value;
    public event Action<T> OnValueChanged;


    protected abstract BaseVariable<T> LinkedVariable { get; }

    [NonSerialized] private T m_value;

    protected void SetValue(T newValue)
    {
        m_value = newValue;

        if (LinkedVariable != null)
        {
            LinkedVariable.Value = newValue;
        }

        OnValueChanged?.Invoke(newValue);
    }

    protected virtual void SetValueWithoutNotify(T newValue)
    {
        m_value = newValue;

        if (LinkedVariable != null)
        {
            LinkedVariable.SetValueWithoutNotify(newValue);
        }
    }

    private string GetTypeFilter
    {
        get
        {
            var thisType = typeof(BaseVariable<T>);
            var q = thisType.Assembly.GetTypes()
                .Where(x => !x.IsAbstract) // Excludes BaseClass
                .Where(x => !x.IsGenericTypeDefinition) // Excludes C1<>
                .Where(x => thisType.IsAssignableFrom(x)); // Excludes classes not inheriting from BaseClass

            Debug.Log(q.Select(t => t.Name).Aggregate((t, ag) => $"{ag} t:{t}"));
            return q.Select(t => t.Name).Aggregate((t, ag) => $"{ag} t:{t}");
        }
    }
}
