﻿using System;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinPrimitiveRenderer : IPrimitiveRenderer
    {
        public void DrawBoxModel(Rect rect, StyleRuleSet style)
        {
            //Widths of border
            var bt = style.Get<double>(GUIStyleName.BorderTop);
            var br = style.Get<double>(GUIStyleName.BorderRight);
            var bb = style.Get<double>(GUIStyleName.BorderBottom);
            var bl = style.Get<double>(GUIStyleName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(GUIStyleName.PaddingTop);
            var pr = style.Get<double>(GUIStyleName.PaddingRight);
            var pb = style.Get<double>(GUIStyleName.PaddingBottom);
            var pl = style.Get<double>(GUIStyleName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            var borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            //if (ptl.X > ptr.X) return;//TODO what if (ptl.X > ptr.X) happens?
            var paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);

            // draw background in padding-box
            var gradient = (Gradient)style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding = style.Get<double>(GUIStyleName.BorderTopLeftRadius);//FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float)borderRounding);
                this.PathFill(bgColor);
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }

            //Content
            //Content-box
            //no content

            //Border
            //  Top
            if (!MathEx.AmostZero(bt))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    PathLineTo(ptl);
                    PathLineTo(btl);
                    PathLineTo(btr);
                    PathLineTo(ptr);
                    PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(br))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    PathLineTo(ptr);
                    PathLineTo(btr);
                    PathLineTo(bbr);
                    PathLineTo(pbr);
                    PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(bb))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    PathLineTo(pbr);
                    PathLineTo(bbr);
                    PathLineTo(bbl);
                    PathLineTo(pbl);
                    PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(bl))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    PathLineTo(pbl);
                    PathLineTo(bbl);
                    PathLineTo(btl);
                    PathLineTo(ptl);
                    PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    PathRect(btl, bbr);
                    PathStroke(outlineColor, true, outlineWidth);
                }
            }

#if DrawPaddingBox
            PathRect(ptl, pbr);
            PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            PathRect(ctl, cbr);
            PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }

        public void DrawBoxModel(TextPrimitive textPrimitive, Rect rect, StyleRuleSet style)
        {
            //Widths of border
            var bt = style.Get<double>(GUIStyleName.BorderTop);
            var br = style.Get<double>(GUIStyleName.BorderRight);
            var bb = style.Get<double>(GUIStyleName.BorderBottom);
            var bl = style.Get<double>(GUIStyleName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(GUIStyleName.PaddingTop);
            var pr = style.Get<double>(GUIStyleName.PaddingRight);
            var pb = style.Get<double>(GUIStyleName.PaddingBottom);
            var pl = style.Get<double>(GUIStyleName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            var borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            //if (ptl.X > ptr.X) return;//TODO what if (ptl.X > ptr.X) happens?
            var paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);

            // draw background in padding-box
            var gradient = (Gradient)style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding = style.Get<double>(GUIStyleName.BorderTopLeftRadius);//FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float)borderRounding);
                this.PathFill(bgColor);
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }

            //Content
            //Content-box
            if (ctl.X < ctr.X)//content should not be visible when ctl.X > ctr.X
            {
                if (textPrimitive != null)
                {
                    //var textSize = style.CalcSize(text);
                    /*HACK Don't check text size because the size calculated by Typography is not accurate. */
                    /*if (textSize.Height < contentBoxRect.Height && textSize.Width < contentBoxRect.Width)*/
                    {
                        DrawText(textPrimitive, contentBoxRect, style);
                    }
                }
            }

            //Border
            //  Top
            if (!MathEx.AmostZero(bt))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    PathLineTo(ptl);
                    PathLineTo(btl);
                    PathLineTo(btr);
                    PathLineTo(ptr);
                    PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(br))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    PathLineTo(ptr);
                    PathLineTo(btr);
                    PathLineTo(bbr);
                    PathLineTo(pbr);
                    PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(bb))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    PathLineTo(pbr);
                    PathLineTo(bbr);
                    PathLineTo(bbl);
                    PathLineTo(pbl);
                    PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(bl))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    PathLineTo(pbl);
                    PathLineTo(bbl);
                    PathLineTo(btl);
                    PathLineTo(ptl);
                    PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    PathRect(btl, bbr);
                    PathStroke(outlineColor, true, outlineWidth);
                }
            }

#if DrawPaddingBox
            PathRect(ptl, pbr);
            PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            PathRect(ctl, cbr);
            PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }

        public void DrawBoxModel(ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style)
        {
            //Widths of border
            var bt = style.Get<double>(GUIStyleName.BorderTop);
            var br = style.Get<double>(GUIStyleName.BorderRight);
            var bb = style.Get<double>(GUIStyleName.BorderBottom);
            var bl = style.Get<double>(GUIStyleName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(GUIStyleName.PaddingTop);
            var pr = style.Get<double>(GUIStyleName.PaddingRight);
            var pb = style.Get<double>(GUIStyleName.PaddingBottom);
            var pl = style.Get<double>(GUIStyleName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            var borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            //if (ptl.X > ptr.X) return;//TODO what if (ptl.X > ptr.X) happens?
            var paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);

            // draw background in padding-box
            var gradient = (Gradient)style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding = style.Get<double>(GUIStyleName.BorderTopLeftRadius);//FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float)borderRounding);
                this.PathFill(bgColor);
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }

            //Content
            //Content-box
            if (ctl.X < ctr.X)//content should not be visible when ctl.X > ctr.X
            {
                if (imagePrimitive != null)
                {
                    DrawImage(imagePrimitive, contentBoxRect, style);
                }
            }

            //Border
            //  Top
            if (!MathEx.AmostZero(bt))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    PathLineTo(ptl);
                    PathLineTo(btl);
                    PathLineTo(btr);
                    PathLineTo(ptr);
                    PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(br))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    PathLineTo(ptr);
                    PathLineTo(btr);
                    PathLineTo(bbr);
                    PathLineTo(pbr);
                    PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(bb))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    PathLineTo(pbr);
                    PathLineTo(bbr);
                    PathLineTo(bbl);
                    PathLineTo(pbl);
                    PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(bl))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    PathLineTo(pbl);
                    PathLineTo(bbl);
                    PathLineTo(btl);
                    PathLineTo(ptl);
                    PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    PathRect(btl, bbr);
                    PathStroke(outlineColor, true, outlineWidth);
                }
            }

#if DrawPaddingBox
            PathRect(ptl, pbr);
            PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            PathRect(ctl, cbr);
            PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }
    }
}