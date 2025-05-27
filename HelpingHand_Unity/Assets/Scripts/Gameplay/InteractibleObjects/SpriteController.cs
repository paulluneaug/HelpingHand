using System;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;

using UnityUtility.SerializedDictionary;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteController : SerializedMonoBehaviour
{
    [SerializeField]
    private SerializedDictionary<EntityState, Sprite> m_dictionary = new();

    private readonly Dictionary<EntityState, Action> m_actions = new();

    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        foreach (EntityState entityState in m_dictionary.Keys)
        {
            m_actions[entityState] = () => OnStateChanged(entityState);
            entityState.AddListener(m_actions[entityState]);
        }
    }

    private void OnStateChanged(EntityState state)
    {
        m_spriteRenderer.sprite = m_dictionary[state];
    }

    private void OnDestroy()
    {
        foreach (EntityState entityState in m_dictionary.Keys)
        {
            entityState.RemoveListener(m_actions[entityState]);
        }
    }
}
