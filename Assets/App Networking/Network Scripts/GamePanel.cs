using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour
{
    
    protected virtual void HidePanel()
    {
        this.gameObject.SetActive(false);
    }

    protected virtual void ShowPanel()
    {
        this.gameObject.SetActive(true);
    }
    
}
