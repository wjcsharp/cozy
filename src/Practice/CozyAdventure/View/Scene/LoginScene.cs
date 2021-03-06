﻿using CocosSharp;
using CozyAdventure.View.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyAdventure.View.Scene
{
    public class LoginScene : CCScene
    {
        public LoginScene() : base(AppDelegate.SharedWindow)
        {
            var bg = new BackgroundLayer(@"pic\1.png");
            AddChild(bg);

            var ui = new LoginUiLayer();
            AddChild(ui);
        }
    }
}
