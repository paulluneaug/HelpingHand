
using System;

using Cysharp.Threading.Tasks;

using Sirenix.OdinInspector;

using UnityEngine;

using XNode;

[NodeWidth(250)]
public class TimerControlNode : BaseNode
{
    private enum Operation
    {
        Start,
        Stop,
        Pause,
        Resume,
    }
    
    [Input(ShowBackingValue.Never)]
    [SerializeField]
    private DialogueFlow m_in;
    
    [Output]
    [SerializeField]
    private DialogueFlow m_out;

    [SerializeField] 
    [HideLabel]
    private StandaloneTimer m_timer;

    [SerializeField] 
    [EnumToggleButtons] 
    private Operation m_operation;

    protected override async UniTask ExecuteNode(GraphRunnerHandler handler, NodePort inPort)
    {
        switch (m_operation)
        {
            case Operation.Start:
                m_timer.Init();
                m_timer.Start();
                break;
            case Operation.Stop:
                m_timer.Stop();
                break;
            case Operation.Pause:
                m_timer.Pause();
                break;
            case Operation.Resume:
                m_timer.Resume();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
