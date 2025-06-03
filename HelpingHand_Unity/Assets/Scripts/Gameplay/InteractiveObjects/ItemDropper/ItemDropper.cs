using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Splines;

using UnityUtility.CustomAttributes;
using UnityUtility.Easings;
using UnityUtility.Extensions;
using UnityUtility.MathU;
using UnityUtility.Timer;

using Title = UnityUtility.CustomAttributes.TitleAttribute;

public class ItemDropper : MonoBehaviour
{
    private enum DropperState
    {
        Inactive,
        Ready,
        Transition,
        TransitionOut,
    }

    private class DropperItem
    {
        public DroppableItem Item => m_item;

        [NonSerialized] private readonly DroppableItem m_item;

        [NonSerialized] private float m_startPosition;
        [NonSerialized] private float m_targetPosition;
        [NonSerialized] private float m_currentPosition;

        [NonSerialized] private readonly SplineContainer m_spline;


        public DropperItem(DroppableItem item, SplineContainer spline)
        {
            m_item = item;

            m_startPosition = 0.0f;
            m_targetPosition = 0.0f;

            m_spline = spline;
        }

        public void Init(float startPosition)
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

            m_item.transform.position = m_spline.EvaluatePosition(clampedPosition);
        }

        public void UpdatePosition(float transitionProgress, Easings.EasingFunction easingFunction)
        {
            float clampedProgress = MathUf.Clamp01(transitionProgress);

            float position = MathUf.Lerp(m_startPosition, m_targetPosition, Easings.Ease(clampedProgress, easingFunction));
            SetPositionOnSpline(position);

        }
    }



    private const int DISPLAYED_ITEMS = 3;

    [Title("Variable references")]
    [SerializeField] private RotaryEncoderInputEvent m_selectorInput;
    [SerializeField] private BaseVariable<bool> m_validationInput;
    [SerializeField] private BaseVariable<bool> m_isActiveVariable;

    [Title("Options")]
    [SerializeField] private DroppableItem[] m_choices;
    [SerializeField, RequiredListLength(DISPLAYED_ITEMS), Range(0.0f, 1.0f)]
    private float[] m_displayedItemsPositions = new float[DISPLAYED_ITEMS];
    [SerializeField, Range(0, DISPLAYED_ITEMS - 1)] private int m_selectedItemIndex = 1;
    [SerializeField] private bool m_bufferInputs;

    [Title("Transition")]
    [SerializeField] private SplineContainer m_spline;
    [SerializeField] private float m_transitionTime;
    [SerializeField] private Easings.EasingFunction m_transitionEasingFunction;

    // Cache
    [NonSerialized] private DropperState m_currentState;
    [NonSerialized] private int m_currentIndex;
    [NonSerialized] private Timer m_transitionTimer;

    [SerializeField] private Dequeue<DropperItem> m_displayedItems;
    [SerializeField] private Dequeue<DropperItem> m_storedItems;
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
                ApplyOffsetIfNeeded();

                break;
            case DropperState.Transition:
                UpdateTransition();
                break;
            case DropperState.TransitionOut:
                UpdateTransitionOut();
                break;
            default:
                break;
        }
    }

    private void ApplyOffsetIfNeeded()
    {
        if (m_bufferedOffsets.Count == 0)
        {
            return;
        }
        ApplyOffset(m_bufferedOffsets.Dequeue());
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

        m_displayedItems.ForEach(item => item.UpdatePosition(transitionProgress, m_transitionEasingFunction));
        m_storedMovingItem?.UpdatePosition(transitionProgress, m_transitionEasingFunction);

        if (m_currentState  == DropperState.Ready)
        {
            ApplyOffsetIfNeeded();
        }
    }

    private void UpdateTransitionOut()
    {
        if (!m_transitionTimer.IsRunning)
        {
            return;
        }

        float transitionProgress;
        if (m_transitionTimer.Update(Time.deltaTime))
        {
            m_currentState = DropperState.Inactive;
            transitionProgress = 1.0f;
        }
        else
        {
            transitionProgress = m_transitionTimer.Progress;
        }

        m_displayedItems.ForEach(item => item.UpdatePosition(transitionProgress, m_transitionEasingFunction));
        m_storedMovingItem?.UpdatePosition(transitionProgress, m_transitionEasingFunction);

        if (m_currentState == DropperState.Inactive)
        {
            Dispose();
        }
    }

    private void Dispose()
    {
        m_displayedItems.Clear();
        m_storedItems.Clear();
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
        m_currentState = DropperState.Transition;

        m_selectorInput.AddStepLeftListener(OnSelectionChangedLeft);
        m_selectorInput.AddStepRightListener(OnSelectionChangedRight);
        m_validationInput.AddListener(OnValidateInput);

        m_currentIndex = 0;

        for (int iChoice = 0; iChoice < m_choices.Length; iChoice++)
        {
            DroppableItem choice = m_choices[iChoice];
            DropperItem item = new DropperItem(choice, m_spline);

            if (iChoice < DISPLAYED_ITEMS)
            {
                m_displayedItems.EnqueueFront(item);
                continue;
            }
            m_storedItems.EnqueueFront(item);
        }

        float baseOffset = -m_displayedItemsPositions[DISPLAYED_ITEMS - 1];
        for (int displayedIndex = 0; displayedIndex < m_displayedItems.Count; displayedIndex++)
        {
            DropperItem displayedItem = m_displayedItems.At(displayedIndex);
            float targetOffset = m_displayedItemsPositions[displayedIndex];
            displayedItem.Init(baseOffset + targetOffset);
            displayedItem.SetTarget(targetOffset, false);
        }

        m_transitionTimer.Reset();
        m_transitionTimer.Start();
    }

    private void Desactivate()
    {
        m_selectorInput.RemoveStepLeftListener(OnSelectionChangedLeft);
        m_selectorInput.RemoveStepRightListener(OnSelectionChangedRight);
        m_validationInput.RemoveListener(OnValidateInput);

        float baseOffset = 1.0f - m_displayedItemsPositions[0];
        int index = 0;
        m_displayedItems.ForEach(item => item.SetTarget(baseOffset + m_displayedItemsPositions[index++]));
        m_storedMovingItem = null;

        m_transitionTimer.Reset();
        m_transitionTimer.Start();

        m_currentState = DropperState.TransitionOut;
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
        if (!m_bufferInputs && m_currentState != DropperState.Ready)
        {
            return;
        }

        m_bufferedOffsets.Enqueue(offset);
    }

    private void ApplyOffset(int offset)
    {
        m_currentState = DropperState.Transition;
        int optionsCount = m_choices.Length;
        m_currentIndex = (m_currentIndex + offset + optionsCount) % optionsCount;

        if (offset > 0)
        {
            DropperItem itemToStore = m_displayedItems.DequeueFront();
            DropperItem itemToDisplay = m_storedItems.DequeueFront();

            m_displayedItems.EnqueueRear(itemToDisplay);
            m_storedItems.EnqueueRear(itemToStore);

            m_storedMovingItem = itemToStore;
            m_storedMovingItem.SetTarget(0.0f);

            itemToDisplay.SetPositionOnSpline(1.0f);
        }
        else
        {
            DropperItem itemToStore = m_displayedItems.DequeueRear();
            DropperItem itemToDisplay = m_storedItems.DequeueRear();

            m_displayedItems.EnqueueFront(itemToDisplay);
            m_storedItems.EnqueueFront(itemToStore);

            m_storedMovingItem = itemToStore;
            m_storedMovingItem.SetTarget(1.0f);

            itemToDisplay.SetPositionOnSpline(0.0f);
        }

        int index = 0;
        m_displayedItems.ForEach(item => item.SetTarget(m_displayedItemsPositions[index++]));

        m_transitionTimer.Reset();
        m_transitionTimer.Start();
    }

    private void OnValidateInput(bool validate)
    {
        if (!validate)
        {
            return;
        }

        if (m_currentState != DropperState.Ready)
        {
            // @TODO Play error sound
            return;
        }

        m_displayedItems.At(m_selectedItemIndex).Item.DropItem();
    }
}
