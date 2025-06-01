using TMPro;

using UnityEngine;

public class UIActionDisplayController : MonoBehaviour
{
    [SerializeField] private TMP_Text m_actionName;
    [SerializeField] private TMP_Text m_actionBinding;

    public void Initialize(string actionName, string actionBinding)
    {
        m_actionName.text = $"{actionName}:";
        m_actionBinding.text = actionBinding;
    }
}
