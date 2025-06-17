public class SubtitleSizeSelectedOption : EnumSelectedOption<SubtitleSize>
{
    protected override string ValueToDisplayString(SubtitleSize value)
    {
        return value switch
        {
            SubtitleSize.Small => "Petite",
            SubtitleSize.Medium => "Moyenne",
            SubtitleSize.Large => "Grande",
            _ => throw new System.ArgumentOutOfRangeException(),
        };
    }
}
