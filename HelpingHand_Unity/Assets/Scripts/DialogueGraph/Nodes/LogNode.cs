using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(300)]
public class LogNode : BaseNode
{
    [Input(ShowBackingValue.Never)]
    public DialogueFlow m_in;

    [Input(ShowBackingValue.Never)]
    public string m_stringIn;
    
    [Output]
    public DialogueFlow m_out;

    [SerializeField] [TextArea(3, 3)] [HideLabel]
    private string m_content;

    [SerializeField] 
    private LogType m_logType = LogType.Log;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        string inputString = GetInputPort(nameof(m_stringIn)).GetInputValue<string>();
        DebugLog($"{m_content}{(string.IsNullOrEmpty(inputString) ? $": {inputString}" : string.Empty)}", m_logType);
        await UniTask.CompletedTask;
    }
}