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

        Color backgroundColor = new Color(0, 255, 0);

        var trackImage = new Image("track.png");
        var trackTexture = new Texture(trackImage);
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

        Vector2f ligneA = new Vector2f(1823.1675f, 1092.793f);
        Vector2f ligneB = new Vector2f(848.5702f, 1085.793f);
        Vector2f ligneC = new Vector2f(844.4715f, 1045.8372f);
        Vector2f ligneD = new Vector2f(749.4571f, 1070.1763f);
        Vector2f ligneE = new Vector2f(702f, 1091f);
        Vector2f ligneF = new Vector2f(518f, 1096f);

        int i = 0;

        window.KeyPressed += (sender, e) =>
        {
            if (e.Code == Keyboard.Key.M)
            {
                var bounds = voiture.GetGlobalBounds();
                var center = voiture.Position + new Vector2f(bounds.Width / 2, bounds.Height / 2);
                Console.WriteLine($"📍 Position de la voiture : X = {center.X}, Y = {center.Y}");
            }
        };

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

            var bounds = voiture.GetGlobalBounds();
            view.Center = voiture.Position + new Vector2f(bounds.Width / 2, bounds.Height / 2);
            window.SetView(view);

            var carCenter = voiture.Position + new Vector2f(bounds.Width / 2, bounds.Height / 2);

            float d1 = DistancePointToSegment(carCenter, ligneA, ligneB);
            float d2 = DistancePointToSegment(carCenter, ligneB, ligneC);
            float d3 = DistancePointToSegment(carCenter, ligneC, ligneD);
            float d4 = DistancePointToSegment(carCenter, ligneD, ligneE);
            float d5 = DistancePointToSegment(carCenter, ligneE, ligneF);

            if (d1 < 10f || d2 < 10f || d3 < 10f)
            {
                i++;
                Console.WriteLine($"🚨 Ligne franchie ! i = {i}");
            }

            window.Clear(backgroundColor);
            window.Draw(trackSprite);
            window.Draw(voiture);
            DrawLine(window, ligneA, ligneB, Color.Red);
            DrawLine(window, ligneB, ligneC, Color.Red);
            DrawLine(window, ligneC, ligneD, Color.Red);
            DrawLine(window, ligneD, ligneE, Color.Red);
            DrawLine(window, ligneE, ligneF, Color.Red);
            window.Display();
        }
    }

    static float DistancePointToSegment(Vector2f p, Vector2f a, Vector2f b)
    {
        Vector2f ab = b - a;
        Vector2f ap = p - a;
        float ab2 = ab.X * ab.X + ab.Y * ab.Y;
        float t = Math.Max(0, Math.Min(1, (ap.X * ab.X + ap.Y * ab.Y) / ab2));
        Vector2f projection = a + ab * t;
        return (float)Math.Sqrt((p.X - projection.X) * (p.X - projection.X) + (p.Y - projection.Y) * (p.Y - projection.Y));
    }

    static void DrawLine(RenderWindow window, Vector2f start, Vector2f end, Color color)
    {
        Vertex[] line = new Vertex[2];
        line[0] = new Vertex(start, color);
        line[1] = new Vertex(end, color);
        window.Draw(line, PrimitiveType.Lines);
    }
}
