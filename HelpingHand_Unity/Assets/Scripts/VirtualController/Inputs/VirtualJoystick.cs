using UnityEngine;

public class VirtualJoystick : VirtualInput<Vector2>
{
    [SerializeField] private VirtualButton m_northButton;
    [SerializeField] private VirtualButton m_northEastButton;
    [SerializeField] private VirtualButton m_eastButton;
    [SerializeField] private VirtualButton m_southEastButton;
    [SerializeField] private VirtualButton m_southButton;
    [SerializeField] private VirtualButton m_southWestButton;
    [SerializeField] private VirtualButton m_westButton;
    [SerializeField] private VirtualButton m_northWestButton;

    private void Update()
    {
        Vector2 newValue =
            (m_northButton.Value ? Vector2.up : Vector2.zero) +
            (m_northEastButton.Value ? new Vector2(1.0f, 1.0f) : Vector2.zero) +
            (m_eastButton.Value ? Vector2.right : Vector2.zero) +
            (m_southEastButton.Value ? new Vector2(1.0f, -1.0f) : Vector2.zero) +
            (m_southButton.Value ? Vector2.down : Vector2.zero) +
            (m_southWestButton.Value ? new Vector2(-1.0f, -1.0f) : Vector2.zero) +
            (m_westButton.Value ? Vector2.left : Vector2.zero) +
            (m_northWestButton.Value ? new Vector2(-1.0f, 1.0f) : Vector2.zero);

        if (newValue != Value)
        {
            SetValue(newValue);
        }
    }

    protected override BaseVariable<Vector2> LinkedVariable { get; }
}
