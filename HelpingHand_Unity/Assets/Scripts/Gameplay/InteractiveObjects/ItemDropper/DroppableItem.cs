using UnityEngine;

public abstract class DroppableItem : MonoBehaviour
{
    public Sprite Preview => m_preview;

    [SerializeField] protected GameObject m_model;
    [SerializeField] private Sprite m_preview;


    private void Awake()
    {
        DeactivateModel();
    }

    public void ActivateModel()
    {
        m_model.SetActive(true);
    }
    public void DeactivateModel()
    {
        m_model.SetActive(false);
    }

    public abstract bool CanDrop();

    public abstract void DropItem();
}
