#version 330 core

in vec3 vertexColor; // Color passed from vertex shader
uniform float opacity; // Opacity passed from C#

out vec4 FragColor;

void main()
{
    FragColor = vec4(vertexColor, opacity); // Set the final color with opacity
}
