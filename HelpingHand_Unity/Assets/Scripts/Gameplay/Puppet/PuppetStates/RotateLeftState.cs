public class RotateLeftState : RotateState
{
    public override void InitState(Puppet puppet)
    {
        base.InitState(puppet);
        m_direction = -1.0f;
    }
}
