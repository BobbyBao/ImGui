﻿using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Button : Control
    {
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        private string text;
        public string Text
        {
            get { return text; }
            private set
            {
                if (Text == value)
                {
                    return;
                }

                text = value;
                NeedRepaint = true;
            }
        }
        public Rect Rect { get; private set; }
        public bool Result { get; private set; }
        private ToolTip t;
        public override void OnUpdate()
        {
            var style = Skin.current.Button[State];
            Layout.MaxWidth = (int)Rect.Width;
            Layout.MaxHeight = (int)Rect.Height;
            Layout.Text = Text;

            var oldState = State;
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.GetMousePos(Form));
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.GetMousePos(Form));
            if (active)
            {
                State = "Active";
            }
            else if (hover)
            {
                State = "Hover";
                //if(t ==null)
                //{
                //    t = new ToolTip();
                //    Application.Forms.Add(t);
                //}
                //t.TipText = Text;
                //t.Show();
            }
            else
            {
                State = "Normal";
            }

            if(State != oldState)
            {
                NeedRepaint = true;
            }

            bool clicked = Input.Mouse.LeftButtonClicked && Rect.Contains(Input.Mouse.GetMousePos(Form));
            Result = clicked;
        }

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(Layout), Skin.current.Button[State]);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        internal Button(string name, BaseForm form, string text, Rect rect)
            : base(name, form)
        {
            Rect = rect;
            Text = text;

            var font = Skin.current.Button[State].Font;
            Format = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                    {
                        {"fontFamilyName", font.FontFamily},
                        {"fontWeight", font.FontWeight},
                        {"fontStyle", font.FontStyle},
                        {"fontStretch", font.FontStretch},
                        {"fontSize", (float) font.Size}
                    });
            var textStyle = Skin.current.Button[State].TextStyle;
            Format.Alignment = textStyle.TextAlignment;
            Layout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                    {
                        {"text", Text},
                        {"textFormat", Format},
                        {"maxWidth", (int)Rect.Width},
                        {"maxHeight", (int)Rect.Height}
                    });

        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static bool DoControl(Context g, BaseForm form, Rect rect, string text, string name)
        {
            //The control hasn't been created, create it.
            if (!form.Controls.ContainsKey(name))
            {
                var button = new Button(name, form, text, rect);
                button.OnUpdate();
                button.OnRender(g);
            }

            var control = form.Controls[name] as Button;
            Debug.Assert(control != null);

            //Check if the control need to be relayout

            return control.Result;
        }
    }
}