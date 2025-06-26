using System;
using trajectorx.Library.Struct;

namespace trajectorx
{
    public class GravityBody
    {
        // Public variables
        public string Name { get; set; }
        public double Mass { get; set; }
        public GravityBodyVector Position { get; set; }
        public GravityBodyVector Velocity { get; set; }
        public double Radius { get; set; }
        public GravityBodyVector RotationAxis { get; set; }
        public double AngularSpeed { get; set; }
        public double RotationAngle { get; set; }
        public Color ObjectColor { get; set; } = Color.LightBlue;

        // Constructor
        public GravityBody(
            string name,
            double mass,
            GravityBodyVector position,
            GravityBodyVector velocity,
            double radius = 0.0,
            GravityBodyVector? rotationAxis = null,  // Nullable type
            double angularSpeed = 0.0,
            double rotationAngle = 0.0, 
            Color? objectColor = null)
        {
            Name = name;
            Mass = mass;
            Position = position;
            Velocity = velocity;
            Radius = radius;
            RotationAxis = rotationAxis ?? new GravityBodyVector(0, 1, 0); // Default to Y-axis if null
            RotationAxis = RotationAxis.Normalized();
            AngularSpeed = angularSpeed;
            RotationAngle = rotationAngle;
            ObjectColor = objectColor ?? Color.LightBlue;
        }

        // Update position based on velocity and time step (dt in seconds)
        public void UpdatePosition(double dt)
        {
            Position += Velocity * dt;
        }

        // Update rotation based on angular speed and time step (dt in seconds)
        public void UpdateRotation(double dt)
        {
            RotationAngle += AngularSpeed * dt;
            const double TwoPi = 2.0 * Math.PI;
            if (RotationAngle >= TwoPi) RotationAngle -= TwoPi;
            if (RotationAngle < 0) RotationAngle += TwoPi;
        }

        // Apply a force (in Newtons) to the body, updating velocity
        public void ApplyForce(GravityBodyVector force, double dt)
        {
            var acceleration = force * (1.0 / Mass);
            Velocity += acceleration * dt;
        }

        // Calculate gravitational force exerted by another body
        public GravityBodyVector GravitationalForce(GravityBody other, double G = 6.67430e-11)
        {
            var diff = other.Position - Position;
            double dist = diff.Length();
            if (dist == 0) return new GravityBodyVector(0, 0, 0); // Avoid division by zero
            double forceMagnitude = G * Mass * other.Mass / (dist * dist);
            return diff * (forceMagnitude / dist);
        }
    }
}
