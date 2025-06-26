public class SphereMesh
{
    public float[] Vertices;
    public float[] Normals;
    public uint[] Indices;

    public SphereMesh(float radius, int sectorCount, int stackCount)
    {
        List<float> verts = new List<float>();
        List<float> norms = new List<float>();
        List<uint> inds = new List<uint>();

        for (int i = 0; i <= stackCount; ++i)
        {
            float stackAngle = (float)Math.PI / 2 - i * (float)Math.PI / stackCount;
            float xy = radius * (float)Math.Cos(stackAngle);
            float z = radius * (float)Math.Sin(stackAngle);

            for (int j = 0; j <= sectorCount; ++j)
            {
                float sectorAngle = j * 2 * (float)Math.PI / sectorCount;
                float x = xy * (float)Math.Cos(sectorAngle);
                float y = xy * (float)Math.Sin(sectorAngle);

                verts.Add(x); verts.Add(y); verts.Add(z);
                OpenTK.Mathematics.Vector3 normal = OpenTK.Mathematics.Vector3.Normalize(new OpenTK.Mathematics.Vector3(x, y, z));
                norms.Add(normal.X); norms.Add(normal.Y); norms.Add(normal.Z);
            }
        }

        for (int i = 0; i < stackCount; ++i)
        {
            int k1 = i * (sectorCount + 1);
            int k2 = k1 + sectorCount + 1;

            for (int j = 0; j < sectorCount; ++j, ++k1, ++k2)
            {
                if (i != 0)
                {
                    inds.Add((uint)k1);
                    inds.Add((uint)k2);
                    inds.Add((uint)k1 + 1);
                }
                if (i != (stackCount - 1))
                {
                    inds.Add((uint)k1 + 1);
                    inds.Add((uint)k2);
                    inds.Add((uint)k2 + 1);
                }
            }
        }

        Vertices = verts.ToArray();
        Normals = norms.ToArray();
        Indices = inds.ToArray();
    }
}
