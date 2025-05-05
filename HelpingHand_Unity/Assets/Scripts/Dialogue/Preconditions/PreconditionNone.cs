[System.Serializable]
public class PreconditionNone : PreconditionBase
{
    public override bool Test()
    {
        return true;
    }

    public override void Initialize()
    {
        base.Initialize();
    }
}
