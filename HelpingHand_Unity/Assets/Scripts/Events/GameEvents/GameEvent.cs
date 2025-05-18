using Sirenix.OdinInspector;

using UnityEngine;

namespace Events
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Events/Game Event")]
    public class GameEvent : BaseGameEvent
    {
        [Button("Raise")]
        private void Internal_Raise()
        {
            Raise();
        }
    }
}