// Copyright (C) 2006 by:
//
// Author:
//   Piotr Zurek, p.zurek@gmail.com
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;

namespace Psycho
{
        public class TopicFrame
        {
                Cairo.Context context;
                double frameHeight, frameWidth;
                Cairo.PointD origin, center, left, right, top, bottom;
                double octDist, octDistHor, octDistVer;
                double hexDist, hexDistHor;
                double radius;
                Topic topic;
                double sqrt2 = System.Math.Sqrt (2);
                double sqrt3 = System.Math.Sqrt (3);
                double PI = System.Math.PI;

                public TopicFrame (Topic iTopic)
                {
                        this.topic = iTopic;
                }

                public Topic Topic
                {
                        get { return topic; }
                }

                public double Width
                {
                        get
                        {
                                frameWidth = this.topic.TextWidth +
                                             this.topic.Style.LeftMargin +
                                             this.topic.Style.RightMargin +
                                             this.topic.Style.StrokeWidth;
                                return frameWidth;
                        }
                }

                public double Height
                {
                        get
                        {
                                frameHeight = this.topic.TextHeight +
                                              this.topic.Style.TopMargin +
                                              this.topic.Style.BottomMargin +
                                              this.topic.Style.StrokeWidth;
                                return frameHeight;
                        }
                }

                double OctDistHor
                {
                        get
                        {
                                octDistHor = new double ();
                                if (this.topic.Style.Shape == TopicShape.Octagon)
                                        octDistHor = this.Height / (2 + sqrt2);
                                else
                                        octDistHor = 0;
                                return octDistHor;
                        }
                }

                double OctDistVer
                {
                        get
                        {
                                octDistVer = new double ();
                                if (this.topic.Style.Shape == TopicShape.Octagon)
                                        octDistVer = (this.Height - OctDistHor) / 2;
                                else
                                        octDist = 0;
                                return octDistVer;
                        }
                }

                double OctDist
                {
                        get
                        {
                                octDist = new double ();
                                octDist = OctDistHor * sqrt2;
                                return octDist;
                        }
                }

                double HexDistHor
                {
                        get
                        {
                                hexDistHor = new double ();
                                if (this.topic.Style.Shape == TopicShape.Hexagon) {
                                        hexDistHor = this.Height / (2 * sqrt3);
                                }
                                else
                                        hexDistHor = 0;
                                return hexDistHor;
                        }
                }

                double HexDist
                {
                        get
                        {
                                hexDist = new double ();
                                hexDist = 2 * HexDistHor;
                                return hexDist;
                        }
                }

                double Radius
                {
                        get
                        {
                                radius = new double ();
                                if (this.topic.Style.Shape == TopicShape.RoundedRectangle) {
                                        radius = 6 /*this.topic.Style.MinMargin*/;
                                }
                                else
                                        radius = 0;
                                return radius;
                        }
                }

                public Cairo.PointD Origin
                {
                        get
                        {
                                origin.X = this.topic.Offset.X -
                                           this.topic.Style.LeftMargin;
                                origin.Y = this.topic.Offset.Y -
                                           this.topic.Style.TopMargin;
                                return origin;
                        }
                }

                public Cairo.PointD Center
                {
                        get
                        {
                                center.X = this.topic.Offset.X +
                                           this.Width / 2;
                                center.Y = this.topic.Offset.Y +
                                           this.Height / 2;
                                return center;
                        }
                }

                public Cairo.PointD Left
                {
                        get
                        {
                                left.X = this.topic.Offset.X -
                                         this.topic.Style.LeftMargin -
                                         this.HexDistHor -
                                         this.OctDistHor -
                                         this.Radius;

                                if (topic.Style.Shape == TopicShape.Line)
                                        left.Y = this.topic.Offset.Y +
                                                  this.Height;
                                else
                                        left.Y = this.topic.Offset.Y +
                                                  this.Height / 2;
                                return left;
                        }
                }

                public Cairo.PointD Right
                {
                        get
                        {
                                right.X = this.topic.Offset.X +
                                          this.topic.TextWidth +
                                          this.topic.Style.RightMargin +
                                          this.HexDistHor +
                                          this.OctDistHor +
                                          this.Radius;

                                if (topic.Style.Shape == TopicShape.Line)
                                        right.Y = this.topic.Offset.Y +
                                                  this.topic.Frame.Height;
                                else
                                        right.Y = this.topic.Offset.Y +
                                                  this.topic.Frame.Height / 2;
                                return right;
                        }
                }

                public Cairo.PointD Top
                {
                        get
                        {
                                top.X = this.topic.Offset.X +
                                         this.topic.TextWidth / 2;
                                top.Y = this.topic.Offset.Y -
                                        this.topic.Style.TopMargin;
                                return top;
                        }
                }

                public Cairo.PointD Bottom
                {
                        get
                        {
                                bottom.X = this.topic.Offset.X +
                                         this.topic.TextWidth / 2;
                                bottom.Y = this.topic.Offset.Y +
                                          this.topic.TextHeight +
                                           this.topic.Style.BottomMargin;
                                return bottom;
                        }
                }

                public void Sketch (Cairo.Context iContext)
                {
                        context = iContext;
                        switch (this.topic.Style.Shape) {
                        case TopicShape.Rectangle:
                        sketchRectangle ();
                        break;
                        case TopicShape.Line:
                        sketchLine ();
                        break;
                        case TopicShape.RoundedRectangle:
                        sketchRoundedRectangle ();
                        break;
                        case TopicShape.Hexagon:
                        sketchHexagon ();
                        break;
                        case TopicShape.Octagon:
                        sketchOctagon ();
                        break;
                        case TopicShape.None:
                        break;
                        default:
                        sketchRectangle ();
                        break;
                        }
                }

                void sketchRectangle ()
                {
                        context.Rectangle (Origin, Width, Height);
                }

                void sketchLine ()
                {
                        context.MoveTo (Origin);
                        context.RelMoveTo (0, this.Height);
                        context.RelLineTo (this.Width, 0);
                }

                void sketchRoundedRectangle ()
                {
                        double angle1 = 0.0 * (PI / 180.0);
                        double angle2 = 90.0 * (PI / 180.0);
                        double angle3 = 180.0 * (PI / 180.0);
                        double angle4 = 270.0 * (PI / 180.0);

                        context.MoveTo (Origin);
                        context.ArcNegative (Origin.X, (Origin.Y + radius), radius, angle4, angle3);
                        context.ArcNegative (Origin.X, (Origin.Y + this.Height - radius), radius, angle3, angle2);
                        context.ArcNegative ((Origin.X + this.Width), (Origin.Y + this.Height - radius), radius, angle2, angle1);
                        context.ArcNegative ((Origin.X + this.Width), (Origin.Y + radius), radius, angle1, angle4);
                        context.ClosePath ();
                }

                void sketchOctagon ()
                {
                        context.MoveTo (Origin);
                        context.RelLineTo (this.Width, 0);
                        context.RelLineTo (this.OctDistHor, this.OctDistVer);
                        context.RelLineTo (0, this.OctDist);
                        context.RelLineTo (-this.OctDistHor, this.OctDistVer);
                        context.RelLineTo (-this.Width, 0);
                        context.RelLineTo (-this.OctDistHor, -this.OctDistVer);
                        context.RelLineTo (0, -this.OctDist);
                        context.ClosePath ();
                }

                void sketchHexagon ()
                {
                        context.MoveTo (Origin);
                        context.RelLineTo (this.Width, 0);
                        context.RelLineTo (HexDistHor, this.Height / 2);
                        context.RelLineTo (-HexDistHor, this.Height / 2);
                        context.RelLineTo (-this.Width, 0);
                        context.RelLineTo (-HexDistHor, -this.Height / 2);
                        context.ClosePath ();
                }
        }
}