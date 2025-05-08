using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(300)]
public class DialogueNode : InterruptableNode
{
    [Input] [SerializeField]
    private DialogueFlow m_in;

    [Output] [SerializeField]
    private DialogueFlow m_out;

    [HideLabel, Multiline(3)] [PropertySpace(SpaceAfter = 10, SpaceBefore = 0)] [SerializeField]
    private string m_content;

    [SerializeField][LabelWidth(100)]
    private bool m_multipleReads;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private bool m_hasBeenRead;

    [FoldoutGroup("Debug")] [ShowInInspector, LabelWidth(125), ReadOnly]
    private int m_readCount;

    public string Content => m_content;
    public bool MultipleReads => m_multipleReads;
    public bool HasBeenRead => m_hasBeenRead;
    public int ReadCount => m_readCount;

    public override object GetValue(NodePort port)
    {
        return m_out;
    }

    protected override void Init()
    {
        base.Init();
        m_description = "Display the content. Loops back if interrupted";
    }

    public override void Initialize()
    {
        base.Initialize();
        m_hasBeenRead = false;
        m_readCount = 0;
    }

    protected override async UniTask ContinueFlow(GraphRunnerHandler handler)
    {
        if (m_hasBeenInterrupted)
        {
            m_hasBeenInterrupted = false;
            // wait for interruption graph to end
            bool isCancelled = await UniTask.WaitUntilCanceled(handler.ResumeToken).SuppressCancellationThrow();
            if (isCancelled)
            {
                Debug.Log($"{Debug_GetLogHeader()} Wait cancelled");
                return;
            }
            // re-execute the node
            await ExecuteNode(handler);
        }
        else
        {
            Debug.Log($"{Debug_GetLogHeader()} End");
            await ContinueFlow(handler, GetOutputPort(nameof(m_out)));
        }
    }

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler)
    {
        Debug.Log($"{Debug_GetLogHeader()} Play");
        DialogueManager.Instance.PlayDialog(this);
        DialogueManager.Instance.OnDialogueEnded += OnDialogueEnded;
        Debug.Log($"{Debug_GetLogHeader()} Wait for end");
        bool isCancelled = await WaitForDialogueEnd(handler).SuppressCancellationThrow();
        if (isCancelled)
        {
            // Normalement le dialogue pouvait être interrompu, pas besoin de retester
            // On arrive ici si le dialogue est interrompu au milieu d'une phrase par un autre dialogue
            // ou si le graph est mis en pause 
            Debug.Log($"{Debug_GetLogHeader()} Interrupted");
            m_hasBeenInterrupted = true;
            DialogueManager.Instance.InterruptDialogue();
        }

        await ContinueFlow(handler);
    }

    private async UniTask WaitForDialogueEnd(GraphRunnerHandler handler)
    {
        await UniTask.WaitUntil(() => m_hasBeenRead || m_hasBeenInterrupted, PlayerLoopTiming.Update, handler.StopToken);
    }

    private void OnDialogueEnded()
    {
        DialogueManager.Instance.OnDialogueEnded -= OnDialogueEnded;
        m_hasBeenRead = true;
        m_readCount++;
    }
}