using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInitAnim : MonoBehaviour
{
    void Update()
    {
        if (GetComponent<SpriteRenderer>().color.a == 0)
            Destroy(gameObject);
    }

}
