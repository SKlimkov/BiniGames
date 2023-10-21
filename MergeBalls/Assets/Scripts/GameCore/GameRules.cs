using UnityEngine;

namespace BiniGames.GameCore {
    [CreateAssetMenu(fileName = "GameRules", menuName = "ScriptableObjects/GameCore/GameRules", order = 1)]
    public class GameRules : ScriptableObject {
        public enum EPlayerSpawnType {
            StaticGrade,
            RandomGrade
        };
        [Header("StartSpawn")]
        public int MinStartBallsCount = 1;
        public int MaxStartBallsCount = 4;
        public int MaxStartBallsGrade = 3;

        [Header("Player")]
        public EPlayerSpawnType PlayerSpawnType;
        [ConditionalHide("PlayerSpawnType", 0)]
        public int MaxPlayerBallGrade = 6;
        [ConditionalHide("PlayerSpawnType", 1), Range(0, 9)]
        public int PlayerGrade = 0;        
        public float ForcePower = 4f;

        [Header("Misc")]
        public float SightLineLenght = 7.5f;
        public int WinGrade = 9;
        public float MinRelativeVelocityForEffects = 3f;

        public float SqrMinRelativeVelocityForEffects { get { return MinRelativeVelocityForEffects * MinRelativeVelocityForEffects; } }
    }
}
