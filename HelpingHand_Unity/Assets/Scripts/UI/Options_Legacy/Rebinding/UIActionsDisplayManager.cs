using UnityEngine;
using UnityEngine.InputSystem;

public class UIActionsDisplayManager : MonoBehaviour
{
    [SerializeField] private InputActionReference[] m_actions;
    [SerializeField] private UIActionDisplayController m_actionDisplayPrefab;
    [SerializeField] private RectTransform m_actionsDisplayContainer;

    private void Start()
    {
        foreach (var action in m_actions)
        {
            var actionDisplay = Instantiate(m_actionDisplayPrefab, m_actionsDisplayContainer);
            actionDisplay.Initialize(action.action.name, action.action.bindings[0].effectivePath);
        }
    }
}
