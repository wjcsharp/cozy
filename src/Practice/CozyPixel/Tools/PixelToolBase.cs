﻿using CozyPixel.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CozyPixel.Draw;
using System.Windows.Forms;

namespace CozyPixel.Tools
{
    public abstract class PixelToolBase
    {
        public IPixelColor ColorHolder { get; set; }

        public abstract bool WillModify { get; }

        public abstract Keys KeyCode { get; }

        protected IPixelGridDrawable Target { get; set; }

        public PixelToolBase(IPixelColor holder)
        {
            ColorHolder = holder;
        }

        public void Begin(IPixelGridDrawable paint, Point p)
        {
            OnEnter(paint);
            OnBegin(p);
        }

        public void Move(Point p)
        {
            OnMove(p);
        }

        public bool End(Point p)
        {
            var retVal = OnEnd(p);
            OnExit();
            return retVal;
        }

        protected virtual void OnEnter(IPixelGridDrawable paint)
        {
            Target = paint;
        }

        protected virtual void OnBegin(Point p)
        {
            // Nothing Todo
        }

        protected virtual void OnMove(Point p)
        {
            // Nothing Todo
        }

        protected virtual bool OnEnd(Point p)
        {
            return false;
        }

        protected virtual void OnExit()
        {
            Target = null;
        }
    }
}
