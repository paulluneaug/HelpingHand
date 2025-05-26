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
        GameManager.Instance.RegisterPuppet(this);
        m_splineToFollow.AddListener(OnSplineToFollowChanged);

        StopWalk();
    }

    private void OnDestroy()
    {
        GameManager.Instance.UnregisterPuppet();
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
    [Button("Begin walk", ButtonSizes.Small)]
    public void BeginWalk()
    {
        if (m_splineToFollow.Value == null)
        {
            throw new NullReferenceException("Puppet has non spline to follow");
        }
        m_hasReachedEndOfSpline = false;
        SetWalkState(true);

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
        SetWalkState(false);
        m_hasReachedEndOfSpline = false;
        m_progressAlongSpline = 0.0f;
    }

    public void PauseWalk()
    {
        if (m_isWalking)
        {
            SetWalkState(false);
        }
    }

    public void ResumeWalk()
    {
        if (!m_isWalking)
        {
            SetWalkState(true);
        }
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

    private void SetWalkState(bool walk)
    {
        m_isWalking = walk;
        m_puppetAnimator.SetBool(s_isWalkingAnimatorParameter, walk);
    }

}
