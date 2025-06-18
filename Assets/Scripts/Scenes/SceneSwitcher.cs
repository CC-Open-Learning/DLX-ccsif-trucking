using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace VARLab.CCSIF
{
    /// <summary>
    /// A tool used to switch scene through an event listener.
    /// </summary>
    public class SceneSwitcher : MonoBehaviour
    {
        [SerializeField] private SceneType sceneType;

        public UnityEvent OnSceneEnter;
        public UnityEvent OnSceneExit;
        
        /// <summary>
        /// Dictionary for connecting the easy to select SceneType enum, to the module B scene names.
        /// </summary>
        private readonly Dictionary<SceneType, string> sceneNames = new Dictionary<SceneType, string>()
        {
            { SceneType.InspectionOne, "Module B First Inspection Scene" },
            { SceneType.InspectionTwo, "Module B Second Inspection Scene" },
            { SceneType.CutsceneOne, "Module B First Cutscene Scene" },
            { SceneType.CutsceneTwo, "Module B Second Cutscene Scene" },
        };

        private void Start()
        {
            OnSceneEnter ??= new UnityEvent();
            OnSceneExit ??= new UnityEvent();
            
            StartCoroutine(StartSceneDelay());
        }

        private IEnumerator StartSceneDelay()
        {
            yield return new WaitForSeconds(1f);
            OnSceneEnter.Invoke();
        }

        public void BeginSceneTransition()
        {
            OnSceneExit.Invoke();
        }
        
        /// <summary>
        /// Loads the next scene chosen in the inspector, used as an event listener.
        /// </summary>
        public void LoadNextScene()
        {
            sceneNames.TryGetValue(sceneType, out string sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}
