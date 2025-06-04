using UnityEngine;

public class GearsAnimator : MonoBehaviour
{
    [SerializeField] private BaseVariable<float> var;
    private Animator m_anim;
    private int m_progressVariableHash;
    
    
    void Start()
    {
        m_anim = GetComponent<Animator>();
        m_progressVariableHash = Animator.StringToHash("Progress");
    }

    // Update is called once per frame
    void Update()
    {
        m_anim.SetFloat(m_progressVariableHash, var.Value);
    }
}
