using UnityEngine;

namespace VARLab.CCSIF
{
    /// <summary>
    /// Allows all scoring scriptable objects to be present in every scene so that no data is lost with scene loading
    /// </summary>
    public class ScorePersistence : MonoBehaviour
    {
        [SerializeField] private ScoreDataSO scoreDataFirst;
        [SerializeField] private ScoreDataSO scoreDataSecond;
    }
}
