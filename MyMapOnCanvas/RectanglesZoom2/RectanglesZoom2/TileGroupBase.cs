using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace RectanglesZoom2
{
    class TileGroupBase:UniformGrid
    {
        public TileGroupBase()
        {
         Rows=   Columns = 1;
           
        }

       //будет содержать либо 4 итема либо 1
        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }
     
    }
}
