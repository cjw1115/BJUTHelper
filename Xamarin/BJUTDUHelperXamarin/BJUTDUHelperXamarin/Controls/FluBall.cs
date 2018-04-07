using DeviceMotion.Plugin;
using DeviceMotion.Plugin.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
namespace BJUTDUHelperXamarin.Controls
{
    public class FluBall:ContentView
    {
        public static BindableProperty MaxValueProperty = BindableProperty.Create("MaxValue", typeof(double), typeof(FluBall), 0d);
        public double MaxValue
        {
            get
            {
                return (double)this.GetValue(MaxValueProperty);
            }
            set
            {
                this.SetValue(MaxValueProperty, value);
            }
        }
        public static BindableProperty ValueProperty = BindableProperty.Create("Value", typeof(double), typeof(FluBall), 0d);
        public double Value
        {
            get
            {
                return (double)this.GetValue(ValueProperty);
            }
            set
            {
                this.SetValue(ValueProperty, value);
            }
        }
        public static BindableProperty FullColorProperty = BindableProperty.Create("FullColor", typeof(Color), typeof(FluBall), Color.FromRgb(55,0xff,00));
        public Color FullColor
        {
            get
            {
                return (Color)this.GetValue(FullColorProperty);
            }
            set
            {
                this.SetValue(FullColorProperty, value);
            }
        }
        public static BindableProperty EmptyColorProperty = BindableProperty.Create("EmptyColor", typeof(Color), typeof(FluBall), Color.FromRgb(0xff,0x2a,0x00));
        public Color EmptyColor
        {
            get
            {
                return (Color)this.GetValue(EmptyColorProperty);
            }
            set
            {
                this.SetValue(EmptyColorProperty, value);
            }
        }

        public static BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(Command), typeof(FluBall));
        public Command Command
        {
            get
            {
                return (Command)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }
        SKCanvasView canvasView = new SKCanvasView();

        public event EventHandler Click;
        public FluBall()
        { 
            canvasView.PaintSurface += CanvasView_PaintSurface;
            this.Content = canvasView;

            TapGestureRecognizer tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += TapGesture_Tapped;
            this.canvasView.GestureRecognizers.Add(tapGesture);
        }

        private void TapGesture_Tapped(object sender, EventArgs e)
        {
            Click?.Invoke(this, null);
            Command?.Execute(null);
        }
        
        public static readonly BindableProperty FluPercentProperty = BindableProperty.Create("FluPercent", typeof(double), typeof(FluBall), 1d, propertyChanged: (o, oldValue, newValue) =>
        {
            var fluBall = o as FluBall;
            var color = InsertColor(fluBall.FullColor, fluBall.EmptyColor, (double)newValue);

            fluBall.wavePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = color
            };
        });
        public double FluPercent
        {
            get
            {
                return (double)this.GetValue(FluPercentProperty);
            }
            set
            {
                this.SetValue(FluPercentProperty,value);
            }
        }
       
        public static SKColor InsertColor(Color start, Color end, double percent)
        {
            start.ToSKColor().ToHsl(out float sh, out float ss, out float sl);

            end.ToSKColor().ToHsl(out float eh, out float es, out float el);

            float newH = (sh - eh) * (float)percent+eh;

            float newS = (ss - es) * (float)percent+es;

            float newL = (sl - el) * (float)percent+el;
             
            return SKColor.FromHsl(newH, newS, newL);
        }

        float transX = 0;
        float transY = 0;
        float radius = 0;
        bool isAnimate = false;
        Size lastSize;
        bool isChanged = false;
        SKPath circleMaskPath;
        SKPath wavePath;

        SKPaint wavePaint;
        SKPaint circlePaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = Color.White.ToSKColor()
        };
        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {


            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            if (lastSize.Width != info.Width || lastSize.Height != info.Height)
            {
                lastSize.Width = info.Width;
                lastSize.Height = info.Height;

                radius = (float)Math.Min(lastSize.Width, lastSize.Height);

                isChanged = true;
            }
            if (isChanged)
            {

                circleMaskPath = CreateCircleMaskPath(info.Width, info.Height);
                isChanged = false;
            }
            wavePath = CreateWaveMaskPath(info.Width, info.Height, radius/5/2/2/2, transX, transY,5);

            if (wavePaint == null)
            {
                wavePaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = FullColor.ToSKColor()
                };
                
            }


            SKPath cirlePath = new SKPath();
            cirlePath.AddCircle(info.Width / 2, info.Height / 2, Math.Min(info.Width, info.Height) / 2);
            
            canvas.ClipPath(cirlePath);

            
            canvas.DrawPath(cirlePath, circlePaint);
            canvas.DrawPath(wavePath, wavePaint);
            //canvas.DrawPath(circleMaskPath, circlePaint);

            if (isAnimate == false)
            {
                Animate();
                isAnimate = true;
            }

        }
        public SKPath CreateWaveMaskPath(float width, float height, float controlOffset, float transX, float transY,int waveCount=1)
        {
            var min = Math.Min(width, height);
            var startX = (width - min) / 2;

            SKPath sinePath = new SKPath();
            sinePath.MoveTo(new SKPoint(startX + transX, transY));
            var waveWidth = min / waveCount;
            
            for (int i = 0; i < waveCount*2; i++)
            {
                sinePath.CubicTo(new SKPoint(startX + transX + waveWidth / 2 + waveWidth * i, transY - controlOffset), new SKPoint(startX + transX + waveWidth / 2 + waveWidth * i, transY + controlOffset), new SKPoint(startX + transX + waveWidth+waveWidth*i, transY));
            }
            //sinePath.CubicTo(new SKPoint(startX + transX + min / 2, transY - controlOffset), new SKPoint(startX + transX + min / 2, transY + controlOffset), new SKPoint(startX + transX + min, transY));
            //sinePath.CubicTo(new SKPoint(startX + transX + min / 2 + min, transY - controlOffset), new SKPoint(startX + transX + min / 2 + min, transY + controlOffset), new SKPoint(startX + transX + min * 2, transY));

            sinePath.LineTo(new SKPoint(startX + transX + min * 2, height + transY));
            sinePath.LineTo(new SKPoint(startX + transX, height + transY));
            sinePath.LineTo(new SKPoint(startX + transX, transY));
            sinePath.Close();


            return sinePath;
        }
        public SKPath CreateCircleMaskPath(float width, float height)
        {
            var min = Math.Min(width, height);

            SKPath path = new SKPath();
            path.FillType = SKPathFillType.EvenOdd;

            path.AddCircle(width / 2, height / 2, min / 2);
            path.AddRect(new SKRect(0, 0, width, height));

            path.Close();

            return path;
        }
        Stopwatch timer = new Stopwatch();
        public async Task Animate()
        {
            while (true)
            {
                await Task.Delay(10);

                if (-transX > radius)
                {
                    transX = 0;
                }
                else
                {
                    transX -= radius / 200;
                }

                transY = radius/50+(float) (1-FluPercent) * (radius - (radius / 50 )*2);

                canvasView.InvalidateSurface();
            }
        }
    }
}
