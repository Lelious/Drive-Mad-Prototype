using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Configs 
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/Level Config")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Assets")]
        public AssetReferenceGameObject LevelPrefabReference;
    }
}
