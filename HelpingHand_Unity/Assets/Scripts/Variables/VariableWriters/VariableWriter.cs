using UnityEngine;

public class VariableWriter<T> : MonoBehaviour
{
    [SerializeField] private BaseVariable<T> m_variable;
    [SerializeField] private T m_value;

    private void Start()
    {
        m_variable.Value = m_value;
    }
}
