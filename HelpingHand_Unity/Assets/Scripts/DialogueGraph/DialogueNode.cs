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

    #region Wwise States Dialogue

    [SerializeField] private RepetitionState repetition;
    [SerializeField] private EtatState etat;
    [SerializeField] private ObjetState objet;
    [SerializeField] private NarraState narra;
    public enum RepetitionState { IGNORE, R1, R2, R3, R4 }
    public enum EtatState { IGNORE, On, Off }
    public enum ObjetState { IGNORE, Spot, Rideaux, Armure, Carton, Rien }
    public enum NarraState { IGNORE, Narra1, Narra2, Narra3, Narra4, Narra5, Narra6, Narra7, Narra8, Narra9, Narra10 }
    //Rajouter des champs si besoin, j'adapterai dans Wwise!
    #endregion
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
        AudioManager.Instance.PlayDialogueWithStates(repetition.ToString(), etat.ToString(), objet.ToString(), narra.ToString());
        Debug.Log($"[Dialogue Triggered] {repetition}, {etat}, {objet}, {narra}");
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