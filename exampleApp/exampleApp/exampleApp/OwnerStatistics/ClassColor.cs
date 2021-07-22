using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using SkiaSharp;
using System.Collections;
using Color = Xamarin.Forms.Color;

namespace exampleApp.OwnerStatistics
{

    public static class ClassColor
    {
        public static List<Color> colors = new List<Color>()
        {
            Color.Red, Color.Blue, Color.Green, Color.Purple, Color.Gold, Color.Aqua, Color.Orange, Color.Brown,
            Color.Turquoise, Color.WhiteSmoke, Color.Coral, Color.DeepPink, Color.GreenYellow, Color.Silver, Color.Tan, Color.SteelBlue,
            Color.Peru, Color.Yellow, Color.Tomato, Color.Azure, Color.OrangeRed, Color.Orchid, Color.Bisque, Color.LightYellow
        };
        public static List<Color> ListColors { get { return colors; } }
    }
}
