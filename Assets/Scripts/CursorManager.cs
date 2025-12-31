using UnityEngine;

public static class CursorManager
{
    public static void SetGameplay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void SetUI()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
