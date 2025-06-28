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

        var carTexture = new Texture("car.png");
        var voiture = new Sprite(carTexture)
        {
            Position = new Vector2f(1730, 1120),
            Scale = new Vector2f(0.05f, 0.05f),
            Rotation = 180f
        };

        var view = new View(new FloatRect(0, 0, 800, 600));

        Vector2f velocity = new Vector2f(0f, 0f);
        float acceleration = 0.1f;
        float friction = 0.06f;
        float brakePower = 0.15f;
        float maxSpeed = 4f;
        float rotationSpeed = 2.0f;

        Vector2f[] innerContour = new Vector2f[]
        {
            new Vector2f(1823.1675f, 1092.793f),
            new Vector2f(848.5702f, 1085.793f),
            new Vector2f(844.4715f, 1045.8372f),
            new Vector2f(700.4571f, 1090.1763f),
            new Vector2f(540f, 1090f),
            new Vector2f(481f, 1075f),
            new Vector2f(409f, 1050f),
            new Vector2f(335f, 985),
            new Vector2f(275f, 860),
            new Vector2f(210, 455),
            new Vector2f(165, 445),
            new Vector2f(60, 170),
            new Vector2f(65, 125),
            new Vector2f(350, 70),
            new Vector2f(550, 335),
            new Vector2f(1015, 790),
            new Vector2f(1085, 785),
            new Vector2f(1105, 790),
            new Vector2f(1190, 840),
            new Vector2f(2090, 860),
            new Vector2f(2128, 890),
            new Vector2f(2140, 950),
            new Vector2f(2110, 1015),
            new Vector2f(2060, 1040)
        };

        Vector2f[] outerContour = ComputeOuterContourProperly(innerContour, 50f);

        int i = 0;
        bool hasTouchedLine = false;

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

            bool accelerating = Keyboard.IsKeyPressed(Keyboard.Key.Up);
            bool braking = Keyboard.IsKeyPressed(Keyboard.Key.Down);

            float angleRad = voiture.Rotation * (float)Math.PI / 180f;
            Vector2f forward = new Vector2f((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));

            if (accelerating)
            {
                velocity += forward * acceleration;
            }

            if (braking)
            {
                if (Length(velocity) > brakePower)
                {
                    velocity -= Normalize(velocity) * brakePower;
                }
                else
                {
                    velocity = new Vector2f(0, 0);
                }
            }

            float speed = Length(velocity);
            if (speed > maxSpeed)
            {
                velocity = Normalize(velocity) * maxSpeed;
            }

            if (!accelerating && !braking)
            {
                if (speed > 0)
                {
                    Vector2f frictionForce = Normalize(velocity) * friction;
                    if (Length(frictionForce) > speed)
                        velocity = new Vector2f(0, 0);
                    else
                        velocity -= frictionForce;
                }
            }

            if (speed > 0.1f)
            {
                if (Keyboard.IsKeyPressed(Keyboard.Key.Left))
                    voiture.Rotation -= rotationSpeed;

                if (Keyboard.IsKeyPressed(Keyboard.Key.Right))
                    voiture.Rotation += rotationSpeed;
            }

            voiture.Position += velocity;

            var bounds = voiture.GetGlobalBounds();
            view.Center = voiture.Position + new Vector2f(bounds.Width / 2, bounds.Height / 2);
            window.SetView(view);

            Vector2f topLeft = voiture.Transform.TransformPoint(new Vector2f(0, 0));
            Vector2f topRight = voiture.Transform.TransformPoint(new Vector2f(carTexture.Size.X, 0));
            Vector2f bottomLeft = voiture.Transform.TransformPoint(new Vector2f(0, carTexture.Size.Y));
            Vector2f bottomRight = voiture.Transform.TransformPoint(new Vector2f(carTexture.Size.X, carTexture.Size.Y));
            Vector2f middleTop = voiture.Transform.TransformPoint(new Vector2f(carTexture.Size.X / 2, 0));
            Vector2f middleBottom = voiture.Transform.TransformPoint(new Vector2f(carTexture.Size.X / 2, carTexture.Size.Y));
            Vector2f middleLeft = voiture.Transform.TransformPoint(new Vector2f(0, carTexture.Size.Y / 2));
            Vector2f middleRight = voiture.Transform.TransformPoint(new Vector2f(carTexture.Size.X, carTexture.Size.Y / 2));

            Vector2f[] carPoints = new Vector2f[]
            {
                topLeft, topRight, bottomLeft, bottomRight,
                middleTop, middleBottom, middleLeft, middleRight
            };

            float tolerance = 1f;
            bool touched = false;

            foreach (var point in carPoints)
            {
                for (int j = 0; j < innerContour.Length; j++)
                {
                    Vector2f a = innerContour[j];
                    Vector2f b = innerContour[(j + 1) % innerContour.Length];
                    if (DistancePointToSegment(point, a, b) <= tolerance)
                    {
                        touched = true;
                        break;
                    }
                }
                if (touched) break;

                for (int j = 0; j < outerContour.Length; j++)
                {
                    Vector2f a = outerContour[j];
                    Vector2f b = outerContour[(j + 1) % outerContour.Length];
                    if (DistancePointToSegment(point, a, b) <= tolerance)
                    {
                        touched = true;
                        break;
                    }
                }
                if (touched) break;
            }

            if (touched && !hasTouchedLine)
            {
                i++;
                hasTouchedLine = true;
                Console.WriteLine($"🚨 Collision détectée ! i = {i}");
            }
            else if (!touched)
            {
                hasTouchedLine = false;
            }

            window.Clear(backgroundColor);

            for (int j = 0; j < innerContour.Length; j++)
                DrawLine(window, innerContour[j], innerContour[(j + 1) % innerContour.Length], Color.Red);

            for (int j = 0; j < outerContour.Length; j++)
                DrawLine(window, outerContour[j], outerContour[(j + 1) % outerContour.Length], Color.Red);

            window.Draw(voiture);
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

    static Vector2f[] ComputeOuterContourProperly(Vector2f[] contour, float offset)
    {
        int n = contour.Length;
        Vector2f[] outer = new Vector2f[n];

        float winding = ComputeSignedArea(contour);
        bool isClockwise = winding < 0;

        for (int i = 0; i < n; i++)
        {
            Vector2f prev = contour[(i - 1 + n) % n];
            Vector2f curr = contour[i];
            Vector2f next = contour[(i + 1) % n];

            Vector2f dir1 = Normalize(curr - prev);
            Vector2f dir2 = Normalize(next - curr);

            Vector2f normal1 = new Vector2f(-dir1.Y, dir1.X);
            Vector2f normal2 = new Vector2f(-dir2.Y, dir2.X);

            Vector2f avgNormal = Normalize(normal1 + normal2);

            if (!isClockwise)
                avgNormal *= -1;

            outer[i] = curr + avgNormal * offset;
        }

        return outer;
    }

    static float ComputeSignedArea(Vector2f[] points)
    {
        float area = 0;
        for (int i = 0; i < points.Length; i++)
        {
            Vector2f p1 = points[i];
            Vector2f p2 = points[(i + 1) % points.Length];
            area += (p1.X * p2.Y - p2.X * p1.Y);
        }
        return area / 2f;
    }

    static Vector2f Normalize(Vector2f v)
    {
        float length = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        if (length == 0) return new Vector2f(0, 0);
        return v / length;
    }

    static float Length(Vector2f v)
    {
        return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
    }
}
