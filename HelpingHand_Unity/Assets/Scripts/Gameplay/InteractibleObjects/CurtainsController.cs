using UnityEngine;

public class CurtainsController : MonoBehaviour
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

    protected void Start()
    {
        // Todo set virtual slider value
        m_sliderFloatVariable.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(m_startValue);
    }

    private void OnSliderValueChanged(float value)
    {
        m_transform.position = Vector3.Lerp(m_downPosition, m_upPosition, value);
    }
}