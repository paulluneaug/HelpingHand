public abstract class PuppetState
{
    protected Puppet m_puppet;

    public virtual void InitState(Puppet puppet)
    {
        m_puppet = puppet;
    }

    public virtual void BeginState()
    {
    }

    public virtual void UpdateState(float progress, float deltaTime)
    {
    }

    public virtual void EndState()
    {

    }
}
