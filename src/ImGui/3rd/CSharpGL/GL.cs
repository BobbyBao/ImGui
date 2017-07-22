﻿using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace CSharpGL
{
    /// <summary>
    /// The OpenGL class wraps Sun's OpenGL 3D library.
    /// </summary>
    public static partial class GL
    {
        #region Native

        [DllImport("libGL.so", EntryPoint = "glXGetProcAddress")]
        internal static extern IntPtr glxGetProcAddress(string s);


        /// <summary>
        /// Gets a proc address.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <returns>The address of the function.</returns>
        [DllImport(OpenGL32, SetLastError = true)]
        public static extern IntPtr wglGetProcAddress(string name);

        #endregion

        /// <summary>
        /// Returns a delegate for an extension function. This delegate can be called to execute the extension function.
        /// </summary>
        /// <typeparam name="T">The extension delegate type.</typeparam>
        /// <returns>The delegate that points to the extension function.</returns>
        private static T GetDelegateFor<T>() where T : class
        {
            if(GL.allFunctionsLoaded)
            {
                throw new InvalidOperationException("GetDelegateFor cannot be used at rendering time");
            }

            //  Get the type of the extension function.
            Type delegateType = typeof(T);

            //  Get the name of the extension function.
            string name = delegateType.Name;

            IntPtr proc = IntPtr.Zero;
            if(ImGui.Utility.CurrentOS.IsWindows)
            {
                // check https://www.opengl.org/wiki/Load_OpenGL_Functions
                proc = wglGetProcAddress(name);
                var pointer = proc.ToInt64();
                if (-1 <= pointer && pointer <= 3)
                {
                    proc = Win32.GetProcAddress(name);
                    pointer = proc.ToInt64();
                    if(-1 <= pointer && pointer <= 3)
                    {
                        throw new NotSupportedException("Extension function " + name + " not supported.");
                    }
                }
            }
            else if(ImGui.Utility.CurrentOS.IsLinux)
            {
                proc = glxGetProcAddress(name);
                if(proc == IntPtr.Zero)
                {
                    throw new NotSupportedException("Extension function " + name + " not supported.");
                }
            }
            else if(ImGui.Utility.CurrentOS.IsMac)
            {
                throw new NotImplementedException("Binding for macOS hasn't been implemented.");
            }
            else
            {
                throw new NotImplementedException("Unsupported OS.");
            }

            //  Get the delegate for the function pointer.
            Delegate del = Marshal.GetDelegateForFunctionPointer<T>(proc) as Delegate;

            return del as T;
        }
    }
}
