using System;

using Events;

using Sirenix.OdinInspector;

using Unity.Mathematics;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

using UnityUtility.MathU;

using Separator = UnityUtility.CustomAttributes.SeparatorAttribute;

public class Puppet : MonoBehaviour, ILateAwaker
{
    public bool HasReachedEndOfSpline => m_hasReachedEndOfSpline;

    [Title("Component References")]
    [SerializeField] private Transform m_objectInHandParent;
    [SerializeField] private Transform m_objectOnHeadParent;

    [Title("Animation")]
    [SerializeField] private Animator m_puppetAnimator;
    [SerializeField] private PuppetAnimatorParameterContainer m_animatorParameterContainer;

    [Title("Game Variable References")]
    [SerializeField] private BaseVariable<SplineContainer> m_splineToFollow;
    [SerializeField] private BaseVariable<float> m_speedAlongSpline;

    [Separator]

    [SerializeField] private PuppetObjectInHandStateCollection m_objectInHand;
    [SerializeField] private PuppetObjectOnHeadStateCollection m_objectOnHead;

    [Separator]

    [SerializeField] private BaseVariable<bool> m_lookUp;
    [SerializeField] private GameEvent m_strike;
    [SerializeField] private BaseVariable<bool> m_victory;
    [SerializeField] private BaseVariable<bool> m_defeat;


    // Cache
    [NonSerialized] private bool m_isWalking = false;
    [NonSerialized] private bool m_hasReachedEndOfSpline = false;
    [NonSerialized] private float m_progressAlongSpline = 0.0f;
    [NonSerialized] private float m_splineLength = 0.0f;
    [NonSerialized] private Spline m_currentSpline = null;


    public void LateAwake()
    {
        GameManager.Instance.RegisterPuppet(this);

        m_splineToFollow.AddListener(OnSplineToFollowChanged);
        m_lookUp.AddListener(SetLookUpAnimation);
        m_strike.AddListener(StrikeAnimation);
        m_victory.AddListener(SetVictoryAnimation);
        m_defeat.AddListener(SetDefeatAnimation);

        m_animatorParameterContainer.Init();

        m_objectInHand.OnPuppetHeldObjectChanged(ObjectInHand.None);
        m_objectOnHead.OnPuppetWornObjectChanged(ObjectOnHead.None);

        StopWalk();
    }

    private void OnDestroy()
    {
        if (!GameManager.ApplicationIsQuitting)
        {
            GameManager.Instance.UnregisterPuppet();
        }
        m_splineToFollow.RemoveListener(OnSplineToFollowChanged);
        m_lookUp.RemoveListener(SetLookUpAnimation);
        m_strike.RemoveListener(StrikeAnimation);
        m_victory.RemoveListener(SetVictoryAnimation);
        m_defeat.RemoveListener(SetDefeatAnimation);
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
            throw new NullReferenceException("Puppet has no spline to follow");
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

    public bool IsHoldingObjectInHand()
    {
        return m_objectInHand.HeldObject != ObjectInHand.None;
    }

    public bool IsWearingObjectOnHead()
    {
        return m_objectOnHead.WornObject != ObjectOnHead.None;
    }

    public void HoldObjectInHand(DroppableHandProp handObject)
    {
        handObject.transform.SetParent(m_objectInHandParent);
        handObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        m_objectInHand.OnPuppetHeldObjectChanged(handObject.ObjectType);
    }

    public void WearObjectOnHead(DroppableHeadProp headObject)
    {
        headObject.transform.SetParent(m_objectOnHeadParent);
        headObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        m_objectOnHead.OnPuppetWornObjectChanged(headObject.ObjectType);
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

    #region Animator controller
    private void SetWalkState(bool walk)
    {
        m_isWalking = walk;
        m_puppetAnimator.SetBool(m_animatorParameterContainer.IsWalking, walk);
    }

    private void SetIsHoldingObjectAnimation(bool holdingObject)
    {
        m_puppetAnimator.SetBool(m_animatorParameterContainer.HoldsObject, holdingObject);
    }

    private void SetLookUpAnimation(bool lookUp)
    {
        m_puppetAnimator.SetBool(m_animatorParameterContainer.LookUp, lookUp);
    }

    private void StrikeAnimation()
    {
        m_puppetAnimator.SetTrigger(m_animatorParameterContainer.Strike);
    }

    private void SetVictoryAnimation(bool victory)
    {
        m_puppetAnimator.SetBool(m_animatorParameterContainer.Defeat, victory);
    }

    private void SetDefeatAnimation(bool defeat)
    {
        m_puppetAnimator.SetBool(m_animatorParameterContainer.Defeat, defeat);
    }
    #endregion
}
