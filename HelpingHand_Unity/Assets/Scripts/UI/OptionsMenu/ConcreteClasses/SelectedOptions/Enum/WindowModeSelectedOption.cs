public class WindowModeSelectedOption : EnumSelectedOption<WindowMode>
{
    protected override string ValueToDisplayString(WindowMode value)
    {
        return value switch
        {
            WindowMode.Windowed => "Fenêtré",
            WindowMode.FullScreen => "Plein écran",
            _ => throw new System.ArgumentOutOfRangeException(),
        };
    }
}
