using System;

using UnityEngine;

using UnityUtility.Extensions;
using UnityUtility.MathU;

public class PuppetStringController : MonoBehaviour
{
    private class PuppetString
    {
        private readonly Transform m_puppet;
        private readonly Transform m_puppetAttatchment;
        private readonly LineRenderer m_stringRenderer;

        private Vector2 m_targetPosition;
        private Vector2 m_currentPosition;

        public PuppetString(Transform puppet, Transform puppetAttatchment, LineRenderer stringRenderer)
        {
            m_puppet = puppet;
            m_puppetAttatchment = puppetAttatchment;
            m_stringRenderer = stringRenderer;
        }

        public void UpdateString(float deltaTime, float stringsHeight, float stringTargetFactor, float halfLife)
        {
            m_targetPosition = GetAttachmentLocalPosition().XZ() * stringTargetFactor;
            m_currentPosition = MathUf.SmoothLerp(m_currentPosition, m_targetPosition, deltaTime, halfLife);

            m_stringRenderer.SetPosition(0, m_puppetAttatchment.position);
            m_stringRenderer.SetPosition(1, new Vector3(m_currentPosition.x, stringsHeight, m_currentPosition.y));
        }

        private Vector3 GetAttachmentLocalPosition()
        {
            return m_puppetAttatchment.position - m_puppet.position;
        }
    }

    [SerializeField] private Transform[] m_stringAttachments;
    [SerializeField] private float m_stringsHeightOffset;
    [SerializeField] private float m_stringFollowHalfLife;
    [SerializeField] private float m_stringTargetFactor;

    [SerializeField] private LineRenderer m_stringRendererPrefab;

    [NonSerialized] private PuppetString[] m_puppetStrings;

    private void Start()
    {
        m_puppetStrings = new PuppetString[m_stringAttachments.Length];
        for (int i = 0; i < m_stringAttachments.Length; i++)
        {
            m_puppetStrings[i] = new PuppetString(transform, m_stringAttachments[i], CreateStringRenderer(m_stringAttachments[i]));
        }
    }

    private void Update()
    {
        float stringsHeight = transform.position.y + m_stringsHeightOffset;
        float deltaTime = Time.deltaTime;
        m_puppetStrings.ForEach(puppetString => puppetString.UpdateString(deltaTime, stringsHeight, m_stringTargetFactor, m_stringFollowHalfLife));
    }

    private LineRenderer CreateStringRenderer(Transform parent)
    {
        LineRenderer stringRenderer = Instantiate(m_stringRendererPrefab, parent);
        stringRenderer.name = $"{parent.name}_StringRenderer";
        stringRenderer.gameObject.SetActive(true);
        return stringRenderer;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        foreach (Transform attachment in m_stringAttachments)
        {
            if (attachment == null)
            {
                return;
            }
            Gizmos.DrawSphere(attachment.position, 0.1f);
        }
    }

}
