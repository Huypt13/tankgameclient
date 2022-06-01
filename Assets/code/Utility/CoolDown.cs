using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CoolDown
{


    private float length;
    private float currentTime;
    private bool onCoolDown;
    public CoolDown(float Length = 1, bool StartWithCooldown = false)
    {
        currentTime = 0;
        length = Length;
        onCoolDown = StartWithCooldown;
    }
    public void CoolDownUpdate()
    {
        if (onCoolDown)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= length)
            {
                currentTime = 0;
                onCoolDown = false;
            }
        }
    }
    public bool IsOnCooldown()
    {
        return onCoolDown;
    }
    public void StarCoolDown()
    {
        onCoolDown = true;
        currentTime = 0;
    }
}
