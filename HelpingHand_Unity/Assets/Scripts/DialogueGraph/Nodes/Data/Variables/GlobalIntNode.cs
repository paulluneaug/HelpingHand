using Sirenix.OdinInspector;

using UnityEngine;

[CreateNodeMenu("Data/Variables/Globals/Int")] 
public class GlobalIntNode : GlobalVariableNode<IntVariable>
{
    [Output(ShowBackingValue.Always)] [SerializeField] [InlineEditor]
    protected IntVariable m_value;

    protected override IntVariable Value => m_value;
}