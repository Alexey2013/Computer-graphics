using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace Game2.Graphics
{
    internal class Texture
    {
        public int ID;

        public Texture(String filepath)
        {
            ID = GL.GenTexture();

            // Активируем текстурный блок 0 и привязываем 
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            // Регулировка

            //повторяем текстуру если она по горизонтали и вертикали координаты текстуры выходят за [0,1]
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);//S-эквивалента U в текстурных кооринатах
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);//T-эквивалента U в текстурных кооринатах

            //при масштабировании цвет ближайшего соседа 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);//уменьшение 
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);//увеличение


            StbImage.stbi_set_flip_vertically_on_load(1);// Переворачиваем текстуру 

            ImageResult Texture = ImageResult.FromStream(File.OpenRead("../../../Textures/" + filepath), ColorComponents.RedGreenBlueAlpha);//загрузка из файла

            // загружаем данные текстуры в графический процессор для отображения
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Texture.Width, Texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Texture.Data);

            Unbind();
        }

        public void Bind() { GL.BindTexture(TextureTarget.Texture2D, ID); }

        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }

        public void Delete() { GL.DeleteTexture(ID); }
    }
}