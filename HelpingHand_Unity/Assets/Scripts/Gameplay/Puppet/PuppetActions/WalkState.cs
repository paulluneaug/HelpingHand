using System;

[Serializable]
public class WalkState : PuppetState
{
    //[SerializeField, Min(0)] private int m_tilesToAdvance;
    //[SerializeField, Min(0.1f)] private float m_speedMultiplier = 1.0f;

    //[NonSerialized] private float m_progress;
    //[NonSerialized] private bool m_stopeedThisTile;
    //[NonSerialized] private Vector3 m_targetPosition;

    //public override void BeginState(Puppet puppet)
    //{
    //    base.BeginState(puppet);
    //    m_progress = 0.0f;
    //    m_targetPosition = puppet.transform.position + puppet.transform.forward * m_tilesToAdvance * puppet.Settings.TileSize;
    //}

    //public override void UpdateState(float deltaTime)
    //{
    //    if (m_finished)
    //    {
    //        return;
    //    }
    //    base.UpdateState(deltaTime);

    //    float addedProgress = deltaTime * m_puppet.Settings.PuppetAbsoluteSpeed * m_speedMultiplier;

    //    if ((int)m_progress != (int)m_progress + addedProgress)
    //    {
    //        // New tile
    //        m_stopeedThisTile = !CheckNextTile();
    //    }

    //    // Make sure not to overshoot
    //    if (addedProgress > m_tilesToAdvance - m_progress)
    //    {
    //        addedProgress = m_tilesToAdvance - m_progress;
    //        FinishAction();
    //    }

    //    m_progress += addedProgress;

    //    if (!m_stopeedThisTile)
    //    {
    //        m_puppet.MoveForward(addedProgress);
    //    }

    //}

    //private bool CheckNextTile()
    //{
    //    return true;
    //}

    //public override void EndState()
    //{
    //    base.EndState();
    //    m_puppet.SetPosition(m_targetPosition);
    //}
}
