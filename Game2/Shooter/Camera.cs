using OpenTK.Mathematics;

namespace Game2
{
    internal class Camera
    {
        private float SCREENWIDTH; 
        private float SCREENHEIGHT;

        public Vector3 position; 

        //вектора для ориентации камеры 
        Vector3 up = Vector3.UnitY;
        Vector3 front = -Vector3.UnitZ; // по стандарту ось z вглубь от экрана
        Vector3 right =  Vector3.UnitX;

        public Camera(float width, float height, Vector3 position)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
            this.position = position;
        }

        public Matrix4 GetViewMatrix()//матрица вида
        {
            return Matrix4.LookAt(position, position + front, up);//позиция камеры, точка, куда она смотрит, и направление вверх

        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), SCREENWIDTH / SCREENHEIGHT, 0.1f, 100.0f); 
        }

    }
}
