using UnityEngine;

using XNode;

public abstract class LocalVariableNode<T> : BaseNode
{
    [Output] [SerializeField]
    protected LocalVariable<T> m_variableOut;

    [SerializeField] 
    protected T m_startValue;

    public override void Initialize()
    {
        base.Initialize();
        m_variableOut = new LocalVariable<T>(m_startValue);
    }

    public override object GetValue(NodePort port)
    {
        return m_variableOut;
    }
}
