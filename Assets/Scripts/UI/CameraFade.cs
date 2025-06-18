using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    public class CameraFade : MonoBehaviour
    {
        private VisualElement root;
        public VisualElement Root { get => root; }

        public UnityEvent OnFadeIn;
        public UnityEvent OnFadeOut;
        private const float delay = 1f; //References animation time in CameraFade.uss

        [SerializeField] bool playOnAwake = true;
        
        void Start()
        {
            OnFadeIn ??= new UnityEvent();
            OnFadeOut ??= new UnityEvent(); 
            UIDocument document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
            if (playOnAwake)
            {
                FadeOut();
            }
        }
        public void FadeIn()
        {
            UIHelper.Show(root);
            root.AddToClassList("fade-in");
            root.RemoveFromClassList("fade-out");
            StartCoroutine(FadeInEnumerator());
        }

        public void FadeOut()
        {
            StartCoroutine(fadeOutEnumerator());
        }
        
        private IEnumerator FadeInEnumerator()
        {
            yield return new WaitForSeconds(delay); //wait for animation to fade before hiding the root to allow inspections
            OnFadeIn?.Invoke();
        }
        
        private IEnumerator fadeOutEnumerator()
        {
            root.AddToClassList("fade-out");
            root.RemoveFromClassList("fade-in");
            yield return new WaitForSeconds(delay); //wait for animation to fade before hiding the root to allow inspections
            UIHelper.Hide(root);
            OnFadeOut?.Invoke();
        }
    }
}
