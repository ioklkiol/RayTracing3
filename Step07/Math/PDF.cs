using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//概率密度分布函数   probability density function
public abstract class PDF
{
    public abstract float Value(Vector3D direction);
    public abstract Vector3D Generate();
}

public class CosinePDF : PDF
{
    private ONB uvw = new ONB();
    public CosinePDF(Vector3D w)
    {
        uvw.BuildFromW(w);
    }

    public override float Value(Vector3D direction) 
    {
        float cosine = direction.UnitVector() * uvw.W;
        if (cosine > 0)
            return cosine / Mathf.PI;
        else
            return 0;
    }
    public override Vector3D Generate()
    {
        return uvw.Local(Mathf.RandomCosineDirection());
    }
}
public class HitablePDF : PDF
{
    private Vector3D o;
    private Hitable ptr;

    public HitablePDF(Hitable ptr,Vector3D o)
    {
        this.o = o;
        this.ptr = ptr;
    }

    public override float Value(Vector3D direction)
    {
        return ptr.PDFValue(o, direction);
    }

    public override Vector3D Generate()
    {
        return ptr.Random(o);
    }
}
public class MixturePDF : PDF
{
    PDF[] p = new PDF[2];
    public MixturePDF(PDF p0, PDF p1)
    {
        p[0] = p0;
        p[1] = p1;
    }

    public override Vector3D Generate()
    {
        if (Mathf.Randomfloat() < 0.5f)
            return p[0].Generate();
        else
            return p[1].Generate();
    }

    public override float Value(Vector3D direction)
    {
        return 0.5f * p[0].Value(direction) + 0.5f * p[1].Value(direction);
    }
}