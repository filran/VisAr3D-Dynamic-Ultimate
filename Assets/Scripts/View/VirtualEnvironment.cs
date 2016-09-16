using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace View
{
    /// <summary>
    /// VirtualEnvironment is the static class that implement methods that can be invoked anywhere virtual environment
    /// </summary>
    public static class VirtualEnvironment
    {
        #region PULBIC METHODS

        //Scale the number from XMI to UNITY
        public static float scale(float n)
        {
            return n * 0.02f;
        }
        #endregion
    }
}
