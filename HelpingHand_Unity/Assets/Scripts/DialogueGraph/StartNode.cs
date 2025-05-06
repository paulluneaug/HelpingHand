using System.Collections;

using XNode;

public class StartNode : BaseNode
{
    [Output(ShowBackingValue.Never, ConnectionType.Override)]
    public DialogueFlow m_out;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
    }

    public override IEnumerator Execute()
    {
        yield return ContinueFlow();
    }
}