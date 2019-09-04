using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalMethods
{

    public static bool Epsilon(float x, float y)
    {
        if (x > y-0.01f && x < y + 0.01f)
            return true;
        else 
            return false;
    }

}
