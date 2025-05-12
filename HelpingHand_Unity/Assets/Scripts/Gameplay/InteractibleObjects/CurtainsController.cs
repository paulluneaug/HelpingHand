using UnityEngine;

public class CurtainsController : SingleSliderInteractiveObject
{
    [SerializeField]
    private FloatVariable m_sliderFloatVariable;

    [Range(0, 1)] [SerializeField]
    private float m_startValue;

    [SerializeField]
    private Vector3 m_downPosition;

    [SerializeField]
    private Vector3 m_upPosition;

    private Transform m_transform;

    private void Awake()
    {
        m_transform = transform;
    }

    protected override void Start()
    {
        base.Start();
        m_masterSlider.SetValueWithoutNotify(m_startValue);
        // m_masterSlider.OnSliderValueChanged += OnSliderValueChanged;
        // OnSliderValueChanged(m_startValue);
        m_sliderFloatVariable.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(m_startValue);
    }

    private new void OnSliderValueChanged(float value)
    {
        m_transform.position = Vector3.Lerp(m_downPosition, m_upPosition, value);
    }
}