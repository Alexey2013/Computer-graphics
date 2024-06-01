using OpenTK.Graphics.OpenGL4;

namespace Game2.Graphics
{
    internal class VAO 
    {
        public int ID;
        public VAO()
        {
            // Генерация и привязка
            ID = GL.GenVertexArray();
            GL.BindVertexArray(ID);
        }
        
        public void LinkToVAO(int location, int size, VBO vbo) //связывание vao и vbo
        {
            Bind();
            vbo.Bind();

            GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0);// Настраивается указатель на массив вершинных атрибутов
            //false- нормализация данных(нет)
            //последние два значения это шаг между атрибутами и смещение от начала буфера

            GL.EnableVertexAttribArray(location);//aктивация
            Unbind();
        }

        public void Bind() { GL.BindVertexArray(ID); }
        public void Unbind() { GL.BindVertexArray(0); }
        public void Delete() { GL.DeleteVertexArray(ID); } 
    }
}
