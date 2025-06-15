using System;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutoutMask : Image
{
    public override Material materialForRendering 
    { 
        get
        {
            Material overrideMaterial = new Material(base.materialForRendering);
            overrideMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);

            return overrideMaterial;
        } 
    }
}
