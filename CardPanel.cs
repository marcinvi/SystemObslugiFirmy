// Potrzebujesz dodać tę klasę pomocniczą do swojego projektu
// Najlepiej w osobnym pliku, np. UI/Controls/CardPanel.cs
namespace Reklamacje_Dane
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Windows.Forms;

    public class CardPanel : Panel
    {
        public int BorderRadius { get; set; } = 5;

        public CardPanel()
        {
            this.DoubleBuffered = true;
            this.Padding = new Padding(1);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Tło i cień
            using (var path = GetFigurePath(this.ClientRectangle, BorderRadius))
            {
                // Cień
                using (var shadowBrush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                {
                    var shadowRect = this.ClientRectangle;
                    shadowRect.Offset(2, 2);
                    e.Graphics.FillPath(shadowBrush, GetFigurePath(shadowRect, BorderRadius));
                    shadowRect.Offset(1, 1);
                    e.Graphics.FillPath(shadowBrush, GetFigurePath(shadowRect, BorderRadius));
                }

                // Główne tło
                using (var brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // Obramowanie
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath GetFigurePath(Rectangle rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float curveSize = radius * 2F;

            path.StartFigure();
            path.AddArc(rect.X, rect.Y, curveSize, curveSize, 180, 90);
            path.AddArc(rect.Right - curveSize, rect.Y, curveSize, curveSize, 270, 90);
            path.AddArc(rect.Right - curveSize, rect.Bottom - curveSize, curveSize, curveSize, 0, 90);
            path.AddArc(rect.X, rect.Bottom - curveSize, curveSize, curveSize, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}

