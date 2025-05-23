using System;

using UnityEngine;

public class CurtainsController : MonoBehaviour
{
    [SerializeField]
    private FloatVariable m_inputEvent;

    [Range(0, 1)] [SerializeField]
    private float m_startValue;

    [SerializeField]
    private Vector3 m_downPosition;

    [SerializeField]
    private Vector3 m_upPosition;

    [SerializeField]
    private float m_smoothTime = .5f;
    
    private Transform m_transform;
    private Vector3 m_targetPosition;
    private Vector3 m_currentVelocity;
    
    private void Awake()
    {
        m_transform = transform;
    }

    private void OnEnable()
    {
        m_inputEvent.OnActivate -= OnInputEventActivate;
        m_inputEvent.OnActivate += OnInputEventActivate;
        m_inputEvent.RemoveListener(OnValueChanged);
        m_inputEvent.AddListener(OnValueChanged);
    }

    private void OnDisable()
    {
        m_inputEvent.OnActivate -= OnInputEventActivate;
        m_inputEvent.RemoveListener(OnValueChanged);
    }

    private void OnInputEventActivate()
    {
        OnValueChanged(m_inputEvent.Value);
    }

    protected void Start()
    {
        OnValueChanged(m_inputEvent.Value);
        m_transform.position = m_targetPosition;
    }

    private void OnValueChanged(float value)
    {
        m_targetPosition = Vector3.Lerp(m_downPosition, m_upPosition, value);
    }

    private void Update()
    {   
        if (m_targetPosition != m_transform.position)
        {
            m_transform.position = Vector3.SmoothDamp(m_transform.position, m_targetPosition, ref m_currentVelocity, m_smoothTime);
        }
    }
}