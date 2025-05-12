using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class LogNode : BaseNode
{
    [Input]
    public DialogueFlow m_in;

    [Input]
    public string m_stringIn;

    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private string m_content;

    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "m_stringIn")
        {
            return m_stringIn;
        }
        else
        {
            return m_out;
        }
    }

    public override void Initialize()
    {
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        string inputString = GetInputPort(nameof(m_stringIn)).GetInputValue<string>();
        Debug.Log($"{m_content}: {inputString}");
        await ContinueFlow(handler);
    }
}