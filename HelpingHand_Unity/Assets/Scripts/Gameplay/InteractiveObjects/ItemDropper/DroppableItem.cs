using UnityEngine;

public abstract class DroppableItem : MonoBehaviour
{
    [SerializeField] protected GameObject m_model;
    [SerializeField] protected GameObject m_preview;

    private void Awake()
    {
        DeactivateModel();
    }

    public void ActivatePreview(Transform parent)
    {
        m_preview.transform.SetParent(parent);
        m_preview.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        m_preview.SetActive(true);
    }

    public void DeactivatePreview()
    {
        m_preview.transform.SetParent(transform);
        m_preview.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        m_preview.SetActive(false);
    }

    public void ActivateModel()
    {
        m_model.SetActive(true);
    }
    public void DeactivateModel()
    {
        m_model.SetActive(false);
    }

    public abstract void DropItem();
}
