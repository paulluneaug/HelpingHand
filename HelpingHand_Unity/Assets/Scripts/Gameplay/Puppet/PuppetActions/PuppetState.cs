public abstract class PuppetState
{
    protected Puppet m_puppet;

    public virtual void BeginState(Puppet puppet)
    {
        m_puppet = puppet;
    }

    public virtual void UpdateState(float deltaTime)
    {
    }

    public virtual void EndState()
    {

    }
}
