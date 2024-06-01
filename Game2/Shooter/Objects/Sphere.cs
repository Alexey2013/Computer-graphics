using Game2.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace Game2
{
    internal class Sphere
    {
        public List<Vector3> gameVerts; // Вершины сферы
        public List<Vector2> gameGraphic = new List<Vector2>(); // Текстурные координаты
        public List<uint> gameInd; // Индексы для отрисовки

        //OpenGL 
        VAO gameVAO; // Массив вершин
        VBO gameVertexVBO; // Буфер вершин
        VBO gameGraphicVBO; // Буфер текстурных координат
        IBO gameIBO; // Буфер индексов
        Texture texture; // Текстура для сферы

        public Vector3 CentPos = new Vector3(-12f, -1.2f, -22f);//центр

        public Sphere()
        {
            GenerateSphereForm(1.0f, out gameVerts, out gameInd);
            for (int i = 0; i < gameVerts.Count; i++)
            {
                gameVerts[i].Normalize(); //длина 1, направление не меняется

                // текстурные горизонтальные и вертикальные координаты
                float u = (float)(MathHelper.Atan2(gameVerts[i].X, gameVerts[i].Z) / (2 * MathHelper.Pi) + 0.5f);
                float v = gameVerts[i].Y * 0.5f + 0.5f;

                gameGraphic.Add(new Vector2(u, v));
            }


            //cвязывание и настройка 
            gameVAO = new VAO();
            gameVAO.Bind();

            gameVertexVBO = new VBO(gameVerts);
            gameVertexVBO.Bind();
            gameVAO.LinkToVAO(0, 3, gameVertexVBO);

            gameGraphicVBO = new VBO(gameGraphic);
            gameGraphicVBO.Bind();
            gameVAO.LinkToVAO(1, 2, gameGraphicVBO);

            gameIBO = new IBO(gameInd);

            texture = new Texture("banan.png");

        }

        //переход от сферической системы координат к декратовой 
        private void GenerateSphereForm(float scale, out List<Vector3> vertices, out List<uint> indices) //scale-масштаб
        {
            vertices = new List<Vector3>(); //вершины
            indices = new List<uint>(); //индексы для треугольников

            int horizontal_count = 20; // Количество горизонтальных сегментов 
            int vertical_count = 10; // Количество вертикальных сегментов 

            // Генерация вершин в сферических координатах 
            for (int i = 0; i <= vertical_count; i++)
            {
                float v = (float)i / vertical_count;// Вычисление вертикальной координаты для равномерного деления

                float phi = v * (float)Math.PI;//полярный угол(зенитный)

                for (int j = 0; j <= horizontal_count; j++)
                {
                    // Вычисление горизонтальной координаты 
                    float u = (float)j / horizontal_count;

                    float theta = u * 2f * (float)Math.PI;//азимутальный угол


                    float x = (float)(Math.Cos(theta) * Math.Sin(phi));
                    //cos дает координату x на xy,а sin дает радиус-вектор r проекции точки на сверу xy ->
                    //умножение дает x в сферических координатах


                    float y = (float)(Math.Sin(theta) * Math.Sin(phi));
                    //1-ый sin дает координату y на сфере,а 2-ой sin дает радиус-вектор r проекции точки на сверу xy ->
                    //умножение дает y в сферических координатах

                    float z = (float)Math.Cos(phi);//cos дает координату y в сферических координатах


                    vertices.Add(new Vector3(x, y, z) * scale);// Добавление с учетом масштаба
                }
            }

            // Генерация индексов для формирования треугольников
            for (int i = 0; i < vertical_count; i++)
            {
                for (int j = 0; j < horizontal_count; j++)
                {
                    int first = (i * (horizontal_count + 1)) + j;
                    //+ 1 - количество вершин в каждом горизонтальном кольце на единицу больше, чем количество сегментов
                    int second = first + horizontal_count + 1;

                    // первый треугольник
                    indices.Add((uint)first);
                    indices.Add((uint)second);
                    indices.Add((uint)(first + 1));

                    // второй треугольник
                    indices.Add((uint)second);
                    indices.Add((uint)(second + 1));
                    indices.Add((uint)(first + 1));
                }
            }
        }

        public void Render(ShaderProgram program)
        {
            //привязка
            program.Bind();
            gameVAO.Bind();
            gameIBO.Bind();
            texture.Bind();
            //отрисовка
            GL.DrawElements(PrimitiveType.Triangles, gameInd.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete() //удаление
        {
            gameVAO.Delete();
            gameVertexVBO.Delete();
            gameGraphicVBO.Delete();
            gameIBO.Delete();
            texture.Delete();
        }
    }
}
