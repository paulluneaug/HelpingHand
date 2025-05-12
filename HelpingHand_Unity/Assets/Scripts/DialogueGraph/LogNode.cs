using Cysharp.Threading.Tasks;

using UnityEngine;

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