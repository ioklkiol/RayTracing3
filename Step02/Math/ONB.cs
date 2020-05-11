using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Ortho-normal Bases 标准正交集合
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
        axis[2] = n.UnitVector();
        Vector3D a;
        if (Mathf.Abs(W.X) > 0.9)
            a = new Vector3D(0, 1, 0);
        else
            a = new Vector3D(1, 0, 0);
        axis[1] = Vector3D.Cross(W, a).UnitVector();
        axis[0] = Vector3D.Cross(W, V);
    }
}