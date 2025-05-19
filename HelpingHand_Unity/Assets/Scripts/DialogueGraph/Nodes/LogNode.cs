using Cysharp.Threading.Tasks;

using UnityEngine;

using XNode;

public class LogNode : BaseNode
{
    [Input(ShowBackingValue.Never)]
    public DialogueFlow m_in;

    [Input]
    public string m_stringIn;
    
    [Output]
    public DialogueFlow m_out;

    [SerializeField]
    private string m_content;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        string inputString = GetInputPort(nameof(m_stringIn)).GetInputValue<string>();
        DebugLog($"{m_content}: {inputString}");
        await UniTask.CompletedTask;
    }
}