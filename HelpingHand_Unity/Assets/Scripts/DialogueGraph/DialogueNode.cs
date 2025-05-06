
using System.Collections;

using UnityEngine;

using XNode;

[NodeWidth(200)]
public class DialogueNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;
    
    [Output]
    public DialogueFlow m_out;

    [TextArea(3, 3)]
    public string m_content;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
    }

    public override IEnumerator Execute()
    {
        Debug.Log(m_content);
        yield return ContinueFlow();
    }
}