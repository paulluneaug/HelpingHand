using System;

[Serializable]
public abstract class PuppetAction
{
    public bool IsFinished => m_finished;

    protected bool m_finished = false;

    protected Puppet m_puppet;

    public virtual void StartAction(Puppet puppet)
    {
        m_finished = false;
        m_puppet = puppet;
    }

    public virtual void UpdateAction(float deltaTime)
    {
    }

    public virtual void EndAction()
    {

    }

    protected void FinishAction()
    {
        m_finished = true;
    }
}
