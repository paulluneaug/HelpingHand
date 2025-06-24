using Cysharp.Threading.Tasks;

using UnityEngine;

public class ConditionListenerAudio : ConditionListener
{
    [Space]
    [SerializeField]
    private AK.Wwise.Event m_wwiseEvent;

    protected override void OnConditionUpdated()
    {
        bool test = m_condition.Test();
        if (test)
        {
            AudioManager.Instance.PostWwiseEventAsync(m_wwiseEvent, gameObject).Forget();
        }
    }
}
