using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainHover : MonoBehaviour
{    
    public Material mat, selectedMat;
    public new MeshRenderer renderer;
    void OnMouseOver()
    {
        renderer.material = selectedMat;     
        if (Input.GetMouseButtonDown(1))
        {
            //ShootingController.instance.Hovered = gameObject;
        }
    }

    void OnMouseExit()
    {
        renderer.material = mat;
    }
}
