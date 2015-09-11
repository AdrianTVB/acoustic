using System;
using System.Windows;

namespace CSharpQuadTree
{
    public interface IPoint
    {
        /// <summary>
        /// The get point.
        /// </summary>
        /// <returns>
        /// The <see cref="Point"/>.
        /// </returns>
        Point GetPoint();
        //event EventHandler BoundsChanged;
    }
}