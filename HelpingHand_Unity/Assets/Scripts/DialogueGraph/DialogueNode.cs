using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(300)]
public class DialogueNode : BaseNode
{
    [Input] [SerializeField]
    private DialogueFlow m_in;

    [Output] [SerializeField]
    private DialogueFlow m_out;

    [HideLabel, Multiline(3)] [PropertySpace(SpaceAfter = 10, SpaceBefore = 0)] [SerializeField]
    private string m_content;

    [FoldoutGroup("Properties")] [LabelWidth(125)] [SerializeField]
    private int m_priority;

    [FoldoutGroup("Properties")] [LabelWidth(125)] [SerializeField]
    private bool m_canBeInterrupted;

    [FoldoutGroup("Properties")] [LabelWidth(125)] [SerializeField]
    private bool m_canBeReadMultipleTimes;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private bool m_hasBeenRead;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private int m_readCount;

    public string Content => m_content;
    public int Priority => m_priority;
    public bool CanBeInterrupted => m_canBeInterrupted;
    public bool CanBeReadMultipleTimes => m_canBeReadMultipleTimes;
    public bool HasBeenRead => m_hasBeenRead;
    public int ReadCount => m_readCount;


    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    public override void Initialize()
    {
        m_hasBeenRead = false;
        m_readCount = 0;
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        Debug.Log($"Dialogue {name} Play");
        DialogueManager.Instance.PlayDialog(this);
        DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
        Debug.Log($"Dialogue {name} Wait for end");
        bool isCancelled = await WaitForDialogueEnd(handler).SuppressCancellationThrow();
        if (isCancelled)
        {
            Debug.Log($"DialogueNode {name}: dialogue cancelled");
            return;
        }

        Debug.Log($"Dialogue {name} end");
        await ContinueFlow(handler);
    }

    private async UniTask WaitForDialogueEnd(GraphRunnerHandler handler)
    {
        await UniTask.WaitUntil(() => m_hasBeenRead, PlayerLoopTiming.Update, handler.StopToken);
    }

    private void OnDialogueEnded()
    {
        DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
        m_hasBeenRead = true;
        m_readCount++;
    }

    private void OnDialogueInterrupted()
    {
        
    }
}