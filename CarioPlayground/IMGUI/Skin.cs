﻿using Cairo;

namespace IMGUI
{
    public sealed class Skin
    {
		public Font Font { get; private set; }

        public Style Button { get; private set; }
        public Style Label { get; private set; }
        public Style Toggle { get; private set; }

        internal static Skin _current;

        static Skin()
        {
            _current = new Skin();
        }

        public Skin()
        {
            Font = new Font();
            
            Label = new Style();
            Label.Active.FontColor = new Color(0,0,1);

            Button = new Style();
            {
                Button.BorderTop = Button.BorderRight = Button.BorderBottom = Button.BorderLeft = new Length(1, Unit.Pixel);
                Button.BorderTopColor = CairoEx.ColorArgb(0xFFB3B3B3);
                Button.BorderRightColor = CairoEx.ColorArgb(0xFF7A7A7A);
                Button.BorderBottomColor = CairoEx.ColorArgb(0xFF7A7A7A);
                Button.BorderLeftColor = CairoEx.ColorArgb(0xFFB3B3B3);
                Button.TextStyle = new TextStyle {TextAlign = TextAlignment.Center};

                Button.Normal.FontColor = CairoEx.ColorBlack;
                Button.Normal.FontWeight = FontWeight.Normal;
                Button.Active.FontWeight = FontWeight.Bold;
                Button.Normal.BackgroundColor = CairoEx.ColorRgb(0x9F, 0x9F, 0x9F);
                Button.Hover.BackgroundColor = CairoEx.ColorArgb(0xFFAFAFAF);
                Button.Active.BackgroundColor = CairoEx.ColorArgb(0xFF8F8F8F);

                var gradient = new LinearGradient(0, 0, 0, 1);
                gradient.AddColorStop(0, new Color(0.87, 0.93, 0.96));
                gradient.AddColorStop(1, new Color(0.65, 0.85, 0.96));
                Button.Normal.BackgroundPattern = gradient;
                Button.Hover.BackgroundPattern = gradient;
                Button.Active.BackgroundPattern = gradient;
            }

            Toggle = new Style();
            {
                Toggle.Normal.BackgroundColor = CairoEx.ColorRgb(0x9F, 0x9F, 0x9F);
                Toggle.Hover.BackgroundColor = CairoEx.ColorRgb(0x9F, 0x9F, 0x9F);
                Toggle.Active.BackgroundColor = CairoEx.ColorRgb(0x9F, 0x9F, 0x9F);
            }

        }


    }
}
