using OpenTK.Graphics.OpenGL4;

namespace Lab3.Common
{
    public class Shader
    {
        public int ID;

        public Shader(string vertPath, string fragPath)
        {
            ID = GL.CreateProgram();

            // вершинный шейдер
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, File.ReadAllText(vertPath));
            GL.CompileShader(vertexShader);

            //фрагментный шейдер
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, File.ReadAllText(fragPath));
            GL.CompileShader(fragmentShader);

            // Присоединяем 
            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);

            // Линкуем программу с OpenGL
            GL.LinkProgram(ID);

            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
        }

        // Метод для активации программы шейдера.
        public void Use()
        {
            GL.UseProgram(ID);
        }
    }
}