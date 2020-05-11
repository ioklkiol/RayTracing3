using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcDx;

public class Preview : DxWindow
{
    public override void Update()
    {
        for (int i = 0; i < Buff.Length; i++)
           Buff[i] = (byte)Mathf.Range(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4], 0, 255);
           //之前是在这里进行的伽马校正，现在为了不影响渲染效果改了，进行直接光源采样后，在这里进行伽马校正会导致所渲染的图片有层状光圈
           // Buff[i] = (byte)Mathf.Range(Mathf.Sqrt(Renderer.main.buff[i] * 255 / Renderer.main.changes[i / 4]), 0, 255);
    }
}

public class Renderer
{
    private int width = 512;
    private int height = 512;
    private HitableList world = new HitableList();
    private Preview preview= new Preview(); 


    public static Renderer main;
    public int samples = 1000;
    public float[] buff;
    public int[] changes;
    public Bitmap bmp;
    public Camera camera;
    public void Init()
    {
        main = this;
        buff = new float[width * height * 4];
        changes = new int[width * height];
        bmp = new Bitmap(width, height);
        InitScene();

       Start();

        preview.Run(new DxConfiguration("Preview", width, height));
        System.Environment.Exit(0);
    }
    private void InitScene()
    {
        Final();
    }
    private void Final()
    {

        Vector3D lookFrom = new Vector3D(278, 278, -800);
        Vector3D lookAt = new Vector3D(278, 278, 0);
        float diskToFocus = 10;
        float aperture = 0;
        float vfov = 40;
        camera = new Camera(lookFrom, lookAt, new Vector3D(0, 1, 0), vfov,
            (float)width / (float)height, aperture, diskToFocus, 0, 1);

        List<Hitable> list = new List<Hitable>();

        Material red = new Lambertian(new ConstantTexture(new Vector3D(0.65f, 0.05f, 0.05f)));
        Material white = new Lambertian(new ConstantTexture(new Vector3D(0.73f, 0.73f, 0.73f)));
        Material green = new Lambertian(new ConstantTexture(new Vector3D(0.12f, 0.45f, 0.15f)));
        Material light = new DiffuseLight(new ConstantTexture(new Vector3D(15, 15, 15)));

        list.Add(new XZRect(213,343, 227, 332, 554, light));
        list.Add(new FlipNormals(new YZRect(0, 555, 0, 555, 555, green)));
        list.Add(new YZRect(0, 555, 0, 555, 0, red));
        list.Add(new FlipNormals(new XZRect(0, 555, 0, 555, 555, white)));
        list.Add(new XZRect(0, 555, 0, 555, 0, white));
        list.Add(new FlipNormals(new XYRect(0, 555, 0, 555, 555, white)));

        list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
           new Vector3D(165, 165, 165), white), -18), new Vector3D(130, 0, 65)));
        list.Add(new Translate(new RotateY(new Box(new Vector3D(0, 0, 0),
            new Vector3D(165, 330, 165), white), 15), new Vector3D(265, 0, 295)));
        BVHNode b = new BVHNode(list, list.Count, 0, 1);
        world.list.Add(b);
    }
    private class ScannerCofig
    {
        public int width;
        public int height;

        public ScannerCofig(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    private async void Start()
    {
        ThreadPool.SetMaxThreads(15, 15);
        await Task.Factory.StartNew(
            delegate { LinearScanner(new ScannerCofig(width,height)); });
        for (int i = 1; i < samples; i++)
        {
            ThreadPool.QueueUserWorkItem(LinearScanner, new ScannerCofig(width, height));
        }
    }
    private void LinearScanner(object o)
    {
        ScannerCofig config = (ScannerCofig)o;
        for (int j = 0; j < config.height; j++)
        {
            for (int i = 0; i < config.width; i++)
            {
                Vector3D color = new Vector3D(0, 0, 0);
                float u = (float)(i + Mathf.Randomfloat()) / (float)width;
                float v = 1 - (float)(j + Mathf.Randomfloat()) / (float)height;
                Ray ray = camera.GetRay(u, v);
                color = GetColor(ray, world, 0);

                //之前不在这里进行伽马校正是为了渲染速度，但现在进行直接光源采样，为了不影响渲染效果改了
                color = new Vector3D(Mathf.Sqrt(color.X), Mathf.Sqrt(color.Y), Mathf.Sqrt(color.Z));//这里不应进行伽马校正(误)                                                                                           
                SetPixel(i, j, color);
            }
        }
    }


    private Vector3D GetColor(Ray r, HitableList world, int depth)
    {
        HitRecord rec;
        /*这里的0.001不能改为0，当tmin设0的时候会导致，遍历hitlist时候，ray的t求解出来是0，
         * hit的时候全走了else，导致递归到50层的时候，最后return的是0，* attenuation结果还是0。
         * 距离越远，散射用到random_in_unit_sphere生成的ray误差越大
         */
        if (world.Hit(r, 0.001f, float.MaxValue, out rec))
        {
            Ray scattered;
            Vector3D emitted = rec.matPtr.Emitted(r,rec, rec.u, rec.v, rec.p);
            float pdf;
            Vector3D albedo;
            if (depth < 50 && rec.matPtr.Scatter(r, rec, out albedo, out scattered,out pdf))   
            {
                Vector3D onLight = new Vector3D(213 + Mathf.Randomfloat() * (343 - 213), 554, 227 + Mathf.Randomfloat() * (332 - 227));
                Vector3D toLight = onLight - rec.p;
                float distanceSquared = toLight.SquaredMagnitude();
                toLight.Normalize();
                if (toLight * rec.normal < 0)
                    return emitted;
                float lightArea = (343 - 213) * (332 - 227);
                float lightCosine = Math.Abs(toLight.Y);
                if (lightCosine < 0.000001)
                    return emitted;
                pdf = distanceSquared / (lightCosine * lightArea);
                scattered = new Ray(rec.p, toLight, r.Time);
                Vector3D color = GetColor(scattered, world, depth + 1);      //每次光线衰减之后深度加一
                return emitted + rec.matPtr.ScatteringPDF(r, rec, scattered)
                   *  new Vector3D(albedo.X * color.X, albedo.Y * color.Y, albedo.Z * color.Z)/pdf;
            }
            else
            {
                return emitted;
            }
        }
        else
        {
            //Vector3D unitDirection = r.Direction.UnitVector();
            //float t = 0.5f * (unitDirection.Y + 1f);
            //return (1 - t) * new Vector3D(1, 1, 1) + t * new Vector3D(0.5f, 0.7f, 1);
            return new Vector3D(0, 0, 0);
        }
    }

    private void SetPixel(int x, int y, Vector3D color)
    {
        var i = width * 4 * y + x * 4;
        changes[width * y + x]++;
        buff[i] += color.X;
        buff[i + 1] += color.Y;
        buff[i + 2] += color.Z;
        buff[i + 3] += 1;
    }
}