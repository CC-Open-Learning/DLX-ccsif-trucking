using UnityEngine;
using UnityEngine.SceneManagement;

namespace VARLab.CCSIF
{
    public class DevTools : MonoBehaviour
    {
        [SerializeField] private bool developerToolsEnabled;
        private const string moduleBScene = "Module B First Inspection Scene";
        private const string moduleAScene = "Module A Scene";

        /// <summary>
        /// Disables developer tools when the DLX is ran outside of the Unity editor
        /// </summary>
        private void Awake()
        {
#if !UNITY_EDITOR
            gameObject.SetActive(false);
 #endif     
        }

        private void Update()
        {
            if (developerToolsEnabled)
            {
                if(Input.GetKeyDown(KeyCode.I))
                {
                    SceneManager.LoadScene(moduleAScene);
                }

                if(Input.GetKeyDown(KeyCode.O)) 
                {
                    SceneManager.LoadScene(moduleBScene);
                }
            }
        }
    }
}
