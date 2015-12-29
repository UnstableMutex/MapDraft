using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TestZoom
{
    class ImageMoveResize
    {
        private readonly float _zoomSpeed;

        public ImageMoveResize(float zoomSpeed)
        {
            _zoomSpeed = zoomSpeed;
        }

        float GetScale(int wheel)
        {
             wheel = wheel > 0 ? -1 : 1;
            if (wheel > 0)
            {
                //увеличение
                return 1 / _zoomSpeed;
            }
            if (wheel < 0)
            {
                //уменьшение
                return _zoomSpeed;
            }
            throw new NotImplementedException();
        }

        float GetVectorMultiplier(int wheel)
        {
            return 1 - GetScale(wheel);
        }
        /// <summary>
        /// получает новы размер прямоугольника
        /// </summary>
        /// <param name="size"></param>
        /// <param name="wheel"></param>
        /// <returns></returns>
        public Size GetNewSize(Size size,  int wheel)
        {
            var s = this.GetScale(wheel);
            return new Size(size.Width*s,size.Height*s); 
        }

        /// <summary>
        /// получает вектор на который будем двигать прямоугольник с учетом позиции мыши
        /// </summary>
        /// <param name="mouse"></param>
        /// <param name="rect"></param>
        /// <param name="wheel"></param>
        /// <returns></returns>
        public Vector GetOffSet(Point mouse, Rect rect, int wheel)
        {
        
       
            float vectorMultiplier = GetVectorMultiplier(wheel);
            var deltamX = mouse.X - rect.X;
            var deltamY = mouse.Y - rect.Y;
            var v = new Vector(deltamX * vectorMultiplier, deltamY * vectorMultiplier);
            return v;
        }


    }
}
