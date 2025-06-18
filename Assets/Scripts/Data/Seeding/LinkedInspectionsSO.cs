using System.Collections.Generic;
using UnityEngine;

namespace VARLab.CCSIF
{
    [CreateAssetMenu(fileName = "LinkedInspectionsSO", menuName = "ScriptableObjects/Seeding/LinkedInspectionsSO")]
    public class LinkedInspectionsSO : ScriptableObject
    {
        public BrokenStateSO DamageSource;
        public List<BrokenStateSO> DamageEndpoints;
    }
}
