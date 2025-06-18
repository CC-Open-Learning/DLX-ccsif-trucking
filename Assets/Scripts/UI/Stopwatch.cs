using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace VARLab.CCSIF
{
    /// <summary>
    /// UI for the stopwatch to track the amount of inspections the player interacts with
    /// </summary>
    public class Stopwatch : MonoBehaviour
    {
        private StopwatchElement timerElement;

        private VisualElement root;

        public UnityEvent OnStopwatchStarted;
        public UnityEvent OnStopwatchRepeatingInterval;
        public UnityEvent OnStopwatchDeduction;
        public UnityEvent<int> OnRequestStopwatchTime;
        
        [SerializeField] private ScoreMetricsSO scoreMetrics;
        
        private void Start()
        {
            UIDocument document = GetComponent<UIDocument>();
            root = document.rootVisualElement;
            timerElement = root.Q<StopwatchElement>();

            OnStopwatchStarted ??= new UnityEvent();
            OnStopwatchRepeatingInterval ??= new UnityEvent();
            OnStopwatchDeduction ??= new UnityEvent();
            OnRequestStopwatchTime ??= new UnityEvent<int>();

            StartStopWatch(scoreMetrics.StartingStopwatchTime, scoreMetrics.StopwatchPenaltyIncrementSize);
        }

        private void Update()
        {
            if ((timerElement.CurrentTime < 0) && (timerElement.CurrentTime % scoreMetrics.StopwatchPenaltyIncrementSize == 0))
            {
                OnStopwatchRepeatingInterval.Invoke();
            }
        }

        public void DeductTime(int amount)
        {
            timerElement.CurrentTime -= amount;
            OnStopwatchDeduction.Invoke();
        }

        public void AddTime(int amount)
        {
            timerElement.CurrentTime += amount;
        }

        public void StartStopWatch(int startTime, int repeatingTime)
        {
            timerElement.StartTime = startTime;
            timerElement.CurrentTime = startTime;
            timerElement.TimeLimitRepeat = repeatingTime;
            OnStopwatchStarted.Invoke();
        }

        public void PassStopwatchTime()
        {
            OnRequestStopwatchTime.Invoke((int)timerElement.CurrentTime);
        }
    }
}
