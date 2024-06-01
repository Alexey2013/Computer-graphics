using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using Game2.Graphics;


namespace Game2.Objects
{
    internal class Box
    {
        public Vector3 position;//позиция
        public List<Vector3> vertices;//список вершин 
        public List<Vector2> uv; // Список координат для текстурирования куба
        public List<uint> ids; //  индексы вершин 
        public int type;

        // OpenGL 
        VAO vao;//состояния, необходимые для привязки и конфигурации буферов вершин.
        VBO vert_vbo;//координаты вершин
        VBO uv_vbo;// текстурные координаты (UV) вершин.
        IBO ibo;// индексы для порядка отрисовывания
        Texture texture;//данные текстуры

        public float len_x, len_y, len_z; // длины куба

        public Box(Vector3 position_, float len_x_, float len_y_, float len_z_, int type_, string texture_name)
        {
            //установка куба
            position = position_; 
            len_x = len_x_; 
            len_y = len_y_; 
            len_z = len_z_; 
            type = type_; 


            List<Vector3> vertices_without_offset = new List<Vector3>()
        {
            // Передняя грань
            new Vector3(-1f, 1f, 1f), new Vector3(1f, 1f, 1f),
            new Vector3(1f, -1f, 1f), new Vector3(-1f, -1f, 1f),
            //  Правая грань
            new Vector3(1f, 1f, 1f), new Vector3(1f, 1f, -1f),
            new Vector3(1f, -1f, -1f), new Vector3(1f, -1f, 1f),
            // Задняя грань
            new Vector3(1f, 1f, -1f), new Vector3(-1f, 1f, -1f),
            new Vector3(-1f, -1f, -1f), new Vector3(1f, -1f, -1f),
            // Левая грань
            new Vector3(-1f, 1f, -1f), new Vector3(-1f, 1f, 1f),
            new Vector3(-1f, -1f, 1f), new Vector3(-1f, -1f, -1f),
            // Верхняя грань
            new Vector3(-1f, 1f, -1f), new Vector3(1f, 1f, -1f),
            new Vector3(1f, 1f, 1f), new Vector3(-1f, 1f, 1f),
            // Нижняя грань
            new Vector3(-1f, -1f, 1f), new Vector3(1f, -1f, 1f),
            new Vector3(1f, -1f, -1f), new Vector3(-1f, -1f, -1f)
        };

            // Установка координат для текстурирования куба
            uv = new List<Vector2>()
        {
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f),
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f), new Vector2(0f, 0f)
        };

            // Индексы вершин для формирования граней куба
            ids = new List<uint>()
        {
            0, 1, 2, 2, 3, 0,   // Передняя грань
            4, 5, 6, 6, 7, 4,   // Правая грань
            8, 9, 10, 10, 11, 8, // Задняя грань
            12, 13, 14, 14, 15, 12, // Левая грань
            16, 17, 18, 18, 19, 16, // Верхняя грань
            20, 21, 22, 22, 23, 20  // Нижняя грань
        };

            vertices = new List<Vector3>();// Создание списка вершин куба для смещенных вершин

            foreach (var vert in vertices_without_offset)
            {
                // Установка координат вершин с учетом позиции и размеров куба
                vertices.Add(new Vector3(
                    vert.X * len_x + position.X,
                    vert.Y * len_y + position.Y,
                    vert.Z * len_z + position.Z
                ));
            }

            // Создание объектов OpenGL
            vao = new VAO(); 
            vert_vbo = new VBO(vertices); 
            ibo = new IBO(ids);
            uv_vbo = new VBO(uv);
            texture = new Texture(texture_name);

            // Привязка буферов к массиву вершин
            vao.Bind();
            vert_vbo.Bind();
            vao.LinkToVAO(0, 3, vert_vbo); // Привязка буфера вершин к VAO 
            uv_vbo.Bind();
            vao.LinkToVAO(1, 2, uv_vbo); // Привязка буфера UV-координат к VAO
        }

        public void Render(ShaderProgram program)
        {
            //привязка
            program.Bind();
            vao.Bind();
            ibo.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, ids.Count, DrawElementsType.UnsignedInt, 0); // Отрисовка 
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
