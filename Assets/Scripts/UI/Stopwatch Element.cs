using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace VARLab.CCSIF
{
    public class StopwatchElement : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StopwatchElement, UxmlTraits> { }

        private float startTime;
        public float StartTime { get { return startTime; } set { startTime = value; animiationStartTime = value; } }

        private float currentTime;
        public float CurrentTime
        {
            get => currentTime;
            set
            {
                animiationCurrentTime = value;
                currentTime = value;
                TimeSpan ts = TimeSpan.FromSeconds(Math.Abs(currentTime));
                if (currentTime >= 0) { StopwatchLabel.text = ts.ToString("ss"); StopwatchLabel.style.color = borderYellow1; } // Yellow when time is positive
                else { StopwatchLabel.text = "-" + ts.ToString("ss"); StopwatchLabel.style.color = borderRed1; } // Red when time is negative
                MarkDirtyRepaint();
            }
        }

        private float timeLimitRepeat;
        public float TimeLimitRepeat { get { return timeLimitRepeat; } set { timeLimitRepeat = value; } }

        private float animiationStartTime;
        private float animiationCurrentTime;
        public Label StopwatchLabel { get; set; }

        private static readonly string ussClassName = "timer-countdown";
        private static readonly string labelUssClassName = "stopwatch-label";
        private static readonly string labelElementName = "TimerLabel";

        private Color borderRed1 = new Color(223 / 255f, 61f / 255f, 32f / 255f);
        private Color borderRed2 = new Color(109 / 255f, 56f / 255f, 61f / 255f);
        private Color borderYellow1 = new Color(255f / 255f, 204f / 255f, 51f / 255f);
        private Color borderYellow2 = new Color(102f / 255f, 78f / 255f, 39f / 255f);

        private CustomStyleProperty<Color> outerBorderColourProperty = new CustomStyleProperty<Color>("--border-outer-colour");
        private CustomStyleProperty<Color> outerBorderColour2Property = new CustomStyleProperty<Color>("--border-outer-colour-2");
        private CustomStyleProperty<Color> progressColourProperty = new CustomStyleProperty<Color>("--progress-colour");
        private CustomStyleProperty<Color> trackerColourProperty = new CustomStyleProperty<Color>("--tracker-colour");

        private Color outerBorderColour;
        private Color outerBorderColour2;
        private Color progressColour;
        private Color trackerColour;

        private const float InnerStartingArcAngle = 0.0f;
        private const float MiddleRadius = 125f;
        private const float OuterRadius = 175f;
        private const float InnerLineWidth = 3.0f;
        private const float InnerLineHeight = 0.12f;
        private const float OuterLineHeight = 1.8f;
        private const float MiddleLineWidth = 19.0f;
        private const float RightAngle = 90f;
        private const float FullCircle = 360f;
        private const float OuterLineWidth = 15.0f;
        private const int InnerCircleCount = 58;
        private const int NumberOfTicks = 46;

        //all changes with Scale Factor and Size need to be updated manually in the stopwatch uxml to the newly scaled size
        //font size in Stopwatch.uss is NOT updated dynamically any cannot as far as I know. This value needs to be updated manually
        private const float ScaleFactor = 0.8f;

        private const float Size = 400;

        public StopwatchElement()
        {
            AddToClassList(ussClassName);

            StopwatchLabel = new Label() { name = labelElementName };
            StopwatchLabel.AddToClassList(labelUssClassName);
            Add(StopwatchLabel);

            RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent += OnVisualContentGenerated;

            CurrentTime = startTime;

            //automatically update the UXML to have the applied scale factor
            this.style.width = Size * ScaleFactor;
            this.style.height = Size * ScaleFactor;
        }

        private static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            StopwatchElement element = evt.currentTarget as StopwatchElement;
            element.UpdateCustomStyles();
        }

        // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
        // the control.
        private void UpdateCustomStyles()
        {
            bool repaint = false;
            if (customStyle.TryGetValue(outerBorderColourProperty, out outerBorderColour)) { repaint = true; }
            if (customStyle.TryGetValue(outerBorderColour2Property, out outerBorderColour2)) { repaint = true; }
            if (customStyle.TryGetValue(progressColourProperty, out progressColour)) { repaint = true; }
            if (customStyle.TryGetValue(trackerColourProperty, out trackerColour)) { repaint = true; }
            if (repaint) { MarkDirtyRepaint(); }
        }

        private void OnVisualContentGenerated(MeshGenerationContext context)
        {
            Vector2 center = new Vector2(Size * ScaleFactor, Size * ScaleFactor) / 2f;
            Painter2D painter = context.painter2D;
            float arcAngle = InnerStartingArcAngle;

            ApplyColouring();
            InnerTicks(arcAngle, painter, center);
            ProgressAnimation(arcAngle, painter, center);
            OuterRingAnimation(arcAngle, painter, center);

            painter.ClosePath();
        }

        // Colouring
        private void ApplyColouring()
        {
            if (currentTime >= 0)
            {
                progressColour = borderYellow1;
                outerBorderColour = borderYellow1;
                outerBorderColour2 = borderYellow2;
            }
            else
            {
                progressColour = borderRed1;
                outerBorderColour = borderRed1;
                outerBorderColour2 = borderRed2;
            }
        }

        // Inner Circle (Gray Ticks)
        private void InnerTicks(float arcAngle, Painter2D painter, Vector2 center)
        {
            painter.lineWidth = InnerLineWidth;
            painter.lineCap = LineCap.Round;
            painter.strokeColor = trackerColour;

            for (int i = 0; i < InnerCircleCount; i++)
            {
                painter.BeginPath();
                painter.Arc(center, MiddleRadius * ScaleFactor, arcAngle, arcAngle + InnerLineHeight);
                painter.Stroke();
                arcAngle += 360f / InnerCircleCount;
            }
        }

        // Progress
        private void ProgressAnimation(float arcAngle, Painter2D painter, Vector2 center)
        {
            painter.lineWidth = MiddleLineWidth * ScaleFactor;
            painter.strokeColor = progressColour;
            painter.BeginPath();

            if (currentTime >= 0) { painter.Arc(center, MiddleRadius * ScaleFactor, RightAngle, FullCircle * (animiationCurrentTime / animiationStartTime) + RightAngle); }
            else { painter.Arc(center, MiddleRadius * ScaleFactor, RightAngle, FullCircle * ((animiationCurrentTime % TimeLimitRepeat) / TimeLimitRepeat) + RightAngle); }

        }

        // Outer Ring Animation
        private void OuterRingAnimation(float arcAngle, Painter2D painter, Vector2 center)
        {
            painter.Stroke();
            painter.lineWidth = OuterLineWidth * ScaleFactor;
            painter.lineCap = LineCap.Butt;
            painter.BeginPath();
            arcAngle = -RightAngle;

            float remainder = animiationStartTime % 2f;
            float flooredTime = Mathf.Floor(animiationCurrentTime);
            float percent = animiationCurrentTime - flooredTime;
            float startAngle = FullCircle * percent - RightAngle;

            for (int i = 0; i < NumberOfTicks; i++)
            {
                bool remainderCheck = flooredTime % 2 == remainder;

                if (arcAngle >= startAngle || animiationCurrentTime <= 0f) { painter.strokeColor = remainderCheck ? outerBorderColour : outerBorderColour2; }
                else { painter.strokeColor = remainderCheck ? outerBorderColour2 : outerBorderColour; }
                painter.BeginPath();
                painter.Arc(center, OuterRadius * ScaleFactor, arcAngle, arcAngle + OuterLineHeight * ScaleFactor);
                painter.Stroke();
                arcAngle += FullCircle / NumberOfTicks;
            }
        }

        ~StopwatchElement()
        {
            UnregisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
            generateVisualContent -= OnVisualContentGenerated;
        }
    }
}

