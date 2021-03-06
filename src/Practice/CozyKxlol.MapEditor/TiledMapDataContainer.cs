﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CozyKxlol.Engine.Tiled;
using Microsoft.Xna.Framework;
using CozyKxlol.MapEditor.Command;
using CozyKxlol.MapEditor.Event;

namespace CozyKxlol.MapEditor
{
    public class TiledMapDataContainer : ICloneable
    {
        // get set
        // LoadMap
        // SaveMap

        public CozyTiledData TiledData { get; set; }
        public Point MapSize { get; set; }

        private string DataPath = @"Data.db";

        public void Write(int x, int y, uint data)
        {
            TiledData.Modify(x, y, data);
            DataMessage(TiledData, new TiledDataMessageArgs(x, y, data));
        }

        public uint Read(int x, int y)
        {
            return TiledData[x, y];
        }

        public TiledMapDataContainer(int x, int y)
        {
            TiledData   = new CozyTiledData(x, y);
            MapSize     = new Point(x, y);
        }

        public event EventHandler<TiledDataMessageArgs> DataMessage;
        public event EventHandler<TiledClearMessageArgs> ClearMessage;
        public event EventHandler<TiledRefreshMessageArgs> RefreshMessage;

        public void LoadMap()
        {
            var loader = new CozyTiledDataLoader(DataPath);
            loader.Load(TiledData);
            Refresh();
        }

        public void SaveMap()
        {
            var writer = new CozyTiledDataWriter(DataPath);
            writer.Write(TiledData);
        }

        public void Clear()
        {
            TiledData.Clear();
            ClearMessage(TiledData, new TiledClearMessageArgs());
        }

        public object Clone()
        {
            var obj         = (this.MemberwiseClone() as TiledMapDataContainer);
            obj.TiledData   = (TiledData.Clone() as CozyTiledData);
            return obj;
        }

        public void Refresh()
        {
            if(RefreshMessage != null)
            {
                int x = TiledData.DataSize.X;
                int y = TiledData.DataSize.Y;
                uint[,] data = new uint[x, y];

                for (int i = 0; i < x; ++i)
                {
                    for (int j = 0; j < y; ++j)
                    {
                        data[i, j] = TiledData[i, j];
                    }
                }
                RefreshMessage(TiledData, new TiledRefreshMessageArgs(data));
            }
        }
    }
}
