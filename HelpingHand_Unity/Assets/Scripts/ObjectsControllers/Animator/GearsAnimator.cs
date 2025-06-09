using UnityEngine;

public class GearsAnimator : MonoBehaviour, IBaseVariableContainer<float>
{
    public BaseVariable<float> Variable => m_controllingVariable;

    [SerializeField] private BaseVariable<float> m_controllingVariable;
    [SerializeField] private string m_animatorVariableName = "Progress";


    private Animator m_animator;
    private int m_progressVariableHash;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_progressVariableHash = Animator.StringToHash(m_animatorVariableName);
    }

    // Update is called once per frame
    private void Update()
    {
        m_animator.SetFloat(m_progressVariableHash, m_controllingVariable.Value);
    }
}
