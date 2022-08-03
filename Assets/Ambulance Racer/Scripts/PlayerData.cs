using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Create new PlayerData", order = 51)]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public Sprite playerSprite;
    public float maxSpeed;
    public int acceleration;
    public int handling;
}
