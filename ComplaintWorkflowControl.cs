// ############################################################################
// Plik: ComplaintWorkflowControl.cs (WERSJA OSTATECZNA - NAPRAWIONY BŁĄD CS1061)
// ############################################################################

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Reklamacje_Dane
{
    // ZMIANA: Dziedziczymy po UserControl zamiast Control, aby naprawić błąd AutoScaleMode
    public partial class ComplaintWorkflowControl : UserControl
    {
        private List<string> _steps = new List<string>
        {
            "Nowe", "Weryfikacja", "Logistyka", "Serwis", "Decyzja", "Zakończone"
        };

        private int _currentStepIndex = 0;

        public int CurrentStepIndex
        {
            get => _currentStepIndex;
            set
            {
                if (value >= 0 && value < _steps.Count)
                {
                    _currentStepIndex = value;
                    Invalidate(); // Wymusza przerysowanie kontrolki
                }
            }
        }

        public ComplaintWorkflowControl()
        {
            this.Height = 60;
            this.Dock = DockStyle.Top;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.ResizeRedraw, true);
        }

        // Metoda do ustawiania statusu na podstawie tekstu z bazy
        public void SetStatus(string statusText)
        {
            if (string.IsNullOrEmpty(statusText))
            {
                CurrentStepIndex = 0;
                return;
            }

            // Logika mapowania statusów
            if (statusText.Contains("Nowe") || statusText.Contains("Zgłoszone")) CurrentStepIndex = 0;
            else if (statusText.Contains("Weryfikacja") || statusText.Contains("Oczekuje")) CurrentStepIndex = 1;
            else if (statusText.Contains("transport") || statusText.Contains("Kurier") || statusText.Contains("W drodze")) CurrentStepIndex = 2;
            else if (statusText.Contains("Serwis") || statusText.Contains("Producent")) CurrentStepIndex = 3;
            else if (statusText.Contains("Decyzja")) CurrentStepIndex = 4;
            else if (statusText.Contains("Zamknięte") || statusText.Contains("Zakończone") || statusText.Contains("Odrzucone")) CurrentStepIndex = 5;
            else CurrentStepIndex = 0;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Czyścimy tło (pobieramy kolor rodzica, żeby wyglądało naturalnie)
            e.Graphics.Clear(this.Parent?.BackColor ?? Color.White);

            if (_steps.Count == 0) return;

            int stepWidth = this.Width / _steps.Count;
            int circleSize = 24;
            int lineY = this.Height / 2 - 10;

            for (int i = 0; i < _steps.Count; i++)
            {
                int x = i * stepWidth + (stepWidth / 2);

                // 1. Rysowanie linii łączącej
                if (i < _steps.Count - 1)
                {
                    int xNext = (i + 1) * stepWidth + (stepWidth / 2);
                    Color lineColor = i < _currentStepIndex ? Color.SeaGreen : Color.LightGray;

                    using (var pen = new Pen(lineColor, 4))
                    {
                        e.Graphics.DrawLine(pen, x, lineY, xNext, lineY);
                    }
                }

                // 2. Rysowanie kółka
                Color circleColor;
                if (i < _currentStepIndex) circleColor = Color.SeaGreen;
                else if (i == _currentStepIndex) circleColor = Color.DodgerBlue;
                else circleColor = Color.LightGray;

                Rectangle circleRect = new Rectangle(x - circleSize / 2, lineY - circleSize / 2, circleSize, circleSize);

                using (var brush = new SolidBrush(circleColor))
                {
                    e.Graphics.FillEllipse(brush, circleRect);
                }

                if (i < _currentStepIndex)
                {
                    using (var brush = new SolidBrush(Color.White))
                        e.Graphics.FillEllipse(brush, x - 4, lineY - 4, 8, 8);
                }

                // 3. Tekst pod kółkiem
                string text = _steps[i];
                using (var font = new Font(this.Font.FontFamily, 9, i == _currentStepIndex ? FontStyle.Bold : FontStyle.Regular))
                {
                    SizeF textSize = e.Graphics.MeasureString(text, font);
                    Color textColor = i == _currentStepIndex ? Color.Black : Color.Gray;

                    using (var brush = new SolidBrush(textColor))
                    {
                        e.Graphics.DrawString(text, font, brush, x - textSize.Width / 2, lineY + circleSize / 2 + 5);
                    }
                }
            }
        }
    }
}