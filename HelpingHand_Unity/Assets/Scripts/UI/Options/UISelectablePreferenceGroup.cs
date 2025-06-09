using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UISelectablePreferenceGroup : MonoBehaviour
{
    [SerializeField] private InputActionReference m_cancelAction;

    [SerializeField] private Selectable m_saveButton;
    [SerializeField] private Selectable m_resetButton;

    private UIOptionController[] m_selectableControllerList;

    public int ControllerCount => m_selectableControllerList.Length;

    private UIOptionController m_currentSelectedController;

    private void Start()
    {
        m_cancelAction.action.performed += OnCancel;

        m_selectableControllerList = GetComponentsInChildren<UIOptionController>();
        if (m_selectableControllerList.Length == 0)
        {
            return;
        }

        for (var i = 0; i < m_selectableControllerList.Length; i++)
        {
            var controller = m_selectableControllerList[i];
            controller.Init(this, i);
            controller.OnControllerSelected += OnControllerSelected;
            var previous = m_selectableControllerList[(i - 1 + m_selectableControllerList.Length) % m_selectableControllerList.Length];
            var next = m_selectableControllerList[(i + 1) % m_selectableControllerList.Length];
            controller.SetNavigationUp(previous.MainChild);
            controller.SetNavigationDown(next.MainChild);
        }

        var firstController = m_selectableControllerList[0];
        var lastController = m_selectableControllerList[^1];

        firstController.SetNavigationUp(m_saveButton);
        lastController.SetNavigationDown(m_saveButton);

        Navigation navigation = m_saveButton.navigation.Clone();
        navigation.selectOnUp = lastController.MainChild;
        navigation.selectOnDown = firstController.MainChild;
        m_saveButton.navigation = navigation;

        navigation = m_resetButton.navigation.Clone();
        navigation.selectOnUp = lastController.MainChild;
        navigation.selectOnDown = firstController.MainChild;
        m_resetButton.navigation = navigation;
    }

    private void OnCancel(InputAction.CallbackContext obj)
    {
        m_currentSelectedController?.Deselect();
        m_currentSelectedController = null;
        m_saveButton.Select();
    }

    private void OnControllerSelected(UIOptionController controller)
    {
        m_currentSelectedController?.Deselect();
        m_currentSelectedController = controller;
    }
}
