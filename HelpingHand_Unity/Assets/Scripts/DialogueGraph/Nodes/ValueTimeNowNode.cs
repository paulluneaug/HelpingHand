using UnityEngine;

[CreateNodeMenu("Data/Values/Time Now")] 
public class ValueTimeNowNode : ValueNodeBase<float>
{
    [Output(ShowBackingValue.Never)] [SerializeField]
    protected float m_TimeNow;
    
    protected override float Value => Time.time;
}