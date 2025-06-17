#version 330 core

layout(location = 0) in vec3 aPosition; // Vertex position
out vec3 vertexColor; // Color that will be passed to the fragment shader

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform vec3 objectColor; // Global color uniform passed from C#

void main()
{
    gl_Position = projection * view * model * vec4(aPosition, 1.0);
    vertexColor = objectColor; // Pass the object color to the fragment shader
}
