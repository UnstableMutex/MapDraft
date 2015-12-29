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
    class TileGroupBase : Panel
    {
        public TileGroupBase()
        {
            // Rows = Columns = 1;

        }

        public Point GetCoords()
        {
            if (this.Parent is Canvas)
            {
                var x = Canvas.GetLeft(this);
                var y = Canvas.GetTop(this);
                return new Point(x, y);
            }
            else
            {
                var points1 = new Point[]
            {
                new Point(0,0),new Point(this.Width,0),new Point(0,this.Height),
                new Point(this.Width,this.Height)    
            };
                var p = Parent as TileGroupBase;
                var c = p.GetCoords();
                var index = p.InternalChildren.IndexOf(this);
                var p1 = points1[index];
                c.Offset(p1.X, p1.Y);
                return c;

            }

        }

        //будет содержать либо 4 итема либо 1
        protected override Size ArrangeOverride(Size finalSize)
        {
            // Перебрать все дочерние элементы.
            var size = new Size(finalSize.Width / 2, finalSize.Height / 2);
            var points = new Point[]
            {
                new Point(0,0),new Point(size.Width,0),new Point(0,size.Height),new Point(size.Width,size.Height)    
            };
            if (this.InternalChildren.Count == 1)
            {
                foreach (UIElement element in base.InternalChildren)
                {
                    // Назначить дочернему элементу его границы.
                    Rect bounds = new Rect(new Point(0, 0), finalSize);
                    element.Arrange(bounds);
                    // (Теперь вы можете прочитать element.ActualHeight и
                    // element.ActualWidth, чтобы определить его размеры.)
                }
            }
            else
            {
                var i = 0;
                foreach (UIElement element in base.InternalChildren)
                {
                    // Назначить дочернему элементу его границы.
                    Rect bounds = new Rect(points[i++], size);
                    element.Arrange(bounds);
                    // (Теперь вы можете прочитать element.ActualHeight и
                    // element.ActualWidth, чтобы определить его размеры.)
                }
            }








            // Определить, сколько места займет эта панель.
            // Эта информация будет использована для установки
            // свойств ActualHeight и ActualWidth панели,
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.InternalChildren.Count == 1)
            {
                return new Size(256, 256);
            }
            else
            {

                var ele = this.InternalChildren[0];
                var ds = ele.DesiredSize;
                return new Size(ds.Width * 2, ds.Height * 2);


                // foreach (UIElement element in base.InternalChildren)
                //{
                //   // Запросить у каждого дочернего элемента желательное для него
                //   // пространство, применяя ограничение availableSize

                // element.Measure(availableSize);
                //    var ds = element.DesiredSize;
                //    // (Здесь можно прочитать element.DesiredSize, чтобы получить запрошенный размер.)
                //}               
            }
            // Проверить все дочерние элементы.

            // Показать, сколько места требует данная панель.
            // Будет использовано для установки свойства DesiredSize панели
            // return new Size (...);
        }

    }
}
