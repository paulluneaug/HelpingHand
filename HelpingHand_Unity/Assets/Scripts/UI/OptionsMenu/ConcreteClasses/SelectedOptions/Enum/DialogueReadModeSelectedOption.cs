public class DialogueReadModeSelectedOption : EnumSelectedOption<DialogueReadMode>
{
    protected override string ValueToDisplayString(DialogueReadMode value)
    {
        return value switch
        {
            DialogueReadMode.Manual => "Manuel",
            DialogueReadMode.Auto => "Automatique",
            _ => throw new System.ArgumentOutOfRangeException(),
        };
    }
}
