#pragma once
#include <string>
#include <cmath>

////////////////////////////////////////////////////////////////////
// Simple 3D vector struct for position, velocity, and rotation
////////////////////////////////////////////////////////////////////
struct gravity_body_vector
{
    double x, y, z;
    gravity_body_vector(double x = 0, double y = 0, double z = 0) : x(x), y(y), z(z) {}
    gravity_body_vector operator+(const gravity_body_vector& other) const { return gravity_body_vector(x + other.x, y + other.y, z + other.z); }
    gravity_body_vector operator-(const gravity_body_vector& other) const { return gravity_body_vector(x - other.x, y - other.y, z - other.z); }
    gravity_body_vector operator*(double scalar) const { return gravity_body_vector(x * scalar, y * scalar, z * scalar); }
    gravity_body_vector operator/(double scalar) const { return gravity_body_vector(x / scalar, y / scalar, z / scalar); } // <-- ADD THIS LINE
    gravity_body_vector& operator+=(const gravity_body_vector& other) { x += other.x; y += other.y; z += other.z; return *this; }
    double length() const { return std::sqrt(x * x + y * y + z * z); }
    double lengthSq() const { return x * x + y * y + z * z; }
    gravity_body_vector normalized() const {
        double len = length();
        if (len == 0) return gravity_body_vector(0, 0, 0);
        return gravity_body_vector(x / len, y / len, z / len);
    }
};

inline gravity_body_vector operator*(double scalar, const gravity_body_vector& v) {
    return gravity_body_vector(v.x * scalar, v.y * scalar, v.z * scalar);
}


class gravity_body
{
    ////////////////////////////////////////////////////////////////////
    // Public Variables 
    ////////////////////////////////////////////////////////////////////
    public:
        // General
        std::string name;                   // Name of the body (optional, for identification)
        double mass;                        // Mass in kilograms
        gravity_body_vector position;       // Position in 3D space (meters)
        gravity_body_vector velocity;       // Velocity in 3D space (meters/second)
        double radius;                      // Radius in meters (optional, for collisions/visualization)
        // Rotation
        gravity_body_vector rotation_axis;  // Axis of rotation (should be normalized)
        double angular_speed;               // Angular speed (radians per second)
        double rotation_angle;              // Current rotation angle (radians)

    ////////////////////////////////////////////////////////////////////
    // Constructor
    ////////////////////////////////////////////////////////////////////
    gravity_body(
        const std::string& name,
        double mass,
        const gravity_body_vector& position,
        const gravity_body_vector& velocity,
        double radius = 0.0,
        const gravity_body_vector& rotation_axis = gravity_body_vector(0, 1, 0),
        double angular_speed = 0.0,
        double rotation_angle = 0.0
    )
        : name(name),
        mass(mass),
        position(position),
        velocity(velocity),
        radius(radius),
        rotation_axis(rotation_axis.normalized()),
        angular_speed(angular_speed),
        rotation_angle(rotation_angle)
    {
    }

    ////////////////////////////////////////////////////////////////////
    // Update position based on velocity and time step (dt in seconds)
    ////////////////////////////////////////////////////////////////////
    void update_position(double dt)
    {
        position += velocity * dt;
    }

    ////////////////////////////////////////////////////////////////////
    // Update rotation based on angular speed and time step (dt in seconds)
    ////////////////////////////////////////////////////////////////////
    void update_rotation(double dt)
    {
        rotation_angle += angular_speed * dt;
        // Keep angle in [0, 2*PI) for convenience
        const double two_pi = 2.0 * 3.14159265358979323846;
        if (rotation_angle >= two_pi) rotation_angle -= two_pi;
        if (rotation_angle < 0) rotation_angle += two_pi;
    }

    ////////////////////////////////////////////////////////////////////
    // Apply a force (in Newtons) to the body, updating velocity
    ////////////////////////////////////////////////////////////////////
    void apply_force(const gravity_body_vector& force, double dt)
    {
        gravity_body_vector acceleration = force * (1.0 / mass);
        velocity += acceleration * dt;
    }

    ////////////////////////////////////////////////////////////////////
    // Calculate gravitational force exerted by another body
    ////////////////////////////////////////////////////////////////////
    gravity_body_vector gravitational_force(const gravity_body& other, double G = 6.67430e-11) const
    {
        gravity_body_vector diff = other.position - position;
        double dist = diff.length();
        if (dist == 0) return gravity_body_vector(0, 0, 0); // Avoid division by zero
        double force_mag = G * mass * other.mass / (dist * dist);
        return diff * (force_mag / dist);
    }

    ////////////////////////////////////////////////////////////////////
    // Calculate time dilation factor at the body's surface (Schwarzschild metric, ignoring rotation)
    ////////////////////////////////////////////////////////////////////
    double time_dilation() const
    {
        const double c = 299792458.0; // Speed of light in m/s
        if (radius == 0) return 1.0; // Avoid division by zero
        double rs = 2 * 6.67430e-11 * mass / (c * c); // Schwarzschild radius
        if (radius <= rs) return 0.0; // Inside event horizon
        return std::sqrt(1.0 - rs / radius);
    }
};
