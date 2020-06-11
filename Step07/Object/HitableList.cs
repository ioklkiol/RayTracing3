using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class HitableList:Hitable
{
    public List<Hitable> list=new List<Hitable>();


    public HitableList() { }
    public HitableList(List<Hitable> list)
    {
        this.list = list;
    }

    public override bool BoundingBox(float t0, float t1, out AABB box)
    {
        throw new NotImplementedException();
    }

    public override bool Hit(Ray r, float tMin, float tMax,out HitRecord rec)
    {
        rec = new HitRecord();
        HitRecord tempRec=new HitRecord();
        bool hitAnything = false;
        float closestSoFar = tMax;       //没有物体遮挡的情况下我们可以看无限远
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Hit(r, tMin, closestSoFar, out tempRec))
            {
                hitAnything = true;
                closestSoFar = tempRec.t;   //当有物体遮挡视线之后视线可达到的最远处就是上一个击中点
                rec = tempRec;
            }
        }
        return hitAnything;
    }

    public override float PDFValue(Vector3D o, Vector3D v)
    {
        float weight = 1.0f / list.Count;
        float sum = 0;
        for (int i = 0; i < list.Count; i++)
            sum += weight * list[i].PDFValue(o, v);
        return sum;
                 
    }

    public override Vector3D Random(Vector3D o)
    {
        int index = (int)Mathf.Range((Mathf.Randomfloat() * list.Count),0,1);  //这里要注意防止溢出
        return list[index].Random(o);

    }
}