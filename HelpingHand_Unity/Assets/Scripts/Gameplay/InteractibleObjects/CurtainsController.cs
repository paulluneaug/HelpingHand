using System;

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

    [Header("States")]

    [SerializeField]
    private float m_activationDistance;

    [SerializeField]
    private EntityState m_fullyDownState;

    [SerializeField]
    private EntityState m_fullUpState;

    [SerializeField]
    private EntityState m_visibleState;

    private Transform m_transform;

    private void Awake()
    {
        m_transform = transform;
    }

    protected override void Start()
    {
        base.Start();
        m_masterSlider.SetValueWithoutNotify(m_startValue);
        m_masterSlider.OnSliderValueChanged += OnSliderValueChanged;
        OnSliderValueChanged(m_startValue);
        // m_sliderFloatVariable.AddListener(OnSliderValueChanged);
        // OnSliderValueChanged(m_startValue);
    }

    private new void OnSliderValueChanged(float value)
    {
        Vector3 oldPosition = m_transform.position;
        m_transform.position = Vector3.Lerp(m_downPosition, m_upPosition, value);

        if (oldPosition != m_transform.position)
        {
            if ((m_downPosition - transform.position).magnitude <= m_activationDistance)
            {
                m_fullyDownState.Set();
            }
            else
            {
                m_fullyDownState.Unset();
            }

            if ((m_upPosition - transform.position).magnitude <= m_activationDistance)
            {
                m_fullUpState.Set();
            }
            else
            {
                m_fullUpState.Unset();
            }
        }
    }
}