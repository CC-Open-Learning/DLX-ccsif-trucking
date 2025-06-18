using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;
using VARLab.Analytics;

namespace VARLab.CCSIF
{
    public class AnalyticsHelper : MonoBehaviour
    {
        private void Awake()
        {
            CoreAnalytics.Initialize();
            LoginEvent();
            DLXVersionCheck();
        }

        /// <summary>
        /// Sends the analytics login event with the users devices unique identifier as the unique ID to PlayFab
        /// </summary>
        private void LoginEvent()
        {
            CoreAnalytics.LoginUser(SystemInfo.deviceUniqueIdentifier, loginSuccessCallback, loginErrorCallback);
        }

        /// <summary>
        /// Login result success callback
        /// </summary>
        /// <param name="loginResult"> result data </param>
        private void loginSuccessCallback(LoginResult loginResult)
        {
            Debug.Log("Login Success Callback");
            
        }

        /// <summary>
        /// Login result failure callback
        /// </summary>
        /// <param name="loginError"> result data </param>
        private void loginErrorCallback(PlayFabError loginError)
        {
            Debug.Log("Login Error Callback");
        }

        /// <summary>
        /// Sends the version analytics event to PlayFab
        /// </summary>
        private void DLXVersionCheck()
        {
            CoreAnalytics.VersionCheck(versionResult);
        }

        /// <summary>
        /// Version check result success callback
        /// </summary>
        /// <param name="versionResult"> result data </param>
        private void versionResult(VersionResult versionResult)
        {
            string versionLogMessage = "Version Check Callback";
            versionLogMessage += "\nIs a Correct Version " + versionResult.IsVersionCorrect;
            versionLogMessage += "\nCurrent Version " + versionResult.CurentVersionNumber;
            versionLogMessage += "\nNew Version " + versionResult.NewVersionNumber;
            Debug.Log(versionLogMessage);
        }

        /// <summary>
        /// Sends analytics custom event when the user requests to view inspection results
        /// </summary>
        /// Invoked from <see cref="InspectionResults.OnPlayAgain"/>
        public void DLXPlayAgain(List<string> inspectionData)
        {
            int playthroughCount = GetPlaythoughCount(inspectionData);

            if (playthroughCount != -1) 
            {
                CoreAnalytics.CustomEvent("Play_Again", $"Playthrough_{playthroughCount}", inspectionData);
            }
            else
            {
                Debug.LogWarning("Inspection result data sent to analytics helper has an error");
            }

        }

        /// <summary>
        /// The first list item in the inspection results data should the playthrough count. This method
        /// removes it as an Int and then removes it from the list.
        /// </summary>
        /// <param name="inspectionData"> A list of all completed inspections </param>
        /// <returns> The playthrough count </returns>
        private int GetPlaythoughCount(List<string> inspectionData) 
        {
            int playthroughCount = 0;
            if (int.TryParse(inspectionData[0], out playthroughCount))
            {
                inspectionData.RemoveAt(0);
                return playthroughCount;
            }

            return -1;
        }
    }
}
