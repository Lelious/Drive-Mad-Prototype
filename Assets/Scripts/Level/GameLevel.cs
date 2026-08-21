using UnityEngine;

namespace Levels
{
    public class GameLevel : MonoBehaviour
    {
        [SerializeField] private FinishZone _finishZone;
        [SerializeField] private Transform _startPoint;

        public Transform GetSpawnPoint() => _startPoint;
        public void RestartLevel() => _finishZone.ResetZone();
    }
}
