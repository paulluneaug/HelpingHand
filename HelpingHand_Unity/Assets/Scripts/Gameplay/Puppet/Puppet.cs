using System;

using Sirenix.OdinInspector;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Splines;

using UnityUtility.MathU;

public class Puppet : MonoBehaviour
{
    private static readonly int s_isWalkingAnimatorParameter = Animator.StringToHash("IsWalking");

    public bool HasReachedEndOfSpline => m_hasReachedEndOfSpline;

    [Title("Components References")]
    [SerializeField] private Animator m_puppetAnimator;

    [Title("Variable References")]
    [SerializeField] private BaseVariable<SplineContainer> m_splineToFollow;
    [SerializeField] private BaseVariable<float> m_speedAlongSpline;


    // Cache
    [NonSerialized] private bool m_isWalking = false;
    [NonSerialized] private bool m_hasReachedEndOfSpline = false;
    [NonSerialized] private float m_progressAlongSpline = 0.0f;
    [NonSerialized] private float m_splineLength = 0.0f;
    [NonSerialized] private Spline m_currentSpline = null;


    private void Start()
    {
        StopWalk();
        m_splineToFollow.AddListener(OnSplineToFollowChanged);
    }

    private void OnDestroy()
    {
        m_splineToFollow.RemoveListener(OnSplineToFollowChanged);
    }

    private void Update()
    {
        if (!m_isWalking)
        {
            return;
        }

        float addedProgress = m_speedAlongSpline.Value / m_splineLength * Time.deltaTime;
        m_progressAlongSpline = MathUf.Clamp(m_progressAlongSpline + addedProgress, 0.0f, 1.0f);
        UpdatePositionAndRotation(m_progressAlongSpline);
        if (m_progressAlongSpline >= 1.0f)
        {
            m_hasReachedEndOfSpline = true;
            StopWalk();
        }

    }

    [DisableIf("m_isWalking")]
    [HorizontalGroup("Split", 0.5f)]
    [Button("Start walk", ButtonSizes.Small)]
    public void StartWalk()
    {
        if (m_splineToFollow.Value == null)
        {
            throw new NullReferenceException("");
        }
        m_isWalking = true;
        m_hasReachedEndOfSpline = false;
        m_puppetAnimator.SetBool(s_isWalkingAnimatorParameter, true);

        m_currentSpline = m_splineToFollow.Value.Spline;
        m_splineLength = m_splineToFollow.Value.CalculateLength();

        m_progressAlongSpline = 0.0f;
        UpdatePositionAndRotation(0.0f);

    }

    [EnableIf("m_isWalking")]
    [HorizontalGroup("Split", 0.5f)]
    [Button("Stop walk", ButtonSizes.Small)]
    public void StopWalk()
    {
        m_isWalking = false;
        m_puppetAnimator.SetBool(s_isWalkingAnimatorParameter, false);
    }

    private void UpdatePositionAndRotation(float time)
    {
        float3 startPosition = m_currentSpline.EvaluatePosition(time);
        float3 startForward = m_currentSpline.EvaluateTangent(time);
        Quaternion startRotation = Quaternion.LookRotation(startForward, Vector3.up);

        transform.SetPositionAndRotation(startPosition, startRotation);
    }

    private void OnSplineToFollowChanged(SplineContainer container)
    {
        if (m_isWalking) 
        {
            throw new InvalidOperationException("Don't change the spline the puppet is following while it is walking : Call Puppet.StopWalk() before doing so");
        }
    }
}
