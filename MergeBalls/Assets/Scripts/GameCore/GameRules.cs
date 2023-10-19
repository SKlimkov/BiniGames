using UnityEngine;

namespace BiniGames.GameCore {
    [CreateAssetMenu(fileName = "GameRules", menuName = "ScriptableObjects/GameCore/GameRules", order = 1)]
    public class GameRules : ScriptableObject {
        public enum EPlayerSpawnType {
            StaticGrade,
            RandomGrade
        };
        public int MinStartBallsCount = 1;
        public int MaxStartBallsCount = 4;
        public int MaxStartBallsGrade = 3;
        public EPlayerSpawnType PlayerSpawnType;
        [ConditionalHide("PlayerSpawnType", 0)]
        public int MaxPlayerBallGrade = 6;
        [ConditionalHide("PlayerSpawnType", 1), Range(0, 9)]
        public int PlayerGrade = 0;
        public float SightLineLenght = 7.5f;
        public float ForcePower = 4f;
    }
}
