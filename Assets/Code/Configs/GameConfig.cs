using UnityEngine;

namespace Code.Infrastructure.Data
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        public int DebugTimeShiftHours = 5;
    }
}