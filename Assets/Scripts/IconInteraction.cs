using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconInteraction : MonoBehaviour
{
    public void OnFlowerClick()
    {
        Debug.Log("Flower Icon Clicked");
        // 꽃 아이콘 동작
    }

    public void OnKeyClick()
    {
        Debug.Log("Key Icon Clicked");
        // 열쇠 아이콘 동작
    }

    public void OnKnotClick()
    {
        Debug.Log("Knot Icon Clicked");
        // 매듭 아이콘 동작
    }

    public void OnMountClick()
    {
        Debug.Log("Mount Icon Clicked");
        // 산 아이콘 동작
    }
}
