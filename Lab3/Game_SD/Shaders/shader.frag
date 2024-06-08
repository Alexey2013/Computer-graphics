#version 330 core

out vec4 outputColor;
in vec3 glPosition;

#define EPSILON 0.001
#define BIG 1000000.0

//reflection types:
const int DIFFUSE = 1;
const int MIRROR_REFLECTION = 2;

/*** DATA STRUCTURES ***/

struct SCamera {
    vec3 Position;
    vec3 View; //direction
    vec3 Up; 
    vec3 Side; //to calculate directions of rays 
    vec2 Scale;// for projection
};
SCamera uCamera;

struct SRay {
    vec3 Origin;
    vec3 Direction;
};

struct SLight
{
    vec3 Position;
};
SLight light;

struct SSphere
{
    vec3 Center;
    float Radius;
    int MaterialIdx;
};
SSphere spheres[3];

struct STriangle
{
    vec3 v1;
    vec3 v2;
    vec3 v3;
    int MaterialIdx;
};
STriangle triangles[12];

struct SIntersection
{
    float Time;
    vec3 Point;
    vec3 Normal;
    vec3 Color;

    vec4 LightCoeffs; // ambient, diffuse, specular, specular shininess
    float ReflectionCoef;// 0 - non-reflection, 1 - mirror
    float RefractionCoef; // from physic
    int MaterialType;
};

struct SMaterial
{
    vec3 Color;

    vec4 LightCoeffs; // ambient, diffuse, specular, specular shininess
    float ReflectionCoef;// 0 - non-reflection, 1 - mirror
    float RefractionCoef; // from physic
    int MaterialType;
};
SMaterial materials[7];

struct STracingRay
{
    SRay ray;
    float contribution;//percent
    int depth;//how many times a ray has been reflected or refracted
};

/*** FUNCTIONS ***/

SRay GenerateRay()
{
    vec2 coords = glPosition.xy * uCamera.Scale;//used to shift ray
    vec3 direction = uCamera.View + uCamera.Side * coords.x + uCamera.Up * coords.y;
    return SRay(uCamera.Position, normalize(direction) );
}

SCamera initializeDefaultCamera()
{
	SCamera camera;
	camera.Position = vec3(0.0, -1.0, -8.0);
	camera.View = vec3(0.0, 0.0, 1.0);
	camera.Up = vec3(0.0, 1.0, 0.0);
	camera.Side = vec3(1.0, 0.0, 0.0);
	camera.Scale = vec2(1.0);
	return camera;
}

void initializeDefaultScene(){
    float size = 10.0;

    /* left wall */
    triangles[0].v1 = vec3(-size, -size, -size);
    triangles[0].v2 = vec3(-size, size, size);
    triangles[0].v3 = vec3(-size, size, -size);
    triangles[0].MaterialIdx = 3;

    triangles[1].v1 = vec3(-size, -size, -size);
    triangles[1].v2 = vec3(-size, -size, size);
    triangles[1].v3 = vec3(-size, size, size);
    triangles[1].MaterialIdx = 3;

    /* back wall */
    triangles[2].v1 = vec3(-size, -size, size);
    triangles[2].v2 = vec3(size, -size, size);
    triangles[2].v3 = vec3(-size, size, size);
    triangles[2].MaterialIdx = 5;

    triangles[3].v1 = vec3(size, size, size);
    triangles[3].v2 = vec3(-size, size, size);
    triangles[3].v3 = vec3(size, -size, size);
    triangles[3].MaterialIdx = 5;

    /* right wall */
    triangles[4].v1 = vec3(size, size, size);
    triangles[4].v2 = vec3(size, -size, size);
    triangles[4].v3 = vec3(size, size, -size);
    triangles[4].MaterialIdx = 4;

    triangles[5].v1 = vec3(size, size, -size);
    triangles[5].v2 = vec3(size, -size, size);
    triangles[5].v3 = vec3(size, -size, -size);
    triangles[5].MaterialIdx = 4;

    /* down wall */
    triangles[6].v1 = vec3(-size, -size, -size);
    triangles[6].v2 = vec3(size, -size, size);
    triangles[6].v3 = vec3(-size, -size, size);
    triangles[6].MaterialIdx = 5;

    triangles[7].v1 = vec3(-size, -size, -size);
    triangles[7].v2 = vec3(size, -size, -size);
    triangles[7].v3 = vec3(size, -size, size);
    triangles[7].MaterialIdx = 5;

    /* up wall */
    triangles[8].v1 = vec3(-size, size, -size);
    triangles[8].v2 = vec3(-size, size, size);
    triangles[8].v3 = vec3(size, size, size);
    triangles[8].MaterialIdx = 5;

    triangles[9].v1 = vec3(-size, size, -size);
    triangles[9].v2 = vec3(size, size, size);
    triangles[9].v3 = vec3(size, size, -size);
    triangles[9].MaterialIdx = 5;

    /* front wall */
    triangles[10].v1 = vec3(-size, -size, -size);
    triangles[10].v2 = vec3(size, -size, -size);
    triangles[10].v3 = vec3(-size, size, -size);
    triangles[10].MaterialIdx = 5;

    triangles[11].v1 = vec3(size, size, -size);
    triangles[11].v2 = vec3(-size, size, -size);
    triangles[11].v3 = vec3(size, -size, -size);
    triangles[11].MaterialIdx = 5;

    /* spheres */
    spheres[0].Center = vec3(-2.0, -1.0, -2.0);
    spheres[0].Radius = 2;
    spheres[0].MaterialIdx = 0;

    spheres[1].Center = vec3(2.0, 1.0, 1.0);
    spheres[1].Radius = 1.0;
    spheres[1].MaterialIdx = 1;

    spheres[2].Center = vec3(2.0, -3.0, 1.0);
    spheres[2].Radius = 1.0;
    spheres[2].MaterialIdx = 2;
}

bool IntersectSphere (SSphere sphere, SRay ray, float start, float final, out float time )
{
    //ray P=O+t*D 
    //sphere ||P-C||^2=R^2
    //At^2+Bt+C=0, t-time

    //A=D*D
    //B=2*(O*D)
    //C=O*O-R^2

    ray.Origin -= sphere.Center; //centrt of sphere in (0,0,0)

    float A = dot ( ray.Direction, ray.Direction );//equal to 1 or length squared

    float B = dot ( ray.Direction, ray.Origin );
    float C = dot ( ray.Origin, ray.Origin ) - sphere.Radius * sphere.Radius;

    float D = B * B - A * C; //no 2 because of B

    if ( D > 0.0 )
    {
        D = sqrt ( D );
        float t1 = ( -B - D ) / A;
        float t2 = ( -B + D ) / A;

        if(t1 < 0 && t2 < 0) //both roots are less than zero 
        return false;

        if(min(t1, t2) < 0)
        {
            time = max(t1,t2);
            return true;
        }

        time = min(t1, t2);
        return true;
    }
    return false;
}

bool IntersectTriangle(SRay ray, vec3 v1, vec3 v2, vec3 v3, out float time )
{
    //ray P=O+t*D 
    //plane N*(P-V(1|2|3))=0
    time = -1;
  
    vec3 A = v2 - v1;
    vec3 B = v3 - v1;

    //new normal vector is cross product of two vectors
    vec3 N = cross(A, B);   

    float N_dot_Ray = dot(N, ray.Direction); //how much the direction is same to N

    if (abs(N_dot_Ray) < 0.001) //vector is almost parallel to the plane of the triangle.
        return false;
    
    float lenght = dot(N, v1); // lenght from the origin to the plane.

    float t = -(dot(N, ray.Origin) - lenght) / N_dot_Ray; //t from  P=O+t*D 

    if (t < 0)
        return false;

    vec3 P = ray.Origin + t * ray.Direction;//point

    //first side
    vec3 side1 = v2 - v1; //side of triangle
    vec3 VP1 = P - v1;//vector of point 1
    vec3 C = cross(side1, VP1);

    if (dot(N, C) < 0) //C is directed in the opposite direction
        return false;

    //second side
    vec3 side2 = v3 - v2;
    vec3 VP2 = P - v2;

    C = cross(side2, VP2);
    if (dot(N, C) < 0)
        return false;

    //third side
    vec3 side3 = v1 - v3;
    vec3 VP3 = P - v3;

    C = cross(side3, VP3);
    if (dot(N, C) < 0)
        return false;

    time = t;
    return true;
}

void initializeDefaultLightMaterials(out SLight light)
{
    light.Position = vec3(0.0, 1.0, -4.5f);
    vec4 lightCoefs = vec4(0.4, 0.5, 0.0, 512.0);

    //big sphere
    materials[0].Color = vec3(1.0, 1.0, 1.0);
    materials[0].LightCoeffs = vec4(lightCoefs);
    materials[0].ReflectionCoef = 0.5;
    materials[0].RefractionCoef = 1.0;
    materials[0].MaterialType = MIRROR_REFLECTION;
    
    // sphere 2
    materials[1].Color = vec3(1.0, 2.0, 1.0);
    materials[1].LightCoeffs = vec4(lightCoefs);
    materials[1].ReflectionCoef = 0.5;
    materials[1].RefractionCoef = 1.0;
    materials[1].MaterialType = MIRROR_REFLECTION;

    // sphere 3
    materials[2].Color = vec3(2.0, 2.0, 1.0);
    materials[2].LightCoeffs = vec4(lightCoefs);
    materials[2].ReflectionCoef = 0.5;
    materials[2].RefractionCoef = 1.0;
    materials[2].MaterialType = DIFFUSE;
    
    //left wall
    materials[3].Color = vec3(2.0, 1.0, 1.0);
    materials[3].LightCoeffs = vec4(lightCoefs);
    materials[3].ReflectionCoef = 0.5;
    materials[3].RefractionCoef = 1.0;
    materials[3].MaterialType = DIFFUSE;

    //right wall
    materials[4].Color = vec3(1.0, 1.0, 2.0);
    materials[4].LightCoeffs = vec4(lightCoefs);
    materials[4].ReflectionCoef = 0.5;
    materials[4].RefractionCoef = 1.0;
    materials[4].MaterialType = DIFFUSE;

    // white
    materials[5].Color = vec3(1.0, 1.0, 1.0);  
    materials[5].LightCoeffs = vec4(lightCoefs);
    materials[5].ReflectionCoef = 0.0;  
    materials[5].RefractionCoef = 1.0;  
    materials[5].MaterialType = DIFFUSE;
}



bool Raytrace_func(SRay ray, SSphere spheres[3], STriangle triangles[12], SMaterial materials[7], float start, float final, inout SIntersection intersect)
{
    bool result = false;
    float test = start;
    intersect.Time = final;

      for (int i = 0; i < 3; ++i) {
         SSphere sphere = spheres[i];
            if( IntersectSphere (sphere, ray, start, final, test ) && test < intersect.Time )
        {
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize ( intersect.Point - spheres[i].Center );
            intersect.Color = materials[i].Color;
            intersect.LightCoeffs = materials[i].LightCoeffs;
            intersect.ReflectionCoef = materials[i].ReflectionCoef;
            intersect.RefractionCoef = materials[i].RefractionCoef;
            intersect.MaterialType = materials[i].MaterialType;
            result = true;
        }
    } 
    //calculate intersect with triangles
    for(int i = 0; i < 12; i++)
    {
        STriangle triangle = triangles[i];
        if(IntersectTriangle(ray, triangle.v1, triangle.v2, triangle.v3, test) && test < intersect.Time)
        {
			int numMat = triangles[i].MaterialIdx;
            intersect.Time = test;
            intersect.Point = ray.Origin + ray.Direction * test;
            intersect.Normal = normalize(cross(triangle.v1 - triangle.v2, triangle.v3 - triangle.v2));
            intersect.Color = materials[numMat].Color;
            intersect.LightCoeffs = materials[numMat].LightCoeffs;
            intersect.ReflectionCoef = materials[numMat].ReflectionCoef;
            intersect.RefractionCoef = materials[numMat].RefractionCoef;
            intersect.MaterialType = materials[numMat].MaterialType;
            result = true;
        }
    }
    return result;
}


vec3 Phong(SIntersection intersect, SLight currLight, float shadowFactor)
{
    vec3 light = normalize ( currLight.Position - intersect.Point );
    float diffuse = max(dot(light, intersect.Normal), 0.0);
    vec3 view = normalize(uCamera.Position - intersect.Point);
    vec3 reflected= reflect( -view, intersect.Normal );
    float specular = pow(max(dot(reflected, light), 0.0), intersect.LightCoeffs.w);

    vec3 ambient = intersect.LightCoeffs.x * intersect.Color;
    vec3 diffuseComponent = intersect.LightCoeffs.y * diffuse * intersect.Color * shadowFactor;
    vec3 specularComponent = intersect.LightCoeffs.z * specular * intersect.Color * shadowFactor;

    return ambient + diffuseComponent + specularComponent;
}


float Shadow(SLight currLight, SIntersection intersect)
{
    float shadowing = 1.0; //1 - no shading

    vec3 direction = normalize(currLight.Position - intersect.Point);

    float distanceLight = distance(currLight.Position, intersect.Point);

    SRay shadowRay;
    shadowRay.Origin = intersect.Point + direction * EPSILON;
    shadowRay.Direction =direction;

    SIntersection shadowIntersect;
    shadowIntersect.Time = BIG;

    bool raytr_res = Raytrace_func(shadowRay, spheres, triangles, materials, 0.0f, distanceLight, shadowIntersect);
    if(raytr_res)
    {
        shadowing = 0.0;
    }
    return shadowing;
}



/***  STACK ***/
const int MAX_STACK_SIZE = 10;
const int MAX_TRACE_DEPTH = 8;
STracingRay stack[MAX_STACK_SIZE];
int stack_size = 0;

bool push(STracingRay newRay)
{
    if(stack_size < MAX_STACK_SIZE - 1 && newRay.depth < MAX_TRACE_DEPTH)
    {
        stack[stack_size] = newRay;
        stack_size++;
        return true;
    }
    return false;
}

bool isEmpty()
{
    return (stack_size < 0);
}

STracingRay pop()
{
    stack_size--;
    return stack[stack_size];
}

/*** ***/

void main(void)
{
   	float start, final;

    uCamera = initializeDefaultCamera();
    SRay ray = GenerateRay();

    vec3 resultColor = vec3(0.0,0.0,0.0); //black

    SIntersection intersect;
    intersect.Time = BIG;

    initializeDefaultScene();
    initializeDefaultLightMaterials(light);

    STracingRay trRay = STracingRay(ray, 1, 0);
    push(trRay);

    while(!isEmpty()){
        STracingRay trRay = pop();
        ray = trRay.ray;
        SIntersection intersect;
        intersect.Time = BIG;
        start = 0;
        final = BIG;
        
        if (Raytrace_func(ray, spheres, triangles, materials, start, final, intersect)) //if intersects an object
        {
            switch(intersect.MaterialType) {
            case DIFFUSE: //only shadow and phong
            {

                float shadowing = Shadow(light, intersect);
                resultColor += trRay.contribution * Phong ( intersect, light, shadowing );
                break;
            }

            case MIRROR_REFLECTION:
            {
                if(intersect.ReflectionCoef < 1) //like DIFFUSE
                {
                    float contribution = trRay.contribution * (1 - intersect.ReflectionCoef);
                    float shadowing = Shadow(light, intersect);
                    resultColor += contribution * Phong(intersect, light, shadowing);
                }

                vec3 reflectDirection = reflect(ray.Direction, intersect.Normal); //direction

                float contribution = trRay.contribution * intersect.ReflectionCoef;//Calculation contribution
                
                //Creating a new reflected ray
                STracingRay reflectRay = STracingRay(
                SRay(intersect.Point + reflectDirection * EPSILON, reflectDirection),
                contribution, trRay.depth + 1);

                push(reflectRay);
                break;
            }

            }
        }
    }
    outputColor = vec4(resultColor, 1.0);
}