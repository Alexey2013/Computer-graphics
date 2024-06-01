using OpenTK.Graphics.OpenGL4; 

namespace Game2.Graphics
{
    class IBO //Index Buffer Object
    {
        private int ID;
        public IBO(List<uint> data)
        {
            ID = GL.GenBuffer(); // создание буфера

            // показываем что буфер для  индексного массива
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID);

            GL.BufferData(BufferTarget.ElementArrayBuffer, data.Count * sizeof(uint), data.ToArray(), BufferUsageHint.StaticDraw);// размер буфера в байтах
            //данные только для чтения 
             Unbind();
        }
        public void Bind() { GL.BindBuffer(BufferTarget.ElementArrayBuffer, ID); } //отвязка
        public void Unbind() { GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);} //привязка
        public void Delete() { GL.DeleteBuffer(ID); } //удаление
    }
}

