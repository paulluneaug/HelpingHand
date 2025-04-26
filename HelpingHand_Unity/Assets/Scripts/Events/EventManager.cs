using UnityEngine;

using UnityUtility.Singletons;

namespace Events
{
    public class EventManager : MonoBehaviourSingleton<EventManager>
    {
        [field: SerializeField]
        public FloatGameEvent OnSlider1ValueChanged { get; private set; }

        [field: SerializeField]
        public FloatGameEvent OnSlider2ValueChanged { get; private set; }

        [field: SerializeField]
        public FloatGameEvent OnSlider3ValueChanged { get; private set; }

        [field: SerializeField]
        public FloatGameEvent OnSlider4ValueChanged { get; private set; }
    }
}