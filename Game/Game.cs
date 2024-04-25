using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbImageSharp;
using System.Diagnostics;
using System.Reflection;
using System.Transactions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Game
{
    public class Game : GameWindow
    {
        double time;
        int width, height;
        static float BaseStepX = 0.0008f;
        static float BaseStepY = 0.003f;
        static float BaseSpeedY = 0.0000045f;

        float[] gunVertices = {
        -0.6f, -0.2f, -1.7f,
        -0.2f, -0.2f, -1.7f,
        -0.2f, -0.5f, -1.7f,
        -0.6f, -0.5f, -1.7f
        };

        float[] logsVertices = {
        -0.6f, -0.25f, -1.7f,
        -0.45f, -0.25f, -1.7f,
        -0.45f, -0.45f, -1.7f,
        -0.6f, -0.45f, -1.7f
        };

        float[] wallVertices = {
        -1f,1f,-1.7f,
        1f,1f,-1.7f,
        1f,-1f,-1.7f,
        -1f,-1f,-1.7f
        };

        float[] texCoord =
        {
            0.0f, 1.0f,
            1.0f, 1.0f,
            1.0f, 0.0f,
            0.0f, 0.0f
        };

        uint[] indeces =
        {
            0, 1, 2,
            2, 3, 0
        };

        int gunVao;
        int gunVbo;
        int gunEbo;
        int gunTextureID;
        int gunTextureVBO;
        int gunIndecesLength;

        int logsVao;
        int logsVbo;
        int logsEbo;
        int logsTextureID;
        int logsTextureVBO;
        int logsIndecesLength;

        int wallVao;
        int wallVbo;
        int wallEbo;
        int wallTextureID;
        int wallTextureVBO;
        int wallIndecesLength;

        int shaderProgram;
        int vertexShader;
        int fragmentShader;

        bool flight = false;
        float minY = 0f;
        float speedY = BaseSpeedY;
        float startStepY = BaseStepY;
        float stepY = 0f;
        float posY = 0f;

        float startStepX = BaseStepX;
        float stepX = 0f;
        float logsPosX = 1.3f;
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title }) { this.width = width; this.height = height; }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            float oldWidth = this.width, oldHeight = this.height;
            this.width = e.Width;
            this.height = e.Height;
            stepX = 0f;
            startStepX = BaseStepX / oldWidth * this.width;
            startStepY = BaseStepY / oldHeight * this.height;
            speedY = BaseSpeedY / oldHeight * this.height;

        }

        protected override void OnLoad()
        {
            base.OnLoad();

            string gunPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\gun.png";
            string logsPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\logs.png";
            string wallPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\wall.png";

            gunVao = GL.GenVertexArray();
            GL.BindVertexArray(gunVao);

            gunVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, gunVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, gunVertices.Length * sizeof(float), gunVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(gunVao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            gunEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, gunEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(uint), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            gunTextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, gunTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoord.Length * sizeof(float), texCoord, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(gunVao, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            gunTextureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, gunTextureID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult gunImage = ImageResult.FromStream(File.OpenRead(gunPath), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, gunImage.Width, gunImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, gunImage.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);


            logsVao = GL.GenVertexArray();
            GL.BindVertexArray(logsVao);

            logsVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, logsVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, logsVertices.Length * sizeof(float), logsVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(logsVao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            logsEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, logsEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(uint), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            logsTextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, logsTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoord.Length * sizeof(float), texCoord, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(logsVao, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            logsTextureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, logsTextureID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult logsImage = ImageResult.FromStream(File.OpenRead(logsPath), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, logsImage.Width, logsImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, logsImage.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);


            wallVao = GL.GenVertexArray();
            GL.BindVertexArray(wallVao);

            wallVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, wallVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, wallVertices.Length * sizeof(float), wallVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(wallVao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            wallEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, wallEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(uint), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            wallTextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, wallTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoord.Length * sizeof(float), texCoord, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(wallVao, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            wallTextureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, wallTextureID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult wallImage = ImageResult.FromStream(File.OpenRead(wallPath), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, wallImage.Width, wallImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, wallImage.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);


            string vertexPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\shader.vert";
            string fragmentPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\shader.frag";

            shaderProgram = GL.CreateProgram();

            vertexShader = GL.CreateShader(ShaderType.VertexShader);
            string VertexShaderSource = File.ReadAllText(vertexPath);
            GL.ShaderSource(vertexShader, VertexShaderSource);
            GL.CompileShader(vertexShader);

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            string FragmentShaderSource = File.ReadAllText(fragmentPath);
            GL.ShaderSource(fragmentShader, FragmentShaderSource);
            GL.CompileShader(fragmentShader);

            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);

            GL.LinkProgram(shaderProgram);

            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(gunVao);
            GL.DeleteBuffer(gunVbo);
            GL.DeleteBuffer(gunEbo);
            GL.DeleteTexture(gunTextureID);

            GL.DeleteVertexArray(logsVao);
            GL.DeleteBuffer(logsVbo);
            GL.DeleteBuffer(logsEbo);
            GL.DeleteTexture(logsTextureID);

            GL.DeleteVertexArray(wallVao);
            GL.DeleteBuffer(wallVbo);
            GL.DeleteBuffer(wallEbo);
            GL.DeleteTexture(wallTextureID);

            GL.DeleteProgram(shaderProgram);
        }
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            time += args.Time;
            if (time > 0.0001)
            {
                time = 0;
                if (flight)
                {
                    posY += stepY;
                    stepY -= speedY;
                    if (posY <= minY)
                    {
                        posY = 0f;
                        flight = false;
                    }
                }

                if (posY < 0.2f && -0.15f < logsPosX && logsPosX < 0.4f) stepX = 0;

                GL.ClearColor(0.6f, 0.3f, 1f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.UseProgram(shaderProgram);


                GL.BindVertexArray(wallVao);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, wallEbo);
                GL.BindTexture(TextureTarget.Texture2D, wallTextureID);

                Matrix4 wallModel = Matrix4.Identity;
                Matrix4 wallView = Matrix4.Identity;
                Matrix4 wallProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                Matrix4 wallTranslation = Matrix4.CreateTranslation(0f, 0f, 0f);
                wallModel *= wallTranslation;

                int wallModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                int wallViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                int wallProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                GL.UniformMatrix4(wallModelLocation, true, ref wallModel);
                GL.UniformMatrix4(wallViewLocation, true, ref wallView);
                GL.UniformMatrix4(wallProjectionLocation, true, ref wallProjection);

                GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);


                GL.BindVertexArray(gunVao);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, gunEbo);
                GL.BindTexture(TextureTarget.Texture2D, gunTextureID);

                Matrix4 gunModel = Matrix4.Identity;
                Matrix4 gunView = Matrix4.Identity;
                Matrix4 gunProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                Matrix4 gunTranslation = Matrix4.CreateTranslation(0f, posY, 0f);
                gunModel *= gunTranslation;

                int gunModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                int gunViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                int gunProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                GL.UniformMatrix4(gunModelLocation, true, ref gunModel);
                GL.UniformMatrix4(gunViewLocation, true, ref gunView);
                GL.UniformMatrix4(gunProjectionLocation, true, ref gunProjection);

                GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);


                GL.BindVertexArray(logsVao);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, logsEbo);
                GL.BindTexture(TextureTarget.Texture2D, logsTextureID);

                Matrix4 logsModel = Matrix4.Identity;
                Matrix4 logsView = Matrix4.Identity;
                Matrix4 logsProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                Matrix4 logsTranslation = Matrix4.CreateTranslation(logsPosX, 0f, 0f);
                logsModel *= logsTranslation;

                int logsModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                int logsViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                int logsProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                GL.UniformMatrix4(logsModelLocation, true, ref logsModel);
                GL.UniformMatrix4(logsViewLocation, true, ref logsView);
                GL.UniformMatrix4(logsProjectionLocation, true, ref logsProjection);

                GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);

                Context.SwapBuffers();
            }

            base.OnRenderFrame(args);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape)){Close();}
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            {
              
            }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.D))
            {

            }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter))
            {
    
            }
            base.OnUpdateFrame(args);
        }
    }
}
