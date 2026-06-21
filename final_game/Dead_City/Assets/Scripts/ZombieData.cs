using UnityEngine;

// 좀비 데이터 ScriptableObject (Zombie2_1_R 원본)
// 층별로 다른 ZombieData를 만들어 사용 가능
[CreateAssetMenu(menuName = "Scriptable/ZombieData", fileName = "Zombie Data")]
public class ZombieData : ScriptableObject
{
    public float health = 100f;
    public float damage = 20f;
    public float speed = 2f;
    public Color skinColor = Color.white;
}
