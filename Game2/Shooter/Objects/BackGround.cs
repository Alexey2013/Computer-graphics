using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Game2.Graphics;

namespace Game2.Objects
{
    internal class BackGround
    {
        public Vector3 position;
        public List<Vector3> vertices;// Вершины
        public List<Vector2> uv;// Координаты текстуры
        public List<uint> ids;// Индексы

        // OpenGL 
        VAO vao;//состояния, необходимые для привязки и конфигурации буферов вершин.
        VBO vert_vbo;//координаты вершин
        VBO uv_vbo;// текстурные координаты вершин.
        IBO ibo;// индексы для порядка отрисовывания
        Texture texture;//данные текстуры

        public BackGround(Vector3 position_, string texture_name)
        {
            position = position_;

            // Вершины квадрата
            vertices = new List<Vector3>() {
            new Vector3(-40f, 40f, -40f), // Верхний левый угол
            new Vector3(40f, 40f, -40f),  // Верхний правый угол
            new Vector3(40f, -40f, -40f), // Нижний правый угол
            new Vector3(-40f, -40f, -40f) // Нижний левый угол
            };

            // Координаты для текстурирования
            uv = new List<Vector2>()
            {
            new Vector2(0f, 1f), // Верхний левый угол 
            new Vector2(1f, 1f), // Верхний правый угол 
            new Vector2(1f, 0f), // Нижний правый угол 
            new Vector2(0f, 0f)  // Нижний левый угол 
            };

            // Индексы вершин для рисования треугольников
            ids = new List<uint>() 
            {
            0, 1, 2,
            2, 3, 0
            };

            // Инициализация объектов OpenGL
            vao = new VAO();
            vert_vbo = new VBO(vertices);
            uv_vbo = new VBO(uv);
            ibo = new IBO(ids);
            texture = new Texture(texture_name);

            // Связывание и настройка VAO
            vao.Bind();
            vert_vbo.Bind();
            vao.LinkToVAO(0, 3, vert_vbo); 
            uv_vbo.Bind();
            vao.LinkToVAO(1, 2, uv_vbo); 
        }

        public void Render(ShaderProgram program)
        {
            // Привязка 
            program.Bind();
            vao.Bind();
            ibo.Bind();
            texture.Bind();

            GL.DrawElements(PrimitiveType.Triangles, ids.Count, DrawElementsType.UnsignedInt, 0);// Отрисовка
        }

        public void Delete()
        {
            vao.Delete();
            vert_vbo.Delete();
            uv_vbo.Delete();
            ibo.Delete();
            texture.Delete();
        }
    }
}
