[System.Serializable]
public class ConditionNone : ConditionBase
{
    public override bool Test()
    {
        return true;
    }

    public override void Initialize()
    {
        base.Initialize();
    }

    public override int Depth()
    {
        return 0;
    }
}
