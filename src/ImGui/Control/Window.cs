﻿using ImGui.Layout;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    internal class Window
    {
        public int ID;
        public Point Position;
        public Size Size;
        public Size FullSize;
        public WindowFlags Flags;
        public DrawList DrawList;
        public Rect ClipRect;
        public Point PosFloat;

        public long LastActiveFrame;

        public StackLayout StackLayout { get; set; }

        public Stack<int> IDStack { get; set; } = new Stack<int>();

        public Window(string name, Point position, Size size, WindowFlags Flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;

            this.ID = name.GetHashCode();
            this.IDStack.Push(this.ID);
            this.Flags = Flags;
            this.PosFloat = position;
            this.Position = new Point((int)PosFloat.X, (int)PosFloat.Y);
            this.Size = this.FullSize = size;
            this.DC = new GUIDrawContext();
            this.DrawList = new DrawList();
            this.MoveID = GetID("#MOVE");
            this.Active = WasActive = false;


            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(34, 43, 46);
                style.Set(GUIStyleName.PaddingTop, 1.0);
                style.Set(GUIStyleName.PaddingRight, 1.0);
                style.Set(GUIStyleName.PaddingBottom, 2.0);
                style.Set(GUIStyleName.PaddingLeft, 1.0);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.00f, 0.00f, 0.00f, 0.70f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.30f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.60f), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.90f), GUIState.Active);
                this.Style = style;
            }
            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(86, 90, 160);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.27f, 0.27f, 0.54f, 0.83f));
                style.Set(GUIStyleName.BackgroundColor, new Color(0.32f, 0.32f, 0.63f, 0.87f), GUIState.Active);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.40f, 0.40f, 0.80f, 0.20f), GUIState.Disabled);
                style.Set(GUIStyleName.FontColor, Color.White);
                this.HeaderStyle = style;
            }

            this.StackLayout = new StackLayout(this.ID, this.Size);

            g.Windows.Add(this);
        }

        public int GetID(int int_id)
        {
            int seed = IDStack.Peek();
            var id = Hash(seed, int_id);
            return id;
        }

        public int GetID(string str_id)
        {
            int seed = IDStack.Peek();
            int int_id = str_id.GetHashCode();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        public int GetID(ITexture texture)
        {
            int seed = IDStack.Peek();
            int int_id = texture.GetHashCode();
            var id = Hash(seed, int_id);

            GUIContext g = Form.current.uiContext;
            g.KeepAliveID(id);
            return id;
        }

        public void ApplySize(Size new_size)
        {
            GUIContext g = Form.current.uiContext;
            Window window = this;
            window.FullSize = new_size;
        }

        public Rect Rect => new Rect(Position, Size);

        public double TitleBarHeight => HeaderStyle.PaddingVertical + HeaderStyle.PaddingVertical + HeaderStyle.FontSize*96.0/72.0;

        public Rect TitleBarRect => new Rect(Position, Size.Width, TitleBarHeight);

        public Rect ClientRect { get; internal set; }

        public bool Collapsed { get; internal set; } = false;
        public bool Active
        {
            get;
            internal set;
        }
        public Window RootWindow { get; internal set; }

        /// <summary>
        /// == window->GetID("#MOVE")
        /// </summary>
        public int MoveID { get; internal set; }
        public Rect WindowClippedRect { get; internal set; }
        public bool WasActive { get; internal set; }
        public bool SkipItems { get; internal set; } = false;
        public GUIDrawContext DC { get; internal set; }
        public int HiddenFrames { get; internal set; } = 0;
        public object ParentWindow { get; internal set; }
        public int BeginCount { get; internal set; }
        public bool Accessed { get; internal set; }

        public GUIStyle Style;
        public GUIStyle HeaderStyle;

        internal void PopClipRect()
        {
            //this.DrawList.PopClipRect();
            //var clipRectStack = this.DrawList._ClipRectStack;
            //this.ClipRect = clipRectStack[clipRectStack.Count-1];
        }

        internal Rect GetRect(int id, Size size, GUIStyle style, LayoutOption[] options)
        {
            var rect = StackLayout.GetRect(id, size, style, options);
            rect.Offset(this.Position.X, this.TitleBarHeight + this.Position.Y);
            return rect;
        }

        internal void ProcessLayout()
        {
            if (this.StackLayout.Dirty)
            {
                this.StackLayout.Layout(this.ClientRect.Size);
            }
        }

        private int Hash(int seed, int int_id)
        {
            int hash = seed + 17;
            hash = hash * 23 + this.ID.GetHashCode();
            var result = hash * 23 + int_id;
            return result;
        }

    }

    public partial class GUILayout
    {
        public static bool Begin(string name, ref bool open)
        {
            return Begin(name, ref open, Point.Zero, new Size(400, 300), 1, WindowFlags.Default);
        }

        public static bool Begin(string name, ref bool open, Point position, Size size, double bg_alpha, WindowFlags flags)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Debug.Assert(name != null);                        // Window name required
            Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // Called ImGui::Render() or ImGui::EndFrame() and haven't called ImGui::NewFrame() again yet

            if (flags.HaveFlag(WindowFlags.NoInputs))
            {
                flags |= WindowFlags.NoMove | WindowFlags.NoResize;
            }

            // Find or create
            bool window_is_new = false;
            Window window = g.FindWindowByName(name);
            if (window == null)
            {
                window = new Window(name, position, size, flags);
                window_is_new = true;
            }

            long current_frame = g.FrameCount;
            bool first_begin_of_the_frame = (window.LastActiveFrame != current_frame);
            if (first_begin_of_the_frame)
            {
                window.Flags = flags;
            }
            else
            {
                flags = window.Flags;
            }

            // Add to stack
            Window parent_window = (!(g.CurrentWindowStack.Count == 0)) ? g.CurrentWindowStack[g.CurrentWindowStack.Count - 1] : null;
            g.CurrentWindowStack.Add(window);
            g.CurrentWindow = window;
            //CheckStacksSize(window, true);
            Debug.Assert(parent_window != null || !(flags.HaveFlag(WindowFlags.ChildWindow)));

            bool window_was_active = (window.LastActiveFrame == current_frame - 1);

            bool window_appearing_after_being_hidden = (window.HiddenFrames == 1);

            // Update known root window (if we are a child window, otherwise window == window->RootWindow)
            int root_idx, root_non_popup_idx;
            for (root_idx = g.CurrentWindowStack.Count - 1; root_idx > 0; root_idx--)
            {
                if (!(g.CurrentWindowStack[root_idx].Flags.HaveFlag(WindowFlags.ChildWindow)))
                    break;
            }
            for (root_non_popup_idx = root_idx; root_non_popup_idx > 0; root_non_popup_idx--)
            {
                if (!(g.CurrentWindowStack[root_non_popup_idx].Flags.HaveFlag(WindowFlags.ChildWindow | WindowFlags.Popup)))
                    break;
            }
            window.ParentWindow = parent_window;
            window.RootWindow = g.CurrentWindowStack[root_idx];

            // When reusing window again multiple times a frame, just append content (don't need to setup again)
            if (first_begin_of_the_frame)
            {
                window.Active = true;
                window.ClipRect = new Rect(float.MinValue, float.MinValue, float.MaxValue, float.MaxValue);
                window.LastActiveFrame = current_frame;

                window.DrawList.Clear();
                Rect fullScreenRect = form.Rect;

                // clip
                window.ClipRect = fullScreenRect;

                // Collapse window by double-clicking on title bar
                if (g.HoveredWindow == window && g.IsMouseHoveringRect(window.TitleBarRect) && Input.Mouse.LeftButtonDoubleClicked)
                {
                    window.Collapsed = !window.Collapsed;
                }

                #region size

                // Apply minimum/maximum window size constraints and final size
                window.ApplySize(window.FullSize);
                window.Size = window.Collapsed ? window.TitleBarRect.Size : window.FullSize;

                #endregion

                #region position

                window.Position = new Point((int)window.PosFloat.X, (int)window.PosFloat.Y);

                #endregion

                // Draw window + handle manual resize
                GUIStyle style = window.Style;
                GUIStyle headerStyle = window.HeaderStyle;
                Rect title_bar_rect = window.TitleBarRect;
                float window_rounding = 3;
                if (window.Collapsed)
                {
                    // Draw title bar only
                    window.DrawList.RenderFrame(title_bar_rect.TopLeft, title_bar_rect.BottomRight, new Color(0.40f, 0.40f, 0.80f, 0.20f), true, window_rounding);
                }
                else
                {
                    Color resize_col = Color.Clear;
                    double resize_corner_size = Math.Max(window.Style.FontSize * 1.35, window_rounding + 1.0 + window.Style.FontSize * 0.2);
                    if (!flags.HaveFlag(WindowFlags.AlwaysAutoResize) && !flags.HaveFlag(WindowFlags.NoResize))
                    {
                        // Manual resize
                        var br = window.Rect.BottomRight;
                        Rect resize_rect = new Rect(br - new Vector(resize_corner_size * 0.75f, resize_corner_size * 0.75f), br);
                        int resize_id = window.GetID("#RESIZE");
                        bool hovered, held;
                        GUIBehavior.ButtonBehavior(resize_rect, resize_id, out hovered, out held, ButtonFlags.FlattenChilds);
                        resize_col =
                            held ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Active) :
                            hovered ? style.Get<Color>(GUIStyleName.ResizeGripColor, GUIState.Hover) :
                            style.Get<Color>(GUIStyleName.ResizeGripColor);

                        if (hovered || held)
                        {
                            Input.Mouse.Cursor = Cursor.NeswResize;
                        }

                        if (held)
                        {
                            // We don't use an incremental MouseDelta but rather compute an absolute target size based on mouse position
                            var t = Input.Mouse.MousePos - g.ActiveIdClickOffset - window.Position;
                            Size resize_size = new Size(t.X + resize_rect.Width, t.Y + resize_rect.Height);
                            window.ApplySize(resize_size);
                        }

                        window.Size = window.FullSize;
                        title_bar_rect = window.TitleBarRect;
                    }


                    // Window background
                    Color bg_color = style.BackgroundColor;
                    if (bg_alpha >= 0.0f)
                        bg_color.A = bg_alpha;
                    if (bg_color.A > 0.0f)
                        window.DrawList.AddRectFilled(window.Position + new Vector(0, window.TitleBarHeight), window.Rect.BottomRight, bg_color, window_rounding, flags.HaveFlag(WindowFlags.NoTitleBar) ? 15 : 4 | 8);

                    // Title bar
                    if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                    {
                        window.DrawList.AddRectFilled(title_bar_rect.TopLeft, title_bar_rect.BottomRight,
                            g.FocusedWindow == window ?
                            headerStyle.Get<Color>(GUIStyleName.BackgroundColor, GUIState.Active) :
                            headerStyle.Get<Color>(GUIStyleName.BackgroundColor), window_rounding, 1 | 2);
                    }

                    // Render resize grip
                    // (after the input handling so we don't have a frame of latency)
                    if (!flags.HaveFlag(WindowFlags.NoResize))
                    {
                        Point br = window.Rect.BottomRight;
                        var borderSize = 4;
                        window.DrawList.PathLineTo(br + new Vector(-resize_corner_size, -borderSize));
                        window.DrawList.PathLineTo(br + new Vector(-borderSize, -resize_corner_size));
                        window.DrawList.PathArcToFast(new Point(br.X - window_rounding - borderSize, br.Y - window_rounding - borderSize), window_rounding, 0, 3);
                        window.DrawList.PathFill(resize_col);
                    }

                    // Save clipped aabb so we can access it in constant-time in FindHoveredWindow()
                    window.WindowClippedRect = window.Rect;
                    window.WindowClippedRect.Intersect(window.ClipRect);
                }

                window.ClientRect = new Rect(point1: new Point(window.Position.X, window.Position.Y + window.TitleBarHeight),
                    point2: window.Rect.BottomRight);

                // Title bar
                if (!flags.HaveFlag(WindowFlags.NoTitleBar))
                {
                    //const float pad = 2.0f;
                    //const float rad = (window.TitleBarHeight - pad * 2.0f) * 0.5f;
                    //if (CloseButton(window.GetID("#CLOSE"), window.Rect.TopRight + new Vector(-pad - rad, pad + rad), rad))
                    //    open = false;

                    Size text_size = headerStyle.CalcSize(name, GUIState.Normal, null);
                    //if (!flags.HaveFlag(WindowFlags.NoCollapse))
                    //    RenderCollapseTriangle(window->Pos + style.FramePadding, !window.Collapsed, 1.0f, true);

                    Point text_min = window.Position + new Vector(style.PaddingLeft, style.PaddingTop);
                    Point text_max = window.Position + new Vector(window.Size.Width - style.PaddingHorizontal, style.PaddingVertical * 2 + text_size.Height);
                    //ImVec2 clip_max = ImVec2(window->Pos.x + window->Size.x - (p_open ? title_bar_rect.GetHeight() - 3 : style.FramePadding.x), text_max.y); // Match the size of CloseWindowButton()
                    window.DrawList.DrawText(new Rect(text_min, text_max), name, headerStyle, GUIState.Normal);
                }
            }

            // Clear 'accessed' flag last thing
            if (first_begin_of_the_frame)
                window.Accessed = false;
            window.BeginCount++;

            window.StackLayout.Begin();

            // Return false if we don't intend to display anything to allow user to perform an early out optimization
            window.SkipItems = window.Collapsed || !window.Active;
            return !window.SkipItems;
        }

        public static void End()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;

            window.PopClipRect();   // inner window clip rectangle

            window.ProcessLayout();

            // Pop
            g.CurrentWindowStack.RemoveAt(g.CurrentWindowStack.Count - 1);
            g.CurrentWindow = ((g.CurrentWindowStack.Count == 0) ? null : g.CurrentWindowStack[g.CurrentWindowStack.Count - 1]);
        }
    }

    [Flags]
    public enum WindowFlags
    {
        Default = 0,
        NoTitleBar = 1 << 0,   // Disable title-bar
        NoResize = 1 << 1,   // Disable user resizing with the lower-right grip
        NoMove = 1 << 2,   // Disable user moving the window
        NoScrollbar = 1 << 3,   // Disable scrollbars (window can still scroll with mouse or programatically)
        NoScrollWithMouse = 1 << 4,   // Disable user vertically scrolling with mouse wheel
        NoCollapse = 1 << 5,   // Disable user collapsing window by double-clicking on it
        AlwaysAutoResize = 1 << 6,   // Resize every window to its content every frame
        ShowBorders = 1 << 7,   // Show borders around windows and items
        NoSavedSettings = 1 << 8,   // Never load/save settings in .ini file
        NoInputs = 1 << 9,   // Disable catching mouse or keyboard inputs
        MenuBar = 1 << 10,  // Has a menu-bar
        HorizontalScrollbar = 1 << 11,  // Allow horizontal scrollbar to appear (off by default). You may use SetNextWindowContentSize(ImVec2(width,0.0f)); prior to calling Begin() to specify width. Read code in imgui_demo in the "Horizontal Scrolling" section.
        NoFocusOnAppearing = 1 << 12,  // Disable taking focus when transitioning from hidden to visible state
        NoBringToFrontOnFocus = 1 << 13,  // Disable bringing window to front when taking focus (e.g. clicking on it or programatically giving it focus)
        AlwaysVerticalScrollbar = 1 << 14,  // Always show vertical scrollbar (even if ContentSize.y < Size.y)
        AlwaysHorizontalScrollbar = 1 << 15,  // Always show horizontal scrollbar (even if ContentSize.x < Size.x)
        AlwaysUseWindowPadding = 1 << 16,  // Ensure child windows without border uses style.WindowPadding (ignored by default for non-bordered child windows, because more convenient)
        // [Internal]
        ChildWindow = 1 << 20,  // Don't use! For internal use by BeginChild()
        ChildWindowAutoFitX = 1 << 21,  // Don't use! For internal use by BeginChild()
        ChildWindowAutoFitY = 1 << 22,  // Don't use! For internal use by BeginChild()
        ComboBox = 1 << 23,  // Don't use! For internal use by ComboBox()
        Tooltip = 1 << 24,  // Don't use! For internal use by BeginTooltip()
        Popup = 1 << 25,  // Don't use! For internal use by BeginPopup()
        Modal = 1 << 26,  // Don't use! For internal use by BeginPopupModal()
        ChildMenu = 1 << 27   // Don't use! For internal use by BeginMenu()
    };
}