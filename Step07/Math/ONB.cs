using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Ortho-normal Bases 标准正交基
public class ONB
{
    private Vector3D[] axis=new Vector3D[3];
    
    public Vector3D U { get => axis[0]; set => axis[0] = value; }
    public Vector3D V { get => axis[1]; set => axis[1] = value; }
    public Vector3D W { get => axis[2]; set => axis[2] = value; }
    public Vector3D this[int index]
    {
        get => axis[index];
        set => axis[index] = value;
    }
    public ONB() { }

    public Vector3D Local(float a, float b, float c)
    {
        return a * U + b * V + c * W;
    }
    public Vector3D Local(Vector3D a)
    {
        return a.X * U + a.Y * V + a.Z * W;
    }
    public void BuildFromW(Vector3D n)
    {
        axis[2] = n.UnitVector();                       //法线方向我们做为z轴
        Vector3D a;
        if (Mathf.Abs(W.X) > 0.9)                       //而后我们再随意取一个方向做为辅助变量，为了确保该向量长度不为0，且不平行为n，
            a = new Vector3D(0, 1, 0);                  //因此我们先看n是否是平行于x轴，若不是则取x轴，若是则取y轴为辅助向量。
        else
            a = new Vector3D(1, 0, 0);
        axis[1] = Vector3D.Cross(W, a).UnitVector();   //有了该辅助向量a，取axis[2](也就是z轴)与这个向量求cross就得到了另一个轴，我们计为y轴
        axis[0] = Vector3D.Cross(W, V);                //再与这个y轴求cross得到x轴
    }
}