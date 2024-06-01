using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace Game2.Graphics
{
    internal class VBO// Vertex Buffer Object
    {
        public int ID;  // Идентификатор буфера.

        public VBO(List<Vector3> data)
        {
            ID = GL.GenBuffer();// Генерация 
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);  // Привязка созданного буфера к целевому ArrayBuffer

            // Копирование данных из списка data в буфер, вычисление размера данных
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector3.SizeInBytes, data.ToArray(), BufferUsageHint.StaticDraw);  
            
            Unbind();
        }

        public VBO(List<Vector2> data)
        {
            ID = GL.GenBuffer();  // Генерация 
            GL.BindBuffer(BufferTarget.ArrayBuffer, ID);  // Привязка созданного буфера к целевому ArrayBuffer
            // Копирование данных из списка data в буфер, вычисление размера данных
            GL.BufferData(BufferTarget.ArrayBuffer, data.Count * Vector2.SizeInBytes, data.ToArray(), BufferUsageHint.StaticDraw);
            
            Unbind();
        }
        public void Bind() { GL.BindBuffer(BufferTarget.ArrayBuffer, ID); }
        public void Unbind() { GL.BindBuffer(BufferTarget.ArrayBuffer, 0); }
        public void Delete() { GL.DeleteBuffer(ID); }
    }
}
