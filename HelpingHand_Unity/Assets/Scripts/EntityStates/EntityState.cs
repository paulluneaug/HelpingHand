using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/States/State")]
public class EntityState : BaseVariable<bool>
{
    public bool IsSet => Value;

    public void Set()
    {
        if (IsSet)
        {
            return;
        }

        Value = true;
    }

    public void Unset()
    {
        if (!IsSet)
        {
            return;
        }

        Value = false;
    }
}