using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutOutMaskUI : Image
{
    public override Material materialForRendering
    {
        get
        {
            Material mat = base.materialForRendering;
            mat.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return mat;
                
        }
    }
}
