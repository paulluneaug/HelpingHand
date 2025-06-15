using System;

using UnityEngine;

using UnityUtility.Extensions;
using UnityUtility.MathU;

public class MenuCogController : MonoBehaviour
{
    [SerializeField] private RectTransform m_cogTransform;

    [NonSerialized] private float m_currentRotation;
    [NonSerialized] private float m_startRotation;
    [NonSerialized] private float m_targetRotation;

    private void Start()
    {
        m_currentRotation = m_cogTransform.eulerAngles.z;
    }

    public void SetTarget(float target)
    {
        m_targetRotation = target;
        m_startRotation = m_currentRotation;
    }

    public void UpdateTransition(float progress)
    {
        m_currentRotation = MathUf.Lerp(m_startRotation, m_targetRotation, progress);
        m_cogTransform.eulerAngles = m_cogTransform.eulerAngles.WhereZ(m_currentRotation);
    }

}
