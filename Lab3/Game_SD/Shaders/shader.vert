#version 330 core

//location - index of a vertex attribute
layout(location = 0) in vec3 aPosition; 

//variable to pass the vertex position
out vec3 glPosition;

void main(void)
{
    gl_Position = vec4(aPosition, 1.0);    // Convert to a 4-component vector

    //next stage
    glPosition = aPosition;
}
