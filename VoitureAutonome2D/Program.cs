using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Program
{
    static void Main()
    {
        var window = new RenderWindow(new VideoMode(800, 600), "Voiture autonome 2D");
        window.Closed += (_, __) => window.Close();

        var carTexture = new Texture("car.png");
        var voiture = new Sprite(carTexture) {
            Position = new Vector2f(30, 30)
        };

        window.SetFramerateLimit(60);

        while (window.IsOpen)
        {
            window.DispatchEvents();

            voiture.Position += new Vector2f(1f, 0);

            window.Clear(Color.Black);
            window.Draw(voiture);
            window.Display();
        }
    }
}
