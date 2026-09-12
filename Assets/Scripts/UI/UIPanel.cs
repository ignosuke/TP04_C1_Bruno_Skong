using UnityEngine;

public class UIPanel : MonoBehaviour
{
    public bool IsOpen()
    {
        return gameObject.activeInHierarchy;
    }

    public void Open()
    {
        gameObject.SetActive(true); 
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}