﻿using CozyPixel.Controls.Other;
using CozyPixel.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CozyPixel
{
    public partial class CozyPixelForm
    {
        private int RefreshCurrGridWidth()
        {
            int gw = 0;
            if (!int.TryParse(GridWidthBox.Text, out gw))
            {
                gw                  = DefaultGridWidth;
                GridWidthBox.Text   = DefaultGridWidth.ToString();
            }
            return gw;
        }

        private void ChangePixelPainterImage(Bitmap bmp)
        {
            int gw = RefreshCurrGridWidth();
            
            CurrPixelMap = new PixelMap()
            {
                ShowGrid    = ShowGridCheckBox.Checked,
                data        = bmp,
                PixelWidth  = DefaultPixelWidth - gw,
                GridWidth   = gw,
                GridColor   = GridColorButton.BackColor,
            };

            PixelPainter.SourceImage    = CurrPixelMap;
        }

        private bool ShowSaveDialog()
        {
            var r = MetroFramework.MetroMessageBox.Show(this, "是否保存", "", MessageBoxButtons.YesNoCancel);
            if (r == DialogResult.Yes)
            {
                if (!SaveFile())
                {
                    return false;
                }
            }
            else if (r == DialogResult.Cancel)
            {
                return false;
            }
            return true;
        }

        private void TestColor()
        {
            ColorList.AddColor(OstwaldColor.GetColor());
            foreach (var c in OstwaldColor.GetColor(50))
            {
                ColorPicker.AddColor("Color", c);
            }
        }

        private void SetCurrPathStatusLabel(string label)
        {
            CurrPathStatusLabel.Text    = "当前文件 ： " + (label ?? string.Empty);
        }


        private void RefreshThumb()
        {
            ThumbListView.ImageClear();

            DirectoryInfo di = new DirectoryInfo(CurrDire);
            var fs = di.GetFiles("*.*", SearchOption.TopDirectoryOnly);

            foreach (var file in fs)
            {
                if (file.Extension == ".bmp" || file.Extension == ".jpg" || file.Extension == ".png")
                {
                    ThumbListView.TryAddImage(file.FullName);
                }
            }
        }
    }
}
