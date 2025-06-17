namespace trajectorx.Library.Struct
{
    // Simple 3D vector struct for position, velocity, and rotation
    public struct GravityBodyVector
    {
        public double X, Y, Z;

        public GravityBodyVector(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static GravityBodyVector operator +(GravityBodyVector a, GravityBodyVector b)
        {
            return new GravityBodyVector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static GravityBodyVector operator -(GravityBodyVector a, GravityBodyVector b)
        {
            return new GravityBodyVector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static GravityBodyVector operator *(GravityBodyVector v, double scalar)
        {
            return new GravityBodyVector(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static GravityBodyVector operator /(GravityBodyVector v, double scalar)
        {
            return new GravityBodyVector(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }

        public static GravityBodyVector operator *(double scalar, GravityBodyVector v)
        {
            return new GravityBodyVector(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double LengthSquared()
        {
            return X * X + Y * Y + Z * Z;
        }

        public GravityBodyVector Normalized()
        {
            double len = Length();
            if (len == 0) return new GravityBodyVector(0, 0, 0);
            return new GravityBodyVector(X / len, Y / len, Z / len);
        }
    }
}
