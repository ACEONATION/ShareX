﻿#region License Information (GPL v3)

/*
    ShareX - A program that allows you to take screenshots and share any file type
    Copyright (c) 2007-2018 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using ShareX.HelpersLib;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace ShareX.ScreenCaptureLib
{
    public class ImageDrawingShape : BaseDrawingShape
    {
        public override ShapeType ShapeType { get; } = ShapeType.DrawingImage;

        public Image Image { get; protected set; }
        public ImageEditorInterpolationMode ImageInterpolationMode { get; protected set; }

        public override void OnConfigLoad()
        {
            ImageInterpolationMode = AnnotationOptions.ImageInterpolationMode;
        }

        public override void OnConfigSave()
        {
            AnnotationOptions.ImageInterpolationMode = ImageInterpolationMode;
        }

        public void SetImage(Image img, bool centerImage)
        {
            Dispose();

            Image = img;

            if (Image != null)
            {
                Point location;
                Size size = Image.Size;

                if (centerImage)
                {
                    location = new Point(Rectangle.X - size.Width / 2, Rectangle.Y - size.Height / 2);
                }
                else
                {
                    location = Rectangle.Location;
                }

                Rectangle = new Rectangle(location, size);
            }
        }

        public override void OnDraw(Graphics g)
        {
            DrawImage(g);
        }

        protected void DrawImage(Graphics g)
        {
            if (Image != null)
            {
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.InterpolationMode = Manager.GetInterpolationMode(ImageInterpolationMode);

                g.DrawImage(Image, Rectangle);

                g.PixelOffsetMode = PixelOffsetMode.Default;
                g.InterpolationMode = InterpolationMode.Bilinear;
            }
        }

        public override void OnMoved()
        {
            Rectangle canvas = Manager.Form.CanvasRectangle;
            Rectangle combinedImageRectangle = Manager.Shapes.OfType<ImageDrawingShape>().Select(x => x.Rectangle).Combine();

            if (!canvas.Contains(combinedImageRectangle))
            {
                Padding margin = new Padding(Math.Max(0, canvas.X - combinedImageRectangle.X), Math.Max(0, canvas.Y - combinedImageRectangle.Y),
                    Math.Max(0, combinedImageRectangle.Right - canvas.Right), Math.Max(0, combinedImageRectangle.Bottom - canvas.Bottom));
                Image img = ImageHelpers.AddCanvas(Manager.Form.Canvas, margin);

                if (img != null)
                {
                    Manager.Form.CanvasRectangle = Manager.Form.CanvasRectangle.LocationOffset(-margin.Left, -margin.Top);
                    Manager.UpdateCanvas(img, false);
                }
            }
        }

        public override void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
            }
        }
    }
}