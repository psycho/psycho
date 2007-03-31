// Copyright 2006 by:
//
// Author:
//   Piotr Zurek, p.zurek@gmail.com
//
//   www.psycho-project.org
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
using System.Collections.Generic;
using System.Text;
using Gtk;
using Gdk;
using Cairo;

namespace Psycho
{
        class MindView : ScrolledWindow, IView
        {
                IModel Model;
                IControl Control;

                DrawingArea mapArea;
                Viewport mapViewPort;

                Gdk.GC gc;
                Cairo.Context mapContext;
                Pango.Layout text;

                public MindView ()
                        : base ()
                {
                        mapArea = new DrawingArea ();
                        mapViewPort = new Viewport ();
                        mapArea.ExposeEvent += OnMapExpose;
                        mapArea.Realized += OnMapRealize;
                        mapArea.KeyPressEvent += OnKeyPressEvent;
                        mapArea.ButtonPressEvent += new ButtonPressEventHandler (mapArea_ButtonPressEvent);
                        mapArea.FocusInEvent += OnFocusInEvent;
                        mapArea.AddEvents ((int) EventMask.ButtonPressMask
                                         | (int) EventMask.ButtonReleaseMask
                                         | (int) EventMask.KeyPressMask
                                         | (int) EventMask.PointerMotionMask);
                        mapArea.CanFocus = true;
                        this.ShadowType = ShadowType.EtchedIn;
                        this.HscrollbarPolicy = PolicyType.Always;
                        this.VscrollbarPolicy = PolicyType.Always;
                        this.mapViewPort.Add (mapArea);
                        this.Add (mapViewPort);
                }

                void mapArea_ButtonPressEvent (object sender, ButtonPressEventArgs args)
                {
                        if (args.Event.Type != Gdk.EventType.ButtonPress)
                                return;
                        ClearCurrentTopic ();

                        mapArea.HasFocus = true;
                        Gdk.EventButton pos = args.Event;
                        SetCurrentByCoords ((int) pos.X, (int) pos.Y);
                        args.RetVal = true;
                }

                void Vadjustment_ValueChanged (object sender, EventArgs args)
                {
                        Refresh ();
                }

                void Hadjustment_ValueChanged (object sender, EventArgs args)
                {
                        Refresh ();
                }


                public void WireUp (IControl iControl, IModel iModel)
                {
                        if (Model != null) {
                                Model.RemoveObserver (this);
                        }

                        Model = iModel;
                        Control = iControl;

                        Control.SetModel (Model);
                        Control.SetView (this);
                        Model.AddObserver (this);
                        Update (Model);
                }

                void OnMapRealize (object sender, EventArgs e)
                {
                        gc = new Gdk.GC (this.mapArea.GdkWindow);
                }

                void OnMapExpose (object sender, ExposeEventArgs args)
                {
                        mapContext = Gdk.CairoHelper.Create (args.Event.Window);
                        mapContext.Translate (1000 - Model.CentralTopic.Left, 1000 - Model.CentralTopic.Top);
                        DrawBackground (mapContext);
                        DrawTopics (mapContext);
                        this.mapArea.SetSizeRequest ((int) Model.CentralTopic.GlobalWidth + 2000, (int) Model.CentralTopic.GlobalHeight + 2000);
                        ((IDisposable) mapContext.Target).Dispose ();
                        ((IDisposable) mapContext).Dispose ();
                }

                void OnFocusInEvent (object sender, FocusInEventArgs args)
                {
                        mapArea.GrabFocus ();
                        mapArea.GrabDefault ();
                }

                void OnKeyPressEvent (object sender, KeyPressEventArgs args)
                {
                        string key = args.Event.Key.ToString ();
                        switch (key) {
                                case "Return":
                                AddTopic ();
                                return;
                                case "Insert":
                                AddSubtopic ();
                                return;
                                case "Delete":
                                DeleteTopic ();
                                return;
                                case "Left":            //At the moment it's Right-Child Down-Next but that should
                                SetCurrentUp ();        //be Subtopic layout dependent
                                return;
                                case "Right":
                                SetCurrentDown ();
                                return;
                                case "Up":
                                SetCurrentBack ();
                                args.RetVal = true;
                                return;
                                case "Down":
                                SetCurrentForward ();
                                return;
                                default: break;
                        }
                }

                private void DrawBackground (Context iContext)
                {
                        iContext.Save ();
                        Surface background = new ImageSurface (IconLoader.paperPath);
                        SurfacePattern pattern = new SurfacePattern (background);
                        pattern.Extend = Extend.Repeat;
                        iContext.Pattern = pattern;
                        iContext.Paint ();
                        iContext.Restore ();
                        pattern.Destroy ();
                }

                void DrawTopics (Context iContext)
                {
                        //DrawConnections (iContext, Model.CentralTopic);
                        DrawFrames (iContext, Model.CentralTopic);
                        DrawFrame (iContext, Model.CentralTopic);
                        DrawText (/*iContext,*/ Model.CentralTopic);
                }

                public void DrawConnections (Cairo.Context iContext, Topic iTopic)
                {
                        foreach (Topic TempTopic in iTopic.SubtopicList) {
                                if (TempTopic.IsExpanded)
                                        DrawConnections (iContext, TempTopic);
                                DrawConnection (iContext, TempTopic);
                        }
                }

                public void DrawFrames (Cairo.Context iContext, Topic iTopic)
                {
                        foreach (Topic TempTopic in iTopic.SubtopicList) {
                                DrawRegion (iContext, iTopic);
                                if (TempTopic.IsExpanded) {
                                        DrawFrames (iContext, TempTopic);
                                }
                                DrawFrame (iContext, TempTopic);
                                DrawText (/*iContext, */TempTopic);
                        }
                }

                public void DrawTexts (Cairo.Context iContext, Topic iTopic)
                {
                        foreach (Topic TempTopic in iTopic.SubtopicList) {
                                if (TempTopic.IsExpanded)
                                        DrawTexts (iContext, TempTopic);
                                DrawText (/*iContext, */TempTopic);
                        }
                }

                void DrawText (/*Cairo.Context iContext,*/ Topic iTopic)
                {
                        gc = mapArea.Style.TextAAGC (StateType.Normal);
                        gc.Foreground = new Gdk.Color (0, 0, 0);
                        text = iTopic.TextLayout;
                        mapArea.GdkWindow.DrawLayout (gc,
                                (int) (iTopic.Offset.X - iTopic.TextWidth / 2 - Model.CentralTopic.Left + 1000),
                                (int) (iTopic.Offset.Y - iTopic.TextHeight / 2 - Model.CentralTopic.Top + 1000),
                                text);
                        gc.Dispose ();
                }

                static void DrawConnection (Cairo.Context iContext, Topic iTopic)
                {
                        iContext.Save ();
                        iContext.Color = iTopic.Style.StrokeColor.ToCairoColor ();
                        iContext.LineWidth = iTopic.Style.StrokeWidth;

                        int reminder = (int) System.Math.IEEERemainder (iContext.LineWidth, 2);
                        if (reminder != 0) {
                                iContext.Save ();
                                iContext.Translate (0.5, 0.5);
                        }

                        iTopic.Connection.Sketch (iContext);
                        iContext.Stroke ();

                        if (reminder != 0) {
                                iContext.Restore ();
                        }

                        iContext.Restore ();
                }

                static void DrawFrame (Cairo.Context iContext, Topic iTopic)
                {
                        iContext.Save ();
                        Cairo.Color strokeColor = iTopic.Style.StrokeColor.ToCairoColor ();
                        if (iTopic.IsCurrent)
                                strokeColor = new Cairo.Color (0.75, 0.75, 0.75);
                        Cairo.Color fillColor = strokeColor;
                        iContext.LineWidth = iTopic.Style.StrokeWidth;

                        int reminder = (int) System.Math.IEEERemainder (iContext.LineWidth, 2);
                        if (reminder != 0) {
                                iContext.Translate (0.5, 0.5);
                        }

                        iTopic.Frame.Sketch (iContext);
                        fillColor.A = 0.16;
                        iContext.Color = fillColor;
                        iContext.FillPreserve ();
                        iContext.Color = strokeColor;
                        iContext.Stroke ();
                        iContext.Restore ();
                }

                static void DrawRegion (Cairo.Context iContext, Topic iTopic)
                {
                        iContext.Save ();
                        Cairo.Color strokeColor = iTopic.Style.StrokeColor.ToCairoColor ();
                        strokeColor = new Cairo.Color (0, 0, 0);
                        iContext.LineWidth = 1;
                        iContext.Translate (0.5, 0.5);
                        if (iTopic.IsCentral)
                                iContext.Rectangle (iTopic.Left,
                                                    iTopic.Top,
                                                    iTopic.GlobalWidth,
                                                    iTopic.GlobalHeight);
                        else
                                iContext.Rectangle (System.Math.Floor (iTopic.Offset.BaseX),
                                            System.Math.Floor (iTopic.Offset.BaseY),
                                            System.Math.Floor (iTopic.TotalWidth),
                                            System.Math.Floor (iTopic.TotalHeight));

                        iContext.Stroke ();
                        iContext.Restore ();
                }

                public void Update (IModel iModel)
                {
                        this.QueueDraw ();
                }

                public void Refresh ()
                {
                        this.mapArea.QueueDrawArea (0, 0 /*((int) Vadjustment.Value)*/, mapArea.Allocation.Width, mapArea.Allocation.Height);
                }

                public void AddTopic ()
                {
                        Control.RequestAddTopic ();
                }

                public void AddSubtopic ()
                {
                        Control.RequestAddSubtopic ();
                }

                public void DeleteTopic ()
                {
                        Control.RequestDelete ();
                }

                public void ExpandTopic (string iGuid, bool isExpanded)
                {
                        Control.RequestExpand (this.Model.CurrentTopic.GUID, !this.Model.CurrentTopic.IsExpanded);
                }

                public void SetCurrentByCoords (int iX, int iY)
                {
                        Control.RequestSetCurrentByCoords (iX - 1000 + Model.CentralTopic.Left, iY - 1000 + Model.CentralTopic.Top);
                }

                public void ClearCurrentTopic ()
                {
                        Control.RequestClearCurrent ();
                }

                public void SetCurrentForward ()
                {
                        Control.RequestCurrentForward ();
                }

                public void SetCurrentBack ()
                {
                        Control.RequestCurrentBack ();
                }

                public void SetCurrentUp ()
                {
                        Control.RequestCurrentUp ();
                }

                public void SetCurrentDown ()
                {
                        Control.RequestCurrentDown ();
                }

                public void CommitChange (Topic iTopic)
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void EditTitle (string Title)
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void SetCurrentTopic ()
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void TriggerEdit (bool editPending)
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void DisableAddSibling ()
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void DisableDelete ()
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void EnableAddSibling ()
                {
                        throw new Exception ("The method or operation is not implemented.");
                }

                public void EnableDelete ()
                {
                        throw new Exception ("The method or operation is not implemented.");
                }
        }
}