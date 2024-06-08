using OpenTK.Graphics.OpenGL4;
using Lab3.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;

namespace Lab3
{
    public class Window : GameWindow
    {
        private int vbo;
        private int vao;
        private int ibo;
        private Shader shader;

        private readonly float[] wall =
        {
             -1f, -1f, 0.0f, // Нижний левый угол
             1f, -1f, 0.0f,  // Нижний правый угол
             -1f,  1f, 0.0f, // Верхний левый угол
             1f,  1f, 0.0f   // Верхний правый угол
        };

        private readonly uint[] indices = {
            0, 3, 1,   // Первый треугольник
            0, 3, 2    // Второй треугольник
        };

        // Конструктор
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings){}

        protected override void OnLoad()// загрузка экрана
        {
            base.OnLoad();

            GL.Enable(EnableCap.DepthTest);//тест глубины

            // Генерация и привязка VBO
            vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, wall.Length * sizeof(float), wall, BufferUsageHint.StaticDraw);

            // Генерация и привязка VAO
            vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // Генерация и привязка IBO
            ibo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            // Установка указателя на атрибуты вершин
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // Загрузка и использование шейдера
            shader = new Shader("../../../Shaders/shader.vert", "../../../Shaders/shader.frag");
            shader.Use();
        }

        //отрисовка кадра
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Очистка буфера цвета и глубины
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Использование шейдера
            shader.Use();

            // Привязка VAO
            GL.BindVertexArray(vao);

            // Отрисовка элементов
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            // Обмен буферами
            SwapBuffers();
        }
    }
}