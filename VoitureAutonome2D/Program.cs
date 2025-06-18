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
        var voiture = new Sprite(carTexture)
        {
            Position = new Vector2f(100, 400),
            Origin = new Vector2f(carTexture.Size.X / 2, carTexture.Size.Y / 2)
        };

        var view = new View(new FloatRect(0, 0, 800, 600));
        window.SetFramerateLimit(60);

        while (window.IsOpen)
        {
            window.DispatchEvents();
            voiture.Position += new Vector2f(1f, 0);
            view.Center = voiture.Position;
            window.SetView(view);

            window.Clear(Color.Black);

            window.Draw(voiture);
            window.Display();
        }
    }
}
