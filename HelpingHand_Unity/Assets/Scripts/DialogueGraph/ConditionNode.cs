using System.Collections;

using UnityEngine;

using XNode;

[NodeWidth(300)]
public class ConditionNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private PreconditionBase m_condition;

    public override void Initialize()
    {
        m_condition.Initialize();
    }

    public override IEnumerator Execute()
    {
        if (!m_condition.Test())
        {
            yield break;
        }
        yield return ContinueFlow();
    }

    public override object GetValue(NodePort port)
    {
        return base.GetValue(port);
    }
}