﻿using CozyAnywhere.ClientCore.EventArg;
using System;

namespace CozyAnywhere.ClientCore
{
    public partial class AnywhereClient
    {
        public event EventHandler<PluginChangedEvnetArgs> PluginChangedHandler;

        public event EventHandler<CaptureRefreshEventArgs> CaptureRefreshHandler;

        public event EventHandler<ServerConnectEventArgs> ServerConnectHandler;

        public event EventHandler<CurrentFilePathRefreshEventArgs> CurrentFilePathHandler;
    }
}