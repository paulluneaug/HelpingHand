
using System;

using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.VFX;

using UnityUtility.CustomAttributes;
using UnityUtility.Easings;
using UnityUtility.MathU;
using UnityUtility.Timer;

public class DropperItem : MonoBehaviour
{
    [Title("Preview")]
    [SerializeField] private SpriteRenderer m_previewRenderer;

    [Title("Misc")]
    [SerializeField] private VisualEffect m_fallVFX;
    [SerializeField] private float m_equipDelay;

    [Title("Animation")]
    [SerializeField] private Animator m_animator;
    [NonSerialized] private readonly int m_openParameter = Animator.StringToHash("Open");
    [NonSerialized] private readonly int m_shakeParameter = Animator.StringToHash("Shake");

    [NonSerialized] private DroppableItem m_item;

    [NonSerialized] private float m_startPosition;
    [NonSerialized] private float m_targetPosition;
    [NonSerialized] private float m_currentPosition;

    [NonSerialized] private SplineContainer m_spline;
    [NonSerialized] private Timer m_equipTimer;

    [NonSerialized] private bool m_alreadyDroppedItem;


    public void Init(DroppableItem item, SplineContainer spline)
    {
        m_item = item;

        m_equipTimer = new Timer(m_equipDelay, false);

        m_startPosition = 0.0f;
        m_targetPosition = 0.0f;

        m_spline = spline;

        m_alreadyDroppedItem = false;

        m_item.DeactivateModel();
        m_previewRenderer.sprite = m_item.Preview;

        m_animator.SetBool(m_openParameter, false);
    }

    public void StartItem(float startPosition)
    {
        m_startPosition = startPosition;
        m_targetPosition = startPosition;

        SetPositionOnSpline(startPosition);
    }

    public void SetTarget(float target, bool setStartPosition = true)
    {
        if (setStartPosition)
        {
            m_startPosition = m_currentPosition;
        }
        m_targetPosition = target;
    }

    public void SetPositionOnSpline(float position)
    {
        float clampedPosition = MathUf.Clamp01(position);
        m_currentPosition = clampedPosition;

        transform.position = m_spline.EvaluatePosition(clampedPosition);
    }

    public void UpdatePosition(float transitionProgress, Easings.EasingFunction easingFunction)
    {
        float clampedProgress = MathUf.Clamp01(transitionProgress);

        float position = MathUf.Lerp(m_startPosition, m_targetPosition, Easings.Ease(clampedProgress, easingFunction));
        SetPositionOnSpline(position);

    }

    public void DropItem(GameObject lootbox)
    {
        if (m_alreadyDroppedItem || !m_item.CanDrop())
        {

            _ = AudioManager.Instance.EventManager.Play_ItemDropperError.Post(lootbox);
            return;
        }

        // @TODO Play open trappe sound
        _ = AudioManager.Instance.EventManager.Play_ItemDropperBox_Open.Post(lootbox);

        m_animator.SetBool(m_openParameter, true);

        m_alreadyDroppedItem = true;

        m_fallVFX.Play();
        _ = AudioManager.Instance.EventManager.Play_FallingObjectAndSmokeImpact.Post(lootbox);
        m_equipTimer.Start();
    }

    public void ResetItem()
    {
        m_item = null;

        m_startPosition = 0.0f;
        m_targetPosition = 0.0f;

        m_spline = null;

        m_animator.SetBool(m_openParameter, false);
    }

    private void Update()
    {
        if (!m_equipTimer.IsRunning)
        {
            return;
        }

        if (m_equipTimer.Update(Time.deltaTime))
        {
            m_equipTimer.Stop();
            m_item.DropItem();
        }
    }
}