using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Game2.Graphics;
using Game2.Objects;


namespace Game2
{
    internal class Game : GameWindow
    {
        int width, height;

        BackGround background;

        Box monkey;
        Box target;
        Box boom;

        Sphere banan;

        Camera camera;

        ShaderProgram program;

        bool flight = false;
        bool movingUp = true;

        float angle = 1f;
        double time = 0;

        float startStepX = 0.0008f;
        float startStepY = 0.004f;
        float speedY = 0.000004f;
        float speedX = 0.000009f;

        float minY = -30f;
        float maxX = 90f;

        float stepY = 0f;
        float stepX = 0f;

        float target_x = 12f;
        float target_y = 2f;

        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.width = width;
            this.height = height;
            CenterWindow(new Vector2i(width, height));
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();

            background = new BackGround(new Vector3(0, 10, 0), "wall.jpg");

            banan = new Sphere();

            monkey = new Box(new Vector3(-12f, -1f, -22f), 1f, 1f, 1f, 0, "monkey.jpg");

            target = new Box(new Vector3(target_x, target_y, -22f), 1f, 1f, 1f, 0, "target.png");

            boom = new Box(new Vector3(target_x, target_y, -22f), 1f, 1f, 1f, 0, "boom.png");

            program = new ShaderProgram("Default.vert", "Default.frag");

            camera = new Camera(width, height, new Vector3(0f, 3f, 3f));

            GL.Enable(EnableCap.DepthTest);

        }

        protected override void OnUnload() 
        {
            base.OnUnload();
            background.Delete();
            banan.Delete();
            monkey.Delete();
            program.Delete();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            time += args.Time;
            if (time > 0.0001)
            {
                time = 0;

                if (flight)
                {
                    
                    banan.CentPos.X -= stepX;
                    banan.CentPos.Y += stepY;
                   
                    stepY -= speedY;
                    stepX -= speedX;

                    if (banan.CentPos.Y <= minY || banan.CentPos.X >= maxX)
                    {
                        flight = false;
                        banan.CentPos.X = -12f;
                        banan.CentPos.Y = -1.2f;
                    }
                }

                Matrix4 model = Matrix4.Identity;
                Matrix4 view = camera.GetViewMatrix();
                Matrix4 projection = camera.GetProjectionMatrix();

                int modelLocation = GL.GetUniformLocation(program.ID, "model");
                int viewLocation = GL.GetUniformLocation(program.ID, "view");
                int projectionLocation = GL.GetUniformLocation(program.ID, "projection");

        
                GL.UniformMatrix4(modelLocation, true, ref model);
                GL.UniformMatrix4(viewLocation, true, ref view);
                GL.UniformMatrix4(projectionLocation, true, ref projection);

                monkey.Render(program);

                background.Render(program);

                KeyUpdate();

                model = Matrix4.Identity;   

                Matrix4 translation = Matrix4.CreateTranslation(banan.CentPos); 

                model *= translation;

                GL.UniformMatrix4(modelLocation, true, ref model);
                GL.UniformMatrix4(viewLocation, true, ref view);
                GL.UniformMatrix4(projectionLocation, true, ref projection);

                if (flight)
                {
                    banan.Render(program);
                }

                 model = Matrix4.Identity;
                 view = camera.GetViewMatrix();
                 projection = camera.GetProjectionMatrix();

                 modelLocation = GL.GetUniformLocation(program.ID, "model");
                 viewLocation = GL.GetUniformLocation(program.ID, "view");
                 projectionLocation = GL.GetUniformLocation(program.ID, "projection");

                if (movingUp)
                {
                    if (target_y <= 9f)
                    {
                        target_y += 0.001f;
                    }
                    else
                    {
                        movingUp = false;
                    }
                }

                else
                {
                    if (target_y >= -8f)
                    {
                        target_y -= 0.001f;
                    }
                    else
                    {
                        movingUp = true;
                    }
                }

                translation = Matrix4.CreateTranslation(0,target_y ,0);

                model *= translation;

                GL.UniformMatrix4(modelLocation, true, ref model);
                GL.UniformMatrix4(viewLocation, true, ref view);
                GL.UniformMatrix4(projectionLocation, true, ref projection);
                if (distance_to_target(banan.CentPos) > 2f)
                {
                    target.Render(program);
                }
                else {
                    boom.Render(program);
                }

                Context.SwapBuffers();
            }
            base.OnRenderFrame(args);
        }

        float distance_to_target(Vector3 v)
        {
            float x = (v.X - target_x) * (v.X - target_x) + (v.Y - target_y) * (v.Y - target_y);
            return (float)MathHelper.Sqrt(x);
        }

        protected void KeyUpdate() {

            if (KeyboardState.IsKeyDown(Keys.Up) || KeyboardState.IsKeyDown(Keys.Down)) {
                if (KeyboardState.IsKeyDown(Keys.Up)) { angle += 0.002f; }
                if (KeyboardState.IsKeyDown(Keys.Down)) { angle -= 0.002f; }
            }

            if (KeyboardState.IsKeyDown(Keys.Space)){
                if (!flight)
                {
                    flight = true;
                    stepY = startStepY * angle;
                    stepX = startStepX * angle;
                }
            }

           if (KeyboardState.IsKeyDown(Keys.Escape)) {Close();}
        }
    }
}