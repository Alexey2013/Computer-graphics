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
        static float BaseStepX = 0.00008f;
        static float BaseStepY = 0.0015f;
        static float BaseSpeedY = 0.000004f;
        static float BaseSpeedX = 0.000004f;

        float[] gunVertices = {
        -0.6f, -0.2f, -1.7f,
        -0.2f, -0.2f, -1.7f,
        -0.2f, -0.5f, -1.7f,
        -0.6f, -0.5f, -1.7f
        };

        float[] targetVertices = {
        -0.6f, -0.2f, -1.7f,
        -0.2f, -0.2f, -1.7f,
        -0.2f, -0.5f, -1.7f,
        -0.6f, -0.5f, -1.7f
        };

        float[] wallVertices = {
        -1f,1f,-1.7f,
        1f,1f,-1.7f,
        1f,-1f,-1.7f,
        -1f,-1f,-1.7f
        };

        float[] ballVertices = {
        -0.6f, -0.2f, -1.7f,
        -0.2f, -0.2f, -1.7f,
        -0.2f, -0.5f, -1.7f,
        -0.6f, -0.5f, -1.7f
        };


        float[] boomVertices = {
        -0.6f, -0.2f, -1.7f,
        -0.2f, -0.2f, -1.7f,
        -0.2f, -0.5f, -1.7f,
        -0.6f, -0.5f, -1.7f
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

        int targetVao;
        int targetVbo;
        int targetEbo;
        int targetTextureID;
        int targetTextureVBO;

        int wallVao;
        int wallVbo;
        int wallEbo;
        int wallTextureID;
        int wallTextureVBO;

        int ballVao;
        int ballVbo;
        int ballEbo;
        int ballTextureID;
        int ballTextureVBO;
        int ballIndecesLength;

        int boomVao;
        int boomVbo;
        int boomEbo;
        int boomTextureID;
        int boomTextureVBO;
        int boomIndecesLength;


        int shaderProgram;
        int vertexShader;
        int fragmentShader;


        float angle=1f;


        bool flight = false;
        float minY = -2f;
        float maxX = 3f;

        float speedY = BaseSpeedY;
        float startStepY = BaseStepY;
        float stepY = 0f;

        float startStepX = BaseStepX;
        float speedX = BaseSpeedX;
        float stepX = 0f;

        float ballPosX = -0.2f;
        float ballPosY = 0.0f;

        float targetPosX = 1.1f;
        float targetPosY = 0.0f;
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
            string targetPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\target.png";
            string wallPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\wall.png";
            string ballPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\cannonball.png";
            string boomPath = "C:\\Users\\alexe\\Desktop\\Game\\Game\\boom.png";

            //gun
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
            //target
            targetVao = GL.GenVertexArray();
            GL.BindVertexArray(targetVao);

            targetVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, targetVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, targetVertices.Length * sizeof(float), targetVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(targetVao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            targetEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, targetEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(uint), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            targetTextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, targetTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoord.Length * sizeof(float), texCoord, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(targetVao, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            targetTextureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, targetTextureID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult targetImage = ImageResult.FromStream(File.OpenRead(targetPath), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, targetImage.Width, targetImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, targetImage.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            //wall
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

            //ball
            ballVao = GL.GenVertexArray();
            GL.BindVertexArray(ballVao);

            ballVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ballVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, ballVertices.Length * sizeof(float), ballVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(ballVao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            ballEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ballEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(uint), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            ballTextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ballTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoord.Length * sizeof(float), texCoord, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(ballVao, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            ballTextureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ballTextureID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult ballImage = ImageResult.FromStream(File.OpenRead(ballPath), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ballImage.Width, ballImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ballImage.Data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);

            GL.BindTexture(TextureTarget.Texture2D, 0);

            //boom
            boomVao = GL.GenVertexArray();
            GL.BindVertexArray(boomVao);

            boomVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, boomVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, boomVertices.Length * sizeof(float), boomVertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(boomVao, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            boomEbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, boomEbo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indeces.Length * sizeof(uint), indeces, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            boomTextureVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, boomTextureVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, texCoord.Length * sizeof(float), texCoord, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexArrayAttrib(boomVao, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindVertexArray(0);

            boomTextureID = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, boomTextureID);

            StbImage.stbi_set_flip_vertically_on_load(1);

            ImageResult boomImage = ImageResult.FromStream(File.OpenRead(boomPath), ColorComponents.RedGreenBlueAlpha);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, boomImage.Width, boomImage.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, boomImage.Data);

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
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape)) { Close(); }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.W))
            {

                if (!flight && angle <= 90) { angle += 0.0005f; }
            }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.S))
            {

                if (!flight && angle <= 90) { angle -= 0.0005f; }
            }

            if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Enter))
            {

                if (!flight)
                {
                    flight = true;
                    stepY = startStepY * angle;
                    stepX = startStepX * angle;
                }
            }
            base.OnUpdateFrame(args);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            GL.DeleteVertexArray(gunVao);
            GL.DeleteBuffer(gunVbo);
            GL.DeleteBuffer(gunEbo);
            GL.DeleteTexture(gunTextureID);

            GL.DeleteVertexArray(targetVao);
            GL.DeleteBuffer(targetVbo);
            GL.DeleteBuffer(targetEbo);
            GL.DeleteTexture(targetTextureID);

            GL.DeleteVertexArray(wallVao);
            GL.DeleteBuffer(wallVbo);
            GL.DeleteBuffer(wallEbo);
            GL.DeleteTexture(wallTextureID);


            GL.DeleteVertexArray(ballVao);
            GL.DeleteBuffer(ballVbo);
            GL.DeleteBuffer(ballEbo);
            GL.DeleteTexture(ballTextureID);


            GL.DeleteVertexArray(boomVao);
            GL.DeleteBuffer(boomVbo);
            GL.DeleteBuffer(boomEbo);
            GL.DeleteTexture(boomTextureID);

            GL.DeleteProgram(shaderProgram);
        }
        protected override void OnRenderFrame(FrameEventArgs args) {
            time += args.Time;
            if (time > 0.0001)
            {
                time = 0;
                if (flight)
                {
                    ballPosY += stepY;
                    ballPosX -= stepX;
                    stepY -= speedY;
                    stepX -= speedX;
                    if (ballPosY <= minY || ballPosX>=maxX)
                    {
                        ballPosY = 0f;
                        ballPosX = 0.0f;
                        flight = false;
                    }
                }
                GL.ClearColor(0.6f, 0.3f, 1f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.UseProgram(shaderProgram);

                //wall
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

                //gun
                GL.BindVertexArray(gunVao);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, gunEbo);
                GL.BindTexture(TextureTarget.Texture2D, gunTextureID);

                Matrix4 gunModel = Matrix4.Identity;
                Matrix4 gunView = Matrix4.Identity;
                Matrix4 gunProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                Matrix4 gunTranslation = Matrix4.CreateTranslation(-0.45f, 0f, 0f);
                gunModel *= gunTranslation;

                int gunModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                int gunViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                int gunProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                GL.UniformMatrix4(gunModelLocation, true, ref gunModel);
                GL.UniformMatrix4(gunViewLocation, true, ref gunView);
                GL.UniformMatrix4(gunProjectionLocation, true, ref gunProjection);

                GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);

                //target
                GL.BindVertexArray(targetVao);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, targetEbo);
                GL.BindTexture(TextureTarget.Texture2D, targetTextureID);

                Matrix4 targetModel = Matrix4.Identity;
                Matrix4 targetView = Matrix4.Identity;
                Matrix4 targetProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                Matrix4 targetTranslation = Matrix4.CreateTranslation(targetPosX, targetPosY, 0f);
                targetModel *= targetTranslation;

                int targetModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                int targetViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                int targetProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                GL.UniformMatrix4(targetModelLocation, true, ref targetModel);
                GL.UniformMatrix4(targetViewLocation, true, ref targetView);
                GL.UniformMatrix4(targetProjectionLocation, true, ref targetProjection);

                GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);

                //ball
                if (flight)
                {
                    GL.BindVertexArray(ballVao);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, ballEbo);
                    GL.BindTexture(TextureTarget.Texture2D, ballTextureID);

                    Matrix4 ballModel = Matrix4.Identity;
                    Matrix4 ballView = Matrix4.Identity;
                    Matrix4 ballProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                    Matrix4 ballTranslation = Matrix4.CreateTranslation(ballPosX, ballPosY + 0.1f, 0f);
                    ballModel *= ballTranslation;

                    int ballModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                    int ballViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                    int ballProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                    GL.UniformMatrix4(ballModelLocation, true, ref ballModel);
                    GL.UniformMatrix4(ballViewLocation, true, ref ballView);
                    GL.UniformMatrix4(ballProjectionLocation, true, ref ballProjection);

                    GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);
                }

                if (Math.Abs(ballPosX-targetPosX)<0.15f && Math.Abs(ballPosY - targetPosY) < 0.15f)
                {
                    GL.BindVertexArray(boomVao);
                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, boomEbo);
                    GL.BindTexture(TextureTarget.Texture2D, boomTextureID);

                    Matrix4 boomModel = Matrix4.CreateTranslation(targetPosX, targetPosY, 0f);
                    Matrix4 boomView = Matrix4.Identity; 
                    Matrix4 boomProjection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), width / height, 0.1f, 100.0f);

                    int boomModelLocation = GL.GetUniformLocation(shaderProgram, "model");
                    int boomViewLocation = GL.GetUniformLocation(shaderProgram, "view");
                    int boomProjectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                    GL.UniformMatrix4(boomModelLocation, true, ref boomModel);
                    GL.UniformMatrix4(boomViewLocation, true, ref boomView);
                    GL.UniformMatrix4(boomProjectionLocation, true, ref boomProjection);

                    GL.DrawElements(PrimitiveType.Triangles, indeces.Length, DrawElementsType.UnsignedInt, 0);
                }

                Context.SwapBuffers();

            }

            base.OnRenderFrame(args);
        }
    }
}
