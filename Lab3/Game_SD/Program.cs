using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Lab3
{
    public static class Program
    {
        private static void Main()
        {
            using (var window = new Window(GameWindowSettings.Default,new NativeWindowSettings() { ClientSize = new Vector2i(800, 600)}))
            {
                window.Run();
            }
        }
    }
}