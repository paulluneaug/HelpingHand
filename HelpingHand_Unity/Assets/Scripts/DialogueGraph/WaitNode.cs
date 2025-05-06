using System.Collections;

using UnityEngine;

using XNode;

public class WaitNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    public float m_waitTime;
    public bool m_unscaled = false;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
    }

    public override IEnumerator Execute()
    {
        yield return m_unscaled ? new WaitForSecondsRealtime(m_waitTime) : new WaitForSeconds(m_waitTime);
        yield return ContinueFlow();
    }
}