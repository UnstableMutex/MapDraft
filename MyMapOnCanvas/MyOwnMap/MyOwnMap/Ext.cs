using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MyOwnMap
{
   static class Ext
    {
       /// <summary>
       /// получает новую позицию для картинки
       /// </summary>
       /// <param name="image">картинка</param>
       /// <param name="v">вектор перемещения</param>
       /// <param name="c">канвас для вычисления того видима ли будет картинка после перемещения</param>
       /// <returns>null если картинка будет невидима, координаты если видима</returns>
       public static Point? GetNewPosition(this Image image, Vector v, Canvas c)
       {
             var x = Canvas.GetLeft(image);
           var y = Canvas.GetTop(image);
           var p=new Point(x,y);
           p.Offset(v.X,v.Y);
           var isInvisible = p.X > c.ActualWidth | p.Y > c.ActualHeight;
           isInvisible |= p.X + Constants.tileSize < 0 | p.Y + Constants.tileSize < 0;
           if (isInvisible)
           {
               return null;
           }
           else
           {
               return p;
           }
       }
      
    }
}
