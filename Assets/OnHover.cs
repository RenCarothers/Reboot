using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHover : MonoBehaviour
{   
    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
    }

    public virtual void OnMouseOver() 
    { 
       sprite.color = new Color (1, 0, 1, 1);
    } 
    
    public virtual void OnMouseExit() 
    {
        sprite.color = new Color (1, 1, 1, 1);
    } 
}

// https://forum.unity.com/threads/spriterenderer-color-doesnt-work-bug.721547/