using SFML.Graphics;
using SFML.System;
using SFML.Window;

class Program
{
    static void Main()
    {
        var window = new RenderWindow(new VideoMode(800, 600), "Voiture autonome 2D");
        window.Closed += (_, __) => window.Close();

        Color backgroundColor = new Color(128, 128, 128);

        var trackTexture = new Texture("track.png");
        var trackSprite = new Sprite(trackTexture)
        {
            Position = new Vector2f(0, 0),
            Scale = new Vector2f(2f, 2f)  // Grossit le circuit x2
        };

        var carTexture = new Texture("car.png");
        var voiture = new Sprite(carTexture)
        {
            Position = new Vector2f(150, 300)  // Position de départ sur le circuit agrandi
        };

        window.SetFramerateLimit(60);

        var view = new View(new FloatRect(0, 0, 800, 600));

        while (window.IsOpen)
        {
            window.DispatchEvents();


            view.Center = voiture.Position + new Vector2f(voiture.GetGlobalBounds().Width / 2, voiture.GetGlobalBounds().Height / 2);
            window.SetView(view);

            window.Clear(backgroundColor);
            window.Draw(trackSprite);
            window.Draw(voiture);
            window.Display();
        }
    }
}
