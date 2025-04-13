using System;

using UnityEngine;

public class WalkState : PuppetState
{
    //[SerializeField, Min(0)] private int m_tilesToAdvance;
    //[SerializeField, Min(0.1f)] private float m_speedMultiplier = 1.0f;

    //[NonSerialized] private float m_progress;
    //[NonSerialized] private bool m_stopeedThisTile;
    [NonSerialized] private Vector3 m_startPosition;
    [NonSerialized] private Vector3 m_targetPosition;

    [NonSerialized] private bool m_stopped;
    [NonSerialized] private bool m_checkedNextTile;

    public override void BeginState()
    {
        base.BeginState();

        m_stopped = false;
        m_checkedNextTile = false;

        m_startPosition = m_puppet.transform.position;
        m_targetPosition = m_startPosition + m_puppet.transform.forward * m_puppet.Settings.TileSize;
    }

    public override void UpdateState(float progress, float deltaTime)
    {
        base.UpdateState(progress, deltaTime);

        if (!m_checkedNextTile && progress >= m_puppet.Settings.CheckNextTileProgress)
        {
            m_checkedNextTile = true;
            m_stopped = !CheckNextTile();
        }

        if (m_stopped)
        {
            return;
        }

        m_puppet.SetPosition(Vector3.Lerp(m_startPosition, m_targetPosition, progress));
    }

    private bool CheckNextTile()
    {
        return true;
    }

    public override void EndState()
    {
        base.EndState();
        if (!m_stopped)
        {
            m_puppet.SetPosition(m_targetPosition);
        }
    }
}
