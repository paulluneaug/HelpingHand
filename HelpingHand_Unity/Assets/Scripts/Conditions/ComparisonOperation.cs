using Sirenix.OdinInspector;

public enum ComparisonOperation
{
    [LabelText("==")] Equal,
    [LabelText("!=")] NotEqual,
    [LabelText("<")] StrictlyLowerThan,
    [LabelText("<=")] LowerOrEqualThan,
    [LabelText(">")] StrictlyGreaterThan,
    [LabelText(">=")] GreaterOrEqualThan,
}