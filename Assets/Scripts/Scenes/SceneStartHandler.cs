using UnityEngine;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    public class SceneStartHandler : MonoBehaviour
    {
        public UnityEvent OnSceneStart;
        void Start()
        {
            OnSceneStart ??= new UnityEvent();
            OnSceneStart?.Invoke();
        }
    }
}
