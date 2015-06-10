﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozyKxlol.Engine;
using Starbound.Input;
using CozyKxlol.Engine.Tiled;
using CozyKxlol.MapEditor.Command;
using Microsoft.Xna.Framework;
using Starbound.UI.XNA.Renderers;
using Starbound.UI.Controls;
using Starbound.UI.Resources;
using CozyKxlol.MapEditor.Event;

namespace CozyKxlol.MapEditor.OperateLayer
{
    public class MapEditorSceneOperateLayer : CozyLayer
    {
        // accept keyboard shortcuts
        // and accept mouse click
        // then modify TiledMapDataContainer`s data

        private Random random = new Random();

        List<Control> controls;
        StackPanel panel;
        XNARenderer renderer;

        public MouseEvents Mouse { get; set; }
        public KeyboardEvents Keyboard { get; set; }

        public bool IsLeftMouseButtonPress { get; set; }

        public const uint S_Add     = 0000;
        public const uint S_Remove  = 0001;

        public uint Status { get; set; }

        public uint CurrentTiledId { get; set; }
        public Point CurrentPosition { get; set; }

        public Vector2 NodeContentSize { get; set; }

        private Dictionary<UIElement, Action> ClickEvent { get; set; } 

        public MapEditorSceneOperateLayer(Vector2 nodeSize)
        {
            ClickEvent = new Dictionary<UIElement, Action>();
            renderer = new XNARenderer();
            controls = new List<Control>();
            panel = new StackPanel()
            {
                Orientation = Orientation.Veritical,
                ActualWidth = 1280,
                ActualHeight = 800,
                X = 990
            };
            panel.UpdateLayout();
            var button = new SampleButton(10, 220);
            RegisterButtonAction(button, () => { button.Content = "Click1"; });
            panel.AddChild(button);
            var button2 = new SampleButton(10, 350);
            RegisterButtonAction(button2, () => { button2.Content = "Click2"; });
            panel.AddChild(button2);

            Mouse               = new MouseEvents();
            Keyboard            = new KeyboardEvents();

            NodeContentSize     = nodeSize;

            Mouse.ButtonPressed     += new EventHandler<MouseButtonEventArgs>(OnButtonPressed);

            Mouse.ButtonClicked     += new EventHandler<MouseButtonEventArgs>(OnButtonClicked);

            Mouse.ButtonReleased    += new EventHandler<MouseButtonEventArgs>(OnButtonReleased);

            Mouse.MouseMoved        += new EventHandler<MouseEventArgs>(OnMouseMoved);

            Keyboard.KeyPressed     += new EventHandler<KeyboardEventArgs>(OnKeyPressed);

            Keyboard.KeyReleased    += new EventHandler<KeyboardEventArgs>(OnKeyReleased);

            Status          = S_Add;
            CurrentTiledId  = 1;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);

            Mouse.Update(gameTime);
            Keyboard.Update(gameTime);
        }

        protected override void DrawSelf(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            spriteBatch.End();
            foreach (Control control in controls)
            {
                renderer.Render(control, spriteBatch);
            }

            foreach (Control control in panel.Children)
            {
                renderer.Render(control, spriteBatch);
            }
            spriteBatch.Begin();

            if(Status == S_Add)
            {
                CozyTiledFactory.GetInstance(CurrentTiledId).DrawAt(gameTime, spriteBatch, CurrentPosition.ToVector2(), NodeContentSize);
            }
        }

        public event EventHandler<TiledCommandArgs> TiledCommandMessages;

        protected void OnButtonPressed(object sender, MouseButtonEventArgs msg)
        {
            if (msg.Button == MouseButton.Left)
            {
                IsLeftMouseButtonPress = true;
            }
        }

        protected void RegisterButtonAction(UIElement elemt, Action act)
        {
            if(elemt != null && act != null)
            {
                ClickEvent[elemt] = act;
            }
        }

        protected void DispatchClick(Point clickPoint)
        {
            foreach (var obj in panel.Children)
            {
                if (clickPoint.X > obj.X && clickPoint.X < obj.X + obj.ActualWidth && 
                    clickPoint.Y > obj.Y && clickPoint.Y < obj.Y + obj.ActualHeight)
                {
                    var click = ClickEvent[obj];
                    if (click != null)
                    {
                        click();
                    }
                }
            }
        }

        protected void OnButtonClicked(object sender, MouseButtonEventArgs msg)
        {
            if (msg.Button == MouseButton.Left)
            {
                if (Status == S_Add)
                {
                    Point p = CozyTiledPositionHelper.ConvertPositionToTiledPosition(CurrentPosition.ToVector2(), NodeContentSize);
                    AddTiled(p);
                }
                DispatchClick(msg.Current.Position);
            }
            else if (msg.Button == MouseButton.Right)
            {
            }
        }

        protected void OnButtonReleased(object sender, MouseButtonEventArgs msg)
        {
            if (msg.Button == MouseButton.Left)
            {
                IsLeftMouseButtonPress = false;
            }
        }

        protected void OnKeyPressed(object sender, KeyboardEventArgs msg)
        {

        }

        protected void OnKeyReleased(object sender, KeyboardEventArgs msg)
        {

        }

        private void AddTiled(Point p)
        {
            // noity tiled modify to Container
            var command = new ContainerModifyOne(p.X, p.Y, CurrentTiledId);
            TiledCommandMessages(this, new TiledCommandArgs(command));
        }

        protected void OnMouseMoved(object sender, MouseEventArgs msg)
        {
            CurrentPosition = msg.Current.Position;
            if (IsLeftMouseButtonPress && Status == S_Add)
            {
                Point p = CozyTiledPositionHelper.ConvertPositionToTiledPosition(CurrentPosition.ToVector2(), NodeContentSize);
                AddTiled(p);
            }
        }
    }
}
