using UnityEngine;

[CreateAssetMenu(fileName = "NewLocation", menuName = "Game/Location Data")]
public class LocationData : ScriptableObject
{
    public LocationType Type;
    public string DisplayName;
    public Sprite MapIcon;
    public bool UnlockedByDefault = true;
    public int UnlockReputation;             // 0 = always open
}
