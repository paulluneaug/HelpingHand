using System;

using UnityEngine;
using UnityEngine.VFX;

public class BossController : MonoBehaviour
{
    [SerializeField] private EntityState m_fireGameEvent;
    [SerializeField] private Animator m_bossAnimator;
    [SerializeField] private VisualEffect m_fireVFX;
    [SerializeField] private string m_fireParameterName = "Fire";

    [NonSerialized] private int m_fireParameterHash;

    private void Awake()
    {
        m_fireGameEvent.AddListener(OnFireValueChanged);
        m_fireParameterHash = Animator.StringToHash(m_fireParameterName);
        OnFireValueChanged(m_fireGameEvent.Value);
    }

    private void OnDestroy()
    {
        m_fireGameEvent.RemoveListener(OnFireValueChanged);
    }

    private void OnFireValueChanged(bool fire)
    {
        m_bossAnimator.SetBool(m_fireParameterHash, fire);

        if (fire)
        {
            m_fireVFX.Play();
        }
        else
        {
            m_fireVFX.Stop();
        }
    }
}
