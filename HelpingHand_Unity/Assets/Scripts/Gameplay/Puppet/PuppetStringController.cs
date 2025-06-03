using System;

using UnityEngine;

public class PuppetStringController : MonoBehaviour
{
    private class PuppetString
    {
        public Transform PuppetAttatchment;
        public LineRenderer StringRenderer;
    }

    [SerializeField] private Transform[] m_stringAttachments;
    [SerializeField] private float m_stringWidth;

    [NonSerialized] private PuppetString[] m_puppetStrings;

    private void Start()
    {
        m_puppetStrings = new PuppetString[m_stringAttachments.Length];
        for (int i = 0; i < m_stringAttachments.Length; i++)
        {
            m_puppetStrings[i] = new PuppetString()
            {
                PuppetAttatchment = m_stringAttachments[i],
                StringRenderer = m_stringAttachments[i].gameObject.AddComponent<LineRenderer>()
            };
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (Transform attachment in m_stringAttachments)
        {
            Gizmos.DrawSphere(attachment.position, 0.1f);
        }
    }

}
