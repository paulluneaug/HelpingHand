using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Waiting/Wait Dialogue")]
[NodeTint(0.2f, 0.1f, .3f)]
public class WaitDialogueNode : WaitNodeBase
{
    [Space]
    [SerializeField]
    [HideLabel]
    private DialogueNode m_dialogue;

    protected override void UpdateWaitUntilTest()
    {
        m_stopWait = m_dialogue.HasBeenRead.Value;
    }
}