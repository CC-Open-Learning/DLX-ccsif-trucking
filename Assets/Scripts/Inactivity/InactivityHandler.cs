using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VARLab.Velcro;

namespace VARLab.CCSIF
{
    //this class handles notifying the user upon remaining inactive for a set amonut of time
    public class InactivityHandler : MonoBehaviour
    {
        [SerializeField] private Notification notificationInstance;

        [SerializeField, Tooltip("Time in seconds of player inactivity to be considered inactive")]
        private float inactivityDuration = 60.0f;

        private Coroutine inactiveCoroutine;
        private const float RepeatNotificationDelay = 8f;

        private void Start()
        {
            inactiveCoroutine = StartCoroutine(InactivityDelay());
        }

        private void Update()
        {
            if (MouseMovementCheck())
            {
                ResetInactivityCoroutine();
            }
        }

        private IEnumerator InactivityDelay()
        {
            yield return new WaitForSeconds(inactivityDuration);

            while (true)
            {
                notificationInstance.HandleDisplayUI(NotificationType.Info, "Hey! You should get back to fixing the truck!", FontSize.Large, Align.Center);
                yield return new WaitForSeconds(RepeatNotificationDelay);
            }
        }

        private void ResetInactivityCoroutine()
        {
            StopCoroutine(inactiveCoroutine);

            inactiveCoroutine = StartCoroutine(InactivityDelay());
        }

        private bool MouseMovementCheck()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                return true;

            if (Input.mouseScrollDelta != new Vector2(0, 0))
                return true;

            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                return true;

            return false;
        }
    }
}