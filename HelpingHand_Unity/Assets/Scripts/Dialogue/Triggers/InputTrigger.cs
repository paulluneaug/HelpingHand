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

    public event Action OnTriggerRaised;
    public bool IsActive => m_isActive;
    public virtual bool IsRaised => m_isRaised;

    protected bool m_isRaised;
    
    private bool m_isActive;
    private WaitForSecondsRealtime m_wait;
    private Coroutine m_triggerCoroutine;

    public virtual void Initialize()
    {
        m_wait = new WaitForSecondsRealtime(m_reactivateTime);
        m_isActive = false;
        m_triggerCoroutine = null;
    }

    public void RaiseTrigger()
    {
        OnTriggerRaised?.Invoke();
    }

    protected void SetActive(bool isActive)
    {
        if (m_isActive == isActive)
        {
            return;
        }
        m_isActive = isActive;
        if (m_isActive)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    protected abstract void Activate();
    
    protected abstract void Deactivate();

    protected IEnumerator ReactivateCoroutine()
    {
        if (!m_canReactivate)
        {
            yield break;
        }
        
        yield return m_wait;
        m_isRaised = false;
        SetActive(true);
    }
}
