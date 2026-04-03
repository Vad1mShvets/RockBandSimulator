using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Game/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] Lines;
}
