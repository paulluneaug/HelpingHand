using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Splines;

using UnityUtility.Easings;
using UnityUtility.Extensions;
using UnityUtility.MathU;
using UnityUtility.Timer;

public class ItemDropper : MonoBehaviour
{
    private enum DropperState
    {
        Inactive,
        Ready,
        InTransition,
    }

    [Serializable]
    private class DropperChoice
    {
        [SerializeField] private Transform m_movingItem;

        public Transform MovingItem => m_movingItem;
    }

    private class DropperItem
    {
        [NonSerialized] private readonly Transform m_item;

        [NonSerialized] private float m_startPosition;
        [NonSerialized] private float m_targetPosition;
        [NonSerialized] private float m_currentPosition;

        [NonSerialized] private readonly SplineContainer m_spline;
        [NonSerialized] private readonly Easings.EasingFunction m_easingFunction;

        public DropperItem(Transform item, SplineContainer spline, Easings.EasingFunction easingFunction)
        {
            m_item = item;

            m_startPosition = 0.0f;
            m_targetPosition = 0.0f;

            m_spline = spline;

            m_easingFunction = easingFunction;
        }

        public void Init(float startPosition)
        {
            m_currentPosition = startPosition;
            m_startPosition = startPosition;

            UpdatePosition(startPosition);
        }

        public void SetTarget(float target)
        {
            m_startPosition = m_currentPosition;
            m_targetPosition = target;
        }

        public void UpdatePosition(float progress)
        {
            float clampedProgress = MathUf.Clamp01(progress);

            m_currentPosition = MathUf.Lerp(m_startPosition, m_targetPosition, Easings.Ease(clampedProgress, m_easingFunction));

            m_item.position = m_spline.EvaluatePosition(m_currentPosition);
        }
    }



    private const int DISPLAYED_ITEMS = 3;

    [Title("Variable references")]
    [SerializeField] private RotaryEncoderInputEvent m_selectorInput;
    [SerializeField] private BaseVariable<bool> m_validationInput;
    [SerializeField] private BaseVariable<bool> m_isActiveVariable;

    [Title("Options")]
    [SerializeField] private DropperChoice[] m_choices;
    [SerializeField] private SplineContainer m_spline;
    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_transitionEasingFunction;

    [SerializeField, RequiredListLength(DISPLAYED_ITEMS), Range(0.0f, 1.0f)]
    private float[] m_displayedItemsPositions = new float[DISPLAYED_ITEMS];

    // Cache
    [NonSerialized] private DropperState m_currentState;
    [NonSerialized] private int m_currentIndex;
    [NonSerialized] private Timer m_transitionTimer;

    [NonSerialized] private Dequeue<DropperItem> m_displayedItems;
    [NonSerialized] private Dequeue<DropperItem> m_storedItems;
    [NonSerialized] private DropperItem m_storedMovingItem;

    [NonSerialized] private Queue<int> m_bufferedOffsets;

    private void Start()
    {
        if (m_choices.Length <= DISPLAYED_ITEMS)
        {
            throw new ArgumentException("Not enough choices");
        }

        m_isActiveVariable.AddListener(OnIsActiveChanged);
        m_transitionTimer = new Timer(m_transitionTime, false);

        m_displayedItems = new Dequeue<DropperItem>(DISPLAYED_ITEMS);
        m_storedItems = new Dequeue<DropperItem>(m_choices.Length - DISPLAYED_ITEMS);
        m_bufferedOffsets = new Queue<int>();

        m_displayedItemsPositions.Sort();

        OnIsActiveChanged(m_isActiveVariable.Value);
    }

    private void OnDestroy()
    {
        m_isActiveVariable.RemoveListener(OnIsActiveChanged);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (m_displayedItemsPositions.IsSorted())
        {
            return;
        }

        m_displayedItemsPositions.Sort();
    }
#endif

    // Update is called once per frame
    private void Update()
    {
        switch (m_currentState)
        {
            case DropperState.Inactive:
                break;

            case DropperState.Ready:
                if (m_bufferedOffsets.Count == 0)
                {
                    break;
                }
                ApplyOffset(m_bufferedOffsets.Dequeue());

                break;
            case DropperState.InTransition:
                UpdateTransition();
                break;
            default:
                break;
        }
    }

    private void UpdateTransition()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }

        float transitionProgress;
        if (m_transitionTimer.Update(Time.deltaTime))
        {
            m_currentState = DropperState.Ready;
            transitionProgress = 1.0f;
        }
        else
        {
            transitionProgress = m_transitionTimer.Progress;
        }

        m_displayedItems.ForEach(item => item.UpdatePosition(transitionProgress));
        m_storedMovingItem.UpdatePosition(transitionProgress);
    }

    private void OnIsActiveChanged(bool isActive)
    {
        if (isActive)
        {
            Activate();
            return;
        }
        Desactivate();
    }

    private void Activate()
    {
        m_currentState = DropperState.InTransition;

        m_selectorInput.AddStepLeftListener(OnSelectionChangedLeft);
        m_selectorInput.AddStepRightListener(OnSelectionChangedRight);
        m_validationInput.AddListener(OnValidateInput);

        m_currentIndex = 0;

        for (int iChoice = 0; iChoice < m_choices.Length; iChoice++)
        {
            DropperChoice choice = m_choices[iChoice];
            DropperItem item = new DropperItem(choice.MovingItem, m_spline, m_transitionEasingFunction);

            if (iChoice < DISPLAYED_ITEMS)
            {
                m_displayedItems.EnqueueFront(item);
                continue;
            }
            m_storedItems.EnqueueFront(item);
        }

        float baseOffset = 1.0f - m_displayedItemsPositions[0];
        for (int displayedIndex = 0; displayedIndex < m_displayedItems.Count; displayedIndex++)
        {
            DropperItem displayedItem = m_displayedItems.At(displayedIndex);
            float targetOffset = m_displayedItemsPositions[displayedIndex];
            displayedItem.Init(baseOffset + targetOffset);
            displayedItem.SetTarget(targetOffset);
        }
    }

    private void Desactivate()
    {
        m_currentState = DropperState.Inactive;

        m_selectorInput.RemoveStepLeftListener(OnSelectionChangedLeft);
        m_selectorInput.RemoveStepRightListener(OnSelectionChangedRight);
        m_validationInput.RemoveListener(OnValidateInput);

        m_displayedItems.Clear();
        m_storedItems.Clear();
    }

    private void OnSelectionChangedLeft()
    {
        OnSelectionChanged(-1);
    }

    private void OnSelectionChangedRight()
    {
        OnSelectionChanged(1);
    }

    private void OnSelectionChanged(int offset)
    {
        m_bufferedOffsets.Enqueue(offset);
    }

    private void ApplyOffset(int offset)
    {
        m_currentState = DropperState.InTransition;
        int optionsCount = m_choices.Length;
        m_currentIndex = (m_currentIndex + offset + optionsCount) % optionsCount;

        if (offset > 0)
        {
            DropperItem itemToStore = m_displayedItems.DequeueFront();
            DropperItem itemToDisplay = m_storedItems.DequeueFront();

            m_displayedItems.EnqueueFront(itemToDisplay);
            m_storedItems.EnqueueFront(itemToStore);
            m_storedMovingItem = itemToStore;
        }
        else
        {
            DropperItem itemToStore = m_displayedItems.DequeueRear();
            DropperItem itemToDisplay = m_storedItems.DequeueRear();

            m_displayedItems.EnqueueRear(itemToDisplay);
            m_storedItems.EnqueueRear(itemToStore);
            m_storedMovingItem = itemToStore;
        }


        m_transitionTimer.Reset();
        m_transitionTimer.Start();
    }

    private void OnValidateInput(bool validate)
    {
        if (!validate)
        {
            return;
        }
    }

}
