using OpenTK.Graphics.OpenGL4;

namespace Game2.Graphics
{
    internal class ShaderProgram
    {
        public int ID;

        public ShaderProgram(string vertexShaderFilepath, string fragmentShaderFilepath)
        {
            ID = GL.CreateProgram();
            //вершинный шейдер
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, LoadShaderSource(vertexShaderFilepath));
            GL.CompileShader(vertexShader);


            //фрагментный шейдер
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, LoadShaderSource(fragmentShaderFilepath));//фрагментный шейдер
            GL.CompileShader(fragmentShader);

            // Присоединяем 
            GL.AttachShader(ID, vertexShader);
            GL.AttachShader(ID, fragmentShader);

            // Линкуем программу с OpenGL
            GL.LinkProgram(ID);


            Unbind();

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Bind() {GL.UseProgram(ID);}
        public void Unbind() {GL.UseProgram(0);}
        public void Delete() {GL.DeleteShader(ID);}

        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file: " + e.Message);
            }

            return shaderSource;
        }
    }
}
