﻿using System;

namespace ImGui
{
    [Flags]
    enum Platform
    {
        Windows = 0,
        Mac     = 1,
        Linux   = 2,
        Android = 4,
        iOS     = 8,
    }
}
