using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;

class Program
{
    static void Main()
    {
        var window = new RenderWindow(new VideoMode(800, 600), "Voiture autonome 2D");
        window.Closed += (_, __) => window.Close();
        window.SetFramerateLimit(60);

        Color backgroundColor = new Color(128, 128, 128);

        var trackTexture = new Texture("track.png");
        var trackImageSize = trackTexture.Size;
        var trackSprite = new Sprite(trackTexture)
        {
            Position = new Vector2f(0, 0),
            Scale = new Vector2f(4f, 4f)
        };

        var carTexture = new Texture("car.png");
        var voiture = new Sprite(carTexture)
        {
            Position = new Vector2f(380 * 4, 280 * 4),
            Scale = new Vector2f(0.05f, 0.05f),
            Rotation = 180f
        };

        var view = new View(new FloatRect(0, 0, 800, 600));
        float speed = 2.5f;
        float rotationSpeed = 2f;

        while (window.IsOpen)
        {
            window.DispatchEvents();
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up))
            {
                float angleRad = voiture.Rotation * (float)Math.PI / 180f;
                var direction = new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
                voiture.Position += direction * speed;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                voiture.Rotation -= rotationSpeed;
            if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                voiture.Rotation += rotationSpeed;
            view.Center = voiture.Position + new Vector2f(
                voiture.GetGlobalBounds().Width / 2,
                voiture.GetGlobalBounds().Height / 2);
            window.SetView(view);
            var bounds = voiture.GetGlobalBounds();
            FloatRect circuitBounds = new FloatRect(
                0,
                0,
                trackImageSize.X * 4,
                trackImageSize.Y * 4
            );

            if (!circuitBounds.Contains(bounds.Left, bounds.Top) ||
                !circuitBounds.Contains(bounds.Left + bounds.Width, bounds.Top) ||
                !circuitBounds.Contains(bounds.Left, bounds.Top + bounds.Height) ||
                !circuitBounds.Contains(bounds.Left + bounds.Width, bounds.Top + bounds.Height))
            {
                Console.WriteLine("⚠️ La voiture est sortie de l'image du circuit !");
            }
            window.Clear(backgroundColor);
            window.Draw(trackSprite);
            window.Draw(voiture);
            window.Display();
        }
    }
}
