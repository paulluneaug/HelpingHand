using System;
using System.Collections;

using UnityEngine;

[Serializable]
public abstract class InputTrigger
{
    [SerializeField]
    private bool m_canReactivate = true;

    [SerializeField]
    private float m_reactivateTime;

    public event Action OnTriggered;
    public bool IsArmed => m_isArmed;
    public virtual bool IsRaised => m_isRaised;

    protected bool m_isRaised;

    private bool m_isArmed;
    private WaitForSecondsRealtime m_wait;
    private Coroutine m_triggerCoroutine;

    public virtual void Initialize()
    {
        m_wait = new WaitForSecondsRealtime(m_reactivateTime);
        m_isArmed = false;
        m_triggerCoroutine = null;
    }

    public void RaiseTriggeredEvent()
    {
        OnTriggered?.Invoke();
    }

    protected abstract void ArmTrigger();

    protected abstract void DisarmTrigger();

    protected IEnumerator RearmTriggerCoroutine()
    {
        if (!m_canReactivate)
        {
            yield break;
        }

        yield return m_wait;
        m_isRaised = false;
        ArmTrigger();
    }
}
